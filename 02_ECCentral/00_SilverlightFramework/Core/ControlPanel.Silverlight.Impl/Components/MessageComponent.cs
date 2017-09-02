using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Containers;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface IMessageComponent : IComponent
    {}

    public class MessageComponent : IMessageComponent
    {
        private IPageBrowser m_browser;


        #region IComponent Members

        public string Name
        {
            get { return "MessageBox"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
            m_browser = browser;
            browser.ApplyTemplateHandle += new ApplyTemplateEventHandler(MessageBox.browser_ApplyTemplate);
            browser.SelectionChanged += new SelectionChangedEventHandler(browser_SelectionChanged);
        }

        public object GetInstance(TabItem tab)
        {
            return new MessageBox(tab as PageTab, m_browser);
        }

        void browser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PageTab tab;

            if (e.RemovedItems.Count > 0)
            {
                tab = e.RemovedItems[0] as PageTab;

                if (tab != null && tab.View != null)
                {
                    if (MessageBox.m_panel != null)
                    {
                        MessageBox.m_panel.Visibility = Visibility.Collapsed;

                        MessageBox.m_tbMessage.Text = "";
                        MessageBox.m_btnTitle.Content = "";
                    }
                }
            }

            if (e.AddedItems.Count > 0)
            {
                tab = e.AddedItems[0] as PageTab;

                if (tab != null && tab.View != null)
                {
                    if (MessageBox.m_panel != null)
                    {

                        MessageBox.m_panel.Visibility = Visibility.Collapsed;

                        MessageBox.m_tbMessage.Text = "";
                        MessageBox.m_btnTitle.Content = "";
                    }


                    MessageBox.m_panel.Tag = tab.View.Context.Window.MessageBox;
                    if (MessageBox.m_panel.Tag != null)
                    {
                        (MessageBox.m_panel.Tag as MessageBox).SetMessageBox(true);
                    }
                }

            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        private class MessageBox:IMessageBox
        {
            internal static Button m_btnTitle;
            internal static Grid m_container;
            internal static StackPanel m_panel;
            internal static TextBox m_tbMessage;
            internal static Grid m_rightFooter;
            internal static Grid m_centerFooter;
            internal static Grid m_leftFooter;
            internal static Grid m_content;
            internal static Border m_borderTitle;
            //internal static Grid m_gridClose;
            internal static Button m_btnClose;

            private const int CONST_TIMESPAN = 3000;
            private IPageBrowser m_browser;
            internal bool m_autoExpand = true;
            private PageTab m_pageTab;
            internal Timer m_timer;
            internal MessageInfo m_message;

            public PageTab PageTab
            {
                get
                {
                    return m_pageTab;
                }
            }

           public MessageBox(PageTab tab,IPageBrowser browser)
           {
                m_pageTab = tab;
                m_browser = browser;

                if (m_panel != null)
                {
                    m_panel.Tag = this;
                }
           }

            public static void browser_ApplyTemplate(object sender, ApplyTemplateEventArgs args)
            {
                m_panel = args.GetChildTemplate("stackMessageBox") as StackPanel;
                m_container = args.GetChildTemplate("Container") as Grid;
                m_tbMessage = args.GetChildTemplate("tbMessage") as TextBox;
                m_btnTitle = args.GetChildTemplate("btnTitle") as Button;
                m_content = args.GetChildTemplate("gridContent") as Grid;
                m_leftFooter = args.GetChildTemplate("gridFooter_left") as Grid;
                m_rightFooter = args.GetChildTemplate("gridFooter_right") as Grid;
                m_centerFooter = args.GetChildTemplate("gridFooter_center") as Grid;
                m_borderTitle = args.GetChildTemplate("borderTitle") as Border;
                //m_gridClose = args.GetChildTemplate("gridClose") as Grid;
                m_btnClose = args.GetChildTemplate("btnClose") as Button;
               

                m_panel.MouseEnter += new MouseEventHandler(m_panel_MouseEnter);
                m_panel.MouseLeave += new MouseEventHandler(m_panel_MouseLeave);
               
                
                m_btnTitle.Click += new RoutedEventHandler(m_btnTitle_Click);
                m_btnClose.Click += new RoutedEventHandler(m_btnClose_Click);
            }

          
            #region IMessageBox Members

            
            static void m_panel_MouseLeave(object sender, MouseEventArgs e)
            {
                MessageBox control;
                if (m_panel.Tag != null && (control = m_panel.Tag as MessageBox) != null)
                {
                    if (control.m_message != null && control.m_message.Type == MessageBoxType.Success && control.m_timer != null)
                    {
                        control.m_timer.Change(CONST_TIMESPAN, 0);
                    }
                }

            }

            static void m_panel_MouseEnter(object sender, MouseEventArgs e)
            {
                MessageBox control;
                if (m_panel.Tag != null && (control = m_panel.Tag as MessageBox) != null)
                {
                    if (control.m_message != null && control.m_message.Type == MessageBoxType.Success && control.m_timer != null)
                    {
                        control.m_timer.Change(Timeout.Infinite, 0);
                    }
                }
            }
            

           

            static void m_btnClose_Click(object sender, RoutedEventArgs e)
            {
                MessageBox control;
                if (m_panel.Tag != null && (control = m_panel.Tag as MessageBox) != null)
                {
                    control.Clear();
                }
            }

            static void m_btnTitle_Click(object sender, RoutedEventArgs e)
            {
                MessageBox control;

                if (m_panel.Tag != null && (control = m_panel.Tag as MessageBox) != null)
                {
                    control.m_autoExpand = false;
                    if (control.m_message != null)
                    {
                        if (m_container != null)
                        {
                            control.SetMessageBox(m_container.Visibility != Visibility.Collapsed);
                        }
                    }
                    else
                    {
                        if (m_container != null && m_tbMessage != null)
                        {
                            m_container.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            internal void SetMessageBox(bool isCollapsed)
            {
                string styleKey;

                if (m_browser.SelectedItem != PageTab)
                {
                    return;
                }

                if (m_message == null && m_panel != null)
                {
                    m_panel.Visibility = Visibility.Collapsed;
                    return;
                }
                styleKey = "Close_white";
                if (m_message.Type == MessageBoxType.Information || m_message.Type== MessageBoxType.Warning)
                {
                    styleKey = "Close_Black";

                    
                }

                if (m_panel != null && m_panel.Resources.Contains(styleKey))
                {
                    m_btnClose.Style = m_panel.Resources[styleKey] as Style;
                }
               
               

                if (m_borderTitle != null)
                {
                    styleKey = string.Format("borderTitle_{0}", m_message.Type.ToString());
                    if (m_panel != null && m_panel.Resources.Contains(styleKey))
                    {
                        m_borderTitle.Background = m_panel.Resources[styleKey] as Brush;
                    }

                    styleKey = string.Format("borderTitle_boderColor_{0}", m_message.Type.ToString());
                    if (m_panel != null && m_panel.Resources.Contains(styleKey))
                    {
                        m_borderTitle.BorderBrush = m_panel.Resources[styleKey] as Brush;
                    }
                }

                //if (m_gridClose != null)
                //{
                //    styleKey = string.Format("borderTitle_{0}", m_message.Type.ToString());
                //    if (m_panel != null && m_panel.Resources.Contains(styleKey))
                //    {
                //        m_gridClose.Background = m_panel.Resources[styleKey] as Brush;
                //    }
                //}

                if (isCollapsed)
                {
                    if (m_message != null)
                    {
                        styleKey = string.Format("btnTitle_{0}", m_message.Type.ToString());
                        if (m_panel != null && m_panel.Resources.Contains(styleKey))
                        {
                            m_panel.Visibility = Visibility.Visible;

                            if (m_btnTitle != null)
                            {
                                m_btnTitle.Style = m_panel.Resources[styleKey] as Style;
                                m_btnTitle.DataContext = m_message;
                            }
                        }
                    }
                    else
                    {
                        if (m_panel != null )
                        {
                            m_panel.Visibility = Visibility.Collapsed;
                        }
                    }

                    if (m_container != null)
                    {
                        m_container.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                   
                    if (m_container != null)
                    {
                        m_container.Visibility = Visibility.Visible;
                    }
                    
                    if (m_tbMessage != null)
                    {
                        m_tbMessage.Text = m_message.Body;
                    }
                }

                if (m_btnTitle != null && isCollapsed && m_autoExpand)
                {
                    m_btnTitle.ApplyTemplate();
                    Queue<DependencyObject> queue = new Queue<DependencyObject>();
                    DependencyObject current, child;
                    TextBlock txtMessage = null;
                    queue.Enqueue(m_btnTitle);

                    while (queue.Count > 0)
                    {
                        current = queue.Dequeue();
                        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(current); i++)
                        {
                            child = System.Windows.Media.VisualTreeHelper.GetChild(current, i);
                            if ((child as FrameworkElement).Name == "txtMessage")
                            {
                                txtMessage = child as TextBlock;
                                break;
                            }
                            else
                            {
                                queue.Enqueue(child);
                            }

                        }

                        if (txtMessage != null)
                        {
                            break;
                        }
                    }

                    if (txtMessage != null)
                    {
                        TextBlock txt = new TextBlock()
                        { 
                             Text = txtMessage.Text,
                             FontFamily = txtMessage.FontFamily,
                             FontSize = txtMessage.FontSize,
                             FontSource = txtMessage.FontSource,
                             FontStretch = txtMessage.FontStretch,
                             FontStyle =txtMessage.FontStyle,
                             FontWeight = txtMessage.FontWeight,
                             TextWrapping = TextWrapping.Wrap,
                             Margin = txtMessage.Margin,
                             Padding = txtMessage.Padding,
                             Width = txtMessage.ActualWidth
                        };

                        if (txtMessage.ActualHeight > m_btnTitle.Height)
                        {
                            this.SetMessageBox(false);
                        }
                    }
                }
            }

            public void Show(string message)
            {
                Show(message, MessageBoxType.Success);
            }

            public void Show(string message, MessageBoxType type)
            {
                Show(new MessageInfo(message, type));
            }

            public void Show(MessageInfo message)
            {
                if (LayoutMask.RootVisualContainerCount > 0)
                {
                    var rootContainer = (Application.Current.RootVisual as UserControl).FindName("LayoutRoot") as Panel;
                    if (!rootContainer.Children.Contains(m_panel))
                    {
                        LayoutMask.OldMessageBoxParent = m_panel.Parent as Panel;
                        LayoutMask.MessageBoxPanel = m_panel;
                        ((Panel)m_panel.Parent).Children.Remove(m_panel);
                        rootContainer.Children.Add(m_panel);
                    }
                }

                if (PageTab == null)
               {
                   return;
               }

               m_autoExpand = true;
               m_message = message;
               if (message.Type == MessageBoxType.Success)
               {
                   if (m_timer == null)
                   {
                       m_timer = new Timer(new TimerCallback(delegate(object tab)
                       {
                           if (tab != null)
                           {
                               (tab as TabItem).Dispatcher.BeginInvoke(new Action(delegate()
                               {
                                   if (tab != null)
                                   {
                                       (tab as TabItem).UpdateLayout();
                                       if ((tab as TabItem).IsSelected)
                                       {
                                           Clear();
                                       }
                                   }
                               }));

                           }
                       }), PageTab, CONST_TIMESPAN, 0);
                   }
                   else
                   {
                       m_timer.Change(CONST_TIMESPAN, 0);
                   }
               }
               else
               {
                   if (m_timer != null && PageTab != null)
                   {
                       m_timer.Dispose();
                       m_timer = null;
                   }
               }

               SetMessageBox(true);
            }

            public void Clear()
            {
                if (m_browser.SelectedItem == PageTab)
                {
                    Dispose();
                    SetMessageBox(true);
                }
                else
                {
                    Dispose();
                }
            }


            #endregion


            #region IDisposable Members

            public void  Dispose()
            {
                if (m_timer != null)
                {
                    m_timer.Dispose();
                }
                m_timer = null;
                m_message = null;
            }

            #endregion
            }

        public class MessageInfo
        {
            private MessageBoxType m_type;
            private string m_body;
            private DateTime m_date;

            public MessageBoxType Type
            {
                get
                {
                    return m_type;
                }
            }

            public string Body
            {
                get
                {
                    return m_body;
                }
            }

            public DateTime CreateTime
            {
                get
                {
                    return m_date;
                }
            }

            public MessageInfo(string body, MessageBoxType type)
            {
                m_body = body;
                m_type = type;
                m_date = DateTime.Now;
            }
        }
    }
}
