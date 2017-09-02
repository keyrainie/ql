using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Utility
{
    public interface IContext
    {
        /// <summary>
        /// 当前操作用户的系统唯一编号
        /// </summary>
        int UserSysNo { get; }

        /// <summary>
        /// 当前用户的ID
        /// </summary>
        string UserID { get; }

        /// <summary>
        /// 当前操作用户的显示名
        /// </summary>
        string UserDisplayName { get; }

        /// <summary>
        /// 当前操作客户端的IP地址
        /// </summary>
        string ClientIP { get; }

        /// <summary>
        /// 将当前的 IContext 实例附加到指定的 IContext 实例
        /// </summary>
        /// <param name="owner">要附加的IContext实例</param>
        void Attach(IContext owner);

        string this[string key]
        {
            get;
        }
    }
}
