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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.ExternalSYS;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.UserControls;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CpsUserManagement : PageBase
    {
        private CpsUserFacade facade;
        private CpsUserQueryVM model;
        public CpsUserManagement()
        {
            InitializeComponent();
            this.CpsUserResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(CpsUserResult_LoadingDataSource);
            this.Loaded += (sender, e) => 
            {
                facade = new CpsUserFacade();
                model = new CpsUserQueryVM();
                facade.GetWebSiteType((obj, arg) => 
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    List<WebSiteType> templist = new List<WebSiteType>() {new WebSiteType(){SelectValue=null,Description="--所有--"} };

                    foreach (var item in  arg.Result.Rows)
                    {
                        templist.Add(new WebSiteType() { SelectValue = item.value, Description = item.Description });  
                    }
                    model.ListWebSiteType = templist;
                    model.WebSiteType = (from p in model.ListWebSiteType where p.SelectValue == null select p).ToList().FirstOrDefault();
                   this.DataContext = model;
                });
                
            };
        }

        void CpsUserResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            
            facade.GetCpsUser(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.CpsUserResult.ItemsSource = arg.Result.Rows;
                this.CpsUserResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.CpsUserResult.Bind();
        }

        /// <summary>
        /// 禁用启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlActive_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
             HyperlinkButton link = (HyperlinkButton)sender;
             IsActive eum = link.Tag == null ? IsActive.DeActive : (IsActive)link.Tag;
            CpsUserVM vm = new CpsUserVM() 
            { BasicUser = new CpsBasicUserInfoVM() {IsActive=eum==IsActive.Active?IsActive.DeActive:IsActive.Active} ,
                SysNo=d.SysNo};
           
            facade.UpdateUserStatus(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                Window.MessageBox.Show(string.Format("{0}成功!", link.Content.ToString()), MessageBoxType.Success);
                this.CpsUserResult.Bind();
            });
        }

        private void hlWebSiteUrl_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = (HyperlinkButton)sender;
            Window.Navigate(link.Content.ToString());
        }

        private void hlUserBasic_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
            CpsUserBasicMaintain item = new CpsUserBasicMaintain();
            CpsUserVM data = new CpsUserVM()
            {
                BasicUser = new CpsBasicUserInfoVM() 
                {
                    AllianceAccount = d.CustomerID,
                    Contact = d.ContactName,
                    ContactAddress = d.ContactAddress,
                    ContactPhone = d.ContactPhone,
                    Email = d.Email,
                    IsActive = d.IsAvailable == null ? IsActive.DeActive : d.IsAvailable,
                    WebSiteAddress = d.WebSiteUrl,
                    WebSiteCode = d.WebSiteCode,
                    WebSiteName = d.WebSiteName,
                    ZipCode = d.ZipCode
                },
                SysNo = d.SysNo,
            };
            data.ListWebSiteType = model.ListWebSiteType;
             data.WebSiteType = (from p in data.ListWebSiteType where p.SelectValue == d.WebSiteCode select p).ToList().FirstOrDefault();

             if (data.WebSiteType == null)
             {
                 data.WebSiteType = (from p in data.ListWebSiteType where p.SelectValue == null select p).ToList().FirstOrDefault();
             }
            
            data.UserType = d.UserType;
            item.Data = data;
            item.Dialog = Window.ShowDialog("基本信息", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CpsUserResult.Bind();
                }
            });
        }

        private void hlAccount_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
            ReceivablesAccountMaintain item = new ReceivablesAccountMaintain();
            CpsUserVM data = new CpsUserVM()
            {
              
                ReceivablesAccount = new CpsReceivablesAccountVM() 
                {
                    BranchBank = d.BranchBank,
                    BrankCardNumber = d.BankCardNumber,
                    BrankCode = d.BankCode,
                    BrankName = d.BankName,
                    ReceiveablesName = d.ReceivableName,
                    IsLock=d.BankLock,
                    ReceivablesAccountType = d.BankAccountType
                },
                SysNo=d.SysNo
            };
            item.Data = data;
            item.Dialog = Window.ShowDialog(" 收款账户信息", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CpsUserResult.Bind();
                }
            });

        }

        private void hlAddSource_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
            CpsUserSourceEdit item = new CpsUserSourceEdit() { UserType = d.UserType,SysNo=d.SysNo };
            item.Dialog = Window.ShowDialog("添加子Source", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CpsUserResult.Bind();
                }
            });
        }

        private void hlViewSource_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
            CpsUserSourceMaintain item = new CpsUserSourceMaintain() { UserSysNo = d.SysNo, Source = d.CustomerID };
            item.Dialog = Window.ShowDialog("查看Source", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.Cancel)
                {
                    this.CpsUserResult.Bind();
                }
            },new Size(600,400));
        }

        private void hlDetail_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
            AuditHistory item = new AuditHistory() { SysNo = d.SysNo };
            item.Dialog = Window.ShowDialog("审核记录", item, (s, args) =>
            {
                
            });
        }

        private void hlAuditClearance_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem;
            CpsUserVM vm = new CpsUserVM() {SysNo=d.SysNo,Status=AuditStatus.AuditClearance };
            facade.AuditUser(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                Window.MessageBox.Show("审核成功!", MessageBoxType.Success);
                this.CpsUserResult.Bind();
            });
        }

        private void hlAuditNoClearance_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.CpsUserResult.SelectedItem as dynamic;
            CpsUserAudit item = new CpsUserAudit() { SysNo = d.SysNo };
            item.Dialog = Window.ShowDialog("审核拒绝", item, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    this.CpsUserResult.Bind();
                }
            });
        }

     

    }
}
