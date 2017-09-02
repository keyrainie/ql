using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOComplainList : UserControl
    {
        public int SOSysNo { get; set; }

        public SOComplainList()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOComplainList_Loaded);
        }

        void SOComplainList_Loaded(object sender, RoutedEventArgs e)
        {
            Bind();
        }

        /// <summary>
        /// 对外接口
        /// </summary>
        public void Bind()
        {
            dataGridComplainList.Bind();
        }

        private void dataGridComplainList_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ComplainQueryFilter query = new ComplainQueryFilter();
            query.SOSysNo = SOSysNo;
            query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade();
            facade.QueryComplainList(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridComplainList.TotalCount = args.Result.TotalCount;
                dataGridComplainList.ItemsSource = args.Result.Rows;
            });
        }

        private void hlbtnComplain_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.dataGridComplainList.SelectedItem as DynamicXml;
            if (info != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.SO_ComplainReplyUrl, info["SysNo"]), null, true);
            }
        }
    }
}
