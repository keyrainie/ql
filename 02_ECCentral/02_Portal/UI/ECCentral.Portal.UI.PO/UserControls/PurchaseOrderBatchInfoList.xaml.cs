using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderBatchInfoList : UserControl
    {

        public PurchaseOrderFacade serviceFacade;

        public IDialog Dialog { get; set; }
        public int? ProductSysNo { get; set; }
        public int? ItemSysNo { get; set; }
        public int? StockSysNo { get; set; }
        public string batchInfo = null;
        public PurchaseOrderBatchNumberQueryFilter queryFilter { get; set; }
        public List<PurchaseOrderItemBatchInfoVM> list { get; set; }

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

        public PurchaseOrderBatchInfoList(int itemSysNo, int productSysNo, int stockSysNo,string batchInfo)
        {
            InitializeComponent();
            queryFilter = new PurchaseOrderBatchNumberQueryFilter();
            list = new List<PurchaseOrderItemBatchInfoVM>();
            this.ItemSysNo = itemSysNo;
            this.ProductSysNo = productSysNo;
            this.StockSysNo = stockSysNo;
            this.batchInfo = batchInfo;
            this.Loaded += new RoutedEventHandler(PurchaseOrderBatchInfoList_Loaded);
        }

        void PurchaseOrderBatchInfoList_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderBatchInfoList_Loaded;
            serviceFacade = new PurchaseOrderFacade(CurrentPage);
            this.gridBatchNumberInfoList.Bind();
        }

        private void gridBatchNumberInfoList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            queryFilter.ProductSysNo = ProductSysNo;
            queryFilter.StockSysNo = StockSysNo;
            serviceFacade.QueryPurchaseOrderBatchNumberList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                list = DynamicConverter<PurchaseOrderItemBatchInfoVM>.ConvertToVMList(args.Result.Rows);
                //填充退货数量
                if (!string.IsNullOrEmpty(batchInfo))
                {
                    var strs = batchInfo.Split(new char[] { ';' });
                    for (var i = 0; i < strs.Length; i++)
                    {
                        var str = strs[i].Split(new char[] { ':' });
                        if (str.Length == 3)
                        {
                            foreach (PurchaseOrderItemBatchInfoVM tVm in list)
                                if (tVm.BatchInfoNumber == str[0] && tVm.StockSysNo.Value.ToString() == str[1])
                                {
                                    string tStr = string.IsNullOrEmpty(str[2]) ? "0" : str[2];
                                    tVm.ReturnQty = Convert.ToInt32(tStr);
                                }
                        }

                    }
                }

                this.gridBatchNumberInfoList.ItemsSource = list;
            });
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //验证退货数量是否大于最大值
            var tItems = (from tObj in list
                          where tObj.ReturnQty > tObj.BatchInfoStockNumber||tObj.ReturnQty<0
                          select tObj).ToList();
            if (tItems != null && tItems.Count > 0)
            {
                labErrorMsg.Visibility = Visibility.Visible;
                labErrorMsg.Text = "退货数量应为0到库存数量之间的整数！";
                return;
            }
            else
            {
                labErrorMsg.Visibility = Visibility.Collapsed;
                labErrorMsg.Text = "";
            }
            // 更新操作:
            PurchaseOrderItemInfo info = new PurchaseOrderItemInfo()
            {
                ItemSysNo = this.ItemSysNo,
                BatchInfo = BuildItemBatchInfo()
            };
            if (!string.IsNullOrEmpty(info.BatchInfo))
            {
                serviceFacade.UpdatePurchaseOrderBatchInfo(info, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    batchInfo = info.BatchInfo;
                    Dialog.ResultArgs.Data = batchInfo;
                    Dialog.Close(true);
                });
            }
            else
            {
                Dialog.Close(true);
            }
        }
        private string BuildItemBatchInfo()
        {
            string returnString = string.Empty;
            if (null != list && 0 < list.Count)
            {
                list.ForEach(x =>
                {
                    if (x.ReturnQty.HasValue && x.ReturnQty>0)
                    {
                         returnString += string.Format("{0}:{1}:{2};", x.BatchInfoNumber, x.StockSysNo, x.ReturnQty);
                    }
                   
                });
            }
            return returnString;
        }
    }
}
