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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOEnergyVM : ModelBase
    {
        private int? m_SysNo;
        public int? SysNo
        {
            get { return m_SysNo; }
            set { SetValue("SysNo", ref m_SysNo, value); }
        }

        private int? m_SoSysNo;
        public int? SoSysNo
        {
            get { return m_SoSysNo; }
            set { SetValue("SoSysNo", ref m_SoSysNo, value); }
        }

        private string m_CertificaterName;
        public string CertificaterName
        {
            get { return m_CertificaterName; }
            set { SetValue("CertificaterName", ref m_CertificaterName, value); }
        }

        private string m_CertificateNo;
        public string CertificateNo
        {
            get { return m_CertificateNo; }
            set { SetValue("CertificateNo", ref m_CertificateNo, value); }
        }

        private int? m_CertificateType;
        public int? CertificateType
        {
            get { return m_CertificateType; }
            set { SetValue("CertificateType", ref m_CertificateType, value); }
        }
    }
}
