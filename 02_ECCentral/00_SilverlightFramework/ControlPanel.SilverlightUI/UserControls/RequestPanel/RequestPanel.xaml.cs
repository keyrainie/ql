using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Windows.Media;
using ControlPanel.SilverlightUI;
using System.Windows.Media.Imaging;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.RequestPanel
{
    public partial class RequestPanel : UserControl
    {
        private ObservableCollection<RequestInfo> m_requestInfos;
        private ProgressBar m_progressBar;
        private RequestLoader m_loader;
        private Dictionary<string, UIElement> m_requestControls;
        private Grid m_overlay;

        public Popup Popup { get; set; }

        public RequestPanel(Popup popup)
        {
            InitializeComponent();

            this.Popup = popup;

            this.Loaded += new RoutedEventHandler(RequestPanel_Loaded);
            this.Unloaded += new RoutedEventHandler(RequestPanel_Unloaded);

            CPApplication.Current.RequestPanel = popup;

            this.m_overlay = new Grid
            {
                Background = new SolidColorBrush(Colors.Transparent)
            };
            this.m_overlay.SetValue(Canvas.ZIndexProperty, 50);
            this.m_overlay.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(m_grid_MouseLeftButtonDown);

            this.ButtonOk.Click += new RoutedEventHandler(ButtonOk_Click);
            this.TabControlRequest.SelectionChanged += new SelectionChangedEventHandler(TabControlRequest_SelectionChanged);

            this.m_loader = new RequestLoader();
            this.m_loader.DownloadProgress += new System.Net.DownloadProgressChangedEventHandler(m_loader_DownloadProgress);

            this.m_requestControls = new Dictionary<string, UIElement>();

            BindTabControl();
        }

        void RequestPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Contains(m_overlay))
            {
                ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Add(m_overlay);
            }
        }

        void RequestPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Contains(m_overlay))
            {
                ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Remove(m_overlay);
            }
        }


        void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.Popup != null)
            {
                this.Popup.IsOpen = false;
            }
        }

        void m_grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Popup.IsOpen = !Popup.IsOpen;
        }

        void TabControlRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabItem = ((TabControl)sender).SelectedItem as TabItem;

            if (tabItem != null)
            {
                var requestInfo = tabItem.Tag as RequestInfo;
                GridPlaceHolder.Children.Clear();

                if (requestInfo != null)
                {
                    if (m_requestControls.ContainsKey(requestInfo.ClassName))
                    {
                        var control = m_requestControls[requestInfo.ClassName];

                        if (!GridPlaceHolder.Children.Contains(control))
                        {
                            GridPlaceHolder.Children.Add(control);
                        }
                    }
                    else
                    {
                        LoadContent(requestInfo);
                    }
                }
            }
        }

        private void LoadContent(RequestInfo requestInfo)
        {
            if (this.m_progressBar == null)
            {
                this.m_progressBar = new ProgressBar();
                this.m_progressBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                this.m_progressBar.Width = 180;
                this.m_progressBar.Height = 16;
            }
            this.m_progressBar.Value = 0;

            TabControlRequest.IsEnabled = false;

            if (!GridPlaceHolder.Children.Contains(this.m_progressBar))
            {
                GridPlaceHolder.Children.Add(this.m_progressBar);
            }

            m_loader.Load(requestInfo, obj =>
            {
                GridPlaceHolder.Children.Remove(this.m_progressBar);
                TabControlRequest.IsEnabled = true;

                if (obj is UIElement)
                {
                    var element = obj as UIElement;

                    if (!GridPlaceHolder.Children.Contains(element))
                    {
                        GridPlaceHolder.Children.Add(element);
                    }

                    if (requestInfo != null)
                    {
                        m_requestControls[requestInfo.ClassName] = element;
                    }
                }
                else
                {
                    var ex = obj as Exception;

                    if (ex != null)
                    {
                        var errorText = BuildErrorMsg(ex.Message);

                        if (!GridPlaceHolder.Children.Contains(errorText))
                        {
                            GridPlaceHolder.Children.Add(errorText);
                        }

                        if (requestInfo != null)
                        {
                            m_requestControls[requestInfo.ClassName] = errorText;
                        }
                    }
                }
            });
        }

        void m_loader_DownloadProgress(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (this.m_progressBar != null)
            {
                this.m_progressBar.Value = e.ProgressPercentage;
            }
        }

        private StackPanel BuildErrorMsg(string text)
        {
            var errorPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var errorImg = new Image();
            errorImg.Source = new BitmapImage(new Uri("/Themes/Default/Images/PageBrower/rp_error.png", UriKind.Relative));

            var errorText = new TextBlock
                       {
                           Text = text,
                           VerticalAlignment = System.Windows.VerticalAlignment.Center,
                           FontFamily = new System.Windows.Media.FontFamily("Tahoma,SimSun,PMingLiU"),
                           FontSize = 12,
                           Foreground = new SolidColorBrush(Colors.Red)
                       };

            errorPanel.Children.Add(errorImg);
            errorPanel.Children.Add(errorText);

            return errorPanel;
        }

        private void BindTabControl()
        {
            m_requestInfos = m_loader.LoadDataSource();

            if (m_requestInfos != null && m_requestInfos.Count > 0)
            {
                TabControlRequest.Items.Clear();

                var supportCount = (m_requestInfos.Count <= 12 ? m_requestInfos.Count : 12);

                for (var i = 0; i < supportCount; i++)
                {
                    var tabName = m_requestInfos[i].TabName;
                    var tabItem = new TabItem
                                      {
                                          MaxWidth = 120,
                                          Header = tabName,
                                          Tag = m_requestInfos[i]
                                      };
                    if (tabName.Length > 12)
                    {
                        ToolTipService.SetToolTip(tabItem, m_requestInfos[i].TabName);
                    }

                    if (m_requestInfos.Count == 1)
                    {
                        tabItem.Style = Application.Current.Resources["MiniSingleTabItemStyle"] as Style;
                    }
                    else if (i == 0)
                    {
                        tabItem.Style = Application.Current.Resources["MiniFirstTabItemStyle"] as Style;
                    }
                    else if (i == (supportCount - 1))
                    {
                        tabItem.Style = Application.Current.Resources["MiniLastTabItemStyle"] as Style;
                    }
                    else
                    {
                        tabItem.Style = Application.Current.Resources["MiniMiddleTabItemStyle"] as Style;
                    }

                    TabControlRequest.Items.Add(tabItem);
                }
            }
            else
            {
                var errorText = BuildErrorMsg(PageResource.RequestPanel_Msg_NoContent);

                GridPlaceHolder.Children.Clear();
                GridPlaceHolder.Children.Add(errorText);
            }
        }
    }
}
