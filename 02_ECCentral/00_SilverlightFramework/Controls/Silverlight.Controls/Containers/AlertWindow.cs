using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Resources;
using Newegg.Oversea.Silverlight.Utilities;
using System.Windows.Shapes;
using System.Threading;

namespace Newegg.Oversea.Silverlight.Controls.Containers
{
    public class AlertWindow : ChildWindow, IAlert, IConfirm
    {
        private event ResultHandler m_handler;

        private ResultEventArgs m_resultArgs;
        private MessageType m_messageType;
        private IPageBrowser m_browser;
        private ButtonType m_buttonType;
        private Grid m_rootElement;
        private Button m_comfirmElement;
        private Button m_cancelElement;
        private Button m_closeElement;
        private Border m_chrome;

        public AlertWindow()
        {
            this.DefaultStyleKey = typeof(AlertWindow);
            this.m_resultArgs = new ResultEventArgs() { DialogResult = DialogResultType.Cancel };

            this.KeyDown += new KeyEventHandler(AlertWindow_KeyDown);
        }

        public AlertWindow(IPageBrowser browser)
            : this()
        {
            this.m_browser = browser;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                if (Dialog.Dialogs.Count <= 0)
                {
                    Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (this.m_handler != null)
            {
                this.m_handler(this, this.m_resultArgs);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.m_rootElement = this.GetTemplateChild("Root") as Grid;
            this.m_comfirmElement = this.GetTemplateChild("ConfirmElement") as Button;
            this.m_cancelElement = this.GetTemplateChild("CancelElement") as Button;
            this.m_chrome = this.GetTemplateChild("Chrome") as Border;
            this.m_closeElement = this.GetTemplateChild("CloseButton") as Button;

            if (this.m_comfirmElement != null && this.m_cancelElement != null)
            {
                InitializeButtons();
            }

            if (this.m_rootElement != null)
            {
                InitializeType();
            }
        }

        #region IConfirm Members

        public void Confirm(string title, string content, ResultHandler callback, Panel container)
        {
            this.Confirm(title, content, callback, ButtonType.OKCancel, container);
        }

        public void Confirm(string title, string content, ResultHandler callback, ButtonType buttonType, Panel container)
        {
            if (UtilityHelper.IsNullOrEmpty(title))
            {
                title = MessageResource.ResourceManager.GetString(string.Format("PopupBox_Title_{0}", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information.ToString()));
            }

            if (callback != null)
            {
                this.m_handler = callback;
            }

            this.m_buttonType = buttonType;

            this.Show(title, content, Components.MessageType.Confirm);
        }

        #endregion

        #region IAlert Members

        public void Alert(string title, string content, MessageType type, ResultHandler handle, Panel container)
        {
            if (UtilityHelper.IsNullOrEmpty(title))
            {
                title = MessageResource.ResourceManager.GetString(string.Format("PopupBox_Title_{0}", Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information.ToString()));
            }

            if (handle != null)
            {
                this.m_handler = handle;
            }

            this.Show(title, content, type);
        }

        #endregion

        #region IComponent Members

        public string Version
        {
            get { return "1.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            this.m_browser = browser;
        }

        public object GetInstance(TabItem tab)
        {
            return new AlertWindow(this.m_browser);
        }

        public void Dispose()
        {
            this.Close();
        }

        #endregion

        #region Event

        void AlertWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void m_cancelElement_Click(object sender, RoutedEventArgs e)
        {
            this.m_resultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Close();
        }

        void m_comfirmElement_Click(object sender, RoutedEventArgs e)
        {
            this.m_resultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK };
            this.Close();
        }

        #endregion

        #region Private Methods

        private void InitializeButtons()
        {
            if (this.m_messageType != MessageType.Confirm)
            {
                this.m_cancelElement.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.m_comfirmElement.Content = this.m_buttonType == ButtonType.OKCancel ? MessageResource.PopupBox_Button_Confirm : MessageResource.PopupBox_Button_Yes;

            this.m_cancelElement.Content = this.m_buttonType == ButtonType.OKCancel ? MessageResource.PopupBox_Button_Cancel :
                MessageResource.PopupBox_Button_No;

            this.m_comfirmElement.Click += m_comfirmElement_Click;
            this.m_cancelElement.Click += m_cancelElement_Click;
        }

        private void InitializeType()
        {
            switch (this.m_messageType)
            {
                case MessageType.Error:
                    {
                        this.m_chrome.Style = this.m_rootElement.Resources["borderErrorStyle"] as Style;
                        this.m_closeElement.Style = this.m_rootElement.Resources["errorButtonStyle"] as Style;

                        this.Title = MessageResource.PopupBox_Title_Error;
                    }
                    break;
                case MessageType.Information:
                    {
                        this.m_chrome.Style = this.m_rootElement.Resources["borderInfoStyle"] as Style;
                        this.m_closeElement.Style = this.m_rootElement.Resources["infoButtonStyle"] as Style;

                        this.Title = MessageResource.PopupBox_Title_Information;
                    }
                    break;
                case MessageType.Warning:
                    {
                        this.m_chrome.Style = this.m_rootElement.Resources["borderWarningStyle"] as Style;
                        this.m_closeElement.Style = this.m_rootElement.Resources["warningButtonStyle"] as Style;

                        this.Title = MessageResource.PopupBox_Title_Warning;
                    }
                    break;
                case MessageType.Confirm:
                    {
                        this.m_chrome.Style = this.m_rootElement.Resources["borderConfirmStyle"] as Style;
                        this.m_closeElement.Style = this.m_rootElement.Resources["confirmButtonStyle"] as Style;

                        this.Title = MessageResource.PopupBox_Title_Confirm;
                    }
                    break;
            }
        }

        private void Show(string title, string content, MessageType messageType)
        {
            this.Content = CreateContent(content);
            this.Title = title;
            this.m_messageType = messageType;
            this.Show();
        }

        private TextBlock CreateContent(string content)
        {
            return new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = System.Windows.TextAlignment.Left,
                MinHeight = 40,
                Text = content,
                FontSize = 12D,
                Margin = new Thickness(5, 5, 5, 5),
                FontFamily = new System.Windows.Media.FontFamily(Application.Current.Resources["DefaultFontFamily"].ToString())
            };
        }

        #endregion
    }
}
