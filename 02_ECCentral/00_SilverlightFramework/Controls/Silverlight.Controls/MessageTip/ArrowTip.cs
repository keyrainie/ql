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
    public class ArrowTip : ContentControl
    {
        public ArrowTip()
        {
            this.DefaultStyleKey = typeof(ArrowTip);
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(ArrowTipOrientation), typeof(ArrowTip), new PropertyMetadata(OrientationPropertyMetadataChanged));
        public ArrowTipOrientation Orientation
        {
            get { return (ArrowTipOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(ArrowTip), new PropertyMetadata(OffsetPropertyMetadataChanged));
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        private Polygon m_Arrow;


        private static void OrientationPropertyMetadataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArrowTip element = sender as ArrowTip;
            element.LayoutArrow();
        }

        private static void OffsetPropertyMetadataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArrowTip element = sender as ArrowTip;
            element.LayoutArrow();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_Arrow = (Polygon)this.GetTemplateChild("ArrowLeft");
            LayoutArrow();
            this.m_Arrow.Visibility = Visibility.Visible;
        }

        private void LayoutArrow()
        {
            if (this.m_Arrow != null)
            {
                this.m_Arrow.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (this.Orientation == ArrowTipOrientation.Left)
            {
                this.m_Arrow = (Polygon)this.GetTemplateChild("ArrowLeft");
            }
            else if (this.Orientation == ArrowTipOrientation.Right)
            {
                this.m_Arrow = (Polygon)this.GetTemplateChild("ArrowRight");
            }
            else if (this.Orientation == ArrowTipOrientation.Top)
            {
                this.m_Arrow = (Polygon)this.GetTemplateChild("ArrowTop");
            }
            else
            {
                this.m_Arrow = (Polygon)this.GetTemplateChild("ArrowBottom");
            }

            if (this.m_Arrow != null)
            {
                if (this.Orientation == ArrowTipOrientation.Left)
                {
                    this.m_Arrow.Margin = new Thickness(-1, Offset, 0, 0);
                }
                else if (this.Orientation == ArrowTipOrientation.Right)
                {
                    this.m_Arrow.Margin = new Thickness(0, Offset, -1, 0);
                }
                else if (this.Orientation == ArrowTipOrientation.Top)
                {
                    this.m_Arrow.Margin = new Thickness(Offset, -1, 0, 0);
                }
                else
                {
                    this.m_Arrow.Margin = new Thickness(Offset, 0, 0, -1);
                }
                this.m_Arrow.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }

     public enum ArrowTipOrientation
     {
         Bottom,
         Left,
         Right,
         Top
     }
}
