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
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ECDynamicCategoryQuery : PageBase
    {
        public ECDynamicCategoryQueryVM QueryVM { get; private set; }

        public ECDynamicCategoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);            
            
            QueryVM = new ECDynamicCategoryQueryVM();
            this.GridCondition.DataContext = QueryVM;
            this.ucMaintain.VM = new ECDynamicCategoryVM { CategoryType = this.QueryVM.CategoryType };

            this.ucMaintain.EditCompleted += new EventHandler<ECDynamicCategoryActionEventArgs>(ucMaintain_EditCompleted);
            this.ucMaintain.CancelClick += new EventHandler(ucMaintain_CancelClick);
            this.ucMaintain.AddCompleted += new EventHandler<ECDynamicCategoryActionEventArgs>(ucMaintain_AddCompleted);
            this.ucMaintain.DeleteCompleted += new EventHandler<ECDynamicCategoryActionEventArgs>(ucMaintain_DeleteCompleted);            
        }

        void ucMaintain_DeleteCompleted(object sender, ECDynamicCategoryActionEventArgs e)
        {
            var targetNode = e.Data;
            if (targetNode != null)
            {
                var selected = this.tvECDynamicCategoryTree.SelectedItem as TreeViewItem;
                if (selected != null)
                {
                    var parent = selected.Parent as TreeViewItem;
                    if (parent != null)
                    {
                        if (this.QueryVM.IsOnlyShowActive)
                        {
                            parent.Items.Remove(selected);
                        }
                        else
                        {
                            var foreColor = Colors.Red;
                            selected.Foreground = new SolidColorBrush(foreColor);         
                        }                                      
                    }
                }
            }
        }

        void ucMaintain_AddCompleted(object sender, ECDynamicCategoryActionEventArgs e)
        {
            var targetNode = e.Data;

            var entity = targetNode.ConvertVM<ECDynamicCategoryVM, ECDynamicCategory>();           

            if (targetNode != null)
            {
                var selected = this.tvECDynamicCategoryTree.SelectedItem as TreeViewItem;
                if (selected != null)
                {
                    var item = new TreeViewItem
                    {
                        Header = string.Format("[{0}]{1}", targetNode.SysNo, targetNode.Name),
                        Tag = entity
                    };
                    selected.Items.Add(item);
                    this.tvECDynamicCategoryTree.SelectItem(item);
                }
            }
        }

        void ucMaintain_CancelClick(object sender, EventArgs e)
        {
            OnTreeSelectedItemChanged(this.tvECDynamicCategoryTree.SelectedItem);
        }

        void ucMaintain_EditCompleted(object sender, ECDynamicCategoryActionEventArgs e)
        {
            var targetNode = e.Data;
            if (targetNode != null)
            {
                if (targetNode.Status == DynamicCategoryStatus.Deactive && this.cbShowActiveOnly.IsChecked == true)
                {
                    //删除选中的节点
                    var parent = this.tvECDynamicCategoryTree.GetParentItem(this.tvECDynamicCategoryTree.SelectedItem) as TreeViewItem;
                    if (parent != null)
                    {
                        parent.Items.Remove(this.tvECDynamicCategoryTree.SelectedItem);
                    }
                }
                else
                {
                    Color foreColor = Colors.Black;
                    if (targetNode.IsDeActive)
                    {
                        foreColor = Colors.Red;
                    }                   
                   
                    //更新节点的名称
                    var selected = this.tvECDynamicCategoryTree.SelectedItem as TreeViewItem;
                    if (selected != null)
                    {
                        selected.Header = string.Format("[{0}]{1}", targetNode.SysNo, targetNode.Name);
                        selected.Foreground = new SolidColorBrush(foreColor);
                    }
                    selected.Tag = e.Data.DeepCopy().ConvertVM<ECDynamicCategoryVM,ECDynamicCategory>();
                }
            }
        }

        private void BuildTree(ECDynamicCategory root, ItemsControl tvRoot)
        {
            TreeViewItem tvItem = new TreeViewItem();

            tvItem.Header = (root.SysNo.HasValue && root.SysNo.Value > 0 ? "[" + root.SysNo.Value + "]" : "") + root.Name;
            tvItem.Tag = root;
            //只展开Root节点
            bool isRoot = root.SysNo.HasValue && root.SysNo.Value == 0;
            tvItem.IsExpanded = isRoot;
            tvItem.IsSelected = isRoot;
            if (!isRoot)
            {
                ToolTipService.SetToolTip(tvItem, string.Format(ResECCategory.TextBlock_TreeNodeTip, root.Priority));
            }
            Color foreColor = Colors.Black;
            if (root.Status == DynamicCategoryStatus.Deactive)
            {
                foreColor = Colors.Red;
            }
            tvItem.Foreground = new SolidColorBrush(foreColor);

            tvRoot.Items.Add(tvItem);
            if (root.SubCategories != null)
            {
                foreach (var c in root.SubCategories)
                {
                    BuildTree(c, tvItem);
                }
            }
        }

        private void tvECDynamicCategoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            OnTreeSelectedItemChanged(e.NewValue);
        }

        private void OnTreeSelectedItemChanged(object selectedItem)
        {
            var selectedNode = selectedItem as TreeViewItem;
            if (selectedNode == null) return;
            var data = selectedNode.Tag as ECDynamicCategory;
            if (data == null) return;  

            if (data.SysNo.HasValue && data.SysNo > 0)
            {
                var vm = data.DeepCopy().Convert<ECDynamicCategory, ECDynamicCategoryVM>();               

                this.ucMaintain.SelectedVM = vm;

                this.ucMaintain.BindDataGrid();
            }
            else
            {
                ECDynamicCategoryVM vm = new ECDynamicCategoryVM { CategoryType = this.QueryVM.CategoryType };

                this.ucMaintain.SelectedVM = vm;
            }
        }      

        private void cbShowActiveOnly_Click(object sender, RoutedEventArgs e)
        {
            LoadECCategoryTree();
        }

        private void LoadECCategoryTree()
        {                      
            new ECDynamicCategoryFacade(this).LoadTree(this.QueryVM, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                this.tvECDynamicCategoryTree.Items.Clear();
                BuildTree(args.Result, this.tvECDynamicCategoryTree);
            });
        }

        private void txtSysNoOrName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSearchTree();
            }
        }

        private void txtSysNoOrName_ICONClick(object sender, EventArgs e)
        {
            OnSearchTree();
        }

        private void OnSearchTree()
        {
            string input = this.txtSysNoOrName.Text.Trim();
            int sysNo = 0;
            if (int.TryParse(input, out sysNo))
            {
                ExpandTreeViewItemIfFound(this.tvECDynamicCategoryTree.Items, (model) => model.SysNo == sysNo);
            }
            else
            {
                ExpandTreeViewItemIfFound(this.tvECDynamicCategoryTree.Items, (model) => string.Compare(model.Name, input, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }

        /// <summary>
        /// 根据设置的predicate条件动态张开TreeViewItem节点
        /// </summary>
        /// <param name="items">TreeView的items</param>
        /// <param name="predicate">查找节点的条件</param>
        private void ExpandTreeViewItemIfFound(ItemCollection items, Predicate<ECDynamicCategory> predicate)
        {
            foreach (var item in items)
            {
                var tvItem = item as TreeViewItem;
                if (tvItem != null)
                {
                    var model = tvItem.Tag as ECDynamicCategory;
                    if (model != null && predicate(model))
                    {
                        tvItem.IsSelected = true;
                        TreeViewItem tvParent = tvItem;
                        while (tvParent != null)
                        {
                            tvParent.IsExpanded = true;
                            tvParent = tvParent.Parent as TreeViewItem;
                        }
                    }
                    ExpandTreeViewItemIfFound(tvItem.Items, predicate);
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadECCategoryTree();
        }

        private void cmbCategoryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ucMaintain.VM != null && this.ucMaintain.VM.SysNo == null)
            {
                this.ucMaintain.VM.CategoryType = this.QueryVM.CategoryType;
            }
            
            LoadECCategoryTree();
        }       
    }

}
