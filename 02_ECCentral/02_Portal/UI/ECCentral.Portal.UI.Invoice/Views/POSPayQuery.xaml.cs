using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 核对POS支付查询
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class POSPayQuery : PageBase
    {
        private POSPayQueryVM queryVM;
        private POSPayQueryVM lastQueryVM;
        private POSPayFacade m_POSPayFacade;
        private SaleIncomeFacade m_SOIncomeFace;

        private DependencyProperty TotalForPageDenpendency = DependencyProperty.Register("TotalForPage", typeof(POSPayQueryStatisticVM), typeof(POSPayQuery), null);
        private DependencyProperty TotalForAllDenpendency = DependencyProperty.Register("TotalForAll", typeof(POSPayQueryStatisticVM), typeof(POSPayQuery), null);
        private DependencyProperty IsShowTotalInfoDenpendency = DependencyProperty.Register("IsShowTotalInfo", typeof(Visibility), typeof(POSPayQuery), null);


        public POSPayQueryStatisticVM TotalForPage
        {
            get
            {
                return (POSPayQueryStatisticVM)GetValue(TotalForPageDenpendency);
            }
            set
            {
                SetValue(TotalForPageDenpendency, value);
            }
        }

        public POSPayQueryStatisticVM TotalForAll
        {
            get
            {
                return (POSPayQueryStatisticVM)GetValue(TotalForAllDenpendency);
            }
            set
            {
                SetValue(TotalForAllDenpendency, value);
            }
        }

        public Visibility IsShowTotalInfo
        {
            get
            {
                return (Visibility)GetValue(IsShowTotalInfoDenpendency);
            }
            set
            {
                SetValue(IsShowTotalInfoDenpendency, value);
            }
        }


        public POSPayQuery()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(POSPayQuery_Loaded);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            VerifyPermissions();
            base.OnPageLoad(sender, e);

        }

        private void POSPayQuery_Loaded(object sender, RoutedEventArgs e)
        {

            Loaded -= new RoutedEventHandler(POSPayQuery_Loaded);


            m_POSPayFacade = new POSPayFacade(this);
            m_SOIncomeFace = new SaleIncomeFacade(this);

            queryVM = new POSPayQueryVM();
            this.SeachBuilder.DataContext = lastQueryVM = queryVM;

            this.cmbChannel.SelectedIndex = 0;

            this.TotalInfoPannel.DataContext = this;

            this.IsShowTotalInfo = Visibility.Collapsed;

            this.TotalForPage = new POSPayQueryStatisticVM();
            this.TotalForAll = new POSPayQueryStatisticVM();
        }

        private void VerifyPermissions()
        {
            this.dgPOSPayQueryResult.IsShowAllExcelExporter = AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_POSPay_Export);

        }

        private void btnBatchConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_POSPay_BatchConfirm))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            var selectedSysNoList = GetSelectedSOIncomeSysNoList();
            if (selectedSysNoList.Count <= 0)
            {
                Window.Alert(ResCommon.Message_AtLeastChooseOneRecord);
                return;
            }
            //TODO:调用中蛋定制化的Service Facade方法，需要将权限信息传到服务端做数据隔离
            m_SOIncomeFace.BatchConfirm(selectedSysNoList, false, (msg) =>
            {
                Window.Alert(msg, () => dgPOSPayQueryResult.Bind());
            });
        }

        private List<int> GetSelectedSOIncomeSysNoList()
        {
            var selectedSysNoList = new List<int>();
            var itemSource = dgPOSPayQueryResult.ItemsSource as List<POSPayVM>;
            if (itemSource != null)
            {
                selectedSysNoList = itemSource.Where(w => w.IsChecked == true)
                    .Select(s => s.SOIncomeSysNo.Value)
                    .ToList();
            }
            return selectedSysNoList;
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = dgPOSPayQueryResult.ItemsSource as List<POSPayVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = (((System.Windows.Controls.CheckBox)sender).IsChecked ?? false)
                    && w.AutoConfirmStatus == AutoConfirmStatus.Fault
                    && w.SOIncomeStatus == SOIncomeStatus.Origin);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<POSPayQueryVM>(queryVM);

                this.dgPOSPayQueryResult.Bind();
            }
        }

        private void dgPOSPayQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            m_POSPayFacade.QueryPOSPayConfirmList(lastQueryVM, e.PageSize, e.PageIndex, e.SortField,
                (result) =>
                {
                    //绑定查询结果列表
                    this.dgPOSPayQueryResult.ItemsSource = result.ResultList;
                    this.dgPOSPayQueryResult.TotalCount = result.TotalCount;
                    this.IsShowTotalInfo = Visibility.Visible;
                    if (result.Statistic != null
                        && result.Statistic.Count > 0)
                    {
                        POSPayQueryStatisticVM totalForAll = result.Statistic.FirstOrDefault(item => item.StatisticType == StatisticType.Total);
                        POSPayQueryStatisticVM totalForPage = result.Statistic.FirstOrDefault(item => item.StatisticType == StatisticType.Page);
                        totalForAll = totalForAll ?? new POSPayQueryStatisticVM();
                        totalForPage = totalForPage ?? new POSPayQueryStatisticVM();
                        this.TotalForAll = totalForAll;
                        this.TotalForPage = totalForPage;
                    }
                    else
                    {
                        //Window.Alert("Warning", "搞毛线，没有数据");
                        Window.Alert("Warning", ResPOSPayQuery.Msg_HaveNoData);
                    }
                    //TODO:绑定统计信息
                });
        }

        /// <summary>
        /// 跳转到订单维护页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            var row = dgPOSPayQueryResult.SelectedItem as POSPayVM;
            if (row != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, row.SOSysNo), null, true);
            }
        }

        /// <summary>
        /// 跳转到顾客维护页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnCustomerSysNo_Click(object sender, RoutedEventArgs e)
        {
            var row = dgPOSPayQueryResult.SelectedItem as POSPayVM;
            if (row != null)
            {
                Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, row.CustomerSysNo), null, true);
            }
        }
    }
}