using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.AppService;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        #region 查询
        /// <summary>
        /// 查询调拨单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/ExperienceHall/QueryExperienceHall", Method = "POST")]
        //[DataTableSerializeOperationBehavior]
        //public QueryResult QueryExperience(ExperienceQueryFilter request)
        //{
        //    int totalCount;
        //    var dataTable = ObjectFactory<IExperienceQueryDA>.Instance.QueryExperience(request, out totalCount);
        //    return new QueryResult()
        //    {
        //        Data = dataTable,
        //        TotalCount = totalCount
        //    };
        //}

        /// <summary>
        /// 统计当前条件下的调拨单初始状态和作废状态的成本
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/ExperienceHall/QueryLendCostbyStatus", Method = "POST")]
        //[DataTableSerializeOperationBehavior]
        //public QueryResult QueryLendCostbyStatus(ExperienceQueryFilter request)
        //{            
        //    var dataTable = ObjectFactory<IExperienceQueryDA>.Instance.QueryLendCostbyStatus(request);
        //    return new QueryResult()
        //    {
        //        Data = dataTable               
        //    };
        //}

        /// <summary>
        /// 查询调拨单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[WebInvoke(UriTemplate = "/ExperienceHall/ExportAllByPM", Method = "POST")]
        //[DataTableSerializeOperationBehavior]
        //public QueryResult ExportAllByPM(ExperienceQueryFilter request)
        //{
        //    int totalCount;
        //    var dataTable = ObjectFactory<IExperienceQueryDA>.Instance.ExportAllByPM(request, out totalCount);
        //    return new QueryResult()
        //    {
        //        Data = dataTable,
        //        TotalCount = totalCount
        //    };
        //}

        /// <summary>
        /// 体验厅库存查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/ExperienceHallInventoryQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryExperienceHallInventory(ExperienceHallInventoryInfoQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IExperienceHallQueryDA>.Instance.
                QueryExperienceHallInventory(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 体验厅调拨单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/ExperienceHallAllocateOrderQuery", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryExperienceHallAllocateOrder(ExperienceHallAllocateOrderQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IExperienceHallQueryDA>.Instance.
                QueryExperienceHallAllocateOrder(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }


        #endregion 查询

        #region 维护

        /// <summary>
        /// 根据系统编号加载调拨单
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/Load/{requestSysNo}", Method = "GET")]
        public ExperienceInfo LoadExperienceInfo(string requestSysNo)
        {
            int sysNo = int.Parse(requestSysNo);
            ExperienceInfo result = ObjectFactory<ExperienceHallAppService>.Instance.GetExperienceInfoBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 创建调拨单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/CreateExperience", Method = "POST")]
        public virtual int CreateExperience(ExperienceInfo entityToCreate)
        {
            ExperienceInfo result = ObjectFactory<ExperienceHallAppService>.Instance.CreateExperience(entityToCreate);
            return result.SysNo.Value;
        }

        /// <summary>
        /// 更新调拨单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/UpdateExperience", Method = "PUT")]
        public virtual int UpdateExperience(ExperienceInfo entityToUpdate)
        {
            ExperienceInfo result = ObjectFactory<ExperienceHallAppService>.Instance.UpdateRequest(entityToUpdate);
            return result.SysNo.Value;
        }


        /// <summary>
        /// 作废调拨单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/AbandonExperience", Method = "PUT")]
        public virtual int AbandonExperience(ExperienceInfo entityToUpdate)
        {
            ObjectFactory<ExperienceHallAppService>.Instance.AbandonExperience(entityToUpdate);
            return entityToUpdate.SysNo.Value;
        }

        /// <summary>
        /// 审核调拨单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/AuditExperience", Method = "PUT")]
        public virtual int AuditExperience(ExperienceInfo entityToUpdate)
        {
            ObjectFactory<ExperienceHallAppService>.Instance.AuditExperience(entityToUpdate);
            return entityToUpdate.SysNo.Value;
        }

        /// <summary>
        /// 取消审核调拨单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/CancelAuditExperience", Method = "PUT")]
        public virtual int CancelAuditExperience(ExperienceInfo entityToUpdate)
        {
            ObjectFactory<ExperienceHallAppService>.Instance.CancelAuditExperience(entityToUpdate);
            return entityToUpdate.SysNo.Value;
        }

        /// <summary>
        /// 体验厅接收/仓库接收
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ExperienceHall/ExperienceInOrOut", Method = "PUT")]
        public virtual int ExperienceInOrOut(ExperienceInfo entityToUpdate)
        {
            ObjectFactory<ExperienceHallAppService>.Instance.ExperienceInOrOut(entityToUpdate);
            return entityToUpdate.SysNo.Value;
        }

        #endregion 维护
    }
}
