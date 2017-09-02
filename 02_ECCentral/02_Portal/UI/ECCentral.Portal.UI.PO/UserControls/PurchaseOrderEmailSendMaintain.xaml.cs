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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderEmailSendMaintain : UserControl
    {
        public VendorFacade vendorFacade;
        public int VendorSysNo;
        public IDialog Dialog { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public string AutoMail_AutoMailAddress = string.Empty;
        public string AutoMail_VendorMailAddress = string.Empty;
        public string AutoMail_HaveSentMailAddress = string.Empty;
        public List<ValidationEntity> validationList;

        public PurchaseOrderEmailSendMaintain(int vendorSysNo, string autoMailAddress, string vendorMailAddress, string haveSentMailAddress)
        {
            InitializeComponent();
            this.VendorSysNo = vendorSysNo;
            AutoMail_AutoMailAddress = autoMailAddress;
            AutoMail_VendorMailAddress = vendorMailAddress;
            AutoMail_HaveSentMailAddress = haveSentMailAddress;
            validationList = new List<ValidationEntity>();
            validationList.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^([a-z0-9A-Z]+[-|\.]?)+[a-z0-9A-Z]@([a-z0-9A-Z]+(-[a-z0-9A-Z]+)?\.)+[a-zA-Z]{2,}$", "电子邮件格式不正确!"));
            this.Loaded += new RoutedEventHandler(PurchaseOrderAutoSendMailMaintain_Loaded);
        }

        void PurchaseOrderAutoSendMailMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderAutoSendMailMaintain_Loaded;
            this.vendorFacade = new VendorFacade(CurrentPage);

            //加载“选择电子邮件”列表:
            BuildVendorMailList();
            //加载"PO收件人列表":
            BuildAutoMailList();
            //加载"已发送邮件地址":
            BuildHaveSentMailList();
        }

        #region [加载]
        private void BuildVendorMailList()
        {
            if (!string.IsNullOrEmpty(AutoMail_VendorMailAddress))
            {
                this.cmbEmailListInfo.Items.Clear();
                if (AutoMail_VendorMailAddress.IndexOf(';') != -1)
                {
                    string[] mailAddresses = AutoMail_VendorMailAddress.Split(';');
                    foreach (string item in mailAddresses)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            this.cmbEmailListInfo.Items.Add(item);
                        }
                    }
                }
                else
                {
                    this.cmbEmailListInfo.Items.Add(AutoMail_VendorMailAddress);
                }
                this.cmbEmailListInfo.SelectedIndex = 0;
            }
        }

        private void BuildAutoMailList()
        {
            if (!string.IsNullOrEmpty(AutoMail_AutoMailAddress))
            {
                this.lbReceiveMailAddress.Items.Clear();
                if (AutoMail_AutoMailAddress.IndexOf(';') != -1)
                {
                    string[] mailAddresses = AutoMail_AutoMailAddress.Split(';');
                    foreach (string mail in mailAddresses)
                    {
                        if (!string.IsNullOrEmpty(mail))
                        {
                            this.lbReceiveMailAddress.Items.Add(mail);
                        }
                    }
                }
                else
                {
                    this.lbReceiveMailAddress.Items.Add(AutoMail_AutoMailAddress);
                }
            }
        }

        private void BuildHaveSentMailList()
        {
            if (!string.IsNullOrEmpty(AutoMail_HaveSentMailAddress))
            {
                this.lbHaveSendMailAddress.Items.Clear();
                if (AutoMail_HaveSentMailAddress.IndexOf(';') != -1)
                {
                    string[] mailAddresses = AutoMail_HaveSentMailAddress.Split(';');
                    foreach (string mail in mailAddresses)
                    {
                        if (!string.IsNullOrEmpty(mail))
                        {
                            this.lbHaveSendMailAddress.Items.Add(mail);
                        }
                    }
                }
                else
                {
                    this.lbHaveSendMailAddress.Items.Add(AutoMail_HaveSentMailAddress);
                }
            }
        }

        #endregion

        #region [Events]
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //确认操作:
            List<string> getSelectedReceivedAddressList = new List<string>();
            if (this.lbReceiveMailAddress.Items.Count > 0)
            {
                foreach (object item in this.lbReceiveMailAddress.Items)
                {
                    getSelectedReceivedAddressList.Add(item.ToString());
                }
            }


            //跳转到Print页面，并发送邮件:
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("updatedVendorMailAddress", AutoMail_VendorMailAddress);
            dict.Add("updatedReceiveMailAddress", getSelectedReceivedAddressList);

            Dialog.ResultArgs.Data = dict;
            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            Dialog.Close(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消:
            Dialog.ResultArgs.Data = null;
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close(true);
        }

        private void btnAddSelectedMail_Click(object sender, RoutedEventArgs e)
        {
            //添加选择的电子邮件:
            if (null != this.cmbEmailListInfo.SelectedItem)
            {
                if (null != lbReceiveMailAddress.Items && 0 < lbReceiveMailAddress.Items.Count)
                {
                    foreach (var item in lbReceiveMailAddress.Items)
                    {
                        if (item.ToString() == this.cmbEmailListInfo.SelectedItem.ToString())
                        {
                            CurrentWindow.Alert(ResPurchaseOrderMaintain.InfoMsg_POMailExist);
                            return;
                        }
                    }
                }
                this.lbReceiveMailAddress.Items.Add(this.cmbEmailListInfo.SelectedItem.ToString());
            }
        }

        private void btnAddCustomizeEmailAddress_Click(object sender, RoutedEventArgs e)
        {
            //添加自定义邮件地址到PO收件人列表:
            if (!string.IsNullOrEmpty(this.txtCustomizeEmailAddress.Text))
            {
                if (!ValidationHelper.Validation(this.txtCustomizeEmailAddress, validationList))
                {
                    return;
                }

                string getInputCustomizedMailAddresses = this.txtCustomizeEmailAddress.Text.Trim();
                List<string> targetAddMailList = new List<string>();
                if (getInputCustomizedMailAddresses.IndexOf(';') != -1)
                {
                    string[] mailList = getInputCustomizedMailAddresses.Split(';');
                    foreach (string item in mailList)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (!targetAddMailList.Contains(item))
                            {
                                targetAddMailList.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    targetAddMailList.Add(getInputCustomizedMailAddresses);
                }
                foreach (string targetAddress in targetAddMailList)
                {
                    //验证是否存在:
                    if (null != lbReceiveMailAddress.Items && 0 < lbReceiveMailAddress.Items.Count)
                    {
                        foreach (var itemMail in lbReceiveMailAddress.Items)
                        {
                            if (itemMail.ToString() == targetAddress)
                            {
                                CurrentWindow.Alert(string.Format(ResPurchaseOrderMaintain.InfoMsg_POMailExistFormatString, itemMail));
                                return;
                            }
                        }
                    }
                    this.lbReceiveMailAddress.Items.Add(targetAddress);
                }
            }
            else
            {
                CurrentWindow.Alert(ResPurchaseOrderMaintain.InfoMsg_POMailInput);
                return;
            }
        }

        private void btnSaveCustomizeEmailAddress_Click(object sender, RoutedEventArgs e)
        {
            //保存自定义邮件地址到Vendor:
            if (string.IsNullOrEmpty(this.txtCustomizeEmailAddress.Text.Trim()))
            {
                CurrentWindow.Alert(ResPurchaseOrderMaintain.InfoMsg_OneMailInput);
                return;
                //TODO：邮箱正则验证:
            }
            List<string> addMailList = new List<string>();
            if (this.txtCustomizeEmailAddress.Text.Trim().IndexOf(';') != -1)
            {
                foreach (string mail in this.txtCustomizeEmailAddress.Text.Trim().Split(';'))
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        if (addMailList.Contains(mail))
                        {
                            //验证输入的邮件地址是否有重复:
                            CurrentWindow.Alert(ResPurchaseOrderMaintain.InfoMsg_SameMailAddress);
                            return;
                        }
                        addMailList.Add(mail.Trim());
                    }
                }
            }
            else
            {
                addMailList.Add(this.txtCustomizeEmailAddress.Text.Trim());
            }
            //验证是否存在新添加的邮件地址:
            int existMailCount = 0;
            foreach (string x in addMailList)
            {
                foreach (var cmbItem in this.cmbEmailListInfo.Items)
                {
                    if (cmbItem.ToString() == x)
                    {
                        existMailCount++;
                    }
                }
            }

            if (existMailCount > 0)
            {
                CurrentWindow.Alert(ResPurchaseOrderMaintain.InfoMsg_SameMailAddress2);
                return;
            }

            string getSaveStringList = string.Join(";", addMailList);
            //更新Vendor MailAddress:
            VendorInfo vendorInfo = new VendorInfo()
            {
                SysNo = VendorSysNo,
                VendorBasicInfo = new VendorBasicInfo()
                {
                    EmailAddress = getSaveStringList
                }
            };
            vendorFacade.UpdateVendorMailAddress(vendorInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                CurrentWindow.Alert("保存成功!");
                //更新ComboBox:重新绑定Vendor 邮件地址:
                this.AutoMail_VendorMailAddress = this.AutoMail_VendorMailAddress.TrimEnd(';') + ";" + getSaveStringList;
                BuildVendorMailList();
            });
        }

        private void btnDeleleAddress_Click(object sender, RoutedEventArgs e)
        {
            //删除PO收件人列表:
            if (null != this.lbReceiveMailAddress.SelectedItem)
            {
                this.lbReceiveMailAddress.Items.Remove(this.lbReceiveMailAddress.SelectedItem);
            }
            else
            {
                CurrentWindow.Alert(ResPurchaseOrderMaintain.InfoMsg_POMailDelete);
            }
        }

        #endregion

    }
}
