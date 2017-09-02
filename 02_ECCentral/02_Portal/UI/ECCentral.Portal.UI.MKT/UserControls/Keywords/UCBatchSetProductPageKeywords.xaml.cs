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
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;
using System.Text.RegularExpressions;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCBatchSetProductPageKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        /// <summary>
        /// 是否为批量添加，否则批量删除
        /// </summary>
        public bool BatchAdd { get; set; }
        /// <summary>
        /// 选中的商品
        /// </summary>
        public List<string> productList { get; set; }

        private ProductKeywordsQueryFacade facade;

        public UCBatchSetProductPageKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCBatchSetProductPageKeywords_Loaded);
        }

        private void UCBatchSetProductPageKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCBatchSetProductPageKeywords_Loaded);
            facade = new ProductKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            ECCentral.Portal.Basic.Utilities.CodeNamePairHelper.GetList("MKT", "KeywordsListForProductPageKeywords", (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                comKeywordsCategory.ItemsSource = args.Result;
                comKeywordsCategory.SelectedIndex = 0;
            });

            if (BatchAdd)
                btnSaveDelete.Visibility = Visibility.Collapsed;
            else
                btnSaveAdd.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAdd_Click(object sender, RoutedEventArgs e)
        {
            //CheckHtml
            if (string.IsNullOrEmpty(tbKeywords.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_KeywordsIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            else if (StringUtility.CheckHtml(tbKeywords.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_KeywordsDonotInclude, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            else if (!UtilityHelper.CheckScript(tbKeywords.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_KeywordsDonotIncludeScript, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            else
            {
                //选择keyword0  或  1

                ProductPageKeywordsRsp rsp = new ProductPageKeywordsRsp();
                rsp.ProductList = productList;
                rsp.BatchAdd = true;
                if (comKeywordsCategory.SelectedIndex == 0)
                    rsp.ReplKeywords0 = tbKeywords.Text;
                else
                    rsp.ReplKeywords1 = tbKeywords.Text;
                rsp.LanguageCode = ConstValue.BizLanguageCode;
                rsp.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.BatchUpdateProductPageKeywords(rsp, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_SettingSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);

                });
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDelete_Click(object sender, RoutedEventArgs e)
        {
            //CheckHtml
            if (string.IsNullOrEmpty(tbKeywords.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_KeywordsIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            else if (StringUtility.CheckHtml(tbKeywords.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_KeywordsDonotInclude, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            else if (!UtilityHelper.CheckScript(tbKeywords.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_KeywordsDonotIncludeScript, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            else
            {
                ProductPageKeywordsRsp rsp = new ProductPageKeywordsRsp();
                rsp.ProductList = productList;
                rsp.BatchAdd = false;
                if (comKeywordsCategory.SelectedIndex == 0)
                    rsp.ReplKeywords0 = tbKeywords.Text;
                else
                    rsp.ReplKeywords1 = tbKeywords.Text;
                rsp.LanguageCode = ConstValue.BizLanguageCode;
                rsp.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.BatchUpdateProductPageKeywords(rsp, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_SettingSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);

                });
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

        //public bool CheckScript(string content)
        //{
        //    Regex regex = null;
        //    Match match = null;
        //    string str = "<script.*?/.*?script.*?>";
        //    if (!String.IsNullOrEmpty(content))
        //    {
        //        regex = new Regex(str, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //        match = regex.Match(content.Trim());
        //        if (match.Success)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
    }
}
