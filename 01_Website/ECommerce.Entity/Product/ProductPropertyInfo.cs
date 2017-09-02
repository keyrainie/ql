using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProductPropertyInfo
    {
        /// <summary>
        /// 获取或设置商品SysNo
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 获取或设置商品 Code
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 获取或设置商品当前价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 获取或设置商品当前状态
        /// </summary>
        public ProductStatus ProStatus { get; set; }

        /// <summary>
        /// 获取或设置商品默认图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 获取或设置商品图片版本
        /// </summary>
        public string ImageVersion { get; set; }

        /// <summary>
        /// 获取或设置属性名称编号
        /// </summary>
        public int ParentPropertySysNo { get; set; }

        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string ParentPropertyName { get; set; }

        /// <summary>
        /// 获取或设置属性是否显示为图片
        /// </summary>
        public string ParentDisplayPic { get; set; }

        /// <summary>
        /// 获取或设置属性是否为聚集属性
        /// </summary>
        public string ParentIsPloymeric { get; set; }

        /// <summary>
        /// 获取或设置属性值编号
        /// </summary>
        public int ParentValueSysNo { get; set; }

        /// <summary>
        /// 获取或设置属性值
        /// </summary>
        public string ParentValue { get; set; }

        /// <summary>
        /// 获取或设置属性值排序
        /// </summary>
        public int ParentPriority { get; set; }

        private string parentDisplayPic;
        /// <summary>
        /// 获取是否显示为图片
        /// </summary>
        public bool IsParentDisplayPic
        {
            get
            {
                return this.parentDisplayPic == "Y";
            }
        }

        /// <summary>
        /// 获取或设置属性名称编号
        /// </summary>
        public int PropertySysNo { get; set; }
        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 获取或设置属性是否显示为图片
        /// </summary>
        public string DisplayPic { get; set; }

        /// <summary>
        /// 获取或设置属性是否为聚集属性
        /// </summary>
        public string IsPloymeric { get; set; }

        /// <summary>
        /// 获取或设置属性值编号
        /// </summary>
        public int ValueSysNo { get; set; }

        /// <summary>
        /// 获取或设置属性值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置属性值排序
        /// </summary>
        public int Priority { get; set; }

        private string displayPic;
        /// <summary>
        /// 获取是否显示为图片
        /// </summary>
        public bool IsDisplayPic
        {
            get
            {
                return this.displayPic == "Y";
            }
        }

        /// <summary>
        /// 获取或设置类型 1：存在第一第二属性 2：仅第一属性
        /// </summary>
        public int Type { get; set; }
    }
}
