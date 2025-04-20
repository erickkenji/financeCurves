using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curves
{
    public class StandardCurve
    {
        public readonly DateTime referenceDate;
        public readonly Dictionary<DateTime, double> buckets;
        public readonly bool continuousPlot;

        public StandardCurve(DateTime referenceDate, Dictionary<DateTime, double> buckets, bool singlePlot)
        {
            this.referenceDate = referenceDate;
            this.buckets = buckets;
            this.continuousPlot = singlePlot;   
        }
    }
}
