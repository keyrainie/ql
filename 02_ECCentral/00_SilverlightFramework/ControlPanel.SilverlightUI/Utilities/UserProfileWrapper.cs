using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Menu;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI
{
    public static class UserProfileWrapper
    {
        private static ObservableCollection<AuthMenuItem> s_list;

        static UserProfileWrapper()
        {
            s_list = ComponentFactory.GetComponent<IAuth>().AuthorizedNavigateToList();
        }

        public static void InitProfile()
        {
            ProcessFavorite();
            ProcessQuickLink();
        }

        #region Private Methods


        private static void ProcessFavorite()
        {
            var data = ComponentFactory.GetComponent<IUserProfile>().Get<List<ItemData>>(UserProfileKey.Key_Favorite);

            if (data != null)
            {
                for (int i = data.Count - 1; i >= 0; i--)
                {
                    var itemData = data[i] as ItemData;

                    if (itemData != null && itemData.SubItems != null)
                    {
                        for (int j = itemData.SubItems.Count - 1; j >= 0; j--)
                        {
                            bool isRemove = true;
                            foreach (AuthMenuItem menuItem in s_list)
                            {
                                if (menuItem.Type == AuthMenuItemType.Page)
                                {
                                    if (!menuItem.Id.IsNullOrEmpty() && !itemData.Id.IsNullOrEmpty()
                                        && string.Compare(menuItem.Id, itemData.SubItems[j].Id, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        itemData.SubItems[j].Name = menuItem.Name;
                                        itemData.SubItems[j].Url = menuItem.URL;
                                        itemData.Name = FindParentName(menuItem);

                                        isRemove = false;
                                        break;
                                    }
                                }
                            }
                            if (isRemove)
                            {
                                itemData.SubItems.Remove(itemData.SubItems[j]);
                            }
                        }
                        if (itemData.SubItems.Count == 0)
                        {
                            data.Remove(itemData);
                        }
                    }
                }

                ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_Favorite, data, true);
            }
        }

        private static void ProcessQuickLink()
        {
            var data = ComponentFactory.GetComponent<IUserProfile>().Get<List<QuickLinksModel>>(UserProfileKey.Key_QuickLink);

            if (data != null)
            {
                for (int i = data.Count - 1; i >= 0; i--)
                {
                    bool isRemove = true;
                    var quickLink = data[i] as QuickLinksModel;

                    if (quickLink != null)
                    {

                        foreach (AuthMenuItem menuItem in s_list)
                        {
                            if (menuItem.Type == AuthMenuItemType.Page)
                            {
                                if ((!menuItem.URL.IsNullOrEmpty() && !quickLink.BaseUrl.IsNullOrEmpty()) && (menuItem.URL.Equals(quickLink.BaseUrl)
                                    || quickLink.BaseUrl.Equals(menuItem.URL)))
                                {
                                    if (!quickLink.IsRename)
                                    {
                                        quickLink.QuickLinkName = menuItem.Name;
                                    }
                                    quickLink.BaseUrl = menuItem.URL;
                                    isRemove = false;
                                    break;
                                }
                            }
                        }

                        if (isRemove)
                        {
                            data.Remove(quickLink);
                        }
                    }
                }

                ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, data, true);
            }
        }

        private static string FindParentName(AuthMenuItem menuItem)
        {
            AuthMenuItem current = menuItem.Parent == null ? menuItem : menuItem.Parent;
            if (current.Parent == null)
            {
                foreach (var item in s_list)
                {
                    if (current.Id == item.Id)
                    {
                        current = item.Parent;
                        break;
                    }
                }

            }
            if (current != null)
            {
                return current.Name;
            }
            else
            {
                return "";
            }
        }


        #endregion
    }

}
