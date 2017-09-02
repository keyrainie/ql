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
    public class ListBoxItemExt : ListBoxItem
    {
        public event RoutedEventHandler ItemDeleted;
        public event RoutedEventHandler ItemSelected;

        private const string Name_DeleteButton = "DeleteButton";
        private ButtonBase DeleteButton { get; set; }

        public ListBoxItemExt()
        {
            DefaultStyleKey = typeof(ListBoxItemExt);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.DeleteButton != null)
            {
                this.DeleteButton.Click -= new RoutedEventHandler(DeleteButton_Click);
            }
            this.DeleteButton = GetTemplateChild(Name_DeleteButton) as ButtonBase;
            if (this.DeleteButton != null)
            {
                this.DeleteButton.Click += new RoutedEventHandler(DeleteButton_Click);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (this.ItemSelected != null)
            {
                this.ItemSelected(this, new RoutedEventArgs());
            }
        }

        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ItemDeleted != null)
            {
                this.ItemDeleted(this, new RoutedEventArgs());
            }
        }
    }
}
