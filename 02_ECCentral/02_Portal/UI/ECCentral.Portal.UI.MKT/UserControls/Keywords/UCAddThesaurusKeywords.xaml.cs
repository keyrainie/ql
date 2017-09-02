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
    public partial class UCAddThesaurusKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private ThesaurusKeywordsVM vm;
        private ThesaurusKeywordsQueryFacade facade;


        public UCAddThesaurusKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddThesaurusKeywords_Loaded);
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

        private void UCAddThesaurusKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddThesaurusKeywords_Loaded);
            facade = new ThesaurusKeywordsQueryFacade(CPApplication.Current.CurrentPage);

            cbTypeSource.ItemsSource = EnumConverter.GetKeyValuePairs<ThesaurusWordsType>();
            if (SysNo > 0)
            {
                facade.LoadThesaurusWords(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    vm = args.Result.Convert<ThesaurusInfo, ThesaurusKeywordsVM>();
                    vm.ChannelID = "1";
                    LayoutRoot.DataContext = vm;
                });
            }
            else
            {
                vm = new ThesaurusKeywordsVM();
                vm.ChannelID = "1";
                vm.Type = ThesaurusWordsType.Monodirectional;
                vm.Status = ADTStatus.Active;
                LayoutRoot.DataContext = vm;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            //vm = LayoutRoot.DataContext as ThesaurusKeywordsVM;
            ThesaurusInfo item = EntityConvertorExtensions.ConvertVM<ThesaurusKeywordsVM, ThesaurusInfo>(vm, (v, t) =>
            {
                t.ThesaurusWords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.ThesaurusWords);
            });
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            facade.AddThesaurusWords(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
               
            });
        }
    }
}
