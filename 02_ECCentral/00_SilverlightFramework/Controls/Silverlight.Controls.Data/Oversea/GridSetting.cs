using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class GridSetting
    {
        public string GridGuid { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public int PageSize { get; set; }

        public uint RowBackground { get; set; }

        public uint AlternatingRowBackground { get; set; }

        public double RowHeight { get; set; }

        public List<GridColumn> Columns { get; set; }
    }

    public class GridColumn
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public DataGridLength Width { get; set; }

        public double ActualWidth { get; set; }

        public bool IsHided { get; set; }

        public bool IsFreezed { get; set; }

        public string DisplayName { get; set; }
    }
}
