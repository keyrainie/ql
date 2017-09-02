using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Shopping
{
    public class AllocatedItemInventoryInfo
    {
        #region [ fields ]

        private int productID;
        private string productCode;
        private int warehouseNumber;
        private string warehouseName;
        private string productName;
        private string briefName;
        private int shoppingQty;
        private int availableQty;
        private SOItemType productType;
        private int stockAvailableQty;
        private int stockConsignQty;
        private int stockVirtualQty;
        private int wareHouseScore;
        private string warehouseCountryCode;

        #endregion

        #region [ properties ]

        /// <summary>
        /// 商品ID
        /// </summary>
        public int ProductID
        {
            get { return this.productID; }
            set { this.productID = value; }
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductCode
        {
            get { return this.productCode; }
            set { this.productCode = value; }
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName
        {
            get
            {
                return this.productName;
            }
            set
            {
                this.productName = value;
            }
        }

        /// <summary>
        /// 产品简称
        /// </summary>
        public string BrieftName
        {
            get
            {
                return this.briefName;
            }
            set
            {
                this.briefName = value;
            }
        }

        /// <summary>
        /// 配送仓库号
        /// </summary>
        public int WarehouseNumber
        {
            get { return this.warehouseNumber; }
            set { this.warehouseNumber = value; }
        }

        /// <summary>
        /// 配送仓库名称
        /// </summary>
        public string WarehouseName
        {
            get { return this.warehouseName; }
            set { this.warehouseName = value; }
        }

        /// <summary>
        /// 希望购买的数量
        /// </summary>
        public int ShoppingQty
        {
            get { return this.shoppingQty; }
            set { this.shoppingQty = value; }
        }

        /// <summary>
        /// 实际能购买的数量
        /// </summary>
        public int AvailableQty
        {
            get { return this.availableQty; }
            set { this.availableQty = value; }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public SOItemType ProductType
        {
            get { return this.productType; }
            set { this.productType = value; }
        }

        /// <summary>
        /// 对应仓库的VirtualQty数量
        /// </summary>
        public int StockVirtualQty
        {
            get { return stockVirtualQty; }
            set { stockVirtualQty = value; }
        }

        /// <summary>
        /// 对应仓库的ConsignQty数量
        /// </summary>
        public int StockConsignQty
        {
            get { return stockConsignQty; }
            set { stockConsignQty = value; }
        }

        /// <summary>
        /// 对应仓库的AvailableQty数量
        /// </summary>
        public int StockAvailableQty
        {
            get { return stockAvailableQty; }
            set { stockAvailableQty = value; }
        }

        public int WareHouseScore
        {
            get { return wareHouseScore; }
            set { wareHouseScore = value; }
        }

        /// <summary>
        /// 仓库所在国家代码
        /// </summary>
        public string WarehouseCountryCode
        {
            get { return warehouseCountryCode; }
            set { warehouseCountryCode = value; }
        }

        #endregion
    }
}
