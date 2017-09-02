using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 6种合作方式定义
    /// </summary>
    public class VendorInfo
    {
        #region [ consts ]
        public const string NEG = "NEG";
        public const string MET = "MET";
        #endregion

        #region [ fields ]

        private string stockType;
        private string shippingType;
        private string invoiceType;

        #endregion

        #region [ properties ]

        public int VendorSysno { get; set; }


        public string VendorID { get; set; }


        public string VendorName { get; set; }


        public string StockType
        {
            get
            {
                if (string.IsNullOrEmpty(this.stockType))
                {
                    this.stockType = "NEG";
                }
                return this.stockType;
            }
            set { this.stockType = value; }
        }

       
        public string ShippingType
        {
            get
            {
                if (string.IsNullOrEmpty(this.shippingType))
                {
                    this.shippingType = "NEG";
                }
                return this.shippingType;
            }
            set { this.shippingType = value; }
        }

       
        public string InvoiceType
        {
            get
            {
                if (string.IsNullOrEmpty(this.invoiceType))
                {
                    this.invoiceType = "NEG";
                }
                return this.invoiceType;
            }
            set { this.invoiceType = value; }
        }

        /// <summary>
        /// 获取或设置商家Logo
        /// </summary>
        public string VendorLogo { get; set; }

        /// <summary>
        /// 获取或设置商家 VendorBriefName
        /// </summary>
        public string VendorBriefName { get; set; }

        /// <summary>
        /// 获取或设置商家区域编号
        /// </summary>
        public string RepairPostcode { get; set; }

        /// <summary>
        /// 获取或设置商家维修地址
        /// </summary>
        public string RepairAddress { get; set; }


        public string IsShowStore { get; set; }


        public string ScoreQQNumber { get; set; }


        public string ScoreIM_Enabled { get; set; }


        public string VendorEnglishName { get; set; }

        /// <summary>
        /// 是否显示商家店铺
        /// </summary>
        public bool ShowStore
        {
            get
            {
                return this.IsShowStore == "Y";
            }
        }

        /// <summary>
        /// 6种类型
        /// </summary>
        public int SellerType
        {
            get
            {
                if (this.stockType == NEG && this.shippingType == NEG && this.invoiceType == NEG)
                {
                    return 1;
                }
                else if (this.stockType == MET && this.shippingType == NEG && this.invoiceType == NEG)
                {
                    return 2;
                }
                else if (this.stockType == MET && this.shippingType == MET && this.invoiceType == NEG)
                {
                    return 3;
                }
                else if (this.stockType == NEG && this.shippingType == NEG && this.invoiceType == MET)
                {
                    return 4;
                }
                else if (this.stockType == MET && this.shippingType == NEG && this.invoiceType == MET)
                {
                    return 5;
                }
                else if (this.stockType == MET && this.shippingType == MET && this.invoiceType == MET)
                {
                    return 6;
                }
                else
                {
                    return 1;
                }
            }
        }

        #endregion
    }
}
