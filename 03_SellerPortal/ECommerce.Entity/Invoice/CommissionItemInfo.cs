using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ECommerce.Enums;

namespace ECommerce.Entity.Invoice
{
    public class CommissionItemInfo : EntityBase
    {

        public int SysNo { get; set; }


        public int CommissionMasterSysNo { get; set; }


        public int VendorManufacturerSysNo { get; set; }

        public int RuleSysNo { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal SalesCommissionFee { get; set; }


        public decimal OrderCommissionFee { get; set; }

        public decimal TotalSaleAmt { get; set; }

        public string InUser { get; set; }


        public DateTime InDate { get; set; }


        public string EditUser { get; set; }


        public DateTime EditDate { get; set; }

        public string CurrencyCode { get; set; }


        public CommissionType CommissionType { get; set; }


        public int TotalQty { get; set; }

        /// <summary>
        /// 销售规则（Xml）
        /// </summary>

        public string SalesRuleXml { get; set; }

        public string SalesRuleStr { get; set; }
        /// <summary>
        /// 销售规则（实体）
        /// </summary>


        public SalesRuleInfo SalesRuleEntity
        {
            get
            {
                SalesRuleInfo salesRule = null;
                if (!string.IsNullOrEmpty(this.SalesRuleXml))
                {
                    StringReader xmlReader = new StringReader(this.SalesRuleXml);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(SalesRuleInfo));
                    salesRule = xmlSerializer.Deserialize(xmlReader) as SalesRuleInfo;
                }
                return salesRule;
            }
        }

        public string ManufacturerName { get; set; }

        public string C2Name { get; set; }



        public string C3Name { get; set; }


        public string BrandName { get; set; }

        public string ManufacturerAndCategory
        {
            get
            {

                string result = string.Empty;
                result = string.Format("{0}{1}({2})"
                    , this.ManufacturerName
                    , string.IsNullOrEmpty(this.BrandName) ? "" : string.Format("({0})", this.BrandName)
                    , string.IsNullOrEmpty(this.C3Name) ? this.C2Name : this.C3Name);

                return result;
            }
        }


        public decimal SalesRuleDEF { get; set; }

        public decimal SalesRuleSOC { get; set; }

        /// <summary>
        ///货款
        /// </summary>
        public decimal ProductSaleAmt { get; set; }

        /// <summary>
        /// 税金
        /// </summary>
        public decimal TariffAmt { get; set; }
    }

    [XmlRoot("SalesRuleEntity")]
    [Serializable]
    public class SalesRuleInfo
    {
        public class SaleRuleItem
        {
            public decimal StartAmt { get; set; }

            public decimal EndAmt { get; set; }

            public float Percentage { get; set; }

            public int Order { get; set; }
        }

        [XmlArray("Rules")]
        [XmlArrayItem("Rule")]
        public List<SaleRuleItem> Rules { get; set; }

        [XmlElement("MinCommissionAmt")]
        public decimal MinCommissionAmt { get; set; }
    }
}
