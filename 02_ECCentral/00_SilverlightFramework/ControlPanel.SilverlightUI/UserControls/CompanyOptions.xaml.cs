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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class CompanyOptions : UserControl
    {
        public CompanyOptions()
        {
            InitializeComponent();

            GridCompanyOptions.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            GridCompanyOptions.MouseLeave += new MouseEventHandler(Menu_MouseLeave);
            GridPopCompanyOptions.MouseEnter += new MouseEventHandler(Menu_MouseEnter);
            GridPopCompanyOptions.MouseLeave += new MouseEventHandler(Menu_MouseLeave);

            ListBoxCompanyOptions.SelectionChanged += new SelectionChangedEventHandler(ListBoxCompanyOptions_SelectionChanged);
        }

        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CPApplication.Current.CompanyList.Count > 1)
            {
                iconTriangle.Visibility = Visibility.Visible;
            }
            else
            {
                iconTriangle.Visibility = Visibility.Collapsed;
            }
            TextBlockDisplayName.Text = CPApplication.Current.CompanyName; 

            ListBoxCompanyOptions.Items.Clear();
            foreach(UICompany company in CPApplication.Current.CompanyList)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = company.CompanyName;
                item.Tag = company.CompanyCode;
                 
                ListBoxCompanyOptions.Items.Add(item);
            }

            ListBoxCompanyOptions.SelectedValue = CPApplication.Current.CompanyCode;
            
        }

        void ListBoxCompanyOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selectedValue = ListBoxCompanyOptions.SelectedValue.ToString();

            ListBoxItem item = (ListBoxItem)ListBoxCompanyOptions.SelectedItem;
            string companyCode = item.Tag.ToString();
            if (companyCode.ToLower() != CPApplication.Current.CompanyCode.ToLower())
            {
                CPApplication.Current.Browser.Confirm("Warning", PageResource.LblChangeCompanyConfirm, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            CPApplication.Current.CompanyCode = companyCode;
                            UtilityHelper.SetCurrentCompanyCode(companyCode);

                            CPApplication.Current.CompanyName = item.Content.ToString();
                            UtilityHelper.SetCurrentCompanyName(item.Content.ToString());

                            UtilityHelper.RestartApplication();
                        }
                        else
                        {
                            ListBoxCompanyOptions.SelectedValue = CPApplication.Current.CompanyCode;
                        }
                    });
            }
        }

        void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupCompanyOptions.IsOpen = false;
            GridCompanyOptions_Background.Visibility = Visibility.Collapsed;
        }

        void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            PopupCompanyOptions.IsOpen = true;
            GridCompanyOptions_Background.Visibility = Visibility.Visible;
        }
    }
}
