using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.CommonDomain.Models;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Utilities;
using System.Collections.Generic;
using System;

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class UCAddLinkPage : UserControl
    {
        private MenuItemModel m_category;
        private RestClient m_restServiceClient;
        private ObservableCollection<MenuItemModel> m_menuTree;
        private ObservableCollection<MenuItemModel> m_menuCollection;
        private ObservableCollection<MenuItemModel> m_functions;

        public IDialog Dialog { get; set; }

        public UCAddLinkPage(ObservableCollection<MenuItemModel> menuTree, ObservableCollection<MenuItemModel> menuCollection,MenuItemModel category)
        {
            InitializeComponent();

            m_restServiceClient = new RestClient("/Service/Framework/V50/MenuRestService.svc", CPApplication.Current.CurrentPage);

            ButtonSave.Click += new System.Windows.RoutedEventHandler(ButtonSave_Click);
            ButtonCancel.Click += new System.Windows.RoutedEventHandler(ButtonCancel_Click);

            this.m_category = category;

            this.TextBlockCategoryName.Text = category.LocalizedDisplayName;
            this.tvMenu.ItemsSource = menuTree;
            this.m_menuTree = menuTree;
            this.m_menuCollection = menuCollection;
            this.m_functions = GetPageFunctions(menuCollection);
        }

        void ButtonCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.Close();
            }
        }

        void ButtonSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectItem = tvTree.SelectedItem as MenuItemModel;

            if (selectItem != null && selectItem.Type == "P")
            {
                var menuItem = selectItem.Clone() as MenuItemModel;

                if (menuItem != null)
                {
                    var data = new MenuItemModel();
                    data.MenuId = null;
                    data.DisplayName = menuItem.DisplayName;
                    data.LinkPath = menuItem.MenuId.ToString();
                    data.Type = "L";
                    data.ParentMenuId = m_category.MenuId;
                    data.Status = "A";
                    data.IsDisplay = menuItem.IsDisplay;
                    data.ApplicationId = menuItem.ApplicationId;
                    data.LanguageCode = menuItem.LanguageCode;
                    data.InUser = CPApplication.Current.LoginUser.ID;

                    ButtonSave.IsEnabled = false;
                    ButtonCancel.IsEnabled = false;
                    m_restServiceClient.Create<MenuItemModel>("", data, (target, args) =>
                    {
                        ButtonSave.IsEnabled = true;
                        ButtonCancel.IsEnabled = true;
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        var result = args.Result;

                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(CommonResource.Info_CreateSuccessfully, MessageBoxType.Success);

                        if (Dialog != null)
                        {
                            Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = result };
                            Dialog.Close();
                        }
                    });
                }

            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("Warning", MenuMaintainResource.FieldLabel_SelectPage, MessageType.Warning);
            }
        }

        void ICONActive_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchFunctions();
        }
        private void TextBoxSearchPage_KeyUp(object sender, KeyEventArgs e)
        {
            SearchFunctions();
        }
        

        private ObservableCollection<MenuItemModel> GetPageFunctions(ObservableCollection<MenuItemModel> source)
        {
            return new ObservableCollection<MenuItemModel>(
                from item in source
                where item.Type == "P"
                select item
            );
        }

        private void GenerateMenuPath(string parentMenuId, ref List<string> menuPaths)
        {
            foreach (var item in m_menuCollection)
            {
                if (item.MenuId.Value.ToString() == parentMenuId)
                {
                    menuPaths.Insert(0, item.DisplayName);

                    if (item.ParentMenuId != null)
                    {
                        GenerateMenuPath(item.ParentMenuId.Value.ToString(), ref menuPaths);
                    }
                }
            }
        }

        private void SearchFunctions()
        {
            if (!TextBoxSearchPage.Text.IsNullOrEmpty())
            {
                var searchResult = new ObservableCollection<MenuItemModel>(
                        from item in m_functions
                        where item.LocalizedDisplayName.Trim().ToLower().Contains(TextBoxSearchPage.Text.Trim().ToLower())
                        select item
                    );

                foreach (var item in searchResult)
                {
                    var menuPaths = new List<string>();
                    GenerateMenuPath(item.ParentMenuId.HasValue ? item.ParentMenuId.Value.ToString() : null, ref menuPaths);

                    var menuPath = string.Empty;

                    for (var i = 0; i < menuPaths.Count; i++)
                    {
                        if (i == 0)
                        {
                            menuPath = menuPaths[i];
                        }
                        else
                        {
                            menuPath += "=>" + menuPaths[i];
                        }
                    }

                    item.MenuPath = menuPath;
                }

                this.tvMenu.ItemsSource = searchResult;
            }
            else
            {
                this.tvMenu.ItemsSource = this.m_menuTree;
            }
        }

        
    }
}
