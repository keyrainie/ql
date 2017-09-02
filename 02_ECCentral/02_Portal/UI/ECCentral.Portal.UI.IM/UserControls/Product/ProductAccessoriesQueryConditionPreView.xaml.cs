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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductAccessoriesQueryConditionPreView : UserControl
    {
       
     
        public int MaterSysNo { private get; set; }// 查询功能SysNo
        private ProductAccessoriesFacade facade;
        private List<ProductAccessoriesQueryConditionVM> ConditionList;
        private List<AccessoriesConditionValue> ConditionValueList1;
        private List<AccessoriesConditionValue> ConditionValueList2;
        private List<AccessoriesConditionValue> ConditionValueList3;
        private List<AccessoriesConditionValue> ConditionValueList4;
         //公开属性
        public int ConditionValueSysNo1 { get; private set; }
        public int ConditionValueSysNo2 { get; private set; }
        public int ConditionValueSysNo3 { get; private set; }
        public int ConditionValueSysNo4 { get; private set; }
        public int NodeNumber { get; private set; } //节点数
        public bool IsTreeQuery { get; private set; }
        public ProductAccessoriesQueryConditionPreView()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                facade = new ProductAccessoriesFacade();
                ConditionList = new List<ProductAccessoriesQueryConditionVM>();
                ConditionValueList1 = new List<AccessoriesConditionValue>() ;
                ConditionValueList2 = new List<AccessoriesConditionValue>() ;
                ConditionValueList3 = new List<AccessoriesConditionValue>() ;
                ConditionValueList4 = new List<AccessoriesConditionValue>() ;
                IsTreeQuery = false;
                facade.GetAccessoriesQueryConditionBySysNo(MaterSysNo, (obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    foreach (var item in arg.Result.Rows)
                    {
                        IsTreeQuery = item.IsTreeQuery == "Y";
                        ConditionList.Add(new ProductAccessoriesQueryConditionVM()
                        {
                            Condition = new AccessoriesQueryCondition() { ConditionName = item.ConditionName, Priority = (PriorityType)item.Level, SysNo = item.SysNo},
                            ParentCondition = new AccessoriesQueryCondition() { ConditionName = item.ParentConditionName, SysNo = item.ParentSysNo },
                            Priority = (PriorityType)item.Level
                        });
                    }
                    BingControl();
                });
                
              
            };
        }

        /// <summary>
        /// bing control
        /// </summary>
        void BingControl()
        {
            if (ConditionList.Count > 0)
            {
                var data = (from p in ConditionList where p.ParentCondition.SysNo == 0 select p).FirstOrDefault();
                if (data != null)
                {
                    spCondition.Children.Add(GetStackPanel(data, 0));
                    Bing(data, 1);
                }
            }
        }

        /// <summary>
        /// 从父节点向下找，确定子节点
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="index"></param>
        private void Bing(ProductAccessoriesQueryConditionVM vm, int index)
        {
            if (index < ConditionList.Count)
            {   
                if (IsTreeQuery) //树形结构需要一层一层按节点加载
                {
                    var tempdata = (from p in ConditionList where p.ParentCondition.SysNo == vm.Condition.SysNo select p).FirstOrDefault();
                    if (tempdata != null)
                    {
                        spCondition.Children.Add(GetStackPanel(tempdata, index));
                        Bing(tempdata, index + 1);
                    }
                }
                else //平行结构直接加载
                {
                    spCondition.Children.Clear();
                    index = 0;
                    foreach (var item in ConditionList)
                    {
                        spCondition.Children.Add(GetStackPanel(item, index));
                        index = index + 1;
                    }
                }
            }
        }
        /// <summary>
        /// 返回生成的控件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private StackPanel GetStackPanel(ProductAccessoriesQueryConditionVM condition,int index)
        {
            TextBlock txt = new TextBlock() { VerticalAlignment = VerticalAlignment.Center };
            System.Windows.Data.Binding bing = new System.Windows.Data.Binding("Condition.ConditionName");
            txt.SetBinding(TextBlock.TextProperty, bing);
            ComboBox cb = new ComboBox() { Width = 120, Height = 25, Margin = new Thickness(20, 0, 0, 0) };
            cb.Name = "cb" + index.ToString();
            cb.DisplayMemberPath = "ConditionValue";
            cb.SelectedValuePath = "SysNo";
            cb.SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);
             StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            sp.Children.Add(txt);
            sp.Children.Add(cb);
            sp.DataContext = condition;
            NodeNumber = index;
            switch (index) //得到每个条件的数据源
            {
                case 0:
                    ConditionValueList1 = GetConditionValueList(condition.Condition.SysNo,index);
                    break;
                case 1:
                    ConditionValueList2 = GetConditionValueList(condition.Condition.SysNo, index);
                     break;
                case 2:
                    ConditionValueList3 = GetConditionValueList(condition.Condition.SysNo, index);
                    break;
                case 3:
                    ConditionValueList4 = GetConditionValueList(condition.Condition.SysNo, index);
                    break;
                default:
                    break;
            }
           
           
            return sp;
        }

        /// <summary>
        /// 所有Combox的cb_SelectionChanged事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            ComboBox cb = (ComboBox)sender;
            int sysno = (int)(cb.SelectedValue == null ? 0 : cb.SelectedValue);
            switch (cb.Name)
            {
                case "cb0":
                    if (IsTreeQuery) //树结构需要级联效果
                    {
                        if (spCondition.FindName("cb1") != null)
                        {
                            ((ComboBox)spCondition.FindName("cb1")).ItemsSource = GetComboxItemSource(ConditionValueList2, sysno);
                            ((ComboBox)spCondition.FindName("cb1")).SelectedIndex = 0;
                        }
                    }
                    ConditionValueSysNo1 = sysno;
                    break;
                case "cb1":
                    if (IsTreeQuery)
                    {
                        if (spCondition.FindName("cb2") != null)
                        {
                            ((ComboBox)spCondition.FindName("cb2")).ItemsSource = GetComboxItemSource(ConditionValueList3, sysno);
                            ((ComboBox)spCondition.FindName("cb2")).SelectedIndex = 0;

                        }
                    }
                    ConditionValueSysNo2 = sysno;
                    break;
                case "cb2":
                    if (IsTreeQuery)
                    {
                        if (spCondition.FindName("cb3") != null)
                        {
                            ((ComboBox)spCondition.FindName("cb3")).ItemsSource = GetComboxItemSource(ConditionValueList4, sysno);
                            ((ComboBox)spCondition.FindName("cb3")).SelectedIndex = 0;

                        }
                    }
                    ConditionValueSysNo3 = sysno;
                    break;
                case "cb3":
                    ConditionValueSysNo4 = sysno;
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 返回ComBox的数据源
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sysno"></param>
        /// <returns></returns>
        private List<AccessoriesConditionValue> GetComboxItemSource(List<AccessoriesConditionValue> list,int sysno)
        {
            List<AccessoriesConditionValue> tempdata;
            if (IsTreeQuery)
            {
                 tempdata = (from p in list
                                where p.ParentSysNo == sysno
                                select p).ToList();
            }
            else
            {
                tempdata = (from p in list
                            where 1==1 
                            select p).ToList();
            }
            if (tempdata.Count == 0)
            {
                tempdata.Insert(0, new AccessoriesConditionValue() { ConditionValue = "--请选择--", SysNo = 0, ParentSysNo = 0 });
            }
             return tempdata;
        }
        /// <summary>
        /// 根据条件SysNo加载选项值
        /// </summary>
        /// <param name="ConditionSysNo"></param>
        /// <returns></returns>
        private List<AccessoriesConditionValue> GetConditionValueList(int ConditionSysNo,int index)
        {
            List<AccessoriesConditionValue> list = new List<AccessoriesConditionValue>();
            facade.GetConditionValueByQuery(ConditionSysNo, MaterSysNo, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                list.Add(new AccessoriesConditionValue() { ConditionValue = "--请选择--", SysNo = 0, ParentSysNo = 0 });
                foreach (var item in arg.Result.Rows)
                {
                    list.Add(new AccessoriesConditionValue()
                    {
                        ConditionValue = item.ConditionValue,
                        SysNo = item.SysNo,
                        ParentSysNo = item.ParentSysNo
                    });
                }
                //初始化加载
                if (index == 0) 
                {
                    ((ComboBox)spCondition.FindName("cb0")).ItemsSource = list;
                    ((ComboBox)spCondition.FindName("cb0")).SelectedIndex = 0;
                }
                if (index == 1) 
                {
                    ((ComboBox)spCondition.FindName("cb1")).ItemsSource = list;
                    ((ComboBox)spCondition.FindName("cb1")).SelectedIndex = 0;
                }
                if (index == 2) 
                {
                    ((ComboBox)spCondition.FindName("cb2")).ItemsSource = list;
                    ((ComboBox)spCondition.FindName("cb2")).SelectedIndex = 0;
                }
                if (index == 3) 
                {
                    ((ComboBox)spCondition.FindName("cb3")).ItemsSource = list;
                    ((ComboBox)spCondition.FindName("cb3")).SelectedIndex = 0;
                }
            });
            return list;
           
        }


      
    }
}
