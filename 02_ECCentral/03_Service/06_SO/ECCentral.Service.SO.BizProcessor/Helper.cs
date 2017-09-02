using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.IO;

namespace ECCentral.Service.SO.BizProcessor
{

    public static partial class AppSettingHelper
    {
        /// <summary>
        /// 电子卡默认出库仓库
        /// </summary>
        public static int ElectronicCardDefaultStockSysNo
        {
            get
            {
                int result = int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "ElectronicCardDefaultStockSysNo"), out result) ? result : 0;
                return result;
            }
        }

        /// <summary>
        /// 商品折扣比率
        /// </summary>
        public static decimal ProductDiscountRatio
        {
            get
            {
                decimal discountRatio = decimal.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "ProductDiscountRatio"), out discountRatio) ? discountRatio : 0M;
                return discountRatio;
            }
        }

        /// <summary>
        /// 推荐人加经验比率
        /// </summary>
        public static decimal RecommendExperienceRatio
        {
            get
            {
                decimal ratio = decimal.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "RecommendExperienceRatio"), out ratio) ? ratio : 0M;
                return ratio;
            }
        }
        /// <summary>
        /// 投诉来源类型默认值
        /// </summary>
        public static string ComplainSourceTypeDefault
        {
            get
            {
                return AppSettingManager.GetSetting(SOConst.DomainName, "ComplainSourceTypeDefault");
            }
        }
        /// <summary>
        /// 大件商品重量
        /// </summary>
        public static decimal LargeProductWeight
        {
            get
            {
                decimal weight = decimal.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "LargeProductWeight"), out weight) ? weight : 0M;
                return weight;
            }
        }

        /// <summary>
        /// 订单跟踪类型—补发赠品
        /// </summary>
        public static int? InternalMemo_CallType_GiftSO
        {
            get
            {
                int t;
                if (int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "LargeProductWeight"), out t))
                {
                    return t;
                }
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static XElement OrderBizConfig
        {
            get
            {
                string value = AppSettingManager.GetSetting(SOConst.DomainName, "OrderBizConfig");
                StringReader reader = new StringReader(value);
                return XElement.Load(reader);
            }
        }

        /// <summary>
        /// 赠品订单支付方式
        /// </summary>
        public static int? GiftSOPayTypeSysNo
        {
            get
            {
                int t;
                if (int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "GiftSO_PayTypSysNo"), out t))
                {
                    return t;
                }
                return null;
            }
        }

        /// <summary>
        /// 赠品订单运送方式
        /// </summary>
        public static int? GiftSOShipTypeSysNo
        {
            get
            {
                int t;
                if (int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "GiftSO_ShipTypeSysNo"), out t))
                {
                    return t;
                }
                return null;
            }
        }
    }

    public static class BizExceptionHelper
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

    public static class ResourceHelper
    {
        /// <summary>
        /// 订单提示信息文件SO/SOInfo.[Language].xml的路径
        /// </summary>
        public const string MessagePath_SOInfo = "SO.SOInfo";

        public static string Get(string sourceKey, params object[] value)
        {
            ResouceManager.GetMessageString(MessagePath_SOInfo, sourceKey);
            return string.Format(ResouceManager.GetMessageString(MessagePath_SOInfo, sourceKey), value);
        }
    }
}