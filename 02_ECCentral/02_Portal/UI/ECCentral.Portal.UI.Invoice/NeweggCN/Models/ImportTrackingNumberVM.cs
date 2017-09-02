using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Models
{
    public class ImportTrackingNumberVM : ModelBase
    {
        public ImportTrackingNumberVM()
        {
            StockList = new List<CodeNamePair>();
        }

        private int? m_StockSysNo;
        public int? StockSysNo
        {
            get
            {
                return m_StockSysNo;
            }
            set
            {
                base.SetValue("StockSysNo", ref m_StockSysNo, value);
            }
        }

        private List<CodeNamePair> m_StockList;
        public List<CodeNamePair> StockList
        {
            get
            {
                return m_StockList;
            }
            set
            {
                base.SetValue("StockList", ref m_StockList, value);
            }
        }
    }
}