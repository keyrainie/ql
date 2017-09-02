using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.AppService
{
    [VersionExport(typeof(DefaultRMAPolicyService))]
    public class DefaultRMAPolicyService
    {
        #region private
        private DefaultRMAPolicyProcessor Biz =
            ObjectFactory<DefaultRMAPolicyProcessor>.Instance;
        #endregion

        #region Method
        public void DefaultRMAPolicyInfoAdd(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            Biz.DefaultRMAPolicyInfoAdd(defaultRMAPolicy);
        }
        public void UpdateDefaultRMAPolicyBySysNo(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            Biz.UpdateDefaultRMAPolicyBySysNo(defaultRMAPolicy);
        }
        public void DelDelDefaultRMAPolicyBySysNoBySysNos(List<DefaultRMAPolicyInfo> defaultRMAPolicyInfos)
        {
            Biz.DelDelDefaultRMAPolicyBySysNoBySysNos(defaultRMAPolicyInfos);
        }
        #endregion
    }
}
