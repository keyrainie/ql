using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using Newegg.Oversea.Silverlight.CommonDomain.Models.Statistics;

namespace Newegg.Oversea.Silverlight.CommonDomain.Views
{
    [View(NeedAccess = false)]
    public partial class TestPage : PageBase
    {
        public TestPage()
        {
            InitializeComponent();

            var list = new ObservableCollection<PageViewsModel>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new PageViewsModel
                {
                    UserId = "az73",
                    Page = "Query Log",
                    Pageviews = 100,
                    UniquePageviews = 14,
                    Url = "/Views/QueryLog",
                    Percentage = "31%"
                });
            }

            this.DataContext = list;

        }
    }
}
