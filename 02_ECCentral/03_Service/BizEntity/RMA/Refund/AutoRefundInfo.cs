using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// 自动退款单信息
    /// </summary>
    public class AutoRefundInfo
    {
        /// <summary>
        /// 订单编号
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
        /// 是否合并订单
        /// </summary>
        public int? IsCombinedSO
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo
        {
            get;
            set;
        }
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
    }
}