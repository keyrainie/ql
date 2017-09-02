using System;
using System.Windows;

using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Converters;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class SOOriginNoteCollection : PageBase
    {
        public SOOriginNoteCollection()
        {

            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IntiPageData();
        }

        private void IntiPageData()
        {
            dataGrid.Bind();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
           dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            DefaultQueryFilter query = new DefaultQueryFilter();
            query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            query.CompanyCode = CPApplication.Current.CompanyCode;
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.OZZOOriginNoteQuery(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                foreach (var item in args.Result.Rows)
                { 
                    string lastEdit = string.Empty;
                    if (item["LastEditUserName"] != null && item["LastEditUserName"].ToString() != "")
                    {
                        lastEdit = string.Format("{0}[{1}]", item["LastEditUserName"], ((DateTime)item["LastEditDate"]).ToString(ResConverter.DateTime_LongFormat));
                    }
                    item["LastEdit"] = lastEdit;
                }
                dataGrid.TotalCount = args.Result.TotalCount;
                dataGrid.ItemsSource = args.Result.Rows;
            });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml info = this.dataGrid.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, info["SoID"]), null, true);
            }
        }
    }
}
