//************************************************************************
// 用户名				泰隆优选
// 系统名				商品价格变动申请单据
// 子系统名		        商品价格变动申请单据Restful实现
// 作成者				Tom
// 改版日				2012.4.23
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM.Product.Request;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.IM.AppService;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.IM.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.IM.Restful
{
    public partial class IMService
    {
        #region 查询

        /// <summary>
        /// 查询商品价格变动申请单据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/QueryProductPriceRequesList", Method = "POST")]
        //[DataTableSerializeOperationBehavior]
        public QueryResult QueryProductPriceRequesList(ProductPriceRequestQueryFilter request)
        {
            if (request == null)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPriceRequest", "ProductPriceRequestCondtionIsNull"));
            }
            int totalCount;
            var data = ObjectFactory<IProductPriceRequestQueryDA>.Instance.QueryProductPriceRequesList(request, out totalCount);
            ObjectFactory<ProductPriceRequestAppService>.Instance.AddOtherData(data);
            var source = new QueryResult { Data = data, TotalCount = totalCount };
            return source;
        }

        /// <summary>
        /// 根据SysNO获取商品价格变动单据
        /// </summary>
        /// <param name="auditProductPriceSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetProductPriceRequestInfoBySysNo", Method = "POST")]
        //[DataTableSerializeOperationBehavior]
        public ProductPriceRequestInfo GetProductPriceRequestInfoBySysNo(int auditProductPriceSysNo)
        {
            var result = ObjectFactory<ProductPriceRequestAppService>.Instance.GetProductPriceRequestInfoBySysNo(auditProductPriceSysNo);
            return result;
        }

        /// <summary>
        /// 根据SysNO获取商品价格变动单据
        /// </summary>
        /// <param name="auditProductPriceSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/GetNeweggProductPriceRequestInfoBySysNo", Method = "POST")]
        //[DataTableSerializeOperationBehavior]
        public ProductPriceRequestMsg GetNeweggProductPriceRequestInfoBySysNo(int auditProductPriceSysNo)
        {
            var result = ObjectFactory<ProductPriceRequestAppService>.Instance.GetProductPriceRequestInfoBySysNo(auditProductPriceSysNo);
            if (result != null)
            {
                var responseMsg = new ProductPriceRequestMsg
                                      {
                                          PriceRequestMsg = result,
                                          MinMarginAmount =
                                              Convert.ToDecimal(AppSettingManager.GetSetting("IM", "IM_MinMarginAmount"))
                                      };
                int productSysNo = 0;
                var discountResult =
                    ObjectFactory<ProductPriceRequestAppService>.Instance.GetProductPromotionDiscountInfoList(
                        auditProductPriceSysNo, ref productSysNo);
                if (discountResult != null && discountResult.Count > 0)
                {
                    string returnMsgStr = string.Empty;
                    var resultMsg = ObjectFactory<IIMBizInteract>.Instance.GetProductPromotionMargin(result,
                                                                    productSysNo, "", 0m, ref returnMsgStr);
                    if (resultMsg != null && resultMsg.Count > 0)
                    {
                        var promotionMsgs = new List<ProductPromotionMsg>();
                        var i = 0;
                        resultMsg.ForEach(v =>
                        {
                            var msg = new ProductPromotionMsg
                            {
                                Discount = discountResult[i].Discount,
                                Margin = v.Margin,
                                PromotionType = v.PromotionType,
                                ReferenceSysNo = v.ReferenceSysNo
                            };
                            i++;
                            promotionMsgs.Add(msg);
                        });
                        responseMsg.PromotionMsg = promotionMsgs;
                    }

                }
                return responseMsg;
            }
            return null;
        }
        #endregion

        #region 审核
        /// <summary>
        /// 审核商品价格变动单据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Product/AuditProductPriceRequest", Method = "PUT")]
        public virtual void AuditProductPriceRequest(List<ProductPriceRequestInfo> entity)
        {
            ObjectFactory<ProductPriceRequestAppService>.Instance.AuditProductPriceRequest(entity);
        }
        #endregion
    }
}
