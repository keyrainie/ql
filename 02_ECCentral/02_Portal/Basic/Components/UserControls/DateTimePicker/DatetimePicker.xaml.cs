using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using ECCentral.Portal.Basic.Converters;

namespace ECCentral.Portal.Basic.Components.UserControls.DatetimePicker
{
    public partial class DatetimePicker : UserControl
    {
        private bool isOpen;
        private Canvas outsidePopup;
        private Canvas popupChildCanvas;
        private FrameworkElement popupChild;

        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }
            set
            {
                this.isOpen = value;

                this.popBody.IsOpen = value;
            }
        }

        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DatetimePicker), new PropertyMetadata((double)1.0 / (double)0.0, new PropertyChangedCallback(OnMaxDropDownHeightChanged)));
        public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(DatetimePicker), new PropertyMetadata(null, new PropertyChangedCallback(SelectedDateTimePropertyChangedCallback)));

        private static void OnMaxDropDownHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DatetimePicker)d).OnMaxDropDownHeightChanged((double)e.NewValue);
        }

        private static void SelectedDateTimePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DatetimePicker;
            if (e.NewValue != null)
            {
                DateTime value = (DateTime)e.NewValue;

                control.calDate.SelectedDate = value.Date;
                control.tpTime.Value = value;
                control.textBoxValue.Text = value.ToString(ResConverter.DateTime_LongFormat);
            }
            else
            {
                control.calDate.SelectedDate = DateTime.Now; ;
                control.tpTime.Value = DateTime.Now;
                control.textBoxValue.Text = string.Empty;
            }
        }

        private void OnMaxDropDownHeightChanged(double newValue)
        {
            this.ArrangePopup();
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

        public DateTime? SelectedDateTime
        {
            get
            {
                return (DateTime?)base.GetValue(SelectedDateTimeProperty);
            }
            set
            {
                base.SetValue(SelectedDateTimeProperty, value);
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            this.ArrangePopup();
            return size;
        }

        public DatetimePicker()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(DatetimePicker_Loaded);

            this.LostFocus += new RoutedEventHandler(DatetimePicker_LostFocus);
        }

        void OutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsOpen = false;
        }

        void PopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }

        void DatetimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(DatetimePicker_Loaded);

            if (SelectedDateTime.HasValue)
            {
                calDate.SelectedDate = SelectedDateTime.Value.Date;
                tpTime.Value = SelectedDateTime.Value;
            }
            else
            {
                calDate.SelectedDate = DateTime.Now;
                tpTime.Value = DateTime.Now;
            }

            var exp = this.GetBindingExpression(DatetimePicker.SelectedDateTimeProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                binding.Converter = DateTimeConverter;
                binding.ConverterParameter = "Long";
                binding.StringFormat = "yyyy-MM-dd HH:mm:ss"; 
                textBoxValue.SetBinding(TextBox.TextProperty, binding);
            }

            this.popupChild = this.popBody.Child as FrameworkElement;
            this.outsidePopup = new Canvas();

            this.outsidePopup.Background = new SolidColorBrush(Colors.Transparent);
            this.outsidePopup.MouseLeftButtonDown += new MouseButtonEventHandler(OutsidePopup_MouseLeftButtonDown);

            if (this.popupChild != null)
            {
                this.popupChild.SizeChanged += new SizeChangedEventHandler(this.PopupChild_SizeChanged);
                this.popupChildCanvas = new Canvas();
            }
            if ((this.popupChildCanvas != null) && (this.outsidePopup != null))
            {
                this.popBody.Child = this.popupChildCanvas;
                this.popupChildCanvas.Children.Add(this.outsidePopup);
                this.popupChildCanvas.Children.Add(this.popupChild);
            }
            this.IsOpen = false;
        }

        void DatetimePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        private void imgCancel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsOpen = false;
        }

        private void imgOK_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (calDate.SelectedDate.HasValue && tpTime.Value.HasValue)
            {
                string strDate = calDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                string strTime = tpTime.Value.Value.ToString("HH:mm:ss");
                SelectedDateTime = DateTime.Parse(strDate + " " + strTime);
                this.IsOpen = !this.IsOpen;
            }
        }

        private void textBoxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            string str = textBoxValue.Text;
            if (string.IsNullOrEmpty(str.Trim()))
            {
                SelectedDateTime = null;
            }
            else
            {
                DateTime dt = DateTime.Now;
                if (DateTime.TryParse(str, out dt))
                {
                    SelectedDateTime = dt;
                    calDate.SelectedDate = dt.Date;
                    tpTime.Value = dt;
                }
                else
                {
                    if (SelectedDateTime.HasValue)
                    {
                        textBoxValue.Text = SelectedDateTime.Value.ToString(ResConverter.DateTime_LongFormat);
                    }
                    else
                    {
                        textBoxValue.Text = "";
                    }
                }
            }
        }

        private void imgPopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsOpen = !this.IsOpen;
        }

        private void ArrangePopup()
        {
            if (((this.popBody != null) && (this.popupChild != null)) && (this.IsOpen))
            {
                var content = Application.Current.Host.Content;
                double actualWidth = content.ActualWidth;
                double actualHeight = content.ActualHeight;
                double num3 = this.popupChild.ActualWidth;
                double num4 = this.popupChild.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((num3 != 0.0) && (num4 != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this.gridHeader.TransformToVisual(null);
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
                        this.popBody.HorizontalOffset = 0;
                        this.popBody.VerticalOffset = 0;
                        this.outsidePopup.Width = actualWidth;
                        this.outsidePopup.Height = actualHeight;
                        Matrix identity = Matrix.Identity;
                        identity.M11 = point5.X - point4.X;
                        identity.M12 = point5.Y - point4.Y;
                        identity.M21 = point6.X - point4.X;
                        identity.M22 = point6.Y - point4.Y;
                        identity.OffsetX -= point4.X;
                        identity.OffsetY -= point4.Y;
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = identity;
                        this.outsidePopup.RenderTransform = transform2;
                        this.popupChild.MinWidth = num8;
                        this.popupChild.MaxWidth = actualWidth;
                        this.popupChild.MinHeight = num7;
                        this.popupChild.MaxHeight = Math.Max(0.0, maxDropDownHeight);
                        this.popupChild.HorizontalAlignment = HorizontalAlignment.Left;
                        this.popupChild.VerticalAlignment = VerticalAlignment.Top;
                        var top = num11 - y;
                        Canvas.SetLeft(this.popupChild, num10 - x);
                        if (top > 0)
                            Canvas.SetTop(this.popupChild, top - 3);
                        else
                            Canvas.SetTop(this.popupChild, top + 2);
                    }
                }
            }
        }
    }
}
