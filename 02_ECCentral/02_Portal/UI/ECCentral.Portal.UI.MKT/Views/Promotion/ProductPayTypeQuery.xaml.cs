using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models.Promotion;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.MKT.Promotion;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views.Promotion
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductPayTypeQuery : PageBase
    {
        private ProductPayTypeQueryFilter _filter;
        private ProductPayTypeQueryVM _model;
        private ProductPayTypeFacade _facade;
        public IDialog Dialog { get; set; }

        public ProductPayTypeQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, System.EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _facade = new ProductPayTypeFacade(this);
            _filter = new ProductPayTypeQueryFilter();
            _model = new ProductPayTypeQueryVM();
            DataContext = _model;
            cbPayTypeStatus.ItemsSource =
                EnumConverter.GetKeyValuePairs<PayTypeStatus>(EnumConverter.EnumAppendItemType.All);

        }

        private void UCPayType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnProductPayTypeSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(QuerySection))
            {
                if (dpBeginDateFrom.SelectedDateTime.HasValue 
                    && dpBeginDateTo.SelectedDateTime.HasValue 
                    && dpBeginDateFrom.SelectedDateTime.Value.CompareTo(dpBeginDateTo.SelectedDateTime.Value) > 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(ResProductPayTypeQuery.Error_BeginDateMoreEndDate, MessageBoxType.Warning);
                    return;
                }
                if (dpEndDateFrom.SelectedDateTime.HasValue
                    && dpEndDateTo.SelectedDateTime.HasValue
                    && dpEndDateFrom.SelectedDateTime.Value.CompareTo(dpEndDateTo.SelectedDateTime.Value) > 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(ResProductPayTypeQuery.Error_BeginDateMoreEndDate, MessageBoxType.Warning);
                    return;
                }
                _filter = _model.ConvertVM<ProductPayTypeQueryVM, ProductPayTypeQueryFilter>();
                dgPayTypeQueryResult.QueryCriteria = _filter;
                dgPayTypeQueryResult.Bind();
            }
        }

        private void dgPayTypeQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            _filter.PageInfo = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            _facade.QueryProductPayType(_filter, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                dgPayTypeQueryResult.ItemsSource = DynamicConverter<ProductPayTypeQueryVM>.ConvertToVMList<List<ProductPayTypeQueryVM>>(args.Result.Rows);
                dgPayTypeQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnPayTypeNew_Click(object sender, RoutedEventArgs e)
        {
            var item = new UCProductPayTypeMaintain();

            item.Dialog = Window.ShowDialog(ResProductPayTypeQuery.Add_Title, item, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    dgPayTypeQueryResult.Bind();
                }
            });
        }

        private void btnPayTypeAbort_Click(object sender, RoutedEventArgs e)
        {
            var payTypeIds = string.Empty;
            var viewlist = dgPayTypeQueryResult.ItemsSource as dynamic;
            if (viewlist == null) return;
            foreach (var item in viewlist)
            {
                if (item.IsChecked)
                {
                    payTypeIds += string.Format(",{0}", item.SysNo);
                }
            }
            if (payTypeIds == string.Empty)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(
                    ResProductPayTypeQuery.Error_PayTypeIsEmpty,MessageBoxType.Warning);
            }
            else
            {
                Window.Confirm("确认要中止?", (objs, a) =>
                    {
                        if (a.DialogResult == DialogResultType.OK)
                        {
                            payTypeIds = payTypeIds.Substring(1, payTypeIds.Length - 1);
                            _facade.BathAbortProductPayType(payTypeIds,
                                                            CPApplication.Current.LoginUser.
                                                                LoginName,
                                                            (s, args) =>
                                                            {
                                                                if (args.FaultsHandle())
                                                                    return;
                                                                dgPayTypeQueryResult.Bind();
                                                            });
                        }
                    });
            }
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb == null) return;
            var viewlist = dgPayTypeQueryResult.ItemsSource as dynamic;
            if (viewlist == null) return;
            foreach (var item in viewlist)
            {
                item.IsChecked = item.IsEnable && cb.IsChecked != null && cb.IsChecked.Value;
            }
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
