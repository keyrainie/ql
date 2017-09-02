using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    [Serializable]
    public class EntityBase
    {
        /// <summary>
        /// Gets or sets a value indicate the system physic number of this entity.
        /// </summary>
        [DataMapping("SysNo", DbType.Int32)]
        public int SystemNumber { get; set; }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNumber { get; set; }

        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int? LastEditUserSysNumber { get; set; }

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime? CreateDate { get; set; }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime? LastEditDate { get; set; }

        [DataMapping("CreateUserName", DbType.String)]
        public string CreateUserName { get; set; }

        [DataMapping("LastEditUserName", DbType.String)]
        public string LastEditUserName { get; set; }

    }
}
