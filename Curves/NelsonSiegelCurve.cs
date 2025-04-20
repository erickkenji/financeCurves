using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curves
{
    public class NelsonSiegelCurve
    {
        private readonly Dictionary<DateTime, double> rates;
        private readonly DateTime referenceDate;
        private double beta0, beta1, beta2, tau;
        private readonly bool SinglePlot;

        public NelsonSiegelCurve(DateTime referenceDate, Dictionary<DateTime, double> rates, bool continuousPlot)
        {
            this.referenceDate = referenceDate;
            this.rates = rates;
            FitCurve();
            this.SinglePlot = continuousPlot;
        }

        private double NelsonSiegelFunction(double maturity, double beta0, double beta1, double beta2, double tau)
        {
            double term1 = (1 - Math.Exp(-maturity / tau)) / (maturity / tau);
            double term2 = term1 - Math.Exp(-maturity / tau);

            return beta0 + beta1 * term1 + beta2 * term2;
        }

        private void FitCurve()
        {
            var maturities = rates.Keys.Select(date => (date - referenceDate).TotalDays / 365.0).ToArray();
            var yields = rates.Values.ToArray();

            this.beta0 = rates.Values.Average();
            this.beta1 = (yields[1] - yields[0]) / Math.Log(maturities[1] / maturities[0]) * 0.1;
            this.beta2 = beta1 / 2;
            this.tau = maturities.Average() * 0.1;

            double[] parameters = { beta0, beta1, beta2, tau };
            NelderMeadOptimization(parameters, maturities, yields);
        }

        private void NelderMeadOptimization(double[] parameters, double[] maturities, double[] yields)
        {
            int maxIterations = 10000;
            double tolerance = 1e-8;
            double step = 0.1;

            double ObjectiveFunction(double[] p) => maturities.Select((t, i) => Math.Pow(yields[i] - NelsonSiegelFunction(t, p[0], p[1], p[2], p[3]), 2)).Sum();

            for (int i = 0; i < maxIterations; i++)
            {
                double[] newParameters = parameters.Select(p => p + (new Random().NextDouble() * 2 - 1) * step).ToArray();
                if (ObjectiveFunction(newParameters) < ObjectiveFunction(parameters))
                {
                    parameters = newParameters;
                    step *= 0.9;
                }
                if (step < tolerance) break;
            }

            beta0 = parameters[0];
            beta1 = parameters[1];
            beta2 = parameters[2];
            tau = parameters[3];
        }

        public double GetRate(DateTime date)
        {
            double maturity = (date - referenceDate).TotalDays / 365.0;
            return NelsonSiegelFunction(maturity, beta0, beta1, beta2, tau);
        }

        /*
        public Dictionary<DateTime, double> GetInterpolatedCurve()
        {
            return rates.Keys.ToDictionary(date => date, date => GetRate(date));
        }
        */

        public StandardCurve GetStandardCurve()
        {
            DateTime startDate = rates.Keys.Min();
            DateTime endDate = rates.Keys.Max();

            List<DateTime> allDates = new List<DateTime>();
            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                allDates.Add(currentDate);
            }

            return new StandardCurve(
                this.referenceDate,
                allDates.ToDictionary(date => date, date => GetRate(date)),
                this.SinglePlot);
        }
    }
}
