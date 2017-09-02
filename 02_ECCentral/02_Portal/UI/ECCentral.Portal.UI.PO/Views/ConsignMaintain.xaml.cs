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
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Threading;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ConsignMaintain : PageBase
    {
        public string ConsignSysNo;
        public ConsignSettlementFacade serviceFacade;
        public ConsignSettlementInfoVM consignSettleVM;
        public List<ConsignSettlementItemInfoVM> mergedItemList;

        /// <summary>
        /// 结算单总金额
        /// </summary>
        public decimal SettleProductsTotalAmt = 0;
        /// <summary>
        /// 使用返利
        /// </summary>
        public decimal SettleProductsTotalReturnPoints = 0;
        /// <summary>
        /// 合计
        /// </summary>
        public decimal SettleProductsTotalSummary = 0;


        public ConsignMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new ConsignSettlementFacade(this);
            mergedItemList = new List<ConsignSettlementItemInfoVM>();
            InitializeComboBoxData();
            ConsignSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(ConsignSysNo))
            {
                //加载代销结算单信息:
                LoadConsignInfo();
            }
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_Settle))
            {
                btnSettle.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_CancelSettle))
            {
                btnCancelSettled.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_CancelAbandon))
            {
                btnCancelAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_Save))
            {
                btnSave.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_AddProduct))
            {
                btnAddSettleProducts.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Edit_DeleteProduct))
            {
                btnDelSettleProducts.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_Query_Audit))
            {
                btnAudit.IsEnabled = false;
            }
        }

        private void InitializeComboBoxData()
        {
            //税率:
            this.cmbTaxRateData.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderTaxRate>(EnumConverter.EnumAppendItemType.Select);
        }

        /// <summary>
        /// 加载代销结算单信息
        /// </summary>
        private void LoadConsignInfo()
        {
            serviceFacade.GetConsignSettlementInfo(ConsignSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                consignSettleVM = new ConsignSettlementInfoVM();
                consignSettleVM = EntityConverter<ConsignSettlementInfo, ConsignSettlementInfoVM>.Convert(args.Result, (s, t) =>
                {
                    if (s.PMInfo.SysNo.HasValue)
                    {
                        t.StockSysNo = s.SourceStockInfo.SysNo;
                        t.StockID = s.SourceStockInfo.SysNo.Value.ToString();
                        t.StockName = s.SourceStockInfo.StockName.ToString();
                    }
                    t.PMSysNo = s.PMInfo.SysNo.HasValue ? s.PMInfo.SysNo.Value.ToString() : null;
                    t.SettleUserSysNo = s.SettleUser.SysNo;
                    t.SettleUserName = s.SettleUser.UserName;
                    t.VendorInfo.VendorBasicInfo.PaySettleCompany = s.VendorInfo.VendorBasicInfo.PaySettleCompany;
                    t.ConsignSettlementItemInfoList.ForEach(x =>
                    {
                        x.ConsignToAccLogInfo.SettleType = x.SettleType;
                        x.IsSettleCostTextBoxReadOnly = (t.Status != SettleStatus.WaitingAudit || x.SettleType != SettleType.O) ? true : false;
                        x.IsSettlePercentageTextBoxReadOnly = (t.Status != SettleStatus.WaitingAudit || x.SettleType != SettleType.P) ? true : false;
                        x.SettlePercentageTextBoxVisibility = x.SettleType == SettleType.P ? Visibility.Visible : Visibility.Collapsed;
                    });
                    if (t.PM_ReturnPointSysNo.HasValue)
                    {
                        t.EIMSInfo.ReturnPointName = s.EIMSInfo.ReturnPointName;
                        t.EIMSInfo.ReturnPointSysNo = s.EIMSInfo.ReturnPointSysNo;
                        t.EIMSInfo.UsingReturnPoint = s.EIMSInfo.UsingReturnPoint;
                        t.EIMSInfo.RemnantReturnPoint = s.EIMSInfo.RemnantReturnPoint;
                    }                    
                    t.LeaseType = s.LeaseType;
                    t.DeductAmt = s.DeductAmt;
                    t.DeductMethod = s.DeductMethod;
                    t.DeductAccountType = s.DeductAccountType;
                    t.ConsignRange = s.ConsignRange;
                });

                CountConsignSettleItems();
                this.DataContext = consignSettleVM;
                //由于暂时无法自动绑定返点信息到控件上，特加手动绑定临时解决
                if (consignSettleVM.PM_ReturnPointSysNo.HasValue)
                {
                    //txtReturnPointSysNo.Text = consignSettleVM.EIMSInfo.ReturnPointSysNo.ToString();
                    //txtReturnPointName.Text = consignSettleVM.EIMSInfo.ReturnPointName;
                }
                this.SettleProductsQueryResultGrid.Bind();
                CalcSettleProducts();
                ShowActionButton(consignSettleVM.Status.Value);
                this.consignSettleVM.ValidationErrors.Clear();

            });
        }

        /// <summary>
        /// 合并代销结算单Items:
        /// </summary>
        private void CountConsignSettleItems()
        {
            List<ConsignSettlementItemInfoVM> cloneList = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<List<ConsignSettlementItemInfoVM>>(this.consignSettleVM.ConsignSettlementItemInfoList);
            List<ConsignSettlementItemInfoVM> mergedItemList = new List<ConsignSettlementItemInfoVM>();
            if (null != this.consignSettleVM.ConsignSettlementItemInfoList)
            {
                int index = 0;
                foreach (ConsignSettlementItemInfoVM item in cloneList)
                {
                    if (item.SettleSysNo != -1)
                    {
                        ConsignSettlementItemInfoVM vm = mergedItemList.SingleOrDefault(x =>
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
                                item.ContractReturnPointSet = ResConsignMaintain.Msg_NotSet;
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
                                    consignSettleVM.ConsignSettlementItemInfoList[index].Cost = item.Cost;
                                }
                                else
                                {
                                    item.Cost = item.ConsignToAccLogInfo.SalePrice.Value - item.ConsignToAccLogInfo.MinCommission.Value;
                                }
                            }
                            else
                            {
                                item.Cost = item.SettleRuleSysNo.HasValue ? item.SettlePrice : item.Cost;
                                consignSettleVM.ConsignSettlementItemInfoList[index].Cost = item.Cost;
                            }
                            item.ConsignToAccLogInfo.FoldCost = item.ConsignToAccLogInfo.CreateCost - item.Cost;//折扣
                            item.ConsignToAccLogInfo.RateMargin = item.ConsignToAccLogInfo.SalePrice - item.Cost;//毛利
                            mergedItemList.Add(item);
                        }
                        else
                        {
                            //如果存在，则进行累加:
                            vm.ConsignToAccLogInfo.ProductQuantity += item.ConsignToAccLogInfo.ProductQuantity;//商品总数
                            if ((vm.Cost ?? 0m) >= (item.Cost ?? 0m))
                            {
                                vm.Cost = item.SettleRuleSysNo.HasValue ? item.SettlePrice : item.Cost;
                                consignSettleVM.ConsignSettlementItemInfoList[index].Cost = vm.Cost;
                            }
                            vm.OrderCount += 1;//订单数量+1
                            vm.AllOrderSysNoFormatString += string.Format("-{0}", item.ConsignToAccLogInfo.LogSysNo);
                            if (vm.AcquireReturnPointType.HasValue)
                            {
                                vm.ContractReturnPointSet = ResConsignMaintain.Msg_AlreadySet;
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
                foreach (var item in mergedItemList)
                {
                    if (item.ContractReturnPointSet == ResConsignMaintain.Msg_NotSet)
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

            this.mergedItemList = mergedItemList;
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

            SettleProductsTotalReturnPoints = consignSettleVM.EIMSInfo.UsingReturnPoint.HasValue ? consignSettleVM.EIMSInfo.UsingReturnPoint.Value : 0m;

            SettleProductsTotalSummary = SettleProductsTotalAmt - SettleProductsTotalReturnPoints;
           // this.lblTotalStatistics.Text = string.Format(ResConsignMaintain.InfoMsg_TotalAmt, SettleProductsTotalAmt.ToString("f2"), SettleProductsTotalReturnPoints.ToString("f2"), SettleProductsTotalSummary.ToString("f2"));
            //计算扣款
            CalcDeductAmt();          
            consignSettleVM.TotalAmt = totalCountMany - consignSettleVM.DeductAmt;
            consignSettleVM.CreateCostTotalAmt = totalCreateCost;
            consignSettleVM.Difference = totalDiffAmt;
            consignSettleVM.RateMarginCount = totalMarginRate;

          
            //this.txtUsingReturnPoint.Text = consignSettleVM.EIMSInfo.UsingReturnPoint.HasValue ? consignSettleVM.EIMSInfo.UsingReturnPoint.Value.ToString("f2") : "0.00";
            //this.txtRemainReturnPoint.Text = consignSettleVM.EIMSInfo.RemnantReturnPoint.HasValue ? consignSettleVM.EIMSInfo.RemnantReturnPoint.Value.ToString("f2") : "0.00";
        }

        //扣款计算
        private void CalcDeductAmt()
        {
            if (consignSettleVM.VendorInfo!=null && consignSettleVM.VendorInfo.VendorDeductInfo!=null)
            {
                VendorDeductInfoVM info = consignSettleVM.VendorInfo.VendorDeductInfo;
                if (info.DeductSysNo>0)
                {
                    var calcIems = this.mergedItemList.Where(p=>p.SettleSysNo != -1);
                    switch (info.CalcType)
                    {

                        case  VendorCalcType.Fix:
                            consignSettleVM.DeductAmt =Convert.ToDecimal(info.FixAmt);
                            break;
                        case VendorCalcType.Cost:                           
                            consignSettleVM.DeductAmt = calcIems.Sum(p => p.ConsignToAccLogInfo.CreateCost.Value * p.ConsignToAccLogInfo.ProductQuantity.Value) * Convert.ToDecimal(info.DeductPercent);
                            break;
                        case VendorCalcType.Sale:
                            consignSettleVM.DeductAmt = calcIems.Sum(p => p.ConsignToAccLogInfo.SalePrice.Value * p.ConsignToAccLogInfo.ProductQuantity.Value) *Convert.ToDecimal(info.DeductPercent);
                            break;
                        default:
                            break;
                    }
                    if (Convert.ToDecimal(info.MaxAmt) > 0)
                    {
                        consignSettleVM.DeductAmt = consignSettleVM.DeductAmt > Convert.ToDecimal(info.MaxAmt) ? Convert.ToDecimal(info.MaxAmt) : consignSettleVM.DeductAmt;              
                    }                   
                }
            }
            else
            {
                consignSettleVM.DeductAmt = 0;
            }
            
        }

        #region     [Events]

        private void SettleProductsQueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var consignList = mergedItemList.Where(x => x.SettleSysNo != -1).ToList();
            SettleProductsQueryResultGrid.TotalCount = consignList.Count;
            SettleProductsQueryResultGrid.ItemsSource = consignList;
        }

        private void btnSettle_Click(object sender, RoutedEventArgs e)
        {
            //结算操作:
            Window.Confirm(ResConsignMaintain.InfoMsg_Title, ResConsignMaintain.ConfirmMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.SettleConsignSettlement(getSettlementInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResConsignMaintain.InfoMsg_OperationSuccess);
                    });
                }
            });
        }

        private void btnCancelSettled_Click(object sender, RoutedEventArgs e)
        {
            //取消结算操作:
            Window.Confirm(ResConsignMaintain.InfoMsg_Title, ResConsignMaintain.ConfirmMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.CancelSettleConsignSettlement(getSettlementInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResConsignMaintain.InfoMsg_OperationSuccess);
                    });
                }
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            #region [验证]
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (this.consignSettleVM.ConsignSettlementItemInfoList.Where(x => x.SettleSysNo != -1).Count() <= 0)
            {
                Window.Alert("请先选择结算商品!");
                return;
            }
            #endregion
            //保存操作
            ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
            Window.Confirm(ResConsignMaintain.ConfirmMsg_Save, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    //保存PM高级权限，用于业务端验证
                    getSettlementInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                    //代销商品规则检测
                    getSettlementInfo.ConsignSettlementItemInfoList.ForEach(item =>
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
                    serviceFacade.UpdateConsignSettlement(getSettlementInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResConsignMaintain.InfoMsg_Title, ResConsignMaintain.InfoMsg_SaveSuccess, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Refresh();
                                return;
                            }
                        });
                    });
                }
            });
        }

        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            //作废操作:
            Window.Confirm(ResConsignMaintain.InfoMsg_Title, ResConsignMaintain.ConfirmMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.AbandonConsignSettlement(getSettlementInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResConsignMaintain.InfoMsg_OperationSuccess);
                    });
                }
            });
        }

        private void btnCancelAbandon_Click(object sender, RoutedEventArgs e)
        {
            //取消作废操作:
            Window.Confirm(ResConsignMaintain.InfoMsg_Title, ResConsignMaintain.ConfirmMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.CancelAbandonConsignSettlement(getSettlementInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResConsignMaintain.InfoMsg_OperationSuccess);
                    });
                }
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            // 取消审核操作:
            Window.Confirm(ResConsignMaintain.InfoMsg_Title, ResConsignMaintain.ConfirmMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.CancelAuditConsignSettlement(getSettlementInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResConsignMaintain.InfoMsg_OperationSuccess);
                    });
                }
            });

        }

        /// <summary>
        /// //审核代销结算单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResConsignMaintain.InfoMsg_Title, ResConsignQuery.AlertMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignSettlementInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.AuditConsignSettlement(getSettlementInfo, (obj2, args2) =>
                            {
                                if (args2.FaultsHandle())
                                {
                                    return;
                                }
                                AlertAndRefreshPage(ResConsignMaintain.InfoMsg_OperationSuccess);
                            });
                }
            });

        }

        /// <summary>
        /// 添加结算商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSettleProducts_Click(object sender, RoutedEventArgs e)
        {
            //添加结算商品
            SettledProductsQuery queryDialog = new SettledProductsQuery
(null, consignSettleVM.StockSysNo, consignSettleVM.StockName, consignSettleVM.VendorInfo.SysNo, consignSettleVM.VendorInfo.VendorBasicInfo.VendorNameLocal, this.consignSettleVM.ConsignSettlementItemInfoList);
            queryDialog.Dialog = Window.ShowDialog(ResConsignMaintain.Button_Add, queryDialog, (obj, args) =>
            {
                if (queryDialog.Dialog != null && queryDialog.Dialog.ResultArgs.DialogResult == DialogResultType.OK && queryDialog.Dialog.ResultArgs.Data != null)
                {
                    //清空ViewModel ，重新绑定Items , 并刷新统计String:
                    List<ConsignSettlementItemInfoVM> getNewVMList = args.Data as List<ConsignSettlementItemInfoVM>;
                    if (null != getNewVMList)
                    {
                        consignSettleVM.ConsignSettlementItemInfoList = getNewVMList;
                        CalcChooseItemCost();
                        CountConsignSettleItems();
                        CalcSettleProducts();
                        SettleProductsQueryResultGrid.Bind();
                    }
                }
            }, new Size(900, 700));
        }

        private void CalcChooseItemCost()
        {
            foreach (ConsignSettlementItemInfoVM item in consignSettleVM.ConsignSettlementItemInfoList)
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

        /// <summary>
        /// 删除结算商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelSettleProducts_Click(object sender, RoutedEventArgs e)
        {
            List<ConsignSettlementItemInfoVM> delItems = new List<ConsignSettlementItemInfoVM>();
            int deleteCount = this.mergedItemList.Where(i => i.IsCheckedItem == true).Count();

            if (0 >= deleteCount)
            {
                Window.Alert(ResConsignMaintain.InfoMsg_CheckDelete);
                return;
            }
            //删除结算商品:
            Window.Confirm(ResConsignMaintain.InfoMsg_ConfirmDelete, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {

                    this.mergedItemList.Where(i => i.IsCheckedItem == true).ToList().ForEach(x =>
                    {
                        x.SettleSysNo = -1;

                        foreach (var item in this.consignSettleVM.ConsignSettlementItemInfoList)
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

        #endregion

        /// <summary>
        /// 根据单据当前的状态,显示操作的按钮
        /// </summary>
        /// <param name="requestStatus"></param>
        private void ShowActionButton(SettleStatus requestStatus)
        {
            switch (requestStatus)
            {
                //初始状态， 显示保存,作废, 添加结算商品，删除结算商品 按钮:
                case SettleStatus.WaitingAudit:
                    this.btnSave.Visibility = Visibility.Visible;
                    this.btnAbandon.Visibility = Visibility.Visible;
                    this.btnAddSettleProducts.Visibility = Visibility.Visible;
                    this.btnDelSettleProducts.Visibility = Visibility.Visible;
                    this.ucStock.IsEnabled = true;
                    this.cmbTaxRateData.IsEnabled = true;
                    this.ucCurrency.IsEnabled = true;
                    //this.btnChooseEIMS.IsEnabled = true;
                    //this.txtUsingReturnPoint.IsReadOnly = false;
                    this.txtMemo.IsReadOnly = false;
                    this.txtNote.IsReadOnly = false;
                    this.SettleProductsQueryResultGrid.Columns[11].Visibility = Visibility.Collapsed;
                    this.SettleProductsQueryResultGrid.Columns[12].Visibility = Visibility.Visible;
                    this.btnAudit.Visibility = Visibility.Visible;
                    break;
                //已审核状态，显示 结算，取消审核 按钮:
                case SettleStatus.AuditPassed:
                    this.btnSettle.Visibility = Visibility.Visible;
                    this.btnCancelAudit.Visibility = Visibility.Visible;
                    this.ucVendor.IsAllowVendorSelect = false;
                    this.btnAudit.Visibility = Visibility.Collapsed;
                    break;
                //已结算，显示 取消结算 按钮:
                case SettleStatus.SettlePassed:
                    this.btnCancelSettled.Visibility = Visibility.Visible;
                    this.ucVendor.IsAllowVendorSelect = false;
                    this.btnAudit.Visibility = Visibility.Collapsed;
                    break;
                //作废状态， 显示 取消作废 按钮:
                case SettleStatus.Abandon:
                    this.btnCancelAbandon.Visibility = Visibility.Visible;

                    //this.btnChooseEIMS.IsEnabled = true;
                    //this.txtUsingReturnPoint.IsReadOnly = false;

                    this.ucVendor.IsAllowVendorSelect = false;
                    this.btnAudit.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private ConsignSettlementInfo BuildActionEntity()
        {
            return EntityConverter<ConsignSettlementInfoVM, ConsignSettlementInfo>.Convert(consignSettleVM, (s, t) =>
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
                    SysNo = s.StockID.ToNullableToInteger(),
                    StockName = s.StockName
                };
                t.PMInfo = new BizEntity.IM.ProductManagerInfo()
                {
                    SysNo = s.PMSysNo.ToNullableToInteger()
                };
                t.SettleUser = new BizEntity.Common.UserInfo()
                {
                    SysNo = s.SettleUserSysNo,
                    UserName = s.SettleUserName
                };
            });
        }

        private void AlertAndRefreshPage(string alertString)
        {
            Window.Alert(ResConsignMaintain.InfoMsg_Title, alertString, MessageType.Information, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.Cancel)
                {
                    Window.Refresh();
                }
            });
        }

        //更换事件为 失去焦点 详情请看失去焦点事件的处理
        ////TextChanged="txtUsingReturnPoint_TextChanged"
        //private void txtUsingReturnPoint_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    decimal getChangedVal = 0;
        //    if (decimal.TryParse(this.txtUsingReturnPoint.Text.Trim(), out getChangedVal))
        //    {
        //        consignSettleVM.EIMSInfo.UsingReturnPoint = getChangedVal;
        //    }
        //    else
        //    {
        //        consignSettleVM.EIMSInfo.UsingReturnPoint = 0m;
        //    }
        //    CalcSettleProducts();
        //}

        private void btnChooseEIMS_Click(object sender, RoutedEventArgs e)
        {
            //设置返点信息:
            if (!this.consignSettleVM.VendorInfo.SysNo.HasValue)
            {
                Window.Alert(ResConsignMaintain.ErrorMsg_CannotSetReturnPoint);
                return;
            }
            ConsignSettlementEIMSView eimsView = new ConsignSettlementEIMSView(consignSettleVM);
            eimsView.Dialog = Window.ShowDialog(ResConsignMaintain.InfoMsg_ReturnPointSearch, eimsView, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    ConsignSettlementEIMSInfo info = args.Data as ConsignSettlementEIMSInfo;
                    if (null != info)
                    {
                        this.consignSettleVM.EIMSInfo.ReturnPointName = info.ReturnPointName;
                        this.consignSettleVM.EIMSInfo.ReturnPointSysNo = info.ReturnPointSysNo;
                        this.consignSettleVM.EIMSInfo.RemnantReturnPoint = info.RemnantReturnPoint;
                        this.consignSettleVM.PM_ReturnPointSysNo = info.ReturnPointSysNo;
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
            //待审核状态，可以编辑:
            if (this.consignSettleVM != null && this.consignSettleVM.Status == SettleStatus.WaitingAudit)
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



                            for (int i = 0; i < consignSettleVM.ConsignSettlementItemInfoList.Count; i++)
                            {

                                var x = consignSettleVM.ConsignSettlementItemInfoList[i];

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
                consignSettleVM.EIMSInfo.UsingReturnPoint = getChangedVal;
            }
            else
            {
                consignSettleVM.EIMSInfo.UsingReturnPoint = 0m;
            }
            CalcSettleProducts();
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
                    foreach (var item in this.consignSettleVM.ConsignSettlementItemInfoList)
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
                    foreach (var item in this.consignSettleVM.ConsignSettlementItemInfoList)
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


        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (null == ConsignSysNo || consignSettleVM == null)
            {
                return;
            }
            //打印操作:
            if (consignSettleVM.LeaseType == VendorIsToLease.Lease)//转租赁
            {
                HtmlViewHelper.WebPrintPreview("PO", "ConsignLeasePrint", new Dictionary<string, string>() { { "ConsignSysNo", ConsignSysNo } });
            }
            else//代销
            {
                HtmlViewHelper.WebPrintPreview("PO", "ConsignUnLeasePrint", new Dictionary<string, string>() { { "ConsignSysNo", ConsignSysNo } });
            }
        }
    }
}
