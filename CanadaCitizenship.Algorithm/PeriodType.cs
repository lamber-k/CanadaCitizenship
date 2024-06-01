namespace CanadaCitizenship.Algorithm
{
    /// <summary>
    /// Define the kind of period
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// Unknown period, not explained - No Status
        /// </summary>
        None,
        /// <summary>
        /// Valid Permanent Resident of Canada - Valid Status
        /// </summary>
        PR,
        /// <summary>
        /// Having Valid temporary Status - Either Work, Student or implicit status
        /// </summary>
        Temporary,
        /// <summary>
        /// Tourist in Canada - No Status
        /// </summary>
        Tourist,
        /// <summary>
        /// No Status
        /// </summary>
        NoStatus,
        /// <summary>
        /// Outside of Canada for vacation
        /// </summary>
        Vacation,
        /// <summary>
        /// Info not provided - No Status
        /// </summary>
        Other
    }
}