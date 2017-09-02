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
using Newegg.Oversea.Silverlight.Core.Components;
using ControlPanel.SilverlightUI;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Menu
{
    public partial class SubMenuItemPanel : UserControl
    {
        public SubMenuItemPanel()
        {
            InitializeComponent();
        }

        private ItemData m_itemBinding;
        public ItemData ItemBinding
        {
            get
            {
                return m_itemBinding;
            }
            set
            {
                m_itemBinding = value;
                Bind(value);
            }
        }

        public object ParentNode
        {
            get;
            set;
        }

        public MenuControl Menu { get; set; }

        private static ItemData s_lastMenuItemData = ComponentFactory.GetComponent<IUserProfile>().Get<ItemData>(UserProfileKey.Key_AllStatus);


        private void Bind(ItemData value)
        {
            SubMenuItemTitle.Text = value.Name;
            SubMenuItemTitle.DataContext = value;
            SubMenuItemTitle.Menu = Menu;
            SubMenuItemTitle.SubPanel = this;
            SubMenuItemTitle.Tag = StackPanelContent;
            SubMenuItemTitle.ParentNode = ParentNode;
            if (value.SubItems != null && value.SubItems.Count > 0)
            {
                SubMenuItemTitle.Type = SubMenuItemIconType.Folder;
                SubMenuItemTitle.ParentNode = ParentNode;
                foreach (var item in value.SubItems)
                {
                    var subMenuItem = new SubMenuItem { Text = item.Name, DataContext = item, Menu = Menu, SubPanel = this, ParentNode = SubMenuItemTitle };
                    subMenuItem.ClickICON = (obj, args) =>
                    {
                        
                        var itemInner = subMenuItem.DataContext as ItemData;
                        itemInner.IsFavorited = !itemInner.IsFavorited;
                        if (itemInner.IsFavorited)
                        {
                            subMenuItem.Type = SubMenuItemIconType.Heart;
                        }
                        else
                        {
                            subMenuItem.Type = SubMenuItemIconType.HeartEmpty;
                        }
                        NotifyQuickSearchArea(itemInner);
                    };
                    subMenuItem.ClickItem = (obj, args) =>
                    {
                        if (Menu.Navigating != null)
                        {
                            Menu.Navigating(this, new NavigateArgs { Data = (ItemData)subMenuItem.DataContext });
                        }
                        ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_TabStatus, true);
                        
                    };
                    StackPanelContent.Children.Add(subMenuItem);
                    if (item.IsFavorited)
                    {
                        subMenuItem.Type = SubMenuItemIconType.Heart;
                    }
                    if (s_lastMenuItemData != null && s_lastMenuItemData.Id == item.Id)
                    {
                        Menu.LastSubMenuItemInAll = subMenuItem;
                        subMenuItem.SetSelected(true);
                    }
                }
                SubMenuItemTitle.ClickICON = (obj, args) =>
                {
                    if (StackPanelContent.Visibility == Visibility.Visible)
                    {
                        StackPanelContent.Visibility = System.Windows.Visibility.Collapsed;
                        SubMenuItemTitle.Type = SubMenuItemIconType.Folder;
                    }
                    else
                    {
                        SubMenuItemTitle.Type = SubMenuItemIconType.FolderOpen;
                        StackPanelContent.Visibility = System.Windows.Visibility.Visible;
                    }
                };
                SubMenuItemTitle.ClickItem = (obj, args) =>
                {
                    if (StackPanelContent.Visibility == Visibility.Visible)
                    {
                        StackPanelContent.Visibility = System.Windows.Visibility.Collapsed;
                        SubMenuItemTitle.Type = SubMenuItemIconType.Folder;
                    }
                    else
                    {
                        StackPanelContent.Visibility = System.Windows.Visibility.Visible;
                        SubMenuItemTitle.Type = SubMenuItemIconType.FolderOpen;
                    }
                };
            }
            else
            {
                SubMenuItemTitle.ClickICON = (obj, args) =>
                {
                    m_itemBinding.IsFavorited = !m_itemBinding.IsFavorited;
                    if (m_itemBinding.IsFavorited)
                    {
                        SubMenuItemTitle.Type = SubMenuItemIconType.Heart;
                    }
                    else
                    {
                        SubMenuItemTitle.Type = SubMenuItemIconType.HeartEmpty;
                    }
                    NotifyQuickSearchArea(m_itemBinding);
                };
                SubMenuItemTitle.ClickItem = (obj, args) =>
                {
                    if (Menu.Navigating != null)
                    {
                        Menu.Navigating(this, new NavigateArgs { Data = m_itemBinding });
                    }
                    ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_TabStatus, true);
                    

                };
                if (m_itemBinding.IsFavorited)
                {
                    SubMenuItemTitle.Type = SubMenuItemIconType.Heart;
                }
            }
            if (s_lastMenuItemData != null && s_lastMenuItemData.Id == value.Id)
            {
                Menu.LastSubMenuItemInAll = SubMenuItemTitle;
                SubMenuItemTitle.SetSelected(true);
            }
        }

        void NotifyQuickSearchArea(ItemData value)
        {
            foreach (var item in Menu.StackPanelContent3.Children)
            {
                var subItem = item as SubMenuItem;
                if (value.IsFavorited)
                {
                    subItem.Type = SubMenuItemIconType.Heart;
                }
                else
                {
                    subItem.Type = SubMenuItemIconType.HeartEmpty;
                }
            }
        }

    }
}
