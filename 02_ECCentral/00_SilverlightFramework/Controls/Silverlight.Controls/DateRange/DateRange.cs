using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Newegg.Oversea.Silverlight.Controls.Resources;

namespace Newegg.Oversea.Silverlight.Controls
{
    public enum RangeType
    {
        /// <summary>
        /// 不指定日期
        /// </summary>
        None,
        /// <summary>
        /// 今天
        /// </summary>
        Today,
        /// <summary>
        /// 最近3天
        /// </summary>
        Last3Days,
        /// <summary>
        /// 最近7天
        /// </summary>
        Last7Days,
        /// <summary>
        /// 最近30天
        /// </summary>
        Last30Days,
        /// <summary>
        /// 特定日期
        /// </summary>
        SpecialDay,
        /// <summary>
        /// 特定日期之前
        /// </summary>
        BeforSpecialDay,
        /// <summary>
        /// 特定日期之后
        /// </summary>
        AfterSpecialDay,
        /// <summary>
        /// 日期范围
        /// </summary>
        Range
    }

    public class DateRange : Control
    {
        private static Dictionary<RangeType, string> rangeTypeDic = new Dictionary<RangeType, string>();


        public static readonly DependencyProperty TextBoxTemplateProperty = DependencyProperty.Register("TextBoxTemplate", typeof(ControlTemplate), typeof(DateRange), new PropertyMetadata(null));
        public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(DateRange), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedRangeTypeProperty = DependencyProperty.Register("SelectedRangeType", typeof(RangeType), typeof(DateRange), new PropertyMetadata(RangeType.Range, new PropertyChangedCallback(OnSelectedRangeTypeChanged)));
        public static readonly DependencyProperty SelectedDateStartProperty = DependencyProperty.Register("SelectedDateStart", typeof(DateTime?), typeof(DateRange), new PropertyMetadata(null, new PropertyChangedCallback(SelectedDatePropertyChangedCallback)));
        public static readonly DependencyProperty SelectedDateEndProperty = DependencyProperty.Register("SelectedDateEnd", typeof(DateTime?), typeof(DateRange), new PropertyMetadata(null, new PropertyChangedCallback(SelectedDatePropertyChangedCallback)));
        public static readonly DependencyProperty DisplaySpecificDateFormatProperty = DependencyProperty.Register("DisplaySpecificDateFormat", typeof(string), typeof(DateRange), new PropertyMetadata(MessageResource.DateRange_DisplaySpecificDateFormat));
        public static readonly DependencyProperty DisplayRangeDateFormatProperty = DependencyProperty.Register("DisplayRangeDateFormat", typeof(string), typeof(DateRange), new PropertyMetadata(MessageResource.DateRange_DisplayRangeDateFormat));
        public static readonly DependencyProperty DisplayBeforeDateFormatProperty = DependencyProperty.Register("DisplayBeforeDateFormat", typeof(string), typeof(DateRange), new PropertyMetadata(MessageResource.DateRange_DisplayBeforeDateFormat));
        public static readonly DependencyProperty DisplayAfterDateFormatProperty = DependencyProperty.Register("DisplayAfterDateFormat", typeof(string), typeof(DateRange), new PropertyMetadata(MessageResource.DateRange_DisplayAfterDateFormat));

        private static void SelectedDatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DateRange;
            control.DisplayText();
        }

        public static readonly string RangeType_Today_Code = MessageResource.DateRange_RangeType_Today_Code;
        public static readonly string RangeType_Last3days_Code = MessageResource.DateRange_RangeType_Last3days_Code;
        public static readonly string RangeType_Last7days_Code = MessageResource.DateRange_RangeType_Last7days_Code;
        public static readonly string RangeType_Last30days_Code = MessageResource.DateRange_RangeType_Last30days_Code;
        public static readonly string RangeType_ClearDays_Code = MessageResource.DateRange_RangeType_ClearDays_Code;
        public static readonly string RangeType_SpecificDate_Code = MessageResource.DateRange_RangeType_SpecificDate_Code;
        public static readonly string RangeType_BeforeDate_Code = MessageResource.DateRange_RangeType_BeforeDate_Code;
        public static readonly string RangeType_AfterDate_Code = MessageResource.DateRange_RangeType_AfterDate_Code;
        public static readonly string RangeType_RangeDate_Code = MessageResource.DateRange_RangeType_RangeDate_Code;

