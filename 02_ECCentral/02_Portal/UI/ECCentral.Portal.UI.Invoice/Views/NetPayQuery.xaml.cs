using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 核对网上支付查询页面
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class NetPayQuery : PageBase
    {
        private NetPayQueryVM queryVM;
        private NetPayQueryVM lastQueryVM;
        private NetPayFacade facade;

        public NetPayQuery()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(NetPayQuery_Loaded);
        }

        private void NetPayQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(NetPayQuery_Loaded);
            facade = new NetPayFacade(this);
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.queryVM = new NetPayQueryVM();
            this.gridQueryBuilder.DataContext = lastQueryVM = queryVM;

            this.LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            CommonDataFacade commonFacade = new CommonDataFacade(this);

            // 绑定分仓列表
            commonFacade.GetStockList(true, (obj, args) =>
            {
                this.queryVM.StockList = args.Result;
            });
            // 绑定配送日期范围
            CodeNamePairHelper.GetList(ConstValue.DomainName_Common, ConstValue.Key_TimeRange, CodeNamePairAppendItemType.Custom_All, (obj, args) =>
            {
                this.queryVM.DeliveryTimeRangeList = args.Result;
            });
            cmbChannel.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (queryVM.AmountFrom > queryVM.AmountTo)
            {
                tbAmtFrom.Validation(ResNetPayQuery.Message_LessThanOrEqualValue);
                tbAmtTo.Validation(ResNetPayQuery.Message_MoreThanOrEqualValue);
                return;
            }
            else
            {
                tbAmtFrom.ClearValidationError();
                tbAmtTo.ClearValidationError();
            }
            var flag = ValidationManager.Validate(this.gridQueryBuilder);
            if (!flag)
                return;

            this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<NetPayQueryVM>(queryVM);

            this.dgNetPayQueryResult.Bind();
        }

        private void dgNetPayQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.Query(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.dgNetPayQueryResult.ItemsSource = result.ResultList;
                this.dgNetPayQueryResult.TotalCount = result.TotalCount;
            });
        }

        /// <summary>
        /// 创建网上支付
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_NetPay_New))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;

            }


            new UCNetPayMaintain().ShowDialog(ResNetPayQuery.Message_Create, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    dgNetPayQueryResult.Bind();
                }
            });
        }

        //选择全部
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var itemSource = dgNetPayQueryResult.ItemsSource as List<NetPayVM>;
            if (itemSource != null)
            {
                itemSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        /// <summary>
        /// 单条件记录审核操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionAbsolute(AuthKeyConst.Invoice_NetPay_Approve))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
 
            }

            int netpaySysNo = int.Parse((sender as HyperlinkButton).Tag.ToString());
            new UCNetPayMaintain(netpaySysNo).ShowDialog(ResNetPayQuery.Message_Audit, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    dgNetPayQueryResult.Bind();
                }
            });
        }

        /// <summary>
        /// 单条件记录作废操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnAbandon_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_NetPay_Abandon))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
 
            }


            int netpaySysNo = int.Parse((sender as HyperlinkButton).Tag.ToString());
            Window.Confirm(ResNetPayQuery.Message_ConfirmAbandonContent, () =>
            {
                facade.Abandon(netpaySysNo, () => dgNetPayQueryResult.Bind());
            });
        }

        /// <summary>
        /// 链接到订单维护页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            string orderSysNo = (sender as HyperlinkButton).Content.ToString();
            Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, orderSysNo), null, true);
        }

        /// <summary>
        /// 批量审核操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchAudit_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count == 0)
            {
                Window.Alert(ResNetPayQuery.Message_AtLeastChooseOneRecord);
                return;
            }

            facade.BatchAudit(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgNetPayQueryResult.Bind());
            });
        }

        private List<int> GetSelectedSysNoList()
        {
            var selectedSysNoList = new List<int>();
            var itemSource = dgNetPayQueryResult.ItemsSource as List<NetPayVM>;
            if (itemSource != null)
            {
                selectedSysNoList = itemSource.Where(w => w.IsChecked == true)
                    .Select(s => s.SysNo.Value)
                    .ToList();
            }
            return selectedSysNoList;
        }
    }
}