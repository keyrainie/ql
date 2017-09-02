using System;
using System.Collections.Generic;
using ECommerce.Entity.Category;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity;
using ECommerce.Enums;
using ECommerce.Entity.Recommend;

namespace ECommerce.DataAccess.Recommend
{
    public class RecommendDA
    {
        /// <summary>
        /// 获得推荐位的商品
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="pageType"></param>
        /// <param name="posID"></param>
        /// <param name="count"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryRecommendProduct(int pageID,
            int pageType,
            int posID,
            int count,
            string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryRecommendProduct");
            cmd.SetParameterValue("@PageID", pageID);
            cmd.SetParameterValue("@PageType", pageType);
            cmd.SetParameterValue("@PositionID", posID);
            cmd.SetParameterValue("@Count", count);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        /// <summary>
        /// 获得推荐位的新上架商品
        /// </summary>
        /// <param name="count"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryNewRecommendProduct(
            int count,
            string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryNewRecommendProduct");
            cmd.SetParameterValue("@Count", count);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

       
        /// <summary>
        /// 一级类别新品上架(推荐位补位)
        /// </summary>
        /// <param name="c1SysNo">前台一级类别的sysno</param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<RecommendProduct> QueryNewProductForC1(int c1SysNo, string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryNewProductForC1");
            cmd.SetParameterValue("@ECCateSysNo", c1SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        public static List<RecommendProduct> QuerySuperSpecialProductForC1(int c1SysNo, string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QuerySuperSpecialProductForC1");
            cmd.SetParameterValue("@ECCateSysNo", c1SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }
      
        public static List<RecommendProduct> QueryHotProductForC1(int c1SysNo, string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryHotProductForC1");
            cmd.SetParameterValue("@ECCateSysNo", c1SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        public static List<RecommendProduct> QuerySuperSpecialProductForC2(int c2SysNo, string languageCode,
           string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QuerySuperSpecialProductForC2");
            cmd.SetParameterValue("@ECCateSysNo", c2SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        public static List<RecommendProduct> QuerySuperSpecialProductForC3(int c3SysNo, string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QuerySuperSpecialProductForC3");
            cmd.SetParameterValue("@ECCateSysNo", c3SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        public static List<RecommendProduct> QueryHotProductForC2(int c2SysNo, string languageCode,
           string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryHotProductForC2");
            cmd.SetParameterValue("@ECCateSysNo", c2SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }


        public static List<RecommendProduct> QueryHotProductForC3(int c3SysNo, string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryHotProductForC3");
            cmd.SetParameterValue("@ECC3SysNo", c3SysNo);
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        public static List<RecommendProduct> QueryNewProduct(string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryNewProduct");
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        public static List<RecommendProduct> QueryWeeklyHotProduct(string languageCode,
            string companyCode)
        {
            var cmd = DataCommandManager.GetDataCommand("QueryWeeklyHotProduct");
            cmd.SetParameterValue("@LanguageCode", languageCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<RecommendProduct>();
        }

        /// <summary>
        /// 获取广告信息
        /// </summary>
        /// <param name="pageID">页面ID</param>
        /// <param name="pageType">页面类型</param>
        /// <param name="positionID">广告位ID</param>
        /// <returns>广告信息列表</returns>
        public static List<BannerInfo> GetBannerInfoByPositionID(int pageID, PageType pageType, BannerPosition? positionID)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetBannerInfoByPositionID");
            dataCommand.SetParameterValue("@PageID", pageID);
            dataCommand.SetParameterValue("@PageType", pageType);
            dataCommand.SetParameterValue("@PositionID", positionID);

            List<BannerInfo> entitys = dataCommand.ExecuteEntityList<BannerInfo>();
            return entitys;
        }



        #region 首页楼层添加

        /// <summary>
        /// 获取楼层的基本信息
        /// </summary>
        /// <returns></returns>
        public static List<FloorEntity> GetFloorInfo(PageCodeType pageType, int pageCode)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetFloorInfo");
            dataCommand.SetParameterValue("@PageCode", pageCode);
            dataCommand.SetParameterValue("@PageType", pageType);
            List<FloorEntity> floorEntitys = dataCommand.ExecuteEntityList<FloorEntity>();
            return floorEntitys;
        }

        /// <summary>
        /// 获取楼层的Tab信息
        /// </summary>
        /// <param name="floorSysNo">楼层系统编号</param>
        /// <returns></returns>
        public static List<FloorSectionEntity> GetFloorSections(int floorSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetFloorSections");
            dataCommand.SetParameterValue("@SysNo", floorSysNo);
            List<FloorSectionEntity> entitys = dataCommand.ExecuteEntityList<FloorSectionEntity>();
            return entitys;
        }

        /// <summary>
        /// 获取楼层的Tab信息
        /// </summary>
        /// <param name="floorSysNo">楼层系统编号</param>
        /// <returns></returns>
        public static List<FloorItemBase> GetFloorSectionItems(int floorSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Content_GetFloorSectionItems");
            dataCommand.SetParameterValue("@SysNo", floorSysNo);
            List<FloorItemBase> entitys = dataCommand.ExecuteEntityList<FloorItemBase>();
            return entitys;
        }


        #endregion

        /// <summary>
        /// 获取推荐的品牌信息
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="pageID">页面ID,比如首页约定为0，商品分类(PageType.TabStore,PageType.MidCategory,PageType.SubStore)是分类的编号等</param>
        /// <param name="positionID">广告位ID</param>
        /// <returns>品牌信息列表</returns>
        public static List<RecommendBrandInfo> GetRecommendBrands(PageType pageType, int pageID)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Recommend_QueryRecommendBrand");
            dataCommand.SetParameterValue("@PageType", pageType);
            dataCommand.SetParameterValue("@PageID", pageID);

            List<RecommendBrandInfo> entitys = dataCommand.ExecuteEntityList<RecommendBrandInfo>();
            return entitys;
        }
    }
}
