using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    /// <summary>
    /// 销售收款单查询过滤条件
    /// </summary>
    public class SOIncomeQueryFilter
    {
        public int? SysNo
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }

        public string CustomerSysNo
        {
            get;
            set;
        }

        public SOIncomeOrderType? OrderType
        {
            get;
            set;
        }

        public DateTime? CreateDateFrom
        {
            get;
            set;
        }

        public DateTime? CreateDateTo
        {
            get;
            set;
        }

        public DateTime? ConfirmDateFrom
        {
            get;
            set;
        }

        public DateTime? ConfirmDateTo
        {
            get;
            set;
        }

        public DateTime? SOOutDateFrom
        {
            get;
            set;
        }

        public DateTime? SOOutDateTo
        {
            get;
            set;
        }

        public DateTime? RORefundDateFrom
        {
            get;
            set;
        }

        public DateTime? RORefundDateTo
        {
            get;
            set;
        }

        public SOIncomeOrderStyle? IncomeType
        {
            get;
            set;
        }

        public SOIncomeStatus? IncomeStatus
        {
            get;
            set;
        }

        public List<SOIncomeStatus> InIncomeStatusList
        {
            get;
            set;
        }

        public string PayTypeSysNo
        {
            get;
            set;
        }

        public string ShipTypeSysNo
        {
            get;
            set;
        }

        public string IncomeConfirmer
        {
            get;
            set;
        }

        public string FreightMen
        {
            get;
            set;
        }

        public string OrderSysNo
        {
            get;
            set;
        }

        public bool IsAudit
        {
            get;
            set;
        }

        public string BankName
        {
            get;
            set;
        }

        public string PostAddress
        {
            get;
            set;
        }

        public string ReferenceID
        {
            get;
            set;
        }

        public bool IsIncludeShipInfo
        {
            get;
            set;
        }

        public bool IsIncludeReturnInfo
        {
            get;
            set;
        }

        public bool IsCheckShip
        {
            get;
            set;
        }

        public bool IsDifference
        {
            get;
            set;
        }

        public SOStatus? SOOutStatus
        {
            get;
            set;
        }

        public bool IsCash
        {
            get;
            set;
        }

        public bool IsTotal
        {
            get;
            set;
        }

        public bool IsException
        {
            get;
            set;
        }

        public string WarehouseNumber
        {
            get;
            set;
        }

        public bool IsCombine
        {
            get;
            set;
        }

        public bool IsByWareHourse
        {
            get;
            set;
        }

        public string ByCustomer
        {
            get;
            set;
        }

        public decimal? IncomeAmt
        {
            get;
            set;
        }

        public decimal? PrepayAmt
        {
            get;
            set;
        }

        public OperationSignType IncomeAmtOper
        {
            get;
            set;
        }

        public OperationSignType PrepayAmtOper
        {
            get;
            set;
        }

        public CashOnDeliveryType? CashOnDeliveryType { get; set; }

        public string SapCoCode
        {
            get;
            set;
        }

        public bool IsRelated
        {
            get;
            set;
        }

        public bool IsSalesOrder
        {
            get;
            set;
        }

        public bool IsGiftCard
        {
            get;
            set;
        }

        //网关收款时间起止
        public DateTime? PayedDateFrom
        {
            get;
            set;
        }

        public DateTime? PayedDateTo
        {
            get;
            set;
        }

        public bool IsNetPay
        {
            get;
            set;
        }

        public RefundPayType? RMARefundPayType
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        #region 分页信息

        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        #endregion 分页信息
    }
}