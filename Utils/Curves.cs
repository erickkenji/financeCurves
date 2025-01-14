using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using QuantLib;
using Calendar = QuantLib.Calendar;
using DateTime = System.DateTime;

namespace Utils
{
    public class Vertex
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }

    public class FutureCurve
    {
        public Dictionary<DateTime, double> Rates { get; private set; }
        public Dictionary<DateTime, double> Factors { get; private set; }
        public DateTime ReferenceDate { get; private set; }
        public Calendar Calendar { get; private set; }

        public FutureCurve(Dictionary<DateTime, double> factors, DateTime referenceDate, double spotPrice, Calendar calendar)
        {
            this.Factors = factors;
            this.ReferenceDate = referenceDate;

            // Calculates implicit rate from factor
            foreach (KeyValuePair<DateTime, double> kvp in factors)
            {
                double priceFactor = (spotPrice - kvp.Value) / kvp.Value;
                double periods = 365 / calendar.businessDaysBetween(QuantLibUtils_.GetQuantLibDateFromDateTime(referenceDate), QuantLibUtils_.GetQuantLibDateFromDateTime(kvp.Key));
                double implicitRate = priceFactor * periods;

                this.Rates.Add(kvp.Key, implicitRate);
            }
        }
    }

    public static class CurveUtils
    {
        // Calculates implicit rate from factor
        public static List<Vertex> RetrieveRatesFromFactor(List<Vertex> futurePrices, Vertex spotPrice, Calendar calendar)
        {
            List<Vertex> ratesFromFactor = new List<Vertex>() { };

            foreach (Vertex futurePrice in futurePrices)
            {
                double priceFactor = (spotPrice.Value - futurePrice.Value) / futurePrice.Value;
                double periods = 365 / calendar.businessDaysBetween(QuantLibUtils_.GetQuantLibDateFromDateTime(spotPrice.Date), QuantLibUtils_.GetQuantLibDateFromDateTime(futurePrice.Date));
                double implicitRate = priceFactor * periods;

                ratesFromFactor.Add(new Vertex({Date = futurePrice.Date, Value = implicitRate});
            }

            return ratesFromFactor;
        }
    }
}
