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
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Threading;

using Newegg.Oversea.Silverlight.Controls.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    //public delegate void ResultHandler(object sender, ResultEventArgs e);

    [TemplatePart(Name = "RootElement", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "ContentElement", Type = typeof(ContentControl))]
    [TemplatePart(Name = "CloseElement", Type = typeof(TextBlock))]
    [TemplatePart(Name = "TitleElement", Type = typeof(TextBlock))]
    [ContentProperty("Content")]
    public class PopupBox : ContentControl, IAlert, IConfirm, IDialog
    {
        #region private element

        private Grid m_rootElement;
        private Grid m_areaElement;
        private Grid m_titlePanel;
        private ContentControl m_contentElement;
        private Button m_closeIcon;
        private Button m_confirmElement;
        private Button m_cancelElement;
        private TextBlock m_titleElement;
        private IPageBrowser m_browser;
        private TranslateTransform m_translate;
        private TransformGroup m_tf;
        private Panel m_container;
        private PopupBox m_popupBox;
        private ButtonType m_buttonType;

        private Border m_borderContent;

        private bool m_attachedRootVisualListener = false;

        #endregion

        #region event

        public event EventHandler Closed;
        public event EventHandler<ClosingEventArgs> Closing;

        public event EventHandler Showed;

        protected void OnShowed(EventArgs e)
        {
            EventHandler handler = Showed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event ResultHandler ResultHandler;

        private void OnResult(ResultEventArgs e)
        {
            ResultHandler handler = ResultHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region DependencyProperty

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PopupBox), null);

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public IPageBrowser Browser
        {
            get { return m_browser; }
            set { m_browser = value; }
        }

        #endregion

        #region properties

        public Point LastDragPosition { get; set; }
        public bool IsDragging { get; set; }
        public bool IsModal { get; set; }
        public PopType PopType { get; set; }
        public MessageType? MessageType { get; set; }
        public Image ImageIcon { get; set; }
        public ResultEventArgs ResultArgs { get; set; }
        public Size RootElementSize { get; set; }
        public bool IsOnClose { get; set; }
        public LayoutMask LayoutMask { get; set; }

        internal static Stack<PopupBox> PopupBoxes = new Stack<PopupBox>();

        protected Panel Container
        {
            get
            {
                return m_container;
            }
        }

        private PopupBox Current
        {
            get
            {
                return m_popupBox;
            }
        }
        #endregion

        public PopupBox()
        {
            this.ResultArgs = new ResultEventArgs();
        }

        public PopupBox(Panel layoutRoot)
            : this()
        {
            this.DefaultStyleKey = typeof(PopupBox);
            m_container = layoutRoot;
            LayoutMask = new LayoutMask(m_container);

            if (this.m_container is Grid)
            {
                Grid container = this.m_container as Grid;

                if (container.RowDefinitions.Count > 1)
                {
                    Grid.SetRowSpan(this.LayoutMask.MaskPanel, container.RowDefinitions.Count);
                }

                if (container.ColumnDefinitions.Count > 1)
                {
                    Grid.SetColumnSpan(this.LayoutMask.MaskPanel, container.ColumnDefinitions.Count);
                }
            }

            m_translate = new TranslateTransform();
            m_tf = new TransformGroup();
            m_tf.Children.Add(m_translate);
            this.RenderTransform = m_tf;
            this.KeyDown += new KeyEventHandler(PopupBox_KeyDown);
        }

        void PopupBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                m_closeElement_Click(null, null);
            }
            else if (e.Key == Key.Enter)
            {
                if (PopType == Containers.PopType.Confirm)
                {
                    m_confirmElement_Click(null, null);
                }
                else if (PopType != Containers.PopType.Dialog)
                {
                    m_closeElement_Click(null, null);
                }
            }
        }

        public PopupBox(Panel layoutRoot, IPageBrowser browser)
            : this(layoutRoot)
        {
            m_browser = browser;
        }

        public PopupBox(Panel layoutRoot, PopupBox parent)
            : this(layoutRoot)
        {
            m_popupBox = parent;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.m_rootElement = base.GetTemplateChild("RootElement") as Grid;

            this.m_areaElement = base.GetTemplateChild("AreaElement") as Grid;
            this.m_contentElement = base.GetTemplateChild("ContentElement") as ContentControl;
            this.m_closeIcon = base.GetTemplateChild("CloseElement") as Button;

            if (this.m_closeIcon != null)
            {
                this.m_closeIcon.Cursor = Cursors.Arrow;
            }

            this.m_confirmElement = base.GetTemplateChild("ConfirmElement") as Button;
            this.m_cancelElement = base.GetTemplateChild("CancelElement") as Button;

            if (PopType == Containers.PopType.Alert && m_cancelElement != null)
            {
                m_cancelElement.Focus();
            }
            else if (PopType == Containers.PopType.Confirm && m_confirmElement != null)
            {
                m_confirmElement.Focus();
            }

            this.m_titleElement = base.GetTemplateChild("TitleElement") as TextBlock;
            this.m_titlePanel = base.GetTemplateChild("TitlePanel") as Grid;

            this.m_borderContent = base.GetTemplateChild("BorderContent") as Border;
            if (this.PopType == Containers.PopType.Dialog)
            {
                var bytes = BitConverter.GetBytes(0xFFF1F1F1);
                this.m_borderContent.Background = new SolidColorBrush(Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]));
            }


            this.m_closeIcon.Click += new RoutedEventHandler(m_closeIcon_Click);
            this.m_closeIcon.MouseEnter += new MouseEventHandler(m_closeElement_MouseEnter);
            this.m_closeIcon.MouseLeave += new MouseEventHandler(m_closeElement_MouseLeave);

            this.m_confirmElement.MouseEnter += new MouseEventHandler(m_confirmElement_MouseEnter);
            this.m_confirmElement.MouseLeave += new MouseEventHandler(m_confirmElement_MouseLeave);

            this.m_cancelElement.MouseEnter += new MouseEventHandler(m_cancelElement_MouseEnter);
            this.m_cancelElement.MouseLeave += new MouseEventHandler(m_cancelElement_MouseLeave);

            this.m_titlePanel.MouseLeftButtonDown += new MouseButtonEventHandler(m_titlePanel_MouseLeftButtonDown);
            this.m_titlePanel.MouseMove += new MouseEventHandler(m_titlePanel_MouseMove);
            this.m_titlePanel.MouseLeftButtonUp += new MouseButtonEventHandler(m_titlePanel_MouseLeftButtonUp);
            this.m_titlePanel.MouseEnter += new MouseEventHandler(m_titlePanel_MouseEnter);
            this.m_contentElement.MouseLeftButtonDown += new MouseButtonEventHandler(m_contentElement_MouseLeftButtonDown);


            if (!double.IsNaN(RootElementSize.Width) && !double.IsInfinity(RootElementSize.Height) && PopType == PopType.Dialog)
            {
                this.m_rootElement.Width = RootElementSize.Width;
            }

            if (!double.IsNaN(RootElementSize.Height) && !double.IsInfinity(RootElementSize.Height) && PopType == PopType.Dialog)
            {
                this.m_rootElement.Height = RootElementSize.Height;
            }

            this.SetControlValues();
        }

        void m_closeIcon_Click(object sender, RoutedEventArgs e)
        {
            this.ResultArgs = new ResultEventArgs() { DialogResult = DialogResultType.Cancel };
            this.Close();
        }

        #region impl event

        void m_contentElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        void m_cancelElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.m_cancelElement.Cursor = Cursors.Arrow;
        }

        void m_cancelElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.m_cancelElement.Cursor = Cursors.Hand;
        }

        void m_confirmElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.m_confirmElement.Cursor = Cursors.Arrow;
        }

        void m_confirmElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.m_confirmElement.Cursor = Cursors.Hand;
        }

        void m_closeElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsOnClose = false;
        }

        void m_closeElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.IsOnClose = true;
        }

        private void m_confirmElement_Click(object sender, RoutedEventArgs e)
        {
            this.ResultArgs = new ResultEventArgs() { DialogResult = DialogResultType.OK };
            this.Close();
        }

        private void m_closeElement_Click(object sender, RoutedEventArgs e)
        {
            this.ResultArgs = new ResultEventArgs() { DialogResult = DialogResultType.Cancel };
            this.Close();
        }

        private void m_titlePanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsDragging && !this.IsOnClose)
            {
                this.IsDragging = false;
                this.ReleaseMouseCapture();
            }
        }

        private void m_titlePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsOnClose)
            {
                this.Focus();
                this.IsDragging = true;
                this.LastDragPosition = e.GetPosition(this.LayoutMask.MaskPanel);
                this.m_titlePanel.CaptureMouse();
            }
        }

        private void m_titlePanel_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void m_titlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsDragging && !this.IsOnClose)
            {
                Point currentPosition = e.GetPosition(this.LayoutMask.MaskPanel);
                double xDelta = currentPosition.X - this.LastDragPosition.X;
                double yDelta = currentPosition.Y - this.LastDragPosition.Y;

                m_translate.X += xDelta;
                m_translate.Y += yDelta;

                this.LastDragPosition = currentPosition;
            }
        }

        private void m_titlePanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsDragging)
            {
                this.IsDragging = false;
                this.m_titlePanel.ReleaseMouseCapture();

                Point point = this.m_titlePanel.TransformToVisual(this.m_container).Transform(new Point(0, 0));

                if (point.X <= -this.m_titlePanel.ActualWidth
                    || point.X >= this.m_container.ActualWidth
                    || point.Y <= -this.m_titlePanel.ActualHeight
                    || point.Y >= this.m_container.ActualHeight
                    )
                {
                    m_translate.X = 0;
                    m_translate.Y = 0;
                }
            }
        }

        #endregion

        private void SetControlValues()
        {
            if (this.m_contentElement != null && this.Content != null)
            {
                if (this.Content as Panel != null)
                {
                    this.m_contentElement.Tag = (this.Content as Panel).Tag;
                }
            }

            if (this.m_titleElement != null)
            {
                this.m_titleElement.Text = this.Title;
            }


            switch (PopType)
            {
                case PopType.Alert:

                    if (this.m_confirmElement.Visibility == Visibility.Visible)
                    {
                        this.m_confirmElement.Visibility = Visibility.Collapsed;
                        //this.m_rootElement.RowDefinitions[2].Height = new GridLength(35);
                    }

                    this.m_cancelElement.Content = MessageResource.PopupBox_Button_Confirm;
                    this.m_cancelElement.SetValue(Grid.ColumnSpanProperty, 3);
                    this.m_cancelElement.SetValue(Grid.ColumnProperty, 0);
                    this.m_cancelElement.HorizontalAlignment = HorizontalAlignment.Center;
                    DisplayIcon(MessageType.Value);
                    break;
                case PopType.Dialog:
                    if (this.m_areaElement.Visibility == Visibility.Visible)
                    {
                        this.m_areaElement.Visibility = Visibility.Collapsed;
                        //this.m_rootElement.RowDefinitions[2].Height = new GridLength(5);
                    }
                    break;
                case PopType.Confirm:

                    if (this.m_confirmElement.Visibility == Visibility.Collapsed)
                    {
                        this.m_confirmElement.Visibility = Visibility.Visible;
                        //this.m_rootElement.RowDefinitions[2].Height = new GridLength(35);
                    }

                    this.m_confirmElement.Content = MessageResource.PopupBox_Button_Confirm;
                    this.m_cancelElement.Content = MessageResource.PopupBox_Button_Cancel;
                    DisplayIcon(MessageType.Value);
                    break;
            }

            if (this.m_cancelElement != null)
            {
                if (m_buttonType == ButtonType.YesNo)
                {
                    this.m_cancelElement.Content = MessageResource.PopupBox_Button_No;
                }

                this.m_cancelElement.Click += new RoutedEventHandler(m_closeElement_Click);
            }

            if (this.m_confirmElement != null)
            {
                if (m_buttonType == ButtonType.YesNo)
                {
                    this.m_confirmElement.Content = MessageResource.PopupBox_Button_Yes;
                }

                this.m_confirmElement.Click += new RoutedEventHandler(m_confirmElement_Click);
            }
        }

        public void Show(bool isModal)
        {
            IsModal = isModal;
            if (LayoutMask != null)
            {
                LayoutMask.AddBox(this);
                if (this.Current != null)
                {
                   Current.Tag = LayoutMask.MaskPanel;
                }
                this.Focus();
            }
            Canvas.SetZIndex(this, LayoutMask.MaxZIndex + 3000);
            OnShowed(EventArgs.Empty);
            DoShow();
        }

        void DoShow()
        {
            //注册Mainpage的GotFocus事件，来解决在PopupBox弹出的时候，还可以操作它下面的内容。
            if (Application.Current.RootVisual != null
                && Application.Current.RootVisual is UserControl
                && (Application.Current.RootVisual as UserControl).Content is Panel
                && (Application.Current.RootVisual as UserControl).Content == this.m_container)
            {
                UIElementCollection collection = ((Application.Current.RootVisual as UserControl).Content as Panel).Children;
                foreach (var item in collection)
                {
                    if (item.GetType().Name == "MainPage")
                    {
                        var control = item as UserControl;
                        if (control != null && !m_attachedRootVisualListener)
                        {
                            control.GotFocus += new RoutedEventHandler(control_GotFocus);
                            m_attachedRootVisualListener = true;
                        }
                        break;
                    }
                }
            }
        }

        void control_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        public void Close()
        {
            this.Close(false);
        }

        public void Close(bool isForce)
        {
            if (this.LayoutMask != null)
            {
                if (this.Current != null)
                {
                    Current.Tag = null;
                }

                if (Closing != null && !isForce)
                {
                    var args = new ClosingEventArgs { Cancel = false };
                    Closing(this, args);

                    if (!args.Cancel)
                    {
                        DoClose();
                    }
                }
                else
                {
                    DoClose();
                }

                if (this.ResultArgs != null)
                {
                    OnResult(this.ResultArgs);
                }
            }
        }

        void DoClose()
        {

            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
            LayoutMask.RemoveBox(this);
            if (this.Content is Border)
            {
                (this.Content as Border).Child = null;
            }
            //注销Mainpage的GotFocus事件
            if (Application.Current.RootVisual != null 
                && Application.Current.RootVisual is UserControl 
                && (Application.Current.RootVisual as UserControl).Content is Panel
                && (Application.Current.RootVisual as UserControl).Content == this.m_container)
            {
                UIElementCollection collection = ((Application.Current.RootVisual as UserControl).Content as Panel).Children;
                foreach (var item in collection)
                {
                    if (item.GetType().Name == "MainPage")
                    {
                        var control = item as UserControl;
                        if (control != null && m_attachedRootVisualListener)
                        {
                            control.GotFocus -= new RoutedEventHandler(control_GotFocus);
                            m_attachedRootVisualListener = false;
                        }
                        break;
                    }
                }
            }
        }

        #region method

        public void ShowDialog(FrameworkElement content, string title, bool isModal)
        {
            ShowDialog(content, title, isModal, Size.Empty, null);
        }

        public void ShowDialog(FrameworkElement content, string title, bool isModal, Size size, ResultHandler handler)
        {
            if (content == null)
            {
                return;
            }
            Border border = new Border { Margin = new Thickness(-20, -10, -20, -20), Child = content };
            if (handler != null)
            {
                ResultHandler = handler;
            }
            Content = border;
            Title = title;
            PopType = PopType.Dialog;
            this.RootElementSize = size;

            Show(isModal);
        }

        public void ShowDialog(FrameworkElement content, string title, bool isModal, Size size)
        {
            ShowDialog(content, title, isModal, size, null);
        }

        public void ShowDialog(FrameworkElement content, string title, bool isModal, ResultHandler handler)
        {
            ShowDialog(content, title, isModal, Size.Empty, handler);
        }

        public void ShowDialog(string title, string url, bool isModal, Size size, ResultHandler handler)
        {
            if (handler != null)
            {
                ResultHandler = handler;
            }
            GridContainer gridContainer = new GridContainer();
            gridContainer.LoadModule += new EventHandler<Newegg.Oversea.Silverlight.Core.Components.LoadedMoudleEventArgs>(canvasContainer_LoadModule);
            gridContainer.Tag = isModal;
            CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.Show();
            gridContainer.Load(url);
            Content = gridContainer;
            PopType = PopType.Dialog;
            this.RootElementSize = size;
            this.Title = title;
        }

        public void ShowDialog(string title, string url, bool isModal, Size size)
        {
            ShowDialog(title, url, isModal, size, null);
        }

        public void ShowDialog(string title, string url, bool isModal, ResultHandler handler)
        {
            ShowDialog(title, url, isModal, Size.Empty, handler);
        }

        public void ShowDialog(string title, string url, bool isModal)
        {
            ShowDialog(title, url, isModal, Size.Empty, null);
        }

        void canvasContainer_LoadModule(object sender, LoadedMoudleEventArgs e)
        {
            if (e.Status == LoadModuleStatus.End)
            {
                Request request = e.Request;
                IViewInfo viewInfo = request.ModuleInfo.GetViewInfoByName(request.ViewName);
                if (viewInfo != null)
                {
                    object view = viewInfo.GetViewInstance(new PageContext(request, m_browser));
                    IPage page = view as IPage;
                    (sender as IContainer).Children.Add(view as UIElement);
                    var pageBase = view as PageBase;
                    if (pageBase != null)
                    {
                        pageBase.Window.DialogClosed += new EventHandler<ResultEventArgs>(Window_DialogClosed);
                        this.Closed += (obj, args) =>
                        {
                            pageBase.Window.DialogClosed -= new EventHandler<ResultEventArgs>(Window_DialogClosed);
                        };

                        this.Closing += (obj, args) =>
                        {
                            var args2 = new ClosingEventArgs { Cancel = false };

                            pageBase.Window.ClosingDialog(args2);

                            args.Cancel = args2.Cancel;
                        };
                    }

                    if (page != null && pageBase != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            page.Context.OnPageLoad(page, new EventArgs());
                        });
                    }
                    CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.Hide();
                    Show((bool)(sender as Panel).Tag);
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.LoadingSpin.Hide();
                    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(MessageResource.PopupBox_CannotFoundPage, MessageBoxType.Error);
                }
            }
        }

        void Window_DialogClosed(object sender, ResultEventArgs e)
        {
            this.ResultArgs = e;
            this.Close(e.isForce);
        }

        public void ShowDialog(FrameworkElement content, string title)
        {
            ShowDialog(content, title, true);
        }


        public void Alert(string content, MessageType messageType)
        {
            string title = MessageResource.ResourceManager.GetString(string.Format("PopupBox_Title_{0}", messageType.ToString()));
            Show(content, title, PopType.Alert, messageType, true);
        }

        public void Confirm(string content, ResultHandler handler)
        {
            ResultHandler += handler;
            string title = MessageResource.ResourceManager.GetString(string.Format("PopupBox_Title_{0}", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Confirm.ToString()));
            Show(content, title, PopType.Confirm, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Confirm);
        }

        private void Show(string content, string title, PopType popType, MessageType messageType)
        {
            Show(content, title, popType, messageType, true);
        }

        private void Show(string content, string title, PopType popType, MessageType messageType, bool isModal)
        {
            Grid grid = new Grid();
            grid.Width = 380d;
            grid.VerticalAlignment = VerticalAlignment.Top;
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);

            col1.Width = GridLength.Auto;

            TextBlock txtContent = new TextBlock();

            txtContent.TextWrapping = TextWrapping.Wrap;
            txtContent.TextAlignment = TextAlignment.Left;
            txtContent.Text = content;
            txtContent.FontSize = 12D;
            txtContent.Margin = new Thickness(12, 0, 0, 0);
            txtContent.VerticalAlignment = VerticalAlignment.Center;
            txtContent.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            txtContent.FontFamily = new FontFamily("Tahoma,SimSun,PMingLiU");

            txtContent.SetValue(Grid.ColumnProperty, 1);

            grid.Tag = content;
            Content = grid;
            Title = title;
            PopType = popType;
            MessageType = messageType;
            ImageIcon = new Image();
            ImageIcon.Stretch = Stretch.None;
            ImageIcon.Margin = new Thickness(0, 1, 0, 0);
            ImageIcon.VerticalAlignment = VerticalAlignment.Top;
            ImageIcon.SetValue(Grid.ColumnProperty, 0);

            grid.Children.Add(txtContent);
            grid.Children.Add(ImageIcon);

            Show(isModal);
        }

        private void DisplayIcon(MessageType type)
        {
            switch (type)
            {
                case Newegg.Oversea.Silverlight.Controls.Components.MessageType.Error:
                    {
                        ImageIcon.Style = this.m_rootElement.Resources["errorStyle"] as Style;
                        this.m_titleElement.Text = MessageResource.PopupBox_Title_Error;
                    }
                    break;
                case Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information:
                    {
                        ImageIcon.Style = this.m_rootElement.Resources["infoStyle"] as Style;
                        this.m_titleElement.Text = MessageResource.PopupBox_Title_Information;
                    }
                    break;
                case Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning:
                    {
                        ImageIcon.Style = this.m_rootElement.Resources["warningStyle"] as Style;
                        this.m_titleElement.Text = MessageResource.PopupBox_Title_Warning;
                    }
                    break;
                case Newegg.Oversea.Silverlight.Controls.Components.MessageType.Confirm:
                    {
                        ImageIcon.Style = this.m_rootElement.Resources["confirmStyle"] as Style;
                        this.m_titleElement.Text = MessageResource.PopupBox_Title_Confirm;
                    }
                    break;
            }
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "PopupBoxComponent"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            m_browser = browser;
        }

        public object GetInstance(TabItem tab)
        {
            return new PopupBox(tab.Content as Panel, Browser);
        }

        public void Dispose()
        { }
        #endregion

        #region IAlert Members
        public void Alert(string title, string content, MessageType messageType)
        {
            Alert(title, content, messageType, null, null);
        }

        public void Alert(string title, string content, MessageType messageType, ResultHandler handler, Panel container)
        {
            if (this.Current == null)
            {
                if (container == null)
                {
                    container = m_browser.SelectedContent as Panel;
                }

                PopupBox box = new PopupBox(container, this);
                box.ResultHandler += handler;
                box.Alert(title, content, messageType, handler, container);
            }
            else
            {
                if (UtilityHelper.IsNullOrEmpty(title))
                {
                    title = MessageResource.ResourceManager.GetString(string.Format("PopupBox_Title_{0}", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information.ToString()));
                }

                Show(content, title, PopType.Alert, messageType);
            }
        }

        #endregion

        #region IConfirm Members

        public void Confirm(string title, string content, ResultHandler handler, Panel container)
        {
            Confirm(title, content, handler, ButtonType.OKCancel, container);
        }

        public void Confirm(string title, string content, ResultHandler handler, ButtonType buttonType, Panel container)
        {
            if (this.Current == null)
            {
                if (container == null)
                {
                    container = m_browser.SelectedContent as Panel;
                }
                PopupBox box = new PopupBox(container, this);
                box.ResultHandler += handler;
                box.Confirm(title, content, handler, buttonType, container);
            }
            else
            {
                if (UtilityHelper.IsNullOrEmpty(title))
                {
                    title = MessageResource.ResourceManager.GetString(string.Format("PopupBox_Title_{0}", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Confirm.ToString()));
                }

                m_buttonType = buttonType;

                Show(content, title, PopType.Confirm, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Confirm);
            }
        }


        #endregion


        #region IDialog Members

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, IPageBrowser pageBrowser)
        {
            return ShowDialog(title, url, callback, size, null, pageBrowser);
        }

        public IDialog ShowDialog(string title, string url, ResultHandler callback, Size size, Panel container, IPageBrowser pageBrowser)
        {
            if (m_browser == null)
            {
                m_browser = pageBrowser;
            }

            if (this.Current == null)
            {
                if (container == null)
                {
                    container = m_browser.SelectedContent as Panel;
                }

                PopupBox box = new PopupBox(container, this);
                box.Closed = this.Closed;
                box.ShowDialog(title, url, callback, size, container, pageBrowser);
                return box;
            }
            else
            {
                this.ShowDialog(title, url, true, size, callback);
                return this;
            }
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size)
        {
            return ShowDialog(title, content, callback, size, null);
        }

        public IDialog ShowDialog(string title, FrameworkElement content, ResultHandler callback, Size size, Panel container)
        {
            if (this.Current == null)
            {
                if (container == null)
                {
                    container = m_browser.SelectedContent as Panel;
                }
                PopupBox box = new PopupBox(container, this);
                box.Closed = this.Closed;
                box.ShowDialog(title, content, callback, size, container);
                return box;
            }
            else
            {
                this.ShowDialog(content, title, true, size, callback);
                return this;
            }
        }

        #endregion
    }
}
