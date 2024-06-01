using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanadaCitizenship.Algorithm
{
    public class PeriodTypePriorityComparer : IComparer<PeriodType>
    {
        public static PeriodTypePriorityComparer Default { get; } = new PeriodTypePriorityComparer();
        public int Compare(PeriodType x, PeriodType y)
        {
            if (x == y) return 0;
            if (x == PeriodType.PR)
            {
                return 2;
            }
            else if (y == PeriodType.PR)
            {
                return -2;
            }
            if (x == PeriodType.Temporary)
            {
                return 1;
            }
            else if (y == PeriodType.Temporary)
            {
                return -1;
            }
            return 0;
        }
    }
}
