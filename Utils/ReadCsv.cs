﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using QuantLib;

namespace Utils
{
    public static class ReadCsv
    {
        public static Dictionary<DateTime, double> ReadCsvFile(string filePath)
        {
            Dictionary<DateTime, double> futurePrices = new Dictionary<DateTime, double>() {};

            using (StreamReader reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();
                if (headerLine == null || !headerLine.Contains("Month") || !headerLine.Contains("Price"))
                {
                    throw new Exception("Invalid CSV file.");
                }

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    if (parts.Length != 2)
                    {
                        throw new Exception($"Invalid line on CSV file: {line}");
                    }

                    if (!DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime month))
                    {
                        throw new Exception($"Invalid date on line: {line}");
                    }

                    if (!double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                    {
                        throw new Exception($"Invalid price on line: {line}");
                    }

                    futurePrices.Add(month, price);
                }
            }

            return futurePrices;
        }
    }
}
