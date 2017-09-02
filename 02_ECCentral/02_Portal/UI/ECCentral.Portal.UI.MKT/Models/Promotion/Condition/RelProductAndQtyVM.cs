using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class RelProductAndQtyViewModel : ModelBase
   {
       private Int32? m_ProductSysNo;
       public Int32? ProductSysNo
       {
           get { return this.m_ProductSysNo; }
           set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
       }

       private string m_MinQty;
       [Validate(ValidateType.Required)]
       [Validate(ValidateType.Regex, @"^[1-9]\d{0,3}$", ErrorMessage = "请输入1至9999的整数！")]
       public string MinQty
       {
           get { return this.m_MinQty; }
           set { this.SetValue("MinQty", ref m_MinQty, value); }
       }

       /**** 下面的都是用来显示和UI操作的 *****/
       private string m_ProductID;
       public string ProductID
       {
           get { return this.m_ProductID; }
           set { this.SetValue("ProductID", ref m_ProductID, value); }
       }

       private string m_ProductName;
       public string ProductName
       {
           get { return this.m_ProductName; }
           set { this.SetValue("ProductName", ref m_ProductName, value); }
       }

       private Int32? m_AvailableQty;
       public Int32? AvailableQty
       {
           get { return this.m_AvailableQty; }
           set { this.SetValue("AvailableQty", ref m_AvailableQty, value); }
       }

       private Int32? m_ConsignQty;
       public Int32? ConsignQty
       {
           get { return this.m_ConsignQty; }
           set { this.SetValue("ConsignQty", ref m_ConsignQty, value); }
       }

       private Int32? m_VirtualQty;
       public Int32? VirtualQty
       {
           get { return this.m_VirtualQty; }
           set { this.SetValue("VirtualQty", ref m_VirtualQty, value); }
       }

       private Decimal? m_UnitCost;
       public Decimal? UnitCost
       {
           get { return this.m_UnitCost; }
           set { this.SetValue("UnitCost", ref m_UnitCost, value); }
       }

       private Decimal? m_CurrentPrice;
       public Decimal? CurrentPrice
       {
           get { return this.m_CurrentPrice; }
           set { this.SetValue("CurrentPrice", ref m_CurrentPrice, value); }
       }      

       private Decimal? m_GrossMarginRate;
       public Decimal? GrossMarginRate
       {
           get { return this.m_GrossMarginRate; }
           set { this.SetValue("GrossMarginRate", ref m_GrossMarginRate, value); }
       }

       private Int32? m_Priority;
       public Int32? Priority
       {
           get { return this.m_Priority; }
           set { this.SetValue("Priority", ref m_Priority, value); }
       }


       private bool? isChecked=false;
       public bool? IsChecked
       {
           get { return isChecked; }
           set { base.SetValue("IsChecked", ref isChecked, value); }
       }

   }
}
