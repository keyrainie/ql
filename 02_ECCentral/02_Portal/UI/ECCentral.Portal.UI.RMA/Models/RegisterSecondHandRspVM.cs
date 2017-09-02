using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RegisterSecondHandRspVM : ModelBase
    {
        private int? m_SysNo;
        public int? SysNo
        {
            get { return m_SysNo; }
            set { SetValue("SysNo", ref m_SysNo, value); }
        }

        private string m_ProductID;
        public string ProductID 
        {
            get { return m_ProductID; }
            set { SetValue("ProductID", ref m_ProductID, value); }
        }
    }
}
