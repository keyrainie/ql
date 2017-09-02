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
    public class PopupButton :  ToggleButton
    {
        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(PopupButton), null);

        public object PopupContent
        {
            get { return GetValue(PopupButton.PopupContentProperty); }
            set { this.SetValue(PopupButton.PopupContentProperty, value); }
        }

        private Popup m_popup;
        private Grid m_popupContent;
        private FrameworkElement m_popupRoot;
        private Canvas m_canvasOutside;
        private Canvas m_canvasPopupOutside;
        

        public PopupButton()
        {
            this.DefaultStyleKey = typeof(PopupButton);
            this.Click += new RoutedEventHandler(PopupButton_Click);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.m_popup = this.GetTemplateChild("PopupControl") as Popup;
            this.m_popup.Opened += new EventHandler(m_popup_Opened);
            this.m_popup.Closed += new EventHandler(m_popup_Closed);


            this.m_popupContent = this.GetTemplateChild("PopContent") as Grid;
            this.m_popupRoot = this.m_popup.Child as FrameworkElement;

            if (this.m_popup != null && this.m_popupRoot != null)
            {
                if (this.m_canvasOutside == null)
                {
                    this.m_popupRoot.SizeChanged += new SizeChangedEventHandler(m_popupRoot_SizeChanged);
                    this.m_canvasPopupOutside = new Canvas();
                    this.m_canvasPopupOutside.Background = new SolidColorBrush(Colors.Transparent);
                    this.m_canvasPopupOutside.MouseLeftButtonUp += new MouseButtonEventHandler(m_canvasPopupOutside_MouseLeftButtonUp);

                    this.m_canvasOutside = new Canvas();

                    this.m_popup.Child = this.m_canvasOutside;
                    this.m_canvasOutside.Children.Add(this.m_canvasPopupOutside);
                    this.m_canvasOutside.Children.Add(this.m_popupRoot);
                }
            }
        }

        void m_popup_Closed(object sender, EventArgs e)
        {
            this.IsChecked = false;
        }

        void m_popup_Opened(object sender, EventArgs e)
        {
            this.IsChecked = true;
        }

        void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            this.ArrangePopup();
            this.m_popup.IsOpen = !this.m_popup.IsOpen;
        }

        void m_canvasPopupOutside_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.m_popup.IsOpen = false;
        }

        void m_popupRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }

        private void ArrangePopup()
        {
            if (((this.m_popup != null) && (this.m_popupRoot != null)) && this.m_popup.IsOpen)
            {
                System.Windows.Interop.Content content = Application.Current.Host.Content;
                double actualWidth = content.ActualWidth;
                double actualHeight = content.ActualHeight;
                double num3 = this.m_popupRoot.ActualWidth;
                double num4 = this.m_popupRoot.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((num3 != 0.0) && (num4 != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this.m_popupRoot.TransformToVisual(null);
                    }
                    catch
                    {
                        //Don't need to do nothing.
                    }
                    if (transform != null)
                    {
                        Point point = new Point(0.0, 0.0);
                        Point point2 = new Point(1.0, 0.0);
                        Point point3 = new Point(0.0, 1.0);
                        Point point4 = transform.Transform(point);
                        Point point5 = transform.Transform(point2);
                        Point point6 = transform.Transform(point3);
                        double x = point4.X;
                        double y = point4.Y;
                        double num7 = base.ActualHeight;
                        double num8 = base.ActualWidth;

                        this.m_popup.HorizontalOffset = 0.0;
                        this.m_popup.VerticalOffset = 0.0;
                        this.m_canvasPopupOutside.Width = actualWidth;
                        this.m_canvasPopupOutside.Height = actualHeight;
                        Matrix identity = Matrix.Identity;
                        identity.M11 = point5.X - point4.X;
                        identity.M12 = point5.Y - point4.Y;
                        identity.M21 = point6.X - point4.X;
                        identity.M22 = point6.Y - point4.Y;
                        identity.OffsetX -= point4.X;
                        identity.OffsetY -= point4.Y;
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = identity;
                        this.m_canvasPopupOutside.RenderTransform = transform2;
                        this.m_popupRoot.MinWidth = num8;
                        this.m_popupRoot.MaxWidth = actualWidth;
                        this.m_popupRoot.MinHeight = num7;
                        this.m_popupRoot.MaxHeight = actualHeight;
                        this.m_popupRoot.HorizontalAlignment = HorizontalAlignment.Left;
                        this.m_popupRoot.VerticalAlignment = VerticalAlignment.Top;



                        try
                        {
                            transform = this.TransformToVisual(null);
                        }
                        catch
                        {
                            //Don't need to do nothing.
                        }
                        point4 = transform.Transform(point);
                        if ((this.m_popupRoot.ActualWidth - base.ActualWidth) <= point4.X)
                        {
                            Canvas.SetLeft(this.m_popupRoot, -(this.m_popupRoot.ActualWidth - this.ActualWidth));
                        }
                        else
                        {
                            Canvas.SetLeft(this.m_popupRoot, 0);
                        }

                        if (this.m_popupRoot.ActualHeight <= (actualHeight - point4.Y - base.ActualHeight))
                        {
                            Canvas.SetTop(this.m_popupRoot, base.ActualHeight);
                        }
                        else
                        {
                            Canvas.SetTop(this.m_popupRoot, -this.m_popupRoot.ActualHeight);
                        }




                        bool isTop = false;
                        bool isLeft = false;
                        double popY = Canvas.GetTop(this.m_popupRoot);
                        double popX = Canvas.GetLeft(this.m_popupRoot);
                        if (popY < 0)
                        {
                            isTop = true;
                        }
                        if (popX < 0)
                        {
                            isLeft = true;
                        }

                        if (isTop && isLeft)
                        {
                            this.m_popupContent.Margin = new Thickness(0, 1, 0, 0);
                            VisualStateManager.GoToState(this, "PopTopLeftState", false);
                        }
                        else if (!isTop && isLeft)
                        {
                            this.m_popupContent.Margin = new Thickness(0, -1, 0, 0);
                            VisualStateManager.GoToState(this, "PopBottomLeftState", false);
                        }
                        else if (!isTop && !isLeft)
                        {
                            this.m_popupContent.Margin = new Thickness(0, -1, 0, 0);
                            VisualStateManager.GoToState(this, "PopBottomRightState", false);
                        }
                        else if (isTop && !isLeft)
                        {
                            this.m_popupContent.Margin = new Thickness(0, 1, 0, 0);
                            VisualStateManager.GoToState(this, "PopTopRightState", false);
                        }

                    }
                }
            }
        }

    }
}
