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
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class AdvancedTextBox : System.Windows.Controls.TextBox
    {
        private static readonly string s_icon_normal = "/Themes/Default/Images/TextBox/clear_normal.png";
        private static readonly string s_icon_hover = "/Themes/Default/Images/TextBox/clear_hover.png";

        private Image m_clearIcon;

        public AdvancedTextBox()
        {
            this.DefaultStyleKey = typeof(AdvancedTextBox);
            this.TextChanged += new TextChangedEventHandler(AdvancedTextBox_TextChanged);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.m_clearIcon != null)
            {
                this.m_clearIcon.MouseLeftButtonDown -= new MouseButtonEventHandler(m_clearIcon_MouseLeftButtonDown);
                this.m_clearIcon.MouseEnter -= new MouseEventHandler(m_clearIcon_MouseEnter);
                this.m_clearIcon.MouseLeave -= new MouseEventHandler(m_clearIcon_MouseLeave);
            }
            else
            {
                this.m_clearIcon = base.GetTemplateChild("ClearIcon") as Image;
                this.m_clearIcon.Source = new BitmapImage(new Uri(s_icon_normal, UriKind.Relative));
            }
            this.m_clearIcon.MouseLeftButtonDown += new MouseButtonEventHandler(m_clearIcon_MouseLeftButtonDown);
            this.m_clearIcon.MouseEnter += new MouseEventHandler(m_clearIcon_MouseEnter);
            this.m_clearIcon.MouseLeave += new MouseEventHandler(m_clearIcon_MouseLeave);

            this.OnValueChanged();
        }

        #region Events

        void AdvancedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.OnValueChanged();
        }

        void m_clearIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            this.m_clearIcon.Source = new BitmapImage(new Uri(s_icon_normal, UriKind.Relative));
        }

        void m_clearIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            this.m_clearIcon.Source = new BitmapImage(new Uri(s_icon_hover, UriKind.Relative));
        }

        void m_clearIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Text = string.Empty;
            e.Handled = true;
        }

        #endregion

        #region Private Methods

        private void OnValueChanged()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                if (!string.IsNullOrWhiteSpace(this.Text))
                {
                    if (this.m_clearIcon.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        this.m_clearIcon.Source = new BitmapImage(new Uri(s_icon_normal, UriKind.Relative));
                        this.m_clearIcon.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    if (this.m_clearIcon.Visibility == System.Windows.Visibility.Visible)
                    {
                        this.m_clearIcon.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        #endregion
    }
}
