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
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Windows.Media.Imaging;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using ControlPanel.SilverlightUI;
using System.Windows.Data;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class UCQuickLinks : UserControl
    {
        public ObservableCollection<QuickLinksModel> ListItems { get; set; }
        public ObservableCollection<QuickLinksModel> MainListItems { get; set; }
        public ObservableCollection<QuickLinksModel> HidListItems { get; set; }

        private static string m_CloseHelpGrid = "CloseHelpGrid";
        private static string m_ErrorPage = "/PageBrowser/Error";
        private double m_preAutoListBoxWidth = 0;
        private Grid m_PopupCloseHelpGrid;
        private bool m_menuContextInvoke;

        public UCQuickLinks()
        {
            InitializeComponent();

            m_PopupCloseHelpGrid = new Grid
            {
                Name = m_CloseHelpGrid,
                Opacity = 0,
                Background = new SolidColorBrush(Color.FromArgb(32, 13, 33, 33))
            };

            m_PopupCloseHelpGrid.SetValue(Canvas.ZIndexProperty, 50);
            this.LSLayoutRoot.SetValue(Canvas.ZIndexProperty, 99999);

            this.RightButton.MouseLeftButtonDown += new MouseButtonEventHandler(RightButton_MouseLeftButtonDown);
            this.RightButton.MouseLeave += new MouseEventHandler(RightButton_MouseLeave);
            this.RightButton.MouseEnter += new MouseEventHandler(RightButton_MouseEnter);

            this.AutoListBox.SizeChanged += new SizeChangedEventHandler(AutoListBox_SizeChanged);
            this.ListBoxDrop.DragSourceDropped += new Controls.DropEventHandler(ListBoxDrop_DragSourceDropped);

            m_PopupCloseHelpGrid.MouseLeftButtonDown += new MouseButtonEventHandler(m_PopupCloseHelpGrid_MouseLeftButtonDown);
            RichContentPopup.Opened += new EventHandler(RichContentPopup_Opened);
            RichContentPopup.Closed += new EventHandler(RichContentPopup_Closed);
        }

        #region Event

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            this.RichContentPopup.IsOpen = !this.RichContentPopup.IsOpen;
        }

        void RightButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (RichContentPopup.IsOpen == false)
            {
                //RightButton.SetValue(ListBox.StyleProperty, this.Resources["BorderButtonDefaultStyle"]);

                RightButtonBorder.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        void RightButton_MouseEnter(object sender, MouseEventArgs e)
        {
            RightButtonBorder.Visibility = System.Windows.Visibility.Visible;
        }

        void RightButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RichContentPopup.IsOpen = !this.RichContentPopup.IsOpen;
        }

        void RichContentPopup_Opened(object sender, EventArgs e)
        {

            //RightButton.SetValue(ListBox.StyleProperty, this.Resources["BorderButtonStyle"]);

            if (!((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Contains(m_PopupCloseHelpGrid))
            {
                ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Add(m_PopupCloseHelpGrid);
            }

        }

        void RichContentPopup_Closed(object sender, EventArgs e)
        {
            RightButtonBorder.Visibility = System.Windows.Visibility.Collapsed;

            if (((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Contains(m_PopupCloseHelpGrid))
            {
                ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Remove(m_PopupCloseHelpGrid);
            }

        }

        void m_PopupCloseHelpGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RichContentPopup.IsOpen = false;
        }

        void MainListBox_Loaded(object sender, RoutedEventArgs e)
        {
            MainListBox = (ListBox)sender;
            LoadListBox();

            if (MainListBox != null)
            {
                //获得隐藏listbox的宽度给横向显示的
                MainListBox.Width = AutoListBox.ActualWidth;
            }
        }

        void MainListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnSizeChangeArrangeLine();
        }

        void AutoListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MainListBox != null)
            {
                MainListBox.Width = AutoListBox.ActualWidth;
            }
        }

        void ListBoxDrop_DragSourceDropped(object sender, Controls.DropEventArgs args)
        {
            var tabItem = ((args.DragSource.Content as FrameworkElement).FindName("HeaderTopSelected") as FrameworkElement).Tag as PageTab;

            if (tabItem != null)
            {
                //TODO: 这里有待改进.
                if (tabItem.View.Context.Request.UserState == null)
                {
                    int orderindex = GetItemCountByStatus("Y");

                    if (GetItemCountByStatus("") < 25)
                    {
                        Request request = tabItem.View.Context.Request;

                        string Urlkey = request.URL.Substring(request.URL.IndexOf("#") + 1);

                        //排除错误页面
                        if (Urlkey.ToUpper() != m_ErrorPage.ToUpper())
                        {
                            ListItems.Add(new QuickLinksModel()
                                               {
                                                   QuickLinkName = tabItem.Header.ToString(),
                                                   ViewStatus = "Y",
                                                   OrderIndex = orderindex,
                                                   GUID = Guid.NewGuid().ToString(),
                                                   BaseUrl = Urlkey,
                                                   IsVisibable = orderindex == 0 ? false : true,
                                                   UserState = null
                                               });
                        }

                    }

                    ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);

                    ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Add", "QuickLink");
                   
                    LoadListBox();
                    OnSizeChangeArrangeLine();
                }
            }
        }

        void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (QuickLinksModel)((FrameworkElement)sender).DataContext;

            var riverContextMenu = new QuickLinksRightMenu();
            riverContextMenu.DataContext = item;
            var menu = riverContextMenu.menu;

            menu.VerticalOffset = e.GetPosition(null).Y + 10;
            menu.HorizontalOffset = e.GetPosition(null).X + 10;
            menu.IsOpen = true;
            riverContextMenu.OnRightMenuDeleteClick -= new QuickLinksRightMenu.DeleteEventHandler(riverContextMenu_OnRightMenuDeleteClick);
            riverContextMenu.OnRightMenuRenameClick -= new QuickLinksRightMenu.ReNameEventHandler(riverContextMenu_OnRightMenuRenameClick);
            riverContextMenu.OnDeleteAllClick -= new QuickLinksRightMenu.DeleteAllEventHandler(riverContextMenu_OnDeleteAllClick);
            riverContextMenu.OnRightMenuDeleteClick += new QuickLinksRightMenu.DeleteEventHandler(riverContextMenu_OnRightMenuDeleteClick);
            riverContextMenu.OnRightMenuRenameClick += new QuickLinksRightMenu.ReNameEventHandler(riverContextMenu_OnRightMenuRenameClick);
            riverContextMenu.OnDeleteAllClick += new QuickLinksRightMenu.DeleteAllEventHandler(riverContextMenu_OnDeleteAllClick);
        }


        private void bt_QuickLink_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton linkBT = sender as HyperlinkButton;
            QuickLinksModel linkModel = linkBT.DataContext as QuickLinksModel;

            string menuURL = linkModel.BaseUrl.Trim();

            //
            if (menuURL.IndexOf("#") >= 0)
            {
                menuURL = menuURL.Substring(menuURL.IndexOf("#") + 1);
            }

            if (linkModel.UserState != null)
            {
                CPApplication.Current.Browser.Navigate(menuURL, linkModel.UserState);
            }
            else
            {
                CPApplication.Current.Browser.Navigate(menuURL);
            }

            ComponentFactory.GetComponent<IEventTracker>().TraceEvent(CPApplication.Current.DefaultPage, "Click", "QuickLink");

            if (RichContentPopup.IsOpen == true)
            {
                RichContentPopup.IsOpen = false;
            }
        }

        private void bt_QuickLink_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void bt_QuickLink_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            QuickLinksModel item = (QuickLinksModel)((FrameworkElement)sender).DataContext;

            QuickLinksRightMenu riverContextMenu = new QuickLinksRightMenu();
            riverContextMenu.DataContext = item;
            ContextMenu menu = riverContextMenu.menu;

            menu.VerticalOffset = e.GetPosition(null).Y + 10;
            menu.HorizontalOffset = e.GetPosition(null).X + 10;
            menu.IsOpen = true;

            riverContextMenu.OnRightMenuDeleteClick -= new QuickLinksRightMenu.DeleteEventHandler(riverContextMenu_OnRightMenuDeleteClick);
            riverContextMenu.OnRightMenuRenameClick -= new QuickLinksRightMenu.ReNameEventHandler(riverContextMenu_OnRightMenuRenameClick);
            riverContextMenu.OnDeleteAllClick -= new QuickLinksRightMenu.DeleteAllEventHandler(riverContextMenu_OnDeleteAllClick);
            riverContextMenu.OnRightMenuDeleteClick += new QuickLinksRightMenu.DeleteEventHandler(riverContextMenu_OnRightMenuDeleteClick);
            riverContextMenu.OnRightMenuRenameClick += new QuickLinksRightMenu.ReNameEventHandler(riverContextMenu_OnRightMenuRenameClick);
            riverContextMenu.OnDeleteAllClick += new QuickLinksRightMenu.DeleteAllEventHandler(riverContextMenu_OnDeleteAllClick);
        }

        void ListBoxDragDropTarget_ItemDragCompleted(object sender, ItemDragEventArgs e)
        {
            //当拖拽发生后，把横向quicklink中的所有的item的status都更新成"Y"
            for (int i = 0; i < MainListBox.Items.Count; i++)
            {

                (MainListBox.Items[i] as QuickLinksModel).ViewStatus = "Y";
                if (i == 0)
                {
                    (MainListBox.Items[i] as QuickLinksModel).IsVisibable = false;
                }
                else
                {
                    (MainListBox.Items[i] as QuickLinksModel).IsVisibable = true;
                }


            }
            //当拖拽发生后，把纵向quicklink中的所有的item的status都更新成"N"
            for (int i = 0; i < PopupListBox.Items.Count; i++)
            {
                (PopupListBox.Items[i] as QuickLinksModel).ViewStatus = "N";
            }

            ResetOrderIndexforLinks();

            ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);
            
            LoadListBox();

            OnSizeChangeArrangeLine();
            
            //如果下拉列表是显示的，在拖拽完成后则恢复用于其点击消失的隐藏层。
            if (RichContentPopup.IsOpen == true)
            {
                if (!((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Contains(m_PopupCloseHelpGrid))
                {
                    ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Add(m_PopupCloseHelpGrid);
                }


            }

        }

        void ListBoxDragDropTarget_ItemDragStarting(object sender, ItemDragEventArgs e)
        {
            //如果下拉列表时显示的，则在开始拖拽的时候暂时移除掉,用于鼠标点击任意位置使其消失的隐藏层。
            if (RichContentPopup.IsOpen == true)
            {
                if (((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Contains(m_PopupCloseHelpGrid))
                {
                    ((Panel)((UserControl)Application.Current.RootVisual).Content).Children.Remove(m_PopupCloseHelpGrid);
                }
            }

        }

        void riverContextMenu_OnRightMenuDeleteClick(object sender)
        {
            this.m_menuContextInvoke = true;
            CPApplication.Current.CurrentPage.Context.Window.Confirm(null, PageResource.LblDeleteQuickLinksConfirm, (elem, result) =>
            {
                if (result.DialogResult == DialogResultType.OK)
                {
                    QuickLinksModel LinkModel = sender as QuickLinksModel;
                    int DeleteIndex = LinkModel.OrderIndex;

                    string ViewStatus = LinkModel.ViewStatus;

                    ListItems.Remove(LinkModel);

                    int ItemCount = ListItems.Count;

                    for (int j = ItemCount - 1; j >= 0; j--)
                    {
                        var _LinkModel = ListItems[j] as QuickLinksModel;

                        if (_LinkModel.OrderIndex > DeleteIndex && _LinkModel.ViewStatus == ViewStatus)
                        {
                            _LinkModel.OrderIndex--;
                        }
                        //如果删除的时第一横向显示的quicklink的话，则把横向的最新的第一quicklink的SplitBarVisibable改为不可见。
                        if (_LinkModel.OrderIndex == 0 && _LinkModel.ViewStatus == "Y")
                        {
                            _LinkModel.IsVisibable = false;
                        }

                    }

                    ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);
                    
                    LoadListBox();

                    OnSizeDelArrangeLine();
                }
            });


        }

        void riverContextMenu_OnRightMenuRenameClick(object sender)
        {
            QuickLinksModel LinkModel = sender as QuickLinksModel;

            QuickLinksReName QuickLinksRenameElement = new QuickLinksReName(LinkModel);
            var diglog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog(PageResource.LbRenameTitle, QuickLinksRenameElement);
            QuickLinksRenameElement.diglog = diglog;
            diglog.Closed += new EventHandler(diglog_Closed);
        }

        void riverContextMenu_OnDeleteAllClick(object sender)
        {
            CPApplication.Current.CurrentPage.Context.Window.Confirm(null, PageResource.LblDeleteAllQuickLinksConfirm, (elem, result) =>
            {
                if (result.DialogResult == DialogResultType.OK)
                {
                    ListItems = new ObservableCollection<QuickLinksModel>();

                    ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);
                    
                    LoadListBox();
                }
            });
        }

        void diglog_Closed(object sender, EventArgs e)
        {
            IDialog diglog = sender as IDialog;
            if (diglog.ResultArgs.DialogResult == DialogResultType.OK)
            {
                ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);
               
                LoadListBox();

                OnSizeChangeArrangeLine();
            }

        }

        #endregion

        #region Methods

        /// <summary>
        /// 加载Listbox
        /// </summary>
        private void LoadListBox()
        {
            if (MainListBox == null)
                MainListBox = new ListBox();

            if (ListItems == null)
                ListItems = new ObservableCollection<QuickLinksModel>();

            var QuickLinks = ComponentFactory.GetComponent<IUserProfile>().Get<ObservableCollection<QuickLinksModel>>(UserProfileKey.Key_QuickLink);
            
            if (QuickLinks != null)
                ListItems = QuickLinks;

            MainListItems = new ObservableCollection<QuickLinksModel>((from p in ListItems where p.ViewStatus == "Y" orderby p.OrderIndex ascending select p).ToList());

            HidListItems = new ObservableCollection<QuickLinksModel>((from p in ListItems where p.ViewStatus == "N" orderby p.OrderIndex ascending select p).ToList());

            for (var i = 0; i < this.MainListItems.Count; i++)
            {
                if (i == 0)
                {
                    this.MainListItems[i].IsVisibable = false;
                }
                else
                {
                    this.MainListItems[i].IsVisibable = true;
                }
            }
            
            MainListBox.ItemsSource = MainListItems;


            if (MainListBox.Items.Count != 0)
            {
                ImageHelpICON.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ImageHelpICON.Visibility = System.Windows.Visibility.Visible;
            }
            

            if (HidListItems != null && HidListItems.Count == 0)
            {
                this.RichContentPopup.IsOpen = false;
            }

            PopupListBox.ItemsSource = HidListItems;


            if (PopupListBox != null)
            {
                if (PopupListBox.Items.Count > 0)
                {
                    RightButton.Visibility = System.Windows.Visibility.Visible;
                    this.PopupListBox.SelectedIndex = -1;
                }
                else
                {
                    RightButton.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                RightButton.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        /// <summary>
        /// 更新排列序号
        /// </summary>
        private void ResetOrderIndexforLinks()
        {

            for (int i = 0; i < MainListBox.Items.Count; i++)
            {
                foreach (QuickLinksModel linksModel in ListItems)
                {

                    string strGUID = (MainListBox.Items[i] as QuickLinksModel).GUID;
                    string ViewStatus = (MainListBox.Items[i] as QuickLinksModel).ViewStatus;
                    if (linksModel.GUID == strGUID)
                    {
                        linksModel.OrderIndex = i;
                        linksModel.ViewStatus = ViewStatus;
                        linksModel.IsVisibable = true;
                        if (i == 0)
                        {
                            linksModel.IsVisibable = false;

                        }
                    }

                }
            }

            for (int i = 0; i < PopupListBox.Items.Count; i++)
            {
                foreach (QuickLinksModel linksModel in ListItems)
                {

                    string strGUID = (PopupListBox.Items[i] as QuickLinksModel).GUID;
                    string ViewStatus = (PopupListBox.Items[i] as QuickLinksModel).ViewStatus;
                    if (linksModel.GUID == strGUID)
                    {
                        linksModel.OrderIndex = i;
                        linksModel.ViewStatus = ViewStatus;

                    }

                }
            }



        }

        /// <summary>
        /// 当从新排列Item
        /// </summary>
        private void OnSizeChangeArrangeLine()
        {
            if (MainListBox != null)
            {
                //当前显示的所有的元素的长度
                double LineWidth = 0;
                //记录原始的容器长度
                if (m_preAutoListBoxWidth == 0)
                {
                    m_preAutoListBoxWidth = MainListBox.ActualWidth;
                }
                //当前整个容器的长度
                double maximumWith = MainListBox.ActualWidth;

                LineWidth = GetAllMainItemLength();

                //获得所有item的数量
                int ItemCount = GetItemCountByStatus(string.Empty);

                //如果当前所有Item的长度大于了容器的总长度则从最后一个item 开始隐藏
                if (ItemCount > 1 && LineWidth > maximumWith)
                {
                    OnSizeAddArrangeLine();
                }
                else
                {
                    OnSizeDelArrangeLine();

                }
                m_preAutoListBoxWidth = MainListBox.ActualWidth;
            }
        }

        private void OnSizeAddArrangeLine()
        {
            double LineWidth = 0;
            LineWidth = GetAllMainItemLength();

            //当前整个容器的长度
            double maximumWith = MainListBox.ActualWidth;

            int ItemCount = GetItemCountByStatus(string.Empty);

            //获取状态为y的最后一个items的索引
            int LastMainOrderIndex = GetItemCountByStatus("Y");

            for (int i = ItemCount - 1; i >= 0; i--)
            {
                var LinkModel = ListItems[i] as QuickLinksModel;
                //当隐藏隐藏item的时候需要找到主列表中最后一个为Y的item进行更改
                if (LinkModel.ViewStatus == "Y" && LinkModel.OrderIndex == LastMainOrderIndex - 1)
                {
                    AddOrderIndex();

                    double itemWidth = CalculateItemWith(LinkModel.QuickLinkName);

                    LinkModel.ViewStatus = "N";
                    LinkModel.OrderIndex = 0;
                    LinkModel.IsVisibable = true;

                    LineWidth -= itemWidth;

                    ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);
                    
                    LoadListBox();

                    if (LineWidth <= maximumWith)
                    {
                        return;
                    }

                    OnSizeAddArrangeLine();
                }
            }

        }

        /// <summary>
        /// 当删除QuickLink时，从新排列item
        /// </summary>
        private void OnSizeDelArrangeLine()
        {
            double LineWidth = 0;
            LineWidth = GetAllMainItemLength();

            double maximumWith = MainListBox.ActualWidth;

            int ItemCount = GetItemCountByStatus(string.Empty);

            int orderIndex = 0;
            for (int i = 0; i < ItemCount; i++)
            {
                var LinkModel = ListItems[i] as QuickLinksModel;

                if (LinkModel.ViewStatus == "N" && LinkModel.OrderIndex == orderIndex)
                {
                    double itemWidth = CalculateItemWith(LinkModel.QuickLinkName);

                    if (LineWidth + itemWidth >= maximumWith)
                    {
                        break;
                    }
                    else
                    {
                        int LastMainOrderIndex = GetItemCountByStatus("Y");
                        // orderIndex++;
                        LinkModel.ViewStatus = "Y";
                        LinkModel.OrderIndex = LastMainOrderIndex;
                        LineWidth += itemWidth;

                        minusOrderIndex();

                        ComponentFactory.GetComponent<IUserProfile>().Set(UserProfileKey.Key_QuickLink, ListItems, true);
                        
                        LoadListBox();

                        OnSizeDelArrangeLine();
                    }
                }
            }
        }

        /// <summary>
        /// 获得当前横向显示的所有的quicklink的长度。
        /// </summary>
        /// <returns></returns>
        private double GetAllMainItemLength()
        {
            double LineWidth = 0;
            if (MainListBox != null)
            {
                foreach (QuickLinksModel LinkModel in MainListBox.Items)
                {
                    HyperlinkButton linkbt = new HyperlinkButton()
                    {
                        Content = LinkModel.QuickLinkName
                    };

                    this.LSLayoutRoot.Children.Add(linkbt);
                    linkbt.Measure(new Size(999, 999));
                    double itemWidth = 0d;
                    //if (linkbt.DesiredSize.Width > 135)
                    //{
                    //    itemWidth = 135;
                    //}
                    //else
                    //{
                    //    itemWidth = linkbt.DesiredSize.Width + 9;
                    //}

                    itemWidth = linkbt.DesiredSize.Width + 16;

                    LineWidth += itemWidth;

                    this.LSLayoutRoot.Children.Remove(linkbt);
                }
            }

            return LineWidth;

        }

        /// <summary>
        /// 根据item名称计算link所需要的with
        /// </summary>
        /// <param name="ItemName"></param>
        /// <returns></returns>
        private double CalculateItemWith(string ItemName)
        {
            double itemWidth = 0d;
            HyperlinkButton linkbt = new HyperlinkButton()
            {
                Content = ItemName,
                Opacity = 0d
            };

            this.LSLayoutRoot.Children.Add(linkbt);
            linkbt.Measure(new Size(999, 999));
            if (linkbt.DesiredSize.Width > 135)
            {
                itemWidth = 135;
            }
            else
            {
                itemWidth = linkbt.DesiredSize.Width + 9;
            }

            this.LSLayoutRoot.Children.Remove(linkbt);

            return itemWidth;
        }

        /// <summary>
        ///根据ViewStatus获得item的数量
        /// </summary>
        /// <param name="viewStatus"></param>
        /// <returns></returns>
        private int GetItemCountByStatus(string viewStatus)
        {
            int itemsCount = 0;
            if (string.IsNullOrEmpty(viewStatus))
            {
                itemsCount = (from p in ListItems select p).Count();
            }
            else
            {
                itemsCount = (from p in ListItems where p.ViewStatus == viewStatus select p).Count();
            }
            return itemsCount;
        }

        /// <summary>
        /// item的orderIndex依次加1
        /// </summary>
        private void AddOrderIndex()
        {
            int ItemCount = ListItems.Count;

            for (int j = ItemCount - 1; j >= 0; j--)
            {
                var _LinkModel = ListItems[j] as QuickLinksModel;
                if (_LinkModel.ViewStatus == "N")
                {
                    _LinkModel.OrderIndex++;
                }
            }
        }

        /// <summary>
        /// item的orderIndex依次减1
        /// </summary>
        private void minusOrderIndex()
        {
            int ItemCount = ListItems.Count;

            for (int j = ItemCount - 1; j >= 0; j--)
            {
                var _LinkModel = ListItems[j] as QuickLinksModel;

                if (_LinkModel.ViewStatus == "N")
                {
                    _LinkModel.OrderIndex--;
                }
            }
        }

        #endregion

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.PopupListBox.SelectedIndex = -1;
            if (!this.m_menuContextInvoke)
            {
                var linkModel = (sender as Grid).DataContext as QuickLinksModel;
                if (linkModel != null)
                {
                    var menuURL = linkModel.BaseUrl.Trim();

                    if (menuURL.IndexOf("#") >= 0)
                    {
                        menuURL = menuURL.Substring(menuURL.IndexOf("#") + 1);
                    }

                    if (linkModel.UserState != null)
                    {
                        CPApplication.Current.Browser.Navigate(menuURL, linkModel.UserState);
                    }
                    else
                    {
                        CPApplication.Current.Browser.Navigate(menuURL);
                    }

                    if (RichContentPopup.IsOpen == true)
                    {
                        RichContentPopup.IsOpen = false;
                    }
                }
            }
            else
            {
                this.m_menuContextInvoke = false;
            }
        }
    }
}
