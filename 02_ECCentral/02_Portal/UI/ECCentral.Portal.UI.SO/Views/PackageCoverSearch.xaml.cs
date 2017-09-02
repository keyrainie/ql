using System;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class PackageCoverSearch : PageBase
    {
        CommonDataFacade m_commonFacade;
        SOPackageCoverSearchFilter m_queryRequest;
        public PackageCoverSearch()
        {
            InitializeComponent();
        }
             
        //页面加载事件 
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
        }
        //初始化页面
        private void IniPageData() {
            this.dtpShippedOutTimeTo.SelectedDateTime = DateTime.Now;
            m_commonFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            m_queryRequest = new SOPackageCoverSearchFilter();
            this.SearchCondition.DataContext = m_queryRequest;
            BindComboBoxData();
        }

        //查询
        private void btnSearch_Click(object sender,RoutedEventArgs e) {
            this.QueryResultGrid.Bind();
        }

        private void dataGridPackageList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            m_queryRequest.ReceiveAreaSysNo = ucReceiveArea.QueryAreaSysNo;
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOPackageCoverSearch(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.QueryResultGrid.TotalCount = args.Result.TotalCount;
                this.QueryResultGrid.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
            });
        }

        //导出数据到Excel
        private void dataGridPackageCoverSearchList_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_ExcelExport))
            {
                Window.Alert(ResSO.Msg_Error_Right, MessageType.Error);
                return;
            }

            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            ColumnSet col = new ColumnSet(QueryResultGrid);
            facade.ExportSOPackageCoverSearch(m_queryRequest, new ColumnSet[] { col });
        }


        //绑定下拉框数据
        private void BindComboBoxData()
        {
            //包裹签收状态
            this.cmbpackageSign.ItemsSource = EnumConverter.GetKeyValuePairs<PackageSignStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbpackageSign.SelectedIndex = 0;
        }


        /// <summary>
        /// 查看对应的运单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtn_SOID_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hlbtn = sender as HyperlinkButton;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, hlbtn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }      
    }
}

