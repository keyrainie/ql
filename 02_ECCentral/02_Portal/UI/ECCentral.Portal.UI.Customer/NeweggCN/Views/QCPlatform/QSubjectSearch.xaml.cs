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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.NeweggCN.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.NeweggCN.Models;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class QSubjectSearch : PageBase
    {
        private QCSubjectQueryVM viewModel;
        private QCSubject oraginSubject;
        public QSubjectSearch()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            var list = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            list.Insert(0, new UIWebChannel() { ChannelName = ResCommonEnum.Enum_Select });
            this.lstChannelList.ItemsSource = list;
            this.lstChannelList.SelectedIndex = 1;

            viewModel = new QCSubjectQueryVM();
            this.DataContext = viewModel;
            CheckRights();
        }


        private void BuildTree(QCSubject root, ItemsControl tvRoot)
        {
            TreeViewItem tvItem = new TreeViewItem();

            tvItem.Header = root.Name;
            tvItem.Tag = root;
            //只展开Root节点
            bool isRoot = root.SysNo.HasValue && root.SysNo.Value == 0;
            tvItem.IsExpanded = isRoot;
            tvItem.IsSelected = isRoot;

            Color foreColor = Colors.Black;
            if (root.Status == QCSubjectStatus.Hidden)
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

        private void tvQCSubjectTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            OnTreeSelectedItemChanged(e.NewValue);
        }

        private void OnTreeSelectedItemChanged(object selectedItem)
        {
            var selectedNode = selectedItem as TreeViewItem;
            if (selectedNode == null) return;
            var data = selectedNode.Tag as QCSubject;
            if (data == null) return;
            if (data.SysNo == 0)// 选中root则为添加模式
            {
                this.SetCreateMode();
                return;
            }
            if (data.SysNo.HasValue && data.SysNo > 0)
            {
                var model = data.DeepCopy().Convert<QCSubject, QCSubjectVM>();
                new QCSubjectFacade(this).GetParents(model, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result.Count != 0)
                        cmbParents.ItemsSource = args.Result;
                    else
                        SetCmbParentAsRoot();
                    viewModel.QCSubject = model;
                });
            }
            else
            {
                SetCmbParentAsRoot();
            }
        }
        private void SetCmbParentAsRoot()
        {
            viewModel.QCSubject = new QCSubjectVM();
            var list = new List<QCSubject>();
            list.Add(new QCSubject() { SysNo = 0, Name = "Root" });
            cmbParents.ItemsSource = list;
        }

        private void lstChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadQCSubjectTree();
        }

        private void cbShowActiveOnly_Click(object sender, RoutedEventArgs e)
        {
            LoadQCSubjectTree();
        }

        private void LoadQCSubjectTree()
        {
            if (this.lstChannelList.SelectedValue == null)
            {
                return;
            }
            string channelID = this.lstChannelList.SelectedValue.ToString();
            QCSubjectStatus? status = this.cbShowActiveOnly.IsChecked.Value ? null : (QCSubjectStatus?)QCSubjectStatus.Show;
            new QCSubjectFacade(this).LoadTree(channelID, status, (s, args) =>
            {

                if (args.FaultsHandle())
                    return;
                oraginSubject = args.Result.DeepCopy();
                this.tvQCSubjectTree.Items.Clear();
                BuildTree(args.Result, this.tvQCSubjectTree);
                if (!string.IsNullOrEmpty(this.txtSysNoOrName.Text.Trim()))
                {
                    ExpandTreeViewItemIfFound(this.tvQCSubjectTree.Items, (model) => string.Compare(model.Name, this.txtSysNoOrName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase) == 0);
                }
            });
        }

        private void txtSysNoOrName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = this.txtSysNoOrName.Text.Trim();
                int sysNo = 0;
                if (int.TryParse(input, out sysNo))
                {
                    ExpandTreeViewItemIfFound(this.tvQCSubjectTree.Items, (model) => model.SysNo == sysNo);
                }
                else
                {
                    ExpandTreeViewItemIfFound(this.tvQCSubjectTree.Items, (model) => string.Compare(model.Name, input, StringComparison.InvariantCultureIgnoreCase) == 0);
                }

            }

        }

        /// <summary>
        /// 根据设置的predicate条件动态张开TreeViewItem节点
        /// </summary>
        /// <param name="items">TreeView的items</param>
        /// <param name="predicate">查找节点的条件</param>
        private void ExpandTreeViewItemIfFound(ItemCollection items, Predicate<QCSubject> predicate)
        {
            foreach (var item in items)
            {
                var tvItem = item as TreeViewItem;
                var model = tvItem.Tag as QCSubject;
                if (model != null && predicate(model))
                {
                    tvItem.IsSelected = true;
                    TreeViewItem tvParent = tvItem;
                    while (tvParent != null)
                    {
                        tvParent.IsExpanded = true;
                        tvParent = tvParent.Parent as TreeViewItem;
                    }
                    break;
                }
                ExpandTreeViewItemIfFound(tvItem.Items, predicate);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            SetCreateMode();
        }
        private void SetCreateMode()
        {
            viewModel.QCSubject = new QCSubjectVM();
            var list = oraginSubject.ChildrenList.DeepCopy();
            list.Insert(0, new QCSubject() { SysNo = 0, Name = "Root" });
            cmbParents.ItemsSource = list;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.gridMantain);
            if (viewModel.HasValidationErrors)
                return;
            if (viewModel.QCSubject.SysNo != null)
            {
                new QCSubjectFacade(this).Update(viewModel.QCSubject, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    Window.Alert("更新成功！");
                    ReShowWhenUpdated(viewModel.QCSubject);
                });
            }
            else
            {
                new QCSubjectFacade(this).Create(viewModel.QCSubject, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    Window.Alert("新增成功！");
                    ReShowWhenUpdated(viewModel.QCSubject);
                });
            }
        }
        private void ReShowWhenUpdated(QCSubjectVM vm)
        {
            this.txtSysNoOrName.Text = vm.Name;
            if (vm.Status == QCSubjectStatus.Hidden)
                cbShowActiveOnly.IsChecked = true;
            LoadQCSubjectTree();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadQCSubjectTree();
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_QSubject_Add))
                this.btnNew.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_Qsubject_Edit))
                this.btnSave.IsEnabled = false;
        }
        #endregion
    }

}
