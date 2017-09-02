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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddUniversalCategoryKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public CategoryKeywordsVM VM { get; set; }
        private CategoryKeywordsQueryFacade facade;

        public UCAddUniversalCategoryKeywords()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCAddUniversalCategoryKeywords_Loaded);
        }

        private void UCAddUniversalCategoryKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddUniversalCategoryKeywords_Loaded);
            facade = new CategoryKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            if (VM != null)
            {
                VM.ChannelID = "1";
                ucKeywordCategory.IsEnabled = false;
                lstChannel.IsEnabled = false;
                ucKeywordCategory.LoadCategoryCompleted += InitCategory;
            }
            else
            {
                VM = new CategoryKeywordsVM();
                VM.ChannelID = "1";
                LayoutRoot.DataContext = VM;
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
            }
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            CategoryKeywords item = EntityConvertorExtensions.ConvertVM<CategoryKeywordsVM, CategoryKeywords>(VM, (v, t) =>
            {
                t.CommonKeywords = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, v.CommonKeywords);
            });
            if (VM.SysNo.HasValue)//更新
            {
                facade.UpdateCommonKeyWords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                });
            }
            else
            {
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.CheckCategoryKeywordsC3SysNo(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    if (args.Result)
                        CPApplication.Current.CurrentPage.Context.Window.Confirm(ResKeywords.Information_BUpdateCommonKeywords, (obj2, args2) =>
                        {
                            if(args2.DialogResult==Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                                facade.AddCommonKeyWords(item, (obj1, args1) =>
                                {
                                    if (args1.FaultsHandle())
                                        return;

                                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                                });
                        });
                    else
                        facade.AddCommonKeyWords(item, (obj1, args1) =>
                        {
                            if (args1.FaultsHandle())
                                return;

                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);

                        });
                });
            }
        }
    }
}
