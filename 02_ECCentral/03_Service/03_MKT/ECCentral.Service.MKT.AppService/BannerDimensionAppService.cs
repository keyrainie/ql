using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(BannerDimensionAppService))]
    public class BannerDimensionAppService
    {
        private BannerDimensionProcessor _bannerDimensionProcessor = ObjectFactory<BannerDimensionProcessor>.Instance;
         /// <summary>
        /// 创建广告尺寸
        /// </summary>
        /// <param name="bannerDimension">广告尺寸</param>
        public virtual void Create(BannerDimension bannerDimension)
        {
            _bannerDimensionProcessor.Create(bannerDimension);
        }

         /// <summary>
        /// 更新广告信息
        /// </summary>
        /// <param name="bannerDimension">广告尺寸</param>
        public virtual void Update(BannerDimension bannerDimension)
        {
            _bannerDimensionProcessor.Update(bannerDimension);
        }

        /// <summary>
        /// 加载广告尺寸
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns></returns>
        public virtual BannerDimension Load(int sysNo)
        {
            return _bannerDimensionProcessor.Load(sysNo);
        }
    }
}
