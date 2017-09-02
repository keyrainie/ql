//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理QueryModels
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class SellerProductRequestQueryVM : ModelBase
    {

        public List<KeyValuePair<SellerProductRequestStatus?, string>> StatusList { get; set; }
        public List<KeyValuePair<SellerProductRequestType?, string>> TypeList { get; set; }


        public SellerProductRequestQueryVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<SellerProductRequestStatus>(EnumConverter.EnumAppendItemType.None);
            this.StatusList.Insert(0, new KeyValuePair<SellerProductRequestStatus?, string>((SellerProductRequestStatus)0, ResCategoryKPIMaintain.SelectTextAll));
            this.TypeList = EnumConverter.GetKeyValuePairs<SellerProductRequestType>(EnumConverter.EnumAppendItemType.None);
            this.TypeList.Insert(0, new KeyValuePair<SellerProductRequestType?, string>((SellerProductRequestType)0, ResCategoryKPIMaintain.SelectTextAll));
        }

        /// <summary>
        /// 一级类
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        private string productSysNo;
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,8}$", ErrorMessageResourceType=typeof(ResBrandQuery),ErrorMessageResourceName="Error_ValidateIntHint")]
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        /// <summary>
        /// 请求开始时间
        /// </summary>
        public DateTime? RequestStartDate { get; set; }

        /// <summary>
        /// 请求结束时间
        /// </summary>
        public DateTime? RequestEndDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 需求类型
        /// </summary>
        public SellerProductRequestType Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SellerProductRequestStatus Status { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// CommonSKUNumber
        /// </summary>
        public string CommonSKUNumber { get; set; }

        /// <summary>
        /// 是否含有图片
        /// </summary>
        public SellerProductRequestTakePictures IsTakePictures { get; set; }


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
}
