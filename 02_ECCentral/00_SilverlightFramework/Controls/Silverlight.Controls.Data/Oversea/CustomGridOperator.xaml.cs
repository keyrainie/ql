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
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.Controls.Primitives;

namespace Newegg.Oversea.Silverlight.Controls.Data.Oversea
{
    public partial class CustomGridOperator : UserControl
    {
        #region Fields
        private ObservableCollection<ICustomGridListItem> m_Source = null;
        #endregion

        #region Constructor
        public CustomGridOperator()
        {
            InitializeComponent();
        }
        #endregion

        #region Dependency Property

        public CustomDataGridDialog Dialog { get; set; }


        public static readonly DependencyProperty ListSourceProperty = DependencyProperty.Register(
            "ListSource",
            typeof(ObservableCollection<ICustomGridListItem>),
            typeof(CustomGridOperator),
            new PropertyMetadata(null));

        public ObservableCollection<ICustomGridListItem> ListSource
        {
            get { return (ObservableCollection<ICustomGridListItem>)GetValue(ListSourceProperty); }
            set
            {
                SetValue(ListSourceProperty, value);
                this.m_Source = value;
                this.MainContentList.ItemsSource = m_Source;
            }
        }
        #endregion

        #region Private Functions
        private void NewListModeSwitch(bool isInputMode)
        {
            textBlockTitle.Visibility = isInputMode ? Visibility.Collapsed : Visibility.Visible;
            textBoxNewValue.Visibility = isInputMode ? Visibility.Visible : Visibility.Collapsed;
            btnNewSave.Visibility = isInputMode ? Visibility.Visible : Visibility.Collapsed;
            btnNewCancel.Visibility = isInputMode ? Visibility.Visible : Visibility.Collapsed;
            imageAdd.Visibility = isInputMode ? Visibility.Collapsed : Visibility.Visible;

            if (isInputMode)
            {
                textBoxNewValue.Focus();
                textBoxNewValue.SelectAll();
            }
            else
                textBoxNewValue.Text = string.Empty;
        }

        private SolidColorBrush GetTextStyle(bool isDelete)
        {
            Color c = new Color();

            if (isDelete)
            {
                c.B = 204;
                c.G = 204;
                c.R = 204;
            }
            else
            {
                c.B = 0;
                c.G = 0;
                c.R = 0;
            }
            c.A = 255;

            SolidColorBrush scb = new SolidColorBrush(c);
            return scb;
        }
        #endregion

