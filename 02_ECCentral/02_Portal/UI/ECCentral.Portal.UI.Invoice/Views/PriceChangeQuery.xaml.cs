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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PriceChangeQuery : PageBase
    {
        private PriceChangeQueryVM queryVM;
        private InvoiceFacade facade;
        private ObservableCollection<PriceChangeVM> resultCollection;

        public PriceChangeQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.queryVM = new PriceChangeQueryVM();
            this.facade = new InvoiceFacade(this);
            this.resultCollection = new ObservableCollection<PriceChangeVM>();

            this.querygd.DataContext = this.queryVM;
            this.resultgd.ItemsSource = this.resultCollection;

            this.resultgd.Bind();
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.querygd))
                return;

            this.resultgd.Bind();
        }

        private void hlbEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = this.resultgd.SelectedItem as PriceChangeVM;
            Window.Navigate(string.Format("/ECCentral.Portal.UI.Invoice/PriceChangeMaintain/{0}", item.SysNo.Value), null, true);
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            var SysList = from item in this.resultCollection
                          where item.IsChecked == true
                          select item.SysNo.Value;

            if (SysList.Count() > 0)
            {
                facade.BatchRunPriceChange(SysList.ToList(), (msg) =>
                {
                    Window.Alert(ResPriceChangeQuery.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                    {
                        this.resultgd.Bind();
                    });
                });
            }
            else
            {
                //Window.Alert("请至少选择一条记录！");
                Window.Alert(ResPriceChangeQuery.Msg_SelectRecord);
            }
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            var SysList = from item in this.resultCollection
                          where item.IsChecked == true
                          select item.SysNo.Value;

            if (SysList.Count() > 0)
            {
                facade.BatchAbortPriceChange(SysList.ToList(), (msg) =>
                {
                    Window.Alert(ResPriceChangeQuery.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                    {
                        this.resultgd.Bind();
                    });
                });
            }
            else
            {
                //Window.Alert("请至少选择一条记录！");
                Window.Alert(ResPriceChangeQuery.Msg_SelectRecord);
            }
        }

        private void hlbDetail_Click(object sender, RoutedEventArgs e)
        {
            var item = this.resultgd.SelectedItem as PriceChangeVM;
            Window.Navigate(string.Format("/ECCentral.Portal.UI.Invoice/PriceChangeMaintain/{0}", item.SysNo.Value), null, true);
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbox = sender as CheckBox;

            foreach (var item in this.resultCollection)
            {
                item.IsChecked = cbox.IsChecked;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(string.Format("/ECCentral.Portal.UI.Invoice/PriceChangeMaintain"), null, true);
        }

        private void btnClone_Click(object sender, RoutedEventArgs e)
        {
            var SysList = from item in this.resultCollection
                          where item.IsChecked == true
                          select item.SysNo.Value;

            if (SysList.Count() > 0)
            {
                int count = this.resultCollection.Count((x) =>
                {
                    return x.IsChecked == true &&
                        x.Status != RequestPriceStatus.Aborted && x.Status != RequestPriceStatus.Finished;
                });
                if (count > 0)
                {
                    //Window.Alert("只有终止、已完成状态的数据可以进行复制！");
                    Window.Alert(ResPriceChangeQuery.Msg_JustStopOrFinishedCanCopy);
                    return;
                }

                facade.ClonePriceChange(SysList.ToList(), (msg) =>
                {
                    Window.Alert(ResPriceChangeQuery.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                    {
                        this.resultgd.Bind();
                    });
                });
            }
            else
            {
                //Window.Alert("请至少选择一条记录！");
                Window.Alert(ResPriceChangeQuery.Msg_SelectRecord);
            }
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_PriceChange_Audit))
            {
                //Window.Alert("不能进行审核操作，你没有审核权限！");
                Window.Alert(ResPriceChangeQuery.Msg_NoAuditAuthority);
                return;
            }

            UCPriceChangeSetter uc = new UCPriceChangeSetter();
            uc.IsBatchModel = true;

            IDialog dialog = Window.ShowDialog(ResPriceChangeQuery.Msg_AudtiMemo, uc, (obj1, args1) =>
            {
                if (args1.DialogResult == DialogResultType.OK)
                {
                    string auditMemo = Convert.ToString(args1.Data);

                    var SysList = from item in this.resultCollection
                                  where item.IsChecked == true
                                  select item.SysNo.Value;

                    if (SysList.Count() > 0)
                    {
                        Dictionary<int, string> dic = new Dictionary<int, string>();

                        foreach (var item in SysList)
                        {
                            dic.Add(item, auditMemo);
                        }

                        facade.BatchAuditPriceChange(dic, (msg) =>
                        {
                            Window.Alert(ResPriceChangeQuery.Msg_Tips, msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information, (obj, args) =>
                            {
                                this.resultgd.Bind();
                            });
                        });
                    }
                    else
                    {
                        //Window.Alert("请至少选择一条记录！");
                        Window.Alert(ResPriceChangeQuery.Msg_SelectRecord);
                    }
                }
            });

            uc.DialogHanlder = dialog;

        }

        private void resultgd_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            facade.QueryPriceChange(this.queryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                this.resultCollection = new ObservableCollection<PriceChangeVM>(DynamicConverter<PriceChangeVM>.ConvertToVMList(args.Result.Rows.ToList("IsChecked", false)));

                this.resultgd.ItemsSource = this.resultCollection;
                this.resultgd.TotalCount = args.Result.TotalCount;
            });
        }
    }
}
