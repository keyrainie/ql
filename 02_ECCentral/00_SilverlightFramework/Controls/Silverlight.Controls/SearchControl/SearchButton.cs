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

using Newegg.Oversea.Silverlight.Utilities;
//using Newegg.Oversea.Silverlight.Controls.Behaviours;
using Newegg.Oversea.Silverlight.Controls.Resources;
using Newegg.Oversea.Silverlight.Core.Components;
using System.ComponentModel;

namespace Newegg.Oversea.Silverlight.Controls
{
    [TemplatePart(Name = SearchButton.Name_RootElement, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SearchButton.Name_RichButton, Type = typeof(RichButton))]
    [TemplatePart(Name = SearchButton.Name_RichListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = SearchButton.Name_NameTextbox, Type = typeof(TextBox))]
    [TemplatePart(Name = SearchButton.Name_SaveButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = SearchButton.Name_ClearButton, Type = typeof(ButtonBase))]
    public class SearchButton : ContentControl
    {
        public static readonly DependencyProperty SearchConditionIdProperty = DependencyProperty.Register("SearchConditionId", typeof(string), typeof(SearchButton), null);
        public static readonly DependencyProperty SearchConditionTypeProperty = DependencyProperty.Register("SearchConditionType", typeof(Type), typeof(SearchButton), new PropertyMetadata(typeof(object)));

        private IUserProfile m_userProfile;

        public Func<object> GetSearchConditionHandler { get; set; }

        public event RoutedEventHandler Click;
        public event EventHandler<RoutedDataEventArgs> SearchConditionChanged;

        public SearchButton()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                m_userProfile = ComponentFactory.GetComponent<IUserProfile>();
            }

