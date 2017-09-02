using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Models.Product
{
    public class ProductERPCategoryQueryVM : ModelBase
    {
        /// <summary>
        /// 大类码Code
        /// </summary>
        public string SPCode { get; set; }

        /// <summary>
        /// 大类码名称
        /// </summary>
        public string SPName { get; set; }
    }
}
