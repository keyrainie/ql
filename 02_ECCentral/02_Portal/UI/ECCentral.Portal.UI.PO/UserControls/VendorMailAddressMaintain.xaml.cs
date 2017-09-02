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
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class VendorMailAddressMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public VendorFacade serviceFacade;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public List<string> VendorMailList;
        public List<ValidationEntity> validationList;

        public VendorMailAddressMaintain(List<string> vendorMailList)
        {
            InitializeComponent();
            VendorMailList = new List<string>();
            validationList = new List<ValidationEntity>();
            validationList.Add(new ValidationEntity(ValidationEnum.RegexCheck, @"^([a-z0-9A-Z_]+[-|\.]?)+[a-z0-9A-Z]@([a-z0-9A-Z_]+(-[a-z0-9A-Z_]+)?\.)+[a-zA-Z]{2,}$", "电子邮件格式不正确!"));
            this.Loaded += new RoutedEventHandler(VendorMailAddressMaintain_Loaded);
            this.VendorMailList = vendorMailList;
        }

        void VendorMailAddressMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= VendorMailAddressMaintain_Loaded;
            RefreshListBox();
        }

        private void btnAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //添加电子邮箱地址:
            if (string.IsNullOrEmpty(this.txtAddMailAddress.Text.Trim()))
            {
                CurrentWindow.Alert("电子邮件不能为空!");
                return;
            }
            if (!ValidationHelper.Validation(this.txtAddMailAddress, validationList))
            {
                return;
            }
            if (this.txtAddMailAddress.Text.Trim().IndexOf(';') >= 0)
            {
                string[] getMailSplitString = txtAddMailAddress.Text.Trim().Split(';');
                foreach (string item in getMailSplitString)
                {
                    if (!string.IsNullOrEmpty(item) && !CheckVendorMailAddressExists(item))
                    {
                        this.VendorMailList.Add(item.Trim());
                    }
                }
            }
            else
            {
                if (!CheckVendorMailAddressExists(this.txtAddMailAddress.Text.Trim()))
                {
                    this.VendorMailList.Add(this.txtAddMailAddress.Text.Trim());
                }
            }
            this.txtAddMailAddress.Text = string.Empty;
            RefreshListBox();
        }

        private void RefreshListBox()
        {
            this.lstMailAddressList.Items.Clear();
            if (null != this.VendorMailList)
            {
                VendorMailList.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        this.lstMailAddressList.Items.Add(x);
                    }
                });
            }
        }

        private bool CheckVendorMailAddressExists(string mailAddress)
        {
            if (null != VendorMailList)
            {
                if (VendorMailList.SingleOrDefault(i => i == mailAddress) != null)
                {
                    CurrentWindow.Alert(string.Format("重复的电子邮件:{0}", mailAddress));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.Data = VendorMailList;
            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            this.Dialog.Close(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作;
            this.Dialog.ResultArgs.Data = null;
            this.Dialog.Close(true);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //删除操作:
            if (this.lstMailAddressList.SelectedIndex >= 0)
            {
                this.VendorMailList.Remove(this.lstMailAddressList.SelectedItem.ToString());
                RefreshListBox();
            }
            else
            {
                CurrentWindow.Alert("请选择要删除的电子邮件!");
                return;
            }
        }

    }
}
