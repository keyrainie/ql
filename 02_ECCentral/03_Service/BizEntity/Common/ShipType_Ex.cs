using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 配送方式扩展信息
    /// </summary>
    public class ShipType_Ex
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;

        }
        /// <summary>
        /// 地区编号
        /// </summary>

        public int? AreaSysNo
        {
            get;
            set;

        }
        /// <summary>
        /// 自提点联系人
        /// </summary>

        public string ContactName
        {
            get;
            set;

        }
        /// <summary>
        /// 自提点联系电话
        /// </summary>

        public string ContactPhoneNumber
        {
            get;
            set;

        }
        /// <summary>
        /// Email
        /// </summary>

        public string Email
        {
            get;
            set;

        }
        /// <summary>
        /// 自提点地址
        /// </summary>

        public string Address
        {
            get;
            set;
        }
    }
}
