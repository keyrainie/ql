using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    public class StoreDetailModel
    {
        /// <summary>
        /// 企业(商家)基本信息
        /// </summary>
        public StoreBasicInfoModel StoreBasicInfo { get; set; }

        /// <summary>
        /// 头部信息
        /// </summary>
        public string HeaderInfo { get; set; }

        /// <summary>
        /// 导航
        /// </summary>
        public List<StoreNavigationModel> StoreNavigationInfo { get; set; }

        /// <summary>
        /// 新品
        /// </summary>
        public List<StoreNewProductRecommendModel> StoreNewProductRecommendInfo { get; set; }

        /// <summary>
        /// 一周排行
        /// </summary>
        public List<StoreWeekRankingProductModel> StoreWeekRankingProductInfo { get; set; }
    }
}