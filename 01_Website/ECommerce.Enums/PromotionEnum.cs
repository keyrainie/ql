using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum VoucherType
    {
        [Description("Valid")]
        Valid = 0,
        [Description("InValid")]
        InValid = -1
    }

    public enum ComboType
    {
        /// <summary>
        /// 普通绑定
        /// </summary>
        Common,
        /// <summary>
        /// 加N元送X商品
        /// </summary>
        //NYuanSend
    }

    public enum SaleGiftType
    {
        /// <summary>
        /// 单品买赠
        /// </summary>
        [Description("单品买赠")]
        Single,
        /// <summary>
        /// 同时购买
        /// </summary>
        [Description("同时购买")]
        Multiple,
        /// <summary>
        /// 买满即送
        /// </summary>
        [Description("买满即送")]
        Full,
        /// <summary>
        /// 厂商赠品
        /// </summary>
        [Description("厂商赠品")]
        Vendor,
         
    }

    public enum GroupReCommandType
    {
        [Description("普通商品")]
        Common=0,
        [Description("左侧合并6产品")]
        LT06=7,
    }
}
