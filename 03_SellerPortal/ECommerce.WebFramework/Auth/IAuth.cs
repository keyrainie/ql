using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.WebFramework
{
    public interface IAuth
    {   
        /// <summary>
        /// 验证客户登录有效性
        /// </summary>       
        /// <returns></returns>
        bool ValidateAuth();
    }
}
