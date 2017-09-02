using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.RMA.Restful.ResponseMsg
{
    public class RegisterDetailInfoRsp
    {
        public RMARegisterInfo Register { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public CustomerRank? CustomerRank { get; set; }        
        public int? SOSysNo { get; set; }
        public int? RequestSysNo { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public ProcessType ProcessType { get; set; }
        public InvoiceType? InvoiceType { get; set; }
        public int? RefundSysNo { get; set; }
        /// <summary>
        /// 商家信息
        /// </summary>
        public string BusinessModel { get; set; }
        /// <summary>
        /// 单件产品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 单件
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 发货产品系统编号
        /// </summary>
        public string RevertProductID { get; set; }

        public List<ProductInventoryInfo> ProductInventoryInfo { get; set; }

        public ProductInventoryType InventoryType { get; set; }
    }
}
