using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Browser;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Views;
using ControlPanel.SilverlightUI;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class PersonalOptions : UserControl
    {
        public PersonalOptions()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PersonalOptions_Loaded);

            GridPersonalOptions.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            GridPersonalOptions.MouseLeave += new MouseEventHandler(Menu_MouseLeave);
            GridPopPersonalOptions.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            GridPopPersonalOptions.MouseLeave += new MouseEventHandler(Menu_MouseLeave);
        }

        void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupPersonalOptions.IsOpen = false;
            GridPersonalOptions_Background.Visibility = Visibility.Collapsed;
        }

        void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            PopupPersonalOptions.IsOpen = true;
            GridPersonalOptions_Background.Visibility = Visibility.Visible;
        }


        void PersonalOptions_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockDisplayName.Text = CPApplication.Current.LoginUser.DisplayName;
        }

        private void ListItem_LoginOut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm(PageResource.LblLogOutConfirm, (obj, result) =>
            {
                if (result.DialogResult == DialogResultType.OK)
                {
                    if (!Application.Current.IsRunningOutOfBrowser)
                    {
                        var url = string.Format("{0}#{1}", CPApplication.Current.PortalBaseAddress, CPApplication.Current.DefaultPage);
                        OnUrlChanged(url);
                    }
                    ComponentFactory.GetComponent<ILogin>().Logout(() =>
                    {
                        UtilityHelper.RestartApplication();
                    });
                }
            });
        }


        private void OnUrlChanged(string url)
        {
            try
            {
                HtmlPage.Window.Eval(string.Format(@"
                $(window).unbind(""hashchange"",window.hashHandle);
                window.location.href = ""{0}"";
                 $(window).bind(""hashchange"",window.hashHandle);
                $(window).hashchange();
            ", url.Replace("\"", "\\\"")));
            }
            catch (Exception ex)
            {
                ComponentFactory.Logger.LogError(ex, new object[] { string.Format(@"
                $(window).unbind(""hashchange"",window.hashHandle);
                window.location.href = ""{0}"";
                 $(window).bind(""hashchange"",window.hashHandle);
                $(window).hashchange();
            ", url.Replace("\"", "\\\"")) });
            }
        }

    }
}
