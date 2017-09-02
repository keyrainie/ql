using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ECommerce.Entity
{
    public class EntityBase
    {
        /// <summary>
        /// 创建者系统编号
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者的显示名
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ScriptIgnore]
        public DateTime InDate { get; set; }

        public string InDateStr
        {
            get { return InDate.ToString("yyyy-MM-dd HH:mm:ss"); }
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
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [ScriptIgnore]
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
        public int? SellerSysNo { get; set; }

        private string m_CompanyCode = "8601";
        /// <summary>
        /// 平台公司系统编号
        /// </summary>
        public string CompanyCode
        {
            get
            {
                return m_CompanyCode;
            }
            set
            {
                m_CompanyCode = value;
            }
        }

        private string m_LanguageCode = "zh-CN";
        /// <summary>
        /// 语言编码
        /// </summary>
        public string LanguageCode
        {
            get
            {
                return m_LanguageCode;
            }
            set
            {
                m_LanguageCode = value;
            }
        }
    }
}
