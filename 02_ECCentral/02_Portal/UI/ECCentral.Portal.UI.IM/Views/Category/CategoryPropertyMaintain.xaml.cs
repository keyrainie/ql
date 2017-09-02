using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Windows.Data;
using ECCentral.Portal.Basic.Components.UserControls.Language;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CategoryPropertyMaintain : PageBase
    {
        public CategoryPropertyMaintain()
        {
            InitializeComponent();
         
        }

        

        #region 属性

        CategoryPropertyQueryVM model;

        #endregion

        #region 初始化加载

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new CategoryPropertyQueryVM { PropertyList = new List<KeyValuePair<int?, string>>() };
            this.DataContext = model;
            this.CmbPropertyTypeList.SelectedIndex = 0;
        }

        #endregion

        #region 查询绑定
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dgCategoryPropertyQueryResult.Bind();
        }

        private void dgCategoryPropertyQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CategoryPropertyQueryFacade facade = new CategoryPropertyQueryFacade(this);
            var _vm = (CategoryPropertyQueryVM)this.DataContext;
            facade.QueryCategoryProperty(_vm, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dgCategoryPropertyQueryResult.ItemsSource = args.Result.Rows;
                this.dgCategoryPropertyQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件
        /// <summary>
        /// 设置属性组多语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkMultiLanguageEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectItem = this.dgCategoryPropertyQueryResult.SelectedItem as dynamic;

            UCMultiLanguageMaintain item = new UCMultiLanguageMaintain(selectItem.PropertyGroupSysNo, "PIM_PropertyGroup");

            item.Dialog = Window.ShowDialog("设置属性组多语言", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    //this.dgProductPropertyInfo.Bind();
                }
            }, new Size(750, 600));
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = dgCategoryPropertyQueryResult.ItemsSource as dynamic;
            if (viewList == null) return;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value;
            }
            dgCategoryPropertyQueryResult.ItemsSource = viewList;
        }

        private void hyperlinkOperationEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic categoryProperty = this.dgCategoryPropertyQueryResult.SelectedItem as dynamic;
            int sysNo = Convert.ToInt32(categoryProperty.SysNo);
            //类别属性编辑
            CategoryPropertyMaintainDetail detail = new CategoryPropertyMaintainDetail();
            detail.SysNo = sysNo;
            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryPropertyMaintainDetail.Dialog_EditCategoryProperty, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dgCategoryPropertyQueryResult.Bind();
                }
            }, new Size(450, 350));
        }

        private void btnCatetoryPropertyDelete_Click(object sender, RoutedEventArgs e)
        {
            var viewList = (IEnumerable<dynamic>)dgCategoryPropertyQueryResult.ItemsSource;
            if (viewList == null || viewList.Count() == 0)
            {
                Window.Alert(ResCategoryPropertyMaintain.Msg_OnSelectCatetoryProperty, MessageType.Error);
                return;
            }
            var source = viewList.ToList();
            var checkList = source.Where(p => p.IsChecked == true).Select(p => p.SysNo).ToList();
            if (checkList == null || checkList.Count == 0)
            {
                Window.Alert(ResCategoryPropertyMaintain.Msg_OnSelectCatetoryProperty, MessageType.Error);
                return;
            }
            Window.Confirm(ResCategoryPropertyMaintain.Msg_OnDeleteCatetoryProperty, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var facade = new CategoryPropertyQueryFacade(this);
                    facade.DelCategoryProperty(checkList, OnDelCallback);
                    return;
                }
            });
          
        }


        private void OnDelCallback(object obj, RestClientEventArgs<dynamic> args)
        {
            if (args.FaultsHandle())
            {

                return;
            }
            Window.Alert(ResCategoryPropertyMaintain.Info_SaveSuccessfully);
            dgCategoryPropertyQueryResult.Bind();
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategoryPropertyQueryVM;
            var facade = new CategoryPropertyQueryFacade(this);
            facade.CreateCategoryProperty(vm, OnCreateCallback);
        }

        private void OnCreateCallback(object obj, RestClientEventArgs<CategoryProperty> args)
        {
            if (args.FaultsHandle())
            {

                return;
            }
            Window.Alert(ResCategoryPropertyMaintain.Info_SaveSuccessfully);
            dgCategoryPropertyQueryResult.Bind();
        }

        private void ImageAllPropertyPicker_Click(object sender, RoutedEventArgs e)
        {
            PropertyFacade facade = new PropertyFacade(this);
            var vm = DataContext as CategoryPropertyQueryVM;
            facade.GetPropertyListByPropertyName(vm.PropertyName, (obj, args) =>
            {
                if (args.FaultsHandle())
                {

                    return;
                }
                if (args.Result == null || args.Result.Count == 0)
                {

                    vm.PropertyList = new List<KeyValuePair<int?, string>>();
                }
                else
                {
                    vm.PropertyList = args.Result.ToDictionary(p => p.SysNo, k => k.PropertyName.Content).ToList();
                }

                ListBox_CategoryPropertyList.ItemsSource = vm.PropertyList;
            });
        }




        #endregion

        #endregion

        #region 跳转
        private void btnCatetoryPropertyEdit_Click(object sender, RoutedEventArgs e)
        {
            //类别属性编辑
            CategoryPropertyMaintainDetail detail = new CategoryPropertyMaintainDetail();

            detail.Dialog = Window.ShowDialog(ECCentral.Portal.UI.IM.Resources.ResCategoryPropertyMaintainDetail.Dialog_EditCategoryProperty, detail, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    //dgPropertyQueryResult.Bind();
                }
            }, new Size(450, 350));
        }
        #endregion

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CategoryPropertyQueryVM;
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (vm.CategorySysNo == vm.SourceCategorySysNo)
            {
                Window.Alert("自己不能复制自己！",MessageType.Error);
                return;
            }
                CategoryPropertyFacade facade = new CategoryPropertyFacade(this);
               
                facade.CopyCategoryOutputTemplateProperty(vm, (obj, arg) => {
                    if (arg.FaultsHandle())
                    {
                         return;
                    }
                     Window.Alert("复制成功！");
                     dgCategoryPropertyQueryResult.Bind();
                });
        }

        private void BtnBat_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.dgCategoryPropertyQueryResult.ItemsSource as dynamic;
            List<CategoryPropertyVM> list = new List<CategoryPropertyVM>();
            foreach (var item in d)
            {
                if (item.IsChecked == true)
                {
                    list.Add(new CategoryPropertyVM()
                    {
                        IsInAdvSearchBat = item.IsInAdvSearch == CategoryPropertyStatus.Yes ? true : false,

                        IsItemSearchBat = item.IsItemSearch == CategoryPropertyStatus.Yes ? true : false,
                        IsMustInputBat = item.IsMustInput == CategoryPropertyStatus.Yes ? true : false,
                        DisplayStyle = item.WebDisplayStyle,
                        Priority = item.Priority == null ? 0 : item.Priority,
                        PropertyGroup = new PropertyGroupInfoVM(){SysNo=item.PropertyGroupSysNo,PropertyGroupName=item.GroupDescription},
                        Property = new PropertyVM() { PropertyName = item.PropertyName, SysNo = item.PropertySysNo },
                        CategoryInfo=new CategoryVM(){SysNo=item.CategoryInfoSysNo},
                        SearchPriority = item.SearchPriority == null ? 0 : item.SearchPriority,
                        PropertyType = item.PropertyType,
                        LastDate = item.LastEditTime,
                        UserName = item.LastEditUser,
                        SysNo = item.SysNo
                    });
                }
            }
            if (list.Count > 0)
            {
                CategoryPropertyBatEdit edit = new CategoryPropertyBatEdit();
                edit.DataSource = list;
                edit.Dialog = Window.ShowDialog("批量更新", edit, (s, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        dgCategoryPropertyQueryResult.Bind();
                    }
                }, new Size(1100, 600));
            }
            else
            {
                Window.Alert("没有选择批量更新的记录!",MessageType.Error);
            }
        }



    }
 

}
