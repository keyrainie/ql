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
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AdvertisersMaintain : PageBase
    {
        private AdvertiserQueryFilter filter;
        private AdvertiserQueryFilter filterVM;
        private AdvertiserFacade facade;
        private AdvertisersQueryVM model;
        private List<AdvertisersVM> gridVM;

        public AdvertisersMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            model = new AdvertisersQueryVM();
            model.ChannelID = "1";
            QuerySection.DataContext = model;
            btnStackPanel.DataContext = model;
            filter = new AdvertiserQueryFilter();
            filter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            facade = new AdvertiserFacade(this);
            cbShowComment.SelectedIndex = 0;
            Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn status = QueryResultGrid.Columns[4] as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
            status.Binding.ConverterParameter = typeof(ADStatus);

            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
           
            facade.QueryAdvertiser(QueryResultGrid.QueryCriteria as AdvertiserQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<AdvertisersVM>.ConvertToVMList<List<AdvertisersVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                //QueryResultGrid.ItemsSource = args.Result.Rows;
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                btnNewItem.Visibility = System.Windows.Visibility.Visible;
                
                if (gridVM != null)
                {
                    btnBathchSetValid.Visibility =  Visibility.Visible;
                    btnBathchSetInvalid.Visibility = Visibility.Visible;
                }
                else
                {
                    btnBathchSetValid.Visibility = Visibility.Collapsed;
                    btnBathchSetInvalid.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void btnNewItem_Click(object sender, RoutedEventArgs e)
        {
            UCAddAdvertisers usercontrol = new UCAddAdvertisers();
            usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_CreateAdvertiser, usercontrol, (obj, args) =>
            {
                filter = model.ConvertVM<AdvertisersQueryVM, AdvertiserQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvertiserQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            });
        }

        /// <summary>
        /// 编辑该行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = this.QueryResultGrid.SelectedItem as dynamic;
            if (item != null)
            {
                //Window.Navigate(string.Format("/ECCentral.Portal.UI.MKT/UCAddAdvertisers/{0}", adv.SysNo), null, true);
                UCAddAdvertisers usercontrol = new UCAddAdvertisers();
                usercontrol.SysNo = item.SysNo;
                usercontrol.Dialog = Window.ShowDialog(ResNewsInfo.Title_EditAdvertiser, usercontrol, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
            }
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (model.HasValidationErrors) return;

            filter = model.ConvertVM<AdvertisersQueryVM, AdvertiserQueryFilter>();
            filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvertiserQueryFilter>(filter);
            QueryResultGrid.QueryCriteria = this.filter;
            QueryResultGrid.Bind();
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBathchSetValid_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if (invalidSysNo.Count > 0)
                facade.SetAdvertiserValid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBathchSetInvalid_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            gridVM.ForEach(item =>
            {
                if (item.IsChecked == true)
                    invalidSysNo.Add(item.SysNo.Value);
            });
            if(invalidSysNo.Count>0)
                facade.SetAdvertiserInvalid(invalidSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    QueryResultGrid.Bind();
                });
            else
                Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
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
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResNewsInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<AdvertisersQueryVM, AdvertiserQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            //col.Insert(0, "ProductId", ResRMAReports.Excel_ProductID, 20) .SetWidth("ProductName", 30);
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

    }
    
}
