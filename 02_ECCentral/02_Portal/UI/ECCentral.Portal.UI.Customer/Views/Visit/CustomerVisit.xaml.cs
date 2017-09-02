using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Customer.Resources;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class CustomerVisit : PageBase
    {
        private CustomerVisitView PageView;
        private CustomerVisitView newQueryView;
        private CustomerVisitView TempView;
        private VisitSeachType currentType;
        private CustomerVisitFacade facade;

        #region 页面初始化
        public CustomerVisit()
        {
            PageView = new CustomerVisitView()
            {
                QueryInfo = new CustomerVisitQueryVM
                {
                    SeachType = VisitSeachType.Visited,
                    IsVip = false
                }
            };
            InitializeComponent();

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
            facade = new CustomerVisitFacade();
            expanderQueryCondition.DataContext = PageView.QueryInfo;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerVist_Export))
            {
                this.dgVisited.IsShowAllExcelExporter = this.dgWaitingVisit.IsShowAllExcelExporter =
                    this.dgMaintenance.IsShowAllExcelExporter = this.dgFollowUpMaintenance.IsShowAllExcelExporter =
                    this.dgFollowUpVisit.IsShowAllExcelExporter = false;
            }
        }

        /// <summary>
        /// 初始化页面数据
        /// </summary>
        private void IniPageData()
        {
            cmbSeachType.ItemsSource = EnumConverter.GetKeyValuePairs<VisitSeachType>(EnumConverter.EnumAppendItemType.None);
            cmbSeachType.SelectedIndex = 0;

            cmbCustomerRank.ItemsSource = EnumConverter.GetKeyValuePairs<CustomerRank>(EnumConverter.EnumAppendItemType.All);
            cmbCustomerRank.SelectedIndex = 0;

            cbCallResultConnected.Content = VisitCallResult.Connected.ToDescription();
            cbCallResultNotConnected.Content = VisitCallResult.NotConnected.ToDescription();
            cbCallResultCustomerReject.Content = VisitCallResult.CustomerReject.ToDescription();
            cbCallResultNotSelf.Content = VisitCallResult.NotSelf.ToDescription();
            cbCallResultNoIsError.Content = VisitCallResult.NumberIsError.ToDescription();
            cbCallResultOtherTime.Content = VisitCallResult.OtherTime.ToDescription();

            cmbDealStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VisitDealStatus>(EnumConverter.EnumAppendItemType.All);
            cmbDealStatus.SelectedIndex = 0;

            cmbConsumeDesire.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatusThree>(EnumConverter.EnumAppendItemType.All);
            cmbConsumeDesire.SelectedIndex = 0;

            cmbActivated.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            cmbActivated.SelectedIndex = 0;

            cmbMaintain.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
            cmbMaintain.SelectedIndex = 0;

            //Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn colRank = dataGridVisit.Columns[5] as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
            //colRank.Binding.ConverterParameter = typeof(CustomerRank);

            new OtherDomainQueryFacade(this).GetCustomerServiceList((obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                args.Result.Insert(0, new BizEntity.Common.UserInfo() { UserDisplayName = ResCommonEnum.Enum_All });
                cmbEditUser.ItemsSource = args.Result;
                cmbEditUser.SelectedIndex = 0;
            });

        }
        #endregion

        #region 查询回访数据

        #region 查询条件显示相关操作
        private void cbMore_Click(object sender, RoutedEventArgs e)
        {
            spSpider.Visibility = cbMore.IsChecked.Value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void cmbSeachType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbMore.Visibility = PageView.QueryInfo.SeachType.Value == VisitSeachType.WaitingVisit ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            spSpider.Visibility = (cbMore.IsChecked.Value && cbMore.Visibility == System.Windows.Visibility.Visible) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            drSpiderTime.Visibility = (
                PageView.QueryInfo.SeachType.Value == VisitSeachType.Visited
                || PageView.QueryInfo.SeachType.Value == VisitSeachType.Maintenance
                || PageView.QueryInfo.SeachType.Value == VisitSeachType.FollowUpMaintenance
                ) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            cmbDealStatus.Visibility = PageView.QueryInfo.SeachType.Value == VisitSeachType.Maintenance ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            cmbActivated.IsEnabled = PageView.QueryInfo.SeachType.Value == VisitSeachType.Visited;
            cmbMaintain.IsEnabled = PageView.QueryInfo.SeachType.Value == VisitSeachType.Visited;
            cmbDealStatus.IsEnabled = PageView.QueryInfo.SeachType.Value == VisitSeachType.Visited;

            tbCallResult.Visibility = PageView.QueryInfo.SeachType.Value == VisitSeachType.Maintenance ? Visibility.Collapsed : System.Windows.Visibility.Visible;

            PageView.QueryInfo.IsActivated = null;
            PageView.QueryInfo.IsMaintain = null;
            PageView.QueryInfo.DealStatus = null;
            switch (PageView.QueryInfo.SeachType.Value)
            {
                case VisitSeachType.Visited:
                    this.exVisited.Visibility = System.Windows.Visibility.Visible;
                    this.exFollowUpVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exWaitingVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    cmbMaintain.SelectedIndex = 0;
                    cmbDealStatus.SelectedIndex = 0;
                    break;
                case VisitSeachType.WaitingVisit:
                    this.exVisited.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exWaitingVisit.Visibility = System.Windows.Visibility.Visible;
                    this.exMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    cmbMaintain.SelectedIndex = 0;
                    cmbDealStatus.SelectedIndex = 0;
                    break;
                case VisitSeachType.FollowUpMaintenance:
                    this.exVisited.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exWaitingVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpMaintenance.Visibility = System.Windows.Visibility.Visible;
                    cmbMaintain.SelectedIndex = 0;
                    cmbDealStatus.SelectedIndex = 1;
                    break;
                case VisitSeachType.Maintenance:
                    this.exVisited.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exWaitingVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exMaintenance.Visibility = System.Windows.Visibility.Visible;
                    this.exFollowUpMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    PageView.QueryInfo.IsActivated = YNStatus.Y;
                    PageView.QueryInfo.IsMaintain = YNStatus.Y;
                    PageView.QueryInfo.DealStatus = VisitDealStatus.FollowUp;
                    cmbMaintain.SelectedIndex = 1;
                    cmbDealStatus.SelectedIndex = 0;
                    break;
                case VisitSeachType.FollowUpVisit:
                    PageView.QueryInfo.DealStatus = VisitDealStatus.FollowUp;
                    this.exVisited.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpVisit.Visibility = System.Windows.Visibility.Visible;
                    this.exWaitingVisit.Visibility = System.Windows.Visibility.Collapsed;
                    this.exMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    this.exFollowUpMaintenance.Visibility = System.Windows.Visibility.Collapsed;
                    cmbDealStatus.SelectedIndex = 0;
                    cmbMaintain.SelectedIndex = 0;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (PageView.QueryInfo.HasValidationErrors)
                return;
            currentType = PageView.QueryInfo.SeachType.Value;
            newQueryView = PageView.DeepCopy();
            switch (currentType)
            {
                case VisitSeachType.Visited:
                    this.dgVisited.Bind();
                    break;
                case VisitSeachType.WaitingVisit:
                    this.dgWaitingVisit.Bind();
                    break;
                case VisitSeachType.FollowUpVisit:
                    this.dgFollowUpVisit.Bind();
                    break;
                case VisitSeachType.Maintenance:
                    this.dgMaintenance.Bind();
                    break;
                case VisitSeachType.FollowUpMaintenance:
                    this.dgFollowUpMaintenance.Bind();
                    break;
            }
        }

        private void SetQuery(Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CustomerVisitQueryVM query = PageView.QueryInfo;

            query.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            #region 取得 filter.CallResult
            query.CallResult = new List<VisitCallResult>();
            if (cbCallResultConnected.IsChecked.HasValue && cbCallResultConnected.IsChecked.Value)
            {
                query.CallResult.Add(VisitCallResult.Connected);
            }
            if (cbCallResultCustomerReject.IsChecked.HasValue && cbCallResultCustomerReject.IsChecked.Value)
            {
                query.CallResult.Add(VisitCallResult.CustomerReject);
            }
            if (cbCallResultNoIsError.IsChecked.HasValue && cbCallResultNoIsError.IsChecked.Value)
            {
                query.CallResult.Add(VisitCallResult.NumberIsError);
            }
            if (cbCallResultNotConnected.IsChecked.HasValue && cbCallResultNotConnected.IsChecked.Value)
            {
                query.CallResult.Add(VisitCallResult.NotConnected);
            }
            if (cbCallResultNotSelf.IsChecked.HasValue && cbCallResultNotSelf.IsChecked.Value)
            {
                query.CallResult.Add(VisitCallResult.NotSelf);
            }
            if (cbCallResultOtherTime.IsChecked.HasValue && cbCallResultOtherTime.IsChecked.Value)
            {
                query.CallResult.Add(VisitCallResult.OtherTime);
            }
            #endregion
        }

        #endregion

        #region Grid绑定及数据导出
        //回访统计查询
        private void dgVisited_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SetQuery(e);
            if (this.cbMore.IsChecked.Value)
                TempView = PageView;
            else
            {
                IsNormalQuery(PageView.QueryInfo.SeachType.Value);
                TempView = newQueryView;
            }
            facade.Query(TempView, (restSender, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var a in args.Result.Rows)
                {
                    if (a.IsDefined("Cellphone") && !string.IsNullOrEmpty(a.Cellphone))
                    {
                        a.Phone = string.Format("{0},{1}", a.Phone, a.Cellphone);
                    }
                }
                dgVisited.TotalCount = args.Result.TotalCount;
                dgVisited.ItemsSource = args.Result.Rows;
            });
        }

        //回访统计数据导出
        private void dgVisited_ExportAllClick(object sender, EventArgs e)
        {
            if (dgVisited.ItemsSource == null || dgVisited.TotalCount == 0)
            {
                Window.Alert(ResCustomerVisit.Msg_NoData);
                return;
            }
            CustomerVisitQueryVM query = PageView.QueryInfo;
            query.PageInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            CustomerVisitFacade facade = new CustomerVisitFacade(this);
            ColumnSet col = new ColumnSet(dgVisited, true);
            facade.ExportCustomerVisit(query, new ColumnSet[] { col });
        }

        //待回访查询
        private void dgWaitingVisit_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SetQuery(e);
            facade.Query(PageView, (restSender, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var a in args.Result.Rows)
                {
                    if (a.IsDefined("Cellphone") && !string.IsNullOrEmpty(a.Cellphone))
                    {
                        a.Phone = string.Format("{0},{1}", a.Phone, a.Cellphone);
                    }
                }
                dgWaitingVisit.TotalCount = args.Result.TotalCount;
                dgWaitingVisit.ItemsSource = args.Result.Rows;
            });
        }

        private void dgWaitingVisit_ExportAllClick(object sender, EventArgs e)
        {
            if (dgWaitingVisit.ItemsSource == null || dgWaitingVisit.TotalCount == 0)
            {
                Window.Alert(ResCustomerVisit.Msg_NoData);
                return;
            }
            CustomerVisitQueryVM query = PageView.QueryInfo;
            query.PageInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            CustomerVisitFacade facade = new CustomerVisitFacade(this);
            ColumnSet col = new ColumnSet(dgWaitingVisit, true);
            facade.ExportCustomerVisit(query, new ColumnSet[] { col });
        }

        //需跟进回访
        private void dgFollowUpVisit_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SetQuery(e);
            if (this.cbMore.IsChecked.Value)
                TempView = PageView;
            else
            {
                IsNormalQuery(PageView.QueryInfo.SeachType.Value);
                TempView = newQueryView;
            }
            facade.Query(TempView, (restSender, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var a in args.Result.Rows)
                {
                    if (a.IsDefined("Cellphone") && !string.IsNullOrEmpty(a.Cellphone))
                    {
                        a.Phone = string.Format("{0},{1}", a.Phone, a.Cellphone);
                    }
                }
                dgFollowUpVisit.TotalCount = args.Result.TotalCount;
                dgFollowUpVisit.ItemsSource = args.Result.Rows;
            });
        }

        private void dgFollowUpVisit_ExportAllClick(object sender, EventArgs e)
        {
            if (dgFollowUpVisit.ItemsSource == null || dgFollowUpVisit.TotalCount == 0)
            {
                Window.Alert(ResCustomerVisit.Msg_NoData);
                return;
            }
            CustomerVisitQueryVM query = PageView.QueryInfo;
            query.PageInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            CustomerVisitFacade facade = new CustomerVisitFacade(this);
            ColumnSet col = new ColumnSet(dgFollowUpVisit, true);
            facade.ExportCustomerVisit(query, new ColumnSet[] { col });
        }

        //待维护
        private void dgMaintenance_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SetQuery(e);
            if (this.cbMore.IsChecked.Value)
                TempView = PageView;
            else
            {
                IsNormalQuery(PageView.QueryInfo.SeachType.Value);
                TempView = newQueryView;
            }
            facade.Query(TempView, (restSender, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var a in args.Result.Rows)
                {
                    if (a.IsDefined("Cellphone") && !string.IsNullOrEmpty(a.Cellphone))
                    {
                        a.Phone = string.Format("{0},{1}", a.Phone, a.Cellphone);
                    }
                }
                dgMaintenance.TotalCount = args.Result.TotalCount;
                dgMaintenance.ItemsSource = args.Result.Rows;
            });
        }

        private void dgMaintenance_ExportAllClick(object sender, EventArgs e)
        {
            if (dgMaintenance.ItemsSource == null || dgMaintenance.TotalCount == 0)
            {
                Window.Alert(ResCustomerVisit.Msg_NoData);
                return;
            }
            CustomerVisitQueryVM query = PageView.QueryInfo;
            query.PageInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            CustomerVisitFacade facade = new CustomerVisitFacade(this);
            ColumnSet col = new ColumnSet(dgMaintenance, true);
            facade.ExportCustomerVisit(query, new ColumnSet[] { col });
        }

        //需跟进维护
        private void dgFollowUpMaintenance_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SetQuery(e);
            if (this.cbMore.IsChecked.Value)
                TempView = PageView;
            else
            {
                IsNormalQuery(PageView.QueryInfo.SeachType.Value);
                TempView = newQueryView;
            }
            facade.Query(TempView, (restSender, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var a in args.Result.Rows)
                {
                    if (a.IsDefined("Cellphone") && !string.IsNullOrEmpty(a.Cellphone))
                    {
                        a.Phone = string.Format("{0},{1}", a.Phone, a.Cellphone);
                    }
                }
                dgFollowUpMaintenance.TotalCount = args.Result.TotalCount;
                dgFollowUpMaintenance.ItemsSource = args.Result.Rows;
            });
        }

        private void dgFollowUpMaintenance_ExportAllClick(object sender, EventArgs e)
        {
            if (dgFollowUpMaintenance.ItemsSource == null || dgFollowUpMaintenance.TotalCount == 0)
            {
                Window.Alert(ResCustomerVisit.Msg_NoData);
                return;
            }
            CustomerVisitQueryVM query = PageView.QueryInfo;
            query.PageInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            CustomerVisitFacade facade = new CustomerVisitFacade(this);
            ColumnSet col = new ColumnSet(dgFollowUpMaintenance, true);
            facade.ExportCustomerVisit(query, new ColumnSet[] { col });
        }

        //普通查询
        private void IsNormalQuery(VisitSeachType type)
        {
            switch (type)
            {
                case VisitSeachType.Visited:
                    newQueryView.QueryInfo.CallResult = null;
                    newQueryView.QueryInfo.DealStatus = null;
                    newQueryView.QueryInfo.ConsumeDesire = null;
                    newQueryView.QueryInfo.IsActivated = null;
                    newQueryView.QueryInfo.IsMaintain = null;
                    newQueryView.QueryInfo.LastEditorSysNo = null;
                    newQueryView.QueryInfo.FromVisitDate = newQueryView.QueryInfo.ToVisitDate = null;
                    newQueryView.QueryInfo.SpiderOrderDateFrom = newQueryView.QueryInfo.SpiderOrderDateTo = null;
                    break;
                case VisitSeachType.FollowUpVisit:
                    newQueryView.QueryInfo.CallResult = null;
                    newQueryView.QueryInfo.ConsumeDesire = null;
                    newQueryView.QueryInfo.LastEditorSysNo = null;
                    newQueryView.QueryInfo.FromVisitDate = newQueryView.QueryInfo.ToVisitDate = null;
                    break;
                case VisitSeachType.Maintenance:
                    newQueryView.QueryInfo.ConsumeDesire = null;
                    newQueryView.QueryInfo.LastEditorSysNo = null;
                    newQueryView.QueryInfo.FromVisitDate = newQueryView.QueryInfo.ToVisitDate = null;
                    newQueryView.QueryInfo.SpiderOrderDateFrom = newQueryView.QueryInfo.SpiderOrderDateTo = null;
                    break;
                case VisitSeachType.FollowUpMaintenance:
                    newQueryView.QueryInfo.CallResult = null;
                    newQueryView.QueryInfo.ConsumeDesire = null;
                    newQueryView.QueryInfo.LastEditorSysNo = null;
                    newQueryView.QueryInfo.FromVisitDate = newQueryView.QueryInfo.ToVisitDate = null;
                    newQueryView.QueryInfo.SpiderOrderDateFrom = newQueryView.QueryInfo.SpiderOrderDateTo = null;
                    break;
            }
        }
        #endregion

        #region 操作列
        //查看
        private void hlbVisitedView_Click(object sender, RoutedEventArgs e)
        {
            dynamic visit = this.dgVisited.SelectedItem as dynamic;
            if (visit != null)
            {
                VisitDetail content = new VisitDetail();
                content.Width = 800d;
                content.Height = 440d;
                content.VisitSysNo = visit.SysNo;
                IDialog dialog = this.Window.ShowDialog(ResCustomerVisit.Dialog_Title_ViewVisitDetail, content, (obj, args) =>
                {

                });
                content.Dialog = dialog;
            }
        }

        //待回访，待维护
        private void hlbEvent_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerVist_WaitMaintain)
                || !AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_CustomerVist_WaitVisit))
            {
                Window.Alert(ResCustomerVisit.Msg_HasNoRights);
                return;
            }
            dynamic visit = null;
            switch (currentType)
            {
                case VisitSeachType.WaitingVisit:
                    visit = this.dgWaitingVisit.SelectedItem as dynamic;
                    if (visit != null)
                    {
                        //使用Dialog需要注意的地方
                        VisitMaintain content = new VisitMaintain(this.Window);
                        content.Width = 800d;
                        content.Height = 440d;
                        content.CustomerSysNo = visit.CustomerSysNo;
                        content.IsOrderVisit = currentType == VisitSeachType.Maintenance || currentType == VisitSeachType.FollowUpMaintenance;
                        IDialog dialog = this.Window.ShowDialog(ResCustomerVisit.Dialog_Title_AddVisitLog, content, (obj, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                dgWaitingVisit.Bind();
                            }
                        });
                        content.Dialog = dialog;

                    }
                    break;
                case VisitSeachType.FollowUpVisit:
                    visit = this.dgFollowUpVisit.SelectedItem as dynamic;
                    if (visit != null)
                    {
                        //使用Dialog需要注意的地方
                        VisitMaintain content = new VisitMaintain(this.Window);
                        content.Width = 800d;
                        content.Height = 440d;
                        content.CustomerSysNo = visit.CustomerSysNo;
                        content.IsOrderVisit = currentType == VisitSeachType.Maintenance || currentType == VisitSeachType.FollowUpMaintenance;
                        IDialog dialog = this.Window.ShowDialog(ResCustomerVisit.Dialog_Title_AddVisitLog, content, (obj, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                dgFollowUpVisit.Bind();
                            }
                        });
                        content.Dialog = dialog;
                    }
                    break;
                case VisitSeachType.Maintenance:
                    visit = this.dgMaintenance.SelectedItem as dynamic;
                    if (visit != null)
                    {
                        //使用Dialog需要注意的地方
                        VisitMaintain content = new VisitMaintain(this.Window);
                        content.Width = 800d;
                        content.Height = 440d;
                        content.CustomerSysNo = visit.CustomerSysNo;
                        content.IsOrderVisit = currentType == VisitSeachType.Maintenance || currentType == VisitSeachType.FollowUpMaintenance;
                        IDialog dialog = this.Window.ShowDialog(ResCustomerVisit.Dialog_Title_AddMaintainLog, content, (obj, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                dgMaintenance.Bind();
                            }
                        });
                        content.Dialog = dialog;
                    }
                    break;
                case VisitSeachType.FollowUpMaintenance:
                    visit = this.dgFollowUpMaintenance.SelectedItem as dynamic;
                    if (visit != null)
                    {
                        //使用Dialog需要注意的地方
                        VisitMaintain content = new VisitMaintain(this.Window);
                        content.Width = 800d;
                        content.Height = 440d;
                        content.CustomerSysNo = visit.CustomerSysNo;
                        content.IsOrderVisit = currentType == VisitSeachType.Maintenance || currentType == VisitSeachType.FollowUpMaintenance;
                        IDialog dialog = this.Window.ShowDialog(ResCustomerVisit.Dialog_Title_AddMaintainLog, content, (obj, args) =>
                        {
                            if (args.DialogResult == DialogResultType.OK)
                            {
                                dgFollowUpMaintenance.Bind();
                            }
                        });
                        content.Dialog = dialog;
                    }
                    break;
            }
        }
        #endregion
    }
}


