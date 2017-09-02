using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(HomePageSectionAppService))]
    public class HomePageSectionAppService
    {
        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create(HomePageSectionInfo item)
        {
            ValidateEntity(item);

            ObjectFactory<HomePageSectionProcessor>.Instance.Create(item);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update(HomePageSectionInfo item)
        {
            ValidateEntity(item);
            ObjectFactory<HomePageSectionProcessor>.Instance.Update(item);
        }

        /// <summary>
        /// 加载
        /// </summary>
        public virtual HomePageSectionInfo Load(int sysNo)
        {
            return ObjectFactory<HomePageSectionProcessor>.Instance.Load(sysNo);
        }

        private void ValidateEntity(HomePageSectionInfo entity)
        {
            if (ObjectFactory<HomePageSectionProcessor>.Instance.CheckNameExists(entity.SysNo, entity.DomainName, entity.CompanyCode, entity.WebChannel.ChannelID))
            {
                throw new BizException(ResouceManager.GetMessageString("MKT.HomePageSection", "HomePageSection_ExistsSameName"));
            }
        }

    }
}
