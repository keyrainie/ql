using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class EnergySubsidyVM : ModelBase
    {
        #region 基本属性
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

        /// <summary>
        /// 详细信息查看类型(新增时计算补贴金额需要判断节能补贴商品信息的有效性)
        /// </summary>
        public string ViewType { get; set; }

        /// <summary>
        /// 查询类型（查询节能补贴基本信息|查询节能补贴商品信息）
        /// </summary>
        public string QueryType { get; set; }

        public string CompanyCode { get; set; }

        #endregion

        #region 界面展示转换

        public const string MoneyFormat = "#####0.00";

        /// <summary>
        /// 购买方式显示
        /// </summary>
        public string BuyType
        {
            get
            {
                if (CertificateType == 0)
                    return "个人";
                else if (CertificateType == 1)
                    return "公司";
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 订单金额显示
        /// </summary>
        public string OrderAmtStr
        {
            get
            {
                if (OrderAmt != null)
                    return OrderAmt.Value.ToString(MoneyFormat) + "元";
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 节能补贴金额显示
        /// </summary>
        public string AmountStr
        {
            get
            {
                if (Amount != null)
                    return Amount.Value.ToString(MoneyFormat) + "元";
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 使用积分显示
        /// </summary>
        public string PointUsedStr
        {
            get
            {
                return PointUsed.ToString() + "积分";
            }
        }

        /// <summary>
        /// 优惠券优惠显示
        /// </summary>
        public string PromotionStr
        {
            get
            {
                if (Promotion != null)
                    return Promotion.Value.ToString(MoneyFormat) + "元";
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 收货人显示类型
        /// </summary>
        public string ReceiveType
        {
            get
            {
                if (CertificateType == 0)
                    return ResRefundAdjust.Label_ReceiveName;
                else
                    return ResRefundAdjust.Label_CompanyName;
            }
        }

        public string CardIDStr
        {
            get
            {
                if (CertificateType == 0)
                    return CardID;
                else
                    return ReceiveName;
            }
        }
        #endregion

    }
}
