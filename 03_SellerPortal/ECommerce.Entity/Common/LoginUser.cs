using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Common
{
    [Serializable]
    public class LoginUser
    {
        /// <summary>
        /// 用户系统编号
        /// </summary>
        public int UserSysNo { get; set; }

        /// <summary>
        /// 登录名称
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary> 
        public CommonStatus Status { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// 用户权限点列表
        /// </summary>
        public List<string> UserAuthKeyList { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int SellerSysNo { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string SellerName { get; set; }

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