            DefaultStyleKey = typeof(SearchButton);
        }

        private const string Name_RootElement = "RootElement";
        private const string Name_RichButton = "RichButton";
        private const string Name_RichListBox = "RichListBox";
        private const string Name_NameTextbox = "NameTextbox";
        private const string Name_SaveButton = "SaveButton";
        private const string Name_ClearButton = "ClearButton";

        private FrameworkElement RootElement { get; set; }
        private RichButton RichButton { get; set; }
        private ListBox RichListBox { get; set; }
        private TextBox NameTextbox { get; set; }
        private ButtonBase SaveButton { get; set; }
        private ButtonBase ClearButton { get; set; }

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

        public object GetSearchCondition()
        {
            object dataSource = null;

            if (this.SearchConditionId != null && this.SearchConditionType != null)
            {
                if (this.GetSearchConditionHandler != null)
                {
                    dataSource = this.GetSearchConditionHandler();
                }
                else
                {
                    dataSource = this.DataContext;
                }

                if (dataSource == null || dataSource.GetType() != this.SearchConditionType)
                {
                    dataSource = Activator.CreateInstance(this.SearchConditionType);
                }
                else
                {
                    dataSource = UtilityHelper.DeepClone(dataSource, this.SearchConditionType);
                }
            }

            return dataSource;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.RootElement = GetTemplateChild(Name_RootElement) as FrameworkElement;
            this.RichButton = GetTemplateChild(Name_RichButton) as RichButton;
            this.RichListBox = GetTemplateChild(Name_RichListBox) as ListBox;
            this.NameTextbox = GetTemplateChild(Name_NameTextbox) as TextBox;
            this.SaveButton = GetTemplateChild(Name_SaveButton) as ButtonBase;
            this.ClearButton = GetTemplateChild(Name_ClearButton) as ButtonBase;

            if (this.RichButton != null)
            {
                if (this.SearchConditionId == null || this.SearchConditionType == null)
                {
                    this.RichButton.CanPopup = false;
                }
                else
                {
                   // new MouseWheelBehavior().Attach(this.RichButton);
                }

                if (this.RichButton.Content == null)
                {
                    this.RichButton.Content = MessageResource.SearchControl_SearchButtonContent;
                }

                this.RichButton.Click += new RoutedEventHandler(RichButton_Click);
            }

            if (this.SearchConditionId != null && this.SearchConditionType != null)
            {
                if (this.RichListBox != null)
                {
                    //m_userProfile.LoadSearchCondition(this.SearchConditionId, settings =>
                    //{
                    //    BindSearchConditionListBox(settings);
                    //});
                }
                if (this.NameTextbox != null)
                {
                    this.NameTextbox.Text = "";
                    this.SaveButton.IsEnabled = false;
                    this.NameTextbox.TextChanged += new TextChangedEventHandler(NameTextbox_TextChanged);
                }
                if (this.SaveButton != null)
                {
                    //this.SaveButton.Click += new RoutedEventHandler(SaveButton_Click);
                }
                if (this.ClearButton != null)
                {
                    //this.ClearButton.Click += new RoutedEventHandler(ClearButton_Click);
                }
            }
        }

        void RichButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Click != null)
            {
                this.Click(sender, e);
            }
        }

        void NameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SaveButton.IsEnabled = this.NameTextbox.Text.Trim() != String.Empty;
        }

        //void ListBoxItem_ItemSelected(object sender, RoutedEventArgs e)
        //{
        //    ListBoxItemExt li = this.RichListBox.SelectedItem as ListBoxItemExt;
        //    if (li != null)
        //    {
        //        SearchCondition condition = li.DataContext as SearchCondition;
        //        object dataSource = UtilityHelper.XmlDeserialize(condition.ConditionString, this.SearchConditionType);
        //        if (dataSource == null || dataSource.GetType() != this.SearchConditionType)
        //        {
        //            dataSource = Activator.CreateInstance(this.SearchConditionType);
        //        }

        //        this.NameTextbox.Text = condition.Name;
        //        this.NameTextbox.IsEnabled = true;

        //        if (this.SearchConditionChanged != null)
        //        {
        //            this.SearchConditionChanged(this, new RoutedDataEventArgs()
        //            {
        //                Data = dataSource
        //            });
        //        }
        //    }

        //    this.RichButton.ClosePopup();
        //}

        //void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.SearchConditionId != null && this.SearchConditionType != null)
        //    {
        //        object dataSource = this.GetSearchCondition();

        //        SearchConditionData itemData = new SearchConditionData()
        //        {
        //            ConditionGuid = this.SearchConditionId,
        //            Item = new SearchCondition()
        //            {
        //                Name = this.NameTextbox.Text.Trim(),
        //                IsDefault = true,
        //                ConditionType = this.SearchConditionType.FullName,
        //                ConditionString = UtilityHelper.XmlSerialize(dataSource)
        //            }
        //        };

        //        this.NameTextbox.Text = String.Empty;
        //        this.RichButton.ClosePopup();

        //        m_userProfile.SaveSearchCondition(itemData, settings =>
        //        {
        //            BindSearchConditionListBox(settings);
        //        });
        //    }
        //}

        //void ClearButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.SearchConditionId != null && this.SearchConditionType != null && this.SearchConditionChanged != null)
        //    {
        //        this.SearchConditionChanged(this, new RoutedDataEventArgs()
        //        {
        //            Data = Activator.CreateInstance(this.SearchConditionType)
        //        });

        //        this.RichButton.ClosePopup();
        //    }
        //}

        //void ListBoxItem_ItemDeleted(object sender, RoutedEventArgs e)
        //{
        //    if (this.SearchConditionId != null && this.SearchConditionType != null)
        //    {
        //        ListBoxItemExt li = sender as ListBoxItemExt;
        //        SearchCondition dataContext = li.DataContext as SearchCondition;

        //        SearchConditionData itemData = new SearchConditionData()
        //        {
        //            ConditionGuid = this.SearchConditionId,
        //            Item = new SearchCondition()
        //            {
        //                Name = dataContext.Name
        //            }
        //        };

        //        m_userProfile.RemoveSearchCondition(itemData, settings =>
        //        {
        //            BindSearchConditionListBox(settings);
        //        });
        //    }
        //}

        //private void BindSearchConditionListBox(List<SearchCondition> conditionList)
        //{
        //    this.RichListBox.Items.Clear();

        //    if (conditionList != null && conditionList.Count > 0)
        //    {
        //        Style style = null;
        //        if (this.RootElement != null)
        //        {
        //            style = (Style)this.RootElement.Resources["RichButtonListBoxItemStyle"];
        //        }
        //        conditionList.ForEach(settingsItem =>
        //        {
        //            ListBoxItemExt li = new ListBoxItemExt();
        //            li.Style = style;
        //            li.DataContext = settingsItem;
        //            li.ItemSelected += new RoutedEventHandler(ListBoxItem_ItemSelected);
        //            li.ItemDeleted += new RoutedEventHandler(ListBoxItem_ItemDeleted);

        //            this.RichListBox.Items.Add(li);
        //        });
        //    }
        //}
    }
}
