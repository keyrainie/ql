using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.SearchEngine
{
    public class ProductACSearchRecord
    {
        [SolrNet.Attributes.SolrField("*")]
        public string Fields
        {
            get;
            set;
        }
    }
}
