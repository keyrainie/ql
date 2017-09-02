using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class ConsignNew : PageBase
    {

        public ConsignSettlementInfoVM newVM;
        public ConsignSettlementFacade serviceFacade;
        public List<ConsignSettlementItemInfoVM> mergedItemList;
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

        public ConsignNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            newVM = new ConsignSettlementInfoVM() { CurrencyCode = "1", TaxRateData = PurchaseOrderTaxRate.Percent017 };
            mergedItemList = new List<ConsignSettlementItemInfoVM>();
            serviceFacade = new ConsignSettlementFacade(this);
            InitializeComboBoxData();
            this.DataContext = newVM;
            CalcSettleProducts();
            SetAccessControl();
            //供应商附加选择事件
            ucVendorPicker.VendorSelected += new EventHandler<VendorSelectedEventArgs>(ucVendorPicker_VendorSelected);
        }

        private void SetAccessControl()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_New))
            {
                btnAddSettleItems.IsEnabled = false;
                btnDeleteSettleItems.IsEnabled = false;
                btnReset.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
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
                this.mergedItemList.ForEach(delegate(ConsignSettlementItemInfoVM itemVM)
                {
                    if (itemVM.SettleSysNo != -1)
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
                    }
                });
            }
            totalDiffAmt = totalCountMany - totalCreateCost;

            SettleProductsTotalReturnPoints = newVM.EIMSInfo.UsingReturnPoint.HasValue ? newVM.EIMSInfo.UsingReturnPoint.Value : 0m;

            SettleProductsTotalSummary = SettleProductsTotalAmt - SettleProductsTotalReturnPoints;
            this.lblTotalStatistics.Text = string.Format(ResConsignNew.InfoMsgl_TotalStatisticsFormatString, SettleProductsTotalAmt.ToString("f2"), SettleProductsTotalReturnPoints.ToString("f2"), SettleProductsTotalSummary.ToString("f2"));


            newVM.TotalAmt = totalCountMany;
            newVM.CreateCostTotalAmt = totalCreateCost;
            newVM.Difference = totalDiffAmt;
            newVM.RateMarginCount = totalMarginRate;


            //this.txtUsingReturnPoint.Text = newVM.EIMSInfo.UsingReturnPoint.HasValue ? newVM.EIMSInfo.UsingReturnPoint.Value.ToString("f2") : "0.00";
            //this.txtRemainReturnPoint.Text = newVM.EIMSInfo.RemnantReturnPoint.HasValue ? newVM.EIMSInfo.RemnantReturnPoint.Value.ToString("f2") : "0.00";
        }

        /// <summary>
        /// 合并代销结算单Items:
        /// </summary>
        private void CountConsignSettleItems()
        {
            List<ConsignSettlementItemInfoVM> cloneList = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<List<ConsignSettlementItemInfoVM>>(this.newVM.ConsignSettlementItemInfoList);
            List<ConsignSettlementItemInfoVM> tmpmergedItemList = new List<ConsignSettlementItemInfoVM>();
            int index = 0;
            if (null != this.newVM.ConsignSettlementItemInfoList)
            {
                foreach (ConsignSettlementItemInfoVM item in cloneList)
                {
                    if (item.SettleSysNo != -1)
                    {
                        ConsignSettlementItemInfoVM vm = tmpmergedItemList.SingleOrDefault(x =>
                            x.ProductID == item.ProductID
                            && x.ConsignToAccLogInfo.CreateCost == item.ConsignToAccLogInfo.CreateCost
                            && x.ConsignToAccLogInfo.SettleType == item.ConsignToAccLogInfo.SettleType
                            && x.SettlePercentage == item.SettlePercentage
                            && x.ConsignToAccLogInfo.SalePrice == item.ConsignToAccLogInfo.SalePrice
                            && x.ConsignToAccLogInfo.Point.ToInteger() == item.ConsignToAccLogInfo.Point.ToInteger()
                            && x.ConsignToAccLogInfo.StockSysNo == item.ConsignToAccLogInfo.StockSysNo
                            && x.SettleRuleSysNo == item.SettleRuleSysNo);

                        //如果不存在，则添加进List:
                        if (vm == null)
                        {
                            item.OrderCount = 1;
                            item.AllOrderSysNoFormatString = (item.ConsignToAccLogInfo.LogSysNo.HasValue ? item.ConsignToAccLogInfo.LogSysNo.Value.ToString() : string.Empty);
                            if (!item.AcquireReturnPointType.HasValue)
                            {
                                item.ContractReturnPointSet = ResConsignNew.InfoMsg_ReturnPointNotSet;
                            }
                            else
                            {
                                item.ContractReturnPointSet = ResConsignMaintain.Msg_AlreadySet;

                            }
                            if (item.SettleType == SettleType.P)
                            {
                                //代销（佣金百分比添加） 设置数据信息
                                var profit = item.ConsignToAccLogInfo.SalePrice.ToDecimal() * item.SettlePercentage.ToDecimal() / 100;
                                if (profit >= item.ConsignToAccLogInfo.MinCommission.ToDecimal())
                                {
                                    //<!--CRL21118 去除积分扣除
                                    item.Cost = item.ConsignToAccLogInfo.SalePrice.ToDecimal() * (1 - item.SettlePercentage.ToDecimal() / 100); //- (item.ConsignToAccLogInfo.Point.ToInteger() / 10.0)).ToString("f2");
                                    newVM.ConsignSettlementItemInfoList[index].Cost = item.Cost;
                                }
                                else
                                {
                                    item.Cost = item.ConsignToAccLogInfo.SalePrice.Value - item.ConsignToAccLogInfo.MinCommission.Value;
                                }
                            }
                            else
                            {
                                item.Cost = item.SettleRuleSysNo.HasValue ? item.SettlePrice : item.Cost;
                                newVM.ConsignSettlementItemInfoList[index].Cost = item.Cost;
                            }
                            item.ConsignToAccLogInfo.FoldCost = item.ConsignToAccLogInfo.CreateCost - item.Cost;//结算成本差额
                            item.ConsignToAccLogInfo.RateMargin = item.ConsignToAccLogInfo.SalePrice - item.Cost;//毛利
                            tmpmergedItemList.Add(item);
                        }
                        else
                        {
                            //如果存在，则进行累加:
                            vm.ConsignToAccLogInfo.ProductQuantity += item.ConsignToAccLogInfo.ProductQuantity;//商品总数
                            if((vm.Cost??0m)>=(item.Cost??0m))
                            {
                                vm.Cost = item.SettleRuleSysNo.HasValue ? item.SettlePrice : item.Cost;
                                newVM.ConsignSettlementItemInfoList[index].Cost = vm.Cost;
                            }
                            vm.OrderCount += 1;//订单数量+1
                            vm.AllOrderSysNoFormatString += string.Format("-{0}", item.ConsignToAccLogInfo.LogSysNo);
                            if (vm.AcquireReturnPointType.HasValue)
                            {
                                vm.ContractReturnPointSet = ResConsignNew.InfoMsg_ReturnPointSet;
                                if (vm.AcquireReturnPointType == 0)
                                {
                                    vm.ExpectGetPoint = vm.AcquireReturnPoint * vm.ConsignToAccLogInfo.ProductQuantity;
                                }
                                if (vm.AcquireReturnPointType == 1)
                                {
                                    vm.ExpectGetPoint = vm.AcquireReturnPoint / 100 * vm.ConsignToAccLogInfo.ProductQuantity * (vm.Cost ?? 0m);
                                }
                            }
                        }
                    }
                    index++;
                }
                // 重新计算总金额，毛利总额:
                foreach (var item in tmpmergedItemList)
                {
                    if (item.ContractReturnPointSet == ResConsignNew.InfoMsg_ReturnPointNotSet)
                    {
                        item.ExpectGetPoint = null;
                    }
                    else
                    {
                        if (item.AcquireReturnPointType == 0)
                        {
                            item.ExpectGetPoint = item.ConsignToAccLogInfo.ProductQuantity * item.AcquireReturnPoint;
                        }
                        else
                        {
                            item.ExpectGetPoint = item.ConsignToAccLogInfo.ProductQuantity * item.AcquireReturnPoint * item.Cost.ToDecimal() / 100;
                        }
                    }
                    item.ConsignToAccLogInfo.CountMany = item.ConsignToAccLogInfo.ProductQuantity * item.Cost.ToDecimal();
                    item.ConsignToAccLogInfo.RateMarginTotal = item.ConsignToAccLogInfo.RateMargin * item.ConsignToAccLogInfo.ProductQuantity;
                }
            }

            this.mergedItemList = tmpmergedItemList;
        }

        #region [Events]

        public void ucVendorPicker_VendorSelected(object sendor, VendorSelectedEventArgs e)
        {
            this.newVM.LeaseType = e.SelectedVendorInfo.VendorBasicInfo.VendorIsToLease;
            txtLeaseType.Text = this.newVM.LeaseTypeDisplay;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (this.newVM.ConsignSettlementItemInfoList.Where(x => x.SettleSysNo != -1).Count() <= 0)
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
                    newVM.ConsignSettlementItemInfoList = (from tItem in newVM.ConsignSettlementItemInfoList
                                                                       where tItem.SettleSysNo != -1
                                                                       select tItem).ToList();

                    ConsignSettlementInfo info = EntityConverter<ConsignSettlementInfoVM, ConsignSettlementInfo>.Convert(newVM, (s, t) =>
                    {
                        decimal usingReturnPoint = 0m;
                        t.EIMSInfo = new ConsignSettlementEIMSInfo();
                        if (decimal.TryParse("", out usingReturnPoint))
                        {
                            t.EIMSInfo.UsingReturnPoint = usingReturnPoint;
                        }
                        else
                        {
                            t.EIMSInfo.UsingReturnPoint = null;
                        }
                        t.SourceStockInfo = new BizEntity.Inventory.StockInfo()
                        {
                            SysNo = s.StockSysNo,
                        };
                        t.PMInfo = new BizEntity.IM.ProductManagerInfo()
                        {
                            SysNo = Convert.ToInt32(s.PMSysNo),
                        };
                        t.SettleUser = new BizEntity.Common.UserInfo()
                        {
                            SysNo = s.SettleUserSysNo,
                        };
                    });

                    info.ConsignSettlementItemInfoList.ForEach(x =>
                    {
                        x.ConsignToAccLogInfo.Cost = !x.ConsignToAccLogInfo.Cost.HasValue ? 0m : x.ConsignToAccLogInfo.Cost;
                    });
                    //保存PM高级权限，用于业务端验证
                    info.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                    //代销商品规则检测
                    info.ConsignSettlementItemInfoList.ForEach(item =>
                    {
                        if (item.SettleType == SettleType.O)
                        {
                            if (item.SettleRuleSysNo.HasValue && item.Cost != item.SettlePrice)
                            {
                                item.SettleRuleSysNo = null;
                                item.SettleRuleName = null;
                            }
                        }
                    });
                    //转租赁
                    info.LeaseType = this.newVM.LeaseType;
                    serviceFacade.CreateConsignSettlement(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResConsignNew.InfoMsg_Title, ResConsignNew.InfoMsg_CreateSuccess, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Close();
                            }
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
                    this.newVM = new ConsignSettlementInfoVM();
                    this.mergedItemList = new List<ConsignSettlementItemInfoVM>();
                    this.newVM.TaxRateData = PurchaseOrderTaxRate.Percent017;                   
                    this.DataContext = newVM;
                    this.SettleProductsQueryResultGrid.Bind();
                    CalcSettleProducts();
                }
            });
        }

        private void btnDeleteSettleItems_Click(object sender, RoutedEventArgs e)
        {
            List<ConsignSettlementItemInfoVM> delItems = new List<ConsignSettlementItemInfoVM>();

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

                        foreach (var item in this.newVM.ConsignSettlementItemInfoList)
                        {
                            if (item.ProductID == x.ProductID
                                && item.ConsignToAccLogInfo.CreateCost == x.ConsignToAccLogInfo.CreateCost
                                && item.ConsignToAccLogInfo.SettleType == x.ConsignToAccLogInfo.SettleType
                                && item.ConsignToAccLogInfo.SalePrice == x.ConsignToAccLogInfo.SalePrice
                                && item.ConsignToAccLogInfo.StockSysNo == x.ConsignToAccLogInfo.StockSysNo
                             && item.SettleRuleSysNo == x.SettleRuleSysNo
                                && item.ConsignToAccLogInfo.Point.ToInteger() == x.ConsignToAccLogInfo.Point.ToInteger())
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

            //if (!newVM.StockSysNo.HasValue)
            //{
            //    Window.Alert(ResConsignNew.ErrorMsg_Stock);
            //    return;
            //}
            if (!newVM.VendorInfo.SysNo.HasValue || string.IsNullOrEmpty(newVM.VendorInfo.VendorBasicInfo.VendorNameLocal))
            {
                Window.Alert(ResConsignNew.ErrorMsg_Vendor);
                return;
            }
            if (this.ucVendorPicker.IsConsign == VendorConsignFlag.GatherPay)
            {
                Window.Alert("不能选择业务模式4的商家！");
                return;
            }
            SettledProductsQuery queryDialog = new SettledProductsQuery
(null, newVM.StockSysNo, newVM.StockName, newVM.VendorInfo.SysNo, newVM.VendorInfo.VendorBasicInfo.VendorNameLocal, this.newVM.ConsignSettlementItemInfoList);
            queryDialog.Dialog = Window.ShowDialog(ResConsignNew.InfoMsg_AddTitle, queryDialog, (obj, args) =>
                {
                    if (queryDialog.Dialog != null && queryDialog.Dialog.ResultArgs.DialogResult == DialogResultType.OK && queryDialog.Dialog.ResultArgs.Data != null)
                    {
                        //清空ViewModel ，重新绑定Items , 并刷新统计String:
                        List<ConsignSettlementItemInfoVM> getNewVMList = args.Data as List<ConsignSettlementItemInfoVM>;
                        if (null != getNewVMList)
                        {
                            newVM.ConsignSettlementItemInfoList = getNewVMList;
                            CalcChooseItemCost();
                            CountConsignSettleItems();
                            CalcSettleProducts();
                            SettleProductsQueryResultGrid.Bind();
                        }
                    }
                }, new Size(700, 580));
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
            foreach (ConsignSettlementItemInfoVM item in newVM.ConsignSettlementItemInfoList)
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
                }
            }
        }

        private void btnChooseEIMS_Click(object sender, RoutedEventArgs e)
        {
            //设置返点信息:
            if (!this.newVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResConsignNew.ErrorMsg_ReturnPoints);
                return;
            }
            ConsignSettlementEIMSView eimsView = new ConsignSettlementEIMSView(newVM);
            eimsView.Dialog = Window.ShowDialog(ResConsignNew.InfoMsg_PointSearchTitle, eimsView, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    ConsignSettlementEIMSInfo info = args.Data as ConsignSettlementEIMSInfo;
                    if (null != info)
                    {
                        this.newVM.EIMSInfo.ReturnPointName = info.ReturnPointName;
                        this.newVM.EIMSInfo.ReturnPointSysNo = info.ReturnPointSysNo;
                        this.newVM.EIMSInfo.RemnantReturnPoint = info.RemnantReturnPoint;
                        this.newVM.PM_ReturnPointSysNo = info.ReturnPointSysNo;
                        //this.txtReturnPointName.Text = info.ReturnPointName;
                        //this.txtReturnPointSysNo.Text = info.ReturnPointSysNo.Value.ToString();
                        //this.txtRemainReturnPoint.Text = info.RemnantReturnPoint.Value.ToString();
                    }
                }
            }, new Size(700, 500));
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
                        if (item is ConsignSettlementItemInfoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((ConsignSettlementItemInfoVM)item).IsCheckedItem)
                                {
                                    ((ConsignSettlementItemInfoVM)item).IsCheckedItem = true;
                                }
                            }
                            else
                            {
                                if (((ConsignSettlementItemInfoVM)item).IsCheckedItem)
                                {
                                    ((ConsignSettlementItemInfoVM)item).IsCheckedItem = false;
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
            ConsignSettlementItemInfoVM vm = this.SettleProductsQueryResultGrid.SelectedItem as ConsignSettlementItemInfoVM;
            if (null != vm)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/AccountLogQuery/{0}", vm.AllOrderSysNoFormatString), null, true);
            }
        }

        private void hpyContractSet_Click(object sender, RoutedEventArgs e)
        {
            ConsignSettlementItemInfoVM vm = this.SettleProductsQueryResultGrid.SelectedItem as ConsignSettlementItemInfoVM;
            if (null != vm)
            {
                ConsignSettlementContractSet contractSetCtrl = new ConsignSettlementContractSet(vm);
                contractSetCtrl.Dialog = Window.ShowDialog("结算商品返利信息", contractSetCtrl, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK && args.Data != null)
                    {
                        string actionStr = ((List<object>)args.Data)[0].ToString();
                        vm = (ConsignSettlementItemInfoVM)((List<object>)args.Data)[1];

                        for (int i = 0; i < newVM.ConsignSettlementItemInfoList.Count; i++)
                        {

                            var x = newVM.ConsignSettlementItemInfoList[i];

                            bool condition = false;
                            switch (actionStr)
                            {
                                case "Save":
                                    condition = (x.ProductSysNo == vm.ProductSysNo
                                    && x.ConsignToAccLogInfo.CreateCost == vm.ConsignToAccLogInfo.CreateCost
                                    && x.ConsignToAccLogInfo.SettleType == vm.ConsignToAccLogInfo.SettleType
                                    && x.ConsignToAccLogInfo.SalePrice == vm.ConsignToAccLogInfo.SalePrice
                                    && x.ConsignToAccLogInfo.Point.ToInteger() == vm.ConsignToAccLogInfo.Point.ToInteger());
                                    break;
                                case "SaveSame":
                                    condition = (x.ProductSysNo == vm.ProductSysNo);
                                    break;
                                case "SaveAll":
                                    condition = true;
                                    break;
                                default:
                                    break;
                            }


                            if (condition)
                            {
                                if (x.SettleType == SettleType.P && x.SettlePercentage != vm.SettlePercentage)
                                {
                                    continue;
                                }
                                x.AcquireReturnPointType = vm.AcquireReturnPointType;
                                x.AcquireReturnPoint = vm.AcquireReturnPoint;

                                if (x.AcquireReturnPointType.HasValue)
                                {
                                    x.ContractReturnPointSet = ResConsignMaintain.Msg_AlreadySet;
                                    if (x.AcquireReturnPointType == 0)
                                    {
                                        x.ExpectGetPoint = x.AcquireReturnPoint * x.ConsignToAccLogInfo.ProductQuantity;
                                    }
                                    if (x.AcquireReturnPointType == 1)
                                    {
                                        x.ExpectGetPoint = x.AcquireReturnPoint / 100 * x.ConsignToAccLogInfo.ProductQuantity * vm.Cost.ToDecimal();
                                    }
                                }
                            }
                        }
                        CountConsignSettleItems();
                        CalcSettleProducts();
                        this.SettleProductsQueryResultGrid.Bind();
                    }
                }, new Size(500, 150));
            }

        }

        private void hpkSettleRuleName_Click(object sender, RoutedEventArgs e)
        {
            ConsignSettlementItemInfoVM vm = this.SettleProductsQueryResultGrid.SelectedItem as ConsignSettlementItemInfoVM;
            if (null != vm)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/SettledProductsRuleQuery/{0}", vm.SettleRuleName), null, true);
            }
        }

        private void txtUsingReturnPoint_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal getChangedVal = 0;
            if (decimal.TryParse("", out getChangedVal))
            {
                newVM.EIMSInfo.UsingReturnPoint = getChangedVal;
            }
            else
            {
                newVM.EIMSInfo.UsingReturnPoint = 0m;
            }

            CalcSettleProducts();
        }

        private void txtSettlePercentage_LostFocus(object sender, RoutedEventArgs e)
        {

            TextBox txt = sender as TextBox;
            if (null != txt && !txt.IsReadOnly)
            {
                this.SettleProductsQueryResultGrid.UpdateLayout();
                decimal getInputPercentage = 0m;
                if (!string.IsNullOrEmpty(txt.Text.Trim()) && !decimal.TryParse(txt.Text.Trim(), out getInputPercentage))
                {
                    getInputPercentage = 0m;
                }

                //根据修改的佣金百分比，重新计算此行的其它Row:
                ConsignSettlementItemInfoVM getSelectedItemVM = txt.DataContext as ConsignSettlementItemInfoVM;
                if (null != getSelectedItemVM)
                {
                    //结算:
                    var profit = getSelectedItemVM.ConsignToAccLogInfo.SalePrice.ToDecimal() * getInputPercentage / 100;
                    var cost = getSelectedItemVM.ConsignToAccLogInfo.SalePrice * (1 - getInputPercentage / 100);
                    if (profit >= getSelectedItemVM.ConsignToAccLogInfo.MinCommission.ToDecimal())
                    {
                        getSelectedItemVM.Cost = cost;
                    }
                    else
                    {
                        getSelectedItemVM.Cost = getSelectedItemVM.ConsignToAccLogInfo.SalePrice - getSelectedItemVM.ConsignToAccLogInfo.MinCommission;
                    }

                    decimal getCost = getSelectedItemVM.Cost ?? 0m;
                    //佣金百分比
                    getSelectedItemVM.SettlePercentage = getInputPercentage.ToString();
                    //总金额:
                    getSelectedItemVM.ConsignToAccLogInfo.CountMany = getSelectedItemVM.ConsignToAccLogInfo.ProductQuantity * getCost;
                    //毛利:
                    getSelectedItemVM.ConsignToAccLogInfo.RateMargin = getSelectedItemVM.ConsignToAccLogInfo.SalePrice - getCost;
                    //毛利总额 ：
                    getSelectedItemVM.ConsignToAccLogInfo.RateMarginTotal = getSelectedItemVM.ConsignToAccLogInfo.RateMargin * getSelectedItemVM.ConsignToAccLogInfo.ProductQuantity;
                    //差额
                    getSelectedItemVM.ConsignToAccLogInfo.FoldCost = (getSelectedItemVM.ConsignToAccLogInfo.CreateCost ?? 0m) - (getSelectedItemVM.Cost ?? 0m);
                    //把改变的结算价格更新到数据源中:
                    foreach (var item in this.newVM.ConsignSettlementItemInfoList)
                    {
                        if (item.ProductID == getSelectedItemVM.ProductID
                            && item.ConsignToAccLogInfo.CreateCost == getSelectedItemVM.ConsignToAccLogInfo.CreateCost
                            && item.ConsignToAccLogInfo.SettleType == getSelectedItemVM.ConsignToAccLogInfo.SettleType
                            && item.ConsignToAccLogInfo.SalePrice == getSelectedItemVM.ConsignToAccLogInfo.SalePrice
                            && item.ConsignToAccLogInfo.StockSysNo == getSelectedItemVM.ConsignToAccLogInfo.StockSysNo
                            && item.ConsignToAccLogInfo.Point.ToInteger() == getSelectedItemVM.ConsignToAccLogInfo.Point.ToInteger())
                        {
                            item.Cost = getCost;
                            item.SettlePercentage = getInputPercentage.ToString();
                            item.ConsignToAccLogInfo.CountMany = item.ConsignToAccLogInfo.ProductQuantity * getCost;
                            item.ConsignToAccLogInfo.RateMargin = item.ConsignToAccLogInfo.SalePrice - getCost;
                            item.ConsignToAccLogInfo.RateMarginTotal = item.ConsignToAccLogInfo.RateMargin * item.ConsignToAccLogInfo.ProductQuantity;
                        }
                    }
                    CalcSettleProducts();
                }
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
                ConsignSettlementItemInfoVM getSelectedItemVM =txt.DataContext as ConsignSettlementItemInfoVM;
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
                    foreach (var item in this.newVM.ConsignSettlementItemInfoList)
                    {
                        if (item.ProductID == getSelectedItemVM.ProductID
                            && item.ConsignToAccLogInfo.CreateCost == getSelectedItemVM.ConsignToAccLogInfo.CreateCost
                            && item.ConsignToAccLogInfo.SettleType == getSelectedItemVM.ConsignToAccLogInfo.SettleType
                            && item.ConsignToAccLogInfo.SalePrice == getSelectedItemVM.ConsignToAccLogInfo.SalePrice
                             && item.ConsignToAccLogInfo.StockSysNo == getSelectedItemVM.ConsignToAccLogInfo.StockSysNo
                             && item.SettleRuleSysNo == getSelectedItemVM.SettleRuleSysNo
                            && item.ConsignToAccLogInfo.Point.ToInteger() == getSelectedItemVM.ConsignToAccLogInfo.Point.ToInteger())
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
