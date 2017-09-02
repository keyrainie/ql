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

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class PlatTextBox : TextBox
    {
        public Visibility StrikethroughVisibility
        {
            get { return (Visibility)GetValue(StrikethroughVisibilityProperty); }
            set { SetValue(StrikethroughVisibilityProperty, value); }
        }

        public static readonly DependencyProperty StrikethroughVisibilityProperty =
            DependencyProperty.Register("StrikethroughVisibility", typeof(Visibility), typeof(PlatTextBox), new PropertyMetadata(Visibility.Collapsed));        

        public PlatTextBox()
        {
            DefaultStyleKey = typeof(PlatTextBox);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = false;
        }
    }
}
