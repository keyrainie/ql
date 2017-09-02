using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Product
{
    [Serializable]
    public class ProductPurchaseQueryBasicInfo
    {
        public int sysno { get; set; }
        public string poid { get; set; }
        public int VendorSysNo { get; set; }
        public string vendorname { get; set; }
        public string stockname { get; set; }
        public string ITStockName { get; set; }
        public string createname { get; set; }
        public DateTime? createtime { get; set; }
        public string auditname { get; set; }
        public DateTime? audittime { get; set; }
        public string inname { get; set; }
        public DateTime? intime { get; set; }
        public PurchaseOrderStatus status { get; set; }
        public string etp { get; set; }
        public int ComfirmUserSysNo { get; set; }
        public DateTime ComfirmTime { get; set; }
        public string ComfirmUser { get; set; }
        public DateTime? PrintTime { get; set; }
        public bool HasSendMail { get; set; }
        public string MailAddress { get; set; }
        public string Email { get; set; }
        public int StockSysNo { get; set; }
        public int ITStockSysNo { get; set; }
        public DateTime? MaxTastDate { get; set; }
        public int PreEimsCount { get; set; }
        public int QtyEimsCount { get; set; }
        public int PreCount { get; set; }
        public int QtyCount { get; set; }
        public int AllPurchaseQty { get; set; }
        public int AllQuantity { get; set; }
        public int EIMSNo { get; set; }
        public string Source { get; set; }
        public string CloseUser { get; set; }
        public DateTime? CloseTime { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ApportionTime { get; set; }
        public int ApportionUserSysNo { get; set; }
        public string ApportionUserName { get; set; }
        public decimal EIMSAmt { get; set; }
        public decimal UsedEIMSAmt { get; set; }
        public string EIMSNoList { get; set; }
        public string PaySettleCompany { get; set; }
        public string LeaseFlag { get; set; }
        public int RowNumber { get; set; }
        public int AllPrePurchaseQty { get; set; }
        public string StatusName
        {
            get
            {
                return EnumHelper.GetDescription(status);
            }
        }
    }
}
