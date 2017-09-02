using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT.Floor
{
    /// <summary>
    /// 楼层的分组标签信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class FloorSection
    {

        /// <summary>
        ///系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        ///楼层编号
        /// </summary>
        [DataMember]
        public int FloorMasterSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///分组标签名称
        /// </summary>
        [DataMember]
        public string SectionName
        {
            get;
            set;
        }

        /// <summary>
        ///分组标签优先级，数字越小排在前面，第一个分组标签为默认标签
        /// </summary>
        [DataMember]
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        ///状态，通用状态，共两种：有效，无效；
        /// </summary>
        [DataMember]
        public ADStatus Status
        {
            get;
            set;
        }

        

    }
}

