using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using Newegg.Oversea.Silverlight.CommonDomain.Models.Statistics;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.CommonDomain.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View(NeedAccess = false)]
    public partial class QueryStatisticsReport : PageBase
    {
        private RestClient m_restClient;
        private static object s_asynObj = new object();

        public QueryStatisticsReport()
        {
            InitializeComponent();

            this.m_restClient = new RestClient("/Service/Framework/V50/StatisticService.svc");
            this.SearchAreaContainer.DataContext = GetQueryCritiera();
            this.BtnSearch.Click += new RoutedEventHandler(BtnSearch_Click);

            this.DataGridPageViews.SelectionChanged += new SelectionChangedEventHandler(DataGridPageViews_SelectionChanged);
            this.DataGridPageViews.LoadingDataSource += new EventHandler<Silverlight.Controls.Data.LoadingDataEventArgs>(DataGridPageViews_LoadingDataSource);

            this.DataGridRelatedUser.LoadingDataSource += new EventHandler<Silverlight.Controls.Data.LoadingDataEventArgs>(DataGridRelatedUser_LoadingDataSource);
            this.DataGridPageActions.LoadingDataSource += new EventHandler<Silverlight.Controls.Data.LoadingDataEventArgs>(DataGridPageActions_LoadingDataSource);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.Window.DocumentVerticalScrollBar = ScrollBarVisibility.Auto;

            this.BtnSearch_Click(null, null);
        }

        #region Events

        void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.SearchAreaContainer))
            {
                this.DataGridPageViews.QueryCriteria = this.GetQueryCritiera();
                this.DataGridPageViews.Bind();

                InitializeChart();
            }
        }

        void DataGridPageViews_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var value = e.AddedItems[0] as PageViewsModel;
                if (value != null)
                {
                    this.ExpanderRelatedUser.Header = QueryReportResource.Expander_Header_UserPageViews + " - " + value.Page;
                    this.ExpanderPageAction.Header = QueryReportResource.Expander_Header_PageActionStatistics + " - " + value.Page;

                    var criteria = this.SearchAreaContainer.DataContext as BaseQueryCriteria;
                    criteria.Url = value.Url;

                    this.DataGridRelatedUser.QueryCriteria = criteria;
                    this.DataGridPageActions.QueryCriteria = criteria;

                    this.DataGridRelatedUser.Bind();
                    this.DataGridPageActions.Bind();
                }
            }
            else
            {
                this.ExpanderRelatedUser.Header = QueryReportResource.Expander_Header_UserPageViews;
                this.ExpanderPageAction.Header = QueryReportResource.Expander_Header_PageActionStatistics;
                this.DataGridRelatedUser.ItemsSource = null;
                this.DataGridPageActions.ItemsSource = null;
            }
        }

        void DataGridPageViews_LoadingDataSource(object sender, Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var criteria = e.QueryCriteria as BaseQueryCriteria;

            m_restClient.Query<ObservableCollection<PageViewsModel>>("QueryPVStatistic", criteria, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.DataGridPageViews.ItemsSource = GetOrderdResult(args.Result);
                if (args.Result.Count > 0)
                {
                    this.DataGridPageViews.SelectedIndex = 0;
                }
            });
        }

        void DataGridPageActions_LoadingDataSource(object sender, Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var criteria = e.QueryCriteria as BaseQueryCriteria;

            m_restClient.Query<ObservableCollection<PageActionsModel>>("QueryActionStatistic", criteria, (obj, args) =>
            {
                if (args.FaultsHandle(this))
                {
                    return;
                }
                this.DataGridPageActions.ItemsSource = this.AppendID(args.Result.OrderByDescending(p => p.Count).ToList());
            });
        }

        void DataGridRelatedUser_LoadingDataSource(object sender, Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var criteria = e.QueryCriteria as BaseQueryCriteria;

            m_restClient.Query<ObservableCollection<PageViewsModel>>("QueryUserPVStatistic", criteria, (obj, args) =>
            {
                if (args.FaultsHandle(this))
                {
                    return;
                }
                this.DataGridRelatedUser.ItemsSource = this.AppendID(args.Result.OrderByDescending(p => p.Pageviews).ToList());
            });
        }

        #endregion

        #region Private Methods

        private void InitializeChart()
        {
            var loadCompletedCount = 0;
            var chart = new ChartGenerator();
            var query = this.GetQueryCritiera();
            IEnumerable list = null;
            IEnumerable uniqueList = null;


            this.ShowLoading();
            this.BtnSearch.IsEnabled = false;

            this.m_restClient.Query<ObservableCollection<LoginStatisticsModel>>("QueryLoginStatistic", query, (obj, args) =>
            {
                if (args.FaultsHandle(this))
                {
                    return;
                }

                Interlocked.Increment(ref loadCompletedCount);

                list = args.Result.OrderBy(p => p.InDate).ToList();

                if (loadCompletedCount == 2)
                {
                    OnLoadCompleted(chart, list, uniqueList);
                }
            });

            this.m_restClient.Query<ObservableCollection<LoginStatisticsModel>>("QueryUniqueLoginStatistic", query, (obj, args) =>
            {
                if (args.FaultsHandle(this))
                {
                    return;
                }

                Interlocked.Increment(ref loadCompletedCount);

                uniqueList = args.Result.OrderBy(p => p.InDate).ToList();


                if (loadCompletedCount == 2)
                {
                    OnLoadCompleted(chart, list, uniqueList);
                }
            });
        }

        private void OnLoadCompleted(ChartGenerator chart, IEnumerable list, IEnumerable uniqueList)
        {
            this.AppendSeries(chart, list, QueryReportResource.Chart_Title_LoginCount);
            this.AppendSeries(chart, uniqueList, QueryReportResource.Chart_Title_UniqueLoginCount);

            this.ChartContainer.Children.Clear();
            this.CloseLoading();
            this.BtnSearch.IsEnabled = true;

            this.LoadChart(chart);
        }

        private IEnumerable GetOrderdResult(ObservableCollection<PageViewsModel> list)
        {
            decimal totalCount = 0;

            list.ToList().ForEach(item => { totalCount += item.Pageviews; });

            var lst = list.OrderByDescending(p => p.Pageviews).ToList();

            for (int i = 0; i < lst.Count; i++)
            {
                var item = lst[i];
                var views = decimal.Parse(item.Pageviews.ToString());
                var percentage = Math.Round((views / totalCount) * 100, 2);

                item.ID = i + 1;
                item.Percentage = string.Format("{0}%", percentage);
            }

            return lst;
        }

        private IEnumerable AppendID(IEnumerable list)
        {
            if (list is List<PageViewsModel>)
            {
                var lst = list as List<PageViewsModel>;

                for (int i = 0; i < lst.Count; i++)
                {
                    lst[i].ID = i + 1;
                }

                return lst;
            }
            if (list is List<PageActionsModel>)
            {
                var lst = list as List<PageActionsModel>;

                for (int i = 0; i < lst.Count; i++)
                {
                    lst[i].ID = i + 1;
                }

                return lst;
            }
            return null;
           
        }

        private BaseQueryCriteria GetQueryCritiera()
        {
            BaseQueryCriteria criteria;

            if (this.SearchAreaContainer.DataContext == null)
            {
                criteria = new BaseQueryCriteria();

                criteria.DateTo = DateTime.Now.Date.AddDays(-1);
                criteria.DateFrom = DateTime.Now.Date.AddDays(-7);
            }
            else
            {
                criteria = this.SearchAreaContainer.DataContext as BaseQueryCriteria;

                if (criteria.DateFrom.HasValue)
                {
                    criteria.DateFrom = criteria.DateFrom.Value.Date;
                }

                if (criteria.DateTo.HasValue)
                {
                    criteria.DateTo = criteria.DateTo.Value.Date;
                }
            }

            //处理时区转换问题。
            criteria.DateFrom = DateTime.SpecifyKind(criteria.DateFrom.Value, DateTimeKind.Utc);
            criteria.DateTo = DateTime.SpecifyKind(criteria.DateTo.Value, DateTimeKind.Utc);

            return criteria;
        }

        private void ShowLoading()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.ChartLoading != null)
                {
                    Canvas.SetZIndex(this.ChartLoading, 1);
                    this.ChartLoading.Start();

                    this.ChartContainer.Opacity = 0.5;
                }
            });
        }

        private void CloseLoading()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.ChartLoading != null)
                {
                    Canvas.SetZIndex(this.ChartLoading, -1);
                    this.ChartLoading.Stop();

                    this.ChartContainer.Opacity = 1;
                }
            });
        }

        #endregion

        #region Charting Methods

        private void AppendSeries(ChartGenerator chart, IEnumerable dataSource, string title)
        {
            if (((ICollection)dataSource).Count > 0)
            {
                chart.CreateSeries(title, dataSource, "InDate", "Count");
            }
        }

        private void LoadChart(ChartGenerator chart)
        {
            var axis = this.GenerateAxis(chart.DataSources);
            chart.LoadChart(this.ChartContainer, axis);

            //chart.LoadChart(this.ChartContainer, this.GenerateAxis(chart.DataSources));
        }

        private List<IAxis> GenerateAxis(IList<IEnumerable> dataSources)
        {

            var list = new List<IAxis>();
            var maxValue = this.GetMaxCountValue(dataSources);
            var maxCount = this.GetMaxDataSourceCount(dataSources);

            //如果没有数据，将不绘制坐标
            if (maxCount == 0)
            {
                return list;
            }

            //生成横向的Axis
            var axisX = new DateTimeAxis();

            //var style = new Style(typeof(DateTimeAxis));
            //style.Setters.Add(new Setter(AxisLabel.StringFormatProperty, "{}{0:yyyy-MM-dd ddd}"));
            //axisX.AxisLabelStyle = style;


            axisX.Orientation = AxisOrientation.X;
            axisX.ShowGridLines = true;

            double interval = Math.Round(maxCount / 15, 0); ;

            if (maxCount <= 1)
            {
                interval = Math.Round(365d / 15, 0);
            }
            else if (maxCount == 0 || interval == 0)
            {
                interval = 1;
            }

            axisX.IntervalType = DateTimeIntervalType.Days;
            axisX.Interval = (double)interval;

            //生成纵向的Axis
            var axisY = new LinearAxis();
            axisY.Orientation = AxisOrientation.Y;
            axisY.ShowGridLines = true;

            var maximum = 0d;
            if (maxValue == 0 || maxValue == int.MinValue)
            {
                maximum = 10;
            }
            else
            {
                maximum = Math.Round(maxValue * 1.3, 0);
            }

            axisY.Maximum = maximum;
            axisY.Minimum = 0;

            list.Add(axisX);
            list.Add(axisY);

            return list;
        }

        private double GetMaxDataSourceCount(IList<IEnumerable> dataSources)
        {
            double maxCount = 0;
            foreach (var dataSource in dataSources)
            {
                var count = ((ICollection)dataSource).Count;
                if (count >= maxCount)
                {
                    maxCount = count;
                }
            }
            return maxCount;
        }

        private int GetMaxCountValue(IList<IEnumerable> dataSources)
        {
            int maxCount = int.MinValue;
            foreach (var dataSource in dataSources)
            {
                foreach (LoginStatisticsModel item in dataSource)
                {
                    if (item.Count > maxCount)
                    {
                        maxCount = item.Count;
                    }
                }
            }

            return maxCount;
        }

        #endregion
    }
}
