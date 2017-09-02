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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VendorRMARefundMaintain : PageBase
    {
        public string RefundSysNo { get; set; }
        public VendorRefundFacade serviceFacade;
        public VendorRefundInfoVM viewVM;

        public bool isPM = true;
        public bool isPMD = false;
        public bool isPMCC = false;

        public VendorRMARefundMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            viewVM = new VendorRefundInfoVM();
            serviceFacade = new VendorRefundFacade(this);
            RefundSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(RefundSysNo))
            {
                //加载供应商退款单详细信息:
                LoaPORefundDetailInfo();
            }
        }

        /// <summary>
        /// 根据状态来隐藏相应的操作按钮
        /// </summary>
        private void HideActionButtons()
        {
            if (viewVM.Status.HasValue)
            {
                EnableMemoTextBoxs(isPM, isPMD, isPMCC);
                switch (viewVM.Status.Value)
                {
                    case VendorRefundStatus.Abandon:
                    //废弃状态:
                    case VendorRefundStatus.Origin:
                        //初始状态 ：
                        this.btnAuditPassed.IsEnabled = false;
                        this.btnAuditDenied.IsEnabled = false;
                        this.btnUpdate.IsEnabled = false;
                        break;
                    case VendorRefundStatus.PMDVerify:
                        //PMD已审核:

                        this.btnAuditPassed.IsEnabled = false;
                        this.btnAuditDenied.IsEnabled = false;
                        this.btnUpdate.IsEnabled = false;

                        break;
                    case VendorRefundStatus.PMCCVerify:
                        //PMCC已审核:
                        this.btnAuditPassed.IsEnabled = false;
                        this.btnAuditDenied.IsEnabled = false;
                        this.btnUpdate.IsEnabled = false;
                        break;
                    case VendorRefundStatus.PMCCToVerify:
                        //待PMCC审核:
                        if ((isPMD || isPM) && !isPMCC)
                        {
                            this.btnAuditPassed.IsEnabled = false;
                            this.btnAuditDenied.IsEnabled = false;
                            this.btnUpdate.IsEnabled = false;
                        }
                        else if (!isPM && !isPMD && !isPMCC)
                        {
                            this.btnAuditPassed.IsEnabled = false;
                            this.btnAuditDenied.IsEnabled = false;
                            this.btnUpdate.IsEnabled = false;
                        }
                        break;
                    case VendorRefundStatus.Verify:
                        //已提交审核:
                        if (!isPM && !isPMD && !isPMCC)
                        {
                            this.btnAuditPassed.IsEnabled = false;
                            this.btnAuditDenied.IsEnabled = false;
                            this.btnUpdate.IsEnabled = false;
                        }
                        break;
                    case VendorRefundStatus.PMVerify:
                        //PM已审核:
                        if (isPM && !isPMCC && !isPMD)
                        {
                            this.btnAuditPassed.IsEnabled = false;
                            this.btnAuditDenied.IsEnabled = false;
                        }
                        else if (!isPM && !isPMD && !isPMCC)
                        {
                            this.btnAuditPassed.IsEnabled = false;
                            this.btnAuditDenied.IsEnabled = false;
                            this.btnUpdate.IsEnabled = false;
                        }
                        break;
                    default:
                        this.btnAuditPassed.IsEnabled = false;
                        this.btnAuditDenied.IsEnabled = false;
                        this.btnUpdate.IsEnabled = false;
                        break;
                }
            }

        }

        private void EnableMemoTextBoxs(bool isPM, bool isPMD, bool isPMCC)
        {
            if (isPMCC)
            {
                this.txtPMCCMemo.IsReadOnly = false;
            }
            else if (isPMD)
            {
                this.txtPMDMemo.IsReadOnly = false;
            }
            else if (isPM)
            {
                this.txtPMMemo.IsReadOnly = false;
            }
        }

        private void LoaPORefundDetailInfo()
        {
            serviceFacade.LoadVendorRefundInfo(RefundSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.viewVM = EntityConverter<VendorRefundInfo, VendorRefundInfoVM>.Convert(args.Result);
                this.DataContext = viewVM;
                this.gridRefundProductsList.Bind();

                //权限判断:PM,PMD,PMCC
                isPM = viewVM.UserRole == "PM";
                isPMD = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VendorRefund_PMDVerify);
                isPMCC = AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VendorRefund_PMCCVerify);
                if (isPMD)
                {
                    viewVM.UserRole = "PMD";
                }
                else if (isPMCC)
                {
                    viewVM.UserRole = "PMCC";
                }

                #region [验证权限角色]
                if (viewVM.NotPMAndPMD == true && !isPMD && !isPMCC)
                {
                    this.lblAlertText.Text = "您不是当前产品的PM，也不是当前产品PM的备份PM，无法审核！";
                    this.btnUpdate.IsEnabled = false;
                    this.btnAuditPassed.IsEnabled = false;
                    this.btnAuditDenied.IsEnabled = false;
                    return;
                }
                if (!isPM && !isPMD && !isPMCC)
                {
                    this.lblAlertText.Text = " 你既不是PM,也不是PMD,无法进行任何操作 !";
                    this.btnUpdate.IsEnabled = false;
                    this.btnAuditPassed.IsEnabled = false;
                    this.btnAuditDenied.IsEnabled = false;
                    return;
                }
                #endregion

                HideActionButtons();
            });
        }

        #region [Events]

        private void Hyperlink_RegisterSysNo_Click(object sender, RoutedEventArgs e)
        {
            //跳转到 RMA - 单件处理中心:
            VendorRefundItemInfoVM getSelectedObj = this.gridRefundProductsList.SelectedItem as VendorRefundItemInfoVM;
            if (null != getSelectedObj)
            {
                Window.Navigate(string.Format("/ECCentral.Portal.UI.RMA/RegisterMaintain/{0}", getSelectedObj.RegisterSysNo.Value.ToString()), null, true);
            }
        }

        private void gridRefundProductsList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.gridRefundProductsList.ItemsSource = this.viewVM.ItemList;
        }

        private void btnAuditPassed_Click(object sender, RoutedEventArgs e)
        {
            //审核通过操作 ：
            string confirmMsg = string.Empty;
            serviceFacade.GetVendorPayBalanceByVendorSysNo(viewVM.VendorSysNo.Value, (obj3, args3) =>
            {

                if (args3.FaultsHandle())
                {
                    return;
                }

                if (args3.Result >= 0 && args3.Result < (viewVM.RefundCashAmt??0m))
                {
                    //根据权限来显示:
                    if (isPM)
                    {
                        confirmMsg = ResVendorRMARefundMaintain.ConfirmMsg_VendorPayBalanceVerifyResoucePM;
                    }
                    else if (isPMD)
                    {
                        confirmMsg = ResVendorRMARefundMaintain.ConfirmMsg_VendorPayBalanceVerifyResoucePMD;
                    }
                    else if (isPMCC)
                    {
                        confirmMsg = ResVendorRMARefundMaintain.ConfirmMsg_VendorPayBalanceVerifyResoucePMCC;
                    }
                    else
                    {

                    }
                }
                if (!string.IsNullOrEmpty(confirmMsg))
                {
                    //角色是PM:
                    if (isPM)
                    {
                        Window.Alert(confirmMsg);
                        return;
                    }
                    //角色是PMD:
                    else if (isPMD)
                    {
                        Window.Confirm(confirmMsg, (obj, args) =>
                      {
                          if (args.DialogResult == DialogResultType.OK)
                          {
                              VendorRefundInfo info = EntityConverter<VendorRefundInfoVM, VendorRefundInfo>.Convert(viewVM);
                              serviceFacade.SubmitToPMCC(info, (obj2, args2) =>
                              {
                                  if (args2.FaultsHandle())
                                  {
                                      return;
                                  }
                                  Window.Alert(ResVendorRMARefundMaintain.AlertMsg_AuditSuc);
                                  Window.Refresh();
                              });
                          }
                      });
                    }
                    //角色是PMCC:
                    else if (isPMCC)
                    {
                        Window.Confirm(confirmMsg, (obj4, args4) =>
                          {
                              if (args4.DialogResult == DialogResultType.OK)
                              {
                                  ApproveAction();
                              }
                          });
                    }
                }
                Window.Confirm(ResVendorRMARefundMaintain.AlertMsg_ConfirmAudit, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        ApproveAction();
                    }
                });
            });
        }

        private void btnAuditDenied_Click(object sender, RoutedEventArgs e)
        {
            //审核拒绝操作 ：
            Window.Confirm(ResVendorRMARefundMaintain.AlertMsg_ConfirmAuditDinied, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    VendorRefundInfo info = EntityConverter<VendorRefundInfoVM, VendorRefundInfo>.Convert(viewVM);
                    serviceFacade.RejectVendorRefundInfo(info, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResVendorRMARefundMaintain.AlertMsg_AuditSuc);
                        Window.Refresh();
                    });
                }
            });
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //更新操作:
            VendorRefundInfo info = EntityConverter<VendorRefundInfoVM, VendorRefundInfo>.Convert(viewVM);
            serviceFacade.UpdateVendorRefundInfo(info, (obj2, args2) =>
            {
                if (args2.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResVendorRMARefundMaintain.AlertMsg_UpdateSuc);
                Window.Refresh();
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //关闭:
            this.Window.Close(true);
        }

        private void ApproveAction()
        {
            VendorRefundInfo info = EntityConverter<VendorRefundInfoVM, VendorRefundInfo>.Convert(viewVM);
            serviceFacade.ApproveVendorRefundInfo(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResVendorRMARefundMaintain.AlertMsg_AuditSuc);
                Window.Refresh();
            });
        }

        #endregion
    }

}
