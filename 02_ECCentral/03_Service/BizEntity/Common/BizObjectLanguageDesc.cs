using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    [Serializable]
    [DataContract]
    public class BizObjectLanguageDesc
    {

        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 如果业务类型是商品，则BizObjectSysNo不存入值，存入BizObjectId
        /// </summary>
        [DataMember]
        public string BizObjectType
        {
            get;
            set;
        }


        [DataMember]
        public int? BizObjectSysNo
        {
            get;
            set;
        }

        [DataMember]
        public string BizObjectId
        {
            get;
            set;
        }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }


        [DataMember]
        public string Description
        {
            get;
            set;
        }

      
    }
}
