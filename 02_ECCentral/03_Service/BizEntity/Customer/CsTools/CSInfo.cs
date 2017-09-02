using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// CS信息
    /// </summary>
    public class CSInfo : ICompany, IIdentity
    {
        /// <summary>
        /// CS系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 下属数量
        /// </summary>
        public int? UnderlingNum { get; set; }
        /// <summary>
        /// 角色   普通、组长、经理
        /// </summary>
        public int? Role { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public int? IPPUserSysNo { get; set; }
        /// <summary>
        /// 所属组长CS编号
        /// </summary>
        public int? LeaderSysNo { get; set; }
        /// <summary>
        /// 组长对应的系统编号
        /// </summary>
        public int? LeaderIPPUserSysNo { get; set; }
        /// <summary>
        /// 组长姓名
        /// </summary>
        public string LeaderUserName { get; set; }
        /// <summary>
        /// 所属经理的CS编号
        /// </summary>
        public int? ManagerSysNo { get; set; }
        /// <summary>
        /// 经理对应的系统编号 
        /// </summary>
        public int? ManagerIPPUserSysNo { get; set; }
        /// <summary>
        /// 经理姓名
        /// </summary>
        public string ManagerUserName { get; set; }
        /// <summary>
        /// 所有下属的编号  只有组长才有下属
        /// </summary>
        public List<int> CSIPPUserSysNos { get; set; }
        /// <summary>
        /// 所有下属的名称 只有组长才有下属
        /// </summary>
        public List<string> CSUserNames { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

    }
}
