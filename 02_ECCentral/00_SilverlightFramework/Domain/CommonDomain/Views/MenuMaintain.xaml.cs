using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newegg.Oversea.Silverlight.CommonDomain.UserControls;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using Newegg.Oversea.Silverlight.CommonDomain.Models;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.CommonDomain.Controls;
using System.Collections;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities;
using System.IO;
using System.Text;
using Newegg.Oversea.Silverlight.CommonDomain.Utilities;

namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View]
    public partial class MenuMaintain : PageBase
    {
        private bool m_canTreeViewItemDrop;
        private bool m_isEditTree;

        private UCAddLinkPage addLinkPage;
        private RestClient m_restServiceClient;
        private MenuItemModel m_menuContextMenuItem;
        private MenuExporter m_menuContoller;
        private ObservableCollection<MenuItemModel> m_menuTree;
        private ObservableCollection<MenuItemModel> m_menuSearchCollection;
        private ObservableCollection<MenuItemModel> m_menuCollection;


        public MenuMaintain()
        {
            InitializeComponent();

            this.m_menuContoller = new MenuExporter(this);
            this.m_restServiceClient = new RestClient("/Service/Framework/V50/MenuRestService.svc", this);
        }


        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);


            this.Window.DocumentVerticalScrollBar = ScrollBarVisibility.Disabled;

            this.ComboBoxApplidations.ItemsSource = CPApplication.Current.KS_Applications;

            m_restServiceClient.Query<ObservableCollection<MenuItemModel>>("", (target, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var result = args.Result.Clone();
                m_menuCollection = args.Result;
                InitLinkMenuItem(result);

                this.tvMenu.ItemsSource = result.GenerateMenuTree();

                this.btnImportMenu.IsEnabled = true;
                this.btnReloadTree.IsEnabled = true;
                this.btnSaveTree.IsEnabled = true;
                this.btnEditTree.IsEnabled = true;
            });

            this.dtTree.AllowDrop = false;
            this.tvTree.AllowDrop = false;

            this.ToEditorMode();
            this.ResetEditor();
        }

        # region Context Menu Events

        private void cmlExportAllMenu_Click(object sender, RoutedEventArgs e)
        {
            m_menuContoller.Export(null);
        }

        private void cmlExportMenu_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as FrameworkElement;
            if (selectedItem != null)
            {
                var dataItem = selectedItem.DataContext as MenuItemModel;
                if (dataItem != null)
                {
                    m_menuContoller.Export(dataItem);
                }
            }
        }

        private void cmiAddCategory_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement menuItem = sender as FrameworkElement;
            if (menuItem != null)
            {
                MenuItemModel dataItem = menuItem.DataContext as MenuItemModel;
                if (dataItem == null)
                {
                    ComboBoxApplidations.IsEnabled = true;
                }
                else
                {
                    ComboBoxApplidations.IsEnabled = false;
                }

                this.gridEditor.Tag = new MenuItemModel().NewCategory(dataItem);
                this.gridEditor.DataContext = new MenuItemModel().NewCategory(dataItem);

                this.btnSave.IsEnabled = true;
                this.btnCancel.IsEnabled = true;
            }
        }

        private void cmiAddPage_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement menuItem = sender as FrameworkElement;
            if (menuItem != null && menuItem.DataContext is MenuItemModel)
            {
                MenuItemModel dataItem = menuItem.DataContext as MenuItemModel;

                if (dataItem == null)
                {
                    ComboBoxApplidations.IsEnabled = true;
                }
                else
                {
                    ComboBoxApplidations.IsEnabled = false;
                }

                this.gridEditor.Tag = new MenuItemModel().NewPage(dataItem);
                this.gridEditor.DataContext = new MenuItemModel().NewPage(dataItem);

                this.btnSave.IsEnabled = true;
                this.btnCancel.IsEnabled = true;
            }
        }

        private void cmiDelete_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as FrameworkElement;
            var dataContext = menuItem.DataContext as MenuItemModel;

            this.Window.Confirm(null, (dataContext.Type == "P" ? MenuMaintainResource.Confirm_Message_DeletePage : MenuMaintainResource.Confirm_Message_DeleteItem), (elem, result) =>
            {
                if (result.DialogResult == DialogResultType.OK)
                {
                    if (menuItem != null && menuItem.DataContext is MenuItemModel)
                    {
                        var originalItem = menuItem.DataContext as MenuItemModel;

                        if (originalItem.Children != null && originalItem.Children.Count > 0)
                        {
                            this.Window.Alert(MenuMaintainResource.Alert_Warning_Delete_HasSubItems, MessageType.Warning);

                            return;
                        }

                        this.m_restServiceClient.Delete("Delete", originalItem.MenuId.ToString(), (target, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }

                            if (dataContext.Type == "P")
                            {
                                m_restServiceClient.Query<ObservableCollection<MenuItemModel>>("", (o, a) =>
                                {
                                    if (args.FaultsHandle())
                                    {
                                        return;
                                    }

                                    var r = a.Result.Clone();
                                    m_menuCollection = a.Result;

                                    InitLinkMenuItem(r);

                                    this.tvMenu.ItemsSource = r.GenerateMenuTree();

                                    this.Window.MessageBox.Show(CommonResource.Info_SaveSuccessfully, MessageBoxType.Success);
                                });
                            }
                            else
                            {
                                var parentItems = (originalItem != null && originalItem.Parent != null) ? (originalItem.Parent.Children) : (this.tvMenu.ItemsSource as ObservableCollection<MenuItemModel>);
                                var isEditorUse = (originalItem == this.gridEditor.Tag);

                                if (parentItems != null)
                                {
                                    parentItems.Remove(originalItem);

                                    UpdateNormalDataSource(originalItem, "D");

                                    if (isEditorUse)
                                    {
                                        this.ResetEditor();
                                    }
                                }

                                this.Window.MessageBox.Show(CommonResource.Info_SaveSuccessfully, MessageBoxType.Success);
                            }
                        });
                    }
                }
            }, ButtonType.YesNo);
        }

        private void cmlAddLink_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as FrameworkElement;
            m_menuContextMenuItem = (menuItem.DataContext as MenuItemModel).Clone() as MenuItemModel;

            addLinkPage = new UCAddLinkPage(tvMenu.ItemsSource as ObservableCollection<MenuItemModel>, m_menuCollection, m_menuContextMenuItem);
            addLinkPage.Dialog = this.Window.ShowDialog(MenuMaintainResource.ContextMenuItem_Header_AddPage, addLinkPage);

            addLinkPage.Dialog.Closed += new EventHandler(Dialog_Closed);
        }

        void Dialog_Closed(object sender, EventArgs e)
        {
            if (addLinkPage.Dialog != null && addLinkPage.Dialog.ResultArgs != null)
            {
                var data = addLinkPage.Dialog.ResultArgs.Data as MenuItemModel;
                if (data != null)
                {
                    var original = m_menuCollection.SingleOrDefault(p => p.MenuId.ToString() == data.LinkPath);

                    if (original != null)
                    {
                        data.DisplayName = original.DisplayName;
                        data.Description = original.Description;
                        data.LinkPath = original.LinkPath;
                        data.AuthKey = original.AuthKey;
                        data.Status = original.Status;
                        data.Parent = m_menuContextMenuItem;
                        data.ApplicationId = original.ApplicationId;
                        data.IsDisplay = original.IsDisplay;
                        data.LocalizedResCollection = original.LocalizedResCollection;

                        if (m_menuContextMenuItem != null)
                            m_menuContextMenuItem.Children.Add(data);
                    }
                }
            }
        }


        #endregion

        # region Tree View Events

        private void tvTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetEditorEnabled(true);
            if (!this.m_isEditTree && e.NewValue != null && e.NewValue is MenuItemModel)
            {
                MenuItemModel dataItem = e.NewValue as MenuItemModel;

                this.gridEditor.Tag = dataItem;
                this.gridEditor.DataContext = dataItem.Clone();

                this.btnSave.IsEnabled = true;
                this.btnCancel.IsEnabled = true;

                if (dataItem.Type == "C" && dataItem.ParentMenuId == null)
                {
                    ComboBoxApplidations.IsEnabled = true;
                }

                if (dataItem.Type == "L")
                {
                    SetEditorEnabled(false);
                }
            }
            else
            {
                this.gridEditor.Tag = null;
                this.gridEditor.DataContext = null;

                this.btnSave.IsEnabled = false;
                this.btnCancel.IsEnabled = false;
            }
        }

        private void MenuItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement menuItem = sender as FrameworkElement;

            if (menuItem != null && menuItem.DataContext is MenuItemModel)
            {
                MenuItemModel dataItem = menuItem.DataContext as MenuItemModel;
                tvTree.SelectItem(dataItem);
                ContextMenu contextMenu = ContextMenuService.GetContextMenu(menuItem);

                if (this.m_isEditTree)
                {
                    contextMenu.Visibility = Visibility.Collapsed;
                }
                else
                {
                    contextMenu.Visibility = Visibility.Visible;

                    MenuItem cmlAddCategory = contextMenu.FindName("cmlAddCategory") as MenuItem;
                    MenuItem cmlAddPage = contextMenu.FindName("cmlAddPage") as MenuItem;
                    MenuItem cmlAddLink = contextMenu.FindName("cmlAddLink") as MenuItem;


                    cmlAddCategory.IsEnabled = dataItem.Type == "C";
                    cmlAddPage.IsEnabled = dataItem.Type == "C";

                    cmlAddLink.IsEnabled = dataItem.Type == "C";
                }
            }
        }

        private void TreeViewDragDropTarget_Drop(object sender, Microsoft.Windows.DragEventArgs e)
        {
            m_canTreeViewItemDrop = true;

            TreeViewDragDropTarget2 dragTarget = sender as TreeViewDragDropTarget2;
            if (dragTarget != null)
            {
                ItemsControl itemsControl = dragTarget.GetRealDropTarget(e);
                MenuItemModel targetModel = itemsControl.DataContext as MenuItemModel;

                if (itemsControl is TreeView || (targetModel != null && (targetModel.Type == "P" || targetModel.Type == "L")))
                {
                    m_canTreeViewItemDrop = false;
                }

                ItemDragEventArgs args = e.Data.GetData(typeof(ItemDragEventArgs)) as ItemDragEventArgs;
                if (args != null && m_canTreeViewItemDrop)
                {
                    SelectionCollection selectionCollection = args.Data as SelectionCollection;
                    if (selectionCollection != null && selectionCollection.Count == 1)
                    {
                        MenuItemModel sourceModel = selectionCollection[0].Item as MenuItemModel;
                        if (sourceModel != null)
                        {
                            sourceModel.Parent = targetModel;
                        }
                    }
                }
            }

            if (!m_canTreeViewItemDrop)
            {
                e.Handled = true;
            }
        }

        private void TreeViewDragDropTarget_ItemDroppedOnTarget(object sender, ItemDragEventArgs e)
        {
            if (!m_canTreeViewItemDrop)
            {
                e.Cancel = true;
                e.Handled = true;
            }
        }

        private void WaterMarkTextBoxTreeViewSearch_KeyUp(object sender, KeyEventArgs e)
        {
            m_menuSearchCollection = m_menuCollection.Clone();

            if (((TextBox)sender).Text.Trim().Length != 0)
            {
                RootMenu.Visibility = System.Windows.Visibility.Collapsed;
                btnEditTree.IsEnabled = false;
                m_menuSearchCollection = new ObservableCollection<MenuItemModel>(
                    (
                        from p in m_menuSearchCollection
                        where (
                                    p.LocalizedDisplayName.ToLower().Contains(WaterMarkTextBoxTreeViewSearch.Text.Trim().ToLower())
                                    &&
                                    (p.Type == "P" || p.Type == "L")
                            )
                        select p
                     ).ToList()
                  );

                foreach (var item in m_menuSearchCollection)
                {

                    ObservableCollection<string> menuPathList = new ObservableCollection<string>();

                    CreateMenuPath(item.ParentMenuId.HasValue ? item.ParentMenuId.Value.ToString() : null, ref menuPathList);

                    string MenuPath = string.Empty;

                    for (int i = 0; i < menuPathList.Count; i++)
                    {
                        if (i == 0)
                        {
                            MenuPath = menuPathList[i].ToString();
                        }
                        else
                        {
                            MenuPath += "=>" + menuPathList[i].ToString();
                        }
                    }
                    item.MenuPath = MenuPath;
                }
                tvMenu.ItemsSource = m_menuSearchCollection;
            }
            else
            {
                RootMenu.Visibility = System.Windows.Visibility.Visible;
                btnEditTree.IsEnabled = true;
                tvMenu.ItemsSource = m_menuSearchCollection.GenerateMenuTree();
            }

            this.ResetEditor();
        }

        private void ICONActive_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_menuSearchCollection = m_menuCollection.Clone();

            if (WaterMarkTextBoxTreeViewSearch.Text.Trim().Length != 0)
            {
                RootMenu.Visibility = System.Windows.Visibility.Collapsed;
                btnEditTree.IsEnabled = false;
                m_menuSearchCollection = new ObservableCollection<MenuItemModel>((from p in m_menuSearchCollection where (p.DisplayName.ToLower().Contains(WaterMarkTextBoxTreeViewSearch.Text.Trim().ToLower()) && p.Type == "P") select p).ToList());
                tvMenu.ItemsSource = m_menuSearchCollection;
            }
            else
            {
                RootMenu.Visibility = System.Windows.Visibility.Visible;
                btnEditTree.IsEnabled = true;
                tvMenu.ItemsSource = m_menuSearchCollection.GenerateMenuTree();
            }

            this.ResetEditor();
        }


        #endregion

        #region Button Events

        private void btnImportMenu_Click(object sender, RoutedEventArgs e)
        {
            m_menuContoller.Import(null, (result) =>
            {
                var r = result.Clone();
                m_menuCollection = result;

                InitLinkMenuItem(r);

                this.tvMenu.ItemsSource = r.GenerateMenuTree();
                this.Window.MessageBox.Show(CommonResource.Info_ImportSuccessfully, MessageBoxType.Success);
            });
        }

        private void btnReloadTree_Click(object sender, RoutedEventArgs e)
        {
            WaterMarkTextBoxTreeViewSearch.Text = "";
            m_restServiceClient.Query<ObservableCollection<MenuItemModel>>("", (target, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                var result = args.Result.Clone();
                m_menuCollection = args.Result;

                InitLinkMenuItem(result);

                this.tvMenu.ItemsSource = result.GenerateMenuTree();


                this.Window.MessageBox.Show(CommonResource.Info_LoadDataSuccessfully, MessageBoxType.Success);
            });
            btnEditTree.IsEnabled = true;
            this.ResetEditor();
        }

        private void btnEditTree_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_isEditTree)
            {
                ToEditorMode();

                this.tvMenu.ItemsSource = this.m_menuTree;
            }
            else
            {
                ToTreeMode();

                ObservableCollection<MenuItemModel> dataSource = this.tvMenu.ItemsSource as ObservableCollection<MenuItemModel>;
                this.m_menuTree = dataSource.Clone();
            }
        }

        private void btnSaveTree_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<MenuItemModel> dataItems = this.tvMenu.ItemsSource as ObservableCollection<MenuItemModel>;
            if (dataItems != null)
            {
                ObservableCollection<MenuItemModel> fullDataList = new ObservableCollection<MenuItemModel>();
                ObservableCollection<MenuItemModel> updateDataList = new ObservableCollection<MenuItemModel>();

                GetTreeData(dataItems, fullDataList, updateDataList);

                this.m_restServiceClient.Update("", updateDataList, (target, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    m_restServiceClient.Query<ObservableCollection<MenuItemModel>>("", (o, a) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        var r = a.Result.Clone();
                        m_menuCollection = a.Result;

                        InitLinkMenuItem(r);

                        this.tvMenu.ItemsSource = r.GenerateMenuTree();

                        this.ToEditorMode();
                        this.Window.MessageBox.Show(CommonResource.Info_SaveSuccessfully, MessageBoxType.Success);
                    });


                });

                this.ResetEditor();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var dataItem = this.gridEditor.DataContext as MenuItemModel;

            if (dataItem != null)
            {
                if (!ValidationManager.Validate(this.gridEditor))
                {
                    return;
                }


                if (dataItem.MenuId == null)
                {
                    dataItem.InUser = CPApplication.Current.LoginUser.ID;

                    m_restServiceClient.Create<MenuItemModel>("", dataItem, (target, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        var originalItem = this.gridEditor.Tag as MenuItemModel;
                        var item = args.Result;

                        m_menuCollection.Add(item);

                        ObservableCollection<MenuItemModel> parentItems = (originalItem.Parent != null) ? (originalItem.Parent.Children) : (this.tvMenu.ItemsSource as ObservableCollection<MenuItemModel>);

                        originalItem.SetFields(item);
                        dataItem.SetFields(item);

                        if (parentItems != null)
                        {
                            originalItem.Children = new ObservableCollection<MenuItemModel>();

                            parentItems.Add(originalItem);
                        }

                        this.Window.MessageBox.Show(CommonResource.Info_CreateSuccessfully, MessageBoxType.Success);
                    });
                }
                else
                {
                    dataItem.EditUser = CPApplication.Current.LoginUser.ID;

                    m_restServiceClient.Update<MenuItemModel>(String.Format("/{0}", dataItem.MenuId), dataItem, (target, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        var item = args.Result;

                        if (item.Type == "P")
                        {
                            m_restServiceClient.Query<ObservableCollection<MenuItemModel>>("", (o, a) =>
                            {
                                if (args.FaultsHandle())
                                {
                                    return;
                                }

                                var r = a.Result.Clone();
                                m_menuCollection = a.Result;

                                InitLinkMenuItem(r);

                                this.tvMenu.ItemsSource = r.GenerateMenuTree();

                                this.Window.MessageBox.Show(CommonResource.Info_SaveSuccessfully, MessageBoxType.Success);
                            });
                        }
                        else
                        {
                            var originalItem = this.gridEditor.Tag as MenuItemModel;


                            originalItem.SetFields(item);
                            dataItem.SetFields(item);

                            UpdateNormalDataSource(item, "U");

                            this.Window.MessageBox.Show(CommonResource.Info_SaveSuccessfully, MessageBoxType.Success);
                        }
                    });
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MenuItemModel dataItem = this.gridEditor.Tag as MenuItemModel;

            if (dataItem != null)
            {
                this.gridEditor.DataContext = dataItem.Clone();
            }
        }


        #endregion

        #region Editor Events

        private void TextBoxName_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ImageTooltip.Source = new BitmapImage(new Uri(@"/Images/menuMaintain/help_name.png", UriKind.Relative));
            this.ImageTooltip.Visibility = Visibility.Visible;
        }

        private void TextBoxIconStyle_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ImageTooltip.Source = new BitmapImage(new Uri(@"/Images/menuMaintain/help_iconStyle.jpg", UriKind.Relative));
            this.ImageTooltip.Visibility = Visibility.Visible;
        }

        private void TextBoxLinkPath_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ImageTooltip.Source = new BitmapImage(new Uri(@"/Images/menuMaintain/help_linkPath.png", UriKind.Relative));
            this.ImageTooltip.Visibility = Visibility.Visible;
        }

        private void TextBoxAuthKey_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ImageTooltip.Source = new BitmapImage(new Uri(@"/Images/menuMaintain/help_authKey.jpg", UriKind.Relative));
            this.ImageTooltip.Visibility = Visibility.Visible;
        }

        private void TextBoxDescrption_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ImageTooltip.Source = new BitmapImage(new Uri(@"/Images/menuMaintain/Help_Descrption.jpg", UriKind.Relative));
            this.ImageTooltip.Visibility = Visibility.Visible;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ImageTooltip.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Private Methods

        private void SetEditorEnabled(bool status)
        {
            TextBoxDisplayName.IsReadOnly = !status;
            TextBoxDescription.IsReadOnly = !status;
            TextBoxLinkPath.IsReadOnly = !status;
            TextBoxAuthKey.IsReadOnly = !status;
            //ComboxStatus.IsEnabled = status;
            RbStatusActive.IsEnabled = status;
            RbStatusInactive.IsEnabled = status;
            ComboBoxApplidations.IsEnabled = false;
            CheckBoxIsDisplay.IsEnabled = status;

            btnSave.IsEnabled = status;
            btnCancel.IsEnabled = status;
            DisableLocalizedGridBorder.Visibility = status ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ResetEditor()
        {
            this.gridEditor.Tag = null;
            this.gridEditor.DataContext = null;

            this.btnSave.IsEnabled = false;
            this.btnCancel.IsEnabled = false;
        }

        private void ToTreeMode()
        {
            this.dtTree.AllowDrop = true;
            this.tvTree.AllowDrop = true;

            this.btnImportMenu.Visibility = Visibility.Collapsed;
            this.btnReloadTree.Visibility = Visibility.Collapsed;
            this.btnSaveTree.Visibility = Visibility.Visible;
            this.btnEditTree.Content = MenuMaintainResource.Button_CancelEditTree;
            this.ArrowTipSaveTree.Visibility = Visibility.Visible;

            this.ResetEditor();

            this.gridEditor.Visibility = Visibility.Collapsed;

            this.m_isEditTree = true;
            ScrollViewerTree.Content = dtTree;
            dtTree.Content = tvTree;
        }

        private void ToEditorMode()
        {
            this.dtTree.AllowDrop = false;
            this.tvTree.AllowDrop = false;

            this.btnImportMenu.Visibility = Visibility.Visible;
            this.btnReloadTree.Visibility = Visibility.Visible;
            this.btnSaveTree.Visibility = Visibility.Collapsed;
            this.btnEditTree.Content = MenuMaintainResource.Button_EditTree;
            this.ArrowTipSaveTree.Visibility = Visibility.Collapsed;
            this.gridEditor.Visibility = Visibility.Visible;

            this.m_isEditTree = false;

            dtTree.Content = null;
            ScrollViewerTree.Content = tvTree;
        }


        private void GetTreeData(ObservableCollection<MenuItemModel> list, ObservableCollection<MenuItemModel> fullDataList, ObservableCollection<MenuItemModel> updateDataList)
        {
            int index = list.Count;
            foreach (MenuItemModel item in list)
            {
                int? sortIndex = index--;
                Guid? parentId = null;

                if (item.Parent != null)
                {
                    parentId = item.Parent.MenuId;
                }

                DateTime? editDate = DateTime.Now;
                string editUser = CPApplication.Current.LoginUser.ID;

                fullDataList.Add(new MenuItemModel(item)
                {
                    SortIndex = sortIndex,
                    ParentMenuId = parentId,
                    EditDate = editDate,
                    EditUser = editUser
                });

                updateDataList.Add(new MenuItemModel()
                {
                    MenuId = item.MenuId,
                    SortIndex = sortIndex,
                    ParentMenuId = parentId,
                    EditDate = editDate,
                    EditUser = editUser
                });

                if (item.Children != null)
                {
                    GetTreeData(item.Children, fullDataList, updateDataList);
                }
            }
        }

        private ObservableCollection<MenuItemModel> GenerateCategoryTree(ObservableCollection<MenuItemModel> dataItems, MenuItemModel parentItem)
        {
            var matchedItems = new ObservableCollection<MenuItemModel>();

            if (dataItems != null)
            {
                // 1、Find matched items
                foreach (MenuItemModel item in dataItems)
                {
                    if ((parentItem == null && item.ParentMenuId == null) || (parentItem != null && item.ParentMenuId == parentItem.MenuId))
                    {
                        if (item.Type == "C")
                        {
                            matchedItems.Add(item);
                        }
                    }
                }
                // 2、Remove matched items from original collection
                foreach (MenuItemModel item in matchedItems)
                {
                    dataItems.Remove(item);
                }
                // 3、Find children for matched items
                foreach (MenuItemModel item in matchedItems)
                {
                    item.Parent = parentItem;
                    item.Children = GenerateCategoryTree(dataItems, item);
                }
            }

            return matchedItems;
        }

        private void UpdateNormalDataSource(MenuItemModel menuItem, string operationStr)
        {
            if (operationStr == "U")
            {

                for (int i = m_menuCollection.Count - 1; i >= 0; i--)
                {
                    if (m_menuCollection[i].MenuId == menuItem.MenuId)
                    {
                        m_menuCollection[i] = menuItem;

                        return;
                    }
                }
            }

            if (operationStr == "D")
            {
                for (int i = m_menuCollection.Count - 1; i >= 0; i--)
                {
                    if (m_menuCollection[i].MenuId == menuItem.MenuId)
                    {
                        m_menuCollection.RemoveAt(i);
                        return;
                    }
                }
            }

        }

        private void InitLinkMenuItem(ObservableCollection<MenuItemModel> result)
        {
            foreach (var item in result)
            {
                if (item.Type == "L")
                {
                    var original = result.ToList().SingleOrDefault(p => p.MenuId.ToString() == item.LinkPath);

                    if (original != null)
                    {
                        item.DisplayName = original.DisplayName;
                        item.Description = original.Description;
                        item.LinkPath = original.LinkPath;
                        item.AuthKey = original.AuthKey;
                        item.Status = original.Status;
                        item.ApplicationId = original.ApplicationId;
                        item.IsDisplay = original.IsDisplay;
                        item.LocalizedResCollection = original.LocalizedResCollection;
                    }
                }
            }
        }

        private void CreateMenuPath(string ParentMenuId, ref ObservableCollection<string> MenuPathList)
        {
            foreach (var item in m_menuCollection)
            {
                if (item.MenuId.ToString() == ParentMenuId)
                {
                    MenuPathList.Insert(0, item.DisplayName);

                    if (item.ParentMenuId == null || item.ParentMenuId == item.MenuId)
                    {
                        return;
                    }

                    CreateMenuPath(item.ParentMenuId.ToString(), ref MenuPathList);
                }
            }



        }

        #endregion
    }
}
