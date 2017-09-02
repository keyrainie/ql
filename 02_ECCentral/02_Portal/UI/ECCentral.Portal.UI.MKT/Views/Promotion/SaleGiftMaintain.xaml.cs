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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.ComponentModel.DataAnnotations;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SaleGiftMaintain : PageBase
    {
        public SaleGiftInfoViewModel _saleGiftInfoVM = new SaleGiftInfoViewModel();
        private SaleGiftDiscountBelongType? _discountBelongSnap;
        public SaleGiftFacade _facade;
        private string _operationType = "";
        int? currentSysNo = null;

        public SaleGiftMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            _facade = new SaleGiftFacade(this);

            int tempsysNo = -1;           

            if (this.Request.QueryString != null
                && this.Request.QueryString.ContainsKey("sysno")
                && int.TryParse(this.Request.QueryString["sysno"].ToString().Trim(), out tempsysNo))
            {
                currentSysNo = tempsysNo;
                if (this.Request.QueryString.ContainsKey("operation")
                    && !string.IsNullOrEmpty(this.Request.QueryString["operation"].ToString().Trim()))
                {
                    _operationType = this.Request.QueryString["operation"].ToString().ToLower();
                }

            }

            if (_operationType == "edit")
            {
                //TODO：判断是否有编辑的权限，如果没有，则将自动变为view权限
            }
            else if (_operationType == "mgt")
            {
            }
            else
            {
            }

            _saleGiftInfoVM.BeginDate = DateTime.Now;
            _saleGiftInfoVM.EndDate = DateTime.Now.AddDays(1);

            _facade.Load(currentSysNo, _saleGiftInfoVM,(obj, args) =>
            {
                _saleGiftInfoVM = args.Result;
                this.DataContext = _saleGiftInfoVM;
                SetControlByOperation();
                SetCanelAuditByRequest();
                _discountBelongSnap = _saleGiftInfoVM.DisCountType;
            });

             
        }

        /// <summary>
        /// 商家商品不能撤销审核
        /// </summary>
        private void SetCanelAuditByRequest()
        {
            if (_saleGiftInfoVM.RequestSysNo > 0)
            {
                btnCancelAudit.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnCancelAudit.Visibility = Visibility.Visible;
            }
        }

        private void SetControlByOperation()
        {
            tabItemProductRange.Visibility = System.Windows.Visibility.Collapsed;
            tabItemGiftRule.Visibility = System.Windows.Visibility.Collapsed;
            if (_saleGiftInfoVM.SysNo.HasValue)
            {
                tabItemProductRange.Visibility = System.Windows.Visibility.Visible;
                tabItemGiftRule.Visibility = System.Windows.Visibility.Visible;
            }
             
            if (_saleGiftInfoVM.Type.Value == SaleGiftType.Full)
            {
                ucBasic.tbMinAmount.SetReadOnly(false);
            }
            else
            {
                ucBasic.tbMinAmount.SetReadOnly(true);
            }

            spFunction.Visibility = System.Windows.Visibility.Collapsed;
            _saleGiftInfoVM.IsOnlyViewMode = true;
            if (_saleGiftInfoVM.SysNo.HasValue)
            {
                this.ucBasic.cmbType.IsEnabled = false;                

                if (_operationType == "edit")
                {
                    _saleGiftInfoVM.IsOnlyViewMode = false;

                    //this.Title = string.Format("赠品[{0}]-编辑", _saleGiftInfoVM.SysNo);
                    this.Title = string.Format(ResSaleGiftMaitain.Msg_GiftEdit, _saleGiftInfoVM.SysNo);
                }
                else if (_operationType == "mgt")
                {
                    //this.Title = string.Format("赠品[{0}]-管理", _saleGiftInfoVM.SysNo);
                    this.Title = string.Format(ResSaleGiftMaitain.Msg_GiftManage, _saleGiftInfoVM.SysNo);
                    spFunction.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                   // this.Title = string.Format("赠品[{0}]-浏览", _saleGiftInfoVM.SysNo);
                    this.Title = string.Format(ResSaleGiftMaitain.Msg_GiftView, _saleGiftInfoVM.SysNo);
                }
            }
            else
            {
                _saleGiftInfoVM.IsOnlyViewMode = false;
                this.ucBasic.cmbType.IsEnabled = true;
                //this.Title = string.Format("赠品-新增");
                this.Title = string.Format(ResSaleGiftMaitain.Msg_AddNew);
            }

            if (_saleGiftInfoVM.Type.Value == SaleGiftType.Full)
            //|| _saleGiftInfoVM.Type.Value == SaleGiftType.FirstOrder   || _saleGiftInfoVM.Type.Value == SaleGiftType.Additional)
            {
                if (!currentSysNo.HasValue)
                {
                    _saleGiftInfoVM.IsGlobalProduct = true;
                }

                this.ucProductRange.ucLimitProduct.Visibility = System.Windows.Visibility.Collapsed;
                this.ucProductRange.rowLimitProduct.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Auto));//加上这句防止页面出现大量空白区域
                this.ucProductRange.ucLimitScope.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                //对于非“买满即送”，是否整网商品永远是False
                _saleGiftInfoVM.IsGlobalProduct = false;
                this.ucProductRange.ucLimitProduct.Visibility = System.Windows.Visibility.Visible;
                this.ucProductRange.ucLimitScope.Visibility = System.Windows.Visibility.Collapsed;
                this.ucProductRange.rowLimitScope.SetValue(RowDefinition.HeightProperty, new GridLength(0, GridUnitType.Auto));//加上这句防止页面出现大量空白区域
            }


            if (_operationType.ToLower() == "mgt")
            {
                btnSubmitAudit.Visibility = System.Windows.Visibility.Collapsed;
                btnCancelAudit.Visibility = System.Windows.Visibility.Collapsed;
                btnAuditApprove.Visibility = System.Windows.Visibility.Collapsed;
                btnAuditRefuse.Visibility = System.Windows.Visibility.Collapsed;
                btnStop.Visibility = System.Windows.Visibility.Collapsed;
                btnVoid.Visibility = System.Windows.Visibility.Collapsed;
                switch (_saleGiftInfoVM.Status.Value)
                {
                    case SaleGiftStatus.Init:
                        btnSubmitAudit.Visibility = System.Windows.Visibility.Visible;                
                        break;
                    case SaleGiftStatus.WaitingAudit:
                        btnCancelAudit.Visibility = System.Windows.Visibility.Visible;
                        btnAuditApprove.Visibility = System.Windows.Visibility.Visible;
                        btnAuditRefuse.Visibility = System.Windows.Visibility.Visible;                  
                        break;
                    case SaleGiftStatus.Ready:
                        btnCancelAudit.Visibility = System.Windows.Visibility.Visible;
                        btnVoid.Visibility = System.Windows.Visibility.Visible;     
                        break;
                    case SaleGiftStatus.Run:
                        btnStop.Visibility = System.Windows.Visibility.Visible;     
                        break;
                    default:
                        break;
                }

            }


            if (_saleGiftInfoVM.IsOnlyViewMode)
            {
                this.btnSaveBasic.Visibility = System.Windows.Visibility.Collapsed;
                OperationControlStatusHelper.SetControlsStatus(this.ucBasic, true);
                OperationControlStatusHelper.SetControlsStatus(this.ucGiftRule, true);
                OperationControlStatusHelper.SetControlsStatus(this.ucProductRange, true);
            }
        }

        private void btnSaveBasic_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.MessageBox.Clear();

            ValidationManager.Validate(this.ucBasic.gridBasic);
            if (_saleGiftInfoVM.HasValidationErrors)
            {
                return;
            }

            if (_saleGiftInfoVM.Type != SaleGiftType.Vendor)
            {
                if (string.IsNullOrEmpty(_saleGiftInfoVM.Title))
                {
                    _saleGiftInfoVM.ValidationErrors.Add(new ValidationResult(ResSaleGiftMaitain.Msg_RequireTitle, new List<string> { "Title" }));
                }
            }

            if (_saleGiftInfoVM.Type == SaleGiftType.Full)
            //|| _saleGiftInfoVM.Type == SaleGiftType.FirstOrder || _saleGiftInfoVM.Type == SaleGiftType.Additional)
            {

                if (string.IsNullOrEmpty(_saleGiftInfoVM.Description))
                {
                    _saleGiftInfoVM.ValidationErrors.Add(new ValidationResult(ResSaleGiftMaitain.Msg_RequireDescription, new List<string> { "Description" }));
                }

                if (string.IsNullOrEmpty(_saleGiftInfoVM.OrderCondition.OrderMinAmount))
                {
                    _saleGiftInfoVM.ValidationErrors.Add(new ValidationResult(ResSaleGiftMaitain.Msg_RequireOrderMinAmount, new List<string> { "OrderMinAmount" }));
                }
               
            }
            if (_saleGiftInfoVM.HasValidationErrors) return;

            _saleGiftInfoVM.ValidationErrors.Clear();

            bool needReload = false;
            if (_saleGiftInfoVM.SysNo.HasValue && _saleGiftInfoVM.DisCountType != _discountBelongSnap)
            {
                needReload = true;
            }
 
            _facade.SaveMaster(_saleGiftInfoVM, (obj, args) =>
            {
                _saleGiftInfoVM = args.Result;

                if (needReload)
                {
                    _facade.Load(_saleGiftInfoVM.SysNo, _saleGiftInfoVM, (obj1, args1) =>
                    {
                        _saleGiftInfoVM = args1.Result;
                        this.DataContext = _saleGiftInfoVM;
                        if (_saleGiftInfoVM.ProductOnlyList != null && _saleGiftInfoVM.ProductOnlyList.Count > 0)
                        {
                            this.ucProductRange.ucLimitProduct.dgProductOnly.ItemsSource = _saleGiftInfoVM.ProductOnlyList;                            
                        }

                        if (_saleGiftInfoVM.GiftItemList != null && _saleGiftInfoVM.GiftItemList.Count > 0)
                        {
                            this.ucGiftRule.dgGiftProduct.ItemsSource = _saleGiftInfoVM.GiftItemList;                            
                        }
                        _discountBelongSnap = _saleGiftInfoVM.DisCountType;
                        //Window.Alert("保存成功！");
                        Window.Alert(ResSaleGiftMaitain.Msg_SaveSuccess);
                    });
                }
                else
                {
                    _operationType = "edit";
                   
                    SetControlByOperation();
                    //Window.Alert("保存成功！");
                    Window.Alert(ResSaleGiftMaitain.Msg_SaveSuccess);
                }
            });
            
        }

        private void btnSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
           // SaleGiftProcess(PSOperationType.SubmitAudit, "提交审核", SaleGiftStatus.WaitingAudit);
            SaleGiftProcess(PSOperationType.SubmitAudit, ResSaleGiftMaitain.Msg_SubAudit, SaleGiftStatus.WaitingAudit);
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            //SaleGiftProcess(PSOperationType.CancelAudit, "撤销审核", SaleGiftStatus.Init);
            SaleGiftProcess(PSOperationType.CancelAudit, ResSaleGiftMaitain.Msg_CancelAudit, SaleGiftStatus.Init);
        }

        private void btnAuditApprove_Click(object sender, RoutedEventArgs e)
        {
            //SaleGiftProcess(PSOperationType.AuditApprove, "审核通过", SaleGiftStatus.Ready);
            SaleGiftProcess(PSOperationType.AuditApprove, ResSaleGiftMaitain.Msg_AppoveAudit, SaleGiftStatus.Ready);
        }

        private void btnAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            //SaleGiftProcess(PSOperationType.AuditRefuse, "审核拒绝", SaleGiftStatus.Init);
            SaleGiftProcess(PSOperationType.AuditRefuse, ResSaleGiftMaitain.Msg_RefuseAuit, SaleGiftStatus.Void);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //SaleGiftProcess(PSOperationType.Stop, "中止", SaleGiftStatus.Stoped);
            SaleGiftProcess(PSOperationType.Stop, ResSaleGiftMaitain.Msg_StopAudit, SaleGiftStatus.Stoped);
        }

        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            //SaleGiftProcess(PSOperationType.Void, "作废", SaleGiftStatus.Void);
            SaleGiftProcess(PSOperationType.Void, ResSaleGiftMaitain.Msg_VoidAudit, SaleGiftStatus.Void);
        }


        private void SaleGiftProcess(PSOperationType opt, string optname, SaleGiftStatus targetStatus)
        {
            List<int?> sysNoList = new List<int?>();
            sysNoList.Add(_saleGiftInfoVM.SysNo);

            _facade.BatchProcessSaleGift(sysNoList, opt, (obj, args) =>
            {
                if (args.Result.FailureRecords.Count == 0)
                {
                    if (opt == PSOperationType.SubmitAudit)
                    {
                        _facade.Load(_saleGiftInfoVM.SysNo, _saleGiftInfoVM, (obj1, args1) =>
                        {
                            _saleGiftInfoVM = args1.Result;
                            this.DataContext = _saleGiftInfoVM;
                            SetControlByOperation();
                        });
                    }
                    else
                    {
                        _saleGiftInfoVM.Status = targetStatus;
                        SetControlByOperation();
                    }
                    //Window.Alert(string.Format("赠品活动-{0}成功！", optname));
                    Window.Alert(string.Format(ResSaleGiftMaitain.Msg_DealSuccess, optname));
                }
                else
                {
                    string msg = args.Result.FailureRecords.Join("\r\n") + Environment.NewLine;
                    Window.Alert(msg);
                }

            });

        }
    }

}
