using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class CollectionPaymentNew : PageBase
    {

        public CollectionPaymentInfoVM newVM;
        public CollectionPaymentFacade serviceFacade;
       
        public List<CollectionPaymentItemInfoVM> mergedItemList;
        /// <summary>
        /// 结算商品 - 结算单总金额
        /// </summary>
        public decimal SettleProductsTotalAmt = 0;
        /// <summary>
        /// 结算商品 - 使用返利
        /// </summary>
        public decimal SettleProductsTotalReturnPoints = 0;
        /// <summary>
        /// 结算商品 - 合计
        /// </summary>
        public decimal SettleProductsTotalSummary = 0;

        public CollectionPaymentNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            newVM = new CollectionPaymentInfoVM() { CurrencyCode = "1", TaxRateData = PurchaseOrderTaxRate.Percent017 };
            mergedItemList = new List<CollectionPaymentItemInfoVM>();
            serviceFacade = new CollectionPaymentFacade(this);
            InitializeComboBoxData();
            this.DataContext = newVM;
            CalcSettleProducts();
            SetAccessControl();
            //供应商附加选择事件
            ucVendorPicker.VendorSelected += new EventHandler<VendorSelectedEventArgs>(ucVendorPicker_VendorSelected);
        }

    
        private void SetAccessControl()
        {
            btnAddSettleItems.IsEnabled = true;
            btnDeleteSettleItems.IsEnabled = true;
            btnReset.IsEnabled = true;
            btnSave.IsEnabled = true;
        }

        private void InitializeComboBoxData()
        {
            //税率:
            this.cmbTaxRateData.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderTaxRate>(EnumConverter.EnumAppendItemType.Select);
            this.cmbTaxRateData.SelectedIndex = 0;

        }

        /// <summary>
        /// 根据items重新计算总金额，返利和合计:
        /// </summary>
        private void CalcSettleProducts()
        {
            SettleProductsTotalAmt = 0;
            SettleProductsTotalReturnPoints = 0;
            SettleProductsTotalSummary = 0;

            decimal totalCreateCost = 0m;
            decimal totalCountMany = 0m;
            decimal totalDiffAmt = 0m;
            decimal totalMarginRate = 0m;
            if (0 < this.mergedItemList.Count)
            {
                this.mergedItemList.ForEach(delegate(CollectionPaymentItemInfoVM itemVM)
                {
                        itemVM.ConsignToAccLogInfo.ProductQuantity = itemVM.ConsignToAccLogInfo.ProductQuantity.HasValue ? itemVM.ConsignToAccLogInfo.ProductQuantity.Value : 0;
                        //itemVM.ConsignToAccLogInfo.Cost = itemVM.ConsignToAccLogInfo.Cost;
                        itemVM.ConsignToAccLogInfo.CreateCost = itemVM.ConsignToAccLogInfo.CreateCost.HasValue ? itemVM.ConsignToAccLogInfo.CreateCost.Value : 0;
                        itemVM.ConsignToAccLogInfo.CountMany = itemVM.ConsignToAccLogInfo.CountMany.HasValue ? itemVM.ConsignToAccLogInfo.CountMany.Value : 0;
                        itemVM.ConsignToAccLogInfo.RateMarginTotal = itemVM.ConsignToAccLogInfo.RateMarginTotal.HasValue ? itemVM.ConsignToAccLogInfo.RateMarginTotal.Value : 0;


                        //被删除的商品不用来计算总额:
                        if (itemVM.SettleSysNo != -1)
                        {
                            //商品数量 X 结算
                            itemVM.ConsignToAccLogInfo.TotalAmt = itemVM.ConsignToAccLogInfo.ProductQuantity * itemVM.Cost.ToDecimal();
                            //
                            SettleProductsTotalAmt += itemVM.ConsignToAccLogInfo.CountMany.Value;
                            //创建时成本 X 商品数量
                            totalCreateCost += (itemVM.ConsignToAccLogInfo.CreateCost.Value * itemVM.ConsignToAccLogInfo.ProductQuantity.Value);
                            //总金额:
                            totalCountMany += itemVM.ConsignToAccLogInfo.CountMany.Value;
                            //毛利总额:
                            totalMarginRate += itemVM.ConsignToAccLogInfo.RateMarginTotal.Value;
                        }
                });
            }
            totalDiffAmt = totalCountMany - totalCreateCost;

            SettleProductsTotalSummary = SettleProductsTotalAmt - SettleProductsTotalReturnPoints;
            this.lblTotalStatistics.Text = string.Format(ResConsignNew.InfoMsgl_TotalStatisticsFormatString, SettleProductsTotalAmt.ToString("f2"),  SettleProductsTotalSummary.ToString("f2"));


            newVM.TotalAmt = totalCountMany;
            newVM.CreateCostTotalAmt = totalCreateCost;
            newVM.Difference = totalDiffAmt;
            newVM.RateMarginCount = totalMarginRate;
        }

        /// <summary>
        /// 合并代销结算单Items:
        /// </summary>
        private void CountConsignSettleItems()
        {
            List<CollectionPaymentItemInfoVM> cloneList = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<List<CollectionPaymentItemInfoVM>>(this.newVM.MergedSettleItems);
            List<CollectionPaymentItemInfoVM> mergedItemList = new List<CollectionPaymentItemInfoVM>();

            this.mergedItemList = cloneList;
        }

        #region [Events]

        public void ucVendorPicker_VendorSelected(object sendor, EventArgs e)
        {
            var selectVendor = e as VendorSelectedEventArgs;
            if (selectVendor.SelectedVendorInfo.SysNo != null)
            {
                txtPaySettleCompany.Text = EnumConverter.GetDescription(selectVendor.SelectedVendorInfo.VendorBasicInfo.PaySettleCompany);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (this.newVM.SettleItems.Where(x => x.SettleSysNo != -1).Count() <= 0)
            {
                Window.Alert("请先选择结算商品!");
                return;
            }
            #endregion
            //保存操作:
            Window.Confirm(ResConsignNew.ConfirmMsg_Save, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    //去除已经删除的Item（SysNo=-1）
                    newVM.SettleItems = (from tItem in newVM.SettleItems
                                          where tItem.SettleSysNo != -1
                                          select tItem).ToList();
                    CollectionPaymentInfo info = EntityConverter<CollectionPaymentInfoVM, CollectionPaymentInfo>.Convert(newVM, (s, t) =>
                    {
                        //t.ReturnPointPM = int.Parse( s.PMSysNo);
                        t.TaxRateData=s.TaxRateData;
                        
                       
                        t.PMInfo = new BizEntity.IM.ProductManagerInfo()
                        {
                            SysNo = Convert.ToInt32(s.PMSysNo),
                        };
                        t.SettleUser = new BizEntity.Common.UserInfo()
                        {
                            SysNo = s.SettleUserSysNo,
                        };
                        t.CurrentUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
                        
                    });

                    info.SettleItems.ForEach(x =>
                    {
                        x.ConsignToAccLogInfo.Cost = !x.ConsignToAccLogInfo.Cost.HasValue ? 0m : x.ConsignToAccLogInfo.Cost;
                    });
                    //保存PM高级权限，用于业务端验证
                    //info.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                    //代销商品规则检测
                    info.SettleItems.ForEach(item =>
                    {
                        if (item.SettleRuleSysNo.HasValue && item.Cost != item.SettlePrice)
                        {
                            item.SettleRuleSysNo = null;
                            item.SettleRuleName = null;
                        }
                    });
                    serviceFacade.CreateConsignSettlement(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResConsignNew.InfoMsg_Title, ResCollectionPaymentNew.InfoMsg_CreateSuccess, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Close();
                            }
                            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/CollectionPaymentMaintain/{0}", args2.Result.SysNo.Value), null, true);
                            //Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/AccountLogQuery/{0}", vm.AllOrderSysNoFormatString), null, true);
                        });
                    });
                }
            });
        }

        private void SettleProductsQueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var consignList=mergedItemList.Where(i => i.SettleSysNo != -1).ToList();
            SettleProductsQueryResultGrid.TotalCount = consignList.Count;
            this.SettleProductsQueryResultGrid.ItemsSource = consignList;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            
            //重置操作:
            Window.Confirm(ResConsignNew.ConfirmMsg_Reset, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.newVM = new CollectionPaymentInfoVM();
                    this.mergedItemList = new List<CollectionPaymentItemInfoVM>();
                    this.newVM.TaxRateData = PurchaseOrderTaxRate.Percent017;
                    this.txtPaySettleCompany.Text = string.Empty;
                    this.DataContext = newVM;
                    this.SettleProductsQueryResultGrid.Bind();
                    CalcSettleProducts();
                }
            });
            
        }

        private void btnDeleteSettleItems_Click(object sender, RoutedEventArgs e)
        {
            
            List<CollectionPaymentItemInfoVM> delItems = new List<CollectionPaymentItemInfoVM>();

            int deleteCount = this.mergedItemList.Where(i => i.IsCheckedItem == true && i.SettleSysNo != -1).Count();

            if (0 >= deleteCount)
            {
                Window.Alert(ResConsignNew.InfoMsg_CheckDeleteProducts);
                return;
            }
            //删除结算商品:
            Window.Confirm(ResConsignNew.ConfirmMsg_DeleteProducts, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.mergedItemList.Where(i => i.IsCheckedItem == true && i.SettleSysNo != -1).ToList().ForEach(x =>
                    {
                        x.SettleSysNo = -1;

                        foreach (var item in this.newVM.SettleItems)
                        {
                            if (item.ProductID == x.ProductID
                                && item.ConsignToAccLogInfo.CreateCost == x.ConsignToAccLogInfo.CreateCost
                                && item.ConsignToAccLogInfo.SettleType == x.ConsignToAccLogInfo.SettleType
                                && item.ConsignToAccLogInfo.SalePrice == x.ConsignToAccLogInfo.SalePrice
                                && item.ConsignToAccLogInfo.StockSysNo == x.ConsignToAccLogInfo.StockSysNo
                             && item.SettleRuleSysNo == x.SettleRuleSysNo
                               )
                            {
                                item.SettleSysNo = -1;
                            }
                        }

                    });

                    this.SettleProductsQueryResultGrid.Bind();
                    CalcSettleProducts();
                }
            });
            

        }

        private void btnAddSettleItems_Click(object sender, RoutedEventArgs e)
        {
            if (!newVM.VendorInfo.SysNo.HasValue || string.IsNullOrEmpty(newVM.VendorInfo.VendorBasicInfo.VendorNameLocal))
            {
               
                Window.Alert(ResConsignNew.ErrorMsg_Vendor);
                return;
            }

            
            //newVM.VendorInfo = new VendorFacade(this).GetVendorBySysNo(newVM.VendorInfo.SysNo, null);
            if (this.ucVendorPicker.IsConsign != VendorConsignFlag.GatherPay)
            {
                Window.Alert("只能选择业务模式4的商家！");
                return;
            }
            CollectionPaymentProductsQuery queryDialog = new CollectionPaymentProductsQuery
(null, newVM.StockSysNo, newVM.StockName, newVM.VendorInfo.SysNo, newVM.VendorInfo.VendorBasicInfo.VendorNameLocal, this.newVM.SettleItems);
            queryDialog.Dialog = Window.ShowDialog(ResConsignNew.InfoMsg_AddTitle, queryDialog, (obj, args) =>
                {
                    if (queryDialog.Dialog != null && queryDialog.Dialog.ResultArgs.DialogResult == DialogResultType.OK && queryDialog.Dialog.ResultArgs.Data != null)
                    {
                        //清空ViewModel ，重新绑定Items , 并刷新统计String:
                        List<CollectionPaymentItemInfoVM> getNewVMList = args.Data as List<CollectionPaymentItemInfoVM>;
                        if (null != getNewVMList)
                        {
                            newVM.SettleItems = getNewVMList;
                            CalcChooseItemCost();
                            CountConsignSettleItems();
                            CalcSettleProducts();
                            SettleProductsQueryResultGrid.Bind();
                        }
                    }
                }, new Size(900, 700));
            
        }

        #region 产品线相关检测  CRL21776  2012-11-6  by Jack  暂未启用  已移到service端验证
        
        //private void CheckProductLine(ConsignSettlementInfoVM poInfoVM, List<ConsignSettlementItemInfoVM> addItemList, Func<object, object> callback)
        //{
        //    //1.检测当前登陆PM对选择商品是否有操作权限
        //    serviceFacade.GetProductLineInfoByPM(CPApplication.Current.LoginUser.UserSysNo.Value, (obj, args) =>
        //    {
        //        if (args.FaultsHandle())
        //            return;
        //        List<ProductPMLine> tPMLineList = args.Result;
        //        //是否存在高级权限
        //        bool tIsManager = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
        //        if (tPMLineList.Count > 0 || tIsManager)
        //        {
        //            //获取已选和新选择的商品SysNo集合
        //            List<int> tProList = poInfoVM.ConsignSettlementItemInfoList.Select(x => x.ProductSysNo.Value).ToList<int>();
        //            if (addItemList != null)
        //                addItemList.ForEach(x => tProList.Add(x.ProductSysNo.Value));
        //            //获取ItemList中的 产品线和主PM
        //            serviceFacade.GetProductLineSysNoByProductList(tProList.ToArray(), (obj1, args1) =>
        //            {
        //                if (args1.FaultsHandle())
        //                    return;
        //                List<ProductPMLine> tList = args1.Result;
        //                string tErrorMsg = string.Empty;
        //                //检测没有产品线的商品
        //                tList.ForEach(x =>
        //                {
        //                    if (x.ProductLineSysNo == null)
        //                        tErrorMsg += x.ProductID + Environment.NewLine;
        //                });
        //                if (!tErrorMsg.Equals(string.Empty))
        //                {
        //                    Window.Alert(ResConsignNew.AlertMsg_NotLine + Environment.NewLine + tErrorMsg);
        //                    return;
        //                }
        //                //检测当前登陆PM对ItemList中商品是否有权限
        //                if (!tIsManager)
        //                    tList.ForEach(x =>
        //                    {
        //                        if (tPMLineList.SingleOrDefault(item => item.ProductLineSysNo == x.ProductLineSysNo) == null)
        //                            tErrorMsg += x.ProductID + Environment.NewLine;
        //                    });
        //                if (!tErrorMsg.Equals(string.Empty))
        //                {
        //                    Window.Alert(ResConsignNew.AlertMsg_NotAccessLine + Environment.NewLine + tErrorMsg);
        //                    return;
        //                }
        //                //验证ItemList中产品线是否唯一
        //                if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count != 1)
        //                {
        //                    Window.Alert(ResConsignNew.AlertMsg_NotOnlyOneLine);
        //                    return;
        //                }
        //                if ((string.IsNullOrEmpty(poInfoVM.PMSysNo)) || (int.Parse(poInfoVM.PMSysNo) != tList[0].PMSysNo))
        //                {
        //                    //需要根据商品的产品线加载PO单的所属PM
        //                    poInfoVM.PMSysNo = tList[0].PMSysNo.ToString();
        //                }
        //                callback(null);
        //            });
        //        }
        //        else
        //        {
        //            Window.Alert(ResConsignNew.AlertMsg_NotAccessLine);
        //            return;
        //        }
        //    });
        //}

        //private void WritePMSysNo(Func<object, object> callback)
        //{
        //    if (newVM.ConsignSettlementItemInfoList.Count > 0)
        //    {
        //        serviceFacade.GetProductLineSysNoByProductList(newVM.ConsignSettlementItemInfoList.Select(x => x.ProductSysNo.Value).ToArray(), (obj1, args1) =>
        //        {
        //            if (args1.FaultsHandle())
        //                return;
        //            List<ProductPMLine> tList = args1.Result;
        //            //如果当前ItemList中只存在一条产品线，则更新所属PM
        //            if (tList != null && tList.Count != 0)
        //                if (tList.Select(x => x.ProductLineSysNo.Value).Distinct().ToList().Count == tList.Count)
        //                {
        //                    newVM.PMSysNo = tList[0].PMSysNo.ToString();
        //                }
        //            callback(null);
        //        });
        //    }
        //    else
        //    {
        //        //如果删完item，则清空所属PM
        //        newVM.PMSysNo = null;
        //        callback(null);
        //    }
        //}

        #endregion

        private void CalcChooseItemCost()
        {
            
            foreach (CollectionPaymentItemInfoVM item in newVM.SettleItems)
            {
                if (item.SettleSysNo != -1)
                {
                    if (item.SettleType == SettleType.P)
                    {
                        //代销（佣金百分比添加） 设置数据信息
                        var profit = item.ConsignToAccLogInfo.SalePrice.ToDecimal() * item.SettlePercentage.ToDecimal() / 100;
                        if (profit >= item.ConsignToAccLogInfo.MinCommission.ToDecimal())
                        {
                            //<!--CRL21118 去除积分扣除
                            item.Cost = item.ConsignToAccLogInfo.SalePrice.ToDecimal() * (1 - item.SettlePercentage.ToDecimal() / 100); //- (item.ConsignToAccLogInfo.Point.ToInteger() / 10.0)).ToString("f2");
                        }
                        else
                        {
                            item.Cost = item.ConsignToAccLogInfo.SalePrice.Value - item.ConsignToAccLogInfo.MinCommission.Value;
                        }
                    }
                    else
                    {
                        item.Cost = item.SettleRuleSysNo.HasValue ? item.SettlePrice : item.Cost;
                    }
                    item.ConsignToAccLogInfo.FoldCost = item.ConsignToAccLogInfo.CreateCost - item.Cost;//折扣
                    item.ConsignToAccLogInfo.RateMargin = item.ConsignToAccLogInfo.SalePrice - item.Cost;//毛利
                    //item.FoldCost=item.ConsignToAccLogInfo.FoldCost.HasValue?item.ConsignToAccLogInfo.FoldCost.Value:0M;
                    //item.RateMargin = item.ConsignToAccLogInfo.RateMargin.HasValue ? item.ConsignToAccLogInfo.RateMargin.Value : 0M;
                    //item.RetailPrice = item.ConsignToAccLogInfo.SalePrice;
                    //item.OrderCount =1;
                    //item.ProductQuntity = item.ConsignToAccLogInfo.ProductQuantity.Value;
                    
                }
            }
            
        }

       

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {

                if (null != this.SettleProductsQueryResultGrid.ItemsSource)
                {
                    foreach (var item in this.SettleProductsQueryResultGrid.ItemsSource)
                    {
                        if (item is CollectionPaymentItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((CollectionPaymentItemInfoVM)item).IsCheckedItem)
                                {
                                    ((CollectionPaymentItemInfoVM)item).IsCheckedItem = true;
                                }
                            }
                            else
                            {
                                if (((CollectionPaymentItemInfoVM)item).IsCheckedItem)
                                {
                                    ((CollectionPaymentItemInfoVM)item).IsCheckedItem = false;
                                }
                            }

                        }
                    }
                }
            }
        }


        #endregion

        private void hpkOrderCount_Click(object sender, RoutedEventArgs e)
        {
            //订单数量链接，链接至代销转财务记录查询:
            CollectionPaymentItemInfoVM vm = this.SettleProductsQueryResultGrid.SelectedItem as CollectionPaymentItemInfoVM;
            if (null != vm)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/AccountLogQuery/{0}", vm.AllOrderSysNoFormatString), null, true);
            }
        }

        private void hpkSettleRuleName_Click(object sender, RoutedEventArgs e)
        {
            CollectionPaymentItemInfoVM vm = this.SettleProductsQueryResultGrid.SelectedItem as CollectionPaymentItemInfoVM;
            if (null != vm)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/SettledProductsRuleQuery/{0}", vm.SettleRuleName), null, true);
            }
        }

        private void txtSettleCost_LostFocus(object sender, RoutedEventArgs e)
        {

            TextBox txt = sender as TextBox;
            if (null != txt && !txt.IsReadOnly)
            {
                this.SettleProductsQueryResultGrid.UpdateLayout();

                decimal getInputDecimal = 0;
                if (!string.IsNullOrEmpty(txt.Text.Trim()) && !decimal.TryParse(txt.Text.Trim(), out getInputDecimal))
                {
                    getInputDecimal = 0;
                    //txt.Text = getInputDecimal.ToString("f2");
                }

                //根据修改的结算金额，重新计算此行的其它Row:
                CollectionPaymentItemInfoVM getSelectedItemVM =txt.DataContext as CollectionPaymentItemInfoVM;
                if (null != getSelectedItemVM)
                {
                    //结算:
                    getSelectedItemVM.Cost = getInputDecimal;

                    //总金额:
                    getSelectedItemVM.ConsignToAccLogInfo.CountMany = getSelectedItemVM.ConsignToAccLogInfo.ProductQuantity * getInputDecimal;
                    //毛利:
                    getSelectedItemVM.ConsignToAccLogInfo.RateMargin = getSelectedItemVM.ConsignToAccLogInfo.SalePrice - getInputDecimal;
                    //毛利总额 ：
                    getSelectedItemVM.ConsignToAccLogInfo.RateMarginTotal = getSelectedItemVM.ConsignToAccLogInfo.RateMargin * getSelectedItemVM.ConsignToAccLogInfo.ProductQuantity;
                    //差额
                    getSelectedItemVM.ConsignToAccLogInfo.FoldCost = (getSelectedItemVM.ConsignToAccLogInfo.CreateCost ?? 0m) - (getSelectedItemVM.Cost ?? 0m);
                    //把改变的结算价格更新到数据源中:
                    foreach (var item in this.newVM.SettleItems)
                    {
                        if (item.ProductID == getSelectedItemVM.ProductID
                            && item.ConsignToAccLogInfo.CreateCost == getSelectedItemVM.ConsignToAccLogInfo.CreateCost
                            && item.ConsignToAccLogInfo.SettleType == getSelectedItemVM.ConsignToAccLogInfo.SettleType
                            && item.ConsignToAccLogInfo.SalePrice == getSelectedItemVM.ConsignToAccLogInfo.SalePrice
                            && item.ConsignToAccLogInfo.StockSysNo == getSelectedItemVM.ConsignToAccLogInfo.StockSysNo
                             && item.SettleRuleSysNo == getSelectedItemVM.SettleRuleSysNo
                            )
                        {
                            item.Cost = getInputDecimal;
                            item.ConsignToAccLogInfo.FoldCost = (item.ConsignToAccLogInfo.CreateCost ?? 0m) - (item.Cost ?? 0m);
                            item.ConsignToAccLogInfo.CountMany = item.ConsignToAccLogInfo.ProductQuantity * getInputDecimal;
                            item.ConsignToAccLogInfo.RateMargin = item.ConsignToAccLogInfo.SalePrice - getInputDecimal;
                            item.ConsignToAccLogInfo.RateMarginTotal = item.ConsignToAccLogInfo.RateMargin * item.ConsignToAccLogInfo.ProductQuantity;
                        }
                    }
                    CalcSettleProducts();
                }
            }
        }

    }

}
