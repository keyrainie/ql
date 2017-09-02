using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Newegg.Oversea.Silverlight.Core.Components;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class AttachAuthComponent
    {
        public static string GetFunction(DependencyObject d)
        {
            return (string)d.GetValue(FunctionProperty);
        }

        public static void SetFunction(DependencyObject d, string functionName)
        {
            d.SetValue(FunctionProperty, functionName);
        }

        public static readonly DependencyProperty FunctionProperty = DependencyProperty.RegisterAttached(
            "Function",
            typeof(string),
            typeof(AttachAuthComponent),
            new PropertyMetadata(new PropertyChangedCallback(OnFunctionPropertyChanged)));

        static void OnFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;

            string functionPoint = e.NewValue as string;

            if (string.IsNullOrEmpty(functionPoint))
            {
                throw new Exception(string.Format("Invalid Function Name on control {0}", element.Name));
            }           
            //Check permission
            var auth = ComponentFactory.GetComponent<IAuth>();

            var flag =auth.HasFunction(functionPoint);
            
            var btn = element as Control;
            if (btn != null)
            {
                btn.IsEnabled = flag;
            }            
        }       
    }
}
