using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 加载一个优惠券信息
        /// </summary>
        [WebGet(UriTemplate = "/Coupons/{sysNo}")]
        public virtual CouponsInfo LoadCoupons(string sysNo)
        {
            int id = Convert.ToInt32(sysNo);
            return ObjectFactory<CouponsAppService>.Instance.Load(id);
        }

        [WebGet(UriTemplate = "/Coupons/GetCouponGrossMarginRate/{productSysNo}")]
        public virtual decimal? GetCouponGrossMarginRate(string productSysNo)
        {
            return ObjectFactory<CouponsAppService>.Instance.GetCouponGrossMarginRate(int.Parse(productSysNo));
        }

        /// <summary>
        /// Load 活动基本信息
        /// </summary>
        [WebGet(UriTemplate = "/Coupons/Master/{sysNo}")]
        public virtual CouponsInfo LoadCouponsMaster(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.Coupons", "Coupons_ActivitySysNoIsNotActive"));
            }
            return ObjectFactory<CouponsAppService>.Instance.LoadMaster(id);
        }

        /// <summary>
        /// 根据优惠券SysNo 获取对应优惠券的折扣活动信息
        /// </summary>
        /// <param name="sysNo">优惠券系统SysNo</param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/Coupons/GetCouponsInfoByCouponCodeSysNo/{couponCodeSysNo}")]
        public virtual CouponsInfo GetCouponsInfoByCouponCodeSysNo(string couponCodeSysNo)
        {
            return ObjectFactory<CouponsAppService>.Instance.GetCouponsInfoByCouponCodeSysNo(int.Parse(couponCodeSysNo));
        }

        [WebInvoke(UriTemplate = "/Coupons/CreateMaster", Method = "POST")]
        public virtual int? CreateCouponsMaster(CouponsInfo info)
        {
            return ObjectFactory<CouponsAppService>.Instance.CreateMaster(info);
        }

        [WebInvoke(UriTemplate = "/Coupons/UpdateMaster", Method = "PUT")]
        public virtual CouponsInfo UpdateCouponsMaster(CouponsInfo info)
        {
            return ObjectFactory<CouponsAppService>.Instance.UpdateMaster(info);
        }

        [WebInvoke(UriTemplate = "/Coupons/SetProductCondition", Method = "PUT")]
        public virtual void SetCouponsProductCondition(CouponsInfo info)
        {
            ObjectFactory<CouponsAppService>.Instance.SetProductCondition(info);
        }

        [WebInvoke(UriTemplate = "/Coupons/SetDiscountRule", Method = "PUT")]
        public virtual void SetCouponsDiscountRule(CouponsInfo info)
        {
            ObjectFactory<CouponsAppService>.Instance.SetDiscountRule(info);
        }

        [WebInvoke(UriTemplate = "/Coupons/SetSaleRuleEx", Method = "PUT")]
        public virtual void SetCouponsSaleRuleEx(CouponsInfo info)
        {
            ObjectFactory<CouponsAppService>.Instance.SetSaleRuleEx(info);
        }

        [WebInvoke(UriTemplate = "/Coupons/SetCustomerCondition", Method = "PUT")]
        public virtual void SetCouponsCustomerCondition(CouponsInfo info)
        {
            ObjectFactory<CouponsAppService>.Instance.SetCustomerCondition(info);
        }

        /// <summary>
        /// 查询优惠券代码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Coupons/CouponCode/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCouponCode(CouponCodeQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICouponsQueryDA>.Instance.QueryCouponCode(request, out totalCount).Tables[0];
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Coupons/CouponCode/Create", Method = "POST")]
        public virtual void CreateCouponCode(CouponCodeSetting info)
        {
            ObjectFactory<CouponsAppService>.Instance.CreateCouponCode(info);
        }

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="couponCodeSysNoList"></param>
        [WebInvoke(UriTemplate = "/Coupons/CouponCode/DelCouponCode", Method = "DELETE")]
        public virtual void DelCouponCode(List<int?> couponCodeSysNoList)
        {
            ObjectFactory<CouponsAppService>.Instance.DelCouponCode(couponCodeSysNoList);
        }

        /// <summary>
        /// 删除全部优惠券
        /// </summary>
        /// <param name="couponsSysNo"></param>
        [WebInvoke(UriTemplate = "/Coupons/CouponCode/DelAllCouponCode", Method = "DELETE")]
        public virtual void DelAllCouponCode(int? couponsSysNo)
        {
            ObjectFactory<CouponsAppService>.Instance.DelAllCouponCode(couponsSysNo);
        }


        [WebInvoke(UriTemplate = "/Coupons/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCoupons(CouponsQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICouponsQueryDA>.Instance.QueryCoupons(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Coupons/QueryCustomerRedeemLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCouponCodeRedeemLog(CouponCodeRedeemLogFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICouponsQueryDA>.Instance.QueryCouponCodeRedeemLog(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Coupons/QueryCustomerGetLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryCouponCodeCustomerLog(CouponCodeCustomerLogFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ICouponsQueryDA>.Instance.QueryCouponCodeCustomerLog(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }


        //---------------全局行为-------------------------
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/Coupons/SubmitAudit", Method = "POST")]
        public virtual BatchResultRsp CouponsSubmitAudit(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();

            ObjectFactory<CouponsAppService>.Instance.SubmitAudit(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysNo"></param>
       [WebInvoke(UriTemplate = "/Coupons/CancelAudit", Method = "POST")]
        public virtual BatchResultRsp CouponsCancelAudit(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<CouponsAppService>.Instance.CancelAudit(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

        /// <summary>
        /// 审核，包含审核通过和审核拒绝
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="auditType"></param>
        [WebInvoke(UriTemplate = "/Coupons/AuditApprove", Method = "POST")]
        public virtual BatchResultRsp CouponsAuditApprove(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<CouponsAppService>.Instance.Audit(sysNoList, PromotionAuditType.Approve, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }
        [WebInvoke(UriTemplate = "/Coupons/AuditRefuse", Method = "POST")]
        public virtual BatchResultRsp CouponsAuditRefuse(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<CouponsAppService>.Instance.Audit(sysNoList, PromotionAuditType.Refuse, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }
        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/Coupons/Void", Method = "POST")]
        public virtual BatchResultRsp CouponsVoid(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();

            ObjectFactory<CouponsAppService>.Instance.Void(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

        /// <summary>
        /// 手动提前中止
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/Coupons/ManualStop", Method = "POST")]
        public virtual BatchResultRsp CouponsManualStop(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<CouponsAppService>.Instance.ManualStop(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

    }

}
