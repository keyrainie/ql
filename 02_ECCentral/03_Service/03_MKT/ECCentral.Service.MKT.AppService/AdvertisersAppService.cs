using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(AdvertisersAppService))]
    public class AdvertisersAppService
    {
        #region 广告商
        /// <summary>
        /// 创建广告商
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateAdvertisers(Advertisers item)
        {
            ObjectFactory<AdvertisersProcessor>.Instance.CreateAdvertisers(item);
        }

        /// <summary>
        /// 编辑广告商
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateAdvertisers(Advertisers item)
        {
            ObjectFactory<AdvertisersProcessor>.Instance.UpdateAdvertisers(item);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetAdvertiserValid(List<int> item)
        {
            ObjectFactory<AdvertisersProcessor>.Instance.SetAdvertiserValid(item);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetAdvertiserInvalid(List<int> item)
        {
            ObjectFactory<AdvertisersProcessor>.Instance.SetAdvertiserInvalid(item);
        }

        /// <summary>
        /// 加载广告商
        /// </summary>
        /// <returns></returns>
        public virtual Advertisers LoadAdvertiser(int sysNO)
        {
            return ObjectFactory<AdvertisersProcessor>.Instance.LoadAdvertiser(sysNO);
        }

        #endregion

    }
}
