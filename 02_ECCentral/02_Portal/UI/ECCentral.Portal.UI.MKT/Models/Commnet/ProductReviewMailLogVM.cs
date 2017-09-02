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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductReviewMailLogVM : ModelBase
    {
        public string CompanyCode { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 邮件内容
        /// </summary>
        private string topicMailContent;
        public string TopicMailContent
        {
            get { return topicMailContent; }
            set { base.SetValue("TopicMailContent", ref topicMailContent, value); }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        //private string newMailContent;
        //[Validate(ValidateType.Required)]
        //public string NewMailContent
        //{
        //    get { return newMailContent; }
        //    set { base.SetValue("NewMailContent", ref newMailContent, value); }
        //}
    }
}
