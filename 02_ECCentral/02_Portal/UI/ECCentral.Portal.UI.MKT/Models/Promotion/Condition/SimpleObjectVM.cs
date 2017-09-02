using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class SimpleObjectViewModel : ModelBase
   {
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private String m_ID;
       public String ID
       {
           get { return this.m_ID; }
           set { this.SetValue("ID", ref m_ID, value);  }
       }

       private String m_Name;
       public String Name
       {
           get { return this.m_Name; }
           set { this.SetValue("Name", ref m_Name, value);  }
       }

       private PSRelationType? m_Relation;
       public PSRelationType? Relation
       {
           get { return this.m_Relation; }
           set { this.SetValue("Relation", ref m_Relation, value); }
       }

       private bool? isChecked;
       public bool? IsChecked
       {
           get { return isChecked; }
           set { base.SetValue("IsChecked", ref isChecked, value); }
       }

       private string m_BakString1;
       public string BakString1 {
           get { return m_BakString1; }
           set { base.SetValue("BakString1", ref m_BakString1, value); }
       }

       private string m_BakString2;
       public string BakString2
       {
           get { return m_BakString2; }
           set { base.SetValue("BakString2", ref m_BakString2, value); }
       }

       private string m_BakString3;
       public string BakString3
       {
           get { return m_BakString3; }
           set { base.SetValue("BakString3", ref m_BakString3, value); }
       }

       private int? m_BakInt1;
       public int? BakInt1 {
           get { return m_BakInt1; }
           set { base.SetValue("BakInt1", ref m_BakInt1, value); }
       }

       private decimal? m_BakDecimal;
       public decimal? BakDecimal {
           get { return m_BakDecimal; }
           set { base.SetValue("BakDecimal", ref m_BakDecimal, value); }
       }

   }
}
