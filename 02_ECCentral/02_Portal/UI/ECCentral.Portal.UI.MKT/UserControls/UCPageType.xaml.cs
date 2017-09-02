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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public enum RenderMode
    {
        /// <summary>
        /// 单行
        /// </summary>
        SingleLine,
        /// <summary>
        /// 多行
        /// </summary>
        MultiLine
    }

    public enum BizMode
    {
        Query,
        Maintain
    }

    public partial class UCPageType : UserControl
    {
        public UCPageType()
        {
            InitializeComponent();
        }
        private bool flag = true;
        private void lstPageType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PageType.HasValue)
            {
                flag = true;
                ReloadPages(this.PageType.Value.ToString());
            }
            else
            {
                if (flag)
                {
                    flag = false;
                    ReloadPages("-1");
                }
            }


            if (PositionCombox != null)
            {
                if (ModuleType == BizEntity.MKT.ModuleType.ProductRecommend)
                {
                    //加载商品推荐的位置信息
                    ReloadProductRecommendPosition(this.PageType);
                }
                else if (ModuleType == BizEntity.MKT.ModuleType.Banner)
                {
                    //加载广告位置信息
                    ReloadBannerPosition(this.PageType);
                }
                else if (ModuleType == BizEntity.MKT.ModuleType.HotSale)
                {
                    //加载首页热销排行的位置信息
                    ReloadHotSaleCategoryPosition(this.PageType);
                }
            }
            RaisePageTypeSelectionChanged();
        }


        /// <summary>
        /// 一个页面会有多个位置信息，如何需要根据选择的页面动态加载页面的位置信息，则需要设置一个用来显示的Combox
        /// </summary>
        public Combox PositionCombox
        {
            get { return (Combox)GetValue(PositionComboxProperty); }
            set { SetValue(PositionComboxProperty, value); }
        }
        public static readonly DependencyProperty PositionComboxProperty =
            DependencyProperty.Register("PositionCombox", typeof(Combox), typeof(UCPageType), new PropertyMetadata(null));


        /// <summary>
        /// ModuleType,比如ProductRecommend,Banner等
        /// </summary>
        public ModuleType ModuleType
        {
            get { return (ModuleType)GetValue(ModuleTypeProperty); }
            set { SetValue(ModuleTypeProperty, value); }
        }
        public static readonly DependencyProperty ModuleTypeProperty =
            DependencyProperty.Register("ModuleTypeProperty", typeof(ModuleType), typeof(UCPageType), new PropertyMetadata(ModuleType.Banner, (d, e) =>
                {
                    UCPageType ucPagePosition = d as UCPageType;
                    ucPagePosition.ReloadPageTypes();
                }));


        /// <summary>
        /// 扩展生效CheckBox是否可见
        /// </summary>
        public Visibility ExtendC3Visibility
        {
            get { return (Visibility)GetValue(ExtendC3VisibilityProperty); }
            set { SetValue(ExtendC3VisibilityProperty, value); }
        }

        public static readonly DependencyProperty ExtendC3VisibilityProperty =
            DependencyProperty.Register("ExtendC3Visibility", typeof(Visibility), typeof(UCPageType), new PropertyMetadata(Visibility.Collapsed, (d, e) =>
            {

            }));

        /// <summary>
        /// 设置默认类别CheckBox是否可见
        /// </summary>
        public Visibility SetDefaultCategoryVisibility
        {
            get { return (Visibility)GetValue(SetDefaultCategoryVisibilityProperty); }
            set { SetValue(SetDefaultCategoryVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SetDefaultCategoryVisibilityProperty =
            DependencyProperty.Register("SetDefaultCategoryVisibility", typeof(Visibility), typeof(UCPageType), new PropertyMetadata(Visibility.Visible, (d, e) =>
            {
            }));

        /// <summary>
        /// 控件呈现方式，单行，多行
        /// </summary>
        public RenderMode RenderMode
        {
            get { return (RenderMode)GetValue(RenderModeProperty); }
            set { SetValue(RenderModeProperty, value); }
        }

        public static readonly DependencyProperty RenderModeProperty =
            DependencyProperty.Register("RenderMode", typeof(RenderMode), typeof(UCPageType), new PropertyMetadata(RenderMode.SingleLine, (d, e) =>
                {
                    UCPageType ucPagePosition = d as UCPageType;
                    ucPagePosition.ChangeRenderMode();
                }));

        /// <summary>
        ///业务模式，查询，维护 
        /// </summary>
        private BizMode _bizMode;
        public BizMode BizMode
        {
            get
            {
                return _bizMode;
            }
            set
            {
                _bizMode = value;
            }
        }

        public void ChangeRenderMode()
        {
            if (this.RenderMode == RenderMode.SingleLine)
            {
                Grid.SetRow(this.lstPage, 0);
                Grid.SetColumn(this.lstPage, 1);
                Grid.SetRow(this.ucCategoryPicker, 0);
                Grid.SetColumn(this.ucCategoryPicker, 1);
                Grid.SetRow(this.cbSetDefault, 0);
                Grid.SetColumn(this.cbSetDefault, 2);
                Grid.SetRow(this.cbExtendC3, 0);
                Grid.SetColumn(this.cbExtendC3, 3);
            }
            else if (this.RenderMode == RenderMode.MultiLine)
            {
                Grid.SetRow(this.cbSetDefault, 0);
                Grid.SetColumn(this.cbSetDefault, 1);
                Grid.SetRow(this.cbExtendC3, 0);
                Grid.SetColumn(this.cbExtendC3, 2);

                Grid.SetRow(this.lstPage, 1);
                Grid.SetColumn(this.lstPage, 0);
                Grid.SetColumnSpan(this.lstPage, 4);
                Grid.SetRow(this.ucCategoryPicker, 1);
                Grid.SetColumn(this.ucCategoryPicker, 0);
                Grid.SetColumnSpan(this.ucCategoryPicker, 4);
            }
            this.UpdateLayout();
        }

        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return (string)GetValue(ChannelIDProperty); }
            set { SetValue(ChannelIDProperty, value); }
        }
        public static readonly DependencyProperty ChannelIDProperty =
            DependencyProperty.Register("ChannelID", typeof(string), typeof(UCPageType), new PropertyMetadata(null, (d, e) =>
                {
                    if (e.NewValue != e.OldValue)
                    {
                        UCPageType ucPagePosition = d as UCPageType;
                        ucPagePosition.ReloadPageTypes();
                    }
                }));

        public PageTypePresentationType PagePresentationType
        {
            get;
            private set;
        }

        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType
        {
            get
            {
                int pageType;
                if (this.lstPageType.SelectedValue != null
                    && int.TryParse(this.lstPageType.SelectedValue.ToString(), out pageType))
                {
                    return pageType;
                }
                else
                {
                    return null;
                }
            }
        }
        #region "由于所有NoneSubPages类型的PageType的PageID都为null，在维护广告页面时需要对这些页面的PageID赋不同的值，所以需公开PapeType实体以便于区分"

        public CodeNamePair PageTypeInfo
        {
            get { return (CodeNamePair)lstPageType.SelectedItem; }
        }

        #endregion
        public void SetPageType(int? pageType)
        {
            this.lstPageType.SelectedValue = pageType;
        }

        public void SetFirstPageTypeSelected()
        {
            this.lstPageType.SelectedIndex = 0;
        }

        /// <summary>
        /// 具体页面编号
        /// </summary>
        public int? PageID
        {
            get
            {
                if (this.cbSetDefault.IsChecked == true && this.cbSetDefault.Visibility == System.Windows.Visibility.Visible)
                {
                    //如果选中了设置默认类别，则返回固定值
                    return -1;
                }

                if (this.lstPage.Visibility == System.Windows.Visibility.Visible)
                {
                    int pageID;
                    if (this.lstPage.SelectedValue != null
                    && int.TryParse(this.lstPage.SelectedValue.ToString(), out pageID))
                    {
                        return pageID;
                    }
                }
                else if (ucCategoryPicker.Visibility == System.Windows.Visibility.Visible)
                {
                    int? cSysNo = null;
                    if (ucCategoryPicker.ShowLevel == ECCategoryLevel.Category1)
                    {
                        cSysNo = ucCategoryPicker.SelectedCategory1SysNo;
                    }
                    else if (ucCategoryPicker.ShowLevel == ECCategoryLevel.Category2)
                    {
                        cSysNo = ucCategoryPicker.SelectedCategory2SysNo;
                    }
                    else
                    {
                        cSysNo = ucCategoryPicker.SelectedCategory3SysNo;
                    }
                    if (BizMode == UserControls.BizMode.Maintain && cSysNo == null)
                    {
                        switch (ModuleType)
                        {
                            case BizEntity.MKT.ModuleType.Banner:
                            case BizEntity.MKT.ModuleType.NewsAndBulletin:
                            case BizEntity.MKT.ModuleType.SEO:
                                //在创建，编辑模式下，如果未选择分类则表示对所有分类有效，置为特殊值-1
                                return -1;
                            default:
                                return cSysNo;
                        }
                    }
                    else
                    {
                        return cSysNo;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 三级分类是否扩展生效
        /// </summary>
        public bool IsExtendValid
        {
            get
            {
                return this.cbExtendC3.IsChecked.HasValue ? this.cbExtendC3.IsChecked.Value : false;
            }
        }

        /// <summary>
        /// 是否设置默认类别
        /// </summary>
        public bool IsSetDefaultCategory
        {
            get
            {
                return this.cbSetDefault.IsChecked.HasValue ? this.cbSetDefault.IsChecked.Value : false;
            }
        }

        public void SetPageID(int? pageID)
        {
            switch (this.PagePresentationType)
            {
                case PageTypePresentationType.NoneSubPages:
                    break;
                case PageTypePresentationType.Category1:
                    this.ucCategoryPicker.SetSelectedCategory1SysNo(pageID);
                    ClickDefaultCategory(pageID??0);
                    break;
                case PageTypePresentationType.Category2:
                    this.ucCategoryPicker.SetSelectedCategory2SysNo(pageID);
                    ClickDefaultCategory(pageID ?? 0);
                    break;
                case PageTypePresentationType.Category3:
                    this.ucCategoryPicker.SetSelectedCategory3SysNo(pageID);
                    ClickDefaultCategory(pageID ?? 0);
                    break;
                default:
                    this.lstPage.SelectedValue = pageID;
                    break;
            }
        }

        /// <summary>
        /// 根据渠道，模块类型等加载前台网站的页面类型
        /// </summary>
        public void ReloadPageTypes()
        {
            if (CPApplication.Current.CurrentPage == null)
                return;
            if (string.IsNullOrEmpty(ChannelID))
            {
                BindPageType(null);
                return;
            }
            PageTypeFacade facade = new PageTypeFacade(CPApplication.Current.CurrentPage);
            facade.GetPageTypes(CPApplication.Current.CompanyCode, this.ChannelID, (int)this.ModuleType, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    BindPageType(args.Result);

                    RaisePageTypeLoadCompleted();
                });
        }

        private void BindPageType(List<CodeNamePair> items)
        {
            if (items == null)
            {
                items = new List<CodeNamePair>();
            }

            if (BizMode == UserControls.BizMode.Query)
            {
                //在查询模式加一个所有选项
                items.Insert(0, new CodeNamePair { Name = ResCommonEnum.Enum_All });
            }
            else
            {
                items.Insert(0, new CodeNamePair { Name = ResCommonEnum.Enum_Select });
            }
            this.lstPageType.ItemsSource = items;
        }

        /// <summary>
        /// 加载页面类型对应的页面
        /// </summary>
        public void ReloadPages(string pageTypeID)
        {
            //先隐藏所有的页面相关的控件
            this.ucCategoryPicker.Visibility = Visibility.Collapsed;
            this.cbSetDefault.Visibility = System.Windows.Visibility.Collapsed;
            this.cbExtendC3.Visibility = System.Windows.Visibility.Collapsed;
            this.lstPage.Visibility = Visibility.Collapsed;

            //根据选定的页面类型加载页面列表
            PageTypeFacade facade = new PageTypeFacade(CPApplication.Current.CurrentPage);
            facade.GetPages(CPApplication.Current.CompanyCode, this.ChannelID == null ? "1" : this.ChannelID, (int)this.ModuleType, pageTypeID, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                //根据PresentationType动态控件显示哪些控件
                this.PagePresentationType = args.Result.PresentationType;
                switch (args.Result.PresentationType)
                {
                    case PageTypePresentationType.NoneSubPages:
                        this.ucCategoryPicker.LoadAllECCategory(this.ChannelID, null);
                        RaisePageLoadCompleted();
                        break;
                    case PageTypePresentationType.Category1:
                        SetDefaultCategory();
                        this.ucCategoryPicker.BizMode = BizMode;
                        this.ucCategoryPicker.Visibility = System.Windows.Visibility.Visible;
                        this.ucCategoryPicker.ShowLevel = ECCategoryLevel.Category1;
                        this.ucCategoryPicker.LoadAllECCategory(this.ChannelID, null);
                        RaisePageLoadCompleted();
                        break;
                    case PageTypePresentationType.Category2:
                        SetDefaultCategory();
                        this.ucCategoryPicker.BizMode = BizMode;
                        this.ucCategoryPicker.Visibility = System.Windows.Visibility.Visible;
                        this.ucCategoryPicker.ShowLevel = ECCategoryLevel.Category2;
                        this.ucCategoryPicker.LoadAllECCategory(this.ChannelID, null);

                        RaisePageLoadCompleted();
                        break;
                    case PageTypePresentationType.Category3:
                        SetDefaultCategory();
                        if (BizMode == UserControls.BizMode.Maintain)
                        {
                            this.cbExtendC3.Visibility = this.ExtendC3Visibility;
                        }
                        this.ucCategoryPicker.BizMode = BizMode;
                        this.ucCategoryPicker.Visibility = System.Windows.Visibility.Visible;
                        this.ucCategoryPicker.ShowLevel = ECCategoryLevel.Category3;
                        this.ucCategoryPicker.LoadAllECCategory(this.ChannelID, null);

                        RaisePageLoadCompleted();
                        break;

                    default:

                        if (args.Result.PresentationType == PageTypePresentationType.Stores ||
                            args.Result.PresentationType == PageTypePresentationType.Brand ||
                            args.Result.PresentationType == PageTypePresentationType.OtherSales)
                        {
                            SetDefaultCategory();
                            if (PageID.HasValue) ClickDefaultCategory(PageID.Value);
                        }

                        if (args.Result.PageList == null)
                        {
                            args.Result.PageList = new List<WebPage>();
                        }
                        //在查询模式加一个所有选项
                        if (BizMode == UserControls.BizMode.Query)
                        {
                            args.Result.PageList.Insert(0, new WebPage { ID = null, PageName = ResCommonEnum.Enum_All });
                        }
                        else
                        {
                            args.Result.PageList.Insert(0, new WebPage { ID = null, PageName = ResCommonEnum.Enum_Select });
                        }
                        this.lstPage.ItemsSource = args.Result.PageList;
                        this.lstPage.Visibility = Visibility.Visible;

                        RaisePageLoadCompleted();

                        if (this.lstPage.Items.Count > 0 && this.lstPage.SelectedValue == null)
                        {
                            this.lstPage.SelectedIndex = 0;
                        }

                        break;
                }
            });
        }

        private void SetDefaultCategory()
        {
            if (BizMode == UserControls.BizMode.Maintain)
            {
                switch (ModuleType)
                {
                    case BizEntity.MKT.ModuleType.Banner:
                    case BizEntity.MKT.ModuleType.NewsAndBulletin:
                    case BizEntity.MKT.ModuleType.SEO:
                        this.cbSetDefault.IsChecked = false;
                        this.cbSetDefault.Visibility = this.SetDefaultCategoryVisibility;
                        break;
                }
            }
        }

        /// <summary>
        /// 加载商品推荐的位置信息
        /// </summary>
        /// <param name="selectedPageType">选中的页面类型ID</param>
        public void ReloadProductRecommendPosition(int? selectedPageType)
        {
            if (selectedPageType.HasValue)
            {
                new ProductRecommendFacade(CPApplication.Current.CurrentPage)
                .GetPosition(selectedPageType.Value, (result) =>
                {
                    BindProductRecommendPosition(result);
                    RaisePagePositionLoadCompleted();
                });
            }
            else
            {
                //没有选定页面类型，则只显示所有选项
                BindProductRecommendPosition(null);
            }
        }

        private void BindProductRecommendPosition(List<CodeNamePair> result)
        {
            object selectValue = null;
            if (PositionCombox.SelectedValue != null)
            {
                selectValue = PositionCombox.SelectedValue;
            }
            if (PositionCombox != null)
            {
                PositionCombox.ItemsSource = AppendAllForCodeNamePairListIfNeeded(result);
                PositionCombox.SelectedValue = selectValue;
            }

        }

        //为商品推荐的位置列表增加所有选项
        private List<CodeNamePair> AppendAllForCodeNamePairListIfNeeded(List<CodeNamePair> result)
        {
            result = result ?? new List<CodeNamePair>();
            if (BizMode == UserControls.BizMode.Query)
            {
                result.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
            }
            else
            {
                result.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_Select });
            }
            return result;
        }

        /// <summary>
        /// 加载广告位置信息
        /// </summary>
        /// <param name="selectedPageType">选中的页面类型ID</param>
        public void ReloadBannerPosition(int? selectedPageType)
        {
            PositionCombox.ItemsSource = null;
            if (selectedPageType.HasValue)
            {
                new BannerFacade(CPApplication.Current.CurrentPage)
                .QueryDimensions(CPApplication.Current.CompanyCode, ChannelID, selectedPageType.Value, (s, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        var result = args.Result.Convert<BannerDimension, BannerDimensionVM>();
                        BindBannerPosition(result);
                        RaisePagePositionLoadCompleted();
                    });
            }
            else
            {
                //没有选定页面类型，则只显示所有或请选择选项
                BindBannerPosition(null);
            }
        }

        private void BindBannerPosition(List<BannerDimensionVM> result)
        {
            if (PositionCombox != null)
            {
                PositionCombox.ItemsSource = AppendAllForBannerPositionIfNeeded(result);
            }
        }

        //为广告位置列表增加所有选项
        private List<BannerDimensionVM> AppendAllForBannerPositionIfNeeded(List<BannerDimensionVM> result)
        {
            result = result ?? new List<BannerDimensionVM>();
            if (BizMode == UserControls.BizMode.Query)
            {
                result.Insert(0, new BannerDimensionVM { PositionID = null, PositionName = ResCommonEnum.Enum_All });
            }
            else
            {
                result.Insert(0, new BannerDimensionVM { PositionID = null, PositionName = ResCommonEnum.Enum_Select });
            }

            return result;
        }

        /// <summary>
        /// 获取首页排行的位置
        /// </summary>
        /// <param name="selectedPageType">选中的页面类型ID</param>
        public void ReloadHotSaleCategoryPosition(int? selectedPageType)
        {
            PositionCombox.ItemsSource = null;
            if (selectedPageType.HasValue)
            {
                new HotSaleCategoryFacade(CPApplication.Current.CurrentPage)
                .GetPositionList(CPApplication.Current.CompanyCode, ChannelID, selectedPageType.Value, (result) =>
                {
                    PositionCombox.ItemsSource = AppendAllForCodeNamePairListIfNeeded(result);
                    RaisePagePositionLoadCompleted();
                });
            }
            else
            {
                PositionCombox.ItemsSource = AppendAllForCodeNamePairListIfNeeded(null);
            }
        }

        private void lstPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModuleType == ModuleType.ProductRecommend)
            {
                if (PagePresentationType == PageTypePresentationType.Stores || PagePresentationType == PageTypePresentationType.Brand)
                {
                    if (this.PageType.HasValue && this.PageID.HasValue)
                    {
                        //如果是专卖店就加载其它位置列表
                        new ProductRecommendFacade(CPApplication.Current.CurrentPage)
                        .GetBrandPosition(this.PageID.Value, CPApplication.Current.CompanyCode, ChannelID, (result) =>
                            {
                                BindProductRecommendPosition(result);
                                RaisePagePositionLoadCompleted();
                            });
                    }
                    else
                    {
                        BindProductRecommendPosition(null);
                    }
                }
            }

            RaisePageSelectionChanged();
        }

        private void ucCategoryPicker_C1SelectionChanged(object sender, C1SelectitonChangedEventArgs e)
        {
            RaisePageSelectionChanged();
        }

        private void ucCategoryPicker_C2SelectionChanged(object sender, C2SelectitonChangedEventArgs e)
        {
            RaisePageSelectionChanged();
        }

        private void ucCategoryPicker_C3SelectionChanged(object sender, C3SelectitonChangedEventArgs e)
        {
            RaisePageSelectionChanged();
        }

        private void ucCategoryPicker_CategoryLoadCompleted(object sender, EventArgs e)
        {
            RaisePageLoadCompleted();
        }

        #region 自定义事件

        /// <summary>
        /// 页面位置列表加载完成事件
        /// </summary>
        public event EventHandler PagePositionLoadCompleted;
        private void RaisePagePositionLoadCompleted()
        {
            var loadCompleted = PagePositionLoadCompleted;
            if (loadCompleted != null)
            {
                loadCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 页面类型列表加载完成事件
        /// </summary>
        public event EventHandler PageTypeLoadCompleted;
        private void RaisePageTypeLoadCompleted()
        {
            var loadCompleted = PageTypeLoadCompleted;
            if (loadCompleted != null)
            {
                loadCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 页面列表加载完成事件
        /// </summary>
        public event EventHandler PageLoadCompleted;
        private void RaisePageLoadCompleted()
        {
            var loadCompleted = PageLoadCompleted;
            if (loadCompleted != null)
            {
                loadCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 页面类型改变事件
        /// </summary>
        public event EventHandler<PageTypeSelectionChangedEventArgs> PageTypeSelectionChanged;
        private void RaisePageTypeSelectionChanged()
        {
            var pageChangedEvent = PageTypeSelectionChanged;
            if (pageChangedEvent != null)
            {
                var args = new PageTypeSelectionChangedEventArgs(this.PageType);
                pageChangedEvent(this, args);
            }
        }

        /// <summary>
        /// 具体页面发生改变事件
        /// </summary>
        public event EventHandler<PageSelectionChangedEventArgs> PageSelectionChanged;
        private void RaisePageSelectionChanged()
        {
            var pageChangedEvent = PageSelectionChanged;
            if (pageChangedEvent != null)
            {
                var args = new PageSelectionChangedEventArgs(this.PagePresentationType, this.PageType, this.PageID);
                pageChangedEvent(this, args);
            }
        }

        /// <summary>
        /// 设置默认类别单击事件
        /// </summary>
        public event EventHandler<RoutedEventArgs> SetDefaultCategoryClick;
        private void RaiseSetDefaultCategoryClick(RoutedEventArgs e)
        {
            var handler = SetDefaultCategoryClick;
            if (handler != null)
            {
                handler(this.cbSetDefault, e);
            }

        }

        private void ClickDefaultCategory(int pageID)
        {
            this.cbSetDefault.IsChecked = pageID == -1;
            cbSetDefault_Click(this.cbSetDefault, new RoutedEventArgs());
        }

        #endregion

        private void cbSetDefault_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbSetDefault.IsChecked == true)
            {
                this.cbExtendC3.IsChecked = false;
                this.cbExtendC3.IsEnabled = false;
                this.ucCategoryPicker.Visibility = Visibility.Collapsed;
                this.lstPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                switch (this.PagePresentationType)
                {
                    case PageTypePresentationType.Category1:
                    case PageTypePresentationType.Category2:
                        this.ucCategoryPicker.Visibility = Visibility.Visible;
                        break;
                    case PageTypePresentationType.Category3:
                        this.ucCategoryPicker.Visibility = Visibility.Visible;
                        this.cbExtendC3.IsEnabled = true;
                        break;
                    case PageTypePresentationType.Stores:
                    case PageTypePresentationType.OtherSales:
                    case PageTypePresentationType.Brand:
                        this.lstPage.Visibility = Visibility.Visible;
                        break;
                    default:
                        this.ucCategoryPicker.Visibility = Visibility.Collapsed;
                        this.lstPage.Visibility = Visibility.Collapsed;
                        break;
                }
            }

            RaiseSetDefaultCategoryClick(e);
        }

        private void cbExtendC3_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbExtendC3.IsChecked == true)
            {
                this.cbSetDefault.IsChecked = false;
                this.cbSetDefault.IsEnabled = false;
            }
            else
            {
                this.cbSetDefault.IsEnabled = true;
            }
        }
    }

    /// <summary>
    /// 页面类型改变事件
    /// </summary>
    public class PageTypeSelectionChangedEventArgs : EventArgs
    {
        private int? _pageType;
        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType
        {
            get
            {
                return _pageType;
            }
        }

        public PageTypeSelectionChangedEventArgs(int? pageType)
        {
            _pageType = pageType;
        }
    }

    /// <summary>
    /// 具体页面发生改变事件
    /// </summary>
    public class PageSelectionChangedEventArgs : EventArgs
    {
        private PageTypePresentationType _presentationType;
        /// <summary>
        /// 页面类型子级显示模式
        /// </summary>
        public PageTypePresentationType PresentationType
        {
            get
            {
                return _presentationType;
            }
        }

        private int? _pageType;
        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType
        {
            get
            {
                return _pageType;
            }
        }

        private int? _pageID;
        /// <summary>
        /// 页面编号
        /// </summary>
        public int? PageID
        {
            get
            {
                return _pageID;
            }
        }

        public PageSelectionChangedEventArgs(PageTypePresentationType presentationType, int? pageType, int? pageID)
        {
            _presentationType = presentationType;
            _pageType = pageType;
            _pageID = pageID;
        }
    }
}
