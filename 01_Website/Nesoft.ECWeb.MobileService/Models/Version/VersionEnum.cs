using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Nesoft.ECWeb.MobileService.Models.Version
{
    /// <summary>
    /// 客户端类型
    /// </summary>
    public enum ClientType
    {
        [Description("IPhone")]
        IPhone = 1,
        [Description("Android")]
        Android = 2
    }
}