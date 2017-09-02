using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class ComputerConfigMaster : IIdentity, IWebChannel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        public string ComputerConfigName { get; set; }

        public int ComputerConfigTypeSysNo { get; set; }

        public string Note { get; set; }

        public int Priority { get; set; }

        public ComputerConfigStatus Status { get; set; }

        public DateTime InDate { get; set; }

        public string InUser { get; set; }

        public int CustomerSysNo { get; set; }

        public string AgreeCount { get; set; }

        public string DisagreeCount { get; set; }

        public string UniqueValidation { get; set; }

        public List<ComputerConfigItem> ConfigItemList { get; set; }

        //以下字段为DIY装机调度Job使用
        public int CreateUserSysNo { get; set; }
    }
}
