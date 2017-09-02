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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCHomePageSectionMaintain : UserControl
    {
        private bool _isEditing = false;
        private int _editingSysNo;
        private HomePageSectionMaintainVM _maintainVM;
        /// <summary>
        /// 应用Window.ShowDialog弹出时，会返回一个DialogHandle,根据这个DialogHandle可以做关闭对话框等操作
        /// </summary>
        public IDialog DialogHandle { get; set; }


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

        public UCHomePageSectionMaintain()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCHomePageSectionMaintain_Loaded);
        }

        void UCHomePageSectionMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                var maintainFacade = new HomePageSectionFacade(CPApplication.Current.CurrentPage);
                maintainFacade.Load(_editingSysNo.ToString(), (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    _maintainVM = args.Result.Convert<HomePageSectionInfo, HomePageSectionMaintainVM>((info, vm) =>
                    {
                        if (info.WebChannel != null)
                        {
                            vm.ChannelID = info.WebChannel.ChannelID;
                        }
                    });
                    this.DataContext = _maintainVM;
                    LoadComboxData();
                });
            }
            else
            {
                _maintainVM = new HomePageSectionMaintainVM();
                this.DataContext = _maintainVM;
                LoadComboxData();
            }
        }

        private void LoadComboxData()
        {
            this.lstChannel.ItemsSource = CPApplication.Current.CurrentWebChannelList;
            if (!_isEditing)
            {
                this.lstChannel.SelectedIndex = 0;
            }
        }

        public void BeginEditing(int sysNo)
        {
            _isEditing = true;
            _editingSysNo = sysNo;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            if (_maintainVM.C1List.Contains("，"))
            {
                CurrentWindow.Alert(@"不能包含中文""，""",MessageType.Error);
                return;
            }
            var maintainFacade = new HomePageSectionFacade(CPApplication.Current.CurrentPage);
            if (_isEditing)
            {
                maintainFacade.Update(_maintainVM, (s, args) =>
                {
                    if (args.FaultsHandle()) return;
                    CurrentWindow.Alert(ResHomePageSection.Info_EditSuccess);
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                maintainFacade.Create(_maintainVM, (s, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        CurrentWindow.Alert(ResHomePageSection.Info_AddSuccess);
                        CloseDialog(DialogResultType.OK);
                    });
            }
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (DialogHandle != null)
            {
                DialogHandle.ResultArgs.DialogResult = dialogResult;
                DialogHandle.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}
