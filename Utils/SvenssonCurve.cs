using System;
using System.Collections.Generic;
using System.Linq;

public class SvenssonCurve
{
    private Dictionary<DateTime, double> rates;
    private double beta0, beta1, beta2, beta3, tau1, tau2;

    public SvenssonCurve(Dictionary<DateTime, double> rates)
    {
        this.rates = rates;
        FitCurve();
    }

    private double SvenssonFunction(double maturity, double beta0, double beta1, double beta2, double beta3, double tau1, double tau2)
    {
        double term1 = (1 - Math.Exp(-maturity / tau1)) / (maturity / tau1);
        double term2 = term1 - Math.Exp(-maturity / tau1);
        double term3 = (1 - Math.Exp(-maturity / tau2)) / (maturity / tau2) - Math.Exp(-maturity / tau2);

        return beta0 + beta1 * term1 + beta2 * term2 + beta3 * term3;
    }

    private void FitCurve()
    {
        var maturities = rates.Keys.Select(date => (date - DateTime.Today).TotalDays / 365.0).ToArray();
        var yields = rates.Values.ToArray();

        double beta0 = rates.Values.Average();
        double beta1 = (yields[1] - yields[0]) / Math.Log(maturities[1] / maturities[0]);
        double beta2 = beta1 / 2;
        double beta3 = 0; // Pode começar neutro para evitar distorção
        double tau1 = maturities.Average() / 2;
        double tau2 = maturities.Max();

        double[] parameters = { beta0, beta1, beta2, beta3, tau1, tau2 };
        //double[] parameters = { 0.03, -0.02, 0.02, -0.01, 2.0, 5.0 };
        NelderMeadOptimization(parameters, maturities, yields);
    }

    private void NelderMeadOptimization(double[] parameters, double[] maturities, double[] yields)
    {
        int maxIterations = 10000;
        double tolerance = 1e-8;
        double step = 0.1;

        double ObjectiveFunction(double[] p) => maturities.Select((t, i) => Math.Pow(yields[i] - SvenssonFunction(t, p[0], p[1], p[2], p[3], p[4], p[5]), 2)).Sum();

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
        beta3 = parameters[3];
        tau1 = parameters[4];
        tau2 = parameters[5];
    }

    public double GetRate(DateTime date)
    {
        double maturity = (date - DateTime.Today).TotalDays / 365.0;
        return SvenssonFunction(maturity, beta0, beta1, beta2, beta3, tau1, tau2);
    }

    public Dictionary<DateTime, double> GetInterpolatedCurve()
    {
        return rates.Keys.ToDictionary(date => date, date => GetRate(date));
    }
}
