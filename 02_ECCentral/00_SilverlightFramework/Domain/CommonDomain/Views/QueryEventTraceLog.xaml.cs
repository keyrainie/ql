using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View]
    public partial class QueryEventTraceLog : PageBase
    {
        private RestClient m_RestClient;
        private QueryEventLogCriteria m_QueryCriteria ;

        public QueryEventTraceLog()
        {
            InitializeComponent();

            List<MaxCount> list = new List<MaxCount>();
            list.Add(new MaxCount() { Count = 50 });
            list.Add(new MaxCount() { Count = 100 });
            list.Add(new MaxCount() { Count = 500 });
            list.Add(new MaxCount() { Count = 1000 });
            list.Add(new MaxCount() { Count = 5000 });
            comboBoxMaxCount.ItemsSource = list;

            m_RestClient = new RestClient("/Service/Framework/V50/StatisticService.svc", this);

            m_QueryCriteria = new QueryEventLogCriteria();
            stackPanelSearch.DataContext = m_QueryCriteria;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(stackPanelSearch))
            {
                m_RestClient.Query<List<EventLogView>>("QueryEventLog", m_QueryCriteria, (o, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        this.dgSearchResult.ItemsSource = args.Result;

                    }
                });
            }
        }

    }

    public class QueryEventLogCriteria : ModelBase
    {
        public QueryEventLogCriteria()
        {
            TopCount = 50;
            EventDateFrom = DateTime.Now.AddDays(-3).ToShortDateString();
            EventDateTo = DateTime.Now.ToShortDateString(); ;
        }

        public int TopCount { get; set; }

        public string UserID { get; set; }

        public string Page { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public string EventDateFrom { get; set; }

        public string EventDateTo { get; set; }
    }


    public class EventLogView
    {
        public string EventLogID { get; set; }

        public string UserID { get; set; }

        public string IP { get; set; }

        public DateTime EventDate { get; set; }

        public string Url { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public string Page { get; set; }

        public string EventDateForUI { get; set; }
    }

    public class MaxCount
    {
        public int Count { get; set; }
    }
}
