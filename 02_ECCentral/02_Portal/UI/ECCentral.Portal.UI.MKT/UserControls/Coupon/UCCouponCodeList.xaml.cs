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
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCCouponCodeList : UserControl
    {
        CouponCodeQueryFilterViewModel _queryFilterVM = new CouponCodeQueryFilterViewModel();
        bool isLoaded = false;

        private CouponsFacade _facade;

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public event RoutedEventHandler OnPreStepClick;
        public UCCouponCodeList()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(UCCouponCodeList_Loaded);
        }

        void UCCouponCodeList_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
            gridCodeQueryFilter.DataContext = _queryFilterVM;
            _facade = new CouponsFacade(CPApplication.Current.CurrentPage);

            btnQuery_Click(btnQuery, new RoutedEventArgs());
            isLoaded = true;

        }
        

        private void btnPreStep_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnPreStepClick != null)
            {
                this.OnPreStepClick(sender, e);
            }
        }

        private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = dgCodeQueryResult.ItemsSource;
            foreach (dynamic row in rows)
            {
                row["IsChecked"] = chk.IsChecked.Value ;
            }
            dgCodeQueryResult.ItemsSource = rows;            
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.gridCodeQueryFilter);
            if (_queryFilterVM.HasValidationErrors) return;
            

            dgCodeQueryResult.Bind();
        }

        private void dgCodeQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;

            CouponsFacade facade = new CouponsFacade(CPApplication.Current.CurrentPage, vm);
            _queryFilterVM.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            _queryFilterVM.CouponSysNo = vm.SysNo.HasValue ? vm.SysNo : -1;

            facade.QueryCouponCode(_queryFilterVM, (obj, args) =>
            {
                dgCodeQueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false); 
                dgCodeQueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnAddNewCode_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;

            UCCouponCodeEdit ucEdit = new UCCouponCodeEdit();
            ucEdit.CouponsSysNo = vm.SysNo.Value;
            ucEdit.CCCustomerMaxFrequency = string.IsNullOrWhiteSpace(vm.UsingFrequencyCondition.CustomerMaxFrequency) ? "1" : vm.UsingFrequencyCondition.CustomerMaxFrequency;
            ucEdit.CCMaxFrequency = vm.UsingFrequencyCondition.MaxFrequency;
            ucEdit.Dialog = CurrentWindow.ShowDialog("优惠券代码编辑", ucEdit, (obj, args) =>
                {
                    if (args.DialogResult != DialogResultType.OK)
                    {
                        return;
                    }
                    dgCodeQueryResult.Bind();

                });
        }

        private void btnDeleteBachCode_Click(object sender, RoutedEventArgs e)
        {
            List<int?> list = new List<int?>();
            dynamic rows = dgCodeQueryResult.ItemsSource;
            foreach (dynamic row in rows)
            {
                if (row["IsChecked"])
                {
                    list.Add(row["SysNo"]);
                }
            }
            if (list.Count == 0)
            {
                CurrentWindow.Alert("请至少选中一条记录！");
                return;
            }
            _facade.DeleteCouponCodeList(list, () =>
                {
                    CurrentWindow.Alert("删除成功！");
                    dgCodeQueryResult.Bind();
                }
            );

        }

        private void btnDeleteAllCode_Click(object sender, RoutedEventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            _facade.DeleteAllCouponCode(vm.SysNo, () =>
            {
                CurrentWindow.Alert("删除成功！");
                dgCodeQueryResult.Bind();
            }
            );
        }

        private void dgCodeQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;

            CouponsFacade facade = new CouponsFacade(CPApplication.Current.CurrentPage, vm);
            ColumnSet col = new ColumnSet(this.dgCodeQueryResult);
            _queryFilterVM.PageInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            facade.ExportExcelForCouponsCodeList(_queryFilterVM, new ColumnSet[] { col });
        }

         
    }
}
