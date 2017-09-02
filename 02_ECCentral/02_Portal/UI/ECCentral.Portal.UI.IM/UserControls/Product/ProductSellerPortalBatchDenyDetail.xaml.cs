using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductSellerPortalBatchDenyDetail : UserControl
    {

        #region 属性

        public IDialog Dialog { get; set; }


        public List<dynamic> SelectRows { get; set; }

        private SellerProductRequestQueryFacade _facade;
        
        #endregion

        #region 初始化加载

        public ProductSellerPortalBatchDenyDetail()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
          
        }


        #endregion



        #region 按钮事件

        private void btnDeny_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tb_Reason.Text))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("必须输入退回理由.", MessageBoxType.Warning);
                return;
            }

            var auditList = (from c in SelectRows
                             select
                                 new SellerProductRequestVM
                                 {
                                     SysNo = c.SysNo,
                                     RequestSysno=c.RequestSysno,
                                     Status = c.Status,
                                     ProductName = c.ProductName,
                                     Memo = this.tb_Reason.Text
                                 }).ToList();

            _facade = new SellerProductRequestQueryFacade();

            _facade.BatchDenyProductRequest(auditList, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }

                CloseDialog(DialogResultType.OK);
            });
        }

      

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        #endregion

    }
}
