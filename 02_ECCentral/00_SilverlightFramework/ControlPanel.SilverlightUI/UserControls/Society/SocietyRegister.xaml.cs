using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;



namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Society
{
    public partial class SocietyRegister : UserControl
    {
        public SocietyRegister()
        {
            InitializeComponent();
        }
        private bool m_IsQuestionLoaded = false;
        RestClient c_Client;
        public void LoadComBox()
        {
            string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL_Login"].ToString();
            string url = string.Format("{0}/CommonService/ControlPanelSociety/GetSocietyProvince_ComBox", serviceUrl);
            if (!m_IsQuestionLoaded)
            {
                m_IsQuestionLoaded = true;
               // btnSubmit.IsEnabled = false;
                ////加载省市
                c_Client = new RestClient(url);
                c_Client.Query<List<ComBoxData>>(url, null, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    comBoxProvince.DisplayMemberPath = "Name";
                    comBoxProvince.SelectedValue = "ID";
                    this.comBoxProvince.ItemsSource = args.Result;
                    if (args.Result != null && args.Result.Count > 0)
                    {
                        comBoxProvince.SelectedIndex = 0;
                    }

                    //if (loadCompletedCount == 2)
                    //{
                    //    OnLoadCompleted(chart, list, uniqueList);
                    //}
                });
                //加载佣金
            }
        }

        private void linkBack_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as RootVisualWrapper).LoginArea.Visibility = Visibility.Visible;
            (Application.Current.RootVisual as RootVisualWrapper).SocietyRegister.Visibility = Visibility.Collapsed;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSocietyName.Text))
            {
                txtInfo.Text = "Society name is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                txtInfo.Text = "Password is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPasswordConfirm.Password))
            {
                txtInfo.Text = "Confirm password is required.";
                return;
            }
            if (txtPasswordConfirm.Password != txtPassword.Password)
            {
                txtInfo.Text = "The two password is inconsistent.";
                return;
            }
            Register();
            //if(!Regex.IsMatch(txtSocietyName.Text,"^([.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((.[a-zA-Z0-9_-]{2,3}){1,2})$",RegexOptions.IgnoreCase))
            //{
            //    txtInfo.Text = "Email is invalid.";
            //    return;
            //}

            // RetrievalPassword entity = new RetrievalPassword();
            // entity.Email = txtSocietyName.Text.Trim();
            //// entity.Question = comboBoxQuestion.SelectedValue as Question;
            // entity.Answer = txtPassword.Text.Trim();

            // txtInfo.Text = "";
            // btnSubmit.IsEnabled = false;
            // m_Client.Create<RetrievalPasswordResult>("/FindPassword", entity, (target, args) =>
            // {
            //     btnSubmit.IsEnabled = true;

            //     if (args.Error != null 
            //         && args.Error.Faults != null 
            //         && args.Error.Faults.Count > 0)
            //     {
            //         txtInfo.Text = args.Error.Faults[0].ErrorDescription;
            //     }
            //     else
            //     {
            //         txtInfo.Text = args.Result.ResultDescription;
            //     }
            // });
        }

        private void Register()
        {
            string account = txtSocietyName.Text;
            if (string.IsNullOrEmpty(account))
            {
                this.txtInfo.Visibility = System.Windows.Visibility.Visible;
                txtInfo.Text = "用户名未提供，发送验证码失败。";
                return;
            }

            //if (TextBoxPhoneInfo.Visibility == Visibility.Collapsed)
            if (true)
            {
                string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL_Login"].ToString();
                string url = string.Format("{0}/CommonService/ControlPanelSociety/CreateSociety", serviceUrl);

                RestClient c_Client = new RestClient(url);
                LoginCountRequest request = new LoginCountRequest()
                {
                    Action = 1,
                    SystemNo = "ECC",
                    InUser = account
                };
                ControlPanelSociety orientation = new ControlPanelSociety()
                {
                    OrganizationName = txtSocietyName.Text,
                    Password = txtPassword.Password,
                    InDate = DateTime.Now,
                    InUser = txtSocietyName.Text,
                    EditDate = DateTime.Now,
                    EditUser = txtSocietyName.Text,
                    CommissionID = "01",
                    Province = (this.comBoxProvince.SelectedValue as ComBoxData).ID
                };
                c_Client.Create<int>(url, orientation, (target, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        //this.TextBoxPhoneInfo.Visibility = Visibility.Visible;
                        //this.LbPhoneInfo.Visibility = Visibility.Visible;
                        //this.btnGetCode.Visibility = Visibility.Visible;
                        txtInfo.Text = "社团注册完成。";
                        return;
                    }
                    else
                    {
                        LoginAction();
                    }
                });


                //c_Client.Query<int>(url, request, (target, args) =>
                //{
                //    if (args.Result > 3)
                //    {
                //        //this.TextBoxPhoneInfo.Visibility = Visibility.Visible;
                //        //this.LbPhoneInfo.Visibility = Visibility.Visible;
                //        //this.btnGetCode.Visibility = Visibility.Visible;
                //        return;
                //    }
                //    else
                //    {
                //        LoginAction();
                //    }
                //});
            }
            else
            {
                //if (string.IsNullOrEmpty(TextBoxPhoneInfo.Text))
                //{
                //    this.txtInfo.Visibility = System.Windows.Visibility.Visible;
                //    txtInfo.Text = "请输入短信验证码";
                //    return;
                //}
                //else
                //{
                //    DateTime smsDate = DateTime.Now;
                //    string smsCode = string.Empty;
                //    if (!string.IsNullOrEmpty(UtilityHelper.GetIsolatedStorage("smsDate")))
                //    {
                //        smsDate = DateTime.Parse(UtilityHelper.GetIsolatedStorage("smsDate"));
                //        smsCode = UtilityHelper.GetIsolatedStorage("smsCode");
                //        if ((DateTime.Now - smsDate).TotalSeconds > 300)
                //        {
                //            this.txtInfo.Visibility = System.Windows.Visibility.Visible;
                //            txtInfo.Text = "请重新从系统中获取验证码";
                //            return;
                //        }
                //        else
                //        {
                //            if (smsCode != TextBoxPhoneInfo.Text)
                //            {
                //                this.txtInfo.Visibility = System.Windows.Visibility.Visible;
                //                txtInfo.Text = "输入验证码错误，请重新输入";
                //                return;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        this.txtInfo.Visibility = System.Windows.Visibility.Visible;
                //        txtInfo.Text = "请从系统中获取验证码";
                //        return;
                //    }
                //}
                //LoginAction();
            }
        }
        private void LoginAction()
        {
            ComponentFactory.GetComponent<ILogin>().Login(txtSocietyName.Text, txtPassword.Password, (result) =>
            {
                if (result)
                {
                    ((App)App.Current).InitApp();
                    UtilityHelper.SetIsolatedStorage("LoginName", txtSocietyName.Text.Trim());
                    UtilityHelper.SetIsolatedStorage("loginErr", "0");
                }
                else
                {

                    (Application.Current.RootVisual as RootVisualWrapper).BorderLoadingLayer.Visibility = Visibility.Collapsed;
                    (Application.Current.RootVisual as RootVisualWrapper).LoginArea.Visibility = Visibility.Visible;
                    this.txtInfo.Visibility = System.Windows.Visibility.Visible;
                    //login count
                    string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL_Login"].ToString();
                    string url = string.Format("{0}/CommonService/ControlPanelUser/LoginCount", serviceUrl);
                    RestClient c_Client = new RestClient(serviceUrl);
                    LoginCountRequest request = new LoginCountRequest()
                    {
                        Action = 0,
                        SystemNo = "ECC",
                        InUser = txtSocietyName.Text
                    };
                    c_Client.Query<int>(url, request, (target, args) => { });
                }
            });
            (Application.Current.RootVisual as RootVisualWrapper).BorderLoadingLayer.Visibility = Visibility.Visible;
            this.txtInfo.Visibility = System.Windows.Visibility.Collapsed;
        }

    }

    public class ComBoxData
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
