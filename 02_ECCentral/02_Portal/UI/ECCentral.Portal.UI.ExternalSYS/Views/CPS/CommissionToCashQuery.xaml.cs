using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Collections.Generic;
using System.Windows.Controls;
using ECCentral.Portal.UI.ExternalSYS.UserControls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CommissionToCashQuery : PageBase
    {
        #region 属性
        //CommissionToCashQueryVM model;

        #endregion

        #region 初始化加载

        public CommissionToCashQuery()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(CommissionToCashQuery_Loaded);
        }

        void CommissionToCashQuery_Loaded(object sender, RoutedEventArgs e)
        {
            //model = new CommissionToCashQueryVM();
            //this.DataContext = model;
        }

     

        #endregion

        #region 查询绑定
        private void btnCommissionToCashSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            dgCommissionToCashQueryResult.Bind();
        }

        private void dgCommissionToCashQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            //CommissionToCashFacade facade = new CommissionToCashFacade(this);
            //model = (CommissionToCashQueryVM)this.DataContext;

            //PagingInfo p = new PagingInfo
            //{
            //    PageIndex = e.PageIndex,
            //    PageSize = e.PageSize,
            //    SortBy = e.SortField
            //};

            //facade.GetAllCommissionToCash(model, p, (obj, args) =>
            //{
            //    this.dgCommissionToCashQueryResult.ItemsSource = args.Result.Rows.ToList();
            //    this.dgCommissionToCashQueryResult.TotalCount = args.Result.TotalCount;
            //});
        }
        #endregion

        #region 跳转

     
        private void hyperlinkCommissionToCashID_Click(object sender, RoutedEventArgs e)
        {

        }       

        private void hyperlinkOrderSysNo_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

    }
}
