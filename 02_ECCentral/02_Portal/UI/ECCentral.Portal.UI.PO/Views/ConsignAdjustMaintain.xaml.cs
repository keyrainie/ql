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
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ConsignAdjustMaintain : PageBase
    {
        private string getSysNo;
        private ConsignAdjustVM newVM;
        private ConsignAdjustFacade serviceFacade;
        public ConsignAdjustMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            newVM = new ConsignAdjustVM();
            serviceFacade = new ConsignAdjustFacade(this);
            getSysNo = this.Request.Param;
            if (null != getSysNo)
            {
                LoadBasicInfo();
            }
        }

        //private void RefreshCurrent()
        //{
        //    Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/ConsignAdjustMaintain/{0}", getSysNo), null);
        //}
        private void DeductGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.DeductGrid.ItemsSource = newVM.ItemList;
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {
                if (null != this.DeductGrid.ItemsSource)
                {

                    foreach (var item in this.DeductGrid.ItemsSource)
                    {
                        if (item is ConsignAdjustItemVM)
                        {
                            ((ConsignAdjustItemVM)item).IsCheckedItem = chk.IsChecked.Value;
                        }
                    }
                }
            }
        }

        #region [Event]
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            if (string.IsNullOrEmpty(newVM.SettleSysNo))
            {
                Window.Alert("请先匹配可调整的结算单!");
                return;
            }

            Window.Confirm(ResConsignAdjustNew.InfoMsg_ConfirmModify, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ConsignAdjustInfo Info = EntityConverter<ConsignAdjustVM, ConsignAdjustInfo>.Convert(newVM);

                    if (null != Info.SysNo)
                    {
                        if (Info.ItemList.Count == 0)
                        {
                            Window.Alert("请至少添加一条扣款项信息");
                            return;
                        }

                        Info.TotalAmt = Info.ItemList.Sum(p => p.DeductAmt);
                        serviceFacade.Update(Info, (obj1, args1) =>
                        {
                            if (args1.FaultsHandle())
                            {
                                return;
                            }

                            if (args1.Result.SysNo > 0)
                            {
                                Window.Alert(ResConsignAdjustNew.AlertMsg_ModifySuc);
                            }
                            else
                            {
                                Window.Alert(ResConsignAdjustNew.AlertMsg_ModifyFailed);
                                return;
                            }

                        });
                    }
                    else
                    {
                        Window.Alert(ResConsignAdjustNew.AlertMsg_ModifySuc);
                    }
                }
            });
        }

        private void btnAddDeduct_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            UCDeductList deductWindow = new UCDeductList();
            deductWindow.Dialog = Window.ShowDialog("添加扣款项", deductWindow, (obj, args) =>
            {
                if (args.Data != null && args.DialogResult == DialogResultType.OK)
                {
                    List<ConsignAdjustItemVM> addDeduct = args.Data as List<ConsignAdjustItemVM>;
                    List<ConsignAdjustItemVM> source = DeductGrid.ItemsSource as List<ConsignAdjustItemVM>;
                    bool isOK = true;
                    if (source != null)
                    {
                        foreach (var item in source)
                        {
                            //item.ConsignAdjustSysNo=newVM
                            var find = addDeduct.Where(p => p.DeductSysNo == item.DeductSysNo);
                            if (find.Count() > 0)
                            {
                                isOK = false;
                                Window.Alert("添加的选项中包含已经存在的记录，请检查");
                                break;
                            }
                        }
                        if (isOK)
                        {
                            source.AddRange(addDeduct);
                            newVM.ItemList = source;
                            DeductGrid.Bind();
                        }
                    }

                }
            }, new Size(500, 400));

        }

        //从list中移除选中扣款项
        private void btnDelDeduct_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResConsignAdjustNew.AlertMsg_ConfirmDelDeduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<ConsignAdjustItemVM> source = this.DeductGrid.ItemsSource as List<ConsignAdjustItemVM>;

                    if (null != source)
                    {
                        newVM.ItemList = source.Where(p => !p.IsCheckedItem).ToList();
                        this.DeductGrid.Bind();
                    }
                }
            });
        }

        //审核(1--通过)
        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            ConsignAdjustInfo Info = EntityConverter<ConsignAdjustVM, ConsignAdjustInfo>.Convert(newVM);

            if (null != Info.SysNo)
            {
                Info.Status = ConsignAdjustStatus.Audited;
                serviceFacade.Audit(Info, (obj3, args3) =>
                {
                    if (args3.FaultsHandle())
                    {                      
                        return;
                    }

                    if (args3.Result.SysNo > 0)
                    {
                        Window.Alert("审核通过");
                        LoadBasicInfo();
                    }
                    else
                    {
                        Window.Alert("审核不通过");
                    }
                });
            }                   
        }

        //作废(-1--作废)
        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            ConsignAdjustInfo Info = EntityConverter<ConsignAdjustVM, ConsignAdjustInfo>.Convert(newVM);

            if (null != Info.SysNo)
            {
                Info.Status = ConsignAdjustStatus.Abandon;
                serviceFacade.Audit(Info, (obj3, args3) =>
                {
                    if (args3.FaultsHandle())
                    {
                        //Window.Alert(args3.Error.Faults[0].ErrorDescription);
                        return;
                    }

                    if (args3.Result.SysNo > 0)
                    {
                        Window.Alert("作废成功");
                        LoadBasicInfo();
                    }
                    else
                    {
                        Window.Alert("作废失败");
                    }
                });
            }            
        }
        #endregion

        public void LoadBasicInfo()
        {
            serviceFacade.LoadInfo(getSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    Window.Alert(args.Error.Faults[0].ErrorDescription);
                    return;
                }

                if (null != args.Result)
                {
                    newVM = EntityConverter<ConsignAdjustInfo, ConsignAdjustVM>.Convert(args.Result);
                    newVM.SettleRangeDate = Convert.ToDateTime(newVM.SettleRange);
                    this.DataContext = newVM;
                    this.DeductGrid.ItemsSource = newVM.ItemList;

                    //已审核过和作废的单据仅能查看，待审核的可编辑
                    if (this.newVM.Status == ConsignAdjustStatus.WaitAudit)
                    {
                        this.btnSave.IsEnabled = true;
                        this.btnAddDeduct.IsEnabled = true;
                        this.btnDel.IsEnabled = true;
                        this.btnAudit.IsEnabled = true;
                        this.btnAbandon.IsEnabled = true;
                    }
                    else
                    {
                        this.btnSave.IsEnabled = false;
                        this.btnAddDeduct.IsEnabled = false;
                        this.btnDel.IsEnabled = false;
                        this.btnAudit.IsEnabled = false;
                        this.btnAbandon.IsEnabled = false;
                    }
                }
                else
                {
                    Window.Alert("无效的单据号");
                    return;
                }
            });
        }
    }
}
