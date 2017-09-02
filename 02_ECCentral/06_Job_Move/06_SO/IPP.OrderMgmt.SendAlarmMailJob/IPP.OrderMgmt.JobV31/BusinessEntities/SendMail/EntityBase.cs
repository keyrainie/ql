using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using Newegg.Oversea.Framework.Auth;
using System.Runtime.Serialization;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.IO;
namespace IPP.OrderMgmt.JobV31.BusinessEntities.SendMail
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

        [DataMapping("IP2Log", DbType.String)]
        public string IP2Log { get; set; }

        public MessageHeaderInfo MessageHeaderInfo { get; set; }

        /// <summary>
        /// Clone the entity.
        /// </summary>
        /// <returns>The cloned entity.</returns>
        virtual public EntityBase Clone()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, this);
            ms.Position = 0;
            EntityBase CloneObject = formatter.Deserialize(ms) as EntityBase;
            ms.Close();
            return CloneObject;
        }
    }
}
