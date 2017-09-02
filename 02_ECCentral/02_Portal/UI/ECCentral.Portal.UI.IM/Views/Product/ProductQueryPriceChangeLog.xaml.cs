using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.UserControls;
using System.Collections.Generic;
using System.Windows.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductQueryPriceChangeLog :PageBase
    {
        ProductQueryPriceChangeLogQueryVM model;
        public ProductQueryPriceChangeLog()
        {
            InitializeComponent();
            this.QueryPriceChangeLogQueryResult.LoadingDataSource += 
                new EventHandler<LoadingDataEventArgs>(QueryPriceChangeLogQueryResult_LoadingDataSource);
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductQueryPriceChangeLogQueryVM();
            this.DataContext = model;
            this.cbPriceLogType.ItemsSource = model.ListProductPriceType;
            this.cbPriceLogType.DisplayMemberPath = "ItemValue";
            this.cbPriceLogType.SelectedIndex = 0;
            this.cbPriceLogType.SelectionChanged += (obj, arg) =>
            {
                model.PriceLogType = ((cbItem)cbPriceLogType.SelectedItem).ItemKey;
            };
            int ProduictSysNo = 0;
            if (int.TryParse(Request.Param, out ProduictSysNo))
            {
                model.ProductSysno = ProduictSysNo.ToString();
                this.QueryPriceChangeLogQueryResult.Bind();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
           
            this.QueryPriceChangeLogQueryResult.Bind();
        }

        void QueryPriceChangeLogQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductQueryPriceChangeLogFacade facade = new ProductQueryPriceChangeLogFacade();
            facade.GetProductQueryPriceChangeLog(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                this.QueryPriceChangeLogQueryResult.ItemsSource = arg.Result.Rows;
                this.QueryPriceChangeLogQueryResult.TotalCount = arg.Result.TotalCount;

            });
        }

      

    }
    public class PriceLogTypeConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           string result;
           switch (value.ToString())
           {
               case "CountDown":
                   result = "限时促销调价";
                   break;
               case "PMAdjust":
                   result = "PM申请调价";
                   break;
               case "ComparePrice":
                   result = "比价系统自动调价";
                   break;
               case "Seller":
                   result = "商家调价";
                   break;
               case "GroupBuying":
                   result = "团购调价";
                    break;
               default:
                    result = "";
                    break;
            }
           return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
   
}
