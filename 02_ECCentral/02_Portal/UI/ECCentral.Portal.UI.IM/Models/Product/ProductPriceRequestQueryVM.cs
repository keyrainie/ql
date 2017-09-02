using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductPriceRequestQueryVM : ModelBase
    {
        public ProductPriceRequestQueryVM()
        {
            this.ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            StatusList = EnumConverter.GetKeyValuePairs<QueryProductPriceRequestAuditType>(EnumConverter.EnumAppendItemType.All);
            ComparisonList = EnumConverter.GetKeyValuePairs<Comparison>(EnumConverter.EnumAppendItemType.None);
            ManufacturerInfo = new ManufacturerVM();
        }
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 一级分类
        /// </summary>
        private int? _category1;
        public int? Category1
        {
            get { return _category1; }
            set { base.SetValue("Category1", ref _category1, value); }
        }

        /// <summary>
        /// 二级分类
        /// </summary>
        private int? _category2;
        public int? Category2
        {
            get { return _category2; }
            set { base.SetValue("Category2", ref _category2, value); }
        }

        /// <summary>
        /// 三级分类
        /// </summary>
        private int? _category3;
        public int? Category3
        {
            get { return _category3; }
            set { base.SetValue("Category3", ref _category3, value); }
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }


        private int? _manufacturerSysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? ManufacturerSysNo
        {
            get { return _manufacturerSysNo; }
            set { SetValue("ManufacturerSysNo", ref _manufacturerSysNo, value); }
        }

        /// <summary>
        /// 生产商SysNO
        /// </summary>
        public int? ProductSysNo { get; set; }

        private string _manufacturerName;
        /// <summary>
        /// 系统编号
        /// </summary>
        public string ManufacturerName
        {
            get { return _manufacturerName; }
            set { SetValue("ManufacturerName", ref _manufacturerName, value); }
        }


        /// <summary>
        /// 生产商SysNO
        /// </summary>
        public string ProductID { get; set; }

        public ManufacturerVM ManufacturerInfo { get; set; }

        /// <summary>
        /// 比较符
        /// </summary>
        public Comparison ComparisonOperators { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public QueryProductPriceRequestAuditType? RequestStatus { get; set; }

        /// <summary>
        /// 审核类型
        /// </summary>
        public QueryProductPriceRequestAuditType? AuditType { get; set; }

        List<KeyValuePair<ProductStatus?, string>> _productStatusList;

        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList
        {
            get { return _productStatusList; }
            set
            {
                base.SetValue("ProductStatusList", ref _productStatusList, value);
            }
        }

        List<KeyValuePair<QueryProductPriceRequestAuditType?, string>> _statusList;

        public List<KeyValuePair<QueryProductPriceRequestAuditType?, string>> StatusList
        {
            get { return _statusList; }
            set
            {
                base.SetValue("StatusList", ref _statusList, value);
            }
        }

        List<KeyValuePair<Comparison?, string>> _comparisonList;

        public List<KeyValuePair<Comparison?, string>> ComparisonList
        {
            get { return _comparisonList; }
            set
            {
                base.SetValue("ComparisonList", ref _comparisonList, value);
            }
        }

        public bool HasAuditPricePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && (AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_PrimaryAuditPrice) ||
                       AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_AdvancedAuditPrice));
            }
        }

        /// <summary>
        /// 初级审核权限
        /// </summary>
        public bool HasPrimaryAuditPricePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_PrimaryAuditPrice);
            }
        }

        /// <summary>
        /// 高级审核权限
        /// </summary>
        public bool HasAdvancedAuditPricePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPriceMaintain) && AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_AdvancedAuditPrice);
            }
        }

        /// <summary>
        /// 初级查询权限
        /// </summary>
        public bool HasPMPrimary
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPMPrimary);
            }
        }

        /// <summary>
        /// 高级查询权限
        /// </summary>
        public bool HasPMManager
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductPrice_ItemPMManager);
            }
        }

        public int PMRole 
        {
            get 
            {
                if (HasPMManager)
                {
                    return 3;
                }
                else if (HasPMPrimary)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}
