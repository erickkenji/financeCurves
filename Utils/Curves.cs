using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class RateVertex
    {
        public DateTime Date { get; set; }
        public double Rate { get; set; }
    }

    public static class CurveUtils
    {
        // Calculates implicit rate from factor
        static List<RateVertex> RetrieveRatesFromFactor(List<FuturePrice> futurePrices, FuturePrice spotPrice)
        {
            List<RateVertex> ratesFromFactor = new List<RateVertex>() { };

            foreach (FuturePrice futurePrice in futurePrices)
            {
                double priceFactor = (spotPrice.Price - futurePrice.Price) / futurePrice.Price;
                double periods = 365 /;
                double implicitRate = priceFactor * periods;

                ratesFromFactor.Add(new RateVertex({Date = futurePrice.Month, Rate = implicitRate});
            }

            return ratesFromFactor;
        }
    }
}
