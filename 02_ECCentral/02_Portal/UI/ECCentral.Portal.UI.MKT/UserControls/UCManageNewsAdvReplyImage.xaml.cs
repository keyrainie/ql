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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using System.Text;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCManageNewsAdvReplyImage : UserControl
    {
        public int? SysNo { get; set; }

        public IDialog Dialog { get; set; }
        public string ImageList { get; set; }
        private NewsAdvReplyQueryFacade facade;
        private List<CommentImageVM> ImageVM;

        public UCManageNewsAdvReplyImage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCManageNewsAdvReplyImage_Loaded);
        }

        private void UCManageNewsAdvReplyImage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCManageNewsAdvReplyImage_Loaded);
            facade = new NewsAdvReplyQueryFacade(CPApplication.Current.CurrentPage);
            InitProductReivewDate();
        }
        public void InitProductReivewDate()
        {
            if (!string.IsNullOrEmpty(ImageList))
            {
                ImageVM = new List<CommentImageVM>();
                string newsUploadP80ImageUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_NewsUploadP80ImageUrl);

                string[] list = ImageList.Split('|');
                foreach (string url in list)
                {
                    ImageVM.Add(new CommentImageVM() { ImageUrl = newsUploadP80ImageUrl + url, IsChecked = false });
                }
                ProductReivewImagesList.ItemsSource = ImageVM;

                ProductReivewImagesList.Visibility = System.Windows.Visibility.Visible;
                btnBatchDeleteImage.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ProductReivewImagesList.Visibility = System.Windows.Visibility.Collapsed;
                btnBatchDeleteImage.Visibility = System.Windows.Visibility.Collapsed;
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

        /// <summary>
        ///  批量删除图片: 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder updateImage = new StringBuilder();
            ImageVM.ForEach(item =>
            {
                if (!item.IsChecked)
                    updateImage.Append(item.ImageUrl).Append('|');
            });
            if (!string.IsNullOrEmpty(updateImage.ToString()))
            {
                if (updateImage.ToString().TrimEnd('|') != ImageList)
                {
                    string param = updateImage.ToString().TrimEnd('|') + "!" + SysNo.ToString();//拼接SysNo
                    facade.DeleteProductReviewImage(param, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;

                        ImageList = updateImage.ToString().TrimEnd('|');//显示删除后的图片
                        InitProductReivewDate();
                    });
                }
                else
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
            }
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (ImageVM == null || checkBoxAll == null)
                return;
            ImageVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }

        private void imageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Ocean.20130514, Move to ControlPanelConfiguration
            string newsUploadP80ImageUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_NewsUploadP80ImageUrl);
            string newsUploadOriginalImageUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_NewsUploadOriginalImageUrl);

            string url = ((Image)sender).Source.ToString().Replace(newsUploadP80ImageUrl, newsUploadOriginalImageUrl);
            UtilityHelper.OpenWebPage(url);
        }

    }
}
