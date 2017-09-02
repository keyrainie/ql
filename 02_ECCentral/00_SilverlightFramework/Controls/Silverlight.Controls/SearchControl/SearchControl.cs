using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Data;

using Newegg.Oversea.Silverlight.Controls.Resources;
//using Newegg.Oversea.Silverlight.Controls.Behaviours;    

namespace Newegg.Oversea.Silverlight.Controls
{
    [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Advanced", GroupName = "CommonStates")]
    [TemplatePart(Name = SearchControl.Name_RootElement, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SearchControl.Name_ExpanderButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = SearchControl.Name_SearchButton, Type = typeof(SearchButton))]
    public class SearchControl : ContentControl
    {
        public static readonly DependencyProperty AdvancedSearchHeaderProperty = DependencyProperty.Register("AdvancedSearchHeader", typeof(string), typeof(SearchControl), new PropertyMetadata(MessageResource.SearchControl_AdvancedSearchHeader));
        public static readonly DependencyProperty AdvancedSearchContentProperty = DependencyProperty.Register("AdvancedSearchContent", typeof(object), typeof(SearchControl), null);
        public static readonly DependencyProperty SearchConditionIdProperty = DependencyProperty.Register("SearchConditionId", typeof(string), typeof(SearchControl), null);
        public static readonly DependencyProperty SearchConditionTypeProperty = DependencyProperty.Register("SearchConditionType", typeof(Type), typeof(SearchControl), new PropertyMetadata(typeof(object)));
        public static readonly DependencyProperty SearchButtonContentProperty = DependencyProperty.Register("SearchButtonContent", typeof(object), typeof(SearchControl), new PropertyMetadata(MessageResource.SearchControl_SearchButtonContent));
        public static readonly DependencyProperty SearchButtonMarginProperty = DependencyProperty.Register("SearchButtonMargin", typeof(Thickness), typeof(SearchControl), new PropertyMetadata(new Thickness(0)));
        public static readonly DependencyProperty SearchButtonHorizontalAlignmentProperty = DependencyProperty.Register("SearchButtonHorizontalAlignment", typeof(HorizontalAlignment), typeof(SearchControl), new PropertyMetadata(HorizontalAlignment.Right));
        public static readonly DependencyProperty SearchButtonIsEnabledProperty = DependencyProperty.Register("SearchButtonIsEnabled", typeof(bool), typeof(SearchControl), new PropertyMetadata(true, new PropertyChangedCallback(OnSearchButtonIsEnabledChanged)));

        static void OnSearchButtonIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchControl dateRange = d as SearchControl;
            bool newValue = (bool)e.NewValue;
            bool oldValue = (bool)e.OldValue;

            if (dateRange != null && dateRange._buttonSearch != null && newValue != oldValue)
            {
                dateRange._buttonSearch.IsEnabled = newValue;
            }
        }

        public event RoutedEventHandler SearchButtonClick;
        public event EventHandler<RoutedDataEventArgs> SearchConditionChanged;

        public string AdvancedSearchHeader
        {
            get { return (string)GetValue(AdvancedSearchHeaderProperty); }
            set { SetValue(AdvancedSearchHeaderProperty, value); }
        }

        public object AdvancedSearchContent
        {
            get { return (object)GetValue(AdvancedSearchContentProperty); }
            set { SetValue(AdvancedSearchContentProperty, value); }
        }

        public string SearchConditionId
        {
            get { return (string)GetValue(SearchConditionIdProperty); }
            set { SetValue(SearchConditionIdProperty, value); }
        }

        public Type SearchConditionType
        {
            get { return (Type)GetValue(SearchConditionTypeProperty); }
            set { SetValue(SearchConditionTypeProperty, value); }
        }

        public bool SearchButtonIsEnabled
        {
            get { return (bool)GetValue(SearchButtonIsEnabledProperty); }
            set { SetValue(SearchButtonIsEnabledProperty, value); }
        }

        public object SearchButtonContent
        {
            get { return (object)GetValue(SearchButtonContentProperty); }
            set { SetValue(SearchButtonContentProperty, value); }
        }

        public Thickness SearchButtonMargin
        {
            get { return (Thickness)GetValue(SearchButtonMarginProperty); }
            set { SetValue(SearchButtonMarginProperty, value); }
        }

        public HorizontalAlignment SearchButtonHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(SearchButtonHorizontalAlignmentProperty); }
            set { SetValue(SearchButtonHorizontalAlignmentProperty, value); }
        }

        public object GetSearchCondition()
        {
            object dataSource = null;

            if (this._buttonSearch != null)
            {
                dataSource = this._buttonSearch.GetSearchCondition();
            }

            return dataSource;
        }

        public bool IsAdvancedMode
        {
            get
            {
                return this._buttonExpander.IsChecked.Value;
            }
        }

        private const string Name_RootElement = "RootElement";
        private const string Name_ExpanderButton = "ExpanderButton";
        private const string Name_SearchButton = "SearchButton";
        private FrameworkElement _root;
        private ToggleButton _buttonExpander;
        private SearchButton _buttonSearch;

        public SearchControl()
        {
            DefaultStyleKey = typeof(SearchControl);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._root = GetTemplateChild(Name_RootElement) as FrameworkElement;
            this._buttonExpander = GetTemplateChild(Name_ExpanderButton) as ToggleButton;
            this._buttonSearch = GetTemplateChild(Name_SearchButton) as SearchButton;

            if (this._buttonExpander != null)
            {
                this._buttonExpander.Click += new RoutedEventHandler(ExpanderButton_Click);
            }
            if (this._buttonSearch != null)
            {
                this._buttonSearch.IsEnabled = this.SearchButtonIsEnabled;
                this._buttonSearch.GetSearchConditionHandler = new Func<object>(() =>
                {
                    return this.DataContext;
                });

                this._buttonSearch.Click += new RoutedEventHandler(SearchButtonClick_Click);
                this._buttonSearch.SearchConditionChanged += new EventHandler<RoutedDataEventArgs>(SearchButton_SearchConditionChanged);
            }

            if (this.DataContext == null || this.DataContext.GetType() != this.SearchConditionType)
            {
                this.DataContext = Activator.CreateInstance(this.SearchConditionType);
            }

            string status = this.AdvancedSearchContent == null ? "Normal" : "Advanced";

            VisualStateManager.GoToState(this, status, true);
        }


        void ExpanderButton_Click(object sender, RoutedEventArgs e)
        {
            string status = this._buttonExpander.IsChecked == true ? "Checked" : "Unchecked";

            VisualStateManager.GoToState(this, status, false);
        }

        void SearchButtonClick_Click(object sender, RoutedEventArgs e)
        {
            if (this.SearchButtonClick != null)
            {
                this.SearchButtonClick(sender, e);
            }
        }

        void SearchButton_SearchConditionChanged(object sender, RoutedDataEventArgs e)
        {
            this.DataContext = e.Data;

            if (this.SearchConditionChanged != null)
            {
                this.SearchConditionChanged(this, e);
            }
        }
    }
}
