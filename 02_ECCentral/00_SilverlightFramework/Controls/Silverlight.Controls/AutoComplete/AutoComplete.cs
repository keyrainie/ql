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
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class AutoComplete : AutoCompleteBox
    {
        private ButtonBase _buttonToggle;
        private bool _isClickOpenDropDownList;
        private Grid _gridToggleButon;
        private TextBox _textBox;

        public static readonly DependencyProperty ListBoxStyleProperty = DependencyProperty.Register("ListBoxStyle", typeof(Style), typeof(AutoComplete), null);
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object), typeof(AutoComplete), new PropertyMetadata(new PropertyChangedCallback(SelectedValuePropertyChanged)));
        public static readonly DependencyProperty IsShowAllDataWhenKeyDownProperty =
            DependencyProperty.Register("IsShowAllDataWhenKeyDown", typeof(bool), typeof(AutoComplete), new PropertyMetadata(false));

        private static void SelectedValuePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (((AutoComplete)o).ValuePath != null)
            {
                ((AutoComplete)o).SetSelectionFromValue();
            }
        }

        public static readonly DependencyProperty IsShowToggleButtonProperty =
            DependencyProperty.Register("IsShowToggleButton", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true, new PropertyChangedCallback(IsShowToggleButtonPropertyChanged)));
        private static void IsShowToggleButtonPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete autoComplete = o as AutoComplete;
            if (e.NewValue != null 
                && (bool)e.NewValue 
                && autoComplete != null 
                && autoComplete._gridToggleButon != null)
            {
                autoComplete._gridToggleButon.Visibility = Visibility.Visible;
            }
            else if (e.NewValue != null 
                && !(bool)e.NewValue 
                && autoComplete != null 
                && autoComplete._gridToggleButon != null)
            {
                autoComplete._gridToggleButon.Visibility = Visibility.Collapsed;
            }

        }

        public Style ListBoxStyle
        {
            get { return (Style)GetValue(ListBoxStyleProperty); }
            set { SetValue(ListBoxStyleProperty, value); }
        }

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public bool IsShowToggleButton
        {
            get { return (bool)GetValue(IsShowToggleButtonProperty); }
            set { SetValue(IsShowToggleButtonProperty, value); }
        }

        public bool IsShowAllDataWhenKeyDown
        {
            get { return (bool)GetValue(IsShowAllDataWhenKeyDownProperty); }
            set { SetValue(IsShowAllDataWhenKeyDownProperty, value); }
        }


        public string ValuePath { get; set; }
        public ListBox Selector { get; set; }

        private EventHandler m_matchCompleted;

        public event EventHandler MatchCompleted
        {
            add
            {
                 m_matchCompleted += value;
            }
            remove
            {
                m_matchCompleted -= value;
            }
        }


        public AutoComplete()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                this.DefaultStyleKey = typeof(AutoComplete);

                this.Loaded += new RoutedEventHandler(AutoComplete_Loaded);
                this.SelectionChanged += new SelectionChangedEventHandler(AutoComplete_SelectionChanged);
                this.DropDownOpened += new RoutedPropertyChangedEventHandler<bool>(AutoComplete_DropDownOpened);
                this.SizeChanged += new SizeChangedEventHandler(AutoComplete_SizeChanged);
                this.MouseEnter += new MouseEventHandler(AutoComplete_MouseEnter);
                this.MouseLeave += new MouseEventHandler(AutoComplete_MouseLeave);
            }
        }

        void AutoComplete_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(_textBox, "Normal", false);
            VisualStateManager.GoToState(this, "Normal", false);
        }

        void AutoComplete_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(_textBox, "MouseOver", false);
            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        void AutoComplete_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                this.Selector.Width = this.ActualWidth;
            }
        }

        void AutoComplete_DropDownOpened(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (Selector != null && _isClickOpenDropDownList)
            {
                if (this.ItemsSource is IList)
                {
                    IList list = (IList)Selector.ItemsSource;
                    IList originalList = (IList)this.ItemsSource;
                    if (list != null && originalList != null)
                    {
                        list.Clear();
                        for (int i = 0; i < originalList.Count; i++)
                        {
                            list.Add(originalList[i]);
                        }
                    }
                }
                else if (this.ItemsSource is PagedCollectionView)
                {
                    IList list = (IList)Selector.ItemsSource;
                    if (list != null)
                    {
                        list.Clear();
                        foreach (var item in this.ItemsSource)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            else if (m_matchCompleted != null)
            {
                this.m_matchCompleted(this,new EventArgs());
            }
            _isClickOpenDropDownList = false;
            this.InvokeOnLayoutUpdated(() => 
            {
                this.Selector.Width = this.ActualWidth;
            });
            this._textBox.Focus();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignerProperties.IsInDesignTool)
            {
                this._textBox = base.GetTemplateChild("Text") as TextBox;
                this._buttonToggle = base.GetTemplateChild("ToggleButton") as ButtonBase;

                if (_textBox != null)
                {
                    _textBox.KeyDown -= new KeyEventHandler(_textBox_KeyDown);
                    _textBox.KeyDown += new KeyEventHandler(_textBox_KeyDown);

                    _textBox.KeyUp -= new KeyEventHandler(_textBox_KeyUp);
                    _textBox.KeyUp += new KeyEventHandler(_textBox_KeyUp);
                }

                if (this._buttonToggle != null)
                {
                    this._buttonToggle.Click -= new RoutedEventHandler(_buttonToggle_Click);
                }

                if (this._buttonToggle != null)
                {
                    this._buttonToggle.Click += new RoutedEventHandler(_buttonToggle_Click);
                }

                if (this.Selector == null)
                {
                    this.Selector = this.GetTemplateChild("Selector") as ListBox;
                }

                if (this._gridToggleButon == null)
                {
                    _gridToggleButon = this.GetTemplateChild("GridToggleButton") as Grid;
                    if (_gridToggleButon != null && !this.IsShowToggleButton)
                    {
                        this._gridToggleButon.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        void _textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && IsShowAllDataWhenKeyDown)
            {
                if (m_currentSelectedItem != null)
                {
                    if (Selector != null)
                    {
                        Selector.SelectedItem = m_currentSelectedItem;
                    }
                    m_currentSelectedItem = null;
                }
            }
        }

        private object m_currentSelectedItem = null;

        void _textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && IsShowAllDataWhenKeyDown)
            {
                _isClickOpenDropDownList = true;
                if (!this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = true;
                    if (m_currentSelectedItem == null)
                    {
                        m_currentSelectedItem = this.SelectedItem;
                        e.Handled = true;
                    }
                }
            }
        }

        
        void AutoComplete_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectionFromValue();
            this.ValueMemberPath = this.ValuePath;
        }        

        void AutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (!string.IsNullOrEmpty(ValuePath))
                {
                    SelectedValue = GetMemberValue(e.AddedItems[0]);
                }
                else
                {
                    SelectedValue = e.AddedItems[0];
                }
                if (this.ItemsSource is PagedCollectionView)
                {
                    ((PagedCollectionView)this.ItemsSource).MoveCurrentTo(this.SelectedItem);
                }
            }
            else
            {
                SelectedValue = null;
            }

            
        }

        void _buttonToggle_Click(object sender, RoutedEventArgs e)
        {
            _isClickOpenDropDownList = true;
            FrameworkElement fe = sender as FrameworkElement;
            AutoCompleteBox acb = null;
            while (fe != null && acb == null)
            {
                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
                acb = fe as AutoCompleteBox;
            }
            if (acb != null)
            {
                acb.IsDropDownOpen = !acb.IsDropDownOpen;
            }
        }

        private object GetMemberValue(object item)
        {
            if (item == null)
            {
                return null;
            }
            return item.GetType().GetProperty(ValuePath).GetValue(item, null);
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
            if (this.ItemsSource != null)
            {
                foreach (var item in this.ItemsSource)
                {
                    if (MemberValueAreEqual(item, value))
                    {
                        SelectedItem = item;
                        break;
                    }
                }
            }           
        }
    }
}
