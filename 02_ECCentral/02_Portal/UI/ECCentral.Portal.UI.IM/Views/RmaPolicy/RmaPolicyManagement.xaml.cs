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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.IM.Facades;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class RmaPolicyManagement : PageBase
    {
        private RmaPolicyQueryVM QueryVM { get; set; }
        private RmaPolicyFacade facade;
        public RmaPolicyManagement()
        {
            InitializeComponent();
            this.dgRmaPolicyQueryResult.LoadingDataSource += dgRmaPolicyQueryResult_LoadingDataSource;
        }

        void dgRmaPolicyQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.QueryRmaPolicy(QueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, arg) => {
                if (arg.FaultsHandle())
                {
                    return;
                }
                this.dgRmaPolicyQueryResult.ItemsSource = arg.Result.Rows.ToList("IsChecked", false);
                this.dgRmaPolicyQueryResult.TotalCount = arg.Result.TotalCount;
            });
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            QueryVM = new RmaPolicyQueryVM();
            facade = new RmaPolicyFacade(this);
            this.DataContext = QueryVM;
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            this.dgRmaPolicyQueryResult.Bind();
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                dynamic viewlist = this.dgRmaPolicyQueryResult.ItemsSource as dynamic;
                if (viewlist != null)
                {
                    foreach (var item in viewlist)
                    {
                        item.IsChecked = cb.IsChecked != null ? cb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void btnCreate_Click_1(object sender, RoutedEventArgs e)
        {
            RmaPolicyMainain item = new RmaPolicyMainain();
            item.Action = RmaAction.Create;
            item.Dialog = Window.ShowDialog("新建退换货政策", item, (s, args) =>
            {
                this.dgRmaPolicyQueryResult.Bind();
            });
        }

        private void hlEdit_Click_1(object sender, RoutedEventArgs e)
        {
            RmaPolicyMainain item = new RmaPolicyMainain();
            dynamic selectitem = this.dgRmaPolicyQueryResult.SelectedItem as dynamic;
            item.Action = RmaAction.Edit;
            item.Sysno = selectitem.SysNo;
            item.Dialog = Window.ShowDialog("编辑退换货政策", item, (s, args) =>
            {
                
                    this.dgRmaPolicyQueryResult.Bind();
                
            });
        }

        private void btnActive_Click_1(object sender, RoutedEventArgs e)
        {
            List<RmaPolicyVM> list = new List<RmaPolicyVM>();
            dynamic viewlist = this.dgRmaPolicyQueryResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        list.Add(new RmaPolicyVM() 
                        { 
                            RmaType = item.Type, 
                            SysNo = item.SysNo, 
                            ECDisplayName = item.ECDisplayName,
                            ChangeDate =Convert.ToString(item.ChangeDate),
                            ECDisplayDesc = item.ECDisplayDesc,
                            ECDisplayMoreDesc = item.ECDisplayMoreDesc,
                            IsRequest = item.IsOnlineRequst == IsOnlineRequst.YES ? true : false,
                            Priority = Convert.ToString(item.Priority),
                            ReturnDate = Convert.ToString(item.ReturnDate),
                            RMAPolicyName=item.RMAPolicyName,
                            
                        });
                    }
                }
            }
            if (list.Count > 0)
            {
                facade.ActiveRmaPolicy(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    Window.MessageBox.Show("激活成功!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                    dynamic d = this.dgRmaPolicyQueryResult.ItemsSource as dynamic;
                    if (viewlist != null)
                    {
                        foreach (var item in d)
                        {
                            item.IsChecked = false;
                        }
                    }
                    this.dgRmaPolicyQueryResult.Bind();
                });
            }
            else
            {
                Window.MessageBox.Show("请先选择!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Information);
            }
        }

        private void btnCanel_Click_1(object sender, RoutedEventArgs e)
        {
            List<RmaPolicyVM> list = new List<RmaPolicyVM>();

            dynamic viewlist = this.dgRmaPolicyQueryResult.ItemsSource as dynamic;
            if (viewlist != null)
            {
                foreach (var item in viewlist)
                {
                    if (item.IsChecked == true)
                    {
                        list.Add(new RmaPolicyVM() 
                        { 
                            RmaType = item.Type, 
                            SysNo = item.SysNo, 
                            ECDisplayName = item.ECDisplayName,
                            ChangeDate =Convert.ToString(item.ChangeDate),
                            ECDisplayDesc = item.ECDisplayDesc,
                            ECDisplayMoreDesc = item.ECDisplayMoreDesc,
                            IsRequest = item.IsOnlineRequst == IsOnlineRequst.YES ? true : false,
                            Priority=Convert.ToString(item.Priority),
                            ReturnDate=Convert.ToString(item.ReturnDate),
                            RMAPolicyName=item.RMAPolicyName,
                            
                        });
                    }
                }
            }
            if (list.Count > 0)
            {
                facade.DeActiveRmaPolicy(list, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }

                    Window.MessageBox.Show("作废成功!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Success);
                    dynamic d = this.dgRmaPolicyQueryResult.ItemsSource as dynamic;
                    if (viewlist != null)
                    {
                        foreach (var item in d)
                        {
                            item.IsChecked = false;
                        }
                    }
                    this.dgRmaPolicyQueryResult.Bind();
                });
            }
            else
            {
                Window.MessageBox.Show("请先选择!", Newegg.Oversea.Silverlight.Controls.Components.MessageBoxType.Information);
            }
        }

        private void hlSearchLog_Click_1(object sender, RoutedEventArgs e)
        {
            HyperlinkButton link = sender as HyperlinkButton;
            if (link == null)
            {
                Window.MessageBox.Show("无效信息!");
            }
            Window.Navigate(string.Format(ConstValue.IM_RmaPolicyLogManagementUrlFormat, link.Tag), null, true);
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            RmaPolicyMainain item = new RmaPolicyMainain();
            dynamic selectitem = this.dgRmaPolicyQueryResult.SelectedItem as dynamic;
            item.Action = RmaAction.Details;
            item.Sysno = selectitem.SysNo;
            item.Dialog = Window.ShowDialog("查看退换货政策", item, (s, args) =>
            {
                 
                });
        }
       

    }
}
