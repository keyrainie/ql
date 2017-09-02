using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.Basic
{
    public partial class ConstValue
    {
        #region MKT Domain

        /// <summary>
        /// DIY自助装机
        /// </summary>
        public const string MKT_ComputerConfigMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/ComputerConfigMaintain/{0}";
        public const string MKT_PromotionChannelMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/PromotionChannelMaintain/{0}";

        /// <summary>
        /// 限购规则维护页面
        /// </summary>
        public const string MKT_BuyLimitRuleMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/BuyLimitRuleMaintain/{0}";

        /// <summary>
        /// 限购规则查询页面
        /// </summary>
        public const string MKT_BuyLimitRuleQuery = "/ECCentral.Portal.UI.MKT/BuyLimitRuleQuery";

        /// <summary>
        /// 销售立减规则维护页面
        /// </summary>
        public const string MKT_SaleDiscountRuleMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/SaleDiscountRuleMaintain/{0}";

        /// <summary>
        /// 销售立减规则查询页面
        /// </summary>
        public const string MKT_SaleDiscountRuleQuery = "/ECCentral.Portal.UI.MKT/SaleDiscountRuleQuery";
        #endregion MKT Domain
    }
}
