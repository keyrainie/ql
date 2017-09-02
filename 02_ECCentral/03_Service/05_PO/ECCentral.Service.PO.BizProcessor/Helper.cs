using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.BizProcessor
{
    public static class BizExceptionHelper
    {
        #region 提示信息 ,建议信息提示的内容都放到一起。

        /// <summary>
        /// 抛出业务异常信息
        /// </summary>
        /// <param name="sourceKey">资源键值</param>
        /// <param name="value">对应的值，适用于：键对应的值为:PID:{0}商品已处理   value:10030047</param>
        /// <param name="message">业务异常信息</param>
        public static void Throw(string sourceKey, params string[] value)
        {
            throw new BizException(ResourceHelper.Get(sourceKey, value));
        }

        #endregion 提示信息 ,建议信息提示的内容都放到一起。
    }

    public static class ResourceHelper
    {
        /// <summary>
        /// 订单提示信息文件SO/SOInfo.[Language].xml的路径
        /// </summary>
        public const string MessagePath_SOInfo = "ExternalSYS.Info";

        public static string Get(string sourceKey, params object[] value)
        {
            ResouceManager.GetMessageString(MessagePath_SOInfo, sourceKey);
            return string.Format(ResouceManager.GetMessageString(MessagePath_SOInfo, sourceKey), value);
        }
    }
}
