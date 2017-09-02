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
    public class UnifiedImageVM : ModelBase
    {
        /// <summary>
        /// 图片名称
        /// </summary>
        private string imageName;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength,50)]
        public string ImageName
        {
            get { return imageName; }
            set { base.SetValue("ImageName", ref imageName, value); }
        }

        //图片地址
        public string imageUrl;
         [Validate(ValidateType.Required)]
        public string ImageUrl
        {
            get { return imageUrl; }
            set { base.SetValue("ImageUrl", ref imageUrl, value); }
        }
    }
}
