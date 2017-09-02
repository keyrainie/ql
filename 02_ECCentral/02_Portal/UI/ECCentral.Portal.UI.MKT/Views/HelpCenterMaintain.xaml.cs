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
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Resources;
using System.Windows.Browser;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class HelpCenterMaintain : PageBase
    {
        private bool _isEditing;
        public HelpCenterMaintain()
        {
            InitializeComponent();
            
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //页面加载后固定跨列的文本框的宽度
            this.txtTitle.Width = this.txtTitle.ActualWidth;
            this.txtDescription.Width = this.txtDescription.ActualWidth;
            this.txtContent.Width = this.txtContent.ActualWidth;
            this.txtKeywords.Width = this.txtKeywords.ActualWidth;

            LoadComboxData();
            if (string.IsNullOrEmpty(this.Request.Param))
            {
                this.DataContext = new HelpCenterVM();
                _isEditing = false;
                this.Title = ResHelpCenter.PageTitle_Add;
                this.lstChannel.SelectedIndex = 0;
            }
            else
            {
                _isEditing = true;
                this.Title = ResHelpCenter.PageTitle_Edit;
                new HelpCenterFacade(this).Load(this.Request.Param, (s, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        this.DataContext = args.Result.Convert<HelpTopic, HelpCenterVM>((info, vm) =>
                        {
                            if (info.WebChannel != null)
                            {
                                vm.ChannelID = info.WebChannel.ChannelID;
                            }
                        });
                    });
            }
        }

        private void LoadComboxData()
        {
            this.rbDeactive.Content = EnumConverter.GetDescription(ADStatus.Deactive, typeof(ADStatus));
            this.rbActive.Content = EnumConverter.GetDescription(ADStatus.Active, typeof(ADStatus));
            this.rbNone.Content = ResHelpCenter.Label_None;
            this.rbNew.Content = EnumConverter.GetDescription(FeatureType.New, typeof(FeatureType));
            this.rbHot.Content = EnumConverter.GetDescription(FeatureType.Hot, typeof(FeatureType));
            this.lstChannel.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.lstChannel.SelectedIndex = 0;
        }

        void lstChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstChannel.SelectedValue == null) return;
            //如果已经加载了对应渠道的帮助分类列表，直接返回
            if (this.lstHelpCategory.Tag == this.lstChannel.SelectedValue) return;
            HelpCenterFacade helpCenterFacade = new HelpCenterFacade(this);
            helpCenterFacade.QueryCategory(CPApplication.Current.CompanyCode, this.lstChannel.SelectedValue.ToString(), (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<HelpCenterCategoryVM> helpCategoryList = DynamicConverter<HelpCenterCategoryVM>.ConvertToVMList(args.Result.Rows);
                if (helpCategoryList != null)
                {
                    helpCategoryList.Insert(0, new HelpCenterCategoryVM { SysNo = null, Name = ResCommonEnum.Enum_Select });
                }
                this.lstHelpCategory.ItemsSource = helpCategoryList;
                this.lstHelpCategory.Tag = this.lstChannel.SelectedValue;
            });
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
            this.Title = ResHelpCenter.PageTitle_Add;
            this.ButtonSave.IsEnabled = true;
            this.DataContext = new HelpCenterVM();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;

            var viewModel = this.DataContext as HelpCenterVM;
            if (_isEditing)
            {
                new HelpCenterFacade(this).Update(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    Window.Alert(ResHelpCenter.Info_EditSuccess);
                });
            }
            else
            {
                new HelpCenterFacade(this).Create(viewModel, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    Window.Alert(ResHelpCenter.Info_AddSuccess);
                    this.ButtonSave.IsEnabled = false;
                });
            }
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as HelpCenterVM;
            HtmlViewHelper.PreviewPageShow("PreviewPageShow", viewModel.Content);
        }
    }

}
