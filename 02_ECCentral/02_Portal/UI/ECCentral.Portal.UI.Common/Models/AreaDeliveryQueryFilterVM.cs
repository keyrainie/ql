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
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Models
{
   public class AreaDeliveryQueryFilterVM : ModelBase
   {
       public AreaDeliveryQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
        }

        public PagingInfo PagingInfo { get; set; }

        private int? m_WHArea;
        public int? WHArea
        {
            get { return m_WHArea; }
            set { this.SetValue("WHArea", ref m_WHArea, value);  }
        }

       private String m_City;
       public String City
       {
           get { return this.m_City; }
           set { this.SetValue("City", ref m_City, value);  }
       }

   }
}
