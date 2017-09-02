using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductDomainList : UserControl, IListControl<ProductDepartmentCategoryVM>
    {
        private IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public UCProductDomainList()
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

        public void BindData(object filter)
        {
            this.dataProductDomainList.QueryCriteria = filter;
            this.dataProductDomainList.Bind();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            dynamic d = btn.DataContext as dynamic;
            ProductDomainVM vm = DynamicConverter<ProductDomainVM>.ConvertToVM(d);
            string listStr = d.DepartmentMerchandiserSysNoListStr;
            List<int?> sysNoList = new List<int?>();
            if (!string.IsNullOrEmpty(listStr))
            {
                var list = listStr.Split(new char[] { ';' }).ToList();
                list.ForEach(p =>
                {
                    sysNoList.Add(int.Parse(p));                                        
                });                
            }
            vm.DepartmentMerchandiserSysNoList = sysNoList;
            UCProductDomainDetail uc = new UCProductDomainDetail(vm);           
            
            IDialog dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("修改Domain信息", uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.BindData(this.dataProductDomainList.QueryCriteria);
                }
            });
            uc.Dialog = dialog;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = (sender as HyperlinkButton).DataContext;
            CPApplication.Current.CurrentPage.Context.Window.Confirm("确定要删除吗?", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    int sysNo = d.SysNo;
                    int? categorySysNo = d.CategorySysNo;
                    new ProductDomainFacade(this.Page).DeleteDomain(sysNo, categorySysNo, (o,a)=>
                    {
                        var list = this.dataProductDomainList.ItemsSource as List<dynamic>;
                        list.RemoveAll(p => p.SysNo == d.SysNo);
                        this.dataProductDomainList.ItemsSource = list;

                        this.Page.Context.Window.Alert("操作成功!");
                    });
                }
            });
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
                            C2SysNo = row.C2SysNo,
                            SysNo = row.CategorySysNo
                        };
                        list.Add(vm);
                    }
                }
            }
            return list;
        }
    }
}
