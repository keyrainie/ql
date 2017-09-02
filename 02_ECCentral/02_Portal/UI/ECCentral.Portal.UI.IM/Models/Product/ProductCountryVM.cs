using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.IM.Models
{
    /// <summary>
    /// 商品产地
    /// </summary>
    public class ProductCountryVM
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }
    }
}
