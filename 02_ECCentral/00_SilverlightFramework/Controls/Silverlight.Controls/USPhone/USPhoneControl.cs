using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls.Resources;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class USPhoneControl : ContentControl
    {
        private TextBox m_frstTextBox;
        private TextBox m_secondTextBox;
        private TextBox m_thirdTextBox;
        private TextBox m_extTextBox;
        private Grid m_rootGrid;

        private readonly ObservableCollection<ValidationError> m_errors = new ObservableCollection<ValidationError>();
        private readonly ObservableCollection<Control> m_errorControls = new ObservableCollection<Control>();

        public Phone ViewModel { get; private set; }

        public USPhoneControl()
        {
            DefaultStyleKey = typeof(USPhoneControl);               
        }

        public bool IsRequired { get; set; }
        
        public string PhoneNumber
        {
            get { return (string)GetValue(PhoneNumberProperty); }
            set { SetValue(PhoneNumberProperty, value); }
        }

        public static readonly DependencyProperty PhoneNumberProperty =
            DependencyProperty.Register("PhoneNumber", typeof(string), typeof(USPhoneControl), new PropertyMetadata(OnPhoneNumberPropertyChanged));

        static void OnPhoneNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }
            var phoneNumber = e.NewValue.ToString();
            var phoneControl = d as USPhoneControl;
            if (phoneControl.m_frstTextBox != null)
            {
                var array = phoneNumber.Split('-');
                if (array.Length > 0 && array.Length == 3)
                {
                    phoneControl.m_frstTextBox.Text = array[0];
                    phoneControl.m_secondTextBox.Text = array[1];
                    if (array[2].Contains("EXT"))
                    {
                        phoneControl.m_thirdTextBox.Text = array[2].Substring(0, 4);
                        phoneControl.m_extTextBox.Text = array[2].Substring(7);
                    }
                    else
                    {
                        phoneControl.m_thirdTextBox.Text = array[2];
                        phoneControl.m_extTextBox.Text = "";
                    }
                }
            }
        }

        void BindPhone(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var array = phoneNumber.Split('-');
                if (array.Length > 0 && array.Length == 3)
                {
                    m_frstTextBox.Text = array[0];
                    m_secondTextBox.Text = array[1];
                    if (array[2].Contains("EXT"))
                    {
                        m_thirdTextBox.Text = array[2].Substring(0, 4);
                        m_extTextBox.Text = array[2].Substring(7);
                    }
                    else
                    {
                        m_thirdTextBox.Text = array[2];
                        m_extTextBox.Text = "";
                    }
                }
            }
        }

        public bool Validate()
        {
            var first = m_frstTextBox.GetBindingExpression(TextBox.TextProperty);
            first.UpdateSource();

            var second = m_secondTextBox.GetBindingExpression(TextBox.TextProperty);
            second.UpdateSource();

            var third = m_thirdTextBox.GetBindingExpression(TextBox.TextProperty);
            third.UpdateSource();

            var ext = m_extTextBox.GetBindingExpression(TextBox.TextProperty);
            ext.UpdateSource();

            if (m_errors.Count > 0)
            {
                m_errorControls[0].Focus();
                this.SetValue(PhoneNumberProperty, "");
                return false;
            }

            var str = string.Format("{0}-{1}-{2}", this.ViewModel.FirstNumber, this.ViewModel.SecondNumber, this.ViewModel.ThirdNumber);
            if (!string.IsNullOrEmpty(this.ViewModel.ExtNumber))
            {
                str = str + "EXT" + this.ViewModel.ExtNumber;
            }

            this.SetValue(PhoneNumberProperty, str);

            return true;
        }   

        public override void OnApplyTemplate()
        {
            this.m_rootGrid = GetTemplateChild("Root") as Grid;
            this.m_rootGrid.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(m_rootGrid_BindingValidationError);
            this.m_frstTextBox = GetTemplateChild("FirtTextBox") as TextBox;
            this.m_secondTextBox = GetTemplateChild("SecondTextBox") as TextBox;
            this.m_thirdTextBox = GetTemplateChild("ThirdTextBox") as TextBox;
            this.m_extTextBox = GetTemplateChild("ExtTextBox") as TextBox;

            this.ViewModel = new Phone();
            this.ViewModel.IsRequired = this.IsRequired;
            this.m_frstTextBox.DataContext = this.ViewModel;
            this.m_secondTextBox.DataContext = this.ViewModel;
            this.m_thirdTextBox.DataContext = this.ViewModel;
            this.m_extTextBox.DataContext = this.ViewModel;

            var phone = this.GetValue(PhoneNumberProperty);
            if (phone != null)
            {
                BindPhone(phone.ToString());
            }

            base.OnApplyTemplate();
        }

        void m_rootGrid_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            var control = e.OriginalSource as Control;
            if (e.Action == ValidationErrorEventAction.Removed)
            {
                m_errorControls.Remove(control);
                m_errors.Remove(e.Error);
            }
            else if (e.Action == ValidationErrorEventAction.Added)
            {
                m_errorControls.Add(control);
                m_errors.Add(e.Error);
            }
        }
    }

    public class Phone : INotifyPropertyChanged
    {
        private string m_firstNumber;
        private string m_secondNumber;
        private string m_thirdNumber;
        private string m_extNumber;
        private const string IntegerRegex = @"^[0-9]\d*$";

        public bool IsRequired { get; set; }

        public string FirstNumber
        {
            get
            {
                return m_firstNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) && IsRequired)
                {
                    throw new Exception(MessageResource.PhoneControl_RequiredField);
                }
                if (!string.IsNullOrEmpty(value))
                {
                    var regex = new Regex(IntegerRegex);
                    if (!regex.IsMatch(value) || value.Trim().Length != 3)
                    {
                        throw new Exception(MessageResource.PhoneControl_InvalidNumber);
                    }
                }
                this.m_firstNumber = value;
                RaisePropertyChanged("FirstNumber");
            }
        }

        public string SecondNumber
        {
            get
            {
                return m_secondNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) && IsRequired)
                {
                    throw new Exception(MessageResource.PhoneControl_RequiredField);
                }
                if (!string.IsNullOrEmpty(value))
                {
                    var regex = new Regex(IntegerRegex);
                    if (!regex.IsMatch(value) || value.Trim().Length != 3)
                    {
                        throw new Exception(MessageResource.PhoneControl_InvalidNumber);
                    }
                }
                this.m_secondNumber = value;
                RaisePropertyChanged("SecondNumber");
            }
        }

        public string ThirdNumber
        {
            get
            {
                return m_thirdNumber;
            }
            set
            {
                if (string.IsNullOrEmpty(value) && IsRequired)
                {
                    throw new Exception(MessageResource.PhoneControl_RequiredField);
                }
                if (!string.IsNullOrEmpty(value))
                {
                    var regex = new Regex(IntegerRegex);
                    if (!regex.IsMatch(value) || value.Trim().Length != 4)
                    {
                        throw new Exception(MessageResource.PhoneControl_InvalidNumber);
                    }
                }
                this.m_thirdNumber = value;
                RaisePropertyChanged("ThirdNumber");
            }
        }

        public string ExtNumber
        {
            get
            {
                return m_extNumber;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var regex = new Regex(IntegerRegex);
                    if (!regex.IsMatch(value) || value.Trim().Length != 4)
                    {
                        throw new Exception(MessageResource.PhoneControl_InvalidNumber);
                    }
                }
                this.m_extNumber = value;
                RaisePropertyChanged("ExtNumber");
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
