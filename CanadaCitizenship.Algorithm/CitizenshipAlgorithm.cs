namespace CanadaCitizenship.Algorithm
{
    public class CitizenshipAlgorithm
    {
        const int REQUIRED_DAYS = 1095;
        const int MAX_TEMP = 365;
        const int MAX_YEARS = 5;

        public static CitizenshipResult Compute(Profile profile)
        {
            if (!profile.TemporaryDate.HasValue)
            {
                throw new InvalidOperationException("TEMP_NO_VALUE");
            }
            if (!profile.PRDate.HasValue)
            {
                throw new InvalidOperationException("PR_NO_VALUE");
            }
            DateTime begin = profile.TemporaryDate!.Value;
            DateTime prBeginDate = profile.PRDate!.Value.Date;

            double temporaryDays = 0;
            int prDays = 0;

            DateTime temporaryDate = prBeginDate;
            while (temporaryDate > begin && temporaryDays < MAX_TEMP)
            {
                if (!profile.OutOfCountry.Any(ooc => temporaryDate > ooc.Begin && temporaryDate < ooc.End))
                {
                    temporaryDays += 0.5;
                }
                temporaryDate = temporaryDate.AddDays(-1);
            }
            DateTime prDate = prBeginDate;
            while (prDate < DateTime.Today && (prDays + Math.Floor(temporaryDays)) < REQUIRED_DAYS)
            {
                if (!profile.OutOfCountry.Any(ooc => prDate > ooc.Begin && prDate < ooc.End))
                {
                    prDays++;
                }
                prDate = prDate.AddDays(1);
            }
            double remainingDays = REQUIRED_DAYS - (prDays + Math.Floor(temporaryDays));

            DateTime projectedDate = DateTime.Today;
            for (double remaining = remainingDays; remaining > 0;)
            {
                if (!profile.OutOfCountry.Any(ooc => projectedDate > ooc.Begin && projectedDate < ooc.End))
                {
                    remaining -= 1;
                }

                // Temporary date taken is not on the 5 years sliding days. Remove one
                if (projectedDate.AddYears(-MAX_YEARS) > temporaryDate)
                {
                    // Only remove a temporary day if it was counted
                    if (!profile.OutOfCountry.Any(ooc => temporaryDate > ooc.Begin && temporaryDate < ooc.End))
                    {
                        temporaryDays -= 0.5;
                        remaining += 0.5;
                    }
                    temporaryDate = temporaryDate.AddDays(1);
                }
                projectedDate = projectedDate.AddDays(1);
            }
            List<Period> periods = [];
            for (DateTime current = temporaryDate; current < projectedDate;)
            {
                Period? closestNext = profile.OutOfCountry.Where(ooc => ooc.Begin != ooc.End && ooc.Begin >= current).OrderBy(ooc => ooc.Begin).FirstOrDefault();
                if (closestNext is null)
                {
                    // Reached the end
                    periods.Add(new Period
                    {
                        Name = current > prBeginDate ? "PR" : "Temporary",
                        Begin = current,
                        End = projectedDate
                    });
                    current = projectedDate;
                }
                else if (closestNext.Begin < prBeginDate)
                {
                    // Before PR
                    periods.Add(new Period
                    {
                        Name = "Temporary",
                        Begin = current,
                        End = closestNext.Begin
                    });
                    current = closestNext.End;
                }
                else
                {
                    // Need to split if period both Temporary and PR
                    if (current < prBeginDate)
                    {
                        periods.Add(new Period
                        {
                            Name = "Temporary",
                            Begin = current,
                            End = prBeginDate.AddDays(-1)
                        });
                        current = prBeginDate;
                    }
                    else
                    {
                        periods.Add(new Period
                        {
                            Name = "PR",
                            Begin = current,
                            End = closestNext.Begin
                        });
                        current = closestNext.End;
                    }
                }
            }
            return new CitizenshipResult((int)Math.Floor(temporaryDays), temporaryDate, prDays, (int)Math.Floor(remainingDays), projectedDate, periods.ToArray());
        }
    }
}
