using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(AdvertisingProcessor))]
    public class AdvertisingProcessor
    {
        //private IAdvertisingDA advertisingDA = ObjectFactory<IAdvertisingDA>.Instance;

        public virtual AdvertisingInfo Load(int? sysNo)
        {
            AdvertisingInfo info = ObjectFactory<IAdvertisingDA>.Instance.Load(sysNo.Value);
            if (info == null)
            {
                throw new BizException("此广告不存在！");
            }
            return info;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        public AdvertisingInfo CreateAdvertising(AdvertisingInfo info)
        {
            List<AdvertisingInfo> entityList = ObjectFactory<IAdvertisingDA>.Instance.LoadByProductLineSysNoAndType(info);
            int cntTmp = 0;

            if (entityList != null)
            {
                if (info.Type == AdvertisingType.IMG)
                {
                    cntTmp = entityList.Where(ad => ad.ImageHeight == info.ImageHeight && ad.ImageWidth == info.ImageWidth).Count();
                    if (cntTmp > 0)
                    {
                        throw new BizException("该产品线下已存在同一尺寸的广告!");
                    }
                }
                if (info.Type == AdvertisingType.TEXT)
                {
                    cntTmp = entityList.Count();
                    if (cntTmp > 0)
                    {
                        throw new BizException("该产品线下已存在一个文字广告!");
                    }
                }
            }
            return ObjectFactory<IAdvertisingDA>.Instance.CreateAdvertising(info);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        public void UpdateAdvertising(AdvertisingInfo info)
        {
            List<AdvertisingInfo> entityList = ObjectFactory<IAdvertisingDA>.Instance.LoadByProductLineSysNoAndType(info);
            int cntTmp = 0;

            if (info.Status == ValidStatus.Active)
            {
                entityList = entityList.Where(ad => ad.SysNo != info.SysNo).ToList();
            }

            if (entityList != null)
            {
                if (info.Type == AdvertisingType.IMG)
                {
                    cntTmp = entityList.Where(ad => ad.ImageHeight == info.ImageHeight && ad.ImageWidth == info.ImageWidth).Count();
                    if (cntTmp > 0)
                    {
                        throw new BizException("该产品线下已存在同一尺寸的广告在使用!");
                    }
                }
                if (info.Type == AdvertisingType.TEXT)
                {
                    cntTmp = entityList.Count();
                    if (cntTmp > 0)
                    {
                        throw new BizException("该产品线下已存在一个文字广告在使用!");
                    }
                }
            }
            ObjectFactory<IAdvertisingDA>.Instance.UpdateAdvertising(info);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteAdvertising(int SysNo)
        {
            ObjectFactory<IAdvertisingDA>.Instance.DeleteAdvertising(SysNo);
        }
    }
}
