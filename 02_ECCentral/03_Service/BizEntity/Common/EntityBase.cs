using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    public class EntityBase
    {
        /// <summary>
        /// 创建者系统编号
        /// </summary>
        [DataMember]
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者的显示名
        /// </summary>
        [DataMember]
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime? InDate { get; set; }

        public string InDateStr
        {
            get { return this.InDate.HasValue ? InDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
            set
            {
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    InDate = dt;
                }
                else
                {
                    InDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// 最后更新者系统编号
        /// </summary>
        [DataMember]
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        [DataMember]
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }

        public string EditDateStr
        {
            get
            {
                if (EditDate.HasValue)
                {
                    EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return "";
            }
            set
            {
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                {
                    EditDate = dt;
                }
                else
                {
                    EditDate = null;
                }
            }
        }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        public string StoreCompanyCode { get; set; }
    }
}
