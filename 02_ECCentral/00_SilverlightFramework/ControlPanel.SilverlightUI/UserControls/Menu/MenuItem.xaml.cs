using System;
using System.Windows.Media;
using System.Windows.Controls;

using System.Windows;


namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Menu
{
    public partial class MenuItem : UserControl
    {
        public MenuItem()
        {
            InitializeComponent();
            IsSelected = false;
        }

        public MenuControl Menu
        {
            get;
            set;
        }

        private bool m_isLevel2;
        public bool IsLevel2
        {
            get
            {
                return m_isLevel2;
            }
            set
            {
                m_isLevel2 = value;
                SetItemSelected(m_isSelected);
            }
        }

        public object ParentNode
        {
            get;
            set;
        }

        private bool m_isSelected;
        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                m_isSelected = value;
                SetItemSelected(value);
            }
        }

        public string Text
        {
            get
            {
                return TextBlockContent.Text;
            }
            set
            {
                TextBlockContent.Text = value;
            }
        }

        private void SetItemSelected(bool isSelected)
        {
            if (isSelected)
            {
                if (m_isLevel2)
                {
                    LayoutRoot.Margin = new Thickness(0, 0, -7, 0);
                    TextBlockContent.Margin = new Thickness(15, 0, 10, 0);
                }
                else
                {
                    LayoutRoot.Margin = new Thickness(0, 6, -7, 6);
                    TextBlockContent.Margin = new Thickness(15, 0, 10, 0);
                }
                PathBackground.Visibility = Visibility.Visible;
                TextBlockContent.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                if (m_isLevel2)
                {
                    LayoutRoot.Margin = new Thickness(0);
                    TextBlockContent.Margin = new Thickness(15, 4, 10, 4);
                }
                else
                {
                    LayoutRoot.Margin = new Thickness(0);
                    TextBlockContent.Margin = new Thickness(15, 10, 10, 10);
                }

                PathBackground.Visibility = Visibility.Collapsed;
                TextBlockContent.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

    }
}
