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
    public enum SubMenuItemIconType
    {
        Heart,
        HeartEmpty,
        Folder,
        FolderOpen
    }

    public partial class SubMenuItem : UserControl
    {
        public SubMenuItem()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SubMenuItem_Loaded);
        }

        private bool m_isSelected;

        void SubMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetIconImage(m_type);
        }

        public MenuControl Menu
        {
            get;
            set;
        }

        public object ParentNode
        {
            get;
            set;
        }

        public SubMenuItemPanel SubPanel
        {
            get;
            set;
        }

        public bool m_isLevel4;
        public bool IsLevel4
        {
            get
            {
                return m_isLevel4;
            }
            set
            {
                m_isLevel4 = value;
            }
        }

        public EventHandler ClickICON;
        public EventHandler ClickItem;

        public string Text
        {
            get
            {
                return (string)TextBlockItem.Text;
            }
            set
            {
                TextBlockItem.Text = value;
            }
        }

        SubMenuItemIconType m_type = SubMenuItemIconType.HeartEmpty;
        public SubMenuItemIconType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
                SetIconImage(value);
            }
        }

        void SetIconImage(SubMenuItemIconType type)
        {
            if (ImageHeart != null)
            {
                switch (type)
                {
                    case SubMenuItemIconType.Folder:
                        {
                            ImageHeart.Visibility = System.Windows.Visibility.Collapsed;
                            ImageHeartEmpty.Visibility = System.Windows.Visibility.Collapsed;
                            ImageFolder.Visibility = System.Windows.Visibility.Visible;
                            ImageFolderOpen.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        break;
                    case SubMenuItemIconType.FolderOpen:
                        {
                            ImageHeart.Visibility = System.Windows.Visibility.Collapsed;
                            ImageHeartEmpty.Visibility = System.Windows.Visibility.Collapsed;
                            ImageFolder.Visibility = System.Windows.Visibility.Collapsed;
                            ImageFolderOpen.Visibility = System.Windows.Visibility.Visible;
                        }
                        break;
                    case SubMenuItemIconType.Heart:
                        {
                            ImageHeart.Visibility = System.Windows.Visibility.Visible;
                            ImageHeartEmpty.Visibility = System.Windows.Visibility.Collapsed;
                            ImageFolder.Visibility = System.Windows.Visibility.Collapsed;
                            ImageFolderOpen.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        break;
                    case SubMenuItemIconType.HeartEmpty:
                        {
                            ImageHeart.Visibility = System.Windows.Visibility.Collapsed;
                            ImageHeartEmpty.Visibility = System.Windows.Visibility.Visible;
                            ImageFolder.Visibility = System.Windows.Visibility.Collapsed;
                            ImageFolderOpen.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        break;
                }
            }
        }

        public void NotifyDataSource()
        {
            if (SubPanel != null && (this.Type == SubMenuItemIconType.Heart || this.Type == SubMenuItemIconType.HeartEmpty))
            {
                if (((ItemData)this.DataContext).IsFavorited)
                {
                    AddFavorite((ItemData)SubPanel.Tag, (ItemData)this.DataContext);
                }
                else
                {
                    RemoveFavorite((ItemData)this.DataContext);
                }
            }
        }

        void AddFavorite(ItemData level1Data, ItemData leafData)
        {
            foreach (var item in Menu.FavoriteCollection)
            {
                if (level1Data.Id == item.Id)
                {
                    item.SubItems.Add(leafData);
                    Menu.FavoriteCollection = Menu.FavoriteCollection;
                    ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_Favorite,Menu.FavoriteCollection,true);
                    
                    return;
                }
            }
            level1Data.SubItems = new List<ItemData>();
            level1Data.SubItems.Add(leafData);
            Menu.FavoriteCollection.Add(level1Data);
            Menu.FavoriteCollection = Menu.FavoriteCollection;
            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_Favorite, Menu.FavoriteCollection, true);
            
        }

        void RemoveFavorite(ItemData leafData)
        {
            if (Menu.FavoriteCollection != null)
            {
                foreach (var item1 in Menu.FavoriteCollection)
                {
                    if (item1 != null && item1.SubItems != null)
                    {
                        foreach (var item2 in item1.SubItems)
                        {
                            if (item2.Id == leafData.Id)
                            {
                                item1.SubItems.Remove(item2);
                                if (item1.SubItems == null || item1.SubItems.Count == 0)
                                {
                                    Menu.FavoriteCollection.Remove(item1);
                                };
                                Menu.FavoriteCollection = Menu.FavoriteCollection;
                                ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_Favorite, Menu.FavoriteCollection, true);
                                
                                return;
                            }
                        }
                    }

                }
            }
        }


        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ClickICON != null)
            {
                ClickICON(this, new EventArgs());
                this.NotifyDataSource();
            }
        }

        private void TextBlockItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ClickItem != null)
            {
                ClickItem(this, new EventArgs());
            }
            if (Menu != null && (Type == SubMenuItemIconType.Heart || Type == SubMenuItemIconType.HeartEmpty))
            {
                m_isSelected = true;
                if (Menu.LastSubMenuItemInAll != null)
                {
                    Menu.LastSubMenuItemInAll.SetSelected(false);
                }
                Menu.LastSubMenuItemInAll = this;
                Menu.LastSubMenuItemInAll.SetSelected(true);
                ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_AllStatus, this.DataContext, false);
                
            }
        }

        private void TextBlockItem_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlockItem.TextDecorations = TextDecorations.Underline;
            TextBlockItem.Foreground = new SolidColorBrush(Color.FromArgb(255, 39, 83, 156));
        }

        private void TextBlockItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!m_isSelected || Type == SubMenuItemIconType.Folder || Type == SubMenuItemIconType.FolderOpen)
            {
                TextBlockItem.TextDecorations = null;
                TextBlockItem.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// 设置当前页面是否选中
        /// </summary>
        /// <param name="isSelected"></param>
        public void SetSelected(bool isSelected)
        {
            m_isSelected = isSelected;
            if (Type == SubMenuItemIconType.Folder || Type == SubMenuItemIconType.FolderOpen)
            {
                if (isSelected)
                {
                    ((StackPanel)this.Tag).Visibility = System.Windows.Visibility.Visible;
                    Type = SubMenuItemIconType.FolderOpen;
                    ((MenuItem)this.ParentNode).IsSelected = true;
                    ((MenuItem)((MenuItem)this.ParentNode).ParentNode).IsSelected = true;
                }
                else
                {
                    ((StackPanel)this.Tag).Visibility = System.Windows.Visibility.Collapsed;
                    Type = SubMenuItemIconType.Folder;
                    ((MenuItem)this.ParentNode).IsSelected = false;
                    ((MenuItem)((MenuItem)this.ParentNode).ParentNode).IsSelected = false;
                }
            }
            else
            {
                if (isSelected)
                {
                    TextBlockItem_MouseEnter(null, null);
                    if (this.ParentNode is SubMenuItem)
                    {
                        ((SubMenuItem)this.ParentNode).SetSelected(isSelected);
                        ((MenuItem)((SubMenuItem)this.ParentNode).ParentNode).IsSelected = true;
                        ((MenuItem)((MenuItem)((SubMenuItem)this.ParentNode).ParentNode).ParentNode).IsSelected = true;
                    }
                    else
                    {
                        ((MenuItem)this.ParentNode).IsSelected = true;
                        ((MenuItem)((MenuItem)this.ParentNode).ParentNode).IsSelected = true;
                    }
                }
                else
                {
                    TextBlockItem_MouseLeave(null, null);
                    if (this.ParentNode is SubMenuItem)
                    {
                        ((SubMenuItem)this.ParentNode).SetSelected(isSelected);
                        ((MenuItem)((SubMenuItem)this.ParentNode).ParentNode).IsSelected = false;
                        ((MenuItem)((MenuItem)((SubMenuItem)this.ParentNode).ParentNode).ParentNode).IsSelected = false;
                    }
                    else
                    {
                        ((MenuItem)this.ParentNode).IsSelected = false;
                        ((MenuItem)((MenuItem)this.ParentNode).ParentNode).IsSelected = false;
                    }
                }
            }
        }
    }
}
