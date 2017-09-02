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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductAccessoriesQueryConditionSet : UserControl
    {
        public IDialog Dialog { get; set; }
        private ProductAccessoriesFacade facade;
        private ProductAccessoriesQueryConditionVM model;
        public int SysNo { private get; set; } //配件查询的SysNo
        private List<ProductAccessoriesQueryConditionVM> list; //保存datagrid中的值 
        public bool IsTreeQuery { get; set; }
        public ProductAccessoriesQueryConditionSet()
        {
            InitializeComponent();
            this.ConditionSetResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(ConditionSetResult_LoadingDataSource);
            this.Loaded += (sender, e) =>
            {
                facade = new ProductAccessoriesFacade();
                model = new ProductAccessoriesQueryConditionVM();
                this.DataContext = model;
                if (IsTreeQuery)
                {
                    sPPreant.Visibility = Visibility.Visible;
                }
                else
                {
                    sPPreant.Visibility = Visibility.Collapsed;
                }
                this.ConditionSetResult.Bind();
            };

        }

        void ConditionSetResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            List<AccessoriesQueryCondition> tempList = new List<AccessoriesQueryCondition>();
            facade.GetAccessoriesQueryConditionBySysNo(SysNo, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }

                this.ConditionSetResult.ItemsSource = arg.Result.Rows;
                tempList.Add(new AccessoriesQueryCondition() { ConditionName = "--请选择--", SysNo = 0 });
                foreach (var item in arg.Result.Rows)
                {
                    tempList.Add(new AccessoriesQueryCondition() { ConditionName = item.ConditionName, Priority = (PriorityType)item.Level, SysNo = item.SysNo});
                }
                dynamic d = this.ConditionSetResult.ItemsSource as dynamic;
                list= new List<ProductAccessoriesQueryConditionVM>();
                foreach (var item in d)
                {
                    list.Add(new ProductAccessoriesQueryConditionVM()
                    {
                        Condition = new AccessoriesQueryCondition() { ConditionName = item.ConditionName, Priority = (PriorityType)item.Level, SysNo = item.SysNo },
                        ParentCondition = new AccessoriesQueryCondition() { ConditionName = item.ParentConditionName, SysNo = item.ParentSysNo },
                        Priority = (PriorityType)item.Level
                    });
                }
             
                model.ParentConditionList = tempList;
                cboParentConditionList.SelectedIndex = 0;
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

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.ConditionSetResult.SelectedItem as dynamic;
            model.Condition.ConditionName = d.ConditionName;
            model.Condition.SysNo = d.SysNo;
            model.Priority = (PriorityType)d.Level;
            model.Condition.Priority = model.Priority;
            model.ParentCondition.ConditionName = string.IsNullOrEmpty(d.ParentConditionName) ? "--请选择--" : d.ParentConditionName;
            model.ParentCondition.SysNo = d.ParentSysNo;

            //编辑时从list中拿掉编辑的model，才可以和新增用同样的检查了逻辑
            var templist = (from p in list where p.Condition.SysNo != model.Condition.SysNo select p).ToList(); 
            list = templist;
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            dynamic d = this.ConditionSetResult.SelectedItem as dynamic;
            facade.DeleteAccessoriesQueryCondition((int)d.SysNo,(obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("删除成功!");
                this.ConditionSetResult.Bind();
            });
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

            if (CheckModel())
            {
                model.Condition.MasterSysNo = SysNo;
                if (model.Condition.SysNo > 0)
                {
                    facade.UpdateAccessoriesQueryCondition(model, (obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功!");
                        this.ConditionSetResult.Bind();
                    });
                }
                else
                {
                    facade.CreateAccessoriesQueryCondition(model, (obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {
                            return;
                        }
                        CPApplication.Current.CurrentPage.Context.Window.Alert("提交成功!");
                        this.ConditionSetResult.Bind();
                    });
                }
            }
        }
        /// <summary>
        /// check model
        /// </summary>
        /// <returns></returns>
        private bool CheckModel()
        {
            
            if (!ValidationManager.Validate(this))
            {
                return false;
            }
           

            var flag = (from p in list where (int)p.Priority == (int)model.Priority select p).ToList();
            if (flag.Count > 0) //检测优先级
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("优先级不能重复!");
                return false;
            }
           
            //检测父级
            if (IsTreeQuery) //树形结构才需要检查父级
            {
                flag = (from p in list where p.ParentCondition.SysNo == model.ParentCondition.SysNo select p).ToList();
                if ((flag.Count > 0 || model.ParentCondition.SysNo == 0) && model.Priority != PriorityType.One)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("该配件查询条件的父级设置错误!");
                    return false;
                }
            }
            return true;

        }
    }
}
