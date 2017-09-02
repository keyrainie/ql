using System;
using System.Collections.Generic;
using System.Windows;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.UI.ExternalSYS.UserControls.EIMS;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.ExternalSYS;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class ARReceive : PageBase
    {
        ReceivedReportQueryFilter m_queryRequest;
        List<ARReceiveByInvoiceVM> ArReceiveList;

        public ARReceive()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.SearchCondition.DataContext = m_queryRequest = new ReceivedReportQueryFilter();
            BindComboBoxData();
            //this.dgQueryResult.Bind();
        }


        /// <summary>
        /// 查询按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.dgQueryResult.Bind();
        }

        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };

            m_queryRequest.PagingInfo.SortBy = e.SortField;

            ReceivedReportFacade facade = new ReceivedReportFacade(this);
            facade.ARReceiveQuery(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
                ArReceiveList = DynamicConverter<ARReceiveByInvoiceVM>.ConvertToVMList(args.Result.Rows);
                this.dgQueryResult.ItemsSource = ArReceiveList;

                this.myFooter.Visibility = ((ArReceiveList != null && ArReceiveList.Count != 0)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                ARReceiveSumVM sumVm = GetSumARReceive(ArReceiveList);
                this.myFooter.DataContext = sumVm;
            });
        }

        /// <summary>
        /// 绑定下拉列表数据
        /// </summary>
        private void BindComboBoxData()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_EIMSType,
               CodeNamePairAppendItemType.All, (o, p) =>
               {
                   this.cmbFeeType.ItemsSource = p.Result;
                   this.cmbFeeType.SelectedIndex = 0;
               });
        }

        /// <summary>
        /// 导出数据报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_ARReceive_Export))
            {
                Window.Alert(ResEIMSReceiveReport.Msg_HasNoRight);
                return;
            }
            if (dgQueryResult.ItemsSource == null)
            {
                Window.Alert(ResEIMSReceiveReport.Msg_PleaseQueryData);
                return;
            }
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            ReceivedReportFacade facade = new ReceivedReportFacade(this);

            ColumnSet col = new ColumnSet(dgQueryResult);
            facade.ExportARReceive(m_queryRequest, new ColumnSet[] { col });

        }

        /// <summary>
        /// 查看明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtn_ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            ARReceiveByInvoiceVM selectedModel = this.dgQueryResult.SelectedItem as ARReceiveByInvoiceVM;
            if (selectedModel != null)
            {
                m_queryRequest.VendorSysNo = Convert.ToInt32(selectedModel.VendorNumber);
                ARReceiveDetails details = new ARReceiveDetails(m_queryRequest);
                details.Dialog = Window.ShowDialog("单据详细信息", details, (obj, args) =>
                {
                    m_queryRequest.VendorSysNo = null;
                }, new Size(1000, 600)
               );
            }
        }

        //统计汇总信息
        private ARReceiveSumVM GetSumARReceive(List<ARReceiveByInvoiceVM> resultList)
        {
            ARReceiveSumVM sumResult = new ARReceiveSumVM();
            sumResult.SumAccruedTotal = 0;
            sumResult.SumUnbilled = 0;
            sumResult.SumOpenTotal = 0;
            sumResult.SumNoReceived = 0;
            sumResult.SumDueIn30 = 0;
            sumResult.SumDueBetween31And60 = 0;
            sumResult.SumDueBetween61And90 = 0;
            sumResult.SumDueBetween91And120 = 0;
            sumResult.SumDueBetween121And150 = 0;
            sumResult.SumDueBetween151And180 = 0;
            sumResult.SumDueOver180 = 0;
            sumResult.SumUnbilledPercentage = 0;
            sumResult.SumOpenTotalPercentage = 0;
            sumResult.SumDueIn30Percentage = 0;
            sumResult.SumDueBetween31And60Percentage = 0;
            sumResult.SumDueBetween61And90Percentage = 0;
            sumResult.SumDueBetween91And120Percentage = 0;
            sumResult.SumDueBetween121And150Percentage = 0;
            sumResult.SumDueBetween151And180Percentage = 0;
            sumResult.SumDueOver180Percentage = 0;

            if (resultList != null)
            {
                foreach (ARReceiveByInvoiceVM entity in resultList)
                {
                    sumResult.SumAccruedTotal += entity.SumAccruedTotal;
                    sumResult.SumUnbilled += entity.SumUnbilled;
                    sumResult.SumOpenTotal += entity.SumOpenTotal;
                    sumResult.SumNoReceived += entity.SumNoReceived;
                    sumResult.SumDueIn30 += entity.DueIn30;
                    sumResult.SumDueBetween31And60 += entity.DueBetween31And60;
                    sumResult.SumDueBetween61And90 += entity.DueBetween61And90;
                    sumResult.SumDueBetween91And120 += entity.DueBetween91And120;
                    sumResult.SumDueBetween121And150 += entity.DueBetween121And150;
                    sumResult.SumDueBetween151And180 += entity.DueBetween151And180;
                    sumResult.SumDueOver180 += entity.DueOver180;
                }
            }

            if (sumResult.SumAccruedTotal > 0)
            {
                sumResult.SumUnbilledPercentage = sumResult.SumUnbilled / sumResult.SumAccruedTotal;
                sumResult.SumOpenTotalPercentage = sumResult.SumOpenTotal / sumResult.SumAccruedTotal;
            }

            if (sumResult.SumNoReceived > 0)
            {
                sumResult.SumDueIn30Percentage = sumResult.SumDueIn30 / sumResult.SumNoReceived;
                sumResult.SumDueBetween31And60Percentage = sumResult.SumDueBetween31And60 / sumResult.SumNoReceived;
                sumResult.SumDueBetween61And90Percentage = sumResult.SumDueBetween61And90 / sumResult.SumNoReceived;
                sumResult.SumDueBetween91And120Percentage = sumResult.SumDueBetween91And120 / sumResult.SumNoReceived;
                sumResult.SumDueBetween121And150Percentage = sumResult.SumDueBetween121And150 / sumResult.SumNoReceived;
                sumResult.SumDueBetween151And180Percentage = sumResult.SumDueBetween151And180 / sumResult.SumNoReceived;
                sumResult.SumDueOver180Percentage = sumResult.SumDueOver180 / sumResult.SumNoReceived;
            }

            return sumResult;
        }
    }
}
