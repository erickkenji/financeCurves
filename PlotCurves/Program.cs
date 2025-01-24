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
        Calendar calendar = new UnitedStates(UnitedStates.Market.NYSE);

        string historicalDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "HistoricalData");
        string[] historicalDataFiles = Directory.GetFiles(historicalDataPath);

        foreach (string historicalDataFilePath in historicalDataFiles)
        {
            string fullFileName = Path.GetFileName(historicalDataFilePath);
            string fileName = fullFileName.Substring(0, fullFileName.Length - 4);
            if (DateTime.TryParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime referenceDate))
            {
                // Recupera preços dos futuros do historical data (salvo em arquivo)
                Dictionary<DateTime, double> futurePrices = ReadCsv.ReadCsvFile(historicalDataFilePath);

                // Recupera preço spot
                double spotPrice = MarketDataUtils.GetUSDBTCSpotPrice(referenceDate);

                // Calcula taxa implicita
                FutureCurve futureCurve = new FutureCurve(futurePrices, referenceDate, spotPrice, calendar);
                Dictionary<DateTime, double> rates = futureCurve.Rates;

                // Realiza interpolação
                CubicSplineInterpolation interpolation = new CubicSplineInterpolation(rates);
                Dictionary<DateTime, double> interpolatedCurve = interpolation.InterpolateAll();

                // Plota o gráfico
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