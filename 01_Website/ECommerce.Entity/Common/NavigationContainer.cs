using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    /// <summary>
    /// 提供类为向导航控件提供导航用户界面数据和值而实现的接口。 
    /// </summary>
    public class NavigationContainer
    {
        #region [ Privates ]

        private string title;
        private List<NavigationItem> navigationItems;

        #endregion

        #region [ Contrustros ]

        /// <summary>
        /// 初始化 <see cref="NavigationContainer"/> 的实例。
        /// </summary>
        public NavigationContainer()
        {
            this.title = string.Empty;
            this.navigationItems = new List<NavigationItem>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// 标题。
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        /// <summary>
        /// 导航项集合。
        /// </summary>
        public List<NavigationItem> NavigationItems
        {
            get { return this.navigationItems; }
            set { this.navigationItems = value; }
        }

        #endregion
    }
}
