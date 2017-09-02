using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.PO.Models.Vendor
{
    public class SecondDomainQueryVM : ModelBase
    {
        public SecondDomainQueryVM()
        {
            m_vendorSysNoList = new ObservableCollection<int>();
        }

        private ObservableCollection<int> m_vendorSysNoList;

        public ObservableCollection<int> VendorSysNoList
        {
            get { return m_vendorSysNoList; }
            set { base.SetValue("VendorSysNoList", ref m_vendorSysNoList, value); }
        }
    }
}
