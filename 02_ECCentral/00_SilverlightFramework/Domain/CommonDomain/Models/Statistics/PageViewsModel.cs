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
    public class PageViewsModel
    {
        public int ID { get; set; }

        public string Page { get; set; }

        public string Url { get; set; }

        public int Pageviews { get; set; }

        public int UniquePageviews { get; set; }

        public string UserId { get; set; }

        public string Percentage { get; set; }
    }
}
