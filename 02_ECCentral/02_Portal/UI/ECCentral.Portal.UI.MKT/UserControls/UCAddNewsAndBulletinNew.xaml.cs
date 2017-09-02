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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ECCentral.BizEntity.Customer;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using ECCentral.BizEntity;
using ECCentral.Portal.Basic;
namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCAddNewsAndBulletinNew : UserControl
    {
        /// <summary>
        ///编辑时传入的新闻公告对象
        /// </summary>

        NewsInfoMaintainVM viewModel;
        private NewsFacade facade;
        public NewsInfo entity;
        public IDialog dialog;
        public bool LoadCompleted = false;

        public IWindow Window { get; set; }

        public UCAddNewsAndBulletinNew(IWindow _Window)
        {
            InitializeComponent();
            entity = new NewsInfo();
            viewModel = new NewsInfoMaintainVM();
            this.DataContext = viewModel;
            facade = new NewsFacade(CPApplication.Current.CurrentPage);
            this.ucPosition.PageTypeSelectionChanged += new EventHandler<PageTypeSelectionChangedEventArgs>(ucPosition_PageTypeSelectionChanged);
            this.ucPosition.SetDefaultCategoryClick += new EventHandler<RoutedEventArgs>(ucPosition_SetDefaultCategoryClick);
            this.ucPosition.PageTypeLoadCompleted += new EventHandler(ucPosition_PageTypeLoadCompleted);
            this.ucPosition.PageLoadCompleted += new EventHandler(ucPosition_PageLoadCompleted);
            this.Window = _Window;
        }


        void ucPosition_SetDefaultCategoryClick(object sender, RoutedEventArgs e)
        {
            tbContainPageID.Visibility = ucPosition.IsSetDefaultCategory ? Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        void ucPosition_PageTypeSelectionChanged(object sender, PageTypeSelectionChangedEventArgs e)
        {
            if (ucPosition.PageType.HasValue)
            {
                PageTypePresentationType type = PageTypeUtil.ResolvePresentationType(ModuleType.NewsAndBulletin, ucPosition.PageType.Value.ToString());
                this.ckbIsHomePageShow.Visibility = (type == PageTypePresentationType.Category1 || type == PageTypePresentationType.Category2 || type == PageTypePresentationType.Category3) ? Visibility.Visible : Visibility.Collapsed;
                this.ckbIsC1Show.Visibility = (type == PageTypePresentationType.Category2 || type == PageTypePresentationType.Category3) ? Visibility.Visible : Visibility.Collapsed;
                this.ckbIsC2Show.Visibility = (type == PageTypePresentationType.Category3) ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.btnUpdate.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BindPage()
        {
            if (entity.SysNo != null)
            {
                facade.GetNewsInfo(entity.SysNo.Value, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    //  viewModel = args.Result.Convert<NewsInfo, NewsInfoMaintainVM>();
                    viewModel.SysNo = args.Result.SysNo;
                    viewModel.ChannelID = args.Result.WebChannel.ChannelID;
                    viewModel.NewsType = args.Result.NewsType;
                    viewModel.ReferenceSysNo = args.Result.ReferenceSysNo;
                    viewModel.Title = new Regex("<(.[^>]*)>", RegexOptions.IgnoreCase).Replace(args.Result.Title.Content, "");
                    viewModel.LinkUrl = args.Result.LinkUrl;
                    viewModel.ExpireDate = args.Result.ExpireDate.ToString();
                    viewModel.Status = args.Result.Status;
                    viewModel.EnableReplyRank = args.Result.EnableReplyRank;
                    viewModel.TopMost = args.Result.TopMost.Value;
                    viewModel.EnableComment = args.Result.EnableComment.Value;
                    viewModel.IsRed = args.Result.IsRed.Value;
                    viewModel.Content = args.Result.Content.Content;
                    viewModel.AreaShow = args.Result.AreaShow;
                    viewModel.IsHomePageShow = args.Result.IsHomePageShow.HasValue ? args.Result.IsHomePageShow.Value : false;
                    viewModel.IsC1Show = args.Result.IsC1Show.HasValue ? args.Result.IsC1Show.Value : false;
                    viewModel.IsC2Show = args.Result.IsC2Show.HasValue ? args.Result.IsC2Show.Value : false;
                    viewModel.ContainPageId = args.Result.ContainPageId;
                    viewModel.CoverImageUrl = args.Result.CoverImageUrl;
                    viewModel.Subtitle = args.Result.Subtitle;
                    this.ucPosition.SetPageType(viewModel.NewsType);
                    if (!string.IsNullOrEmpty(viewModel.ContainPageId))
                    {
                        this.ucPosition.cbSetDefault.IsChecked = true;
                        this.ucPosition.ucCategoryPicker.Visibility = System.Windows.Visibility.Collapsed;
                        tbContainPageID.Visibility = System.Windows.Visibility.Visible;
                    }

                    if (viewModel.SysNo > 0)
                    {
                        this.btnUpdate.Visibility = System.Windows.Visibility.Visible;
                    }
                });
            }
            else
            {
                //this.ucDisplayArea.LoadAreaCompleted += (o, a) =>
                //{
                //    //创建时默认选中所有区域
                //    this.ucDisplayArea.SetAllAreaSelected();
                //};

                this.viewModel.EnableReplyRank = (CustomerRank)0;
                viewModel.Status = NewsStatus.Active;
            }

            lstChannel.SelectedIndex = 0;
        }

        private void ClearForm()
        {
            //this.ucDisplayArea.SetAllAreaSelected();
            this.viewModel.Title = "";
            this.viewModel.LinkUrl = "";
            this.viewModel.Content = "";
            this.viewModel.ExpireDate = "";
            this.viewModel.Status = NewsStatus.Active;
            this.viewModel.TopMost = false;
            this.viewModel.IsRed = false;
            this.viewModel.EnableReplyRank = (CustomerRank)0;
            this.viewModel.EnableComment = false;

        }

        void ucPosition_PageLoadCompleted(object sender, EventArgs e)
        {
            // if (viewModel.ReferenceSysNo != null && isPageTypeLoadCompleted)
            //this.ucPosition.SetPageID(viewModel.ReferenceSysNo);
            if (!string.IsNullOrEmpty(viewModel.ContainPageId))
            {
                this.ucPosition.cbSetDefault.IsChecked = true;
                this.ucPosition.ucCategoryPicker.Visibility = System.Windows.Visibility.Collapsed;
                tbContainPageID.Visibility = System.Windows.Visibility.Visible;
            }
            if (viewModel.SysNo == null)
            {
                BindPage();
            }
            if (viewModel.ReferenceSysNo!=null)
            this.ucPosition.SetPageID(viewModel.ReferenceSysNo);
           
        }

        void ucPosition_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            BindPage();
        }


        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if(!ValidationManager.Validate(this.LayoutRoot))
            {
                return;
            }           
            if (ucPosition.PageType == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("类型不能为空!", MessageType.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.LinkUrl) && string.IsNullOrWhiteSpace(viewModel.Content))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("正文链接和正文内容不可都为空!", MessageType.Warning);
                return;
            }
            entity = viewModel.ConvertVM<NewsInfoMaintainVM, NewsInfo>();
            entity.Title.Content = viewModel.Title;
            entity.Content.Content = viewModel.Content;
            entity.NewsType = ucPosition.PageType;
            entity.ReferenceSysNo = ucPosition.PageID ?? -1;
            entity.Extendflag = ucPosition.IsExtendValid;
            entity.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = viewModel.ChannelID };
            entity.CoverImageUrl = viewModel.CoverImageUrl;
            entity.Subtitle = viewModel.Subtitle;
            entity.SysNo = 0;
            facade.Create(entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功!");
                dialog.ResultArgs.DialogResult = DialogResultType.OK;
                dialog.Close();
            });

        }

        //添加广告封面
        private void btnAddPic_Click(object sender, RoutedEventArgs e)
        {

            UCUnifiedImageUpload upWindow = new UCUnifiedImageUpload(ConstValue.DomainName_MKT);
            upWindow.AppName = "mkt";
            upWindow.UploadUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_MKT, "ImageBaseUrl");

            upWindow.Dialog = Window.ShowDialog("上传广告图片", upWindow, (obj, args) =>
            {
                if (args.Data != null)
                {
                    CoverImageResourceUrl.Text = ((dynamic)args.Data).ImageUrl;
                }

            }, new Size(600, 300));

        }

        /// <summary>
        /// 前台预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreviewFront_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModel.Content))
            {
                HtmlViewHelper.ViewHtmlInBrowser("MKT", viewModel.Content);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.LayoutRoot);
            if (viewModel.HasValidationErrors)
            {
                return;
            }

            if (ucPosition.PageType == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("类型不能为空。", MessageBoxType.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.LinkUrl) && string.IsNullOrWhiteSpace(viewModel.Content))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("正文链接和正文内容不可都为空。", MessageBoxType.Warning);
                return;
            }
            entity = viewModel.ConvertVM<NewsInfoMaintainVM, NewsInfo>();
            entity.Title = new LanguageContent(viewModel.Title);
            entity.Content = new LanguageContent(viewModel.Content);
            entity.NewsType = ucPosition.PageType;
            entity.ReferenceSysNo = ucPosition.PageID ?? -1;
            entity.Extendflag = ucPosition.IsExtendValid;
            entity.WebChannel = new BizEntity.Common.WebChannel() { ChannelID = viewModel.ChannelID };
            entity.CoverImageUrl = viewModel.CoverImageUrl;
            entity.Subtitle = viewModel.Subtitle;
            if (viewModel.SysNo != null && viewModel.SysNo.Value > 0)
            {
                facade.Update(entity, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功!");
                    dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    dialog.Close();
                });

            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        public void ucPosition_PageTypeSelectionChanged(PageTypeSelectionChangedEventArgs e)
        {
            
        }
    }
}
