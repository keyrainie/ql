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
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core.Components
{
    public interface IAssemblyLoader : IComponent
    {
        void LoadShareAssembly(Action callback);
    }

    public class AssemblyInfo
    {
        public string Name { get; set; }
    }
}
