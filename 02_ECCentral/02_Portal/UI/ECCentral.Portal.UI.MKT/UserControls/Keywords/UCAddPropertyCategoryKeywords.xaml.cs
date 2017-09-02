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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddPropertyCategoryKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public CategoryKeywordsVM VM { get; set; }
        private CategoryKeywordsQueryFacade facade;
        private List<CategoryKeywordPropertyVM> gridVM;
        private int c3SysNo = 0;

        public UCAddPropertyCategoryKeywords()
        {
            InitializeComponent();
        
            Loaded += new RoutedEventHandler(UCAddPropertyCategoryKeywords_Loaded);
            ucKeywordCategory.cmbCategory3SelectionChanged += new EventHandler<EventArgs>(ucKeywordCategory_Category3SelectChanged);//加载属性列表
        }

        private void UCAddPropertyCategoryKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddPropertyCategoryKeywords_Loaded);
            facade = new CategoryKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            if (VM != null)
            {
                VM.ChannelID = "1";
                lstChannel.IsEnabled = false;
                ucKeywordCategory.IsEnabled = false;
                c3SysNo = VM.Category3SysNo.Value;
                ucKeywordCategory.LoadCategoryCompleted += InitCategory;
            }
            else
            {
                VM = new CategoryKeywordsVM();
                VM.ChannelID = "1";
                LayoutRoot.DataContext = VM;
            }
        }

        void ucKeywordCategory_Category3SelectChanged(object sender, EventArgs e)
        {
            if (VM.Category3SysNo.HasValue && VM.Category3SysNo.Value != c3SysNo)
            {
                c3SysNo = VM.Category3SysNo.Value;
                QueryResultGrid.Bind();
            }
        }

        private void InitCategory(object sender, EventArgs e)
        {
            if (VM != null && VM.Category3SysNo.HasValue)
            {
                ucKeywordCategory.Category3SysNo = VM.Category3SysNo.Value;
                VM.Category1SysNo = ucKeywordCategory.Category1SysNo.Value;
                VM.Category2SysNo = ucKeywordCategory.Category2SysNo.Value; 
                LayoutRoot.DataContext = VM;
                QueryResultGrid.Bind();
            }
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if(c3SysNo>0)
                facade.GetPropertyByCategory(c3SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    List<ECCentral.BizEntity.IM.CategoryProperty> list = args.Result;
                    if (list != null && list.Count > 0)
                    {
                        gridVM = new List<CategoryKeywordPropertyVM>();
                        
                        //int PageSize = e.PageSize;
                        //int PageIndex = e.PageIndex;
                        //暂时不提供分页
                        foreach (ECCentral.BizEntity.IM.CategoryProperty p in list)
                        {
                            CategoryKeywordPropertyVM v = new CategoryKeywordPropertyVM();
                            v.SysNo = p.Property.SysNo.Value;
                            v.PropertyName = p.Property.PropertyName.Content;

                            if (string.IsNullOrEmpty(VM.PropertyKeywordsList)||VM.PropertyKeywordsList.IndexOf(p.Property.SysNo.Value.ToString() + ",") < 0)//如果没有添加该属性
                                v.IsChecked = false;
                            else
                                v.IsChecked = true;
                            gridVM.Add(v);
                        }
                        QueryResultGrid.ItemsSource = gridVM;
                    }
                });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (gridVM != null)
            {
                StringBuilder str = new StringBuilder();
                gridVM.ForEach(item =>
                {
                    if (item.IsChecked == true)
                        str.Append(item.SysNo.Value.ToString()).Append(',');
                });
                if (!string.IsNullOrEmpty(str.ToString()))
                {
                    CategoryKeywords item = EntityConvertorExtensions.ConvertVM<CategoryKeywordsVM, CategoryKeywords>(VM, (v, t) =>
                    {
                        //t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
                        t.PropertyKeywords = str.ToString();//.TrimEnd(',');
                    });
                    if (VM.SysNo.HasValue)
                    {
                        facade.UpdatePropertyKeywords(item, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                                return;

                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                        });
                    }
                    else
                    {
                        item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                        facade.AddPropertyKeywords(item, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                                return;

                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                        });
                    }
                }
                else
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Warning);
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_MoreThanOneRecord, MessageType.Information);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
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

    }
}
