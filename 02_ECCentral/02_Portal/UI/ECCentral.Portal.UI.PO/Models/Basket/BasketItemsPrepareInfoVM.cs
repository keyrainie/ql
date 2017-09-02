using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
   public class BasketItemsPrepareInfoVM : ModelBase
   {
       private Int32? m_ItemSysNo;
       public Int32? ItemSysNo
       {
           get { return this.m_ItemSysNo; }
           set { this.SetValue("ItemSysNo", ref m_ItemSysNo, value);  }
       }

       private Int32? m_CreateUserSysNo;
       public Int32? CreateUserSysNo
       {
           get { return this.m_CreateUserSysNo; }
           set { this.SetValue("CreateUserSysNo", ref m_CreateUserSysNo, value);  }
       }

       private Int32? m_ProductSysNo;
       public Int32? ProductSysNo
       {
           get { return this.m_ProductSysNo; }
           set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value);  }
       }

       private Int32? m_Quantity;
       public Int32? Quantity
       {
           get { return this.m_Quantity; }
           set { this.SetValue("Quantity", ref m_Quantity, value);  }
       }

       private Decimal? m_OrderPrice;
       public Decimal? OrderPrice
       {
           get { return this.m_OrderPrice; }
           set { this.SetValue("OrderPrice", ref m_OrderPrice, value);  }
       }

       private DateTime? m_CreateTime;
       public DateTime? CreateTime
       {
           get { return this.m_CreateTime; }
           set { this.SetValue("CreateTime", ref m_CreateTime, value);  }
       }

       private String m_ProductID;
       public String ProductID
       {
           get { return this.m_ProductID; }
           set { this.SetValue("ProductID", ref m_ProductID, value);  }
       }

       private Int32? m_StockSysNo;
       public Int32? StockSysNo
       {
           get { return this.m_StockSysNo; }
           set { this.SetValue("StockSysNo", ref m_StockSysNo, value);  }
       }

       private Int32? m_IsTransfer;
       public Int32? IsTransfer
       {
           get { return this.m_IsTransfer; }
           set { this.SetValue("IsTransfer", ref m_IsTransfer, value);  }
       }

       private Int32? m_LastVendorSysNo;
       public Int32? LastVendorSysNo
       {
           get { return this.m_LastVendorSysNo; }
           set { this.SetValue("LastVendorSysNo", ref m_LastVendorSysNo, value);  }
       }

       private Int32? m_ReadyQuantity;
       public Int32? ReadyQuantity
       {
           get { return this.m_ReadyQuantity; }
           set { this.SetValue("ReadyQuantity", ref m_ReadyQuantity, value);  }
       }

   }
}
