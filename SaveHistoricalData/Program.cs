using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main(string[] args)
    {
        string driverPath = "C:\\Program Files\\Google\\Chrome\\chromedriver-win64"; // Substitua pelo caminho do ChromeDriver no seu sistema

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized"); // Executar em modo headless (opcional)

        using (ChromeDriver driver = new ChromeDriver(driverPath, options))
        {
            try
            {
                string url = "https://www.cmegroup.com/markets/cryptocurrencies/bitcoin/bitcoin.settlements.html#tradeDate=01%2F03%2F2025";
                driver.Navigate().GoToUrl(url);

                // Wait page loading
                System.Threading.Thread.Sleep(5000);

                // Locate table web element
                IWebElement containerDiv = driver.FindElement(By.CssSelector(".main-table-wrapper[role='presentation']"));
                IWebElement table = containerDiv.FindElement(By.CssSelector("table"));
                IList<IWebElement> rows = table.FindElements(By.CssSelector("tbody tr"));

                List<(DateTime Month, double price)> priceCollection = new List<(DateTime Month, double price)>();
                foreach (IWebElement row in rows)
                {
                    try
                    {
                        string monthText = row.FindElement(By.CssSelector("td:nth-child(1)")).Text;
                        string settleText = row.FindElement(By.CssSelector("td:nth-child(7)")).Text;

                        DateTime month = DateTime.ParseExact(monthText, "MMM yy", CultureInfo.InvariantCulture);
                        double settle = double.Parse(settleText, NumberStyles.Any, CultureInfo.InvariantCulture);

                        priceCollection.Add((month, settle));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar uma linha: {ex.Message}");
                    }
                }

                // Save to CSV
                string outputPath = "output.csv";
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    writer.WriteLine("Month,Settle");
                    foreach (var entry in priceCollection)
                    {
                        writer.WriteLine($"{entry.Month:yyyy-MM-dd},{entry.Settle}");
                    }
                }

                Console.WriteLine($"Dados salvos com sucesso em {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }
}