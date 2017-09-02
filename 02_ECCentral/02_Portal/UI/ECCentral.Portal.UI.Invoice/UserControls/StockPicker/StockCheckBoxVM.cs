using System;
using System.Collections.Generic;
using System.Windows;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.UserControls.StockPicker
{
    public class StockCheckBoxVM : ModelBase
    {
        private bool m_IsChecked;
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        private string m_StockName;
        public string StockName
        {
            get
            {
                return m_StockName;
            }
            set
            {
                base.SetValue("StockName", ref m_StockName, value);
            }
        }

        private string m_StockSysNo;
        public string StockSysNo
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
    }
}