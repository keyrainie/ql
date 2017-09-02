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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ProductCostMaintain : PageBase
    {

        public ProductCostQueryVM queryVM;
        public ProductCostQueryFilter queryFilter;
        public ProductCostQueryFacade serviceFacade;

        private DataGridColumn stockNameColumn;
        public DataGridColumn StockNameColumn
        {
            get
            {
                if (stockNameColumn == null)
                {
                    foreach (var col in dgProductCostQueryResult.Columns)
                    {
                        if (col.GetBindingPath().ToUpper().IndexOf("STOCKNAME") > 0)
                        {
                            stockNameColumn = col;
                            break;
                        }
                    }
                }

                return stockNameColumn;
            }
        }

        #region 初始化加载
        public ProductCostMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryFilter = new ProductCostQueryFilter();
            queryVM = new ProductCostQueryVM();
            serviceFacade = new ProductCostQueryFacade(this);

            string getParam = this.Request.Param;
            if (!string.IsNullOrEmpty(getParam))
            {
                queryVM.ProductSysNo = Convert.ToInt32(getParam.Trim());
                this.DataContext = queryVM;
                btnSearch_Click(null, null);
            }

            else
            {
                this.DataContext = queryVM;
            }

        }

        #endregion

        #region 页面内按钮处理事件

        private void dgProductCostResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

            queryFilter = EntityConverter<ProductCostQueryVM, ProductCostQueryFilter>.Convert(queryVM);
            this.queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo();

            this.queryFilter.PagingInfo.PageIndex = e.PageIndex;
            this.queryFilter.PagingInfo.PageSize = e.PageSize;
            this.queryFilter.PagingInfo.SortBy = e.SortField;

            #region 获取自己能访问到的PM
            //queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
            queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            serviceFacade.QueryProductCostInList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dgProductCostQueryResult.TotalCount = args.Result.TotalCount;
                dgProductCostQueryResult.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
            });

            #endregion

        }
      

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.queryFilter = EntityConverter<ProductCostQueryVM, ProductCostQueryFilter>.Convert(this.queryVM);
            if (queryFilter==null||!queryFilter.ProductSysNo.HasValue)
            {
                Window.Alert(string.Format("请输入商品编号"));
                return;
            }
            this.dgProductCostQueryResult.Bind();
        }

        private void btnBatchSave_Click(object sender, RoutedEventArgs e)
        {
            List<ProductCostInfo> list = new List<ProductCostInfo>();
            if (LoadSelect(list))
            {
                //设置库存成本优先级
                serviceFacade.BatchUpdateProductCostPriority(list, (up, ar) =>
                {
                    if (ar.FaultsHandle())
                    {
                        Window.Alert(string.Format("更新失败！"));
                    }
                    else
                    {
                        Window.Alert(string.Format("更新成功！"));
                        this.dgProductCostQueryResult.Bind();
                    }
                });
            }
        }

        private bool LoadSelect(List<ProductCostInfo> list)
        {
            var dynamic = this.dgProductCostQueryResult.ItemsSource as dynamic;
            if (dynamic != null)
            {
                int proPriority = 0;
                foreach (var item in dynamic)
                {
                    if (item.IsCheck)
                    {
                        proPriority = GetValidatedNumber(item["Priority"].ToString());

                        if (proPriority>0&&proPriority<=9999)
                        {
                            list.Add
                            (
                                new ProductCostInfo
                                {
                                    SysNo = (int)item["SysNo"],
                                    Priority = proPriority
                                }
                            );
                        }
                        else
                        {
                            this.Window.Alert(ResProductCostMaintain.Msg_InputInteger);
                            return false;
                        }
                    }
                }
            }

            if (list.Count == 0)
            {
                this.Window.Alert(ResProductCostMaintain.Msg_PleaseSelect);
                return false;
            }
            return true;
        }

        private int GetValidatedNumber(string inputPriority)
        {
            int itemPriority = 0;
            int.TryParse(inputPriority, out itemPriority);
            return itemPriority;
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dgProductCostQueryResult.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }
        #endregion

    }
}
