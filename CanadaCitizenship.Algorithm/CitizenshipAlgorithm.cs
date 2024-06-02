namespace CanadaCitizenship.Algorithm
{
    /// <summary>
    /// Algorithm that computes the Citizenship apply estimated date
    /// See https://eservices.cic.gc.ca/rescalc/resCalcStartNew.do for details
    /// </summary>
    public class CitizenshipAlgorithm
    {
        /// <summary>
        /// Number of days before starting Citizenship process
        /// </summary>
        const int REQUIRED_DAYS = 1095;
        /// <summary>
        /// Maximum number of days that can be counted as temporary
        /// Each day with temporary status is 0.5d
        /// </summary>
        const int MAX_TEMP = 365;
        /// <summary>
        /// Maximum sliding timespan to consider
        /// </summary>
        const int MAX_YEARS = 5;

        /// <summary>
        /// Compute citizenship results
        /// </summary>
        /// <param name="profile">Profile data to consider</param>
        /// <returns>Result of the computation</returns>
        /// <exception cref="InvalidOperationException">PR date has not been provided</exception>
        public static CitizenshipResult Compute(Profile profile, DateTime? now = null)
        {
            if (!profile.PRDate.HasValue)
            {
                throw new InvalidOperationException("PR_NO_VALUE");
            }
            DateTime today = now?.Date ?? DateTime.Today;
            DateTime begin = profile.TemporaryDate ?? profile.PRDate!.Value.Date;
            DateTime prBeginDate = profile.PRDate!.Value.Date;

            List<Period> periods = CreatePeriods(profile.ExclusionPeriods, begin, prBeginDate, today);

            // Split period having Today
            if (!periods.Any(p => p.Begin == today || p.End == today))
            {
                Period enclosedToday = periods.First(p => p.DateEnclosed(today));
                periods.Add(new Period(enclosedToday.Begin, today, enclosedToday.Type));
                enclosedToday.Begin = today;
            }

            // Adjust
            ComputeTemporaryPeriod(periods, prBeginDate, out double temporaryDays, out DateTime temporaryDate);

            int prDays = periods.Where(p => p.Type == PeriodType.PR && p.End <= today).Sum(p => p.Days);
            double remainingDays = Math.Max(0, REQUIRED_DAYS - (prDays + Math.Floor(temporaryDays)));

            // Compute projected date
            DateTime projectedDate = ComputeProjectedDate(periods, remainingDays, today, ref temporaryDays, ref temporaryDate);

            // Remove any period less than max sliding period 
            return new CitizenshipResult((int)temporaryDays, temporaryDate, prDays, (int)remainingDays, projectedDate, periods.OrderBy(p => p.Begin).ToArray());
        }

        /// <summary>
        /// Compute the projected date, and adjust sliding <see cref="MAX_YEARS"/> to be considered
        /// </summary>
        /// <param name="periods">List of all periods including exclusion and valid status</param>
        /// <param name="remainingDays">Remaining days before citizenship application</param>
        /// <param name="temporaryDays">Number of days considered for citizenship application</param>
        /// <param name="temporaryDate">Start of the temporary status date considered</param>
        /// <returns>The computed projected date</returns>
        private static DateTime ComputeProjectedDate(List<Period> periods, double remainingDays, DateTime today, ref double temporaryDays, ref DateTime temporaryDate)
        {
            DateTime currentDate = today.AddDays(1);
            DateTime projectedDate = today;
            for (double remaining = remainingDays; remaining > 0;)
            {
                Period? currentPeriod = periods.FirstOrDefault(p => p.DateEnclosed(currentDate));
                if (currentPeriod is null)
                {
                    // No more period registered, create new one with remaining number of days
                    var lastPeriod = periods.OrderByDescending(p => p.Begin).FirstOrDefault();
                    if (lastPeriod?.Type == PeriodType.PR)
                    {
                        lastPeriod.End = lastPeriod.End.AddDays((int)remaining);
                    }
                    else
                    {
                        periods.Add(new Period(currentDate, currentDate.AddDays((int)remaining), PeriodType.PR));
                    }
                    projectedDate = currentDate.AddDays((int)remaining);
                    remaining = 0;
                }
                else
                {
                    if (currentPeriod.Type == PeriodType.PR)
                    {
                        int remainingToRemove = Math.Min((int)remaining, currentPeriod.Days);
                        // Period already created, move remaining if PR period
                        remaining -= remainingToRemove;
                        if (remaining == 0)
                        {
                            // Reached the end
                            projectedDate = currentDate.AddDays(remainingToRemove);
                        }
                    }
                    currentDate = currentPeriod.End.AddDays(1); // Move to next period
                }
                // Temporary date taken is not on the 5 years sliding days. Remove or strip oldest period
                while (projectedDate.AddYears(-MAX_YEARS) > temporaryDate)
                {
                    DateTime localTemporaryDate = temporaryDate;
                    Period oldestTemporary = periods.First(p => p.Type == PeriodType.Temporary && p.DateEnclosed(localTemporaryDate));

                    if (oldestTemporary.End < projectedDate.AddYears(-MAX_YEARS))
                    {
                        // |-b-t-p-e---PR---|
                        TimeSpan daysToRemove = projectedDate.AddYears(-MAX_YEARS) - temporaryDate;

                        temporaryDays -= daysToRemove.Days * 0.5;
                        remaining += daysToRemove.Days * 0.5;
                        temporaryDate = temporaryDate.Add(daysToRemove);
                    }
                    else
                    {
                        // |-b-----e-p-PR---|
                        temporaryDays -= oldestTemporary.Days * 0.5;
                        remaining += oldestTemporary.Days * 0.5;
                        temporaryDate = temporaryDate.AddDays(oldestTemporary.Days);
                    }
                }
            }
            return projectedDate;
        }

        /// <summary>
        /// Compute how many days are considered for citizenship and returning the temporary date
        /// </summary>
        /// <param name="periods">List of all periods including exclusion and valid status</param>
        /// <param name="prBeginDate">Start of PR status</param>
        /// <param name="temporaryDays">Number of days considered for citizenship application</param>
        /// <param name="temporaryDate">Start of the temporary status date considered</param>
        private static void ComputeTemporaryPeriod(IReadOnlyCollection<Period> periods, DateTime prBeginDate, out double temporaryDays, out DateTime temporaryDate)
        {
            temporaryDays = 0;
            temporaryDate = prBeginDate;
            Stack<Period> temporaryPeriods = new(periods.Where(p => p.Type == PeriodType.Temporary).OrderBy(p => p.Begin));
            while (temporaryPeriods.Count != 0)
            {
                if (!temporaryPeriods.TryPop(out Period? previousPeriod))
                {
                    break;
                }
                if (temporaryDays + previousPeriod.Days * 0.5 > MAX_TEMP)
                {
                    // Reached the maximum, only take what can be considered
                    int daysUsed = (int)((MAX_TEMP - temporaryDays) * 2);
                    temporaryDate = previousPeriod.End.AddDays(-daysUsed);
                    temporaryDays = MAX_TEMP;
                    break;
                }
                else
                {
                    // Whole period can be considered
                    temporaryDays += previousPeriod.Days * 0.5;
                    temporaryDate = previousPeriod.Begin;
                }
            }
        }

        /// <summary>
        /// Create all Temporary and PR periods based on exclusion periods and begin temporary / PR begin date
        /// up to today, or the last exclusion date specified
        /// </summary>
        /// <param name="exclusionPeriods">List of exclusion periods</param>
        /// <param name="begin">Begin temporary status</param>
        /// <param name="prBeginDate">Begin PR date received</param>
        /// <returns>All the periods including exlusion and valid status</returns>
        private static List<Period> CreatePeriods(IReadOnlyCollection<Period> exclusionPeriods, DateTime begin, DateTime prBeginDate, DateTime today)
        {
            List<Period> periods = [];
            Queue<Period> exclusions = new Queue<Period>(exclusionPeriods.OrderBy(excluded => excluded.Begin));
            DateTime maxDate = exclusionPeriods.Select(p => p.End).OrderByDescending(d => d).FirstOrDefault(today);
            maxDate = maxDate > today ? maxDate : today;
            DateTime current = begin;
            while (true)
            {
                if (!exclusions.TryDequeue(out Period? closestNext))
                {
                    // Reached the end
                    //|-----PR---^---|
                    if (current < prBeginDate)
                    {
                        // Have not reached temporary yet
                        //|---t--PR---^---|
                        periods.Add(new Period(current, prBeginDate.AddDays(-1), PeriodType.Temporary));
                        current = prBeginDate;
                    }

                    periods.Add(new Period(current, maxDate, PeriodType.PR));
                    current = maxDate;
                    break;
                }
                else if (closestNext.Begin < prBeginDate)
                {
                    // Before PR
                    //|-^--b--e-PR----|
                    //|-^---b---PR-e--|
                    if (periods.LastOrDefault()?.End != closestNext.Begin)
                    {
                        periods.Add(new Period(current, closestNext.Begin, PeriodType.Temporary));
                    }
                    periods.Add(closestNext);
                    current = closestNext.End;
                }
                else
                {
                    // Need to split if period both Temporary and PR
                    if (current < prBeginDate)
                    {
                        //|-----^---PR-b----e--|
                        periods.Add(new Period(current, prBeginDate.AddDays(-1), PeriodType.Temporary));
                        current = prBeginDate;
                    }
                    //|----PR-^--b----e--|
                    periods.AddRange(
                    [
                        closestNext,
                            new Period(current, closestNext.Begin, PeriodType.PR)
                    ]);
                    current = closestNext.End;
                }
            }
            return periods;
        }
    }
}
