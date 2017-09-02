using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IProductRecommendDA
    {
        void Create(ProductRecommendInfo entity);

        void Update(ProductRecommendInfo entity);

        ProductRecommendInfo Load(int sysNo);

        void Deactive(int sysNo);

         /// <summary>
        /// 判断是否存在相同的pagetype,pageid,positionid
        /// </summary>
        bool ExitsOnlineListPosition(int pageType, int pageId, int positionID, string companyCode, string channelID);

        void CreateLocation(ProductRecommendLocation location);

        void UpdateLocationDesc(int locationSysNo, string desc);

        ProductRecommendLocation LoadLocation(ProductRecommendLocation location);

        int ExitsSameDescription(ProductRecommendLocation location);

        /// <summary>
        /// 获取页面的位置信息
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="pageID">页面编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        List<ProductRecommendLocation> GetProductRecommendLocationList(int pageType,int pageID, string companyCode,string channelID);

        /// <summary>
        /// 获取页面上具体某一位置的商品推荐信息
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="pageID">页面编号</param>
        /// <param name="positionID">位置编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        List<ProductRecommendInfo> GetProductRecommendByPosition(int pageType, int pageID, int positionID, string productID, string companyCode, string channelID);
    }
}
