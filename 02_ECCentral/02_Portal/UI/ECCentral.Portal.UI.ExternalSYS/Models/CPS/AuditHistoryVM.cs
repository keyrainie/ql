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

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class AuditHistoryVM : ModelBase
    {
        public string Note { get; set; }
        public string OpertionUser { get; set; }
        public DateTime OpertionDate { get; set; }
    }
}
