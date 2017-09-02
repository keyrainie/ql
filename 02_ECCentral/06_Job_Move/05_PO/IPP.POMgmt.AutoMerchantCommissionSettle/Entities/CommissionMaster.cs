using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace MerchantCommissionSettle.Entities
{
    public sealed class CommissionMaster
    {
        /// <summary>
        /// 佣金规则系统编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        [DataMapping("MerchantSysNo", DbType.Int32)]
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        /// <summary>
        /// 单据金额
        /// </summary>
        [DataMapping("TotalAmt", DbType.Decimal)]
        public decimal TotalAmt { get; set; }

        /// <summary>
        /// 店租费
        /// </summary>
        [DataMapping("RentFee",DbType.Decimal)]
        public decimal RentFee { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        [DataMapping("DeliveryFee", DbType.Decimal)]
        public decimal DeliveryFee { get; set; }

        /// <summary>
        /// 销售提成
        /// </summary>
        [DataMapping("SalesCommissionFee", DbType.Decimal)]
        public decimal SalesCommissionFee { get; set; }

        /// <summary>
        /// 订单提成
        /// </summary>
        [DataMapping("OrderCommissionFee", DbType.Decimal)]
        public decimal OrderCommissionFee { get; set; }

        /// <summary>
        /// 单据起始时间
        /// </summary>
        [DataMapping("BeginDate", DbType.DateTime)]
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 单据终止时间
        /// </summary>
        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 创建日期时间
        /// </summary>
        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        [DataMapping("CloseDate", DbType.DateTime)]
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// 关闭用户
        /// </summary>
        [DataMapping("CloseUser", DbType.String)]
        public string CloseUser { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 公司代码 StoreCompanyCode
        /// </summary>
        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        [DataMapping("CurrencyCode", DbType.String)]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 语言代码
        /// </summary>
        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 店租佣金比例,根据账期计算获得
        /// </summary>
        public decimal Percentage { get; set; }
    }
}
