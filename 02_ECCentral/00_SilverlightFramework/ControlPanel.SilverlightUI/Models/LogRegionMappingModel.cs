using System;
using System.Net;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Models
{
    public class LogRegionMappingModel
    {
        public string LocalRegion { get; set; }

        public List<string> NamespaceCollection { get; set; }
    }
}
