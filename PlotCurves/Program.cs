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
    private static HashSet<DateTime> referenceDateCollection = new HashSet<DateTime>()
    {
        new DateTime(2025, 01, 10),
        new DateTime(2025, 01, 09),
        new DateTime(2025, 01, 08),
        new DateTime(2025, 01, 07),
        new DateTime(2025, 01, 06),
        new DateTime(2025, 01, 03),
        new DateTime(2025, 01, 02),
        new DateTime(2024, 12, 31),
        new DateTime(2024, 12, 30)
    };

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
            }
            else
            {
                Console.WriteLine($"FileName {fileName} has incorrect format.");
            }
        }
    }
}