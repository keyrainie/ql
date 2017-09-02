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
    public partial class SOInternalMemoSearch : PageBase
    {
        CommonDataFacade m_commonFacade;
        SOInternalMemoFacade m_facade;
        SOInternalMemoQueryFilter m_query;

        public SOInternalMemoSearch()
        {
            InitializeComponent();
            
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            m_commonFacade = new CommonDataFacade(this);
            m_facade = new SOInternalMemoFacade(this);
            spConditions.DataContext = m_query = new SOInternalMemoQueryFilter();
           
            IniPageData();
        }
        private void IniPageData()
        {
            m_facade.QuerySOLogCreater(CPApplication.Current.CompanyCode, (sender, p) =>
            {
                if (p.FaultsHandle()) return;
                p.Result.Insert(0, new CSInfo()
                {
                    UserName = ResCommonEnum.Enum_All
                });
                cmbCreater.ItemsSource = p.Result;
                cmbCreater.SelectedIndex = 0;
            });
            m_facade.QuerySOLogUpdater(CPApplication.Current.CompanyCode, (sender, p) =>
            {
                if (p.FaultsHandle()) return;
                p.Result.Insert(0, new CSInfo()
                {
                    UserName = ResCommonEnum.Enum_All
                });
                cmbOperator.ItemsSource = p.Result;
                cmbOperator.SelectedIndex = 0;
            });

            //状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<SOInternalMemoStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedValue = SOInternalMemoStatus.FollowUp;

            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_SOCallType, CodeNamePairAppendItemType.All, (o, p) =>
            {
                this.cmbCallType.ItemsSource = p.Result;
                this.cmbCallType.SelectedIndex = 0;
            });

            CodeNamePairHelper.GetList(ConstValue.DomainName_SO, ConstValue.Key_CallBackDegree, CodeNamePairAppendItemType.All, (o, p) =>
            {
                this.cmbImportance.ItemsSource = p.Result;
                this.cmbImportance.SelectedIndex = 0;
            });

            m_commonFacade.GetAllEffectiveDepartment(CPApplication.Current.CompanyCode, CPApplication.Current.LanguageCode, (sender, p) =>
            {
                if (p.FaultsHandle()) return;
                p.Result.Insert(0, new DepartmentInfo() { DisplayName = ResCommonEnum.Enum_All });
                cmbResponsibleDep.ItemsSource = p.Result;
                cmbResponsibleDep.SelectedIndex = 0;
            });

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOInternalMemo(m_query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                dataGrid.TotalCount = args.Result.TotalCount;
                dataGrid.ItemsSource = args.Result.Rows.ToList("IsCheck", false);
            });
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dataGrid.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                {
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
                }
            }
        }

        private void btnAssign_Click(object sender, RoutedEventArgs e)
        {
            List<DynamicXml> list = new List<DynamicXml>();
            var dynamic = this.dataGrid.ItemsSource as dynamic;
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

            SOCSAssign ctrl = new SOCSAssign();
            ctrl.Dialog = Window.ShowDialog(ResSOInternalMemo.Header_Assign, ctrl, (s, args) =>
                           {
                               if (args.DialogResult == DialogResultType.OK && args.Data != null)
                               {
                                   //进行派发操作
                                   List<SOInternalMemoInfo> req = new List<SOInternalMemoInfo>();
                                   foreach (var item in list)
                                   {
                                       var soInfoVm = DynamicConverter<SOInternalMemoInfoVM>.ConvertToVM(item, "SourceSysNo", "Importance");
                                       soInfoVm.OperatorSysNo = (int)args.Data;
                                       req.Add(soInfoVm.ConvertVM<SOInternalMemoInfoVM, SOInternalMemoInfo>());
                                   }
                                   m_facade.Assign(req, (o, ar) => {
                                       ar.FaultsHandle();
                                       dataGrid.Bind();
                                   });
                               }
                           }
                           , new Size(300, 155)
                    );
        }

        private void btnCancelAssign_Click(object sender, RoutedEventArgs e)
        {
            List<DynamicXml> list = new List<DynamicXml>();
            var dynamic = this.dataGrid.ItemsSource as dynamic;
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
            m_facade.CancelAssign(list.Select(p => new SOInternalMemoInfo() { SysNo = (int)p["SysNo"] }).ToList(), (s, args) =>
            {
                args.FaultsHandle();
                dataGrid.Bind();
            });
        }

        private void hlbtnNeedFollow_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            DynamicXml info = dataGrid.SelectedItem as DynamicXml;
            int soSysNo = (int)info["SOSysNo"];
            publicProcess content = new publicProcess(this, soSysNo);
            content.Width = 800D;
            content.Height = 500D;
            IDialog dialog = this.Window.ShowDialog(String.Format("{0}{1}", ResSO.UC_Title_SOMemoProcessor, soSysNo), content, (obj, args) =>
            {
                dataGrid.Bind();
            });
            content.Dialog = dialog;
        }

        private void hlbtnClose_Click(object sender, RoutedEventArgs e)
        {
            DynamicXml selectedModel = this.dataGrid.SelectedItem as DynamicXml;
            if (null != selectedModel)
            {
                publicMemoClose ctrl = new publicMemoClose((int)selectedModel["SysNo"]);
                ctrl.Dialog = Window.ShowDialog(
                           ResSOInternalMemo.Header_CloseLog
                           , ctrl
                           , (s, args) =>
                           {
                               if (args.DialogResult == DialogResultType.OK)
                               {
                                   dataGrid.PageIndex = 0;
                                   dataGrid.SelectedIndex = -1;
                                   dataGrid.Bind();
                               }
                           }
                           , new Size(520, 360)
                    );
            }
        }

        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            //连接到SO详细页
            DynamicXml info = this.dataGrid.SelectedItem as DynamicXml;
            if (info != null)
            {
                Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, info["SOSysNo"]), null, true);
            }
        }

        private void dataGrid_ExportAllClick(object sender, EventArgs e)
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
            ColumnSet col = new ColumnSet(dataGrid);
            facade.ExportSOInternalMemo(m_query, new ColumnSet[] { col });
        }
    }
}
