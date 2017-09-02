using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Behaviors
{
    public class ButtonTrackerBehavior : Behavior<ButtonBase>
    {
        public string Action { get; set; }

        public string Label { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += new RoutedEventHandler(AssociatedObject_Click);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Click -= new RoutedEventHandler(AssociatedObject_Click);
        }

        void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ButtonBase;

            var action = string.IsNullOrEmpty(this.Action) ? "Click" : this.Action;
            var label = string.IsNullOrEmpty(this.Label) ? this.GetLabel(btn) : this.Label;

            if (CPApplication.Current.CurrentPage != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.EventTracker.TraceEvent(action, "Button:" + label);
            }
        }

        private string GetLabel(ButtonBase btn)
        {
            var value = btn.Content as string;

            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(btn.Name))
                {
                    return btn.Name;
                }

                throw new Exception("Button's Content or Name should be specified!");
            }
            else
            {
                return value;
            }
        }
    }
}
