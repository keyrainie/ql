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

using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductDomainListForEmptyCategory : UserControl, IListControl<ProductDepartmentCategoryVM>
    {
        public UCProductDomainListForEmptyCategory()
        {
            InitializeComponent();
        }

        private void dataProductDomainList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var filter = this.dataProductDomainList.QueryCriteria as ProductDomainQueryVM;
            new ProductDomainFacade(CPApplication.Current.CurrentPage).QueryProductDomain(filter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.dataProductDomainList.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                this.dataProductDomainList.TotalCount = args.Result.TotalCount;                
            });
        }

        public void BindData(object filter)
        {
            this.dataProductDomainList.QueryCriteria = filter;
            this.dataProductDomainList.Bind();
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = this.dataProductDomainList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
            }
        }

        public List<ProductDepartmentCategoryVM> GetSelectedSysNoList()
        {
            List<ProductDepartmentCategoryVM> list = new List<ProductDepartmentCategoryVM>();
            dynamic rows = this.dataProductDomainList.ItemsSource;
            if (rows != null)
            {
                foreach (dynamic row in rows)
                {
                    if (row.IsChecked)
                    {
                        ProductDepartmentCategoryVM vm = new ProductDepartmentCategoryVM
                        {
                            C2SysNo = row.C2SysNo                            
                        };
                        list.Add(vm);
                    }
                }
            }
            return list;
        }
    }
}
