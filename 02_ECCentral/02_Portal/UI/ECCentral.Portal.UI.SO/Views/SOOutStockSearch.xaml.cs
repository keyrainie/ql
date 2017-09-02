using System;
using System.Windows;
using System.Collections.Generic;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Common;
using System.Windows.Controls;


namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class SOOutStockSearch : PageBase
    {
        SOOutStockQueryFilter m_queryRequest;
        CommonDataFacade m_commonDataFacade;
        List<ValidationEntity> orderList;

        public SOOutStockSearch()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
            orderList = new List<ValidationEntity>();
        }

        /// <summary>
        /// 初始化页面数据
        /// </summary>
        private void IniPageData()
        {
            this.dtpShippedOutTimeTo.SelectedDateTime = DateTime.Now;
            m_commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            m_queryRequest = new SOOutStockQueryFilter();
            this.SearchCondition.DataContext = m_queryRequest;
            BindComboBoxData();
        }
        //查询
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            orderList.Add(new ValidationEntity(ValidationEnum.IsInteger, null, ResSOLogistics.Msg_SoNoIsInteger));
            if (!ValidationHelper.Validation(this.txtSOSysNo, orderList))
                return;
            this.dgQueryResult.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOOutStock(m_queryRequest, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    this.dgQueryResult.TotalCount = args.Result.TotalCount;
                    this.dgQueryResult.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
                });
        }

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
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
            ColumnSet col = new ColumnSet(dgQueryResult);
            facade.ExportOutStock(m_queryRequest, new ColumnSet[] { col });
        }
        //绑定下拉框数据
        private void BindComboBoxData()
        {
            //是否增票
            this.cmbIsVAT.ItemsSource = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsVAT.SelectedIndex = 0;
            //是否大件
            this.cmbIsBig.ItemsSource = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsBig.SelectedIndex = 0;
            //是否已打包裹单
            this.cmbIsPackaged.ItemsSource = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsPackaged.SelectedIndex = 0;
            //是否VIP客户
            this.cmbIsVIPCustomer.ItemsSource = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsVIPCustomer.SelectedIndex = 0;
            //是否特殊订单
            this.cmbIsSpecialOrder.ItemsSource = EnumConverter.GetKeyValuePairs<SOIsSpecialOrder>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsSpecialOrder.SelectedIndex = 1;
            //是否当前数据
            this.cmbIsCurrentData.ItemsSource = EnumConverter.GetKeyValuePairs<SOIsCurrentData>(EnumConverter.EnumAppendItemType.None);
            this.cmbIsCurrentData.SelectedIndex = 0;
            //是否启用客户首次该配送方式过滤
            this.cmbIsFirstDeliveryFilter.ItemsSource = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.None);
            this.cmbIsFirstDeliveryFilter.SelectedIndex = 1;
        }

        /// <summary>
        /// 查看对应的订单信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtn_SOID_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hlbtn = sender as HyperlinkButton;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, hlbtn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        //处理 配送员 选择为 所有 时的情况
        private void cmbDeliveryPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (string.IsNullOrEmpty(cb.SelectedValue.ToString()))
                this.m_queryRequest.DeliveryPsersonNo = null;
        }
    }
}
