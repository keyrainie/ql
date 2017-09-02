using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(BannerAppService))]
    public class BannerAppService
    {
        private BannerProcessor _bannerProcessor = ObjectFactory<BannerProcessor>.Instance;
         /// <summary>
        /// 创建广告信息
        /// </summary>
        /// <param name="bannerLocation">广告信息</param>
        public virtual void Create(BannerLocation bannerLocation)
        {
            _bannerProcessor.Create(bannerLocation);
        }

         /// <summary>
        /// 更新广告信息
        /// </summary>
        /// <param name="bannerLocation">广告信息</param>
        public virtual void Update(BannerLocation bannerLocation)
        {
            _bannerProcessor.Update(bannerLocation);
        }

         /// <summary>
        /// 作废bannerInfo
        /// </summary>
        /// <param name="bannerLocationSysNo">系统编号</param>
        public virtual void Delete(int bannerLocationSysNo)
        {
            _bannerProcessor.Delete(bannerLocationSysNo);
        }

        public virtual BannerLocation Load(int bannerLocationSysNo)
        {
            return _bannerProcessor.Load(bannerLocationSysNo);
        }

          /// <summary>
        /// 检查页面上的Banner位上已有的有效Banner数量
        /// </summary>
        public virtual int CountBannerPosition(int bannerDimensionSysNo, int pageID, string companyCode, string channelID)
        {
            return _bannerProcessor.CountBannerPosition(bannerDimensionSysNo, pageID, companyCode, channelID);
        }
    }
}
