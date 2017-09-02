using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.Service.SO.BizProcessor.Job;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.AppService
{
    [VersionExport(typeof(SOJobAppService))]
    public class SOJobAppService
    {
        public void FPCheck(string companyCode, List<string> ignoreCustomIDList, string interorder, List<int> outStockList)
        {
            ObjectFactory<SOFPCheckProcessor>.Instance.Check(companyCode, ignoreCustomIDList, interorder, outStockList);
        }

        public void CPSSend(string targetUrl, string spCode, decimal fanli, string companyCode)
        {
            ObjectFactory<SOCPSSendProcessor>.Instance.Run(targetUrl, spCode, fanli, companyCode);
        }

        public void InternalMemoReportSend(string emailTo, string emailCC, string companyCode)
        {
            ObjectFactory<InternalMemoReportProcessor>.Instance.Run(emailTo, emailCC, companyCode);
        }

        public void AutoAuditSO(string interOrder, string companyCode, int auditUserSysNo)
        {
            ObjectFactory<SOAutoAuditProcessor>.Instance.AuditSO(interOrder, companyCode, auditUserSysNo);
        }

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        public List<string> GetWaitingFinishShippingList(ExpressType expressType)
        {
            if (expressType == ExpressType.SF)
            {
                return ObjectFactory<SFExpressProcessor>.Instance.GetWaitingFinishShippingList();
            }
            else if (expressType == ExpressType.YT)
            {
                return ObjectFactory<YTExpressProcessor>.Instance.GetWaitingFinishShippingList();
            }
            else if (expressType == ExpressType.KD100)
            {
                return ObjectFactory<KD100Processor>.Instance.GetWaitingFinishShippingList();
            }

            return new List<string>();
        }

        /// <summary>
        /// 查询物流信息
        /// </summary>
        /// <param name="expressQuery"></param>
        public void QueryTracking(ExpressQueryEntity expressQuery)
        {
            if (expressQuery.Type == ExpressType.SF)
            {
                ObjectFactory<SFExpressProcessor>.Instance.QueryTracking(expressQuery.TrackingNumberList);
            }
            else if (expressQuery.Type == ExpressType.YT)
            {
                ObjectFactory<YTExpressProcessor>.Instance.QueryTracking(expressQuery.TrackingNumberList);
            }
            else if (expressQuery.Type == ExpressType.KD100)
            {
                ObjectFactory<KD100Processor>.Instance.QueryTracking(expressQuery.TrackingNumberList);
            }
        }

        /// <summary>
        /// 获取待申报的订单
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareSO> GetWaitDeclareSO()
        {
            return ObjectFactory<EasiPayProcessor>.Instance.GetWaitDeclareSO();
        }

        /// <summary>
        /// 申报订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool DeclareSO(WaitDeclareSO entity)
        {
            return ObjectFactory<EasiPayProcessor>.Instance.DeclareSO(entity);
        }

        #region 商检商品
        /// <summary>
        /// 获取商检状态为审核通过的商品
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetEntryAuditSucess()
        {
            return ObjectFactory<EasiPayProcessor>.Instance.GetEntryAuditSucess();
        }
        /// <summary>
        /// 标记商品商检状态为商检中
        /// </summary>
        /// <param name="products"></param>
        public void MarkInInspection(List<WaitDeclareProduct> products)
        {
            ObjectFactory<EasiPayProcessor>.Instance.MarkInInspection(products);
        }
        /// <summary>
        /// 获取商品商检状态为商检中的商品
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetInInspectionProduct()
        {
            return ObjectFactory<EasiPayProcessor>.Instance.GetInInspectionProduct();
        }
        /// <summary>
        /// 标记商品商检状态为商检通过，待报关
        /// </summary>
        /// <param name="products"></param>
        public void MarkSuccessInspection(List<WaitDeclareProduct> products)
        {
            ObjectFactory<EasiPayProcessor>.Instance.MarkSuccessInspection(products);
        }
        #endregion

        #region 申报商品
        /// <summary>
        /// 获取待申报的商品
        /// </summary>
        /// <returns></returns>
        public List<WaitDeclareProduct> GetWaitDeclareProduct()
        {
            return ObjectFactory<EasiPayProcessor>.Instance.GetWaitDeclareProduct();
        }
        /// <summary>
        /// 申报商品
        /// </summary>
        /// <param name="decalreList">待申报商品列表，必须是同一个商家，不能超过10个商品</param>
        /// <returns></returns>
        public DeclareProductResult DeclareProduct(List<WaitDeclareProduct> decalreList)
        {
            return ObjectFactory<EasiPayProcessor>.Instance.DeclareProduct(decalreList);
        }
        /// <summary>
        /// 申报商品结果处理
        /// </summary>
        /// <param name="callbackString"></param>
        /// <returns></returns>
        public bool DeclareProductCallBack(string callbackString)
        {
            return ObjectFactory<EasiPayProcessor>.Instance.DeclareProductCallBack(callbackString);
        }
        #endregion
    }
}
