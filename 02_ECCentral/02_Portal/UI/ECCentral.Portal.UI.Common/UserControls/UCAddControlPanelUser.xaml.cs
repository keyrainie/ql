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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Common.Facades;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class UCAddControlPanelUser : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }
        public int? _sysNo;

        private ControlPanelUserVM VM;
        private ControlPanelUserFacade facade;

        public UCAddControlPanelUser(int? sysNo)
        {
            InitializeComponent();
            if (sysNo.HasValue && sysNo.Value > 0)
            {
                _sysNo = sysNo;
            }
            Loaded += new RoutedEventHandler(UCAddControlPanelUser_Loaded);
        }

        void UCAddControlPanelUser_Loaded(object sender, RoutedEventArgs e)
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common
              , new string[] { ConstValue.Key_ControlPanelDept }
              , CodeNamePairAppendItemType.Select, (o, p) =>
              {
                  comDept.ItemsSource = p.Result[ConstValue.Key_ControlPanelDept];
                  comDept.SelectedIndex = 0;

                  facade = new ControlPanelUserFacade(CPApplication.Current.CurrentPage);
                  if (_sysNo.HasValue)
                  {
                      facade.GetUserBySysNo(_sysNo, (obj, args) =>
                      {
                          VM = args.Result;
                          VM.IsEdit = true;
                          LayoutRoot.DataContext = VM;

                      });
                  }
                  else
                  {
                      VM = new ControlPanelUserVM() { IsEdit = false };
                      LayoutRoot.DataContext = VM;
                      comStatus.SelectedIndex = 0;
                  }

              });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (string.IsNullOrEmpty(VM.SourceDirectory))
                {
                    Message(string.Format("用户:{0} 无授权信息", VM.LoginName));
                    return;
                }
                VM.CompanyCode = CPApplication.Current.CompanyCode;
                if (!_sysNo.HasValue)
                {
                    VM.InUser = CPApplication.Current.LoginUser.UserSysNo.ToString();
                    facade.CreateUser(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResControlPanelUser.Info_SaveSuccessfully);
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
                else
                {
                    VM.EditUser = CPApplication.Current.CompanyCode;
                    facade.UpdateUser(VM, (s, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Message(ResControlPanelUser.Info_SaveSuccessfully);
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    });
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }

        #region 辅助方法
        private void Message(string msg)
        {
            Page.Context.Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
        #endregion

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FindUserSource();
        }

        private void FindUserSource()
        {
            string tmpStr = textBoxLoginName.Text;
            if (!VM.IsEdit && !string.IsNullOrEmpty(tmpStr))
            {
                facade.GetControlPanelUserByLoginName(tmpStr, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (!string.IsNullOrEmpty(args.Result.PhysicalUserId))
                    {
                        args.Result.IsEdit = true;
                    }
                    else
                    {
                        Message("没有找到授权数据");
                    }
                    VM = args.Result;
                    LayoutRoot.DataContext = VM;
                    comStatus.SelectedIndex = 0;
                });
            }
        }

        private void textBoxLoginName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FindUserSource();
            }
        }

    }
}
