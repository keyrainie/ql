using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        private GiftCardAppService giftCardAppService = ObjectFactory<GiftCardAppService>.Instance;

        /// <summary>
        /// 查询礼品卡
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/QueryGiftCardInfo", Method = "POST")]//, ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryGiftCardInfo(GiftCardFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IGiftCardQueryDA>.Instance.QueryGiftCardInfo(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询礼品卡制作单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/QueryGiftCardFabricationMaster", Method = "POST")]//, ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult QueryGiftCardFabricationMaster(GiftCardFabricationFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IGiftCardQueryDA>.Instance.QueryGiftCardFabricationMaster(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询礼品券关联商品
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/QueryGiftVoucherProductRelation", Method = "POST")]
        public virtual QueryResult QueryGiftVoucherProductRelation(GiftCardProductFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IGiftCardQueryDA>.Instance.QueryGiftVoucherProductRelation(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询礼品券关联请求
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/QueryGiftVoucherProductRelationReq", Method = "POST")]
        public virtual QueryResult QueryGiftVoucherProductRelationReq(GiftCardProductFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IGiftCardQueryDA>.Instance.QueryGiftVoucherProductRelationReq(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 礼品卡商品查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/QueryGiftCardProductInfo", Method = "POST")]
        public virtual QueryResult QueryGiftCardProductInfo(GiftCardProductFilter filter)
        {
            int totalCount;
            var dataTable = ObjectFactory<IGiftCardQueryDA>.Instance.QueryGiftCardProductInfo(filter, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchSetGiftCardInvalid", Method = "PUT")]
        public virtual void BatchSetGiftCardInvalid(List<GiftCardInfo> items)
        {
            giftCardAppService.BatchSetGiftCardInvalid(items);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/SetGiftCardInvalid", Method = "PUT")]
        public virtual void SetGiftCardInvalid(GiftCardInfo item)
        {
            giftCardAppService.SetGiftCardInvalid(item);
        }


        /// <summary>
        /// 批量锁定
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchLockGiftCard", Method = "PUT")]
        public virtual void BatchLockGiftCard(List<int> items)
        {
            giftCardAppService.BatchLockGiftCard(items);
        }


        /// <summary>
        /// 批量解锁
        /// </summary>
        /// <param name="items"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchUnLockGiftCard", Method = "PUT")]
        public virtual void BatchUnLockGiftCard(List<int> items)
        {
            giftCardAppService.BatchUnLockGiftCard(items);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/UpdateGiftCardInfo", Method = "PUT")]
        public virtual void UpdateGiftCardInfo(ECCentral.BizEntity.IM.GiftCardInfo item)
        {
            giftCardAppService.UpdateGiftCardInfo(item);
        }

        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/GetGiftCardOperateLogByCode", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual List<ECCentral.BizEntity.IM.GiftCardOperateLog> GetGiftCardOperateLogByCode(string code)
        {
            return giftCardAppService.GetGiftCardOperateLogByCode(code);
        }


        /// <summary>
        /// 获取礼品卡操作日志
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/GetGiftCardRedeemLogJoinSOMaster", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual List<ECCentral.BizEntity.IM.GiftCardRedeemLog> GetGiftCardRedeemLogJoinSOMaster(string code)
        {
            return giftCardAppService.GetGiftCardRedeemLogJoinSOMaster(code);
        }

        /// <summary>
        /// 更新礼品卡主体信息及其子项信息
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/UpdateGiftCardFabrications", Method = "PUT")]
        public virtual void UpdateGiftCardFabrications(GiftCardFabricationMaster item)
        {
            giftCardAppService.UpdateGiftCardFabrications(item);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="item"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/DeleteGiftCardFabrication", Method = "PUT")]
        public virtual void DeleteGiftCardFabrication(int sysNo)
        {
            giftCardAppService.DeleteGiftCardFabrication(sysNo);
        }

        [WebInvoke(UriTemplate = "/GiftCardInfo/CreatePOGiftCardFabrication", Method = "POST")]
        public virtual int CreatePOGiftCardFabrication(GiftCardFabricationMaster item)
        {
            return giftCardAppService.CreatePOGiftCardFabrication(item);
        }

        /// <summary>
        /// GetGiftCardFabricationItem
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/GetGiftCardFabricationItem", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual GiftCardFabricationItemRsp GetGiftCardFabricationItem(int sysNo)
        {
            GiftCardFabricationItemRsp item = new GiftCardFabricationItemRsp();
            List<ECCentral.BizEntity.IM.GiftCardFabrication> list = giftCardAppService.GetGiftCardFabricationItem(sysNo);

            DataTable dt = giftCardAppService.GetGiftCardFabricationItemSum(sysNo);
            decimal TotalPrice = 0;
            decimal TotalCount =0;
            if (dt != null && dt.Rows.Count > 0)
            {
                TotalPrice =  decimal.Parse(dt.Rows[0]["TotalPrice"].ToString());
                TotalCount =  decimal.Parse(dt.Rows[0]["TotalCount"].ToString());
            }

            return new GiftCardFabricationItemRsp()
            {
                GiftCardFabricationList = list,
                TotalPrice = TotalPrice,
                TotalCount = TotalCount
            };
        }

        /// <summary>
        /// 获取当前生成的需要导出的礼品卡信息
        /// 需要在后台生成excel文件供下载
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/GetGiftCardInfoByGiftCardFabricationSysNo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetGiftCardInfoByGiftCardFabricationSysNo(int sysNo)
        {
            var dataTable = giftCardAppService.GetGiftCardInfoByGiftCardFabricationSysNo(sysNo);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = dataTable.Rows == null ? 0 : dataTable.Rows.Count
            };
        }

        [WebInvoke(UriTemplate = "/GiftCardInfo/GetAddGiftCardInfoList", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual bool GetAddGiftCardInfoList(int sysNo)
        {
            return giftCardAppService.GetAddGiftCardInfoList(sysNo);
        }

        /// <summary>
        /// 批量激活
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchActivateGiftCard",Method = "PUT",ResponseFormat = WebMessageFormat.Json)]
        public virtual string BatchActivateGiftCard(List<int> sysNoList)
        {
            return giftCardAppService.BatchActiveGiftCard(sysNoList);
        }

        /// <summary>
        /// Batch cancel audit gift card product
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchCancelAuditGiftCardProduct", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public virtual string BatchCancelAuditVoucherProduct(List<int> sysNoList)
        {
            return giftCardAppService.BatchCancelAuditVoucherProduct(sysNoList);
        }

        /// <summary>
        /// Batch void gift card product
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchVoidGiftCardProduct", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public virtual string BatchVoidVoucherProduct(List<int> sysNoList)
        {
            return giftCardAppService.BatchVoidVoucherProduct(sysNoList);
        }

        /// <summary>
        /// Batch audit gift card product
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchAuditGiftCardProduct", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public virtual string BatchAuditVoucherProduct(List<int> sysNoList)
        {
            return giftCardAppService.BatchAuditVoucherProduct(sysNoList);
        }

        /// <summary>
        /// 增加礼品券商品
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/AddGiftVoucherProductInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual int AddGiftVoucherProductInfo(GiftVoucherProduct entity)
        {
            return giftCardAppService.AddGiftCardProductInfo(entity);
        }

        /// <summary>
        /// 加载礼品券商品
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [WebGet(UriTemplate ="/GiftCardInfo/GetGiftVoucherProductInfo/{sysNo}")]
        public virtual GiftVoucherProduct GetGiftVoucherProductInfo(string sysNo)
        {
            int val = 0;
            GiftVoucherProduct info = new GiftVoucherProduct();

            if(int.TryParse(sysNo,out val))
            info = giftCardAppService.GetGiftVoucherProductInfo(val);

            return info;
        }

        /// <summary>
        /// 更新礼品券商品
        /// </summary>
        /// <param name="entity"></param>
        [WebInvoke(UriTemplate = "/GiftCardInfo/UpdateVoucherProductInfo", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public virtual GiftVoucherProduct UpdateVoucherProductInfo(GiftVoucherProduct entity)
        {
            return giftCardAppService.UpdateVoucherProductInfo(entity);
        }

        /// <summary>
        /// 批量审核商品关系
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchAuditVoucherRequest", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public virtual string BatchAuditVoucherRequest(List<int> sysNoList)
        {
            return giftCardAppService.BatchAuditVoucherRequest(sysNoList);
        }

        /// <summary>
        /// 批量审核不通过商品关系
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/GiftCardInfo/BatchCancelAuditVoucherRequest", Method = "PUT", ResponseFormat = WebMessageFormat.Json)]
        public virtual string BatchCancelAuditVoucherRequest(List<int> sysNoList)
        {
            return giftCardAppService.BatchCancelAuditVoucherRequest(sysNoList);
        }

        [WebGet(UriTemplate = "/GiftCardInfo/GetGiftVoucherProductRelationRequest/{relationSysNo}")]
        public virtual List<GiftVoucherProductRelationRequest> GetGiftVoucherProductRelationRequest(string relationSysNo)
        {
            int sysNo;
            if(int.TryParse(relationSysNo,out sysNo))
            {
                return ObjectFactory<GiftCardAppService>.Instance.GetGiftVoucherProductRelationRequest(sysNo);
            }
            return null;
        }


    }
}
