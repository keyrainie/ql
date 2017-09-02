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
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductAccessoriesQueryConditionValueSet : UserControl
    {
        public IDialog Dialog { get; set; }
        public List<AccessoriesQueryCondition> ConditionList { private get; set; } //条件集合
        public int MaterSysNo { private get; set; }// 查询功能SysNo
        public int SysNo { private get; set; } //选项值SysNo
        public ProductAccessoriesConditionValueVM Data { private get; set; }//编辑时需要接受数据源
        public int ConditionSysNo { private get; set; } //编辑时需要bing的条件SysNo
        public int ParentConditionValueSysNo { private get; set; } //编辑时需要bing的选项值SysNo
        public bool IsEdit { private get; set; } //编辑还是新建
        private List<AccessoriesConditionValue> conditionValueList;
        private ProductAccessoriesFacade facade;
        private ProductAccessoriesConditionValueVM model;

        public ProductAccessoriesQueryConditionValueSet()
        {
            InitializeComponent();
            cboCondition.SelectionChanged += new SelectionChangedEventHandler(cboCondition_SelectionChanged);
            this.Loaded += (sender, e) =>
            {
                facade = new ProductAccessoriesFacade();
                if (IsEdit) //编辑
                {
                    this.DataContext = Data;
                    var con = (from p in Data.ConditionList where p.SysNo == ConditionSysNo select p).FirstOrDefault();
                    this.cboCondition.SelectedValue = con;
                    this.cboCondition.IsEnabled = false;
                }
                else
                {
                    model = new ProductAccessoriesConditionValueVM() { ConditionList = ConditionList };
                    this.DataContext = model;
                    if (model.ConditionList.Count > 0)
                    {
                        cboCondition.SelectedIndex = 0;
                    }
                }


            };
        }
        //选择条件时 动态加载该条件的所有父级值
        void cboCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AccessoriesQueryCondition condition = cboCondition.SelectedValue as AccessoriesQueryCondition;
            if (condition != null)
            {
                conditionValueList = new List<AccessoriesConditionValue>();
                AccessoriesQueryCondition tempCondition = new AccessoriesQueryCondition();
                //动态加载父节点的选项值
                facade.GetProductAccessoriesConditionValueByCondition(condition.SysNo, MaterSysNo, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    txtValue.IsEnabled = true;
                    dynamic d = arg.Result.Rows as dynamic;
                    foreach (var item in d)
                    {
                        if (string.IsNullOrEmpty(tempCondition.ConditionName))
                        {
                            tempCondition.ConditionName = item.ParentConditionName;
                            tempCondition.SysNo = item.ParentConditionSysNo ?? 0;
                        }
                        conditionValueList.Add(new AccessoriesConditionValue() { ConditionValue = item.ParentConditionValue, SysNo = item.ConditionValueSysNo });
                    }


                    if (conditionValueList.Count > 0) //父节点有值
                    {

                        if (!IsEdit) //新建时 父节点值默认选中第一个
                        {
                            model.ParentConditionValueList = conditionValueList;
                            model.ParentConditionValue = conditionValueList[0];
                        }
                        else //编辑时父节点值绑定
                        {
                            Data.ParentConditionValueList = conditionValueList;
                            var Parent = (from p in Data.ParentConditionValueList where p.SysNo == ParentConditionValueSysNo select p).FirstOrDefault();
                            Data.ParentConditionValue = Parent;

                        }
                        if (!IsEdit)//新建时
                        {
                            model.ParentCondition = tempCondition;
                        }
                        else
                        {
                            Data.ParentCondition = tempCondition;
                        }

                        this.spParent.Visibility = Visibility.Visible;
                    }
                    else if (condition.ParentSysNo > 0)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("父节点没有设置查询值,无法设置子节点值!", MessageType.Error);
                        txtValue.IsEnabled = false;
                    }
                    else
                    {
                        txtValue.IsEnabled = true;
                        this.spParent.Visibility = Visibility.Collapsed;
                    }
                });
            }
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (txtValue.IsEnabled == false)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("设置错误,无法保存!", MessageType.Error);
                return;
            }
            ProductAccessoriesConditionValueVM vm = this.DataContext as ProductAccessoriesConditionValueVM;
            vm.MasterSysNo = MaterSysNo;
          
            if (IsEdit) //更新
            {
                vm.SysNo = SysNo;
                facade.UpdateProductAccessoriesQueryConditionValue(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功!");
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                facade.CreateProductAccessoriesQueryConditionValue(vm, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert("创建成功!");
                    CloseDialog(DialogResultType.OK);
                });
            }


        }
    }
}
