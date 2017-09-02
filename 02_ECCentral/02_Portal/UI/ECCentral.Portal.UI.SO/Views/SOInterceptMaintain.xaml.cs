using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class SOInterceptMaintain : PageBase
    {

        CommonDataFacade m_commonFacade;
        SOInterceptFacade m_Facade;
        SOQueryFacade m_QueryFacde;
        SOInterceptQueryFilter m_QueryFilter;

        public SOInterceptMaintain()
        {
            InitializeComponent();            
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            m_commonFacade = new CommonDataFacade(this);
            m_Facade = new SOInterceptFacade(this);
            m_QueryFacde = new SOQueryFacade(this);
            spConditions.DataContext = m_QueryFilter = new SOInterceptQueryFilter();
            IniPageData();
        }

        private void IniPageData()
        {
            // 读取配置(ECCentral.Service.WebHost  --> SO.zh-cn.config) 初始化配送方式过滤 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_ShipTypeFilter, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbShipTypeEnum.ItemsSource = e.Result;
                    cmbShipTypeEnum.SelectedIndex = 0;
                }
            });

            // 读取配置(ECCentral.Service.WebHost  --> SO.zh-cn.config) 初始化有无运单号 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_HasTrackingNumber, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbHasTrackingNumbe.ItemsSource = e.Result;
                    cmbHasTrackingNumbe.SelectedIndex = 0;
                }
            });

            // 读取配置(ECCentral.Service.WebHost  --> SO.zh-cn.config) 初始化 是否当天配送 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_ShipTimeType, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbShipTimeType.ItemsSource = e.Result;
                    cmbShipTimeType.SelectedIndex = 0;
                }
            });            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchResultsDataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            m_QueryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOIntercept(m_QueryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                searchResultsDataGrid.TotalCount = args.Result.TotalCount;
                searchResultsDataGrid.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
            });
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.searchResultsDataGrid.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsCheck = ckb.IsChecked.Value ? true : false;
            }
        }

        private void btnBatchUpdate_Click(object sender, RoutedEventArgs e)
        {
            string sysNoList = string.Empty;
            List<DynamicXml> list = new List<DynamicXml>();
            var dynamic = this.searchResultsDataGrid.ItemsSource as dynamic;
            if (dynamic != null)
            {
                foreach (var item in dynamic)
                {
                    if (item.IsCheck == true)
                    {
                        list.Add(item);
                    }
                }
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        sysNoList += "," + (int)item["SysNo"];
                    }
                    if (!string.IsNullOrEmpty(sysNoList))
                    {
                        sysNoList = sysNoList.Remove(0, 1);
                    }
                }
                else
                {
                    foreach (var item in dynamic)
                    {
                        sysNoList += "," + (int)item["SysNo"];
                    }
                    if (!string.IsNullOrEmpty(sysNoList))
                    {
                        sysNoList = sysNoList.Remove(0, 1);
                    }
                }
                if (!string.IsNullOrEmpty(sysNoList))
                {
                    UpdatSOIntercept soIntercept = new UpdatSOIntercept(sysNoList);
                    var window = CPApplication.Current.CurrentPage.Context.Window;
                    IDialog dialog = window.ShowDialog(ResSOIntercept.Title_UpdateSOIntercept, soIntercept, (obj, args) =>
                    {
                        if (args.DialogResult == DialogResultType.OK)
                        {
                            searchResultsDataGrid.Bind();
                        }
                    });
                    soIntercept.Dialog = dialog;
                }
            }
            else
            {
                this.Window.Alert(ResSO.Msg_PleaseSelect);
                return;
            }
        }

        /// <summary>
        /// 删除选中的订单拦截信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<DynamicXml> list = new List<DynamicXml>();
            var dynamic = this.searchResultsDataGrid.ItemsSource as dynamic;
            if (dynamic != null)
            {
                foreach (var item in dynamic)
                {
                    if (item.IsCheck == true)
                    {
                        list.Add(item);
                    }
                }
            }
            if (list.Count == 0)
            {
                this.Window.Alert(ResSO.Msg_PleaseSelect);
                return;
            }
            CPApplication.Current.CurrentPage.Context.Window.Confirm(ResSOIntercept.Info_Confirm_DeleteItem, (objConfirm, argsConfirm) =>
            {
                if (argsConfirm.DialogResult == DialogResultType.OK)
                {
                    SOInterceptInfo info = new SOInterceptInfo();
                    foreach (var item in list)
                    {
                        info.Sysnolist+=","+ (int)item["SysNo"];             
                    }
                    if (!string.IsNullOrEmpty(info.Sysnolist))
                    {
                        info.Sysnolist = info.Sysnolist.Remove(0,1);
                        if (!string.IsNullOrEmpty(info.Sysnolist))
                        {
                            new SOInterceptFacade(this).DeleteSOInterceptInfo(info, (obj, args) =>
                            {
                                if (!args.FaultsHandle())
                                {
                                    searchResultsDataGrid.Bind();                                    
                                }
                            });
                        }
                    }
                }
            });
        }
  
        /// <summary>
        /// 添加新拦截配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewCreate_Click(object sender, RoutedEventArgs e)
        {
            AddSOIntercept soIntercept = new AddSOIntercept();
            var window = CPApplication.Current.CurrentPage.Context.Window;
            IDialog dialog = window.ShowDialog(ResSOIntercept.Title_AddSOIntercept, soIntercept, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    searchResultsDataGrid.Bind();
                }
            });
            soIntercept.Dialog = dialog;
        }

        /// <summary>
        /// 设置 配送方式 数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbShipTypeEnum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbShipTypeEnum = (sender) as ComboBox;
            if (!string.IsNullOrEmpty(cmbShipTypeEnum.SelectedValue.ToString()) && cmbShipTypeEnum.SelectedIndex!=0)
            {
                string[] selectedEnum = new string[] { cmbShipTypeEnum.SelectedValue.ToString() };
                if(selectedEnum[0].Contains(","))
                {
                    selectedEnum = new string[2];
                    selectedEnum[0] = "1";
                    selectedEnum[1] = "2";
                }               
                 m_QueryFacde.GetShipTypeList((obj,args)=>
                 {
                   if(!args.FaultsHandle())
                   {
                       List<SOInterceptInfoVM> infoVMList = new List<SOInterceptInfoVM>();
                       if (args.Result != null && args.Result.Count>0)
                       {
                           foreach (var item in args.Result)
                           {
                               if (string.IsNullOrEmpty(item.ShipTypeID))
                               {
                                   item.ShipTypeID = item.SysNo.ToString();
                               }
                               if (selectedEnum.Contains(item.ShipTypeEnum.ToString()) &&  item.IsOnlineShow != HYNStatus.Hide)
                               {
                                   SOInterceptInfoVM infoVM = new SOInterceptInfoVM();
                                   infoVM.ShipTypeSysNo = item.SysNo;
                                   infoVM.ShippingTypeID = item.ShipTypeID;
                                   infoVM.ShippingTypeName = item.ShippingTypeName;
                                   infoVMList.Add(infoVM);
                               }                              
                           }
                           infoVMList.Insert(0, new SOInterceptInfoVM { 
                                   ShippingTypeID = "",
                                   ShippingTypeName = ResCommonEnum.Enum_All
                           });

                           cmbShipTypeSysNo.ItemsSource = infoVMList;
                           cmbShipTypeSysNo.SelectedIndex = 0;
                       }                     
                   }
                 });
            }
            else
            {
                cmbShipTypeSysNo.ItemsSource = null;
            }
        } 
    }
}
