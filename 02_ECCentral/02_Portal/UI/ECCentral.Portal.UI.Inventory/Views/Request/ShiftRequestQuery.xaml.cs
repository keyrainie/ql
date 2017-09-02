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

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.UI.Inventory.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Views.Request;
using ECCentral.Portal.Basic.Converters;


namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ShiftRequestQuery : PageBase
    {

        private ShiftRequestQueryView PageView;
        private ShiftRequestQueryFacade QueryFacade;
        private ShiftRequestMaintainFacade MaintainFacade;
        private ShiftRequestQueryVM ExportQuery;
        //private List<ShiftRequestVM> requestList;

        private const string maintainUrl = "/ECCentral.Portal.UI.Inventory/ConvertRequestMaintain";
        #region 初始化加载

        public ShiftRequestQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            PageView = new ShiftRequestQueryView();
            QueryFacade = new ShiftRequestQueryFacade(this);
            MaintainFacade = new ShiftRequestMaintainFacade(this);
            ExportQuery = new ShiftRequestQueryVM();
            PageView.QueryInfo.CompanyCode = CPApplication.Current.CompanyCode;
            expanderCondition.DataContext = PageView.QueryInfo;
            dgShiftRequestQueryResult.DataContext = PageView;

            CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory, ConstValue.Key_ShiftShippingType, CodeNamePairAppendItemType.All,
                (obj, args) =>
                {
                    if (!args.FaultsHandle() && args.Result != null)
                    {
                        PageView.QueryInfo.ShiftShippingTypeList = args.Result;
                    }
                });

            QueryFacade.QueryShiftRequestCreateUserList((totalCount, vmList) => {
                vmList.Insert(0 ,new UserInfoVM() { 
                    SysNo = null,
                    UserDisplayName = ResShiftRequestQuery.ComboItem_All
                });

                PageView.QueryInfo.CreateUserList = vmList;                
            });

            btnTotal.IsEnabled = btnBatchSpecial.IsEnabled = btnCancelBatchSpecial.IsEnabled
                    = btnBatchLog.IsEnabled = false;

            btnShiftRequestQueryNew.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestQuery_NavigateCreate);  
        }

        #endregion

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ValidationManager.Validate(expanderCondition);
            if (PageView.QueryInfo.HasValidationErrors)
            {
                return;
            }
                        //查询:
            if (PageView.QueryInfo.ProductSysNo.HasValue)
            {
                ShiftRequestQueryFilter queryFilter = new ShiftRequestQueryFilter();
                queryFilter.ProductSysNo = PageView.QueryInfo.ProductSysNo;
                queryFilter.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
                queryFilter.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                queryFilter.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
            }
            dgShiftRequestQueryResult.Bind();
        }

        private void dgShiftRequestQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            txtTotal.Text = string.Empty;
            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            #region 获取自己能访问到的PM
            PageView.QueryInfo.UserSysNo = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.UserSysNo;
            PageView.QueryInfo.UserName = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.LoginUser.LoginName;
            PageView.QueryInfo.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_SeniorPM_Query))
            {
                PageView.QueryInfo.PMQueryRightType = BizEntity.Common.PMQueryType.AllValid;
            }
            else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_IntermediatePM_Query))
            {
                PageView.QueryInfo.PMQueryRightType = BizEntity.Common.PMQueryType.Team;
            }
            else if (AuthMgr.HasFunctionAbsolute(AuthKeyConst.IM_JuniorPM_Query))
            {
                PageView.QueryInfo.PMQueryRightType = BizEntity.Common.PMQueryType.Self;
            }
            else
            {
                PageView.QueryInfo.PMQueryRightType = null;
            }

            #endregion

            ExportQuery = PageView.QueryInfo.DeepCopy();
            QueryFacade.QueryShiftRequest(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.Result = DynamicConverter<ShiftRequestVM>.ConvertToVMList(vmList);                
                PageView.TotalCount = totalCount;                
                dgShiftRequestQueryResult.ItemsSource = PageView.Result;
                dgShiftRequestQueryResult.TotalCount = PageView.TotalCount;
                btnTotal.IsEnabled = btnBatchSpecial.IsEnabled = btnCancelBatchSpecial.IsEnabled 
                    = btnBatchLog.IsEnabled = totalCount > 0;
                if (totalCount > 0)
                {
                    QueryFacade.QueryShiftDataCount(PageView.QueryInfo, (countSender,countArgs) => {
                        if (countArgs.FaultsHandle())
                        {
                            return;
                        }

                        var listAll = countArgs.Result.ToList();
                        if (listAll.Count > 0)
                        {
                            int outStockSheetQuantity = 0;
                            int inStockSheetQuantity = 0;
                            decimal outStockAmount = 0;
                            decimal inStockAmount = 0;
                            int partlyInStockSheetQuantity = 0;
                            decimal partlyOutStockAmount = 0;
                            decimal partlyInStockAmount = 0;
                            decimal partlyTransferAmount = 0;

                            foreach (var item in listAll[0].Rows)
                            {
                                if (((int)item["status"]) == 3)
                                {
                                    outStockSheetQuantity = (int)item["TotalNum"];
                                    outStockAmount = (decimal)item["TotalCost"];
                                }
                                //in stock
                                else
                                {
                                    inStockSheetQuantity = (int)item["TotalNum"];
                                    inStockAmount = (decimal)item["TotalCost"];
                                }
                            }
                            if (listAll.Count > 1 && listAll[1].Rows.ToList().Count > 0)
                            {
                                partlyInStockSheetQuantity = (int)listAll[1].Rows[0]["partlyNum"];
                                partlyInStockAmount = (decimal)listAll[1].Rows[0]["inCost"];
                                partlyOutStockAmount = (decimal)listAll[1].Rows[0]["outCost"];
                                partlyTransferAmount = (decimal)listAll[1].Rows[0]["inWayCost"];
                            }
                            txtTotal.Text = string.Format(ResShiftRequestQuery.Msg_Format_CountData
                                                        , outStockSheetQuantity
                                                        , MoneyConverter.ConvertToString(outStockAmount)
                                                        , inStockSheetQuantity
                                                        , MoneyConverter.ConvertToString(inStockAmount)
                                                        , partlyInStockSheetQuantity
                                                        , MoneyConverter.ConvertToString(partlyOutStockAmount)
                                                        , MoneyConverter.ConvertToString(partlyInStockAmount)
                                                        , MoneyConverter.ConvertToString(partlyTransferAmount)
                                                        );
                        }
                    });
                }

                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestQuery_Total))
                {
                    btnTotal.IsEnabled = false;
                }
                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestQuery_BatchSpecial))
                {
                    btnBatchSpecial.IsEnabled = false;
                }
                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestQuery_CancelBatchSpecial))
                {
                    btnCancelBatchSpecial.IsEnabled = false;
                }
                if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestQuery_BatchLog))
                {
                    btnBatchLog.IsEnabled = false;
                }
            });
        }

        private void btnNewRequest_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.Inventory_ShiftRequestMaintainCreateFormat, null, true);
        }

        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            Window.Navigate(String.Format(ConstValue.Inventory_ShiftRequestMaintainUrlFormat, btn.CommandParameter), null, true);
        }

        private void btnTotal_Click(object sender, RoutedEventArgs e)
        {
            int checkedCount = 0;
            decimal totalAmount = 0.00m;
            decimal totalFee = 0.00m;
            decimal baseRate = 0.003m;
            var checkedItems = PageView.Result.Where(x => x.IsChecked == true);
            if (checkedItems == null || checkedItems.Count() == 0)
            {
                Window.Alert("请选择需要操作的数据行.");
                return;
            }

            checkedCount = checkedItems.Count();

            foreach (ShiftRequestVM item in checkedItems)
            {
                totalAmount += (decimal)item.TotalAmount;
            }

            totalFee = totalAmount * baseRate;
            Window.Alert(string.Format(ResShiftRequestQuery.Msg_Format_Stat, checkedCount, Math.Round(totalAmount,2), Math.Round(totalFee,2)));

        }

        private void btnBatchSpecial_Click(object sender, RoutedEventArgs e)
        {
            List<int> requestSysNoList = GetSelected();
            if (requestSysNoList.Count <= 0)
            {
                Window.Alert("请选择需要操作的数据行.");
                return;
            }

            MaintainFacade.SetSpecialShiftType(requestSysNoList, (vmList) => {
                Window.Alert("操作已成功.");
                dgShiftRequestQueryResult.Bind();
            });
        }

        private void btnCancelBatchSpecial_Click(object sender, RoutedEventArgs e)
        {
            List<int> requestSysNoList = GetSelected();
            if (requestSysNoList.Count <= 0)
            {
                Window.Alert("请选择需要操作的数据行.");
                return;
            }

            MaintainFacade.CancelSpecialShiftType(requestSysNoList, (vmList) =>
            {
                Window.Alert("操作已成功.");
                dgShiftRequestQueryResult.Bind();
            });
        }     

        private void cbSelectAll_Click(object sender, RoutedEventArgs e)
        {
            //PageView.Result.ForEach(d =>
            //{
            //    d.IsChecked = cbDataGridSelectAll.IsChecked;
            //});

             CheckBox chk = sender as CheckBox;
             if (null != chk)
             {
                 if (null != this.dgShiftRequestQueryResult.ItemsSource)
                 {
                     foreach (var item in this.dgShiftRequestQueryResult.ItemsSource)
                     {
                         if (item is ShiftRequestVM)
                         {
                             if (chk.IsChecked == true)
                             {
                                 if (!((ShiftRequestVM)item).IsChecked)
                                 {
                                     ((ShiftRequestVM)item).IsChecked = true;
                                 }
                             }
                             else
                             {
                                 if (((ShiftRequestVM)item).IsChecked)
                                 {
                                     ((ShiftRequestVM)item).IsChecked = false;
                                 }
                             }

                         }
                     }
                 }
             }
        }

        private List<int> GetSelected()
        {
            List<int> sysNoList = new List<int>();
            PageView.Result.ForEach(d =>
            {
                if (d.IsChecked)
                {
                    sysNoList.Add((int)d.SysNo);
                }
            });
            return sysNoList;
        }

        private void btnBatchLog_Click(object sender, RoutedEventArgs e)
        {
            List<int> requestSysNoList = GetSelected();
            if (requestSysNoList.Count <= 0)
            {
                Window.Alert("请选择需要操作的数据行.");
                return;
            }
            ShiftRequestMemo content = new ShiftRequestMemo
            {
                Page = this,
                RequestSysNoList = GetSelected()
            };
            content.Dialog = Window.ShowDialog("添加跟踪日志", content);
        }

        private void dgShiftRequestQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestQuery_ExportExcell))
            {
                Window.Alert("对不起，你没有权限进行此操作！");
                return;
            }
            if (this.dgShiftRequestQueryResult == null || this.dgShiftRequestQueryResult.TotalCount == 0)
            {
                Window.Alert("没有可供导出的数据!");
                return;
            }
            ColumnSet col = new ColumnSet(dgShiftRequestQueryResult);
            ExportQuery.PagingInfo.PageSize = dgShiftRequestQueryResult.TotalCount;
            QueryFacade.Export(ExportQuery, new ColumnSet[] { col });
        }
    }

}
