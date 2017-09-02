using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.POmgmt.ETA.Model
{
    public class POEimsEntity
    {
        #region
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }
        /// <summary>
        /// 采购单编号
        /// </summary>
        [DataMapping("POSysNo", DbType.Int32)]
        public int POSysNo { get; set; }
        /// <summary>
        /// EIMS 编号
        /// </summary>
        [DataMapping("EIMSNo", DbType.Int32)]
        public int EIMSNo { get; set; }
        /// <summary>
        /// 返点使用金额
        /// </summary>
        [DataMapping("EIMSAmt", DbType.Decimal)]
        public decimal EIMSAmt { get; set; }
        /// <summary>
        /// 下单时返点使用情况（已使用金额）
        /// </summary>
        [DataMapping("AlreadyUseAmt", DbType.Decimal)]
        public decimal AlreadyUseAmt { get; set; }
        /// <summary>
        /// 下单时返点剩余金额
        /// </summary>
        [DataMapping("EIMSLeftAmt", DbType.Decimal)]
        public decimal EIMSLeftAmt { get; set; }

        /// <summary>
        /// 返点剩余金额（每次入库修改）
        /// </summary>
        [DataMapping("LeftAmt", DbType.Decimal)]
        public decimal LeftAmt { get; set; }
        #endregion

        #region
        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int CurrencySysNo { get; set; }
        #endregion
    }
}
