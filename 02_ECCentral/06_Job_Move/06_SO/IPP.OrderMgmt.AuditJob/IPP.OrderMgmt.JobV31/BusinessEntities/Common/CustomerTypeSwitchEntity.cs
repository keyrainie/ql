using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    public class CustomerTypeSwitchEntity
    {
        /// <summary>
        /// 检测正常客户
        /// </summary>
        public bool IsCheckCustomerZC 
        {
            get; 
            set;
        }

        /// <summary>
        /// 检测可以客户
        /// </summary>
        public bool IsCheckCustomerKY
        { 
            get;
            set;
        }

        /// <summary>
        /// 检测欺诈客户
        /// </summary>
        public bool IsCheckCustomerQZ 
        { 
            get;
            set; 
        }
 
    }
}
