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
using ECCentral.Portal.UI.MKT.NeweggCN.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SaleDiscountRuleQuery : PageBase
    {
        private SaleDiscountRuleQueryVM _queryVM;
        public SaleDiscountRuleQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _queryVM = new SaleDiscountRuleQueryVM();
            this.GridFilter.DataContext = _queryVM;
            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SaleDiscountRuleStatus>(EnumConverter.EnumAppendItemType.All);
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件，分页信息
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            p.SortBy = string.IsNullOrEmpty(p.SortBy) ? " a.SysNo desc " : p.SortBy;
            SaleDiscountRuleFacade facade = new SaleDiscountRuleFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                
                var rows = args.Result.Rows;
                foreach (var row in rows)
                {
                    if (row.RuleType == SaleDiscountRuleType.AmountRule)
                    {
                        row.Min = row.MinAmt;
                        row.Max = row.MaxAmt;
                    }
                    else
                    {
                        row.Min = row.MinQty;
                        row.Max = row.MaxQty;
                    }
                }
                this.DataGrid.TotalCount = args.Result.TotalCount;
                this.DataGrid.ItemsSource = rows;
            });
        }

        private string GetCustomerRankNames(Dictionary<string, string> ranks, string[] arr)
        {
            string rankNames = "";
            foreach (var item in arr)
            {
                string name = "";
                if (ranks.TryGetValue(item, out name))
                {
                    rankNames += name + ",";
                }
            }
            return rankNames.TrimEnd(',');
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            if (row != null)
            {
                Window.Navigate(string.Format(ConstValue.MKT_SaleDiscountRuleMaintainUrlFormat, row.SysNo), null, true);
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_SaleDiscountRuleMaintainUrlFormat, ""), null, true);
        }
    }
}
