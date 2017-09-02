using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using ControlPanel.SilverlightUI;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.CommonService;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class GlobalOptions : UserControl
    {
        private string m_languageCode;
        private CommonServiceV40Client m_serviceClient;

        public GlobalOptions()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(GlobalOptions_Loaded);

            GridGlobalOptions.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            GridGlobalOptions.MouseLeave += new MouseEventHandler(Menu_MouseLeave);

            GridPopGlobalOptions.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            GridPopGlobalOptions.MouseLeave += new MouseEventHandler(Menu_MouseLeave);

            ListItemLanguage.MouseEnter += new MouseEventHandler(LanguageOptions_MouseEnter);
            ListItemLanguage.MouseLeave += new MouseEventHandler(LanguageOptions_MouseLeave);

            GridPopLanguageOptions.MouseEnter += new MouseEventHandler(LanguageOptions_MouseEnter);
            GridPopLanguageOptions.MouseLeave += new MouseEventHandler(LanguageOptions_MouseLeave);

            ListBoxLanguage.SelectionChanged += new SelectionChangedEventHandler(ListBoxLanguage_SelectionChanged);
            this.m_serviceClient = new CommonServiceV40Client();
            this.m_serviceClient.CheckAppVersionCompleted += new System.EventHandler<CheckAppVersionCompletedEventArgs>(m_serviceClient_CheckAppVersionCompleted);
        }

        void m_serviceClient_CheckAppVersionCompleted(object sender, CheckAppVersionCompletedEventArgs e)
        {
            CPApplication.Current.Browser.LoadingSpin.Hide();
            if (e.Error != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(e.Error.Message, MessageBoxType.Error);
                return;
            }
            if (e.Result.Faults != null && e.Result.Faults.Count > 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(e.Result.Faults[0].ErrorDescription, MessageBoxType.Error);
                return;
            }

            var childWindow = new ChildWindow();
            var about = new About { ChildWindow = childWindow, AppVersion = e.Result };
            childWindow.Style = Application.Current.Resources["AboutWindowStyle"] as Style;
            childWindow.Content = about;
            childWindow.Show();
        }

        void GlobalOptions_Loaded(object sender, RoutedEventArgs e)
        {
            InitCurrentLanguage();
        }

        void LanguageOptions_MouseEnter(object sender, MouseEventArgs e)
        {
            Menu_MouseEnter(null, null);
            PopLanguageOptions.IsOpen = true;
        }

        void LanguageOptions_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid)
            {
                Menu_MouseLeave(null, null);
            }
            PopLanguageOptions.IsOpen = false;
        }

        void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            GridGlobalOptions_Background.Visibility = Visibility.Visible;
            PopupGlobalOptions.IsOpen = true;
        }

        void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            GridGlobalOptions_Background.Visibility = Visibility.Collapsed;
            PopupGlobalOptions.IsOpen = false;
        }

        void ListBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = ListBoxLanguage.SelectedValue.ToString();

            if (selectedValue.Trim().ToLower() != CPApplication.Current.LanguageCode.Trim().ToLower())
            {
                CPApplication.Current.Browser.Confirm("Warning", PageResource.LblChangeLanguageConfirm, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        CPApplication.Current.LanguageCode = selectedValue;
                        UtilityHelper.SetCurrentLanguageCode(CPApplication.Current.LanguageCode);
                        UtilityHelper.RestartApplication();
                    }
                    else
                    {
                        ListBoxLanguage.SelectedValue = m_languageCode;
                    }
                });
            }
        }

        private void ListItemAbout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CPApplication.Current.Browser.LoadingSpin.Show();
            Menu_MouseLeave(null, null);
            m_serviceClient.CheckAppVersionAsync();
        }

        private void ListItemHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //CPApplication.Current.CurrentPage.Context.Window.Alert("Coming soon...");

            Menu_MouseLeave(null, null);
        }

        private void InitCurrentLanguage()
        {
            m_languageCode = CPApplication.Current.LanguageCode.ToLower();

            ListBoxLanguage.SelectedValue = m_languageCode;
        }
    }
}
