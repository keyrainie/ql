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
    public partial class ECCategoryQuery : PageBase
    {
        private ECCategoryQueryVM _queryVM;
        public ECCategoryQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.lstChannelList.ItemsSource = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();

            _queryVM = new ECCategoryQueryVM();

            this.ucMaintain.EditCompleted += new EventHandler<ECCategoryActionEventArgs>(ucMaintain_EditCompleted);
            this.ucMaintain.CancelClick += new EventHandler(ucMaintain_CancelClick);
            this.ucMaintain.AddCompleted += new EventHandler<ECCategoryActionEventArgs>(ucMaintain_AddCompleted);
            this.ucMaintain.DeleteCompleted += new EventHandler<ECCategoryActionEventArgs>(ucMaintain_DeleteCompleted);
        }

        void ucMaintain_DeleteCompleted(object sender, ECCategoryActionEventArgs e)
        {
            var targetNode = e.Data;
            if (targetNode != null)
            {
                var selected = this.tvECCategoryTree.SelectedItem as TreeViewItem;
                if (selected != null)
                {
                    var parent = selected.Parent as TreeViewItem;
                    if (parent != null)
                    {
                        parent.Items.Remove(selected);
                    }
                }
            }
        }

        void ucMaintain_AddCompleted(object sender, ECCategoryActionEventArgs e)
        {
            var targetNode = e.Data;

            var entity = targetNode.ConvertVM<ECCategoryVM, ECCategory>();

            entity.WebChannel = new ECCentral.BizEntity.Common.WebChannel
            {
                ChannelID = targetNode.ChannelID
            };

            if (targetNode != null)
            {
                var selected = this.tvECCategoryTree.SelectedItem as TreeViewItem;
                if (selected != null)
                {
                    selected.Items.Add(new TreeViewItem()
                    {
                        Header = targetNode.DisplayName,
                        Tag = entity
                    });
                }
            }
        }

        void ucMaintain_CancelClick(object sender, EventArgs e)
        {
            OnTreeSelectedItemChanged(this.tvECCategoryTree.SelectedItem);
        }

        void ucMaintain_EditCompleted(object sender, ECCategoryActionEventArgs e)
        {
            var targetNode = e.Data;
            if (targetNode != null)
            {
                if (targetNode.Status == ADStatus.Deactive && this.cbShowActiveOnly.IsChecked == true)
                {
                    //删除选中的节点
                    var parent = this.tvECCategoryTree.GetParentItem(this.tvECCategoryTree.SelectedItem) as TreeViewItem;
                    if (parent != null)
                    {
                        parent.Items.Remove(this.tvECCategoryTree.SelectedItem);
                    }

                }
                else
                {
                    //更新节点的名称
                    var selected = this.tvECCategoryTree.SelectedItem as TreeViewItem;
                    if (selected != null)
                    {
                        selected.Header = targetNode.DisplayName;
                    }
                }
            }
        }

        private void BuildTree(ECCategory root, ItemsControl tvRoot)
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
            if (root.Status == ADStatus.Deactive)
            {
                foreColor = Colors.Red;
            }
            tvItem.Foreground = new SolidColorBrush(foreColor);

            tvRoot.Items.Add(tvItem);
            if (root.ChildrenList != null)
            {
                foreach (var c in root.ChildrenList)
                {
                    BuildTree(c, tvItem);
                }
            }
        }

        private void tvECCategoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            OnTreeSelectedItemChanged(e.NewValue);
        }

        private void OnTreeSelectedItemChanged(object selectedItem)
        {
            var selectedNode = selectedItem as TreeViewItem;
            if (selectedNode == null) return;
            var data = selectedNode.Tag as ECCategory;
            if (data == null) return;  

            if (data.SysNo.HasValue && data.SysNo > 0)
            {
                this.ucMaintain.ChangeToEditMode(data.DeepCopy().Convert<ECCategory, ECCategoryVM>((entity, vm) =>
                {
                    if (entity.WebChannel != null)
                    {
                        vm.ChannelID = entity.WebChannel.ChannelID;                           
                    }
                }));

                this.ucMaintain.BindDataGrid();
            }
            else
            {
                ECCategoryVM toCreateVM = new ECCategoryVM();
                toCreateVM.ChannelID = this.lstChannelList.SelectedValue.ToString();
                this.ucMaintain.ChangeToCreateMode(toCreateVM);
            }
        }

        private void lstChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadECCategoryTree();
        }

        private void cbShowActiveOnly_Click(object sender, RoutedEventArgs e)
        {
            LoadECCategoryTree();
        }

        private void LoadECCategoryTree()
        {
            if (this.lstChannelList.SelectedValue == null)
            {
                return;
            }
            string channelID = this.lstChannelList.SelectedValue.ToString();
            ADStatus? status = this.cbShowActiveOnly.IsChecked == true ? (ADStatus?)ADStatus.Active : null;
            new ECCategoryFacade(this).LoadTree(channelID, status, (s, args) =>
            {
                if (args.FaultsHandle()) return;
                this.tvECCategoryTree.Items.Clear();
                BuildTree(args.Result, this.tvECCategoryTree);
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
                ExpandTreeViewItemIfFound(this.tvECCategoryTree.Items, (model) => model.SysNo == sysNo);
            }
            else
            {
                ExpandTreeViewItemIfFound(this.tvECCategoryTree.Items, (model) => string.Compare(model.Name, input, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }

        /// <summary>
        /// 根据设置的predicate条件动态张开TreeViewItem节点
        /// </summary>
        /// <param name="items">TreeView的items</param>
        /// <param name="predicate">查找节点的条件</param>
        private void ExpandTreeViewItemIfFound(ItemCollection items, Predicate<ECCategory> predicate)
        {
            foreach (var item in items)
            {
                var tvItem = item as TreeViewItem;
                if (tvItem != null)
                {
                    var model = tvItem.Tag as ECCategory;
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
    }

}
