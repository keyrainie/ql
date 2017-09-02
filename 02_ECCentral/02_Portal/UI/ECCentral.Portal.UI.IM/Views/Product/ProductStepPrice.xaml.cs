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
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductStepPrice : PageBase
    {
        public ProductStepPrice()
        {
            InitializeComponent();
        }

        public ProductStepPriceQueryFilterVM _viewModel;
        public ProductPriceRequestFacade _facade;

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _viewModel = new ProductStepPriceQueryFilterVM();
            _facade = new ProductPriceRequestFacade(this);
            QueryFilter.DataContext = _viewModel;
        }

        private void Serch_Click(object sender, RoutedEventArgs e)
        {
            QueryResult.Bind();
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            Dialog(null);
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysnoList = new List<int>();
            dynamic rows = QueryResult.ItemsSource;
            if (rows == null)
            {
                Window.Alert("请选择需要删除的数据！");
                return;
            }

            foreach (var row in rows)
            {
                if (row.IsChecked)
                {
                    sysnoList.Add(row.SysNo);
                }
            }
            if (sysnoList.Count <= 0)
            {
                Window.Alert("请选择需要删除的数据!");
                return;
            }
            Window.Confirm("确定删除所选数据?", (o, s) =>
            {
                if (s.DialogResult == DialogResultType.OK)
                {
                    _facade.DeleteProductStepPrice(sysnoList, (m, args) =>
                    {
                        Window.Alert("提示信息", "删除成功！", MessageType.Information, (g, ar) =>
                        {
                            QueryResult.Bind();
                        });
                    });
                }
            });
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this._viewModel.PagingInfo.PageIndex = e.PageIndex;
            this._viewModel.PagingInfo.PageSize = e.PageSize;

            _facade.GetProductStepPrice(_viewModel, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                QueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                QueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = (CheckBox)sender;
            dynamic rows = QueryResult.ItemsSource;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row.IsChecked = ck.IsChecked.Value;
                }
            }
        }

        private void Hyperlink_EditData_Click(object sender, RoutedEventArgs e)
        {
            ProductStepPriceInfoVM entity = new ProductStepPriceInfoVM();
            dynamic row = QueryResult.SelectedItem;
            if (row != null)
            {
                entity.SysNo = row.SysNo;
                entity.ProductSysNo = row.ProductSysNo;
                entity.ProductID = row.ProductID.ToString();
                entity.BaseCount = row.BaseCount;
                entity.TopCount = row.TopCount;
                entity.StepPrice = row.StepPrice.ToString();
                Dialog(entity);
            }
        }

        private void Dialog(ProductStepPriceInfoVM entity)
        {
            ProductStepPriceMaintain maintain = new ProductStepPriceMaintain(entity) { Page = this };
            maintain.DiolgHander = Window.ShowDialog(entity == null ? "新建阶梯价格" : "编辑阶梯价格", maintain, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    QueryResult.Bind();
                }
            });
        }

    }
}
