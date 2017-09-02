using Nesoft.ECWeb.MobileService.Models.MemberService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ProductDetailModel
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        public BasicInfoModel BasicInfo { get; set; }

        /// <summary>
        /// 商品销售信息
        /// </summary>
        public SalesInfoModel SalesInfo { get; set; }

        /// <summary>
        /// 商品描述信息
        /// </summary>
        public ProductContentModel DescInfo { get; set; }

        /// <summary>
        /// 商品图片列表
        /// </summary>
        public List<ProductImageModel> ImageList { get; set; }

        /// <summary>
        /// 商品组信息
        /// </summary>
        public GroupPropertyModel GroupPropertyInfo { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<AttachmentInfo> AttachmentInfo { get; set; }

        /// <summary>
        /// 配件列表
        /// </summary>
        public List<AttachmentInfo> AccessoryList { get; set; }

        /// <summary>
        /// 促销信息
        /// </summary>
        public PromoInfoModel PromoInfo { get; set; }

        /// <summary>
        /// 团购信息
        /// </summary>
        public GroupBuyDetailModel GroupBuyInfo { get; set; }

        /// <summary>
        /// UI上可执行的操作信息
        /// </summary>
        public UIActionInfo ActionInfo { get; set; }

        /// <summary>
        /// 企业(商家)基本信息
        /// </summary>
        public StoreBasicInfoModel StoreBasicInfo { get; set; }
    }
}