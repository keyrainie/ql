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
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class CategoryPropertyBatEdit : UserControl
    {
        private CategoryPropertyFacade facade;
        /// <summary>
        /// 批量更新的数据源
        /// </summary>
        public List<CategoryPropertyVM> DataSource
        {
            set;
           private get;
        }
        public IDialog Dialog { get; set; }
        public CategoryPropertyBatEdit()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                this.dgCategoryPropertyResult.ItemsSource = DataSource;
                facade = new CategoryPropertyFacade();
            };
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

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<CategoryPropertyVM> list = new List<CategoryPropertyVM>();
            List<CategoryPropertyVM> tempList = this.dgCategoryPropertyResult.ItemsSource as List<CategoryPropertyVM>;
            foreach (var item in tempList)
            {
                item.IsInAdvSearch = item.IsInAdvSearchBat == true ? CategoryPropertyStatus.Yes : CategoryPropertyStatus.No;
                item.IsItemSearch = item.IsItemSearchBat == true ? CategoryPropertyStatus.Yes : CategoryPropertyStatus.No;
                item.IsMustInput = item.IsMustInputBat == true ? CategoryPropertyStatus.Yes : CategoryPropertyStatus.No;
                item.CategoryInfo = new CategoryVM();
                list.Add(item);
            }
            facade.UpdateCategoryPropertyByList(list, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {

                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功");
                CloseDialog(DialogResultType.OK);
            });
        }
    }
}
