using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PayItemListOrderQueryVM : ModelBase
    {
        private PayableOrderType? m_OrderType;
        /// <summary>
        /// 应付款单据类型
        /// </summary>
        public PayableOrderType? OrderType
        {
            get
            {
                return m_OrderType;
            }
            set
            {
                base.SetValue("OrderType", ref m_OrderType, value);
            }
        }

        private string m_OrderID;
        /// <summary>
        /// 单据编号
        /// </summary>
        public string OrderID
        {
            get
            {
                return m_OrderID;
            }
            set
            {
                base.SetValue("OrderID", ref m_OrderID, value);
            }
        }

        private ECCentral.BizEntity.PO.PurchaseOrderStatus? m_POStatus;
        public ECCentral.BizEntity.PO.PurchaseOrderStatus? POStatus
        {
            get
            {
                return m_POStatus;
            }
            set
            {
                base.SetValue("POStatus", ref m_POStatus, value);
            }
        }

        private ECCentral.BizEntity.PO.SettleStatus? m_VendorSettleStatus;
        public ECCentral.BizEntity.PO.SettleStatus? VendorSettleStatus
        {
            get
            {
                return m_VendorSettleStatus;
            }
            set
            {
                base.SetValue("VendorSettleStatus", ref m_VendorSettleStatus, value);
            }
        }

        private DateTime? m_POETPFrom;
        public DateTime? POETPFrom
        {
            get
            {
                return m_POETPFrom;
            }
            set
            {
                base.SetValue("POETPFrom", ref m_POETPFrom, value);
            }
        }

        private DateTime? m_POETPTo;
        public DateTime? POETPTo
        {
            get
            {
                return m_POETPTo;
            }
            set
            {
                base.SetValue("POETPTo", ref m_POETPTo, value);
            }
        }

        #region 扩展属性

        /// <summary>
        /// 单据类型列表
        /// </summary>
        public List<KeyValuePair<PayableOrderType?, string>> OrderTypeList
        {
            get
            {
                var orderTypeList = EnumConverter.GetKeyValuePairs<PayableOrderType>(EnumConverter.EnumAppendItemType.All);
                orderTypeList = orderTypeList.Where(w => w.Key == PayableOrderType.PO || w.Key == PayableOrderType.VendorSettleOrder || !w.Key.HasValue)
                    .Select(s => s).ToList();
                return orderTypeList;
            }
        }

        /// <summary>
        /// PO状态列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.PO.PurchaseOrderStatus?, string>> POStatusList
        {
            get
            {
                var statusList = EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PurchaseOrderStatus>(EnumConverter.EnumAppendItemType.All);
                statusList.RemoveAll(r => r.Key == ECCentral.BizEntity.PO.PurchaseOrderStatus.Origin);
                return statusList;
            }
        }

        /// <summary>
        /// 供应商结算单状态列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.PO.SettleStatus?, string>> VendorSettleStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.SettleStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        public string CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        #endregion 扩展属性
    }
}
