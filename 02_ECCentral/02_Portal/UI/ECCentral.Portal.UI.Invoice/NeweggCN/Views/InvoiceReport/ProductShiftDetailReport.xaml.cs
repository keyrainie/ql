using System;
using System.Windows;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.Portal.UI.Invoice.NeweggCN.UserControls;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Controls;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Controls.Uploader;
using System.IO;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Views.InvoiceReport
{
    /// <summary>
    /// 移仓单明细表
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class ProductShiftDetailReport : PageBase
    {
        #region member
        private string moneyFormat = "#,###,##0.00";
        private ProductShiftDetailReportQueryFilter m_query;
        private ProductShiftDetailFacade m_dataFacade;
        private List<int> m_selectItemIds;
        //readonly string m_selectCountFormat = "当前选中项总金额： 商品总金额(*):{0} ---- 商品税金(*) :{1} ---- 商品总金额:{2}";
        readonly string m_selectCountFormat = ResProductShiftDetailRepor.Msg_SelectTotalAmout;
        decimal m_productCountAmt;
        decimal m_productCountRatAmt;
        decimal m_productCountNoRatAmt;

        #endregion

        public ProductShiftDetailReport()
        {
            InitializeComponent();
            this.SeachBuilder.DataContext = m_query = new ProductShiftDetailReportQueryFilter();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            BindCombo();

            m_dataFacade = new ProductShiftDetailFacade(this);

            CheckStockSelectEnable();
        }

        private void BindCombo()
        {
            CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory
               , ConstValue.Key_InventoryCompanyName
               , CodeNamePairAppendItemType.Select, (sender, e) =>
               {
                   if (e.Result != null)
                   {
                       cbOutCompany.ItemsSource = cbEnterCompany.ItemsSource = e.Result;
                   }
               });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (CheckQueryCondition())
            {
                m_selectItemIds = new List<int>();
                SetSelectCountText();

                DataGrid.Bind();
            }
        }

        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_ProductShiftDetailReport_ShiftForExe))
            {
                Window.Alert(ResCommon.Message_NoAuthorize, MessageType.Error);
                return;
            }
            if (CheckQueryCondition())
            {
                m_query.PagingInfo = new PagingInfo()
                {
                    PageSize = ConstValue.MaxRowCountLimit,
                    PageIndex = 0,
                    SortBy = string.Empty
                };
                m_query.CompanyCode = CPApplication.Current.CompanyCode;
                ColumnSet col = new ColumnSet(DataGrid);
                m_dataFacade.ExportQuery(m_query, new ColumnSet[] { col });
            }
        }

        private void DataGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            m_productCountAmt = m_productCountNoRatAmt = m_productCountRatAmt = 0.0M;
            m_selectItemIds.Clear();


            DataGrid.ItemsSource = null;
            m_query.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            m_query.CompanyCode = CPApplication.Current.CompanyCode;
            m_dataFacade.Query(m_query, (o, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var list = args.Result.Data.Convert<ProductShiftDetail, ProductShiftDetailVM>();

                this.DataGrid.ItemsSource = list;
                this.DataGrid.TotalCount = args.Result.TotalCount;
                if (args.Result.TotalCount == 0)
                {
                    svStatisticInfo.Visibility = System.Windows.Visibility.Collapsed;
                    return;
                }
                else
                {
                    svStatisticInfo.Visibility = System.Windows.Visibility.Visible;
                }
               // Window.Alert(string.Format("IsCheckCompany:{0},IsCheckDetail:{1}", m_query.IsCheckCompany, m_query.IsCheckDetail));
                //加载统计数据
                if (m_query.IsCheckCompany || m_query.IsCheckDetail)
                {
                    if (m_query.IsCheckDetail)
                    {
                        txtAmtInfo.Visibility = System.Windows.Visibility.Visible;
                        txtSelectCountInfo.Visibility = System.Windows.Visibility.Collapsed;
                        //txtAmtInfo.Text = string.Format("移出公司总金额：商品总金额（*）：{0}----商品税金（*）：{1}----商品总金额：{2}"
                        //                                                    , args.Result.OutAmt.AmtCount.Value.ToString("#,###,###,##0.00")
                        //                                                    , args.Result.OutAmt.AmtTaxItem.Value.ToString("#,###,###,##0.00")
                        //                                                    , args.Result.OutAmt.AmtProductCost.Value.ToString("#,###,###,##0.00")
                        //                                                    );
                        txtAmtInfo.Text = string.Format(ResProductShiftDetailRepor.Msg_OutCompantyTotalAmout
                                                                            , args.Result.OutAmt.AmtCount.Value.ToString("#,###,###,##0.00")
                                                                            , args.Result.OutAmt.AmtTaxItem.Value.ToString("#,###,###,##0.00")
                                                                            , args.Result.OutAmt.AmtProductCost.Value.ToString("#,###,###,##0.00")
                                                                            );
                    }
                    else
                    {
                        string alertManual = string.Empty;
                        decimal outAmtCount = 0, outAmtTacItem = 0, outAmtProductCost = 0,
                                inAmtCount = 0, inAmtTacItem = 0, inAmtProductCost = 0;
                        if (args.Result.OutAmt != null)
                        {
                            outAmtCount = args.Result.OutAmt.AmtCount ?? 0;
                            outAmtTacItem = args.Result.OutAmt.AmtTaxItem ?? 0;
                            outAmtProductCost = args.Result.OutAmt.AmtProductCost ?? 0;
                        }
                        if (args.Result.InAmt != null)
                        {
                            inAmtCount = args.Result.InAmt.AmtCount ?? 0;
                            inAmtTacItem = args.Result.InAmt.AmtTaxItem ?? 0;
                            inAmtProductCost = args.Result.InAmt.AmtProductCost ?? 0;
                        }

                        if (args.Result.NeedManualItem != null)
                        {
                            //alertManual = string.Format("存在需要手工调整的金额:{0}"
                            //    , args.Result.NeedManualItem.AmtProductCost.HasValue ? args.Result.NeedManualItem.AmtProductCost.Value.ToString("#0.00") : string.Empty);
                            alertManual = string.Format(ResProductShiftDetailRepor.Msg_ManualAdjustAmout
                                , args.Result.NeedManualItem.AmtProductCost.HasValue ? args.Result.NeedManualItem.AmtProductCost.Value.ToString("#0.00") : string.Empty);
                        }

                        txtAmtInfo.Visibility = System.Windows.Visibility.Visible;
                        txtSelectCountInfo.Visibility = System.Windows.Visibility.Collapsed;
                        //txtAmtInfo.Text = string.Format("移出公司总金额：商品总金额（*）：{0}----商品税金（*）：{1}----商品总金额：{2}；\r\n移入公司总金额：商品总金额（*）：{3}----商品税金（*）：{4}----商品总金额：{5}；\r\n合并后总金额：商品总金额（*）：{6}----商品税金（*）：{7}----商品总金额：{8}；\r\n{9}",
                        //                                                            outAmtCount.ToString("#,###,###,##0.00"), outAmtTacItem.ToString("#,###,###,##0.00"), outAmtProductCost.ToString("#,###,###,##0.00"),
                        //                                                            inAmtCount.ToString("#,###,###,##0.00"), inAmtTacItem.ToString("#,###,###,##0.00"), inAmtProductCost.ToString("#,###,###,##0.00"),
                        //                                                            (outAmtCount - inAmtCount).ToString("#,###,###,##0.00"), (outAmtTacItem - inAmtTacItem).ToString("#,###,###,##0.00"), (outAmtProductCost - inAmtProductCost).ToString("#,###,###,##0.00"),
                        //                                                            alertManual);
                        txtAmtInfo.Text = string.Format(ResProductShiftDetailRepor.Msg_TotalAmout,
                                                        outAmtCount.ToString("#,###,###,##0.00"), outAmtTacItem.ToString("#,###,###,##0.00"), outAmtProductCost.ToString("#,###,###,##0.00"),
                                                        inAmtCount.ToString("#,###,###,##0.00"), inAmtTacItem.ToString("#,###,###,##0.00"), inAmtProductCost.ToString("#,###,###,##0.00"),
                                                        (outAmtCount - inAmtCount).ToString("#,###,###,##0.00"), (outAmtTacItem - inAmtTacItem).ToString("#,###,###,##0.00"), (outAmtProductCost - inAmtProductCost).ToString("#,###,###,##0.00"),
                                                        alertManual);
                    }
                }
                else
                {
                    //Normal Search
                    list.ForEach(p =>
                    {
                        if (m_selectItemIds.Contains(p.StItemSysNo.Value))
                        {
                            p.IsCheck = true;
                        }
                    });

                }

                ReloadSelectCountInfo();
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbSelectAll = sender as CheckBox;
            var list = this.DataGrid.ItemsSource as List<ProductShiftDetailVM>;
            if (list != null)
            {
                list.ForEach(item =>
                {
                    item.IsCheck = cbSelectAll.IsChecked.Value;
                });
            }
        }

        private void btnImportGoldenTaxWare_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_ProductShiftDetailReport_ShiftForGIT))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }
            if (!m_query.StockSysNoA.HasValue)
            {
                //Window.Alert("只有按照仓库查询的结果才能导入金税库！", MessageType.Error);
                Window.Alert(ResProductShiftDetailRepor.Msg_JustSelctByStockCanImport, MessageType.Error);
                return;
            }
            if (CheckQueryCondition())
            {
                m_query.StItemSysNos = m_selectItemIds;
                List<ProductShiftDetailQueryEntity> list = new List<ProductShiftDetailQueryEntity>();
                list.Add(new ProductShiftDetailQueryEntity
                {
                    CompanyCode = m_query.CompanyCode,
                    GoldenTaxNo = m_query.GoldenTaxNo,
                    OutTimeBegin = m_query.OutTimeStart,
                    OutTimeEnd = m_query.OutTimeEnd,
                    //ShiftSysNo = m_query.s
                    //ShiftType = m_query
                    StItemSysNos = m_query.StItemSysNos,
                    StockSysNoA = m_query.StockSysNoA,
                    StockSysNoB = m_query.StockSysNoB
                });
                m_dataFacade.CreateProductShiftDetails(list, (o, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == 0)
                    {
                        //Window.Alert("未找到移仓单可以导入到金税库中！");
                        Window.Alert(ResProductShiftDetailRepor.Msg_NotFoundShitOrder);
                    }
                    else
                    {
                        //Window.Alert(string.Format("成功导入{0}条移仓单到金税库中！0金额移仓单将会被过滤。", args.Result));
                        Window.Alert(string.Format(ResProductShiftDetailRepor.Msg_ImportTips, args.Result));
                    }
                });
            }
        }

        private void btnImportGoldenTaxDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Invoice_ProductShiftDetailReport_ShiftForGIT))
            {
                Window.Alert(ResCommon.Message_NoAuthorize);
                return;
            }

            UCImportGoldenTaxDetail importer = new UCImportGoldenTaxDetail();
            importer.uploader.MultiSelect = false;
            importer.uploader.MultiUpload = false;
            importer.uploader.UploadCompleted += UploadCompleted;
            //importer.ShowDialog("导入金税明细", null);
            importer.ShowDialog(ResProductShiftDetailRepor.Msg_ImportTaxDetail, null);
        }

        #region QueryConditionControl

        private void UploadCompleted(object sender, ECCentral.Portal.Basic.Controls.Uploader.UploadCompletedEventArgs e)
        {
            FileUploader uploader = (FileUploader)sender;
            if (uploader == null
                || e.UploadResult != SingleFileUploadStatus.Success)
            {
                return;
            }
            string serverFilePath = e.ServerFilePath;
            m_dataFacade.ImportProductShiftDetail(serverFilePath, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result)
                {
                    //Window.Alert("导入成功");
                    Window.Alert(ResProductShiftDetailRepor.Msg_ImportSuccess);
                }
                else
                {
                    //Window.Alert("发生未知错误，导入失败，请稍后再试。");
                    Window.Alert(ResProductShiftDetailRepor.Msg_ImportError);
                }
            });
        }

        private void QueryGoldenTax_Checked(object sender, RoutedEventArgs e)
        {
            //SetStockSelectEnabled(false);
            btnImportGoldenTaxWare.IsEnabled = btnImportGoldenTaxDetail.IsEnabled = false;
            m_query.IsCheckDetail = true;
            SetStockSelectIndex(0);
            CheckStockSelectEnable();
        }

        private void QueryGoldenTax_Unchecked(object sender, RoutedEventArgs e)
        {
            //SetStockSelectEnabled(true);
            btnImportGoldenTaxWare.IsEnabled = btnImportGoldenTaxDetail.IsEnabled = true;
            m_query.IsCheckDetail = false;
            CheckStockSelectEnable();
        }

        private void cbCompany_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CheckStockSelectEnable();
        }

        private void ucbStock_cmbStockListSelectionChanged(object sender, EventArgs e)
        {
            if (ucbStockA.SelectedStockSysNo != null || ucbStockB.SelectedStockSysNo != null)
            {
                SetCompanySelectIndex(0);
                SetCompanySelectEnabled(false);
            }
            else
            {
                SetCompanySelectEnabled(true);
            }
        }

        private void CheckStockSelectEnable()
        {
            if (cbEnterCompany.SelectedIndex != 0 || cbOutCompany.SelectedIndex != 0)
            {
                SetStockSelectIndex(0);
                SetStockSelectEnabled(false);
            }
            else
            {
                SetStockSelectEnabled(!m_query.IsCheckDetail);
            }
        }

        private void SetStockSelectEnabled(bool isEnabled)
        {
            ucbStockA.IsEnabled = ucbStockB.IsEnabled = isEnabled;
        }

        private void SetStockSelectIndex(int selectedIndex)
        {
            ucbStockA.SelectedIndex = ucbStockB.SelectedIndex = selectedIndex;
        }

        private void SetCompanySelectEnabled(bool isEnabled)
        {
            cbOutCompany.IsEnabled = cbEnterCompany.IsEnabled = isEnabled;
        }

        private void SetCompanySelectIndex(int selectedIndex)
        {
            cbOutCompany.SelectedIndex = cbEnterCompany.SelectedIndex = selectedIndex;
        }

        #endregion

        bool CheckQueryCondition()
        {
            bool isSelectStockOrCompany = string.IsNullOrEmpty(m_query.OutCompany)
                                        && string.IsNullOrEmpty(m_query.EnterCompany)
                                        && !m_query.StockSysNoA.HasValue;

            if (isSelectStockOrCompany)
            {
                //Window.Alert("按照分公司查询或者按照仓库查询，必要选择一项!");
                Window.Alert(ResProductShiftDetailRepor.Msg_NeedSelectByCompanOrStock);
                return false;
            }
            bool noCheckCompany = !string.IsNullOrEmpty(m_query.OutCompany) || !string.IsNullOrEmpty(m_query.EnterCompany);

            if (m_query.IsCheckCompany == false && noCheckCompany == true)
            {
                //Window.Alert("按照分公司查询时移入公司和移出公司都必须有值!");
                Window.Alert(ResProductShiftDetailRepor.Msg_NeedHaveValueByCompany);
                return false;
            }

            if (!m_query.OutTimeEnd.HasValue && !m_query.OutTimeStart.HasValue)
            {
                //Window.Alert("出库时间不能为空！");
                Window.Alert(ResProductShiftDetailRepor.Msg_OutTimeNotNull);
                return false;
            }

            if (m_query.IsCheckCompany && m_query.OutCompany == m_query.EnterCompany && !m_query.IsCheckDetail)
            {
                //Window.Alert("按照分公司查询时移入公司和移出公司不能相同!");
                Window.Alert(ResProductShiftDetailRepor.Msg_CompanyNeedNotNull);
                return false;
            }
            return true;
        }

        void SetSelectCountText()
        {
            txtAmtInfo.Visibility = System.Windows.Visibility.Collapsed;
            txtSelectCountInfo.Visibility = System.Windows.Visibility.Visible;
            ReloadSelectCountInfo();
        }

        void ReloadSelectCountInfo()
        {
            txtSelectCountInfo.Text = string.Format(m_selectCountFormat
                                                , m_productCountAmt.ToString(moneyFormat)
                                                , m_productCountRatAmt.ToString(moneyFormat)
                                                , m_productCountNoRatAmt.ToString(moneyFormat));
        }

        private void Item_Checked(object sender, RoutedEventArgs e)
        {
            if (m_query.StockSysNoA.HasValue)
            {
                var data = ((CheckBox)sender).Tag as ProductShiftDetailVM;
                if (data != null && !m_selectItemIds.Contains(data.StItemSysNo.Value))
                {
                    m_productCountAmt += data.AmtCount ?? 0;
                    m_productCountRatAmt += data.AmtTaxItem ?? 0;
                    m_productCountNoRatAmt += data.AmtProductCost ?? 0;
                    m_selectItemIds.Add(data.StItemSysNo.Value);
                    ReloadSelectCountInfo();
                }
            }
        }

        private void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            if (m_query.StockSysNoA.HasValue)
            {
                var data = ((CheckBox)sender).Tag as ProductShiftDetailVM;
                if (data != null && m_selectItemIds.Contains(data.StItemSysNo.Value))
                {
                    m_productCountAmt -= data.AmtCount ?? 0;
                    m_productCountRatAmt -= data.AmtTaxItem ?? 0;
                    m_productCountNoRatAmt -= data.AmtProductCost ?? 0;
                    m_selectItemIds.Remove(data.StItemSysNo.Value);
                    ReloadSelectCountInfo();
                }
            }
        }
    }
}