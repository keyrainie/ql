using System.ServiceModel.Web;
using ECCentral.Service.MKT.AppService;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        /// <summary>
        /// 查询销售规则
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RepeatPromotion/GetSaleRules", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetSaleRules(RepeatPromotionQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IRepeatPromotionQueryDA>.Instance.GetSaleRules(msg, out totalCount);
            return new QueryResult
                       {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询赠品
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RepeatPromotion/GetGifts", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetGifts(RepeatPromotionQueryFilter msg)
        {
            int totalCount;
            SetCondtion(msg);
            var ds = ObjectFactory<IRepeatPromotionQueryDA>.Instance.GetGifts(msg, out totalCount);
            return new QueryResult
            {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询优惠券
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RepeatPromotion/GetCoupons", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetCoupons(RepeatPromotionQueryFilter msg)
        {
            int totalCount;
            SetCondtion(msg);
            var ds = ObjectFactory<IRepeatPromotionQueryDA>.Instance.GetCoupons(msg, out totalCount);
            return new QueryResult
            {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询限时抢购
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RepeatPromotion/GetSaleCountDowns", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetSaleCountDowns(RepeatPromotionQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IRepeatPromotionQueryDA>.Instance.GetSaleCountDowns(msg, out totalCount);
            return new QueryResult
            {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询销售规则
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RepeatPromotion/GeSaleCountDownPlan", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GeSaleCountDownPlan(RepeatPromotionQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IRepeatPromotionQueryDA>.Instance.GeSaleCountDownPlan(msg, out totalCount);
            return new QueryResult
            {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        ///  查询团购
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/RepeatPromotion/GetProductGroupBuying", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [DataTableSerializeOperationBehavior]
        public virtual QueryResult GetProductGroupBuying(RepeatPromotionQueryFilter msg)
        {
            int totalCount;
            var ds = ObjectFactory<IRepeatPromotionQueryDA>.Instance.GetProductGroupBuying(msg, out totalCount);
            return new QueryResult
            {
                Data = ds,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 设置条件
        /// </summary>
        /// <param name="msg"></param>
        private void SetCondtion(RepeatPromotionQueryFilter msg)
        {
            var process = ObjectFactory<RepeatPromotionAppService>.Instance;
            var product = process.GetProductInfo(msg.ProductId);
            if(product!=null)
            {
                msg.BrandSysNo = product.ProductBasicInfo.ProductBrandInfo.SysNo??0;
                msg.C3SysNo = product.ProductBasicInfo.ProductCategoryInfo.SysNo ?? 0;
                if(msg.ProductSysNo<=0)
                {
                    msg.ProductSysNo = product.SysNo;
                }
            }
        }

    }
}
