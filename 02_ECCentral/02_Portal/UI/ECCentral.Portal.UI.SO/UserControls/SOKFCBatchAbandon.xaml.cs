using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic.Components.UserControls.ReasonCodePicker;
using ECCentral.BizEntity.Common;
using System;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOKFCBatchAbandon : UserControl
    {
        public int CustomerSysNo { get; set; }

        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page != null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private IPage Page
        {
            get;
            set;
        }

        public SOKFCBatchAbandon(IPage page)
        {
            Page = page;

            InitializeComponent();
            Loaded += new RoutedEventHandler(SOKFCBatchAbandon_Loaded);
        }

        void SOKFCBatchAbandon_Loaded(object sender, RoutedEventArgs e)
        {
            Bind();
        }

        private void dataGridList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            SORequestQueryFilter query = new SORequestQueryFilter();
            query.CustomerSysNo = CustomerSysNo;
            query.SOStatusArray = new List<SOStatus> { SOStatus.Origin, SOStatus.WaitingOutStock, SOStatus.WaitingManagerAudit };

            query.PageInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };

            SOQueryFacade facade = new SOQueryFacade();
            facade.QuerySO(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridList.TotalCount = args.Result.TotalCount;
                dataGridList.ItemsSource = args.Result.Rows.ToList("IsCheck", true); ;
            });
        }

        public void Bind()
        {
            dataGridList.Bind();
        }

        private void cbSelectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dataGridList.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, btn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void btnBatchAbandon_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = new List<int>();
            var dynamic = this.dataGridList.ItemsSource as dynamic;
            if (dynamic != null)
            {
                foreach (var item in dynamic)
                {
                    if (item.IsCheck == true)
                    {
                        list.Add((int)item["SysNo"]);
                    }
                }
            }

            if (list.Count == 0)
            {
                this.Window.Alert(ResSO.Msg_PleaseSelect);
                return;
            }
            Window.Confirm(ResSO.Info_AbandonConfirm, (s, confirmEvent) =>
            {
                if (confirmEvent.DialogResult == DialogResultType.OK)
                {
                    UCReasonCodePicker content = new UCReasonCodePicker();
                    content.ReasonCodeType = ReasonCodeType.Order;
                    content.Dialog = Window.ShowDialog(ResSOMaintain.Info_SOMaintain_AbandSO, content, (obj, args) =>
                    {
                        if (args.Data != null)
                        {
                            KeyValuePair<string, string> data = (KeyValuePair<string, string>)args.Data;
                            (new SOFacade(this.Page)).AbandonSO(list, false, (vm) =>
                            {
                                if (vm != null)
                                {
                                    SOInternalMemoFacade SOInternalMemoFacade = new SOInternalMemoFacade();
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        SOInternalMemoFacade.Create(new SOInternalMemoInfo
                                        {
                                            SOSysNo = list[i],
                                            Content = data.Value,
                                            LogTime = DateTime.Now,
                                            ReasonCodeSysNo = int.Parse(data.Key),
                                            CompanyCode = CPApplication.Current.CompanyCode,
                                            Status = SOInternalMemoStatus.FollowUp
                                        }, null);
                                    }
                                    Bind();
                                    Window.Alert(ResSOMaintain.Info_SOMaintain_SO_Abanded, MessageType.Information);
                                }
                            });
                        }
                    });
                }
            });
        }
    }
}
