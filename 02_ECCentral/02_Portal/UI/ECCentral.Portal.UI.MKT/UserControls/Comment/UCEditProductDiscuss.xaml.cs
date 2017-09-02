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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Comment
{
    public partial class UCEditProductDiscuss : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private ProductDiscussQueryVM vm;
        private ProductDiscussQueryFacade facade;

        public UCEditProductDiscuss()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCEditProductDiscuss_Loaded);
        }

        private void UCEditProductDiscuss_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCEditProductDiscuss_Loaded);
            facade = new ProductDiscussQueryFacade(CPApplication.Current.CurrentPage);

            facade.LoadProductDiscuss(SysNo, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                vm = args.Result.Convert<ProductDiscussDetail, ProductDiscussQueryVM>();

                QueryResultGrid.ItemsSource = vm.ProductDiscussReplyList;

                LayoutRoot.DataContext = vm;
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

        /// <summary>
        /// 添加回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAndReply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbReplyContent.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_ReplyContentIsNotNull,Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else
            {
                ProductDiscussReply item = vm.ConvertVM<ProductDiscussQueryVM, ProductDiscussReply>();
                item.Status = "A";//回复的初始化状态
                item.DiscussSysNo = vm.SysNo.Value;
                item.Content = tbReplyContent.Text;
                item.NeedAdditionalText = cbNeedAdditionalText.IsChecked.Value ? YNStatus.Yes : YNStatus.No;
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

                facade.CreateProductDiscussReply(item, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    facade.LoadProductDiscuss(SysNo, (s2, args2) =>
                    {
                        if (args2.FaultsHandle())
                            return;

                        vm = args2.Result.Convert<ProductDiscussDetail, ProductDiscussQueryVM>();
                        QueryResultGrid.ItemsSource = vm.ProductDiscussReplyList;
                    });	
                });
            }
        }
        
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
        }
    }
}
