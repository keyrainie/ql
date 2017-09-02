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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.UI.PO.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Models.Settlement;
using ECCentral.BizEntity.PO.Settlement;

namespace ECCentral.Portal.UI.PO.Views
{
    /// <summary>
    /// 
    /// 经销商品结算单创建：
    /// 针对经销商品入库生成的采购应付款，负采购生成的采购应付款，进价变价生成的成本变价单，财务在进行结算时，根据需要结算的单据勾选其中部分未付款的采购应付款，生成新的经销商品结算单进行结算
    /// 
    /// </summary>
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class SettleOrderCreate : PageBase
    {
        #region Properties

        SettleOrderCreateQueryVM queryVM = null;

        ConsignSettlementFacade facade = null;

        List<SettleOrderFinancePayItemVM> list = null;

        #endregion

        #region Constructor

        public SettleOrderCreate()
        {
            InitializeComponent();

            queryVM = new SettleOrderCreateQueryVM();
            QueryBuilder.DataContext = queryVM;
            list = new List<SettleOrderFinancePayItemVM>();
            totalAmount.Text = (0).ToString("C");
        }

        #endregion

        #region Override

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new ConsignSettlementFacade(this);
        }

        #endregion

        #region Control Event

        private void DataGrid_QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            ValidationManager.Validate(this.QueryBuilder);
            if (queryVM.HasValidationErrors)
            {
                return;
            }

            //验证单据编号输入格式

            List<int> orderSysNoList = new List<int>();
            string orderSysNoStrs = queryVM.OrderSysNoStrs.Trim();
            if (!string.IsNullOrEmpty(orderSysNoStrs))
            {
                string[] orderSysNoArray = orderSysNoStrs.Split(new string[] { ".",","," " },StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < orderSysNoArray.Length; i++)
                {
                    int tmp = 0;
                    if (int.TryParse(orderSysNoArray[i].Trim(), out tmp))
                    {
                        orderSysNoList.Add(tmp);
                    }
                    else
                    {
                        Window.Alert("单据编号输入有误！");
                        return;
                    }
                }
            }

            facade.QuerySettleAccountWithOrigin(queryVM, orderSysNoList, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    list = DynamicConverter<SettleOrderFinancePayItemVM>.ConvertToVMList(args.Result.Rows);

                    foreach (var sub in list)
                    {
                        sub.Cost = sub.Cost13 + sub.Cost17 + sub.CostOt;
                        sub.Tax = sub.Tax13 + sub.Tax17 + sub.TaxOt;

                        sub.OrderTypeStr = GetTypeStr(sub.OrderType);

                        sub.IsChecked = false;
                    }
                    int totalCount = args.Result.TotalCount;

                    this.DataGrid_QueryResult.ItemsSource = list;
                    this.DataGrid_QueryResult.TotalCount = totalCount;

                    totalAmount.Text = (0).ToString("C");//每一重新请求数据后，总计都为O;
                });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_QueryResult.Bind();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            SettleInfo SettlenIfo = new BizEntity.PO.Settlement.SettleInfo();
            SettlenIfo.SettleItemInfos = new List<SettleItemInfo>();

            var amount = 0m;
            foreach (var sub in list)
            {
                if (sub.IsChecked.Value == true)
                {
                    SettlenIfo.SettleItemInfos.Add(new SettleItemInfo()
                    {
                        OrderSysNo = sub.OrderSysNo,
                        FinancePayOrderType = sub.FinancePayOrderType,
                        FinancePaySysNo = sub.FinancePaySysNo
                    });

                    amount += sub.Amount.Value;
                }
            }

            if (SettlenIfo.SettleItemInfos.Count <= 0)
            {
                Window.Alert("提示", "请选择单据",MessageType.Warning);
                return;
            }

            //总金额
            SettlenIfo.TotalAmt = amount;
            //商家编号（经销商品结算单对应一个特定的商家）
            SettlenIfo.VendorSysNo = queryVM.VendorSysNo;

            facade.CreateSettleAccount(SettlenIfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Window.Alert("提示", "创建成功", MessageType.Information, (obj2, args2) =>
                {
                    btnSearch_Click(null, null);
                });
            });
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                if (list != null)
                {
                    foreach (var sub in list)
                    {
                        sub.IsChecked = (ckb.IsChecked != null ? ckb.IsChecked.Value : false);
                    }
                }
            }
            SetTotalAmount();
        }

        private void SingleCheckClick(object sender, RoutedEventArgs e)
        {
           //计算当前被选中的值
            SetTotalAmount();
        }

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e) { }

        #endregion

        #region OtherHelperMethod

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

        private void SetTotalAmount()
        {
            decimal result = 0m;
            foreach (var sub in list)
            {
                if (sub.IsChecked.Value == true)
                {
                    result += sub.Amount.Value;
                }      
            }

            totalAmount.Text = result.ToString("C");

        }

        #endregion
    }
}
