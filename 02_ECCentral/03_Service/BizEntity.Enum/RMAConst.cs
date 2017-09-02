using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    public static class RMAConst
    {
        /// <summary>
        /// Merchant(商家)
        /// </summary>
        public const string MET = "MET";

        /// <summary>
        /// Self(就是Newegg)
        /// </summary>
        public const string NEG = "NEG";

        public const string ShipVia_PostOffice = "邮局普包";       

        public const string DecimalFormat = "#########0.00";

        /// <summary>
        /// 淘宝账户在AppSettings中的Key
        /// </summary>
        public const string TaoBaoAutoAccountKey = "TaoBaoAutoAccount";

        /// <summary>
        /// 自动RMA用户的登录名
        /// </summary>
        public const string AutoRMALoginUserName = "AutoRMALoginUserName";

        /// <summary>
        /// 自动RMA用户的真实名称
        /// </summary>
        public const string AutoRMAPhysicalUserName = "AutoRMAPhysicalUserName";

        /// <summary>
        /// 自动RMA用户所在的域
        /// </summary>
        public const string AutoRMASourceDirectoryKey = "AutoRMASourceDirectoryKey";
    }
}
