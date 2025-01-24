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

class Program
{
    static void Main(string[] args)
    {
        // CME is an american exchange, using NYC calendar
        Calendar calendar = new UnitedStates(UnitedStates.Market.NYSE);

        string historicalDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "HistoricalData");
        string[] historicalDataFiles = Directory.GetFiles(historicalDataPath);

        foreach (string historicalDataFilePath in historicalDataFiles)
        {
            string fullFileName = Path.GetFileName(historicalDataFilePath);
            string fileName = fullFileName.Substring(0, fullFileName.Length - 4);
            if (DateTime.TryParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime referenceDate))
            {
                // Recover vertexes from file
                Dictionary<DateTime, double> futurePrices = ReadCsv.ReadCsvFile(historicalDataFilePath);

                // Recover spot rate
                double spotPrice = MarketDataUtils.GetUSDBTCSpotPrice(referenceDate);

                // Recover implicit rates
                FutureCurve futureCurve = new FutureCurve(futurePrices, referenceDate, spotPrice, calendar);
                Dictionary<DateTime, double> rates = futureCurve.Rates;

                // Interpolation
                CubicSplineInterpolation interpolation = new CubicSplineInterpolation(rates);
                Dictionary<DateTime, double> interpolatedCurve = interpolation.InterpolateAll();

                // Plot Graph
                PlotGraph plot = new PlotGraph(interpolatedCurve, referenceDate);
                plot.PlotCurve();
            }
            else
            {
                Console.WriteLine($"FileName {fileName} has incorrect format.");
            }
        }
    }
}