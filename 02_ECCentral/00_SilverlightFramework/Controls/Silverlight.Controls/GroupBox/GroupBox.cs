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
using System.ComponentModel;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class GroupBox : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(GroupBox), null);
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(GroupBox), new PropertyMetadata(null));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBox), null);
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GroupBox), null);

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public Brush HeaderForeground
        {
            get { return (Brush)GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        private RectangleGeometry FullRect;
        private RectangleGeometry HeaderRect;
        private ContentControl HeaderContainer;

        public GroupBox()
        {
            DefaultStyleKey = typeof(GroupBox);
            this.SizeChanged += GroupBox_SizeChanged;
            this.Loaded += new RoutedEventHandler(GroupBox_Loaded);
        }

        void GroupBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (HeaderContainer != null)
            {
                this.UpdateLayout();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FullRect = (RectangleGeometry)GetTemplateChild("FullRect");
            HeaderRect = (RectangleGeometry)GetTemplateChild("HeaderRect");
            HeaderContainer = (ContentControl)GetTemplateChild("HeaderContainer");
            if (HeaderContainer != null)
            {
                ((Border)GetTemplateChild("Border")).SetValue(Border.CornerRadiusProperty, this.CornerRadius);
                HeaderContainer.SizeChanged += HeaderContainer_SizeChanged;
            }           
        }

        private void GroupBox_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            FullRect.Rect = new Rect(new Point(), e.NewSize);
        }

        private void HeaderContainer_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            HeaderRect.Rect = new Rect(new Point(HeaderContainer.Margin.Left, 0), e.NewSize);
        }
    } 
}
