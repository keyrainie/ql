using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.UserControls.DynamicQueryFilter.Invoice;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 分公司收款单查询页面
    /// </summary>
    [View(IsSingleton = true, NeedAccess = true, SingletonType = SingletonTypes.Url)]
    public partial class InvoiceQuery : PageBase
    {
        private InvoiceQueryVM queryVM;
        private InvoiceQueryVM lastQueryVM;
        private InvoiceFacade invoiceFacade;
        private OtherDomainDataFacade otherFacade;
        private Dictionary<string, UserControl> dynamicQueryFilters;

        public InvoiceQuery()
        {
            InitializeComponent();
            
            RegisterDynamicQueryFilters();
            InitData();
            Loaded += new RoutedEventHandler(InvoiceQuery_Loaded);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);
        }

        private void InvoiceQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(InvoiceQuery_Loaded);
            
            invoiceFacade = new InvoiceFacade(this);
            otherFacade = new OtherDomainDataFacade(this);
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            //默认选中第一个销售渠道
            this.cmbWebChannel.SelectedIndex = 0;

            //加载审核人列表
            otherFacade.GetBizOperationUser(new BizOperationUserQueryFilter()
            {
                BizTableName = "IPP3.dbo.Finance_SOIncome",
                UserType = BizOperationUserType.ConfirmUser,
                UserStatus = BizOperationUserStatus.Active,
                CompanyCode = CPApplication.Current.CompanyCode
            }, true, confirmList =>
            {
                queryVM.ConfirmerList = confirmList;
            });
        }

        private void RegisterDynamicQueryFilters()
        {
            dynamicQueryFilters = new Dictionary<string, UserControl>();
            dynamicQueryFilters.Add("SO", new DynamicFilterSO());
            dynamicQueryFilters.Add("RO", new DynamicFilterRO());
        }

        private void VerifyPermissions()
        {
            this.btnMerge.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceQuery_GetInvoiceAmount);
            this.dgInvoiceQueryResult.IsShowAllExcelExporter =  AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceQuery_Export);
        }

        private void InitData()
        {
            queryVM = new InvoiceQueryVM();
            SeachBuilder.DataContext = lastQueryVM = queryVM;
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgInvoiceQueryResult.ItemsSource as List<InvoiceVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        private void Hyperlink_CustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgInvoiceQueryResult.SelectedItem as InvoiceVM;
            if (cur != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, cur.CustomerSysNo), null, true);
            }
        }

        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            //根据不同的单据类型跳转到不同的单据维护界面
            var cur = this.dgInvoiceQueryResult.SelectedItem as InvoiceVM;
            if (cur != null)
            {
                switch (cur.OrderType.Value)
                {
                    case SOIncomeOrderType.SO:
                        //链接到订单维护页面
                        Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, cur.OrderSysNo), null, true);
                        break;

                    case SOIncomeOrderType.RO:
                        //链接到退款单维护页面
                        Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, cur.OrderSysNo), null, true);
                        break;

                    case SOIncomeOrderType.RO_Balance:
                        //链接到退款调整单查询页面
                        Window.Navigate(string.Format(ConstValue.RMA_RefundBalanceQueryUrl, cur.OrderSysNo), null, true);
                        break;
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceQueryVM>(queryVM);

                this.dgInvoiceQueryResult.Bind();
            }
        }

        private void dgInvoiceQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            invoiceFacade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                dgInvoiceQueryResult.ItemsSource = result.ResultList;
                dgInvoiceQueryResult.TotalCount = result.TotalCount;

                //计算统计值
                CalcStatisticInfo(result);
            });
        }

        private void Combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamicFilter.Children.Clear();
            if (e.AddedItems.Count > 0)
            {
                UserControl dynamicFilterUC = null;
                KeyValuePair<SOIncomeOrderType?, string> selectedVal = (KeyValuePair<SOIncomeOrderType?, string>)e.AddedItems[0];
                if (selectedVal.Key.HasValue)
                {
                    switch (selectedVal.Key.Value)
                    {
                        case SOIncomeOrderType.SO:
                            dynamicFilterUC = dynamicQueryFilters["SO"];
                            break;
                        case SOIncomeOrderType.RO:
                        case SOIncomeOrderType.RO_Balance:
                            dynamicFilterUC = dynamicQueryFilters["RO"];
                            break;
                    }
                }

                if (dynamicFilterUC != null)
                    dynamicFilter.Children.Add(dynamicFilterUC);
            }
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            var selectedList = GetSelectedInvoiceList();
            if (selectedList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }

            decimal totalInvoiceAmt = 0;
            decimal totalIncomeAmt = 0;
            decimal totalPrepayAmt = 0;
            string prompMessage = string.Empty;

            selectedList.ForEach(p =>
            {
                totalInvoiceAmt += p.InvoiceAmt ?? 0M;
                totalPrepayAmt += p.PrepayAmt ?? 0M;
                totalIncomeAmt += (p.InvoiceAmt ?? 0M) - (p.PrepayAmt ?? 0M);
            });

            if (!selectedList[0].OrderType.HasValue || selectedList[0].OrderType == SOIncomeOrderType.SO)
            {
                prompMessage = string.Format(ResInvoiceQuery.Message_TotalSOAmountFormat
                  , selectedList.Count
                  , ConstValue.Invoice_ToCurrencyString(totalInvoiceAmt)
                  , ConstValue.Invoice_ToCurrencyString(totalPrepayAmt)
                  , ConstValue.Invoice_ToCurrencyString(totalIncomeAmt));
            }
            else
            {
                prompMessage = string.Format(ResInvoiceQuery.Message_TotalAmountFormat
                    , selectedList.Count
                    , ConstValue.Invoice_ToCurrencyString(totalInvoiceAmt));
            }
            Window.Alert(prompMessage);
        }

        private void CalcStatisticInfo(InvoiceQueryResultVM queryResult)
        {
            decimal totalInvoiceAmtForSOOnly = 0M;
            decimal totalInvoiceAmt = 0M;
            decimal totalPrepayAmt = 0M;
            decimal totalIncomeAmt = 0M;
            decimal totalGiftCardPayAmt = 0M;
            decimal totalUnionAmt = 0M;
            decimal totalSOTotalAmt = 0M;

            decimal cInvoiceAmtForSOOnly = 0M;
            decimal cInvoiceAmt = 0M;
            decimal cPrepayAmt = 0M;
            decimal cIncomeAmt = 0M;
            decimal cGiftCardPayAmt = 0M;
            decimal cUnionAmt = 0M;
            decimal cSOTotalAmt = 0M;

            bool includeSO = false;

            if (queryResult.ResultList.Count > 0)
            {
                queryResult.ResultList.ForEach(p =>
                {
                    cInvoiceAmt += p.InvoiceAmt ?? 0M;
                    cPrepayAmt += p.PrepayAmt ?? 0M;
                    cIncomeAmt += p.IncomeAmt ?? 0M;
                    cUnionAmt += p.UnionAmt ?? 0M;
                    cSOTotalAmt += p.SOTotalAmt.HasValue ? p.SOTotalAmt.Value : 0M;
                    if (p.OrderType == SOIncomeOrderType.SO)
                    {
                        cGiftCardPayAmt += p.GiftCardPayAmt ?? 0M;
                        cInvoiceAmtForSOOnly += p.InvoiceAmt ?? 0M;
                    }
                });

                if (!queryVM.OrderType.HasValue || queryVM.OrderType == SOIncomeOrderType.SO)
                {
                    includeSO = true;

                    totalInvoiceAmt = queryResult.InvoiceAmt.TotalAmt;
                    totalPrepayAmt = queryResult.InvoiceAmt.PrepayAmt;
                    totalIncomeAmt = queryResult.InvoiceAmt.IncomeAmt;
                    totalGiftCardPayAmt = queryResult.InvoiceAmt.GiftCardPayAmt;
                    totalInvoiceAmtForSOOnly = queryResult.InvoiceAmt.TotalAmtForSOOnly;
                    totalUnionAmt = queryResult.InvoiceAmt.UnionAmt;
                    totalSOTotalAmt = queryResult.InvoiceAmt.TSOTotalAmt.HasValue ? queryResult.InvoiceAmt.TSOTotalAmt.Value : 0M;
                }
                else
                {
                    totalInvoiceAmt = queryResult.InvoiceAmt.TotalAmt;
                    totalInvoiceAmtForSOOnly = queryResult.InvoiceAmt.TotalAmtForSOOnly;
                    totalUnionAmt = queryResult.InvoiceAmt.UnionAmt;
                    totalSOTotalAmt = queryResult.InvoiceAmt.TSOTotalAmt.HasValue ? queryResult.InvoiceAmt.TSOTotalAmt.Value : 0M;
                }
            }

            // 税后=税前/1.17，税金=税前-税后
            decimal taxRateBase = ConstValue.Invoice_TaxRateBase;

            queryResult.Statistic.Add(new InvoiceQueryStatisticVM()
            {
                StatisticType = StatisticType.Total,
                InvoiceAmt = totalInvoiceAmt,
                InvoiceAmtWithTax = totalInvoiceAmtForSOOnly / taxRateBase,
                InvoiceTax = (includeSO) ? (totalInvoiceAmtForSOOnly - (totalInvoiceAmtForSOOnly / taxRateBase)) : 0.00M,
                PrepayAmt = totalPrepayAmt,
                IncomeAmt = totalIncomeAmt,
                GiftCardPayAmt = totalGiftCardPayAmt,
                GiftCardPayAmtWithTax = totalGiftCardPayAmt / taxRateBase,
                UnionAmt = totalUnionAmt,
                SOTotalAmt = totalSOTotalAmt,
            });
            queryResult.Statistic.Add(new InvoiceQueryStatisticVM()
            {
                StatisticType = StatisticType.Page,
                InvoiceAmt = cInvoiceAmt,
                InvoiceAmtWithTax = cInvoiceAmtForSOOnly / taxRateBase,
                InvoiceTax = (includeSO) ? cInvoiceAmtForSOOnly - (cInvoiceAmtForSOOnly / taxRateBase) : 0M,
                PrepayAmt = cPrepayAmt,
                IncomeAmt = cIncomeAmt,
                GiftCardPayAmt = cGiftCardPayAmt,
                GiftCardPayAmtWithTax = cGiftCardPayAmt / taxRateBase,
                UnionAmt = cUnionAmt,
                SOTotalAmt = cSOTotalAmt
            });
            this.svStatisticInfo.Visibility = Visibility.Visible;
            this.tbStatisticInfo.Text = queryResult.Statistic.ToStatisticText();
        }

        private List<InvoiceVM> GetSelectedInvoiceList()
        {
            var selectedInvoice = new List<InvoiceVM>();
            var itemSource = this.dgInvoiceQueryResult.ItemsSource as List<InvoiceVM>;
            if (itemSource != null)
            {
                //需要去掉SysNo和StockID相同的记录
                selectedInvoice = itemSource
                    .Where(w => w.IsChecked)
                    .Distinct(new InvoiceEqualityComparer())
                    .Select(s => s)
                    .ToList();
            }
            return selectedInvoice;
        }

        private void dgInvoiceQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.dgInvoiceQueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.dgInvoiceQueryResult);
            col.Add("OrderSysNo", ResInvoiceQuery.DataGrid_OrderSysNo);
            col.Add("PayTypeSysNo", ResInvoiceQuery.Grid_PayTypeSysNo);
            col.Add("OutUser", ResInvoiceQuery.DataGrid_OutUser);
            col.Add("OutTime", ResInvoiceQuery.DataGrid_OutTime);
            col.Add("InvoiceCreateTime", ResInvoiceQuery.DataGrid_InvoiceCreateTime);
            col.Add("IncomeAmt", ResInvoiceQuery.DataGrid_IncomeAmt);
            col.Add("SOSysNo", ResInvoiceQuery.DataGrid_SOSysNo);

            invoiceFacade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        private void Hyperlink_ShowFailedDetail_Click(object sender, RoutedEventArgs e)
        {
            InvoiceVM vm = (sender as HyperlinkButton).DataContext as InvoiceVM;
            if (vm.SapInFailedReason != null)
            {
                Window.Alert(vm.SapInFailedReason);
            }
        }

        private void lbtnInvoiceNo_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgInvoiceQueryResult.SelectedItem as InvoiceVM;
            if (cur != null)
            {
                new UCInvoiceNoEdit(cur.DeepCopy())
                    .ShowDialog(ResCommon.Message_ShowDlgDefaultTitle, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            this.dgInvoiceQueryResult.Bind();
                        }
                    });
            }
        }
    }
}