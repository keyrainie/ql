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

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class TabList : ContentControl
    {
        #region Data

        private FrameworkElement _elementRoot;
        private bool _isOpen;
        private FrameworkElement _elementContentPresenterBorder;
        private Canvas _elementOutsidePopup;
        private Canvas _elementPopupChildCanvas;
        private FrameworkElement _elementPopupChild;
        private Grid _expanderGrid;
        private Popup _elementPopup;       

        #endregion

        #region Constants
        
        private const string RootElementName = "RootElement";
        private const string ExpanderButtonName = "ExpanderButton";
        private const string ElementPopupName = "ElementPopup";
        private const string ContentPresenterBorder = "ContentPresenterBorder";               
       
        public static readonly DependencyProperty MaxDropDownHeightProperty;

        #endregion

        public bool IsOpen
        {
            get
            {
                return this._isOpen;
            }
            set
            {
                this._isOpen = value;
                if (this._elementPopup != null)
                {
                    this._elementPopup.IsOpen = value;
                }
            }
        }

        public double MaxDropDownHeight
        {
            get
            {
                return (double)base.GetValue(MaxDropDownHeightProperty);
            }
            set
            {
                base.SetValue(MaxDropDownHeightProperty, value);
            }
        }       

        static TabList()
        {
            MaxDropDownHeightProperty = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(TabList), new PropertyMetadata((double)1.0 / (double)0.0, new PropertyChangedCallback(TabList.OnMaxDropDownHeightChanged)));
        }

        public TabList()
        {
            DefaultStyleKey = typeof(TabList);
        }       

        #region Protected Methods

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            this.ArrangePopup();
            return size;
        }

        public override void OnApplyTemplate()
        {
            this._elementRoot = GetTemplateChild(RootElementName) as FrameworkElement;
            this._expanderGrid = GetTemplateChild(ExpanderButtonName) as Grid;
            this._elementPopup = GetTemplateChild(ElementPopupName) as Popup;
            this._elementContentPresenterBorder = GetTemplateChild(ContentPresenterBorder) as FrameworkElement;
                       
            if (this._elementPopup != null)
            {
                this._elementPopupChild = this._elementPopup.Child as FrameworkElement;
                this._elementOutsidePopup = new Canvas();
            }
            else
            {
                this._elementPopupChild = null;
                this._elementOutsidePopup = null;
            }
            if (this._elementOutsidePopup != null)
            {
                this._elementOutsidePopup.Background = new SolidColorBrush(Colors.Transparent);
                this._elementOutsidePopup.MouseLeftButtonDown += new MouseButtonEventHandler(ElementOutsidePopup_MouseLeftButtonDown);
            }
            if (this._elementPopupChild != null)
            {
                this._elementPopupChild.SizeChanged += new SizeChangedEventHandler(this.ElementPopupChild_SizeChanged);
                this._elementPopupChildCanvas = new Canvas();
            }
            else
            {
                this._elementPopupChildCanvas = null;
            }
            if ((this._elementPopupChildCanvas != null) && (this._elementOutsidePopup != null))
            {
                this._elementPopup.Child = this._elementPopupChildCanvas;
                this._elementPopupChildCanvas.Children.Add(this._elementOutsidePopup);
                this._elementPopupChildCanvas.Children.Add(this._elementPopupChild);
            }

            this._expanderGrid.MouseLeftButtonDown += new MouseButtonEventHandler(_expanderGrid_MouseLeftButtonDown);
            base.SizeChanged += new SizeChangedEventHandler(this.ElementPopupChild_SizeChanged);
            this.IsOpen = false;

            base.OnApplyTemplate();                    
        }

        #endregion

        #region Methods
       
        void _applyColumnConfig_ApplyCompleted(object sender, EventArgs e)
        {
            this.IsOpen = false;
        }

        void _switchGridConfig_SwitchConfigCompleted(object sender, EventArgs e)
        {
            this.IsOpen = false;
        }

        void _expanderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled)
            {
                e.Handled = true;
                base.Focus();
                this.IsOpen = !this.IsOpen;
                if (base.Content is Control)
                {
                    ((Control)base.Content).Focus();
                }
            }
        }

        void ElementPopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }

        void ArrangePopup()
        {
            if (((this._elementPopup != null) && (this._elementPopupChild != null)) && (((this._elementContentPresenterBorder != null) && (this._elementOutsidePopup != null)) && this.IsOpen))
            {
                var content = Application.Current.Host.Content;
                double actualWidth = content.ActualWidth;
                double actualHeight = content.ActualHeight;
                double num3 = this._elementPopupChild.ActualWidth;
                double num4 = this._elementPopupChild.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((num3 != 0.0) && (num4 != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this._elementContentPresenterBorder.TransformToVisual(null);
                    }
                    catch
                    {
                        this.IsOpen = false;
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
                        //加上这句，防止X大于屏幕的宽度，这样算出来的位置不准确
                        x = Math.Min(x, actualWidth);
                        double num7 = base.ActualHeight;
                        double num8 = base.ActualWidth;
                        double maxDropDownHeight = this.MaxDropDownHeight;
                        if (double.IsInfinity(maxDropDownHeight) || double.IsNaN(maxDropDownHeight))
                        {
                            maxDropDownHeight = ((actualHeight - num7) * 3.0) / 5.0;
                        }
                        num3 = Math.Min(num3, actualWidth);
                        num4 = Math.Min(num4, maxDropDownHeight);
                        num3 = Math.Max(num8, num3);
                        double num10 = x;
                        if (actualWidth < (num10 + num3))
                        {
                            num10 = actualWidth - num3;
                            num10 = Math.Max(0.0, num10);
                        }
                        bool flag = true;
                        double num11 = y + num7;
                        if (actualHeight < (num11 + num4))
                        {
                            flag = false;
                            num11 = y - num4;
                            if (num11 < 0.0)
                            {
                                if (y < ((actualHeight - num7) / 2.0))
                                {
                                    flag = true;
                                    num11 = y + num7;
                                }
                                else
                                {
                                    flag = false;
                                    num11 = y - num4;
                                }
                            }
                        }
                        if (flag)
                        {
                            maxDropDownHeight = Math.Min(actualHeight - num11, maxDropDownHeight);
                        }
                        else
                        {
                            maxDropDownHeight = Math.Min(y, maxDropDownHeight);
                        }
                        this._elementPopup.HorizontalOffset = 0;
                        this._elementPopup.VerticalOffset = 0;
                        this._elementOutsidePopup.Width = actualWidth;
                        this._elementOutsidePopup.Height = actualHeight;
                        Matrix identity = Matrix.Identity;
                        identity.M11 = point5.X - point4.X;
                        identity.M12 = point5.Y - point4.Y;
                        identity.M21 = point6.X - point4.X;
                        identity.M22 = point6.Y - point4.Y;
                        identity.OffsetX -= point4.X;
                        identity.OffsetY -= point4.Y;
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = identity;
                        this._elementOutsidePopup.RenderTransform = transform2;
                        this._elementPopupChild.MinWidth = num8;
                        this._elementPopupChild.MaxWidth = actualWidth;
                        this._elementPopupChild.MinHeight = num7;
                        this._elementPopupChild.MaxHeight = Math.Max(0.0, maxDropDownHeight);
                        this._elementPopupChild.HorizontalAlignment = HorizontalAlignment.Left;
                        this._elementPopupChild.VerticalAlignment = VerticalAlignment.Top;
                        Canvas.SetLeft(this._elementPopupChild, num10 - x);
                        //暂时把Left设置为-185，不然Popup的位置计算不准确，还没有找到原因
                        //Canvas.SetLeft(this._elementPopupChild, -185);
                        Canvas.SetTop(this._elementPopupChild, num11 - y);
                    }
                }
            }
        }

        void ElementOutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsOpen = false;
        }

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabList)d).OnMaxDropDownHeightChanged((double)e.NewValue);
        }

        private void OnMaxDropDownHeightChanged(double newValue)
        {
            this.ArrangePopup();
        }

        #endregion
    }
}
