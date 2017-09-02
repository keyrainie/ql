using System;
using System.Windows.Controls;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface IAlert : IComponent
    {
        void Alert(string title, string content, MessageType type, ResultHandler handle, Panel container);
    }
}
