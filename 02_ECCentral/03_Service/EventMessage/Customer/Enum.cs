using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.Customer
{

    public enum IDCardType
    {
        /// <summary> 
        /// 身份证 
        /// </summary> 
        Identity = 0,
        /// <summary>
        /// 导游证
        /// </summary>
        Tourist = 1,
        /// <summary>
        /// 护照
        /// </summary>
        Passport = 2,
        /// <summary>
        /// 港澳回乡证
        /// </summary>
        ReturnHome = 3,
        /// <summary>
        /// 台胞证
        /// </summary>
        Taiwan = 4
    }
}
