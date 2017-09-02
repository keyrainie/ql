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

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAttachmentQueryVM : ModelBase
    {
        /// <summary>
        /// 商品SysNo
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        
        /// <summary>
        /// 附件ID
        /// </summary>
        public string AttachmentID
        {
            get;
            set;
        }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string AttachmentName { get; set; }

        /// <summary>
        /// 创建开始时间
        /// </summary>
        public DateTime? InDateStart { get; set; }

        /// <summary>
        /// 创建结束时间
        /// </summary>
        public DateTime? InDateEnd { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditUser { get; set; }


    }
}
