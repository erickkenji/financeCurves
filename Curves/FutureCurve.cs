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
using Utils;

namespace Curves
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
        public double SpotPrice { get; private set; }
        public bool SinglePlot { get; private set; }

        public FutureCurve(Dictionary<DateTime, double> factors, DateTime referenceDate, double spotPrice, Calendar calendar, bool continuousPlot)
        {
            this.Factors = factors;
            this.ReferenceDate = referenceDate;
            this.SpotPrice = spotPrice;
            this.Calendar = calendar;
            this.SinglePlot = continuousPlot;

            // Calcula taxa implicita do fator
            Dictionary<DateTime, double> rates = new Dictionary<DateTime, double>() { };
            foreach (KeyValuePair<DateTime, double> kvp in this.Factors)
            {
                double priceFactor = (this.SpotPrice - kvp.Value) / kvp.Value;
                double periods = 365 / this.Calendar.businessDaysBetween(QuantLibUtils_.GetQuantLibDateFromDateTime(this.ReferenceDate), QuantLibUtils_.GetQuantLibDateFromDateTime(kvp.Key));
                double implicitRate = priceFactor * periods;

                rates.Add(kvp.Key, implicitRate);
            }
            this.Rates = rates;
        }

        public StandardCurve GetStandardCurve()
        {
            return new StandardCurve(this.ReferenceDate, this.Rates, this.SinglePlot);
        }
    }

    public static class CurveUtils
    {
        public static List<Vertex> RetrieveRatesFromFactor(List<Vertex> futurePrices, Vertex spotPrice, Calendar calendar)
        {
            List<Vertex> ratesFromFactor = new List<Vertex>() { };

            foreach (Vertex futurePrice in futurePrices)
            {
                double priceFactor = (spotPrice.Value - futurePrice.Value) / futurePrice.Value;
                double periods = 365 / calendar.businessDaysBetween(QuantLibUtils_.GetQuantLibDateFromDateTime(spotPrice.Date), QuantLibUtils_.GetQuantLibDateFromDateTime(futurePrice.Date));
                double implicitRate = priceFactor * periods;

                ratesFromFactor.Add(new Vertex{Date = futurePrice.Date, Value = implicitRate});
            }

            return ratesFromFactor;
        }
    }
}
