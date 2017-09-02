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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.Basic.Components.UserControls.CategoryPicker;
using System.Text.RegularExpressions;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCAddSEOItem : UserControl
    {
        public int SysNo { get; set; }

        public IDialog Dialog { get; set; }
        private bool isAdd = true;
        private SEOMetadataVM vm;
        private SEOFacade facade;

        public UCAddSEOItem()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddSEOItem_Loaded);
            ucPageType.PageTypeSelectionChanged += new EventHandler<PageTypeSelectionChangedEventArgs>(ucPageType_PageTypeSelectionChanged);
        }

        void ucPageType_PageTypeSelectionChanged(object sender, PageTypeSelectionChangedEventArgs e)
        {
            if (ucPageType.PageType == 25)
            {
                spProductRange.Visibility = Visibility.Visible;
            }
            else
            {
                spProductRange.Visibility = Visibility.Collapsed;
            }
        }

        private void UCAddSEOItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddSEOItem_Loaded);
            facade = new SEOFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                isAdd = false;

                facade.LoadSEOInfo(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    vm = args.Result.Convert<SEOItem, SEOMetadataVM>();
                    vm.IsExtendValid = false;
                    vm.ChannelID = "1";
                    this.ucPageType.IsEnabled = false;
                   // this.validStatus.IsEnabled = vm.Status == ADStatus.Active;
                    
                    //商品范围控件所需数据源
                    ObservableCollection<ProductVM> listProduct = new ObservableCollection<ProductVM>();
                    List<CategoryVM> listCategory = new List<CategoryVM>();
                    foreach (var item in args.Result.ProductList)
                    {
                        
                        listProduct.Add(new ProductVM() {ProductID=item.ProductId,SysNo=item.SysNo });
                    }
                    foreach (var item in args.Result.CategoryList)
                    {
                        listCategory.Add(new CategoryVM() { CategoryDisplayName=item.CategoryName,SysNo=item.SysNo});
                    }
                    this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
                    this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);
                    seoProductDetail.listCategory = listCategory;
                    seoProductDetail.listProduct = listProduct;
                    seoProductDetail.Bind();
                    LayoutRoot.DataContext = vm;
                });
            }
            else
            {
                vm = new SEOMetadataVM();
                vm.ChannelID = "1" ;
                vm.Status = ADStatus.Deactive;
                vm.IsExtendValid = false;
                vm.PageID = 0;
                seoProductDetail.Bind();
                LayoutRoot.DataContext = vm;
            }
           
        }

        public void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageID(vm.PageID);
        }

        public void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageType(vm.PageType);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            //vm = LayoutRoot.DataContext as SEOMetadataVM;
            vm.PageType = ucPageType.PageType;
            if (vm.PageType == 0)
            {
                vm.PageID = ucPageType.PageID ?? 0;
            }
            else
            {
                vm.PageID = ucPageType.PageID??-1;
            }
           
            vm.IsExtendValid = ucPageType.IsExtendValid;
            //if (vm.PageType > 0 && (vm.PageID == null||vm.PageID==-1)&&vm.PageType!=25)
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Alert("没有选择页面!",MessageType.Error);
            //    return;
            //}
            if (vm.PageType.HasValue)
            {
                SEOItem item = EntityConvertorExtensions.ConvertVM<SEOMetadataVM, SEOItem>(vm, (v, t) =>
                {
                    t.PageDescription = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.PageDescription);
                    t.PageKeywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.PageKeywords);
                });
                if (!CheckCategoryInput(seoProductDetail.CategoryResult))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("类别输入格式不正确!\r格式为:[正整数]字符,ps:[123]abc!");
                    return;
                }
                item.ProductList = new List<SeoProductItem>();
                item.CategoryList = GetSeoCategoryList(seoProductDetail.CategoryResult);
                seoProductDetail.ProductResult.Split('\r').ToList().ForEach(s => { if (!string.IsNullOrEmpty(s)) { item.ProductList.Add(new SeoProductItem() { ProductId = s }); } });
                item.CompanyCode = CPApplication.Current.CompanyCode;
                item.User = new BizEntity.Common.UserInfo() { UserName = CPApplication.Current.LoginUser.LoginName, SysNo = CPApplication.Current.LoginUser.UserSysNo };
                if (isAdd)
                {
                    facade.AddSEOInfo(item, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        CloseDialog(DialogResultType.OK);
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    });
                }
                else
                {
                    item.SysNo = SysNo;
                    facade.UpdateSEOInfo(item, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        CloseDialog(DialogResultType.OK);
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    });
                }
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_NeedPageType, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }
        /// <summary>
        /// check Category
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        private bool CheckCategoryInput(string inputValue)
        {

            if (string.IsNullOrEmpty(inputValue))
            {
                return true; // 可以为空直接返回
            }
            string match_pattern = @"^((\s|\r)*?(?<text>\[(?<value>\d+?)\][^\s\\]+)*?(\s|\r)*?)+$";
            Match m = Regex.Match(inputValue, match_pattern);
            if (m.Success)//匹配成功
            {
                
                return true;
             }
            else
            {
                return false;
            }
            
        }

        private List<SeoCategory> GetSeoCategoryList(string inputValue)
        {
            List<SeoCategory> list = new List<SeoCategory>();
            inputValue.Split('\r').ToList().ForEach(s =>
            {
                if (!string.IsNullOrEmpty(s))
                {
                    string match_pattern = @"^\[(\d*)\]";
                    Match m = Regex.Match(s, match_pattern);
                    list.Add(new SeoCategory() { SysNo = Convert.ToInt32(m.Groups[1].Value), CategoryName = s });
                }
            });
            return list;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }
}
