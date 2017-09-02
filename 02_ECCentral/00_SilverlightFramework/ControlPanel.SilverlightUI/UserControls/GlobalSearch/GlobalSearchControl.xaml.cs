using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using ControlPanel.SilverlightUI;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Browser;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class GlobalSearchControl : UserControl
    {
        private List<GlobalSearchModel> m_globalSearchModel = new List<GlobalSearchModel>();
        private BitmapImage m_normal_image;
        private BitmapImage m_hover_image;
        private bool m_isFocused;

        public GlobalSearchControl()
        {
            InitializeComponent();

            StackPanel_SearchArea.MouseEnter += new MouseEventHandler(StackPanel_SearchArea_MouseEnter);
            StackPanel_SearchArea.MouseLeave += new MouseEventHandler(StackPanel_SearchArea_MouseLeave);
            ButtonGlobalSearch.MouseLeftButtonUp += new MouseButtonEventHandler(ButtonGlobalSearch_MouseLeftButtonUp);
            CbGlobalSearchType.SelectionChanged += new SelectionChangedEventHandler(CbGlobalSearchType_SelectionChanged);

            TextBoxGlobalSearchValue.KeyDown += new KeyEventHandler(TextBoxGlobalSearchValue_KeyDown);
            TextBoxGlobalSearchValue.GotFocus += new RoutedEventHandler(TextBoxGlobalSearchValue_GotFocus);
            TextBoxGlobalSearchValue.LostFocus += new RoutedEventHandler(TextBoxGlobalSearchValue_LostFocus);

            m_normal_image = new BitmapImage(new Uri("/Themes/Default/Images/Button/search_normal.png", UriKind.Relative));
            m_hover_image = new BitmapImage(new Uri("/Themes/Default/Images/Button/search_blue.png", UriKind.Relative));

            InitGlobalSearchConfig();
        }


        private void InitGlobalSearchConfig()
        {
            try
            {
                var authComponent = ComponentFactory.GetComponent<IAuth>();
                var configComponent = ComponentFactory.GetComponent<IConfiguration>();

                var configValue = configComponent.GetConfigValue("Framework", "GlobalSearchConfig");

                if (!configValue.IsNullOrEmpty())
                {
                    var list = UtilityHelper.XmlDeserialize<List<GlobalSearchModel>>(configValue);

                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (authComponent.HasFunctionForPage(item.BaseUrl))
                            {
                                item.Name = GetKeyDescription(item.QuickKey).IsNullOrEmpty() ? item.Name : string.Format("{0}[{1}]", item.Name, GetKeyDescription(item.QuickKey));
                                m_globalSearchModel.Add(item);
                            }
                        }

                        if (m_globalSearchModel.Count > 0)
                        {
                            CbGlobalSearchType.DataContext = new GlobalSearchModel();
                            CbGlobalSearchType.ItemsSource = m_globalSearchModel;


                            var value = ComponentFactory.GetComponent<IUserProfile>().Get<GlobalSearchModel>(UserProfileKey.Key_GlobalSearch);


                            if (value != null)
                            {
                                var selectedValue = m_globalSearchModel.FirstOrDefault(p => p.UrlTemplate == value.UrlTemplate);
                                if (selectedValue != null)
                                {
                                    CbGlobalSearchType.SelectedItem = selectedValue;
                                }
                                else
                                {
                                    CbGlobalSearchType.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                CbGlobalSearchType.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            this.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                this.Visibility = Visibility.Collapsed;
                ComponentFactory.Logger.LogError(ex);
            }
        }

        private string GetKeyDescription(string quickKeys)
        {
            var sb = new StringBuilder();

            var keys = quickKeys.Split(',', '，').ToList();

            foreach (var key in keys)
            {
                if (!key.IsNullOrEmpty())
                {
                    sb.Append(key.Trim().ToUpper());
                    sb.Append(",");
                }
            }

            return sb.ToString().IsNullOrEmpty() ? null : sb.ToString().Trim(','); ;
        }

        private void SetControlVisual(bool isMouseHover)
        {
            if (isMouseHover)
            {
                Grid_Input_MouseOverOrFocus.Visibility = Visibility.Visible;
                Grid_Input_Normal.Visibility = Visibility.Collapsed;

                ButtonGlobalSearch.Source = m_hover_image;
            }
            else
            {
                Grid_Input_MouseOverOrFocus.Visibility = Visibility.Collapsed;
                Grid_Input_Normal.Visibility = Visibility.Visible;
                ButtonGlobalSearch.Source = m_normal_image;
            }
        }

        private void DoSearch()
        {
            GlobalSearchModel globalSearch = null;

            var value = TextBoxGlobalSearchValue.Text.Trim();
            var reg = @"^{0}([\s\S]*)$";

            foreach (var item in (CbGlobalSearchType.ItemsSource as List<GlobalSearchModel>))
            {
                var quickkeys = GetKeyDescription(item.QuickKey);
                if (!quickkeys.IsNullOrEmpty())
                {
                    var keys = quickkeys.Split(',', '，');
                    var matched = false;

                    for (var i = 0; i < keys.Length; i++)
                    {
                        //如果输入的查询条件中存在空格，拆分查询条件
                        string[] keyValue;
                        if ((keyValue = value.Split(' ')).Length == 2)
                        {
                            if (string.Equals(keyValue[0], keys[i], StringComparison.OrdinalIgnoreCase))
                            {
                                value = keyValue[1];
                                globalSearch = item;
                                matched = true;

                                break;
                            }
                        }
                        else
                        {
                            var match = Regex.Match(value, string.Format(reg, keys[i]), RegexOptions.IgnoreCase);

                            if (match.Success)
                            {
                                value = match.Groups[1].Value;
                                globalSearch = item;
                                matched = true;

                                break;
                            }
                        }
                    }
                    if (matched)
                        break;
                }
            }


            if (globalSearch == null)
            {
                globalSearch = CbGlobalSearchType.SelectedItem as GlobalSearchModel;
            }

            if (globalSearch != null && !value.IsNullOrEmpty())
            {
                var url = string.Format(globalSearch.UrlTemplate, HttpUtility.UrlEncode(value.Trim()));

                if (CPApplication.Current != null)
                {
                    CPApplication.Current.Browser.Navigate(url);
                }
            }
            else
            {
                TextBoxGlobalSearchValue.Text = string.Empty;
                TextBoxGlobalSearchValue.Focus();
            }
            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Click", "QuickSearch");
        }

        void TextBoxGlobalSearchValue_LostFocus(object sender, RoutedEventArgs e)
        {
            SetControlVisual(false);
            m_isFocused = false;
        }

        void TextBoxGlobalSearchValue_GotFocus(object sender, RoutedEventArgs e)
        {
            SetControlVisual(true);
            m_isFocused = true;
        }

        void CbGlobalSearchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = CbGlobalSearchType.SelectedItem as GlobalSearchModel;

            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_GlobalSearch, value);

            TextBoxGlobalSearchValue.WaterMarkContent = value.Tip;
        }

        void TextBoxGlobalSearchValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoSearch();
            }
        }

        void ButtonGlobalSearch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoSearch();
        }

        void StackPanel_SearchArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!m_isFocused)
                SetControlVisual(false);
        }

        private void StackPanel_SearchArea_MouseEnter(object sender, MouseEventArgs e)
        {
            SetControlVisual(true);
        }
    }
}
