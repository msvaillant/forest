using LiveCharts;
using LiveCharts.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Configurations;
using LiveCharts.Defaults;

namespace Fire.Forest.WPF
{
    class MainWindowModel : ReactiveObject
    {
        private readonly Random rnd = new Random();
        LineSeries treeSeries, fireSeries, ashesSeries, fireCenterSeries;
        
        public MainWindowModel()
        {
            var mapper = Mappers.Xy<double>()
                .X((point, iter) =>
                {
                    return Math.Log(iter + 1);
                }) //a 10 base log scale in the X axis
                .Y((point, iter) =>
                {
                    if (point == 0)
                        return double.NaN;

                    return Math.Log(point);
                });

            treeSeries = new LineSeries
            {
                Title = "Trees Count",
                Values = new ChartValues<int> {},
                PointGeometry = null,
                Stroke = Brushes.Green
            };
            fireSeries = new LineSeries
            {
                Title = "Fire-points Count",
                Values = new ChartValues<int> { },
                PointGeometry = null,
                Stroke = Brushes.OrangeRed
            };
            ashesSeries = new LineSeries
            {
                Title = "Ashes Count",
                Values = new ChartValues<int> {},
                PointGeometry = null,
                Stroke = Brushes.Gray
            };
            ForestDynamics = new SeriesCollection
            {
                treeSeries,
                fireSeries,
                ashesSeries
            };

            fireCenterSeries = new LineSeries
            {
                Title = "Fire Centers Count",
                PointGeometry = null,
                Values = new ChartValues<double> { }
            };

            FireCentersDynamics = new SeriesCollection(mapper)
            {
                fireCenterSeries
            };
        }

        public float F { get; set; } = 0.02f;
        public float G { get; set; } = 0.001f;
        public float P { get; set; } = 0.03f;


        public SeriesCollection ForestDynamics { get; set; }
        public List<int> ForestDynamicsLabels { get; }
            = new List<int>();
        public string ForestDynamicsYFormatter(double val)
            => val.ToString("C");


        public SeriesCollection FireCentersDynamics { get; set; }
        public List<double> Labels { get; }
            = new List<double>();
        public string YFormatter(double val)
            => (val).ToString("N");

        public double Base { get; set; } = Math.E;

        public void AddDynamicsPoint(int trees, int fires, int ashes)
        {
            treeSeries.Values.Add(trees);
            fireSeries.Values.Add(fires);
            ashesSeries.Values.Add(ashes);
        }
        public IEnumerable<int> SetFireCenterDynamics
        {
            set
            {
                var chartValues = new ChartValues<double>(value.Select(x => (double)x));
                fireCenterSeries.Values = chartValues;
            }
        }
    }
}
