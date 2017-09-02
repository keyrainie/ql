using System;
using System.Collections.Generic;
using System.Windows;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryKPIMaintain : PageBase
    {

        CategroyKPIQueryVM model;
        private List<CategroyKPIVM> _vmList;
        private CategoryType Type {  set;  get; }
        public CategoryKPIMaintain()
        {
            InitializeComponent();
            this.myCategoryType.SelectionChanged += (sender, e) => 
            {
                if ((CategoryType)this.myCategoryType.SelectedValue == CategoryType.CategoryType3)
                {
                    myUCCategoryPicker.Category3Visibility = Visibility.Visible;
                }
                else
                {
                    myUCCategoryPicker.Category3Visibility = Visibility.Collapsed;
                }
            };
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategroyKPIQueryVM();
            this.DataContext = model;
            
            cbStatus.SelectedIndex = 0;
            
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            //必须选择3级类别
            //if (model.C3SysNo < 0)
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Alert(ResCategoryKPIMaintain.NoSelectedC3);
            //    return;
            //}
              Type = (CategoryType)this.myCategoryType.SelectedValue;

              if (Type== CategoryType.CategoryType3)
             {
               
                dgCategoryKPIMaintainQueryResult.Columns[1].Visibility = Visibility.Visible;
                dgCategoryKPIMaintainQueryResult.Columns[5].Visibility = Visibility.Visible;
                dgCategoryKPIMaintainQueryResult.Columns[10].Visibility = Visibility.Visible;
                dgCategoryKPIMaintainQueryResult.Columns[11].Visibility = Visibility.Visible;
                dgCategoryKPIMaintainQueryResult.Columns[12].Visibility = Visibility.Visible;
                dgCategoryKPIMaintainQueryResult.Columns[2].Visibility = Visibility.Collapsed;
            }
            else
            {
                dgCategoryKPIMaintainQueryResult.Columns[2].Visibility = Visibility.Visible;
                dgCategoryKPIMaintainQueryResult.Columns[1].Visibility = Visibility.Collapsed;
                dgCategoryKPIMaintainQueryResult.Columns[5].Visibility = Visibility.Collapsed;
                dgCategoryKPIMaintainQueryResult.Columns[10].Visibility = Visibility.Collapsed;
                dgCategoryKPIMaintainQueryResult.Columns[11].Visibility = Visibility.Collapsed;
                dgCategoryKPIMaintainQueryResult.Columns[12].Visibility = Visibility.Collapsed;
            }

            dgCategoryKPIMaintainQueryResult.Bind();
        }

        private void dgCategoryKPIMaintainQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CategoryKPIQueryFacade facade = new CategoryKPIQueryFacade(this);

            facade.QueryCategoryKPIList(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                _vmList = DynamicConverter<CategroyKPIVM>.ConvertToVMList<List<CategroyKPIVM>>(args.Result.Rows);
                this.dgCategoryKPIMaintainQueryResult.ItemsSource = _vmList;
                this.dgCategoryKPIMaintainQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void hyperlinkOperationEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic manufacturer = this.dgCategoryKPIMaintainQueryResult.SelectedItem as dynamic;
            if (manufacturer == null) return;
            
            var sysNo = Convert.ToInt32(manufacturer.SysNo);
            var propertyMainUC = new CategoryKPIDetail {
                SysNo = sysNo,
                type=Type,
                Category3List=myUCCategoryPicker.Category3List,
                Category2List=myUCCategoryPicker.Category2List
            };
            propertyMainUC.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryKPIDetail.Dialog_CategoryKPIUpdate, propertyMainUC, (s, args) =>
            {

            }, new Size(900, 700));
        }


    }
}
