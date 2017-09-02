/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:      Danish.G.Wang
 * Create Date: 2009-03-04
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.ContentManagement.BusinessEntities.Common
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
        public int SystemNumber 
        { 
            get; 
            set; 
        }

        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int CreateUserSysNumber 
        { 
            get; 
            set; 
        }

        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int LastEditUserSysNumber 
        { 
            get; 
            set; 
        }

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime CreateDate 
        { 
            get; 
            set; 
        }

        [DataMapping("LastEditDate", DbType.DateTime)]
        public DateTime LastEditDate 
        { 
            get; 
            set; 
        }

        public string CreateUserName 
        { 
            get; 
            set; 
        }

        public string LastEditUserName 
        { 
            get; 
            set; 
        }

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
