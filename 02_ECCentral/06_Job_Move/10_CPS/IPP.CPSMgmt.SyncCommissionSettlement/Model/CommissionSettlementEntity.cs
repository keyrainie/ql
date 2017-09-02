using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model
{
    /// <summary>
    ///单据查询 
    /// </summary>
    [Serializable]
    public class CommissionSettlementEntity
    {
        #region Table Field 对应表字段
        /// <summary>
        /// 获取或设置单据编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }


        [DataMapping("UserSysNo", DbType.Int32)]
        public int UserSysNo { get; set; }

        [DataMapping("SettledBeginDate", DbType.DateTime)]
        public DateTime SettledBeginDate { get; set; }

        [DataMapping("SettledEndDate", DbType.DateTime)]
        public DateTime SettledEndDate { get; set; }

        /// <summary>
        /// 获取或设置结算状态
        /// </summary>
        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }


        /// <summary>
        /// 获取或设置结算时间
        /// </summary>
        [DataMapping("SettledTime", DbType.DateTime)]
        public DateTime? SettledTime { get; set; }

        /// <summary>
        /// 获取或设置付款时间
        /// </summary>
        [DataMapping("PaidTime", DbType.DateTime)]
        public DateTime? PaidTime { get; set; }

        /// <summary>
        /// 获取或设置结算金额【Job结算金额】
        /// </summary>
        [DataMapping("CommissionAmt", DbType.Decimal)]
        public decimal? CommissionAmt { get; set; }

        /// <summary>
        /// 获取或设置确认金额【人工结算金额】
        /// </summary>
        [DataMapping("ConfirmCommissionAmt", DbType.Decimal)]
        public decimal? ConfirmCommissionAmt { get; set; }
        #endregion Table Field 对应表字段

        #region Common Field 公用字段
        /// <summary>
        /// 获取或设置创建人
        /// </summary>
        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        /// <summary>
        /// 获取或设置创建时间
        /// </summary>
        [DataMapping("InDate", DbType.DateTime)]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 获取或设置编辑人
        /// </summary>
        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        /// <summary>
        /// 获取或设置编辑时间
        /// </summary>
        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 获取或设置公司代码
        /// </summary>
        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 获取或设置公司代码
        /// </summary>
        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 获取或设置语言编码
        /// </summary>
        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }
        #endregion Common Field
    }
}
