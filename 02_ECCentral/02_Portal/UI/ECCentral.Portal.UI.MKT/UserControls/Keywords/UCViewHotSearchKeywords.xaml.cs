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
using ECCentral.BizEntity.MKT;
using System.Text;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCViewHotSearchKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        private HotKeywordsQueryFacade facade;
        public HotSearchKeyWords Model { get; set; }

        public UCViewHotSearchKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCViewHotSearchKeywords_Loaded);
        }

        private void UCViewHotSearchKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCViewHotSearchKeywords_Loaded);
            facade = new HotKeywordsQueryFacade(CPApplication.Current.CurrentPage);

            if (Model != null)
                facade.GetHotKeywordsListByPageType(Model, (obj, args) =>{
                    if (args.FaultsHandle())
                        return;

                    StringBuilder str = new StringBuilder();
                    foreach (string keyword in args.Result)
                    {
                        str.Append(keyword).Append("|"); 
                    }
                    tbIndexPageKeywords.Text = str.ToString().TrimEnd('|');
                });
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_DonotExsitTheKeywords, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
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

    }
}
