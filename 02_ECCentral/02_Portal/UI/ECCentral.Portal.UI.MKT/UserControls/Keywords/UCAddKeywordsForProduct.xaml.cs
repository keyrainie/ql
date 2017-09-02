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
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddKeywordsForProduct : UserControl
    {
        public IDialog Dialog { get; set; }
        public KeyWordsForProductQueryVM VM { get; set; }
        private KeyWordsForProductQueryFacade facade;
        private bool isAdd = true;

        public UCAddKeywordsForProduct()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddKeywordsForProduct_Loaded);
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

        private void UCAddKeywordsForProduct_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddKeywordsForProduct_Loaded);
            facade = new KeyWordsForProductQueryFacade(CPApplication.Current.CurrentPage);

            if (VM != null)
            {
                isAdd = false;
                VM.ChannelID = "1";
                lstChannel.IsEnabled = false;
                LayoutRoot.DataContext = VM;
            }
            else
            {
                VM = new KeyWordsForProductQueryVM();
                VM.ChannelID = "1";
                LayoutRoot.DataContext = VM;
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            
            if (string.IsNullOrEmpty(VM.ProductSysNo))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择商品!");
                return;
            }
            if (string.IsNullOrEmpty(VM.Priority))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("优先级不能为空!");
                return;
            }
            if (string.IsNullOrEmpty(VM.Keywords))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请输入关键字!");
                return;
            }


            ProductKeywordsInfo item = EntityConvertorExtensions.ConvertVM<KeyWordsForProductQueryVM, ProductKeywordsInfo>(VM, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            if (isAdd)
            {
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.AddProductKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                });
            }
            else
            {
                facade.EditProductKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                });

            }
        }
    }
}
