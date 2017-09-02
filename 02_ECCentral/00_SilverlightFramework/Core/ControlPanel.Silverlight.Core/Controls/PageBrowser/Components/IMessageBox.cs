using System;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public enum MessageBoxType
    {
        Success = 0,
        Warning,
        Error,
        Information
    }

    public interface IMessageBox : IDisposable
    {
        void Show(string message);

        void Show(string message, MessageBoxType type);

        void Clear();
    }
}
