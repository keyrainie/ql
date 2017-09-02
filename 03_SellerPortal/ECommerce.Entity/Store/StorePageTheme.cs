using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Entity;

namespace ECommerce.Entity.Store
{

    /// <summary>
    ///页面主题(字典表)
    /// </summary>
    [Serializable]
    [DataContract]
    public class StorePageTheme : EntityBase
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
        ///主题名称
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///主题css文件url地址
        /// </summary>
        [DataMember]
        public string CssResUrl
        {
            get;
            set;
        }

        /// <summary>
        ///Mock地址
        /// </summary>
        [DataMember]
        public string MockUrl
        {
            get;
            set;
        }

        /// <summary>
        ///1:可用
        ///0:不可用
        /// </summary>
        [DataMember]
        public int? Status
        {
            get;
            set;
        }



    }
}
