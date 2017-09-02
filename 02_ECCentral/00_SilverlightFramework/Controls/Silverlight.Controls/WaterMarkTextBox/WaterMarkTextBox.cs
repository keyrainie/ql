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

namespace Newegg.Oversea.Silverlight.Controls
{
    public class WaterMarkTextBox : TextBox
    {
        public static readonly DependencyProperty WaterMarkContentProperty = DependencyProperty.Register("WaterMarkContent", typeof(object), typeof(WaterMarkTextBox), null);

        private bool m_hasFocus = false;

        

        public WaterMarkTextBox()
        {
            DefaultStyleKey = typeof(WaterMarkTextBox);

            base.GotFocus += new RoutedEventHandler(WaterMarkTextBox_GotFocus);
            base.LostFocus += new RoutedEventHandler(WaterMarkTextBox_LostFocus);
            base.TextChanged += new TextChangedEventHandler(WaterMarkTextBox_TextChanged);
        }

        private const string Name_WaterMarkContentPresenter = "WaterMarkContentPresenter";
        private ContentPresenter WaterMarkContentPresenter { get; set; }
        private Image m_ICONActive;
        private Image m_ICONActive_Focus;
        private Image m_ICONNormal;

        public bool RightICON
        {
            get;
            set;
        }
        public event EventHandler ICONClick;

        public static readonly DependencyProperty ICONActiveProperty = DependencyProperty.Register("ICONActive", typeof(string), typeof(WaterMarkTextBox), null);
        public string ICONActive
        {
            get { return (string)GetValue(ICONActiveProperty); }
            set { SetValue(ICONActiveProperty, value); }
        }
        public static readonly DependencyProperty ICONNoramlProperty = DependencyProperty.Register("ICONNoraml", typeof(string), typeof(WaterMarkTextBox), null);
        public string ICONNormal
        {
            get { return (string)GetValue(ICONNoramlProperty); }
            set { SetValue(ICONNoramlProperty, value); }
        }
        public static readonly DependencyProperty InnerSpaceProperty = DependencyProperty.Register("InnerSpace", typeof(double), typeof(WaterMarkTextBox), null);
        public double InnerSpace
        {
            get { return (double)GetValue(InnerSpaceProperty); }
            set { SetValue(InnerSpaceProperty, value); }
        }


        public object WaterMarkContent
        {
            get { return (object)GetValue(WaterMarkContentProperty); }
            set { SetValue(WaterMarkContentProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.WaterMarkContentPresenter = GetTemplateChild(Name_WaterMarkContentPresenter) as ContentPresenter;
            if (this.m_ICONActive == null)
            {
                this.m_ICONActive = (Image)this.GetTemplateChild("ICONActive");

                if (ICONClick != null && !IsReadOnly && IsEnabled)
                {
                    this.m_ICONActive.Cursor = Cursors.Hand;
                    this.m_ICONActive.MouseLeftButtonDown += new MouseButtonEventHandler(ICON_MouseLeftButtonDown);
                }
            }
            if (this.m_ICONActive_Focus == null)
            {
                this.m_ICONActive_Focus = (Image)this.GetTemplateChild("ICONActive_Focus");

                if (ICONClick != null && !IsReadOnly && IsEnabled)
                {
                    this.m_ICONActive_Focus.Cursor = Cursors.Hand;
                    this.m_ICONActive_Focus.MouseLeftButtonDown += new MouseButtonEventHandler(ICON_MouseLeftButtonDown);
                }
            }
            if (this.m_ICONNormal == null)
            {
                this.m_ICONNormal = (Image)this.GetTemplateChild("ICONNormal");

                if (ICONClick != null && !IsReadOnly && IsEnabled)
                {
                    this.m_ICONNormal.Cursor = Cursors.Hand;
                    this.m_ICONNormal.MouseLeftButtonDown += new MouseButtonEventHandler(ICON_MouseLeftButtonDown);
                }
            }

            if (ICONActive != null && ICONActive.Trim().Length != 0 && ICONNormal != null && ICONNormal.Trim().Length != 0)
            {
                if (this.m_ICONActive != null)
                {
                    if (RightICON)
                    {
                        this.m_ICONActive.Margin = new Thickness(this.InnerSpace, 0, 0, 0);
                        this.m_ICONActive_Focus.Margin = new Thickness(this.InnerSpace, 0, 0, 0);
                        
                    }
                    else
                    {
                        this.m_ICONActive.Margin = new Thickness(0, 0, this.InnerSpace, 0);
                        this.m_ICONActive_Focus.Margin = new Thickness(0, 0, this.InnerSpace, 0);
                    }
                    
                }


                if (this.m_ICONNormal != null)
                {
                    if (RightICON)
                    {
                        this.m_ICONNormal.Margin = new Thickness(this.InnerSpace, 0, 0, 0);
                    }
                    else
                    {
                        this.m_ICONNormal.Margin = new Thickness(0, 0, this.InnerSpace, 0);
                    }
                }
            }
            else
            {
                if (this.m_ICONActive != null)
                {
                    this.m_ICONActive.Visibility = Visibility.Collapsed;
                    this.m_ICONActive_Focus.Visibility = Visibility.Collapsed;
                }


                if (this.m_ICONNormal != null)
                    this.m_ICONNormal.Visibility = Visibility.Collapsed;
            }

            if (RightICON)
            {
                var gridContent = (Grid)this.GetTemplateChild("GridContent");
                if (gridContent != null)
                {
                    gridContent.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                    gridContent.ColumnDefinitions[1].Width = GridLength.Auto;
                    m_ICONActive.SetValue(Grid.ColumnProperty, 1);
                    m_ICONNormal.SetValue(Grid.ColumnProperty, 1);
                    m_ICONActive_Focus.SetValue(Grid.ColumnProperty, 1);
                    var content1 = this.GetTemplateChild("ContentElement");
                    content1.SetValue(Grid.ColumnProperty, 0);
                    var content2 = this.GetTemplateChild("WaterMarkContentPresenterOuter");
                    content2.SetValue(Grid.ColumnProperty, 0);
                }
            }

            this.SetWaterMark();
        }

        void ICON_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ICONClick(this, new EventArgs());
        }

        void WaterMarkTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.m_hasFocus = true;

            this.SetWaterMark();
        }

        void WaterMarkTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.m_hasFocus = false;

            this.SetWaterMark();
        }

        void WaterMarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetWaterMark();
        }

        private void SetWaterMark()
        {
            if (this.WaterMarkContentPresenter != null)
            {
                if ((this.m_hasFocus && !this.IsReadOnly) || (this.Text != null && this.Text.Trim() != String.Empty))
                {
                    this.WaterMarkContentPresenter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.WaterMarkContentPresenter.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
