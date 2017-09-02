using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel.Web;

using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 团购分页查询服务
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/Query", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryGroupBuying(GroupBuyingQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IGroupBuyingQueryDA>.Instance.Query(msg, out totalCount);
            //为DataSet中的两个table建立关系
            DataRelation relGroupBuyingPrice = new DataRelation("GroupBuyingPrice",
                ds.Tables[0].Columns["SysNo"],
                ds.Tables[1].Columns["ProductGroupBuyingSysNo"]);
            ds.Relations.Add(relGroupBuyingPrice);
            //在第一个table中增加一列容纳阶梯价格
            const string priceInfoColName = "PriceInfo";
            string priceInfoFormat = "{0}({1}人)" + Environment.NewLine;
            var dtMaster = ds.Tables[0];
            dtMaster.Columns.Add(priceInfoColName, typeof(string));
            //将阶梯价格拼接到自定义列
            foreach (DataRow drMaster in dtMaster.Rows)
            {
                string priceInfoContent = "";
                foreach (DataRow drPrice in drMaster.GetChildRows(relGroupBuyingPrice))
                {
                    priceInfoContent += string.Format(priceInfoFormat
                        , Convert.ToDecimal(drPrice["GroupBuyingPrice"]).ToString("F2")
                        , drPrice["SellCount"]);
                }
                drMaster[priceInfoColName] = priceInfoContent;
            }
            return new QueryResult()
            {
                Data = ds.Tables[0],
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 创建团购
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/Create", Method = "POST")]
        public virtual GroupBuyingInfo CreateGroupBuying(GroupBuyingInfo item)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.Create(item);
        }

        /// <summary>
        /// 更新团购
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/Update", Method = "PUT")]
        public virtual GroupBuyingInfo UpdateGroupBuying(GroupBuyingInfo item)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.Update(item);
        }

        /// <summary>
        /// 加载一个团购信息
        /// </summary>
        [WebGet(UriTemplate = "/GroupBuying/{sysNo}")]
        public virtual GroupBuyingInfo LoadGroupBuying(string sysNo)
        {
            int id = 0;
            if (!int.TryParse(sysNo, out id))
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.Promotion.GroupBuying", "GroupBuying_ActivitySysNoIsNotActive"));
            }

            return ObjectFactory<GroupBuyingAppService>.Instance.Load(id);
        }

        [WebGet(UriTemplate = "/GroupBuying/GetGroupBuyingTypes")]
        public virtual Dictionary<int, string> GetGroupBuyingTypes()
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.GetGroupBuyingTypes();
        }

        [WebGet(UriTemplate = "/GroupBuying/GetGroupBuyingAreas")]
        public virtual Dictionary<int, string> GetGroupBuyingAreas()
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.GetGroupBuyingAreas();
        }

        [WebGet(UriTemplate = "/GroupBuying/GetGroupBuyingVendors")]
        public virtual Dictionary<int, string> GetGroupBuyingVendors()
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.GetGroupBuyingVendors();
        }

        /// <summary>
        /// 作废
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GroupBuyingVoid", Method = "POST")]
        public virtual void Void(List<int> sysNoList)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.Void(sysNoList);
        }

        /// <summary>
        /// 终止
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GroupBuyingStop", Method = "POST")]
        public virtual void Stop(List<int> sysNoList)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.Stop(sysNoList);
        }

        /// <summary>
        /// 提交
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GroupBuyingSubmitAudit", Method = "PUT")]
        public virtual void SubmitAudit(int sysNo)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.SubmitAudit(sysNo);
        }

        /// <summary>
        /// 撤销
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GroupBuyingCancelAudit", Method = "PUT")]
        public virtual void CancelAudit(int sysNo)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.CancelAudit(sysNo);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GroupBuyingAuditApprove", Method = "PUT")]
        public virtual void AuditApprove(GroupBuyingAuditReq msg)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.AuditApprove(msg.SysNo.Value, msg.Reasons);
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GroupBuyingAuditRefuse", Method = "PUT")]
        public virtual void AuditRefuse(GroupBuyingAuditReq msg)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.AuditRefuse(msg.SysNo.Value, msg.Reasons);
        }

        /// <summary>
        ///  创建团购后提示各项毛利
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/LoadMarginRateInfo", Method = "POST")]
        public virtual List<GroupBuySaveInfo> LoadMarginRateInfo(GroupBuyingInfo groupBuyInfo)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.LoadMarginRateInfo(groupBuyInfo);
        }

        /// <summary>
        /// 获取商品的原价
        /// </summary>
        [WebInvoke(UriTemplate = "/GroupBuying/GetProductOriginalPrice", Method = "POST")]
        public virtual List<object> GetProductOriginalPrice(OriginalPriceReq req)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.GetProductOriginalPrice(req.ProductSysNo.Value, req.IsByGroup, req.CompanyCode);
        }
        /// <summary>
        /// 获取随心配在团购中的毛利率
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GroupBuying/GetProductPromotionMarginByGroupBuying", Method = "POST")]
        public virtual string GetProductPromotionMarginByGroupBuying(GroupBuyingInfo info)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.GetProductPromotionMarginByGroupBuying(info);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/CreateGroupBuyingCategory", Method = "POST")]
        public GroupBuyingCategoryInfo CreateGroupBuyingCategory(GroupBuyingCategoryInfo info)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.CreateGroupBuyingCategory(info);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/UpdateGroupBuyingCategory", Method = "PUT")]
        public GroupBuyingCategoryInfo UpdateGroupBuyingCategory(GroupBuyingCategoryInfo info)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.UpdateGroupBuyingCategory(info);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/GetAllGroupBuyingCategory", Method = "GET")]
        public List<GroupBuyingCategoryInfo> GetAllGroupBuyingCategory()
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.GetAllGroupBuyingCategory();
        }

        [WebInvoke(UriTemplate = "/GroupBuying/BatchReadGroupbuyingFeedback", Method = "PUT")]
        public string BatchReadGroupbuyingFeedback(List<int> sysNoList)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.BatchReadGroupbuyingFeedback(sysNoList);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/BatchHandleGroupbuyingBusinessCooperation", Method = "PUT")]
        public string BatchHandleGroupbuyingBusinessCooperation(List<int> sysNoList)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.BatchHandleGroupbuyingBusinessCooperation(sysNoList);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/HandleGroupbuyingBusinessCooperation", Method = "PUT")]
        public void HandleGroupbuyingBusinessCooperation(int sysNo)
        {
            ObjectFactory<GroupBuyingAppService>.Instance.HandleGroupbuyingBusinessCooperation(sysNo);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/QueryGroupbuyingFeedback", Method = "POST")]
        public virtual QueryResult QueryGroupbuyingFeedback(GroupBuyingFeedbackQueryFilter filter)
        {
            int totalCount;
            var dt = ObjectFactory<IGroupBuyingQueryDA>.Instance.QueryFeedback(filter, out totalCount);

            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/GroupBuying/QueryBusinessCooperation", Method = "POST")]
        public virtual QueryResult QueryBusinessCooperation(BusinessCooperationQueryFilter filter)
        {
            int totalCount;
            var dt = ObjectFactory<IGroupBuyingQueryDA>.Instance.QueryBusinessCooperation(filter, out totalCount);

            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/GroupBuying/QuerySettlement", Method = "POST")]
        public virtual QueryResult QuerySettlement(GroupBuyingSettlementQueryFilter filter)
        {
            int totalCount;
            var dt = ObjectFactory<IGroupBuyingQueryDA>.Instance.QuerySettlement(filter, out totalCount);

            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/GroupBuying/QueryGroupBuyingTicket", Method = "POST")]
        public virtual QueryResult QueryGroupBuyingTicket(GroupBuyingTicketQueryFilter filter)
        {
            int totalCount;
            var dt = ObjectFactory<IGroupBuyingQueryDA>.Instance.QueryGroupBuyingTicket(filter, out totalCount);

            return new QueryResult()
            {
                Data = dt,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/GroupBuying/BatchAuditGroupBuyingSettlement", Method = "PUT")]
        public string BatchAuditGroupBuyingSettlement(List<int> sysNoList)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.BatchAuditPassGroupBuyingSettlement(sysNoList);
        }

        [WebInvoke(UriTemplate = "/GroupBuying/LoadGroupBuyingSettlementItemBySettleSysNo", Method = "POST")]
        public QueryResult LoadGroupBuyingSettlementItemBySettleSysNo(int settlementSysNo)
        {            
            var dt = ObjectFactory<IGroupBuyingQueryDA>.Instance.LoadGroupBuyingSettlementItemBySettleSysNo(settlementSysNo);

            return new QueryResult()
            {
                Data = dt,                
            };
        }

        [WebInvoke(UriTemplate = "/GroupBuying/LoadTicketByGroupBuyingSysNo", Method = "POST")]
        public QueryResult LoadTicketByGroupBuyingSysNo(int groupBuyingSysNo)
        {
            var dt = ObjectFactory<IGroupBuyingQueryDA>.Instance.LoadTicketByGroupBuyingSysNo(groupBuyingSysNo);

            return new QueryResult()
            {
                Data = dt,
            };
        }

        [WebInvoke(UriTemplate = "/GroupBuying/BatchVoidGroupBuyingTicket", Method = "PUT")]
        public string BatchVoidGroupBuyingTicket(List<int> sysNoList)
        {
            return ObjectFactory<GroupBuyingAppService>.Instance.BatchVoidGroupBuyingTicket(sysNoList);
        }
    }
}
