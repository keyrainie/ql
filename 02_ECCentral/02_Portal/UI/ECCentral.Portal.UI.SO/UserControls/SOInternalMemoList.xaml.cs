using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOInternalMemoList : UserControl
    {
        public int SOSysNo { get; set; }

        public SOInternalMemoList()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOInternalMemoList_Loaded);
        }

        void SOInternalMemoList_Loaded(object sender, RoutedEventArgs e)
        {
            Bind();
        }

        private void dataGridLogList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SOInternalMemoQueryFilter query = new SOInternalMemoQueryFilter();
            query.SOSysNo = SOSysNo;
            query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };

            SOQueryFacade facade = new SOQueryFacade();
            facade.QuerySOInternalMemo(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridLogList.TotalCount = args.Result.TotalCount;
                dataGridLogList.ItemsSource = args.Result.Rows;
            });
        }

        public void Bind()
        {
            dataGridLogList.Bind();
        }

        private void hlbtnClose_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGridLogList.SelectedItem as DynamicXml;
            if (null != selectedModel)
            {
                publicMemoClose ctrl = new publicMemoClose((int)selectedModel["SysNo"]);
                ctrl.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(
                           ResSOInternalMemo.Header_CloseLog
                           , ctrl
                           , (s, args) =>
                           {
                               //关闭没有返回值，同于列表页面
                               if (args.DialogResult == DialogResultType.OK)
                               {
                                   dataGridLogList.PageIndex = 0;
                                   dataGridLogList.SelectedIndex = -1;
                                   dataGridLogList.Bind();
                               }
                           }
                           , new Size(520, 360)
                    );
            }
        }
    }
}
