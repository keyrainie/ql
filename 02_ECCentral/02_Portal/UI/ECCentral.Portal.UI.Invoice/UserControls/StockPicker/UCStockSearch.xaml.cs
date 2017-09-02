using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Invoice.UserControls.StockPicker
{
    public partial class StockSearch : PopWindow
    {
        public StockSearch()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(StockSearch_Loaded);
        }

        private List<string> m_StockSysNoList;

        public StockSearch(string stockSysNo)
            : this()
        {
            m_StockSysNoList = stockSysNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToList();
        }

        private void StockSearch_Loaded(object sender, RoutedEventArgs e)
        {
            CommonDataFacade _facadeCommon = new CommonDataFacade(CPApplication.Current.CurrentPage);
            _facadeCommon.GetStockList(false, (_, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var dataSource = EntityConverter<StockInfo, StockCheckBoxVM>.Convert(args.Result, (s, v) =>
                {
                    v.StockSysNo = s.SysNo.ToString();
                });
                if (dataSource != null)
                {
                    dataSource.ForEach(w => w.IsChecked = m_StockSysNoList.Contains(w.StockSysNo));
                }
                this.StockList.ItemsSource = dataSource;
            });
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.StockList.ItemsSource as List<StockCheckBoxVM>;
            if (dataSource != null)
            {
                var data = dataSource.Where(w => w.IsChecked)
                    .Select(s => s)
                    .ToList();
                CloseDialog(data, DialogResultType.OK);
            }
        }
    }
}