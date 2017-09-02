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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true)]
    public partial class StorePageQuery : PageBase
    {
        public VendorFacade vendorFacade;
        public StorePageQueryVM storePageQueryVM;
        public StorePageQueryFilter queryRequest;

        public StorePageQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            storePageQueryVM = new StorePageQueryVM();
            vendorFacade = new VendorFacade(this);

            vendorFacade.QueryStorePageType((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                List<KeyValuePair<string, string>> pagetypes = new List<KeyValuePair<string, string>>();
                foreach (var row in args.Result.Rows)
                {
                    KeyValuePair<string, string> temp = new KeyValuePair<string, string>(row["Key"], row["Name"]);
                    pagetypes.Add(temp);
                }
                pagetypes.Insert(0, new KeyValuePair<string, string>(null, "所有"));
                cmbPageType.ItemsSource = pagetypes;
            });

            this.SearchCondition.DataContext = storePageQueryVM;

            base.OnPageLoad(sender, e);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var queryRequest = EntityConverter<StorePageQueryVM, StorePageQueryFilter>.Convert(storePageQueryVM);

            queryRequest.PageInfo = new PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };

            vendorFacade.QueryStorePageInfo(queryRequest, (obj, args) => {

                if (args.FaultsHandle())
                {
                    return;
                }

                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows.ToList("IsChecked", false))
                {
                    if (row["Status"] == "待审核")
                    {
                        row["AuditThrough"] = "审核通过";
                        row["AuditThroughNot"] = "审核不通过";
                    }
                    list.Add(row);
                }

                QueryResultGrid.TotalCount = args.Result.TotalCount;
                QueryResultGrid.ItemsSource = list;

            });

        }

        //选择全部
        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }
        }

        private void hyperlinkPreview_Click(object sender, RoutedEventArgs e)
        {
            dynamic getSelectedItem = this.QueryResultGrid.SelectedItem as dynamic;
            if (getSelectedItem != null)
            {
                vendorFacade.getPreviewPath((obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    this.Window.Navigate(args.Result.ToString()+getSelectedItem["LinkUrl"], null, true);
                });
            }
        }

        private List<int> GetSelectedSysNoList()
        {
            List<int> sysNoList = new List<int>();
            if (this.QueryResultGrid.ItemsSource != null)
            {
                dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        sysNoList.Add(view.SysNo);
                    }
                }
            }
            return sysNoList;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchHide_Click(object sender, RoutedEventArgs e)
        {
            List<int> list = GetSelectedSysNoList();

            if (list.Count == 0)
            {
                Window.Alert("请选择要删除的数据！");
                return;
            }
            else
            {
                vendorFacade.BatchDeleteStorePageInfo(list, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert("批量删除成功！");
                    QueryResultGrid.Bind();
                });
            }
        }

       /// <summary>
       /// 审核通过
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void HyperlinkAuditThrough_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确定要执行审核通过操作！", (obj1, args1) =>
            {
                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dynamic row = this.QueryResultGrid.SelectedItem as dynamic;
                    if (row != null)
                    {
                        vendorFacade.CheckStorePageInfo((int)row.SysNo, (int)StoreStatus.Checked, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("操作成功！");
                            QueryResultGrid.Bind();
                        });
                    }
                }
            });
        }
        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkAuditThroughNot_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确定要执行审核不通过操作", (obj1, args1) =>
            {
                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dynamic row = this.QueryResultGrid.SelectedItem as dynamic;
                    if (row != null)
                    {
                        vendorFacade.CheckStorePageInfo((int)row.SysNo, (int)StoreStatus.Unchecked, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("操作成功！");
                            QueryResultGrid.Bind();
                        });
                    }
                }
            });
        }

    }
}
