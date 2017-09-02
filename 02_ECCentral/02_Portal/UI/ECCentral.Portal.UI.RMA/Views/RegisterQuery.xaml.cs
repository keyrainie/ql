using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class RegisterQuery : PageBase
    {
        private RegisterFacade facade;
        private RegisterQueryVM queryVM;
        //private RegisterQueryVM lastQueryVM;
        //private RegisterQueryVM tempVM;
        private CommonDataFacade commonFacade;
        public RegisterQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            queryVM = new RegisterQueryVM();
            facade = new RegisterFacade(this);
            commonFacade = new CommonDataFacade(this);
            LoadComboBoxData();
            this.QueryFilter.DataContext = queryVM;
            base.OnPageLoad(sender, e);
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                int ProductSysNo;
                if (int.TryParse(this.Request.QueryString["ProductSysNo"], out ProductSysNo))
                {
                    queryVM.ProductSysNo = ProductSysNo;
                    queryVM.ProductID = this.Request.QueryString["ProductID"];
                    this.DataGrid_ResultList.Bind();

                }

            }
        }
        private void LoadComboBoxData()
        {
            ComBox_CompareSymbol.SelectedIndex = 0;
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "NextHandlerList", CodeNamePairAppendItemType.None, (obj, args) =>
            {
                foreach (var item in args.Result as List<CodeNamePair>)
                {
                    queryVM.CheckBoxList.Add(new CheckBoxVM() { IsChecked = false, Name = item.Name, Code = item.Code });
                }
            });
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "SellerOperationInfo", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                this.queryVM.SellerOperationInfoList = args.Result;
            });
            CodeNamePairHelper.GetList(ConstValue.DomainName_RMA, "RMAReason", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                this.queryVM.RMAReasonList = args.Result;
            });
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.RMA_EditRegisterUrl);
        }

        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this.QueryFilter.DataContext = queryVM;
            facade.Query(queryVM,
                e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.DataGrid_ResultList.ItemsSource = args.Result.Rows;
                this.DataGrid_ResultList.TotalCount = args.Result.TotalCount;
            });
        }

        private void ckb_MoreQueryBuilder_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_MoreQueryBuilder.IsChecked.HasValue && ckb_MoreQueryBuilder.IsChecked.Value)
            {
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                spMoreQueryBuilder.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(this.QueryFilter);
            if (queryVM.HasValidationErrors)
            {
                return;
            }
            queryVM.IsQuickSearch = false;
            queryVM.PMUserSysNo = (int?)this.ucPMPicker.SelectedPMSysNo;
            List<string> li = new List<string>();
            foreach (var item in queryVM.CheckBoxList)
            {
                if (item.IsChecked)
                {
                    //Query时条件为 Dunlog.Feedback IN ('联系不上','尚未收到') 
                    li.Add("'" + item.Code + "'");
                }
            }
            if (li.Count > 0)
            {
                string nextHandlerList = string.Empty;
                nextHandlerList += string.Join(",", li.ToArray());
                queryVM.NextHandlerList = nextHandlerList;
            }

            // Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RegisterQueryVM>(queryVM);
            if (this.ckb_MoreQueryBuilder.IsChecked.Value)
            {
               // tempVM = lastQueryVM;
            }
            else
            {
                IsNormalQuery();
              //  tempVM = lastQueryVM;
            }          
            this.DataGrid_ResultList.Bind();
        }

        private void IsNormalQuery()
        {
            queryVM.CreateTimeFrom =  null;
            queryVM.RecvTimeFrom =   null;
            queryVM.CheckTimeFrom =  null;
            queryVM.OutboundTimeFrom = null;
            queryVM.ResponseTimeFrom =  null;
            queryVM.RefundTimeFrom = null;
            queryVM.ReturnTimeFrom =  null;
            queryVM.RevertTimeFrom = null;
            queryVM.RevertStatus = null;
            queryVM.NewProductStatus = null;
            queryVM.OutBoundStatus = null;
            queryVM.ReturnStatus = null;
            queryVM.RequestStatus = null;
            queryVM.RefundStatus = null;
            queryVM.PMUserSysNo = null;
            queryVM.NextHandler = null;
            queryVM.IsVIP = null;
            queryVM.IsWithin7Days = null;
            queryVM.RMAReason = null;
            queryVM.IsRecommendRefund = null;
            queryVM.IsRepeatRegister = null;
            queryVM.ProductCount = string.Empty;
        }

        private void Combox_NextHandler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NextHandlerList.Visibility = queryVM.NextHandler == RMANextHandler.Dun ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ComBox_IsRepeatRegister_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ComBox_CompareSymbol.IsEnabled = this.TextBox_ProductCount.IsEnabled = queryVM.IsRepeatRegister == true ? true : false;
        }

        private void Button_QuickSearch_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string commandParameter = btn.CommandParameter.ToString();          
            //清空普通和高级条件
            queryVM = new RegisterQueryVM { IsMoreQueryBuilderCheck = queryVM.IsMoreQueryBuilderCheck};
            LoadComboBoxData();
            if (!queryVM.IsMoreQueryBuilderCheck)
            {             
                IsNormalQuery();             
            }      
           
            switch (commandParameter)
            {
                case "Within21Days":
                    queryVM.QuickSearchCondition.IsWithin7Days = null;
                    queryVM.QuickSearchCondition.RecvTimeFromDiffDays = -21;
                    queryVM.QuickSearchCondition.RecvTimeToDiffDays = 0;
                    queryVM.QuickSearchCondition.RequestStatus = RMARequestStatus.Handling;
                    //刷新界面
                    queryVM.RecvTimeFrom = DateTime.Today.AddDays(-20);
                    queryVM.RecvTimeTo = DateTime.Today.AddDays(1);                  
                    queryVM.RequestStatus = RMARequestStatus.Handling;
                    break;
                case "Over21Days":
                    queryVM.QuickSearchCondition.IsWithin7Days = null;
                    queryVM.QuickSearchCondition.RecvTimeFromDiffDays = null;
                    queryVM.QuickSearchCondition.RecvTimeToDiffDays = -21;
                    queryVM.QuickSearchCondition.RequestStatus = RMARequestStatus.Handling;                
                    //刷新界面         
                   // queryVM.RecvTimeFrom = DateTime.MinValue;
                    queryVM.RecvTimeTo = DateTime.Today.AddDays(1);
                    queryVM.RequestStatus = RMARequestStatus.Handling;
                    break;
                case "7DaysWithin9Days":
                    queryVM.QuickSearchCondition.IsWithin7Days = true;
                    queryVM.QuickSearchCondition.RecvTimeFromDiffDays = -9;
                    queryVM.QuickSearchCondition.RecvTimeToDiffDays = 0;
                    queryVM.QuickSearchCondition.RequestStatus = RMARequestStatus.Handling;
                    //刷新界面
                    queryVM.RecvTimeFrom = DateTime.Today.AddDays(-8);
                    queryVM.RecvTimeTo = DateTime.Today.AddDays(1);
                    queryVM.RequestStatus = RMARequestStatus.Handling;
                    queryVM.IsWithin7Days = true;
                    break;
                case "7DaysOver9Days":
                    queryVM.QuickSearchCondition.IsWithin7Days = true;
                    queryVM.QuickSearchCondition.RecvTimeFromDiffDays = null;
                    queryVM.QuickSearchCondition.RecvTimeToDiffDays = -9;
                    queryVM.QuickSearchCondition.RequestStatus = RMARequestStatus.Handling;
                    //刷新界面                
                   // queryVM.RecvTimeFrom = DateTime.MinValue;
                    queryVM.RecvTimeTo = DateTime.Today.AddDays(1);
                    queryVM.RequestStatus = RMARequestStatus.Handling;
                    queryVM.IsWithin7Days = true;
                    break;
            }
            queryVM.IsQuickSearch = true;
           // lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<RegisterQueryVM>(queryVM);
           
            this.DataGrid_ResultList.Bind();
        }

        private void btnEditRegister_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.RMA_EditRegisterUrl, vm.SysNo);
            Window.Navigate(url, null, true);

        }

        private void btnEditSO_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, vm.SOSysNo);
            Window.Navigate(url, null, true);

        }

        private void btnEditRequest_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.RMA_RequestMaintainUrl, vm.RequestSysNo);
            Window.Navigate(url, null, true);

        }

        private void btnProductID_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            // 链接到website 产品页面
            //Ocean.20130514, Move to ControlPanelConfiguration
            string urlFormat = this.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_WebSiteProductPreviewUrl);
            UtilityHelper.OpenWebPage(string.Format(urlFormat, vm.ProductSysNo));
        }

        private void btnProductName_Click(object sender, RoutedEventArgs e)
        {
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            string url = string.Format(ConstValue.IM_ProductPurchaseHistoryUrlFormat, vm.ProductSysNo);
            Window.Navigate(url, null, true);

        }

        private void DataGrid_ResultList_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.RMA_Register_Export))
            {
                Window.Alert(ResRegister.Msg_AuthError);
                return;
            }
            if (queryVM == null || this.DataGrid_ResultList.TotalCount < 1)
            {
                Window.Alert(ResRegister.Msg_ExportError);
                return;
            }
            if (this.DataGrid_ResultList.TotalCount > 10000) 
            {
                Window.Alert(ResRegister.Msg_ExportExceedsLimitCount);
                return;
            }

            ColumnSet colSet = new ColumnSet(this.DataGrid_ResultList, true);
            facade.ExportRegisterExcelFile(queryVM, new ColumnSet[] { colSet });
        }
    }
}
