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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class ImageSizeVM : ModelBase
    {
        private string imageWidth;
         [Validate(ValidateType.Required)]
          [Validate(ValidateType.Interger)]
        public string ImageWidth
        {
            get { return imageWidth; }
            set { SetValue("ImageWidth", ref imageWidth, value); }
        }

        private string imageHeight;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string ImageHeight
        {
            get { return imageHeight; }
            set { SetValue("ImageHeight", ref imageHeight, value); }
        }
     }
}
