using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrics
{
    public class StandardDeviation
    {
        public static double Calculate(double[] values)
        {
            // Calcular a média (μ) dos valores
            double medium = values.Average();

            // Calcular a soma dos quadrados das diferenças em relação à média
            double quadraticSum = values.Select(val => Math.Pow(val - medium, 2)).Sum();

            // Calcular o desvio padrão
            double standardDeviation = Math.Sqrt(quadraticSum / values.Length);
            return standardDeviation;
        }
    }
}
