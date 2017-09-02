using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;
using ECommerce.Entity.SearchEngine;
using ECommerce.Utility; 

namespace ECommerce.Entity.SolrSearch
{
    public class ProductSearchResult
    {
        private NavigationContainer navigation;
        private NavigationContainer filterNvigation;
        private PagedResult<ProductSearchResultItem> productDataList;
        private long numberOfProduct;

        /// <summary>
        /// 导航(类别专用)
        /// </summary>
        public NavigationContainer Navigation
        {
            get { return this.navigation; }
            set { this.navigation = value; }
        }


        /// <summary>
        /// 过滤条件专用
        /// </summary>
        public NavigationContainer FilterNavigation
        {
            get { return this.filterNvigation; }
            set { this.filterNvigation = value; }
        }
        /// <summary>
        /// 产品列表
        /// </summary>
        public PagedResult<ProductSearchResultItem> ProductDataList
        {
            get { return this.productDataList; }
            set { this.productDataList = value; }
        }

        /// <summary>
        /// 产品数据
        /// </summary>
        public long NumberOfProduct
        {
            get { return this.numberOfProduct; }
            set { this.numberOfProduct = value; }
        }

    }
}
