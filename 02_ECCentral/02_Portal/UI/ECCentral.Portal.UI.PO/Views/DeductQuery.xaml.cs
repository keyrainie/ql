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
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Models.PurchaseOrder;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.UserControls;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true)]
    public partial class DeductQuery : PageBase
    {
        public DeductFacade deductFacade;
        public DeductQueryVM deductQueryVM;
        public DeductQueryFilter queryRequest;
        public DeductQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            deductQueryVM = new DeductQueryVM();
            deductFacade = new DeductFacade(this);

            InitComBoxData();

            this.SearchCondition.DataContext = deductQueryVM;

            base.OnPageLoad(sender, e);
        }

        public void InitComBoxData()
        {
            //扣款项目类型
            this.cmbDeductType.ItemsSource = EnumConverter.GetKeyValuePairs<DeductType>(EnumConverter.EnumAppendItemType.All);
            //this.cmbDeductType.SelectedIndex = 0;
            //记成本/费用
            this.cmbAccountType.ItemsSource = EnumConverter.GetKeyValuePairs<AccountType>(EnumConverter.EnumAppendItemType.All);
            //this.cmbAccountType.SelectedIndex = 0;
            //扣款方式
            this.cmbDeductMethod.ItemsSource = EnumConverter.GetKeyValuePairs<DeductMethod>(EnumConverter.EnumAppendItemType.All);
            //this.cmbDeductMethod.SelectedIndex = 0;

            //状态
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<Status>(EnumConverter.EnumAppendItemType.All);

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //构造查询条件
            //扣款名称
            string name = this.txtName.Text.Trim();
            if (name.Length > 200)
            {
                return;
            }

            deductQueryVM.Name = this.txtName.Text.Trim();
            //构造查询条件
            this.queryRequest = EntityConverter<DeductQueryVM, DeductQueryFilter>.Convert(deductQueryVM);

            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var queryRequest = EntityConverter<DeductQueryVM, DeductQueryFilter>.Convert(deductQueryVM);
            queryRequest.PageInfo = new PagingInfo()
          {
              PageSize = QueryResultGrid.PageSize,
              PageIndex = QueryResultGrid.PageIndex,
              SortBy = e.SortField
          };
            deductFacade.QueryDeducts(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var deductList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;

                //作废扣款项只能查看
                foreach (var item in deductList)
                {
                    bool edtiVisibility = false;
                    bool abandonVisibility = true;
                    bool viewVisibility = false;
                    if (item.Status == Status.Effective)
                    {
                        edtiVisibility = true;
                        abandonVisibility = true;
                        viewVisibility = false;
                    }
                    else
                    {
                        abandonVisibility = false;
                        viewVisibility = true;
                    }
                    item.ViewButtonVisibility = viewVisibility;
                    item.AbandonButtonVisibility = abandonVisibility;
                    item.EditButtonVisibility = edtiVisibility;
                }
                QueryResultGrid.ItemsSource = deductList;
            });
        }

        //编辑
        private void Hyperlink_EditDeduct_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;

            if (null != getSelectedItem)
            {
                //Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/DeductMaintain/{0}", getSelectedItem["SysNo"]), null, true);
                UCDeductMaintain ucDeduct = new UCDeductMaintain(getSelectedItem["SysNo"].ToString());
                ucDeduct.Dialog = Window.ShowDialog("编辑扣款项", ucDeduct, (obj, args) =>
                {
                    //Window.Alert(args.Data.ToString());
                    //Window.Refresh();
                    this.QueryResultGrid.Bind();
                }, new Size(500, 260));
            }
        }
        //作废
        private void Hyperlink_DeleteDeduct_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResDeductQuery.AlertMsg_AlertTitle, ResDeductQuery.AlertMsg_ConfirmDelInfo, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
                    if (null != getSelectedItem)
                    {
                        int sysNo = Convert.ToInt32(getSelectedItem["SysNo"]);
                        deductFacade.DelSingleDeductInfo(sysNo, (_obj, _args) =>
                        {
                            if (_args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert(ResDeductQuery.AlertMsg_DelSingleDeductSuc);
                            QueryResultGrid.Bind();
                        });
                    }
                }
            });

        }

        //转到新增/编辑页面
        private void BtnNewDeduct_Click(object sender, RoutedEventArgs e)
        {
            //Window.Navigate("/ECCentral.Portal.UI.PO/DeductMaintain", null, true);

            UCDeductMaintain ucDeduct = new UCDeductMaintain(null);
            ucDeduct.Dialog = Window.ShowDialog("新增扣款项", ucDeduct, (obj, args) =>
            {
                QueryResultGrid.Bind();
            }, new Size(500, 260));
        }

        //查看扣款项
        private void Hyperlink_ViewDeduct_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;

            if (null != getSelectedItem)
            {
                UCDeductMaintain ucDeduct = new UCDeductMaintain(getSelectedItem["SysNo"].ToString());
                ucDeduct.Dialog = Window.ShowDialog("查看扣款项", ucDeduct, (obj, args) =>
                {
                    this.QueryResultGrid.Bind();
                }, new Size(500, 260));
            }
        }
    }
}
