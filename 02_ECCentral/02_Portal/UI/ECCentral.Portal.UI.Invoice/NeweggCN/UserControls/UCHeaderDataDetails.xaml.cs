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
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCHeaderDataDetails : PopWindow
    {
        private HeaderDataFacade facade;

        HeaderDataQueryFilter m_filter;

        public HeaderDataQueryFilter Filter
        {
            get { return m_filter; }
            set { m_filter = value; }
        }

        public UCHeaderDataDetails()
        {
            InitializeComponent();
        }

        public UCHeaderDataDetails(HeaderDataQueryFilter filter)
        {
            this.Filter = filter;
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCHeaderDataDetails_Loaded);
        }

        void UCHeaderDataDetails_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new HeaderDataFacade();
            this.dgQueryResult.Bind();
        }

        //重置数据
        private void btnDataReset_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = GetSelectedTransactionNumber();
            if (list == null || list.Count == 0)
            {
                CurrentWindow.Alert(ResHeaderDataQuery.Msg_PleaseSelect);
                return;
            }
            facade.UpdateSAPStatus(list, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    if (args.Result > 0)
                    {
                        CurrentWindow.Alert(ResHeaderDataQuery.Msg_OpreateSucess);
                        CloseDialog(DialogResultType.OK);
                    }
                });

        }

        //全选
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var items = this.dgQueryResult.ItemsSource as dynamic;
            if (items != null)
            {
                var cbx = sender as CheckBox;
                foreach (var item in items)
                {
                    item.IsChecked = cbx.IsChecked ?? false;
                }
            }
        }

        //获取选中的TransactionNumber集合
        private List<int> GetSelectedTransactionNumber()
        {
            List<int> TransactionNumbers = new List<int>();
            var items = this.dgQueryResult.ItemsSource as dynamic;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.IsChecked == true)
                    {
                        TransactionNumbers.Add(item.TransactionNumber);
                    }
                }
            }
            return TransactionNumbers;
        }

        //查询明细结果
        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.HeaderDataDetailsQuery(Filter, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    this.dgQueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                    this.dgQueryResult.TotalCount = args.Result.TotalCount;
                });
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseDialog(DialogResultType.Cancel);
        }
    }
}
