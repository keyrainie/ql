using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Menu;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using ControlPanel.SilverlightUI;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            InitialValues.ContainingLayoutPanel = this.LayoutRoot;
           
            InitMenu();
            CPApplication.Current.Browser = this.PageBrowserRightArea;
            InitMostUsedData();

            this.PageBrowserRightArea.DefaultPage = string.IsNullOrEmpty(CPApplication.Current.DefaultPage) ? this.PageBrowserRightArea.DefaultPage : CPApplication.Current.DefaultPage;
        }

        private void InitMostUsedData()
        {
            var mgr = CPApplication.Current.Browser.ComponentCollection.First(p => p.Name == "PageRecordManager") as PageRecordManager;
            List<ItemData> itemData = new List<ItemData>();
            foreach (var record in mgr.GetHotHitPages(10))
            {
                itemData.Add(new ItemData { Name = record.Title, Url = record.Url });
            }
            MenuControlArea.MostUsedData = itemData;
        }


        #region Init Menu

        void InitMenu()
        {
            var items = new List<ItemData>();
            int count = 0;
            var authMenuItems = ComponentFactory.GetComponent<IAuth>().GetAuthorizedMenuItems();
            foreach (var item in authMenuItems)
            {
                items.Add(GenerateMenuItemData(item));
                count++;
                if (count == 12)
                {
                    break;
                }
            }
            var favorite = ComponentFactory.GetComponent<IUserProfile>().Get<List<ItemData>>(UserProfileKey.Key_Favorite);
            if (favorite != null)
            {
                MenuControlArea.FavoriteCollection = favorite;
                MarkFavoritedItemData(items, favorite);
            }
            MenuControlArea.MenuCollection = items;
            
            MenuControlArea.Navigating = (obj, args) =>
            {
                if (((ItemData)args.Data).Url != null && ((ItemData)args.Data).Url.Trim().Length > 0)
                {
                    this.PageBrowserRightArea.Navigate(((ItemData)args.Data).Url);
                    MenuControlArea.IsOpen = false;

                    if (((ItemData)args.Data).IsFavorited)
                    {
                        ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Click", "Menu:Favorites");
                    }
                    else
                    {
                        ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Click", "Menu:Page");
                    }
                                   
                }
            };
            
        }

        //标识已经收藏的菜单项
        private void MarkFavoritedItemData(List<ItemData> items, List<ItemData> favorites)
        {
            foreach (var item in items)
            {
                foreach (var fvt in favorites)
                {
                    if (item.Id == fvt.Id)
                    {
                        foreach (var subItem in item.SubItems)
                        {
                            RecursiveMarkFavoritedItemData(subItem, fvt.SubItems);
                        }
                    }
                }
            }  
        }

        void RecursiveMarkFavoritedItemData(ItemData data, List<ItemData> subFavorites)
        {
            if(subFavorites.Contains(data))
            {
                data.IsFavorited = true;
            }
            if (data.SubItems != null)
            {
                foreach (var item in data.SubItems )
                {
                    RecursiveMarkFavoritedItemData(item, subFavorites);
                }
            }
        }


        ItemData GenerateMenuItemData(AuthMenuItem item)
        {
            var item1 = new ItemData { Name = item.Name, Url = item.URL, Id = item.Id, IsPage = (item.Type != AuthMenuItemType.Category ? true : false) };
            if (item.Type == AuthMenuItemType.Page)
            {
                MenuControlArea.MenuFlatData.Add(item1);
            }
            if (item.Items != null)
            {
                item1.SubItems = new List<ItemData>();
                foreach (var subItem in item.Items)
                {
                    item1.SubItems.Add(GenerateMenuItemData(subItem));
                }
            }
            return item1;
        }
        #endregion        
    }
}
