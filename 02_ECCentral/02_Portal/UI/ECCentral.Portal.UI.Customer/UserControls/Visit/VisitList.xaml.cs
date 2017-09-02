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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class VisitList : UserControl
    {
        public VisitList()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(VisitList_Loaded);
        }

        void VisitList_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsOrderVisit)
            {
                gridVisitLogs.Columns[4].Visibility = gridVisitLogs.Columns[8].Visibility = gridVisitLogs.Columns[9].Visibility = gridVisitLogs.Columns[10].Visibility = System.Windows.Visibility.Collapsed;
                gridVisitLogs.Columns[10].Visibility = gridVisitLogs.Columns[7].Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                gridVisitLogs.Columns[4].Visibility = gridVisitLogs.Columns[8].Visibility = gridVisitLogs.Columns[9].Visibility = gridVisitLogs.Columns[10].Visibility = System.Windows.Visibility.Visible;
                gridVisitLogs.Columns[10].Visibility = gridVisitLogs.Columns[7].Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private List<VisitLogVM> logs;
        public List<VisitLogVM> Logs
        {
            get
            {
                return logs;
            }
            set
            {
                logs = value;
                gridVisitLogs.ItemsSource = logs;
                gridVisitLogs.Bind();
            }
        }
        public bool IsOrderVisit
        {
            get;
            set;
        }
    }
}
