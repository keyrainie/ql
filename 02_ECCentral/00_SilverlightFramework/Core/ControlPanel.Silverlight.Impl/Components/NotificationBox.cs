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
using System.Threading;

using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class NotificationBox : INotificationBox
    {
        private IPageBrowser m_browser;
        private NotificationTip m_NotifyTip;

        #region IComponent Members
        public string Name
        {
            get { return "NotificationBox"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            m_browser = browser;
            browser.ApplyTemplateHandle += new ApplyTemplateEventHandler(browser_ApplyTemplate);            
        }       

        void browser_ApplyTemplate(object sender, ApplyTemplateEventArgs e)
        {
            m_NotifyTip = e.GetChildTemplate("NotificationTip") as NotificationTip;
            m_NotifyTip.ApplyTemplate();                
        }

        public object GetInstance(System.Windows.Controls.TabItem tab)
        {
            return this;
        }

        public void Dispose()
        {
            
        }
        #endregion

        #region INotificationBox Members

        public void Show(string title, string content, TimeSpan displayTime)
        {
            TextBlock txt = new TextBlock();
            txt.TextTrimming = TextTrimming.WordEllipsis;
            txt.Text = content;
            txt.TextWrapping = TextWrapping.Wrap;
            txt.TextAlignment = TextAlignment.Left;
            txt.HorizontalAlignment = HorizontalAlignment.Left;

            Show(title, txt, displayTime);
        }

        public void Show(string title, FrameworkElement content, TimeSpan displayTime)
        {
            m_NotifyTip.Title = title;
            m_NotifyTip.Content = content;
            m_NotifyTip.DisplayTime = displayTime;
            m_NotifyTip.Show();
        }

        #endregion
    }

    public class NotificationTip : ContentControl
    {
        private Timer m_timer;
        private ContentControl m_content;
        private Button m_closeButton;
        private Image m_imageIcon;
        private TextBlock m_textBlockTitle;


        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public TimeSpan DisplayTime { get; set; }

        public string Title { get; set; }

        public object Content { get; set; }
        

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotificationTip), new PropertyMetadata(null));

        public NotificationTip()
        {
            DefaultStyleKey = typeof(NotificationTip);
        }
      
        public override void OnApplyTemplate()
        {
            m_imageIcon = GetTemplateChild("ImageIcon") as Image;
            m_textBlockTitle = GetTemplateChild("TextBlockTitle") as TextBlock;
            m_content = GetTemplateChild("ContentHolder") as ContentControl;
            m_closeButton = GetTemplateChild("ButtonClose") as Button;
            m_closeButton.Click += new RoutedEventHandler(m_closeButton_Click);
          
            if (this.Icon != null)
            {
                m_imageIcon.Source = this.Icon;
            }

            base.OnApplyTemplate();
        }

        void m_closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       

        void Close()
        {
            if (m_timer != null)
            {
                m_timer = null;
            }

            VisualStateManager.GoToState(this, "Close", false);
        }

        public void Show()
        {
            if (m_timer == null)
            {
                m_timer = new System.Threading.Timer(new System.Threading.TimerCallback(p =>
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        Close();
                    });

                }), null, DisplayTime, TimeSpan.Zero);
            }
            else
            {
                m_timer.Change(DisplayTime, TimeSpan.Zero);
            }

            m_textBlockTitle.Text = Title;
            m_content.Content = Content;

            this.Visibility = System.Windows.Visibility.Visible;
            this.SetValue(Canvas.ZIndexProperty, 999);

            VisualStateManager.GoToState(this, "Show", false);
        }
    } 
}
