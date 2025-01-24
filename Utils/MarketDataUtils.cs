using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using QuantLib;
using Calendar = QuantLib.Calendar;
using Path = System.IO.Path;
using Utils;

namespace Utils
{
    public static class MarketDataUtils
    {
        // Fixing Data utilizando design pattern Lazy
        private static Lazy<Dictionary<DateTime, double>> usdBtcFixings = new Lazy<Dictionary<DateTime, double>>(() => LoadFixings());
        public static Dictionary<DateTime, double> UsdBtcFixings => usdBtcFixings.Value;

        private static Dictionary<DateTime, double> LoadFixings()
        {
            Dictionary<DateTime, double> fixingsCollection = new Dictionary<DateTime, double>() { };

            string driverRelativePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "chromedriver-win64");
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            
            using (ChromeDriver driver = new ChromeDriver(driverRelativePath, options))
            {
                try
                {
                    string formattedUrl = $"https://finance.yahoo.com/quote/BTC-USD/history/";
                    driver.Navigate().GoToUrl(formattedUrl);
                    System.Threading.Thread.Sleep(2000);
                    var tableRows = driver.FindElements(By.XPath("//table[contains(@class, 'table yf-1jecxey noDl')]/tbody/tr"));

                    if (tableRows != null && tableRows.Count > 0)
                    {
                        foreach (var row in tableRows)
                        {
                            var columns = row.FindElements(By.TagName("td"));
                            if (columns != null && columns.Count >= 6)
                            {
                                string dateText = columns[0].Text.Trim();
                                string priceText = columns[4].Text.Trim();

                                string[] formats = { "MMM d, yyyy", "MMM dd, yyyy" };
                                DateTime date = DateTime.TryParseExact(dateText, formats,
                                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                                    ? date
                                    : DateTime.MinValue;
                                double price = double.Parse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture);

                                fixingsCollection.Add(date, price);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error: {ex.Message}");
                }
                driver.Quit();
            }
            return fixingsCollection;
        }

        public static double GetUSDBTCSpotPrice(DateTime referenceDate)
        {
            double price = UsdBtcFixings.TryGetValue(referenceDate, out price) ? price : 0.0;
            return price;
        }
    }
}
