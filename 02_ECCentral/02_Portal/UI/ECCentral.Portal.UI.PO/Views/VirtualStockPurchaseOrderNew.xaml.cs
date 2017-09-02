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
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VirtualStockPurchaseOrderNew : PageBase
    {

        public int SOSysNo;
        public int ProductSysNo;
        public int SOItemSysNo;

        public VirtualPurchaseOrderFacade serviceFacade;
        public VirtualStockPurchaseOrderInfoVM viewVM;

        public VirtualStockPurchaseOrderNew()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            viewVM = new VirtualStockPurchaseOrderInfoVM();
            serviceFacade = new VirtualPurchaseOrderFacade(this);
            string getParams = this.Request.Param;
            if (!string.IsNullOrEmpty(getParams))
            {
                int.TryParse(getParams.Split(',')[0].Trim(), out SOSysNo);
                int.TryParse(getParams.Split(',')[1].Trim(), out ProductSysNo);
                //加载信息:
                LoadInfo();
            }
            else
            {
                Window.Alert("销售单号错误! ");
                return;
            }
            SetAccessControl();
        }

        private void SetAccessControl()
        {

            //生成虚库采购单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_VirtualPO_Create))
            {
                this.btnGanerate.IsEnabled = false;
            }
        }

        private void LoadInfo()
        {
            //通过soSysNo和productSysNo加载相关的信息:
            serviceFacade.LoadVirtualPurchaseInfoBySOItemSysNo(string.Format("{0}-{1}", SOSysNo, ProductSysNo), (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result == null)
                {
                    this.btnGanerate.IsEnabled = false;
                    this.lblAlertText.Text = "销售单号错误 !";
                    return;
                }
                viewVM = EntityConverter<VirtualStockPurchaseOrderInfo, VirtualStockPurchaseOrderInfoVM>.Convert(args.Result);
                this.DataContext = viewVM;
                if (viewVM.SOVirtualCount.HasValue && viewVM.SOVirtualCount.Value > 0)
                {
                    this.btnGanerate.IsEnabled = false;
                    this.lblAlertText.Text = "订单已经生成了虚库采购单，不能重复生成 !";
                    return;
                }

            });

        }

        #region [Events]
        private void btnGanerate_Click(object sender, RoutedEventArgs e)
        {
            //生成操作 ：
            string queryString = string.Format("{0}-{1}-{2}", SOSysNo, ProductSysNo, viewVM.PurchaseQty);
            serviceFacade.IsVSPOItemPriceLimited(queryString, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                #region
                //TODO:创建虚库采购单的权限:
                //if (QueryProviderFactory.GetQueryProvider<IQueryVSPO>().IsLimitedSVR(entity.SOItemSysNo, entity.PurchaseQty.Value))
                //{
                //    if (!HasRight("CanAddLimited"))
                //    {
                //        return Json(new
                //        {
                //            Right = Resources.GlobalResources.Warning_VSPO_NotToAdd
                //        });
                //    }
                //}
                //else
                //{
                //    if (!HasRight("CanAdd"))
                //    {
                //        return Json(new
                //        {
                //            Right = Resources.GlobalResources.Warning_VSPO_NotToAdd
                //        });
                //    }
                //}
                #endregion

                VirtualStockPurchaseOrderInfo info = EntityConverter<VirtualStockPurchaseOrderInfoVM, VirtualStockPurchaseOrderInfo>.Convert(viewVM, (s, t) =>
                {
                    t.PurchaseQty = 1;
                    t.Status = VirtualPurchaseOrderStatus.Normal;
                    t.CreateTime = DateTime.Now;
                    t.InStockOrderType = VirtualPurchaseInStockOrderType.PO;
                });

                serviceFacade.CreateVSPO(info, (obj2, args2) =>
                {

                    if (args2.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert("提示", "成功生成虚库商品采购单，且发邮件通知PM负责人员!", MessageType.Information, (obj3, args3) =>
                    {
                        if (args3.DialogResult == DialogResultType.Cancel)
                        {
                            this.btnGanerate.IsEnabled = false;
                        }
                    });
                });
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //关闭操作:
            Window.Close(true);
        }
        #endregion

    }

}
