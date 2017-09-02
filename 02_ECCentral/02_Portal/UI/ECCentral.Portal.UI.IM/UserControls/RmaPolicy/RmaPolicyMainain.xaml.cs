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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class RmaPolicyMainain : UserControl
    {
        private RmaPolicyVM VM { get; set; }
       /// <summary>
       /// 操作类型
       /// </summary>
        public RmaAction Action { get; set; }
        public int Sysno { get; set; }
        private RmaPolicyFacade facade;
        public IDialog Dialog { get; set; }
        public RmaPolicyMainain()
        {
            InitializeComponent();
            this.Loaded += RmaPolicyMainain_Loaded;
        }

        void RmaPolicyMainain_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new RmaPolicyFacade(CPApplication.Current.CurrentPage);
            BindPage();
           
        }
        private void BindPage() 
        {
            if (Action == RmaAction.Create)
            {
                VM = new RmaPolicyVM();
                this.DataContext = VM;
            }
            if (Action == RmaAction.Edit || Action == RmaAction.Details)
            {
                facade.QueryRmaPolicyBySysNo(Sysno, (obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    if (arg.Result == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("该退换货信息不存在!", MessageBoxType.Error);
                        return;
                    }
                    VM = new RmaPolicyVM()
                    {
                        ChangeDate = arg.Result.ChangeDate.ToString(),
                        ECDisplayDesc = arg.Result.ECDisplayDesc,
                        ECDisplayMoreDesc = arg.Result.ECDisplayMoreDesc,
                        ECDisplayName = arg.Result.ECDisplayName,
                        Priority = arg.Result.Priority.ToString(),
                        ReturnDate = arg.Result.ReturnDate.ToString(),
                        RMAPolicyName = arg.Result.RMAPolicyName,
                        RmaType = arg.Result.RmaType,
                        IsRequest = arg.Result.IsOnlineRequest ==IsOnlineRequst.YES?true:false
                    };
                    this.DataContext = VM;
                });
                if (Action == RmaAction.Details)
                {
                    this.IsEnabled = false;
                    spAction.Visibility = Visibility.Collapsed;
                }
            }
           
        }

        //private void btnPreview_Click_1(object sender, RoutedEventArgs e)
        //{
            
        //}

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
             if (Action == RmaAction.Create)
            {
                facade.CreateRmaPolicy(VM, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
                });
            }
             if (Action == RmaAction.Edit)
             {
                 VM.SysNo = Sysno;
                 facade.UpdateRmaPolicy(VM, (obj, arg) =>
                 {
                     if (arg.FaultsHandle())
                     {
                         return;
                     }
                     CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功!", MessageBoxType.Success);
                 });
             }
        }

        private void btnCanel_Click_1(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void btnSearchLog_Click_1(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.IM_RmaPolicyLogManagementUrlFormat, Sysno), null, true);
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }

    /// <summary>
    ///操作类型
    /// </summary>
    public enum RmaAction {
    Edit=0,
     Create=1,
        Details
    }
}
