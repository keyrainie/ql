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
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using System.Windows.Data;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.BizEntity.SO;
namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SplitInvoice : UserControl
    {
        #region  页面初始化
        SOQueryFacade SOQueryFacade = null;
        InvoiceSplitView PageView = null;

        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page == null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private IPage Page
        {
            get;
            set;
        }

        SOFacade SOFacade;
        int m_soSysNo;
        SOVM m_soVM;

        public SplitInvoice(IPage page, SOVM so)
        {
            PageView = new InvoiceSplitView();
            this.Page = page;
            m_soSysNo = so.SysNo.Value;
            PageView.SOBaseInfo = so.BaseInfoVM;
            InitializeComponent();
            Loaded += new RoutedEventHandler(SplitInvoice_Loaded);
            this.DataContext = PageView;
        }

        void SplitInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            SOQueryFacade = new SOQueryFacade(Page);
            SOFacade = new SOFacade(Page);
            int totalQty = 0;
            //need reload sovm
            SOQueryFacade.QuerySOInfo(m_soSysNo, vm =>
            {
                if (vm == null)
                {
                    Window.Alert(ResSO.Info_SOIsNotExist, ResSO.Info_SOIsNotExist, MessageType.Warning, (obj, args) =>
                    {
                        Window.Close();
                    });
                    return;
                }
                m_soVM = vm;
                m_soVM.ItemsVM.ForEach(soItem =>
                {
                    if (soItem.ProductType != SOProductType.Coupon)
                    {
                        if (soItem.ProductType != SOProductType.ExtendWarranty)
                        {
                            totalQty += soItem.Quantity.Value;
                        }

                        InvoiceProductVM invoiceProductVM = new InvoiceProductVM
                        {
                            InvoiceNo = 1,
                            Price = soItem.Price.Value,
                            ProductID = soItem.ProductID,
                            ProductName = soItem.ProductName,
                            ProductSysNo = soItem.ProductSysNo.Value,
                            Quantity = soItem.Quantity.Value,
                            StockSysNo = soItem.StockSysNo,
                            InvoiceQuantity = soItem.Quantity.Value,
                            IsExtendWarrantyItem = soItem.ProductType == SOProductType.ExtendWarranty
                        };

                        string[] mpSysNo = null;
                        if (soItem.MasterProductSysNo != null)
                        {
                            mpSysNo = soItem.MasterProductSysNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }

                        if (mpSysNo != null)
                        {
                            invoiceProductVM.MasterProductSysNo = invoiceProductVM.MasterProductSysNo ?? new List<int>();
                            mpSysNo.ForEach(n =>
                            {
                                int no; if (int.TryParse(n, out no))
                                {
                                    invoiceProductVM.MasterProductSysNo.Add(no);
                                }
                            });
                        }
                        PageView.OriginalProductList.Add(invoiceProductVM);
                    }
                });
                SOFacade.GetSOInvoiceList(m_soSysNo, PageView.OriginalProductList, (isSplited, invoiceList) =>
                {
                    if (isSplited)
                    {
                        PageView.SplitedProductList = invoiceList;
                        btnSave.IsEnabled = false;
                        btnCancelSplit.IsEnabled = true;
                    }
                    else
                    {
                        PageView.SplitedProductList = PageView.OriginalProductList.DeepCopy();
                        ECCentral.BizEntity.SO.SOStatus soStatus = m_soVM.BaseInfoVM.Status.Value;
                        btnSave.IsEnabled = totalQty > 1 &&
                            (soStatus == ECCentral.BizEntity.SO.SOStatus.Origin ||
                            soStatus == ECCentral.BizEntity.SO.SOStatus.WaitingOutStock ||
                            soStatus == ECCentral.BizEntity.SO.SOStatus.Shipping);
                        btnCancelSplit.IsEnabled = false;
                    }
                    btnGroupSort.IsEnabled = btnSave.IsEnabled;
                    SortInvoice();
                });
            });
            
        }
        #endregion

        #region 拆分发票事件 处理
        private void hlbtnSplit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            InvoiceProductVM data = btn.DataContext as InvoiceProductVM;

            if (data.InvoiceQuantity > 1) //如果商品的数量>1，则将此商品拆分到新的发票中
            {
                data.InvoiceQuantity--;
                var numberList = data.InvoiceNumberList;
                int newNo = data.InvoiceNumberList.Count;

                InvoiceProductVM newData = new InvoiceProductVM
                {
                    InvoiceNo = newNo,
                    Price = data.Price,
                    ProductID = data.ProductID,
                    ProductName = data.ProductName,
                    ProductSysNo = data.ProductSysNo,
                    Quantity = data.Quantity,
                    StockSysNo = data.StockSysNo,
                    IsExtendWarrantyItem = data.IsExtendWarrantyItem,
                    MasterProductSysNo = data.MasterProductSysNo,
                    InvoiceQuantity = 1
                };
                PageView.SplitedProductList.Add(newData);
            }
            SortInvoice();
        }

        private void hlbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            InvoiceProductVM data = btn.DataContext as InvoiceProductVM;

            data.InvoiceNo = 1;

            SortInvoice();
        }

        private void btnGroupSort_Click(object sender, RoutedEventArgs e)
        {
            SortInvoice();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SortInvoice())
            {
                SOFacade.SplitInvoice(m_soVM.BaseInfoVM.SysNo.Value, m_soVM.BaseInfoVM.CompanyCode, PageView.SplitedProductList, () =>
                {
                    Page.Context.Window.Alert(ResSO.Info_SplitInvoice_Success);
                    InvoiceIsSplited(true);
                });
            }
        }

        private void btnCancelSplit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.CancelSplitInvoice(m_soSysNo, () =>
            {
                Page.Context.Window.Alert(ResSO.Info_SplitInvoice_CancelSuccess);
                InvoiceIsSplited(false);
            });
        }

        /// <summary>
        /// 为发票排序
        /// </summary>
        private bool SortInvoice()
        {
            bool result = true;
            List<InvoiceProductVM> invoiceProductList = new List<InvoiceProductVM>();

            //合并同一发票中相同的商品
            PageView.SplitedProductList.ForEach(item =>
           {
               InvoiceProductVM oldItem = invoiceProductList.FirstOrDefault(i => item.ProductID == i.ProductID && item.InvoiceNo == i.InvoiceNo);
               if (oldItem == null)
               {
                   if (item.InvoiceQuantity > 0)
                   {
                       invoiceProductList.Add(item);
                   }
               }
               else
               {
                   oldItem.InvoiceQuantity += item.InvoiceQuantity;
               }
           });

            var quantityMoreThanOneList = (from item in PageView.OriginalProductList
                                           where item.Quantity > 1
                                           orderby item.ProductSysNo
                                           select item);
            //相关商品在所有发票中的数量总合小于购买数量则，将剩余的数量添加到发标编号为1的发票中
            quantityMoreThanOneList.ForEach(item =>
            {
                var tList = from tItem in invoiceProductList
                            where tItem.ProductID == item.ProductID
                            select tItem;
                var dItem = tList.FirstOrDefault(tItem => tItem.InvoiceNo == 1);
                var tq = item.Quantity - tList.Sum(tItem => tItem.InvoiceQuantity);
                if (tq > 0)
                {
                    if (dItem == null)
                    {
                        dItem = new InvoiceProductVM
                        {
                            InvoiceNo = 1,
                            Price = item.Price,
                            ProductID = item.ProductID,
                            ProductName = item.ProductName,
                            ProductSysNo = item.ProductSysNo,
                            Quantity = item.Quantity,
                            StockSysNo = item.StockSysNo,
                            IsExtendWarrantyItem = item.IsExtendWarrantyItem,
                            MasterProductSysNo = item.MasterProductSysNo,
                            InvoiceQuantity = tq
                        };
                        invoiceProductList.Add(dItem);
                    }
                    else
                    {
                        dItem.InvoiceQuantity += tq;
                    }
                }
                else if (tq < 0)
                {
                    this.Window.Alert(String.Format(ResSO.Msg_SplitInvoice_ProductQuantityIsError, item.ProductID));
                    result = false;
                    return;
                }
            });
            //按仓库，发票编号排序
            invoiceProductList.Sort((i1, i2) =>
            {
                int i = i1.StockSysNo.Value - i2.StockSysNo.Value;
                i = i == 0 ? i1.InvoiceNo - i2.InvoiceNo : i;
                return i == 0 ? i1.ProductSysNo - i2.ProductSysNo : i;
            });

            //为发票编号从0开始排序.
            Dictionary<int, List<KeyValuePair<int, string>>> stockInvoiceNumberDic = new Dictionary<int, List<KeyValuePair<int, string>>>();

            int oldStockSysNo = invoiceProductList[0].StockSysNo.Value;
            int oldInvoiceNo = invoiceProductList[0].InvoiceNo;
            int invoiceIndex = 1;
            for (int i = 0; i < invoiceProductList.Count; i++)
            {
                InvoiceProductVM item = invoiceProductList[i];
                if (!stockInvoiceNumberDic.ContainsKey(item.StockSysNo.Value))
                {
                    stockInvoiceNumberDic.Add(item.StockSysNo.Value, new List<KeyValuePair<int, string>>());
                    stockInvoiceNumberDic[item.StockSysNo.Value].AddRange(new KeyValuePair<int, string>[] { new KeyValuePair<int, string>(0, "New"), new KeyValuePair<int, string>(1, String.Format("{0}_1", item.StockSysNo)) });
                }
                if (oldStockSysNo == item.StockSysNo)
                {
                    if (oldInvoiceNo != item.InvoiceNo)
                    {
                        oldInvoiceNo = item.InvoiceNo;
                        invoiceIndex++;
                        stockInvoiceNumberDic[item.StockSysNo.Value].Add(new KeyValuePair<int, string>(invoiceIndex, String.Format("{0}_{1}", item.StockSysNo, invoiceIndex)));
                    }
                }
                else
                {
                    oldStockSysNo = item.StockSysNo.Value;
                    oldInvoiceNo = item.InvoiceNo;
                    invoiceIndex = 1;
                }
                item.InvoiceNo = invoiceIndex;
            }
            invoiceProductList.ForEach(item =>
            {
                item.InvoiceNumberList = stockInvoiceNumberDic[item.StockSysNo.Value];
            });
            //保存拆分排序结果
            PageView.SplitedProductList = invoiceProductList;
            return result;
        }

        private void InvoiceIsSplited(bool isSplited)
        {
            PageView.SplitedProductList.ForEach(item =>
            {
                item.IsSplited = isSplited;
            });
            btnCancelSplit.IsEnabled = isSplited;
            btnSave.IsEnabled = !btnCancelSplit.IsEnabled;
            btnGroupSort.IsEnabled = btnSave.IsEnabled;
            PageView.SplitedProductList = PageView.SplitedProductList;
        }
        #endregion
    }
}
