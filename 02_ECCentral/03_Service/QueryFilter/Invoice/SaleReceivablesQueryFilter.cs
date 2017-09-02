using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
   public class SaleReceivablesQueryFilter
    {

        /// <summary>
        /// 分页信息
        /// </summary>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 支付类型
        /// </summary>
        public Int32? PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 统计日期
        /// </summary>
        public String QueryDate
        {
            get;
            set;
        }

        /// <summary>
        /// 货币单位
        /// </summary>
        public SaleCurrency? Currency
        {
            get;
            set;
        }

    }
}
