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

namespace Newegg.Oversea.Silverlight.CommonDomain.Controls
{
    public class TreeViewDragDropTarget2 : TreeViewDragDropTarget
    {
        public ItemsControl GetRealDropTarget(Microsoft.Windows.DragEventArgs args)
        {
            return base.GetDropTarget(args);
        }
    }

}