        static void OnSelectedRangeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateRange dateRange = d as DateRange;
            RangeType oldValue, newValue;
            Enum.TryParse<RangeType>(e.OldValue.ToString(), out oldValue);
            Enum.TryParse<RangeType>(e.NewValue.ToString(), out newValue);

            string str1 = rangeTypeDic.FirstOrDefault(p => p.Key == newValue).Value;
            if (dateRange._listbox != null && newValue != oldValue)
            {
                SelectItemByTag(dateRange._listbox, str1);
                SetSelectedDate(dateRange, str1);
            }
        }

        public ControlTemplate TextBoxTemplate
        {
            get { return (ControlTemplate)GetValue(TextBoxTemplateProperty); }
            set { SetValue(TextBoxTemplateProperty, value); }
        }

        public Style TextBoxStyle
        {
            get { return (Style)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        public RangeType SelectedRangeType
        {
            get { return (RangeType)GetValue(SelectedRangeTypeProperty); }
            set { SetValue(SelectedRangeTypeProperty, value); }
        }

        public DateTime? SelectedDateStart
        {
            get { return (DateTime?)GetValue(SelectedDateStartProperty); }
            set { SetValue(SelectedDateStartProperty, value); }
        }

        public DateTime? SelectedDateEnd
        {
            get { return (DateTime?)GetValue(SelectedDateEndProperty); }
            set { SetValue(SelectedDateEndProperty, value); }
        }

        public string DisplaySpecificDateFormat
        {
            get { return (string)GetValue(DisplaySpecificDateFormatProperty); }
            set { SetValue(DisplaySpecificDateFormatProperty, value); }
        }

        public string DisplayRangeDateFormat
        {
            get { return (string)GetValue(DisplayRangeDateFormatProperty); }
            set { SetValue(DisplayRangeDateFormatProperty, value); }
        }

        public string DisplayBeforeDateFormat
        {
            get { return (string)GetValue(DisplayBeforeDateFormatProperty); }
            set { SetValue(DisplayBeforeDateFormatProperty, value); }
        }

        public string DisplayAfterDateFormat
        {
            get { return (string)GetValue(DisplayAfterDateFormatProperty); }
            set { SetValue(DisplayAfterDateFormatProperty, value); }
        }

        private FrameworkElement _root;
        private TextBox _textBox;
        private ButtonBase _buttonDropDown;
        private Popup _popup;
        private FrameworkElement _popupRoot;
        private Canvas _canvasOutside;
        private Canvas _canvasPopupOutside;

        private ListBox _listbox;
        private Panel _panelMain;
        private Panel _panelSimple;
        private TextBlock _textBlockSimple;
        private Calendar _calendarSimple;
        private Panel _panelRange;
        private Calendar _calendarStart;
        private Calendar _calendarEnd;
        private ButtonBase _buttonDone;

        static DateRange()
        {
            rangeTypeDic.Add(RangeType.None, "C");
            rangeTypeDic.Add(RangeType.Today, "T");
            rangeTypeDic.Add(RangeType.Last3Days, "L3");
            rangeTypeDic.Add(RangeType.Last7Days, "L7");
            rangeTypeDic.Add(RangeType.Last30Days, "L30");
            rangeTypeDic.Add(RangeType.SpecialDay, "SD");
            rangeTypeDic.Add(RangeType.BeforSpecialDay, "BD");
            rangeTypeDic.Add(RangeType.AfterSpecialDay, "AD");
            rangeTypeDic.Add(RangeType.Range, "RD");
        }

        public DateRange()
        {
            this.DefaultStyleKey = typeof(DateRange);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._popup = base.GetTemplateChild("Popup") as Popup;
            this._popupRoot = base.GetTemplateChild("PopupRoot") as FrameworkElement;
            if (this._popup != null && this._popupRoot != null)
            {
                if (this._canvasOutside == null)
                {
                    this._popupRoot.SizeChanged += new SizeChangedEventHandler(_popupRoot_SizeChanged);
                    this._canvasPopupOutside = new Canvas();
                    this._canvasPopupOutside.Background = new SolidColorBrush(Colors.Transparent);
                    this._canvasPopupOutside.MouseLeftButtonDown += new MouseButtonEventHandler(_canvasPopupOutside_MouseLeftButtonDown);

                    this._canvasOutside = new Canvas();
                    this._popup.Child = this._canvasOutside;
                    this._canvasOutside.Children.Add(this._canvasPopupOutside);
                    this._canvasOutside.Children.Add(this._popupRoot);
                }
            }

            this._root = base.GetTemplateChild("Root") as FrameworkElement;
            this._textBox = base.GetTemplateChild("TextBox") as TextBox;

            this._panelMain = base.GetTemplateChild("PanelMain") as Panel;
            this._panelSimple = base.GetTemplateChild("PanelSimple") as Panel;
            this._textBlockSimple = base.GetTemplateChild("TextBlockSimple") as TextBlock;
            this._calendarSimple = base.GetTemplateChild("CalendarSimple") as Calendar;
            this._panelRange = base.GetTemplateChild("PanelRange") as Panel;
            this._calendarStart = base.GetTemplateChild("CalendarStart") as Calendar;
            this._calendarEnd = base.GetTemplateChild("CalendarEnd") as Calendar;

            if (this._buttonDropDown != null)
            {
                this._buttonDropDown.Click -= new RoutedEventHandler(this._buttonDropDown_Click);
            }
            this._buttonDropDown = base.GetTemplateChild("ButtonDropDown") as ButtonBase;
            if (this._buttonDropDown != null)
            {
                this._buttonDropDown.Click += new RoutedEventHandler(this._buttonDropDown_Click);
            }

            if (this._listbox != null)
            {
                this._listbox.MouseLeftButtonUp -= new MouseButtonEventHandler(_listbox_MouseLeftButtonUp);
                this._listbox.SelectionChanged -= new SelectionChangedEventHandler(_listbox_SelectionChanged);
                foreach (object item in this._listbox.Items)
                {
                    ListBoxItem listBoxItem = item as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        string itemTag = listBoxItem.Tag as string;
                        if (itemTag == RangeType_SpecificDate_Code || itemTag == RangeType_BeforeDate_Code || itemTag == RangeType_AfterDate_Code || itemTag == RangeType_RangeDate_Code)
                        {
                            listBoxItem.MouseEnter -= new MouseEventHandler(_listBoxItemAutoSelect_MouseEnter);
                        }
                    }
                }
            }
            this._listbox = base.GetTemplateChild("ListBox") as ListBox;
            if (this._listbox != null)
            {
                this._listbox.MouseLeftButtonUp += new MouseButtonEventHandler(_listbox_MouseLeftButtonUp);
                this._listbox.SelectionChanged += new SelectionChangedEventHandler(_listbox_SelectionChanged);
                foreach (object item in this._listbox.Items)
                {
                    ListBoxItem listBoxItem = item as ListBoxItem;
                    if (listBoxItem != null)
                    {
                        string itemTag = listBoxItem.Tag as string;
                        if (itemTag == RangeType_SpecificDate_Code || itemTag == RangeType_BeforeDate_Code || itemTag == RangeType_AfterDate_Code || itemTag == RangeType_RangeDate_Code)
                        {
                            listBoxItem.MouseEnter += new MouseEventHandler(_listBoxItemAutoSelect_MouseEnter);
                        }
                    }
                }

                SelectItemByTag(this._listbox, rangeTypeDic[this.SelectedRangeType]);
            }

            if (this._buttonDone != null)
            {
                this._buttonDone.Click -= new RoutedEventHandler(_buttonDone_Click);
            }
            this._buttonDone = base.GetTemplateChild("ButtonDone") as ButtonBase;
            if (this._buttonDone != null)
            {
                this._buttonDone.Click += new RoutedEventHandler(_buttonDone_Click);
            }
        }

        void _popupRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ArrangePopup();
        }

        void _canvasPopupOutside_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ClosePopup();
        }

        void _buttonDropDown_Click(object sender, RoutedEventArgs e)
        {
            this.ArrangePopup();

            if (this._popup != null)
            {
                bool isOpen = !this._popup.IsOpen;
                if (isOpen)
                {
                    if (this._panelMain != null && this._panelMain.Visibility == Visibility.Visible)
                    {
                        this._panelMain.Visibility = Visibility.Collapsed;
                    }
                    if (this._listbox != null)
                    {
                        this._listbox.SelectedIndex = -1;
                    }
                }
                this._popup.IsOpen = isOpen;
            }
        }

        void _listbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                ListBoxItem selectedItem = listBox.SelectedItem as ListBoxItem;
                if (selectedItem != null)
                {
                    string selectedItemTag = selectedItem.Tag as string;

                    if (selectedItemTag == RangeType_Today_Code || selectedItemTag == RangeType_Last3days_Code || selectedItemTag == RangeType_Last7days_Code || selectedItemTag == RangeType_Last30days_Code || selectedItemTag == RangeType_ClearDays_Code)
                    {
                        this.ClosePopup();
                    }
                }
            }
        }

        void _listBoxItemAutoSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ((ListBoxItem)sender).IsSelected = true;
            }
        }

        void _listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                bool isDropDownOpen = true; //this._popup.IsOpen;
                bool needArrangePopup = false;

                ListBoxItem selectedItem = listBox.SelectedItem as ListBoxItem;
                if (selectedItem != null)
                {
                    DateTime dateTimeNow = DateTime.Now;
                    string selectedItemTag = selectedItem.Tag as string;

                    if (selectedItemTag == RangeType_Today_Code || selectedItemTag == RangeType_Last3days_Code || selectedItemTag == RangeType_Last7days_Code || selectedItemTag == RangeType_Last30days_Code || selectedItemTag == RangeType_ClearDays_Code)
                    {
                        SetSelectedDate(this, selectedItemTag);

                        if (this._panelMain != null)
                        {
                            this._panelMain.Visibility = Visibility.Collapsed;
                        }
                        if (this._panelSimple != null)
                        {
                            this._panelSimple.Visibility = Visibility.Collapsed;
                        }
                        if (this._panelRange != null)
                        {
                            this._panelRange.Visibility = Visibility.Collapsed;
                        }

                        isDropDownOpen = false;

                        this.SelectedRangeType = rangeTypeDic.FirstOrDefault(p => p.Value == selectedItemTag).Key;
                    }
                    else if (selectedItemTag == RangeType_SpecificDate_Code || selectedItemTag == RangeType_BeforeDate_Code || selectedItemTag == RangeType_AfterDate_Code)
                    {
                        DateTime dateTimeSimple = dateTimeNow;
                        string simpleText = String.Empty;

                        if (selectedItemTag == RangeType_SpecificDate_Code)
                        {
                            dateTimeSimple = this.SelectedDateStart ?? dateTimeNow;
                            simpleText = MessageResource.DateRange_Calendar_SpecificDate_Title;
                        }
                        else if (selectedItemTag == RangeType_BeforeDate_Code)
                        {
                            dateTimeSimple = this.SelectedDateEnd ?? dateTimeNow;
                            simpleText = MessageResource.DateRange_Calendar_AllDatesBefore_Title;
                        }
                        else if (selectedItemTag == RangeType_AfterDate_Code)
                        {
                            dateTimeSimple = this.SelectedDateStart ?? dateTimeNow;
                            simpleText = MessageResource.DateRange_Calendar_AllDatesAfter_Title;
                        }

                        if (this._textBlockSimple != null)
                        {
                            this._textBlockSimple.Text = simpleText;
                        }
                        if (this._calendarSimple != null)
                        {
                            this._calendarSimple.SelectedDate = dateTimeSimple;
                            this._calendarSimple.DisplayDate = dateTimeSimple;
                        }

                        if (this._panelMain != null && this._panelMain.Visibility == Visibility.Collapsed)
                        {
                            needArrangePopup = true;
                            this._panelMain.Visibility = Visibility.Visible;
                        }
                        if (this._panelSimple != null && this._panelSimple.Visibility == Visibility.Collapsed)
                        {
                            needArrangePopup = true;
                            this._panelSimple.Visibility = Visibility.Visible;
                        }
                        if (this._panelRange != null && this._panelRange.Visibility == Visibility.Visible)
                        {
                            needArrangePopup = true;
                            this._panelRange.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (selectedItemTag == RangeType_RangeDate_Code)
                    {
                        DateTime dateTimeStart = this.SelectedDateStart ?? dateTimeNow;
                        DateTime dateTimeEnd = (this.SelectedDateEnd ?? dateTimeNow.AddDays(1)).AddDays(-1);

                        if (this._calendarStart != null)
                        {
                            this._calendarStart.SelectedDate = dateTimeStart;
                            this._calendarStart.DisplayDate = dateTimeStart;
                        }
                        if (this._calendarEnd != null)
                        {
                            this._calendarEnd.SelectedDate = dateTimeEnd;
                            this._calendarEnd.DisplayDate = dateTimeEnd;
                        }

                        if (this._panelMain != null && this._panelMain.Visibility == Visibility.Collapsed)
                        {
                            needArrangePopup = true;
                            this._panelMain.Visibility = Visibility.Visible;
                        }
                        if (this._panelSimple != null && this._panelSimple.Visibility == Visibility.Visible)
                        {
                            needArrangePopup = true;
                            this._panelSimple.Visibility = Visibility.Collapsed;
                        }
                        if (this._panelRange != null && this._panelRange.Visibility == Visibility.Collapsed)
                        {
                            needArrangePopup = true;
                            this._panelRange.Visibility = Visibility.Visible;
                        }
                    }
                }

                if (!isDropDownOpen)
                {
                    needArrangePopup = false;

                    this.ClosePopup();
                }
                if (needArrangePopup)
                {
                    this.ArrangePopup();
                }

                this.DisplayText();
            }
        }

        void _buttonDone_Click(object sender, RoutedEventArgs e)
        {
            this.SetDateByCalendar();
        }


        public void ClosePopup()
        {
            this.ArrangePopup();

            this._popup.IsOpen = false;
        }

        private static void SetSelectedDate(DateRange dateRange, string type)
        {
            DateTime dateTimeNow = DateTime.Now;

            if (type == RangeType_Today_Code)
            {
                dateRange.SelectedDateStart = dateTimeNow.Date;
                dateRange.SelectedDateEnd = dateTimeNow.Date.AddDays(1).AddSeconds(-1);
            }
            else if (type == RangeType_Last3days_Code)
            {
                dateRange.SelectedDateStart = dateTimeNow.Date.AddDays(-3);
                dateRange.SelectedDateEnd = dateTimeNow.Date.AddDays(1).AddSeconds(-1);
            }
            else if (type == RangeType_Last7days_Code)
            {
                dateRange.SelectedDateStart = dateTimeNow.Date.AddDays(-7);
                dateRange.SelectedDateEnd = dateTimeNow.Date.AddDays(1).AddSeconds(-1);
            }
            else if (type == RangeType_Last30days_Code)
            {
                dateRange.SelectedDateStart = dateTimeNow.Date.AddDays(-30);
                dateRange.SelectedDateEnd = dateTimeNow.Date.AddDays(1).AddSeconds(-1);
            }
            else if (type == RangeType_ClearDays_Code)
            {
                dateRange.SelectedDateStart = null;
                dateRange.SelectedDateEnd = null;
            }
        }

        private static void SelectItemByTag(ListBox listbox, string type)
        {
            if (listbox != null)
            {
                int selectIndex = -1;

                if (type != null)
                {
                    for (int i = 0; i < listbox.Items.Count; i++)
                    {
                        ListBoxItem li = listbox.Items[i] as ListBoxItem;

                        if (li != null)
                        {
                            string itemTag = li.Tag as string;
                            if (itemTag == type)
                            {
                                selectIndex = i;
                            }
                        }
                    }
                }

                listbox.SelectedIndex = selectIndex;
            }
        }

        private void SetDateByCalendar()
        {
            if (this._listbox != null)
            {
                ListBoxItem selectedItem = this._listbox.SelectedItem as ListBoxItem;
                if (selectedItem != null)
                {
                    string itemTag = selectedItem.Tag as string;

                    DateTime? dateTimeSimple = null;
                    DateTime? dateTimeFrom = null;
                    DateTime? dateTimeTo = null;
                    if (this._calendarSimple != null && this._calendarSimple.SelectedDate != null)
                    {
                        dateTimeSimple = this._calendarSimple.SelectedDate.Value.Date;
                    }
                    if (this._calendarStart != null && this._calendarStart.SelectedDate != null)
                    {
                        dateTimeFrom = this._calendarStart.SelectedDate.Value.Date;
                    }
                    if (this._calendarEnd != null && this._calendarEnd.SelectedDate != null)
                    {
                        dateTimeTo = this._calendarEnd.SelectedDate.Value.Date;
                    }

                    if (itemTag == RangeType_SpecificDate_Code)
                    {
                        if (dateTimeSimple != null)
                        {
                            this.SelectedDateStart = dateTimeSimple.Value;
                            this.SelectedDateEnd = dateTimeSimple.Value.AddDays(1);
                        }
                        else
                        {
                            this.SelectedDateStart = null;
                            this.SelectedDateEnd = null;
                        }
                    }
                    else if (itemTag == RangeType_BeforeDate_Code)
                    {
                        this.SelectedDateStart = null;
                        this.SelectedDateEnd = dateTimeSimple;
                    }
                    else if (itemTag == RangeType_AfterDate_Code)
                    {
                        this.SelectedDateStart = dateTimeSimple;
                        this.SelectedDateEnd = null;
                    }
                    else if (itemTag == RangeType_RangeDate_Code)
                    {
                        if (dateTimeTo != null)
                        {
                            dateTimeTo = dateTimeTo.Value.AddDays(1);
                        }
                        this.SelectedDateStart = dateTimeFrom;
                        this.SelectedDateEnd = dateTimeTo;
                    }

                    this.SelectedRangeType = rangeTypeDic.FirstOrDefault(p => p.Value == itemTag).Key;

                    this.ClosePopup();
                    this.DisplayText();
                }
            }
        }

        private void DisplayText()
        {
            if (this._listbox != null && this._textBox != null)
            {
                string displayText = String.Empty; // MessageResource.DateRange_RangeType_ClearDays_Text;

                string selectedItemTag = rangeTypeDic.FirstOrDefault(p => p.Key == this.SelectedRangeType).Value;

                if (selectedItemTag == RangeType_Today_Code)
                {
                    displayText = MessageResource.DateRange_RangeType_Today_Text;
                }
                else if (selectedItemTag == RangeType_Last3days_Code)
                {
                    displayText = MessageResource.DateRange_RangeType_Last3days_Text;
                }
                else if (selectedItemTag == RangeType_Last7days_Code)
                {
                    displayText = MessageResource.DateRange_RangeType_Last7days_Text;
                }
                else if (selectedItemTag == RangeType_Last30days_Code)
                {
                    displayText = MessageResource.DateRange_RangeType_Last30days_Text;
                }
                else if (selectedItemTag == RangeType_SpecificDate_Code)
                {
                    displayText = String.Format(this.DisplaySpecificDateFormat, this.SelectedDateStart);
                }
                else if (selectedItemTag == RangeType_BeforeDate_Code)
                {
                    displayText = String.Format(this.DisplayBeforeDateFormat, this.SelectedDateEnd);
                }
                else if (selectedItemTag == RangeType_AfterDate_Code)
                {
                    displayText = String.Format(this.DisplayAfterDateFormat, this.SelectedDateStart);
                }
                else if (selectedItemTag == RangeType_RangeDate_Code)
                {                  
                    if (this.SelectedDateStart != null && this.SelectedDateEnd != null)
                    {
                        displayText = String.Format(this.DisplayRangeDateFormat, this.SelectedDateStart, this.SelectedDateEnd.Value);

                    }
                    else if (this.SelectedDateStart != null && this.SelectedDateEnd == null)
                    {
                        displayText = String.Format(this.DisplayAfterDateFormat, this.SelectedDateStart);
                    }
                    else if (this.SelectedDateStart == null && this.SelectedDateEnd != null)
                    {
                        displayText = String.Format(this.DisplayBeforeDateFormat, this.SelectedDateEnd);
                    } 
                }

                this._textBox.Text = displayText; // + String.Format(" [{0: yyyy-MM-dd hh:mm:ss} - {1: yyyy-MM-dd hh:mm:ss}]", this.SelectedDateStart, this.SelectedDateEnd);
            }
        }

        private void ArrangePopup()
        {
            if (((this._popup != null) && (this._popupRoot != null)) && this._popup.IsOpen)
            {
                System.Windows.Interop.Content content = Application.Current.Host.Content;
                double actualWidth = content.ActualWidth;
                double actualHeight = content.ActualHeight;
                double num3 = this._popupRoot.ActualWidth;
                double num4 = this._popupRoot.ActualHeight;
                if (((actualHeight != 0.0) && (actualWidth != 0.0)) && ((num3 != 0.0) && (num4 != 0.0)))
                {
                    GeneralTransform transform = null;
                    try
                    {
                        transform = this._popupRoot.TransformToVisual(null);
                    }
                    catch
                    {
                        //Don't need to do nothing.
                    }

                    if (transform != null)
                    {
                        Point point = new Point(0.0, 0.0);
                        Point point2 = new Point(1.0, 0.0);
                        Point point3 = new Point(0.0, 1.0);
                        Point point4 = transform.Transform(point);
                        Point point5 = transform.Transform(point2);
                        Point point6 = transform.Transform(point3);
                        double x = point4.X;
                        double y = point4.Y;
                        double num7 = base.ActualHeight;
                        double num8 = base.ActualWidth;

                        num3 = Math.Min(num3, actualWidth);
                        num4 = Math.Min(num4, actualHeight);
                        num3 = Math.Max(num8, num3);
                        double num10 = x;
                        if (actualWidth <= (num10 + num3))
                        {
                            num10 = actualWidth - num3;
                            num10 = Math.Max(0.0, num10);
                        }
                        double num11 = y + num7;
                        if (actualHeight <= (num11 + num4))
                        {
                            num11 = y - num4;
                            if (num11 < 0.0)
                            {
                                if (y < ((actualHeight - num7) / 2.0))
                                {
                                    num11 = y + num7;
                                }
                                else
                                {
                                    num11 = y - num4;
                                }
                            }
                        }
                        this._popup.HorizontalOffset = 0.0;
                        this._popup.VerticalOffset = 0.0;
                        this._canvasPopupOutside.Width = actualWidth;
                        this._canvasPopupOutside.Height = actualHeight;
                        Matrix identity = Matrix.Identity;
                        identity.M11 = point5.X - point4.X;
                        identity.M12 = point5.Y - point4.Y;
                        identity.M21 = point6.X - point4.X;
                        identity.M22 = point6.Y - point4.Y;
                        identity.OffsetX -= point4.X;
                        identity.OffsetY -= point4.Y;
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = identity;
                        this._canvasPopupOutside.RenderTransform = transform2;
                        this._popupRoot.MinWidth = num8;
                        this._popupRoot.MaxWidth = actualWidth;
                        this._popupRoot.MinHeight = num7;
                        this._popupRoot.MaxHeight = actualHeight;
                        this._popupRoot.HorizontalAlignment = HorizontalAlignment.Left;
                        this._popupRoot.VerticalAlignment = VerticalAlignment.Top;

                        Canvas.SetLeft(this._popupRoot, num10 - x);
                        Canvas.SetTop(this._popupRoot, num11 - y);
                    }
                }
            }
        }
    }
}
