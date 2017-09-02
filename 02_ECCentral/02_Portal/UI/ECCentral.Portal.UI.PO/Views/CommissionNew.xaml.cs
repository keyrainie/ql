using System;
using System.Linq;
using System.Windows;

using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.QueryFilter.PO;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CommissionNew : PageBase
    {
        public CommissionMasterVM m_masterVM;
        public VendorCommissionFacade m_serviceFacade;

        public CommissionNew()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            m_serviceFacade = new VendorCommissionFacade(this);
            this.DataContext = m_masterVM = new CommissionMasterVM();
            InitComboBoxData();
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //查询代收结算单相关:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Commission_Create))
            {
                this.btnSearch.IsEnabled = false;
            }
        }

        private void InitComboBoxData()
        {
            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidInput())
            {
                //加载佣金信息:
                m_serviceFacade.GetManualCommissionMaster(m_masterVM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    m_masterVM = args.Result.Convert<CommissionMaster, CommissionMasterVM>();
                    this.DataContext = m_masterVM;
                    if (this.m_masterVM.ItemList.Count > 0)
                    {
                        this.m_masterVM.ItemList.ForEach(x =>
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
                        btnCreate.IsEnabled = true;
                    }
                    BindItemGrid();
                });
            }
        }

        private bool ValidInput()
        {
            if (!this.m_masterVM.MerchantSysNo.HasValue)
            {
                Window.Alert(ResCommissionItemView.InfoMsg_ErrorVendorSelect);
                return false;
            }
            if (!this.m_masterVM.BeginDate.HasValue || !this.m_masterVM.EndDate.HasValue)
            {
                Window.Alert(ResCommissionItemView.InfoMsg_ErrorDateSelect);
                return false;
            }
            if (this.m_masterVM.BeginDate.Value > this.m_masterVM.EndDate.Value)
            {
                Window.Alert(ResCommissionItemView.InfoMsg_ErrorStartMoreThanEnd);
                return false;
            }
            return true;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (ValidInput())
            {
                btnCreate.IsEnabled = false;
                m_serviceFacade.CreateCommission(m_masterVM, (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Navigate(string.Format(ConstValue.PO_CommissionItemView, args.Result.SysNo));
                });
            }
        }

        private void BindItemGrid()
        {
            //绑定Item Grid:
            this.Grid_Sales.ItemsSource = this.Grid_Orders.ItemsSource = this.Grid_ShippingPrice.ItemsSource = this.m_masterVM.ItemList.Where(i => i.SaleRule != null);
            //佣金信息汇总:
            string summaryString = ResCommissionItemView.Label_CommissionSummary + Environment.NewLine;
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary2, m_masterVM.RentFee.Value.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary3, m_masterVM.SalesCommissionFee.Value.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary4, m_masterVM.OrderCommissionFee.Value.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary5, m_masterVM.DeliveryFee.Value.ToString("f2"));
            summaryString += string.Format(ResCommissionItemView.Label_CommissionSummary6, m_masterVM.TotalAmt.Value.ToString("f2"));
            this.txtTotalAmtAlert.Text = summaryString;
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
    }
}
