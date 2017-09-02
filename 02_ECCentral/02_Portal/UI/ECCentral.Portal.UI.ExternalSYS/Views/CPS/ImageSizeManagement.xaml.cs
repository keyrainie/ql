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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.ExternalSYS.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ImageSizeManagement : PageBase
    {
        private ImageSizeFacade facade;
        public ImageSizeManagement()
        {
            InitializeComponent();
            this.ImageSizeResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ImageSizeResult_LoadingDataSource);
            this.Loaded += (sender, e) =>
            {
                facade = new ImageSizeFacade();
                this.ImageSizeResult.Bind();
            };

        }

        void ImageSizeResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetAllImageSize(e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.ImageSizeResult.ItemsSource = arg.Result.Rows;
                this.ImageSizeResult.TotalCount = arg.Result.TotalCount;
            });
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ImageSizeMaintain item = new ImageSizeMaintain();
            item.Dialog = Window.ShowDialog("添加尺寸", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ImageSizeResult.Bind();
                }
            }, new Size(450, 250));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hyperlinkBrandID_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否删除?", (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    HyperlinkButton link = (HyperlinkButton)sender;
                    int sysNo = (int)link.Tag;
                    facade.DeleteImageSize(sysNo, (objs, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert("删除成功!");
                        this.ImageSizeResult.Bind();
                    });
                }
            });



        }
    }
}
