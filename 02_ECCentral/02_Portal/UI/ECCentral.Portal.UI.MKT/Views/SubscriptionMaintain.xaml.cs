using System;
using System.Collections.Generic;
using System.Windows;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SubscriptionMaintain : PageBase
    {
        private SubscriptionQueryFilter filter;
        private SubscriptionQueryFilter filterVM;
        private SubscriptionQueryFacade facade;
        private SubscriptionCategoryQueryFacade _facadeCatetory;
        private SubscriptionQueryVM model;

        public SubscriptionMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new SubscriptionQueryFacade(this);
            _facadeCatetory = new SubscriptionCategoryQueryFacade(this);
            filter = new SubscriptionQueryFilter();
            model = new SubscriptionQueryVM();
            model.ChannelID = "1";
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            QuerySection.DataContext = model;
            _facadeCatetory.QuerySubscriptionCategory((s, args) =>
           {
               if (args.FaultsHandle())
                   return;
               if (args.Result != null)
               {
                   var subscriptionCategory = new SubscriptionCategory { SubscriptionCategoryName = ResCommonEnum.Enum_All };
                   args.Result.Insert(0, subscriptionCategory);
               }
               lstSubscriptionCategory.ItemsSource = args.Result;
               lstSubscriptionCategory.SelectedIndex = 0;
           });
            base.OnPageLoad(sender, e);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            facade.QuerySubscription(QueryResultGrid.QueryCriteria as SubscriptionQueryFilter, e.PageSize, e.PageIndex, e.SortField, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                QueryResultGrid.ItemsSource = DynamicConverter<SubscriptionQueryVM>.ConvertToVMList<List<SubscriptionQueryVM>>(args.Result.Rows);
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }
        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResKeywords.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<SubscriptionQueryVM, SubscriptionQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }


        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<SubscriptionQueryVM, SubscriptionQueryFilter>();
                if (lstSubscriptionCategory.SelectedValue != null)
                {
                    filter.SubscriptionCategoryID = Convert.ToInt32(lstSubscriptionCategory.SelectedValue);
                }
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SubscriptionQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

    }

}
