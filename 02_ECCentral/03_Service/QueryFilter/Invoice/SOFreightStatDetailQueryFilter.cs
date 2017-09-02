using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Invoice
{
    public class SOFreightStatDetailQueryFilter
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
        /// 订单系统编号,多个订单号之间用.隔开
        /// </summary>
        public string SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 运费支出确认状态
        /// </summary>
        public RealFreightStatus? RealFreightConfirm
        {
            get;
            set;
        }

        /// <summary>
        /// 运费收入确认状态
        /// </summary>
        public CheckStatus? SOFreightConfirm
        {
            get;
            set;
        }
    }
}
