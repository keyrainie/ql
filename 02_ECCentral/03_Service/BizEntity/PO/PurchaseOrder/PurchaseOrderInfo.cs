using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购单
    /// </summary>
    public class PurchaseOrderInfo : IIdentity, ICompany
    {

        public PurchaseOrderInfo()
        {
            PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo() { Privilege = new List<PurchaseOrderPrivilege>(), ProductManager = new IM.ProductManagerInfo(), ETATimeInfo = new PurchaseOrderETATimeInfo(), MemoInfo = new PurchaseOrderMemoInfo() };
            VendorInfo = new VendorInfo();
            POItems = new List<PurchaseOrderItemInfo>();
            ReceivedInfoList = new List<PurchaseOrderReceivedInfo>();
            EIMSInfo = new PurchaseOrderEIMSInfo() { EIMSInfoList = new List<EIMSInfo>() };
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 采购单基本信息
        /// </summary>
        public PurchaseOrderBasicInfo PurchaseOrderBasicInfo { get; set; }

        /// <summary>
        /// 采购单供应商信息
        /// </summary>
        public VendorInfo VendorInfo { get; set; }

        /// <summary>
        /// 采购单商品列表
        /// </summary>
        public List<PurchaseOrderItemInfo> POItems { get; set; }

        /// <summary>
        /// 采购单收货信息
        /// </summary>
        public List<PurchaseOrderReceivedInfo> ReceivedInfoList { get; set; }

        /// <summary>
        /// 采购单返点信息
        /// </summary>
        public PurchaseOrderEIMSInfo EIMSInfo { get; set; }

        /// <summary>
        /// 已扣减返点
        /// </summary>
        public decimal? UsedEIMSTotal { get; set; }
    }
}
