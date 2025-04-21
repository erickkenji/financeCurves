using Curves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metrics
{
    public class MetricsMethods
    {
        public static double CalculateMeanSquareError(StandardCurve referenceCurve, StandardCurve predictionCurve)
        {
            Dictionary<DateTime, double> reference = referenceCurve.buckets;
            Dictionary<DateTime, double> predictions = predictionCurve.buckets;

            double meanSquareError = 0;
            foreach (var pair in reference) 
            {
                double error = reference[pair.Key] - predictions[pair.Key];
                meanSquareError += error * error;
            }
            double mse = meanSquareError / reference.Count;
            
            return mse;
        }

        public static double CalculateFirstDerivativeStandardDeviation(StandardCurve curve)
        {
            Dictionary<DateTime, double> buckets = curve.buckets;
            List<DateTime> orderedDates = buckets.Keys.OrderBy(x  => x).ToList();

            List<double> derivatives = new List<double>();
            for (int i = 1; i < orderedDates.Count; i++)
            {
                var deltaX = (orderedDates[i] - orderedDates[i - 1]).TotalDays; // Diferença entre datas (em dias)
                var deltaY = buckets[orderedDates[i]] - buckets[orderedDates[i - 1]]; // Diferença entre os valores
                derivatives.Add(deltaY / deltaX); // Calcula a derivada (taxa de variação)
            }

            return MetricsMethods.CalculateStandardDeviation(derivatives.ToArray());
        }

        public static double CalcularPenalidadeDeSuavidade(StandardCurve curve)
        {
            var curva = curve.buckets;
            // Ordena os pontos da curva pela data
            var pontos = curva.OrderBy(x => x.Key).ToList();

            double penalidade = 0;

            // Calcula a segunda derivada discreta e soma o quadrado
            for (int i = 1; i < pontos.Count - 1; i++)
            {
                double h = (pontos[i + 1].Key - pontos[i - 1].Key).TotalDays; // Considerando o espaçamento entre as datas
                double fDoublePrime = (pontos[i + 1].Value - 2 * pontos[i].Value + pontos[i - 1].Value) / (h * h);

                // Penalidade de suavidade (soma do quadrado da segunda derivada)
                penalidade += Math.Pow(fDoublePrime, 2);
            }

            return penalidade;
        }

        public static double CalculateStandardDeviation(double[] values)
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
