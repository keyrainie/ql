using System;
using System.Windows;
using System.Linq;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using System.Windows.Controls;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.SO.Views
{
    [View(IsSingleton = true)]
    public partial class SOWHUpdate : PageBase
    {
        #region 字段属性定义
        SOWHUpdateQueryFilter m_queryRequest;
        SOWHUpdateInfoVM m_WHUpdateQueryView;
        List<StockVM> m_stockData;
        List<SOWHUpdateInfoVM> soWHUpdatelist;
        SOVM soView;

        public int? SOSysNo { get; set; }
        #endregion

        #region 页面初始化
        public SOWHUpdate()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            IniPageData();
        }

        private void IniPageData()
        {
            m_WHUpdateQueryView = new SOWHUpdateInfoVM();
            m_queryRequest = new SOWHUpdateQueryFilter();
            m_WHUpdateQueryView.SOSysNo = string.Empty;
            StockListBind();
        }
        #endregion

        #region 根据条件查询 验证
        /// <summary>
        /// 查询事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.SearchCondition.DataContext = m_WHUpdateQueryView;
            if (!m_WHUpdateQueryView.HasValidationErrors)
            {
                m_queryRequest = EntityConverter<SOWHUpdateInfoVM, SOWHUpdateQueryFilter>.Convert(m_WHUpdateQueryView);
                this.dgQueryResult.Bind();
            }
        }

        private void dgQueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            facade.QuerySOWHUpdate(m_queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                this.dgQueryResult.TotalCount = args.Result.TotalCount;
                soWHUpdatelist = DynamicConverter<SOWHUpdateInfoVM>.ConvertToVMList(args.Result.Rows);
                soWHUpdatelist.ForEach(p =>
                {
                    p.StockList = m_stockData;
                });

                this.dgQueryResult.ItemsSource = soWHUpdatelist;
            });
        }

        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            m_queryRequest.PagingInfo = new PagingInfo()
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };
            SOQueryFacade facade = new SOQueryFacade(this);
            ColumnSet col = new ColumnSet(dgQueryResult);
            facade.ExportWHUpdate(m_queryRequest, new ColumnSet[] { col });
        }

        /// <summary>
        /// 回车键进行查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void functionPanel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.btnSearch.Focus();
                btnSearch_Click(null, null);
            }
        }
        /// <summary>
        /// 绑定仓库列表
        /// </summary>
        private void StockListBind()
        {
            StockQueryFacade m_StockQueryFacade = new StockQueryFacade();
            m_StockQueryFacade.QueryStockListByWebChannel("1", (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                m_stockData = args.Result.Convert<StockInfo, StockVM>();
            });
        }
        #endregion

        #region 查询结果操作
        /// <summary>
        /// 跳转到订单维护页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlbtn_SOSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hlbtn = sender as HyperlinkButton;
            string url = string.Format(ConstValue.SOMaintainUrlFormat, hlbtn.CommandParameter);
            this.Window.Navigate(url, null, true);
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            dynamic viewList = this.dgQueryResult.ItemsSource as dynamic;
            if (viewList != null)
            {
                foreach (var view in viewList)
                    view.IsCheck = ckb.IsChecked.Value ? true : false;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<SOWHUpdateInfoVM> selectList = GetCheckStockList(this.dgQueryResult.ItemsSource as List<SOWHUpdateInfoVM>);

            if (selectList != null)
            {
                List<SOWHUpdateInfo> soWHUpdateInfoList = new List<SOWHUpdateInfo>();
                foreach (var item in selectList)
                {
                    item.CompanyCode = CPApplication.Current.CompanyCode;
                    soWHUpdateInfoList.Add(item.ConvertVM<SOWHUpdateInfoVM, SOWHUpdateInfo>());
                }

                SOQueryFacade soQueryFacade = new SOQueryFacade(this);

                soQueryFacade.QuerySOInfo(Convert.ToInt32(soWHUpdateInfoList[0].SOSysNo), vm =>
                {
                    soView = vm;

                    //只有处于审核状态的订单才可以修改
                    if (soView == null
                    || soView.BaseInfoVM.Status != (int)SOStatus.Origin)
                    {
                        this.Window.Alert(ResSO.Msg_SOUpdate_WarningSOAuditOutStock);
                        return;
                    }

                    #region 更新操作
                    try
                    {
                        bool result = false;
                        SOFacade soFacade = new SOFacade(this);
                        soFacade.WHUpdateStock(soWHUpdateInfoList[0], (obj, args) =>
                        {
                            if (args.FaultsHandle()) return;
                            result = (Boolean)args.Result;
                            if (result)
                            {
                                m_WHUpdateQueryView.SOSysNo = SOSysNo.ToString();
                                this.Window.Alert(ResSO.Info_WHUpdate_UpdateSucceed);
                                this.dgQueryResult.Bind();
                            }
                            else
                                this.Window.Alert(ResSO.Info_WHUpdate_UpdateFail);
                        });

                    }
                    catch (BusinessException be)
                    {
                        this.Window.Alert(be.ErrorDescription);
                    }
                    #endregion
                });
            }
        }

        /// <summary>
        /// 检查仓库信息，如果仓库status= -1或者源仓库和目标仓库相同则禁止修改
        /// </summary>
        /// <returns></returns>
        private List<SOWHUpdateInfoVM> GetCheckStockList(List<SOWHUpdateInfoVM> list)
        {
            if (list == null) return list;

            var selectList = list.Where(p => p.IsCheck);
            if (selectList.Count() != 0)
            {
                foreach (var item in selectList)
                {
                    if (item.StockSysNo == null
                        || item.StockSysNo.ToString() == item.SourceStockSysNo.ToString())
                    {
                        this.Window.Alert(ResSO.Msg_SOWHUpdate_TargetStockError);
                        return null;
                    }
                }
            }
            else
            {
                this.Window.Alert(ResSO.Msg_PleaseSelect);
                return null;
            }
            return selectList.ToList();
        }

        #endregion
    }
}

