using Nesoft.ECWeb.Entity;
using Nesoft.ECWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.MemberService
{
    public class FrontProductCategoryInfoModel: EntityBase
    {
        public FrontProductCategoryInfoModel()
        {
            Children = new List<FrontProductCategoryInfoModel>();
        }

        /// <summary>
        /// 系统编号
        /// </summary>
        public string N { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 类别编码
        /// </summary>
        public string CategoryCode { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 类别父编码
        /// </summary>
        public string ParentCategoryCode { get; set; }
        /// <summary>
        /// 是否是最终叶子节点
        /// </summary>
        public CommonYesOrNo IsLeaf { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 页面链接模式
        /// </summary>
        public FPCLinkUrlModeType FPCLinkUrlMode { get; set; }
        /// <summary>
        /// 前台展示方式
        /// </summary>
        public UIModeType UIModeType { get; set; }
        /// <summary>
        /// 链接URL
        /// </summary>
        public string FPCLinkUrl { get; set; }
        /// <summary>
        /// 最终叶子节点关联的后台C3编码
        /// </summary>
        public int? C3SysNo { get; set; }

        public List<FrontProductCategoryInfoModel> Children { get; set; }
    }
}