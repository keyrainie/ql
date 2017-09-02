using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
namespace ECommerce.Entity
{
    public class HotSearchKeyword
    {
        /// <summary>
        /// 获取或设置系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 获取或设置关键字信息
        /// </summary>
        public string Keyword { get; set; }
    }
}
