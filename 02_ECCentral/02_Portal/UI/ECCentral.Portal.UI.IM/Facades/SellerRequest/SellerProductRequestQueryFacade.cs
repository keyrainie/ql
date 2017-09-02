//************************************************************************
// 用户名				泰隆优选
// 系统名				类别管理
// 子系统名		        类别管理查询Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class SellerProductRequestQueryFacade
    {
        #region 字段以及构造函数
        private readonly RestClient restClient;
        const string GetRelativeUrl = "/IMService/SellerProductRequest/QuerySellerProductRequest";
        const string BatchApproveRelativeUrl = "/IMService/SellerProductRequest/BatchApproveProductRequest";
        const string BatchDenyRelativeUrl = "/IMService/SellerProductRequest/BatchDenyProductRequest";
        const string BatchCreateIDRelativeUrl = "/IMService/SellerProductRequest/BatchCreateItemIDForNewProductRequest";

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }

        public SellerProductRequestQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public SellerProductRequestQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 查询分类属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QuerySellerProductRequest(SellerProductRequestQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            SellerProductRequestQueryFilter filter = model.ConvertVM<SellerProductRequestQueryVM, SellerProductRequestQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };


            restClient.QueryDynamicData(GetRelativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (!(args.Result == null || args.Result.Rows == null))
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                    }
                    callback(obj, args);
                }
                );
        }

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public virtual void BatchApproveProductRequest(List<SellerProductRequestVM> vmList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var auditList = (from c in vmList
                             select
                                 new SellerProductRequestInfo
                                 {
                                     SysNo = c.SysNo,
                                     Status = c.Status,
                                     ProductName = c.ProductName,
                                     InUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                     EditUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                     Auditor = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                     LanguageCode = CPApplication.Current.LanguageCode,
                                     CompanyCode = CPApplication.Current.CompanyCode
                                 }).ToList();

            restClient.Update(BatchApproveRelativeUrl, auditList, callback);
        }

        /// <summary>
        /// 批量审核通过
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public virtual void BatchCreateIDProductRequest(List<SellerProductRequestVM> vmList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var createIDList = (from c in vmList
                                select
                                    new SellerProductRequestInfo
                                    {
                                        SysNo = c.SysNo,
                                        Status = c.Status,
                                        ProductName = c.ProductName,
                                        InUser = new UserInfo {SysNo=CPApplication.Current.LoginUser.UserSysNo,UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                        EditUser = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                        Auditor = new UserInfo { SysNo = CPApplication.Current.LoginUser.UserSysNo, UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                        LanguageCode=c.LanguageCode,
                                        CompanyCode=CPApplication.Current.CompanyCode
                                    }).ToList();

            restClient.Update(BatchCreateIDRelativeUrl, createIDList, callback);
        }


        /// <summary>
        /// 批量退回
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public virtual void BatchDenyProductRequest(List<SellerProductRequestVM> vmList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var denyList = (from c in vmList
                            select
                                new SellerProductRequestInfo
                                {
                                    SysNo = c.SysNo,
                                    Status = c.Status,
                                    ProductName = c.ProductName,
                                    RequestSysno=c.RequestSysno,
                                    Memo = c.Memo,
                                    InUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                    EditUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                    Auditor = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName, UserDisplayName = CPApplication.Current.LoginUser.DisplayName },
                                    LanguageCode = CPApplication.Current.LanguageCode,
                                    CompanyCode = CPApplication.Current.CompanyCode
                                }).ToList();

            restClient.Update(BatchDenyRelativeUrl, denyList, callback);
        }

        public void ExportSellerProductRequestExcelFile(SellerProductRequestQueryVM vm, ColumnSet[] columns)
        {
            SellerProductRequestQueryFilter queryFilter = new SellerProductRequestQueryFilter();

            queryFilter = vm.ConvertVM<SellerProductRequestQueryVM, SellerProductRequestQueryFilter>();
           
            queryFilter.Type = SellerProductRequestType.NewCreated;

            queryFilter.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };


            restClient.ExportFile(GetRelativeUrl, queryFilter, columns);
        }
    }
}
