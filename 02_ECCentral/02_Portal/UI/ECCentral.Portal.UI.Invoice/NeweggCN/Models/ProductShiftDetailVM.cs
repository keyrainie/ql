using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
   public class ProductShiftDetailVM : ModelBase
   {
       private bool m_IsCheck;

       public bool IsCheck
       {
           get { return m_IsCheck; }
           set { SetValue("IsCheck", ref m_IsCheck, value); }
       }

       private Int32? m_StItemSysNo;
       public Int32? StItemSysNo
       {
           get { return this.m_StItemSysNo; }
           set { this.SetValue("StItemSysNo", ref m_StItemSysNo, value);  }
       }

       private DateTime? m_OutTime;
       public DateTime? OutTime
       {
           get { return this.m_OutTime; }
           set { this.SetValue("OutTime", ref m_OutTime, value);  }
       }

       public string OutTimeString
       {
           get
           {
               if (OutTime != null)
               {
                   return OutTime.Value.ToString("yyyy-MM");
               }

               return string.Empty;
           }
       }

       private DateTime? m_InTime;
       public DateTime? InTime
       {
           get { return this.m_InTime; }
           set { this.SetValue("InTime", ref m_InTime, value);  }
       }

       private Int32? m_ShiftSysNo;
       public Int32? ShiftSysNo
       {
           get { return this.m_ShiftSysNo; }
           set { this.SetValue("ShiftSysNo", ref m_ShiftSysNo, value);  }
       }

       private Int32? m_ProductSysNo;
       public Int32? ProductSysNo
       {
           get { return this.m_ProductSysNo; }
           set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value);  }
       }

       private String m_ProductID;
       public String ProductID
       {
           get { return this.m_ProductID; }
           set { this.SetValue("ProductID", ref m_ProductID, value);  }
       }

       private String m_ProductName;
       public String ProductName
       {
           get { return this.m_ProductName; }
           set { this.SetValue("ProductName", ref m_ProductName, value);  }
       }

       private Int32? m_ShiftQty;
       public Int32? ShiftQty
       {
           get { return this.m_ShiftQty; }
           set { this.SetValue("ShiftQty", ref m_ShiftQty, value);  }
       }

       private Decimal? m_UnitCost;
       public Decimal? UnitCost
       {
           get { return this.m_UnitCost; }
           set { this.SetValue("UnitCost", ref m_UnitCost, value);  }
       }

       private Decimal? m_AmtCount;
       public Decimal? AmtCount
       {
           get { return this.m_AmtCount; }
           set { this.SetValue("AmtCount", ref m_AmtCount, value);  }
       }

       private Decimal? m_AmtTaxItem;
       public Decimal? AmtTaxItem
       {
           get { return this.m_AmtTaxItem; }
           set { this.SetValue("AmtTaxItem", ref m_AmtTaxItem, value);  }
       }

       private Decimal? m_AmtProductCost;
       public Decimal? AmtProductCost
       {
           get { return this.m_AmtProductCost; }
           set { this.SetValue("AmtProductCost", ref m_AmtProductCost, value);  }
       }

       private Decimal? m_AtTotalAmt;
       public Decimal? AtTotalAmt
       {
           get { return this.m_AtTotalAmt; }
           set { this.SetValue("AtTotalAmt", ref m_AtTotalAmt, value);  }
       }

       private String m_StockNameA;
       public String StockNameA
       {
           get { return this.m_StockNameA; }
           set { this.SetValue("StockNameA", ref m_StockNameA, value);  }
       }

       private String m_StockNameB;
       public String StockNameB
       {
           get { return this.m_StockNameB; }
           set { this.SetValue("StockNameB", ref m_StockNameB, value);  }
       }

       private String m_GoldenTaxNo;
       public String GoldenTaxNo
       {
           get { return this.m_GoldenTaxNo; }
           set { this.SetValue("GoldenTaxNo", ref m_GoldenTaxNo, value);  }
       }

       private String m_InvoiceNo;
       public String InvoiceNo
       {
           get { return this.m_InvoiceNo; }
           set { this.SetValue("InvoiceNo", ref m_InvoiceNo, value);  }
       }

       private Int32? m_StockSysNoA;
       public Int32? StockSysNoA
       {
           get { return this.m_StockSysNoA; }
           set { this.SetValue("StockSysNoA", ref m_StockSysNoA, value);  }
       }

       private Int32? m_StockSysNoB;
       public Int32? StockSysNoB
       {
           get { return this.m_StockSysNoB; }
           set { this.SetValue("StockSysNoB", ref m_StockSysNoB, value);  }
       }

       private Int32? m_ShiftType;
       public Int32? ShiftType
       {
           get { return this.m_ShiftType; }
           set { this.SetValue("ShiftType", ref m_ShiftType, value);  }
       }

       private Decimal? m_AjustRate;
       public Decimal? AjustRate
       {
           get { return this.m_AjustRate; }
           set { this.SetValue("AjustRate", ref m_AjustRate, value);  }
       }

       private String m_SapCoCodeFrom;
       public String SapCoCodeFrom
       {
           get { return this.m_SapCoCodeFrom; }
           set { this.SetValue("SapCoCodeFrom", ref m_SapCoCodeFrom, value);  }
       }

       private String m_SapCoCodeTo;
       public String SapCoCodeTo
       {
           get { return this.m_SapCoCodeTo; }
           set { this.SetValue("SapCoCodeTo", ref m_SapCoCodeTo, value);  }
       }

       private String m_AmtCompanyCountInfo;
       public String AmtCompanyCountInfo
       {
           get { return this.m_AmtCompanyCountInfo; }
           set { this.SetValue("AmtCompanyCountInfo", ref m_AmtCompanyCountInfo, value);  }
       }

       private Boolean m_NeedManual;
       public Boolean NeedManual
       {
           get { return this.m_NeedManual; }
           set { this.SetValue("NeedManual", ref m_NeedManual, value);  }
       }

   }
}
