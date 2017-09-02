using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;
using ECommerce.Utility;

namespace ECommerce.Web
{
    public class MenuConfig
    {
        private static List<MenuItem> m_MenuItemList = null;

        public static List<MenuItem> GetMenuItemList()
        {
            string menuPath = HttpContext.Current.Server.MapPath("~/Configuration/Menu.config");
            return CacheManager.GetWithLocalCache(menuPath, _GetMenuItemList, menuPath);
        }

        private static List<MenuItem> _GetMenuItemList()
        {
            string menuPath = HttpContext.Current.Server.MapPath("~/Configuration/Menu.config");
            m_MenuItemList = ECommerce.Utility.SerializationUtility.LoadFromXml<List<MenuItem>>(menuPath);
            if (m_MenuItemList == null)
            {
                m_MenuItemList = new List<MenuItem>();
            }
            m_MenuItemList.Sort(new MenuItemCodeSort());
            return m_MenuItemList;
        }

        public static List<MenuItem> GetMenuTreePath(MenuItem curMenuItem)
        {           
            List<MenuItem> treePathList = new List<MenuItem>();
            ThroughTree(treePathList, curMenuItem);
            return treePathList;
        }

        private static void ThroughTree(List<MenuItem> treePathList, MenuItem curMenuItem)
        {
            treePathList.Insert(0, curMenuItem);
            if (curMenuItem.ParentMenuCode != "0")
            {               
                List<MenuItem> allList = GetMenuItemList();
                MenuItem parentItem = allList.Find(f => f.MenuCode == curMenuItem.ParentMenuCode);
                if (parentItem != null)
                {
                    ThroughTree(treePathList, parentItem);
                }
            }             
        }


    }

    [Serializable] 
    public class MenuItem
    {
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }
        [XmlAttribute]
        public string MenuCode
        {
            get;
            set;
        }
        [XmlAttribute]
        public string ParentMenuCode
        {
            get;
            set;
        }
        [XmlAttribute]
        public string IsVisiable
        {
            get;
            set;
        }
        [XmlAttribute]
        public string AuthKey
        {
            get;
            set;
        }
        [XmlAttribute]
        public string LinkUrl
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Class
        {
            get;
            set;
        }
    }


    public class MenuItemCodeSort : IComparer<MenuItem>
    {
        public int Compare(MenuItem x, MenuItem y)
        {
            return x.MenuCode.CompareTo(y.MenuCode);
        }
    }
}