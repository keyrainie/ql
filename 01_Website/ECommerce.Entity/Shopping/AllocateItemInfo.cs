using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Shopping
{
    public class AllocateItemInfo
    {
        #region [ fields ]

        private int id;
        private string code;
        private string name;
        private SOItemType productType;
        private int productQuantity;

        #endregion

        #region [Properties]


        /// <summary>
        /// Gets or sets product's id
        /// </summary>
        public int ProductID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets product's code
        /// </summary>

        public string ProductCode
        {
            get { return this.code; }
            set { this.code = value; }
        }

        /// <summary>
        /// Gets or sets product's name
        /// </summary>
        public string ProductName
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int Quantity
        {
            get { return this.productQuantity; }
            set { this.productQuantity = value; }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public SOItemType ProductType
        {
            get { return productType; }
            set { productType = value; }
        }

        #endregion
    }
}
