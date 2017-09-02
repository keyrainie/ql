using System;
using System.Windows;

using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.Enum.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views.ProductDomain
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductDomainManagement : PageBase
    {
        public IListControl<ProductDepartmentCategoryVM> ListControl { get; set; }

        public ProductDomainQueryVM Filter
        {
            get
            {
                return this.ucFilter.DataContext as ProductDomainQueryVM;
            }
            set
            {
                this.ucFilter.DataContext = value;
            }
        }

        public ProductDomainManagement()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);           
            
            this.ListControl = this.ucProductDomainList;

            this.Filter = new ProductDomainQueryVM();
        }

        private void btnBatchUpdatePM_Click(object sender, RoutedEventArgs e)
        {
            if (this.Filter.AsAggregateStyle)
            {
                return;
            }
            var list = this.ListControl.GetSelectedSysNoList();
            if (list.Count == 0)
            {
                Window.Alert("请选择要修改的数据!", MessageType.Warning);
                return;
            }
            var uc = new UCBatchUpdatePM(this, list);
            var dialog = Window.ShowDialog("批量修改PM", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.ListControl.BindData(this.Filter);
                }
            });
            uc.Dialog = dialog;
        }       

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            foreach (FrameworkElement element in this.gridListContainer.Children)
            {
                element.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (this.Filter.AsAggregateStyle)
            {
                this.ucProductDomainListForAggregateStyle.Visibility = System.Windows.Visibility.Visible;
                this.ListControl = this.ucProductDomainListForAggregateStyle;
            }
            else if (this.Filter.IsSearchEmptyCategory)
            {
                this.ucProductDomainListForEmptyCategory.Visibility = System.Windows.Visibility.Visible;
                this.ListControl = this.ucProductDomainListForEmptyCategory;
            }
            else
            {
                this.ucProductDomainList.Visibility = System.Windows.Visibility.Visible;
                this.ListControl = this.ucProductDomainList;
            }

            var clonedFilter = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductDomainQueryVM>(this.Filter);
            this.ListControl.BindData(clonedFilter);
        }       

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var vm = new ProductDomainVM();
            UCProductDomainDetail uc = new UCProductDomainDetail(vm);            
            IDialog dialog = Window.ShowDialog("添加Domain", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.ListControl.BindData(this.Filter);
                }
            });
            uc.Dialog = dialog;
        }

         
    }
}
