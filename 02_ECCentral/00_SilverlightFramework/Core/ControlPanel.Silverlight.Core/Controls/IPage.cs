using System;
using System.Windows;

namespace Newegg.Oversea.Silverlight.Controls
{
    public interface IPage
    {
        string Title { get; }
        PageContext Context { get;}
        DependencyObject Description { get; set; }
    }
}
