using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.AppService.Promotion;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.MKT.Promotion;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.BizEntity;
using System.Data;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        [WebGet(UriTemplate = "/SaleGift/{sysNo}")]
        public virtual SaleGiftInfo LoadSaleGiftInfo(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.SaleGift", "SaleGift_ActivitySysNoIsNotActive"));
            }
            return ObjectFactory<SaleGiftAppService>.Instance.Load(id);
        }


        /// <summary>
        /// 创建赠主信息 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SaleGift/CreateMaster", Method = "POST")]
        public virtual int? CreateSaleGiftMaster(SaleGiftInfo info)
        {
            return ObjectFactory<SaleGiftAppService>.Instance.CreateMaster(info);
        }

        /// <summary>
        /// 更新主信息
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/SaleGift/UpdateMaster", Method = "PUT")]
        public void UpdateSaleGiftMaster(SaleGiftInfo info)
        {
            ObjectFactory<SaleGiftAppService>.Instance.UpdateMaster(info);
        }

        /// <summary>
        /// 促销活动规则设置
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/SaleGift/SetSaleRules", Method = "PUT")]
        public void SetSaleGiftSaleRules(SaleGiftInfo info)
        {
            ObjectFactory<SaleGiftAppService>.Instance.SetSaleRules(info);
        }

        /// <summary>
        /// 赠品设置
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/SaleGift/SetGiftItemRules", Method = "PUT")]
        public void SetSaleGiftGiftItemRules(SaleGiftInfo info)
        {
            ObjectFactory<SaleGiftAppService>.Instance.SetGiftItemRules(info);
        }

        //-------------------------查询---------------
        [WebInvoke(UriTemplate = "/SaleGift/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QuerySaleGift(SaleGiftQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<ISaleGiftQueryDA>.Instance.QuerySaleGift(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询商品上下架LOG
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SaleGift/QuerySaleGiftLog", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QuerySaleGiftLog(SaleGiftLogQueryFilter request)
        {
            int totalCount;
            var data = ObjectFactory<ISaleGiftQueryDA>.Instance.QuerySaleGiftLog(request, out totalCount);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }


        //---------------全局行为-------------------------
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/SaleGift/SubmitAudit", Method = "POST")]
        public virtual BatchResultRsp SaleGiftSubmitAudit(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();

            ObjectFactory<SaleGiftAppService>.Instance.SubmitAudit(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/SaleGift/CancelAudit", Method = "POST")]
        public virtual BatchResultRsp SaleGiftCancelAudit(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<SaleGiftAppService>.Instance.CancelAudit(sysNoList, out successRecords, out failureRecords);

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
        [WebInvoke(UriTemplate = "/SaleGift/AuditApprove", Method = "POST")]
        public virtual BatchResultRsp SaleGiftAuditApprove(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<SaleGiftAppService>.Instance.Audit(sysNoList, PromotionAuditType.Approve, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }
        [WebInvoke(UriTemplate = "/SaleGift/AuditRefuse", Method = "POST")]
        public virtual BatchResultRsp SaleGiftAuditRefuse(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<SaleGiftAppService>.Instance.Audit(sysNoList, PromotionAuditType.Refuse, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }
        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/SaleGift/Void", Method = "POST")]
        public virtual BatchResultRsp SaleGiftVoid(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();

            ObjectFactory<SaleGiftAppService>.Instance.Void(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

        /// <summary>
        /// 手动提前中止
        /// </summary>
        /// <param name="sysNo"></param>
        [WebInvoke(UriTemplate = "/SaleGift/ManualStop", Method = "POST")]
        public virtual BatchResultRsp SaleGiftManualStop(List<int?> sysNoList)
        {
            List<string> successRecords = new List<string>();
            List<string> failureRecords = new List<string>();
            ObjectFactory<SaleGiftAppService>.Instance.ManualStop(sysNoList, out successRecords, out failureRecords);

            BatchResultRsp response = new BatchResultRsp();
            response.SuccessRecords = successRecords;
            response.FailureRecords = failureRecords;
            return response;
        }

        /// <summary>
        /// 复制新建
        /// </summary>
        /// <param name="oldSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SaleGift/CopyCreateNew", Method = "POST")]
        public virtual int? CopyCreateNew(int? oldSysNo)
        {
            return ObjectFactory<SaleGiftAppService>.Instance.CopyCreateNew(oldSysNo);

        }


        [WebInvoke(UriTemplate = "/SaleGift/GetValidVenderGifts/{productSysNo}", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetValidVenderGifts(string productSysNo)
        {
            int totalCount;
            int sysNo = 0;
            if (string.IsNullOrEmpty(productSysNo) || !int.TryParse(productSysNo, out sysNo))
            {
                return null;
            }

            DataTable dataTable = ObjectFactory<ISaleGiftQueryDA>.Instance.GetValidVenderGifts(sysNo, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };

        }

        [WebInvoke(UriTemplate = "/SaleGift/GetGiftItemByMasterProducts", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetGiftItemByMasterProduct(GetGiftItemByMasterProductsReq request)
        {
            DataTable dt = ObjectFactory<ISaleGiftQueryDA>.Instance.GetGiftItemByMasterProducts(request.BeginDateTime, request.EndDateTime, request.MasterProductSysNoList);
            return new QueryResult()
            {
                Data = dt,
                TotalCount = dt == null ? 0 : dt.Rows.Count
            };
        }

        /// <summary>
        /// 批量创建赠品。
        /// </summary>
        /// <param name="info"></param>
        [WebInvoke(UriTemplate = "/SaleGift/BatchCreateSaleGift", Method = "POST")]
        public void BatchCreateSaleGift(SaleGiftBatchInfo info)
        {
            ObjectFactory<BatchCreateSaleGiftAppService>.Instance.BatchCreateSaleGift(info);
        }
         /// <summary>
        ///检查主商品和赠品库存后返回结果
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
         [WebInvoke(UriTemplate = "/SaleGift/CheckGiftStockResult", Method = "POST")]
        public string CheckGiftStockResult(SaleGiftInfo info)
        {
            return ObjectFactory<SaleGiftAppService>.Instance.CheckGiftStockResult(info);
        }

        /// <summary>
        /// 获取赠品的所有商家
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/SaleGift/GetGiftVendorList", Method = "POST")]
         public List<RelVendor> GetGiftVendorList() 
         {
             return ObjectFactory<SaleGiftAppService>.Instance.GetGiftVendorList();
         }
    }
}
