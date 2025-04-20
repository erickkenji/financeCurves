using Curves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
    public class MeanSquareError
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
    }
}