        #region Event Handle
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            NewListModeSwitch(false);
        }
        private void textBlockTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewListModeSwitch(true);
        }
        private void borderTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Dialog.SaveProfileData();
            var content = (sender as Border).DataContext as ICustomGridListItem;
            if (content != null)
            {
                Dialog.SetDefault(content.DisplayContent);
                var config = Dialog.OwningGrid.GridSettings.FirstOrDefault(p => p.Name == content.DisplayContent);
                if (config != null)
                {
                    Dialog.RefreshDefaultProfileData();
                    Dialog.popupButton.Close();
                }
            }
        }

        private void btnNewCancel_Click(object sender, RoutedEventArgs e)
        {
            NewListModeSwitch(false);
            TextBlockErrorTip1.Visibility = Visibility.Collapsed;
        }

        private void btnNewSave_Click(object sender, RoutedEventArgs e)
        {
            this.TextBlockErrorTip1.Text = string.Empty;
            TextBlockErrorTip1.Visibility = Visibility.Visible;
            if (string.IsNullOrEmpty(this.textBoxNewValue.Text))
            {
                this.TextBlockErrorTip1.Text = Resource.ProflieDataGrid_CanNotBeEmptyTip;
                return;
            }
            else
            {
                var result = this.Dialog.OwningGrid.GridSettings.FirstOrDefault(p => p.Name.ToLower() == this.textBoxNewValue.Text.ToLower());
                if (result != null)
                {
                    this.TextBlockErrorTip1.Text = Resource.ProflieDataGrid_ExistsTip;
                    return;
                }
            }

            this.Dialog.SaveProfileData();

            var config = Utilities.UtilityHelper.DeepClone<GridSetting>(Dialog.OwningGrid.m_resetGridConfig);
            config.GridGuid = this.Dialog.OwningGrid.GridID;
            config.Name = this.textBoxNewValue.Text;
            this.Dialog.OwningGrid.GridSettings.Add(config);

            var list = this.ListSource as ObservableCollection<ICustomGridListItem>;
            list.Insert(0, new CustomProfileItem { DisplayContent = this.textBoxNewValue.Text });

            this.MainContentList.SelectedIndex = 0;
            this.Dialog.SetDefault(this.textBoxNewValue.Text);
            this.Dialog.RefreshDefaultProfileData();
            this.Dialog.popupButton.Close();
            TextBlockErrorTip1.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion

        private void gridDataTemplate_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid g = (sender as Grid);
            ICustomGridListItem content = g.DataContext as ICustomGridListItem;

            if (content.IsRenaming == false && content.IsDeleting == false)
            {
                NoboarderIconButton btnDelete = g.FindName("btnDelete") as NoboarderIconButton;
                NoboarderIconButton btnRename = g.FindName("btnRename") as NoboarderIconButton;

                btnDelete.Visibility = Visibility.Visible;
                btnRename.Visibility = Visibility.Visible;
            }
        }

        private void gridDataTemplate_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid g = (sender as Grid);
            ICustomGridListItem content = g.DataContext as ICustomGridListItem;

            if (content.IsRenaming == false && content.IsDeleting == false)
            {
                TextBlock textBlockTitle = g.FindName("textBlockTitle") as TextBlock;
                TextBox textBoxNewValue = g.FindName("textBoxNewValue") as TextBox;
                NoboarderIconButton btnDelete = g.FindName("btnDelete") as NoboarderIconButton;
                NoboarderIconButton btnRename = g.FindName("btnRename") as NoboarderIconButton;
                NoboarderIconButton btnSave = g.FindName("btnSave") as NoboarderIconButton;
                NoboarderIconButton btnCancel = g.FindName("btnCancel") as NoboarderIconButton;

                textBlockTitle.Visibility = Visibility.Visible;
                textBoxNewValue.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnRename.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
                btnCancel.Visibility = Visibility.Collapsed;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            NoboarderIconButton btn = sender as NoboarderIconButton;
            Grid g = VisualTreeHelper.GetParent(btn) as Grid;

            TextBlock textBlockTitle = g.FindName("textBlockTitle") as TextBlock;
            TextBox textBoxNewValue = g.FindName("textBoxNewValue") as TextBox;
            NoboarderIconButton btnDelete = g.FindName("btnDelete") as NoboarderIconButton;
            NoboarderIconButton btnRename = g.FindName("btnRename") as NoboarderIconButton;
            NoboarderIconButton btnSave = g.FindName("btnSave") as NoboarderIconButton;
            NoboarderIconButton btnCancel = g.FindName("btnCancel") as NoboarderIconButton;

            textBlockTitle.Foreground = GetTextStyle(true);
            textBlockTitle.Visibility = Visibility.Visible;
            textBoxNewValue.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            btnRename.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Visible;

            // Mark
            ICustomGridListItem content = g.DataContext as ICustomGridListItem;
            content.IsDeleting = true;
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            NoboarderIconButton btn = sender as NoboarderIconButton;
            Grid g = VisualTreeHelper.GetParent(btn) as Grid;

            TextBlock textBlockTitle = g.FindName("textBlockTitle") as TextBlock;
            TextBox textBoxNewValue = g.FindName("textBoxNewValue") as TextBox;
            NoboarderIconButton btnDelete = g.FindName("btnDelete") as NoboarderIconButton;
            NoboarderIconButton btnRename = g.FindName("btnRename") as NoboarderIconButton;
            NoboarderIconButton btnSave = g.FindName("btnSave") as NoboarderIconButton;
            NoboarderIconButton btnCancel = g.FindName("btnCancel") as NoboarderIconButton;

            textBlockTitle.Visibility = Visibility.Collapsed;
            textBoxNewValue.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Collapsed;
            btnRename.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;

            textBoxNewValue.Text = textBlockTitle.Text;
            textBoxNewValue.SelectAll();
            textBoxNewValue.Focus();

            // Mark
            ICustomGridListItem content = g.DataContext as ICustomGridListItem;
            content.IsRenaming = true;
        }

        private void btnSetDefault_Click(object sender, RoutedEventArgs e)
        {
            // Remove Old
            foreach (ICustomGridListItem item in m_Source)
            {
                item.DefaultIconVisibility = Visibility.Collapsed;
            }
            this.MainContentList.ItemsSource = null;
            this.MainContentList.ItemsSource = m_Source;

            // Set
            NoboarderIconButton btn = sender as NoboarderIconButton;
            Grid g = VisualTreeHelper.GetParent(btn) as Grid;

            Image imageDefaultIcon = g.FindName("imageDefaultIcon") as Image;
            imageDefaultIcon.Visibility = Visibility.Visible;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
          
            NoboarderIconButton btn = sender as NoboarderIconButton;
            Grid g = VisualTreeHelper.GetParent(btn) as Grid;

            TextBlock textBlockTitle = g.FindName("textBlockTitle") as TextBlock;
            TextBlock TextBlockErrorTip1 = g.FindName("TextBlockErrorTip1") as TextBlock;
            TextBlockErrorTip1.Visibility = Visibility.Visible;
            TextBox textBoxNewValue = g.FindName("textBoxNewValue") as TextBox;
            NoboarderIconButton btnDelete = g.FindName("btnDelete") as NoboarderIconButton;
            NoboarderIconButton btnRename = g.FindName("btnRename") as NoboarderIconButton;
            NoboarderIconButton btnSave = g.FindName("btnSave") as NoboarderIconButton;
            NoboarderIconButton btnCancel = g.FindName("btnCancel") as NoboarderIconButton;

            TextBlockErrorTip1.Text = string.Empty;
            if (string.IsNullOrEmpty(textBoxNewValue.Text))
            {
                TextBlockErrorTip1.Text = Resource.ProflieDataGrid_CanNotBeEmptyTip;
                return;
            }
            else
            {
                var result = this.Dialog.OwningGrid.GridSettings.FirstOrDefault(p => p.Name.ToLower() == textBoxNewValue.Text.ToLower());
                if (result != null && result.Name != textBlockTitle.Text)
                {
                    TextBlockErrorTip1.Text = Resource.ProflieDataGrid_ExistsTip;
                    return;
                }
            }

            this.Dialog.Rename(textBlockTitle.Text, textBoxNewValue.Text.Trim());

            if (this.Dialog.TextBlockContent.Text == textBlockTitle.Text)
            {
                this.Dialog.TextBlockContent.Text = textBoxNewValue.Text.Trim();
                this.Dialog.TextBlockNeedCoverProfileName.Text = textBoxNewValue.Text.Trim();
            }
            TextBlockErrorTip1.Visibility = Visibility.Collapsed;
            textBlockTitle.Text = textBoxNewValue.Text.Trim();
            (g.DataContext as ICustomGridListItem).DisplayContent = textBlockTitle.Text;
            textBlockTitle.Foreground = GetTextStyle(false);
            textBlockTitle.Visibility = Visibility.Visible;
            textBoxNewValue.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Visible;
            btnRename.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;

            // Mark
            ICustomGridListItem content = g.DataContext as ICustomGridListItem;
            content.IsRenaming = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NoboarderIconButton btn = sender as NoboarderIconButton;
            Grid g = VisualTreeHelper.GetParent(btn) as Grid;
            TextBlock textBlockTitle = g.FindName("textBlockTitle") as TextBlock;
            TextBox textBoxNewValue = g.FindName("textBoxNewValue") as TextBox;
            NoboarderIconButton btnDelete = g.FindName("btnDelete") as NoboarderIconButton;
            NoboarderIconButton btnRename = g.FindName("btnRename") as NoboarderIconButton;
            NoboarderIconButton btnSave = g.FindName("btnSave") as NoboarderIconButton;
            NoboarderIconButton btnCancel = g.FindName("btnCancel") as NoboarderIconButton;
            TextBlock TextBlockErrorTip1 = g.FindName("TextBlockErrorTip1") as TextBlock;

            textBoxNewValue.Text = textBlockTitle.Text;

            TextBlockErrorTip1.Visibility = Visibility.Collapsed;
            textBlockTitle.Foreground = GetTextStyle(false);
            textBlockTitle.Visibility = Visibility.Visible;
            textBoxNewValue.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Visible;
            btnRename.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;

            // Mark
            ICustomGridListItem content = g.DataContext as ICustomGridListItem;
            content.IsRenaming = false;
            content.IsDeleting = false;
        }

        private void LayoutRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            // Delete
        }
    }
}