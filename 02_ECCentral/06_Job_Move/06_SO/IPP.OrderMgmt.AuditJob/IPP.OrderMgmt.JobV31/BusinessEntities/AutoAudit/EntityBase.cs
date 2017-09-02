using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    /// <summary>
    /// Class which indicate the base class of Business Entity
    /// </summary>
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
