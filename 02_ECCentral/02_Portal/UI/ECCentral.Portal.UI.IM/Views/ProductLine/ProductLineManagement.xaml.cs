using System;
using System.Windows;

using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.Enum.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.IM.Views.ProductLine
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductLineManagement : PageBase
    {
        public IListControl<ProductLineVM> ListControl { get; set; }

        public ProductLineQueryVM Filter
        {
            get
            {
                return this.ucFilter.DataContext as ProductLineQueryVM;
            }
            set
            {
                this.ucFilter.DataContext = value;
            }
        }

        public ProductLineManagement()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.Filter = new ProductLineQueryVM();
            this.ListControl = this.ucProductLineList;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Clear();
            foreach (FrameworkElement element in this.gridListContainer.Children)
            {
                element.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (this.Filter.IsSearchEmptyCategory)
            {
                this.ucProductLineListForEmptyCategory.Visibility = System.Windows.Visibility.Visible;
                this.ListControl = this.ucProductLineListForEmptyCategory;
            }
            else
            {
                this.ucProductLineList.Visibility = System.Windows.Visibility.Visible;
                this.ListControl = this.ucProductLineList;
            }

            var clonedFilter = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ProductLineQueryVM>(this.Filter);
            this.ListControl.BindData(clonedFilter);
            if (this.Filter.IsSearchEmptyCategory) 
            {
                btnBatchUpdate.Content = "批量创建产品线";
                IsEmptyC2Create = true;
            }
            else
            {
                btnBatchUpdate.Content = "批量修改产品经理";
                IsEmptyC2Create = false;
            }
        }
        public bool IsEmptyC2Create { get; set; }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Clear();
            var vm = new ProductLineVM();
            UCProductLineDetail uc = new UCProductLineDetail(vm);
            IDialog dialog = Window.ShowDialog("添加产品线", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.ListControl.BindData(this.Filter);
                }
            });
            uc.Dialog = dialog;
        }
        private void btnBatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            Window.MessageBox.Clear();
            var list = this.ListControl.GetSelectedSysNoList();
            if (list.Count == 0)
            {
                Window.MessageBox.Show("请选择要修改的数据!", MessageBoxType.Warning);
                return;
            }
            var uc = new UCBatchUpdateProductLine(list);

            string title = "批量修改产品经理";
            if (IsEmptyC2Create)
            {
                uc.OperatorTypePanel.Visibility = System.Windows.Visibility.Collapsed;
                title = "批量创建产品线";
            }

            var dialog = Window.ShowDialog(title, uc, (obj, args) =>
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
