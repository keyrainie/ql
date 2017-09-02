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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 应付款-发票匹配审核查询
    /// </summary>
    [View]
    public partial class InvoiceInputQuery : PageBase
    {
        private InvoiceInputQueryVM queryVM;
        private InvoiceInputQueryVM lastQueryVM;
        private InvoiceInputFacade facade;
        private List<InvoiceInputVM> list;

        public InvoiceInputQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new InvoiceInputFacade(this);
            queryVM = new InvoiceInputQueryVM();
            this.QueryBuilder.DataContext = lastQueryVM = queryVM;
            LoadComboBoxData();
            base.OnPageLoad(sender, e);
            SetControlStatus();
        }

        private void SetControlStatus()
        {
            
            Button_BatchVPCancel.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInput_BatchVPCancel) ? true : false;
            Button_BatchSubmit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInput_BatchSubmit) ? true : false;
            Button_BatchCancel.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInput_BatchCancel) ? true : false;
            Button_BatchAudit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInput_BatchPass) ? true : false;
            Button_BatchRefuse.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInput_BatchRefuse) ? true : false;
            this.DataGrid_QueryResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInput_Export) ? true : false;
        }

        private void LoadComboBoxData()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_Invoice, "InvoiceInputComeFrom", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                Combox_ComeFrom.ItemsSource = args.Result;
            });
        }

        /// <summary>
        /// 录入发票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format(ConstValue.InvoiceInputMaintainUrlFormat, ""), null, true);
        }

        private void DataGrid_QueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.DataGrid_QueryResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }

            ColumnSet col = new ColumnSet(this.DataGrid_QueryResult);
            col.Remove("SapImportedStatusDesc");
            col.Insert(2, "Status", ResInvoiceInputQuery.Grid_Status, 10)
                .SetWidth("VendorName", 40);

            facade.ExportExcelFile(lastQueryVM, new ColumnSet[] { col });
        }

        private void DataGrid_QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ValidationManager.Validate(this.QueryBuilder);
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            facade.QueryInvoiceInput(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                list = DynamicConverter<InvoiceInputVM>.ConvertToVMList(args.Result.Rows);
                this.DataGrid_QueryResult.ItemsSource = list;
                this.DataGrid_QueryResult.TotalCount = args.Result.TotalCount;

                if (list != null && list.Count > 0)
                {
                    decimal totlePOAmtSum = 0;
                    decimal totleInvoiceAmt = 0;
                    list.ForEach(p =>
                    {
                        totlePOAmtSum += p.POAmtSum ?? 0;
                        totleInvoiceAmt += p.InvoiceAmt ?? 0;
                    });
                    Text_Statistic.Visibility = Visibility.Visible;
                    Text_Statistic.Text = string.Format(ResInvoiceInputQuery.Msg_Statistic,
                        decimal.Round(totlePOAmtSum, 2), decimal.Round(totleInvoiceAmt, 2));
                }
                else
                {
                    Text_Statistic.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.DataGrid_QueryResult.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                }
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            InvoiceInputVM vm = (sender as HyperlinkButton).DataContext as InvoiceInputVM;
            Window.Navigate(string.Format(ConstValue.InvoiceInputMaintainUrlFormat, vm.SysNo), null, true);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceInputQueryVM>(queryVM);

            this.DataGrid_QueryResult.Bind();
        }

        private List<InvoiceInputVM> GetAPInvoiceMasterList()
        {
            List<InvoiceInputVM> sysNoList = new List<InvoiceInputVM>();
            if (this.DataGrid_QueryResult.ItemsSource != null)
            {
                List<InvoiceInputVM> viewList = this.DataGrid_QueryResult.ItemsSource as dynamic;

                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        sysNoList.Add(view);
                    }
                }
            }
            return sysNoList;
        }

        private void Button_BatchVPCancel_Click(object sender, RoutedEventArgs e)
        {
            List<InvoiceInputVM> selectList = GetAPInvoiceMasterList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_SelectData);
                return;
            }
            var checkView = selectList.Where(x =>
                {
                    return x.IsVendorPortal != 1;
                }).ToList().Count;
            if (checkView > 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_IsNotVendorPortal);
                return;
            }

            checkView = selectList.Where(x =>
                {
                    return x.Status != APInvoiceMasterStatus.Origin;
                }).ToList().Count;
            if (checkView > 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_StatusIsNotOrigin);
                return;
            }
            List<int> li = selectList.Select(p => p.SysNo.Value).ToList();

            UCVPCancelReason uctl = new UCVPCancelReason(li);
            uctl.Dialog = this.Window.ShowDialog(ResInvoiceInputQuery.Dialog_VPCancelReason, uctl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.DataGrid_QueryResult.Bind();
                }
            });
        }

        private void Button_BatchSubmit_Click(object sender, RoutedEventArgs e)
        {
            List<InvoiceInputVM> selectList = GetAPInvoiceMasterList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_SelectData);
                return;
            }

            var checkView = selectList.Where(x =>
              {
                  return x.Status != APInvoiceMasterStatus.Origin;
              }).ToList().Count;
            if (checkView > 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_StatusIsNotOrigin);
                return;
            }
            List<int> li = selectList.Select(p => p.SysNo.Value).ToList();
            facade.BatchSubmit(li, (obj, args) =>
            {
                this.DataGrid_QueryResult.Bind();
            });
        }

        private void Button_BatchCancel_Click(object sender, RoutedEventArgs e)
        {
            List<InvoiceInputVM> selectList = GetAPInvoiceMasterList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_SelectData);
                return;
            }

            var checkView = selectList.Where(x =>
            {
                return x.Status != APInvoiceMasterStatus.NeedAudit;
            }).ToList().Count;
            if (checkView > 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_StatusIsNotNeedAudit);
                return;
            }
            List<int> li = selectList.Select(p => p.SysNo.Value).ToList();
            facade.BatchCancel(li, (obj, args) =>
            {
                this.DataGrid_QueryResult.Bind();
            });
        }

        private void Button_BatchAudit_Click(object sender, RoutedEventArgs e)
        {
            List<InvoiceInputVM> selectList = GetAPInvoiceMasterList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_SelectData);
                return;
            }

            var checkView = selectList.Where(x =>
            {
                return x.Status != APInvoiceMasterStatus.NeedAudit;
            }).ToList().Count;
            if (checkView > 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_StatusIsNotNeedAudit);
                return;
            }
            List<int> li = selectList.Select(p => p.SysNo.Value).ToList();
            facade.BatchAudit(li, (obj, args) =>
            {
                Window.Alert(args.Result);
                this.DataGrid_QueryResult.Bind();
            });
        }

        private void Button_BatchRefuse_Click(object sender, RoutedEventArgs e)
        {
            List<InvoiceInputVM> selectList = GetAPInvoiceMasterList();
            if (selectList.Count == 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_SelectData);
                return;
            }

            var checkView = selectList.Where(x =>
            {
                return x.Status != APInvoiceMasterStatus.NeedAudit;
            }).ToList().Count;
            if (checkView > 0)
            {
                Window.Alert(ResInvoiceInputQuery.Msg_StatusIsNotNeedAudit);
                return;
            }
            List<int> li = selectList.Select(p => p.SysNo.Value).ToList();
            facade.BatchRefuse(li, (obj, args) =>
            {
                this.DataGrid_QueryResult.Bind();
            });
        }
    }
}