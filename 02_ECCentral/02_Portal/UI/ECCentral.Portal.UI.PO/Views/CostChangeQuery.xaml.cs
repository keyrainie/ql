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
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CostChangeQuery : PageBase
    {
        public CostChangeQueryVM queryVM;
        public CostChangeQueryFilter queryFilter;
        public CostChangeFacade serviceFacade;
        //public VendorFacade vendorFacade;
        public string CostChangeSysNo;

        public CostChangeQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new CostChangeQueryVM();
            serviceFacade = new CostChangeFacade(this);
            //vendorFacade = new VendorFacade(this);

            queryFilter = new CostChangeQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };

            this.DataContext = queryVM;
            LoadComboBoxData();
            base.OnPageLoad(sender, e);

            //if (null != this.Request.QueryString)
            //{
            //    if (this.Request.QueryString.Keys.Contains("ProductSysNo"))
            //    {
            //        queryVM.ProductSysNo = this.Request.QueryString["ProductSysNo"];
            //    }
            //    if (this.Request.QueryString.Keys.Contains("QueryStatus"))
            //    {
            //        queryVM.StatusList = this.Request.QueryString["QueryStatus"];
            //    }
            //    if (this.Request.QueryString.Keys.Contains("POSysNo"))
            //    {
            //        POSysNo = this.Request.QueryString["POSysNo"];
            //        queryVM.POSysNoExtention = POSysNo;
            //    }
            //    btnSearch_Click(null, null);
            //}
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //查询成本变价单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Query))
            {
                this.btnSearch.IsEnabled = false;
            }
            //创建成本变价单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CostChange_Create))
            {
                this.btnCreateCostChange.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            //成本变价单状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<CostChangeStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
        }

        #region [Events]

        private void btnCreateCostChange_Click(object sender, RoutedEventArgs e)
        {
            //成本变价单 - 操作:
            Window.Navigate("/ECCentral.Portal.UI.PO/CostChangeNew", null, true);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            queryFilter = EntityConverter<CostChangeQueryVM, CostChangeQueryFilter>.Convert(queryVM);

            ////TODO:权限管理，这里给查询赋予最高权限:
            //queryFilter.PMQueryType = PMQueryType.All;

            this.QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };

            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryCostChange(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                var list = args.Result.Rows.ToList();
                //构造状态列的显示
                foreach (var item in list)
                {
                    if (item["Status"] != null )
                    {
                        item["StatusDisplay"] = EnumConverter.GetDescription(item["Status"], typeof(CostChangeStatus));
                    }
                    else
                    {
                        item["StatusDisplay"] = string.Empty;
                    }
                }
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = list;
            });
        }

        #endregion

        /// <summary>
        /// 编辑成本变价单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_EditCostChangeItem_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/CostChangeMaintain/{0}", getSelectedItem["SysNo"].ToString()), null, true);
        }

    }
}
