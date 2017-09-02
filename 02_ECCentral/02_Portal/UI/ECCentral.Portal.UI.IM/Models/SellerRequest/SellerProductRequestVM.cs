//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理Models
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************
using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class SellerProductRequestVM : ModelBase
    {

        public List<KeyValuePair<SellerProductRequestStatus?, string>> StatusList { get; set; }
        public List<KeyValuePair<SellerProductRequestType?, string>> TypeList { get; set; }
        public List<KeyValuePair<VendorConsignFlag?, string>> ConsignList { get; set; }
        public List<KeyValuePair<SellerProductRequestTakePictures?, string>> TakePicturesList { get; set; }
        public List<KeyValuePair<SellerProductRequestOfferInvoice?, string>> OfferInvoiceList { get; set; }

        public SellerProductRequestVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<SellerProductRequestStatus>(EnumConverter.EnumAppendItemType.None);
            this.TypeList = EnumConverter.GetKeyValuePairs<SellerProductRequestType>(EnumConverter.EnumAppendItemType.None);
            this.ConsignList = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.None);
            this.TakePicturesList = EnumConverter.GetKeyValuePairs<SellerProductRequestTakePictures>(EnumConverter.EnumAppendItemType.None);
            this.OfferInvoiceList = EnumConverter.GetKeyValuePairs<SellerProductRequestOfferInvoice>(EnumConverter.EnumAppendItemType.None);
        }

        public string CompanyCode { get; set; }

        public int? SysNo { get; set; }

        public int RequestSysno { get; set; }

        public int SellerSysNo { get; set; }

        public string SellerName { get; set; }

        public string SellerSite { get; set; }

        public int GroupSysno { get; set; }


        /// <summary>
        /// 三级分类
        /// </summary>
        public CategoryVM CategoryInfo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public BrandVM Brand { get; set; }

        /// <summary>
        /// 生产商
        /// </summary>
        public ManufacturerVM Manufacturer { get; set; }

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

        /// <summary>
        /// 是否含有图片
        /// </summary>
        public SellerProductRequestTakePictures IsTakePictures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProductLink { get; set; }

        /// <summary>
        /// 包装清单
        /// </summary>
        public string PackageList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Attention { get; set; }


        /// <summary>
        /// 主机保修天数
        /// </summary>
        private string hostWarrantyDay;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string HostWarrantyDay
        {
            get { return hostWarrantyDay; }
            set { base.SetValue("HostWarrantyDay", ref hostWarrantyDay, value); }
        }

        /// <summary>
        /// 配件保修天数
        /// </summary>
        private string partWarrantyDay;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string PartWarrantyDay
        {
            get { return partWarrantyDay; }
            set { base.SetValue("PartWarrantyDay", ref partWarrantyDay, value); }
        }

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
        /// 
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 商品重量
        /// </summary>
        private string weight;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string Weight
        {
            get
            {
                return weight;
            }
            set
            {
                base.SetValue("Weight", ref weight, value);
            }
        }

        /// <summary>
        /// 商品长度
        /// </summary>
        private string length;
        [Validate(ValidateType.Regex, @"^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResProductMaintain),ErrorMessageResourceName="Error_ProductPropertyLengthMessage")]
        public string Length
        {
            get
            {
                return length;
            }
            set
            {
                decimal result;
                if (decimal.TryParse(value, out result))
                {
                    value = result.ToString("0.00");
                }
                base.SetValue("Length", ref length, value);

                if (result <= 0)
                {
                    string error = "";
                    base.SetValue("Length", ref error, "error");
                }

            }
        }

        /// <summary>
        /// 商品宽度
        /// </summary>
        private string width;
        [Validate(ValidateType.Regex, @"^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResProductMaintain),ErrorMessageResourceName="Error_ProductPropertyLengthMessage")]
        public string Width
        {
            get
            {
                return width;
            }
            set
            {

                decimal result;
                if (decimal.TryParse(value, out result))
                {
                    value = result.ToString("0.00");
                }

                base.SetValue("Width", ref width, value);

                if (result <= 0)
                {
                    string error = "";
                    base.SetValue("Width", ref error, "error");
                }
            }
        }

        /// <summary>
        /// 商品高度
        /// </summary>
        private string height;
        [Validate(ValidateType.Regex, @"^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResProductMaintain),ErrorMessageResourceName="Error_ProductPropertyLengthMessage")]
        public string Height
        {
            get
            {
                return height;
            }
            set
            {
                decimal result;
                if (decimal.TryParse(value, out result))
                {
                    value = result.ToString("0.00");
                }

                base.SetValue("Height", ref height, value);

                if (result <= 0)
                {
                    string error = "";
                    base.SetValue("Height", ref error, "error");
                }

            }
        }

        /// <summary>
        /// 最小包装数
        /// </summary>
        private string minPackNumber;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,7}$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntMessage")]
        public string MinPackNumber
        {
            get { return minPackNumber; }
            set { base.SetValue("MinPackNumber", ref minPackNumber, value); }
        }

        /// <summary>
        /// 正常采购价格
        /// </summary>
        private string virtualPrice;
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResCategoryKPIMaintain),ErrorMessageResourceName="Error_InputIntDoubleMessage")]
        public string VirtualPrice
        {
            get
            {
                return virtualPrice;
            }
            set
            {
                decimal result;
                if (decimal.TryParse(value, out result))
                {
                    value = result.ToString("0.00");
                }

                base.SetValue("VirtualPrice", ref virtualPrice, value);
            }
        }

        public string GrossMarginRate
        {
            get { return (decimal.Parse(VirtualPrice) == 0 ? "1.00" : (((CurrentPrice - decimal.Parse(VirtualPrice)) / CurrentPrice) * 100).ToString("0.00")) + "%"; }
        }

        /// <summary>
        /// 图片数
        /// </summary>
        public int PicNumber { get; set; }

        /// <summary>
        /// 会员等级
        /// </summary>
        public int OnlyForRank { get; set; }

        /// <summary>
        /// 不提供发票
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
        /// 
        /// </summary>
        public int GroupRequestSysno { get; set; }

        /// <summary>
        /// PM
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
        /// 泰隆优选价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 商品详细描述
        /// </summary>
        public string ProductDescLong { get; set; }

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
        /// 语言
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 商品属性信息
        /// </summary>
        public List<SellerProductRequestPropertyVM> SellerProductRequestPropertyList { get; set; }

        /// <summary>
        /// 商品文件信息
        /// </summary>
        public List<SellerProductRequestFileVM> SellerProductRequestFileList { get; set; }

        public bool HasItemVendorPortalNewProductApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SellerPortalProduct_ItemVendorPortalNewProductApprove); }
        }

        public bool HasItemVendorPortalNewProductCreateIDPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SellerPortalProduct_ItemVendorPortalNewProductCreateID); }
        }

        public bool HasItemVendorPortalNewProductDenyPermission
        {
            get
            {
                return
                    AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SellerPortalProduct_ItemVendorPortalNewProductDecline) ||
                    AuthMgr.HasFunctionPoint(AuthKeyConst.IM_SellerPortalProduct_ItemVendorPortalNewProductSpecialDeny);
            }
        }
    }

    /// <summary>
    /// 商品属性信息
    /// </summary>
    public partial class SellerProductRequestPropertyVM : ModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProductRequestSysno { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int GroupSysno { set; get; }

        /// <summary>
        /// 
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

        public List<PropertyValueVM> PropertyValueList { get; set; }

    }

    /// <summary>
    /// 商品图片信息
    /// </summary>
    public partial class SellerProductRequestFileVM : ModelBase
    {

        public int? SysNo { set; get; }

        /// <summary>
        /// 
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

        public Uri AbsolutePathOnServer
        {
            get
            {
                string path = Path;

                if (path.Length > 0 && path[0] == '/')
                {
                    path = path.Substring(1);
                }

                //Ocean.20130514, Move to ControlPanelConfiguration
                string imageServicePath = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_SellerPortalImageServicePath);
                string fileServicePath = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_SellerPortalFileServicePath);
                if (string.IsNullOrEmpty(imageServicePath))
                    imageServicePath = "http://127.0.0.1/";
                if (string.IsNullOrEmpty(fileServicePath))
                    fileServicePath = "http://127.0.0.1/";
                Uri imageServiceUri = new Uri(imageServicePath);
                Uri fileServiceUri = new Uri(fileServicePath);
                Uri outPath;

                // photos
                if (Type == SellerProductRequestFileType.Image)
                {
                    if (Uri.TryCreate(imageServiceUri, path, out outPath))
                    {
                        return outPath;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    if (Uri.TryCreate(fileServiceUri, path, out outPath))
                    {
                        return outPath;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}