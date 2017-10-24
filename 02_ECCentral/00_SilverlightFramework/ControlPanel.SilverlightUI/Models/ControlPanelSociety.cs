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

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Models
{
    public class ControlPanelSociety
    {
        
        public int? OrganizationID { get; set; }

        
        public string OrganizationName { get; set; }

        
        public string Province { get; set; }

        
        public string InUser { get; set; }

        
        public DateTime? InDate { get; set; }

        
        public string EditUser { get; set; }

        
        public DateTime? EditDate { get; set; }

        
        public string CommissionID { get; set; }

        
        public string Password { get; set; }
    }

}
