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
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class LoadingControl : Control
    {
        private Storyboard m_storyboard1;
        private List<Ellipse> m_Ellipses;
        private Ellipse m_e1, m_e2, m_e3, m_e4, m_e5, m_e6, m_e7, m_e8, m_e9, m_e10;

        public LoadingControl()
        {
            DefaultStyleKey = typeof(LoadingControl);
        }

        public static readonly DependencyProperty SpinColorProperty =
                DependencyProperty.Register("SpinColor", typeof(Brush), typeof(LoadingControl), null);

        public Brush SpinColor
        {
            get { return (Brush)GetValue(SpinColorProperty); }
            set { SetValue(SpinColorProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_storyboard1 = GetTemplateChild("Storyboard1") as Storyboard;
            m_e1 = GetTemplateChild("E1") as Ellipse;
            m_e2 = GetTemplateChild("E2") as Ellipse;
            m_e3 = GetTemplateChild("E3") as Ellipse;
            m_e4 = GetTemplateChild("E4") as Ellipse;
            m_e5 = GetTemplateChild("E5") as Ellipse;
            m_e6 = GetTemplateChild("E6") as Ellipse;
            m_e7 = GetTemplateChild("E7") as Ellipse;
            m_e8 = GetTemplateChild("E8") as Ellipse;
            m_e9 = GetTemplateChild("E9") as Ellipse;
            m_e10 = GetTemplateChild("E10") as Ellipse;

            m_Ellipses = new List<Ellipse>
            {
                m_e1,m_e2,m_e3,m_e4,m_e5,m_e6,m_e7,m_e8,m_e9,m_e10
            };
            m_Ellipses.ForEach(i => i.Fill = SpinColor);

            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Start()
        {
            if (m_storyboard1 != null)
            {
                this.Visibility = System.Windows.Visibility.Visible;
                m_storyboard1.Begin();
            }
        }

        public void Stop()
        {
            if (m_storyboard1 != null)
            {
                m_storyboard1.Stop();
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
