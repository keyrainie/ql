using System;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;

using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.BizEntity;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 查询创建日志用户列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns>用户列表信息</returns>
        [WebInvoke(UriTemplate = "/SO/QueryCreateLogUser/{companyCode}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public List<CSInfo> QueryCreateLogUser(string companyCode)
        {
            return ObjectFactory<SOInternalMemoAppService>.Instance.GetSOLogCreater(companyCode);
        }

        /// <summary>
        /// 查询更新日志用户列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns>用户列表信息</returns>
        [WebInvoke(UriTemplate = "/SO/QueryUpdateLogUser/{companyCode}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public List<CSInfo> QueryUpdateLogUser(string companyCode)
        {
            return ObjectFactory<SOInternalMemoAppService>.Instance.GetSOLogUpdater(companyCode);
        }

        /// <summary>
        /// 创建订单跟进日志
        /// </summary>
        /// <param name="info">请求体</param>
        [WebInvoke(UriTemplate = "/SO/AddSOInternalMemoInfo", Method = "POST")]
        public virtual void AddSOInternalMemoInfo(SOInternalMemoInfo info)
        {
            ObjectFactory<SOInternalMemoAppService>.Instance.AddSOInternalMemoInfo(info, info.CompanyCode);
        }

        /// <summary>
        /// 批量分配订单跟进日志
        /// </summary>
        /// <param name="infoList">请求体列表</param>
        [WebInvoke(UriTemplate = "/SO/BatchAssignSOInternalMemo", Method = "PUT")]
        public virtual void BatchAssignSOInternalMemo(List<SOInternalMemoInfo> infoList)
        {
            ObjectFactory<SOInternalMemoAppService>.Instance.BatchAssignSOInternalMemo(infoList);              
        }

        /// <summary>
        /// 批量取消分配订单跟进日志
        /// </summary>
        /// <param name="infoList">请求体列表</param>
        [WebInvoke(UriTemplate = "/SO/BathCanceAssignSOInternalMemo", Method = "PUT")]
        public virtual void BatchCanceAssignSOInternalMemo(List<SOInternalMemoInfo> infoList)
        {
            ObjectFactory<SOInternalMemoAppService>.Instance.BatchCanceAssignSOInternalMemo(infoList);        
        }  

        /// <summary>
        /// 关闭订单跟进日志
        /// </summary>
        /// <param name="info">请求体</param>
        [WebInvoke(UriTemplate = "/SO/CloseSOInternalMemo", Method = "PUT")]
        public virtual void CloseSOInternalMemo(SOInternalMemoInfo info)
        {
            ObjectFactory<SOInternalMemoAppService>.Instance.CloseSOInternalMemo(info);
        }
    }
}
