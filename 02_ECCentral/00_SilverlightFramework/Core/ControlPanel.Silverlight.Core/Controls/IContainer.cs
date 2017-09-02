using System;
using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Controls
{
    public interface IContainer
    {
        #region EventHandler

        event EventHandler<LoadedMoudleEventArgs> LoadModule;

        event EventHandler<LoadProgressEventArgs> LoadProgress;

        #endregion

        #region Property
        UIElementCollection Children { get; }
        #endregion

        void Load(Request request);

        void Load(string url);

        bool CancelLoadModuleAsync(Request request);
    }
}
