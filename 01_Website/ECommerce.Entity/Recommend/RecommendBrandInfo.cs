using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Recommend
{
    /// <summary>
    /// 推荐品牌信息
    /// </summary>
    public class RecommendBrandInfo
    {
        /// <summary>
        /// 页面类型
        /// </summary>
        public int Level_No { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int Level_Code { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Level_Name { get; set; }

        /// <summary>
        /// 品牌系统编号
        /// </summary>
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 品牌图片
        /// </summary>
        public string NeweggUrl { get; set; }

        /// <summary>
        /// 品牌LOGO
        /// </summary>
        public string ADImage { get; set; }
        /// <summary>
        /// Gets or sets the brand name_ en.
        /// </summary>
        /// <value>
        /// The brand name_ en.
        /// </value>
        public string BrandName_En { get; set; }

        /// <summary>
        /// Gets or sets the brand name_ ch.
        /// </summary>
        /// <value>
        /// The brand name_ ch.
        /// </value>
        public string BrandName_Ch { get; set; }
    }
}
