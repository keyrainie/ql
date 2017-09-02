using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.MKT.Floor
{
    /// <summary>
    /// 楼层的主信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class FloorMaster
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
        ///模板编号
        /// </summary>
        [DataMember]
        public int TemplateSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///楼层名称
        /// </summary>
        [DataMember]
        public string FloorName
        {
            get;
            set;
        }

        /// <summary>
        ///楼层的Logo图片的Url
        /// </summary>
        [DataMember]
        public string FloorLogoSrc
        {
            get;
            set;
        }

        /// <summary>
        ///楼层优先级，数字越小排在前面
        /// </summary>
        [DataMember]
        public int Priority
        {
            get;
            set;
        }

        /// <summary>
        ///备注信息
        /// </summary>
        [DataMember]
        public string Remark
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

        /// <summary>
        ///创建者系统编号
        /// </summary>
        [DataMember]
        public int? InUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///创建者显示名
        /// </summary>
        [DataMember]
        public string InUserName
        {
            get;
            set;
        }

        /// <summary>
        ///创建时间
        /// </summary>
        [DataMember]
        public DateTime? InDate
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人系统编号
        /// </summary>
        [DataMember]
        public int? EditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改人显示名
        /// </summary>
        [DataMember]
        public string EditUserName
        {
            get;
            set;
        }

        /// <summary>
        ///最后修改时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate
        {
            get;
            set;
        }

        /// <summary>
        /// 页面编码
        /// </summary>
        [DataMember]
        public PageCodeType? PageType
        {
            get;
            set;
        }

        /// <summary>
        /// 页面ID
        /// </summary>
        [DataMember]
        public string PageCode
        {
            get;
            set;
        }

        /// <summary>
        /// 页面名称
        /// </summary>
        [DataMember]
        public string PageName
        {
            get;
            set;
        }
    }
}

