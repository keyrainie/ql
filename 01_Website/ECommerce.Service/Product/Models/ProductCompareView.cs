using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;

namespace ECommerce.Facade.Product.Models
{
    /// <summary>
    /// 产品比较页面模型
    /// </summary>
    public class ProductCompareView
    {
        private List<ProductCompareInfo> productCellList;
        private List<string> removeList;
        private List<CompareProperty> comparePropertyList;

        public List<ProductCompareInfo> ProductCellList
        {
            get { return this.productCellList; }
            set { this.productCellList = value; }
        }

        public List<string> RemoveList
        {
            get { return this.removeList; }
            set { this.removeList = value; }
        }

        public List<CompareProperty> ComparePropertyList
        {
            get { return this.comparePropertyList; }
            set { this.comparePropertyList = value; }
        }

    }
}
