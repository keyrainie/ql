using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    public class FPSwitchEntity
    {
        /// <summary>
        /// 检测可以订单
        /// </summary>
        public bool IsCheckKY 
        {
            get;
            set; 
        }

        /// <summary>
        /// 检测串货订单
        /// </summary>
        public bool IsCheckCH 
        { 
            get;
            set; 
        }

        /// <summary>
        /// 检测炒货订单
        /// </summary>
        public bool IsCheckCC 
        { 
            get;
            set;
        }

        /// <summary>
        /// 检测重复订单
        /// </summary>
        public bool IsCheckCF 
        { 
            get;
            set;
        }
 
    }
}
