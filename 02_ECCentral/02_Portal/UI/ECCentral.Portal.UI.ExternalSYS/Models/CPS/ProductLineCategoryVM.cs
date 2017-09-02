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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    /// <summary>
    /// 产品线类别
    /// </summary>
    public class ProductLineCategoryVM : ModelBase
    {
        public int SysNo { get; set; }

        private string categoryName;
        public string CategoryName 
        {
            get { return categoryName; }
            set { SetValue("CategoryName", ref categoryName, value); }
        }
    }
}
