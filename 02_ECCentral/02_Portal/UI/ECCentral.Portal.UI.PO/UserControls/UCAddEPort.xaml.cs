using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models.EPort;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCAddEPort : UserControl
    {
        public int? _sysNo;
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        private EPortVM VM=new EPortVM();
        private EPortFacade facade;

        public UCAddEPort(int? sysNo)
        {
            InitializeComponent();
            if (sysNo.HasValue && sysNo > 0)
            {
                _sysNo = sysNo;
            }
            Loaded += new RoutedEventHandler(UCAddEPort_Loaded);
        }
        #region 事件
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(ValidationManager.Validate(this.LayoutRoot))
            {
                if (!_sysNo.HasValue)
                {
                    VM.InUser = CPApplication.Current.LoginUser.UserSysNo;
                    facade.CreatEPort (VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message("保存成功！");
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
                else
                {
                    VM.LastEditUser = CPApplication.Current.LoginUser.UserSysNo;
                    facade.UpdateEPortr(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message("更新成功！");
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UCAddEPort_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new EPortFacade(CPApplication.Current.CurrentPage);
            if (_sysNo.HasValue)
            {
                facade.GetEPortEntity(_sysNo, (obj, args) =>
                {
                    VM = args.Result;
                    LayoutRoot.DataContext = VM;

                });
            }
            else
            {
                VM = new EPortVM();
                LayoutRoot.DataContext = VM;
                comStatus.SelectedIndex = 0;
            }
        }
        #endregion

        #region 私有方法
        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
        private void Message(string msg)
        {
            CurrentWindow.Alert(msg);
        }
        #endregion

    }
}
