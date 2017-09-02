
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductExtVM : ModelBase
    {

        public int SysNo { get; set; }

        /// <summary>
        /// 是否可以退货
        /// </summary>
        public int IsPermitRefund { get; set; }
    }
}
