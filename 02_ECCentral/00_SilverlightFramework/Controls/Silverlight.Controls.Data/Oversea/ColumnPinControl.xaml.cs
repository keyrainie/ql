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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public partial class ColumnPinControl : UserControl, INotifyPropertyChanged
    {
        private static readonly string s_pinnedIcon = "/Themes/Default/Images/Grid/pin_normal.png";
        private static readonly string s_unPinnedIcon = "/Themes/Default/Images/Grid/unPin_normal.png";
        private static readonly string s_pinnedIcon_hover = "/Themes/Default/Images/Grid/pin_hover.png";
        private static readonly string s_unPinnedIcon_hover = "/Themes/Default/Images/Grid/unPin_hover.png";

        private bool m_isPinned;
        private string m_icon;

        public DataGridColumn Column
        {
            get;
            set;
        }

        public Newegg.Oversea.Silverlight.Controls.Data.DataGrid OwningGrid
        {
            get;
            set;
        }


        public bool IsPinned
        {
            get
            {
                return this.m_isPinned;
            }
            set
            {
                this.m_isPinned = value;
                this.OnPropertyChanged("IsPinned");
            }
        }

        public string Icon
        {
            get
            {
                return this.m_icon;
            }
            set
            {
                this.m_icon = value;
                this.OnPropertyChanged("Icon");
            }
        }

        public ColumnPinControl()
        {
            InitializeComponent();
            this.PropertyChanged += new PropertyChangedEventHandler(ColumnPinControl_PropertyChanged);

            this.Reset();
            this.DataContext = this;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        void ICON_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsPinned = !this.IsPinned;
            this.Reset();

            e.Handled = true;
        }

        private void ICON_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsPinned)
            {
                this.Icon = s_pinnedIcon_hover;
            }
            else
            {
                this.Icon = s_unPinnedIcon_hover;
            }
        }

        private void ICON_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsPinned)
            {
                this.Icon = s_pinnedIcon;
            }
            else
            {
                this.Icon = s_unPinnedIcon;
            }
        }

        void ColumnPinControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsPinned")
            {
                if (this.IsPinned)
                {
                    int frozenCount = this.OwningGrid.FrozenColumnCount;
                    if (this.Column.DisplayIndex >= this.OwningGrid.FrozenColumnCount)
                    {
                        frozenCount++;
                    }
                    this.Column.DisplayIndex = 0;
                    this.OwningGrid.FrozenColumnCount = frozenCount;
                }
                else
                {
                    this.Column.DisplayIndex = this.OwningGrid.FrozenColumnCount - 1;
                    this.OwningGrid.FrozenColumnCount = this.OwningGrid.FrozenColumnCount - 1;
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        private void Reset()
        {
            if (this.IsPinned)
            {
                this.Icon = s_pinnedIcon;
                ToolTipService.SetToolTip(ICON, Resource.Column_Unpin);
            }
            else
            {
                this.Icon = s_unPinnedIcon;
                ToolTipService.SetToolTip(ICON, Resource.Column_Pin);
            }
        }


    }
}
