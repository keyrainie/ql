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
using ECCentral.Portal.UI.Inventory.Models.Inventory;
using ECCentral.QueryFilter.Inventory.Request;
using ECCentral.Portal.UI.Inventory.Facades.Request;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class BatchDetailMaintain : PageBase
    {
        protected List<ProductBatchInfoVM> batches;
        protected ProductBatchQueryFilter queryFilter;
        private ProductBatchQueryFacade serviceFacade;
        public List<KeyValuePair<string, string>> BatchStatusDataSource;
        public int ProductSysNo { get; set; }
        public int StockSysNo { get; set; }

        #region 初始化加载
        public BatchDetailMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryFilter = new ProductBatchQueryFilter();
            serviceFacade = new ProductBatchQueryFacade(this);
            BatchStatusDataSource = new List<KeyValuePair<string, string>>() 
            {
                new KeyValuePair<string, string>("A","正常"),
                new KeyValuePair<string, string>("R","临期"),
                new KeyValuePair<string, string>("I","过期"),
            };

            string getParam = this.Request.Param;
            if (!string.IsNullOrEmpty(getParam))
            {
                string[] strPrams = getParam.Split(new char[] { '&' });

                if (strPrams.Length == 2)
                {
                    this.ProductSysNo = Convert.ToInt32(strPrams[0]);
                    this.StockSysNo = Convert.ToInt32(strPrams[1]);
                }
            }
            //this.DataContext = BatchStatusDataSource;
            this.dgBatchDetailQueryResult.Bind();
            //SelectionChanged
            
        }

        #endregion

        #region 页面内按钮处理事件

        private void dgBatchDetailQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            //无参数传入
            if (this.ProductSysNo == 0 || this.StockSysNo == 0)
            {
                dgBatchDetailQueryResult.TotalCount = 0;
                dgBatchDetailQueryResult.ItemsSource = null;
            }
            else
            {
                this.queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo();
                this.queryFilter.PagingInfo.PageIndex = e.PageIndex;
                this.queryFilter.PagingInfo.PageSize = e.PageSize;
                this.queryFilter.PagingInfo.SortBy = e.SortField;

                this.queryFilter.ProductSysNo = this.ProductSysNo;
                this.queryFilter.StockSysNo = this.StockSysNo;

                serviceFacade.QueryProductBatchInfo(queryFilter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    batches = new List<ProductBatchInfoVM>();

                    List<ProductBatchDetailVM> lstPbv = DynamicConverter<ProductBatchDetailVM>.ConvertToVMList(args.Result.Rows);
                    foreach (ProductBatchDetailVM pbv in lstPbv)
                    {
                        pbv.BatchStatusDataSource = BatchStatusDataSource;
                    }

                    dgBatchDetailQueryResult.TotalCount = args.Result.TotalCount;
                    dgBatchDetailQueryResult.ItemsSource = lstPbv;

                });
            }

        }

        private void cmbBatchStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBatchDetailQueryResult.SelectedItem == null)
                return;

            ProductBatchDetailVM current = dgBatchDetailQueryResult.SelectedItem as ProductBatchDetailVM;

            InventoryBatchDetailsInfo productBatchInfo =
                new InventoryBatchDetailsInfo()
                {
                    BatchNumber = current.BatchNumber.ToString(),
                    ProductSysNo = current.ProductSysNo,
                    Status = current.Status
                };

            serviceFacade.UpdateProductBatchStatus(productBatchInfo, (up, ar) =>
            {
                if (ar.FaultsHandle())
                {
                    Window.Alert(string.Format("更新失败！"));
                }
                else
                {
                    Window.Alert(string.Format("更新成功！"));
                    this.dgBatchDetailQueryResult.Bind();
                }
            });
        }
        #endregion

    }
}
