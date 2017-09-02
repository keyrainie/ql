using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;

namespace Newegg.Oversea.Silverlight.CommonDomain.Utilities
{

    public class ChartGenerator
    {
        private IList<ISeries> m_series;
        internal IList<IEnumerable> DataSources { get; set; }

        public ChartGenerator()
        {
            this.m_series = new List<ISeries>();
            this.DataSources = new List<IEnumerable>();
        }

        public void CreateSeries(string title, IEnumerable dataSource, string independentValueBinding, string dependentValueBinding)
        {
            var series = new LineSeries();

            series.Title = title;
            series.ItemsSource = dataSource;
            series.IndependentValuePath = independentValueBinding;
            series.DependentValuePath = dependentValueBinding;
           
            this.DataSources.Add(dataSource);
            this.m_series.Add(series);
        }

        /// <summary>
        /// 加载图表
        /// </summary>
        /// <param name="container"></param>
        public void LoadChart(Panel container, List<IAxis> axes)
        {
            var chart = this.CreateChart();
            if (axes != null)
            {
                axes.ForEach(item => { chart.Axes.Add(item); });
            }

            foreach (var item in this.m_series)
            {
                chart.Series.Add(item);
            }
            container.Children.Add(chart);
        }
        /// <summary>
        /// 创建图表对象
        /// </summary>
        private Chart CreateChart()
        {
            var chart = new Chart();
            chart.Height = 180;
            chart.Style = Application.Current.Resources["OVSChartStyle"] as Style;

            return chart;
        }
    }
}
