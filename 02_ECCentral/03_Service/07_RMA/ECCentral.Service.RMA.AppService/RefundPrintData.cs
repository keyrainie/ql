using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.AppService
{
    public class RefundPrintData : IPrintDataBuild
    {
        #region IPrintDataBuild Members

        public void BuildData(NameValueCollection requestPostData, out KeyValueVariables variables, out KeyTableVariables tableVariables)
        {
            variables = new KeyValueVariables();
            tableVariables = new KeyTableVariables();

            string sysNo = requestPostData["SysNo"];
            if (!string.IsNullOrEmpty(sysNo))
            {              
                int refundSysNo;

                if (int.TryParse(sysNo, out refundSysNo))
                {                   
                    var refundInfo = GetRefundInfo(refundSysNo);
                    decimal? totalAmt = refundInfo.Items.Sum(p =>
                    {
                        if (!p.IsPoint)
                        {
                            return p.TotalAmount;
                        }
                        return 0;
                    });
                    if (refundInfo.CashAmount != null && refundInfo.CashAmount != 0)
                    {
                        variables.Add("JsFunction", "sf();");
                        variables.Add("SOSysNo", refundInfo.SOSysNo);
                        variables.Add("Now", DateTime.Now.ToShortDateString());
                        variables.Add("AuditUserName", refundInfo.AuditUserName);
                        variables.Add("ReceiveName", refundInfo.ReceiveName);
                        variables.Add("CustomerSysNo", refundInfo.CustomerSysNo);
                        variables.Add("CustomerName", refundInfo.CustomerName);
                        variables.Add("ReceiveAddress", refundInfo.ReceiveAddress);
                        variables.Add("ReceivePhone", refundInfo.ReceivePhone);
                        variables.Add("InvoiceNote", refundInfo.InvoiceNote);
                        variables.Add("PayTypeName", refundInfo.PayTypeName);
                        variables.Add("ShipTypeName", refundInfo.ShipTypeName);
                        variables.Add("RefundID", refundInfo.RefundID);
                        variables.Add("TotalAmount", ToLowerChineseMoney(totalAmt.Value));
                        variables.Add("ChineseTotalAmount", ToUpperChineseMoney(totalAmt.Value));
                    }                    
                                       
                    DataTable t = new DataTable();
                    t.Columns.Add("ProductID");
                    t.Columns.Add("ProductName");
                    t.Columns.Add("PrintPrice");
                    t.Columns.Add("PrintQuantity");
                    t.Columns.Add("PrintTotalAmount");

                    if (refundInfo.Items != null)
                    {
                        refundInfo.Items.ForEach(p =>
                        {
                            t.Rows.Add(p.ProductID, p.ProductName, p.Price, p.Quantity, p.PrintTotalAmount);
                        });
                    }

                    tableVariables.Add("RefundItems", t);                    
                }
            }
        }
        
        private RefundPrintInfo GetRefundInfo(int refundSysNo)
        {
            RefundPrintInfo refundInfo = new RefundPrintInfo();
            refundInfo.Items = new List<RefundItemPrintInfo>();
            
            var dtRefund = ObjectFactory<RefundProcessor>.Instance.GetRefundPrintDetail(refundSysNo);
            var dtRefundItems = ObjectFactory<RefundProcessor>.Instance.GetRefundPrintItems(refundSysNo);
            if (dtRefund.Rows.Count == 0)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_Refund_NotExists"));
            }
            DataMapper.AutoMap<RefundPrintInfo>(refundInfo, dtRefund.Rows[0]);
            if (dtRefundItems.Rows.Count > 0)
            {
                foreach (DataRow row in dtRefundItems.Rows)
                {
                    RefundItemPrintInfo item = new RefundItemPrintInfo();
                    DataMapper.AutoMap<RefundItemPrintInfo>(item, row);
                    refundInfo.Items.Add(item);
                }
            }

            if (refundInfo.Items != null)
            {
                refundInfo.Items.ForEach(item =>
                {
                    item.Quantity = -1;
                    item.IsPoint = false;
                    item.Price = refundInfo.RefundPayType == RefundPayType.TransferPointRefund ? 0M : (item.RefundCash ?? 0M);
                    item.TotalAmount = item.Quantity * item.Price;
                });

                if (refundInfo.DeductPointFromCurrentCash != null && refundInfo.DeductPointFromCurrentCash != 0)
                {
                    RefundItemPrintInfo printItem = new RefundItemPrintInfo()
                    {
                        ProductID = "赎回积分",
                        IsPoint = false,
                        TotalAmount = refundInfo.DeductPointFromCurrentCash.Value
                    };

                    refundInfo.Items.Add(printItem);
                }

                if (refundInfo.CompensateShipPrice != null && refundInfo.CompensateShipPrice.Value != 0)
                {
                    RefundItemPrintInfo printItem = new RefundItemPrintInfo()
                    {
                        ProductID = "补偿运费",
                        IsPoint = false,
                        TotalAmount = -1 * refundInfo.CompensateShipPrice.Value
                    };

                    refundInfo.Items.Add(printItem);
                }

                if (refundInfo.PointAmount != null && refundInfo.PointAmount.Value != 0)
                {
                    RefundItemPrintInfo printItem = new RefundItemPrintInfo()
                    {
                        ProductID = "影响积分",
                        IsPoint = true,
                        TotalAmount = refundInfo.PointAmount.Value + (refundInfo.RefundPayType == RefundPayType.TransferPointRefund ? (refundInfo.CashAmount ?? 0M) * ExternalDomainBroker.GetPointToMoneyRatio() : 0M)
                    };

                    refundInfo.Items.Add(printItem);
                }
            }

            return refundInfo;
        }

        private string ToLowerChineseMoney(decimal money)
        {
            return money.ToString("￥#0.00");
        }

        private string ToUpperChineseMoney(decimal money)
        {
            return MoneyUtility.GetChineseMoney(money);            
        }

        #endregion
    }    

    /// <summary>
    /// 此实体仅为了打印退款单用，故不放在BizEntity中，以免污染实体
    /// </summary>
    public class RefundPrintInfo
    {
        public int? SysNo { get; set; }

        public string RefundID { get; set; }

        public decimal? DeductPointFromCurrentCash { get; set; }

        public decimal? CompensateShipPrice { get; set; }

        public decimal? CashAmount { get; set; }

        public int? PointAmount { get; set; }

        public int? SOSysNo { get; set; }

        public int? CustomerSysNo { get; set; }

        public string CustomerName { get; set; }

        public string ReceiveName { get; set; }

        public string ReceiveAddress { get; set; }

        public string ReceivePhone { get; set; }

        public RMARefundStatus? Status { get; set; }

        public string AuditUserName { get; set; }

        public RefundPayType? RefundPayType { get; set; }

        public string InvoiceNote { get; set; }

        public string ShipTypeName { get; set; }

        public string PayTypeName { get; set; }

        public List<RefundItemPrintInfo> Items { get; set; }
    }

    /// <summary>
    /// 此实体仅为了打印退款单用，故不放在BizEntity中，以免污染实体
    /// </summary>
    public class RefundItemPrintInfo
    {
        public int? SysNo { get; set; }

        public decimal? RefundCash { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public bool IsPoint { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public decimal? TotalAmount { get; set; }

        public string PrintPrice
        {
            get
            {
                if (this.Price != null)
                {
                    return this.Price.Value.ToString(RMAConst.DecimalFormat);
                }

                return String.Empty;
            }
        }

        public string PrintQuantity
        {
            get
            {
                if (this.Quantity != null)
                {
                    return this.Quantity.Value.ToString();
                }

                return String.Empty;
            }
        }

        public string PrintTotalAmount
        {
            get
            {
                if (this.IsPoint)
                {
                    return String.Format("P.{0:#0}", this.TotalAmount ?? 0M);
                }
                else
                {
                    return (this.TotalAmount ?? 0M).ToString(RMAConst.DecimalFormat);
                }
            }
        }
    }
}
