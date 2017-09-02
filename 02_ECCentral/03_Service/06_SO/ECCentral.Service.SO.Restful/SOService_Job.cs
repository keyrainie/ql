using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.AppService;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.SO.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.SO.Restful
{
    public partial class SOService
    {
        #region Job 相关方法
        /// <summary>
        /// 处理团购已完成了的订单和无效的团购订单
        /// </summary>
        /// <param name="companyCode"></param>
        [WebInvoke(UriTemplate = "SO/Job/ProcessFinishedAndInvalidGroupBuySO", Method = "PUT")]
        public virtual void ProcessFinishedAndInvalidGroupBuySO(string companyCode)
        {
            ObjectFactory<SOAppService>.Instance.ProcessFinishedGroupBuySO(companyCode);
            ObjectFactory<SOAppService>.Instance.ProcessorInvalidGroupBuySO(companyCode);
        }

        /// <summary>
        /// 作废48小时内没有付款的团购订单
        /// </summary>
        /// <param name="companyCode"></param>
        [WebInvoke(UriTemplate = "SO/Job/AbandonNotPayGroupBuySO", Method = "PUT")]
        public virtual void AbandonNotPayGroupBuySO(string companyCode)
        {
            ObjectFactory<SOAppService>.Instance.AbandonNotPayGroupBuySO(companyCode);
        }

        ///// <summary>
        ///// 审核订单通过发送邮件和短信以及更新数据库
        ///// </summary>
        ///// <param name="soSysNo">订单编号</param>
        //[WebInvoke(UriTemplate = "/SO/Job/AutoSendMessageSO/{soSysNo}", Method = "PUT")]
        //public virtual void AuditSendMailAndUpdateSO(string soSysNo)
        //{
        //    int sysNo;
        //    if (int.TryParse(soSysNo, out sysNo))
        //    {
        //        ObjectFactory<SOAppService>.Instance.AuditSendMailAndUpdateSO(sysNo);
        //    }
        //}
        #endregion

        //FPCheck
        /// <summary>
        /// 处理FPCheck检查
        /// </summary>
        /// <param name="req">请求体</param>
        [WebInvoke(UriTemplate = "/SO/Job/FPCheck", Method = "PUT")]
        public void FPCheck(JobFPCheckReq req)
        {
            ObjectFactory<SOJobAppService>.Instance.FPCheck(req.CompanyCode, req.IgnoreCustomIDList
                                                            , req.Interorder, req.OutStockList);
        }

        //SendCPS
        /// <summary>
        /// CPS发送请求
        /// </summary>
        /// <param name="req">请求消息体</param>
        [WebInvoke(UriTemplate = "/SO/Job/CPSSend", Method = "PUT")]
        public void CPSSend(JobCPSSendReq req)
        {
            ObjectFactory<SOJobAppService>.Instance.CPSSend(req.TargetUrl, req.SPCode, req.Fanli, req.CompanyCode);
        }

        //InternalMemoReportSend
        /// <summary>
        /// 订单跟进报告
        /// </summary>
        /// <param name="req">请求消息体</param>
        [WebInvoke(UriTemplate = "/SO/Job/InternalMemoReportSend", Method = "PUT")]
        public void InternalMemoReportSend(JobInternalMemoReportReq req)
        {
            ObjectFactory<SOJobAppService>.Instance.InternalMemoReportSend(req.EmailTo, req.EmailCC, req.CompanyCode);
            //test
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            //ObjectFactory<SOJobAppService>.Instance.InternalMemoReportSend("love@ccas.com", "", "8601");
        }

        //AutoAuditSO
        /// <summary>
        /// 自动审单
        /// </summary>
        /// <param name="req">请求消息体</param>
        [WebInvoke(UriTemplate = "/SO/Job/AutoAuditSO", Method = "PUT")]
        public void AutoAuditSO(JobAutoAuditSOReq req)
        {
            ObjectFactory<SOJobAppService>.Instance.AutoAuditSO(req.Interorder, req.CompanyCode, req.AutoAuditUserSysNo);
            //test
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            //ObjectFactory<SOJobAppService>.Instance.AutoAuditSO("PromotionIntel2009Q4", "8601", 700);
        }

        #region
        [WebInvoke(UriTemplate = "SO/Job/DownLoadChannalSOMessage", Method = "PUT")]
        public virtual void DownLoadChannalSOMessage()
        {

        }
        #endregion

        /// <summary>
        /// 订单出库后
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        [WebInvoke(UriTemplate = "/SO/Job/SendCommentNotifyMail", Method = "POST")]
        public void SendCommentNotifyMail(string soSysNo)
        {
            int sysNo;
            if (int.TryParse(soSysNo, out sysNo) && sysNo > 0)
            {
                ObjectFactory<SOAppService>.Instance.SendOrderCommentNotifyMail(sysNo);
            }
        }

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/GetWaitingFinishShippingList", Method = "POST")]
        public List<string> GetWaitingFinishShippingList(ExpressType expressType)
        {
            return ObjectFactory<SOJobAppService>.Instance.GetWaitingFinishShippingList(expressType);
        }

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/QueryTracking", Method = "POST")]
        public void QueryTracking(ExpressQueryEntity expressQuery)
        {
            ObjectFactory<SOJobAppService>.Instance.QueryTracking(expressQuery);
        }

        /// <summary>
        /// 获取待申报的订单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/GetWaitDeclareSO", Method = "GET")]
        public List<WaitDeclareSO> GetWaitDeclareSO()
        {
            return ObjectFactory<SOJobAppService>.Instance.GetWaitDeclareSO();
        }

        /// <summary>
        /// 申报订单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/DeclareSO", Method = "POST")]
        public bool DeclareSO(WaitDeclareSO entity)
        {
            return ObjectFactory<SOJobAppService>.Instance.DeclareSO(entity);
        }

        #region 商检商品
        /// <summary>
        /// 获取商检状态为审核通过的商品
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/GetEntryAuditSucess", Method = "GET")]
        public List<WaitDeclareProduct> GetEntryAuditSucess()
        {
            return ObjectFactory<SOJobAppService>.Instance.GetEntryAuditSucess();
        }
        /// <summary>
        /// 标记商品商检状态为商检中
        /// </summary>
        /// <param name="products"></param>
        [WebInvoke(UriTemplate = "/SO/Job/MarkInInspection", Method = "PUT")]
        public void MarkInInspection(List<WaitDeclareProduct> products)
        {
            ObjectFactory<SOJobAppService>.Instance.MarkInInspection(products);
        }
        /// <summary>
        /// 获取商品商检状态为商检中的商品
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/GetInInspectionProduct", Method = "GET")]
        public List<WaitDeclareProduct> GetInInspectionProduct()
        {
            return ObjectFactory<SOJobAppService>.Instance.GetInInspectionProduct();
        }
        /// <summary>
        /// 标记商品商检状态为商检通过，待报关
        /// </summary>
        /// <param name="products"></param>
        [WebInvoke(UriTemplate = "/SO/Job/MarkSuccessInspection", Method = "PUT")]
        public void MarkSuccessInspection(List<WaitDeclareProduct> products)
        {
            ObjectFactory<SOJobAppService>.Instance.MarkSuccessInspection(products);
        }
        #endregion

        #region 申报商品
        /// <summary>
        /// 获取待申报的商品
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/GetWaitDeclareProduct", Method = "GET")]
        public List<WaitDeclareProduct> GetWaitDeclareProduct()
        {
            return ObjectFactory<SOJobAppService>.Instance.GetWaitDeclareProduct();
        }
        /// <summary>
        /// 申报商品
        /// </summary>
        /// <param name="decalreList">待申报商品列表，必须是同一个商家，不能超过10个商品</param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/DeclareProduct", Method = "POST")]
        public DeclareProductResult DeclareProduct(List<WaitDeclareProduct> decalreList)
        {
            return ObjectFactory<SOJobAppService>.Instance.DeclareProduct(decalreList);
        }
        /// <summary>
        /// 申报商品结果处理
        /// </summary>
        /// <param name="callbackString"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SO/Job/DeclareProductCallBack", Method = "POST")]
        public bool DeclareProductCallBack(string callbackString)
        {
            return ObjectFactory<SOJobAppService>.Instance.DeclareProductCallBack(callbackString);
        }
        #endregion
    }
}
