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
using System.Windows.Controls.Primitives;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class AdvanceTooltipService : DependencyObject
    {
        public static readonly DependencyProperty AdvanceTooltipProperty = DependencyProperty.RegisterAttached("AdvanceTooltip",
                        typeof(AdvanceTooltip),
                        typeof(AdvanceTooltipService),
                        new PropertyMetadata(null));


        public static void SetAdvanceTooltip(FrameworkElement element, AdvanceTooltip value)
        {
            if (value != null && value is AdvanceTooltip && GetAdvanceTooltip(element) == null)
            {
                (value as AdvanceTooltip).SetTargetElement(element);
            }
            element.SetValue(AdvanceTooltipProperty, value);
        }
        public static AdvanceTooltip GetAdvanceTooltip(FrameworkElement element)
        {
            return (AdvanceTooltip)element.GetValue(AdvanceTooltipProperty);
        }
    }
}
