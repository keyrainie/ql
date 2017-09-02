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
    public class RichButton : ContentControl
    {
        public static readonly DependencyProperty RichContentProperty = DependencyProperty.Register("RichContent", typeof(object), typeof(RichButton), null);
        public static readonly DependencyProperty CanPopupProperty = DependencyProperty.Register("CanPopup", typeof(bool), typeof(RichButton), new PropertyMetadata(true, OnSetCanPopupCallback));

        public event RoutedEventHandler Click;

        public object RichContent
        {
            get { return (object)GetValue(RichContentProperty); }
            set { SetValue(RichContentProperty, value); }
        }

        public bool CanPopup
        {
            get { return (bool)GetValue(CanPopupProperty); }
            set { SetValue(CanPopupProperty, value); }
        }

        private static void OnSetCanPopupCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RichButton button = dependencyObject as RichButton;
            if (button != null)
            {
                VisualStateManager.GoToState(button, (bool)e.NewValue ? "Normal" : "Simple", false);
            }
        }

        public RichButton()
        {
            DefaultStyleKey = typeof(RichButton);
        }

        private Popup _popup;
        private FrameworkElement _popupRoot;
        private Border _borderRichContent;
        private Canvas _canvasOutside;
        private Canvas _canvasPopupOutside;

        private Grid _popContent;

        private Border _clickButton;
        private Grid _contentButton;
        private Border _popButton;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._popup = GetTemplateChild("RichContentPopup") as Popup;
            this._popup.Opened += new EventHandler(_popup_Opened);
            this._popup.Closed += new EventHandler(_popup_Closed);

            this._popContent = GetTemplateChild("PopContent") as Grid;

            this._popupRoot = this._popup.Child as FrameworkElement;
            if (this._popup != null && this._popupRoot != null)
            {
                if (this._canvasOutside == null)
                {
                    this._popupRoot.SizeChanged += new SizeChangedEventHandler(_popupRoot_SizeChanged);
                    this._canvasPopupOutside = new Canvas();
                    this._canvasPopupOutside.Background = new SolidColorBrush(Colors.Transparent);
                    this._canvasPopupOutside.MouseLeftButtonUp += new MouseButtonEventHandler(_canvasPopupOutside_MouseLeftButtonUp);

                    this._canvasOutside = new Canvas();

                    this._popup.Child = this._canvasOutside;
                    this._canvasOutside.Children.Add(this._canvasPopupOutside);
                    this._canvasOutside.Children.Add(this._popupRoot);
                }
            }

            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(RichButton_IsEnabledChanged);

            this._contentButton = GetTemplateChild("ContentButton") as Grid;

            this._popButton = GetTemplateChild("PopButton") as Border;
            this._popButton.MouseEnter += new MouseEventHandler(_popButton_MouseEnter);
            this._popButton.MouseLeave += new MouseEventHandler(_popButton_MouseLeave);

            this._popButton.MouseLeftButtonUp += new MouseButtonEventHandler(_popButton_MouseLeftButtonUp);


            this._clickButton = GetTemplateChild("ClickButton") as Border;
            this._clickButton.MouseLeftButtonUp += new MouseButtonEventHandler(_clickButton_MouseLeftButtonUp);
            this._clickButton.MouseEnter += new MouseEventHandler(_clickButton_MouseEnter);
            this._clickButton.MouseLeave += new MouseEventHandler(_clickButton_MouseLeave);

            VisualStateManager.GoToState(this,  "Normal", false);

            this.OnIsEnabledPropertyChanged();
        }

        void _popButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this._popup.IsOpen)
            {
                VisualStateManager.GoToState(this, "PopButton_Normal", false);
            }
        }

        void _popButton_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "PopButton_Light", false);
        }

        void _clickButton_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        void _clickButton_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ClickButton_MouseOver", false);
        }

        void RichButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnIsEnabledPropertyChanged();
        }

        void _clickButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }

        void _canvasPopupOutside_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.ClosePopup();
        }

        void _popButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ArrangePopup();
            this._popup.IsOpen = !this._popup.IsOpen;
        }

        void _popup_Opened(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "PopButton_Light", false);
        }

        void _popup_Closed(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "PopButton_Normal", false);
        }

        void _popupRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }


        private void OnIsEnabledPropertyChanged()
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", false);
            }
        }

        public void ClosePopup()
        {
            this._popup.IsOpen = false;
        }

        private void ArrangePopup()
        {
            if (((this._popup != null) && (this._popupRoot != null)) && this._popup.IsOpen)
            {
                System.Windows.Interop.Content content = Application.Current.Host.Content;
                double actualWidth = content.ActualWidth;
                double actualHeight = content.ActualHeight;
                double num3 = this._popupRoot.ActualWidth;
                double num4 = this._popupRoot.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((num3 != 0.0) && (num4 != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this._popupRoot.TransformToVisual(null);
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

                        this._popup.HorizontalOffset = 0.0;
                        this._popup.VerticalOffset = 0.0;
                        this._canvasPopupOutside.Width = actualWidth;
                        this._canvasPopupOutside.Height = actualHeight;
                        Matrix identity = Matrix.Identity;
                        identity.M11 = point5.X - point4.X;
                        identity.M12 = point5.Y - point4.Y;
                        identity.M21 = point6.X - point4.X;
                        identity.M22 = point6.Y - point4.Y;
                        identity.OffsetX -= point4.X;
                        identity.OffsetY -= point4.Y;
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = identity;
                        this._canvasPopupOutside.RenderTransform = transform2;
                        this._popupRoot.MinWidth = num8;
                        this._popupRoot.MaxWidth = actualWidth;
                        this._popupRoot.MinHeight = num7;
                        this._popupRoot.MaxHeight = actualHeight;
                        this._popupRoot.HorizontalAlignment = HorizontalAlignment.Left;
                        this._popupRoot.VerticalAlignment = VerticalAlignment.Top;



                        try
                        {
                            transform = this.TransformToVisual(null);
                        }
                        catch
                        {
                            //Don't need to do nothing.
                        }
                        point4 = transform.Transform(point);
                        if ((this._popupRoot.ActualWidth - base.ActualWidth) <= point4.X)
                        {
                            Canvas.SetLeft(this._popupRoot, -(this._popupRoot.ActualWidth - this.ActualWidth));
                        }
                        else
                        {
                            Canvas.SetLeft(this._popupRoot, 0);
                        }

                        if (this._popupRoot.ActualHeight <= (actualHeight - point4.Y - base.ActualHeight))
                        {
                            Canvas.SetTop(this._popupRoot, base.ActualHeight);
                        }
                        else
                        {
                            Canvas.SetTop(this._popupRoot, -this._popupRoot.ActualHeight);
                        }




                        bool isTop = false;
                        bool isLeft = false;
                        double popY = Canvas.GetTop(this._popupRoot);
                        double popX = Canvas.GetLeft(this._popupRoot);
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
                            _popContent.Margin = new Thickness(0, 1, 0, 0);
                            VisualStateManager.GoToState(this, "PopTopLeftState", false);
                        }
                        else if (!isTop && isLeft)
                        {
                            _popContent.Margin = new Thickness(0, -1, 0, 0);
                            VisualStateManager.GoToState(this, "PopBottomLeftState", false);
                        }
                        else if (!isTop && !isLeft)
                        {
                            _popContent.Margin = new Thickness(0, -1, 0, 0);
                            VisualStateManager.GoToState(this, "PopBottomRightState", false);
                        }
                        else if (isTop && !isLeft)
                        {
                            _popContent.Margin = new Thickness(0, 1, 0, 0);
                            VisualStateManager.GoToState(this, "PopTopRightState", false);
                        }

                    }
                }
            }
        }

    }

}
