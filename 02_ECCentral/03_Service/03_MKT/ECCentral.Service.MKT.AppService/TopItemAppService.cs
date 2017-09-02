using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.BizProcessor;
using System.Transactions;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(TopItemAppService))]
    public class TopItemAppService
    {
        public void SetTopItem(TopItemInfo entity)
        {
            ObjectFactory<TopItemProcessor>.Instance.SetTopItem(entity);
        }

        public void CancleTopItem(List<TopItemInfo> list)
        {
            ObjectFactory<TopItemProcessor>.Instance.CancleTopItem(list);

        }

        public void TopItemConfigUpdate(TopItemConfigInfo entity)
        {
            ObjectFactory<TopItemProcessor>.Instance.TopItemConfigUpdate(entity);
        }

        public TopItemConfigInfo LoadTopItemConfig(int PageType, int RefSysNo)
        {
            return ObjectFactory<TopItemProcessor>.Instance.LoadTopItemConfig(PageType, RefSysNo);
        }
    }
}
