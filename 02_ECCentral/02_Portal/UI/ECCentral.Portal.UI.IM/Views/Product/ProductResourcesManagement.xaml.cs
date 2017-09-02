using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductResourcesManagement : PageBase
    {
        #region 属性
        private ProductResourcesManagementFacade _facade;
        private ProductFacade _productFacade;
        private int _sysNo;
        private int _groupSysNo;
        private ProductResourcesManagementVM _productResourcesVM;
        private string imageFor360Url = "";
        private string videoUrl = "";
        private  string flvUrl;
        #endregion

        #region 初始化加载
        public ProductResourcesManagement()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            var param = Request.Param;
            flvUrl = Application.Current.Host.Source.AbsoluteUri;
            flvUrl = flvUrl.Substring(0, flvUrl.IndexOf("ClientBin")) + "Flvplay/flvplayer.htm?url=";
            if (int.TryParse(param, out _sysNo))
            {
                BindPage(_sysNo);
            }
            else if (Request.Param == null)
            {
                BindPage(0);
            }
            else
            {
                Window.MessageBox.Show("无效编号.", MessageBoxType.Warning);
                return;
            }
        }
        #endregion

        #region 查询绑定
        private void BindPage(int sysNo)
        {
            if (sysNo > 0)
            {
                _productFacade = new ProductFacade();
                _productFacade.GetProductGroup(sysNo, (o, group) =>
                            {
                                if(group.FaultsHandle())
                                {
                                    return;
                                }
                                if(group.Result.SysNo!=null)
                                _groupSysNo = group.Result.SysNo.Value;
                            });
                _productFacade.GetProductInfo(sysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == null || args.Result.ProductBasicInfo == null || args.Result.ProductCommonInfoSysNo == null || args.Result.ProductCommonInfoSysNo.Value <= 0)
                    {
                        Window.MessageBox.Show("获取商品资源信息失败.", MessageBoxType.Warning);
                        return;
                    }
                    Panel_ProductDesc.Visibility = Visibility.Visible;
                    var vm = args.Result.Convert<ProductInfo, ProductResourcesManagementVM>((v, t) =>
                            {
                                t.PromotionTitle = v.PromotionTitle == null || string.IsNullOrWhiteSpace(args.Result.PromotionTitle.Content) ? args.Result.ProductName : args.Result.PromotionTitle.Content;
                                t.ProductModel = v.ProductBasicInfo.ProductModel == null
                                        ? "" : args.Result.ProductBasicInfo.ProductModel.Content;

                            });
                    vm.ProductResources = args.Result.ProductBasicInfo.ProductResources;
                    vm.ProductCommonInfoSysNo = args.Result.ProductCommonInfoSysNo.Value;
                    vm.CommonSKUNumber = args.Result.ProductBasicInfo.CommonSkuNumber;
                    vm.ResourceCollection = new ProductResourcesCollection();
                    vm.ProductSysNo = sysNo;
                    _productResourcesVM = vm;
                    if (vm.ProductResources.Any())
                    {
                        string websiteProductImgUrl = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsiteProductImageUrl);
                        string websiteProductOtherImgUrl = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsiteProductOtherImageUrl);
                        
                        vm.ProductResources.ForEach(v=>
                                        {
                                            v.Resource.TemporaryName = v.Resource.ResourceURL;
                                            v.ProductCommonInfoSysNo = vm.ProductCommonInfoSysNo;
                                            if (v.Resource.Type == ResourcesType.Image)
                                            {
                                                //Ocean.20130514, Move ImgUrl to ControlMenuConfiguration
                                                v.Resource.ResourceURL = websiteProductImgUrl + v.Resource.ResourceURL;
                                            }
                                            else
                                            {
                                                //Ocean.20130514, Move ImgUrl to ControlMenuConfiguration
                                                v.Resource.ResourceURL = websiteProductOtherImgUrl + v.Resource.ResourceURL;
                                            }
                                        });
                        SetResourcesStatus();
                        vm.ProductResources=vm.ProductResources.Where(p => p.Resource.Type == ResourcesType.Image).ToList();
                    }
                    DataContext = vm;
                     _productResourcesVM.IsNeedWatermark = true;
                    StackPanelEdit.Visibility = Visibility.Visible;
                    StackPanelAdd.Visibility = Visibility.Collapsed;
                });
            }
            else
            {
                Panel_ProductDesc.Visibility = Visibility.Collapsed;
                var vm = new ProductResourcesManagementVM { ResourceCollection = new ProductResourcesCollection() };
                _productResourcesVM = vm;
                DataContext = vm;
                StackPanelEdit.Visibility = Visibility.Collapsed;
                StackPanelAdd.Visibility = Visibility.Visible;
                 _productResourcesVM.IsNeedWatermark = true;
            }
        }

        private void Bind(int sysNo)
        {
            if (sysNo > 0 && _productResourcesVM != null)
            {
                _productFacade = new ProductFacade();
                _productFacade.GetProductInfo(sysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    _productResourcesVM.ProductResources = args.Result.ProductBasicInfo.ProductResources;
                    if (_productResourcesVM.ProductResources.Any())
                    {
                        string websiteProductImgUrl = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsiteProductImageUrl);
                        string websiteProductOtherImgUrl = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebsiteProductOtherImageUrl);                       

                        _productResourcesVM.ProductResources.ForEach(v =>
                        {
                            if (args.Result.ProductCommonInfoSysNo.HasValue)
                            {
                                v.ProductCommonInfoSysNo = args.Result.ProductCommonInfoSysNo.Value;
                            }
                            v.Resource.TemporaryName = v.Resource.ResourceURL;
                            if (v.Resource.Type == ResourcesType.Image)
                            {
                                v.Resource.ResourceURL = websiteProductImgUrl + v.Resource.ResourceURL;
                            }
                            else
                            {
                                v.Resource.ResourceURL = websiteProductOtherImgUrl + v.Resource.ResourceURL;
                            }
                        });
                        SetResourcesStatus();
                        _productResourcesVM.ProductResources = _productResourcesVM.ProductResources.Where(p => p.Resource.Type == ResourcesType.Image).ToList();
                    }
                    DataContext = _productResourcesVM;

                });
            }
            else
            {
                Window.MessageBox.Show("没有获得商品资源信息.", MessageBoxType.Warning);
                return;
            }
        }

        private void OtherBind(int sysNo)
        {
          dgProductResource.Bind();
        }

        private void Sort()
        {
            if (_productResourcesVM == null) return;
            _productResourcesVM.ProductResources =
                _productResourcesVM.ProductResources.OrderBy(p => p.Resource.Priority).ToList();
            DataContext = _productResourcesVM;
        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            _facade.Save();
        }

        private void MoveTopButton_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            var item = ListBox_ImageList.SelectedItem as dynamic;
            _facade.OnSort = Sort;
            if (item == null || item.Resource == null || item.Resource.ResourceSysNo <= 0)
            {
                Window.MessageBox.Show("请选择一条记录.", MessageBoxType.Warning);
                return;
            }
            _facade.MoveTop(item.Resource.ResourceSysNo);
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            var item = ListBox_ImageList.SelectedItem as dynamic;
            _facade.OnSort = Sort;
            if (item == null || item.Resource == null || item.Resource.ResourceSysNo <= 0)
            {
                Window.MessageBox.Show("请选择一条记录.", MessageBoxType.Warning);
                return;
            }
            _facade.MoveUp(item.Resource.ResourceSysNo);
            ListBox_ImageList.SelectedItem = item;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            var item = ListBox_ImageList.SelectedItem as dynamic;
            _facade.OnSort = Sort;
            if (item == null || item.Resource == null || item.Resource.ResourceSysNo <= 0)
            {
                Window.MessageBox.Show("请选择一条记录.", MessageBoxType.Warning);
                return;
            }
            if (_productResourcesVM == null || _productResourcesVM.ProductResources.Count==1)
            {
                Window.MessageBox.Show("不能删除最后一张图片.", MessageBoxType.Warning);
                return;
            }
            _facade.Delete(item.Resource.ResourceSysNo);
            SaveButton_Click(null,null);
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            var item = ListBox_ImageList.SelectedItem as dynamic;
            _facade.OnSort = Sort;
            if (item == null || item.Resource == null || item.Resource.ResourceSysNo <= 0)
            {
                Window.MessageBox.Show("请选择一条记录.", MessageBoxType.Warning);
                return;
            }
            _facade.MoveDown(item.Resource.ResourceSysNo);
            ListBox_ImageList.SelectedItem = item;
        }

        private void MoveBottomButton_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            var item = ListBox_ImageList.SelectedItem as dynamic;
            _facade.OnSort = Sort;
            if (item == null || item.Resource == null || item.Resource.ResourceSysNo <= 0)
            {
                Window.MessageBox.Show("请选择一条记录.", MessageBoxType.Warning);
                return;
            }
            _facade.MoveBottom(item.Resource.ResourceSysNo);
        }

        private void btnSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            _facade.SelectFiles();
            var fileNames = _productResourcesVM.ResourceCollection.Select(p => p.File.Name).ToList();
            _facade.IsExistFileName(fileNames,(obj,arg)=>
                    {
                          if(arg.FaultsHandle())
                          {
                              return;
                          }
                          var files = arg.Result;
                          foreach (var item in _productResourcesVM.ResourceCollection)
                          {
                              if (item.FileUploadProcessStates == FileUploadProcessStates.Checking)
                              {
                                  item.FileUploadProcessStates = FileUploadProcessStates.WaitingToUpload;
                                  if (files != null && files.Contains(item.File.Name))
                                  {
                                      item.Remark = "存在相同的图片或视频";
                                      if (!item.FileUploadProcessStatesDesc.Contains("*"))
                                          item.FileUploadProcessStatesDesc = item.FileUploadProcessStatesDesc + "(*)";
                                  }
                              }
                                

                          }
                    });
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            if (_sysNo > 0)
                _facade.OnBind += Bind;
            else
                _facade.OnBind += OtherBind;
            _facade.Upload();

           
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade(_productResourcesVM);
            if(_sysNo>0)
            _facade.OnBind += Bind;
            else
            {
                _facade.OnBind += OtherBind;
            }
            _facade.ClearWaitUploadFiles();
        }

        private void dgProductResource_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _facade = new ProductResourcesManagementFacade();
            if (_productResourcesVM == null) return;
            var count =
                _productResourcesVM.ResourceCollection.Where(
                    p => p.FileUploadProcessStates == FileUploadProcessStates.Finished).Count();
            if (count > 0)
            {
                var queryVM = new ProductResourcesQueryVM
                {
                    CommonSKUNumberList = _productResourcesVM.ResourceCollection
                        .Where(p => p.FileUploadProcessStates == FileUploadProcessStates.Finished)
                        .Select(p => p.CommonSKUNumber).ToList()
                };
                _facade.QueryResources(queryVM, e.SortField, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    dgProductResource.ItemsSource = args.Result.Rows;
                    _productResourcesVM.ProductGroupCount = args.Result.TotalCount;
                    var skuCount = _productResourcesVM.ResourceCollection
                        .Select(p => p.CommonSKUNumber)
                        .Distinct().Count();
                    _productResourcesVM.CommonSKUCount = skuCount;
                    _productResourcesVM.SucessCount = count;
                    var tempCount = _productResourcesVM.ResourceCollection.Count - count;
                    _productResourcesVM.FaileCount = tempCount;

                });
            }
            else
            {
                dgProductResource.ItemsSource = null;
                _productResourcesVM.ProductGroupCount = 0;
                _productResourcesVM.CommonSKUCount = 0;
                _productResourcesVM.SucessCount = 0;
                _productResourcesVM.FaileCount = 0;
            }
        }

        private void ImageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = sender as ListBox;
            if (null != list)
            {
                var item = (ProductResourceForNewegg)list.SelectedItem;
                if (item != null)
                    txt_PicResourceUrl.Text = item.Resource.ResourceURL;
            }
        }

        private void HyperlinkPromotionTitle_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.IM_ProductGroupMaintainFormat, _groupSysNo), null, true);
        }

        private void HyperlinkCommonSKUNumber_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.IM_ProductMaintainUrlFormat, _productResourcesVM.ProductSysNo), null, true);
        }
        #endregion

        private void PreviewSWF_Click(object sender, RoutedEventArgs e)
        {
            var tempFlvUrl = flvUrl + imageFor360Url;
            SetWindows(tempFlvUrl);
            //var htmlContext = tempfacade.GetHtmlContext(imageFor360Url);

        }

        private void PreviewVideo_Click(object sender, RoutedEventArgs e)
        {
            var tempFlvUrl = flvUrl + videoUrl;
            SetWindows(tempFlvUrl);
            //var htmlContext = tempfacade.GetHtmlContext(videoUrl);
          
        }

        public void SetWindows(string url)
        {
            var option = new HtmlPopupWindowOptions();
            option.Directories = true;//是否开启ie地址栏
            option.Height = 280;//浏览器窗口高度
            option.Width = 380;//浏览器窗口宽度
            option.Status = false;//状态栏是否可见
            option.Location = false;//是否弹出窗口
            option.Menubar = false;//菜单栏是否可见
            option.Resizeable = true;//是否可调整窗口高宽度
            option.Scrollbars = false;//滚动条是否可见
            option.Toolbar = false;//工具栏是否可见
            option.Left = option.Width / 2;//窗口的X坐标
            option.Top = option.Height / 2;//窗口的Y坐标
            HtmlPage.PopupWindow(new Uri(url), "_blank", option);


        }
        #endregion

        #region 设置视频以及360图片
         private void SetResourcesStatus()
         {
             var image =
                 _productResourcesVM.ProductResources.Where(p => p.Resource.Type == ResourcesType.Image360).
                     FirstOrDefault();
             if(image!=null&&image.Resource!=null)
             {
                 ImageFor360Browse.Visibility = Visibility.Visible;
                 imageFor360Url = image.Resource.ResourceURL;
             }
             else
             {
                 ImageFor360Browse.Visibility = Visibility.Collapsed;
             }
             var video =
                _productResourcesVM.ProductResources.Where(p => p.Resource.Type == ResourcesType.Video).
                    FirstOrDefault();
             if (video != null && video.Resource != null)
             {
                 VideoBrowse.Visibility = Visibility.Visible;
                 videoUrl = video.Resource.ResourceURL;
             }
             else
             {
                 VideoBrowse.Visibility = Visibility.Collapsed;
             }
         }
        #endregion

        #region 跳转
        #endregion


    }

}
