
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Category
{
    public class CategroyKPIDetailVM : ModelBase
    {
        /// <summary>
        /// 三级分类编号
        /// </summary>
        public int CategorySysNo { get; set; }

        /// <summary>
        /// 基本指标
        /// </summary>
        public CategoryKPIBasicInfoVM CategoryBasicInfo { get; set; }

        /// <summary>
        /// 毛利下限
        /// </summary>
        public CategoryKPIMinMarginVM CategoryMinMarginInfo { get; set; }

        /// <summary>
        /// RMA信息
        /// </summary>
        public CategoryKPIRMAInfoVM CategoryRMAInfo { get; set; }

        /// <summary>
        /// 一级类名称
        /// </summary>
        public string C1Name { get; set; }

        /// <summary>
        /// 二级类名称
        /// </summary>
        public string C2Name { get; set; }

        /// <summary>
        /// 三级类名称
        /// </summary>
        public string C3Name { get; set; }
        
        public int? Category2SysNo { get; set; }
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        public CategoryStatus Status { get; set; }
    }
}
