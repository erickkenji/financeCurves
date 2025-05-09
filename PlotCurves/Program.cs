﻿using System;
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
        StandardCurve bootstrappedRates;

        HashSet<StandardCurve> allRates = new HashSet<StandardCurve>();
        foreach (string historicalDataFilePath in historicalDataFiles)
        {
            string fullFileName = Path.GetFileName(historicalDataFilePath);
            string fileName = fullFileName.Substring(0, fullFileName.Length - 4);
            if (DateTime.TryParseExact(fileName, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime referenceDate))
            {
                if (referenceDate == new DateTime(2025, 01, 17))
                {
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
                        calendar:calendar,
                        continuousPlot: false);

                    // Cubic Splines
                    CubicSplineCurve cubicSplineCurve = new CubicSplineCurve(
                        referenceDate:referenceDate.AddHours(1),
                        rates: futureCurve.Rates,
                        continuousPlot: true
                    );

                    // Nelson Siegel
                    NelsonSiegelCurve nelsonSiegelCurve = new NelsonSiegelCurve(
                        referenceDate: referenceDate.AddHours(2), 
                        rates: futureCurve.Rates,
                        continuousPlot: true);
                    
                    allRates.Add(cubicSplineCurve.GetStandardCurve());
                    allRates.Add(nelsonSiegelCurve.GetStandardCurve());
                    allRates.Add(futureCurve.GetStandardCurve());

                    // Plota o gráfico individual
                    //PlotGraph plot = new PlotGraph(curve);
                    //plot.PlotCurveWithDates();

                    // Plota todas as curvas
                    PlotMultipleGraphs plotAll = new PlotMultipleGraphs(allRates);
                    plotAll.PlotCurveWithDates();
                }
            } 
            else
            {
                Console.WriteLine($"FileName {fileName} has incorrect format.");
            }
        }
    }
}