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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.NeweggCN.Resources;
using ECCentral.Portal.UI.MKT.NeweggCN.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AmbassadorNews : PageBase
    {
        private AmbassadorNewsQueryVM _queryVM;

        private AmbassadorNewsFacade CurrentFacade;

        public AmbassadorNews()
        {
            InitializeComponent();
            
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.ucBigArea.BizMode = UCBigArea.BigAreaEdiMode.Query;
            _queryVM = new AmbassadorNewsQueryVM();
            this.QueryGrid.DataContext = _queryVM;
            CurrentFacade = new AmbassadorNewsFacade(this);
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            //1.初始化查询条件
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            if (CurrentFacade != null)
            {

                CurrentFacade.Query(_queryVM, p, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    var rows = args.Result.Rows.ToList();

                    this.DataGrid.ItemsSource = rows;
                    this.DataGrid.TotalCount = args.Result.TotalCount;
                });
            }
        }

        public AmbassadorNewsVM GetAmbassadorNewsBySysNo(int sysNo)
        {
            return new AmbassadorNewsVM();
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.QueryGrid);
            if (this._queryVM.HasValidationErrors)
                return;
            this.DataGrid.Bind();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            //this.Window.Navigate(string.Format(ConstValue.MKT_AmbassadorNewsMaintainUrlFormat, ""), null, true);

            AmbassadorNewsMaintain maintainAmbassadorNewsForm = new AmbassadorNewsMaintain();

           maintainAmbassadorNewsForm.CurrentDialog=Window.ShowDialog(ResAmbassadorNews.Expander_CreateAmbassadorNews, maintainAmbassadorNewsForm, (s, args) =>
            {
                
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid.Bind();
                }
            });
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            //this.Window.Navigate(string.Format(ConstValue.MKT_AmbassadorNewsMaintainUrlFormat, row.SysNo), null, true);

            AmbassadorNewsMaintain maintainAmbassadorNewsForm = new AmbassadorNewsMaintain();
            maintainAmbassadorNewsForm.AmbassadorNewsSysNo = row.SysNo;
            maintainAmbassadorNewsForm.CurrentDialog= Window.ShowDialog(ResAmbassadorNews.Expander_MaintainAmbassaderNews, maintainAmbassadorNewsForm, (s,args) => {
                if (args.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid.Bind();
                }
            });
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (this.DataGrid == null || this.DataGrid.ItemsSource == null || checkBoxAll == null)
                return;

            dynamic items = this.DataGrid.ItemsSource as dynamic;

            if (items == null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                dynamic item = items[i];

                item.IsChecked = checkBoxAll.IsChecked ?? false;
            }
        }

        /// <summary>
        /// 批量展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBatchDisplay_Click(object sender, RoutedEventArgs e)
        {
            BatchUpdateAmbassadorNewsStatus(AmbassadorNewsStatus.Display);

        }

        /// <summary>
        /// 批量屏蔽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBatchUnDisplay_Click(object sender, RoutedEventArgs e)
        {
            BatchUpdateAmbassadorNewsStatus(AmbassadorNewsStatus.UnDisplay);
        }

        /// <summary>
        /// 批量修改泰隆优选大使公告状态。
        /// </summary>
        /// <param name="status"></param>
        private void BatchUpdateAmbassadorNewsStatus(AmbassadorNewsStatus status)
        {
            dynamic items = this.DataGrid.ItemsSource as dynamic;

            if (items == null)
                return;

            List<int> sysNos = new List<int>();

            for (int i = 0; i < items.Count; i++)
            {
                dynamic item = items[i];
                if (item.IsChecked)
                {
                    sysNos.Add(item.SysNo);
                }
            }
            if (sysNos.Count > 0)
            {
                if (CurrentFacade != null)
                {
                    CurrentFacade.BatchUpdateAmbassadorNewsStatus(sysNos, status, (s, args) =>
                    {
                        if (args.FaultsHandle())
                            return;

                        this.DataGrid.Bind();
                    });
                }
            }
            else
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("没有选择记录", MessageBoxType.Error);
            }
        }
    }
}
