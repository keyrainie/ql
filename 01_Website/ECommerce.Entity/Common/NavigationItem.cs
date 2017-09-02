using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    public class NavigationItem
    {
        #region [ Fields ]

        private int numberOfItem;
        private int priority;
        private string rootValue;
        private string rootName;
        private string name;
        private string value;
        private string description;
        private string navigateUrl;
        private NavigationItemType itemType;
        private List<NavigationItem> subNavigationItems;

        #endregion

        #region [ Contonstructors ]

        /// <summary>
        /// 初始化 <see cref="NavigationItem"/> 的实例。
        /// </summary>
        public NavigationItem()
        {
            this.itemType = NavigationItemType.Attribute;
            this.subNavigationItems = new List<NavigationItem>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// 获取导航项下对应的Item数。
        /// </summary>
        public int NumberOfItem
        {
            get { return this.numberOfItem; }
            set { this.numberOfItem = value; }
        }

        /// <summary>
        /// 属性之间的优先级
        /// </summary>
        public int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        /// <summary>
        /// 获取导航项的类型。
        /// </summary>
        public NavigationItemType ItemType
        {
            get { return this.itemType; }
            set { this.itemType = value; }
        }

        /// <summary>
        /// 导航根节ID。
        /// </summary>
        public string RootValue
        {
            get { return this.rootValue; }
            set { this.rootValue = value; }
        }

        /// <summary>
        /// 导航根节点Name
        /// </summary>
        public string RootName
        {
            get { return this.rootName; }
            set { this.rootName = value; }
        }

        /// <summary>
        /// 获取表示导航控件的导航节点的名称的文本。
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 获取一个非显示值，该值用于存储有关导航节点的任何其他数据。
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// 获取表示导航控件的导航节点的说明的文本。
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// 获取单击导航节点时要导航至的 URL。
        /// </summary>
        public string NavigateUrl
        {
            get { return this.navigateUrl; }
            set { this.navigateUrl = value; }
        }


        /// <summary>
        /// 获取该层次子导航结点。
        /// </summary>
        public List<NavigationItem> SubNavigationItems
        {
            get { return this.subNavigationItems; }
            set { this.subNavigationItems = value; }
        }

        #endregion	
    }
}

