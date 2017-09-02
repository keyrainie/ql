using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSGiftItemRuleViewModel : ModelBase
   {
       private List<PSGiftItemViewModel> m_GiftItemSysNoList;
       public List<PSGiftItemViewModel> GiftItemSysNoList
       {
           get { return this.m_GiftItemSysNoList; }
           set { this.SetValue("GiftItemSysNoList", ref m_GiftItemSysNoList, value);  }
       }

       private List<Int32?> m_MasterProductSysNoList;
       public List<Int32?> MasterProductSysNoList
       {
           get { return this.m_MasterProductSysNoList; }
           set { this.SetValue("MasterProductSysNoList", ref m_MasterProductSysNoList, value); }
       }

   }

   public class PSGiftItemViewModel : ModelBase
   {
       private Int32? m_GiftItemSysNo;
       public Int32? GiftItemSysNo
       {
           get { return this.m_GiftItemSysNo; }
           set { this.SetValue("GiftItemSysNo", ref m_GiftItemSysNo, value); }
       }

       private Int32? m_GiftItemCount;
       public Int32? GiftItemCount
       {
           get { return this.m_GiftItemCount; }
           set { this.SetValue("GiftItemCount", ref m_GiftItemCount, value); }
       }

       private Int32? m_Priority;
       public Int32? Priority
       {
           get { return this.m_Priority; }
           set { this.SetValue("Priority", ref m_Priority, value); }
       }

   }
}
