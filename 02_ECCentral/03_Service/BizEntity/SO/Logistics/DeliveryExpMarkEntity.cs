using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    public class DeliveryExpMarkEntity : ICompany
    {
        /// <summary>
        /// 订单类型
        /// </summary>
        public int? OrderType { get; set; }

        /// <summary>
        /// 订单编号List
        /// </summary>
        public List<int?> OrderSysNos {get;set;}

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
