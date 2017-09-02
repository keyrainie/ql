using System;
using System.Windows;
using System.Linq;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.Portal.UI.SO.Resources;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class SpecialSOSearch : PageBase
    {
        CommonDataFacade m_commonDataFacade;
        SpecialSOSearchQueryFilter m_query;

        public SpecialSOSearch()
        {
            InitializeComponent();
            m_commonDataFacade = new Basic.Components.Facades.CommonDataFacade(this);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            SeachBuilder.DataContext = m_query = new SpecialSOSearchQueryFilter();
            IntiPageData();
        }

        private void IntiPageData()
        {
            m_commonDataFacade.GetStockList(true, (sender, e) =>
            {
                if (e.FaultsHandle()) return;
                cmbStockV.ItemsSource = e.Result;
                if (e.Result.Count > 0)
                {
                    cmbStockV.SelectedIndex = 0;
                }
                cmbStockNV.ItemsSource = e.Result;
                if (e.Result.Count > 0)
                {
                    cmbStockNV.SelectedIndex = 0;
                }
            });

            this.cmbSOStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbSOStatus.SelectedIndex = 0;

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //虚库仓和实库仓请选择不同的仓库


            dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PageInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            m_query.CompanyCode = CPApplication.Current.CompanyCode;

            //实仓库和虚仓库不能相同
            if (m_query.StockNV == m_query.StockV) 
            {
                Window.Alert(ResSO.Msg_Error_VAndNVMustDiff);
                return;
            }

            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySpecialSO(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid.TotalCount = args.Result.TotalCount;

                List<object> resultList = new List<object>();
                //去掉重复数据，将同订单ID不同商品ID合并成一条
                List<object> list = args.Result.Rows.ToList();

                var group = list.GroupBy(p => { return (string)((DynamicXml)p)["SOID"]; });

                foreach (var grpItem in group)
                {
                    var addItem = grpItem.ElementAt(0);
                    string productID = string.Join(",", grpItem.Select(p => (string)((DynamicXml)p)["ProductID"]).ToArray());
                    ((DynamicXml)addItem)["ProductID"] = productID;
                    resultList.Add(grpItem.ElementAt(0));
                }

                dataGrid.ItemsSource = resultList;
            });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.dataGrid.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, info["SOSysNo"]), null, true);
            }
        }
    }
}
