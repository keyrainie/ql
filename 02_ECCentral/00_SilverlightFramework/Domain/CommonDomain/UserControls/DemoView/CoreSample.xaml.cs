using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media.Animation;

using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Windows.Data;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Primitives;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.IO;
using Newegg.Oversea.Silverlight.Utilities;

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{
    public partial class CoreSample : UserControl
    {
        public class TestClass : ModelBase
        {
            private int m_id;
            [Integer]
            public int ID 
            {
                get 
                {
                    return m_id;
                }
                set 
                {
                    base.SetValue("ID", ref this.m_id, value);
                }
            }
        }

        public CoreSample()
        {
            InitializeComponent();
            TextBoxError.DataContext = new TestClass();
            TextBoxError.Text = "Flase";
            CheckBoxError.DataContext = new TestClass();
            CheckBoxError.Content = "Invalid";
            RadioButtonError.DataContext = new TestClass();
            RadioButtonError.Content = "Invalid";
            ComboBoxError.DataContext = new TestClass();
            ComboBoxError.SelectedIndex = 0;

            RichTextBox1.Xaml = "<Section xml:space=\"preserve\" HasTrailingParagraphBreakOnPaste=\"False\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph FontSize=\"12\" FontFamily=\"Tahoma,SimSun,PMingLiU\" Foreground=\"#FF000000\" FontWeight=\"Normal\" FontStyle=\"Normal\" FontStretch=\"Normal\" TextAlignment=\"Left\"><Hyperlink Foreground=\"#FF337CBB\" TextDecorations=\"Underline\" NavigateUri=\"http://www.baidu.com/\" MouseOverForeground=\"#FFED6E00\"><Run Text=\"Test\" /></Hyperlink></Paragraph></Section>";
            RichTextBox2.Xaml = "<Section xml:space=\"preserve\" HasTrailingParagraphBreakOnPaste=\"False\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph FontSize=\"12\" FontFamily=\"Tahoma,SimSun,PMingLiU\" Foreground=\"#FF000000\" FontWeight=\"Normal\" FontStyle=\"Normal\" FontStretch=\"Normal\" TextAlignment=\"Left\"><Hyperlink Foreground=\"#FF337CBB\" TextDecorations=\"Underline\" NavigateUri=\"http://www.baidu.com/\" MouseOverForeground=\"#FFED6E00\"><Run Text=\"Test\" /></Hyperlink></Paragraph></Section>";
            RichTextBox3.Xaml = "<Section xml:space=\"preserve\" HasTrailingParagraphBreakOnPaste=\"False\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph FontSize=\"12\" FontFamily=\"Tahoma,SimSun,PMingLiU\" Foreground=\"#FF000000\" FontWeight=\"Normal\" FontStyle=\"Normal\" FontStretch=\"Normal\" TextAlignment=\"Left\"><Hyperlink Foreground=\"#FF337CBB\" TextDecorations=\"Underline\" NavigateUri=\"http://www.baidu.com/\" MouseOverForeground=\"#FFED6E00\"><Run Text=\"Test\" /></Hyperlink></Paragraph></Section>";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                var profile = ComponentFactory.GetComponent<IUserProfile>();
                List<GridSetting> settings = null;
                settings = profile.Get<List<GridSetting>>(GridKeys.KEY_UP_GRIDSETTINGS);
                Stream stream = sfd.OpenFile();
                StreamWriter sw = new StreamWriter(stream);
                sw.Write(UtilityHelper.XmlSerialize(settings));
                sw.Dispose();
                stream.Dispose();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                var profile = ComponentFactory.GetComponent<IUserProfile>();
                List<GridSetting> settings = null;
                Stream stream = ofd.File.OpenRead();
                StreamReader sw = new StreamReader(stream);
                settings = UtilityHelper.XmlDeserialize<List<GridSetting>>(sw.ReadToEnd());
                profile.Set(GridKeys.KEY_UP_GRIDSETTINGS, settings);
            }
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //System.Diagnostics.Debug.WriteLine("Double Click");
                MessageBox.Show("You click twice");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                var profile = ComponentFactory.GetComponent<IUserProfile>();
                List<Setting> settings = null;
                settings = profile.Get<List<Setting>>("DataGrid_UserSetting");
                Stream stream = sfd.OpenFile();
                StreamWriter sw = new StreamWriter(stream);
                sw.Write(UtilityHelper.XmlSerialize(settings));
                sw.Dispose();
                stream.Dispose();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                var profile = ComponentFactory.GetComponent<IUserProfile>();
                List<Setting> settings = null;
                Stream stream = ofd.File.OpenRead();
                StreamReader sw = new StreamReader(stream);
                settings = UtilityHelper.XmlDeserialize<List<Setting>>(sw.ReadToEnd());
                profile.Set("DataGrid_UserSetting", settings);
            }
        }
    }
}
