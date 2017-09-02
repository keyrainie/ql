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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.UserControls.Keywords;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Controls.Uploader;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    /// <summary>
    /// 产品页面关键字
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductKeywords : PageBase
    {
        private ProductKeywordsQueryFacade facade;
        private ProductKeywordsQueryFilter filter;
        private ProductKeywordsQueryFilter filterVM;
        private List<ProductKeywordsQueryVM> gridVM;
        private ProductKeywordsQueryVM model;

        public ProductKeywords()
        {
            InitializeComponent();
            myCategory.cmbCategory3SelectionChanged += (sender, e) => 
            {
                SelectCategoryProperty.Category3SysNo = myCategory.Category3SysNo;
                SelectCategoryProperty.BindData();
                
                
            };
            this.uploadImoportFile.AllFileUploadCompleted += new Basic.Controls.Uploader.AllUploadCompletedEvent(uploadImoportFile_AllFileUploadCompleted);
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
                Window.Alert(ResKeywords.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<ProductKeywordsQueryVM, ProductKeywordsQueryFilter>();
            filter.PropertySysNo = SelectCategoryProperty.CategoryPropertySysNo;
            filter.PropertyValueSysNo = SelectCategoryProperty.PropertyValueSysNo;
            filter.InputValue = SelectCategoryProperty.InputValue;
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ProductKeywordsQueryFacade(this);
            filter = new ProductKeywordsQueryFilter();
            model = new ProductKeywordsQueryVM();
            model.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            comProductStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.IM.ProductStatus>(EnumConverter.EnumAppendItemType.All);
            this.DataContext = model;

            facade.GetProductKeywordsEditUserList(Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                List<UserInfo> userList = args.Result;
                userList.Insert(0, new UserInfo { SysNo = null, UserName = ResCommonEnum.Enum_All });
                comEditUser.ItemsSource = userList;
            });	

            base.OnPageLoad(sender, e);
        }

        #region maintain

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (QueryResultGrid.QueryCriteria == null)
            {
                filter = model.ConvertVM<ProductKeywordsQueryVM, ProductKeywordsQueryFilter>();
                filter.PropertySysNo = SelectCategoryProperty.CategoryPropertySysNo;
                filter.PropertyValueSysNo = SelectCategoryProperty.PropertyValueSysNo;
                filter.InputValue = SelectCategoryProperty.InputValue;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
            }
            if ((QueryResultGrid.QueryCriteria as ProductKeywordsQueryFilter).EditUser == ResCommonEnum.Enum_All)
            {
                (QueryResultGrid.QueryCriteria as ProductKeywordsQueryFilter).EditUser = null;
            }
            facade.QueryProductPageKeywords(QueryResultGrid.QueryCriteria as ProductKeywordsQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                gridVM = DynamicConverter<ProductKeywordsQueryVM>.ConvertToVMList<List<ProductKeywordsQueryVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = gridVM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;

                if (gridVM != null)
                {
                    btnBatchAdd.Visibility = Visibility.Visible;
                    btnBatchDelete.Visibility = Visibility.Visible; 
                }
                else
                {
                    btnBatchDelete.Visibility = Visibility.Collapsed;
                    btnBatchAdd.Visibility = Visibility.Collapsed;
                }
            });	
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchDelete_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            if (gridVM != null)
            {
                gridVM.ForEach(vm =>
                {
                    if (vm.IsChecked == true)
                        list.Add(vm.ProKeySysNo);
                });
            }
            if (list.Count > 0)
            {
                UCBatchSetProductPageKeywords usercontrol = new UCBatchSetProductPageKeywords();
                usercontrol.BatchAdd = false;
                usercontrol.productList = list;
                usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_BatchDeleteProductPageKeywords, usercontrol, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
            }
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
        }

        private void btnBatchAdd_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            if (gridVM != null)
            {
                gridVM.ForEach(vm =>
                {
                    if (vm.IsChecked == true)
                        list.Add(vm.ProKeySysNo);
                });
            }
            if (list.Count > 0)
            {
                UCBatchSetProductPageKeywords usercontrol = new UCBatchSetProductPageKeywords();
                usercontrol.BatchAdd = true;
                usercontrol.productList = list;
                usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_BatchCreateProductPageKeywords, usercontrol, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
            }
            else
                Window.Alert(ResKeywords.Information_MoreThanOneRecord,MessageType.Warning);
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<ProductKeywordsQueryVM, ProductKeywordsQueryFilter>();
                filter.PropertySysNo = SelectCategoryProperty.CategoryPropertySysNo  ;
                filter.PropertyValueSysNo = SelectCategoryProperty.PropertyValueSysNo ;
                filter.InputValue = SelectCategoryProperty.InputValue;
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductKeywordsQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
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
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
             ProductKeywordsQueryVM item = this.QueryResultGrid.SelectedItem as ProductKeywordsQueryVM;
            
                UCEditProductKeywords usercontrol = new UCEditProductKeywords();
                usercontrol.VM = item;
                usercontrol.Dialog = Window.ShowDialog(ResKeywords.Title_EditProductPageKeywords, usercontrol, (obj, args) =>
                {
                    QueryResultGrid.Bind();
                });
           

        }

        //更多条件
        private void ckb_MoreQueryBuilder_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_MoreQueryBuilder.IsChecked.HasValue && ckb_MoreQueryBuilder.IsChecked.Value)
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Visible;
            else
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Collapsed;
        }  
        
        #endregion

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadImoportFile_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            uploadImoportFile.Clear();
            if (args.UploadInfo[0].UploadResult == SingleFileUploadStatus.Failed)
            {
                Window.Alert(ResKeywords.Information_UploadFailed, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }

            facade.BatchImportProductKeywords(args.UploadInfo[0].ServerFilePath, (obj, args2) =>
            {
                if (args2.FaultsHandle())
                    return;
                QueryResultGrid.Bind();
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }

    }

}
