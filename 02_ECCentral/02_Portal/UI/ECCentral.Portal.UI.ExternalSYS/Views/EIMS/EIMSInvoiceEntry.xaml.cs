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
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using ECCentral.Portal.UI.ExternalSYS.Resources;
using ECCentral.Portal.UI.ExternalSYS.Models;
using ECCentral.Portal.UI.ExternalSYS.UserControls.EIMS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Views
{
    [View(IsSingleton = true)]
    public partial class EIMSInvoiceEntry : PageBase
    {
        #region 页面初始化
        EIMSInvoiceEntryQueryFilter m_QueryFilter;
        EIMSOrderMgmtFacade m_QueryFacde;
        List<EIMSInvoiceEntryVM> viewList;

        private int currentPageSize;

        public int CurrentPageSize
        {
            get { return currentPageSize; }
            set { currentPageSize = value; }
        }
        private int currentPageIndex;

        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
            set { currentPageIndex = value; }
        }

        public EIMSInvoiceEntry()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            m_QueryFacde = new EIMSOrderMgmtFacade(this);
            viewList = new List<EIMSInvoiceEntryVM>();
            spConditions.DataContext = m_QueryFilter = new EIMSInvoiceEntryQueryFilter();
            IniPageData();
        }

        /// <summary>
        /// 绑定下拉列表
        /// </summary>
        private void IniPageData()
        {
            // 读取配置(ECCentral.Service.WebHost  --> EIMS.zh-cn.config) 初始化  费用类型 过滤 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_EIMSType, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbEIMSType.ItemsSource = e.Result;
                    cmbEIMSType.SelectedIndex = 0;
                }
            });

            // 读取配置(ECCentral.Service.WebHost  --> EIMS.zh-cn.config) 初始化有   收款类型 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ReceivedType, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {

                    List<CodeNamePair> cpList = new List<CodeNamePair>();
                    cpList.Add(new CodeNamePair() { Code = null, Name = ResEIMSInvoiceEntry.Info_All });
                    foreach (var item in e.Result)
                    {
                        if (item.Name == ResEIMSInvoiceEntry.Info_ReceivedType_Cash || item.Name == ResEIMSInvoiceEntry.Info_ReceivedType_ZhangKou)
                        {
                            cpList.Add(item);
                        }
                    }
                    cmbReceiveType.ItemsSource = cpList;
                    cmbReceiveType.SelectedIndex = 0;
                }
            });

            // 读取配置(ECCentral.Service.WebHost  --> EIMS.zh-cn.config) 初始化 单据状态  下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_InvoiceStatus, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    List<CodeNamePair> cpList = new List<CodeNamePair>();
                    cpList.Add(new CodeNamePair() { Code = null, Name = ResEIMSInvoiceEntry.Info_All });
                    foreach (var item in e.Result)
                    {
                        if (item.Name == ResEIMSInvoiceEntry.Info_InvoiceStatus_F || item.Name == ResEIMSInvoiceEntry.Info_InvoiceStatus_O)
                        {
                            cpList.Add(item);
                        }
                    }
                    cmbStatus.ItemsSource = cpList;
                    cmbStatus.SelectedIndex = 0;
                }
            });
            // 读取配置(ECCentral.Service.WebHost  --> EIMS.zh-cn.config) 初始化 发票状态 下拉列表
            CodeNamePairHelper.GetList(ConstValue.DomainName_ExternalSYS, ConstValue.Key_FormStatus, CodeNamePairAppendItemType.All, (sender, e) =>
            {
                if (e.Result != null)
                {
                    cmbInvoiceInputStatus.ItemsSource = e.Result;
                    cmbInvoiceInputStatus.SelectedIndex = 0;
                }
            });

        }

        #endregion

        #region 按钮事件相关
        /// <summary>
        /// 查询按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgQueryResult.Bind();
        }

        private void dataGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            this.CurrentPageIndex = e.PageIndex;
            this.CurrentPageSize = e.PageSize;
            m_QueryFilter.PagingInfo = new PagingInfo()
            {
                PageSize = this.CurrentPageSize,
                PageIndex = this.CurrentPageIndex,
                SortBy = e.SortField
            };

            m_QueryFacde.QueryInvoiceInfoList(m_QueryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                viewList = DynamicConverter<EIMSInvoiceEntryVM>.ConvertToVMList(args.Result.Rows);
                dgQueryResult.TotalCount = args.Result.TotalCount;
                dgQueryResult.ItemsSource = viewList;
            });
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dgQueryResult.ItemsSource as dynamic;
            foreach (var view in viewList)
            {
                view.IsCheck = ckb.IsChecked.Value ? true : false;
            }
        }

        /// <summary>
        /// 批量录入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchEntry_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_InvoiceEntry_BathcInput))
            {
                Window.Alert(ResEIMSInvoiceEntry.Msg_HasNoRight);
                return;
            }
            if (viewList == null
                || viewList.Count == 0)
            {
                this.Window.Alert(ResEIMSInvoiceEntry.Msg_PleaseSelect);
                return;
            }

            List<EIMSInvoiceEntryVM> selectList = new List<EIMSInvoiceEntryVM>();
            foreach (var view in viewList)
            {
                if (view.IsCheck)
                {
                    selectList.Add(view);
                }
            }
            List<string> invoiceNumberList = new List<string>();
            string invoiceNumber = string.Empty;
            int vendorNo = 0;
            ReceiveType receiveType = ReceiveType.AccountDeduction;
            if (selectList.Count == 0)
            {
                this.Window.Alert(ResEIMSInvoiceEntry.Msg_PleaseSelect);
                return;
            }
            for (int i = 0; i < selectList.Count; i++)
            {
                vendorNo = selectList[0].VendorNumber;
                receiveType = selectList[0].ReceiveType;
                if (selectList[i].IsSAPImportedDes == "已上传" || selectList[i].InvoiceInputStatus == "已录入")
                {
                    invoiceNumberList.Add(selectList[i].InvoiceNumber);
                }
                if (selectList[i].VendorNumber != vendorNo || selectList[i].ReceiveType != receiveType)
                {
                    this.Window.Alert(ResEIMSInvoiceEntry.Msg_NotTheSameVendorOrReceiveType);
                    return;
                }
            }
            if (invoiceNumberList.Count > 0)
            {
                for (int i = 0; i < invoiceNumberList.Count; i++)
                {
                    if (i > 0)
                    {
                        invoiceNumber += ";";
                    }
                    invoiceNumber += invoiceNumberList[i];
                }
                this.Window.Alert(string.Format(ResEIMSInvoiceEntry.Msg_HadInputed, invoiceNumber));
            }
            else
            {
                UCBatchInvoiceInput uc = new UCBatchInvoiceInput(selectList);
                uc.Dialog = this.Window.ShowDialog("批量发票录入", uc, (o, s) =>
                {
                    if (s.isForce)
                    {
                        LoadingDataEventArgs m = new LoadingDataEventArgs(this.CurrentPageIndex, this.CurrentPageSize, null, null);
                        dataGrid_LoadingDataSource(null, m);
                    }
                }, new Size(700, 260));
            }
        }

        #endregion

        #region 查询数据操作相关

        /// <summary>
        /// 录入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnInput_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_InvoiceEntry_Input))
            {
                Window.Alert(ResEIMSInvoiceEntry.Msg_HasNoRight);
                return;
            }
            EIMSInvoiceEntryVM selectView = this.dgQueryResult.SelectedItem as EIMSInvoiceEntryVM;
            UCInvoiceInput uc = new UCInvoiceInput(selectView, "Input");
            uc.Dialog = Window.ShowDialog("发票信息录入", uc, (obj, args) =>
                {
                    if (args.isForce)
                    {
                        LoadingDataEventArgs s = new LoadingDataEventArgs(this.CurrentPageIndex, this.CurrentPageSize, null, null);
                        dataGrid_LoadingDataSource(null, s);
                    }
                }, new Size(700, 350));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.EIMS_InvoiceEntry_Edit))
            {
                Window.Alert(ResEIMSInvoiceEntry.Msg_HasNoRight);
                return;
            }
            EIMSInvoiceEntryVM selectView = this.dgQueryResult.SelectedItem as EIMSInvoiceEntryVM;
            m_QueryFacde.QueryInvoiceList(selectView.InvoiceNumber, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                if (args.Result.EIMSList.Convert<EIMSInvoiceInfoEntity, EIMSInvoiceInfoEntityVM>().Count > 1)
                {
                    MutiEdit(selectView);
                }
                else
                {
                    SingleEdit(selectView);
                }
            });
        }

        private void MutiEdit(EIMSInvoiceEntryVM selectView)
        {
            UCInvoiceInput uc = new UCInvoiceInput(selectView, "Edit");
            uc.Dialog = Window.ShowDialog("发票信息编辑", uc, (o, s) =>
            {
                if (s.isForce)
                {
                    LoadingDataEventArgs e = new LoadingDataEventArgs(this.CurrentPageIndex, this.CurrentPageSize, null, null);
                    dataGrid_LoadingDataSource(null, e);
                }
            }, new Size(700, 350));
        }

        private void SingleEdit(EIMSInvoiceEntryVM selectView)
        {
            UCViewInvoice uc = new UCViewInvoice(selectView.InvoiceNumber, "Edit");
            uc.Dialog = Window.ShowDialog("发票信息编辑", uc, (o, s) =>
            {
                if (s.isForce)
                {
                    LoadingDataEventArgs e = new LoadingDataEventArgs(this.CurrentPageIndex, this.CurrentPageSize, null, null);
                    dataGrid_LoadingDataSource(null, e);
                }
            }, new Size(700, 350));
        }

        /// <summary>
        /// 查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtnView_Click(object sender, RoutedEventArgs e)
        {
            EIMSInvoiceEntryVM selectView = this.dgQueryResult.SelectedItem as EIMSInvoiceEntryVM;
            UCViewInvoice uc = new UCViewInvoice(selectView.InvoiceNumber, "View");
            uc.Dialog = Window.ShowDialog("发票信息查看", uc, null, new Size(700, 350));
        }
        #endregion
    }
}
