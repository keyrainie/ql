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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Portal.UI.MKT.Models.ECCategory;
using ECCentral.Portal.UI.MKT.Facades.RequestMsg.ECCategory;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ECCategoryFacade
    {
         private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public ECCategoryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void GetECCategory(string companyCode, string channelID, EventHandler<RestClientEventArgs<ECCategoryListReulst>> callback)
        {
            string relativeUrl = string.Format("/MKTService/ECCategory/{0}/{1}", companyCode, channelID);
            restClient.Query<ECCategoryListReulst>(relativeUrl, callback);
        }


        public void Query(ECCategoryQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<ECCategoryQueryVM, ECCategoryQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/ECCategory/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void Create(ECCategoryVM vm, EventHandler<RestClientEventArgs<ECCategory>> callback)
        {
            var entity = vm.ConvertVM<ECCategoryVM, ECCategory>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            entity.ParentList = new List<ECCategory>();
            foreach (var p in vm.Parents)
            {
                entity.ParentList.Add(p.ConvertVM<ECCategoryVM, ECCategory>());
            }
            entity.ChildrenList = new List<ECCategory>();
            foreach (var c in vm.Children)
            {
                entity.ChildrenList.Add(c.ConvertVM<ECCategoryVM, ECCategory>());
            }
            string relativeUrl = "/MKTService/ECCategory/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(ECCategoryVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<ECCategoryVM, ECCategory>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            entity.ParentList = new List<ECCategory>();
            foreach (var p in vm.Parents)
            {
                entity.ParentList.Add(p.ConvertVM<ECCategoryVM, ECCategory>());
            }
            entity.ChildrenList = new List<ECCategory>();
            foreach (var c in vm.Children)
            {
                entity.ChildrenList.Add(c.ConvertVM<ECCategoryVM, ECCategory>());
            }
            string relativeUrl = "/MKTService/ECCategory/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<ECCategory>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/Load/" + sysNo.ToString();
            restClient.Query<ECCategory>(relativeUrl, callback);
        }

        public void Delete(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/Delete/" + sysNo.ToString();
            restClient.Update(relativeUrl, null, callback);
        }

        public void LoadParentView(int sysNo,ECCategoryLevel currentLevel, EventHandler<RestClientEventArgs<ECCategoryParentView>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/LoadParentView/" + sysNo.ToString() + "/" + currentLevel.ToString();
            restClient.Query<ECCategoryParentView>(relativeUrl, callback);
        }

        public void LoadChildView(int sysNo, ECCategoryLevel currentLevel,int rSysNo, EventHandler<RestClientEventArgs<ECCategoryChildView>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/LoadChildView/" + sysNo.ToString() + "/" + currentLevel.ToString() + "/" + rSysNo.ToString();
            restClient.Query<ECCategoryChildView>(relativeUrl, callback);
        }

        public void LoadTree(string channelID,ADStatus? status,EventHandler<RestClientEventArgs<ECCategory>> callback)
        {
            ECCategoryQueryFilter filter = new ECCategoryQueryFilter();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.ChannelID = channelID;
            filter.Status = status;
            string relativeUrl = "/MKTService/ECCategory/GetECCategoryTree";
            restClient.Query<ECCategory>(relativeUrl, filter, callback);
        }

        public void QueryProductMapping(int categorySysNo, int pageIndex, int pageSize, string sortBy, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ECCategoryMappingQueryFilter filter = new ECCategoryMappingQueryFilter
            {
                CompanyCode = CPApplication.Current.CompanyCode,
                ECCategorySysNo = categorySysNo,
                PagingInfo = new PagingInfo { PageIndex = pageIndex, PageSize = pageSize, SortBy = sortBy }
            };
            string relativeUrl = "/MKTService/ECCategory/QueryMapping";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void CreateMapping(int categorySysNo, List<int> productSysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/InsertCategoryProductMapping";
            ECCategoryMappingReq req = new ECCategoryMappingReq
            {
                ECCategorySysNo = categorySysNo,
                ProductSysNoList = productSysNoList
            };
            restClient.Create(relativeUrl, req, callback);
        }

        public void DeleteMapping(int categorySysNo, List<int> productSysNoList, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/ECCategory/DeleteCategoryProductMapping";
            ECCategoryMappingReq req = new ECCategoryMappingReq
            {
                ECCategorySysNo = categorySysNo,
                ProductSysNoList = productSysNoList
            };
            restClient.Delete(relativeUrl, req, callback);
        }


        public void QueryECCCategory(ECCCategoryManagerVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ECCManageCategoryQueryFilter
 filter = new ECCManageCategoryQueryFilter { Type = model.Type, Category1SysNo = model.Category1SysNo, Category2SysNo = model.Category2SysNo, Status = model.Status, CategoryName = model.CategoryName };

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            const string relativeUrl = "/MKTService/ECCategory/QueryECCCategory";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                }
                );
        }

        public void UpdateECCCategory(ECCCategoryManagerVM VMModel, EventHandler<RestClientEventArgs<object>> callback)
        {
            ECCategoryMange model = new ECCategoryMange();
            model.Category1SysNo = VMModel.Category1SysNo;
            model.Category2SysNo = VMModel.Category2SysNo;
            model.CategoryID = VMModel.CategoryID;
            model.CategoryName = VMModel.CategoryName;
            model.Status = VMModel.Status;
            model.SysNo = VMModel.SysNo;
            model.Type = VMModel.Type;
            string relativeUrl = "/MKTService/ECCategory/UpdateECCategoryManage";

            restClient.Update(relativeUrl, model, callback);

        }
    }
}
