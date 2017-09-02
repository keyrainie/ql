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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CommissionItemView : PageBase
    {
        public string CommissionSysNo;
        public CommissionMasterVM masterVM;
        public VendorCommissionFacade serviceFacade;

        public CommissionItemView()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            masterVM = new CommissionMasterVM();
            serviceFacade = new VendorCommissionFacade(this);

            CommissionSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(CommissionSysNo))
            {
                BindCommissionItems();
            }
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //页面权限控制:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Commission_Close))
            {
                this.btnClose.IsEnabled = false;
            }
        }

        private void BindCommissionItems()
        {
            //加载佣金信息:
            serviceFacade.GetCommissionInfoBySysNo(CommissionSysNo, (obj, args) =>
            {

                if (args.FaultsHandle())
                {
                    return;
                }
                this.masterVM = EntityConverter<CommissionMaster, CommissionMasterVM>.Convert(args.Result);
                this.DataContext = masterVM;
                if (null != this.masterVM.ItemList)
                {
                    this.masterVM.ItemList.ForEach(x =>
                    {
                        if (x.SaleRule != null)
                        {
                            x.SaleRuleDisplayString = string.Format(ResCommissionItemView.Label_SaleRuleDisplayString, x.SaleRule.MinCommissionAmt.Value.ToString("f2"));
                            x.SaleRule.StagedSaleRuleItems.ForEach(j =>
                            {
                                if (j.StartAmt == 0.00m && j.EndAmt == 0.00m)
                                {
                                    x.SaleRuleDisplayString += "\r\n" + string.Format(ResCommissionItemView.Label_SaleRuleDisplayString2, j.Percentage.Value.ToString("f2"));
                                }
                                else if (j.StartAmt == 0.00m)
                                {
                                    x.SaleRuleDisplayString += "\r\n" + string.Format(ResCommissionItemView.Label_SaleRuleDisplayString3, j.EndAmt.Value.ToString("f2"), j.Percentage.Value.ToString("f2"));
                                }
                                else if (j.EndAmt == 0.00m)
                                {
                                    x.SaleRuleDisplayString += "\r\n" + string.Format(ResCommissionItemView.Label_SaleRuleDisplayString4, j.StartAmt.Value.ToString("f2"), j.Percentage.Value.ToString("f2"));
                                }
                                else
                                {
                                    x.SaleRuleDisplayString += "\r\n" + string.Format(ResCommissionItemView.Label_SaleRuleDisplayString5, j.StartAmt.Value.ToString("f2"), j.EndAmt.Value.ToString("f2"), j.Percentage.Value.ToString("f2"));
                                }
                            });
                        }

                    });
                }

                BindItemGrid();
            });
        }

        private void BindItemGrid()
        {
            //绑定Item Grid:
            this.Grid_Orders.Bind();
            this.Grid_Sales.Bind();
            this.Grid_ShippingPrice.Bind();
            //佣金信息汇总:
            decimal totalSaleAmt = this.masterVM.ItemList.Where(i => i.CommissionType == VendorCommissionItemType.SAC).Sum(p => p.SalesCommissionFee??0);
            decimal totalOrderAmt = this.masterVM.ItemList.Where(i => i.CommissionType == VendorCommissionItemType.SOC).Sum(p => p.TotalOrderCommissionFee ?? 0);
            decimal totalShippingAmt = this.masterVM.ItemList.Where(i => i.CommissionType == VendorCommissionItemType.DEF).Sum(p => (p.DeliveryFee ?? 0) * (p.TotalQty ?? 0));
           
            string summaryString = ResCommissionItemView.Label_CommissionSummary + Environment.NewLine;
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary2, masterVM.RentFee.Value.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary3, totalSaleAmt.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary4, totalOrderAmt.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary5, totalShippingAmt.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary6, (masterVM.RentFee.Value + totalSaleAmt + totalOrderAmt + totalShippingAmt).ToString("f2"));
            this.txtTotalAmtAlert.Text = summaryString;
        }

        #region [Events]
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //关闭结算单:
            Window.Confirm(ResCommissionItemView.ConfirmMsg_Close, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    CommissionMaster request = EntityConverter<CommissionMasterVM, CommissionMaster>.Convert(masterVM);
                    serviceFacade.CloseCommission(request, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResCommissionItemView.InfoMsg_AlertTitle, ResCommissionItemView.InfoMsg_CloseSuccess, MessageType.Information, (obj3, args3) =>
                        {
                            if (args3.DialogResult == DialogResultType.Cancel)
                            {
                                Window.Navigate("/ECCentral.Portal.UI.PO/CommissionQuery", false);
                            }
                        });
                    });
                }
            });
        }

        private void Grid_Sales_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //销售提成:
            this.Grid_Sales.ItemsSource = this.masterVM.ItemList.Where(i => i.CommissionType == VendorCommissionItemType.SAC && i.SaleRule != null);
        }
        private void Grid_Orders_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //订单提成:
            this.Grid_Orders.ItemsSource = this.masterVM.ItemList.Where(i => i.CommissionType == VendorCommissionItemType.SOC && i.SaleRule != null);
        }


        private void Grid_ShippingPrice_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //配送费用:
            this.Grid_ShippingPrice.ItemsSource = this.masterVM.ItemList.Where(i => i.CommissionType == VendorCommissionItemType.DEF && i.SaleRule != null);
        }


        private void hpDetailView_ShippingPrice_Click(object sender, RoutedEventArgs e)
        {
            //配送费用:
            CommissionItemVM itemVM = this.Grid_ShippingPrice.SelectedItem as CommissionItemVM;
            if (null != itemVM)
            {
                CommissionItemDetailView viewCtrl = new CommissionItemDetailView(itemVM, VendorCommissionItemType.DEF);
                Window.ShowDialog(ResCommissionItemView.InfoMsg_ShippingTitle, viewCtrl, null, new Size(700, 500));
            }
        }

        private void hpDetailView_Orders_Click(object sender, RoutedEventArgs e)
        {
            //订单提成:
            CommissionItemVM itemVM = this.Grid_Orders.SelectedItem as CommissionItemVM;
            if (null != itemVM)
            {
                CommissionItemDetailView viewCtrl = new CommissionItemDetailView(itemVM, VendorCommissionItemType.SOC);
                Window.ShowDialog(ResCommissionItemView.InfoMsg_OrderTitle, viewCtrl, null, new Size(700, 500));
            }
        }

        private void hpDetailView_Sales_Click(object sender, RoutedEventArgs e)
        {
            //销售提成:
            CommissionItemVM itemVM = this.Grid_Sales.SelectedItem as CommissionItemVM;
            if (null != itemVM)
            {
                CommissionItemDetailView viewCtrl = new CommissionItemDetailView(itemVM, VendorCommissionItemType.SAC);
                Window.ShowDialog(ResCommissionItemView.InfoMsg_SaleTitle, viewCtrl, null, new Size(700, 500));
            }
        }
        #endregion

    }

}
