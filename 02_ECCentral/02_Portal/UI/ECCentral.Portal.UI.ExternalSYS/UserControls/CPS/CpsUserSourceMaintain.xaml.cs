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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;


namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class CpsUserSourceMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        private CpsUserFacade facade;
        public int UserSysNo { private get; set; }
        public string Source { set { txtSource.Text = value; } }
        public CpsUserSourceMaintain()
        {
            InitializeComponent();
            this.UserSourceResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(UserSourceResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
               
                facade = new CpsUserFacade();
                this.UserSourceResult.Bind();
            };

            
        }

      

     

        void UserSourceResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.GetUserSource(UserSysNo, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.UserSourceResult.ItemsSource = arg.Result.Rows;
                this.UserSourceResult.TotalCount = arg.Result.TotalCount;
            });
           
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            StackPanel sp = (StackPanel)((StackPanel)link.Parent).FindName("spUpdate");
            sp.Visibility = Visibility.Visible;
            link.Visibility = Visibility.Collapsed;
            FrameworkElement element = this.UserSourceResult.Columns[1].GetCellContent(this.UserSourceResult.SelectedItem);
            if (element.GetType() == typeof(TextBox))
            {
                ((TextBox)element).IsEnabled = true;
            }
        }

      

        private void hlUpdate_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = UserSourceResult.SelectedItem as dynamic;
            if (string.IsNullOrEmpty(d.Name))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("渠道名称不能为空!",MessageType.Error);
                return;
            }
            CpsUserVM vm = new CpsUserVM()
            {
                UserSource = new CpsUserSourceVM() { ChanlName = d.Name, SysNo = d.SysNo }
            };
            facade.UpdateUserSource(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("更新成功!");
                HyperlinkButton link = (HyperlinkButton)sender;
                setActionStyle(link);
            });
        }

        private void hlClose_Click(object sender, RoutedEventArgs e)
        {
             HyperlinkButton link = (HyperlinkButton)sender;
             setActionStyle(link);

        }

        private void setActionStyle(HyperlinkButton link)
        {
            StackPanel sp = (StackPanel)link.Parent;
            sp.Visibility = Visibility.Collapsed;
            ((HyperlinkButton)((StackPanel)sp.Parent).FindName("hlEdit")).Visibility = Visibility.Visible;
          FrameworkElement element= this.UserSourceResult.Columns[1].GetCellContent(this.UserSourceResult.SelectedItem);
          if (element.GetType() == typeof(TextBox))
          {
              ((TextBox)element).IsEnabled = false;
          }
        }
        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }
}
