using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Service.SO.Restful.RequestMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class publicProcess : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page == null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        private IPage Page
        {
            get;
            set;
        }

        List<UserInfo> m_systemUserList;

        int m_soSysNo;

        public publicProcess(IPage page, int soSysNo)
        {
            InitializeComponent();
            Page = page;
            m_soSysNo = soSysNo;
            Loaded += new RoutedEventHandler(publicProcess_Loaded);
        }

        void publicProcess_Loaded(object sender, RoutedEventArgs e)
        {
            #region 加载控件特殊值

            addpublic.SOSysNo = publicList.SOSysNo = complainList.SOSysNo = m_soSysNo;

            addpublic.RefreshLog = publicList.Bind;

            sendEmail.DataContext = new publicEmailVM();

            #endregion

            RightControl();
        }

        private void RightControl()
        {
            btnSendEmail.Visibility = AuthKeyControlMgr.GetVisibilityByRight(AuthKeyConst.SO_publicMemo_SendOrderpublicMemoEmail);
        }

        //发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            var vm = sendEmail.DataContext as publicEmailVM;
            //验证控件输入
            ValidationManager.Validate(sendEmail);
            if (vm.ValidationErrors.Count > 0) return;

            //发送邮件
            SendEmailReq req = new SendEmailReq();
            req.EmailList = RegexHelper.GetRegexEmail(vm.EmailTo);
            if (req.EmailList.Count > 0)
            {
                DateTimeHelper.GetServerTimeNow(ConstValue.DomainName_SO, p => {
                    req.Title = string.Format(ResSOInternalMemo.Msg_EmailSubject
                                           , m_soSysNo.ToString()
                                           , CPApplication.Current.LoginUser.LoginName
                                           , p);
                    req.Content = vm.SendContent;
                    req.Language = CPApplication.Current.LanguageCode;
                    new SOFacade(Page).SendEmail(req, (o, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(string.Format(ResSOInternalMemo.Msg_SendEmailSuccess, string.Join(";", args.Result.ToArray())));
                    });
                });
            }
        }

        //邮件发送人员搜寻事件
        /// <summary>
        /// 邮件发送人员搜寻事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            var search = ((TextBox)e.OriginalSource).Text;
            if (string.IsNullOrEmpty(search))
            {
                spReceiveSelect.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                //后面选择控件显示
                if (m_systemUserList == null)
                {
                    new CommonDataFacade(Page).GetAllSystemUser(CPApplication.Current.CompanyCode, (o, args) =>
                    {
                        m_systemUserList = args.Result;
                        BindReceiveUser(search);
                    });
                }
                else
                {
                    BindReceiveUser(search);
                }
            }
        }

        //绑定接收人
        void BindReceiveUser(string searchName)
        {
            spReceiveSelect.Visibility = System.Windows.Visibility.Visible;
            var list = m_systemUserList.Where(p => p.UserName.ToLower().StartsWith(searchName.ToLower())).ToList();
            if (list.Count > 0)
            {
                //添加一行选择
                list.Insert(0, new UserInfo() { UserName = ResCommonEnum.Enum_Select });
            }
            cmbReceiveSelect.ItemsSource = list;
        }

        //接收人选择改变事件
        private void cmbReceiveSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectItem = (UserInfo)e.AddedItems[0];
                if (selectItem.SysNo.HasValue)
                {
                    var selectedEmail = RegexHelper.GetRegexEmail(txtEmailList.Text);
                    //获取邮箱的文本，验证是否有重复的存在
                    if (!selectedEmail.Contains(selectItem.EmailAddress))
                    {
                        //如果没有重复的存在将加入到邮件列表后面
                        if (txtEmailList.Text.Trim().Length == 0)
                        {
                            txtEmailList.Text = selectItem.EmailAddress;
                        }
                        else
                        {
                            txtEmailList.Text = string.Format("{0}\r\n{1}", txtEmailList.Text, selectItem.EmailAddress);
                        }
                    }
                }
            }
        }
    }
}
