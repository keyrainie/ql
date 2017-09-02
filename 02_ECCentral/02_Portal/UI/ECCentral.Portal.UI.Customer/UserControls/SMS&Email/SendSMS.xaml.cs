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
using System.Text;
using System.Text.RegularExpressions;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class SendSMS : UserControl
    {
        public IDialog Dialog { get; set; }

        public SendSMS()
        {
            InitializeComponent();

            tabSelect.SelectionChanged += tabSelect_SelectionChanged;
        }

        void tabSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbSendResults.Text = string.Empty;
            tbSendResults.Visibility = Visibility.Collapsed;
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            List<ValidationEntity> ValidationForMessage = new List<ValidationEntity>();
            List<ValidationEntity> ValidationForCellPhone = new List<ValidationEntity>();
            List<ValidationEntity> ValidationForSoSysNo = new List<ValidationEntity>();
            ValidationForMessage.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResSendSMS.validmsg_SMSMessage));
            ValidationForCellPhone.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResSendSMS.validmsg_Cellphone));
            ValidationForSoSysNo.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResSendSMS.validmsg_SoSysNo));
            //tbSendResults.Text = string.Empty;
            if (tabSelect.SelectedIndex == 0)   //按照手机号发送
            {
                if (!(ValidationHelper.Validation(this.tbTelNumber, ValidationForCellPhone) && ValidationHelper.Validation(this.tbSMSContent, ValidationForMessage)))
                    return;
                List<string> list = PerHandle(tbTelNumber.Text, @"^\d{11}$",false);
                if (list != null && list.Count > 0)
                {
                    SendSMSReq request = new SendSMSReq();
                    request.Numbers = list;
                    request.Message = tbSMSContent.Text.Trim();
                    new SMSFacade(CPApplication.Current.CurrentPage).SendSMSByCellphone(request, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        if (args.Result.Count > 0)
                        {
                            List<string> fomateErrorList = new List<string>();
                            args.Result.ForEach(item => { fomateErrorList.Add(item + ResSendSMS.msg_Fail); });
                            ShowResult(fomateErrorList);
                        }
                        else
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResSendSMS.msg_Success);
                            tbSMSContent.Text = string.Empty;
                            tbSendResults.Text = string.Empty;
                            tbSendResults.Visibility = Visibility.Collapsed;
                        }
                    });
                }
                //else
                //{
                //    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请核对手机号码是否正确");
                //    return;
                //}
            }
            else if (tabSelect.SelectedIndex == 1)    //按照订单号发送
            {
                if (!(ValidationHelper.Validation(this.tbSMSContent, ValidationForMessage) && ValidationHelper.Validation(this.tbSONumber, ValidationForSoSysNo)))
                    return;
                List<string> list = PerHandle(tbSONumber.Text, @"^\d*$",true);
                if (list != null && list.Count > 0)
                {
                    SendSMSReq request = new SendSMSReq();
                    request.Numbers = list;
                    request.Message = tbSMSContent.Text.Trim();
                    new SMSFacade(CPApplication.Current.CurrentPage).SendSMSBySOSysNo(request, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        if (args.Result.Count > 0)
                        {
                            List<string> fomateErrorList = new List<string>();
                            args.Result.ForEach(item => { fomateErrorList.Add(item + ResSendSMS.msg_Fail); });
                            ShowResult(fomateErrorList);
                        }
                        else
                        {
                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResSendSMS.msg_Success);
                            tbSMSContent.Text = string.Empty;
                            tbSendResults.Text = string.Empty;
                            tbSendResults.Visibility = Visibility.Collapsed;
                        }
                    });
                }
                //else
                //{
                //    CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("请核对订单号是否正确");
                //    return;
                //}
            }
        }
        private List<string> PerHandle(string text, string regex, bool isSoNum)
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
                    fomateErrorList.Add(list[i] + ResSendSMS.msg_FormateError);

                if (isSoNum)
                {
                    int soNum = 0;
                    if (!int.TryParse(list[i], out soNum))
                    {
                        fomateErrorList.Add(list[i] + "是无效的订单号");
                    }
                }
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
                tbSendResults.Visibility = Visibility.Visible;
            }
            tbSendResults.Text = sb.ToString();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Close();
        }
    }
}
