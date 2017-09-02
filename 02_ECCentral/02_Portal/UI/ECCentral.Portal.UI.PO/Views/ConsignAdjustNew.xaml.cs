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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Collections;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic.Utilities;
using System.Text.RegularExpressions;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(NeedAccess = false, IsSingleton = true)]
    public partial class ConsignAdjustNew : PageBase
    {

        private ConsignAdjustVM newVM;
        private ConsignAdjustFacade serviceFacade;
        public ConsignAdjustNew()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            newVM = new ConsignAdjustVM { SettleRangeDate=DateTime.Today};
            serviceFacade = new ConsignAdjustFacade(this);
            this.DataContext = newVM;
            this.DeductGrid.Bind();
        }
        private void DeductGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
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

            ConsignAdjustInfo Info = EntityConverter<ConsignAdjustVM, ConsignAdjustInfo>.Convert(newVM);
            if (Info.ItemList.Count == 0)
            {
                Window.Alert("请至少添加一条扣款项信息");
                return;
            }
            Info.TotalAmt = Info.ItemList.Sum(p => p.DeductAmt);
            serviceFacade.Add(Info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.SysNo > 0)
                {
                    Window.Alert(ResConsignAdjustNew.Msg_Title, ResConsignAdjustNew.Msg_CreateSuc, MessageType.Information, (objj, argss) =>
                    {
                        if (argss.DialogResult == DialogResultType.Cancel)
                        {
                            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/ConsignAdjustMaintain/{0}", args.Result.SysNo), true);
                        }
                    });
                }
                else
                {
                    Window.Alert(ResConsignAdjustNew.Msg_CreateFailed);
                    return;
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
        private void btnLoadSettleInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!newVM.VenderSysNo.HasValue)
            {
                 Window.Alert("请选择供应商");
                 return;
            }
            if (!newVM.PMSysNo.HasValue)
            {
                Window.Alert("请选择产品经理");
                return;
            }
            ConsignSettlementFacade  frcade= new ConsignSettlementFacade(this);
            ConsignSettleQueryFilter filter = new ConsignSettleQueryFilter
            {
                VendorSysNo = newVM.VenderSysNo,
                PMSysno = newVM.PMSysNo,
                IsManagerPM=true,
                Status = SettleStatus.SettlePassed,
                ConsignRange = newVM.SettleRangeDate.Value.ToString("yyyy-MM")
            };
            frcade.QueryConsignSettlements(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.TotalCount>0)
                {
                    var consignList = args.Result.Rows;
                   newVM.SettleSysNo= consignList[0]["SysNo"].ToString();             
                }
                else
                {
                    newVM.SettleSysNo = string.Empty;
                    Window.Alert("未找到可以调整的代销结算单");
                }
            });
        }        
    }

}
