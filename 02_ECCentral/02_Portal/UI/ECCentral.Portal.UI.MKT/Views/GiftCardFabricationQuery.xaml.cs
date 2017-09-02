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
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GiftCardFabricationQuery : PageBase
    {
        private GiftCardFacade facade;
        private ECCentral.QueryFilter.IM.GiftCardFabricationFilter filter;
        private ECCentral.QueryFilter.IM.GiftCardFabricationFilter filterVM;
        private List<GiftCardFabricationVM> gridVM;
        private GiftCardFabricationVM model;

        public GiftCardFabricationQuery()
        {
            InitializeComponent();
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new GiftCardFacade(this);
            filter = new ECCentral.QueryFilter.IM.GiftCardFabricationFilter();
            model = new GiftCardFabricationVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";

            CodeNamePairHelper.GetList("IM", "GiftCardFabricationStatus",CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comStatus.ItemsSource = args.Result;
            });
            SeachBuilder.DataContext = model;
            btnNew.DataContext = model;
            base.OnPageLoad(sender, e);
        }

        /// <summary>
        /// 链接到PO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPO_Click(object sender, RoutedEventArgs e)
        {
            GiftCardFabricationVM item = (sender as HyperlinkButton).DataContext as GiftCardFabricationVM;
            if (!string.IsNullOrEmpty(item.POSysNo))
                Window.Navigate(string.Format(ConstValue.PO_PurchaseOrderMaintain, item.POSysNo), null, true);
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.DataGrid.TotalCount < 1)
            {
                Window.Alert(ResGiftCardInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.DataGrid);
            filter = model.ConvertVM<GiftCardFabricationVM, ECCentral.QueryFilter.IM.GiftCardFabricationFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportFabricationInfoExcelFile(filterVM, new ColumnSet[] { col });
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            UCGiftCardFabricationMaintain ucMaintain = new UCGiftCardFabricationMaintain();
            GiftCardFabricationVM item = (sender as HyperlinkButton).DataContext as GiftCardFabricationVM;
            ucMaintain.VM = item;

            //this.Window.ShowDialog("编辑礼品卡制作单", ucMaintain);
            ucMaintain.Dialog = Window.ShowDialog(ResGiftCardInfo.Title_EditGiftCard, ucMaintain, (obj, args) =>
            {
                filter = model.ConvertVM<GiftCardFabricationVM, ECCentral.QueryFilter.IM.GiftCardFabricationFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ECCentral.QueryFilter.IM.GiftCardFabricationFilter>(filter);
                DataGrid.QueryCriteria = this.filter;
                DataGrid.Bind();
            });
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            UCGiftCardFabricationMaintain ucMaintain = new UCGiftCardFabricationMaintain();
           // this.Window.ShowDialog("新建礼品卡制作单", ucMaintain);
            ucMaintain.Dialog = Window.ShowDialog(ResGiftCardInfo.Title_NewGiftCard, ucMaintain, (obj, args) =>
            {
                filter = model.ConvertVM<GiftCardFabricationVM, ECCentral.QueryFilter.IM.GiftCardFabricationFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ECCentral.QueryFilter.IM.GiftCardFabricationFilter>(filter);
                DataGrid.QueryCriteria = this.filter;
                DataGrid.Bind();
            });
        }


        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.SeachBuilder))
            {
                filter = model.ConvertVM<GiftCardFabricationVM, ECCentral.QueryFilter.IM.GiftCardFabricationFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ECCentral.QueryFilter.IM.GiftCardFabricationFilter>(filter);
                DataGrid.QueryCriteria = this.filter;
                DataGrid.Bind();
            }
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryGiftCardFabricationMaster(DataGrid.QueryCriteria as ECCentral.QueryFilter.IM.GiftCardFabricationFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<GiftCardFabricationVM>.ConvertToVMList<List<GiftCardFabricationVM>>(args.Result.Rows);

                foreach (GiftCardFabricationVM gvm in gridVM)
                {
                    gvm.ShowStatus = gvm.Status;
                }

                DataGrid.ItemsSource = gridVM;
                DataGrid.TotalCount = args.Result.TotalCount;
            });	
        }

    }

}
