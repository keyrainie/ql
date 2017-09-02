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
using ECCentral.Service.SO.Restful.RequestMsg;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        /// <summary>
        /// 添加订单拦截邮件邮件配置信息
        /// </summary>
        /// <param name="info">拦截邮件邮件配置信息</param>
        [WebInvoke(UriTemplate = "/SO/AddSOInterceptInfo", Method = "POST")]
        public virtual void AddSOInterceptInfo(SOInterceptInfo info)
        {
            ObjectFactory<SOInterceptAppService>.Instance.AddSOInterceptInfo(info, info.CompanyCode);
        }

        /// <summary>
        /// 批量更新订单拦截邮件配置信息
        /// </summary>
        /// <param name="info">订单拦截邮件配置信息</param>
        [WebInvoke(UriTemplate = "/SO/BatchUpdateSOInterceptInfo", Method = "PUT")]
        public virtual void BatchUpdateSOInterceptInfo(SOInterceptInfo info)
        {
            ObjectFactory<SOInterceptAppService>.Instance.BatchUpdateSOInterceptInfo(info);              
        }

        /// <summary>
        /// 删除订单拦截邮件配置信息
        /// </summary>
        /// <param name="info">订单拦截邮件配置信息</param>
        [WebInvoke(UriTemplate = "/SO/DeleteSOInterceptInfo", Method = "PUT")]
        public virtual void DeleteSOInterceptInfo(SOInterceptInfo info)
        {
            ObjectFactory<SOInterceptAppService>.Instance.DeleteSOInterceptInfo(info);
        }


        #region Email

        /// <summary>
        /// 发送订单拦截邮件
        /// </summary>
        /// <param name="request">订单拦截邮件信息</param>
        [WebInvoke(UriTemplate = "/SO/SendSOInterceptEmail", Method = "PUT")]
        public void SendSOOrderInterceptEmail(SendEmailReq request)
        {
             ObjectFactory<SOInterceptAppService>.Instance.SendSOOrderInterceptEmail(request.soInfo, request.Language);
        }

        /// <summary>
        /// 发送发票拦截邮件
        /// </summary>
        /// <param name="request">发票拦截邮件信息</param>
        [WebInvoke(UriTemplate = "/SO/SendSOFinanceInterceptEmail", Method = "PUT")]
        public void SendSOFinanceInterceptEmail(SendEmailReq request)
        {
             ObjectFactory<SOInterceptAppService>.Instance.SendSOFinanceInterceptEmail(request.soInfo, request.Language);
        }

        #endregion
    }
}
