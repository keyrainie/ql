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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductAccessoriesQueryConditionValueMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public int MaterSysNo { private get; set; }
        private ProductAccessoriesConditionValueQueryVM model;
        private ProductAccessoriesFacade facade;
        private List<AccessoriesQueryCondition> ConditionList;
        public ProductAccessoriesQueryConditionValueMaintain()
        {
            InitializeComponent();
            ConditionValueSetResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ConditionValueSetResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                model = new ProductAccessoriesConditionValueQueryVM();
                ConditionList = new List<AccessoriesQueryCondition>() { new AccessoriesQueryCondition(){ConditionName="--全部--",SysNo=0}};
                model.MasterSysNo = MaterSysNo;
                facade = new ProductAccessoriesFacade();
                facade.GetAccessoriesQueryConditionBySysNo(MaterSysNo, (obj, arg) => 
                {
                    if (arg.FaultsHandle()) 
                    {
                        return;
                    }
                    dynamic d = arg.Result.Rows;
                    foreach (var item in d)
                    {
                        ConditionList.Add(new AccessoriesQueryCondition() {ConditionName=item.ConditionName,SysNo=item.SysNo,MasterSysNo=MaterSysNo,ParentSysNo=item.ParentSysNo});
                    }
                    model.ConditionList = ConditionList;
                    this.DataContext = model;
                    cboCondition.SelectedIndex = 0;
                    this.ConditionValueSetResult.Bind();
                });
            };
        }

        void ConditionValueSetResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetProductAccessoriesConditionValueByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.ConditionValueSetResult.ItemsSource = arg.Result.Rows;
                this.ConditionValueSetResult.TotalCount = arg.Result.TotalCount;
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.ConditionValueSetResult.Bind();
        }

        private void BtnCreat_Click(object sender, RoutedEventArgs e)
        {
            if ((from p in ConditionList where p.SysNo != 0 select p).ToList().Count == 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("该查询还没有设置查询条件,不能设置条件值!");
                return;
            }
            ProductAccessoriesQueryConditionValueSet item = new ProductAccessoriesQueryConditionValueSet() { MaterSysNo=MaterSysNo,IsEdit=false};
            item.ConditionList = (from p in ConditionList where p.SysNo != 0 select p).ToList();//去掉全部选项
            item.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("添加查询条件值", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ConditionValueSetResult.Bind();
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
            dynamic d = this.ConditionValueSetResult.SelectedItem as dynamic;
            ProductAccessoriesQueryConditionValueSet item = new ProductAccessoriesQueryConditionValueSet() { MaterSysNo = MaterSysNo, IsEdit = true };
            item.Data = new ProductAccessoriesConditionValueVM()
            {
                ConditionList = (from p in ConditionList where p.SysNo != 0 select p).ToList(),
                ConditionValue = new AccessoriesConditionValue() { ConditionValue = d.ConditionValue, SysNo = d.ConditionValueSysNo },
                ParentCondition = new AccessoriesQueryCondition() { ConditionName = d.ParentConditionName == null ? "" : d.ParentConditionName, SysNo = d.ParentConditionSysNo == null ? 0 : d.ParentConditionSysNo, MasterSysNo = MaterSysNo },
            };
            item.SysNo = d.ConditionValueSysNo;
            item.ConditionSysNo = d.ConditionSysNo;
            item.ParentConditionValueSysNo = d.ParentConditionValueSysNo == null ? 0 : d.ParentConditionValueSysNo;
            item.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("编辑查询条件值", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.ConditionValueSetResult.Bind();
                }
            }, new Size(600, 200));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.ConditionValueSetResult.SelectedItem as dynamic;
            facade.DeleteProductAccessoriesQueryConditionValue((int)d.ConditionValueSysNo, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("删除成功!");
                this.ConditionValueSetResult.Bind();
            });
        }
    }
}
