using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 成本变价单
    /// </summary>
    public class CostChangeInfo : IIdentity, ICompany
    {
        public CostChangeInfo()
        {
            CostChangeItems = new List<CostChangeItemsInfo>();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 成本变价单基本信息
        /// </summary>
        public CostChangeBasicInfo CostChangeBasicInfo { get; set; }

        /// <summary>
        /// 成本变价明细列表
        /// </summary>
        public List<CostChangeItemsInfo> CostChangeItems { get; set; }

    }
}
