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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GiftCardQuery : PageBase
    {
        private GiftCardFacade facade;
        private ECCentral.QueryFilter.IM.GiftCardFilter filter;
        private ECCentral.QueryFilter.IM.GiftCardFilter filterVM;
        private List<GiftCardVM> gridVM;
        private GiftCardVM model;

        public GiftCardQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new GiftCardFacade(this);
            filter = new ECCentral.QueryFilter.IM.GiftCardFilter();
            model = new GiftCardVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";
            comGiftCardStatus.ItemsSource =
                EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.GiftCardStatus>(EnumConverter.EnumAppendItemType.All).Where(status => status.Key != GiftCardStatus.Void && status.Key != GiftCardStatus.Used);
            
            //comGiftCardCategory.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.GiftCardType>(EnumConverter.EnumAppendItemType.All);
            SeachBuilder.DataContext = model;

            base.OnPageLoad(sender, e);
        }      

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {           
            if (ValidationManager.Validate(this.SeachBuilder))
            {
                filter = model.ConvertVM<GiftCardVM, ECCentral.QueryFilter.IM.GiftCardFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ECCentral.QueryFilter.IM.GiftCardFilter>(filter);
                DataGrid.QueryCriteria = this.filter;
                DataGrid.Bind();
            }
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
            filter = model.ConvertVM<GiftCardVM, ECCentral.QueryFilter.IM.GiftCardFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QueryGiftCardInfo(DataGrid.QueryCriteria as ECCentral.QueryFilter.IM.GiftCardFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<GiftCardVM>.ConvertToVMList<List<GiftCardVM>>(args.Result.Rows);
                DataGrid.ItemsSource = gridVM;
                DataGrid.TotalCount = args.Result.TotalCount;
                if (gridVM != null)
                {
                    //btnBatchLock.Visibility = System.Windows.Visibility.Visible;
                    //btnBatchUnLock.Visibility = System.Windows.Visibility.Visible;
                    //btnInvalid.Visibility = System.Windows.Visibility.Visible;
                    //hlEdit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_Invalid_InvalidGiftCard); 
                }
                else
                {
                    //btnBatchLock.Visibility = System.Windows.Visibility.Collapsed;
                    //btnBatchUnLock.Visibility = System.Windows.Visibility.Collapsed;
                    //btnInvalid.Visibility = System.Windows.Visibility.Collapsed;
                }
            });	
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (gridVM == null || checkBoxAll == null)
                return;
            gridVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        /// <summary>
        /// 强制失效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnInvalid_Click(object sender, RoutedEventArgs e)
        //{
        //    List<GiftCardInfo> items= new List<GiftCardInfo>();
        //    gridVM.ForEach(item =>
        //    {
        //        if (item.IsChecked == true)
        //            items.Add(item.ConvertVM<GiftCardVM, GiftCardInfo>());
        //    });

        //    if (items.Count > 0)
        //        facade.BatchSetGiftCardInvalid(items, (obj, args) =>
        //        {
        //            args.FaultsHandle();
        //            DataGrid.Bind();
        //        });
        //    else
        //        Window.Alert(ResGiftCardInfo.Information_MoreThanOneRecord, MessageType.Error);
        //}

        private void btnBatchActivatek_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNoList = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    sysNoList.Add(item.SysNo.Value);
            });

            if (sysNoList.Count > 0)
                facade.BatchActivateGiftCard(sysNoList, (msg) =>
                {
                    Window.Alert("提示",msg, MessageType.Information,(obj,args)=>
                    {
                        DataGrid.Bind();
                    });
                    
                });
            else
                Window.Alert(ResGiftCardInfo.Information_MoreThanOneRecord, MessageType.Error);
        }
        
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            GiftCardVM item = this.DataGrid.SelectedItem as GiftCardVM;
            UCGiftCardUsageDetail ucDetail = new UCGiftCardUsageDetail();
            ucDetail.VM = gridVM.SingleOrDefault(a => a.SysNo.Value == item.SysNo);
            ucDetail.Dialog = this.Window.ShowDialog(ResGiftCardInfo.Information_GiftCardInformation, ucDetail, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    DataGrid.Bind();
                }             
            });
        }
    }

}
