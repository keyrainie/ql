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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddAdvancedKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public AdvancedKeywordsVM VM { get; set; }
        //public int SysNo { get; set; }
        private bool isAdd = true;
        //private AdvancedKeywordsVM vm;
        private AdvancedKeywordsQueryFacade facade;

        public UCAddAdvancedKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddAdvancedKeywords_Loaded);
        }

        private void UCAddAdvancedKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddAdvancedKeywords_Loaded);
            facade = new AdvancedKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            comAutoRedirectSwitch.ItemsSource = EnumConverter.GetKeyValuePairs<NYNStatus>();
            comStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>();
            if (VM!=null)
            {
                isAdd = false;
                VM.ChannelID = "1";
                LayoutRoot.DataContext = VM;
                //facade.LoadAdvancedKeywordsInfo(SysNo, (obj, args) =>
                //{
                //    if (args.FaultsHandle())
                //        return;

                //    VM = args.Result.Convert<AdvancedKeywordsInfo, AdvancedKeywordsVM>();
                //    LayoutRoot.DataContext = VM;
                //});
            }
            else
            {
                VM = new AdvancedKeywordsVM();
                VM.ChannelID = "1";
                VM.Status = ADStatus.Active;
                VM.AutoRedirectSwitch = NYNStatus.Yes;
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

            if (VM.BeginDate != null && VM.EndDate != null)
            {
                if (VM.BeginDate.Value.CompareTo(VM.EndDate) > 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("结束时间不能小于开始时间!", MessageType.Error);
                    return;
                }
            }
            AdvancedKeywordsInfo item = EntityConvertorExtensions.ConvertVM<AdvancedKeywordsVM, AdvancedKeywordsInfo>(VM, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
                t.ShowName = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.ShowName);
            });
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            if (isAdd)
            {
                facade.AddAdvancedKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful);
                });
            }
            else
            {
                facade.EditAdvancedKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful);
                });
            }
        }
    }
}
