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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Resources;
using System.Text.RegularExpressions;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Text;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Portal.UI.Customer.Facades;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class SendEmail : UserControl
    {
        public IDialog Dialog { get; set; }

        public SendEmail()
        {
            InitializeComponent();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            List<ValidationEntity> ValidationForMessage = new List<ValidationEntity>();
            List<ValidationEntity> ValidationForEmail = new List<ValidationEntity>();
            List<ValidationEntity> ValidationForSubject = new List<ValidationEntity>();
            ValidationForMessage.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResSendEmail.msg_ContentIsNull));
            ValidationForEmail.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResSendEmail.msg_AddressIsNull));
            ValidationForSubject.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResSendEmail.msg_SubjectIsNull));
            //tbSendResults.Text = string.Empty;

            if (!(ValidationHelper.Validation(this.tbEmailAddress, ValidationForEmail) && ValidationHelper.Validation(this.tbEmailContent, ValidationForMessage) && ValidationHelper.Validation(this.tbTitle, ValidationForSubject)))
                return;
            List<string> list = PerHandle(tbEmailAddress.Text, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            if (list != null && list.Count > 0)
            {
                SendEmailReq request = new SendEmailReq();
                request.Content = tbEmailContent.Text.Trim();
                request.Title = tbTitle.Text.Trim();
                request.EmailList = list;
                request.CompanyCode = CPApplication.Current.CompanyCode;
                new SMSFacade(CPApplication.Current.CurrentPage).SendEmail(request, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    if (args.Result.Count > 0)
                    {
                        List<string> fomateErrorList = new List<string>();
                        args.Result.ForEach(item => { fomateErrorList.Add(item + ResSendEmail.msg_Fail); });
                        ShowResult(fomateErrorList);
                    }
                    else
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResSendEmail.msg_Success);
                        tbEmailContent.Text = string.Empty;
                        tbTitle.Text = string.Empty;
                    }
                });
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请核对邮件地址是否正确");
            }
        }

        private List<string> PerHandle(string text, string regex)
        {
            text = Regex.Replace(text, @"(，|\n|\s)", ",", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, ",+", ",", RegexOptions.IgnoreCase);
            if (text.Length == 0)
                return null;
            string[] list = Regex.Split(text.TrimEnd(','), ",", RegexOptions.IgnoreCase);
            List<string> fomateErrorList = new List<string>();
            for (int i = 0; i < list.Length; i++)
            {
                if (!Regex.IsMatch(list[i], regex))
                    fomateErrorList.Add(list[i] + ResSendEmail.msg_FormateError);
            }
            if (fomateErrorList.Count > 0)
            {
                ShowResult(fomateErrorList);
                return null;
            }
            return list.Distinct().ToList();
        }

        private void ShowResult(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            if (list.Count > 0)
            {
                list.ForEach(item => { sb.Append(item + "\r"); });
            }
            //tbSendResults.Text = sb.ToString();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }
 
    }
}
