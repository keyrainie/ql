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
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class OrderManagement : PageBase
    {
        #region 属性
        public OrderQueryVM FilterVM
        {
            get
            {
                return this.expSearchCondition.DataContext as OrderQueryVM;
            }
            set
            {
                this.expSearchCondition.DataContext = value;
            }
        }

        private bool islink = false; //是否是其他页面链接过来的
        #endregion

        #region 初始化加载

        public OrderManagement()
        {
            InitializeComponent();

            this.Loaded += (sender, e) => 
            {
                OrderFacade service = new OrderFacade(this);
                this.FilterVM = new OrderQueryVM();
                if(!string.IsNullOrEmpty(Request.Param))
                {
                    islink = true;
                    FilterVM.OrderSysNoList = Request.Param;
                    this.dgOrderQueryResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<OrderQueryVM>(this.FilterVM);
                    this.dgOrderQueryResult.Bind();
                }
              };
        }

       



        #endregion

        #region 查询绑定
        private void btnOrderSearch_Click(object sender, RoutedEventArgs e)
        {

            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (this.FilterVM.OrderSysNoList.Contains("，"))
            {
                Window.MessageBox.Show(@"不能输入中文"",""",MessageBoxType.Error);
                return;
            }
            this.dgOrderQueryResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<OrderQueryVM>(this.FilterVM);
            this.dgOrderQueryResult.Bind();
        }

        private void dgOrderQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            if (Newegg.Oversea.Silverlight.Utilities.Validation.ValidationManager.Validate(this.SeachBuilder))
            {
                new OrderFacade(this).Query(this.dgOrderQueryResult.QueryCriteria as OrderQueryVM, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
                {
                    if (!args.FaultsHandle())
                    {
                        if (islink)
                        {
                            List<DateTime> list = new List<DateTime>();
                            foreach (var item in args.Result.Rows)
                            {
                                list.Add(item.SettlementDate);
                            }
                            list.Sort();
                            if (list.Count > 0)
                            {
                                FilterVM.SettlementDateBegin = list[0];
                                FilterVM.SettlementDateEnd = list[list.Count - 1];
                            }
                        }
                        this.dgOrderQueryResult.ItemsSource = args.Result.Rows.ToList();
                        this.dgOrderQueryResult.TotalCount = args.Result.TotalCount;
                    }
                });
            }
        }
        #endregion


    }
}
