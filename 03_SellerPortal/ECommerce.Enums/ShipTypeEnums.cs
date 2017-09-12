using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum StoreType
    {
        [Description("常温")]
        Narmal = 0,
        [Description("冷藏")]
        Cold = 1,
        [Description("冷冻")]
        Frozen = 2
    }

    public enum ShipTypeEnum
    {
        [Description("OZZO奥硕物流")]
        OZZO = 0,
        [Description("市内自提")]
        SelfGetInCity = 1,
        [Description("仓库自提")]
        SelfGetInStock = 2,
        [Description("第三方物流")]
        TheThird = 3
    }


    public enum ShipDeliveryType
    {
        /// <summary>
        /// 配送点不支持
        /// </summary>
        [Description("不指定预约时间")]
        NoAppointed = 0,

        /// <summary>
        /// 一天一送
        /// </summary>
        [Description("一天一送")]
        OneDayOnce = 1,

        /// <summary>
        /// 一天两送
        /// </summary>
        [Description("一天两送")]
        OneDayTwice = 2,

        /// <summary>
        /// 每天送
        /// </summary>
        [Description("工作日和双休日送")]
        EveryDay = 3,

        /// <summary>
        /// 一天六送
        /// </summary>
        [Description("一天六送")]
        OneDaySix = 4
    }

    public enum ShippingPackStyle
    {
        [Description("塑料袋")]
        Plastic = 0,
        [Description("纸箱")]
        Carton = 1
    }
    /// <summary>
    /// 专用配送方式,是或否
    /// </summary>
    public enum IsSpecial
    {
        Yes,
        No
    }
    /// <summary>
    /// 前后台隐藏,是或否
    /// </summary>
    public enum HYNStatus
    {
        [Description("前后台隐藏")]
        Hide = -1,
        [Description("是")]
        Yes = 1,
        [Description("否")]
        No = 0
    }

    public enum CompanyCustomer
    {
        /// <summary>
        /// 默认商户
        /// </summary>
        [Description("默认商户")]
        Default = 0,

        /// <summary>
        /// 阿斯利康商户
        /// </summary>
        [Obsolete]
        [Description("阿斯利康商户")]
        AstraZeneca = 1
    }
    /// <summary>
    /// 专用配送方式
    /// </summary>
    public enum Specified
    {
        [Description("否")]
        N = 'N',
        [Description("是")]
        Y = 'Y',
    }
    /// <summary>
    /// 24小时承诺
    /// </summary>
    public enum DeliveryPromise
    {
        [Description("不做承诺")]
        No = 0,
        [Description("是")]
        Yes = 1,
    }
    public enum Appointment4CombineSO
    {
        [Description("否")]
        No = 0,
        [Description("是")]
        Yes = 1,
    }
}
