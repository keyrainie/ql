using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IProductRecommendDA))]
    public class ProductRecommendDA : IProductRecommendDA
    {

        #region IProductRecommendDA Members

        public void Create(ProductRecommendInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_Insert");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        public void Update(ProductRecommendInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_Update");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        public ProductRecommendInfo Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_Load");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<ProductRecommendInfo>();
        }

        public void Deactive(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_Deactive");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");
            cmd.ExecuteNonQuery();
        }

        public void CreateLocation(ProductRecommendLocation location)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_InsertLocation");
            cmd.SetParameterValue(location);
            cmd.ExecuteNonQuery();
            location.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
        }

        public void UpdateLocationDesc(int locationSysNo, string desc)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_UpdateLocationDesc");
            cmd.SetParameterValue("@LocationSysNo", locationSysNo);
            cmd.SetParameterValue("@Description", desc);
            cmd.ExecuteNonQuery();
        }

        public ProductRecommendLocation LoadLocation(ProductRecommendLocation location)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_GetProductRecommendLocation");
            cmd.SetParameterValue("@PageType", location.PageType);
            cmd.SetParameterValue("@PageID", location.PageID);
            cmd.SetParameterValue("@PositionID", location.PositionID);
            cmd.SetParameterValue("@CompanyCode", location.CompanyCode);
            //TODO：添加多渠道条件设置

            return cmd.ExecuteEntity<ProductRecommendLocation>();
        }

        public int ExitsSameDescription(ProductRecommendLocation location)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ProductRecommend_ExitsSameDescription");
            cmd.SetParameterValue("@PageType", location.PageType);
            cmd.SetParameterValue("@PageID", location.PageID);
            cmd.SetParameterValue("@PositionID", location.PositionID);
            cmd.SetParameterValue("@Description", location.Description);
            cmd.SetParameterValue("@CompanyCode", location.CompanyCode);
            //TODO：添加多渠道条件设置

            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 判断是否存在相同的pagetype,pageid,positionid
        /// </summary>
        public bool ExitsOnlineListPosition(int pageType, int pageId, int positionID, string companyCode, string channelID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("OnlineList_ExitsOnlineListPosition");
            command.SetParameterValue("@PageType", pageType);
            command.SetParameterValue("@PageID", pageId);
            command.SetParameterValue("@PositionID", positionID);
            command.SetParameterValue("@CompanyCode", companyCode);
            //TODO：添加多渠道条件设置

            return command.ExecuteScalar<int>() > 0 ? true : false;
        }

        /// <summary>
        /// 获取页面的位置信息
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="pageID">页面编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        public List<ProductRecommendLocation> GetProductRecommendLocationList(int pageType,int pageID, string companyCode,string channelID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("OnlineList_GetPagePositionList");
            command.SetParameterValue("@PageType", pageType);
            command.SetParameterValue("@PageID", pageID);
            command.SetParameterValue("@CompanyCode", companyCode);
            //TODO：添加多渠道条件设置

            return command.ExecuteEntityList<ProductRecommendLocation>();
        }

        /// <summary>
        /// 获取页面上具体某一位置的商品推荐信息
        /// </summary>
        /// <param name="pageType">页面类型</param>
        /// <param name="pageID">页面编号</param>
        /// <param name="positionID">位置编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        public List<ProductRecommendInfo> GetProductRecommendByPosition(int pageType, int pageID, int positionID, string productID, string companyCode, string channelID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ProductRecommend_CheckExists");
            command.SetParameterValue("@PageType", pageType);
            command.SetParameterValue("@PageID", pageID);
            command.SetParameterValue("@PositionID", positionID);
            command.SetParameterValue("@ProductID", productID);
            command.SetParameterValue("@CompanyCode", companyCode);
            //TODO：添加多渠道条件设置

            return command.ExecuteEntityList<ProductRecommendInfo>();
        }

        #endregion
    }
}
