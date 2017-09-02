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
    [VersionExport(typeof(IBannerDA))]
    public class BannerDA : IBannerDA
    {
        #region IBannerDA Members

        public void CreateBannerInfo(BannerInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Banner_InsertBannerInfo");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
        }

        public void UpdateBannerInfo(BannerInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Banner_UpdateBannerInfo");

            dc.SetParameterValue(entity);

            dc.ExecuteNonQuery();
        }

        public void UpdateBannerInfoStatus(int bannerInfoSysNo, ADStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Banner_UpdateBannerInfoStatus");
            cmd.SetParameterValue("@SysNo", bannerInfoSysNo);
            cmd.SetParameterValue("@Status", status);

            cmd.ExecuteNonQuery();
        }

        public BannerInfo LoadBannerInfo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Banner_LoadBannerInfo");
            cmd.SetParameterValue("@BannerInfoSysNo", sysNo);

            return cmd.ExecuteEntity<BannerInfo>();
        }

        public void CreateBannerLocation(BannerLocation loc)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Banner_InsertBannerLocation");

            dc.SetParameterValue(loc);

            dc.ExecuteNonQuery();

            loc.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
        }

        public void UpdateBannerLocation(BannerLocation loc)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Banner_UpdateBannerLocation");

            dc.SetParameterValue(loc);

            dc.ExecuteNonQuery();
        }

        public BannerLocation LoadBannerLocation(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Banner_LoadBannerLocation");
            cmd.SetParameterValue("@BannerLocationSysNo", sysNo);

            return cmd.ExecuteEntity<BannerLocation>();
        }

        public void UpdateBannerLocationStatus(int bannerLocationSysNo,ADStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Banner_UpdateBannerLocationStatus");
            cmd.SetParameterValue("@SysNo", bannerLocationSysNo);
            cmd.SetParameterValue("@Status", status);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 检查页面上的Banner位上已有的有效Banner数量
        /// </summary>
        public int CountBannerPosition(int bannerDimensionSysNo, int pageID, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Banner_CountBannerPosition");
            cmd.SetParameterValue("@BannerDimensionSysNo", bannerDimensionSysNo);
            cmd.SetParameterValue("@PageID", pageID);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加渠道参数

            return cmd.ExecuteScalar<int>();
        }
        #endregion
    }
}
