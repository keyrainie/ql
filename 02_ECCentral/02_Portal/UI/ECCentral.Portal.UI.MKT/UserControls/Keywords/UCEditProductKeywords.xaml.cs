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
using System.Text;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCEditProductKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public int? SysNo { get; set; }
        private ProductKeywordsQueryFacade facade;
        public ProductKeywordsQueryVM VM { get; set; }


        public UCEditProductKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCEditProductKeywords_Loaded);
        }
        private void UCEditProductKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCEditProductKeywords_Loaded);
            facade = new ProductKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            if (VM != null)
            {
                VM.ChannelID = "1";
                LayoutRoot.DataContext = VM;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ProductPageKeywords item = EntityConvertorExtensions.ConvertVM<ProductKeywordsQueryVM, ProductPageKeywords>(VM, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, v.Keywords1);
                t.Keywords0 = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, v.Keywords0);
                //t.Keywords2 = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, v.Keywords2);
            });
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            facade.UpdateProductPageKeywords(item, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }

        /// <summary>
        /// 对Keywords1重新分词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReParticipleSave_Click(object sender, RoutedEventArgs e)
        {
            ECCentral.Portal.Basic.Utilities.UtilityHelper.WordSegment(tbKeywords1.Text, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;////处理tbKeywords1
                StringBuilder text = new StringBuilder();
                foreach (string str in args.Result)
                {
                    text.Append(str);
                    text.Append(" ");
                }
                tbKeywords1.Text = text.ToString().TrimEnd(' ');
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
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
