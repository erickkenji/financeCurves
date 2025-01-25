using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static System.Net.Mime.MediaTypeNames;

namespace Utils
{
    public class PlotGraph
    {
        private readonly Dictionary<DateTime, double> curve;
        private readonly DateTime referenceDate;
        public PlotGraph(Dictionary<DateTime, double> curve, DateTime referenceDate)
        {
            this.curve = curve;
            this.referenceDate = referenceDate;
        }

        public void PlotCurve()
        {
            var plotModel = new PlotModel { Title = "Crypto Currency Rates Curve" };

            plotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd/MM/yyyy",
                Title = "Data",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Rate",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = -0.4,  // Limite inferior fixo
                Maximum = 0.4    // Limite superior fixo
            });

            var lineSeries = new LineSeries { Title = "Data", MarkerType = MarkerType.Circle };

            foreach (var point in this.curve)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Key), point.Value));
            }

            plotModel.Series.Add(lineSeries);

            // Exportar o gráfico como SVG
            var exporter = new SvgExporter { Width = 800, Height = 600 };
            using (var stream = File.Create($"{this.referenceDate:yyyy-MM-dd}.svg"))
            {
                exporter.Export(plotModel, stream);
            }
        }
    }

    public class PlotMultipleGraphs
    {
        private readonly Dictionary<DateTime, Dictionary<DateTime, double>> curves; // Lista de curvas

        public PlotMultipleGraphs(Dictionary<DateTime, Dictionary<DateTime, double>> curves)
        {
            this.curves = curves;
        }

        public void PlotCurve()
        {
            var plotModel = new PlotModel { Title = "Crypto Currency Rates Curve" };

            // Configurar eixo X (Data)
            plotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd/MM/yyyy",
                Title = "Data",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid
            });

            // Configurar eixo Y (Taxas)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Rate",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = -0.4,  // Limite inferior fixo
                Maximum = 0.4    // Limite superior fixo
            });

            foreach (var curve in curves)
            {
                DateTime referenceDate = curve.Key;
                var lineSeries = new LineSeries
                {
                    Title = $"{referenceDate:dd/MM}", // Nome único para cada série
                    MarkerType = MarkerType.Circle
                };

                foreach (var point in curve.Value)
                {
                    var monthsSinceBase = ((point.Key.Year - referenceDate.Year) * 12) + (point.Key.Month - referenceDate.Month);
                    lineSeries.Points.Add(new DataPoint(monthsSinceBase, point.Value));
                }
                plotModel.Series.Add(lineSeries);
            }

            // Exportar o gráfico como SVG
            var exporter = new SvgExporter { Width = 800, Height = 600 };
            using (var stream = File.Create($"allRates.svg"))
            {
                exporter.Export(plotModel, stream);
            }
        }
    }
}
