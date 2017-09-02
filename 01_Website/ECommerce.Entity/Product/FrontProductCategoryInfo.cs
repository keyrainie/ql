using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品商家前台分类信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class FrontProductCategoryInfo : EntityBase
    {
        public FrontProductCategoryInfo()
        {
            Children = new List<FrontProductCategoryInfo>();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }
        /// <summary>
        /// 类别编码
        /// </summary>
        [DataMember]
        public string CategoryCode { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }
        /// <summary>
        /// 类别父编码
        /// </summary>
        [DataMember]
        public string ParentCategoryCode { get; set; }
        /// <summary>
        /// 是否是最终叶子节点
        /// </summary>
        [DataMember]
        public CommonYesOrNo IsLeaf { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 页面链接模式
        /// </summary>
        [DataMember]
        public FPCLinkUrlModeType FPCLinkUrlMode { get; set; }
        /// <summary>
        /// 前台展示方式
        /// </summary>
        public UIModeType UIModeType { get; set; }
        /// <summary>
        /// 链接URL
        /// </summary>
        [DataMember]
        public string FPCLinkUrl { get; set; }
        /// <summary>
        /// 最终叶子节点关联的后台C3编码
        /// </summary>
        [DataMember]
        public int? C3SysNo { get; set; }

        public List<FrontProductCategoryInfo> Children{ get; set; }

    }
}
