using System;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Framework.Entity;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;

namespace ECCentral.Job.SO.SendAlarmMailJob.BusinessEntities.SendMail
{
    [Serializable]
    public class EntityBase
    {
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
