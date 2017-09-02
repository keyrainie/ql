using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductRelatedQueryVM : ModelBase
    {
  

        /// <summary>
        /// 主商品ID
        /// </summary>
        public string ProductSysNo { get; set; }

        /// <summary>
        /// 相关商品ID
        /// </summary>
        public string RelatedProductSysNo { get; set; }

        /// <summary>
        /// PMID
        /// </summary>
        public int? PMUserSysNo { get; set; }

        public bool HasRelativeProductBatchMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Product_RelativeProductBatchMaintain); }
        }
    }
}
