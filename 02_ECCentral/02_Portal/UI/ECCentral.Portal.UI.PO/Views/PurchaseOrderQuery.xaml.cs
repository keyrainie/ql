using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using ECCentral.QueryFilter.PO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class PurchaseOrderQuery : PageBase
    {
        public PurchaseOrderQueryVM queryVM;
        public PurchaseOrderQueryFilter queryFilter;
        public PurchaseOrderQueryFilter tempFilter;
        private PurchaseOrderQueryFilter newQueryFilter;
        public PurchaseOrderQueryFilter queryRMAFilter;
        public PurchaseOrderFacade serviceFacade;
        public VendorFacade vendorFacade;
        public string POSysNo;

        public PurchaseOrderQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new PurchaseOrderQueryVM();
            serviceFacade = new PurchaseOrderFacade(this);
            vendorFacade = new VendorFacade(this);

            queryFilter = new PurchaseOrderQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };
            queryRMAFilter = new PurchaseOrderQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };


            this.DataContext = queryVM;
            LoadComboBoxData();
            base.OnPageLoad(sender, e);

            if (null != this.Request.QueryString)
            {
                if (this.Request.QueryString.Keys.Contains("ProductSysNo"))
                {
                    queryVM.ProductSysNo = this.Request.QueryString["ProductSysNo"];
                }
                if (this.Request.QueryString.Keys.Contains("QueryStatus"))
                {
                    queryVM.StatusList = this.Request.QueryString["QueryStatus"];
                }
                if (this.Request.QueryString.Keys.Contains("POSysNo"))
                {
                    POSysNo = this.Request.QueryString["POSysNo"];
                    queryVM.POSysNoExtention = POSysNo;
                }
                btnSearch_Click(null, null);
            }
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //创建PO单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_CreatePO))
            {
                this.btnCreatePO.IsEnabled = false;
            }
        }

        private void LoadComboBoxData()
        {
            //付款结算公司 :
            //this.cmbPaymentClearingCompany.ItemsSource = EnumConverter.GetKeyValuePairs<PaySettleCompany>(EnumConverter.EnumAppendItemType.All);
            //this.cmbPaymentClearingCompany.SelectedIndex = 0;
            ////采购单是否分摊:
            //this.cmbIsApportion.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderIsApportionType>(EnumConverter.EnumAppendItemType.All);
            //this.cmbIsApportion.SelectedIndex = 0;
            //经销/代销:
            List<KeyValuePair<PurchaseOrderConsignFlag?, string>> list = EnumConverter.GetKeyValuePairs<PurchaseOrderConsignFlag>(EnumConverter.EnumAppendItemType.All);
            this.cmbIsConsign.ItemsSource = list;
            this.cmbIsConsign.SelectedIndex = 0;
            //采购单状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
            //待审核状态:
            this.cmbVerifyStatus.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderVerifyStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbVerifyStatus.SelectedIndex = 0;
            //采购单类型:
            this.cmbPOType.ItemsSource = EnumConverter.GetKeyValuePairs<PurchaseOrderType>(EnumConverter.EnumAppendItemType.All);
            this.cmbPOType.SelectedIndex = 0;

            this.chkAdvancedSearch.IsChecked = true;
            SwitchAdvancedSearchCondition(true);
        }

        #region [Events]

        private void chkAdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            SwitchAdvancedSearchCondition(this.chkAdvancedSearch.IsChecked.Value);
        }

        private void SwitchAdvancedSearchCondition(bool isChecked)
        {
            this.spMoreQuery.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnCreatePO_Click(object sender, RoutedEventArgs e)
        {
            //创建采购单 - 操作:
            Window.Navigate("/ECCentral.Portal.UI.PO/PurchaseOrderNew", null, true);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.SearchCondition))
            {
                return;
            }
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            queryFilter = EntityConverter<PurchaseOrderQueryVM, PurchaseOrderQueryFilter>.Convert(queryVM);
            newQueryFilter = queryFilter.DeepCopy();
            if (this.chkAdvancedSearch.IsChecked.Value)
            {
                tempFilter = queryFilter;
            }
            else
            {
                IsNormalQuery();
                tempFilter = newQueryFilter;
            }
            //TODO:权限管理，这里给查询赋予最高权限:
            tempFilter.PMQueryType = PMQueryType.All;

            this.QueryResultGrid.Bind();
        }

        private void IsNormalQuery()
        {
            newQueryFilter.InStockFrom = newQueryFilter.InStockTo = null;
            newQueryFilter.CurrencySysNo = string.Empty;
            newQueryFilter.IsApportion = string.Empty;
            newQueryFilter.IsConsign = null;
            newQueryFilter.Status = null;
            newQueryFilter.VerifyStatus = null;
            newQueryFilter.AuditUser = string.Empty;
            newQueryFilter.PMSysNo = string.Empty;
            newQueryFilter.CreatePOSysNo = string.Empty;
            newQueryFilter.POType = null;
            newQueryFilter.PrintTime = null;
            newQueryFilter.VendorSysNo = string.Empty;
        }

        private void btnSearchRMA_Click(object sender, RoutedEventArgs e)
        {
            //送修超过15天催讨未果RMAList查询 ：
            this.QueryResultGrid_RMA.Bind();
        }

        private void btnRedirectRMA_Click(object sender, RoutedEventArgs e)
        {
            //跳转到RMA - 送修未返还查询:
            Window.Navigate(string.Format("/ECCentral.Portal.UI.RMA/OutBoundNotReturnQuery/{0}|{1}", 15, 1), null, true);
        }

        private void btnCountPO_Click(object sender, RoutedEventArgs e)
        {
            //按采购单状态统计:
            //TODO:权限管理，这里给查询赋予最高权限:
            queryFilter.PMQueryType = PMQueryType.All;

            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
            };
            PurchaseOrderStatisticsQuery statisticsQueryDialog = new PurchaseOrderStatisticsQuery(queryFilter);
            statisticsQueryDialog.Dialog = Window.ShowDialog(ResPurchaseOrderQuery.AlertMsg_CountTitle, statisticsQueryDialog, (obj, args) =>
            {
            }, new Size(600, 400));
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            tempFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            tempFilter.IsManagerPM = AuthMgr.HasFunctionAbsolute(AuthKeyConst.PO_SeniorPM_Query);
            tempFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryPurchaseOrders(tempFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var consignList = args.Result.Rows.ToList();
                int totalCount = args.Result.TotalCount;
                //构造仓库列的显示
                foreach (var item in consignList)
                {
                    item.IsChecked = false;
                    if (item["ITStockSysNo"] != null && item["PaySettleCompany"] != null)
                    {
                        PaySettleITCompany paySettleITCompany = Enum.Parse(typeof(PaySettleITCompany), item["PaySettleCompany"].ToString(), true);
                        item["stockname"] = string.Format("{0}{1}", EnumConverter.GetDescription(paySettleITCompany), item["ITStockName"]);
                    }
                    else
                    {
                        item["stockname"] = string.Format("{0}{1}",EnumConverter.GetDescription(TransferType.Direct),item["stockname"]);
                    }
                }

                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = consignList;
            });
        }

        private void QueryResultGrid_RMA_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRMAFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid_RMA.PageSize,
                PageIndex = QueryResultGrid_RMA.PageIndex,
                SortBy = e.SortField
            };
            //TODO:根据权限,获取PMList:
            serviceFacade.QueryPurchaseOrdersRMAList(queryRMAFilter, (obj, args) =>
            {

                if (args.FaultsHandle())
                {
                    return;
                }
                QueryResultGrid_RMA.TotalCount = args.Result.TotalCount;
                QueryResultGrid_RMA.ItemsSource = args.Result.Rows;
            });

        }
        #endregion

        private void Hyperlink_EditPO_Click(object sender, RoutedEventArgs e)
        {
            //编辑采购单:
            DynamicXml getSelectedItem = this.QueryResultGrid.SelectedItem as DynamicXml;
            Window.Navigate(string.Format("/ECCentral.Portal.UI.PO/PurchaseOrderMaintain/{0}", getSelectedItem["sysno"].ToString()), null, true);
        }

        private void Hyperlink_Email_Click(object sender, RoutedEventArgs e)
        {
            //验证采购单是否通过审批 :
            DynamicXml getObject = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getObject)
            {
                PurchaseOrderStatus status = (PurchaseOrderStatus)Enum.Parse(typeof(PurchaseOrderStatus), getObject["status"].ToString(), false);
                if (!IsAuditedPO(status))
                {
                    Window.Alert(ResPurchaseOrderQuery.InfoMsg_Error, string.Format(ResPurchaseOrderQuery.AlertMsg_EmailError, getObject["sysno"].ToString()));
                    return;
                }
                //TODO:发送Email:
                string getPOSysNo = getObject["sysno"].ToString();

                //PO单:自动发送邮件地址:AutoMailAddress:
                string returnAutoSendMailResult = string.Empty;
                //PO单:已发送邮件MailAddress:
                string returnMailAddresResult = string.Empty;
                //供应商:Vendor MailAddress:
                string returnVendorMailResult = string.Empty;

                #region 获取、更新MailAddress:

                //自动Email:
                if (!string.IsNullOrEmpty(getPOSysNo))
                {
                    serviceFacade.LoadPurchaseOrderInfo(getPOSysNo, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        PurchaseOrderInfo getPOInfo = args.Result;
                        if (null != getPOInfo)
                        {

                            if (!string.IsNullOrEmpty(getPOInfo.PurchaseOrderBasicInfo.AutoSendMailAddress))//AutoSendMail 不为空 只显示当前AutoSendMail 为空 显示 当前登录账户Email + Vendor 的Email
                            {
                                returnAutoSendMailResult = getPOInfo.PurchaseOrderBasicInfo.AutoSendMailAddress;
                            }
                            else if (!string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && !string.IsNullOrEmpty(getPOInfo.VendorInfo.VendorFinanceInfo.AccountContactEmail))
                            {
                                returnAutoSendMailResult = CPApplication.Current.LoginUser.UserEmailAddress + ";" + getPOInfo.VendorInfo.VendorFinanceInfo.AccountContactEmail;
                            }
                            else if (!string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && string.IsNullOrEmpty(getPOInfo.VendorInfo.VendorFinanceInfo.AccountContactEmail))
                            {
                                returnAutoSendMailResult = CPApplication.Current.LoginUser.UserEmailAddress;
                            }
                            else if (string.IsNullOrEmpty(CPApplication.Current.LoginUser.UserEmailAddress) && !string.IsNullOrEmpty(getPOInfo.VendorInfo.VendorFinanceInfo.AccountContactEmail))
                            {
                                returnAutoSendMailResult = getPOInfo.VendorInfo.VendorFinanceInfo.AccountContactEmail;
                            }
                            returnVendorMailResult = getPOInfo.VendorInfo.VendorBasicInfo.EmailAddress;

                            returnMailAddresResult = getPOInfo.PurchaseOrderBasicInfo.MailAddress;
                        }

                        PurchaseOrderEmailSendMaintain mailMaintain = new PurchaseOrderEmailSendMaintain(getPOInfo.VendorInfo.SysNo.Value, returnAutoSendMailResult, returnVendorMailResult, returnMailAddresResult);
                        mailMaintain.Dialog = Window.ShowDialog(ResPurchaseOrderQuery.AlertMsg_EmailMaintain, mailMaintain, (obj2, args2) =>
                        {
                            if (args2.DialogResult == DialogResultType.OK && args2.Data != null)
                            {
                                Dictionary<string, object> getReturnDict = args2.Data as Dictionary<string, object>;
                                if (null != getReturnDict)
                                {
                                    string getUpdatedVendorEmailAddress = getReturnDict["updatedVendorMailAddress"].ToString();
                                    string getSelectedReceivedMailAddress = "";

                                    List<string> getSelectedReceivedMailAddressList = getReturnDict["updatedReceiveMailAddress"] as List<string>;
                                    if (null != getSelectedReceivedMailAddress)
                                    {
                                        getSelectedReceivedMailAddress = string.Join(";", getSelectedReceivedMailAddressList);
                                    }

                                    PurchaseOrderInfo sendMailInfo = new PurchaseOrderInfo()
                                    {
                                        PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo() { MailAddress = getSelectedReceivedMailAddress },
                                        SysNo = Convert.ToInt32(getObject["sysno"].ToString())
                                    };
                                    //更新发送邮件地址 ：
                                    serviceFacade.UpdateMailAddressAndHasSentMail(sendMailInfo, (obj3, args3) =>
                                    {
                                        if (args3.FaultsHandle())
                                        {
                                            return;
                                            //TODO:弹出Print页面进行打印:
                                            //window.open("<%= Url.Action("PrintToEmail")%>/"+InPOSysNO+"$"+InAutoSendMail);     
                                        }
                                        //打印操作:
                                        HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderPrint", new Dictionary<string, string>() { { "POSysNo", getObject["sysno"].ToString() }, { "PrintAccessory", "1" }, { "PrintTitle", "给供应商发采购邮件" } });
                                    });

                                }

                            }

                        }, new Size(600, 300));
                    });
                }
                #endregion
            }

        }

        private void Hyperlink_Print_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getObject = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getObject)
            {
                PurchaseOrderStatus status = (PurchaseOrderStatus)Enum.Parse(typeof(PurchaseOrderStatus), getObject["status"].ToString(), false);
                if (!IsAuditedPO(status))
                {
                    Window.Alert(ResPurchaseOrderQuery.InfoMsg_Error, string.Format(ResPurchaseOrderQuery.AlertMsg_EmailError, getObject["sysno"].ToString()));
                    return;
                }
                //打印操作:
                HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderPrint", new Dictionary<string, string>() { { "POSysNo", getObject["sysno"].ToString() }, { "PrintAccessory", "1" }, { "PrintTitle", ResPurchaseOrderQuery.Label_PrintTitle } });
            }
        }
        private void Hyperlink_PrintWithoutPrice_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml getObject = this.QueryResultGrid.SelectedItem as DynamicXml;
            if (null != getObject)
            {
                PurchaseOrderStatus status = (PurchaseOrderStatus)Enum.Parse(typeof(PurchaseOrderStatus), getObject["status"].ToString(), false);
                if (!IsAuditedPO(status))
                {
                    Window.Alert(ResPurchaseOrderQuery.InfoMsg_Error, string.Format(ResPurchaseOrderQuery.AlertMsg_EmailError, getObject["sysno"].ToString()));
                    return;
                }
                //打印操作:
                HtmlViewHelper.WebPrintPreview("PO", "PurchaseOrderWithoutPricePrint", new Dictionary<string, string>() { { "POSysNo", getObject["sysno"].ToString() }, { "PrintAccessory", "1" }, { "PrintTitle", ResPurchaseOrderQuery.Label_PrintTitleWithoutPirce } });
            }
        }

        

        private bool IsAuditedPO(PurchaseOrderStatus poStatus)
        {
            if (poStatus == PurchaseOrderStatus.Returned || poStatus == PurchaseOrderStatus.AutoAbandoned || poStatus == PurchaseOrderStatus.WaitingPMConfirm || poStatus == PurchaseOrderStatus.Abandoned || poStatus == PurchaseOrderStatus.Created || poStatus == PurchaseOrderStatus.WaitingAudit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            //权限控制:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_PurchaseOrder_ExportPOList))
            {
                Window.Alert("对不起，你没有权限进行此操作!");
                return;
            }
            //导出全部:
            if (null == tempFilter || this.QueryResultGrid.ItemsSource == null || this.QueryResultGrid.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据！");
                return;
            }

            PurchaseOrderQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PurchaseOrderQueryFilter>(tempFilter);
            exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };

            foreach (DataGridColumn col in QueryResultGrid.Columns)
                if (col.Visibility == Visibility.Collapsed)
                    if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn)
                        (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn).NeedExport = false;
                    else if (col is Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn)
                        (col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn).NeedExport = false;
            ColumnSet columnSet = new ColumnSet(QueryResultGrid);
            columnSet.Add("status", ResPurchaseOrderQuery.GridHeader_Status);

            serviceFacade.ExportExcelForPurchaseOrders(exportQueryRequest, new ColumnSet[] { columnSet });


        }

        private void Hyperlink_RMATracking_Click(object sender, RoutedEventArgs e)
        {
            //链接到PM跟进日志 ：
            DynamicXml getSelectedItem = this.QueryResultGrid_RMA.SelectedItem as DynamicXml;
            if (null != getSelectedItem)
            {
                Window.Navigate(string.Format(ConstValue.RMA_RegisterMemoUrl, getSelectedItem["RMASysNo"].ToString()), null, true);
            }

        }

        private void ucCreateUser_PMLoaded(object sender, EventArgs e)
        {
            //硬编码信息导致报错暂时注释掉 2012-8-30 Jack
            //this.ucCreateUser.AddAppendItem(1, 3344, "供应商");
        }

        //选择全部
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }
        }

        /// <summary>
        /// 批量打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchPrint_Click(object sender,RoutedEventArgs e)
        {
            List<int> sysNoList = new List<int>();
            List<int> noSysNoList = new List<int>();
            if (this.QueryResultGrid.ItemsSource != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        PurchaseOrderStatus status = (PurchaseOrderStatus)Enum.Parse(typeof(PurchaseOrderStatus), view.status.ToString(), false);
                        if (IsAuditedPO(status))
                        {
                            sysNoList.Add(view.sysno);
                        }
                        else
                        {
                            noSysNoList.Add(view.sysno);
                        }
                    }
                }
                if ((sysNoList != null && sysNoList.Count > 0)
                    || (noSysNoList != null && noSysNoList.Count>0))
                {
                    
                    if (noSysNoList != null && noSysNoList.Count > 0)
                    {
                        Window.Alert(string.Format(ResPurchaseOrderQuery.Message_BatchPrintDataError, string.Join("|", noSysNoList)), MessageType.Information);
                    }
                    if (sysNoList == null || sysNoList.Count <= 0)
                    {
                        return;
                    }
                    Dictionary<string, string> filter = new Dictionary<string, string>();
                    filter.Add("POSysNoList", String.Join("|", sysNoList));
                    filter.Add("PrintAccessory", "1");
                    filter.Add("PrintTitle", ResPurchaseOrderQuery.Label_PrintTitle);
                    HtmlViewHelper.WebPrintPreview("PO", "POBatchPrintData", filter);
                    
                }
                else
                {
                    Window.Alert(ResPurchaseOrderQuery.Message_SelectData, MessageType.Information);
                    return;
                }
            }

        }
    }

}
