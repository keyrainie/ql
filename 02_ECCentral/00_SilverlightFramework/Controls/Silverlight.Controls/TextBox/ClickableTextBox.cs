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

namespace Newegg.Oversea.Silverlight.Controls
{
    public class ClickableTextBox : TextBox
    {
        public EventHandler<MouseButtonEventArgs> m_click;

        public event EventHandler<MouseButtonEventArgs> Click
        {
            add
            {
                m_click += value;
            }
            remove
            {
                m_click -= value;
            }
        }

        public EventHandler<EventArgs> m_doubleClick;

        public event EventHandler<EventArgs> DoubleClick
        {
            add
            {
                m_doubleClick += value;
            }
            remove
            {
                m_doubleClick -= value;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (m_doubleClick != null)
            {
                if (e.ClickCount == 2)
                {
                    m_doubleClick(this, new EventArgs());
                }
            }
            else if (m_click != null)
            {
                m_click(this, e);
            }
            base.OnMouseLeftButtonDown(e);
        }

        public ClickableTextBox()
        {
            this.DefaultStyleKey = typeof(ClickableTextBox);
        }
    }
}
