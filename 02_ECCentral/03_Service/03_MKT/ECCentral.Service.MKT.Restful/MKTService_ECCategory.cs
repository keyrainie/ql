using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using System.ServiceModel.Web;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.BizEntity.MKT.PageType;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebGet(UriTemplate = "/ECCategory/{companyCode}/{channelID}")]
        public virtual ECCategoryListReulst GetAllECCategory(string companyCode, string channelID)
        {
            ECCategoryListReulst result = new ECCategoryListReulst();
            result.Category1List = _ecCategoryQueryDA.GetAllECCategory1(companyCode, channelID);
            result.Category2List = _ecCategoryQueryDA.GetAllECCategory2(companyCode, channelID);
            result.Category3List = _ecCategoryQueryDA.GetAllECCategory3(companyCode, channelID);

            return result;
        }

        private IECCategoryQueryDA _ecCategoryQueryDA = ObjectFactory<IECCategoryQueryDA>.Instance;

        [WebInvoke(UriTemplate = "/ECCategory/Query", Method = "POST")]
        public virtual QueryResult QueryECCategory(ECCategoryQueryFilter filter)
        {
            int totalCount = 0;
            var dt = _ecCategoryQueryDA.Query(filter, out totalCount);
            QueryResult queryResult = new QueryResult();
            queryResult.Data = dt;
            queryResult.TotalCount = totalCount;
            return queryResult;
        }

        [WebInvoke(UriTemplate = "/ECCategory/GetECCategoryTree", Method = "POST")]
        public virtual ECCategory GetECCategoryTree(ECCategoryQueryFilter filter)
        {
            var list = _ecCategoryQueryDA.GetECCategoryTree(filter);
            ECCategory root = new ECCategory();
            root.Name = "Root";
            root.SysNo = 0;
            root.RParentSysNo = 0;
            root.Status = ADStatus.Active;
            BuildTree(root, list);
            return root;
        }

        private void BuildTree(ECCategory parent, List<ECCategory> list)
        {
            parent.ChildrenList = list.Where(item => (item.RParentSysNo ?? 0) == parent.RSysNo).OrderByDescending(item => item.Priority).ToList();
            foreach (var c in parent.ChildrenList)
            {
                BuildTree(c, list);
            }
        }

        /// <summary>
        /// 获取节点的所有可用父级节点和当前的父级节点编号列表
        /// </summary>
        /// <param name="id">节点的系统编号</param>
        /// <param name="level">节点的级别</param>
        [WebGet(UriTemplate = "/ECCategory/LoadParentView/{id}/{level}")]
        public virtual ECCategoryParentView LoadECCategoryParentView(string id, string level)
        {
            int sysNo = int.Parse(id);
            ECCategoryParentView view = new ECCategoryParentView();
            if (sysNo > 0)
            {
                view.CurrentParentSysNoList = _ecCategoryQueryDA.GetECCategoryCurrentParentSysNos(sysNo);
            }
            ECCategoryLevel currentLevel = (ECCategoryLevel)Enum.Parse(typeof(ECCategoryLevel), level);
            ECCategoryLevel parentLevel = GetParentLevel(currentLevel);
            view.ParentCategoryList = _ecCategoryQueryDA.GetECCategoryParents(parentLevel);

            return view;
        }

        /// <summary>
        /// 获取节点的所有可用子级节点和当前的子级节点编号列表
        /// </summary>
        /// <param name="id">节点的系统编号</param>
        /// <param name="level">节点的级别</param>
        /// <param name="rid">节点的层级关系系统编号</param>
        [WebGet(UriTemplate = "/ECCategory/LoadChildView/{id}/{level}/{rid}")]
        public virtual ECCategoryChildView LoadECCategoryChildView(string id, string level, string rid)
        {
            int sysNo = int.Parse(id);
            //层级关系表中的系统编号
            int rSysNo = int.Parse(rid);
            ECCategoryChildView view = new ECCategoryChildView();
            ECCategoryLevel currentLevel = (ECCategoryLevel)Enum.Parse(typeof(ECCategoryLevel), level);

            var childLevel = GetChildLevel(currentLevel);
            view.ChildCategoryList = _ecCategoryQueryDA.GetECCategoryChildren(childLevel);
            view.CurrentChildSysNoList = _ecCategoryQueryDA.GetECCategoryCurrentChildSysNos(sysNo, rSysNo);

            return view;
        }

        private ECCategoryLevel GetChildLevel(ECCategoryLevel currentLevel)
        {
            switch (currentLevel)
            {
                case ECCategoryLevel.Category2:
                    return ECCategoryLevel.Category3;
                case ECCategoryLevel.Category1:
                    return ECCategoryLevel.Category2;
                default:
                    return ECCategoryLevel.Category3;
            }
        }

        private ECCategoryLevel GetParentLevel(ECCategoryLevel currentLevel)
        {
            switch (currentLevel)
            {
                case ECCategoryLevel.Category3:
                    return ECCategoryLevel.Category2;
                case ECCategoryLevel.Category2:
                    return ECCategoryLevel.Category1;
                default:
                    return ECCategoryLevel.Category1;
            }
        }

        private ECCategoryAppService _ecCategoryAppService = ObjectFactory<ECCategoryAppService>.Instance;

        /// <summary>
        /// 插入前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        [WebInvoke(UriTemplate = "/ECCategory/Create", Method = "POST")]
        public ECCategory InsertECCategory(ECCategory entity)
        {
            return _ecCategoryAppService.Insert(entity);
        }

        /// <summary>
        /// 更新前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        [WebInvoke(UriTemplate = "/ECCategory/Update", Method = "PUT")]
        public void UpdateECCategory(ECCategory entity)
        {
            _ecCategoryAppService.Update(entity);
        }

        /// <summary>
        /// 更新前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        [WebInvoke(UriTemplate = "/ECCategory/UpdateECCategoryManage", Method = "PUT")]
        public void UpdateECCategoryManage(ECCategoryMange entity)
        {
            ECCategory model = new ECCategory();
            model = _ecCategoryAppService.Load(entity.SysNo.Value);
            model.Name = entity.CategoryName;
            if (entity.Status == ECCCategoryManagerStatus.Active)
            {
                model.Status = ADStatus.Active;
            }
            else
            {
                model.Status = ADStatus.Deactive;
            }

            if (model.ParentList == null)
            {
                model.ParentList = new List<ECCategory>();
            }
            else
            {
                model.ParentList.Clear();
            }


            if (entity.Type == ECCCategoryManagerType.ECCCategoryType2 && entity.Category1SysNo.HasValue)
            {
                model.ParentList.Add(_ecCategoryAppService.Load(entity.Category1SysNo.Value));

            }
            if (entity.Type == ECCCategoryManagerType.ECCCategoryType3 && entity.Category2SysNo.HasValue)
            {
                model.ParentList.Add(_ecCategoryAppService.Load(entity.Category2SysNo.Value));
            }

            _ecCategoryAppService.Update(model);
        }
        /// <summary>
        /// 删除前台显示分类
        /// </summary>
        /// <param name="sysNo">前台显示分类系统编号</param>
        [WebInvoke(UriTemplate = "/ECCategory/Delete/{sysNo}", Method = "PUT")]
        public void DeleteECCategory(string sysNo)
        {
            int id = int.Parse(sysNo);
            _ecCategoryAppService.Delete(id);
        }

        /// <summary>
        /// 加载
        /// </summary>
        [WebGet(UriTemplate = "/ECCategory/Load/{sysNo}")]
        public virtual ECCategory LoadECCategory(string sysNo)
        {
            int id = int.Parse(sysNo);
            return _ecCategoryAppService.Load(id);
        }

        [WebInvoke(UriTemplate = "/ECCategory/InsertCategoryProductMapping", Method = "POST")]
        public void InsertECCategoryProductMapping(ECCategoryMappingReq req)
        {
            _ecCategoryAppService.InsertCategoryProductMapping(req.ECCategorySysNo.Value, req.ProductSysNoList);
        }

        [WebInvoke(UriTemplate = "/ECCategory/DeleteCategoryProductMapping", Method = "DELETE")]
        public void DeleteECCategoryProductMapping(ECCategoryMappingReq req)
        {
            _ecCategoryAppService.DeleteCategoryProductMapping(req.ECCategorySysNo.Value, req.ProductSysNoList);
        }

        [WebInvoke(UriTemplate = "/ECCategory/QueryMapping", Method = "POST")]
        public QueryResult QueryECCategoryMapping(ECCategoryMappingQueryFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IECCategoryQueryDA>.Instance.QueryECCategoryMapping(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #region 管理前台类别关系

        [WebInvoke(UriTemplate = "/ECCategory/QueryCategory1", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public List<CategoryInfo> QueryCategory1(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<IECCategoryQueryDA>.Instance.QueryCategory1(queryFilter);
        }

        [WebInvoke(UriTemplate = "/ECCategory/QueryAllCategory2", Method = "POST", ResponseFormat = WebMessageFormat.Json)
        ]
        public List<CategoryInfo> QueryAllCategory2(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<IECCategoryQueryDA>.Instance.QueryAllCategory2(queryFilter);
        }

        [WebInvoke(UriTemplate = "/ECCategory/QueryAllCategory3", Method = "POST", ResponseFormat = WebMessageFormat.Json)
        ]
        public List<CategoryInfo> QueryAllCategory3(CategoryQueryFilter queryFilter)
        {
            return ObjectFactory<IECCategoryQueryDA>.Instance.QueryAllCategory3(queryFilter);
        }

        [WebInvoke(UriTemplate = "/ECCategory/QueryECCCategory", Method = "POST")
        ]
        public QueryResult QueryECCCategory(ECCManageCategoryQueryFilter queryFilter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IECCategoryQueryDA>.Instance.QueryECCCategory(queryFilter, out totalCount);
            return new QueryResult
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion

    }
}
