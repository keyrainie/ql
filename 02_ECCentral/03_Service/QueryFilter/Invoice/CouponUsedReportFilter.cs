using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Invoice
{
    public class CouponUsedReportFilter
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }


        public DateTime? SODateFrom
        {
            get;
            set;
        }

        public DateTime? SODateTo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        private int? soSysNo;
        public int? SoSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 优惠券活动系统编码
        /// </summary>
        public int? CouponSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 供应商编码
        /// </summary>
        public int? MerchantSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 支付编码
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }


        public SOStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 订单支付状态
        /// </summary>
        public SOIncomeStatus? SOPayStatus
        {
            get;
            set;
        }

    }
}
