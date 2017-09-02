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
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class CategoryList : UserControl
    {
        /// <summary>
        /// 0=首页，1=一级栏目推荐，2=二级栏目推荐
        /// </summary>
        public int Level_No { get; set; }
        /// <summary>
        /// Level_No=1时，存一级ECCategorySysNo；=2时，存放2级ECCategorySysNo；=0时，存0
        /// </summary>
        public int Level_Code { get; set; }

        BrandRecommendedFacade facade;
        private List<CategoryItem> list1;
        private List<CategoryItem> list2;
        private List<CategoryItem> list;
        public CategoryList()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CategoryList_Loaded);
            this.cbTopCatrory.SelectionChanged += new SelectionChangedEventHandler(cbTopCatrory_SelectionChanged);
            this.cbShow1Catrory.SelectionChanged += new SelectionChangedEventHandler(cbShow1Catrory_SelectionChanged);
        }

        void cbShow1Catrory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryItem item = this.cbShow1Catrory.SelectedItem as CategoryItem;
            if (item == null)
            {
                return;
            }
            int tempkey = item.Key;
            if (tempkey != -1)
            {
                var templist = (from p in list2 where p.ParentKey == tempkey select p).ToList();
                templist.Insert(0, new CategoryItem() { Key = -1, Value = "--所有--", ParentKey = -1 });
                this.cbShow2Catrory.ItemsSource = templist;
            }
            this.cbShow2Catrory.SelectedIndex = 0;

        }

        void cbTopCatrory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryItem item = this.cbTopCatrory.SelectedItem as CategoryItem;
            if (item == null)
            {
                return;
            }

            //this.spCategory.Visibility = System.Windows.Visibility.Visible;
            //this.cbShow2Catrory.Visibility = Visibility.Visible;

            //if (item.Key == 0)
            //{
            //    this.spCategory.Visibility = System.Windows.Visibility.Collapsed;
            //}
            if (item.Key == 1)
            {
                this.spCategory.Visibility = Visibility.Visible;
                this.cbShow2Catrory.Visibility = Visibility.Collapsed;
            }
            else if (item.Key == 2)
            {
                this.spCategory.Visibility = Visibility.Visible;
                this.cbShow2Catrory.Visibility = Visibility.Visible;
            }
            else
            {
                this.spCategory.Visibility = Visibility.Collapsed;
            }
            
        }

        void CategoryList_Loaded(object sender, RoutedEventArgs e)
        {
            list = new List<CategoryItem>();
            list1 = new List<CategoryItem>();
            list2 = new List<CategoryItem>();

            CodeNamePairHelper.GetList("MKT", "BrandRecommendType", CodeNamePairAppendItemType.None, (s, arg) =>
            {
                if (arg.FaultsHandle())
                    return;
                foreach (var item in arg.Result)
                {
                    CategoryItem citem = new CategoryItem() { Key = int.Parse(item.Code), Value = item.Name };
                    list.Add(citem);
                }
                this.cbTopCatrory.ItemsSource = list;
                this.cbTopCatrory.DisplayMemberPath = "Value";
                this.cbTopCatrory.SelectedValuePath = "Key";
                if (Level_No >= 0)
                {
                    this.cbTopCatrory.SelectedValue = Level_No;
                }
                else
                {
                    this.cbTopCatrory.SelectedIndex = 0;
                }
            });




            //一级类别bind
            list1.Add(new CategoryItem() { Key = -1, Value = "--所有--" });
            facade = new BrandRecommendedFacade();
            facade.GetCategory1List((obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                dynamic viewlist1 = arg.Result.Rows;
                if (viewlist1 != null)
                {
                    foreach (var item in viewlist1)
                    {
                        list1.Add(new CategoryItem() { Key = item.SysNo, Value = item.C1Name, ParentKey = 0 });

                    }
                }
                this.cbShow1Catrory.ItemsSource = list1;
                this.cbShow1Catrory.DisplayMemberPath = "Value";
                this.cbShow1Catrory.SelectedValuePath = "Key";
                this.cbShow1Catrory.SelectedIndex = 0;

                if (Level_No == 1 && Level_Code>0)
                {
                    cbShow1Catrory.SelectedValue = Level_Code;
                }

            });
            //二级类别bind
            list2.Add(new CategoryItem() { Key = -1, Value = "--所有--" });
            facade.GetCategory2List((obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                dynamic viewlist2 = arg.Result.Rows;
                if (viewlist2 != null)
                {
                    foreach (var item in viewlist2)
                    {
                        list2.Add(new CategoryItem() { Key = item.SysNo, Value = item.C2Name, ParentKey = item.C1SysNo });
                    }
                }
                this.cbShow2Catrory.ItemsSource = list2;
                this.cbShow2Catrory.DisplayMemberPath = "Value";
                this.cbShow2Catrory.SelectedValuePath = "Key";
                this.cbShow2Catrory.SelectedIndex = 0;
                if (Level_No == 2 && Level_Code > 0)
                {
                    cbShow2Catrory.SelectedValue = Level_Code;
                }
            });
        }

    }
    public class CategoryItem
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public int ParentKey { get; set; }
    }
}
