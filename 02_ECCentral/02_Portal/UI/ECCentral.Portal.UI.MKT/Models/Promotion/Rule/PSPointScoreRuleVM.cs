using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSPointScoreRuleViewModel : ModelBase
   {
       private Int32? m_PointScore;
       public Int32? PointScore
       {
           get { return this.m_PointScore; }
           set { this.SetValue("PointScore", ref m_PointScore, value);  }
       }

   }
}
