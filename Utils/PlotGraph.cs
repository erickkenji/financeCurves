﻿using System;
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

        public void PlotCurveWithDates()
        {
            var plotModel = new PlotModel { Title = $"Curva de juros de USDBTC - {this.referenceDate:dd-MM-yyyy}" };

            // Configurar eixo X (Datas)
            plotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MM/yy",
                Title = "Data",
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid
            });

            // Configurar eixo Y (Taxas)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Taxa",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = -0.4, // Limite inferior fixo
                Maximum = 0.4   // Limite superior fixo
            });

            var lineSeries = new LineSeries { Title = "Data", MarkerType = MarkerType.Circle };

            foreach (var point in this.curve)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(point.Key), point.Value));
            }

            plotModel.Series.Add(lineSeries);

            // Exportar o gráfico como SVG
            var exporter = new SvgExporter { Width = 800, Height = 600 };
            using (var stream = File.Create($"{referenceDate:dd-MM-yyyy}.svg"))
            {
                exporter.Export(plotModel, stream);
            }
        }

        public void PlotCurveWithMonths()
        {
            var plotModel = new PlotModel { Title = $"Curva de juros de USDBTC - {this.referenceDate:dd-MM-yyyy}" };

            // Configurar eixo X (Meses desde a data de referência)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Meses",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = 0, // Início em 0 meses
                StringFormat = "0M" // Rótulos no formato "1M", "2M", etc.
            });

            // Configurar eixo Y (Taxas)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Taxa",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = -0.4, // Limite inferior fixo
                Maximum = 0.4   // Limite superior fixo
            });

            var lineSeries = new LineSeries
            {
                Title = $"{this.referenceDate:dd/MM}", // Nome único para cada série
                MarkerType = MarkerType.Circle
            };

            foreach (var point in this.curve)
            {
                // Calcular meses desde a data de referência
                var monthsSinceBase = ((point.Key.Year - referenceDate.Year) * 12) + (point.Key.Month - referenceDate.Month);
                lineSeries.Points.Add(new DataPoint(monthsSinceBase, point.Value));
            }

            plotModel.Series.Add(lineSeries);

            // Exportar o gráfico como SVG
            var exporter = new SvgExporter { Width = 800, Height = 600 };
            using (var stream = File.Create($"{referenceDate:dd-MM-yyyy}.svg"))
            {
                exporter.Export(plotModel, stream);
            }
        }
    }

    public class PlotMultipleGraphs
    {
        private readonly Dictionary<DateTime, Dictionary<DateTime, double>> curves;

        public PlotMultipleGraphs(Dictionary<DateTime, Dictionary<DateTime, double>> curves)
        {
            this.curves = curves;
        }

        public void PlotCurve()
        {
            var plotModel = new PlotModel { Title = "Curva de juros de USDBTC" };

            // Configurar eixo X (Meses desde a data de referência)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Meses",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = 0, // Início em 0 meses
                StringFormat = "0M" // Rótulos no formato "1M", "2M", etc.
            });

            // Configurar eixo Y (Taxas)
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Taxa",
                MajorGridlineStyle = LineStyle.Solid,
                Minimum = -0.4, // Limite inferior fixo
                Maximum = 0.4   // Limite superior fixo
            });

            // Iterar sobre as curvas e adicionar séries
            foreach (var curve in curves)
            {
                DateTime referenceDate = curve.Key; // Data de referência para a curva
                var lineSeries = new LineSeries
                {
                    Title = $"{referenceDate:dd/MM}", // Nome único para cada série
                    MarkerType = MarkerType.Circle
                };

                foreach (var point in curve.Value)
                {
                    // Calcular meses desde a data de referência
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
