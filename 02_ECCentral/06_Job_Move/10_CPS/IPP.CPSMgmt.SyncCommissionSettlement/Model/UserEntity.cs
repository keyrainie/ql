using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model
{
    [Serializable]
    public class UserEntity
    {
        #region Table Field 对应表字段
        /// <summary>
        /// 用户编号
        /// </summary>
        [DataMapping("UserSysNo", DbType.Int32)]
        public int UserSysNo { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        [DataMapping("ItemCount", DbType.Int32)]
        public int ItemCount { get; set; }

        #endregion Table Field 对应表字段
    }
    
}
