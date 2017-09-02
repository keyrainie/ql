using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.SearchEngine;
using Nesoft.Utility;

namespace Nesoft.ECWeb.M.Models.Search
{
    public class ProductSearchResultVM
    {
        public string SortKey { get; set; }

        public PagedResult<ProductSearchResultItem> ProductList { get; set; }
    }
}