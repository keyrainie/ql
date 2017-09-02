using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorRefundItemInfoVM : ModelBase
    {
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_RefundSysNo;
        public Int32? RefundSysNo
        {
            get { return this.m_RefundSysNo; }
            set { this.SetValue("RefundSysNo", ref m_RefundSysNo, value); }
        }

        private Int32? m_RegisterSysNo;
        public Int32? RegisterSysNo
        {
            get { return this.m_RegisterSysNo; }
            set { this.SetValue("RegisterSysNo", ref m_RegisterSysNo, value); }
        }

        private String m_ProductID;
        public String ProductID
        {
            get { return this.m_ProductID; }
            set { this.SetValue("ProductID", ref m_ProductID, value); }
        }

        private String m_ProductName;
        public String ProductName
        {
            get { return this.m_ProductName; }
            set { this.SetValue("ProductName", ref m_ProductName, value); }
        }

        private Decimal? m_Cost;
        public Decimal? Cost
        {
            get { return this.m_Cost; }
            set { this.SetValue("Cost", ref m_Cost, value); }
        }

        private Decimal? m_RefundCash;
        public Decimal? RefundCash
        {
            get { return this.m_RefundCash; }
            set { this.SetValue("RefundCash", ref m_RefundCash, value); }
        }

    }
}
