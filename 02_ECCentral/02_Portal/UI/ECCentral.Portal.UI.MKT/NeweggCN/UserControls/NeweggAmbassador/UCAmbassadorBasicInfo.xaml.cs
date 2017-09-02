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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.NeweggCN.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.NeweggCN.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.NeweggCN.UserControls
{
    public partial class UCAmbassadorBasicInfo : UserControl
    {
        private NeweggAmbassadorQueryVM _CurrentQueryVM = new NeweggAmbassadorQueryVM();


        public UCAmbassadorBasicInfo()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(UCAmbassadorBasicInfo_Loaded);

        }

        void UCAmbassadorBasicInfo_Loaded(object sender, RoutedEventArgs e)
        {
            this.Grid_QueryGrid.DataContext = _CurrentQueryVM;
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

            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);

            if (facade != null)
            {
                _CurrentQueryVM.AreaSysNo=_CurrentQueryVM.GetAreaSysNo();
                facade.QueryAmbassadorBasicInfo(_CurrentQueryVM, p, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    var rows = args.Result.Rows.ToList();

                    this.Grid_AmbassadorBasicInfoList.ItemsSource = rows;
                    this.Grid_AmbassadorBasicInfoList.TotalCount = args.Result.TotalCount;
                });
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Grid_AmbassadorBasicInfoList.Bind();
        }

        /// <summary>
        /// 全选CheckBox打钩时。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="targetGrid"></param>
        private void OnCheckAllCheckBoxChecked(object sender, Newegg.Oversea.Silverlight.Controls.Data.DataGrid targetGrid)
        {
            var checkBoxAll = sender as CheckBox;
            if (targetGrid == null || targetGrid.ItemsSource == null || checkBoxAll == null)
                return;

            dynamic items = targetGrid.ItemsSource as dynamic;

            if (items == null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                dynamic item = items[i];

                item.IsChecked = checkBoxAll.IsChecked ?? false;
            }
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            OnCheckAllCheckBoxChecked(sender, this.Grid_AmbassadorBasicInfoList);
        }

        private void HLB_AmbassadorDetailLinker_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton linker=sender as HyperlinkButton;
            dynamic data=linker.DataContext as dynamic;

            if(data==null)
                return;

            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.CustomerMaintainUrlFormat, data.CustomerSysNo), null, true);
            
           

        }

        /// <summary>
        /// 获取可修改成目标状态的泰隆优选大使。
        /// </summary>
        /// <param name="targetStatus">欲修改成的状态</param>
        /// <returns></returns>
        private List<NeweggAmbassadorSatusInfo> GetAmbassadorInfosByChangeStatus(AmbassadorStatus targetStatus)
        {
            dynamic items = this.Grid_AmbassadorBasicInfoList.ItemsSource as dynamic;

            if (items == null)
                return null;

            List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos = new List<NeweggAmbassadorSatusInfo>();

            bool isTrue = false;
            for (int i = 0; i < items.Count; i++)
            {
                dynamic item = items[i];

                if (targetStatus == AmbassadorStatus.Active)
                {
                    if (item.IsChecked && (item.CustomerMark == AmbassadorStatus.UnActive || item.CustomerMark == AmbassadorStatus.Canceled))
                    {
                        isTrue = true;
                    }
                }
                else if (targetStatus == AmbassadorStatus.UnActive)
                {
                    if (item.IsChecked && (item.CustomerMark == AmbassadorStatus.Active))
                    {
                        isTrue = true;
                    }
                }
                else if (targetStatus == AmbassadorStatus.Canceled)
                {
                    if (item.IsChecked && (item.CustomerMark == AmbassadorStatus.UnActive))
                    {
                        isTrue = true;
                    }
                }

                if (isTrue)
                {
                    NeweggAmbassadorSatusInfo statusInfo = new NeweggAmbassadorSatusInfo();
                    statusInfo.AmbassadorSysNo = item.CustomerSysNo;
                    statusInfo.OrignCustomerMark = item.CustomerMark;
                    statusInfo.CompanyCode = CPApplication.Current.CompanyCode;

                    neweggAmbassadorStatusInfos.Add(statusInfo);
                }

                isTrue = false;
            }

            return neweggAmbassadorStatusInfos;

        }

        /// <summary>
        /// 获取泰隆优选大使信息，根据SysNo.
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        private dynamic GetAmbassadorItemBySysNo(int sysNo)
        {
            dynamic dataSource = this.Grid_AmbassadorBasicInfoList.ItemsSource as dynamic;
            if (dataSource == null)
                return null;

            foreach (dynamic d in dataSource)
            {
                if (d.CustomerSysNo == sysNo)
                {
                    return d;
                }
            }

            return null;
        }

        /// <summary>
        /// 激活。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_Active_Click(object sender, RoutedEventArgs e)
        {
            List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos = GetAmbassadorInfosByChangeStatus(AmbassadorStatus.Active);
            if (neweggAmbassadorStatusInfos == null || neweggAmbassadorStatusInfos.Count <= 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_NoUnActiveOrCanceledItems);
                return;
            }

            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);
            facade.TryUpdateAmbassadorStatus(neweggAmbassadorStatusInfos, (s, args) =>
            {//尝试更新泰隆优选大使的状态，返回需要确认的泰隆优选大使。
                if (args.FaultsHandle())
                    return;
                if (args.Result == null)
                {
                    return;
                }

                NeweggAmbassadorBatchInfo confirmBatch = args.Result as NeweggAmbassadorBatchInfo;//需要确认更新的泰隆优选大使。

                if (confirmBatch == null || confirmBatch.NeweggAmbassadors == null || confirmBatch.NeweggAmbassadors.Count <= 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_SucessBatchDeal);
                    return;
                }

                List<NeweggAmbassadorSatusInfo> updateNeweggAmbassadors = new List<NeweggAmbassadorSatusInfo>();

                string alertString = "";
                foreach (NeweggAmbassadorSatusInfo ambassadorStatusInfo in confirmBatch.NeweggAmbassadors)
                {
                    dynamic item = GetAmbassadorItemBySysNo(ambassadorStatusInfo.AmbassadorSysNo.Value);
                    if (item == null)
                    {
                        continue;
                    }

                    NeweggAmbassadorSatusInfo swapStatusInfo = new NeweggAmbassadorSatusInfo();
                    swapStatusInfo.AmbassadorSysNo = ambassadorStatusInfo.AmbassadorSysNo;
                    swapStatusInfo.OrignCustomerMark = ambassadorStatusInfo.OrignCustomerMark;
                    swapStatusInfo.CompanyCode = ambassadorStatusInfo.CompanyCode;

                    updateNeweggAmbassadors.Add(swapStatusInfo);

                    alertString += string.Format(ResNeweggAmbassador.Info_IfActive, item.AmbassadorID, item.CanceledTime, item.CanceledPerson);

                }

                CPApplication.Current.CurrentPage.Context.Window.Confirm(ResNeweggAmbassador.Title_Confirm,alertString,(ss,resultArgs) => {//确认更新。
                    if (resultArgs.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                    {
                        facade.MaintainNeweggAmbassadorStatus(updateNeweggAmbassadors, (sss, confirmArgs) =>
                        {
                            if (args.FaultsHandle())
                                return;

                            CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_SucessBatchDeal);

                            this.Grid_AmbassadorBasicInfoList.Bind();
                            return;

                        });

                        this.Grid_AmbassadorBasicInfoList.Bind();
                    }
                  

                }, Newegg.Oversea.Silverlight.Controls.Components.ButtonType.YesNo);

            });

        }

        /// <summary>
        /// 取消激活。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_UnActive_Click(object sender, RoutedEventArgs e)
        {
            List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos = GetAmbassadorInfosByChangeStatus(AmbassadorStatus.UnActive);
            if (neweggAmbassadorStatusInfos == null || neweggAmbassadorStatusInfos.Count <= 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_NoActiveItems);
                return;
            }

            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);



            facade.MaintainNeweggAmbassadorStatus(neweggAmbassadorStatusInfos, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_SucessBatchDeal);

                this.Grid_AmbassadorBasicInfoList.Bind();
                return;

            });

        }
    
        /// <summary>
        /// 取消申请。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_UnApply_Click(object sender, RoutedEventArgs e)
        {
            List<NeweggAmbassadorSatusInfo> neweggAmbassadorStatusInfos = GetAmbassadorInfosByChangeStatus(AmbassadorStatus.Canceled);
            if (neweggAmbassadorStatusInfos == null || neweggAmbassadorStatusInfos.Count <= 0)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_NoUnActiveItems);
                return;
            }

            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);


            //取消申请。
            facade.CancelRequestNeweggAmbassadorStatus(neweggAmbassadorStatusInfos, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNeweggAmbassador.Info_SucessBatchDeal);

                this.Grid_AmbassadorBasicInfoList.Bind();
                return;

            });
        }

        /// <summary>
        /// 导出全部。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_AmbassadorBasicInfoList_ExportAllClick(object sender, EventArgs e)
        {
            NeweggAmbassadorFacade facade = new NeweggAmbassadorFacade(CPApplication.Current.CurrentPage);

            ColumnSet columnSet=new ColumnSet(this.Grid_AmbassadorBasicInfoList);



            facade.ExportAllBasicInfoToExcel(_CurrentQueryVM,new ColumnSet[]{columnSet});
        }
    }
}
