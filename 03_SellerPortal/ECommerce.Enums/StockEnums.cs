using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum StockStatus
    {
        [Description("有效")]
        Actived = 0,
        [Description("无效")]
        DeActived = -1
    }
    /// <summary>
    /// 海关关区代码
    /// </summary>
    public enum CustomsCodeMode 
    {
        /// <summary> 
        /// 直邮进口模式 
        /// </summary> 
        [Description("直邮进口模式")]
        DirectImportMode = 2244,
        /// <summary> 
        /// 浦东机场自贸模式 
        /// </summary> 
        [Description("浦东机场自贸模式")]
        PudongAirportTradeMode = 2216,
        /// <summary> 
        /// 洋山自贸模式 
        /// </summary> 
        [Description("洋山自贸模式")]
        YangshanTradeMode = 2249,
        /// <summary> 
        /// 外高桥自贸模式 
        /// </summary> 
        [Description("外高桥自贸模式")]
        WaigaoqiaoTradeMode = 2218 
    }

    /// <summary>
    /// 配送类型
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>
        /// 自有配送
        /// </summary>
        SELF,

        /// <summary>
        /// 商家配送
        /// </summary>
        MET
    }
}
