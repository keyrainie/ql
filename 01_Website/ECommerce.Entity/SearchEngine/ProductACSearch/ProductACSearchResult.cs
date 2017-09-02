using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.SolrSearch
{
    public class ProductACSearchResult
    {
        private List<ProductACSearchResultItem> _items;
        public List<ProductACSearchResultItem> Items
        {
            set { _items = value; }
            get
            {
                if (_items == null)
                {
                    _items = new List<ProductACSearchResultItem>();
                }
                return _items;
            }
        }
    }

    public class ProductACSearchResultItem
    {
        public string Keyword { get; set; }

        public int Quantity { get; set; }
    }
}
