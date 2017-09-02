using System;
using System.Windows.Controls;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public enum DialogResultType
    {
        OK,
        Cancel
    }

    public enum ButtonType
    {
        OKCancel,
        YesNo
    }

    public interface IConfirm : IComponent
    {
        void Confirm(string title, string content, ResultHandler callback, Panel container);
        void Confirm(string title, string content, ResultHandler callback, ButtonType buttonType, Panel container);
    }
}
