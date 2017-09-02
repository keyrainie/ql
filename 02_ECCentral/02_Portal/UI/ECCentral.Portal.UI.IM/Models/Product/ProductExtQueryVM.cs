using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductExtQueryVM:ModelBase
    {
        public ProductExtQueryVM()
        {
            ProductTypeList = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.All);
            ProductStatusList = EnumConverter.GetKeyValuePairs<ProductStatus>(EnumConverter.EnumAppendItemType.All);
            ItemExtBackList = EnumConverter.GetKeyValuePairs<IsDefault>(EnumConverter.EnumAppendItemType.All);
        }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus? ProductStatus { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        public ProductType? ProductType { get; set; }

        /// <summary>
        /// 类别1
        /// </summary>
        public int? Category1 { get; set; }

        /// <summary>
        /// 类别2
        /// </summary>
        public int? Category2 { get; set; }

        /// <summary>
        /// 类别3
        /// </summary>
        public int? Category3 { get; set; }

        /// <summary>
        /// 生产商
        /// </summary>
        private string manufacturer;
        public string Manufacturer {
            get { return manufacturer; }
            set { base.SetValue("Manufacturer", ref manufacturer, value); }
        }

        private string manufacturerSysno;
        public string ManufacturerSysno
        {
            get { return manufacturerSysno; }
            set { base.SetValue("ManufacturerSysno", ref manufacturerSysno, value); }
        }
        /// <summary>
        /// 商品价格
        /// </summary>
        private string productPrice;
        [Validate(ValidateType.Regex, @"^[0-9]", ErrorMessageResourceType=typeof(ResProductQuery),ErrorMessageResourceName="Error_MustInputNumber")]
        public string ProductPrice { 
            get{return productPrice;}
            set { base.SetValue("ProductPrice", ref productPrice, value); }
        }

        /// <summary>
        /// 是否可以退货
        /// </summary>
        public IsDefault? IsPermitRefund { get; set; }

        public List<KeyValuePair<ProductType?, string>> ProductTypeList { get; set; }
        public List<KeyValuePair<IsDefault?, string>> ItemExtBackList { get; set; }
        public List<KeyValuePair<ProductStatus?, string>> ProductStatusList { get; set; }

        public bool HasProductRefundMaintainPermission
        {
            get{return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Product_ProductRefundMaintain);}
        }
    }
}
