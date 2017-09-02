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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls.EIMS
{
    public partial class ARReceiveDetails : UserControl
    {
        ReceivedReportQueryFilter m_queryRequest;
        ReceivedReportFacade m_facade;

        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public ARReceiveDetails()
        {
            InitializeComponent();
        }

        public ARReceiveDetails(ReceivedReportQueryFilter filter)
        {
            InitializeComponent();
            m_queryRequest = filter;
            Loaded += new RoutedEventHandler(ARReceiveDetails_Loaded);
        }

        void ARReceiveDetails_Loaded(object sender, RoutedEventArgs e)
        {
            this.dgQueryDetials.Bind();
            m_facade = new ReceivedReportFacade();
            m_facade.ARReceiveDetialsQuery(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryDetials.TotalCount = args.Result.TotalCount;
                this.dgQueryDetials.ItemsSource = args.Result.Rows;
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.Close();
            }
        }

        private void dgQueryDetials_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };

            m_facade = new ReceivedReportFacade();
            m_facade.ARReceiveDetialsQuery(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryDetials.TotalCount = args.Result.TotalCount;
                this.dgQueryDetials.ItemsSource = args.Result.Rows;
            });
        }

        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryDetials_ExportAllClick(object sender, EventArgs e)
        {
            if (dgQueryDetials.ItemsSource == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResEIMSReceiveReport.Msg_PleaseQueryData);
                return;
            }
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            ReceivedReportFacade facade = new ReceivedReportFacade();

            ColumnSet col = new ColumnSet(dgQueryDetials);
            facade.ExportARReceiveDetials(m_queryRequest, new ColumnSet[] { col });
        }
    }
}
