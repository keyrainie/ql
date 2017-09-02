using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT.Floor
{
    /// <summary>
    /// 首页楼层使用到的模板，数据由开发人员初始化好
    /// </summary>
    [Serializable]
    [DataContract]
    public class FloorTemplate
    {

        /// <summary>
        ///模板编号
        /// </summary>
        [DataMember]
        public int? TemplateSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///模板名称
        /// </summary>
        [DataMember]
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        ///模板对应的PartialView
        /// </summary>
        [DataMember]
        public string PartialView
        {
            get;
            set;
        }

        /// <summary>
        ///备注信息
        /// </summary>
        [DataMember]
        public string Remarks
        {
            get;
            set;
        }

        /// <summary>
        ///状态，通用状态，共两种：有效1，无效0，删除是将状态设置为-1
        /// </summary>
        [DataMember]
        public ADStatus Status
        {
            get;
            set;
        }


    }
}

