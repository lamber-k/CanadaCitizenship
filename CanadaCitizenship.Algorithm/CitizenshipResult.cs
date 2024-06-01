namespace CanadaCitizenship.Algorithm;

/// <summary>
/// Citizenship computation result
/// </summary>
/// <param name="TemporaryDays">Number of days considered with valid temporary permit - either work,student,implicit,...</param>
/// <param name="StartTemporary">Start of temporary date used</param>
/// <param name="PRDays">Number of days as a permanent resident of Canada</param>
/// <param name="RemainingDays">Remaining days required before starting Citizenship process</param>
/// <param name="ProjectedDate">Estimated date when to start Citizenship process</param>
/// <param name="Periods">List of all periods between <see cref="StartTemporary"/> and <see cref="ProjectedDate"/></param>
public record CitizenshipResult(int TemporaryDays, DateTime StartTemporary, int PRDays, int RemainingDays, DateTime ProjectedDate, params Period[] Periods);
