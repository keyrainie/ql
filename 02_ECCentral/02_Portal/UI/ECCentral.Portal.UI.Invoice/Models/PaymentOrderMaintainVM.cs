using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 付款单维护ViewModel
    /// </summary>
    public class PaymentOrderMaintainVM : ModelBase
    {
        private int? m_PaySysNo;
        /// <summary>
        /// 应付款编号
        /// </summary>
        public int? PaySysNo
        {
            get
            {
                return m_PaySysNo;
            }
            set
            {
                base.SetValue("PaySysNo", ref m_PaySysNo, value);
            }
        }

        private PayItemStyle? m_PayStyle;
        /// <summary>
        /// 付款类型
        /// </summary>
        public PayItemStyle? PayStyle
        {
            get
            {
                return m_PayStyle;
            }
            set
            {
                base.SetValue("PayStyle", ref m_PayStyle, value);
            }
        }

        private PayableOrderType? m_OrderType;
        /// <summary>
        /// 单据类型
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

        private int? m_OrderSysNo;
        /// <summary>
        /// 单据系统编号
        /// </summary>
        public int? OrderSysNo
        {
            get
            {
                return m_OrderSysNo;
            }
            set
            {
                base.SetValue("OrderSysNo", ref m_OrderSysNo, value);
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

        private int? m_OrderStatus;
        /// <summary>
        /// 单据状态
        /// </summary>
        public int? OrderStatus
        {
            get
            {
                return m_OrderStatus;
            }
            set
            {
                base.SetValue("OrderStatus", ref m_OrderStatus, value);
            }
        }

        public string OrderStatusDesc
        {
            get
            {
                switch (OrderType)
                {
                    case PayableOrderType.PO:
                    case PayableOrderType.POAdjust:
                        return ((PurchaseOrderStatus?)this.OrderStatus).ToDescription();

                    case PayableOrderType.VendorSettleOrder:
                        return ((SettleStatus?)this.OrderStatus).ToDescription();                  

                    case PayableOrderType.RMAPOR:
                        return ((VendorRefundStatus?)this.OrderStatus).ToDescription();

                    case PayableOrderType.CollectionSettlement:
                        return ((GatherSettleStatus?)this.OrderStatus).ToDescription();

                    case PayableOrderType.Commission:
                        return ((VendorCommissionMasterStatus?)this.OrderStatus).ToDescription();
                    case PayableOrderType.GroupSettle:
                        return ((SettlementBillStatus?)this.OrderStatus).ToDescription();
                    case PayableOrderType.CostChange:
                        return ((CostChangeStatus?)this.OrderStatus).ToDescription();
                    case PayableOrderType.ConsignAdjust:
                        return ((ConsignAdjustStatus?)this.OrderStatus).ToDescription();
                    default:
                        return ((BitStatus?)this.OrderStatus).ToDescription();
                }
            }
        }

        private decimal? m_OrderAmt;
        /// <summary>
        /// 应付款
        /// </summary>
        public decimal? OrderAmt
        {
            get
            {
                return m_OrderAmt;
            }
            set
            {
                base.SetValue("OrderAmt", ref m_OrderAmt, value);
            }
        }

        private decimal? m_TotalAmt;
        /// <summary>
        /// 已生成付款单的总金额
        /// </summary>
        public decimal? TotalAmt
        {
            get
            {
                return m_TotalAmt;
            }
            set
            {
                base.SetValue("TotalAmt", ref m_TotalAmt, value);
            }
        }

        private decimal? m_PaidAmt;
        /// <summary>
        /// 已经支付的总金额
        /// </summary>
        public decimal? PaidAmt
        {
            get
            {
                return m_PaidAmt;
            }
            set
            {
                base.SetValue("PaidAmt", ref m_PaidAmt, value);
            }
        }

        private int? m_BatchNumber;
        /// <summary>
        /// 批次号
        /// </summary>
        public int? BatchNumber
        {
            get
            {
                return m_BatchNumber;
            }
            set
            {
                base.SetValue("BatchNumber", ref m_BatchNumber, value);
            }
        }

        private List<PayItemVM> m_PayItemList;
        public List<PayItemVM> PayItemList
        {
            get
            {
                return m_PayItemList.DefaultIfNull();
            }
            set
            {
                base.SetValue("PayItemList", ref m_PayItemList, value);
            }
        }
    }
}