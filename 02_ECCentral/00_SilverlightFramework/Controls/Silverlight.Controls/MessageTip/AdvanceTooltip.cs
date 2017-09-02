using System;
using System.Net;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

using System.Windows.Controls.Primitives;
using System.Threading;
using System.Diagnostics;
using Newegg.Oversea.Silverlight.Utilities;
using System.Windows.Threading;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class AdvanceTooltip : ContentControl
    {
        private Popup m_popup = null;
        private FrameworkElement m_targetElement = null;
        private bool m_isClose = true;
        private bool m_isOpen = true;

        private Border m_overlay = new Border() { Background = new SolidColorBrush(Colors.Transparent) };
        private Grid m_container = new Grid();

        public AdvanceTooltip()
        {
            m_popup = new Popup { Child = new ArrowTip() { Content = this.Content } };
            m_container.Children.Add(m_popup);
            m_overlay.MouseEnter += new MouseEventHandler(m_overlay_MouseEnter);
        }

        void m_overlay_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsManuallyClose)
            {
                IsOpen = false;
                m_isOpen = false;
            }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(TooltipOrientation), typeof(AdvanceTooltip), new PropertyMetadata(TooltipOrientation.LeftOrRight));
        public TooltipOrientation Orientation
        {
            get { return (TooltipOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        public static readonly DependencyProperty IsManuallyCloseProperty = DependencyProperty.Register("IsManuallyClose", typeof(bool), typeof(AdvanceTooltip), new PropertyMetadata(false));
        public bool IsManuallyClose
        {
            get { return (bool)GetValue(IsManuallyCloseProperty); }
            set { SetValue(IsManuallyCloseProperty, value); }
        }


        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(AdvanceTooltip), new PropertyMetadata(false, new PropertyChangedCallback(IsOpenProperty_Changed)));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        static void IsOpenProperty_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AdvanceTooltip tooltip = obj as AdvanceTooltip;
            if (tooltip != null && tooltip.m_popup != null)
            {
                if ((bool)args.NewValue)
                {
                    tooltip.OpenPopup();
                }
                else
                {
                    tooltip.ClosePopup();
                }
            }
        }
        

        /// <summary>
        /// 设置需要Tip的目标对象
        /// </summary>
        /// <param name="element"></param>
        public void SetTargetElement(FrameworkElement element)
        {
            m_targetElement = element;
            m_targetElement.MouseEnter += new MouseEventHandler(m_targetElement_MouseEnter);
            m_targetElement.MouseLeave += new MouseEventHandler(m_targetElement_MouseLeave);
            (m_popup.Child as ArrowTip).MouseEnter += new MouseEventHandler(m_targetElement_MouseEnter);
            (m_popup.Child as ArrowTip).Loaded += new RoutedEventHandler(AdvanceTooltip_Loaded);
        }

        void AdvanceTooltip_Loaded(object sender, RoutedEventArgs e)
        {
            if ((sender as ArrowTip).DataContext == null)
            {
                (sender as ArrowTip).DataContext = m_targetElement.DataContext;
            }
        }

        void m_targetElement_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsManuallyClose)
            {
                IsOpen = false;
                m_isOpen = false;
            }
        }

        void m_targetElement_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ArrowTip)
            {
                if (!m_container.Children.Contains(m_overlay))
                {
                    m_container.Children.Add(m_overlay);
                }
            }
            m_isClose = false;
            IsOpen = true;
            m_isOpen = true;
        }

        void ClosePopup()
        {
            m_isClose = true;
            new Thread(() =>
            {
                Thread.Sleep(50);
                if (m_isClose)
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        if (m_container.Children.Contains(m_overlay))
                        {
                            m_container.Children.Remove(m_overlay);
                        }
                        Panel container = (Application.Current.RootVisual as UserControl).Content as Panel;
                        container.Children.Remove(m_container);
                        m_popup.IsOpen = false;
                        IsOpen = false;
                    });
                }
            }).Start();
        }

        void OpenPopup()
        {
            new Thread(() =>
            {
                Thread.Sleep(50);
                if (m_isOpen)
                {
                   this.Dispatcher.BeginInvoke(() =>
                   {
                       
                       Panel container = (Application.Current.RootVisual as UserControl).Content as Panel;
                       if (!container.Children.Contains(m_container))
                       {
                           container.Children.Add(m_container);
                       }
                       m_popup.IsOpen = true;
                       ArrangePopup();
                   });
                }
            }).Start();
        }

        private void ArrangePopup()
        {
            if (((this.m_popup != null) && (this.m_targetElement != null)) && this.m_popup.IsOpen)
            {
                if ((m_popup.Child as ArrowTip).Content == null)
                {
                    object obj = this.Content;
                    this.Content = null;
                    (m_popup.Child as ArrowTip).Content = obj;
                }
                m_popup.UpdateLayout();
                ArrowTip tip = (m_popup.Child as ArrowTip);

                UserControl rootVisual = Application.Current.RootVisual as UserControl;
                double actualWidth = rootVisual.ActualWidth;
                double actualHeight = rootVisual.ActualHeight;
                double targetWidth = this.m_targetElement.ActualWidth;
                double targetHeight = this.m_targetElement.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((targetWidth != 0.0) && (targetHeight != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this.m_targetElement.TransformToVisual(rootVisual);
                    }
                    catch
                    {
                        //Don't need to do nothing.
                    }
                    if (transform != null)
                    {
                        Point point = new Point(0.0, 0.0);
                        Point position = transform.Transform(point);

                        if (Orientation == TooltipOrientation.LeftOrRight)
                        {
                            bool isLeft = false;

                            if (position.X + tip.ActualWidth + targetWidth > actualWidth)
                            {
                                isLeft = true;
                            }

                            //左
                            if (isLeft)
                            {
                                tip.Orientation = ArrowTipOrientation.Right;
                                m_popup.HorizontalOffset = position.X - tip.ActualWidth;
                                m_popup.VerticalOffset = position.Y - targetHeight;
                                tip.Offset = targetHeight;
                            }
                            else//右
                            {
                                tip.Orientation = ArrowTipOrientation.Left;
                                m_popup.HorizontalOffset = position.X + targetWidth;
                                m_popup.VerticalOffset = position.Y - targetHeight;
                                tip.Offset = targetHeight;
                            }

                            if (actualHeight <= position.Y + tip.ActualHeight)
                            {
                                m_popup.VerticalOffset = actualHeight - tip.ActualHeight;
                                tip.Offset = position.Y - m_popup.VerticalOffset;
                            }
                            else if (position.Y - tip.ActualHeight < 0)
                            {
                                m_popup.VerticalOffset = 0;
                                tip.Offset = position.Y;
                            }
                        }
                        else
                        {
                            bool isTop = false;

                            if (position.Y + tip.ActualHeight + targetHeight > actualHeight)
                            {
                                isTop = true;
                            }

                            //左
                            if (isTop)
                            {
                                tip.Orientation = ArrowTipOrientation.Bottom;
                                m_popup.VerticalOffset = position.Y - tip.ActualHeight;
                                m_popup.HorizontalOffset = position.X;
                                tip.Offset = targetWidth / 2;
                            }
                            else//右
                            {
                                tip.Orientation = ArrowTipOrientation.Top;
                                m_popup.VerticalOffset = position.Y + targetHeight;
                                m_popup.HorizontalOffset = position.X;
                                tip.Offset = targetWidth / 2;
                            }

                            if (actualWidth <= position.X + tip.ActualWidth)
                            {
                                m_popup.HorizontalOffset = actualWidth - tip.ActualWidth;
                                tip.Offset = position.X - m_popup.HorizontalOffset;
                            }
                            else if (position.X - tip.ActualWidth < 0)
                            {
                                m_popup.HorizontalOffset = 0;
                                tip.Offset = position.X;
                            }
                        }

                    }
                }
            }
        }
    }

    public enum TooltipOrientation
    {
        LeftOrRight,
        UpOrDown,
    }
}
