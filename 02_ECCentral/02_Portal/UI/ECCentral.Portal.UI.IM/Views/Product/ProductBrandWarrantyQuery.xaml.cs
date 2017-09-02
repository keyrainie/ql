using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductBrandWarrantyQuery : PageBase
    {

        #region const
        private ProductBrandWarrantyQueryFilter Filter;
        private ProductBrandWarrantyFacade facade;
        private ProductBrandWarrantyQueryVM model;
        List<ProductBrandWarrantyQueryVM> vms;
        #endregion

        #region Method
        public ProductBrandWarrantyQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            //获取页面信息
            model = new ProductBrandWarrantyQueryVM();
            QuerySection.DataContext = model;
            facade = new ProductBrandWarrantyFacade(this);
            this.QueryResultGrid.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(QueryResultGrid_LoadingDataSource);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            Filter = model.ConvertVM<ProductBrandWarrantyQueryVM, ProductBrandWarrantyQueryFilter>();
            facade.GetProductBrandWarrantyByQuery(Filter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
                {
                    vms = DynamicConverter<ProductBrandWarrantyQueryVM>
                        .ConvertToVMList<List<ProductBrandWarrantyQueryVM>>(args.Result.Rows);
                    this.QueryResultGrid.ItemsSource = vms;
                    this.QueryResultGrid.TotalCount = args.Result.TotalCount;
                });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductBrandWarrantyEditMaintain item = new ProductBrandWarrantyEditMaintain();
            item.Dialog = Window.ShowDialog("新建品牌维护", item, (s, args) =>
            {
                this.QueryResultGrid.Bind();
            }, new Size(600, 350));
            QueryResultGrid.Bind();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Boolean isOne = true;
            List<int> editSysNo = new List<int>();
            ProductBrandWarrantyQueryVM vm = null;
            ProductBrandWarrantyEditMaintain edit = new ProductBrandWarrantyEditMaintain();
            vms.ForEach(item =>
            {
                if (item.IsChecked)
                {
                    if (isOne)
                    {
                         vm = new ProductBrandWarrantyQueryVM() { 
                            BrandSysNo = item.BrandSysNo,
                            BrandName = item.BrandName,
                            C1Name = item.C1Name,
                            C2Name = item.C2Name,
                            C3Name = item.C3Name,
                            C1SysNo = item.C1SysNo,
                            C2SysNo = item.C2SysNo,
                            C3SysNo = item.C3SysNo,
                            WarrantyDay = item.WarrantyDay,
                            WarrantyDesc = item.WarrantyDesc
                        };
                        edit.Data = vm;
                        isOne = false;
                    }
                    editSysNo.Add(item.SysNo);
                }
            });
            if (vm != null)
            {
                edit.EditSysNos = editSysNo;
                edit.Dialog = Window.ShowDialog("編輯品牌维护", edit, (s, args) =>
                {
                    this.QueryResultGrid.Bind();

                }, new Size(600, 350));
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请输选择需要维护的品牌!");
            }
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            List<ProductBrandWarrantyQueryVM> delProductBrandWarrantyQuery = new List<ProductBrandWarrantyQueryVM>();
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否需要进行删除操作吗？", (o, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    vms.ForEach(item =>
                    {
                        if (item.IsChecked)
                        {
                            delProductBrandWarrantyQuery.Add(new ProductBrandWarrantyQueryVM()
                            {
                                SysNo = item.SysNo,
                                C3SysNo = item.C3SysNo,
                                BrandSysNo = item.BrandSysNo,
                            });
                        }
                    });
                }
                if (delProductBrandWarrantyQuery != null)
                {
                    facade.DelBrandWarrantyInfoBySysNo(delProductBrandWarrantyQuery, (obj, arg) =>
                    {
                        if (arg.FaultsHandle()) return;
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("删除成功!", MessageBoxType.Success);
                    });
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请输选择需要维护的品牌!");
                }
                QueryResultGrid.Bind();
            });
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (vms == null || checkBoxAll == null)
                return;
            vms.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }
        #endregion
    }
}
