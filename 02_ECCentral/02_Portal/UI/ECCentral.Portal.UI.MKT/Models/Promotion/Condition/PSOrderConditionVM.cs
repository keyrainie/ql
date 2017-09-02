using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
   public class PSOrderConditionViewModel : ModelBase
   {
       private List<int> m_PayTypeSysNoList;
       public List<int> PayTypeSysNoList
       {
           get { return this.m_PayTypeSysNoList; }
           set { this.SetValue("PayTypeSysNoList", ref m_PayTypeSysNoList, value);  }
       }

       private List<int> m_ShippingTypeSysNoList;
       public List<int> ShippingTypeSysNoList
       {
           get { return this.m_ShippingTypeSysNoList; }
           set { this.SetValue("ShippingTypeSysNoList", ref m_ShippingTypeSysNoList, value);  }
       }

       private string m_OrderMinAmount;
       /// <summary>
       /// 订单金额下限
       /// </summary>
       [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessage = "必须是大于等于0的数字")]
       public string OrderMinAmount
       {
           get { return this.m_OrderMinAmount; }
           set { this.SetValue("OrderMinAmount", ref m_OrderMinAmount, value);  }
       }
        

   }
}
