using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.RMA;
using System.Collections.Generic;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.RMA.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Service.RMA.Restful.RequestMsg;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class RMATrackingFacade
    {
        private readonly RestClient restClient;
        private IPage m_CurrentPage;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_RMA, "ServiceBaseUrl");
            }
        }

        public RMATrackingFacade(IPage page)
        {
            m_CurrentPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询RMATrackingCreateUsers
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetRMATrackingCreateUsers(bool needAll, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/RMAService/RMATrackingCreateUsers/GetAll";

            restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UserInfo> userList = args.Result;
                if (needAll)
                {
                    userList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<UserInfo>> eventArgs = new RestClientEventArgs<List<UserInfo>>(userList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 查询RMATrackingUpdateUsers
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetRMATrackingUpdateUsers(bool needAll, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/RMAService/RMATrackingUpdateUsers/GetAll";

            restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UserInfo> userList = args.Result;
                if (needAll)
                {
                    userList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<UserInfo>> eventArgs = new RestClientEventArgs<List<UserInfo>>(userList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        /// <summary>
        /// 查询RMATrackingHandleUsers
        /// </summary>
        /// <param name="needAll"></param>
        /// <param name="callback"></param>
        public void GetRMATrackingHandleUsers(bool needAll, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/RMAService/RMATrackingHandleUsers/GetAll";

            restClient.Query<List<UserInfo>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<UserInfo> userList = args.Result;
                if (needAll)
                {
                    userList.Insert(0, new UserInfo()
                    {
                        UserName = ResCommonEnum.Enum_All
                    });
                }
                RestClientEventArgs<List<UserInfo>> eventArgs = new RestClientEventArgs<List<UserInfo>>(userList, m_CurrentPage);
                callback(obj, eventArgs);
            });
        }

        public void Query(RMATrackingQueryVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            RMATrackingQueryFilter queryFilter = new RMATrackingQueryFilter();
            queryFilter = queryVM.ConvertVM<RMATrackingQueryVM, RMATrackingQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/RMAService/RMATracking/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 派发RMA跟进日志
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void Dispatch(RMATrackingBatchActionReq request, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/RMAService/RMATracking/Dispatch";
            restClient.Update<string>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        /// <summary>
        /// 取消派发RMA跟进日志
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void CancelDispatch(RMATrackingBatchActionReq request, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/RMAService/RMATracking/CancelDispatch";
            restClient.Update<string>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 关闭RMA跟进日志
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void Close(RMATrackingVM vm, EventHandler<RestClientEventArgs<string>> callback)
        {
            InternalMemoInfo data = vm.ConvertVM<RMATrackingVM, InternalMemoInfo>();
            data.Status = InternalMemoStatus.Close;
            string relativeUrl = "/RMAService/RMATracking/Close";
            restClient.Update<string>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 创建RMA跟进日志
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void Create(RMATrackingVM vm, EventHandler<RestClientEventArgs<InternalMemoInfo>> callback)
        {
            InternalMemoInfo data = vm.ConvertVM<RMATrackingVM, InternalMemoInfo>();
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/RMAService/RMATracking/Create";
            restClient.Update<InternalMemoInfo>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void ExportExcelFile(RMATrackingQueryVM queryVM, ColumnSet[] columns)
        {
            RMATrackingQueryFilter queryFilter = new RMATrackingQueryFilter();
            queryFilter = queryVM.ConvertVM<RMATrackingQueryVM, RMATrackingQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = null
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/RMAService/RMATracking/Query";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

    }
}
