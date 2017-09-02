using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model
{
    /// <summary>
    /// 佣金结算单Item实体 【单据SO&RMA】
    /// </summary>
    [Serializable]
    public class CommissionSettlementItemEntity
    {
        #region Table Field 对应表字段

        /// <summary>
        /// \
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        /// <summary>
        /// 获取或设置SO或RMA对应的结算单
        /// </summary>
        [DataMapping("CommissionSettlementSysNo", DbType.Int32)]
        public int? CommissionSettlementSysNo { get; set; }

        /// <summary>
        /// 获取或设置结算Item 对应的单据编号
        /// </summary>
        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo { get; set; }

        /// <summary>
        /// 获取或设置结算Item状态【U：未结算 S：已结算 P：已支付】
        /// </summary>
        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        /// <summary>
        /// 获取或设置结算Item类型单据时间【出库、RMA申请时间】
        /// </summary>
        [DataMapping("OrderDate", DbType.DateTime)]
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// 获取或设置结算Item金额【SO 或 RMA金额】
        /// </summary>
        [DataMapping("OrderAmt", DbType.Decimal)]
        public decimal OrderAmt { get; set; }

        /// <summary>
        /// 获取或设置结算Item类型 SO or RMA
        /// </summary>
        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }


        /// <summary>
        /// 获取或设置结算Item的对应的联盟用户编号
        /// </summary>
        [DataMapping("UserSysNo", DbType.Int32)]
        public int? UserSysNo { get; set; }

        /// <summary>
        /// 获取或设置结算Item的频道编号
        /// </summary>
        [DataMapping("ChannelSysNo", DbType.Int32)]
        public int ChannelSysNo { get; set; }

        /// <summary>
        /// 获取或设置佣金金额
        /// </summary>
        [DataMapping("CommissionAmt", DbType.Int32)]
        public decimal CommissionAmt { get; set; }

        /// <summary>
        /// 获取或设置同步类型：Create , Update
        /// </summary>
        [DataMapping("SyncType", DbType.String)]
        public string SyncType { get; set; }

        /// <summary>
        /// 获取或设置 SubSource cmp3
        /// </summary>
        [DataMapping("SubSource", DbType.String)]
        public string SubSource { get; set; }

        /// <summary>
        /// 单据Item列表
        /// </summary>
        public List<OrderProductEntity> OrderProductList { get; set; }
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
