using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using QuantLib;
using Calendar = QuantLib.Calendar;
using Path = System.IO.Path;
using Utils;
using Curves;
using Metrics;

class Program
{
    private static Dictionary<DateTime, double> referenceDateFixingCollection = new Dictionary<DateTime, double>()
    {
        { new DateTime(2025, 01, 23), 103960.17},
        { new DateTime(2025, 01, 22), 103653.07},
        { new DateTime(2025, 01, 21), 106146.27},
        { new DateTime(2025, 01, 17), 104463.04},
        { new DateTime(2025, 01, 16), 99756.91},
        { new DateTime(2025, 01, 15), 100504.49},
        { new DateTime(2025, 01, 14), 96534.05},
        { new DateTime(2025, 01, 13), 94516.52},
        { new DateTime(2025, 01, 10), 94701.45},
        { new DateTime(2025, 01, 09), 92484.04},
        { new DateTime(2025, 01, 08), 95043.52},
        { new DateTime(2025, 01, 07), 96922.70},
        { new DateTime(2025, 01, 06), 102078.09},
        { new DateTime(2025, 01, 03), 98107.43},
        { new DateTime(2025, 01, 02), 96886.88},
        { new DateTime(2024, 12, 31), 93429.20},
        { new DateTime(2024, 12, 30), 92643.21}
    };

    static void Main(string[] args)
    {
        Calendar calendar = new UnitedStates(UnitedStates.Market.NYSE);
        string historicalDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "HistoricalData");
        string[] historicalDataFiles = Directory.GetFiles(historicalDataPath);

        Dictionary<DateTime, double> cubicSplineMseCollection = new Dictionary<DateTime, double>();
        Dictionary<DateTime, double> nelsonSiegelMseCollection = new Dictionary<DateTime, double>();

        Dictionary<DateTime, double> cubicSplineStandardDeviationDerivativeCollection = new Dictionary<DateTime, double>();
        Dictionary<DateTime, double> nelsonSiegelStandardDeviationDerivativeCollection = new Dictionary<DateTime, double>();

        foreach (string historicalDataFilePath in historicalDataFiles)
        {
            string fullFileName = Path.GetFileName(historicalDataFilePath);
            string fileName = fullFileName.Substring(0, fullFileName.Length - 4);
            DateTime referenceDate = DateTime.ParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            // Recupera preços dos futuros do historical data (salvo em arquivo)
            Dictionary<DateTime, double> futurePrices = ReadCsv.ReadCsvFile(historicalDataFilePath);

            // Recupera preço spot
            //double spotPrice = MarketDataUtils.GetUSDBTCSpotPrice(referenceDate);
            double spotPrice = referenceDateFixingCollection.TryGetValue(referenceDate, out spotPrice) ? spotPrice : 0.0;

            // Calcula taxa implicita
            FutureCurve futureCurve = new FutureCurve(
                factors: futurePrices,
                referenceDate: referenceDate,
                spotPrice: spotPrice,
                calendar: calendar,
                continuousPlot: false);

            // Cubic Splines
            CubicSplineCurve cubicSplineCurve = new CubicSplineCurve(
                referenceDate: referenceDate.AddHours(1),
                rates: futureCurve.Rates,
                continuousPlot: true
            );

            // Nelson Siegel
            NelsonSiegelCurve nelsonSiegelCurve = new NelsonSiegelCurve(
                referenceDate: referenceDate.AddHours(2),
                rates: futureCurve.Rates,
                continuousPlot: true);

            // Calcula erro quadrático médio para cada método
            double cubicSplineMse = MetricsMethods.CalculateMeanSquareError(
                futureCurve.GetStandardCurve(),
                cubicSplineCurve.GetStandardCurve());
            double nelsonSiegelMse = MetricsMethods.CalculateMeanSquareError(
                futureCurve.GetStandardCurve(),
                nelsonSiegelCurve.GetStandardCurve());

            // Adiciona MSE
            cubicSplineMseCollection.Add(referenceDate, cubicSplineMse);
            nelsonSiegelMseCollection.Add(referenceDate, nelsonSiegelMse);

            // Adiciona desvio padrão das primeiras derivadas
            cubicSplineStandardDeviationDerivativeCollection.Add(
                referenceDate,
                MetricsMethods.CalcularPenalidadeDeSuavidade(cubicSplineCurve.GetStandardCurve()));
            nelsonSiegelStandardDeviationDerivativeCollection.Add(
                referenceDate,
                MetricsMethods.CalcularPenalidadeDeSuavidade(nelsonSiegelCurve.GetStandardCurve()));
        }

        // Calcula desvio padrão
        double cubicSplineStandardDeviation = MetricsMethods.CalculateStandardDeviation(cubicSplineMseCollection.Values.ToArray());
        double nelsonSiegelStandardDeviation = MetricsMethods.CalculateStandardDeviation(nelsonSiegelMseCollection.Values.ToArray());
    }
}