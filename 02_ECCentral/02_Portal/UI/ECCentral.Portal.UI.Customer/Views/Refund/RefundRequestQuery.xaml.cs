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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.QueryFilter.Customer;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.UserControls.Refund;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views.Refund
{
    [View]
    public partial class RefundRequestQuery : PageBase
    {
        RefundRequestVM viewModel;
        CommonDataFacade commFacade;
        RefundRequestFacade facade;
        RefundRequestQueryFilter filter;
        public RefundRequestQuery()
        {
            viewModel = new RefundRequestVM();
            filter = new RefundRequestQueryFilter();
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            commFacade = new CommonDataFacade(this);
            facade = new RefundRequestFacade(this);
            InitVM();
            this.DataContext = viewModel;
            base.OnPageLoad(sender, e);
            CheckRights();
            this.SeachBuilder.KeyDown += new KeyEventHandler(SeachBuilder_KeyDown);
        }

        void SeachBuilder_KeyDown(object sender, KeyEventArgs e)
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
                    this.dataGrid1.Bind();
                    e.Handled = true;
                }
            }
        }

        private void InitVM()
        {
            CodeNamePairHelper.GetList("Customer", "RefundType", CodeNamePairAppendItemType.All, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    viewModel.RefundTypeList.Add(item);
                }
            });
        }
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.Bind();
        }

        private void dataGrid1_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter = viewModel.ConvertVM<RefundRequestVM, RefundRequestQueryFilter>();
            filter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.Query(filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid1.ItemsSource = args.Result.Rows.ToList();
                dataGrid1.TotalCount = args.Result.TotalCount;

            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.dataGrid1.ItemsSource as dynamic;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        if (view.IsEnable > 0)
                            view.IsChecked = ckb.IsChecked.Value ? 1 : 0;
                    }
                }
            }
        }

        private void btnRefuse_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = GetSelected();
            dynamic viewList = this.dataGrid1.ItemsSource as dynamic;
            for (int i = 0; i < viewList.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (viewList[i].SysNo == list[j] && viewList[i].Status != BizEntity.Customer.RefundRequestStatus.O)
                    {
                        list.Remove(list[j]);
                    }
                }
            }
            if (list.Count > 0)
            {
                Refuse window = new Refuse();
                window.Dialog = Window.ShowDialog(ResRefundRequestQuery.title_Refuse, window, (obj, args) =>
                {
                    if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        RefundAuditReq filter = new RefundAuditReq();
                        filter.RefundRequestList = list;
                        filter.Memo = args.Data.ToString();
                        filter.Status = BizEntity.Customer.RefundRequestStatus.R;
                        facade.Audit(filter, (obj1, args1) =>
                        {
                            if (args1.FaultsHandle())
                                return;
                            dataGrid1.Bind();
                            Window.Alert(ResRefundRequestQuery.Msg_OperationOk);
                        });

                    }
                }, new Size(320, 250));
            }
            else
            {
                Window.Alert(ResRefundRequestQuery.Msg_OpreationFailed);
                this.dataGrid1.Bind();
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = GetSelected();
            dynamic viewList = this.dataGrid1.ItemsSource as dynamic;
            for (int i = 0; i < viewList.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (viewList[i].SysNo == list[j] && viewList[i].Status != BizEntity.Customer.RefundRequestStatus.O)
                    {
                        list.Remove(list[j]);
                    }
                }
            }
            if (list.Count > 0)
            {
                RefundAuditReq filter = new RefundAuditReq();
                filter.RefundRequestList = list;
                filter.Memo = string.Empty;
                filter.Status = BizEntity.Customer.RefundRequestStatus.A;
                facade.Audit(filter, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    dataGrid1.Bind();
                    Window.Alert(ResRefundRequestQuery.Msg_OperationOk);
                });
            }
            else
            {
                Window.Alert(ResRefundRequestQuery.Msg_OpreationFailed);
                this.dataGrid1.Bind();
            }
        }
        private List<int> GetSelected()
        {
            List<int> list = new List<int>();
            dynamic viewList = this.dataGrid1.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    if (view.IsChecked > 0)
                        list.Add(view.SysNo);
                }
            }
            return list;
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundRequest_Audit))
                this.btnAudit.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_RefundRequest_RefuseAudit))
                this.btnRefuse.IsEnabled = false;
        }
        #endregion
    }

}
