using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{
    public class BasketInfoVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private List<BasketItemsInfoVM> m_Items;
        public List<BasketItemsInfoVM> Items
        {
            get { return this.m_Items; }
            set { this.SetValue("Items", ref m_Items, value); }
        }

    }
}
