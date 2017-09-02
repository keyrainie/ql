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
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public class QuickLinksRightMenu
    {
        public delegate void DeleteEventHandler(object sender);
        public delegate void ReNameEventHandler(object sender);
        public delegate void DeleteAllEventHandler(object sender);
         

        public event DeleteEventHandler OnRightMenuDeleteClick;
        public event ReNameEventHandler OnRightMenuRenameClick;
        public event DeleteAllEventHandler OnDeleteAllClick;

        public ContextMenu menu = new ContextMenu();
        public object DataContext { get; set; }

        public QuickLinksRightMenu()
        {
            MenuItem ReservoirRename = new MenuItem
            {
                Header = PageResource.LbRenameTitle.ToString()
                //,Icon = new Image { Source = new BitmapImage(new Uri("/Themes/Default/Images/ContextMenu/Cut.png", UriKind.Relative)) }
            };
            ReservoirRename.Click += new RoutedEventHandler(ReservoirRename_Click);
            menu.Items.Add(ReservoirRename);


            MenuItem ReservoirDel = new MenuItem
            {
                Header = PageResource.LbDeleteTitle.ToString()
                // ,Icon = new Image { Source = new BitmapImage(new Uri("/Themes/Default/Images/ContextMenu/Cut.png", UriKind.Relative)) }
            };
            ReservoirDel.Click += new RoutedEventHandler(ReservoirDel_Click);
            menu.Items.Add(ReservoirDel);
        }

        void ReservoirDel_Click(object sender, RoutedEventArgs e)
        {
            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage,"Delete", "QuickLink");
            OnRightMenuDeleteClick(DataContext);
        }

        void ReservoirRename_Click(object sender, RoutedEventArgs e)
        {
            OnRightMenuRenameClick(DataContext);
            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Rename", "QuickLink");
               
        }
    }
}
