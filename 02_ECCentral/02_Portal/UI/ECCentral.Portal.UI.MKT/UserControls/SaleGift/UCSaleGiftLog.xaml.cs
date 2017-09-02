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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.MKT.UserControls.SaleGift
{
    public partial class UCSaleGiftLog : UserControl
    {
        public int ProductSysNo;
        public IDialog Dialog { get; set; }




        public UCSaleGiftLog()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSaleGift_Loaded);
        }

        void UCSaleGift_Loaded(object sender, RoutedEventArgs e)
        {
            this.dgGiftProductLog.Bind();
        }

        private void dgGiftProductLog_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            SaleGiftFacade facade = new SaleGiftFacade(CPApplication.Current.CurrentPage);
            SaleGiftLogQueryFilterViewModel model = new SaleGiftLogQueryFilterViewModel();
            model.ProductSysNo = ProductSysNo;
            model.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            }; 

            facade.QueryLog(model, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }

                this.dgGiftProductLog.ItemsSource = list;
                this.dgGiftProductLog.TotalCount = args.Result.TotalCount;
            });

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

    }
}
