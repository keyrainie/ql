using System;
using System.Windows.Controls;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface IComponent:IDisposable
    {
        string Name { get; }
        string Version { get; }

        void InitializeComponent(IPageBrowser browser);
        object GetInstance(TabItem tab);
    }
}
