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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class BasketQuery : PageBase
    {
        public PurchaseOrderBasketQueryVM queryVM;
        public PurchaseOrderBasketQueryFilter queryFilter;
        public PurchaseOrderBasketFacade serviceFacade;
        List<CodeNamePair> m_purchaseOrderCompanyMappingDefaultStock;
        string fileIdentity = string.Empty;
        int failedCount = 0;
        dynamic failedData = null;

        public string FileIdentity
        {
            get { return fileIdentity; }
            set { fileIdentity = value; }
        }
        public BasketQuery()
        {
            InitializeComponent();
        }
        public List<KeyValuePair<YNStatus?, string>> IsTransferDataSource;

        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new PurchaseOrderBasketQueryVM();
            serviceFacade = new PurchaseOrderBasketFacade(this);
            queryFilter = new PurchaseOrderBasketQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };
            this.DataContext = queryVM;

            LoadComboBoxData();
            base.OnPageLoad(sender, e);
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //导入采购篮数据:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Basket_ImportBasket))
            {
                this.btnBatchImportBasket.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            IsTransferDataSource = new List<KeyValuePair<YNStatus?, string>>();
            IsTransferDataSource = EnumConverter.GetKeyValuePairs<YNStatus>();
            //直送仓库默认选择仓
            CodeNamePairHelper.GetList(ConstValue.DomainName_PO, ConstValue.PO_KeyPurchaseOrderCompanyMappingDefaultStock, (o, r) =>
            {
                if (r.FaultsHandle()) return;
                m_purchaseOrderCompanyMappingDefaultStock = r.Result;
            });
        }

        #region [Events]

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PageInfo.SortBy = e.SortField;
            string giftMsg = string.Empty;
            queryFilter.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
            //查询采购篮列表:
            serviceFacade.QueryBasketList(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                serviceFacade.QueryBasketTargetWarehouseList((obj1, args1) =>
                {
                    if (args1.FaultsHandle())
                    {
                        return;
                    }
                    var basketList = args.Result.Rows;
                    List<BasketItemsInfoVM> listVM = DynamicConverter<BasketItemsInfoVM>.ConvertToVMList(basketList);
                    listVM.ForEach(x =>
                    {
                        x.IsTransferData = IsTransferDataSource;
                        x.TargetStockList = args1.Result;
                    });

                    if (listVM.Count > 0)
                    {
                        List<int> psysno = new List<int>();
                        foreach (BasketItemsInfoVM bitem in listVM)
                        {
                            psysno.Add(bitem.ProductSysNo.Value);
                        }
                        psysno.Distinct();
                        //查询采购蓝中item的赠品，验证赠品数量是否合主商品相等
                        serviceFacade.GetGiftBasketItems(psysno, (obj2, args2) =>
                        {

                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            List<BasketItemsInfoVM> giftResultList = EntityConverter<BasketItemsInfo, BasketItemsInfoVM>.Convert(args2.Result);
                            #region MyRegion

                            var group = from item in listVM
                                        group item by new { item.StockSysNo }
                                            into g
                                            select new
                                            {
                                                Key = g.Key,
                                                ResultList = g
                                            };
                            foreach (var item in group)
                            {
                                var newbasketItem = from i in item.ResultList
                                                    where i.Quantity.ToInteger() >= 0
                                                    select i
                                                    ;

                                if (giftResultList != null && giftResultList.Count > 0)
                                {
                                    foreach (BasketItemsInfoVM giftentity in giftResultList)
                                    {
                                        int masterQty = 0;
                                        int giftQty = 0;
                                        string stockName = string.Empty;

                                        #region MyRegion
                                        foreach (var bitem in newbasketItem)
                                        {
                                            if (giftentity.MasterProductSysNo == bitem.ProductSysNo)
                                            {
                                                masterQty = bitem.Quantity.ToInteger();
                                                stockName = bitem.StockName;
                                                if (giftQty != 0)
                                                {
                                                    foreach (var bi in newbasketItem)
                                                    {
                                                        if (bi.ProductSysNo == giftentity.GiftSysNo)
                                                        {
                                                            bi.Quantity = (bi.Quantity.ToInteger() - masterQty).ToString();
                                                        }
                                                    }
                                                }
                                            }
                                            if (giftentity.GiftSysNo == bitem.ProductSysNo)
                                            {
                                                giftQty += bitem.Quantity.ToInteger();
                                                stockName = bitem.StockName;
                                                if (masterQty != 0)
                                                {
                                                    bitem.Quantity = (bitem.Quantity.ToInteger() - masterQty).ToString();
                                                }
                                            }
                                        }
                                        if (masterQty != giftQty && masterQty > giftQty)
                                        {
                                            giftMsg += "目标分仓：" + stockName + " 采购" + giftentity.MasterProductSysNo + "商品" + masterQty + "件， 赠品" + giftentity.GiftSysNo
                                                + "商品" + giftQty + "件（" + giftQty + "<" + masterQty + "），赠品不足." + Environment.NewLine;
                                        }
                                        this.spBasicInfo.Visibility = System.Windows.Visibility.Visible;
                                        this.lblInfoText.Text = giftMsg;
                                        this.hlbtnDownloadFailedData.Visibility = System.Windows.Visibility.Collapsed;
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                            int totalCount = args.Result.TotalCount;
                            QueryResultGrid.TotalCount = totalCount;
                            QueryResultGrid.ItemsSource = listVM;
                        });
                    }
                    else
                    {
                        int totalCount = args.Result.TotalCount;
                        QueryResultGrid.TotalCount = totalCount;
                        QueryResultGrid.ItemsSource = listVM;
                    }
                });
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            queryFilter = EntityConverter<PurchaseOrderBasketQueryVM, PurchaseOrderBasketQueryFilter>.Convert(queryVM);
            this.QueryResultGrid.Bind();
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != this.QueryResultGrid.ItemsSource)
            {
                foreach (var item in this.QueryResultGrid.ItemsSource)
                {
                    var infoVm = item as BasketItemsInfoVM;
                    if (infoVm != null)
                    {
                        infoVm.IsChecked = chk.IsChecked == true;
                    }
                }
            }
        }

        private void btnCreatePO_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            #endregion
            this.spBasicInfo.Visibility = this.gridFailedInfo.Visibility = System.Windows.Visibility.Collapsed;
            //创建PO单：
            int totalCheckCount = 0;
            if (!CheckHasSelectedItem(out totalCheckCount))
            {
                return;
            }
            if (totalCheckCount > 80)
            {
                Window.Alert(ResBasketQuery.ErrorMsg_ProductsMaxCount);
                return;
            }

            if (null != QueryResultGrid.ItemsSource)
            {
                List<BasketItemsInfoVM> vmList = this.QueryResultGrid.ItemsSource as List<BasketItemsInfoVM>;
                vmList = vmList.Where(x => x.IsChecked).ToList();

                #region Check操作
                //供应商必须有
                if (vmList.FirstOrDefault(p => !p.VendorSysNo.HasValue) != null)
                {
                    Window.Alert(ResBasketQuery.ErrorMsg_NotVendor);
                    return;
                }
                //中转必须一致
                if (vmList.GroupBy(p => p.IsTransfer).Count() > 1)
                {
                    Window.Alert(ResBasketQuery.ErrorMsg_IsNotTransfer);
                    return;
                }
                //目标仓库
                if (vmList.GroupBy(p => p.StockSysNo).Count() > 1)
                {
                    Window.Alert(ResBasketQuery.ErrorMsg_IsNotStock);
                    return;
                }
                //是否代销必须一致
                if (vmList.GroupBy(p => p.IsConsign).Count() > 1)
                {
                    Window.Alert(ResBasketQuery.ErrorMsg_IsNotConsign);
                    return;
                }
                //是否同一供应商
                if (vmList.GroupBy(p => p.VendorSysNo).Count() > 1)
                {
                    Window.Alert(ResBasketQuery.ErrorMsg_IsNotVendor);
                    return;
                }
                //是否同一PM
                if (vmList.GroupBy(p => p.PMSysNo).Count() > 1)
                {
                    Window.Alert(ResBasketQuery.ErrorMsg_IsNotPM);
                    return;
                }
                string errorMsg = string.Empty;

                foreach (BasketItemsInfoVM obj in vmList)
                {
                    //检查PM 信息
                    if (!obj.PMSysNo.HasValue)
                    {
                        errorMsg += string.Format(ResBasketQuery.ErrorMsg_PMInfoNotExist, obj.ProductSysNo);
                        break;
                    }
                    //检查产品是否已经存在:
                    if (vmList.Where(x => x.ProductSysNo == obj.ProductSysNo && x.ItemSysNo != obj.ItemSysNo).ToList().Count > 0)
                    {
                        errorMsg += string.Format(ResBasketQuery.ErrorMsg_ProductExist, obj.ProductSysNo);
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    Window.Alert(errorMsg);
                    return;
                }

                #endregion

                Window.Confirm(ResBasketQuery.ConfirmMsg_AddProduct, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        //构建新的PurchaseOrderInfo:
                        PurchaseOrderInfoVM newPOInfoVM = new PurchaseOrderInfoVM();
                        newPOInfoVM.PurchaseOrderBasicInfo.CurrencyCode = "1";
                        newPOInfoVM.PurchaseOrderBasicInfo.TaxRate = ((decimal)PurchaseOrderTaxRate.Percent017) / 100;
                        newPOInfoVM.PurchaseOrderBasicInfo.TaxRateType = PurchaseOrderTaxRate.Percent017;

                        var isTransfer = vmList[0].IsTransfer == YNStatus.Yes;
                        if (isTransfer)
                        {
                            newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo = 50;
                            newPOInfoVM.PurchaseOrderBasicInfo.ITStockSysNo = vmList[0].StockSysNo;
                        }
                        else
                        {
                            newPOInfoVM.PurchaseOrderBasicInfo.StockSysNo = vmList[0].StockSysNo;
                        }

                        newPOInfoVM.PurchaseOrderBasicInfo.ConsignFlag = PurchaseOrderConsignFlag.UnConsign;//不代销
                        newPOInfoVM.PurchaseOrderBasicInfo.SettleCompanySysNo = 3201;
                        newPOInfoVM.PurchaseOrderBasicInfo.SettleCompanyName = ResBasketQuery.Label_DefaultSettleCompany;
                        newPOInfoVM.PurchaseOrderBasicInfo.PurchaseOrderType = PurchaseOrderType.Normal;
                        newPOInfoVM.PurchaseOrderBasicInfo.CurrencyCode = "1";
                        newPOInfoVM.PurchaseOrderBasicInfo.CurrencySymbol = "￥";
                        //送货类型默认为“厂方直送”:
                        newPOInfoVM.PurchaseOrderBasicInfo.ShippingTypeSysNo = "12";

                        int? getVendorSysNo = null;
                        int selectIndex = 0;
                        newPOInfoVM.POItems = new List<PurchaseOrderItemInfoVM>();
                        foreach (BasketItemsInfoVM item in vmList)
                        {
                            PurchaseOrderItemInfoVM itemVM = new PurchaseOrderItemInfoVM()
                            {
                                CurrencyCode = newPOInfoVM.PurchaseOrderBasicInfo.CurrencyCode.ToNullableToInteger(),
                                OrderPrice = item.OrderPrice,
                                ProductSysNo = item.ProductSysNo,
                                Quantity = string.IsNullOrEmpty(item.Quantity) ? 0 : Convert.ToInt32(item.Quantity),
                                ItemSysNo = item.ItemSysNo,
                                ReadyQuantity = item.ReadyQuantity
                            };
                            newPOInfoVM.POItems.Add(itemVM);
                            if (0 == selectIndex)
                            {
                                getVendorSysNo = item.VendorSysNo;
                            }
                            selectIndex++;
                            newPOInfoVM.PurchaseOrderBasicInfo.PMSysNo = item.PMSysNo.Value.ToString();
                        }

                    
                        //供应商信息:
                        newPOInfoVM.VendorInfo = new VendorInfoVM();
                        newPOInfoVM.VendorInfo.SysNo = getVendorSysNo;

                        Window.Navigate("/ECCentral.Portal.UI.PO/PurchaseOrderNew", newPOInfoVM, true);
                    }
                });
            }
        }

        private void btnBatchCreatePO_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            #endregion
            this.spBasicInfo.Visibility = this.gridFailedInfo.Visibility = System.Windows.Visibility.Collapsed;
            //批量创建PO单:
            int totalCheckCount = 0;
            if (!CheckHasSelectedItem(out totalCheckCount))
            {
                return;
            }
            Window.Confirm(ResBasketQuery.ConfirmMsg_CreatePO, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<BasketItemsInfoVM> vmList = this.QueryResultGrid.ItemsSource as List<BasketItemsInfoVM>;
                    if (null != vmList)
                    {
                        vmList = vmList.Where(x => x.IsChecked == true).ToList();
                        //取是否高级权限,用于验证产品线
                        bool tIsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                        vmList.ForEach(x => x.IsManagerPM = tIsManagerPM);
                        serviceFacade.BatchCreatePO(EntityConverter<List<BasketItemsInfoVM>, List<BasketItemsInfo>>.Convert(vmList), (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            string tResultStr = string.Empty;
                            string tPOsStr = string.Empty;
                            if (args2.Result.SucessPOSysNos.Count > 0)
                            {
                                tPOsStr = string.Join(".", args2.Result.SucessPOSysNos.ToArray());
                                tResultStr = ResBasketQuery.InfoMsg_CreatePOSuccess + tPOsStr + Environment.NewLine + Environment.NewLine + args2.Result.ErrorMsg;
                            }
                            else
                            {
                                tResultStr = args2.Result.ErrorMsg;
                            }
                            //跳转到采购单查询页面:
                            Window.Alert("提示", tResultStr, MessageType.Information, (obj3, args3) =>
                            {
                                if (args3.DialogResult == DialogResultType.Cancel)
                                {
                                    if (!tPOsStr.Equals(string.Empty))
                                        Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderQuery/?POSysNo={0}", tPOsStr), true);
                                }
                            });
                        });
                    }
                }
            });
        }

        private void btnAddProducts_Click(object sender, RoutedEventArgs e)
        {
            //添加商品:
            //跳转到Inventory备货中心页面 :
            Window.Navigate("/ECCentral.Portal.UI.Inventory/TransferStockingCenter", null, true);
        }

        private void btnAddGift_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            #endregion
            this.spBasicInfo.Visibility = this.gridFailedInfo.Visibility = System.Windows.Visibility.Collapsed;
            //批量添加赠品:
            int totalCheckCount = 0;
            if (!CheckHasSelectedItem(out totalCheckCount))
            {
                return;
            }
            Window.Confirm(ResBasketQuery.ConfirmMsg_AddGift, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<BasketItemsInfoVM> vmList = this.QueryResultGrid.ItemsSource as List<BasketItemsInfoVM>;
                    if (null != vmList)
                    {
                        vmList = vmList.Where(x => x.IsChecked == true).ToList();
                        serviceFacade.BatchAddGiftForBasket(EntityConverter<List<BasketItemsInfoVM>, List<BasketItemsInfo>>.Convert(vmList), (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResBasketQuery.InfoMsg_OperationSuccess);
                            RefreshPage();
                            return;
                        });
                    }
                }
            });
        }

        private void btnBatchImportBasket_Click(object sender, RoutedEventArgs e)
        {
            //批量导入采购篮操作:
            BasketImportDataView importDialog = new BasketImportDataView();
            importDialog.Dialog = Window.ShowDialog(ResBasketQuery.Title_BatchImportBasket, importDialog, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.FileIdentity = importDialog.FileIdentity;
                    var data = args.Data as dynamic;
                    failedData = data[0];
                    this.spBasicInfo.Visibility = System.Windows.Visibility.Visible;
                    this.lblInfoText.Text = string.Format("提示信息：导入成功{0}条，导入失败{1}条", data[1].Rows[0]["successCount"].ToString(), data[1].Rows[0]["failedCount"].ToString());
                    this.failedCount = int.Parse(data[1].Rows[0]["failedCount"].ToString());
                    if (failedCount > 0)
                    {
                        this.hlbtnDownloadFailedData.Visibility = System.Windows.Visibility.Visible;
                    }
                    this.QueryResultGrid.Bind();
                    this.gridFailedInfo.Bind();
                }
            }, new Size(600, 150));
        }

        private void btnBatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }
            #endregion
            this.spBasicInfo.Visibility = this.gridFailedInfo.Visibility = System.Windows.Visibility.Collapsed;
            //批量更新:
            int totalCheckCount = 0;
            if (!CheckHasSelectedItem(out totalCheckCount))
            {
                return;
            }
            Window.Confirm(ResBasketQuery.ConfirmMsg_SaveBasket, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<BasketItemsInfoVM> vmList = this.QueryResultGrid.ItemsSource as List<BasketItemsInfoVM>;
                    if (null != vmList)
                    {
                        vmList = vmList.Where(x => x.IsChecked == true).ToList();
                        serviceFacade.BatchUpdateBasketItems(EntityConverter<List<BasketItemsInfoVM>, List<BasketItemsInfo>>.Convert(vmList), (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResBasketQuery.InfoMsg_SaveSuccess);
                            RefreshPage();
                            return;
                        });
                    }

                }

            });
        }

        private void btnRemoveProducts_Click(object sender, RoutedEventArgs e)
        {
            //删除商品:
            this.spBasicInfo.Visibility = this.gridFailedInfo.Visibility = System.Windows.Visibility.Collapsed;
            int totalCheckCount = 0;
            if (!CheckHasSelectedItem(out totalCheckCount))
            {
                return;
            }
            Window.Confirm(ResBasketQuery.ConfirmMsg_DeleteProduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<BasketItemsInfoVM> vmList = this.QueryResultGrid.ItemsSource as List<BasketItemsInfoVM>;
                    if (null != vmList)
                    {
                        vmList = vmList.Where(x => x.IsChecked == true).ToList();
                        serviceFacade.BatchDeleteBasketItems(EntityConverter<List<BasketItemsInfoVM>, List<BasketItemsInfo>>.Convert(vmList), (obj2, args2) =>
                        {
                            if (args2.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResBasketQuery.InfoMsg_DeleteSuccess);
                            RefreshPage();
                            return;
                        });
                    }

                }
            });
        }

        #endregion

        /// <summary>
        /// 检查是否选中至少一个商品
        /// </summary>
        /// <param name="checkCount"></param>
        /// <returns></returns>
        private bool CheckHasSelectedItem(out int checkCount)
        {
            int totalCheckCount = 0;
            if (null != QueryResultGrid.ItemsSource)
            {
                foreach (object ovj in QueryResultGrid.ItemsSource)
                {
                    if (ovj is BasketItemsInfoVM)
                    {
                        if (((BasketItemsInfoVM)ovj).IsChecked == true)
                        {
                            totalCheckCount++;
                        }
                    }

                }
            }
            checkCount = totalCheckCount;
            if (0 >= totalCheckCount)
            {
                Window.Alert(ResBasketQuery.ErrorMsg_NoRecord, MessageType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取当前Grid选中的ItemSysNo
        /// </summary>
        /// <returns></returns>
        private List<int> GetSelectedItemSysNoList()
        {
            List<int> returnList = new List<int>();
            if (null != QueryResultGrid.ItemsSource)
            {
                foreach (object ovj in QueryResultGrid.ItemsSource)
                {
                    if (ovj is BasketItemsInfoVM)
                    {
                        if (((BasketItemsInfoVM)ovj).IsChecked == true)
                        {
                            returnList.Add(((BasketItemsInfoVM)ovj).ItemSysNo.Value);
                        }
                    }
                }
            }
            return returnList;
        }

        private void RefreshPage()
        {
            btnSearch_Click(null, null);
        }

        private void btnChooseVendor_Click(object sender, RoutedEventArgs e)
        {
            UCVendorQuery vendorQueryCtrl = new UCVendorQuery();
            vendorQueryCtrl.Dialog = Window.ShowDialog("选择供应商", vendorQueryCtrl, (obj, args) =>
            {
                BasketItemsInfoVM selectVM = this.QueryResultGrid.SelectedItem as BasketItemsInfoVM;
                if (null != selectVM)
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        DynamicXml getSelectedVendor = args.Data as DynamicXml;
                        if (null != getSelectedVendor)
                        {
                            selectVM.VendorSysNo = Convert.ToInt32(getSelectedVendor["SysNo"].ToString());
                            selectVM.VendorName = getSelectedVendor["VendorName"].ToString();
                            selectVM.PaySettleCompany = (PaySettleCompany)getSelectedVendor["PaySettleCompany"];
                            selectVM.VendorIsConsign = (int)getSelectedVendor["IsConsign"];
                            CheckIsEnabledTransfer(selectVM);
                        }
                    }
                }
            }, new Size(700, 650));
        }

        private void hlbtnDownloadFailedData_Click(object sender, RoutedEventArgs e)
        {
            #region 点击下载失败记录
            //ColumnSet col = new ColumnSet(this.gridFailedInfo);

            //col.Insert(0, "LastVendorSysNo", "供应商编号", 10);
            //col.Insert(1, "ProductID", "商品编号", 10);
            //col.Insert(2, "OrderPrice", "订购价格", 10);
            //col.Insert(3, "StockName", "目标分仓", 10);
            //col.Insert(4, "IsTransfer", "是否中转", 2);
            //col.Insert(5, "ErrorMessage", "错误信息", 20);

            //this.serviceFacade.ExportFailedFile(this.FileIdentity, new ColumnSet[] { col });
            #endregion

            this.gridFailedInfo.Visibility = System.Windows.Visibility.Visible;
        }

        //将下载失败记录改为导出当前页数据 norton.c.li 2012.10.31
        private void gridFailedInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            e.PageSize = int.MaxValue;
            this.gridFailedInfo.TotalCount = failedCount;
            this.gridFailedInfo.ItemsSource = failedData.Rows;
        }

        private void cmbStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //先查询行
            var ckb = (ComboBox)sender;
            var row = UtilityHelper.GetParentObject<DataGridRow>(ckb, "");
            var selectedModel = QueryResultGrid.Columns[0].GetCellContent(row).DataContext as BasketItemsInfoVM;
            if (selectedModel != null && selectedModel.IsChecked)
            {
                CheckIsEnabledTransfer(selectedModel);
            }
        }

        private void itemChecked_Checked(object sender, RoutedEventArgs e)
        {
            var ckb = (CheckBox)sender;
            var row = UtilityHelper.GetParentObject<DataGridRow>(ckb, "");
            var selectedModel = QueryResultGrid.Columns[0].GetCellContent(row).DataContext as BasketItemsInfoVM;
            if (selectedModel != null)
            {
                CheckIsEnabledTransfer(selectedModel);
            }
        }

        private void CheckIsEnabledTransfer(BasketItemsInfoVM selectedModel)
        {
            selectedModel.IsTransfer = YNStatus.NO;
            selectedModel.IsEnabledTransfer = false;
            //selectedModel.IsEnabledTransfer = true;
            //if (selectedModel.VendorSysNo.HasValue)
            //{
            //    //判断当选择仓库后，操作是否中转按钮
            //    var defaultStock = m_purchaseOrderCompanyMappingDefaultStock.FirstOrDefault(p => p.Code == ((int)selectedModel.PaySettleCompany).ToString());
            //    if (defaultStock != null)
            //    {
            //        if (selectedModel.StockSysNo.ToString() == defaultStock.Name)
            //        {
            //            selectedModel.IsTransfer = YNStatus.NO;
            //            selectedModel.IsEnabledTransfer = false;
            //        }
            //    }
            //}
        }
    }
}
