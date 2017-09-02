using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.SO.Views.Complain
{
    [View(IsSingleton = true)]
    public partial class ComplainQuery : PageBase
    {
        SOComplainFacade m_facade;
        ComplainQueryFilter m_queryRequest;
        CommonDataFacade m_commonDataFacade;

        public ComplainQuery()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            m_facade = new SOComplainFacade(this);
            m_commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            this.SearchCondition.DataContext = m_queryRequest = new ComplainQueryFilter();
            BindComboBoxData();

            #region 权限控件显示

            btnNew.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_Complain_ComplainFull);
            btnAssign.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_Complain_AssignComplainCase);
            btnCancelAssign.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_Complain_RecallAssignedComplainCase);

            #endregion
        }

        //搜索
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        //创建投诉
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ComplainAddRequestNew newRequestCtrl = new ComplainAddRequestNew();
            newRequestCtrl.Dialog = Window.ShowDialog(
                       ResComplain.Header_AddComplain
                       , newRequestCtrl
                       , (s, args) =>
                       {
                           if (args.DialogResult == DialogResultType.OK)
                           {
                               QueryResultGrid.PageIndex = 0;
                               QueryResultGrid.SelectedIndex = -1;
                               QueryResultGrid.Bind();
                           }
                       }
                       , new Size(570, 320)
                );
        }

        //分配
        private void btnAssign_Click(object sender, RoutedEventArgs e)
        {
            List<DynamicXml> list = new List<DynamicXml>();
            var dynamic = this.QueryResultGrid.ItemsSource as dynamic;
            if (dynamic != null)
            {
                foreach (var item in dynamic)
                {
                    if (item.IsCheck == true)
                    {
                        list.Add(item);
                    }
                }
            }

            if (list.Count == 0)
            {
                this.Window.Alert(ResSO.Msg_PleaseSelect);
                return;
            }

            SOCSAssign ctrl = new SOCSAssign();
            ctrl.Dialog = Window.ShowDialog(ResComplain.Header_AssignUser, ctrl, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    //进行派发操作
                    List<SOComplaintProcessInfo> req = new List<SOComplaintProcessInfo>();
                    foreach (var item in list)
                    {
                        var reqitem = new SOComplaintProcessInfo();
                        reqitem.SysNo = (int)item["SysNo"];
                        reqitem.OperatorSysNo = (int)args.Data;
                        req.Add(reqitem);
                    }
                    m_facade.Assign(req, (o, ar) =>
                    {
                        ar.FaultsHandle();
                        QueryResultGrid.Bind();
                    });
                }
            }
            , new Size(300, 155)
            );
        }

        //取消分配
        private void btnCancelAssign_Click(object sender, RoutedEventArgs e)
        {
            List<DynamicXml> list = new List<DynamicXml>();
            var dynamic = this.QueryResultGrid.ItemsSource as dynamic;
            if (dynamic != null)
            {
                foreach (var item in dynamic)
                {
                    if (item.IsCheck == true)
                    {
                        list.Add(item);
                    }
                }
            }

            if (list.Count == 0)
            {
                this.Window.Alert(ResSO.Msg_PleaseSelect);
                return;
            }
            m_facade.CancelAssign(list.Select(p => new SOComplaintProcessInfo() { SysNo = (int)p["SysNo"] }).ToList(), (s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    QueryResultGrid.Bind();
                }
            });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QueryComplainList(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                QueryResultGrid.TotalCount = args.Result.TotalCount;
                QueryResultGrid.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
            });
        }

        //绑定下拉菜单数据
        private void BindComboBoxData()
        {
            //状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOComplainStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;

            //CodePair
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                , new string[] { ConstValue.Key_ComplainType, ConstValue.Key_SOComplainSourceType, ConstValue.Key_SOResponsibleDept }
                , CodeNamePairAppendItemType.All, (o, p) =>
            {
                this.cmbComplainType.ItemsSource = p.Result[ConstValue.Key_ComplainType];
                this.cmbComplainType.SelectedIndex = 0;

                this.cmbComplainSourceType.ItemsSource = p.Result[ConstValue.Key_SOComplainSourceType];
                this.cmbComplainSourceType.SelectedIndex = 0;

                this.cmbResponsibleDept.ItemsSource = p.Result[ConstValue.Key_SOResponsibleDept];
                this.cmbResponsibleDept.SelectedIndex = 0;
            });

            //超期状态
            this.cmbOverdueType.ItemsSource = EnumConverter.GetKeyValuePairs<OutdatedType>(EnumConverter.EnumAppendItemType.All);
            this.cmbOverdueType.SelectedIndex = 0;

            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_Compare, (o, p) =>
            {
                this.cmbCompareSpendHours.ItemsSource = p.Result;
                this.cmbCompareSpendHours.SelectedIndex = 0;
            });

            //处理人
            m_commonDataFacade.GetUserInfoByDepartmentId(101, (o, p) =>
            {
                if (p.FaultsHandle()) return;
                p.Result.Insert(0, new UserInfo()
                {
                    SysNo = 0,
                    UserDisplayName = ResSO.Expander_NoAssign
                });
                p.Result.Insert(0, new UserInfo()
                {
                    UserDisplayName = ResCommonEnum.Enum_All
                });

                this.cmbOperator.ItemsSource = p.Result;
                this.cmbOperator.SelectedIndex = 0;
            });
        }

        //跳转到投诉回复
        private void ComplainSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SO_ComplainReplyUrl, info["SysNo"]), null, true);
            }
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }

        private void ckbIsReOpen_Click(object sender, RoutedEventArgs e)
        {
            txtReOpenCount.Visibility = ((CheckBox)sender).IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        //按查询条件导出数据到Excel
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_ExcelExport))
            {
                Window.Alert(ResSO.Msg_Error_Right, MessageType.Error);
                return;
            }

            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            ColumnSet col = new ColumnSet(QueryResultGrid);
            col.Insert(0, "CSConfirmComplainType", ResSO.DataGrid_CSConfirmComplainType);
            col.Insert(1, "CSConfirmComplainTypeDetail", ResSO.DataGrid_CSConfirmComplainTypeDetail);

            facade.ExportComplain(m_queryRequest, new ColumnSet[] { col });
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = (Combox)sender;
            var selectStatus = cmb.SelectedValue as SOComplainStatus?;
            CloseDate.IsEnabled = false;
            if (selectStatus != null && selectStatus.HasValue && selectStatus.Value == SOComplainStatus.Complete)
            {
                CloseDate.IsEnabled = true;
            }
        }
    }
}
