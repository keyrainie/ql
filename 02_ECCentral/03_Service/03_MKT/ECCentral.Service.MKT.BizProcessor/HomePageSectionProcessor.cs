using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using System.Transactions;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(HomePageSectionProcessor))]
    public class HomePageSectionProcessor
    {
        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create(HomePageSectionInfo item)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                //创建首页区域
                ObjectFactory<IHomePageSectionDA>.Instance.Create(item);
                //默认为首页区域添加一个广告位
                CreateBannerDimension(item.SysNo.Value, item.DomainName, item.CompanyCode, "");

                ts.Complete();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update(HomePageSectionInfo item)
        {
            ObjectFactory<IHomePageSectionDA>.Instance.Update(item);
        }

        /// <summary>
        /// 加载
        /// </summary>
        public virtual HomePageSectionInfo Load(int sysNo)
        {
            return ObjectFactory<IHomePageSectionDA>.Instance.Load(sysNo);
        }

        public virtual bool CheckNameExists(int? excludeSysNo, string domainName, string companyCode, string channelID)
        {
            return ObjectFactory<IHomePageSectionDA>.Instance.CheckNameExists(excludeSysNo, domainName, companyCode, channelID);
        }

        private void CreateBannerDimension(int sectionSysNo, string sectionName, string companyCode, string channelID)
        {
            BannerDimension sectionBannerDimension = new BannerDimension();
            sectionBannerDimension.PageTypeID = 0;
            sectionBannerDimension.PositionID = int.Parse("10" + sectionSysNo.ToString());
            sectionBannerDimension.PositionName = sectionName;
            sectionBannerDimension.Width = 207;
            sectionBannerDimension.Height = 248;
            sectionBannerDimension.CompanyCode = companyCode;
            sectionBannerDimension.WebChannel = new BizEntity.Common.WebChannel
            {
                ChannelID = channelID
            };

            ObjectFactory<IBannerDimensionDA>.Instance.CreateBannerDimension(sectionBannerDimension);
        }
    }
}
