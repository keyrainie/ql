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
using ECCentral.Portal.UI.MKT.Models.Promotion.QueryFilter;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Views.Promotion
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CouponCodeRedeemLogQuery : PageBase
    {
        private CouponCodeRedeemLogQueryFilterVM _FilterVM = new CouponCodeRedeemLogQueryFilterVM();
        private CouponCodeRedeemLogFacade _Facade;

        public CouponCodeRedeemLogQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender,e);

            int tempsysNo = -1;
            _Facade = new CouponCodeRedeemLogFacade(this);

            List<KeyValuePair<CouponCodeUsedStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<CouponCodeUsedStatus>();
            statusList.Insert(0, new KeyValuePair<CouponCodeUsedStatus?, string>(null, ResCommonEnum.Enum_Select));
            lstStatus.ItemsSource = statusList;
            lstStatus.SelectedIndex = 0;
           


            if (this.Request.QueryString != null
               && this.Request.QueryString.ContainsKey("sysno")
               && int.TryParse(this.Request.QueryString["sysno"].ToString().Trim(), out tempsysNo))
            {
                if (tempsysNo != -1)
                {
                    _FilterVM.CouponBeginNo = tempsysNo.ToString();
                    _FilterVM.CouponEndNo = tempsysNo.ToString();

                    _Facade.Query(_FilterVM, (obj, args) =>
                    {
                        dgResult.ItemsSource = args.Result.Rows.ToList();
                        dgResult.TotalCount = args.Result.TotalCount;
                    });
                }
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
            dgResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CouponCodeRedeemLogQueryFilterVM>(this._FilterVM);

            dgResult.Bind();
        }

        private void dgResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _FilterVM.PageInfo.PageIndex = e.PageIndex;
            _FilterVM.PageInfo.PageSize = e.PageSize;
            _FilterVM.PageInfo.SortBy = e.SortField;
            _Facade.Query(dgResult.QueryCriteria as CouponCodeRedeemLogQueryFilterVM, (obj, args) =>
            {
                dgResult.ItemsSource = args.Result.Rows;
                dgResult.TotalCount = args.Result.TotalCount;
            });
        }
        
    }
}
