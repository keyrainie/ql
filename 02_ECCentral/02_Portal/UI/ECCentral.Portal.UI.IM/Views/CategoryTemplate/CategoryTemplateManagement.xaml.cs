using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Service.IM.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url,NeedAccess=false)]
    public partial class CategoryTemplateManagement : PageBase
    {
         private CategoryTemplateDataVM VM { get; set; }
         private CategoryTemplateQueryVM SearchVM { get; set; }
         private CategoryTemplateFacade facade;
         private int? C3SysNo;
        public CategoryTemplateManagement()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            SearchVM= new CategoryTemplateQueryVM();
            this.sPanelSearch.DataContext = SearchVM;
            VM = new CategoryTemplateDataVM();
            VM.CategoryTemplateProductTitle = new CategoryTemplateVM() { TemplateType = CategoryTemplateType.TemplateProductTitle };
            VM.CategoryTemplateProductName = new CategoryTemplateVM() { TemplateType = CategoryTemplateType.TemplateProductName };
            VM.CategoryTemplateProductDescription = new CategoryTemplateVM() { TemplateType = CategoryTemplateType.TemplateProductDescription };
            VM.CategoryTemplateWeb = new CategoryTemplateVM() { TemplateType = CategoryTemplateType.TemplateWeb };
            this.DataContext = VM;
            facade = new CategoryTemplateFacade();
           
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            if (SearchVM.Category3SysNo == null || SearchVM.Category3SysNo==0)
            {
                Window.MessageBox.Show("三级类不可以为空!", MessageBoxType.Error);
                return;
            }
            C3SysNo = SearchVM.Category3SysNo;
            facade.GetCategoryTemplateDataByC3SysNo(SearchVM.Category3SysNo, (obj, arg) => {
                if (arg.FaultsHandle())
                {
                    return;
                }
              BingVM(arg.Result);
            });
        }
         /// <summary>
         /// Bing VM
         /// </summary>
         /// <param name="result"></param>
        private void BingVM(CategoryTemplateRsp result)
        {
            VM.LastEditDate = result.LastEditDate;
            VM.LastEidtUserName = result.LastEidtUserName;
            List<CategoryTemplatePropertyVM> categoryTemplatePropertyList = new List<CategoryTemplatePropertyVM>();
            result.CategoryTemplatePropertyInfoList.ForEach(s =>
            {
                categoryTemplatePropertyList.Add(new CategoryTemplatePropertyVM() { PropertyDescription = s.PropertyDescription.Content, SysNo = s.SysNo });
            });
            if (categoryTemplatePropertyList.Count == 0)
            {
                Window.MessageBox.Show("该三级类没有属性！",MessageBoxType.Error);
                return;
            }
            VM.CategoryPropertyList = categoryTemplatePropertyList;
            ConvertVM(result.CategoryTemplateList);
            
        }
         /// <summary>
         /// list ConvertVM
         /// </summary>
         /// <param name="list"></param>
        private void ConvertVM(List<CategoryTemplateInfo> list)
        {
            if (list != null && list.Count > 0)
            {
                foreach (CategoryTemplateInfo item in list)
                {

                    if (!string.IsNullOrEmpty(item.Templates))
                    {
                        string[] arr = item.Templates.Split(',');
                        switch (item.TemplateType)
                        {
                            case CategoryTemplateType.TemplateProductTitle:
                                VM.CategoryTemplateProductTitle.CategoryTemplatePropertyDisplay = GetCategoryTemplateString(arr, item.TemplateType);
                                break;
                            case CategoryTemplateType.TemplateProductDescription:
                                VM.CategoryTemplateProductDescription.CategoryTemplatePropertyDisplay = GetCategoryTemplateString(arr, item.TemplateType);
                                break;
                            case CategoryTemplateType.TemplateProductName:
                                VM.CategoryTemplateProductName.CategoryTemplatePropertyDisplay = GetCategoryTemplateString(arr, item.TemplateType);
                                break;
                            case CategoryTemplateType.TemplateWeb:
                                VM.CategoryTemplateWeb.CategoryTemplatePropertyDisplay = GetCategoryTemplateString(arr, item.TemplateType);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
         /// <summary>
         /// LIST convertTo String
         /// </summary>
         /// <param name="arr"></param>
         /// <returns></returns>
        private string GetCategoryTemplateString(string[] arr,CategoryTemplateType type)
        {
            string result = string.Empty;
            var data = (from p in VM.CategoryPropertyList from s in arr where p.SysNo.ToString() == s select p).ToList();
            List<int?> tempdata = new List<int?>();
            foreach (var item in data)
            {
                if (!tempdata.Contains(item.SysNo))
                {
                    result = result + " <" + item.PropertyDescription + ">";
                    tempdata.Add(item.SysNo);
                }
                
            }
            //switch (type)
            //{
            //    case CategoryTemplateType.TemplateProductTitle:
            //        VM.CategoryTemplateProductTitle.CategoryTemplatePropertyList.AddRange(data);
            //        break;
            //    case CategoryTemplateType.TemplateProductDescription:
            //        VM.CategoryTemplateProductDescription.CategoryTemplatePropertyList.AddRange(data);
            //        break;
            //    case CategoryTemplateType.TemplateProductName:
            //        VM.CategoryTemplateProductName.CategoryTemplatePropertyList.AddRange(data);
            //        break;
            //    case CategoryTemplateType.TemplateWeb:
            //        VM.CategoryTemplateWeb.CategoryTemplatePropertyList.AddRange(data);
            //        break;
            //    default:
            //        break;
            //}
            return result;
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            CategoryTemplatePropertyVM item = lbCategoryProperty.SelectedItem as CategoryTemplatePropertyVM;
            if (item == null)
            {
                Window.MessageBox.Show("请先选择!", MessageBoxType.Error);
                return;
            }
            CategoryTemplateType type = (CategoryTemplateType)button.Tag;
           // List<CategoryTemplatePropertyVM> tempdata;
            switch (type)
            {
                   
                case CategoryTemplateType.TemplateProductTitle:
                    // tempdata = (from p in VM.CategoryTemplateProductTitle.CategoryTemplatePropertyList where p.SysNo == item.SysNo select p).ToList();
                    // if (tempdata != null && tempdata.Count > 0)
                    //{
                    //    Window.MessageBox.Show(string.Format("商品标题已存在 {0} 属性!",item.PropertyDescription), MessageBoxType.Error);
                    //    return;
                    //}
                    //VM.CategoryTemplateProductTitle.CategoryTemplatePropertyList.Add(item);
                    VM.CategoryTemplateProductTitle.CategoryTemplatePropertyDisplay = VM.CategoryTemplateProductTitle.CategoryTemplatePropertyDisplay + " <" + item.PropertyDescription + ">";
                    break;
                case CategoryTemplateType.TemplateProductDescription:
                    // tempdata = (from p in VM.CategoryTemplateProductDescription.CategoryTemplatePropertyList where p.SysNo == item.SysNo select p).ToList();
                    //if (tempdata != null && tempdata.Count > 0)
                    //{
                    //    Window.MessageBox.Show(string.Format("商品描述已存在 {0} 属性!",item.PropertyDescription), MessageBoxType.Error);
                    //    return;
                    //}
                    //VM.CategoryTemplateProductDescription.CategoryTemplatePropertyList.Add(item);
                    VM.CategoryTemplateProductDescription.CategoryTemplatePropertyDisplay = VM.CategoryTemplateProductDescription.CategoryTemplatePropertyDisplay + " <" + item.PropertyDescription + ">";
                    break;
                case CategoryTemplateType.TemplateProductName:
                    //tempdata = (from p in VM.CategoryTemplateProductName.CategoryTemplatePropertyList where p.SysNo == item.SysNo select p).ToList();
                    //if (tempdata != null && tempdata.Count > 0)
                    //{
                    //    Window.MessageBox.Show(string.Format("商品简名已存在 {0} 属性!",item.PropertyDescription), MessageBoxType.Error);
                    //    return;
                    //}
                    //VM.CategoryTemplateProductName.CategoryTemplatePropertyList.Add(item);
                       VM.CategoryTemplateProductName.CategoryTemplatePropertyDisplay = VM.CategoryTemplateProductName.CategoryTemplatePropertyDisplay + " <" + item.PropertyDescription + ">";
                    break;
                case CategoryTemplateType.TemplateWeb:
                    //tempdata = (from p in VM.CategoryTemplateWeb.CategoryTemplatePropertyList where p.SysNo == item.SysNo select p).ToList();
                    //if (tempdata != null && tempdata.Count > 0)
                    //{
                    //    Window.MessageBox.Show(string.Format("Web已存在 {0} 属性!",item.PropertyDescription), MessageBoxType.Error);
                    //    return;
                    //}
                        //VM.CategoryTemplateWeb.CategoryTemplatePropertyList.Add(item);
                        VM.CategoryTemplateWeb.CategoryTemplatePropertyDisplay = VM.CategoryTemplateWeb.CategoryTemplatePropertyDisplay + " <" + item.PropertyDescription + ">";
                   
                    break;
                default:
                    break;
            }
        }

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Show("手动填写的属性格式为<属性>，属性之间以空格隔开，属性必须在三级类属性中存在！",MessageBoxType.Information);
            if (string.IsNullOrEmpty(VM.CategoryTemplateProductDescription.CategoryTemplatePropertyDisplay))
            {
                Window.MessageBox.Show("商品描述模板不完整!", MessageBoxType.Error);
                return;
            }
            if (string.IsNullOrEmpty(VM.CategoryTemplateProductName.CategoryTemplatePropertyDisplay))
            {
                Window.MessageBox.Show("商品名称模板不完整!", MessageBoxType.Error);
                return;
            }
            if (string.IsNullOrEmpty(VM.CategoryTemplateProductTitle.CategoryTemplatePropertyDisplay))
            {
                Window.MessageBox.Show("商品标题模板不完整!", MessageBoxType.Error);
                return;
            }
            if (string.IsNullOrEmpty(VM.CategoryTemplateWeb.CategoryTemplatePropertyDisplay))
            {
                Window.MessageBox.Show("Web模板不完整!", MessageBoxType.Error);
                return;
            }
            VM.CategoryTemplateProductDescription.CategoryTemplatePropertyList.Clear();
            VM.CategoryTemplateProductName.CategoryTemplatePropertyList.Clear();
            VM.CategoryTemplateProductTitle.CategoryTemplatePropertyList.Clear();
            VM.CategoryTemplateWeb.CategoryTemplatePropertyList.Clear();
            List<string> tempdata;
            List<int?> temp= new List<int?>();
            tempdata = Regex.Matches(VM.CategoryTemplateProductDescription.CategoryTemplatePropertyDisplay, "(?<=<)[^<>]+(?=>)").Cast<Match>().Select(m => m.Value).ToList();
            foreach (var item in tempdata)
            {
                foreach (var property in VM.CategoryPropertyList)
                {
                    if (item == property.PropertyDescription && !temp.Contains(property.SysNo))
                    {
                        VM.CategoryTemplateProductDescription.CategoryTemplatePropertyList.Add(property);
                        temp.Add(property.SysNo);
                    }
                }
            }
            temp.Clear();
            tempdata = Regex.Matches(VM.CategoryTemplateProductName.CategoryTemplatePropertyDisplay, "(?<=<)[^<>]+(?=>)").Cast<Match>().Select(m => m.Value).ToList();
            foreach (var item in tempdata)
            {
                foreach (var property in VM.CategoryPropertyList)
                {
                    if (item == property.PropertyDescription && !temp.Contains(property.SysNo))
                    {
                        VM.CategoryTemplateProductName.CategoryTemplatePropertyList.Add(property);
                        temp.Add(property.SysNo);
                    }
                }
            }
            temp.Clear();
            tempdata = Regex.Matches(VM.CategoryTemplateProductTitle.CategoryTemplatePropertyDisplay, "(?<=<)[^<>]+(?=>)").Cast<Match>().Select(m => m.Value).ToList();
            foreach (var item in tempdata)
            {
                foreach (var property in VM.CategoryPropertyList)
                {
                    if (item == property.PropertyDescription && !temp.Contains(property.SysNo))
                    {
                        VM.CategoryTemplateProductTitle.CategoryTemplatePropertyList.Add(property);
                        temp.Add(property.SysNo);
                    }
                }
            }
            temp.Clear();
            tempdata = Regex.Matches(VM.CategoryTemplateWeb.CategoryTemplatePropertyDisplay, "(?<=<)[^<>]+(?=>)").Cast<Match>().Select(m => m.Value).ToList();
            foreach (var item in tempdata)
            {
                foreach (var property in VM.CategoryPropertyList)
                {
                    if (item == property.PropertyDescription && !temp.Contains(property.SysNo))
                    {
                        VM.CategoryTemplateWeb.CategoryTemplatePropertyList.Add(property);
                        temp.Add(property.SysNo);
                    }
                }
            }
            facade.SaveCategoryTemplate(VM, C3SysNo, (obj, arg) => {
                if (arg.FaultsHandle())
                {
                    return;
                }
                Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
            });
        }

       

    }
}
