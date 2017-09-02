using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Controls.Uploader;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 收款单自动确认
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class SaleIncomeAutoConfirm : PageBase
    {
        private SaleIncomeFacade facade;

        public SaleIncomeAutoConfirm()
        {
            InitializeComponent();
            RegisterEvents();
            InitData();
        }

        private void InitData()
        {
            this.drOutDate.SelectedDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ClearData();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermission();
            base.OnPageLoad(sender, e);
        }

        private void RegisterEvents()
        {
            this.Loaded += new RoutedEventHandler(SaleIncomeAutoConfirm_Loaded);
            this.uploader.AllFileUploadCompleted += new Basic.Controls.Uploader.AllUploadCompletedEvent(uploader_AllFileUploadCompleted);
        }

        private void SaleIncomeAutoConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new SaleIncomeFacade(this);
            
        }

        private void VerifyPermission()
        {
            this.uploader.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeAutoConfirm_UploadExcel);
            this.dgConfirmSuccessResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeAutoConfirm_SuccessConfirmExport);
            this.dgConfirmFailedsResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_SaleIncomeAutoConfirm_FaultConfirmExport);
        }

        private string successSysNoSplitString = "";
        private string failedSysNoSplitString = "";
        private string failedMessage = "";

        private void uploader_AllFileUploadCompleted(object sender, Basic.Controls.Uploader.AllUploadCompletedEventArgs args)
        {
            ClearData();
            this.uploader.Clear();
            if (args.UploadInfo[0].UploadResult == SingleFileUploadStatus.Failed)
            {
                Window.Alert(ResSaleIncomeAutoConfirm.Message_UploadFailed, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error);
                return;
            }

            facade.AutoConfirm(args.UploadInfo[0].ServerFilePath, drOutDate.SelectedDateStart, drOutDate.SelectedDateEnd, result =>
            {
                successSysNoSplitString = string.Join(",", result.SuccessSysNoList);
                failedSysNoSplitString = string.Join(",", result.FailedSysNoList);
                failedMessage = result.FailedMessage;

                string msg = string.Format(ResSaleIncomeAutoConfirm.Message_TotalStatisticInfo
                    , result.SuccessSysNoList.Count + result.FailedSysNoList.Count
                    , result.SubmitConfirmCount
                    , result.FailedSysNoList.Count
                    , result.SuccessSysNoList.Count);

                this.tbStatisticInfo.Text = msg;
                this.tbStatisticInfo.Visibility = Visibility.Visible;

                this.dgConfirmSuccessResult.Bind();
                this.dgConfirmFailedsResult.Bind();
            });
        }

        private void dgConfirmSuccessResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (!string.IsNullOrEmpty(successSysNoSplitString))
            {
                var queryVM = new SaleIncomeQueryVM();
                queryVM.OrderID = successSysNoSplitString;
                queryVM.CreateDateFrom = null;
                queryVM.CreateDateTo = null;
                facade.Query(queryVM, e.PageSize, e.PageIndex, e.SortField, result =>
                {
                    this.dgConfirmSuccessResult.ItemsSource = result.ResultList;
                    this.dgConfirmSuccessResult.TotalCount = result.TotalCount;

                    var statistic = result.Statistic.Single(w => w.StatisticType == StatisticType.Total);

                    string msg = string.Format(ResSaleIncomeAutoConfirm.Message_SuccessStatisticInfo
                        , ConstValue.Invoice_ToCurrencyString(statistic.OrderAmt)
                        , ConstValue.Invoice_ToCurrencyString(statistic.IncomeAmt)
                        , ConstValue.Invoice_ToCurrencyString(statistic.ShipPrice)
                        , ConstValue.Invoice_ToCurrencyString(statistic.ReturnCash)
                        , statistic.ReturnPoint
                        , ConstValue.Invoice_ToCurrencyString(statistic.ToleranceAmt));
                    this.tbSuccessInfo.Text = msg;
                    this.tbSuccessInfo.Visibility = Visibility.Visible;
                });
            }
        }

        private void dgConfirmFailedsResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (!string.IsNullOrEmpty(failedSysNoSplitString))
            {
                facade.QuerySO(failedSysNoSplitString, e.PageSize, e.PageIndex, e.SortField, result =>
                {
                    this.dgConfirmFailedsResult.ItemsSource = result[0].Rows;
                    this.dgConfirmFailedsResult.TotalCount = result[0].TotalCount;

                    string msg = string.Format(ResSaleIncomeAutoConfirm.Message_FailedStatisticInfo,
                        ConstValue.Invoice_ToCurrencyString(result[1].Rows[0].TotalAmount));
                    this.tbFaultInfo.Text = msg;
                    this.tbFaultInfo.Visibility = Visibility.Visible;
                });
            }
        }

        /// <summary>
        /// 查看失败原因
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFaultMsg_Click(object sender, RoutedEventArgs e)
        {
            var viewer = new UCMessageViewer();
            viewer.Message = failedMessage;
            viewer.ShowDialog(ResSaleIncomeAutoConfirm.Message_GetFailedMsgDlgTitle, null);
        }

        private void ClearData()
        {
            this.dgConfirmSuccessResult.ItemsSource = null;
            this.dgConfirmFailedsResult.TotalCount = 0;
            this.dgConfirmFailedsResult.ItemsSource = null;
            this.dgConfirmFailedsResult.TotalCount = 0;
            this.successSysNoSplitString = "";
            this.failedSysNoSplitString = "";
            this.failedMessage = "";

            this.tbStatisticInfo.Visibility = Visibility.Collapsed;
            this.tbSuccessInfo.Visibility = Visibility.Collapsed;
            this.tbFaultInfo.Visibility = Visibility.Collapsed;
        }

        private void dgConfirmSuccessResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(successSysNoSplitString))
            {
                var queryVM = new SaleIncomeQueryVM();
                queryVM.OrderID = successSysNoSplitString;

                ColumnSet cols = new ColumnSet(dgConfirmSuccessResult);
                facade.ExportSuccessResult(queryVM, new ColumnSet[] { cols });

            }
        }

        private void dgConfirmFailedsResult_ExportAllClick(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(failedSysNoSplitString))
            {

                ColumnSet cols = new ColumnSet(dgConfirmFailedsResult);

                facade.ExportConfirmFailed(failedSysNoSplitString, new ColumnSet[] { cols });


            }
        }
    }
}