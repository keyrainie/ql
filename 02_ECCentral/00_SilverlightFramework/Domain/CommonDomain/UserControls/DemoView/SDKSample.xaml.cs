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
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.CommonDomain.UserControls
{


    public partial class SDKSample : UserControl
    {        
        public SDKSample()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(SDKSample_Loaded);
           
            InitValidation();
        }

        private void InitValidation()
        {
            DataPickerError.DataContext = new TestClass();
            DataPickerError.Text = "Error";

            AutoCompleteBoxError.DataContext = new TestClass();

            AutoCompleteBoxError.Text = "";
        }

        void SDKSample_Loaded(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox1.ItemsSource = new string[]
                                                        {
                                                           "Microsoft Windows 7",
                                                           "Microsoft Expression Blend",
                                                           "Microsoft Silverlight",
                                                           "Silverlight Toolkit",
                                                           "Silverlight SDK"
                                                        };

           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("12312");
        }      
    }

    public class SO : ModelBase
    {
        private int m_soNumber;
        public int SONumber 
        {
            get
            {
                return m_soNumber;
            }
            set 
            {
                base.SetValue("SONumber", ref this.m_soNumber, value);
            }
        }

        public int CustomerNumber { get; set; }

        public DateTime SODate { get; set; }

        public string Status { get; set; }
    }

    public class TestClass: ModelBase
    {
        private int m_testData;

        [Integer]
        public int TestData
        {
            get { return m_testData; }
            set { SetValue("TestData", ref m_testData, value); }
        }
    }
}
