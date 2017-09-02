using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 仓库改单
    /// </summary>
    public class SOPending : IIdentity,ICompany
    {
        public int? SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? WarehouseNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? IsPartialShipping
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public SOPendingStatus? Status
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get;
            set;
        }

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
