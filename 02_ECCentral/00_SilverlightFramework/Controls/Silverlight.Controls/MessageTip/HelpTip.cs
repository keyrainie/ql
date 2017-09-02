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
using Newegg.Oversea.Silverlight.Controls.Resources;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Data;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class HelpTip : ContentControl
    {
        private const long CONST_HIDE_TIMESPAN = 100;

        public bool IsCustomContent { get; set; }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(HelpTip), new PropertyMetadata(PropertyMetadataChanged));
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(HelpBoxPositionType), typeof(HelpTip), new PropertyMetadata(HelpBoxPositionType.Auto));
        public HelpBoxPositionType Position
        {
            get { return (HelpBoxPositionType)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HelpTip), new PropertyMetadata(PropertyMetadataChanged));
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty PopupWidthProperty = DependencyProperty.Register("PopupWidth", typeof(double), typeof(HelpTip), new PropertyMetadata(PropertyMetadataChanged));
        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }


        private Image m_helpICON = null;
        private Popup m_popupHelpBox = null;
        internal Grid m_gridHelpBox = null;
        private Grid m_gridHelpBoxOutter = null;
        
        private Polygon m_arrowLeftOuter2 = null;
        private Polygon m_arrowLeftOuter = null;
        private Polygon m_arrowLeft = null;
        private Polygon m_arrowLeftCoverLayer = null;

        private Polygon m_arrowRightOuter2 = null;
        private Polygon m_arrowRightOuter = null;
        private Polygon m_arrowRight = null;
        private Polygon m_arrowRightCoverLayer = null;

        internal ContentControl m_headerContainer = null;
        private ContentControl m_contentControlBody = null;
        private TextBlock m_textBlockBody = null;

        public HelpTip()
        {
            DefaultStyleKey = typeof(HelpTip);
            this.MouseEnter += new MouseEventHandler(HelpTip_MouseEnter);
            this.MouseLeave += new MouseEventHandler(HelpTip_MouseLeave);
            this.Unloaded += new RoutedEventHandler(HelpTip_Unloaded);
            this.Loaded += new RoutedEventHandler(HelpTip_Loaded);
        }

        void HelpTip_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_gridHelpBox != null)
            {
                m_gridHelpBox.MouseEnter += new MouseEventHandler(m_gridHelpBox_MouseEnter);
                m_gridHelpBox.MouseLeave += new MouseEventHandler(m_gridHelpBox_MouseLeave);
                m_gridHelpBox.MouseLeftButtonUp += new MouseButtonEventHandler(m_gridHelpBox_MouseLeftButtonUp);
            }

            if (m_textBlockBody != null && 
                    m_textBlockBody.GetBindingExpression(TextBlock.TextProperty) == null)
            {
                m_textBlockBody.SetBinding(TextBlock.TextProperty, 
                    new Binding("Content") { RelativeSource = new RelativeSource( RelativeSourceMode.TemplatedParent) });
            }
        }

        void HelpTip_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_gridHelpBox != null)
            {
                m_gridHelpBox.MouseEnter -= new MouseEventHandler(m_gridHelpBox_MouseEnter);
                m_gridHelpBox.MouseLeave -= new MouseEventHandler(m_gridHelpBox_MouseLeave);
                m_gridHelpBox.MouseLeftButtonUp -= new MouseButtonEventHandler(m_gridHelpBox_MouseLeftButtonUp);
            }

            if (m_textBlockBody != null)
            {
                m_textBlockBody.ClearValue(TextBlock.TextProperty);
            }
        }

        private bool m_isMovedHelpTipBox = false;

        void HelpTip_MouseLeave(object sender, MouseEventArgs e)
        {
            //System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(OnDelayCloseHelpTipBox));
            //thread.Start();
            DispatcherTimer timer = new DispatcherTimer() 
            {
                Interval = new TimeSpan(CONST_HIDE_TIMESPAN)
            };
            timer.Tick += new EventHandler(timer_Tick);
            
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            if (this.Parent != null)
            {
                if (!m_isMovedHelpTipBox)
                {
                    m_popupHelpBox.IsOpen = false;
                }
                m_isMovedHelpTipBox = false;
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

        //void OnDelayCloseHelpTipBox()
        //{
        //    System.Threading.Thread.Sleep(100);
        //    this.Dispatcher.BeginInvoke(() =>
        //    {
        //        if (!m_isMovedHelpTipBox)
        //        {
        //            m_popupHelpBox.IsOpen = false;
        //        }
        //        m_isMovedHelpTipBox = false;
        //        VisualStateManager.GoToState(this, "Normal", false);
        //    });
        //}

        void HelpTip_MouseEnter(object sender, MouseEventArgs e)
        {
            m_popupHelpBox.IsOpen = true;
            this.SetHelpBoxPoint();
            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        private static void PropertyMetadataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HelpTip element = sender as HelpTip;
            if (e.Property == HelpTip.PopupWidthProperty)
            {
                if (element.m_gridHelpBox != null)
                {
                    element.m_gridHelpBox.Width = Convert.ToDouble(e.NewValue);
                }
            }
            else if (e.Property == HelpTip.HeaderProperty)
            {
                if (element.m_headerContainer != null)
                {
                    element.m_headerContainer.Content = e.NewValue;
                }
            }
            else if (e.Property == HelpTip.HeaderTemplateProperty)
            {
                if (element.m_headerContainer != null)
                {
                    element.m_headerContainer.ContentTemplate = e.NewValue as DataTemplate;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_headerContainer = this.GetTemplateChild("headerContainer") as ContentControl;

            if (m_helpICON == null)
            {
                m_helpICON = (Image)this.GetTemplateChild("ImageHelpICON");
            }
            if (m_popupHelpBox == null)
            {
                m_popupHelpBox = (Popup)this.GetTemplateChild("PopupHelpBox");
            }
            if (m_gridHelpBox == null)
            {
                m_gridHelpBox = (Grid)this.GetTemplateChild("GridHelpBox");
                
            }
            if (m_gridHelpBoxOutter == null)
            {
                m_gridHelpBox = (Grid)this.GetTemplateChild("GridHelpBoxOutter");
            }
            if (m_arrowLeftOuter2 == null)
            {
                m_arrowLeftOuter2 = (Polygon)this.GetTemplateChild("ArrowLeftOuter2");
            }
            if (m_arrowLeftOuter == null)
            {
                m_arrowLeftOuter = (Polygon)this.GetTemplateChild("ArrowLeftOuter");
            }
            if (m_arrowLeft == null)
            {
                m_arrowLeft = (Polygon)this.GetTemplateChild("ArrowLeft");
            }
            if (m_arrowLeftCoverLayer == null)
            {
                m_arrowLeftCoverLayer = (Polygon)this.GetTemplateChild("ArrowLeftCoverLayer");
            }

            if (m_arrowRightOuter2 == null)
            {
                m_arrowRightOuter2 = (Polygon)this.GetTemplateChild("ArrowRightOuter2");
            }
            if (m_arrowRightOuter == null)
            {
                m_arrowRightOuter = (Polygon)this.GetTemplateChild("ArrowRightOuter");
            }
            if (m_arrowRight == null)
            {
                m_arrowRight = (Polygon)this.GetTemplateChild("ArrowRight");
            }
            if (m_arrowRightCoverLayer == null)
            {
                m_arrowRightCoverLayer = (Polygon)this.GetTemplateChild("ArrowRightCoverLayer");
            }
            //if (m_helpICON != null)
            //{
            //    m_helpICON.MouseEnter += new MouseEventHandler(m_helpICON_MouseEnter);
            //    m_helpICON.MouseLeave += new MouseEventHandler(m_helpICON_MouseLeave);
            //}
            //if (m_gridHelpBoxOutter != null)
            //{
            //    m_gridHelpBoxOutter.MouseEnter += new MouseEventHandler(m_gridHelpBoxOutter_MouseEnter);
            //    m_gridHelpBoxOutter.MouseLeave += new MouseEventHandler(m_gridHelpBoxOutter_MouseLeave);
            //}
            if (m_contentControlBody == null)
            {
                m_contentControlBody = (ContentControl)this.GetTemplateChild("ContentControlMessageBody");
            }
            if (m_textBlockBody == null)
            {
                m_textBlockBody = (TextBlock)this.GetTemplateChild("TextBlockMessageBody");
            }
            if (IsCustomContent)
            {
                if (m_contentControlBody != null)
                {
                    m_contentControlBody.Visibility = System.Windows.Visibility.Visible;
                }
                if (m_textBlockBody != null)
                {
                    m_textBlockBody.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else if (!IsCustomContent)
            {
                if (m_contentControlBody != null)
                {
                    m_contentControlBody.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (m_textBlockBody != null)
                {
                    m_textBlockBody.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        void m_gridHelpBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_popupHelpBox.IsOpen = false;
        }

        void m_gridHelpBox_MouseLeave(object sender, MouseEventArgs e)
        {
            m_popupHelpBox.IsOpen = false;
        }

        void m_gridHelpBox_MouseEnter(object sender, MouseEventArgs e)
        {
            m_popupHelpBox.IsOpen = true;
            m_isMovedHelpTipBox = true;
        }

        private void SetRightStyle()
        {
            m_arrowLeftOuter2.Visibility = Visibility.Visible;
            m_arrowLeftOuter.Visibility = Visibility.Visible;
            m_arrowLeft.Visibility = Visibility.Visible;
            m_arrowLeftCoverLayer.Visibility = Visibility.Visible;

            m_arrowRightOuter2.Visibility = Visibility.Collapsed;
            m_arrowRightOuter.Visibility = Visibility.Collapsed;
            m_arrowRight.Visibility = Visibility.Collapsed;
            m_arrowRightCoverLayer.Visibility = Visibility.Collapsed;
            
            m_popupHelpBox.VerticalOffset = -40;
            m_popupHelpBox.HorizontalOffset = m_helpICON.Width - 2;
            m_gridHelpBox.RenderTransformOrigin = new Point(0,0);
        }

        private void SetLeftStyle()
        {
            m_arrowLeftOuter2.Visibility = Visibility.Collapsed;
            m_arrowLeftOuter.Visibility = Visibility.Collapsed;
            m_arrowLeft.Visibility = Visibility.Collapsed;
            m_arrowLeftCoverLayer.Visibility = Visibility.Collapsed;

            m_arrowRightOuter2.Visibility = Visibility.Visible;
            m_arrowRightOuter.Visibility = Visibility.Visible;
            m_arrowRight.Visibility = Visibility.Visible;
            m_arrowRightCoverLayer.Visibility = Visibility.Visible;

            m_popupHelpBox.VerticalOffset = -40;
            m_popupHelpBox.HorizontalOffset = -this.PopupWidth + 2;
            m_gridHelpBox.RenderTransformOrigin = new Point(0, 1);
        }

        private void SetHelpBoxPoint()
        {
            GeneralTransform objGeneralTransform = m_helpICON.TransformToVisual(Application.Current.RootVisual as UIElement);
            Point helpICONPoint = objGeneralTransform.Transform(new Point(0, 0));
            if (Position == HelpBoxPositionType.Auto)
            {
                if (helpICONPoint.X < this.PopupWidth)
                {
                     this.SetRightStyle();
                }
                else
                {
                    this.SetLeftStyle();
                }
            }
            else if (Position == HelpBoxPositionType.Left)
            {
                this.SetLeftStyle();
            }
            else if (Position == HelpBoxPositionType.Right)
            {
                this.SetRightStyle();
            }
        }
    }

    public enum HelpBoxPositionType
    {
        Auto,
        Left,
        Right
    }

    public class SafeEventHandler<T>
  where T : EventArgs
    {
        private WeakReference m_TargetRef;
        private MethodInfo m_Method;
        private EventHandler<T> m_Handler;

        public SafeEventHandler(EventHandler<T> eventHandler)
        {
            m_TargetRef = new WeakReference(eventHandler.Target);
            m_Method = eventHandler.Method;
            m_Handler = Invoke;
        }

        public void Invoke(object sender, T e)
        {
            object target = m_TargetRef.Target;
            if (target != null)
                m_Method.Invoke(target, new object[] { sender, e });
        }

        public static implicit operator EventHandler<T>(SafeEventHandler<T> handler)
        {
            return handler.m_Handler;
        }
    }
}
