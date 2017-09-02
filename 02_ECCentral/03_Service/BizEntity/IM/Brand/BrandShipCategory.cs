using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 旗舰店首页分类
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrandShipCategory
    {
        /// <summary>
        /// 底层分类编号
        /// </summary>
        [DataMember]
        public int BrandShipCategoryID { get; set; }

        /// <summary>
        /// 厂商编号
        /// </summary>
        [DataMember]
        public int ManufacturerSysNo { get; set; }
    }
}
