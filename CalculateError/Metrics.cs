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

        public static Dictionary<DateTime, double> SecondDerivativePerInterval(StandardCurve interpolatedCurve, StandardCurve originalCurve)
        {
            // Ordenar os pontos pela data (necessário para garantir que estamos calculando as derivadas na ordem correta)
            var interpolratedCurveBuckets = interpolatedCurve.buckets.OrderBy(p => p.Key).ToList();
            var originalCurveBuckets = originalCurve.buckets.OrderBy(p => p.Key).ToList();

            Dictionary<DateTime, double> secondDerivativeByVertex = new Dictionary<DateTime, double>();
            List<double> variationPerVertex = new List<double>();

            // Percorrer os pontos da curva não interpolada
            for (int i = 0; i < originalCurveBuckets.Count - 1; i++)
            {
                // Encontrar os pontos da curva interpolada que estão dentro do intervalo definido pelos pontos da curva não interpolada
                var firstDot = originalCurveBuckets[i];
                var finalDot = originalCurveBuckets[i + 1];

                // Extrair os pontos interpolados que estão entre o intervalo
                var interpolatedInterval = interpolratedCurveBuckets.Where(p => p.Key >= firstDot.Key && p.Key <= finalDot.Key).ToList();

                // Verificar se há pelo menos dois pontos interpolados no intervalo
                if (interpolatedInterval.Count > 1)
                {
                    // Calcular a primeira derivada da curva interpolada
                    List<double> firstDerivative = new List<double>();
                    for (int j = 1; j < interpolatedInterval.Count; j++)
                    {
                        double deltaT = (interpolatedInterval[j].Key - interpolatedInterval[j - 1].Key).TotalDays;
                        double deltaY = interpolatedInterval[j].Value - interpolatedInterval[j - 1].Value;
                        firstDerivative.Add(deltaY / deltaT);
                    }

                    // Calcular a segunda derivada da curva interpolada
                    List<double> secondDerivativeCollection = new List<double>();
                    for (int j = 1; j < firstDerivative.Count; j++)
                    {
                        double deltaT = (interpolatedInterval[j + 1].Key - interpolatedInterval[j].Key).TotalDays;
                        double deltaY = firstDerivative[j] - firstDerivative[j - 1];
                        secondDerivativeCollection.Add(deltaY / deltaT);
                    }

                    // Medir a variação das derivadas no intervalo
                    double variation = 0;
                    foreach (var secondDerivative in secondDerivativeCollection)
                    {
                        variation += secondDerivative;  // Variação total da segunda derivada
                    }

                    variationPerVertex.Add(variation);
                    secondDerivativeByVertex.Add(originalCurveBuckets[i].Key, variation);
                }
            }

            return secondDerivativeByVertex;
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
