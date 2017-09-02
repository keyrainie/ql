using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using System.Data;
using System.Reflection;
using ECCentral.Service.Utility;

namespace ECCentral.BizEntity.Invoice 
{
    public class FinanceExportInfo
    {
        private const string decimalFormat = "###,###,###0.00";

        public int SysNo { get; set; }

        #region 未/勾选按供应商分组
        /// <summary>
        /// 单据ID
        /// </summary>
        public int OrderID { get; set; }

        public DateTime? ETP { get; set; }

        public string ETPDisplay
        {
            get
            {
                return this.ETP.HasValue ? this.ETP.Value.ToShortDateString() : string.Empty;
            }
        }

        /// <summary>
        /// 批次号
        /// </summary>
        public int BatchNumber { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public PayableOrderType? OrderType { get; set; }

        public string OrderTypeDisplay
        {
            get
            {
                if (OrderType.HasValue)
                {
                    return EnumHelper.GetDescription(OrderType.Value);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 单据时间
        /// </summary>
        public DateTime? OrderDate { get; set; }
        public string OrderDateDisplay
        {
            get
            {
                if (OrderDate != null)
                {
                    return OrderDate.Value.ToLongDateString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 应付
        /// </summary>
        public decimal PayableAmt { get; set; }

        public string PayableAmtDisplay
        {
            get
            {
                return PayableAmt.ToString(decimalFormat);
            }
        }

        public decimal OrderAmt { get; set; }

        public string OrderAmtDisplay
        {
            get
            {
                return this.OrderAmt.ToString(decimalFormat);
            }
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 供应商帐期
        /// </summary>
        public string VendorPayTypeName { get; set; }

        public string VendorPayTypeDisplay
        {
            get
            {
                return VendorPayTypeName;
                //if (VendorPayType.HasValue)
                //{
                //    return EnumHelper.GetDescription(VendorPayType.Value);
                //}
                //return string.Empty;
            }
        }
        /// <summary>
        /// 归属PMSysNo
        /// </summary>
        public int PMUserNo { get; set; }

        /// <summary>
        /// 归属PM
        /// </summary>
        public string PMName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderStatus { get; set; }

        public string OrderStatusDisplay
        {
            get
            {
                return GetOrderStatusDescription(OrderStatus, OrderType);
            }
        }

        /// <summary>
        /// 发票状态
        /// </summary>
        public PayableInvoiceStatus InvoiceStatus { get; set; }

        public string InvoiceStatusDisplay
        {
            get
            {
                return EnumHelper.GetDescription((PayableInvoiceStatus)InvoiceStatus);
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public string CreateTimeDisplay
        {
            get
            {
                if (CreateTime != null)
                {
                    return CreateTime.Value.ToShortDateString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public PayableAuditStatus? AuditStatus { get; set; }

        public string AuditStatusDisplay
        {
            get
            {
                return EnumHelper.GetDescription(AuditStatus);
            }
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        public string AuditTimeDisplay
        {
            get
            {
                return AuditTime.HasValue ? AuditTime.Value.ToString("yyyy-MM-dd") : string.Empty;
            }
        }

        public decimal ZXAmt { get; set; }

        public string ZXAmtDisplay
        {
            get
            {
                return ZXAmt.ToString(decimalFormat);
            }
        }

        public decimal KCAmt { get; set; }

        public string KCAmtDisplay
        {
            get
            {
                return KCAmt.ToString(decimalFormat);
            }
        }

        public int RMACount { get; set; }

        /// <summary>
        /// Memo
        /// </summary>
        public string Memo { get; set; }

        public string NewMemo { get; set; }

        /// <summary>
        /// 扩展字段，用于辅助分段审核时存放前端的验证信息
        /// </summary>
        public string Tag { get; set; }
        #endregion

        #region 勾选按供应商分组特有的

        public string AccountID { get; set; }

        public string BankName { get; set; }

        public decimal NotDEIMSAmt { get; set; }
        public string NotDEIMSAmtString { get { return NotDEIMSAmt.ToString(decimalFormat); } set { } }

        public decimal PayAmtUndue { get; set; }
        public string PayAmtUndueString { get { return PayAmtUndue.ToString(decimalFormat); } set { } }

        public decimal PayAmtMature { get; set; }
        public string PayAmtMatureString { get { return PayAmtMature.ToString(decimalFormat); } set { } }

        public decimal PayAmtLocked { get; set; }
        public string PayAmtLockedString { get { return PayAmtLocked.ToString(decimalFormat); } set { } }

        public decimal PayAmtLeft { get; set; }
        public string PayAmtLeftString { get { return PayAmtLeft.ToString(decimalFormat); } set { } }

        public string C1Name { get; set; }
        public string C1NameStr { get { return null == C1Name || string.Empty == C1Name ? "" : C1Name.Substring(0, C1Name.Length - 1); } set { } }

        public VendorConsignFlag? IsConsign { get; set; }

        public string IsConsignDisplay
        {
            get
            {
                if (IsConsign.HasValue)
                {
                    return EnumHelper.GetDescription(IsConsign);
                }
                return string.Empty;
            }
        }

        public string DetailOrderSysNo { get; set; }
        public string DetailOrderSysNoStr { get { return null == DetailOrderSysNo || string.Empty == DetailOrderSysNo ? "" : DetailOrderSysNo.Substring(0, DetailOrderSysNo.Length - 1); } set { } }
        ///// <summary>
        ///// 采购单扣减
        ///// </summary>
        //public decimal R2 { get; set; }

        //public string R2Display
        //{
        //    get
        //    {
        //        return R2.ToString(decimalFormat);
        //    }
        //}

        ///// <summary>
        ///// 帐扣
        ///// </summary>
        //public decimal R4 { get; set; }

        //public string R4Display
        //{
        //    get
        //    {
        //        return R4.ToString(decimalFormat);
        //    }
        //}
        ///// <summary>
        ///// 代销结算单扣减
        ///// </summary>
        //public decimal R3 { get; set; }

        //public string R3Display
        //{
        //    get
        //    {
        //        return R3.ToString(decimalFormat);
        //    }
        //}
        ///// <summary>
        ///// 票扣
        ///// </summary>
        //public decimal R0 { get; set; }

        //public string R0Display
        //{
        //    get
        //    {
        //        return R0.ToString(decimalFormat);
        //    }
        //}
        /// <summary>
        /// 现金
        /// </summary>
        public decimal Cash { get; set; }

        public string CashDisplay
        {
            get
            {
                return Cash.ToString(decimalFormat);
            }
        }
        /// <summary>
        /// 总记录条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// TotalPayableAmt
        /// </summary>
        public decimal TotalPayableAmt { get; set; }

        public string TotalPayableAmtDisplay
        {
            get
            {
                return TotalPayableAmt.ToString(decimalFormat);
            }
        }

        public decimal PendingInvoiceAmount { get; set; }
        public string PendingInvoiceAmountDisplay
        {
            get
            {
                return PendingInvoiceAmount.ToString(decimalFormat);
            }
        }

        public decimal EndBalanceAccrued { get; set; }
        public string EndBalanceAccruedDisplay
        {
            get
            {
                return EndBalanceAccrued.ToString(decimalFormat);
            }
        }

        public decimal ReceiveByPO { get; set; }

        public string ReceiveByPODisplay
        {
            get
            {
                return this.ReceiveByPO.ToString(decimalFormat);
            }
        }

        public decimal ReceiveByAcct { get; set; }

        public string ReceiveByAcctDisplay
        {
            get
            {
                return this.ReceiveByAcct.ToString(decimalFormat);
            }
        }

        public decimal ReceiveByConsign { get; set; }

        public string ReceiveByConsignDisplay
        {
            get
            {
                return this.ReceiveByConsign.ToString(decimalFormat);
            }
        }

        public decimal ReceiveByInvoice { get; set; }

        public string ReceiveByInvoiceDisplay
        {
            get
            {
                return this.ReceiveByInvoice.ToString(decimalFormat);
            }
        }
        #endregion

        public bool IsCheck
        {
            get;
            set;
        }

        #region 合计已到应付
        public double TotalPayAmt { get; set; }

        public string TotalPayAmtDisplay
        {
            get
            {
                return TotalPayAmt.ToString(decimalFormat);
            }
        }
        #endregion

        public string OrderIDDisplay
        {
            get
            {
                switch (this.OrderType)
                {
                    case PayableOrderType.PO:                   
                        return string.Format("{0}-{1}", this.OrderID, this.BatchNumber.ToString().PadLeft(2, '0'));
                    default:
                        return this.OrderID.ToString();
                }
            }
        }

        private string GetOrderStatusDescription(int orderStatus, PayableOrderType? orderType)
        {
            string st = orderStatus.ToString();
            switch (orderType)
            {
                case PayableOrderType.PO:
                case PayableOrderType.POAdjust:
                    st = EnumHelper.GetDescription((PurchaseOrderStatus)orderStatus);
                    break;
                case PayableOrderType.VendorSettleOrder:
                    st = EnumHelper.GetDescription((SettleStatus)orderStatus);
                    break;              
                case PayableOrderType.RMAPOR:
                    st = EnumHelper.GetDescription((VendorRefundStatus)orderStatus);
                    break;
                case PayableOrderType.CollectionSettlement:
                    st = EnumHelper.GetDescription((GatherSettleStatus)orderStatus);
                    break;
                case PayableOrderType.Commission:
                    st = EnumHelper.GetDescription((VendorCommissionMasterStatus)orderStatus);
                    break;
                case PayableOrderType.CollectionPayment:
                    st = EnumHelper.GetDescription((POCollectionPaymentSettleStatus)orderStatus);
                    break;
                default:
                    break;
            }
            return st;
        }


        public static FinanceExportInfo Create(DataRow row)
        {
            if (row == null)
            {
                return null;
            }
            FinanceExportInfo entity = new FinanceExportInfo();
            //Type type = typeof(FinanceExportInfo);
            //PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //string name = string.Empty;
            //for (int i = 0; i < properties.Length; i++)
            //{
            //    PropertyInfo property = properties[i];
            //    name = property.Name;
            //    if (property.CanWrite
            //        && row.Table.Columns.Contains(name))
            //    {
            //        property.SetValue(entity, row[name], null);
            //    }
            //}
            object value;
            var columns = row.Table.Columns;
            if (columns.Contains("SysNo"))
            {
                value = row["SysNo"];
                entity.SysNo = value.ToInteger();
            }
            if (columns.Contains("OrderID"))
            {
                value = row["OrderID"];
                entity.OrderID = value.ToInteger();
            }
            if (columns.Contains("BatchNumber"))
            {
                value = row["BatchNumber"];
                entity.BatchNumber = value.ToInteger();
            }
            if (columns.Contains("OrderType"))
            {
                value = row["OrderType"];
                if (value != null
                    && value != DBNull.Value)
                {
                    entity.OrderType = (PayableOrderType)value.ToInteger();
                }
            }
            if (columns.Contains("CreateTime"))
            {
                value = row["CreateTime"];
                if (value != null
                    && value != DBNull.Value)
                {
                    entity.CreateTime = Convert.ToDateTime(value);
                }
            }
            if (columns.Contains("VendorSysNo"))
            {
                value = row["VendorSysNo"];
                if (value != null
                    && value != DBNull.Value)
                {
                    entity.VendorSysNo = value.ToInteger();
                }
            }
            if (columns.Contains("VendorName"))
            {
                entity.VendorName = row["VendorName"].ToString();
            }
            if (columns.Contains("PMName"))
            {
                entity.PMName = row["PMName"].ToString();
            }
            if (columns.Contains("ETP"))
            {
                value = row["ETP"];
                if (value != null
                    && value != DBNull.Value)
                {
                    entity.ETP = Convert.ToDateTime(value);
                }
            }
            if (columns.Contains("InvoiceStatus"))
            {
                value = row["InvoiceStatus"];
                if (value != null
                    && value != DBNull.Value)
                {
                    entity.InvoiceStatus = (PayableInvoiceStatus)value.ToInteger();
                }
            }
            if (columns.Contains("OrderAmt"))
            {
                entity.OrderAmt = row["OrderAmt"].ToDecimal();
            }

            if (columns.Contains("IsConsign"))
            {
                value = row["IsConsign"];
                if (value != null
                    && value != DBNull.Value)
                {
                    entity.IsConsign = (VendorConsignFlag)value.ToInteger();
                }
            }
            if (columns.Contains("AccountID"))
            {
                entity.AccountID = row["AccountID"].ToString();
            }
            if (columns.Contains("BankName"))
            {
                entity.BankName = row["BankName"].ToString();
            }
            if (columns.Contains("PayAmtMature"))
            {
                entity.PayAmtMature = row["PayAmtMature"].ToDecimal();
            }
            if (columns.Contains("VendorPayType"))
            {
                entity.VendorPayTypeName = row["VendorPayType"].ToString();
            }
            if (columns.Contains("C1Name"))
            {
                entity.C1Name = row["C1Name"].ToString();
            }
            if (columns.Contains("KCAmt"))
            {
                entity.KCAmt = row["KCAmt"].ToDecimal();
            }
            if (columns.Contains("PayAmtLeft"))
            {
                entity.PayAmtLeft = row["PayAmtLeft"].ToDecimal();
            }
            if (columns.Contains("ZXAmt"))
            {
                entity.ZXAmt = row["ZXAmt"].ToDecimal();
            }
            if (columns.Contains("R4"))
            {
                entity.ReceiveByInvoice = row["R4"].ToDecimal();
            }
            if (columns.Contains("R2"))
            {
                entity.ReceiveByAcct = row["R2"].ToDecimal();
            }
            if (columns.Contains("R0"))
            {
                entity.ReceiveByPO = row["R0"].ToDecimal();
            }
            if (columns.Contains("R3"))
            {
                entity.ReceiveByConsign = row["R3"].ToDecimal();
            }
            if (columns.Contains("R1"))
            {
                entity.Cash = row["R1"].ToDecimal();
            }
            if (columns.Contains("DetailOrderSysNo"))
            {
                entity.DetailOrderSysNo = row["DetailOrderSysNo"].ToString();
            }
            if (columns.Contains("PendingInvoiceAmount"))
            {
                entity.PendingInvoiceAmount = row["PendingInvoiceAmount"].ToDecimal();
            }
            if (columns.Contains("EndBalanceAccrued"))
            {
                entity.EndBalanceAccrued = row["EndBalanceAccrued"].ToDecimal();
            }

            return entity;
        }

    }
}
