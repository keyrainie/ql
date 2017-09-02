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
using System.Windows.Markup;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Core.Components;
using ControlPanel.SilverlightUI;
using Newegg.Oversea.Silverlight.Controls.Containers;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Menu
{
    public partial class MenuControl : UserControl
    {
        public EventHandler<NavigateArgs> Navigating { get; set; }
        Grid s_gridCoverLayer = new Grid { Background = new SolidColorBrush(Colors.Transparent) };
        bool m_isLoaded = false;

        public MenuControl()
        {
            InitializeComponent();
            s_gridCoverLayer.SetValue(Canvas.ZIndexProperty, 999999);
            StackPanelMenuContainer1.LayoutUpdated += new EventHandler(StackPanelMenuContainer1_LayoutUpdated);
            PopupMenuArea.Opened += new EventHandler(PopupMenuArea_Opened);
            PopupMenuArea.Closed += new EventHandler(PopupMenuArea_Closed);
            s_gridCoverLayer.MouseLeftButtonUp += new MouseButtonEventHandler(s_gridCoverLayer_MouseLeftButtonUp);
            Loaded += new RoutedEventHandler(MenuControl_Loaded);
            InitMenuTab();
        }

        private List<ItemData> m_menuCollection = new List<ItemData>();
        public List<ItemData> MenuCollection
        {
            get
            {
                return m_menuCollection;
            }
            set
            {
                m_menuCollection = value;
                BuildAllMenu(value);
            }
        }

        private List<ItemData> m_favoriteCollection = new List<ItemData>();
        public List<ItemData> FavoriteCollection
        {
            get
            {
                return m_favoriteCollection;
            }
            set
            {
                m_favoriteCollection = value;
                BuildFavoriteMenu(value);
            }
        }

        List<ItemData> m_menuFlatData = new List<ItemData>();
        public List<ItemData> MenuFlatData
        {
            get
            {
                return m_menuFlatData;
            }
            set
            {
                m_menuFlatData = value;
            }
        }

        internal SubMenuItem LastSubMenuItemInAll
        {
            get;
            set;
        }

        internal TextBlock LastSubMenuItemInFvt
        {
            get;
            set;
        }



        public bool IsOpen
        {
            get
            {
                return PopupMenuArea.IsOpen;
            }
            set
            {
                PopupMenuArea.IsOpen = value;
            }
        }

        #region Menu

        void BuildAllMenu(List<ItemData> data)
        {
            //Build Normal Menu UI
            List<ItemData> menuItems = data;
            if (menuItems == null)
                return;

            foreach (var item1 in menuItems)
            {
                var menuItem1 = new MenuItem { Text = item1.Name, DataContext = item1, Menu = this };
                StackPanel stackPanelItem1 = new StackPanel();
                menuItem1.MouseLeftButtonUp += new MouseButtonEventHandler(menuItem1_MouseLeftButtonUp);

                if (item1.SubItems != null)
                {
                    foreach (var item2 in item1.SubItems)
                    {
                        StackPanel stackPanelItem2 = new StackPanel();
                        var menuItem2 = new MenuItem { Text = item2.Name, IsLevel2 = true, DataContext = item2, ParentNode = menuItem1, Menu = this };
                        menuItem2.MouseLeftButtonUp += new MouseButtonEventHandler(menuItem2_MouseLeftButtonUp);
                        stackPanelItem1.Children.Add(menuItem2);
                        if (item2.SubItems != null)
                        {
                            foreach (var item3 in item2.SubItems)
                            {
                                var menuItem3 = new SubMenuItemPanel { Menu = this, DataContext = item3, Tag = UtilityHelper.DeepClone(item1), ParentNode = menuItem2 };
                                menuItem3.ItemBinding = item3;
                                stackPanelItem2.Children.Add(menuItem3);
                            }
                        }
                        menuItem2.Tag = stackPanelItem2;
                    }
                    menuItem1.Tag = stackPanelItem1;
                    StackPanelMenuContainer1.Children.Add(menuItem1);
                }
            }

            ItemData lastMenuItemData = ComponentFactory.GetComponent<IUserProfile>().Get<ItemData>(UserProfileKey.Key_AllStatus);
           
            if (lastMenuItemData == null)
            {
                //Initial Normal Menu UI
                if (StackPanelMenuContainer1.Children.Count > 0)
                {
                    var menuItem1 = StackPanelMenuContainer1.Children[0] as MenuItem;
                    BuildSubLevel2(menuItem1);
                    if (menuItem1 != null
                        && (StackPanel)((MenuItem)menuItem1).Tag != null
                        && ((StackPanel)((MenuItem)menuItem1).Tag).Children.Count > 0)
                    {
                        var menuItem2 = ((StackPanel)((MenuItem)menuItem1).Tag).Children[0] as MenuItem;
                        BuildSubLevel3(menuItem2);
                    }
                }
            }
            else if (this.LastSubMenuItemInAll != null)
            {
                if (this.LastSubMenuItemInAll.ParentNode is SubMenuItem)
                {
                    InitMenuSelected((MenuItem)((MenuItem)((SubMenuItem)this.LastSubMenuItemInAll.ParentNode).ParentNode).ParentNode, (MenuItem)((SubMenuItem)this.LastSubMenuItemInAll.ParentNode).ParentNode);
                }
                else
                {
                    InitMenuSelected((MenuItem)((MenuItem)this.LastSubMenuItemInAll.ParentNode).ParentNode, (MenuItem)(this.LastSubMenuItemInAll).ParentNode);
                }
            }
        }

        private void InitMenuSelected(MenuItem menuItem1,MenuItem menuItem2)
        {
            ResetBackground((StackPanel)menuItem1.Parent);
            ResetBackground((StackPanel)menuItem2.Parent);
            BuildSubLevel2(menuItem1);
            if (menuItem1 != null
                && (StackPanel)((MenuItem)menuItem1).Tag != null
                && ((StackPanel)((MenuItem)menuItem1).Tag).Children.Count > 0)
            {
                BuildSubLevel3(menuItem2);
            }
        }

        void BuildSubLevel3(MenuItem menuItem3)
        {
            menuItem3.IsSelected = true;
            ScrollViewerMenuContainer3.Content = menuItem3.Tag;
        }

        void BuildSubLevel2(MenuItem menuItem2)
        {
            menuItem2.IsSelected = true;
            ScrollViewerMenuContainer3.Content = null;
            var stackPanel = (StackPanel)menuItem2.Tag;
            ScrollViewerMenuContainer2.Content = (StackPanel)menuItem2.Tag;
            foreach (var item in stackPanel.Children)
            {
                if (((MenuItem)item).IsSelected)
                {
                    BuildSubLevel3((MenuItem)item);
                    break;
                }
            }
        }

        void menuItem2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResetBackground((StackPanel)((MenuItem)sender).Parent);
            BuildSubLevel3(((MenuItem)sender));
            if (Navigating != null)
            {
                Navigating((MenuItem)sender, new NavigateArgs { Data = (ItemData)((MenuItem)sender).DataContext });
            }
        }

        void menuItem1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResetBackground((StackPanel)((MenuItem)sender).Parent);
            ((MenuItem)sender).IsSelected = true;
            if (Navigating != null)
            {
                Navigating((MenuItem)sender, new NavigateArgs { Data = (ItemData)((MenuItem)sender).DataContext });
            }
            BuildSubLevel2(((MenuItem)sender));
        }

        void ResetBackground(StackPanel stackPanel)
        {
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                ((MenuItem)stackPanel.Children[i]).IsSelected = false;
            }
        }

        #endregion

        #region Favorites

        private void BuildFavoriteMenu(List<ItemData> data)
        {
            List<ItemData> favorites = data;
            StackPanelContent2.Children.Clear();
            foreach (var item1 in favorites)
            {
                StackPanelContent2.Children.Add(GenerateLevel1Favorite(item1));
            }
        }
        Grid m_currentFavoriteContainer = null;

        Grid GenerateLevel1Favorite(ItemData favorite)
        {
            var gridContainerOutter = (Grid)XamlReader.Load("<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Rectangle Fill=\"White\" RadiusX=\"2\" RadiusY=\"2\" Stroke=\"#FFBBCCFF\" Margin=\"0\" Visibility=\"Collapsed\"/><Rectangle Fill=\"White\" RadiusX=\"2\" RadiusY=\"2\" Stroke=\"#FFE6ECFF\" Margin=\"1\" Visibility=\"Collapsed\"/><Rectangle Fill=\"White\" RadiusX=\"2\" RadiusY=\"2\" Stroke=\"#FFF2F5FF\" Margin=\"2\" Visibility=\"Collapsed\"/></Grid>");
            gridContainerOutter.MouseEnter += (obj, args) => 
            {
                gridContainerOutter.Children[0].Visibility = System.Windows.Visibility.Visible;
                gridContainerOutter.Children[1].Visibility = System.Windows.Visibility.Visible;
                gridContainerOutter.Children[2].Visibility = System.Windows.Visibility.Visible;
                m_currentFavoriteContainer = (Grid)obj;
            };
            gridContainerOutter.MouseLeave += (obj, args) =>
            {
                gridContainerOutter.Children[0].Visibility = System.Windows.Visibility.Collapsed;
                gridContainerOutter.Children[1].Visibility = System.Windows.Visibility.Collapsed;
                gridContainerOutter.Children[2].Visibility = System.Windows.Visibility.Collapsed;
            };
            var stackPanelContainer = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

            var stackPanelLeftArea = new StackPanel { Width = 120, Orientation = Orientation.Horizontal };
            stackPanelLeftArea.SetValue(ToolTipService.ToolTipProperty, new ToolTip()
                {
                    Content = new TextBlock()
                    {
                        Text = favorite.Name
                    }
                });
            stackPanelLeftArea.Children.Add(new TextBlock
            {
                Text = favorite.Name,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 13,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0),
                Width = 100,
                TextTrimming = System.Windows.TextTrimming.WordEllipsis,
                Foreground = new SolidColorBrush(Colors.Black)
            });
            Path arrowPath = (Path)XamlReader.Load("<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"M0,0 L1.9999996,0 L1.9999996,1 L2.9999998,1 L2.9999998,2 L3.9999998,2 L3.9999998,3 L4.9999995,3 L4.9999995,4 L5.9999995,4 L5.9999995,5 L4.9999995,5 L4.9999995,6 L3.9999998,6 L3.9999998,7 L2.9999998,7 L2.9999998,8 L1.9999996,8 L1.9999996,9 L0,9 L0,8 L0.99999964,8 L0.99999964,7 L1.9999996,7 L1.9999996,6 L2.9999998,6 L2.9999998,5 L3.9999998,5 L3.9999998,4 L2.9999998,4 L2.9999998,3 L1.9999996,3 L1.9999996,2 L0.99999964,2 L0.99999964,1 L0,1 z\" Fill=\"#FF222222\" Margin=\"0\" Stretch=\"Fill\" StrokeThickness=\"1\" Width=\"6\" Height=\"9\" HorizontalAlignment=\"Right\"/>");
            arrowPath.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            stackPanelLeftArea.Children.Add(arrowPath);
            stackPanelContainer.Children.Add(stackPanelLeftArea);

            var listBoxSubItems = new ListBox
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(0),
                Margin = new Thickness(5, 0, 0, 0)
            };
            listBoxSubItems.ItemsSource = favorite.SubItems;
            stackPanelContainer.Children.Add(listBoxSubItems);

            listBoxSubItems.SetValue(ListBox.ItemTemplateProperty, ScrollViewerContent2.Resources["DataTemplateFavorite"]);
            listBoxSubItems.SetValue(ListBox.ItemsPanelProperty, this.Resources["ItemPanelTemplateFavorite"]);
            listBoxSubItems.SetValue(ListBox.ItemContainerStyleProperty, this.Resources["ListBoxItemFavoriteStyle"]);

            gridContainerOutter.Children.Add(stackPanelContainer);
            return gridContainerOutter;
        }

        private void TextBlockButtonFavorite_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Navigating != null)
            {
                Navigating(sender, new NavigateArgs { Data = (ItemData)((FrameworkElement)sender).DataContext });
            }
            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_TabStatus, false);
            
            if (LastSubMenuItemInFvt != null)
            {
                LastSubMenuItemInFvt.Tag = false;
                LastSubMenuItemInFvt.TextDecorations = null;
                LastSubMenuItemInFvt.Foreground = new SolidColorBrush(Colors.Black);
            }
            var currentObj = (TextBlock)sender;
            currentObj.Tag = true;
            currentObj.TextDecorations = TextDecorations.Underline;
            currentObj.Foreground = new SolidColorBrush(Color.FromArgb(255, 39, 83, 156));
            LastSubMenuItemInFvt = currentObj;

            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_FavoriteStatus, currentObj.DataContext);
            
            if (m_currentFavoriteContainer != null)
            {
                m_currentFavoriteContainer.Children[0].Visibility = System.Windows.Visibility.Collapsed;
                m_currentFavoriteContainer.Children[1].Visibility = System.Windows.Visibility.Collapsed;
                m_currentFavoriteContainer.Children[2].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void TextBlockButtonFavorite_MouseEnter(object sender, MouseEventArgs e)
        {
            var currentObj = (TextBlock)sender;
            currentObj.TextDecorations = TextDecorations.Underline;
            currentObj.Foreground = new SolidColorBrush(Color.FromArgb(255, 39, 83, 156));
        }

        private void TextBlockButtonFavorite_MouseLeave(object sender, MouseEventArgs e)
        {
            var currentObj = (TextBlock)sender;
            if (!(currentObj.Tag is bool) || !((bool)currentObj.Tag))
            {
                currentObj.TextDecorations = null;
                currentObj.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void TextBlockButtonFavorite_Loaded(object sender, RoutedEventArgs e)
        {
            var currentObj = (TextBlock)sender;
            if (currentObj.Tag == null)
            {
                currentObj.Tag = false;
            }
            else
            {
                return;
            }

            ItemData lastFvtItemData = ComponentFactory.GetComponent<IUserProfile>().Get<ItemData>(UserProfileKey.Key_FavoriteStatus);
           
            if (lastFvtItemData != null && ((ItemData)currentObj.DataContext).Id == lastFvtItemData.Id)
            {
                currentObj.TextDecorations = TextDecorations.Underline;
                currentObj.Foreground = new SolidColorBrush(Color.FromArgb(255, 39, 83, 156));
                currentObj.Tag = true;
                LastSubMenuItemInFvt = currentObj;
            }
        }

        #endregion

        #region Most Used

        List<ItemData> m_mostUsedData = new List<ItemData>();
        public List<ItemData> MostUsedData
        {
            get
            {
                return m_mostUsedData;
            }
            set
            {
                m_mostUsedData = value;
                BuildMostUsed(value);
            }
        }

        private void BuildMostUsed(List<ItemData> value)
        {
            StackPanelMostUsed.Children.Clear();
            foreach (var item in value)
            {
                var hyperlinkButton = new HyperlinkButton
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = item.Name,
                    Style = (Style)this.Resources["HyperlinkButtonMenuItemStyle"],
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 0, 7),
                    DataContext = item
                };
                hyperlinkButton.Click += (obj, args) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(hyperlinkButton, new NavigateArgs { Data = (ItemData)hyperlinkButton.DataContext });
                    }
                };
                StackPanelMostUsed.Children.Add(hyperlinkButton);
            }
        }

        #endregion

        #region Quick Search Area

        void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (((TextBox)sender).Text.Trim().Length != 0)
            {
                GridContent1.Visibility = Visibility.Collapsed;
                ScrollViewerContent2.Visibility = Visibility.Collapsed;
                ScrollViewerContent3.Visibility = Visibility.Visible;
                var queryResult = QueryMenuItem(((TextBox)sender).Text.Trim());
                BuildQueryResult(queryResult);
            }
            else
            {
                InitMenuTab();
            }
        }

        void BuildQueryResult(List<ItemData> queryResult)
        {
            StackPanelContent3.Children.Clear();
            foreach (var item in queryResult)
            {
                var menuItem3 = new SubMenuItem { Text = item.Name,DataContext = item };
                if (item.IsFavorited)
                {
                    menuItem3.Type = SubMenuItemIconType.Heart;
                }
                else
                {
                    menuItem3.Type = SubMenuItemIconType.HeartEmpty;
                }

                menuItem3.ClickICON = (obj, args) =>
                {
                    ((ItemData)menuItem3.DataContext).IsFavorited  = !((ItemData)menuItem3.DataContext).IsFavorited;
                    if (((ItemData)menuItem3.DataContext).IsFavorited)
                    {
                        menuItem3.Type = SubMenuItemIconType.Heart;
                    }
                    else
                    {
                        menuItem3.Type = SubMenuItemIconType.HeartEmpty;
                    }
                    NotifyMenuArea((ItemData)menuItem3.DataContext);
                };
                menuItem3.ClickItem = (obj, args) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(this, new NavigateArgs { Data = (ItemData)menuItem3.DataContext });
                    }
                };
                StackPanelContent3.Children.Add(menuItem3);
            }
        }

        void NotifyMenuArea(ItemData value)
        {
            foreach (var item in StackPanelMenuContainer1.Children)
            {
                var menuItem= item as MenuItem;
                var stackPanel = menuItem.Tag as StackPanel;
                if (stackPanel != null)
                {
                    foreach (var item2 in stackPanel.Children)
                    {
                        var menuItem2 = item2 as MenuItem;
                        var stackPanel2 = menuItem2.Tag as StackPanel;
                        if (stackPanel2 != null)
                        {
                            foreach (var item3 in stackPanel2.Children)
                            {
                                var menuItem3 = item3 as SubMenuItemPanel;
                                var subItem = (SubMenuItem)((Grid)menuItem3.Content).Children[0];
                                if (subItem.Type == SubMenuItemIconType.Heart
                                    || subItem.Type == SubMenuItemIconType.HeartEmpty)
                                {
                                    if (SetFavoriteIcon(value, subItem))
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    var stackPanel3 = (StackPanel)((Grid)menuItem3.Content).Children[1];
                                    foreach (var item4 in stackPanel3.Children)
                                    {
                                        
                                        var subSubItem = (SubMenuItem)item4;
                                        if (SetFavoriteIcon(value, subSubItem))
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool SetFavoriteIcon(ItemData value, SubMenuItem subItem)
        {
            if (value == subItem.DataContext)
            {
                if (value.IsFavorited)
                {
                    subItem.Type = SubMenuItemIconType.Heart;
                }
                else
                {
                    subItem.Type = SubMenuItemIconType.HeartEmpty;
                }
                subItem.NotifyDataSource();
                return true;
            }
            return false;
        }

        List<ItemData> QueryMenuItem(string keyWorks)
        {
            List<ItemData> queryResult = new List<ItemData>();
            foreach (var item in m_menuFlatData)
            {
                if (item.Name.ToLower().Contains(keyWorks.ToLower()))
                {
                    queryResult.Add(item);
                }
            }
            return queryResult;
        }

        void GridAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StackPanelAllButton.Visibility = Visibility.Visible;
            StackPanelFavoritesButton.Visibility = Visibility.Collapsed;
            GridContent1.Visibility = Visibility.Visible;
            ScrollViewerContent2.Visibility = Visibility.Collapsed;
            ScrollViewerContent3.Visibility = Visibility.Collapsed;
            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_TabStatus, true);
        }

        private void GridFavorites_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StackPanelAllButton.Visibility = Visibility.Collapsed;
            StackPanelFavoritesButton.Visibility = Visibility.Visible;
            GridContent1.Visibility = Visibility.Collapsed;
            ScrollViewerContent2.Visibility = Visibility.Visible;
            ScrollViewerContent3.Visibility = Visibility.Collapsed;
            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_TabStatus, false);
            
        }

        private void ICONActive_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBoxSearch_KeyUp(TextBoxSearch, null);
        }

        void InitMenuTab()
        {
            bool? isAllTab = ComponentFactory.GetComponent<IUserProfile>().Get<bool?>(UserProfileKey.Key_TabStatus);
            
            if (isAllTab == null || isAllTab.Value)
            {
                GridAll_MouseLeftButtonUp(null, null);
            }
            else
            {
                GridFavorites_MouseLeftButtonUp(null, null);
            }
        }

        #endregion

        #region Other


        void MenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = ((Panel)((UserControl)Application.Current.RootVisual).Content);
            panel.KeyDown += new KeyEventHandler(rootPanel_KeyDown);

            if (!m_isLoaded)
            {
                Panel parentPanel = (Panel)StackPanelMenuContainer1.Parent;
                parentPanel.Children.Remove(StackPanelMenuContainer1);
                ((Panel)((UserControl)App.Current.RootVisual).Content).Children.Add(StackPanelMenuContainer1);
                StackPanelMenuContainer1.Opacity = 0;
                StackPanelMenuContainer1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                StackPanelMenuContainer1_LayoutUpdated(null, null);
                this.Dispatcher.BeginInvoke(() =>
                {
                    ((Panel)((UserControl)App.Current.RootVisual).Content).Children.Remove(StackPanelMenuContainer1);
                    parentPanel.Children.Add(StackPanelMenuContainer1);
                    StackPanelMenuContainer1.Opacity = 1;
                });
                m_isLoaded = true;
            }
        }

        void rootPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (LayoutMask.RootVisualContainerCount <= 0)
            {
                if (e.Key == Key.Escape)
                {
                    PopupMenuArea.IsOpen = false;
                }
                ModifierKeys keys = Keyboard.Modifiers;
                if ((e.Key == Key.Z) && keys == (ModifierKeys.Control | ModifierKeys.Alt))
                {
                    PopupMenuArea.IsOpen = !PopupMenuArea.IsOpen;
                }
            }
        }

        void s_gridCoverLayer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopupMenuArea.IsOpen = false;
        }

        void ButtonLogo1_Click(object sender, RoutedEventArgs e)
        {
            PopupMenuArea.IsOpen = true;
        }

        void ButtonLogo2_Click(object sender, RoutedEventArgs e)
        {
            PopupMenuArea.IsOpen = false;
        }

        void PopupMenuArea_Closed(object sender, EventArgs e)
        {
            var panel = ((Panel)((UserControl)Application.Current.RootVisual).Content);
            if (panel.Children.Contains(s_gridCoverLayer))
            {
                panel.Children.Remove(s_gridCoverLayer);
            }
        }

        void PopupMenuArea_Opened(object sender, EventArgs e)
        {
            var panel = ((Panel)((UserControl)Application.Current.RootVisual).Content);
            if (!panel.Children.Contains(s_gridCoverLayer))
            {
                panel.Children.Add(s_gridCoverLayer);
            }

            //显示最后打开的页面项
            if (this.LastSubMenuItemInAll != null)
            {
                this.LastSubMenuItemInAll.SetSelected(true);
                if (this.LastSubMenuItemInAll.ParentNode is SubMenuItem)
                {
                    InitMenuSelected((MenuItem)((MenuItem)((SubMenuItem)this.LastSubMenuItemInAll.ParentNode).ParentNode).ParentNode, (MenuItem)((SubMenuItem)this.LastSubMenuItemInAll.ParentNode).ParentNode);
                }
                else
                {
                    if (((MenuItem)this.LastSubMenuItemInAll.ParentNode).Tag != null)
                    {
                        var stackPanel = ((MenuItem)this.LastSubMenuItemInAll.ParentNode).Tag as StackPanel;
                        foreach (var child in stackPanel.Children)
                        {
                            var uc = child as UserControl;
                            if (((SubMenuItem)((Grid)uc.Content).Children[0]).Type == SubMenuItemIconType.FolderOpen)
                            {
                                ((SubMenuItem)((Grid)uc.Content).Children[0]).Type = SubMenuItemIconType.Folder;
                            }
                            ((FrameworkElement)((Grid)uc.Content).Children[1]).Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    InitMenuSelected((MenuItem)((MenuItem)this.LastSubMenuItemInAll.ParentNode).ParentNode, (MenuItem)(this.LastSubMenuItemInAll).ParentNode);
                }
                InitMenuTab();
            }
        }

        void StackPanelMenuContainer1_LayoutUpdated(object sender, EventArgs e)
        {
            if (GridMenuArea.Tag == null || (double)GridMenuArea.Tag < StackPanelMenuContainer1.ActualHeight)
            {
                GridMenuArea.Height = StackPanelMenuContainer1.ActualHeight + 140;
                GridMenuArea.Tag = GridMenuArea.Height;
            }
        }

        private void StackPanelFavoritesButton_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderFavoriteMouseOver.Visibility = System.Windows.Visibility.Visible;
        }

        private void StackPanelFavoritesButton_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderFavoriteMouseOver.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void StackPanelAllButton_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderAllMouseOver.Visibility = System.Windows.Visibility.Visible;
        }

        private void StackPanelAllButton_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderAllMouseOver.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void GridSwitchButtonArea_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderAllMouseOver.Visibility = System.Windows.Visibility.Collapsed;
            BorderFavoriteMouseOver.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion
    }

    public class NavigateArgs : EventArgs
    {
        public ItemData Data { get; set; }
    }

}
