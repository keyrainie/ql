using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class PayableVM : ModelBase
    {

        public int? PaySysNo
        {
            get;
            set;
        }

        public int? OrderSysNo
        {
            get;
            set;
        }

        public string OrderID
        {
            get;
            set;
        }

        public int? BatchNumber
        {
            get;
            set;
        }

        public PayableOrderType? OrderType
        {
            get;
            set;
        }

        public DateTime? CreateDate
        {
            get;
            set;
        }

        public decimal? PayableAmt
        {
            get;
            set;
        }

        public decimal? AlreadyPayAmt
        {
            get;
            set;
        }

        public int? CurrencySysNo
        {
            get;
            set;
        }

        public string Currency
        {
            get;
            set;
        }

        public string VendorName
        {
            get;
            set;
        }

        public int? VendorSysNo
        {
            get;
            set;
        }

        public int? PayPeriodType
        {
            get;
            set;
        }

        public int? PayTypeSysNo
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string POBelongPMName
        {
            get;
            set;
        }

        public ECCentral.BizEntity.PO.PurchaseOrderStatus? OrderStatus
        {
            get;
            set;
        }

        public PayableStatus? PayStatus
        {
            get;
            set;
        }

        public PayableInvoiceStatus? InvoiceStatus
        {
            get;
            set;
        }

        public PayableInvoiceFactStatus? InvoiceFactStatus
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string POMemo
        {
            get;
            set;
        }

        public string Bank
        {
            get;
            set;
        }

        public string Account
        {
            get;
            set;
        }

        public decimal? ReturnPoint
        {
            get;
            set;
        }

        public string WarehouseName
        {
            get;
            set;
        }

        public int WarehouseNumber
        {
            get;
            set;
        }

        public string InvoiceNumber
        {
            get;
            set;
        }

        public PayItemStyle? PayStyle
        {
            get;
            set;
        }

        public SapImportedStatus? SapImportedStatus
        {
            get;
            set;
        }//   -- 导入状态

        public DateTime? SAPPostDate
        {
            get;
            set;
        }// --导入时间

        public string SAPDocNo
        {
            get;
            set;
        }//    --导入凭证号

        public string SapInFailedReason
        {
            get;
            set;
        } //失败原因

        public DateTime? EGP
        {
            get;
            set;
        }

        public DateTime? ETP
        {
            get;
            set;
        }

        public decimal? InstockAmt
        {
            get;
            set;
        }

        public int? EIMSNo
        {
            get;
            set;
        }

        public string EIMSNoList
        {
            get;
            set;
        }

        public Visibility IsShowFailedDetail
        {
            get
            {
                return this.SapImportedStatus == ECCentral.BizEntity.Invoice.SapImportedStatus.Fault ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IsShowDetail
        {
            get
            {
                return this.SapImportedStatus == ECCentral.BizEntity.Invoice.SapImportedStatus.Fault ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// 产出返点
        /// </summary>
        public decimal? PRDPoint
        {
            get;
            set;
        }

        public bool? IsOldConsignSettle
        {
            get;
            set;
        }

        public int SysNo { get; set; }
        public string InvoiceStatusDesc { get; set; }
        public string InvoiceFactStatusDesc { get; set; }
        public string UpdateInvoiceUserName { get; set; }
        public string InvoiceUpdate { get; set; }
        public string extendSysNo { get; set; }

        public int PMSysNo { get; set; }
        public DateTime? AuditDatetime { get; set; }
        public string AuditStatus { get; set; }
        public int AuditUserSysNo { get; set; }
        public string Memo { get; set; }
        public string NewMemo { get; set; }

        public string Tag { get; set; }
        public string PaySettleCompany { get; set; }
        public DateTime? InStockTime { get; set; }


        /// <summary>
        /// 当前操作员名称
        /// </summary>
        public string OperationUserFullName
        {
            get;
            set;
        }

        public string CompanyCode
        { 
            get;
            set; 
        }

        #region 扩展字段，供UI显示用

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref isChecked, value);
            }
        }

        public string Operation
        {
            get
            {
                if (PayableAmt - AlreadyPayAmt != 0)
                {
                    if (this.OrderType == PayableOrderType.PO)
                    {
                        if (this.OrderStatus != ECCentral.BizEntity.PO.PurchaseOrderStatus.Origin
                            && this.OrderStatus != ECCentral.BizEntity.PO.PurchaseOrderStatus.Abandoned)
                        {
                            return "NewPay";
                        }
                    }
                    else if (this.OrderType == PayableOrderType.VendorSettleOrder)
                    {
                        if (this.OrderStatus == ECCentral.BizEntity.PO.PurchaseOrderStatus.WaitingInStock)
                        {
                            return "NewPay";
                        }
                    }
                    else if (this.OrderType == PayableOrderType.CollectionSettlement)
                    {
                        if (this.OrderStatus == ECCentral.BizEntity.PO.PurchaseOrderStatus.InStocked)
                        {
                            return "NewPay";
                        }
                    }
                    else 
                    {
                        return "NewPay";
                    }
                }
                return "";
            }
        }

        public string OrderIDDesc
        {
            get
            {
                string batchnum = this.BatchNumber.HasValue ? "-" + this.BatchNumber.Value.ToString().PadLeft(2, '0') : string.Empty;
                switch (this.OrderType)
                {
                    case PayableOrderType.PO:
                        return this.OrderID + batchnum;
                    case PayableOrderType.POAdjust:
                        return this.OrderID + "A";
                    //case PayableOrderType.SubInvoice:
                    //case PayableOrderType.SubAccount:
                    //case PayableOrderType.ReturnPointCashAdjust:
                    //    return this.OrderID + batchnum;
                    default:
                        return this.OrderID;
                }
            }
        }

        public decimal? Remainder
        {
            get
            {
                return PayableAmt - AlreadyPayAmt;
            }
        }

        public string PaySettleCompanyStr
        {
            get
            {
                return ECCentral.Portal.Basic.Utilities.EnumConverter.GetDescription(this.PaySettleCompany, typeof(ECCentral.BizEntity.PO.PaySettleCompany));
            }
        }

        #endregion 扩展字段，供UI显示用
    }
}