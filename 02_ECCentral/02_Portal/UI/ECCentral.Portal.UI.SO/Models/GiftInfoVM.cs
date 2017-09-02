using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
namespace ECCentral.Portal.UI.SO.Models
{
    public class GiftInfoVM : ModelBase
   {
       private Int32 m_SysNo;
       public Int32 SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32 m_Quantity;
       public Int32 Quantity
       {
           get { return this.m_Quantity; }
           set { this.SetValue("Quantity", ref m_Quantity, value);  }
       }


       public int QtyPreTime { get; set; }

   }
}
