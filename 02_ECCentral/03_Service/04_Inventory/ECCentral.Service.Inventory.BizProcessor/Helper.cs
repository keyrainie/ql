using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.BizProcessor
{
    internal static class BizExceptionHelper
    {
        #region 提示信息 ,建议信息提示的内容都放到一起。

        #region 暂时删除
        ///// <summary>
        ///// 此订单已被拆分，不允许操作!
        ///// </summary>
        //public static string Message_SO_Hold_SplitComplete
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Hold_SplitComplete");
        //    }
        //}
        ///// <summary>
        ///// 此订单已作废，不允许操作!
        ///// </summary>
        //public static string Message_SO_Hold_Abandoned
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Hold_Abandoned");
        //    }
        //}
        ///// <summary>
        ///// 此订单已出库，不允许操作!
        ///// </summary>
        //public static string Message_SO_Hold_OutStock
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Hold_OutStock");
        //    }
        //}
        ///// <summary>
        ///// 此订单被锁定，不允许操作!
        ///// </summary>
        //public static string Message_SO_Hold_BackHold
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Hold_BackHold");
        //    }
        //}
        ///// <summary>
        ///// 订单已被前台锁定,不允许操作!
        ///// </summary>
        //public static string Message_SO_Hold_WebHold
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Hold_WebHold");
        //    }
        //}
        ///// <summary>
        ///// 前台正在修改并提交订单信息,不允许操作!
        ///// </summary>
        //public static string Message_SO_Hold_Processing
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Hold_Processing");
        //    }
        //}
        ///// <summary>
        ///// 订单不存在！
        ///// </summary>
        //public static string Message_SO_Hold_SOIsNotExist
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_SOIsNotExist");
        //    }
        //}

        ///// <summary>
        ///// 订单不存在主商品！
        ///// </summary>
        //public static string Message_SO_Create_SOHaveNotMainItem
        //{
        //    get
        //    {
        //        return ResouceManager.GetMessageString(MessagePath_SOInfo, "SO_Create_SOHaveNotMainItem");
        //    }
        //}

        ///// <summary>
        ///// 抛出业务异常信息
        ///// </summary>
        ///// <param name="message">业务异常信息</param>
        //public static void Throw(string message)
        //{
        //    throw new BizException(message);
        //}
        #endregion

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

    internal static class ResourceHelper
    {
        /// <summary>
        /// 订单提示信息文件SO/SOInfo.[Language].xml的路径
        /// </summary>
        public const string MessagePath_SOInfo = "Inventory.InventoryInfo";

        public static string Get(string sourceKey, params object[] value)
        {
            ResouceManager.GetMessageString(MessagePath_SOInfo, sourceKey);
            return string.Format(ResouceManager.GetMessageString(MessagePath_SOInfo, sourceKey), value);
        }
    }
}