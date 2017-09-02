using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class VendorQueryFilter
    {

        public VendorQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }
        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public VendorStatus? Status { get; set; }
        public int? VendorType { get; set; }
        public string RANK { get; set; }
        //等级状态
        public VendorRankStatus? RANKStatus { get; set; }

        //代理品牌
        public string ManufacturerSysNo { get; set; }
        public string ManufacturerName { get; set; }
       //付款结算公司
        public PaySettleCompany? PaySettleCompany { get; set; }
        //代理级别
        public string AgentLevel { get; set; }
        //财务审核状态
        public VendorFinanceRequestStatus? RequestStatus { get; set; }
        //代理类别
        public string C1SysNo { get; set; }
        public string C2SysNo { get; set; }
        public string C3SysNo { get; set; }
        //合作到期时间从
        public DateTime? ExpiredDateFrom { get; set; }
        //合作到期时间到
        public DateTime? ExpiredDateTo { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 是否为代销
        /// </summary>
        public VendorConsignFlag? IsConsign { get; set; }

        /// <summary>
        /// 开票方式
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// 仓储方式
        /// </summary>
        public string StockType { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public string ShippingType { get; set; }
        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

        public int? EPortSysNo { get; set; }
    }
}
