using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.QueryFilter.Invoice
{
    public class InvoicePrintAllQueryVM : ModelBase
    {
        private String m_SOSysNo;
        public String SOSysNo
        {
            get
            {
                return this.m_SOSysNo;
            }
            set
            {
                this.SetValue("SOSysNo", ref m_SOSysNo, value);
            }
        }

        private String m_StockSysNo;
        public String StockSysNo
        {
            get
            {
                return this.m_StockSysNo;
            }
            set
            {
                this.SetValue("StockSysNo", ref m_StockSysNo, value);
            }
        }

        private Boolean? m_IsVAT;
        public Boolean? IsVAT
        {
            get
            {
                return this.m_IsVAT;
            }
            set
            {
                this.SetValue("IsVAT", ref m_IsVAT, value);
            }
        }

        private ECCentral.BizEntity.SO.SOType? m_SOType;
        public ECCentral.BizEntity.SO.SOType? SOType
        {
            get
            {
                return this.m_SOType;
            }
            set
            {
                this.SetValue("SOType", ref m_SOType, value);
            }
        }

        private ECCentral.BizEntity.SO.SOStatus? m_SOStatus;
        public ECCentral.BizEntity.SO.SOStatus? SOStatus
        {
            get
            {
                return this.m_SOStatus;
            }
            set
            {
                this.SetValue("SOStatus", ref m_SOStatus, value);
            }
        }

        private DateTime? m_CreateDateFrom;
        public DateTime? CreateDateFrom
        {
            get
            {
                return this.m_CreateDateFrom;
            }
            set
            {
                this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value);
            }
        }

        private DateTime? m_CreateDateTo;
        public DateTime? CreateDateTo
        {
            get
            {
                return this.m_CreateDateTo;
            }
            set
            {
                this.SetValue("CreateDateTo", ref m_CreateDateTo, value);
            }
        }

        private DateTime? m_AuditDateFrom;
        public DateTime? AuditDateFrom
        {
            get
            {
                return this.m_AuditDateFrom;
            }
            set
            {
                this.SetValue("AuditDateFrom", ref m_AuditDateFrom, value);
            }
        }

        private DateTime? m_AuditDateTo;
        public DateTime? AuditDateTo
        {
            get
            {
                return this.m_AuditDateTo;
            }
            set
            {
                this.SetValue("AuditDateTo", ref m_AuditDateTo, value);
            }
        }

        private DateTime? m_OutDateFrom;
        public DateTime? OutDateFrom
        {
            get
            {
                return this.m_OutDateFrom;
            }
            set
            {
                this.SetValue("OutDateFrom", ref m_OutDateFrom, value);
            }
        }

        private DateTime? m_OutDateTo;
        public DateTime? OutDateTo
        {
            get
            {
                return this.m_OutDateTo;
            }
            set
            {
                this.SetValue("OutDateTo", ref m_OutDateTo, value);
            }
        }

        private ECCentral.BizEntity.Invoice.InvoiceType? m_InvoiceType;
        public ECCentral.BizEntity.Invoice.InvoiceType? InvoiceType
        {
            get
            {
                return this.m_InvoiceType;
            }
            set
            {
                this.SetValue("InvoiceType", ref m_InvoiceType, value);
            }
        }

        private ECCentral.BizEntity.Invoice.StockType? m_StockType;
        public ECCentral.BizEntity.Invoice.StockType? StockType
        {
            get
            {
                return this.m_StockType;
            }
            set
            {
                this.SetValue("StockType", ref m_StockType, value);
            }
        }

        private ECCentral.BizEntity.Invoice.DeliveryType? m_ShippingType;
        public ECCentral.BizEntity.Invoice.DeliveryType? ShippingType
        {
            get
            {
                return this.m_ShippingType;
            }
            set
            {
                this.SetValue("ShippingType", ref m_ShippingType, value);
            }
        }

        private Int32? m_VendorSysNo;
        public Int32? VendorSysNo
        {
            get
            {
                return this.m_VendorSysNo;
            }
            set
            {
                this.SetValue("VendorSysNo", ref m_VendorSysNo, value);
            }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get
            {
                return this.m_CompanyCode;
            }
            set
            {
                this.SetValue("CompanyCode", ref m_CompanyCode, value);
            }
        }

        #region 扩展属性

        public List<KeyValuePair<Boolean?, string>> YNList
        {
            get
            {
                return BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 开票方式列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.Invoice.InvoiceType?, string>> InvoiceTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Invoice.InvoiceType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 仓储方式列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.Invoice.StockType?, string>> StockTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Invoice.StockType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 配送方式列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.Invoice.DeliveryType?, string>> ShippingTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Invoice.DeliveryType>(EnumConverter.EnumAppendItemType.All);
            }
        }

        #endregion 扩展属性
    }
}