using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class FunctionPanel : ContentControl
    {
        private static readonly DependencyProperty FunctionContentProperty = DependencyProperty.Register("FunctionContent", typeof(object), typeof(FunctionPanel), new PropertyMetadata(null));
        private static readonly DependencyProperty AnchorsProperty = DependencyProperty.Register("Anchors", typeof(ObservableCollection<AnchorItem>), typeof(FunctionPanel), null);
        private static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(FunctionPanel), new PropertyMetadata(true));
        private static readonly DependencyProperty ContentVerticalScrollBarVisibilityProperty = DependencyProperty.Register("ContentVerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(FunctionPanel), new PropertyMetadata(ScrollBarVisibility.Auto));
        private static readonly DependencyProperty ContentHorizontalScrollBarVisibilityProperty = DependencyProperty.Register("ContentHorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(FunctionPanel), new PropertyMetadata(ScrollBarVisibility.Auto));
        private static readonly DependencyProperty TraceFunctionEventProperty = DependencyProperty.Register("TraceFunctionEvent", typeof(bool), typeof(FunctionPanel), new PropertyMetadata(true));

        public object FunctionContent
        {
            get { return GetValue(FunctionContentProperty); }
            set { SetValue(FunctionContentProperty, value); }
        }

        public ObservableCollection<AnchorItem> Anchors
        {
            get { return (ObservableCollection<AnchorItem>)GetValue(AnchorsProperty); }
            set { SetValue(AnchorsProperty, value); }
        }

        public bool TraceFunctionEvent
        {
            get { return (bool)base.GetValue(TraceFunctionEventProperty); }
            set { base.SetValue(TraceFunctionEventProperty, value); }
        }

        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        public ScrollBarVisibility ContentVerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(ContentVerticalScrollBarVisibilityProperty); }
            set { SetValue(ContentVerticalScrollBarVisibilityProperty, value); }
        }

        public ScrollBarVisibility ContentHorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(ContentHorizontalScrollBarVisibilityProperty); }
            set { SetValue(ContentHorizontalScrollBarVisibilityProperty, value); }
        }

        private Button m_buttonGoTop;
        private Grid m_pinnedPlaceHolder;
        private Grid m_unpinnedPlaceHolder;
        private Grid m_anchorPlaceHolder;
        private Grid m_functionPanel;
        private MiniButton m_buttonPin;
        private ItemsControl m_anchorList;
        private ContentControl m_functionContent;
        private ScrollViewer m_scrollViewerContent;

        private double m_panelHeight;
        private ScrollBar m_verticalScrollBar;
        private List<ButtonBase> m_buttons;  //用于保存在FunctionContent中的所有按钮。


        public FunctionPanel()
        {
            this.DefaultStyleKey = typeof(FunctionPanel);

            this.Anchors = new ObservableCollection<AnchorItem>();
            this.m_buttons = new List<ButtonBase>();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            this.m_scrollViewerContent = GetTemplateChild("ScrollViewerContent") as ScrollViewer;
            this.m_pinnedPlaceHolder = GetTemplateChild("PinnedPlaceHolder") as Grid;
            this.m_unpinnedPlaceHolder = GetTemplateChild("UnpinnedPlaceHolder") as Grid;
            this.m_functionPanel = GetTemplateChild("FuncPanel") as Grid;
            this.m_anchorPlaceHolder = GetTemplateChild("AnchorPlaceHolder") as Grid;

            if (this.FunctionContent == null)
            {
                this.m_functionPanel.Visibility = System.Windows.Visibility.Collapsed;
                IsPinned = false;
            }

            if (this.m_buttonGoTop != null)
            {
                this.m_buttonGoTop.Click -= new RoutedEventHandler(m_buttonGoTop_Click);
            }
            this.m_buttonGoTop = GetTemplateChild("ButtonGoTop") as Button;

            if (this.m_buttonGoTop != null)
            {
                this.m_buttonGoTop.Click += new RoutedEventHandler(m_buttonGoTop_Click);
            }

            if (this.Anchors != null && this.Anchors.Count > 0)
            {
                if (this.m_anchorList != null)
                {
                    this.m_anchorList.MouseLeftButtonUp -= new MouseButtonEventHandler(m_anchorList_MouseLeftButtonUp);
                }
                this.m_anchorList = GetTemplateChild("AnchorList") as ItemsControl;
                this.m_anchorList.MouseLeftButtonUp += new MouseButtonEventHandler(m_anchorList_MouseLeftButtonUp);
                this.m_anchorList.ItemsSource = this.Anchors;
            }
            else
            {
                if (this.m_anchorPlaceHolder != null)
                {
                    this.m_anchorPlaceHolder.Visibility = Visibility.Collapsed;
                }
            }

            if (this.m_buttonPin != null)
            {
                this.m_buttonPin.Click -= new RoutedEventHandler(m_buttonPin_Click);
            }
            this.m_buttonPin = GetTemplateChild("ButtonPin") as MiniButton;
            if (this.m_buttonPin != null)
            {
                this.m_buttonPin.IsPinned = this.IsPinned;
                this.m_buttonPin.Click += new RoutedEventHandler(m_buttonPin_Click);
            }

            if (this.m_pinnedPlaceHolder != null && this.m_unpinnedPlaceHolder != null)
            {
                this.OnIsPinnedChanged();
            }
            if (this.m_scrollViewerContent != null)
            {
                this.m_scrollViewerContent.LayoutUpdated += new EventHandler(m_scrollViewerContent_LayoutUpdated);
            }

            if (this.TraceFunctionEvent && this.FunctionContent != null)
            {
                RegisterTraceEvent((DependencyObject)this.FunctionContent);
            }

        }

        private void RegisterTraceEvent(DependencyObject parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ButtonBase)
                {
                    ((ButtonBase)child).Click -= new RoutedEventHandler(button_Click);
                    ((ButtonBase)child).Click += new RoutedEventHandler(button_Click);
                }
                else
                {
                    this.RegisterTraceEvent(child);
                }
            }
        }

        private string GetLabel(ButtonBase btn)
        {
            var value = btn.Content as string;

            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(btn.Name))
                {
                    return btn.Name;
                }

                throw new Exception("Button's Content or Name should be specified!");
            }
            else
            {
                return value;
            }
        }

        #region Events

        void button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ButtonBase;

            var action = "Click";
            var label = "Button:" + this.GetLabel(btn);

            if (CPApplication.Current.CurrentPage != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.EventTracker.TraceEvent(action, label);
            }
        }

        void m_scrollViewerContent_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.IsPinned)
            {
                this.ArrangeAnchors();
            }
            else
            {
                this.m_anchorPlaceHolder.Margin = new Thickness(0, 0, 20, this.m_functionPanel.ActualHeight - 4);
            }
        }

        void m_anchorList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = e.OriginalSource as Border;
            if (border != null)
            {
                var anchor = border.DataContext as AnchorItem;

                if (anchor != null)
                {
                    if (anchor.Element != null)
                    {
                        ScrollToElement(anchor.Element);
                    }
                    else
                    {
                        anchor.Element = GetTemplateChildByName(m_scrollViewerContent, anchor.ElementName);
                        ScrollToElement(anchor.Element);
                    }
                }
            }
        }

        void m_buttonPin_Click(object sender, RoutedEventArgs e)
        {
            this.IsPinned = !this.IsPinned;

            OnIsPinnedChanged();
        }

        void m_buttonGoTop_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_scrollViewerContent != null)
            {
                if (this.m_scrollViewerContent.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    this.m_scrollViewerContent.ScrollToHorizontalOffset(0);
                }
                if (this.m_scrollViewerContent.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    this.m_scrollViewerContent.ScrollToVerticalOffset(0);
                }
            }
        }

        #endregion


        #region Public Methods

        public void ScrollTo(string elementName)
        {
            var element = this.GetTemplateChildByName(this.m_scrollViewerContent, elementName);

            this.ScrollToElement(element);
        }

        public void ScrollTo(FrameworkElement element)
        {
            this.ScrollToElement(element);
        }

        #endregion

        #region Private Methods

        void OnIsPinnedChanged()
        {
            if (this.IsPinned)
            {
                if (this.m_unpinnedPlaceHolder.Children.Contains(this.m_functionPanel))
                {
                    this.m_unpinnedPlaceHolder.Children.Remove(this.m_functionPanel);
                }
                if (!this.m_pinnedPlaceHolder.Children.Contains(this.m_functionPanel))
                {
                    this.m_pinnedPlaceHolder.Children.Add(this.m_functionPanel);
                }
            }
            else
            {
                if (this.m_pinnedPlaceHolder.Children.Contains(this.m_functionPanel))
                {
                    this.m_pinnedPlaceHolder.Children.Remove(this.m_functionPanel);
                }
                if (!this.m_unpinnedPlaceHolder.Children.Contains(this.m_functionPanel))
                {
                    this.m_unpinnedPlaceHolder.Children.Add(this.m_functionPanel);
                }
            }
        }

        void ScrollToElement(FrameworkElement element)
        {
            if (m_scrollViewerContent != null && element != null)
            {
                Point point = element.TransformToVisual(m_scrollViewerContent).Transform(new Point());
                if (m_scrollViewerContent.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    m_scrollViewerContent.ScrollToHorizontalOffset(point.X + m_scrollViewerContent.HorizontalOffset - 10);
                }

                if (m_scrollViewerContent.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    m_scrollViewerContent.ScrollToVerticalOffset(point.Y + m_scrollViewerContent.VerticalOffset - 10);
                }
            }
        }

        FrameworkElement GetTemplateChildByName(DependencyObject parent, string name)
        {
            int childnum = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childnum; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement && ((FrameworkElement)child).Name == name)
                {
                    return child as FrameworkElement;
                }
                else
                {
                    var s = GetTemplateChildByName(child, name);
                    if (s != null)
                        return s;
                }
            }
            return null;
        }

        void ArrangeAnchors()
        {
            double interval = this.m_scrollViewerContent.ScrollableHeight - this.m_scrollViewerContent.VerticalOffset;
            double scrollBarHeight = 0d;
            var visibility = this.m_scrollViewerContent.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible;
            if (visibility)
            {
                scrollBarHeight = 18d;
            }
            if (interval <= this.m_functionPanel.ActualHeight)
            {
                var bottom = (this.m_functionPanel.ActualHeight - interval + scrollBarHeight) - 4;
                this.m_anchorPlaceHolder.Margin = new Thickness(0, 0, 38, bottom);
            }
            else
            {
                this.m_anchorPlaceHolder.Margin = new Thickness(0, 0, 38, scrollBarHeight - 2);
            }
        }

        #endregion
    }

    public class AnchorItem : DependencyObject
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(AnchorItem), new PropertyMetadata(null));

        public string Title
        {
            get
            {
                return GetValue(TitleProperty) as string;
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public string ElementName { get; set; }

        public FrameworkElement Element { get; internal set; }
    }
}
