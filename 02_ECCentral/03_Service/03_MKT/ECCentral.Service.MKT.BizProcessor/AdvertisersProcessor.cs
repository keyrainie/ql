using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess;
using System.Data;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(AdvertisersProcessor))]
    public class AdvertisersProcessor
    {
        private IAdvertisersDA advDA = ObjectFactory<IAdvertisersDA>.Instance;

        #region 广告商
        /// <summary>
        /// 创建广告商
        /// </summary>
        /// <param name="item"></param>
        public virtual void CreateAdvertisers(Advertisers item)
        {
            if (string.IsNullOrEmpty(item.AdvertiserName))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_AdvertiserNameISNotNull"));
            if (string.IsNullOrEmpty(item.MonitorCode))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_MonitorCodeISNotNull"));
            if (string.IsNullOrEmpty(item.AdvertiserUserName))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_AdvertiserUserNameISNotNull"));
            if (string.IsNullOrEmpty(item.AdvertiserPassword))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_AdvertiserPasswordISNotNull"));
            if (!item.CookiePeriod.HasValue)
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_CookiePeriodISNotNull"));

            if (advDA.CheckExistAdvertiserMonitorCode(item.CompanyCode, item.MonitorCode, null))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_ExsitMonitorCode"));
            advDA.CreateAdvertisers(item);
        }

        /// <summary>
        /// 是否存在该广告商
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //public virtual Advertisers LoadAdvertiser(int sysNo)
        //{
        //    return advDA.LoadAdvertiser(sysNo);
        //}
        
        /// <summary>
        /// 加载广告商
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual Advertisers LoadAdvertiser(int sysNo)
        {
            //Advertisers item = new Advertisers();
            //item.SysNo = sysNo;
            //if (advDA.CheckExistAdvertisers(item))
            //    throw new BizException("不存在该广告商对象！");
            return advDA.LoadAdvertiser(sysNo);
        }

        /// <summary>
        /// 编辑广告商
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateAdvertisers(Advertisers item)
        {
            if (!advDA.CheckExistAdvertisers(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_DonotExsitAdvertiserName"));
            //if (advDA.CheckExistAdvertiserMonitorCode(item.CompanyCode, item.MonitorCode, item.SysNo))
            //    throw new BizException("已经存在该广告监测代码！");
            advDA.UpdateAdvertisers(item);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetAdvertiserValid(List<int> items)
        {
            if (items.Count > 0)
                advDA.SetAdvertiserValid(items);
            else
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_OperatedDataISNull"));
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetAdvertiserInvalid(List<int> items)
        {
            if (items.Count > 0)
                advDA.SetAdvertiserInvalid(items);
            else
                throw new BizException(ResouceManager.GetMessageString("MKT.News", "Advertiser_OperatedDataISNull"));
        }

        #endregion

        #region 广告效果监视

        /// <summary>
        /// 导出订阅用户Email列表
        /// </summary>
        public virtual void ExportToExcel()
        {
            throw new BizException("");
        }
        #endregion
    }
}
