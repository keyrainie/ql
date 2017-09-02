using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
namespace ECCentral.BizEntity.PO
{
    [Serializable]
    [DataContract]
    public class CollectionPaymentInfo : EntityBase
    {
        /// <summary>
        /// 供应商信息
        /// </summary>
        [DataMember]
        public VendorInfo VendorInfo { get; set; }
        /// <summary>
        /// 源渠道仓库
        /// </summary>
        [DataMember]
        public StockInfo SourceStockInfo { get; set; }

        public CollectionPaymentInfo()
        {
            VendorInfo = new VendorInfo();
            SourceStockInfo = new StockInfo();
            TaxRateData = PurchaseOrderTaxRate.Percent017;
        }
        private decimal m_totalAmt;

        [DataMember]
        public string SettleID { get; set; }

        

        [DataMember]
        public decimal TotalAmt
        {
            get
            {
                return m_totalAmt;
            }
            set
            {
                m_totalAmt = this.FormatDecimal(value);
            }
        }

        [DataMember]
        public int? CurrencyCode { get; set; }

        [DataMember]
        public DateTime? CreateTime { get; set; }

        [DataMember]
        public int? CreateUserSysNo { get; set; }

        [DataMember]
        public string CreateUser { get; set; }

        [DataMember]
        public DateTime? AuditTime { get; set; }

        [DataMember]
        public int? AuditUserSysNo { get; set; }

        [DataMember]
        public string AuditUser { get; set; }

        [DataMember]
        public DateTime? SettleTime { get; set; }

        [DataMember]
        public int? SettleUserSysNo { get; set; }

        /// <summary>
        /// 结算人
        /// </summary>
        [DataMember]
        public UserInfo SettleUser { get; set; }


