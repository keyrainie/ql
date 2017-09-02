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
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public class ExportAllDataEventArgs : EventArgs
    {
        public List<Dictionary<string, string>> VisibleColumns { get; private set; }

        public ExportAllDataEventArgs(List<Dictionary<string, string>> visibleColumns)
        {
            this.VisibleColumns = visibleColumns;
        }
    }
}
