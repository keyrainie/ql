using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IBannerDimensionDA))]
    public class BannerDimensionDA : IBannerDimensionDA
    {

        #region IBannerDimensionDA Members

        public void CreateBannerDimension(BannerDimension entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("BannerDimension_InsertBannerDimension");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
        }

        public void UpdateBannerDimension(BannerDimension entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("BannerDimension_UpdateBannerDimension");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();
        }

        public BannerDimension LoadBannerDimension(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("BannerDimension_LoadBannerDimension");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<BannerDimension>();
        }

        public BannerDimension GetBannerDimensionByPositionID(int sysNo,int positionID, string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("BannerDimension_GetBannerDimensionByPositionID");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@PositionID", positionID);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntity<BannerDimension>();
        }

        /// <summary>
        /// 检查同一页面类型下PositionID相同的个数
        /// </summary>
        /// <param name="excludeSysNo">排除系统编号</param>
        /// <param name="pageType">页面类型编号</param>
        /// <param name="positionID">位置编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        public int CheckPositionIDCount(int excludeSysNo, int pageType, int positionID, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("BannerDimension_CheckPositionIDCount");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@PageType", pageType);
            cmd.SetParameterValue("@PositionID", positionID);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO：添加多渠道条件设置

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// 检查同一页面类型下PositionName相同的个数
        /// </summary>
        /// <param name="excludeSysNo">排除系统编号</param>
        /// <param name="pageType">页面类型编号</param>
        /// <param name="positionName">位置名称</param>
        /// <param name="companyCode">公司编号</param>
        /// <param name="channelID">渠道编号</param>
        /// <returns></returns>
        public int CheckPositionNameCount(int excludeSysNo, int pageType, string positionName, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("BannerDimension_CheckPositionNameCount");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@PageType", pageType);
            cmd.SetParameterValue("@PositionName", positionName);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO：添加多渠道条件设置

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        #endregion
    }
}
