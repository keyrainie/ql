using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    /// <summary>
    /// 首页楼层信息
    /// </summary>
    [Serializable]
    public class FloorEntity
    {
        /// <summary>
        /// 获取或设置楼层编号
        /// </summary>
        public int FloorSysNo { get; set; }

        /// <summary>
        /// 获取或设置楼层图片
        /// </summary>
        public string FloorName { get; set; }

        /// <summary>
        /// 获取或设置楼层图片
        /// </summary>
        public string FloorLogoSrc { get; set; }

        /// <summary>
        /// 获取或设置优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 获取或设置Remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 获取或设置模板编号
        /// </summary>
        public int TemplateSysNo { get; set; }

        /// <summary>
        /// 获取或设置模板对应的视图
        /// </summary>
        public string PartialView { get; set; }

        /// <summary>
        /// 获取或设置状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 楼层 Tab 信息
        /// </summary>
        public List<FloorSectionEntity> FloorSections { get; set; }

        /// <summary>
        /// 楼层对应的 Item 信息
        /// </summary>
        public List<FloorItemBase> FloorSectionItems { get; set; }
    }
}
