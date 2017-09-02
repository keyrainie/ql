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
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.PO.Views
{
     [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CollectionPaymentMaintain : PageBase
    {
        public string ConsignSysNo;
        public CollectionPaymentFacade serviceFacade;
        public CollectionPaymentInfoVM consignSettleVM;
        public List<CollectionPaymentItemInfoVM> mergedItemList;

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


        public CollectionPaymentMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            serviceFacade = new CollectionPaymentFacade(this);
            mergedItemList = new List<CollectionPaymentItemInfoVM>();
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
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_Settle))
            {
                btnSettle.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_CancelSettle))
            {
                btnCancelSettled.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_CancelAbandon))
            {
                btnCancelAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_Save))
            {
                btnSave.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_AddProduct))
            {
                btnAddSettleProducts.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_DeleteProduct))
            {
                btnDelSettleProducts.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_CollectionPayment_Edit_Audit))
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
                
                consignSettleVM = new CollectionPaymentInfoVM();
                consignSettleVM = EntityConverter<CollectionPaymentInfo, CollectionPaymentInfoVM>.Convert(args.Result, (s, t) =>
                {
                    
                    if (s.SourceStockInfo!=null )
                    {
                        t.StockSysNo = s.SourceStockInfo.SysNo;
                        t.StockID = s.SourceStockInfo.SysNo.Value.ToString();
                        t.StockName = s.SourceStockInfo.StockName.ToString();
                    }

                    t.PMSysNo = s.PMInfo.SysNo.HasValue ? s.PMInfo.SysNo.Value.ToString() : null;
                    t.SettleUserSysNo = s.SettleUser.SysNo;
                    t.SettleUserName = s.SettleUser.UserName;
                    t.SettleItems.ForEach(x =>
                    {
                        x.ConsignToAccLogInfo.SettleType = x.SettleType;
                        x.IsSettleCostTextBoxReadOnly = (t.Status != POCollectionPaymentSettleStatus.Origin  || x.SettleType != SettleType.O) ? true : false;
                        x.IsSettlePercentageTextBoxReadOnly = (t.Status != POCollectionPaymentSettleStatus.Origin || x.SettleType != SettleType.P) ? true : false;
                        x.SettlePercentageTextBoxVisibility = x.SettleType == SettleType.P ? Visibility.Visible : Visibility.Collapsed;
                    });
                    txtPaySettleCompany.Text = EnumConverter.GetDescription(t.VendorInfo.VendorBasicInfo.PaySettleCompany);
                });

                
                CountConsignSettleItems();
                this.DataContext = consignSettleVM;                
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

            List<CollectionPaymentItemInfoVM> cloneList = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<List<CollectionPaymentItemInfoVM>>(this.consignSettleVM.MergedSettleItems);
            List<CollectionPaymentItemInfoVM> mergedItemList = new List<CollectionPaymentItemInfoVM>();
            
            
            this.mergedItemList = cloneList;
            
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

           // SettleProductsTotalReturnPoints = consignSettleVM.EIMSInfo.UsingReturnPoint.HasValue ? consignSettleVM.EIMSInfo.UsingReturnPoint.Value : 0m;

            SettleProductsTotalSummary = SettleProductsTotalAmt - SettleProductsTotalReturnPoints;
            this.lblTotalStatistics.Text = string.Format(ResCollectionPaymentMaintain.InfoMsg_TotalAmt, SettleProductsTotalAmt.ToString("f2"),  SettleProductsTotalSummary.ToString("f2"));


            consignSettleVM.TotalAmt = totalCountMany;
            consignSettleVM.CreateCostTotalAmt = totalCreateCost;
            consignSettleVM.Difference = totalDiffAmt;
            consignSettleVM.RateMarginCount = totalMarginRate;

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
                    CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.Settle(getSettlementInfo, (obj2, args2) =>
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
                    CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.CancelSettle(getSettlementInfo, (obj2, args2) =>
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
            if (this.consignSettleVM.SettleItems.Where(x => x.SettleSysNo != -1).Count() <= 0)
            {
                Window.Alert("请先选择结算商品!");
                return;
            }
            #endregion
            //保存操作
            CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
            Window.Confirm(ResConsignMaintain.ConfirmMsg_Save, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    //保存PM高级权限，用于业务端验证
                    getSettlementInfo.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
                    //代销商品规则检测
                    getSettlementInfo.SettleItems.ForEach(item =>
                    {
                        if (item.SettleRuleSysNo.HasValue && item.Cost != item.SettlePrice)
                        {
                            item.SettleRuleSysNo = null;
                            item.SettleRuleName = null;
                        }
                    });
                    serviceFacade.Update(getSettlementInfo, (obj2, args2) =>
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
                    CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.Abandon(getSettlementInfo, (obj2, args2) =>
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
                    CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.CancelAbandon(getSettlementInfo, (obj2, args2) =>
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
                    CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
                    serviceFacade.CancelAudited(getSettlementInfo, (obj2, args2) =>
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
                    CollectionPaymentInfo getSettlementInfo = BuildActionEntity();
                    
                    //getSettlementInfo.AuditUser=CPApplication.Current.
                    serviceFacade.Audit(getSettlementInfo, (obj2, args2) =>
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
            if (!this.consignSettleVM.StockSysNo.HasValue)
            {
                Window.Alert(ResConsignNew.ErrorMsg_Stock);
                return;
            }
            if (!this.consignSettleVM.VendorInfo.SysNo.HasValue || string.IsNullOrEmpty(this.consignSettleVM.VendorInfo.VendorBasicInfo.VendorNameLocal))
            {

                Window.Alert(ResConsignNew.ErrorMsg_Vendor);
                return;
            }
            CollectionPaymentProductsQuery queryDialog = new CollectionPaymentProductsQuery
(null, this.consignSettleVM.StockSysNo, this.consignSettleVM.StockName, this.consignSettleVM.VendorInfo.SysNo, this.consignSettleVM.VendorInfo.VendorBasicInfo.VendorNameLocal, this.consignSettleVM.SettleItems);
            queryDialog.Dialog = Window.ShowDialog(ResConsignNew.InfoMsg_AddTitle, queryDialog, (obj, args) =>
            {
                if (queryDialog.Dialog != null && queryDialog.Dialog.ResultArgs.DialogResult == DialogResultType.OK && queryDialog.Dialog.ResultArgs.Data != null)
                {
                    //清空ViewModel ，重新绑定Items , 并刷新统计String:
                    List<CollectionPaymentItemInfoVM> getNewVMList = args.Data as List<CollectionPaymentItemInfoVM>;
                    if (null != getNewVMList)
                    {
                        this.consignSettleVM.SettleItems = getNewVMList;
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
            foreach (CollectionPaymentItemInfoVM item in consignSettleVM.SettleItems)
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

                        foreach (var item in this.consignSettleVM.SettleItems)
                        {
                            //与返点没有关系
                            if (item.ProductID == x.ProductID
                                && item.ConsignToAccLogInfo.CreateCost == x.ConsignToAccLogInfo.CreateCost
                                && item.ConsignToAccLogInfo.SettleType == x.ConsignToAccLogInfo.SettleType
                                && item.ConsignToAccLogInfo.SalePrice == x.ConsignToAccLogInfo.SalePrice
                                && item.ConsignToAccLogInfo.StockSysNo == x.ConsignToAccLogInfo.StockSysNo
                             && item.SettleRuleSysNo == x.SettleRuleSysNo
                                && item.SettleRuleSysNo==x.SettleRuleSysNo
                                
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
       
        #endregion

        /// <summary>
        /// 根据单据当前的状态,显示操作的按钮
        /// </summary>
        /// <param name="requestStatus"></param>
        private void ShowActionButton(POCollectionPaymentSettleStatus requestStatus)
        {
            switch (requestStatus)
            {
                //初始状态， 显示保存,作废, 添加结算商品，删除结算商品 按钮:
                case POCollectionPaymentSettleStatus.Origin:
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
                   // this.SettleProductsQueryResultGrid.Columns[11].Visibility = Visibility.Collapsed;
                    //this.SettleProductsQueryResultGrid.Columns[12].Visibility = Visibility.Visible;
                    this.btnAudit.Visibility = Visibility.Visible;
                    break;
                //已审核状态，显示 结算，取消审核 按钮:
                case POCollectionPaymentSettleStatus.Audited:
                    this.btnSettle.Visibility = Visibility.Visible;
                    this.btnCancelAudit.Visibility = Visibility.Visible;
                    this.ucVendor.IsAllowVendorSelect = false;
                    this.btnAudit.Visibility = Visibility.Collapsed;
                    break;
                //已结算，显示 取消结算 按钮:
                case POCollectionPaymentSettleStatus.Settled:
                    this.btnCancelSettled.Visibility = Visibility.Visible;
                    this.ucVendor.IsAllowVendorSelect = false;
                    this.btnAudit.Visibility = Visibility.Collapsed;
                    break;
                //作废状态， 显示 取消作废 按钮:
                case POCollectionPaymentSettleStatus.Abandon:
                    this.btnCancelAbandon.Visibility = Visibility.Visible;
                    this.ucVendor.IsAllowVendorSelect = false;
                    this.btnAudit.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }


        private CollectionPaymentInfo BuildActionEntity()
        {

            return EntityConverter<CollectionPaymentInfoVM, CollectionPaymentInfo>.Convert(consignSettleVM, (s, t) =>
            {
              
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

                t.CurrentUserSysNo = CPApplication.Current.LoginUser.UserSysNo;
                
               
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
                    foreach (var item in this.consignSettleVM.SettleItems)
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
