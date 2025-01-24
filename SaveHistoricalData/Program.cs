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
        new DateTime(2025, 01, 23),
        new DateTime(2025, 01, 22),
        new DateTime(2025, 01, 21),
        new DateTime(2025, 01, 17),
        new DateTime(2025, 01, 16),
        new DateTime(2025, 01, 15),
        new DateTime(2025, 01, 14),
        new DateTime(2025, 01, 13),
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
        string driverRelativePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "chromedriver-win64");

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        // CME é uma exchange americana, usamos calendário NYC
        Calendar calendar = new UnitedStates(UnitedStates.Market.NYSE); 

        foreach (DateTime referenceDate in referenceDateCollection)
        {
            using (ChromeDriver driver = new ChromeDriver(driverRelativePath, options))
            {
                try
                {
                    string formattedUrl = $"https://www.cmegroup.com/markets/cryptocurrencies/bitcoin/bitcoin.settlements.html#tradeDate={referenceDate.ToString("MM")}%2F{referenceDate.ToString("dd")}%2F{referenceDate.ToString("yyyy")}";
                    
                    driver.Navigate().GoToUrl(formattedUrl);
                    System.Threading.Thread.Sleep(2000);

                    IWebElement containerDiv = driver.FindElement(By.CssSelector(".main-table-wrapper[role='presentation']"));
                    IWebElement table = containerDiv.FindElement(By.CssSelector("table"));
                    IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody tr"));

                    List<(DateTime maturity, double price)> priceCollection = new List<(DateTime maturity, double price)>();
                    foreach (IWebElement row in rows)
                    {
                        try
                        {
                            string monthText = row.FindElement(By.CssSelector("td:nth-child(1)")).Text;
                            string priceText = row.FindElement(By.CssSelector("td:nth-child(7)")).Text;

                            DateTime month = DateTime.ParseExact(monthText, "MMM yy", CultureInfo.InvariantCulture);
                            QuantLib.Date QuantLibMaturityDate = calendar.endOfMonth(QuantLibUtils_.GetQuantLibDateFromDateTime(month));
                            DateTime maturityDate = QuantLibUtils_.GetDateTimeFromQuantLibDate(QuantLibMaturityDate);

                            double price = double.Parse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture);

                            priceCollection.Add((maturityDate, price));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on processing table: {ex.Message}");
                        }
                    }

                    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "HistoricalData", $"{referenceDate:yyyy}{referenceDate:MM}{referenceDate:dd}.csv");
                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        writer.WriteLine("Month, Price");
                        foreach (var entry in priceCollection)
                        {
                            writer.WriteLine($"{entry.maturity:yyyy-MM-dd},{entry.price}");
                        }
                    }

                    Console.WriteLine($"Saved data on {outputPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error: {ex.Message}");
                }
                driver.Quit();
            }
        }
    }
}