using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.UI.RMA.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class RefundBalanceQuery : PageBase
    {
        RefundBalanceFacade facade;
        List<ValidationEntity> validationCondition;
        private List<RefundBalanceVM> list;
        public RefundBalanceQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new RefundBalanceFacade(this);
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                Text_RefundSysNo.Text = this.Request.Param;
            }
            BuildValidateCondition();
            this.DataGrid_ResultList.Bind();
            base.OnPageLoad(sender, e);
            Button_Create.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RefundBalance_Create) ;
            Button_Aduit.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RefundBalance_SubmitAudit) ;
            Button_Refund.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RefundBalance_Refund);
            Button_Void.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_RefundBalance_Abandon) ;

        }
        private void BuildValidateCondition()
        {
            validationCondition = new List<ValidationEntity>();
            validationCondition.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.Text_RefundSysNo.Text.Trim(), ResRefundBalance.Msg_NoQueryCondition));
            validationCondition.Add(new ValidationEntity(ValidationEnum.IsInteger, this.Text_RefundSysNo.Text.Trim(), ResRefundBalance.Msg_IsInteger));
        }
        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (!ValidateCondition())
            {
                return;
            }
            int refundSysNo = int.Parse(this.Request.Param.Trim());
            facade.Query(refundSysNo, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                list = DynamicConverter<RefundBalanceVM>.ConvertToVMList(args.Result.Rows);
                this.DataGrid_ResultList.ItemsSource = list;
                this.DataGrid_ResultList.TotalCount = args.Result.TotalCount;
            });

        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCondition())
            {
                return;
            }
            this.DataGrid_ResultList.Bind();
            //string url = string.Format(ConstValue.RMA_RefundBalanceQueryUrl, Text_RefundSysNo.Text.Trim());
            //Window.Navigate(url, null, false);

        }
        private bool ValidateCondition()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.Text_RefundSysNo, validationCondition))
            {
                ret = false;
            }
            return ret;
        }

        private bool ValidateSelectRow()
        {
            bool ret = true;
            if (list == null)
            {
                ret = false;
            }
            else
            {
                var selectedList = list.Where(item => item.IsChecked).ToList();
                if (selectedList.Count == 0)
                {
                    ret = false;
                }
            }
            return ret;
        }

        private void Button_Maintain_Click(object sender, RoutedEventArgs e)
        {

            Button button = sender as Button;
            int refundBalanceSysNo = 0;
            int refundSysNo = int.Parse(this.Request.Param);
            string action = button.CommandParameter.ToString();

            if (string.IsNullOrEmpty(this.Request.Param))
            {
                this.Window.Alert(ResRefundBalance.Msg_NoData);
                return;
            }
            if (button.CommandParameter.ToString() != "Create")
            {
                if (!ValidateSelectRow())
                {
                    this.Window.Alert(ResRefundBalance.Msg_SelectData);
                    return;
                }
                else
                {
                    refundBalanceSysNo = list.SingleOrDefault(p => p.IsChecked).SysNo;
                }
            }

            UCRefundBalanceMaintain uctl = new UCRefundBalanceMaintain(refundSysNo, refundBalanceSysNo, action);
            uctl.Dialog = Window.ShowDialog(ResRefundBalance.Dialog_RefundBalanceMaintain, uctl, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.DataGrid_ResultList.Bind();
                }
            });
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                var txtBinding = textBox.GetBindingExpression(TextBox.TextProperty);
                if (txtBinding != null)
                {
                    txtBinding.UpdateSource();
                    string url = string.Format(ConstValue.RMA_RefundBalanceQueryUrl, Text_RefundSysNo.Text.Trim());
                    Window.Navigate(url, null, false);
                }
            }
        }
    }
}
