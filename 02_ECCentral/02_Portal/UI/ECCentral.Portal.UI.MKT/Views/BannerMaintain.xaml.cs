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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;
using System.Text.RegularExpressions;
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BannerMaintain : PageBase
    {
        BannerLocationVM _currentVM;
        /// <summary>
        /// 标识界面是否处于编辑记录模式
        /// </summary>
        private bool _isEditing;


        public BannerMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //页面加载后固定跨列的文本框的宽度
            this.txtBannerResourceUrl.Width = this.txtBannerResourceUrl.ActualWidth;
            this.txtBannerLink.Width = this.txtBannerLink.ActualWidth;
            this.txtBannerOnClick.Width = this.txtBannerOnClick.ActualWidth;

            this.rbInvalid.Content = EnumConverter.GetDescription(ADStatus.Deactive, typeof(ADStatus));
            this.rbValid.Content = EnumConverter.GetDescription(ADStatus.Active, typeof(ADStatus));
            this.lstBannerType.ItemsSource = EnumConverter.GetKeyValuePairs<BannerType>();

            //初始化页面数据
            this.lstChannel.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //初始化DataContext
            _isEditing = !string.IsNullOrWhiteSpace(this.Request.Param);
            if (_isEditing)
            {
                new BannerFacade(this).Load(this.Request.Param, (s, args) =>
                {

                    if (args.FaultsHandle()) return;

                    _currentVM = args.Result.Convert<BannerLocation, BannerLocationVM>();

                    _currentVM.Infos = args.Result.Infos.Convert<BannerInfo, BannerInfoVM>();
                    _currentVM.BannerDimension = args.Result.BannerDimension.Convert<BannerDimension, BannerDimensionVM>();
                    if (args.Result.WebChannel != null)
                    {
                        _currentVM.ChannelID = args.Result.WebChannel.ChannelID;
                    }

                    //绑定数据时不触发位置改变事件。
                    this.lstPosition.SelectionChanged -= new SelectionChangedEventHandler(lstPosition_SelectionChanged);
                    this.lstBannerType.SelectionChanged -= new SelectionChangedEventHandler(lstBannerType_SelectionChanged);


                    this.Grid.DataContext = _currentVM;

                    this.SetBannerTypeInfo();

                    this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
                    this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);

                    this.lstBannerType.SelectionChanged += new SelectionChangedEventHandler(lstBannerType_SelectionChanged);
                    this.lstPosition.SelectionChanged += new SelectionChangedEventHandler(lstPosition_SelectionChanged);

                });

            }
            else
            {
                _currentVM = new BannerLocationVM();
                this.Grid.DataContext = _currentVM;
                this.lstChannel.SelectedIndex = 0;
                this.lstPosition.SelectedIndex = 0;
                //创建时默认选中所有区域
                //this.ucDisplayArea.LoadAreaCompleted += (o, a) =>
                //{
                //    //创建时默认选中所有区域
                //    this.ucDisplayArea.SetAllAreaSelected();
                //};
                //this.ucDisplayArea.SetAllAreaSelected();
                this.ucPageType.ExtendC3Visibility = System.Windows.Visibility.Visible;
            }
        }

        void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
            this.ucPageType.SetPageID(_currentVM.PageID);   
        }

        void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            this.ucPageType.SetPageType(_currentVM.BannerDimension.PageTypeID);
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonSave.IsEnabled = true;
            _currentVM = new BannerLocationVM();
            this.Grid.DataContext = _currentVM;
            _isEditing = false;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //输入验证不通过，则直接返回
            if (!ValidationManager.Validate(this.Grid)) return;

            //BannerResourceurl可见时，必须输入值
            if (this.txtBannerResourceUrl.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(_currentVM.Infos.BannerResourceUrl))
                {
                    Window.Alert(ResBanner.Info_BannerResourceUrlRequired);
                    return;
                }
            }

            _currentVM.BannerDimension.PageTypeID = ucPageType.PageType;
            if (ucPageType.PageID == null || ucPageType.PageID == 0)
            {
                if (ucPageType.PageTypeInfo.Code == "0" || ucPageType.PageTypeInfo.Code == "10") //首页||万里通页面
                {
                    _currentVM.PageID = 0;
                }
                else
                {
                    _currentVM.PageID = -1;
                }

                //if (ucPageType.PageTypeInfo.Code == "8" || ucPageType.PageTypeInfo.Code == "7" || ucPageType.PageTypeInfo.Code == "30" || ucPageType.PageTypeInfo.Code == "43") //品牌首页||限时抢购||团购首页||团购活动详情页
                //{
                    
                //}
             }
            else
            {
                _currentVM.PageID = ucPageType.PageID;
            }
            
            _currentVM.IsExtendValid = ucPageType.IsExtendValid;
            _currentVM.ChannelID = "1";

            if (!_currentVM.BannerDimension.PageTypeID.HasValue)
            {
                Window.Alert("请选择页面类别。");
                return;
            }

            if (string.IsNullOrEmpty(_currentVM.BannerDimension.PositionID))
            {
                Window.Alert("请选择位置。");
                return;
            }

            //switch (this.ucPageType.PagePresentationType)
            //{
            //    case PageTypePresentationType.Category1:
            //    case PageTypePresentationType.Category2:
            //    case PageTypePresentationType.Category3:
            //    case PageTypePresentationType.Brand:
            //    case PageTypePresentationType.OtherSales:
            //    case PageTypePresentationType.Merchant:

            //        if (ucPageType.PageID == null)
            //        {
            //            Window.Alert("请选择页面ID。");
            //        }
            //        break;

            //}

            var facade = new BannerFacade(this);
            if (_isEditing)
            {
                facade.Update(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.Window.Alert(ResBanner.Info_EditSuccess);
                });
            }
            else
            {
                facade.Create(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.Window.Alert(ResBanner.Info_AddSuccess);
                    this.ButtonSave.IsEnabled = false;
                });
            }
        }

        //位置改变时，检查页面上的Banner位上已有的有效Banner数量
        private void lstPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = this.lstPosition.SelectedValue != null ? this.lstPosition.SelectedValue.ToString() : "";
            int bannerDimensionSysNo = 0;

            SetBannerTypeInfo();

            if (selectedValue.Length > 0 && int.TryParse(selectedValue, out bannerDimensionSysNo))
            {
                new BannerFacade(this).CountBannerPosition(ucPageType.PageID ?? 0, bannerDimensionSysNo, this.lstChannel.SelectedValue.ToString(), (s, args) =>
                    {
                        if (args.FaultsHandle()) return;

                        if (args.Result > 0)
                        {
                            this.Window.Alert(string.Format(ResBanner.Confirm_AlreadyExists, args.Result));
                        }
                    });
            }

           
        }

        private void SetBannerTypeInfo()
        {

            if (ucPageType.PositionCombox != null && ucPageType.PositionCombox.SelectedItem != null)
            {
                _currentVM.BannerDimension = ucPageType.PositionCombox.SelectedItem as BannerDimensionVM;
            }

            if (_currentVM.BannerDimension.PageTypeID == null)
            {
                _currentVM.BannerDimension.PageTypeID = ucPageType.PageType;
            }

            this.lblBannerLink.Visibility = System.Windows.Visibility.Collapsed;
            this.txtBannerLink.Visibility = System.Windows.Visibility.Collapsed;
            this.lblBannerResourceUrl.Visibility = System.Windows.Visibility.Collapsed;
            this.txtBannerResourceUrl.Visibility = System.Windows.Visibility.Collapsed;
            this.lblBannerResourceUrl2.Visibility = System.Windows.Visibility.Collapsed;
            this.txtBannerResourceUrl2.Visibility = System.Windows.Visibility.Collapsed;
            this.lblBannerText.Visibility = System.Windows.Visibility.Collapsed;
            this.txtBannerText.Visibility = System.Windows.Visibility.Collapsed;
            this.lblBannerResourceUrlMemo.Visibility = System.Windows.Visibility.Collapsed;
            this.lblBannerResourceUrl2Memo.Visibility = System.Windows.Visibility.Collapsed;

            this.imgBanerFrame.Visibility = System.Windows.Visibility.Collapsed;
            this.lstBannerFrame.Visibility = System.Windows.Visibility.Collapsed;
            this.lblBannerFrame.Visibility = System.Windows.Visibility.Collapsed;

            this.lblBannerResourceUrl.Text = "资源地址：";

            if (_currentVM.Infos.BannerType == BannerType.Image)
            {

                this.lblBannerResourceUrl.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerResourceUrl.Visibility = System.Windows.Visibility.Visible;
                this.lblBannerLink.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerLink.Visibility = System.Windows.Visibility.Visible;

                if (_currentVM.BannerDimension.PageTypeID == 0 && (_currentVM.BannerDimension.PositionID == "227" ||
                                                                   _currentVM.BannerDimension.PositionID == "221" || 
                                                                   _currentVM.BannerDimension.PositionID == "238"))
                {
                    this.lblBannerResourceUrl2.Visibility = System.Windows.Visibility.Visible;
                    this.txtBannerResourceUrl2.Visibility = System.Windows.Visibility.Visible;
                    this.lblBannerResourceUrlMemo.Visibility = System.Windows.Visibility.Visible;
                    this.lblBannerResourceUrl2Memo.Visibility = System.Windows.Visibility.Visible;
                    this.lblBannerResourceUrl.Text = "资源地址A：";
                    this.lblBannerResourceUrl2.Text = "资源地址B：";

                    if (_currentVM.BannerDimension.PositionID == "227")
                    {
                        this.lblBannerResourceUrlMemo.Text = "(默认广告图)";
                        this.lblBannerResourceUrl2Memo.Text = "(鼠标移入时广告图)";
                    }
                    else if (_currentVM.BannerDimension.PositionID == "221")
                    {
                        this.lblBannerResourceUrlMemo.Text = "(广告大图)";
                        this.lblBannerResourceUrl2Memo.Text = "(广告缩略图)";
                    }
                    else
                    {
                        this.lblBannerResourceUrlMemo.Text = "(宽屏图片800*147)";
                        this.lblBannerResourceUrl2Memo.Text = "(窄屏图片560*147)";
                    }
                }
                //改图片为上传控制,资源框只读，由上传成功填写
                this.txtBannerResourceUrl.IsEnabled = false;

            }
            else if (_currentVM.Infos.BannerType == BannerType.Flash || _currentVM.Infos.BannerType == BannerType.Video)
            {
                this.lblBannerLink.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerLink.Visibility = System.Windows.Visibility.Visible;
                this.lblBannerResourceUrl.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerResourceUrl.Visibility = System.Windows.Visibility.Visible;
            }
            else if (_currentVM.Infos.BannerType == BannerType.HTML)
            {
                this.lblBannerLink.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerLink.Visibility = System.Windows.Visibility.Visible;
                this.lblBannerText.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerText.Visibility = System.Windows.Visibility.Visible;

                if (_currentVM.BannerDimension.PageTypeID == 0 && _currentVM.BannerDimension.PositionID == "228")
                {
                    new BannerFacade(this).GetBannerFrame(0, 228, (s, args) =>
                      {
                          this.lstBannerFrame.ItemsSource = args.Result;
                          if (_currentVM.Infos.BannerFrameSysNo.HasValue)
                          {
                              this.lstBannerFrame.SelectedValue = _currentVM.Infos.BannerFrameSysNo;
                          }
                          else
                          {
                              this.lstBannerFrame.SelectedIndex = 0;
                          }
                      });
                    this.imgBanerFrame.Visibility = System.Windows.Visibility.Visible;
                    this.lstBannerFrame.Visibility = System.Windows.Visibility.Visible;
                    this.lblBannerFrame.Visibility = System.Windows.Visibility.Visible;
                }

            }
            else if (_currentVM.Infos.BannerType == BannerType.Text)
            {
                this.lblBannerText.Visibility = System.Windows.Visibility.Visible;
                this.txtBannerText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void ucPageType_PageTypeSelectionChanged(object sender, UserControls.PageTypeSelectionChangedEventArgs e)
        {             
            SetBannerTypeInfo();
        }

        private void lstBannerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetBannerTypeInfo();
        }

        private void lstBannerFrame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null) return;
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem != null)
            {
                ECCentral.BizEntity.MKT.BannerFrame bf = cb.SelectedItem as ECCentral.BizEntity.MKT.BannerFrame;
                this.imgBanerFrame.Source = new System.Windows.Media.Imaging.BitmapImage(BanerFramePathOnServer(bf.BannerFrameView));
                this.txtBannerText.Text = bf.BannerFrameText;
            }
        }

        private Uri BanerFramePathOnServer(string url)
        {
            url = GetImageSrc(url).Replace("../images", "WebResources/images");

            if (url.Length > 0 && url[0] == '/')
            {
                url = url.Substring(1);
            }

            Uri imageServicePath = Application.Current.Host.Source;
            Uri outPath;

            if (Uri.TryCreate(imageServicePath, url, out outPath))
            {
                return outPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 读取Img标签的SRC
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetImageSrc(string str)
        {
            string regStr = @"<img[^>]*src=[""']?([^""'\s]+)[""']?[^>]*>";
            string cont1 = string.Empty; //图片的src
            Regex reg = new Regex(regStr, RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            string picSrc = "";
            if(match.Success)
            {
                picSrc = match.Groups[1].Value;              
            }
            return picSrc;
        }
        private void btnAddPic_Click(object sender, RoutedEventArgs e)
        {

            UCUnifiedImageUpload upWindow = new UCUnifiedImageUpload(ConstValue.DomainName_MKT);
            upWindow.AppName = "mkt";

            upWindow.UploadUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_MKT, "ImageBaseUrl");

            upWindow.Dialog = Window.ShowDialog("上传广告图片", upWindow, (obj, args) =>
            {
                if (args.Data != null)
                {
                    txtBannerResourceUrl.Text = ((dynamic)args.Data).ImageUrl;
                }

            }, new Size(600, 300));


        }
    }

}
