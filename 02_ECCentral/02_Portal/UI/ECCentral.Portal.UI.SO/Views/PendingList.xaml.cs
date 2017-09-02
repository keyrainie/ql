using System;
using System.Windows;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.SO.Views
{
    [View]
    public partial class PendingList : PageBase
    {
        CommonDataFacade m_commonFacade;
        SOPendingFacade m_facade;
        SOPendingQueryFilter m_query;

        public PendingList()
        {
            InitializeComponent();
            
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            m_commonFacade = new CommonDataFacade(this);
            m_facade = new SOPendingFacade(this);
            spConditions.DataContext = m_query = new SOPendingQueryFilter();
            IniPageData();
        }
        private void IniPageData()
        {
            m_commonFacade.GetStockList(true, (sender, e) =>
            {
                if (e.FaultsHandle()) return;
                cmbStock.ItemsSource = e.Result;
                cmbStock.SelectedIndex = 0;
            });

            cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOPendingStatus>(EnumConverter.EnumAppendItemType.All);
            cmbStatus.SelectedIndex = 0;

            //处理人
            m_commonFacade.GetUserInfoByDepartmentId(101, (o, p) =>
            {
                if (p.FaultsHandle()) return;
                var list = p.Result;
                list.Insert(0, new UserInfo()
                {
                    UserDisplayName = ResCommonEnum.Enum_All
                });
                this.cmbUpdateUser.ItemsSource = list;
                this.cmbUpdateUser.SelectedIndex = 0;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGridPendingList.Bind();
        }

        private void dataGridPendingList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOPending(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGridPendingList.TotalCount = args.Result.TotalCount;
                dataGridPendingList.ItemsSource = args.Result.Rows;
            });
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            //连接到SO详细页
            DynamicXml selectedModel = this.dataGridPendingList.SelectedItem as DynamicXml;

            if (selectedModel != null)
            {
                //连接到SO详细页
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, selectedModel["SOSysNo"]), null, true);
            }
        }

        #region 子操作事件

        //修改SO
        private void hlbtnModifySO_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGridPendingList.SelectedItem as DynamicXml;

            if (selectedModel != null)
            {
                //连接到SO详细页
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, selectedModel["SOSysNo"]), null, true);
            }
        }
        
        //改单SO
        private void hlbtnUpdateSO_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGridPendingList.SelectedItem as DynamicXml;

            if (selectedModel != null)
            {
                Window.Confirm(ResSO.Msg_ConfirmUpdatePending, (o, p) =>
                {
                    if (p.DialogResult == DialogResultType.OK)
                    {
                        m_facade.UpdateSOPending((int)selectedModel["SOSysNo"], (oo, pp) =>
                        {
                            if (!pp.FaultsHandle())
                            {
                                dataGridPendingList.Bind();
                            }
                        });
                    }
                });
            }
        }

        //关闭
        private void hlbtnCloseSO_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGridPendingList.SelectedItem as DynamicXml;

            if (selectedModel != null)
            {
                Window.Confirm(ResSO.Msg_ConfirmClosePending, (o, p) =>
                {
                    if (p.DialogResult == DialogResultType.OK)
                    {
                        //将数据转为ViewModel
                        m_facade.CloseSOPending((int)selectedModel["SOSysNo"], (oo, pp) =>
                        {
                            if (!pp.FaultsHandle())
                            {
                                dataGridPendingList.Bind();
                            }
                        });
                    }
                });
            }
        }

        //打开
        private void hlbtnOpenSO_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGridPendingList.SelectedItem as DynamicXml;

            if (selectedModel != null)
            {
                Window.Confirm(ResSO.Msg_ConfirmOpenPending, (o, p) =>
                {
                    if (p.DialogResult == DialogResultType.OK)
                    {
                        //将数据转为ViewModel
                        m_facade.OpenSOPending((int)selectedModel["SOSysNo"], (oo, pp) =>
                        {
                            if (!pp.FaultsHandle())
                            {
                                dataGridPendingList.Bind();
                            }
                        });
                    }
                });
            }
        }

        #endregion

        //导出数据到Excel
        private void dataGridPendingList_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.SO_ExcelExport))
            {
                Window.Alert(ResSO.Msg_Error_Right, MessageType.Error);
                return;
            }

            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            ColumnSet col = new ColumnSet(dataGridPendingList);
            facade.ExportSOPending(m_query, new ColumnSet[] { col });
        }
    }

}
