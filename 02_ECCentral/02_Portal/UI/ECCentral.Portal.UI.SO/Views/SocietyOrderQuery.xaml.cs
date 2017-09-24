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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using System.Windows.Browser;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.SO.UserControls;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class SocietyOrderQuery : PageBase
    {
        #region 页面初始化操作
        CommonDataFacade CommonDataFacade;
        SOQueryFacade SOQueryFacade;
        SOFacade SOFacade;
        SOQueryView PageView;

        private SOQueryVM ExportSOQueryInfo = null;

        private string SOSysNo
        {
            get
            {
                return HttpUtility.UrlDecode(this.Request.Param);
            }
        }

        public SocietyOrderQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
            RightControl();
        }

        private void RightControl()
        {
            btnNewSO.Visibility = btnPhoneOrder.Visibility = btnExpiateOrder.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOCreate);
            btnBatchAudit.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAudit);
            btnBatchAbandon.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOAbandon);
            btnBatchAbandonAndReturnInventory.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_SOMaintain_SOEmployeeAbandon);
        }

        private void IniPageData()
        {
            CommonDataFacade = new CommonDataFacade(this);
            PageView = new SOQueryView();
            PageView.QueryInfo.SOSysNo = SOSysNo;
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            SOQueryFacade = new SOQueryFacade(this);
            SOFacade = new SOFacade(this);
            ExportSOQueryInfo = new SOQueryVM();
            CommonDataFacade.GetStockList(true, (sender, e) =>
            {
                PageView.QueryInfo.StockList = e.Result;
            });

            CommonDataFacade.GetAllSystemUser(CPApplication.Current.CompanyCode, (obj, args) =>
            {
                List<ECCentral.BizEntity.Common.UserInfo> userList = new List<BizEntity.Common.UserInfo>();
                userList.Add(new BizEntity.Common.UserInfo { SysNo = null, UserDisplayName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All });
                if (!args.FaultsHandle() && args.Result != null)
                {
                    userList.AddRange(args.Result);
                }
                cmbOutStockUser.ItemsSource = userList;
            });
            CommonDataFacade.GetCustomerServiceList(CPApplication.Current.CompanyCode, (r) =>
            {
                List<ECCentral.BizEntity.Common.UserInfo> userList = new List<BizEntity.Common.UserInfo>();
                userList.Add(new BizEntity.Common.UserInfo { SysNo = null, UserDisplayName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All });
                if (r != null)
                {
                    userList.AddRange(r);
                }
                cmbAuditUser.ItemsSource = userList;
            });

            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                , new string[] { ConstValue.Key_FPStatus, ConstValue.Key_KFCType }
                , CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    PageView.QueryInfo.FPStatusList = e.Result[ConstValue.Key_FPStatus];
                    PageView.QueryInfo.KFCTypeList = e.Result[ConstValue.Key_KFCType];
                }
            });

            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_TimeRange, CodeNamePairAppendItemType.Custom_All, (sender, e) =>
                {
                    PageView.QueryInfo.TimeRangeList = e.Result;
                });

            CommonDataFacade.GetWebChannelList(true, (sender, e) =>
            {
                cmbThirdPlatform.ItemsSource = e.Result;
            });

            spConditions.DataContext = PageView.QueryInfo;
            dataGridSO.DataContext = PageView;
            if (!string.IsNullOrEmpty(PageView.QueryInfo.SOSysNo))
            {
                if (Regex.IsMatch(PageView.QueryInfo.SOSysNo, @"^\d+([, \.]+\d+)*$"))
                {
                    btnSearch_Click(null, null);
                }
            }
        }
        #endregion

        #region 查询订单，绑定列表
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!PageView.QueryInfo.HasValidationErrors)
            {
                ExportSOQueryInfo = PageView.QueryInfo.DeepCopy();
                dataGridSO.Bind();
            }
        }

        private void QuerySO()
        {
            SOQueryFacade.QuerySO(ExportSOQueryInfo, (data, count) =>
            {
                PageView.Result = data;
                PageView.TotalCount = count;
                dataGridSO.ItemsSource = PageView.Result;
            });
        }
        private void dataGridSO_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PageView.QueryInfo.DeliveryDate = dateReceive.SelectedDate;
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            PageView.QueryInfo.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            ExportSOQueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            ExportSOQueryInfo.PageInfo = PageView.QueryInfo.PageInfo;
            QuerySO();
        }
        #endregion

        #region 订单列表中的操作事件
        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void hlbtnCustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.CustomerMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void cbSelectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbSelectAll = sender as CheckBox;
            if (PageView.Result != null)
            {
                PageView.Result.ForEach(item =>
                {
                    if (item.SOType != SOType.VirualGroupBuy)
                        item.IsChecked = cbSelectAll.IsChecked.Value;
                });
            }
        }
        #endregion

        #region 对选中订单的操作事件
        private List<int> SelectedSOSysNoList
        {
            get
            {
                List<int> soSysNoList = new List<int>();
                if (PageView.Result != null)
                {
                    PageView.Result.ForEach(item =>
                    {
                        if (item.IsChecked) soSysNoList.Add(item.SysNo.Value);
                    });
                }
                return soSysNoList;
            }
        }
        private SOActionValidator SOValidator
        {
            get
            {
                SOActionValidator validator = new SOActionValidator();
                if (PageView.Result != null)
                {
                    PageView.Result.ForEach(item =>
                    {
                        if (item.IsChecked)
                        {
                            validator.SOList.Add(new SOActionValidator.SOInfo
                            {
                                SOSysNo = item.SysNo.Value,
                                SOStatus = item.Status,
                                SOType = item.SOType
                            });
                        }
                    });
                }
                return validator;
            }
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSOSysNoList != null && SelectedSOSysNoList.Count > 0)
                Abandon(false);
            else
                Window.Alert(ResSO.Msg_PleaseSelect, MessageType.Information);
        }

        private void btnBatchAudit_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.AuditSO(SOValidator, false, (vm) =>
            {
                QuerySO();
                //请在这里写回调代码
            });
        }

        /// <summary>
        /// 批量作废即返库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchAbandonAndReturnInventory_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSOSysNoList != null && SelectedSOSysNoList.Count > 0)
                Abandon(true);
            else
                Window.Alert(ResSO.Msg_PleaseSelect, MessageType.Information);
        }

        private void Abandon(bool immediatelyReturnInventory)
        {
            Window.Confirm(ResSO.Info_AbandonConfirm, (s, e) =>
            {
                if (e.DialogResult == DialogResultType.OK)
                {
                    UCReasonCodePicker content = new UCReasonCodePicker();
                    content.ReasonCodeType = ReasonCodeType.Order;
                    content.Dialog = Window.ShowDialog(ResSOMaintain.Info_SOMaintain_AbandSO, content, (obj, args) =>
                    {
                        if (args.Data != null)
                        {
                            KeyValuePair<string, string> data = (KeyValuePair<string, string>)args.Data;
                            SOFacade.AbandonSO(SelectedSOSysNoList, immediatelyReturnInventory, (vm) =>
                            {
                                if (vm != null)
                                {
                                    SOInternalMemoFacade SOInternalMemoFacade = new SOInternalMemoFacade();
                                    for (int i = 0; i < SelectedSOSysNoList.Count; i++)
                                    {
                                        SOInternalMemoFacade.Create(new SOInternalMemoInfo
                                        {
                                            SOSysNo = SelectedSOSysNoList[i],
                                            Content = data.Value,
                                            LogTime = DateTime.Now,
                                            ReasonCodeSysNo = int.Parse(data.Key),
                                            CompanyCode = CPApplication.Current.CompanyCode,
                                            Status = SOInternalMemoStatus.FollowUp
                                        }, null);
                                    }
                                    QuerySO();
                                    Window.Alert(ResSOMaintain.Info_SOMaintain_SO_Abanded, MessageType.Information);
                                }
                            });
                        }
                    });
                }
            });
        }

        private void btnVATInvoicePrinted_Click(object sender, RoutedEventArgs e)
        {
            SOFacade.SOVATPrinted(SelectedSOSysNoList, () =>
            {
                QuerySO();
                //请在这里写回调代码
            });
        }

        #endregion

        private void dataGridSO_ExportAllClick(object sender, EventArgs e)
        {
            ColumnSet col = new ColumnSet(dataGridSO);
            ColumnData colData = col["SOIncomeStatusText"];
            colData.FieldName = "SOIncomeStatusText";
            ExportSOQueryInfo.PageInfo.PageSize = dataGridSO.TotalCount;
            SOQueryFacade.ExportSO(ExportSOQueryInfo, new ColumnSet[] { col });
        }

        private void btnNewSO_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(ConstValue.SOMaintainUrl, null, true);
        }

        private void btnBatchPrintSO_Click(object sender, RoutedEventArgs e)
        {
            List<int> selectedSOSysNos = SelectedSOSysNoList;
            if (selectedSOSysNos != null && selectedSOSysNos.Count > 0)
            {
                Dictionary<string, string> t = new Dictionary<string, string>();
                t.Add("SOSysNoList", String.Join("|", SelectedSOSysNoList));
                HtmlViewHelper.WebPrintPreview("SO", "SOInfo", t);
            }
            else
            {
                Window.Alert(ResSOMaintain.Info_SOMaintain_SO_SelectPrintData, MessageType.Information);
            }
        }

        private void hlbtnSOQueryPayFlow_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            SOQueryDataVM data = dataGridSO.SelectedItem as SOQueryDataVM;
            int soSysNo = (int)btn.CommandParameter;
            SOPayFlowQuery payflowQuery = new SOPayFlowQuery(this, soSysNo);
            payflowQuery.Width = 600D;
            payflowQuery.Height = 300D;
            IDialog dialog = this.Window.ShowDialog(String.Format("{0}{1}", "查询支付流水", soSysNo), payflowQuery, (obj, args) =>
            {

            });
            payflowQuery.Dialog = dialog;
        }

        private void hlbtnSOProcess_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            SOQueryDataVM data = dataGridSO.SelectedItem as SOQueryDataVM;
            int soSysNo = (int)btn.CommandParameter;
            SOProcesser content = new SOProcesser(this, soSysNo);
            content.Width = 820D;
            content.Height = 500D;
            IDialog dialog = this.Window.ShowDialog(String.Format("{0}{1}", ResSO.UC_Title_SOProcessor, soSysNo), content, (obj, args) =>
            {

            });
            content.Dialog = dialog;
        }

        private void btnPhoneOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlOtherInfoFormat, ConstValue.Key_SOIsPhoneOrder), null, true);
        }

        private void btnExpiateOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.SOMaintainUrlOtherInfoFormat, ConstValue.Key_SOIsExpiateOrder), null, true);
        }

        private void btnBatchReportedSO_Click(object sender, RoutedEventArgs e)
        {
            //批量申报订单
            List<int> selectedSOSysNos = SelectedSOSysNoList;
            if (selectedSOSysNos != null && selectedSOSysNos.Count > 0)
            {
                SOFacade.BatchReportedSo(selectedSOSysNos, (result) =>
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        QuerySO();
                    }
                    else
                    {
                        if (result.Length > 100)
                        {
                            SoTextboxAlert content = new SoTextboxAlert(result);
                            content.Width = 550D;
                            content.Height = 350D;
                            IDialog dialog = this.Window.ShowDialog(ResSO.UC_Title_SoTextboxAlert, content, (obj, args) =>
                            {
                                QuerySO();
                            });
                        }
                        else
                        {
                            Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning, (obj, args) => { QuerySO(); });
                        }
                    }
                });
            }
            else
            {
                Window.Alert(ResSO.Msg_SOIsNull, MessageType.Information);
            }
        }

        private void btnBatchReported_Click(object sender, RoutedEventArgs e)
        {
            //申报成功;
            List<int> selectedSOSysNos = SelectedSOSysNoList;
            if (selectedSOSysNos != null && selectedSOSysNos.Count > 0)
            {
                SOFacade.BatchReportedSo(selectedSOSysNos, (result) =>
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        QuerySO();
                    }
                    else
                    {
                        if (result.Length > 100)
                        {
                            SoTextboxAlert content = new SoTextboxAlert(result);
                            content.Width = 550D;
                            content.Height = 350D;
                            IDialog dialog = this.Window.ShowDialog(ResSO.UC_Title_SoTextboxAlert, content, (obj, args) =>
                            {
                                QuerySO();
                            });
                        }
                        else
                        {
                            Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning, (obj, args) => { QuerySO(); });
                        }
                    }
                });
            }
            else
            {
                Window.Alert(ResSO.Msg_SOIsNull, MessageType.Information);
            }
        }

        private void btnBatchReject_Click(object sender, RoutedEventArgs e)
        {
            //申报失败;
            List<int> selectedSOSysNos = SelectedSOSysNoList;
            if (selectedSOSysNos != null && selectedSOSysNos.Count > 0)
            {
                SOFacade.BatchOperationUpdateSOStatusToReject(selectedSOSysNos, (result) =>
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        QuerySO();
                    }
                    else
                    {
                        if (result.Length > 100)
                        {
                            SoTextboxAlert content = new SoTextboxAlert(result);
                            content.Width = 550D;
                            content.Height = 350D;
                            IDialog dialog = this.Window.ShowDialog(ResSO.UC_Title_SoTextboxAlert, content, (obj, args) =>
                            {
                                QuerySO();
                            });
                        }
                        else
                        {
                            Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning, (obj, args) => { QuerySO(); });
                        }
                    }
                });
            }
            else
            {
                Window.Alert(ResSO.Msg_SOIsNull, MessageType.Information);
            }
        }

        private void btnBatchCustomsPass_Click(object sender, RoutedEventArgs e)
        {
            //通关成功;
            List<int> selectedSOSysNos = SelectedSOSysNoList;
            if (selectedSOSysNos != null && selectedSOSysNos.Count > 0)
            {
                SOFacade.BatchOperationUpdateSOStatusToCustomsPass(selectedSOSysNos, (result) =>
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        QuerySO();
                    }
                    else
                    {
                        if (result.Length > 100)
                        {
                            SoTextboxAlert content = new SoTextboxAlert(result);
                            content.Width = 550D;
                            content.Height = 350D;
                            IDialog dialog = this.Window.ShowDialog(ResSO.UC_Title_SoTextboxAlert, content, (obj, args) =>
                            {
                                QuerySO();
                            });
                        }
                        else
                        {
                            Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning, (obj, args) => { QuerySO(); });
                        }
                    }
                });
            }
            else
            {
                Window.Alert(ResSO.Msg_SOIsNull, MessageType.Information);
            }
        }

        private void btnBatchCustomsReject_Click(object sender, RoutedEventArgs e)
        {
            //通关失败;
            List<int> selectedSOSysNos = SelectedSOSysNoList;
            if (selectedSOSysNos != null && selectedSOSysNos.Count > 0)
            {
                SOFacade.BatchOperationUpdateSOStatusToCustomsReject(selectedSOSysNos, (result) =>
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        QuerySO();
                    }
                    else
                    {
                        if (result.Length > 100)
                        {
                            SoTextboxAlert content = new SoTextboxAlert(result);
                            content.Width = 550D;
                            content.Height = 350D;
                            IDialog dialog = this.Window.ShowDialog(ResSO.UC_Title_SoTextboxAlert, content, (obj, args) =>
                            {
                                QuerySO();
                            });
                        }
                        else
                        {
                            Window.Alert(ResSO.UC_Title_SoTextboxAlert, result, MessageType.Warning, (obj, args) => { QuerySO(); });
                        }
                    }
                });
            }
            else
            {
                Window.Alert(ResSO.Msg_SOIsNull, MessageType.Information);
            }
        }

        private void txtPromotionCode_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;
            //屏蔽非法按键，只能输入整数
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }
    }
}
