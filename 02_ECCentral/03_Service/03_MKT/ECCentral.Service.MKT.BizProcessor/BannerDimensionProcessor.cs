using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using System.Transactions;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(BannerDimensionProcessor))]
    public class BannerDimensionProcessor
    {
        private IBannerDimensionDA _bannerDimensionDA = ObjectFactory<IBannerDimensionDA>.Instance;
        /// <summary>
        /// 创建广告尺寸
        /// </summary>
        /// <param name="bannerDimension">广告尺寸</param>
        public virtual void Create(BannerDimension bannerDimension)
        {
            Validate(bannerDimension);
            _bannerDimensionDA.CreateBannerDimension(bannerDimension);
        }

        /// <summary>
        /// 更新广告尺寸
        /// </summary>
        /// <param name="bannerDimension">广告尺寸</param>
        public virtual void Update(BannerDimension bannerDimension)
        {
            Validate(bannerDimension);
            _bannerDimensionDA.UpdateBannerDimension(bannerDimension);
        }

        /// <summary>
        /// 加载广告尺寸
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns></returns>
        public virtual BannerDimension Load(int sysNo)
        {
            return _bannerDimensionDA.LoadBannerDimension(sysNo);
        }

        private void Validate(BannerDimension bannerDimension)
        {
            int excludeSysNo = bannerDimension.SysNo.HasValue ? bannerDimension.SysNo.Value : 0;

            //检查同一页面类型下PositionID不能重复
            int positionIDDuplicateCount = _bannerDimensionDA.CheckPositionIDCount(excludeSysNo, bannerDimension.PageTypeID ?? 0, bannerDimension.PositionID ?? 0, bannerDimension.CompanyCode, bannerDimension.WebChannel.ChannelID);
            if (positionIDDuplicateCount > 0)
                //throw new BizException("此页面类型下已存在此位置编号！");
                throw new BizException(ResouceManager.GetMessageString("MKT.BannerDimension","BannerDimension_ExistsSameID"));

            //检查同一页面类型下PositionID不能重复
            int positionNameDuplicateCount = _bannerDimensionDA.CheckPositionNameCount(excludeSysNo, bannerDimension.PageTypeID ?? 0, bannerDimension.PositionName, bannerDimension.CompanyCode, bannerDimension.WebChannel.ChannelID);
            if (positionNameDuplicateCount > 0)
                throw new BizException(ResouceManager.GetMessageString("MKT.BannerDimension","BannerDimension_ExistsSameID"));

            BannerDimension currentDimension = _bannerDimensionDA.GetBannerDimensionByPositionID(bannerDimension.SysNo ?? 0, bannerDimension.PositionID ?? 0, bannerDimension.CompanyCode);
            if (currentDimension != null && !bannerDimension.PositionName.Trim().ToUpper().Equals(currentDimension.PositionName.Trim().ToUpper()))
                //throw new BizException("相同的位置ID位置名称必须相同！");
                throw new BizException(ResouceManager.GetMessageString("MKT.BannerDimension", "BannerDimension_SamePositionSameName"));
        }
    }
}
