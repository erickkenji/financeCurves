using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    private static HashSet<DateTime> referenceDateCollection = new HashSet<DateTime>()
    {
        new DateTime(2025, 01, 03),
        new DateTime(2025, 01, 02),
        new DateTime(2024, 12, 31),
        new DateTime(2024, 12, 30)
    };

    static void Main(string[] args)
    {
        string driverPath = "C:\\Program Files\\Google\\Chrome\\chromedriver-win64";

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        foreach (DateTime referenceDate in referenceDateCollection)
        {
            using (ChromeDriver driver = new ChromeDriver(driverPath, options))
            {
                try
                {
                    string formattedUrl = $"https://www.cmegroup.com/markets/cryptocurrencies/bitcoin/bitcoin.settlements.html#tradeDate={referenceDate.ToString("MM")}%2F{referenceDate.ToString("dd")}%2F{referenceDate.ToString("yyyy")}";
                    
                    driver.Navigate().GoToUrl(formattedUrl);

                    // Wait page loading
                    System.Threading.Thread.Sleep(2000);

                    // Locate table web element
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
                            DateTime maturityDate = month; // TODO: pegar lastBusinessDayOfMonth
                            double price = double.Parse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture);

                            priceCollection.Add((maturityDate, price));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error on processing table: {ex.Message}");
                        }
                    }

                    // Save to CSV
                    string outputPath = $"{referenceDate.ToString("yyyy")}{referenceDate.ToString("MM")}{referenceDate.ToString("dd")}_prices.csv";
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