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
    public class PurgeToolVM : ModelBase
    {
        private string urlList;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?;[\r]{0,}){0,}(http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?[;]?)$", ErrorMessage = "输入正确的URL以;隔开")]
        public string UrlList 
        {
            get { return urlList; }
            set { SetValue("UrlList", ref urlList, value); }
        }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        private string priority;
         [Validate(ValidateType.Interger)]
        public string Priority 
        {
            get { return priority; }
            set { SetValue("Priority", ref priority, value); }
        }

        /// <summary>
        /// 清除时间
        /// </summary>
        private DateTime? clearDate;
        public DateTime? ClearDate 
        {
            get { return clearDate; }
            set { SetValue("ClearDate", ref clearDate, value); }
        }
    }
}
