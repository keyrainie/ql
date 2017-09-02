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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views.Promotion
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class CouponCodeCustomerLogQuery : PageBase
    {
        private CouponCodeCustomerLogQueryFilterVM _FilterVM = new CouponCodeCustomerLogQueryFilterVM();
        private CouponCodeCustomerLogFacade _Facade;

        public CouponCodeCustomerLogQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            int customerSysNo = 0;
            int tempsysNo = -1;
            _Facade = new CouponCodeCustomerLogFacade(this);
            if (this.Request.QueryString != null)
            {
                if (this.Request.QueryString.ContainsKey("sysno")&& int.TryParse(this.Request.QueryString["sysno"].ToString().Trim(),
                    out tempsysNo))
                {
                    _FilterVM.CouponBeginNo = tempsysNo.ToString();
                    _FilterVM.CouponEndNo = tempsysNo.ToString();

                }
                if (this.Request.QueryString.ContainsKey("CustomerSysNo") && int.TryParse(this.Request.QueryString["CustomerSysNo"].ToString().Trim(),
                    out customerSysNo))
                {
                    _FilterVM.CustomerSysNo = customerSysNo;
                }

                _Facade.Query(_FilterVM, (obj, args) =>
                {
                    //Window.Alert("123");
                    dgResult.ItemsSource = args.Result.Rows.ToList();
                    dgResult.TotalCount = args.Result.TotalCount;
                });
            }

            GridQueryFilter.DataContext = _FilterVM;

        }

        private void GridQueryFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonSearch_Click(ButtonSearch, new RoutedEventArgs());
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.dgResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CouponCodeCustomerLogQueryFilterVM>(this._FilterVM);
            dgResult.Bind();
        }

        private void dgResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _FilterVM.PageInfo.PageIndex = e.PageIndex;
            _FilterVM.PageInfo.PageSize = e.PageSize;
            _FilterVM.PageInfo.SortBy = e.SortField;
            _Facade.Query(this.dgResult.QueryCriteria as CouponCodeCustomerLogQueryFilterVM, (obj, args) =>
            {
                dgResult.ItemsSource = args.Result.Rows;
                dgResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void dgResult_ExportAllClick(object sender, EventArgs e)
        {
            if (this.dgResult.QueryCriteria == null || this.dgResult.TotalCount < 1)
            {
                Window.Alert(ResCouponCodeCustomerLogQuery.Msg_ExportError);
                return;
            }
            ColumnSet col = new ColumnSet(this.dgResult);

            _Facade.ExportExcelFile(this.dgResult.QueryCriteria as CouponCodeCustomerLogQueryFilterVM, new ColumnSet[] { col });

        }
    }
}
