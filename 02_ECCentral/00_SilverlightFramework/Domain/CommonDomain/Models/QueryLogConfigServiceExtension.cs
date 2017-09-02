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

namespace Newegg.Oversea.Silverlight.CommonDomain.QueryLogConfigService
{
    public partial class LogGlobalRegionBody
    {
        private bool m_isChecked = false;

        public bool IsChecked
        {
            get
            {
                return m_isChecked;
            }
            set
            {
                m_isChecked = value;
            }
        }
    }

    public partial class LogLocalRegionBody
    {
        private bool m_isChecked = false;

        public bool IsChecked
        {
            get
            {
                return this.m_isChecked;
            }
            set
            {
                m_isChecked = value;
            }
        }
    }

    public partial class LogCategoryBody
    {
        private bool m_isChecked;
        public bool IsChecked
        {
            get
            {
                return m_isChecked;
            }
            set
            {
                m_isChecked = value;
            }
        }
    }
}
