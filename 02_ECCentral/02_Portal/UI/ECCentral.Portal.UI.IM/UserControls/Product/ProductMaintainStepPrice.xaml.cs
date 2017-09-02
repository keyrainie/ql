using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.Portal.UI.IM.Models.Product.ProductMaintain;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductMaintainStepPrice : UserControl
    {
        private ProductPriceRequestFacade m_ucfacade=null;
        private ProductPriceRequestFacade Facade
        {
            get
            {
                if (m_ucfacade == null)
                {
                    m_ucfacade = new ProductPriceRequestFacade();
                }
                return m_ucfacade;
            }
        } 

        public ProductMaintainStepPriceVM VM
        {
            get { return DataContext as ProductMaintainStepPriceVM; }
        }



        public ProductMaintainStepPrice()
        {
            InitializeComponent();
        }

        private void Hyperlink_EditData_Click(object sender, RoutedEventArgs e)
        {
            this.btnAdd.Visibility = Visibility.Collapsed;
            this.btnEdit.Visibility = Visibility.Visible;
            VM.AddEntity = QueryResult.SelectedItem as ProductStepPriceInfoVM;
            ChildLayoutRoot1.DataContext = VM.AddEntity;
            QueryResult.Bind();
        }

        private void Hyperlink_DeleteData_Click(object sender, RoutedEventArgs e)
        {

            List<int> sysnoList = new List<int>();
            dynamic selectItem = QueryResult.SelectedItem as ProductStepPriceInfoVM;
            sysnoList.Add(selectItem.SysNo);
            Facade.DeleteProductStepPrice(sysnoList, (m, args) =>
            {
                QueryResult.Bind();
            });
        }

        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            var ucview = ChildLayoutRoot1.DataContext as ProductStepPriceInfoVM;
            if (!ValidationManager.Validate(this.ChildLayoutRoot1))
                return;
            ucview.ProductSysNo = VM.ProductSysNo;
            if (ucview.ProductSysNo != null && ucview.StepPrice != null)
            {
                ucview.InUser = CPApplication.Current.LoginUser.LoginName;
                Facade.CreateProductStepPrice(ucview, (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == 1)
                    {
                        VM.AddEntity = new ProductStepPriceInfoVM();
                        ChildLayoutRoot1.DataContext = VM.AddEntity;
                        QueryResult.Bind();
                        MessageBox.Show("保存成功！");
                    }
                });
            }
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            Facade.GetProductStepPricebyProductSysNo(VM.ProductSysNo, (priceobj, priceargs) =>
            {
                if (priceargs.FaultsHandle())
                {
                    return;
                }
                VM.QueryResultList = priceargs.Result;
                this.QueryResult.ItemsSource = VM.QueryResultList;
            });
        }

        private void BtnEditClick(object sender, RoutedEventArgs e)
        {
            var ucview = ChildLayoutRoot1.DataContext as ProductStepPriceInfoVM;
            if (!ValidationManager.Validate(this.ChildLayoutRoot1))
                return;
            ucview.ProductSysNo = VM.ProductSysNo;
            if (ucview.ProductSysNo != null && ucview.StepPrice != null)
            {
                ucview.InUser = CPApplication.Current.LoginUser.LoginName;
 
                Facade.CreateProductStepPrice(ucview, (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == 1)
                    {
                        this.btnEdit.Visibility = Visibility.Collapsed;
                        this.btnAdd.Visibility = Visibility.Visible;
                        VM.AddEntity = new ProductStepPriceInfoVM();
                        ChildLayoutRoot1.DataContext = VM.AddEntity;
                        QueryResult.Bind();
                        MessageBox.Show("修改成功！");
                    }
                });
            }
        }
    }
}
