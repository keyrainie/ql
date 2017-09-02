using System;

using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class RepeatPromotionQueryFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");                
            }
        }

        public RepeatPromotionQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public RepeatPromotionQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询销售规则
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void GetSaleRules(RepeatPromotionQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            const string relativeUrl = "/MKTService/RepeatPromotion/GetSaleRules";
            GetPromotion(model, relativeUrl, callback);
        }

        /// <summary>
        /// 查询赠品
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void GetGifts(RepeatPromotionQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            const string relativeUrl = "/MKTService/RepeatPromotion/GetGifts";
            GetPromotion(model, relativeUrl, callback);
        }

        /// <summary>
        /// 查询优惠券
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void GetCoupons(RepeatPromotionQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
          const string relativeUrl = "/MKTService/RepeatPromotion/GetCoupons";
          GetPromotion(model, relativeUrl, callback);
        }

        /// <summary>
        /// 查询限时抢购
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void GetSaleCountDowns(RepeatPromotionQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            const string relativeUrl = "/MKTService/RepeatPromotion/GetSaleCountDowns";
            GetPromotion(model,relativeUrl, callback);
        }

        /// <summary>
        /// 查询销售规则
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void GeSaleCountDownPlan(RepeatPromotionQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            const string relativeUrl = "/MKTService/RepeatPromotion/GeSaleCountDownPlan";
            GetPromotion(model, relativeUrl, callback);
        }

        /// <summary>
        ///  查询团购
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void GetProductGroupBuying(RepeatPromotionQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            const string relativeUrl = "/MKTService/RepeatPromotion/GetProductGroupBuying";
            GetPromotion(model, relativeUrl, callback);
        }

        private void GetPromotion(RepeatPromotionQueryVM model, string relativeUrl, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var filter = model.ConvertVM<RepeatPromotionQueryVM, RepeatPromotionQueryFilter>();

            filter.PageInfo = model.PageInfo;
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                }
                );
        }
       
    }
}
