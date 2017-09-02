using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductAccessories : PageBase
    {
        private ProductAccessoriesQueryVM model;
        private ProductAccessoriesFacade facade;
        public ProductAccessories()
        {
            InitializeComponent();
            this.ProductAccessoriesResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ProductAccessoriesResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                model = new ProductAccessoriesQueryVM();
                this.DataContext = model;
                facade = new ProductAccessoriesFacade();
            };
        }

        void ProductAccessoriesResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetProductAccessoriesByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.ProductAccessoriesResult.ItemsSource = arg.Result.Rows;
                this.ProductAccessoriesResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ProductAccessoriesResult.Bind();
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ProductAccessoriesEditMaintain item = new ProductAccessoriesEditMaintain() { IsEdit=false};
            item.Dialog = Window.ShowDialog("添加配件查询功能", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ProductAccessoriesResult.Bind();
                }
            }, new Size(600,200));
        }

     

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.ProductAccessoriesResult.SelectedItem as dynamic;
            ProductAccessoriesVM vm = new ProductAccessoriesVM()
            {
                IsTreeQuery = d.IsTreeQuery == "Y" ? true : false,
                Status = d.Status,
                BackPictureBigUrl = d.BackPictureBigUrl,
                AccessoriesQueryName = d.AccessoriesQueryName,
                SysNo = d.SysNo,
            };
            ProductAccessoriesEditMaintain item = new ProductAccessoriesEditMaintain() { IsEdit = true, Data = vm };
            item.Dialog = Window.ShowDialog("更新配件查询功能", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ProductAccessoriesResult.Bind();
                }
            }, new Size(600, 200));
        }

        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void BtnSetCondition_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.ProductAccessoriesResult.SelectedItem as dynamic;
            ProductAccessoriesQueryConditionSet item = new ProductAccessoriesQueryConditionSet() { SysNo = d.SysNo, IsTreeQuery = d.IsTreeQuery =="Y"};
            item.Dialog = Window.ShowDialog("设置查询条件", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ProductAccessoriesResult.Bind();
                }
            }, new Size(600, 500));
        }
        
        /// <summary>
        /// 设定查询条件值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void BtnSetConditionValue_Click(object sender, RoutedEventArgs e)
       {
           dynamic d = this.ProductAccessoriesResult.SelectedItem as dynamic;
           ProductAccessoriesQueryConditionValueMaintain item = new ProductAccessoriesQueryConditionValueMaintain() { MaterSysNo = d.SysNo };
           item.Dialog = Window.ShowDialog("设置查询条件值", item, (s, args) =>
           {
               if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
               {
                   this.ProductAccessoriesResult.Bind();
               }
           }, new Size(800, 600));
       }

        /// <summary>
        /// 查询效果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void BtnPreView_Click(object sender, RoutedEventArgs e)
       {
           dynamic d = this.ProductAccessoriesResult.SelectedItem as dynamic;
           ProductAccessoriesQueryConditionPreViewSearch item = new ProductAccessoriesQueryConditionPreViewSearch() { MaterSysNo = d.SysNo };
           item.Dialog = Window.ShowDialog("查询效果设置", item, (s, args) =>
           {
               if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
               {
                   this.ProductAccessoriesResult.Bind();
               }
           }, new Size(800, 600));
       }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void BtnExcelExporter_Click(object sender, RoutedEventArgs e)
       {
            dynamic d = this.ProductAccessoriesResult.SelectedItem as dynamic;
            bool IsTreeQuery = d.IsTreeQuery == "Y"; 
            ColumnSet col = new ColumnSet();
            List<ProductAccessoriesQueryConditionVM> ConditionList = new List<ProductAccessoriesQueryConditionVM>();
           //根据SysNo得到所有条件，动态生成列名
            facade.GetAccessoriesQueryConditionBySysNo((int)d.SysNo, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                foreach (var item in arg.Result.Rows)
                {
                    ConditionList.Add(new ProductAccessoriesQueryConditionVM()
                    {
                        Condition = new AccessoriesQueryCondition() { ConditionName = item.ConditionName, Priority = (PriorityType)item.Level, SysNo = item.SysNo },
                        ParentCondition = new AccessoriesQueryCondition() { ConditionName = item.ParentConditionName, SysNo = item.ParentSysNo },
                        Priority = (PriorityType)item.Level
                    });
                }
                int index=0;
                if (IsTreeQuery)
                {
                    foreach (var item in ConditionList)
                    {
                        string title = item.Condition.ConditionName + "编号";
                        if (index == 0)
                        {
                            col.Insert(0, "FirstSysNo", title, 15);
                            col.Insert(1, "FirstValue", item.Condition.ConditionName, 25);
                        }
                        if (index == 1)
                        {
                            col.Insert(2, "SecondSysNo", title, 15);
                            col.Insert(3, "SecondValue", item.Condition.ConditionName, 25);
                        }
                        if (index == 2)
                        {
                            col.Insert(4, "ThirdSysNo", title, 15);
                            col.Insert(5, "ThirdValue", item.Condition.ConditionName, 25);
                        }
                        if (index == 3)
                        {
                            col.Insert(6, "FourthSysNo", title, 15);
                            col.Insert(7, "FourthValue", item.Condition.ConditionName, 25);
                        }
                        index = index + 1;

                    }
                }
                else
                {
                    col.Insert(0, "ConditionSysNo", "条件编号");
                    col.Insert(1, "ConditionName", "条件");
                    col.Insert(2, "ConditionValueSysNo", "选项值编号");
                    col.Insert(3, "ConditionValue", "选项值");
                    col.Insert(4, "Producut", "商品");
                }
                if (ConditionList.Count > 0&& IsTreeQuery)
                {
                    col.Insert(ConditionList.Count * 2, "Producut", "商品");
                }
                facade.GetAccessoriesQueryExcelOutput((int)d.SysNo, (string)d.IsTreeQuery, new ColumnSet[] { col });
            });
          
          

       }
 }
}
