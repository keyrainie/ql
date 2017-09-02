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

namespace Newegg.Oversea.Silverlight.CommonDomain.Models.Statistics
{
    public class PageActionsModel
    {
        public int ID { get; set; }

        public string Action { get; set; }

        public string Label { get; set; }

        public int Count { get; set; }
    }
}
