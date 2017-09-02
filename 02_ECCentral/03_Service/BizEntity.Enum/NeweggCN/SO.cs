using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 第三方订单
    /// </summary>
    public enum SpecialSOType
    {
        /// <summary>
        /// 普通订单，内部订单
        /// </summary>
        Normal = 0,
        ///// <summary>
        ///// 全免费的ACER赠品
        ///// </summary>
        //[Obsolete("此字段已废弃",true)]
        //AcerPromotion = 1,
        ///// <summary>
        ///// 阿期利康订单
        ///// </summary>
        //[Obsolete("此字段已废弃", true)]
        //AstraZeneca = 2,
        ///// <summary>
        ///// 理光用户订单
        ///// </summary>
        //[Obsolete("此字段已废弃", true)]
        //Ricoh = 3,
        /// <summary>
        /// 淘宝订单
        /// </summary>
        TaoBao = 4,
        ///// <summary>
        ///// 东方网订单
        ///// </summary>
        //[Obsolete("此字段已废弃", true)]
        //DongFang = 5,
        ///// <summary>
        ///// 移动商城订单
        ///// </summary>
        //[Obsolete("此字段已废弃", true)]
        //ChinaMobile = 6,
        ///// <summary>
        ///// JSB订单
        ///// </summary>
        //[Obsolete("此字段已废弃", true)]
        //JSB = 7,
        ///// <summary>
        ///// 迈腾订单
        ///// </summary>
        //[Obsolete("此字段已废弃", true)]
        //MT = 8
    }
}