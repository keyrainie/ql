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
using ECCentral.BizEntity.Common;


namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        #region 查询

        /// <summary>
        /// 查询移仓单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/QueryShiftRequest", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryShiftRequest(ShiftRequestQueryFilter queryFilter)
        {
            int totalCount;
            SetShiftRequestPMRequestRightFilter(queryFilter);
            var dataTable = ObjectFactory<IShiftRequestQueryDA>.Instance.QueryShiftRequest(queryFilter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        private static void SetShiftRequestPMRequestRightFilter(ShiftRequestQueryFilter queryFilter)
        {
            if (null != queryFilter.PMQueryRightType)
            {
                List<int> pms = new List<int>();
                pms = new InentoryAppService().QueryPMListByRight(queryFilter.PMQueryRightType.Value, queryFilter.UserName, queryFilter.CompanyCode);
                if (pms != null && pms.Count > 0)
                {
                    foreach (var item in pms)
                    {
                        queryFilter.AuthorizedPMsSysNumber += "," + item;
                    }
                }
                if (queryFilter.AuthorizedPMsSysNumber.Contains(","))
                {
                    queryFilter.AuthorizedPMsSysNumber = queryFilter.AuthorizedPMsSysNumber.Remove(0, 1);
                }
            }
        }

        [WebInvoke(UriTemplate = "/ShiftRequest/QueryShiftCountData", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResultList QueryShiftCountData(ShiftRequestQueryFilter queryFilter)
        {
            SetShiftRequestPMRequestRightFilter(queryFilter);
            var data = ObjectFactory<IShiftRequestQueryDA>.Instance.QueryCountData(queryFilter);
            var result = new QueryResultList{
            new QueryResult()
            {
                Data = data.Tables[0]
            }};
            if (data.Tables.Count > 1)
            {
                result.Add(new QueryResult { Data = data.Tables[1] });
            }
            return result;
        }

        /// <summary>
        /// 查询移仓单创建人列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/QueryShiftRequestCreateUserList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryShiftRequestCreateUserList(string companyCode)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShiftRequestQueryDA>.Instance.QueryShiftRequestCreateUserList(companyCode, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询移仓单跟进日志
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/QueryShiftRequestMemo", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryShiftRequestMemo(ShiftRequestMemoQueryFilter requestMemo)
        {
            int totalCount;
            var dataTable = ObjectFactory<IShiftRequestMemoQueryDA>.Instance.QueryShiftRequestMemo(requestMemo, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        #endregion 查询

        #region 维护移仓单

        /// <summary>
        /// 根据系统编号加载移仓单详细信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/Load/{requestSysNo}", Method = "GET")]
        public ShiftRequestInfo LoadShiftRequestInfo(string requestSysNo)
        {
            int sysNo = int.Parse(requestSysNo);
            ShiftRequestInfo result = ObjectFactory<ShiftRequestAppService>.Instance.GetShiftRequestInfoBySysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/GetProductLine/{productSysNo}", Method = "GET")]
        public ShiftRequestInfo GetProductLineInfo(string productSysNo)
        {
            int sysNo = int.Parse(productSysNo);
            ShiftRequestInfo result = ObjectFactory<ShiftRequestAppService>.Instance.GetProductLineInfo(sysNo);
            return result;
        }
      
        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/CreateRequest", Method = "POST")]
        public virtual List<ShiftRequestInfo> CreateShiftRequest(ShiftRequestInfo entityToCreate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.CreateRequest(entityToCreate);
        }

        /// <summary>
        /// 更新移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/UpdateRequest", Method = "PUT")]
        public virtual ShiftRequestInfo UpdateShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.UpdateRequest(entityToUpdate);
        }


        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/AbandonRequest", Method = "PUT")]
        public virtual ShiftRequestInfo AbandonShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.AbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// PO作废移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/AbandonRequestForPO", Method = "PUT")]
        public virtual ShiftRequestInfo AbandonShiftRequestForPO(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.AbandonRequestForPO(entityToUpdate);
        }

        /// <summary>
        /// 取消作废移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/CancelAbandonRequest", Method = "PUT")]
        public virtual ShiftRequestInfo CancelAbandonShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.CancelAbandonRequest(entityToUpdate);
        }

        /// <summary>
        /// 审核移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/VerifyRequest", Method = "PUT")]
        public virtual ShiftRequestInfo VerifyShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.VerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 取消审核移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/CancelVerifyRequest", Method = "PUT")]
        public virtual ShiftRequestInfo CancelVerifyShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.CancelVerifyRequest(entityToUpdate);
        }

        /// <summary>
        /// 移仓单出库--仅测试,应该有WMS触发
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/OutStockRequest", Method = "PUT")]
        public virtual ShiftRequestInfo OutStockShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.OutStockRequest(entityToUpdate);
        }

        /// <summary>
        /// 移仓单入库
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/InStockRequest", Method = "PUT")]
        public virtual ShiftRequestInfo InStockShiftRequest(ShiftRequestInfo entityToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.InStockRequest(entityToUpdate);
        }

        /// <summary>
        /// 更新移仓单特殊状态
        /// </summary>
        /// <param name="entityListToUpdate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/UpdateSpecialShiftTypeBatch", Method = "PUT")]
        public virtual List<ShiftRequestInfo> UpdateSpecialShiftTypeBatch(List<ShiftRequestInfo> entityListToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.UpdateSpecialShiftTypeBatch(entityListToUpdate);
        }
        #endregion 维护移仓单

        #region 移仓单跟进日志

        /// <summary>
        /// 创建移仓单跟进日志
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/CreateShiftRequestMemo", Method = "POST")]
        public virtual List<ShiftRequestMemoInfo> CreateShiftRequestMemo(List<ShiftRequestMemoInfo> entityToCreate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.CreateShiftRequestMemo(entityToCreate);
        }

        /// <summary>
        /// 更新移仓单跟进日志
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/UpdateShiftRequestMemoList", Method = "PUT")]
        public virtual List<ShiftRequestMemoInfo> UpdateShiftRequestMemoList(List<ShiftRequestMemoInfo> entityListToUpdate)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.UpdateShiftRequestMemoList(entityListToUpdate);
        }

        /// <summary>
        /// 根据移仓单系统编号获取日志列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/GetShiftRequestMemoListByRequestSysNo/{requestSysNo}", Method = "GET")]
        public List<ShiftRequestMemoInfo> GetShiftRequestMemoListByRequestSysNo(string requestSysNo)
        {
            int sysNo = int.Parse(requestSysNo);
            List<ShiftRequestMemoInfo> result = ObjectFactory<ShiftRequestAppService>.Instance.GetShiftRequestMemoListByRequestSysNo(sysNo);
            return result;
        }

        /// <summary>
        /// 根据移仓单日志系统编号获取日志信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/GetShiftRequestMemoInfoBySysNo/{memoSysNo}", Method = "GET")]
        public ShiftRequestMemoInfo GetShiftRequestMemoInfoBySysNo(string memoSysNo)
        {
            int sysNo = int.Parse(memoSysNo);
            ShiftRequestMemoInfo result = ObjectFactory<ShiftRequestAppService>.Instance.GetShiftRequestMemoInfoBySysNo(sysNo);
            return result;
        }

        #endregion 移仓单跟进日志


        #region 仓库移仓配置
        [WebInvoke(UriTemplate = "/StockShiftConfig/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryStockShiftConfig(StockShiftConfigFilter filter)
        {
            DataTable dt = null;
            int totalCount;
            dt = ObjectFactory<IShiftRequestQueryDA>.Instance.QueryStockShiftConfig(filter, out totalCount);
            return new QueryResult
            {
                Data = dt,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/StockShiftConfig/Create", Method = "POST")]
        public StockShiftConfigInfo CreateStockShiftConfig(StockShiftConfigInfo info)
        {
            return ObjectFactory<ShiftRequestAppService>.Instance.CreateStockShiftConfig(info);
        }
        /// <summary>
        /// 返回true表示修改成功，否则修改失败
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/StockShiftConfig/Update", Method = "PUT")]
        public StockShiftConfigInfo UpdateStockShiftConfig(StockShiftConfigInfo info)
        {
            ObjectFactory<ShiftRequestAppService>.Instance.UpdateStockShiftConfig(info);
            return info;
        }
        [WebInvoke(UriTemplate = "/StockShiftConfig/Get/{sysNo}", Method = "GET")]
        public StockShiftConfigInfo GetStockShiftConfigBySysNo(string sysNo)
        {
            int no = int.TryParse(sysNo, out no) ? no : 0;
            return ObjectFactory<ShiftRequestAppService>.Instance.GetStockShiftConfigBySysNo(no);
        }

        #endregion


        #region RMA移仓单查询

        /// <summary>
        /// 根据系统编号加载移仓单详细信息
        /// </summary>
        /// <param name="shiftSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ShiftRequest/GetRMAShift/{shiftSysNo}", Method = "GET")]
        public virtual QueryResult GetRMAShift(string shiftSysNo)
        {
            int sysNo = int.Parse(shiftSysNo);
            return new QueryResult()
            {
                Data = ObjectFactory<IShiftRequestQueryDA>.Instance.GetRMAShift(sysNo),
                TotalCount = 0
            };
        }

        #endregion
    }
}