        [DataMember]
        public string Memo { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public POCollectionPaymentSettleStatus? Status { get; set; }

        [DataMember]
        public int? SettleBalanceSysNo { get; set; }

        //[DataMember]
        //public decimal? TaxRate { get; set; }

        [DataMember]
        public List<CollectionPaymentItem> SettleItems { get; set; }
        ////////////////CRl16063///////////////

        [DataMember]
        public int? ReturnPointPM { get; set; }

        [DataMember]
        public int? ReturnPointC3SysNo { get; set; }

        [DataMember]
        public int? PM_ReturnPointSysNo { get; set; }

        [DataMember]
        public decimal? UsingReturnPoint { get; set; }

        ////////////////CRL16991////////////////////////

        [DataMember]
        public int? PayPeriodType { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [DataMember]
        public PurchaseOrderTaxRate? TaxRateData { get; set; }

        /// <summary>
        /// 结算所属PM信息
        /// </summary>
        [DataMember]
        public ProductManagerInfo PMInfo { get; set; }

        /// <summary>
        /// 是否高级PM 用于PM产品线相关验证 
        /// </summary>
        [DataMember]
        public bool? IsManagerPM { get; set; }

        /// <summary>
        /// 结算单商品所属商品线
        /// </summary>
        [DataMember]
        public int? ProductLineSysNo { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        [DataMember]
        public int? CurrentUserSysNo { get; set; }

        [DataMember]
        public int CurrencySysNo { get; set; }

        [DataMember]
        public int StockSysNo { get; set; }


        [DataMember]
        public decimal TaxRate { get; set; }

        [DataMember]
        public string VendorName { get; set; }

        [DataMember]
        public int VendorSysNo { get; set; }


    }

    [Serializable]
    [DataContract]
    public class CollectionPaymentItem : EntityBase
    {
        private decimal m_cost;
        private decimal? m_foldCost;

        //public int? SysNO { get; set; }
        public int ConsignSettleRuleSysNO { get; set; }
         [DataMember]
        public int? ItemSysNo
        {
            get;
            set;
        }

        [DataMember]
        public int? SettleSysNo { get; set; }

        [DataMember]
        public decimal Cost
        {
            get
            {
                return m_cost;
            }
            set
            {
                m_cost = FormatDecimal(value);
            }
        }
        [DataMember]
        public int? POConsignToAccLogSysNo { get; set; }

       
        [DataMember]
        public decimal? FoldCost
        {
            get
            {
                m_foldCost = Cost - ConsignToAccLogInfo.CreateCost.Value;
                return m_foldCost;
            }
            set
            {
                m_foldCost = FormatDecimal(value);
            }
        }

        [DataMember]
        public string ProductID { get; set; }


        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public int? ProductSysNo { get; set; }

        [DataMember]
        public string StockName
        {
            get;
            set;
        }

        [DataMember]
        public string Vendor
        {
            get;
            set;
        }

       
        [DataMember]
        public DateTime? CreateTime { get; set; }

        [DataMember]
        public int ConsignQty { get; set; }

        [DataMember]
        public int OnLineQty { get; set; }

       
       
        /// <summary>
        /// 代销结算类型
        /// </summary>

        [DataMember]
        public string SettleType { get; set; }
        /// <summary>
        /// 佣金百分比
        /// </summary>

        [DataMember]
        public decimal? SettlePercentage { get; set; }

       
        [DataMember]
        public decimal? Point { get; set; }


        [DataMember]
        public int? SettleRuleSysNo { get; set; }

        [DataMember]
        public string SettleRuleName { get; set; }

        //ConsignSettleRuleName
        [DataMember]
        public decimal? SettlePrice { get; set; }

        [DataMember]
        public int? AcquireReturnPointType { get; set; }

        [DataMember]
        public decimal? AcquireReturnPoint { get; set; }


        /// <summary>
        /// 代销转财务记录
        /// </summary>
        [DataMember]
        public ConsignToAcctLogInfo ConsignToAccLogInfo { get; set; }

        /// <summary>
        /// 供应商信息
        /// </summary>
        [DataMember]
        public VendorInfo VendorInfo { get; set; }


        [DataMember]
        public ConsignToAccStatus ConsignToAccStatus { get; set; }

        [DataMember]
        public decimal CreateCost { get; set; }

        [DataMember]
        public decimal MinCommission { get; set; }

        [DataMember]
        public int Quantity { get; set; }
        
        [DataMember]
        public decimal RetailPrice { get; set; }

        [DataMember]
        public int StockSysNo { get; set; }

        [DataMember]
        public string VendorName { get; set; }

        [DataMember]
        public int VendorSysNo { get; set; }
    }

    [Serializable]
    [DataContract]
    public class EntityBase : IIdentity, ICompany
    {
        [DataMember]
        public int? SysNo { get; set; }
        private const int CONST_DECIMALFIGURE = 2;

        [DataMember]
        public int? OperationUserSysNumber { get; set; }

        [DataMember]
        public string OperationUserUniqueName { get; set; }

        [DataMember]
        public string OperationIP { get; set; }


        protected decimal FormatDecimal(decimal data)
        {
            return decimal.Round(data, CONST_DECIMALFIGURE);
        }

        protected decimal? FormatDecimal(decimal? data)
        {
            if (data.HasValue)
            {
                data = FormatDecimal(data.Value);
            }

            return data;
        }

        private string companyCode;
        [DataMember]
        public string CompanyCode
        {
            //get

            get
            {
                if(string.IsNullOrEmpty(companyCode))
                {
                    companyCode= "8601";
                }
                return companyCode;
            }
            set
            {
                companyCode = value;
            }
        }

        [DataMember]
        public string StoreCompanyCode
        {
            //get
            //{
            //    if (BusinessContext.Current != null
            //        && BusinessContext.Current.StoreCompanyCode != null)
            //    {
            //        return BusinessContext.Current.StoreCompanyCode;
            //    }
            //    else
            //    {
            //        //VP
            //        return "8601";
            //    }
            //}
            get;
            set;
        }

        //CRL20438 By Kilin
        [DataMember]
        public string LanguageCode
        {
            //get
            //{
            //    if (BusinessContext.Current != null
            //        && BusinessContext.Current.Language != null)
            //    {
            //        return BusinessContext.Current.Language;
            //    }
            //    else
            //    {
            //        //VP
            //        return "zh-CN";
            //    }
            //}
            get;
            set;
        }

        [DataMember]
        public string UserLoginName
        {
            get;
            set;
        }
    }
}

   
