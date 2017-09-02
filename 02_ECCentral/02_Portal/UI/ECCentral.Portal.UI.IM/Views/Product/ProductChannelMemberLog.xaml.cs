using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductChannelMemberLog : PageBase
    {
        #region Const
        ProductChannelMemberQueryVM model;
        List<dynamic> selectSource;
        #endregion

        #region Method

        public ProductChannelMemberLog()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new ProductChannelMemberQueryVM();
            ProductChannelMemberFacade facade = new ProductChannelMemberFacade();
            facade.GetProductChannelMemberInfoList((obj, arg) =>
            {
                if (arg.FaultsHandle()) return;

                model.ChannelList = arg.Result;
                model.ChannelList.Insert(0, new ProductChannelMemberInfo() { SysNo = 0, ChannelName = "所有" });
                this.DataContext = model;
            });

        }

        #region 查询绑定
        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            this.dgProductChannelLogQueryResult.Bind();
        }
        private void dgProductChannelLogQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            ProductChannelMemberFacade facade = new ProductChannelMemberFacade(this);
            model = (ProductChannelMemberQueryVM)this.DataContext;
            facade.GetProductChannelMemberPriceLogs(model, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                var list = new List<dynamic>();
                foreach (var row in args.Result.Rows)
                {
                    list.Add(row);
                }
                //绑定控件上列表
                this.dgProductChannelLogQueryResult.ItemsSource = list;
                //绑定总共有多少行数
                this.dgProductChannelLogQueryResult.TotalCount = args.Result.TotalCount;
            });
        }
        #endregion
        #endregion
    }
}
