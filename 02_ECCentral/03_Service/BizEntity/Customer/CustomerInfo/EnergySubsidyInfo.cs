using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 节能补贴信息
    /// </summary>
    public class EnergySubsidyInfo : IIdentity, ICompany
    {
        public int? SysNo { get; set; }

        public int? SOSysNo { get; set; }

        /// <summary>
        /// 顾客登陆ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 节能补贴金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardID { get; set; }

        /// <summary>
        /// 类型 0个人；1公司
        /// </summary>
        public int CertificateType { get; set; }

        /// <summary>
        /// 收货人信息
        /// </summary>
        public string ReceiveName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 顾客地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal? OrderAmt { get; set; }

        /// <summary>
        /// 使用积分
        /// </summary>
        public int? PointUsed { get; set; }

        /// <summary>
        /// 优惠券优惠
        /// </summary>
        public decimal? Promotion { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string VendorID { get; set; }

        /// <summary>
        /// 单个商品的补贴金额
        /// </summary>
        public decimal Acount { get; set; }

        /// <summary>
        /// 同一类型补贴商品的购买数量
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 分行
        /// </summary>
        public string BranchBank { get; set; }

        /// <summary>
        /// 账号/卡号
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 详细信息查看类型(新增时计算补贴金额需要判断节能补贴商品信息的有效性)
        /// </summary>
        public string ViewType { get; set; }

        /// <summary>
        /// 查询类型（查询节能补贴基本信息|查询节能补贴商品信息）
        /// </summary>
        public string QueryType { get; set; }

        public string CompanyCode { get; set; }
    }
}
