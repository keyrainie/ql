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
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using ControlPanel.SilverlightUI;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Views;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using System.Text.RegularExpressions;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls
{
    public partial class ForgetPassword : UserControl
    {
        private bool m_IsQuestionLoaded = false;    
        private RestClient m_Client = new RestClient("/Service/Framework/V50/RetrievalPasswordService.svc");

        public ForgetPassword()
        {
            InitializeComponent();

        }

        public void LoadQuestion()
        {
            if(!m_IsQuestionLoaded)
            {
                m_IsQuestionLoaded = true;
                
                btnSubmit.IsEnabled = false;
                m_Client.Query<List<Question>>("/Questions", (target, args) =>
                {
                    btnSubmit.IsEnabled = true;
                    comboBoxQuestion.ItemsSource = args.Result;
                    if (args.Result != null && args.Result.Count > 0)
                    {
                        comboBoxQuestion.SelectedIndex = 0;
                    }
                });
            }
        }

        private void linkBack_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as RootVisualWrapper).LoginArea.Visibility = Visibility.Visible;
            (Application.Current.RootVisual as RootVisualWrapper).ForgetPassword.Visibility = Visibility.Collapsed;

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtInfo.Text = "Email is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAnswer.Text))
            {
                txtInfo.Text = "Answer is required.";
                return;
            }
            if(!Regex.IsMatch(txtEmail.Text,"^([.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((.[a-zA-Z0-9_-]{2,3}){1,2})$",RegexOptions.IgnoreCase))
            {
                txtInfo.Text = "Email is invalid.";
                return;
            }

            RetrievalPassword entity = new RetrievalPassword();
            entity.Email = txtEmail.Text.Trim();
            entity.Question = comboBoxQuestion.SelectedValue as Question;
            entity.Answer = txtAnswer.Text.Trim();
                       
            txtInfo.Text = "";
            btnSubmit.IsEnabled = false;
            m_Client.Create<RetrievalPasswordResult>("/FindPassword", entity, (target, args) =>
            {
                btnSubmit.IsEnabled = true;

                if (args.Error != null 
                    && args.Error.Faults != null 
                    && args.Error.Faults.Count > 0)
                {
                    txtInfo.Text = args.Error.Faults[0].ErrorDescription;
                }
                else
                {
                    txtInfo.Text = args.Result.ResultDescription;
                }
            });
        }
        
    }

    public class RetrievalPassword
    {
        public string Email { get; set; }

        public string Answer { get; set; }

        public Question Question { get; set; }
    }

    public class Question
    {
        public string ID { get; set; }

        public string Description { get; set; }
    }

    public class RetrievalPasswordResult
    {
        public bool IsSuccess { get; set; }

        public string ResultDescription { get; set; }
    }
}
