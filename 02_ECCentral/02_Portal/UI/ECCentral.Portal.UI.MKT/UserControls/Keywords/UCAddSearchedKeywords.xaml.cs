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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddSearchedKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private bool isAdd = true;
        private SearchedKeywordsVM vm;
        private SearchedKeywordsQueryFacade facade;

        public UCAddSearchedKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddSearchedKeywords_Loaded);
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

        private void UCAddSearchedKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddSearchedKeywords_Loaded);
            facade = new SearchedKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            cbShowStatus.ItemsSource = new List<KeyValuePair<ADStatus?, string>>() 
            {
                new KeyValuePair<ADStatus?, string>(ADStatus.Active,"展示"),
                new KeyValuePair<ADStatus?, string>(ADStatus.Deactive,"屏蔽"),
            };
            comCreateUserType.ItemsSource = EnumConverter.GetKeyValuePairs<KeywordsOperateUserType>();
            if (SysNo > 0)
            {
                isAdd = false;
                tbKeywords.IsEnabled = false;
                facade.LoadSearchedKeywords(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<SearchedKeywords, SearchedKeywordsVM>();
                    vm.ChannelID = "1";
                    LayoutRoot.DataContext = vm;
                    lstChannel.IsEnabled = false;
                });
            }
            else
            {
                vm = new SearchedKeywordsVM();
                vm.CreateUserType = KeywordsOperateUserType.MKTUser;
                vm.Status = ADStatus.Active;
                vm.ChannelID = "1";
                //cbShowStatus.SelectedIndex = 1;
                //comCreateUserType.SelectedIndex = 1;
                LayoutRoot.DataContext = vm;
                comCreateUserType.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            //vm = LayoutRoot.DataContext as SearchedKeywordsQueryVM;
            //SearchedKeywords item = vm.ConvertVM<SearchedKeywordsQueryVM, SearchedKeywords>();
            SearchedKeywords item = EntityConvertorExtensions.ConvertVM<SearchedKeywordsVM, SearchedKeywords>(vm, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });
            if (isAdd)
            {
               // item.Status = (ADStatus)vm.Status;
               // item.CreateUserType = 0;//MKT
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.AddSearchedKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                });
            }
            else
            {
                item.SysNo = SysNo;
                facade.EditSearchedKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
                });
            }
        }
    }
}
