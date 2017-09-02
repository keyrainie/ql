using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.QueryFilter.PO;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Components.UserControls.LanguagesDescription;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VendorMaintain : PageBase
    {
        public VendorInfoVM vendorInfoVM;
        public VendorFacade serviceFacade;
        public VendorPayTermsFacade payTermsServiceFacade;
        public string getLoadVendorSysNo;
        public string AuditVendorAgentText;

        public VendorMaintain()
        {
            InitializeComponent();
            InitializeComboBoxData();

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            vendorInfoVM = new VendorInfoVM();
            serviceFacade = new VendorFacade(this);
            payTermsServiceFacade = new VendorPayTermsFacade(this);
            //cbxVendorDeductType.SelectionChanged += cbxVendorDeductType_SelectionChanged;
            DeductQueryFilter filter = new DeductQueryFilter() { DeductType = DeductType.Contract, Status = Status.Effective };
            new DeductFacade(this).QueryDeducts(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<DeductVM> ItemList = DynamicConverter<DeductVM>.ConvertToVMList(args.Result.Rows);
                ItemList.Add(new DeductVM { SysNo = 0, Name = "无" });
                this.cbxVendorDeductInfo.ItemsSource = ItemList;
            });
            getLoadVendorSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(getLoadVendorSysNo))
            {
                LoadVendorInfo();
            }

        }

        void cbxVendorDeductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (vendorInfoVM == null)
            {
                return;
            }
            if (vendorInfoVM.VendorDeductInfo.CalcType == VendorCalcType.Fix)
            {
                txtDeductPercent.Text = "0";
                txtMaxAmt.Text = "0";
                txtDeductPercent.IsEnabled = false;
                txtMaxAmt.IsEnabled = false;
            }
            else
            {
                txtDeductPercent.IsEnabled = true;
                txtMaxAmt.IsEnabled = true;
            }
        }

        private void SetAccessControl()
        {
            //更新Vendor信息:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditAndCreateVendor))
            {
                this.btnUpdate.IsEnabled = false;
            }
            //修改供应商状态:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_UpdateVendorStatus))
            {
                this.cmbVendorStatus.IsEnabled = false;
                this.btnConfirmVendor.IsEnabled = false;
            }
            //审核供应商代理信息
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditVendorAgentInfo))
            {
                this.btnConfirmAgentInfo.IsEnabled = false;
                this.btnNewAgentInfo.IsEnabled = false;
                this.btnCancelAgentInfo.IsEnabled = false;
            }
            //编辑供应商采购信息：开户行，账号 ：
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_EditVendorFinanceInfo))
            {
                this.txtBank.IsEnabled = false;
                this.txtccount.IsEnabled = false;
            }
            //更新供应商售后信息:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_UpdateVendorServiceInfo))
            {
                this.ucVendorServiceAreaPicker.IsEnabled = false;
                this.txtRepairContact.IsEnabled = false;
                this.txtRepairContactPhone.IsEnabled = false;
                this.txtRepairAddress.IsEnabled = false;
                this.txtRMAServiceArea.IsEnabled = false;
                this.txtRMAPolicy.IsEnabled = false;
            }
            //锁定供应商:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Vendor_HoldVendor))
            {
                this.btnHoldMark.IsEnabled = false;
                this.btnHoldPM.IsEnabled = false;
            }
        }
        /// <summary>
        /// 加载供应商信息
        /// </summary>
        private void LoadVendorInfo()
        {
            serviceFacade.GetVendorBySysNo(getLoadVendorSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    Window.Alert(args.Error.Faults[0].ErrorDescription);
                    return;
                }


                decimal totalRentFees = 0m;
                vendorInfoVM = EntityConverter<VendorInfo, VendorInfoVM>.Convert(args.Result, (s, t) =>
                {
                    for (int i = 0; i < s.VendorAgentInfoList.Count; i++)
                    {
                        t.VendorAgentInfoList[i].ManufacturerInfo.ManufacturerNameDisplay = s.VendorAgentInfoList[i].ManufacturerInfo.ManufacturerNameLocal.Content;
                        t.VendorAgentInfoList[i].BrandInfo.BrandNameDisplay = s.VendorAgentInfoList[i].BrandInfo.BrandNameLocal.Content;
                        totalRentFees += (s.VendorAgentInfoList[i].VendorCommissionInfo.RentFee.HasValue ? s.VendorAgentInfoList[i].VendorCommissionInfo.RentFee.Value : 0m);
                        t.VendorAgentInfoList[i].VendorCommissionInfo.GuaranteedAmt = s.VendorAgentInfoList[i].VendorCommissionInfo.SaleRuleEntity.MinCommissionAmt.HasValue ? s.VendorAgentInfoList[i].VendorCommissionInfo.SaleRuleEntity.MinCommissionAmt.Value.ToString() : string.Empty;
                    }
                    t.VendorFinanceInfo.IsAutoAudit = !s.VendorFinanceInfo.IsAutoAudit.HasValue ? false : s.VendorFinanceInfo.IsAutoAudit;
                    t.VendorFinanceInfo.IsAutoAuditDisplayString = s.VendorFinanceInfo.IsAutoAudit == true ? ResVendorMaintain.Msg_AutoAudit : ResVendorMaintain.Msg_ManualAudit;
                    ReplaceStockInfo(s.VendorAgentInfoList.Count);
                    t.VendorDeductInfo = new VendorDeductInfoVM
                    {
                        DeductSysNo = s.VendorDeductInfo.DeductSysNo,
                        CalcType = s.VendorDeductInfo.CalcType,
                        DeductPercent = (s.VendorDeductInfo.DeductPercent * 100).ToString(),
                        FixAmt = s.VendorDeductInfo.FixAmt.ToString(),
                        MaxAmt = s.VendorDeductInfo.MaxAmt.ToString(),
                    };
                    BusinessModeGroup.Visibility = System.Windows.Visibility.Visible;
                    if (t.VendorBasicInfo.ConsignFlag.HasValue && t.VendorBasicInfo.ConsignFlag.Value == VendorConsignFlag.GroupBuying)
                    {
                        BusinessModeGroup.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    if (s.VendorCustomsInfo == null)
                        t.VendorCustomsInfo = new VendorCustomsInfoVM();
                });

                //设置店租佣金合计:
                this.txtTotalRentFees.Text = totalRentFees.ToString("f2");

                this.DataContext = vendorInfoVM;
                this.dataGrid_VendorHistoryInfo.Bind();
                this.dataGrid_VendorAgentInfo.Bind();
                //
                if (vendorInfoVM.VendorBasicInfo.ConsignFlag.HasValue && vendorInfoVM.VendorBasicInfo.ConsignFlag.Value == VendorConsignFlag.Consign)
                {
                    //如果供应商是代销:
                    this.dataGrid_VendorAgentInfo.Columns[8].Visibility = Visibility.Visible;
                    this.dataGrid_VendorAgentInfo.Columns[9].Visibility = Visibility.Visible;
                }
                if (vendorInfoVM.VendorBasicInfo.ConsignFlag.HasValue && vendorInfoVM.VendorBasicInfo.ConsignFlag.Value == VendorConsignFlag.GroupBuying)
                {
                    this.tabItemCS.Visibility = System.Windows.Visibility.Collapsed;
                    gridVendorServiceInfo.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    //如果为经销:
                    this.lblAutoAudit.Visibility = Visibility.Visible;
                    this.txtAutoAudit.Visibility = Visibility.Visible;
                    this.lblSettlePeriodType.Visibility = Visibility.Collapsed;
                    this.txtSettlePeriodType.Visibility = Visibility.Collapsed;

                }
                //if (vendorInfoVM.VendorBasicInfo.EPortSysNo == null)
                //{
                //    this.cmbEPort.SelectedEPort = 0;
                //}
                ShowActionButtons();
                BindVendorEmailAddress(vendorInfoVM.VendorBasicInfo.EmailAddress);
                SetAccessControl();
            });
        }

        //替换仓库显示信息
        private void ReplaceStockInfo(int count)
        {
            serviceFacade.GetStockList((obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    for (int i = 0; i < count; i++)
                    {
                        if (vendorInfoVM.VendorAgentInfoList[i].CheckType != VendorModifyActionType.Edit)
                        {
                            vendorInfoVM.VendorAgentInfoList[i].Content = null;
                        }
                        else
                        {
                            for (int j = 0; j < args.Result.Count; j++)
                            {
                                vendorInfoVM.VendorAgentInfoList[i].Content
                                    = vendorInfoVM.VendorAgentInfoList[i].Content != null ? vendorInfoVM.VendorAgentInfoList[i].Content.Replace(args.Result[j].SysNo.ToString(), args.Result[j].WarehouseName) : null;
                            }
                        }
                    }
                    //因为赋值方法出于异步调用中，所以赋值完毕后，必须刷新GRID显示值
                    //否则 会出现 状态为"Add"的代理记录回出现 "详细"链接
                    //by jack.w.wang
                    dataGrid_VendorAgentInfo.Bind();
                });
        }

        private void ShowActionButtons()
        {
            //财务信息按钮:
            if (this.vendorInfoVM.VendorFinanceInfo.FinanceRequestInfo != null && this.vendorInfoVM.VendorFinanceInfo.FinanceRequestInfo.Status == VendorModifyRequestStatus.Apply)
            {
                //待审核财务记录:
                this.btnVendorFinanceInfoAction.Content = ResVendorMaintain.Msg_AuditFinance;
            }
            else
            {
                this.btnVendorFinanceInfoAction.Content = ResVendorMaintain.Msg_ApplyFinance;
            }


            bool pmHoldMark = vendorInfoVM.VendorBasicInfo.HoldPMList.Count > 0;//PM锁定状态
            //锁定/解锁供应商 ：
            if (this.vendorInfoVM.VendorBasicInfo.HoldMark == true)//供应商锁定状态
            {

                this.btnHoldMark.IsEnabled = false;
                this.btnUnHoldMark.IsEnabled = true;

                this.btnHoldPM.IsEnabled = false;
                this.lblVendorHoldStatus.Text = string.Format("[{0}]", ResVendorMaintain.Msg_Hold);

                //被锁定，设置解锁按钮为Enabled,
                //this.btnHoldMark.IsEnabled = false;
                //this.btnUnHoldMark.IsEnabled = true;
                //this.lblVendorHoldStatus.Text = string.Format("[{0}]", ResVendorMaintain.Msg_Hold);

                //this.btnHoldPM.IsEnabled = false;
            }
            else//供应商解锁状态
            {
                if (pmHoldMark == true)
                {
                    this.btnHoldMark.IsEnabled = false;
                    this.btnUnHoldMark.IsEnabled = false;

                    this.btnHoldPM.Content = "解除/更改锁定PM";

                    this.lblVendorHoldStatus.Text = string.Format("[{0}]", "已锁定关联PM");
                }
                else
                {
                    this.btnHoldMark.IsEnabled = true;
                    this.btnUnHoldMark.IsEnabled = false;

                    this.btnHoldPM.Content = "锁定关联PM";

                    this.lblVendorHoldStatus.Text = string.Format("[{0}]", ResVendorMaintain.Msg_UnHold);
                }

                this.btnHoldPM.IsEnabled = true;


                //未锁定,设置锁定按钮为Enabled,设置锁定关联PM为Enabled:
                //this.btnHoldMark.IsEnabled = true;
                //this.btnUnHoldMark.IsEnabled = false;
                //this.lblVendorHoldStatus.Text = string.Format("[{0}]", ResVendorMaintain.Msg_UnHold);


                //this.btnHoldPM.IsEnabled = true;


            }

            //锁定PM：
            //if (vendorInfoVM.VendorBasicInfo.HoldPMList.Count > 0)
            //{
            //    this.btnHoldMark.IsEnabled = false;
            //    this.btnHoldPM.Content = "解除/更改锁定PM";
            //    this.btnHoldPM.IsEnabled = true;
            //    this.lblVendorHoldStatus.Text = string.Format("[{0}]", "已锁定关联PM");

            //}
            //else
            //{
            //    this.btnHoldPM.Content = "锁定关联PM";
            //}

            if (this.vendorInfoVM.VendorBasicInfo.VendorStatus == VendorStatus.WaitApprove)
            {
                btnConfirmVendor.IsEnabled = true;
            }

            if (0 < vendorInfoVM.VendorAgentInfoList.Count)
            {
                int getVerifyCount = vendorInfoVM.VendorAgentInfoList.Where(i => i.RequestType.HasValue && i.RequestType.Value == VendorModifyRequestStatus.Apply).Count();
                if (getVerifyCount > 0)
                {
                    this.btnConfirmAgentInfo.IsEnabled = true;
                    this.btnCancelAgentInfo.IsEnabled = true;
                    this.btnNewAgentInfo.IsEnabled = false;
                    this.dataGrid_VendorAgentInfo.Columns[1].Visibility = Visibility.Collapsed;
                    this.dataGrid_VendorAgentInfo.Columns[0].Visibility = Visibility.Visible;
                }
                else
                {
                    this.dataGrid_VendorAgentInfo.Columns[1].Visibility = Visibility.Visible;
                    this.dataGrid_VendorAgentInfo.Columns[0].Visibility = Visibility.Collapsed;
                }
            }

            if (this.vendorInfoVM.VendorBasicInfo.RequestVendorRank.HasValue && vendorInfoVM.VendorBasicInfo.VendorRank != vendorInfoVM.VendorBasicInfo.RequestVendorRank)
            {
                //如果修改供应商等级不等，则显示审核供应商等级按钮:
                this.cmbRank.IsEnabled = false;
                this.btnCancelLevelApply.IsEnabled = true;
                this.btnConfirmLevelApply.IsEnabled = true;
            }
            if (!this.vendorInfoVM.VendorBasicInfo.RequestVendorRank.HasValue && this.vendorInfoVM.VendorBasicInfo.VendorRank.HasValue)
            {
                this.vendorInfoVM.VendorBasicInfo.RequestVendorRank = this.vendorInfoVM.VendorBasicInfo.VendorRank;
            }

            //if (this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor != null && this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor != this.vendorInfoVM.VendorBasicInfo.BuyWeekDayVendor)
            //{
            //    this.btnCancelDateApply.IsEnabled = true;
            //    this.btnConfrimDateApply.IsEnabled = true;
            //    foreach (UIElement item in this.spVendorBuyWeekCheckBoxList.Children)
            //    {
            //        if (item is CheckBox)
            //        {
            //            ((CheckBox)item).IsEnabled = false;
            //        }
            //    }
            //}
            if (this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor == null)
            {
                this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor = this.vendorInfoVM.VendorBasicInfo.BuyWeekDayVendor;
            }

            //绑定下单日期 - CheckBoxList:
            //BindVendorBuyWeekDayCheckBoxList(vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor);
            //绑定合同显示CheckBoxList:
            BindContractsCheckBoxes();

            //申请/撤回下单日期修改:
            //if (this.vendorInfoVM.VendorBasicInfo.ExtendedInfo.InvoiceType != VendorInvoiceType.NEG || this.vendorInfoVM.VendorBasicInfo.ExtendedInfo.StockType != VendorStockType.NEG || this.vendorInfoVM.VendorBasicInfo.ExtendedInfo.ShippingType != VendorShippingType.NEG)
            //{
            //    this.btnCancelDateApply.Visibility = Visibility.Collapsed;
            //    this.btnConfrimDateApply.Visibility = Visibility.Collapsed;
            //    this.txtBuyWeekDay.Visibility = Visibility.Collapsed;
            //    this.spVendorBuyWeekCheckBoxList.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    this.btnCancelDateApply.Visibility = Visibility.Visible;
            //    this.btnConfrimDateApply.Visibility = Visibility.Visible;
            //    this.txtBuyWeekDay.Visibility = Visibility.Visible;
            //    this.spVendorBuyWeekCheckBoxList.Visibility = Visibility.Visible;
            //}
            InitVendorAttachmentsLink();
            //前台显示:
            //if (vendorInfoVM.VendorBasicInfo.ExtendedInfo.InvoiceType == VendorInvoiceType.NEG && vendorInfoVM.VendorBasicInfo.ExtendedInfo.StockType == VendorStockType.NEG && vendorInfoVM.VendorBasicInfo.ExtendedInfo.ShippingType == VendorShippingType.NEG)
            //{
            //    this.chkIsShowStore.IsChecked = false;
            //    this.chkIsShowStore.IsEnabled = false;
            //}
            //else
            //{
            this.chkIsShowStore.IsEnabled = true;
            //}
            //默认送货分仓:
            //if (vendorInfoVM.VendorBasicInfo.ExtendedInfo.StockType == VendorStockType.MET && vendorInfoVM.VendorBasicInfo.ExtendedInfo.ShippingType == VendorShippingType.NEG)
            //{
            //    this.cmbDefaultStock.Visibility = Visibility.Visible;
            //    this.lblDefaultStock.Visibility = Visibility.Visible;
            //}

        }

        private void InitializeComboBoxData()
        {
            //开票方式:
            this.cmbInvoiceType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorInvoiceType>(EnumConverter.EnumAppendItemType.None);
            this.cmbInvoiceType.SelectedIndex = 0;
            //仓储方式:
            this.cmbStockType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStockType>(EnumConverter.EnumAppendItemType.None);
            this.cmbStockType.SelectedIndex = 0;
            //配送方式:
            this.cmbShippingType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorShippingType>(EnumConverter.EnumAppendItemType.None);
            this.cmbShippingType.SelectedIndex = 0;
            //默认送货分仓:
            this.cmbDefaultStock.ItemsSource = EnumConverter.GetKeyValuePairs<VendorDefaultShippingStock>(EnumConverter.EnumAppendItemType.None);
            this.cmbDefaultStock.SelectedIndex = 0;
            //供应商属性:
            var vendorConsignFlagList = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.None);
            vendorConsignFlagList.RemoveAll(item => item.Key == VendorConsignFlag.Consign);//隐藏代销
            this.cmbConsignFlag.ItemsSource = vendorConsignFlagList;

            //付款结算公司:
            //this.cmbPaySettleCompany.ItemsSource = EnumConverter.GetKeyValuePairs<PaySettleCompany>(EnumConverter.EnumAppendItemType.None);
            //供应商等级:
            this.cmbRank.ItemsSource = EnumConverter.GetKeyValuePairs<VendorRank>(EnumConverter.EnumAppendItemType.None);

            //供应商状态:
            this.cmbVendorStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VendorStatus>(EnumConverter.EnumAppendItemType.None);
            //供应商是否合作:
            this.cmbVendorIsCooperate.ItemsSource = EnumConverter.GetKeyValuePairs<VendorIsCooperate>(EnumConverter.EnumAppendItemType.None);



            //是否转租赁
            this.cmbIsToLease.ItemsSource = EnumConverter.GetKeyValuePairs<VendorIsToLease>(EnumConverter.EnumAppendItemType.None);
            //this.cmbIsToLease.SelectedIndex= 1;  

            this.cbxVendorDeductType.ItemsSource = EnumConverter.GetKeyValuePairs<VendorCalcType>(EnumConverter.EnumAppendItemType.None);
            this.cbxVendorDeductType.SelectedIndex = 0;

        }

        private void SetIsShowStore()
        {
            if (null != this.cmbInvoiceType.SelectedValue && null != this.cmbShippingType.SelectedValue && null != this.cmbStockType.SelectedValue)
            {

                //if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString() && this.cmbStockType.SelectedValue.ToString() == VendorStockType.NEG.ToString() && this.cmbShippingType.SelectedValue.ToString() == VendorShippingType.NEG.ToString())
                //{
                //    this.chkIsShowStore.IsChecked = false;
                //    this.chkIsShowStore.IsEnabled = false;
                //}
                //else
                //{
                this.chkIsShowStore.IsEnabled = true;
                //}
            }
        }

        private void SetControlStatus()
        {
            //if (null != this.cmbInvoiceType.SelectedValue && null != this.cmbShippingType.SelectedValue && null != this.cmbStockType.SelectedValue)
            //{
            //    if (this.cmbInvoiceType.SelectedValue.ToString() == VendorInvoiceType.NEG.ToString())
            //    {
            //        if (this.cmbStockType.SelectedValue.ToString() == VendorStockType.MET.ToString())
            //        {
            //            this.cmbConsignFlag.SelectedIndex = 1;
            //            this.cmbConsignFlag.IsEnabled = false;
            //        }

            //    }
            //    else
            //    {
            //        this.cmbConsignFlag.SelectedIndex = 2;
            //        this.cmbConsignFlag.IsEnabled = false;
            //    }

            //}
        }

        private void BindContractsCheckBoxes()
        {
            if (null != this.vendorInfoVM.VendorAttachInfo)
            {
                if (!string.IsNullOrEmpty(this.vendorInfoVM.VendorAttachInfo.AgreementBeingSold))
                {
                    //经销合同
                    this.chkBeingSoldContract.IsChecked = true;
                }
                if (!string.IsNullOrEmpty(this.vendorInfoVM.VendorAttachInfo.AgreementConsign))
                {
                    //代销合同:
                    this.chkConsignContract.IsChecked = true;
                }
                if (!string.IsNullOrEmpty(this.vendorInfoVM.VendorAttachInfo.AgreementAfterSold))
                {
                    this.chkAfterSoldContract.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 更新"店租佣金合计"
        /// </summary>
        private void UpdateTotalCommissionFees()
        {
            if (null != vendorInfoVM.VendorAgentInfoList && vendorInfoVM.VendorAgentInfoList.Count > 0)
            {
                decimal totalRentFees = 0m;
                vendorInfoVM.VendorAgentInfoList.ForEach(x =>
                {
                    if (x.RowState != VendorRowState.Deleted)
                    {
                        totalRentFees += x.VendorCommissionInfo.RentFee.ToDecimal();
                    }
                });
                this.txtTotalRentFees.Text = totalRentFees.ToString("f2");
            }
        }

        private void InitVendorAttachmentsLink()
        {
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.VendorApplyForm))
            {
                this.hpkVendorNewApplyForm.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.OrganizeCodeCertificate))
            {
                this.hpkOrganizeCodeCertificate.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.EnterpriseBusinessLicence))
            {
                this.hpkEntBizLicence.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.TaxationAffairsRegistration))
            {
                this.hpkTaxRegistration.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.AgreementBeingSold))
            {
                this.hpkAgreementBeingSold.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.AgreementConsign))
            {
                this.hpkAgreementConsign.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.AgreementAfterSold))
            {
                this.hpkAgreementAfterSold.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorAttachInfo.AgreementOther))
            {
                this.hpkAgreementOther.Content = ResVendorMaintain.Label_DownloadAttachment;
            }
        }

        #region [Events]

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ValidationHelper.ClearValidation(this.txtValidDateNewold);
            ValidationHelper.ClearValidation(this.txtExpiredDateNewold);
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            this.tabVendorAdvancedInfo.SelectedIndex = 1;
            this.UpdateLayout();
            if (!ValidationManager.Validate(this.tabVendorAdvancedInfo))
            {
                return;
            }
            //售后
            if (this.tabItemCS.Visibility == System.Windows.Visibility.Visible)
            {
                this.tabVendorAdvancedInfo.SelectedIndex = 2;
                this.UpdateLayout();
                if (!ValidationManager.Validate(this.tabVendorAdvancedInfo))
                {
                    return;
                }
            }
            this.tabVendorAdvancedInfo.SelectedIndex = 0;

            if (string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.EmailAddress))
            {
                Window.Alert(ResVendorMaintain.Msg_VendorMailAddressNull);
                return;
            }

            // 供应商更新操作:
            this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor = BuildSelectVendorBuyWeekDayString();
            Window.Confirm(ResVendorMaintain.InfoMsg_ConfirmModify, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    //更新:
                    VendorInfo editVendorInfo = EntityConverter<VendorInfoVM, VendorInfo>.Convert(vendorInfoVM);
                    serviceFacade.EditVendorInfo(editVendorInfo, (obj4, args4) =>
                    {
                        if (args4.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_UpdateSuc);
                        LoadVendorInfo();
                        return;
                    });
                }
            });
        }

        private void btnNewAgentInfo_Click(object sender, RoutedEventArgs e)
        {
            //新建代理信息
            VendorAgentInfoMaintain agentInfo = new VendorAgentInfoMaintain(this.vendorInfoVM.VendorBasicInfo);
            agentInfo.Dialog = Window.ShowDialog(string.Empty, agentInfo, (obj, args) =>
            {
                if (DialogResultType.OK == args.DialogResult)
                {
                    VendorAgentInfoVM getNewAgentInfoVM = args.Data as VendorAgentInfoVM;
                    if (null != getNewAgentInfoVM)
                    {
                        this.vendorInfoVM.VendorAgentInfoList.Add(getNewAgentInfoVM);
                        UpdateTotalCommissionFees();
                        this.dataGrid_VendorAgentInfo.Bind();
                    }
                }
            });
        }

        private void btnNewHistory_Click(object sender, RoutedEventArgs e)
        {
            //手动添加供应商历史信息:
            VendorHistoryInfoNew newHistoryInfo = new VendorHistoryInfoNew(this.vendorInfoVM.SysNo.Value);
            newHistoryInfo.Dialog = Window.ShowDialog(ResVendorMaintain.AlertMsg_AddHistoryTitle, newHistoryInfo, (obj, args) =>
             {
                 if (args.Data != null && args.DialogResult == DialogResultType.OK)
                 {
                     //重新加载供应商日志信息:
                     serviceFacade.GetVendorHistoryLogBySysNo(this.vendorInfoVM.SysNo.Value.ToString(), (obj2, args2) =>
                     {
                         if (args2.FaultsHandle())
                         {
                             return;
                         }
                         this.vendorInfoVM.VendorHistoryLog = EntityConverter<List<VendorHistoryLog>, List<VendorHistoryLogVM>>.Convert(args2.Result);
                         this.dataGrid_VendorHistoryInfo.Bind();
                     });

                 }
             }, new Size(400, 210));
        }

        /// <summary>
        /// 供应商代理信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_VendorAgentInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.dataGrid_VendorAgentInfo.ItemsSource = vendorInfoVM.VendorAgentInfoList.Where(x => x.RowState != VendorRowState.Deleted).ToList();
        }

        /// <summary>
        /// 供应商历史信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_VendorHistoryInfo_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (null != vendorInfoVM.VendorHistoryLog && 0 < vendorInfoVM.VendorHistoryLog.Count)
            {
                this.dataGrid_VendorHistoryInfo.ItemsSource = vendorInfoVM.VendorHistoryLog;
            }
        }

        private void hpl_AgentInfoEdit_Click(object sender, RoutedEventArgs e)
        {
            //编辑代理信息:
            VendorAgentInfoVM getSelectAgentVM = this.dataGrid_VendorAgentInfo.SelectedItem as VendorAgentInfoVM;
            if (null != getSelectAgentVM)
            {
                VendorAgentInfoMaintain maintainUC = new VendorAgentInfoMaintain(getSelectAgentVM, this.vendorInfoVM.VendorBasicInfo, false);
                maintainUC.Dialog = Window.ShowDialog(ResVendorMaintain.AlertMsg_EditTitle, maintainUC, (obj, args) =>
                {
                    getSelectAgentVM = (VendorAgentInfoVM)args.Data;
                    this.dataGrid_VendorAgentInfo.Bind();
                    UpdateTotalCommissionFees();
                });
            }
        }

        private void hpl_AgentInfoView_Click(object sender, RoutedEventArgs e)
        {
            //查看代理信息:
            VendorAgentInfoVM getSelectAgentVM = this.dataGrid_VendorAgentInfo.SelectedItem as VendorAgentInfoVM;
            if (null != getSelectAgentVM)
            {
                VendorAgentInfoMaintain maintainUC = new VendorAgentInfoMaintain(getSelectAgentVM, this.vendorInfoVM.VendorBasicInfo, true);
                maintainUC.Dialog = Window.ShowDialog(ResVendorMaintain.AlertMsg_EditTitle, maintainUC, (obj, args) =>
                {
                });
            }
        }

        private void hpl_AgentInfoDelete_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResVendorMaintain.AlertMsg_ConfirmDelete, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorAgentInfoVM getSelectAgentVM = this.dataGrid_VendorAgentInfo.SelectedItem as VendorAgentInfoVM;
                    if (null != getSelectAgentVM)
                    {
                        if (getSelectAgentVM.AgentSysNo.HasValue && getSelectAgentVM.AgentSysNo.Value > 0)
                        {
                            //如果是已存在的代理信息:
                            getSelectAgentVM.RowState = VendorRowState.Deleted;
                        }
                        else
                        {
                            //如果是新建的代理信息 ：
                            this.vendorInfoVM.VendorAgentInfoList.Remove(getSelectAgentVM);
                        }
                        this.dataGrid_VendorAgentInfo.Bind();
                        UpdateTotalCommissionFees();
                    }
                }
            });
        }

        // 按钮点击事件执行：商家审核通过
        private void btnConfirmVendor_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.AlertMsg_ConfirmVendor, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorApproveInfo requestInfo = new VendorApproveInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value
                    };
                    serviceFacade.ApproveVendor(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_ApplyAgentSuc);
                        Window.Refresh();
                    });
                }
            });
        }

        /// <summary>
        /// 撤销供应商代理信息申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelAgentInfo_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.AlertMsg_ConfirmCancelAgent, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo requestInfo = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value,
                        RequestType = VendorModifyRequestType.Manufacturer
                    };
                    serviceFacade.CancelVendorModifyRequest(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_CancelAgentSuc);
                        Window.Refresh();
                    });
                }
            });
        }

        /// <summary>
        /// 供应商代理信息申请通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmAgentInfo_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResVendorMaintain.AlertMsg_AlertTitle, ResVendorMaintain.AlertMsg_ConfirmApplyAgent, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo requestInfo = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value,
                        RequestType = VendorModifyRequestType.Manufacturer
                    };
                    serviceFacade.PassVendorModifyRequest(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_ApplyAgentSuc);
                        Window.Refresh();
                    });
                }
            });
        }

        private void btnCancelLevelApply_Click(object sender, RoutedEventArgs e)
        {
            //撤销等级申请:
            Window.Confirm(ResVendorMaintain.AlertMsg_ConfirmCancelLevel, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    #region [Check 逻辑]
                    if (this.vendorInfoVM.VendorAgentInfoList == null || this.vendorInfoVM.VendorAgentInfoList.Count <= 0)
                    {
                        Window.Alert(ResVendorMaintain.AlertMsg_VendorInfoEmpty);
                        return;
                    }
                    #endregion

                    VendorModifyRequestInfo requestInfo = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value,
                        Rank = this.vendorInfoVM.VendorBasicInfo.VendorRank.Value,
                        RequestType = VendorModifyRequestType.Vendor
                    };
                    serviceFacade.CancelVendorModifyRequest(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_CancelLevelSuc);
                        Window.Refresh();

                    });
                }
            });
        }

        private void btnConfirmLevelApply_Click(object sender, RoutedEventArgs e)
        {
            //等级审核通过:
            Window.Confirm(ResVendorMaintain.AlertMsg_ConfirmApplyLevel, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    #region [Check 逻辑]
                    if (this.vendorInfoVM.VendorAgentInfoList == null || this.vendorInfoVM.VendorAgentInfoList.Count <= 0)
                    {
                        Window.Alert(ResVendorMaintain.AlertMsg_VendorInfoEmpty);
                        return;
                    }
                    #endregion

                    VendorModifyRequestInfo requestInfo = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value,
                        Rank = this.vendorInfoVM.VendorBasicInfo.VendorRank.Value,
                        RequestType = VendorModifyRequestType.Vendor
                    };
                    serviceFacade.PassVendorModifyRequest(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_ApplyLevelSuc);
                        Window.Refresh();

                    });
                }
            });
        }

        private void btnCancelDateApply_Click(object sender, RoutedEventArgs e)
        {
            //撤销下单日期审核:
            Window.Confirm(ResVendorMaintain.AlertMsg_ConfirmCancelDate, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo requestInfo = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value,
                        BuyWeekDay = this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor,
                        RequestType = VendorModifyRequestType.BuyWeekDay
                    };
                    serviceFacade.CancelVendorModifyRequest(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_CancelDateSuc);
                        Window.Refresh();
                    });
                }
            });
        }

        private void btnConfrimDateApply_Click(object sender, RoutedEventArgs e)
        {
            //下单日期审核通过:
            Window.Confirm(ResVendorMaintain.AlertMsg_ConfirmApplyDate, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorModifyRequestInfo requestInfo = new VendorModifyRequestInfo()
                    {
                        VendorSysNo = this.vendorInfoVM.SysNo.Value,
                        BuyWeekDay = this.vendorInfoVM.VendorBasicInfo.RequestBuyWeekDayVendor,
                        RequestType = VendorModifyRequestType.BuyWeekDay
                    };
                    serviceFacade.PassVendorModifyRequest(requestInfo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorMaintain.AlertMsg_AuditSuc);
                        Window.Refresh();
                    });
                }
            });
        }

        private void btnHoldMark_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.HoldReason))
            {
                Window.Alert(ResVendorMaintain.Msg_VendorHoldMarkNull);
                return;
            }

            Window.Confirm(ResVendorMaintain.Msg_VendorHoldConfirm, (objj, argss) =>
             {
                 if (argss.DialogResult == DialogResultType.OK)
                 {
                     //锁定商家:
                     VendorInfo info = EntityConverter<VendorInfoVM, VendorInfo>.Convert(vendorInfoVM);
                     serviceFacade.HoldVendor(info, (obj, args) =>
                     {
                         if (args.FaultsHandle())
                         {
                             return;
                         }
                         Window.Alert(string.Format(ResVendorMaintain.Msg_VendorHoldSuc, args.Result));
                         Window.Refresh();
                     });
                 }
             });
        }

        private void btnUnHoldMark_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.HoldReason))
            {
                Window.Alert(ResVendorMaintain.Msg_VendorHoldMarkNull);
                return;
            }
            Window.Confirm(ResVendorMaintain.Msg_VendorUnHoldConfirm, (objj, argss) =>
            {
                if (argss.DialogResult == DialogResultType.OK)
                {
                    //解锁供应商:
                    VendorInfo info = EntityConverter<VendorInfoVM, VendorInfo>.Convert(vendorInfoVM);
                    serviceFacade.UnHoldVendor(info, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(string.Format(ResVendorMaintain.Msg_VendorUnHoldSuc, args.Result));
                        Window.Refresh();
                    });
                }
            });
        }

        private void btnHoldPM_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.HoldReason))
            {
                Window.Alert(ResVendorMaintain.Msg_VendorHoldMarkNull);
                return;
            }
            //锁定/解锁 关联PM：
            VendorHoldPM holdPMCtrl = new VendorHoldPM(this.vendorInfoVM.SysNo.Value, vendorInfoVM.VendorBasicInfo.HoldReason);
            holdPMCtrl.Dialog = Window.ShowDialog("请选择需要锁定的PM", holdPMCtrl, (obj, args) =>
            {
                if (args.Data != null && args.DialogResult == DialogResultType.OK)
                {
                    Window.Alert(args.Data.ToString());
                    Window.Refresh();
                }
            }, new Size(500, 380));
        }

        private void btnVendorFinanceInfoAction_Click(object sender, RoutedEventArgs e)
        {
            //编辑财务信息(审核 ）
            VendorFinanceInfoMaintain financeMaintainDialog;
            if (vendorInfoVM.VendorFinanceInfo.FinanceRequestInfo != null && vendorInfoVM.VendorFinanceInfo.FinanceRequestInfo.Status == VendorModifyRequestStatus.Apply)
            {
                financeMaintainDialog = new VendorFinanceInfoMaintain(vendorInfoVM, false);
            }
            else
            {
                financeMaintainDialog = new VendorFinanceInfoMaintain(vendorInfoVM, true);
            }


            financeMaintainDialog.Dialog = Window.ShowDialog(this.btnVendorFinanceInfoAction.Content.ToString(), financeMaintainDialog, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    Window.Refresh();
                }
            }
            , new Size(650, 240));
        }

        #endregion

        #region [Methods]

        /// <summary>
        /// 构建下单日期 - CheckBoxList
        /// </summary>
        /// <returns></returns>
        private string BuildSelectVendorBuyWeekDayString()
        {
            string returnStr = string.Empty;
            //foreach (var chkItem in this.spVendorBuyWeekCheckBoxList.Children)
            //{
            //    CheckBox chkBox = chkItem as CheckBox;
            //    if (null != chkBox && chkBox.IsChecked.HasValue && chkBox.IsChecked.Value)
            //    {
            //        returnStr += string.Format("{0};", chkBox.Tag.ToString());
            //    }
            //}
            return (!string.IsNullOrEmpty(returnStr) ? returnStr.TrimEnd(';') : returnStr);

        }

        private void BindVendorBuyWeekDayCheckBoxList(string buyWeekDayString)
        {
            //if (!string.IsNullOrEmpty(buyWeekDayString))
            //{
            //    string[] strs = buyWeekDayString.Split(';');
            //    foreach (var strWeek in strs)
            //    {
            //        if (!string.IsNullOrEmpty(strWeek))
            //        {
            //            var chkItem = this.spVendorBuyWeekCheckBoxList.Children.SingleOrDefault(i => ((CheckBox)i).Tag.ToString() == strWeek.Trim()) as CheckBox;
            //            if (null != chkItem)
            //            {
            //                ((CheckBox)chkItem).IsChecked = true;
            //            }
            //        }
            //    }
            //}
        }

        #endregion

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            //新建供应商申请表 - 上传:
            VendorAttachmentsUpload uploadFileCtrl = new VendorAttachmentsUpload();
            uploadFileCtrl.Dialog = Window.ShowDialog(ResVendorMaintain.Label_UploadFileTitle, uploadFileCtrl, (obj, args) =>
            {
                if (args.Data != null && args.DialogResult == DialogResultType.OK)
                {
                    //上传成功!返回文件地址:
                    serviceFacade.ModeVendorAttchmentFile(args.Data.ToString(), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }

                        switch (btn.Name)
                        {
                            case "btnUpload_VendorNewApplyForm":
                                this.hpkVendorNewApplyForm.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkVendorNewApplyForm.Tag = args2.Result.ToString();
                                break;
                            case "btnUpload_OrganizeCodeCertificate":
                                this.hpkOrganizeCodeCertificate.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkOrganizeCodeCertificate.Tag = args2.Result.ToString();

                                break;
                            case "btnUpload_EntBizLicence":
                                this.hpkEntBizLicence.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkEntBizLicence.Tag = args2.Result.ToString();

                                break;
                            case "btnUpload_TaxRegistration":
                                this.hpkTaxRegistration.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkTaxRegistration.Tag = args2.Result.ToString();

                                break;
                            case "btnUpload_AgreementBeingSold":
                                this.hpkAgreementBeingSold.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkAgreementBeingSold.Tag = args2.Result.ToString();

                                break;
                            case "btnUpload_AgreementConsign":
                                this.hpkAgreementConsign.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkAgreementConsign.Tag = args2.Result.ToString();

                                break;
                            case "btnUpload_AgreementAfterSold":
                                this.hpkAgreementAfterSold.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkAgreementAfterSold.Tag = args2.Result.ToString();

                                break;
                            case "btnUpload_AgreementOther":
                                this.hpkAgreementOther.Content = ResVendorMaintain.Label_DownloadAttachment;
                                this.hpkAgreementOther.Tag = args2.Result.ToString();

                                break;
                            default:
                                break;
                        }
                    });
                }
            }, new Size(410, 100));
        }

        private void DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            if (null != btn && btn.Tag != null)
            {
                AppSettingHelper.GetSetting("PO", "VendorAttachmentFilesPath", (obj, args) =>
                {

                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    string url = System.IO.Path.Combine(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl"), args.Result, btn.Tag.ToString());
                    UtilityHelper.OpenWebPage(url.Replace('\\', '/'));
                });
            }
        }

        private void btnEditVendorMailAddress_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //编辑供应商:
            VendorMailAddressMaintain vendorAddressUC = new VendorMailAddressMaintain(BuildVendorMailAddresList());
            vendorAddressUC.Dialog = Window.ShowDialog("维护电子邮件", vendorAddressUC, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    List<string> getVendorMailAddressList = args.Data as List<string>;
                    if (null != getVendorMailAddressList)
                    {
                        BindVendorMailAddressList(getVendorMailAddressList);
                    }
                }
            }, new Size(480, 260));
        }

        private void BindVendorMailAddressList(List<string> vendorMailAddressList)
        {
            this.cmbVendorMailAddress.Items.Clear();
            vendorMailAddressList.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x))
                {
                    this.cmbVendorMailAddress.Items.Add(x);
                }
            });
            if (vendorMailAddressList.Count > 0)
            {
                this.cmbVendorMailAddress.SelectedIndex = 0;
            }
            this.vendorInfoVM.VendorBasicInfo.EmailAddress = vendorMailAddressList.Join(";");
            this.vendorInfoVM.VendorBasicInfo.EmailAddress = vendorInfoVM.VendorBasicInfo.EmailAddress.Length <= 0 ? null : vendorInfoVM.VendorBasicInfo.EmailAddress;
        }

        private List<string> BuildVendorMailAddresList()
        {
            List<string> vendorMailList = new List<string>();
            if (!string.IsNullOrEmpty(vendorInfoVM.VendorBasicInfo.EmailAddress))
            {
                if (vendorInfoVM.VendorBasicInfo.EmailAddress.IndexOf(';') >= 0)
                {
                    string[] list = vendorInfoVM.VendorBasicInfo.EmailAddress.Split(';');
                    foreach (var item in list)
                    {
                        vendorMailList.Add(item.Trim());
                    }
                }
                else
                {
                    vendorMailList.Add(vendorInfoVM.VendorBasicInfo.EmailAddress);
                }
            }
            return vendorMailList;
        }

        private void BindVendorEmailAddress(string vendorMailAddress)
        {
            if (!string.IsNullOrEmpty(vendorMailAddress))
            {
                if (vendorMailAddress.IndexOf(';') >= 0)
                {
                    string[] items = vendorMailAddress.Split(';');
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            this.cmbVendorMailAddress.Items.Add(item);
                        }
                    }
                }
                else
                {
                    this.cmbVendorMailAddress.Items.Add(vendorMailAddress);
                }
            }
            this.cmbVendorMailAddress.SelectedIndex = 0;
        }

        private void hlbtnViewDetials_Click(object sender, RoutedEventArgs e)
        {
            //查看修改内容
            VendorAgentInfoVM selectVM = this.dataGrid_VendorAgentInfo.SelectedItem as VendorAgentInfoVM;
            string[] tmp = selectVM.Content.Split('#');
            string result = string.Empty;
            if (tmp.Length > 1)
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    result += tmp[i] + "\n";
                }
            }
            Window.Alert(result.Replace("#", string.Empty));
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.vendorInfoVM.VendorDeductInfo.DeductSysNo = 0;
            this.vendorInfoVM.VendorDeductInfo.DeductPercent = "0";
            this.vendorInfoVM.VendorDeductInfo.FixAmt = "0";
            this.vendorInfoVM.VendorDeductInfo.MaxAmt = "0";
            this.vendorInfoVM.VendorDeductInfo.CalcType = VendorCalcType.Cost;
        }

        private void gridVendorDeductInfo_Loaded(object sender, RoutedEventArgs e)
        {
            if (vendorInfoVM == null)
            {
                return;
            }
            if (vendorInfoVM.VendorDeductInfo.CalcType == VendorCalcType.Fix)
            {
                txtDeductPercent.Text = "0";
                txtMaxAmt.Text = "0";
                txtDeductPercent.IsEnabled = false;
                txtMaxAmt.IsEnabled = false;
            }
            else
            {
                txtDeductPercent.IsEnabled = true;
                txtMaxAmt.IsEnabled = true;
            }
        }

        private void Button_LanguageSetting_Click(object sender, RoutedEventArgs e)
        {

            UCLanguagesDescription ut3 = new UCLanguagesDescription("Merchant", vendorInfoVM.SysNo.Value.ToString());
            ut3.Dialog = Window.ShowDialog("多语言设置", ut3, (s, args) =>
            {

            });
        }

        private void btnExportBrandInfo_Click(object sender, RoutedEventArgs e)
        {
            int vendorId = vendorInfoVM.SysNo.Value;
            ColumnSet col = new ColumnSet();
            col.Insert(0, "ManufacturerName", "厂商", 30);
            col.Insert(1, "BrandName", "品牌", 30);
            col.Insert(2, "BrandInspectionNo", "商检编号", 30);

            this.serviceFacade.ExportExcelForVendorBrandFiling(vendorId, new[] { col });
        }
    }
}
