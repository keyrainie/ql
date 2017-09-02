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
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductAccessoriesQueryConditionPreViewSearch : UserControl
    {
        private ProductAccessoriesQueryConditionPreViewQueryVM model;
        private ProductAccessoriesFacade facade;
        public int MaterSysNo { get; set; }
        public IDialog Dialog { get; set; }
        public ProductAccessoriesQueryConditionPreViewSearch()
        {
            InitializeComponent();
            this.PreViewResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(PreViewResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                this.ConditionPreView.MaterSysNo = MaterSysNo;
                facade = new ProductAccessoriesFacade();
                model = new ProductAccessoriesQueryConditionPreViewQueryVM();
                model.MasterSysNo = MaterSysNo;
                this.DataContext = model;
            };
        }

        void PreViewResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.QueryAccessoriesQueryConditionBind(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.PreViewResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false); ;
                this.PreViewResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ConditionPreView.IsTreeQuery == true) //树形结构查询需要定位到最后子节点
            {
                if (ConditionPreView.NodeNumber > 0 && ConditionPreView.ConditionValueSysNo1>0)
                {
                    if ((ConditionPreView.NodeNumber == 1 && ConditionPreView.ConditionValueSysNo2 == 0) || (ConditionPreView.NodeNumber == 2 && ConditionPreView.ConditionValueSysNo3 == 0) || (ConditionPreView.NodeNumber == 3 && ConditionPreView.ConditionValueSysNo4 == 0))
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("请先选择配件查询中叶子查询下拉框的值!");
                        return;
                    }
                }
                
         }
            if (!ConditionPreView.IsTreeQuery)
            {
                model.ConditionValueSysNo1 = ConditionPreView.ConditionValueSysNo1;
                model.ConditionValueSysNo2 = ConditionPreView.ConditionValueSysNo2;
                model.ConditionValueSysNo3 = ConditionPreView.ConditionValueSysNo3;
            }
            model.ConditionValueSysNo4 = ConditionPreView.ConditionValueSysNo4;
            this.PreViewResult.Bind();
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.PreViewResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm("确认删除?", (objs, args) => {
                if (args.DialogResult == DialogResultType.OK)
                {
                    List<ProductAccessoriesQueryConditionPreViewInfo> list = new List<ProductAccessoriesQueryConditionPreViewInfo>();
                    dynamic viewlist = this.PreViewResult.ItemsSource as dynamic;
                    if (viewlist != null)
                    {
                        foreach (var item in viewlist)
                        {
                            if (item.IsChecked == true)
                            {
                                list.Add(new ProductAccessoriesQueryConditionPreViewInfo() { ConditionValueSysNo = item.ConditionValueSysNo, ProductSysNo = item.ProductSysNo, masterSysNo = MaterSysNo });
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        facade.DeleteAccessoriesQueryConditionBind(list, (obj, arg) =>
                        {
                            if (arg.FaultsHandle())
                            {
                                return;
                            }
                            CPApplication.Current.CurrentPage.Context.Window.Alert("删除成功!");
                            PreViewResult.Bind();
                        });
                    }
                    else
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("请先选择!");
                    }
                }
            });
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }
}
