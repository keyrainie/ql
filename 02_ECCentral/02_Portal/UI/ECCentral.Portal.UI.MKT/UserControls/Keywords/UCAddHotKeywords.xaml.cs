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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddHotKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public HotKeywordsVM VM { get; set; }
        private HotKeywordsQueryFacade facade;

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
                
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        public UCAddHotKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddHotKeywords_Loaded);
        }

        private void UCAddHotKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddHotKeywords_Loaded);
            facade = new HotKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            cbShowStatus.ItemsSource = new List<KeyValuePair<NYNStatus, string>>() 
            {
                new KeyValuePair<NYNStatus, string>(NYNStatus.Yes,"否"),
                new KeyValuePair<NYNStatus, string>(NYNStatus.No,"是"),
            };
            if (VM != null)
            {
                VM.ChannelID = "1";
                LayoutRoot.DataContext = VM;
               this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
               this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);
              
            }
            else
            {
                VM = new HotKeywordsVM();
                VM.ChannelID = "1";
                VM.IsOnlineShow = NYNStatus.Yes;
                LayoutRoot.DataContext = VM;
            }
        }

        public void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageID(VM.PageID);
        }

        public void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageType(VM.PageType);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
		
            HotSearchKeyWords item = EntityConvertorExtensions.ConvertVM<HotKeywordsVM, HotSearchKeyWords>(VM, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });

            item.CompanyCode = CPApplication.Current.CompanyCode;
            item.PageType = this.ucPageType.PageType;
            item.PageID = this.ucPageType.PageID ?? 0;
            item.Extend = this.ucPageType.IsExtendValid;
            if (!VM.SysNo.HasValue)
            {
                facade.AddHotSearchKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CloseDialog(DialogResultType.OK);
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, MessageType.Information);
                });
            }
            else
            {
                facade.EditHotSearchKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CloseDialog(DialogResultType.OK);
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, MessageType.Information);
                });

            }
        }
    }
}
