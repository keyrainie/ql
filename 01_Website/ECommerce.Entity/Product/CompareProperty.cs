using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Category;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 产品比较模型
    /// </summary>
    public class CompareProperty
    {
        private string propertyName;
        private List<string> compareValueList;

        public string PropertyName
        {
            get { return this.propertyName; }
            set { this.propertyName = value; }
        }

        public List<string> CompareValueList
        {
            get { return this.compareValueList; }
            set { this.compareValueList = value; }
        }
    }
}
