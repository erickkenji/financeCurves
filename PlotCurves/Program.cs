using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        new DateTime(2025, 01, 06)
    };

    static void Main(string[] args)
    {
        // CME is an american exchange, using NYC calendar
        Calendar calendar = new UnitedStates(UnitedStates.Market.NYSE);

        string historicalDataPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "HistoricalData");
        string[] historicalDataFiles = Directory.GetFiles(historicalDataPath);

        foreach (string historicalDataFilePath in historicalDataFiles)
        {
            string fileName = Path.GetFileName(historicalDataFilePath);
            if (DateTime.TryParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime referenceDate))
            {
                // Recover vertexes from file
                List<FuturePrice> futurePrices = ReadCsv.ReadCsvFile(historicalDataFilePath);

                // Recover implicit rates
                List<RateVertex> 
            }
            else
            {
                Console.WriteLine($"FileName {fileName} has incorrect format.");
            }
        }
    }
}