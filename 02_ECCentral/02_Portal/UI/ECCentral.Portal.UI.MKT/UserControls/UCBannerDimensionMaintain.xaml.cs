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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCBannerDimensionMaintain : UserControl
    {
        private BannerDimensionVM _currentVM;
        /// <summary>
        /// 标识界面是否处于创建新记录模式
        /// </summary>
        private bool _isEditing;

        /// <summary>
        /// 当前打开的Tab页面
        /// </summary>
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IDialog DialogHandle { get; set; }

        public UCBannerDimensionMaintain(int currentSysNo)
        {
            InitializeComponent();

            this.lstChannelList.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();

            if (currentSysNo > 0)
            {
                //当前界面正在编辑模式,加载数据
                _isEditing = true;
                var bannerDimensionFacade = new BannerDimensionFacade(CPApplication.Current.CurrentPage);
                bannerDimensionFacade.Load(currentSysNo, (s, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        _currentVM = args.Result.Convert<BannerDimension, BannerDimensionVM>((info, vm) =>
                        {
                            if (info.WebChannel != null)
                            {
                                vm.ChannelID = info.WebChannel.ChannelID;
                            }
                        });
                        this.DataContext = _currentVM;
                    });

            }
            else
            {
                _isEditing = false;
                _currentVM = new BannerDimensionVM();
                this.DataContext = _currentVM;
                this.lstChannelList.SelectedIndex = 0;
            }
        }

        private void lstChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstChannelList.SelectedValue == null) return;

            //根据渠道的变化，动态加载页面类型
            var pageTypeFacade = new PageTypeFacade(CPApplication.Current.CurrentPage);
            pageTypeFacade.GetPageTypes(CPApplication.Current.CompanyCode, this.lstChannelList.SelectedValue.ToString(), (int)ModuleType.Banner, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                this.lstPageTypeList.ItemsSource = args.Result;
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //输入验证不通过，则直接返回
            if (!ValidationManager.Validate(this)) return;

            var bannerDimensionFacade = new BannerDimensionFacade(CPApplication.Current.CurrentPage);
            if (_isEditing)
            {
                //编辑
                bannerDimensionFacade.Update(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResBannerDimension.Info_EditSuccess);
                    CloseDialog(true);
                });
            }
            else
            {
                //新建
                bannerDimensionFacade.Create(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResBannerDimension.Info_AddSuccess);
                    CloseDialog(true);
                });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(false);
        }

        private void CloseDialog(bool refreshParentDataGrid)
        {
            if (DialogHandle != null)
            {
                DialogHandle.ResultArgs = new ResultEventArgs
                {
                    Data = refreshParentDataGrid
                };
                DialogHandle.Close();
            }
        }
    }
}
