using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class SOOutStockSearch4Finance : PageBase
    {

        SOOutStock4FinanceQueryFilter m_queryRequest;
        CommonDataFacade m_commonDataFacade;

        public SOOutStockSearch4Finance()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            m_commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            this.SeachBuilder.DataContext = m_queryRequest = new SOOutStock4FinanceQueryFilter();
            BindComboBoxData();
        }

        //搜索
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PageInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            m_queryRequest.CompanyCode = CPApplication.Current.CompanyCode;
            m_queryRequest.ReceiveAreaSysNo = ucReceiveArea.QueryAreaSysNo;
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QueryOutStock4Finance(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                QueryResultGrid.TotalCount = args.Result.TotalCount;
                QueryResultGrid.ItemsSource = args.Result.Rows;
            });
        }

        //绑定下拉菜单数据
        private void BindComboBoxData()
        {
            //分仓
            m_commonDataFacade.GetStockList(true, (sender, e) =>
            {
                if (e.FaultsHandle()) return;
                cmbStock.ItemsSource = e.Result;
                if (e.Result.Count > 0)
                {
                    cmbStock.SelectedIndex = 0;
                }
            });

            // 读取配置 初始化配送 时间范围
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_TimeRange, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbDeliveryRange.ItemsSource = e.Result;
                }
            });

            //是否特殊订单
            this.cmbIsSpecialOrder.ItemsSource = EnumConverter.GetKeyValuePairs<SOIsSpecialOrder>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsSpecialOrder.SelectedIndex = 1;

            //库存状态
            this.cmbEnoughFlag.ItemsSource = EnumConverter.GetKeyValuePairs<EnoughFlag>(EnumConverter.EnumAppendItemType.All);
            this.cmbEnoughFlag.SelectedIndex = 0;

            //是否赠票
            this.cmbIsVAT.ItemsSource = EnumConverter.GetKeyValuePairs<BooleanType>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsVAT.SelectedIndex = 0;

            //送货方式条件
            this.cmbShipTypeCondition.ItemsSource = EnumConverter.GetKeyValuePairs<ConditionType>(EnumConverter.EnumAppendItemType.None);
            this.cmbShipTypeCondition.SelectedIndex = 0;
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, info["Soid"]), null, true);
            }
        }
    }
}
