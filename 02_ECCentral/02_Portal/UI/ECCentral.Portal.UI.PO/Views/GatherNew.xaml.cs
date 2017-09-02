using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Models.Settlement;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.QueryFilter.PO;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class GatherNew : PageBase
    {
        public GatherSettlementInfoVM gatherInfoVM;
        public GatherSettlementFacade serviceFacade;
        public GatherSettleItemsQueryFilter filter;
        public GatherNew()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            gatherInfoVM = new GatherSettlementInfoVM();
            filter = new GatherSettleItemsQueryFilter();
            serviceFacade = new GatherSettlementFacade(this);
            InitComboBoxData();
            this.DataContext = gatherInfoVM;
            SetAccessControl();
            //供应商附加选择事件
            ucVendor.VendorSelected += new EventHandler<VendorSelectedEventArgs>(ucVendor_VendorSelected);
        }

        private void SetAccessControl()
        {
            //查询代收结算单相关:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Query))
            {
                this.btnSearch.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Gather_Query_New))
            {
                btnCreate.IsEnabled = false;
            }
        }

        private void InitComboBoxData()
        {
            CodeNamePairHelper.GetList("PO", "GatherSettleStockList", (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.cmbStock.ItemsSource = args.Result;
                this.cmbStock.SelectedIndex = 0;
            });
        }

        #region [Events]

        public void ucVendor_VendorSelected(object sendor, EventArgs e)
        {
            var selectVendor = e as VendorSelectedEventArgs;
            if (selectVendor.SelectedVendorInfo.SysNo != null)
            {
                txtPaySettleCompany.Text = EnumConverter.GetDescription(selectVendor.SelectedVendorInfo.VendorBasicInfo.PaySettleCompany);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //查询商品操作 :
            if (this.gatherInfoVM.VendorInfo == null || !this.gatherInfoVM.VendorInfo.SysNo.HasValue || string.IsNullOrEmpty(this.gatherInfoVM.VendorInfo.VendorBasicInfo.VendorNameLocal))
            {
                Window.Alert(ResGatherNew.AlertMsg_VendorSelect);
                return;
            }

            filter = gatherInfoVM.ConvertVM<GatherSettlementInfoVM, GatherSettleItemsQueryFilter>();
            filter.VendorSysNo = gatherInfoVM.VendorInfo.SysNo;

            SettledProductsGrid.Bind();
        }

        private void SettledProductsGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            serviceFacade.QueryGatherSettlementItemList(filter, (obj, args) =>
           {
               if (args.FaultsHandle())
               {
                   return;
               }
               this.SettledProductsGrid.TotalCount = args.Result.TotalCount;

               this.gatherInfoVM.GatherSettlementItemInfoList = EntityConverter<List<GatherSettlementItemInfo>, List<GatherSettlementItemInfoVM>>.Convert(args.Result.ResultList);
               this.SettledProductsGrid.ItemsSource = this.gatherInfoVM.GatherSettlementItemInfoList;
           });
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            //创建代收结算单:
            if (!gatherInfoVM.VendorInfo.SysNo.HasValue || string.IsNullOrEmpty(gatherInfoVM.VendorInfo.VendorBasicInfo.VendorNameLocal))
            {
                Window.Alert(ResGatherNew.AlertMsg_VendorSelect);
                return;
            }

            if (gatherInfoVM.GatherSettlementItemInfoList == null || gatherInfoVM.GatherSettlementItemInfoList.Count <= 0)
            {
                Window.Alert(ResGatherNew.AlertMsg_SearchEmpty);
                return;
            }

            GatherSettlementInfo info = EntityConverter<GatherSettlementInfoVM, GatherSettlementInfo>.Convert(gatherInfoVM, (s, t) =>
            {
                t.SourceStockInfo = new BizEntity.Inventory.StockInfo()
                {
                    SysNo = s.StockSysNo
                };
            });
            serviceFacade.CreateGatherSettlementInfo(info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                //创建成功，转到编辑页面:
                Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/GatherMaintain/{0}", args.Result.SysNo.Value), true);
            });

        }

        private void hlbtnSysNo_Click(object sender, RoutedEventArgs e)
        {
            var item = ((HyperlinkButton)sender).Tag as GatherSettlementItemInfoVM;
            if (item != null)
            {
                switch (item.SettleType)
                {
                    case GatherSettleType.SO:
                        Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, item.InvoiceNumber), null, true);
                        break;
                    case GatherSettleType.RMA:
                        Window.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, item.InvoiceNumber), null, true);
                        break;
                    //case GatherSettleType.RO_Adjust:
                    //    Window.Navigate(string.Format(ConstValue.Customer_RefundAdjustUrl, item.InvoiceNumber), null, true);
                    //    break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
