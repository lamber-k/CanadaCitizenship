﻿using System.Runtime.Serialization;

namespace CanadaCitizenship.Algorithm
{
    /// <summary>
    /// Define a period
    /// </summary>
    public class Period
    {
        public Period() { }
        public Period(DateTime begin, DateTime end, PeriodType type, string? name = null)
        {
            Begin = begin; 
            End = end; 
            Type = type;
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// Name of the Period - Optional
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Type of Period
        /// </summary>
        public PeriodType Type { get; set; } = PeriodType.None;
        /// <summary>
        /// Begin of the period
        /// </summary>
        public DateTime Begin { get; set; }
        /// <summary>
        /// End of the period
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// Number of elapsed days between begin and end of this period
        /// </summary>
        [IgnoreDataMember]
        public int Days => (End - Begin).Days + 1;

        /// <summary>
        /// Indicate if the date provided is enclosed in this period
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool DateEnclosed(DateTime date)
        {
            if (Type == PeriodType.PR || Type == PeriodType.Temporary)
            {
                return date >= Begin && date <= End;
            }
            else
            {
                return date >= Begin && date < End;
            }
        }
    }
}