using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECommerce.Entity.Common
{
    [Serializable]
    [DataContract]
    public class ExcelInfo
    {
        /// <summary>
        ///附件地址
        /// </summary>
        [DataMember]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        ///附件名称
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }
    }
}
