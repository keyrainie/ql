using System;
using System.Windows.Controls;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface ILoadingSpin : IComponent
    {
        bool IsOpen { get; }

        void Show();
        void Show(Panel container);
        void Hide();
        void Hide(Panel container);
        void Clear();
        void Clear(Panel container);
    }
}
