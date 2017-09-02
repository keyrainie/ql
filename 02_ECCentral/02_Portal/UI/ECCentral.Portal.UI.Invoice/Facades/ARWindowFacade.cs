using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class ARWindowFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string InvoiceServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public ARWindowFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询-逾期未收款订单
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryTrackingInfo(TrackingInfoQueryVM query, int pageSize, int pageIndex, string sortField, Action<TrackingInfoQueryResultVM> callback)
        {
            TrackingInfoQueryFilter filter = query.ConvertVM<TrackingInfoQueryVM, TrackingInfoQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/TrackingInfo/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                TrackingInfoQueryResultVM result = new TrackingInfoQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    Action<dynamic, TrackingInfoVM> manualMap = new Action<dynamic, TrackingInfoVM>(TrackingInfoMapper);
                    result.ResultList = DynamicConverter<TrackingInfoVM>.ConvertToVMList(args.Result[0].Rows, manualMap, "LossType");
                    result.TotalCount = args.Result[0].TotalCount;
                }
                if (args.Result[1] != null && args.Result[1].Rows != null && !(args.Result[1].Rows is DynamicXml.EmptyList))
                {
                    result.Statistic = DynamicConverter<TrackingInfoQueryStatisticVM>.ConvertToVM(args.Result[1].Rows[0]);
                }
                callback(result);
            });
        }

        private void TrackingInfoMapper(dynamic source, TrackingInfoVM target)
        {
            target.LossType = source.LossTypeID;
            target.LossTypeDesc = source.LossType;
        }

        /// <summary>
        /// 查询单据
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <param name="orderType"></param>
        /// <param name="callback"></param>
        public void QueryOrder(string orderSysNo, SOIncomeOrderType orderType, Action<OrderQueryResultVM> callback)
        {
            OrderQueryFilter filter = new OrderQueryFilter()
            {
                OrderType = orderType,
                OrderSysNo = orderSysNo
            };

            string relativeUrl = "/InvoiceService/TrackingInfo/QueryOrder";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                OrderQueryResultVM result = new OrderQueryResultVM();
                result.ResultList = DynamicConverter<OrderVM>.ConvertToVMList(args.Result.Rows);
                result.TotalCount = args.Result.TotalCount;

                callback(result);
            });
        }

        /// <summary>
        /// 取得退款原因列表
        /// </summary>
        /// <param name="callback"></param>
        public void GetResponsibleUsersForList(bool needAll, Action<List<CodeNamePair>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/ResponsibleUser/GetAll/{0}", CPApplication.Current.CompanyCode);
            restClient.Query<List<CodeNamePair>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var responsibleUserList = args.Result;
                if (needAll)
                {
                    responsibleUserList.Insert(0, new CodeNamePair()
                    {
                        Name = ResCommonEnum.Enum_All
                    });
                }
                callback(responsibleUserList);
            });
        }

        /// <summary>
        /// 批量提交报损跟踪单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchSubmitTrackingInfo(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/TrackingInfo/BatchSubmit";

            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量关闭跟踪单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchCloseTrackingInfo(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/TrackingInfo/BatchClose";

            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量创建跟踪单
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void BatchCreateTrackingInfo(List<OrderVM> vm, Action<string> callback)
        {
            var trackingInfoList = vm.ConvertVM<OrderVM, TrackingInfo, List<TrackingInfo>>((s, t) =>
            {
                t.OrderSysNo = s.OrderSysNo;
                t.OrderType = s.OrderType;
                t.CompanyCode = CPApplication.Current.CompanyCode;
            });
            string relativeUrl = "/InvoiceService/TrackingInfo/BatchCreate";

            restClient.Create<string>(relativeUrl, trackingInfoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量设置跟踪单责任人姓名
        /// </summary>
        /// <param name="vmList"></param>
        /// <param name="callback"></param>
        public void BatchUpdateTrackingInfoResponsibleUserName(List<TrackingInfoVM> vmList, Action<string> callback)
        {
            var trackingInfoList = vmList.ConvertVM<TrackingInfoVM, TrackingInfo, List<TrackingInfo>>();
            string relativeUrl = "/InvoiceService/TrackingInfo/BatchUpdateResponsibleUser";

            restClient.Update<string>(relativeUrl, trackingInfoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量设置跟踪单损失类型
        /// </summary>
        /// <param name="vmList"></param>
        /// <param name="callback"></param>
        public void BatchUpdateTrackingInfoLossType(List<TrackingInfoVM> vmList, Action<string> callback)
        {
            var trackingInfoList = vmList.ConvertVM<TrackingInfoVM, TrackingInfo, List<TrackingInfo>>();
            string relativeUrl = "/InvoiceService/TrackingInfo/BatchUpdateTrackingInfoLossType";

            restClient.Update<string>(relativeUrl, trackingInfoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 更新跟踪单
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void UpdateTrackingInfo(TrackingInfoVM vm, Action callback)
        {
            var trackingInfo = vm.ConvertVM<TrackingInfoVM, TrackingInfo>();
            string relativeUrl = "/InvoiceService/TrackingInfo/Update";

            restClient.Update(relativeUrl, trackingInfo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 查询跟踪单责任人列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryResponsibleUser(ResponsibleUserQueryVM query, int pageSize, int pageIndex, string sortField, Action<ResponsibleUserQueryResultVM> callback)
        {
            ResponsibleUserQueryFilter filter = query.ConvertVM<ResponsibleUserQueryVM, ResponsibleUserQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/ResponsibleUser/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                ResponsibleUserQueryResultVM result = new ResponsibleUserQueryResultVM();
                result.ResultList = DynamicConverter<ResponsibleUserVM>.ConvertToVMList(args.Result.Rows);
                result.TotalCount = args.Result.TotalCount;
                callback(result);
            });
        }

        /// <summary>
        /// 是否已经存在符合条件的跟踪单责任人
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void ExistedResponsibleUser(ResponsibleUserVM entity, Action<bool> callback)
        {
            var request = entity.ConvertVM<ResponsibleUserVM, ResponsibleUserInfo>();
            string relativeUrl = "/InvoiceService/ResponsibleUser/Exists";

            restClient.Query<bool>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void CreateResponsibleUser(ResponsibleUserVM entity, bool overrideWhenCreate, Action callback)
        {
            var request = new CreateResponsibleUserReq();
            request.ResponsibleUser = entity.ConvertVM<ResponsibleUserVM, ResponsibleUserInfo>();
            request.OverrideWhenCreate = overrideWhenCreate;

            string relativeUrl = "/InvoiceService/ResponsibleUser/Create";
            restClient.Create<object>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void BatchAbandonResponsibleUser(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/ResponsibleUser/BatchAbandon";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 导出跟踪单
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportTrackingInfoExcelFile(TrackingInfoQueryVM queryVM, ColumnSet[] columnSet)
        {
            TrackingInfoQueryFilter queryFilter = queryVM.ConvertVM<TrackingInfoQueryVM, TrackingInfoQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = "Temp.SysNo desc"
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/TrackingInfo/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 导出跟踪单责任人
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportResponsibleUserExcelFile(ResponsibleUserQueryVM queryVM, ColumnSet[] columnSet)
        {
            ResponsibleUserQueryFilter queryFilter = queryVM.ConvertVM<ResponsibleUserQueryVM, ResponsibleUserQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = "sr.SysNo desc"
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/ResponsibleUser/Query";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }
    }
}