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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryRequestApprovalMaintain : UserControl
    {
        private CategoryRequestApprovalFacade facade;
        /// <summary>
        /// 类别
        /// </summary>
        public CategoryType Category { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OperationType? ActionType { get; set; }
        public IDialog Dialog { get; set; }
        public CategoryRequestApprovalVM Data
        {
            get;
            set;
        }
        public CategoryRequestApprovalMaintain()
        {
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                if (ActionType == OperationType.Update)
                {
                    this.spCategory.Visibility = Visibility.Visible;
                }

                switch (Category)
                {
                    case CategoryType.CategoryType1:
                        this.spCategory1.Visibility = Visibility.Visible;
                        this.sptemp1.Visibility = Visibility.Visible;
                        break;
                    case CategoryType.CategoryType2:
                        this.spCategory1.Visibility = Visibility.Visible;
                        this.spCategory2.Visibility = Visibility.Visible;
                        this.sptemp1.Visibility = Visibility.Visible;
                        this.sptemp2.Visibility = Visibility.Visible;
                        break;
                    case CategoryType.CategoryType3:
                        this.spCategory1.Visibility = Visibility.Visible;
                        this.spCategory2.Visibility = Visibility.Visible;
                        this.spCategory3.Visibility = Visibility.Visible;
                        this.sptemp1.Visibility = Visibility.Visible;
                        this.sptemp2.Visibility = Visibility.Visible;
                        this.sptemp3.Visibility = Visibility.Visible;
                        this.sptempC3Code.Visibility = Visibility.Visible;
                        this.spC3Code.Visibility = Visibility.Visible;
                        break;
                    default:
                        this.sptemp1.Visibility = Visibility.Visible;
                        this.sptemp2.Visibility = Visibility.Visible;
                        this.sptemp3.Visibility = Visibility.Visible;
                        this.spCategory1.Visibility = Visibility.Visible;
                        this.spCategory2.Visibility = Visibility.Visible;
                        this.spCategory3.Visibility = Visibility.Visible;
                        break;
                }
                this.DataContext = Data;
                facade = new CategoryRequestApprovalFacade();
            };
        }




        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Approve_Click(object sender, RoutedEventArgs e)
        {
            CategoryRequestApprovalVM model = this.DataContext as CategoryRequestApprovalVM;
            model.SysNo = Data.SysNo;
            model.CategoryID = Data.CategoryID;
            model.CategorySysNo = Data.CategorySysNo;
            model.C3Code = Data.C3Code;
            facade.CategoryRequestAuditPass(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;

                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Deny_Click(object sender, RoutedEventArgs e)
        {
            CategoryRequestApprovalVM model = this.DataContext as CategoryRequestApprovalVM;
            model.SysNo = Data.SysNo;
            model.CategoryID = Data.CategoryID;
            model.CategorySysNo = Data.CategorySysNo;
            facade.CategoryRequestAuditNotPass(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;

                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }
        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            CategoryRequestApprovalVM model = this.DataContext as CategoryRequestApprovalVM;
            model.SysNo = Data.SysNo;
            model.CategoryID = Data.CategoryID;
            model.CategorySysNo = Data.CategorySysNo;
            facade.CategoryRequestCanel(model, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;

                }
                CloseDialog(DialogResultType.OK);
                CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功");
            });
        }
    }
}

