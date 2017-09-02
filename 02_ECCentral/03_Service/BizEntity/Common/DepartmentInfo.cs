using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 部门信息
    /// </summary>
    public class DepartmentInfo:IIdentity,ICompany,ILanguage
    {
        /// <summary>
        /// 部门编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 部门的有效性状态   Active: 有效    DeActive：无效
        /// </summary>
        public DepartmentValidStatus Status { get; set; }

        #region IIdentity Members

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        #endregion

        #region ILanguage Members
        /// <summary>
        /// 使用语言
        /// </summary>
        public string LanguageCode
        {
            get;
            set;
        }

        #endregion

        #region ICompany Members

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
    }
}
