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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Common.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ShipType : PageBase
    {
       // private ShipTypeQueryFilter filter;
        private ShipTypeFacade facade;
        private ShipTypeQueryVM queryVM;

        public ShipType()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            // filter = new ShipTypeQueryFilter();
            facade = new ShipTypeFacade(this);
            queryVM = new ShipTypeQueryVM();
            queryVM.ChannelID = "0";
            SeachBuilder.DataContext = queryVM;
            queryVM.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            comIsWithPackFee.ItemsSource = EnumConverter.GetKeyValuePairs<SYNStatus>(EnumConverter.EnumAppendItemType.All);//收取包裹费
            comIsOnlineShow.ItemsSource = EnumConverter.GetKeyValuePairs<HYNStatus>(EnumConverter.EnumAppendItemType.All);//前台显示

            //facade.LoadWarehouse(queryVM.CompanyCode, (obj, args) =>
            //{
            //    if (args.FaultsHandle())
            //        return;
            //    cbOnlyForStockSysNo.ItemsSource = args.Result;
            //});
            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {

            queryVM.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo() 
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.QueryShipTypeList(queryVM, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                QueryResultGrid.ItemsSource = args.Result.Rows.ToList();
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                //btnNewItem.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Create_CreateAdvertisers);

                if (args.Result.Rows != null)
                    btnNewItem.IsEnabled =true;// AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Valid_ValidAdvertisers);
                else
                    btnNewItem.IsEnabled = false;
            });
        }

        /// <summary>
        /// 编辑该行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipType_Edit))
            {
                Window.Alert(ResShipType.Msg_HasNoRight);
                return;
            }
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;

             Dialog(item.SysNo,queryVM.ChannelID);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipType_Add))
            {
                Window.Alert(ResShipType.Msg_HasNoRight);
                return;
            }
            Dialog(null,string.Empty);
        }
        private void Dialog(int? sysNo,string channelId)
        {
            UCAddShipType usercontrol = new UCAddShipType(sysNo,channelId) { Page = this };
            usercontrol.Dialog = Window.ShowDialog(sysNo.HasValue? ResShipType.Title_EditShipType:ResShipType.Title_NewShipType, usercontrol, (obj, args) =>
            {
                if (args != null)
                {
                    QueryResultGrid.Bind();
                }
            });
        }

    }
}
