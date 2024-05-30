namespace CanadaCitizenship.Algorithm;

public record CitizenshipResult(int TemporaryDays, DateTime StartTemporary, int PRDays, int RemainingDays, DateTime ProjectedDate, params Period[] Periods);
