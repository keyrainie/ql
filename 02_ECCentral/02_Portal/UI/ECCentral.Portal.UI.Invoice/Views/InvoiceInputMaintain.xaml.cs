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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 应付款发票维护
    /// </summary>
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class InvoiceInputMaintain : PageBase
    {
        private InvoiceInputMaintainVM vm;
        private InvoiceInputFacade facade;

        private InvoiceInputMaintainVM lastVM;

        List<ValidationEntity> valPOInput;
        List<ValidationEntity> valInvInput_InvNo;
        List<ValidationEntity> valInvInput_InvDate;
        List<ValidationEntity> valInvInput_Money;

        SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        public InvoiceInputMaintain()
        {
            InitializeComponent();

        }



        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new InvoiceInputFacade(this);
            VerifyPermission();

            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                int sysNo = 0;
                int.TryParse(this.Request.Param, out sysNo);
                facade.LoadAPInvoiceWithItemsBySysNo(sysNo, (obj, args) =>
                {
                    vm = args.Result.Convert<APInvoiceInfo, InvoiceInputMaintainVM>();
                    if (vm != null)
                    {
                        this.DataContext = vm;
                        lastVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceInputMaintainVM>(vm);

                        OperationControlStatusHelper.GetChildObjects<CheckBox>(DataGrid_POItem_Result, "chkbx_SelectAll")[0].IsChecked = true;
                        CheckBox_Click(OperationControlStatusHelper.GetChildObjects<CheckBox>(DataGrid_POItem_Result, "chkbx_SelectAll")[0], null);
                        //OperationControlStatusHelper.GetChildObjects<CheckBox>(DataGrid_POItem_Result, "chkbx_SelectAll")[0]

                        OperationControlStatusHelper.GetChildObjects<CheckBox>(DataGrid_InvoiceItem_Result, "chkbx_SelectAll")[0].IsChecked = true;
                        CheckBox_Click(OperationControlStatusHelper.GetChildObjects<CheckBox>(DataGrid_InvoiceItem_Result, "chkbx_SelectAll")[0], null);
                    }
                    else
                    {
                        Window.Alert(ResInvoiceInputMaintain.Msg_NoData);
                    }
                    SetControlStatus();

                });
            }
            else
            {
                this.DataContext = vm = new InvoiceInputMaintainVM();
                vm.InvoiceDate = DateTime.Today;
                lastVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceInputMaintainVM>(vm);
                SetControlStatus();
            }
            BuildValidateCondition();
        }

        private void VerifyPermission()
        {
            //this.btnSave.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Create);

            //this.btn
        }

        private void BuildValidateCondition()
        {
            valPOInput = new List<ValidationEntity>();
            valPOInput.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.Text_PONo.Text.Trim(), ResInvoiceInputMaintain.Msg_ValidatePONo));
            valInvInput_InvNo = new List<ValidationEntity>();
            valInvInput_InvNo.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.Text_InvoiceNo.Text.Trim(), ResInvoiceInputMaintain.Msg_ValidateInvoiceNo));
            valInvInput_InvDate = new List<ValidationEntity>();
            valInvInput_InvDate.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.DatePicker_InvoiceDate.Text.Trim(), ResInvoiceInputMaintain.Msg_ValidateInvoiceDate));
            valInvInput_Money = new List<ValidationEntity>();
            valInvInput_Money.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.Text_ItemInvoiceAmt.Text.Trim(), ResInvoiceInputMaintain.Msg_ValidateItemInvoiceAmt));
            valInvInput_Money.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^(-)?(\d{1,9})?(\.(\d){1,2})?$", ResInvoiceInputMaintain.Msg_ValidateMoney));
        }

        private bool ValidatePOInput()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.Text_PONo, valPOInput))
            {
                ret = false;
            }
            return ret;
        }

        private bool ValidateInvInput()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.Text_InvoiceNo, valInvInput_InvNo) ||
                !ValidationHelper.Validation(this.DatePicker_InvoiceDate, valInvInput_InvDate) ||
                !ValidationHelper.Validation(this.Text_ItemInvoiceAmt, valInvInput_Money))
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 根据状态及权限设置控件初始状态
        /// </summary>
        public void SetControlStatus()
        {
            if (vm.SysNo > 0)
            {

                if (vm.VendorPortalSysNo.HasValue)
                {
                    Grid_InputInvoiceItem.Visibility = Grid_InputPOItem.Visibility = Visibility.Collapsed;
                    btnDiffCalc.IsEnabled = false;
                    btnVendorPotalCancel.Visibility = Visibility.Visible;
                }

                if (vm.Status == APInvoiceMasterStatus.Origin)
                {
                    if (vm.VendorPortalSysNo == null)
                    {
                        btnDiffCalc.IsEnabled = (vm.POItemList.Count > 0 && vm.InvoiceItemList.Count > 0) ? true : false;
                        Grid_InputInvoiceItem.Visibility = Grid_InputPOItem.Visibility = Visibility.Visible;
                    }
                    btnVendorPotalCancel.IsEnabled
                            = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_VPCancel) ? true : false;
                    btnSubmit.IsEnabled
                        = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Submit) ? true : false;
                    btnSave.IsEnabled = btnCancel.IsEnabled = btnPass.IsEnabled = btnhRefuse.IsEnabled = false;
                }
                else if (vm.Status == APInvoiceMasterStatus.NeedAudit)
                {
                    btnCancel.IsEnabled
                        = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Cancel) ? true : false;
                    btnPass.IsEnabled
                        = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Pass) ? true : false;
                    btnhRefuse.IsEnabled
                        = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Refuse) ? true : false;
                    CheckBox_DiffAmtRight.IsEnabled = true;
                    btnDiffCalc.IsEnabled = btnSave.IsEnabled = btnVendorPotalCancel.IsEnabled = btnSubmit.IsEnabled = false;
                }
                else
                {
                    Grid_InputInvoiceItem.Visibility = Grid_InputPOItem.Visibility = Visibility.Collapsed;
                    VendorPicker.IsEnabled
                        = btnDiffCalc.IsEnabled
                        = btnSave.IsEnabled
                        = btnVendorPotalCancel.IsEnabled
                        = btnSubmit.IsEnabled
                        = btnhRefuse.IsEnabled
                        = btnCancel.IsEnabled
                        = btnPass.IsEnabled
                        = CheckBox_IsDataRight.IsEnabled
                        = CheckBox_DiffAmtRight.IsEnabled
                        = false;
                }
            }
            else
            {
                VendorPicker.IsEnabled = true;
                Grid_InputInvoiceItem.Visibility = Grid_InputPOItem.Visibility = Visibility.Visible;
            }

        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            string commandParameter = ckb.CommandParameter.ToString();
            if (commandParameter == "POItem" || commandParameter == "POItemsAll")
            {
                if (vm.POItemList != null)
                {
                    if (commandParameter == "POItemsAll")
                    {
                        dynamic viewList = vm.POItemList as dynamic;
                        foreach (var view in viewList)
                        {
                            view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                        }
                    }
                    int count = vm.POItemList.Where(p => p.IsChecked).Count();
                    decimal poAmt = vm.POItemList.Where(p => p.IsChecked).Sum(x => x.PoAmt.Value);
                    decimal eimsAmt = vm.POItemList.Where(p => p.IsChecked).Sum(x => x.EIMSAmt.HasValue ? x.EIMSAmt.Value : 0);
                    decimal paymentAmt = vm.POItemList.Where(p => p.IsChecked).Sum(x => x.PaymentAmt.Value);
                    Text_POItemsStatistic.Text = string.Format(ResInvoiceInputMaintain.Label_POItemsStatistic, count, decimal.Round(poAmt, 2), decimal.Round(eimsAmt, 2), decimal.Round(paymentAmt, 2));
                }
            }
            else if (commandParameter == "InvoiceItem" || commandParameter == "InvoiceItemsAll")
            {
                if (vm.POItemList != null)
                {
                    if (commandParameter == "InvoiceItemsAll")
                    {
                        dynamic viewList = vm.InvoiceItemList as dynamic;
                        foreach (var view in viewList)
                        {
                            view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                        }
                    }
                    int count = vm.InvoiceItemList.Where(p => p.IsChecked).Count();
                    decimal invoiceAmt = vm.InvoiceItemList.Where(p => p.IsChecked).Sum(x => x.InvoiceAmt.Value);
                    decimal invoiceTaxAmt = vm.InvoiceItemList.Where(p => p.IsChecked).Sum(x => x.InvoiceTaxAmt.Value);
                    decimal invoiceNetAmt = vm.InvoiceItemList.Where(p => p.IsChecked).Sum(x => x.InvoiceNetAmt.Value);
                    this.Text_InvoiceItemsStatistic.Foreground = BlackBrush;
                    Text_InvoiceItemsStatistic.Text = string.Format(ResInvoiceInputMaintain.Label_InvoiceItemsStatistic, count, decimal.Round(invoiceAmt, 2), decimal.Round(invoiceTaxAmt, 2), decimal.Round(invoiceNetAmt, 2));
                }
            }
        }


        /// <summary>
        /// 根据条件加载供应商未录入的POItems
        /// 1.必须选择供应商。
        /// 2.可选条件--加载日期起始。
        /// 3.加载时会保留原有页面中的数据--只会更新POItems表，且保留原表中的数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_LoadPOItems_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.VendorSysNo.HasValue)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_ValidateVendor);
                return;
            }
            APInvoiceItemInputEntity request = new APInvoiceItemInputEntity();
            request.CompanyCode = CPApplication.Current.CompanyCode;
            request.VendorSysNo = vm.VendorSysNo;
            request.POItemList = vm.ConvertVM<InvoiceInputMaintainVM, APInvoiceInfo>().POItemList;
            request.PODateFrom = vm.PODateFrom;
            facade.LoadNotInputPOItems(request, (obj, args) =>
            {
                APInvoiceInfo info = new APInvoiceInfo();
                info.POItemList = args.Result;
                InvoiceInputMaintainVM resultVM = info.Convert<APInvoiceInfo, InvoiceInputMaintainVM>();
                vm.POItemList = resultVM.POItemList;
                if (vm.POItemList.Count > 0)
                {
                    VendorPicker.IsEnabled = false;
                    this.Text_POItemsStatistic.Foreground = BlackBrush;
                    this.Text_POItemsStatistic.Text = ResInvoiceInputMaintain.Msg_LoadSuccess;
                }
                else
                {
                    VendorPicker.IsEnabled = true;
                    Window.Alert(ResInvoiceInputMaintain.Msg_EmptyPoItems);
                }
                if (vm.POItemList.Count > 0 && vm.InvoiceItemList.Count > 0)
                {
                    btnDiffCalc.IsEnabled = true;
                }
            });


        }

        /// <summary>
        /// 录入PoItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_InputPoItem_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePOInput())
            {
                return;
            }
            APInvoiceItemInputEntity request = new APInvoiceItemInputEntity();
            request.ItemsNoList = vm.POItemNoList;
            request.OrderType = vm.OrderType;
            request.VendorSysNo = vm.VendorSysNo;
            request.VendorName = vm.VendorName;
            request.CompanyCode = CPApplication.Current.CompanyCode;
            request.POItemList = vm.ConvertVM<InvoiceInputMaintainVM, APInvoiceInfo>().POItemList;

            facade.InputPoItem(request, (obj, args) =>
            {
                APInvoiceInfo info = new APInvoiceInfo();
                info.POItemList = args.Result.poItemList;
                info.VendorName = args.Result.VendorName;
                info.VendorSysNo = args.Result.VendorSysNo;

                InvoiceInputMaintainVM resultVM = info.Convert<APInvoiceInfo, InvoiceInputMaintainVM>();
                vm.POItemList = resultVM.POItemList;
                vm.VendorSysNo = resultVM.VendorSysNo;
                vm.VendorName = resultVM.VendorName;
                if (!string.IsNullOrEmpty(args.Result.ErrorMsg))
                {
                    this.Text_POItemsStatistic.Foreground = RedBrush;
                    this.Text_POItemsStatistic.Text = args.Result.ErrorMsg;
                    //Window.Alert(args.Result.ErrorMsg);
                }
                else
                    this.Text_POItemsStatistic.Text = string.Empty;

                if (vm.POItemList.Count > 0)
                {
                    VendorPicker.IsEnabled = false;
                }
                else
                {
                    VendorPicker.IsEnabled = true;
                }
                if (vm.POItemList.Count > 0 && vm.InvoiceItemList.Count > 0)
                {
                    btnDiffCalc.IsEnabled = true;
                }
            });

        }
        /// <summary>
        /// 录入InvoiceItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_InputInvoiceItem_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInvInput())
            {
                if (this.vm.ItemInvoiceAmt == null)
                    this.Text_ItemInvoiceAmt.Focus();
                return;
            }
            if (!vm.VendorSysNo.HasValue)
            {
                this.Text_InvoiceItemsStatistic.Foreground = RedBrush;
                this.Text_InvoiceItemsStatistic.Text = ResInvoiceInputMaintain.Msg_Tips + ResInvoiceInputMaintain.Msg_ValidateVendor;
                //Window.Alert(ResInvoiceInputMaintain.Msg_ValidateVendor);
                return;
            }
            APInvoiceItemInputEntity request = new APInvoiceItemInputEntity();
            request.ItemsNoList = vm.InvoiceItemNoList;
            request.InvoiceDate = vm.InvoiceDate;
            request.InvoiceAmt = vm.ItemInvoiceAmt;
            request.VendorSysNo = vm.VendorSysNo;
            request.VendorName = vm.VendorName;
            request.CompanyCode = CPApplication.Current.CompanyCode;
            request.InvoiceItemList = vm.ConvertVM<InvoiceInputMaintainVM, APInvoiceInfo>().InvoiceItemList;

            facade.InputInvoiceItem(request, (obj, args) =>
            {
                APInvoiceInfo info = new APInvoiceInfo();
                info.InvoiceItemList = args.Result.invoiceItemList;

                InvoiceInputMaintainVM resultVM = info.Convert<APInvoiceInfo, InvoiceInputMaintainVM>();
                vm.InvoiceItemList = resultVM.InvoiceItemList;

                if (!string.IsNullOrEmpty(args.Result.ErrorMsg))
                {
                    //Window.Alert(args.Result.ErrorMsg);
                    this.Text_InvoiceItemsStatistic.Foreground = RedBrush;
                    this.Text_InvoiceItemsStatistic.Text = args.Result.ErrorMsg;
                }
                else
                    this.Text_InvoiceItemsStatistic.Text = string.Empty;
                if (vm.POItemList.Count > 0 && vm.InvoiceItemList.Count > 0)
                {
                    btnDiffCalc.IsEnabled = true;
                }
                this.Text_InvoiceNo.Focus();
            });
        }



        /// <summary>
        /// 删除未选中的Items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_DeleteItems_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hylb = sender as HyperlinkButton;
            string commandStr = hylb.CommandParameter.ToString();
            if (commandStr == "POItem")
            {
                vm.POItemList = vm.POItemList.Where(p => p.IsChecked).ToList();
                if (vm.POItemList.Count > 0)
                {
                    VendorPicker.IsEnabled = false;
                }
                else
                {
                    VendorPicker.IsEnabled = true;
                }
            }
            else if (commandStr == "InvoiceItem")
            {
                vm.InvoiceItemList = vm.InvoiceItemList.Where(p => p.IsChecked).ToList();
            }

        }
        /// <summary>
        /// 差异计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDiffCalc_Click(object sender, RoutedEventArgs e)
        {
            if (vm.POItemList.Where(p => p.IsChecked).Count() == 0 || vm.InvoiceItemList.Where(x => x.IsChecked).Count() == 0)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_ValidateSelectItems);
                return;
            }
            decimal totlePOAmtSum = 0;
            decimal totleInvoiceAmt = 0;
            decimal totleInvoiceTaxAmt = 0;
            vm.POItemList.Where(p => p.IsChecked).ForEach(p =>
            {
                totlePOAmtSum += p.PoAmt ?? 0;
            });
            vm.InvoiceItemList.Where(p => p.IsChecked).ForEach(p =>
                    {
                        totleInvoiceAmt += p.InvoiceAmt ?? 0;
                        totleInvoiceTaxAmt += p.InvoiceTaxAmt ?? 0;
                    });
            vm.InvoiceAmt = decimal.Round(totleInvoiceAmt, 2);
            vm.InvoiceTaxAmt = decimal.Round(totleInvoiceTaxAmt, 2);
            vm.DiffTaxAmt = decimal.Round(totlePOAmtSum - totleInvoiceAmt, 2);
            //根据不同的值设置差异类型的选中状态
            if (vm.DiffTaxAmt < -10)
            {
                vm.DiffTaxTreatmentType = InvoiceDiffType.MultiInvoicingAndTaxDiff;
            }
            else if (vm.DiffTaxAmt >= -10 && vm.DiffTaxAmt < 0)
            {
                vm.DiffTaxTreatmentType = InvoiceDiffType.MultiInvoicingAndExpensing;
            }
            else if (vm.DiffTaxAmt == 0)
            {
                vm.DiffTaxTreatmentType = InvoiceDiffType.Coincident;
            }
            else if (vm.DiffTaxAmt > 0 && vm.DiffTaxAmt <= 10)
            {
                vm.DiffTaxTreatmentType = InvoiceDiffType.LessInvoicingAndExpensing;
            }
            else
            {
                vm.DiffTaxTreatmentType = InvoiceDiffType.LessInvoicingAndTaxDiff;
            }

            //根据isSelected排序
            vm.POItemList = vm.POItemList
                    .OrderByDescending(x => x.IsChecked)
                    .OrderBy(x => x.OrderType)
                    .ThenBy(x => x.PoBaselineDate).ToList();
            vm.InvoiceItemList = vm.InvoiceItemList
                .OrderByDescending(x => x.IsChecked)
                .OrderBy(x => x.InvoiceNo)
                .ThenBy(x => x.InvoiceDate).ToList();

            //差异为零时清除未选择项
            if (vm.DiffTaxAmt == 0)
            {
                vm.POItemList = vm.POItemList.Where(p => p.IsChecked).ToList();
                vm.InvoiceItemList = vm.InvoiceItemList.Where(p => p.IsChecked).ToList();
            }

            //返回发票概况
            var selectInvoiceList = vm.InvoiceItemList.Where(p => p.IsChecked).ToList();
            vm.Memo = string.Format(ResInvoiceInputMaintain.Label_Memo
                , selectInvoiceList.Min(x => Convert.ToInt32(x.InvoiceNo))
                , selectInvoiceList.Max(x => Convert.ToInt32(x.InvoiceNo))
                , selectInvoiceList.Count);
            lastVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<InvoiceInputMaintainVM>(vm);
            CheckBox_IsDataRight.IsEnabled = Text_DiffMemo.IsEnabled = true;
            vm.IsDataRight = false;
            btnSave.IsEnabled = btnSubmit.IsEnabled = false;
        }



        /// <summary>
        /// 创建/更新PO单关联发票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (vm.VendorSysNo == null)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_ValidateVendor);
                return;
            }
            if (vm.POItemList.Count == 0)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NoPOItems);
                return;
            }
            if (vm.InvoiceItemList.Count == 0)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NoInvItems);
                return;
            }
            if (vm.DiffTaxTreatmentType == null)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NeedDiffCal);
                return;
            }
            if (!Compare(vm, lastVM))
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NeedReDiffCal);
                return;
            }
            //只录入选中的单据
            vm.POItemList = vm.POItemList.Where(p => p.IsChecked).ToList();
            vm.InvoiceItemList = vm.InvoiceItemList.Where(p => p.IsChecked).ToList();
            APInvoiceInfo data = vm.ConvertVM<InvoiceInputMaintainVM, APInvoiceInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            if (vm.SysNo > 0)
            {
                facade.UpdateApInvoiceInfo(data, (obj, args) =>
                {
                    Window.Alert(ResInvoiceInputMaintain.Msg_UpdateSuccess);
                });
            }
            else
            {
                facade.CreateAPInvoice(data, (obj, args) =>
                {
                    Window.Alert(ResInvoiceInputMaintain.Msg_CreateSuccess);
                    //vm.SysNo = args.Result.SysNo.Value;
                    //vm.Status = APInvoiceMasterStatus.Origin;
                    vm = new InvoiceInputMaintainVM();
                    this.Window.Refresh();
                    SetControlStatus();
                });

            }
        }

        /// <summary>
        /// 退回供应商--调用查询页面的批量处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVendorPotalCancel_Click(object sender, RoutedEventArgs e)
        {
            List<int> li = new List<int>();
            li.Add(vm.SysNo.Value);
            UCVPCancelReason uctl = new UCVPCancelReason(li);
            uctl.Dialog = this.Window.ShowDialog(ResInvoiceInputMaintain.Dialog_VPCancelReason, uctl, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    Window.Alert(ResInvoiceInputMaintain.Msg_VPCancelSuccess);

                    vm.Status = APInvoiceMasterStatus.VPCancel;
                    SetControlStatus();
                }
            });
        }

        /// <summary>
        /// 提交审核
        /// 1.新增时PO关联发票直接提交审核，在Service中创建新信息并审核
        /// 2.对已有记录审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (vm.VendorSysNo == null)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_ValidateVendor);
                return;
            }
            if (vm.POItemList.Count == 0)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NoPOItems);
                return;
            }
            if (vm.InvoiceItemList.Count == 0)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NoInvItems);
                return;
            }
            if (vm.DiffTaxTreatmentType == null)
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NeedDiffCal);
                return;
            }
            if (!Compare(vm, lastVM))
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_NeedReDiffCal);
                return;
            }
            APInvoiceInfo data = vm.ConvertVM<InvoiceInputMaintainVM, APInvoiceInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            facade.SubmitWithSaveAPInvoice(data, (obj, args) =>
            {
                Window.Alert(ResInvoiceInputMaintain.Msg_SubmitSuccess);
                vm.Status = APInvoiceMasterStatus.NeedAudit;
                SetControlStatus();
            });
        }
        /// <summary>
        /// 撤销审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SysNo > 0)
            {
                List<int> li = new List<int>();
                li.Add(vm.SysNo.Value);
                facade.BatchCancel(li, (obj, args) =>
                {
                    Window.Alert(ResInvoiceInputMaintain.Msg_CancelSuc);
                    vm.Status = APInvoiceMasterStatus.Origin;
                    SetControlStatus();
                });
            }
        }
        /// <summary>
        /// 通过审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SysNo > 0)
            {
                List<int> li = new List<int>();
                li.Add(vm.SysNo.Value);
                facade.BatchAudit(li, (obj, args) =>
                {
                    Window.Alert(ResInvoiceInputMaintain.Msg_AuditPass);
                    vm.Status = APInvoiceMasterStatus.AuditPass;
                    SetControlStatus();
                });
            }
        }
        /// <summary>
        /// 拒绝审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnhRefuse_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SysNo > 0)
            {
                List<int> li = new List<int>();
                li.Add(vm.SysNo.Value);
                facade.BatchRefuse(li, (obj, args) =>
                {
                    Window.Alert(ResInvoiceInputMaintain.Msg_Refuse);
                    vm.Status = APInvoiceMasterStatus.Refuse;
                    SetControlStatus();
                });
            }
        }

        private void CheckBox_IsDataRight_Click(object sender, RoutedEventArgs e)
        {
            if (vm.IsDataRight)
            {
                btnSave.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Create)
    || AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Update) ? true : false;
                btnSubmit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Submit) ? true : false;
            }
            else
            {
                btnSave.IsEnabled = btnSubmit.IsEnabled = false;
            }

        }


        private bool Compare(InvoiceInputMaintainVM c1, InvoiceInputMaintainVM c2)
        {
            if (c1 == c2)
            {
                return true;
            }

            if (c1 == null || c2 == null || c1.POItemList.Count != c2.POItemList.Count || c1.InvoiceItemList.Count != c2.InvoiceItemList.Count)
            {
                return false;
            }

            var poNoStr1 = string.Join(",", c1.POItemList.Select(p => p.PoNo.ToString() + p.PoAmt.Value.ToString()).OrderBy(x => x).ToArray());
            var poNoStr2 = string.Join(",", c2.POItemList.Select(p => p.PoNo.ToString() + p.PoAmt.Value.ToString()).OrderBy(x => x).ToArray());

            var inoNoStr1 = string.Join(",", c1.InvoiceItemList.Select(p => p.InvoiceNo.ToString() + p.InvoiceAmt.Value.ToString()).OrderBy(x => x).ToArray());
            var inoNoStr2 = string.Join(",", c2.InvoiceItemList.Select(p => p.InvoiceNo.ToString() + p.InvoiceAmt.Value.ToString()).OrderBy(x => x).ToArray());

            return (poNoStr1 + inoNoStr1) == (poNoStr2 + inoNoStr2);
        }

        private void CheckBox_DiffAmtRight_Click(object sender, RoutedEventArgs e)
        {
            if (vm.IsDiffAmtRight)
            {
                btnPass.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_InvoiceInputMaintain_Pass) ? true : false;
            }
            else
            {
                btnPass.IsEnabled = false;
            }
        }

        private void OnGetVendorSelected(object sender, Basic.Components.UserControls.VendorPicker.VendorSelectedEventArgs e)
        {
            //this.txtPaySettleCompany.Text = string.Empty;

            //facade.QueryPaySettleCompany(e.SelectedVendorInfo.SysNo.Value, (obj, args) =>
            //{
            //    if (args.FaultsHandle() || args.Result == 0)
            //        return;

            //    this.txtPaySettleCompany.Text = EnumConverter.GetDescription(args.Result, typeof(ECCentral.BizEntity.PO.PaySettleCompany));
            //});
        }

        //回车触发发票录入
        private void Text_InvoiceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.vm.InvoiceItemNoList = this.Text_InvoiceNo.Text;
                HyperlinkButton_InputInvoiceItem_Click(null, null);
            }
        }

        //回车触发发票录入
        private void Text_ItemInvoiceAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                decimal amt = 0m;
                this.vm.ItemInvoiceAmt = decimal.TryParse(this.Text_ItemInvoiceAmt.Text, out amt) ? amt : 0m;
                HyperlinkButton_InputInvoiceItem_Click(null, null);
            }
        }

        //回车触发PO单录入
        private void Text_PONo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.vm.POItemNoList = this.Text_PONo.Text;
                HyperlinkButton_InputPoItem_Click(null, null);
            }
        }
    }
}
