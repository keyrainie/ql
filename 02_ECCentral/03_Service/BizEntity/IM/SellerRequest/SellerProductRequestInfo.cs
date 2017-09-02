using ECCentral.BizEntity.Common;
using System.Collections.Generic;

using ECCentral.BizEntity.PO;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品信息
    /// </summary>
    public class SellerProductRequestInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言编码
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestSysno { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int SellerSysNo { get; set; }

        /// <summary>
        /// 商家名称
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 商家站点
        /// </summary>
        public string SellerSite { get; set; }

        /// <summary>
        /// 分组编号
        /// </summary>
        public int GroupSysno { get; set; }

        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryInfo CategoryInfo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public BrandInfo Brand { get; set; }

        /// <summary>
        /// 生产商
        /// </summary>
        public ManufacturerInfo Manufacturer { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 商品简名
        /// </summary>
        public string BriefName { get; set; }

        /// <summary>
        /// 商品关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 商品UPCCode
        /// </summary>
        public string UPCCode { get; set; }

        /// <summary>
        /// CommonSKUNumber
        /// </summary>
        public string CommonSKUNumber { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID { get; set; }

        private SellerProductRequestTakePictures _isTakePictures;
        /// <summary>
        /// 是否含有图片
        /// </summary>
        public SellerProductRequestTakePictures IsTakePictures
        {
            get
            {
                return _isTakePictures;
            }
            set
            {
                if (value.ToString().Equals("0"))
                {
                    value = SellerProductRequestTakePictures.No;
                }

                if (value.ToString().Equals("1"))
                {
                    value = SellerProductRequestTakePictures.Yes;
                }

                _isTakePictures = value;
            }
        }

        /// <summary>
        /// 产品链接
        /// </summary>
        public string ProductLink { get; set; }

        /// <summary>
        /// 包装清单
        /// </summary>
        public string PackageList { get; set; }

        /// <summary>
        /// 注意事项
        /// </summary>
        public string Attention { get; set; }

        /// <summary>
        /// 主机保修天数
        /// </summary>
        public int HostWarrantyDay { get; set; }

        /// <summary>
        /// 配件保修天数
        /// </summary>
        public int PartWarrantyDay { get; set; }

        /// <summary>
        /// 保修条款
        /// </summary>
        public string Warranty { get; set; }

        /// <summary>
        /// 保修服务电话
        /// </summary>
        public string ServicePhone { get; set; }

        /// <summary>
        /// 报修服务信息
        /// </summary>
        public string ServiceInfo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 商品重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 商品长度
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// 商品宽度
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 商品高度
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 最小包装数
        /// </summary>
        public int MinPackNumber { get; set; }

        /// <summary>
        /// 正常采购价格
        /// </summary>
        public decimal VirtualPrice { get; set; }

        /// <summary>
        /// 图片数
        /// </summary>
        public int PicNumber { get; set; }

        /// <summary>
        /// 图片数
        /// </summary>
        public int OnlyForRank { get; set; }

        /// <summary>
        /// 是否不开增票
        /// </summary>
        public SellerProductRequestOfferInvoice IsOfferInvoice { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SellerProductRequestStatus Status { get; set; }

        /// <summary>
        /// 需求类型
        /// </summary>
        public SellerProductRequestType Type { get; set; }

        /// <summary>
        /// 商品促销信息
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo Auditor { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo InUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 分组申请编号
        /// </summary>
        public int GroupRequestSysno { get; set; }

        /// <summary>
        /// PM信息
        /// </summary>
        public UserInfo PMUser { get; set; }

        /// <summary>
        /// 代销
        /// </summary>
        public VendorConsignFlag IsConsign { get; set; }

        /// <summary>
        /// 市场价格
        /// </summary>
        public decimal BasicPrice { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 商品详细描述
        /// </summary>
        public string ProductDescLong { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 产地Code
        /// </summary>
        public string OrginCode { get; set; }

        /// <summary>
        /// 产地名称
        /// </summary>
        public string OrginName { get; set; }

        /// <summary>
        /// 商品属性信息
        /// </summary>
        public List<SellerProductRequestPropertyInfo> SellerProductRequestPropertyList { get; set; }

        /// <summary>
        /// 商品文件信息
        /// </summary>
        public List<SellerProductRequestFileInfo> SellerProductRequestFileList { get; set; }

    }

    /// <summary>
    /// 商品属性信息
    /// </summary>
    public class SellerProductRequestPropertyInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品申请编号
        /// </summary>
        public int ProductRequestSysno { set; get; }

        /// <summary>
        /// 分组编号
        /// </summary>
        public int GroupSysno { set; get; }

        /// <summary>
        /// 分组描述
        /// </summary>
        public string GroupDescription { set; get; }

        /// <summary>
        /// 属性编号
        /// </summary>
        public int PropertySysno { set; get; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyDescription { get; set; }

        /// <summary>
        /// 属性值编号
        /// </summary>
        public int ValueSysno { set; get; }

        /// <summary>
        /// 属性值
        /// </summary>
        public string ValueDescription { get; set; }

        /// <summary>
        /// 自定义属性值
        /// </summary>
        public string ManualInput { get; set; }
    }

    /// <summary>
    /// 商品图片信息
    /// </summary>
    public class SellerProductRequestFileInfo : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { set; get; }

        /// <summary>
        /// 商品申请编号
        /// </summary>
        public int ProductRequestSysno { set; get; }

        /// <summary>
        /// 文件标题
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { set; get; }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Memo { set; get; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public SellerProductRequestFileType Type { set; get; }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string ImageName { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public SellerProductRequestFileStatus Status { set; get; }
    }
}
