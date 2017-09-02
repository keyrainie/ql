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
using ECCentral.Portal.UI.MKT.Models.PageType;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT.PageType;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCPageTypeMaintain : UserControl
    {
        private PageTypeVM _currentVM;
        /// <summary>
        /// 标识界面是否处于编辑记录模式
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

        public UCPageTypeMaintain(int currentSysNo)
        {
            InitializeComponent();

            if (currentSysNo > 0)
            {
                //当前界面正在编辑模式,加载数据
                _isEditing = true;
                var bannerDimensionFacade = new PageTypeFacade(CPApplication.Current.CurrentPage);
                bannerDimensionFacade.Load(currentSysNo, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    _currentVM = args.Result.Convert<PageType, PageTypeVM>();
                    _currentVM.ChannelID = "1";
                    _currentVM.Status = ADStatus.Active;
                    this.DataContext = _currentVM;
                });

            }
            else
            {
                _isEditing = false;
                _currentVM = new PageTypeVM();
                _currentVM.Status = ADStatus.Active;
                _currentVM.ChannelID = "1";
                this.DataContext = _currentVM;
              
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //输入验证不通过，则直接返回
            if (!ValidationManager.Validate(this.LayoutRoot)) return;

            var bannerDimensionFacade = new PageTypeFacade(CPApplication.Current.CurrentPage);
            if (_isEditing)
            {
                //编辑
                bannerDimensionFacade.Update(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResPageType.Info_EditSuccess);
                    CloseDialog(true);
                });
            }
            else
            {
                //新建
                bannerDimensionFacade.Create(_currentVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResPageType.Info_AddSuccess);
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
