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
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.QueryFilter.SO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Resources;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class ThirdPartSOSearch : PageBase
    {
        CommonDataFacade m_commonFacade;
        SOThirdPartSOSearchFilter m_queryRequest;
        public ThirdPartSOSearch()
        {
            InitializeComponent();
        }
        //页面加载事件 
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
        }
        //初始化页面
        private void IniPageData()
        {
            m_commonFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            m_queryRequest = new SOThirdPartSOSearchFilter();
            this.SearchCondition.DataContext = m_queryRequest;
            BindComboBoxData();
        }

        //查询
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.QueryResultGrid.Bind();
        }

         private void dataGridPackageList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QueryThirdPartSOSearch(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                this.QueryResultGrid.TotalCount = args.Result.TotalCount;
                this.QueryResultGrid.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
            });
        }


        //绑定下拉框数据
        private void BindComboBoxData()    //未写完还有两个下拉框
        {
            //第三方订单
            m_commonFacade.GetWebChannelList(true, (sender, args) => {
                this.cmbWebChannelID.ItemsSource = args.Result;
            });

            CodeNamePairHelper.GetList(ConstValue.DomainName_SO
                , new string[] { ConstValue.Key_SOCreateResultType, ConstValue.Key_SOStatusSyncResultType }
                , CodeNamePairAppendItemType.All, (sender, e) =>
                {
                    if (!e.FaultsHandle())
                    {
                        this.cmbCreateResult.ItemsSource = e.Result[ConstValue.Key_SOCreateResultType];
                        this.cmbStatusSyncResult.ItemsSource = e.Result[ConstValue.Key_SOStatusSyncResultType];
                    }
                });
        }

        /// <summary>
        /// 第三方订单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hlbtn = sender as HyperlinkButton;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, hlbtn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.QueryResultGrid.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }               
    }
}


