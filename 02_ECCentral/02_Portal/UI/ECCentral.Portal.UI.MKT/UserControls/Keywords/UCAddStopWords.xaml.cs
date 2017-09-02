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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddStopWords : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private StopWordsQueryVM vm;
        private StopWordsQueryFacade facade;

        public UCAddStopWords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddStopWords_Loaded);
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
        private void UCAddStopWords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddStopWords_Loaded);
            facade = new StopWordsQueryFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                facade.LoadStopWordsInfo(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<StopWordsInfo, StopWordsQueryVM>();
                    vm.ChannelID = "1";
                    LayoutRoot.DataContext = vm;
                });
            }
            else
            {
                vm = new StopWordsQueryVM();
                vm.ChannelID = "1";
                vm.Status = ADTStatus.Active;
                LayoutRoot.DataContext = vm;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            vm = LayoutRoot.DataContext as StopWordsQueryVM;
            //StopWordsInfo item = vm.ConvertVM<StopWordsQueryVM, StopWordsInfo>();

            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            if (string.IsNullOrEmpty(vm.Keywords))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("阻止词内容不能为空!",MessageType.Error);
                return;
            }
            StopWordsInfo item = EntityConvertorExtensions.ConvertVM<StopWordsQueryVM, StopWordsInfo>(vm, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });
            
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            facade.AddStopWordsInfo(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }
    }
}
