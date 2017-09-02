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
    public class CategoryTemplateQueryVM:ModelBase
    {
        private int? category3SysNO;

        public int? Category3SysNo 
        {
            get { return category3SysNO; }
            set { SetValue("Category3SysNo", ref category3SysNO, value); }
        }
    }
}
