using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IBannerDA
    {
        void CreateBannerInfo(BannerInfo entity);

        void UpdateBannerInfo(BannerInfo entity);

        void UpdateBannerInfoStatus(int bannerInfoSysNo, ADStatus status);

        BannerInfo LoadBannerInfo(int sysNo);

        void CreateBannerLocation(BannerLocation loc);
        void UpdateBannerLocation(BannerLocation loc);
        BannerLocation LoadBannerLocation(int sysNo);

        void UpdateBannerLocationStatus(int bannerLocationSysNo, ADStatus status);

        /// <summary>
        /// 检查页面上的Banner位上已有的有效Banner数量
        /// </summary>
        int CountBannerPosition(int bannerDimensionSysNo, int pageID, string companyCode, string channelID);
    }
}
