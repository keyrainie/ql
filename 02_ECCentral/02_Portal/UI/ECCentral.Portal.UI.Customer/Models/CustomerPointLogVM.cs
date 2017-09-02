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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerPointLogVM : ModelBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 积分数
        /// </summary>
        public int? PointAmount { get; set; }

        /// <summary>
        /// 累计数
        /// </summary>
        public int? TotalPointAmount { get; set; }

        /// <summary>
        /// 积分类型
        /// </summary>
        public int? PointType { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string OrderSysNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

    }
}
