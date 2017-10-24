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
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Models;
using Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Society
{
    public partial class SocietyLogin : UserControl
    {

        KeystoneAuthManager facades = new KeystoneAuthManager();
        public SocietyLogin()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Login_Loaded);

#if VendorPortal
            linkForgetPassword.Visibility = System.Windows.Visibility.Visible;
            lblLoginTitle.Text = PageResource.LoginPage_Welcome;
            rect1.Visibility = System.Windows.Visibility.Collapsed;
            rect2.Visibility = System.Windows.Visibility.Collapsed;
            imgErrorIcon.Opacity = 0;
            lblLoginADPermissionTip.Text = PageResource.LoginPage_LoginMessage;
            lblLoginChoicesTip.Visibility = System.Windows.Visibility.Collapsed;
            lblOtherLoginTip.Visibility = System.Windows.Visibility.Collapsed;
            LbPhoneInfo.Visibility = System.Windows.Visibility.Collapsed;
                        TextBoxPhoneInfo.Visibility = System.Windows.Visibility.Collapsed;
#else
            lblLoginTitle.Text = PageResource.LoginPage_CannotAccessTip;
            lblLoginADPermissionTip.Text = PageResource.LoginPage_ADPermissionTip;
#endif
        }

        void Login_Loaded(object sender, RoutedEventArgs e)
        {
            string loginName = UtilityHelper.GetIsolatedStorage("LoginName");
            if (!string.IsNullOrWhiteSpace(loginName))
            {
                TextBoxLoginName.Text = loginName;
            }
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string account = TextBoxLoginName.Text;
            if (string.IsNullOrEmpty(account))
            {
                this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                TextBlockLoginFaild.Text = "社团名未提供，发送验证码失败。";
                return;
            }

            if (TextBoxPhoneInfo.Visibility == Visibility.Collapsed)
            {
                string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL_Login"].ToString();
                string url = string.Format("{0}/CommonService/ControlPanelSociety/LoginCount", serviceUrl);

                RestClient c_Client = new RestClient(url);
                LoginCountRequest request = new LoginCountRequest()
                {
                    //Action = 1,
                    //SystemNo = "ECC",
                    //InUser = account
                    Action = 1,
                    SystemNo = TextBoxLoginName.Text,
                    InUser = TextBoxPassword.Password
                };
                ControlPanelSociety society = new ControlPanelSociety()
                {
                    //OrganizationID = int.Parse(TextBoxLoginName.Text),
                    OrganizationName = TextBoxLoginName.Text,
                    Password = TextBoxPassword.Password
                };
                c_Client.Query<int>(url, request, (target, args) =>
                {
                    if (args.Result != 1)
                    {
                        this.TextBoxPhoneInfo.Visibility = Visibility.Visible;
                        this.LbPhoneInfo.Visibility = Visibility.Visible;
                        this.btnGetCode.Visibility = Visibility.Visible;
                        return;
                    }
                    else
                    {
                        TextBlockLoginFaild.Text = "登录成功";
                        LoginAction();
                    }
                });
            }
            else
            {
                if (string.IsNullOrEmpty(TextBoxPhoneInfo.Text))
                {
                    this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                    TextBlockLoginFaild.Text = "请输入短信验证码";
                    return;
                }
                else
                {
                    DateTime smsDate = DateTime.Now;
                    string smsCode = string.Empty;
                    if (!string.IsNullOrEmpty(UtilityHelper.GetIsolatedStorage("smsDate")))
                    {
                        smsDate = DateTime.Parse(UtilityHelper.GetIsolatedStorage("smsDate"));
                        smsCode = UtilityHelper.GetIsolatedStorage("smsCode");
                        if ((DateTime.Now - smsDate).TotalSeconds > 300)
                        {
                            this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                            TextBlockLoginFaild.Text = "请重新从系统中获取验证码";
                            return;
                        }
                        else
                        {
                            if (smsCode != TextBoxPhoneInfo.Text)
                            {
                                this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                                TextBlockLoginFaild.Text = "输入验证码错误，请重新输入";
                                return;
                            }
                        }
                    }
                    else
                    {
                        this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                        TextBlockLoginFaild.Text = "请从系统中获取验证码";
                        return;
                    }
                }
                // LoginAction();
            }
        }
        private void LoginAction()
        {
            ComponentFactory.GetComponent<ILogin>().Login(TextBoxLoginName.Text, TextBoxPassword.Password, (result) =>
            {
                if (result)
                {
                    ((App)App.Current).InitApp();
                    UtilityHelper.SetIsolatedStorage("LoginName", TextBoxLoginName.Text.Trim());
                    UtilityHelper.SetIsolatedStorage("loginErr", "0");
                }
                else
                {

                    (Application.Current.RootVisual as RootVisualWrapper).BorderLoadingLayer.Visibility = Visibility.Collapsed;
                    (Application.Current.RootVisual as RootVisualWrapper).LoginArea.Visibility = Visibility.Visible;
                    this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                    //login count
                    string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL_Login"].ToString();
                    string url = string.Format("{0}/CommonService/ControlPanelUser/LoginCount", serviceUrl);
                    RestClient c_Client = new RestClient(serviceUrl);
                    LoginCountRequest request = new LoginCountRequest()
                    {
                        Action = 0,
                        SystemNo = "ECC",
                        InUser = TextBoxLoginName.Text
                    };
                    c_Client.Query<int>(url, request, (target, args) => { });
                }
            });
            (Application.Current.RootVisual as RootVisualWrapper).BorderLoadingLayer.Visibility = Visibility.Visible;
            this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonLogin_Click(null, null);
            }
        }

        private void linkForgetPassword_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as RootVisualWrapper).LoginArea.Visibility = Visibility.Visible;
            (Application.Current.RootVisual as RootVisualWrapper).SocietyLogin.Visibility = Visibility.Collapsed;
        }

        private void btnSMS_Click(object sender, RoutedEventArgs e)
        {
            string account = TextBoxLoginName.Text;
            if (string.IsNullOrEmpty(account))
            {
                this.TextBlockLoginFaild.Visibility = System.Windows.Visibility.Visible;
                TextBlockLoginFaild.Text = "用户名未提供，发送验证码失败。";
                return;
            }

            string serviceUrl = CPApplication.Current.CommonData["ECCentralServiceURL_Login"].ToString();
            string userUrl = string.Format("{0}/CommonService/ControlPanelUser/LoginUser", serviceUrl);
            RestClient c_Client = new RestClient(serviceUrl);
            c_Client.Query<ControlPanelUser>(userUrl, account, (target, args) =>
            {
                if (args == null || args.Result == null)
                {
                    this.TextBlockLoginFaild.Visibility = Visibility.Visible;
                    TextBlockLoginFaild.Text = "账户未绑定手机或是未完成验证绑定，获取验证码失败。";
                    return;
                }
                //step two :send sms
                if (string.IsNullOrEmpty(args.Result.PhoneNumber))
                {
                    this.TextBlockLoginFaild.Visibility = Visibility.Visible;
                    TextBlockLoginFaild.Text = "账户未绑定手机或是未完成验证绑定，获取验证码失败。";
                    return;
                }
                else
                {
                    string code = ValidationCodeHelper.CreateValidateCode(5);
                    string SMSContent = string.Format("您于{0}在泰隆优选申请验证手机。动态验证码：{1}",
                        DateTime.Now.ToString("MM月dd日 HH:mm"), code);
                    string smsurl = string.Format("{0}/CommonService/Message/SendSMS", serviceUrl);
                    RestClient m_Client = new RestClient(serviceUrl);

                    SMSInfo smsc = new SMSInfo()
                    {
                        CellPhoneNum = args.Result.PhoneNumber,
                        Content = SMSContent,
                        Priority = 3
                    };
                    m_Client.Query<List<Question>>(smsurl, smsc, (target1, args1) =>
                    {
                        UtilityHelper.SetIsolatedStorage("smsDate", DateTime.Now.ToString());
                        UtilityHelper.SetIsolatedStorage("smsCode", code);
                    });
                }
            });
        }
    }
}
