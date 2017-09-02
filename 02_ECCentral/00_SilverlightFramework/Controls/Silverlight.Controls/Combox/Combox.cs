using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Newegg.Oversea.Silverlight.Controls.Resources;
using System.Collections;
using System.Windows.Data;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class Combox : ComboBox
    {
        internal TextBlock m_loadingPlaceHolder;
        internal ContentPresenter m_contentPresenter;

        public static DependencyProperty IsShowLoadingProperty = DependencyProperty.Register("IsShowLoading", typeof(bool), typeof(Combox), new PropertyMetadata(false, IsShowLoadingPropertyChanged));
        public static DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(Combox), null);
        public new static DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(Combox), new PropertyMetadata(new PropertyChangedCallback(SelectedValuePropertyChanged)));
        private static void SelectedValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Combox)o).SetSelectionFromValue();
        }

        private static void IsShowLoadingPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var combox = o as Combox;

            if (combox != null 
                && combox.m_loadingPlaceHolder != null
                && combox.m_contentPresenter!=null)
            {
                if (combox.IsShowLoading)
                {
                    combox.m_loadingPlaceHolder.Visibility = System.Windows.Visibility.Visible;
                    combox.m_contentPresenter.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    combox.m_loadingPlaceHolder.Visibility = System.Windows.Visibility.Collapsed;
                    combox.m_contentPresenter.Visibility = System.Windows.Visibility.Visible;
                }
            }

        }

        public static string ExtraAllText
        {
            get
            {
                return MessageResource.Combox_ExtraAllText;
            }
        }

        public static string ExtraSelectText
        {
            get
            {
                return MessageResource.Combox_ExtraSelectText;
            }
        }

        public bool IsShowLoading
        {
            get { return (bool)base.GetValue(Combox.IsShowLoadingProperty); }
            set
            {
                base.SetValue(Combox.IsShowLoadingProperty, value);
            }
        }

        public string SelectedValuePath
        {
            get { return ((string)(base.GetValue(Combox.SelectedValuePathProperty))); }
            set { base.SetValue(Combox.SelectedValuePathProperty, value); }
        }

        public new object SelectedValue
        {
            get { return ((object)(base.GetValue(Combox.SelectedValueProperty))); }
            set { base.SetValue(Combox.SelectedValueProperty, value); }
        }

        public Combox()
        {
            this.Loaded += new RoutedEventHandler(ComboBox_Loaded);
            this.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.m_contentPresenter = base.GetTemplateChild("ContentPresenter") as ContentPresenter;
            this.m_loadingPlaceHolder = base.GetTemplateChild("LoadingPlaceHolder") as TextBlock;
            if (this.m_loadingPlaceHolder != null && m_contentPresenter != null)
            {
                this.m_loadingPlaceHolder.Text = MessageResource.Combox_Loading;

                if (this.IsShowLoading)
                {
                    this.m_loadingPlaceHolder.Visibility = System.Windows.Visibility.Visible;
                    this.m_contentPresenter.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsShowLoading = false;
            base.OnItemsChanged(e);

            SetSelectionFromValue();
        }

        void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectionFromValue();
        }

        void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (SelectedValuePath != null)
                {
                    SelectedValue = GetMemberValue(e.AddedItems[0]);
                }
                else
                {
                    SelectedValue = e.AddedItems[0];
                }
            }
            else
            {
                SelectedValue = null;
            }
        }


        private object GetMemberValue(object item)
        {
            if (item == null)
            {
                return null;
            }

            string valuePath = SelectedValuePath;
            if (valuePath == null || valuePath.Trim() == String.Empty)
            {
                return item;
            }

            return item.GetType().GetProperty(valuePath).GetValue(item, null);
        }

        private bool MemberValueAreEqual(object item, object value)
        {
            var val = GetMemberValue(item);
            if (val == null && value == null)
            {
                return true;
            }
            else if (val == null || value == null)
            {
                return false;
            }

            var valType = val.GetType();
            var valueType = value.GetType();
            if (valType == typeof(string) && valueType != typeof(string))
            {
                return val.Equals(value.ToString());
            }
            else if (valType != typeof(string) && valueType == typeof(string))
            {
                return value.Equals(val.ToString());
            }
            else
            {
                return val.Equals(value);
            }
        }

        private void SetSelectionFromValue()
        {
            var value = SelectedValue;
            if (Items.Count > 0)
            {
                var q = from item in Items
                        where MemberValueAreEqual(item, value)
                        select item;
                var sel = q.FirstOrDefault();
                SelectedItem = sel;
            }
        }
    }

}
