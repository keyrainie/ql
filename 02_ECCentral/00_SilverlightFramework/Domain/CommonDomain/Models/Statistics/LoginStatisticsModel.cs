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

namespace Newegg.Oversea.Silverlight.CommonDomain.Models.Statistics
{
    public class LoginStatisticsModel :  ModelBase
    {
        private int m_Count;
        private DateTime m_InDate;

        public int Count
        {
            get { return this.m_Count; }
            set { this.SetValue("Count", ref m_Count, value); }
        }

        public DateTime InDate
        {
            get { return this.m_InDate; }
            set { this.SetValue("InDate", ref m_InDate, value); }
        }
    }
}
