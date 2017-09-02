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
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class UnifiedImageMaintain : PageBase
    {
         private UnifiedImageVM _currentVM;
         private UnifiedImageFacade facade;
         private UnifiedImageQueryFilterVM filter;
         private ImageSource SelectRowImageSource;

        public UnifiedImageMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _currentVM = new UnifiedImageVM();
            filter = new UnifiedImageQueryFilterVM();
            facade = new UnifiedImageFacade(this);
            this.Grid.DataContext = filter;
            this.DataGrid.Bind();
        }
        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件
            //2.请求服务查询
            filter.PagingInfo = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            facade.QueryUnifiedImages(filter, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.DataGrid.ItemsSource = args.Result.Rows.ToList();
                    this.DataGrid.TotalCount = args.Result.TotalCount;

                });
        }
        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            UCUnifiedImageUpload upWindow = new UCUnifiedImageUpload(ConstValue.DomainName_MKT);
            upWindow.AppName = "mkt";
            upWindow.UploadUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_MKT, "ImageBaseUrl");
            upWindow.Dialog = Window.ShowDialog(ResUnifiedImageMaitain.TextBlock_PicUpload, upWindow, (obj, args) =>
            {
                if (args.Data != null)
                {

                    _currentVM.ImageName = ((dynamic)args.Data).ImageName;
                    _currentVM.ImageUrl = ((dynamic)args.Data).ImageUrl;
                    //保存图片
                    if (string.IsNullOrEmpty(_currentVM.ImageName))
                    {
                        //Window.Alert("请选择要上传的图片！");
                        Window.Alert(ResUnifiedImageMaitain.Info_ImageNull);
                        return;
                    }
                    facade.CreateUnifiedImage(_currentVM, (s, arg) =>
                    {
                        if (arg.FaultsHandle()) return;
                        //this.Window.Alert("保存图片成功！");
                        this.Window.Alert(ResUnifiedImageMaitain.Info_SaveSuccess);
                        this.DataGrid.Bind();
                    });
                }

            }, new Size(600, 300));

        }

        private void BtnQueryImage_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectRowImageSource != null)
            {
                Image tipImage = sender as Image;
                if (tipImage != null)
                {
                    tipImage.Source = SelectRowImageSource;
                }
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            if (image != null)
            {
                SelectRowImageSource = image.Source;
            }
        }
        
    }
}
