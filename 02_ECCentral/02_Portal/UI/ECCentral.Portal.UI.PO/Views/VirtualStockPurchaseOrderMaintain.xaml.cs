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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VirtualStockPurchaseOrderMaintain : PageBase
    {
        public VirtualPurchaseOrderFacade serviceFacade;
        public VirtualStockPurchaseOrderInfoVM infoVM;
        public string VSPOSysNo;

        public VirtualStockPurchaseOrderMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            infoVM = new VirtualStockPurchaseOrderInfoVM();
            serviceFacade = new VirtualPurchaseOrderFacade(this);
            LoadComboBoxData();
            VSPOSysNo = this.Request.Param;
            if (!string.IsNullOrEmpty(VSPOSysNo))
            {
                //加载虚库采购单信息:
                LoaVSPOInfo();
            }
        }

        private void SetAccessControl()
        {
            //权限控制:
            //更新虚库采购单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VirtualPO_Update))
            {
                this.btnUpdate.IsEnabled = false;
            }
            //作废虚库采购单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VirtualPO_Abandon))
            {
                this.btnAbandon.IsEnabled = false;
            }
            //更新CS备注:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VirtualPO_UpdateCSMemo))
            {
                this.btnCSMemoUpdate.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            //单据类型:
            this.cmbInStockOrderType.ItemsSource = EnumConverter.GetKeyValuePairs<VirtualPurchaseInStockOrderType>(EnumConverter.EnumAppendItemType.None);
        }

        private void LoaVSPOInfo()
        {
            serviceFacade.LoadVirtualPurchaseOrderInfo(VSPOSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                infoVM = EntityConverter<VirtualStockPurchaseOrderInfo, VirtualStockPurchaseOrderInfoVM>.Convert(args.Result);
                this.DataContext = infoVM;
                infoVM.ValidationErrors.Clear();

                if (args.Result.EstimateArriveTime.HasValue)
                {
                    this.dpkEstimateArriveDate.Text = args.Result.EstimateArriveTime.Value.ToShortDateString();
                    this.tpEstimateArriveTime.Value = args.Result.EstimateArriveTime.Value;
                }
                ShowActionButtons(args.Result.Status);
                SetAccessControl();
            });
        }

        private void ShowActionButtons(BizEntity.PO.VirtualPurchaseOrderStatus? status)
        {
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case VirtualPurchaseOrderStatus.Normal:
                        break;
                    case VirtualPurchaseOrderStatus.Close:
                        break;
                    case VirtualPurchaseOrderStatus.Abandon:
                        this.btnUpdate.IsEnabled = false;
                        this.btnAbandon.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private VirtualStockPurchaseOrderInfo BuildEntity()
        {
            return EntityConverter<VirtualStockPurchaseOrderInfoVM, VirtualStockPurchaseOrderInfo>.Convert(infoVM, (s, t) =>
            {
                if (this.dpkEstimateArriveDate.SelectedDate.HasValue && this.tpEstimateArriveTime.Value.HasValue)
                {
                    t.EstimateArriveTime = new DateTime(dpkEstimateArriveDate.SelectedDate.Value.Year, dpkEstimateArriveDate.SelectedDate.Value.Month, dpkEstimateArriveDate.SelectedDate.Value.Day, tpEstimateArriveTime.Value.Value.Hour, tpEstimateArriveTime.Value.Value.Minute, tpEstimateArriveTime.Value.Value.Second);

                }
            });
        }

        private void AlertAndRefreshPage(string alertString)
        {
            Window.Alert(ResVirtualStockPurchaseOrderMaintain.AlertMsg_Title, alertString, MessageType.Information, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.Cancel)
                {
                    Window.Refresh();
                }
            });
        }

        #region    [Events]
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            //更新操作:
            serviceFacade.UpdateVirtualPurchaseOrderInfo(BuildEntity(), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                AlertAndRefreshPage(ResVirtualStockPurchaseOrderMaintain.AlertMsg_OperationSuc);
            });
        }

        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            //作废操作:
            Window.Confirm(ResVirtualStockPurchaseOrderMaintain.AlertMsg_Title, ResVirtualStockPurchaseOrderMaintain.AlertMsg_Confirm, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    serviceFacade.AbandonVirtualPurchaseOrder(BuildEntity(), (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                        {
                            return;
                        }
                        AlertAndRefreshPage(ResVirtualStockPurchaseOrderMaintain.AlertMsg_AbandonSuc);
                    });
                }
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //关闭操作:
            Window.Close(true);
        }

        private void btnCSMemoUpdate_Click(object sender, RoutedEventArgs e)
        {
            //更新CS 备注操作:
            serviceFacade.UpdateVirtualPurchaseOrderInfoCSMemo(BuildEntity(), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                AlertAndRefreshPage(ResVirtualStockPurchaseOrderMaintain.AlertMsg_OperationSuc);
            });
        }
        #endregion
    }

}
