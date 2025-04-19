using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.DevTools.V129.Page;

namespace Utils
{
    public class CubicSplineCurve
    {
        private readonly List<double> _x;
        private readonly List<double> _y;
        private readonly List<double> _a;
        private readonly List<double> _b;
        private readonly List<double> _c;
        private readonly List<double> _d;
        private readonly List<DateTime> _dates;
        private readonly DateTime ReferenceDate;
        private readonly bool singlePlot;

        public CubicSplineCurve(DateTime referenceDate, Dictionary<DateTime, double> rates, bool singlePlot)
        {
            this.ReferenceDate = referenceDate;
            this.singlePlot = singlePlot;

            _dates = rates.Keys.OrderBy(d => d).ToList();
            _x = _dates.Select(date => (double)(date - _dates.First()).TotalDays).ToList();
            _y = rates.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();

            int n = _x.Count;
            _a = new List<double>(_y); // valores de _a do polinômio são inicializados com _y
            _b = new List<double>(new double[n - 1]);
            _c = new List<double>(new double[n]);
            _d = new List<double>(new double[n - 1]);

            // Cálculo da distância entre os pontos _x
            double[] h = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                h[i] = _x[i + 1] - _x[i];
            }

            // alpha relaciona as diferenças entre valores de _a (os y) para determinar as inclinações relativas entre pontos consecutivos.
            double[] alpha = new double[n - 1];
            for (int i = 1; i < n - 1; i++)
            {
                alpha[i] = (3 / h[i]) * (_a[i + 1] - _a[i]) - (3 / h[i - 1]) * (_a[i] - _a[i - 1]);
            }

            // Cálculo dos coeficientes cúbicos
            double[] l = new double[n];
            double[] mu = new double[n];
            double[] z = new double[n];
            l[0] = 1;
            mu[0] = 0;
            z[0] = 0;

            for (int i = 1; i < n - 1; i++)
            {
                l[i] = 2 * (_x[i + 1] - _x[i - 1]) - h[i - 1] * mu[i - 1];
                mu[i] = h[i] / l[i];
                z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
            }

            l[n - 1] = 1;
            z[n - 1] = 0;
            _c[n - 1] = 0;

            for (int j = n - 2; j >= 0; j--)
            {
                _c[j] = z[j] - mu[j] * _c[j + 1];
                _b[j] = (_a[j + 1] - _a[j]) / h[j] - h[j] * (_c[j + 1] + 2 * _c[j]) / 3;
                _d[j] = (_c[j + 1] - _c[j]) / (3 * h[j]);
            }
        }

        private double Interpolate(DateTime date)
        {
            double x = (date - _dates.First()).TotalDays;
            int i = _x.BinarySearch(x);
            if (i < 0) i = ~i - 1;
            if (i < 0) i = 0;
            if (i >= _a.Count - 1) i = _a.Count - 2;

            double dx = x - _x[i];
            return _a[i] + _b[i] * dx + _c[i] * dx * dx + _d[i] * dx * dx * dx;
        }

        public Dictionary<DateTime, double> InterpolateAll()
        {
            var interpolated = new Dictionary<DateTime, double>();
            DateTime currentDate = _dates.First();
            DateTime maxDate = _dates.Last();

            while (currentDate <= maxDate)
            {
                interpolated[currentDate] = Interpolate(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            return interpolated;
        }

        public StandardCurve GetStandardCurve()
        {
            return new StandardCurve(this.ReferenceDate, this.InterpolateAll(), this.singlePlot);
        }
    }

}
