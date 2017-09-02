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
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
     [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ShiftRequestMemoMaintain : PageBase
    {
        public ShiftRequestQueryFacade QueryFacade;        
        public ShiftRequestMemoQueryView PageView;
        List<ShiftRequestMemoVM> requestMemoList;

        private int? _requestSysNo;
        private int? RequestSysNo
        {
            get
            {
                if (!_requestSysNo.HasValue)
                {
                    int tSysNo = 0;
                    if (!string.IsNullOrEmpty(Request.Param) && int.TryParse(Request.Param, out tSysNo))
                    {
                        _requestSysNo = tSysNo;
                    }
                }
                return _requestSysNo;
            }
            set
            {
                _requestSysNo = value;
            }
        }

        public ShiftRequestMemoMaintain()
        {
            InitializeComponent();           
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            QueryFacade = new ShiftRequestQueryFacade(this);
            PageView = new ShiftRequestMemoQueryView();
            if (RequestSysNo.HasValue)
            {
                PageView.QueryInfo.RequestSysNo = RequestSysNo.ToString();              
            }
            else
            {
                PageView.QueryInfo.RequestSysNo = null;               
            }
            this.DataContext = PageView.QueryInfo;
            dgShiftRequestMemoMaintainResult.Bind();

            btnNewMemo.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestMemo_NewMemo);
            btnCloseMemo.IsEnabled = AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ShiftRequestMemo_CloseMemo);
        }

        private void dgShiftRequestMemoMaintainResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            QueryFacade.QueryShiftRequestMemo(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                requestMemoList = DynamicConverter<ShiftRequestMemoVM>.ConvertToVMList(vmList);
                dgShiftRequestMemoMaintainResult.ItemsSource = requestMemoList;
                dgShiftRequestMemoMaintainResult.TotalCount = PageView.TotalCount;
            });           
        }

        private void btnCloseMemo_Click(object sender, RoutedEventArgs e)
        {
            List<ShiftRequestMemoVM> getSelectedList = GetSelectedRequestMemoVMList();
            if (null == getSelectedList || getSelectedList.Count <= 0)
            {
                Window.Alert("请至少选择一条数据！");
                return;
            }

            List<int> memoSysNoList = new List<int>();
            getSelectedList.ForEach(x => {
                memoSysNoList.Add((int)x.SysNo);
            });

            ShiftRequestMemoVM memoVM = new ShiftRequestMemoVM { 
                RequestSysNo = this.RequestSysNo
            };
            HyperlinkButton btn = sender as HyperlinkButton;
            ShiftRequestMemoClose content = new ShiftRequestMemoClose
            {
                Page = this,
                MemoSysNoList = memoSysNoList,
                MemoVM = memoVM
            };
            content.Dialog = Window.ShowDialog("关闭跟踪日志", content, (obj, args) =>
            {
                dgShiftRequestMemoMaintainResult.Bind();
            });
        }

        private void btnNewMemo_Click(object sender, RoutedEventArgs e)
        {
            List<int> requestSysNoList = new List<int>();
            requestSysNoList.Add((int)this.RequestSysNo);
            ShiftRequestMemo content = new ShiftRequestMemo
            {
                Page = this,
                RequestSysNoList = requestSysNoList
            };
            content.Dialog = Window.ShowDialog("添加跟踪日志", content, (obj, args) =>{
                dgShiftRequestMemoMaintainResult.Bind();    
            });

        }

        private void chbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (null != chk)
            {
                if (null != this.dgShiftRequestMemoMaintainResult.ItemsSource)
                {
                    foreach (var item in this.dgShiftRequestMemoMaintainResult.ItemsSource)
                    {
                        if (item is ShiftRequestMemoVM)
                        {
                            if (chk.IsChecked == true)
                            {
                                if (!((ShiftRequestMemoVM)item).IsChecked)
                                {
                                    ((ShiftRequestMemoVM)item).IsChecked = true;
                                }
                            }
                            else
                            {
                                if (((ShiftRequestMemoVM)item).IsChecked)
                                {
                                    ((ShiftRequestMemoVM)item).IsChecked = false;
                                }
                            }

                        }
                    }
                }
            }
        }//End SelectAllRow

        private List<ShiftRequestMemoVM> GetSelectedRequestMemoVMList()
        {
            if (null != requestMemoList && requestMemoList.Count > 0)
            {
                return requestMemoList.Where(x => x.IsChecked == true).ToList();
            }
            else
            {
                return null;
            }
        }
    }
}
