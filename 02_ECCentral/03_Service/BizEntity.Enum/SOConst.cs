using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    public static class SOConst
    {
        public const string DomainName = "SO";

        /// <summary>
        /// 自有订单，与第三方订单区别
        /// </summary>
        public const int SelfSO = 0;
        /// <summary>
        /// 时间格式为：yyyy-MM-dd HH:mm
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// 长时间格式：yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string LongTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 时间格式为：yyyy-MM-dd
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd";
        //两位小数的字符串格式
        public const string DecimalFormat = "#########0.00";
    }
}
