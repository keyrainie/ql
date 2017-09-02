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
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO.Settlement;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.PO.Views
{
    /// <summary>
    /// 
    /// 经销商品结算单 详细信息页面：
    /// 
    /// 1.查看详情，2.审核，3.打印
    /// 
    /// </summary>
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class SettleDetailView : PageBase
    {
        #region Properties

        int? SettleSysNo = null;

        ConsignSettlementFacade facade = null;

        #endregion

        #region Constructor

        public SettleDetailView()
        {
            InitializeComponent();
        }

        #endregion

        #region Override

        public override void OnPageLoad(object sender, EventArgs e)
        {

            facade = new ConsignSettlementFacade(this);
            //获取编号 获取数据
            var SettleSysNoStr = this.Request.Param;
            if (SettleSysNoStr != null)
            {
                SettleSysNo = int.Parse(SettleSysNoStr);
            }
            //加载数据
            InitGetDataToShow(SettleSysNo.Value);
            //设置按钮

        }

        #endregion

        #region DataEvent

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnAudit_Click(object sender, RoutedEventArgs e) {

            SettleInfo settleInfo = new SettleInfo() { SysNo = SettleSysNo, Status = POSettleStatus.AuditPassed };
            facade.AuditSettleAccount(settleInfo,(obj,args)=>{
                if(args.FaultsHandle())
                {
                    return;
                }
                else
                {
                     Window.Alert("提示", "审核成功", MessageType.Information, (obj2, args2) =>
                    {
                        Window.Refresh();
                    });
                }
            } );
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnAbandon_Click(object sender, RoutedEventArgs e) {
            SettleInfo settleInfo = new SettleInfo() { SysNo = SettleSysNo, Status = POSettleStatus.Abandon };
            facade.AuditSettleAccount(settleInfo,(obj,args)=>{
                if(args.FaultsHandle())
                {
                    return;
                }
                else
                {
                     Window.Alert("提示", "作废成功", MessageType.Information, (obj2, args2) =>
                    {
                        Window.Refresh();
                    });
                }
            } );
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnPrint_Click(object sender, RoutedEventArgs e) {
            HtmlViewHelper.WebPrintPreview("PO", "SettleProductPrint", new Dictionary<string, string>() { { "SettleSysNo", SettleSysNo.Value.ToString() } });
        }

        #endregion

        #region OtherHelperMethod

        private void InitGetDataToShow(int settleSysNo)
        {
            SettleInfo settleInfo = new BizEntity.PO.Settlement.SettleInfo();
            settleInfo.SysNo = settleSysNo;
            facade.GetSettleAccount(settleInfo, (obj, args) => {
                if (args.FaultsHandle())
                {
                    return;
                }
                settleInfo = args.Result ;

                 //显示数据
                txtSettleSysNo.Text = settleSysNo.ToString();
                txtVendorSysNo.Text = settleInfo.VendorSysNo.Value.ToString();
                txtVendorName.Text = settleInfo.VendorName.ToString();
                txtStatus.Text = EnumConverter.GetDescription(settleInfo.Status,typeof(POSettleStatus));
                txtTotal.Text = settleInfo.TotalAmt.Value.ToString("C");

                foreach (var sub in settleInfo.SettleItemInfos)
                {
                    sub.Cost = sub.Cost13 + sub.Cost17 + sub.CostOther;
                    sub.RateAmount = sub.Rate13 + sub.Rate17 + sub.RateOther;

                    sub.OrderTypeStr = GetTypeStr(sub.OrderType);
                }

                QueryResultGrid.ItemsSource = settleInfo.SettleItemInfos;

                POSettleStatus status = settleInfo.Status.Value;

                if (status == POSettleStatus.Created)
                {
                    btnAbandon.IsEnabled = true;
                    btnAudit.IsEnabled =true;
                }
                else if (status == POSettleStatus.AuditPassed)
                {
                    btnPrint.IsEnabled = true;
                }
                else if (status == POSettleStatus.Abandon)
                {
 
                }
            });
        }


        private string GetTypeStr(int? orderType)
        {
            if (orderType.HasValue)
            {
                switch (orderType)
                {
                    case 3: return "进货单";
                    case 5: return "返厂单";
                    case 7: return "进价变价单";
                    default: return "";
                }
            }
            else
            {
                return "";
            }
        }


        #endregion

    }
}
