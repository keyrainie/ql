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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductLineManagement : PageBase
    {
         private ProductLineFacade facade;
         private ProductLineQueryVM model;
        public ProductLineManagement()
        {
            InitializeComponent();
            this.ProductLineResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ProductLineResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                model = new ProductLineQueryVM();
                facade = new ProductLineFacade();
                this.DataContext = model;
                
            };
        }

        void ProductLineResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductLineQueryVM vm=this.DataContext as ProductLineQueryVM;
           facade.GetProductLineByQuery(vm,e.PageSize,e.PageIndex,e.SortField,(obj,arg)=>
           {
               if (arg.FaultsHandle())
               {
                   return;
               }
               this.ProductLineResult.ItemsSource = arg.Result.Rows;
               this.ProductLineResult.TotalCount = arg.Result.TotalCount;
           });
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.ProductLineResult.Bind();
        }
         /// <summary>
         /// 删除
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否删除?", (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    HyperlinkButton link = (HyperlinkButton)sender;
                    int sysNo = (int)(link.Tag ?? 0);
                    facade.DeleteProductLine(sysNo, (objs, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.MessageBox.Show("删除成功!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                        this.ProductLineResult.Bind();
                    });
                }
            });

        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic d=this.ProductLineResult.SelectedItem as dynamic;
            ProductLineVM vm = new ProductLineVM()
            {
                ProductLineName = d.ProductLineName,
                SysNo = d.SysNo,
                UseScopeDescription = d.Note,
                Priority =Convert.ToString(d.PRI),
                Category=new ProductLineCategoryVM(){CategoryName=d.CategoryName,SysNo=d.CategorySysNo}
                
            };
            ProductLineMaintain item = new ProductLineMaintain();
            item.IsEdit = true;
            item.Data = vm;
            item.Dialog = Window.ShowDialog("编辑", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ProductLineResult.Bind();
                }
            }, new Size(400, 300));
        }

        private void btnProductlineNew_Click(object sender, RoutedEventArgs e)
        {
            ProductLineMaintain item = new ProductLineMaintain();
            item.IsEdit = false;
            item.Dialog = Window.ShowDialog("新建", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ProductLineResult.Bind();
                }
            }, new Size(400,300));
        }

    }


}
