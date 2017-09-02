using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoSpecificationInfoVM : ModelBase
    {

        public ProductMaintainBasicInfoSpecificationInfoVM()
        {
            ProductTypeList = EnumConverter.GetKeyValuePairs<ProductType>(EnumConverter.EnumAppendItemType.None);

            ProductConsignFlagList = EnumConverter.GetKeyValuePairs<VendorConsignFlag>(EnumConverter.EnumAppendItemType.None);

            ProductIsTakePictureList =
                EnumConverter.GetKeyValuePairs<ProductIsTakePicture>(EnumConverter.EnumAppendItemType.None);
        }

        public List<KeyValuePair<ProductType?, string>> ProductTypeList { get; set; }

        public List<KeyValuePair<VendorConsignFlag?, string>> ProductConsignFlagList { get; set; }

        public List<KeyValuePair<ProductIsTakePicture?, string>> ProductIsTakePictureList { get; set; }

        private ManufacturerVM _manufacturerInfo;

        public ManufacturerVM ManufacturerInfo
        {
            get { return _manufacturerInfo; }
            set { SetValue("ManufacturerInfo", ref _manufacturerInfo, value); }
        }

        private BrandVM _brandInfo;

        public BrandVM BrandInfo
        {
            get { return _brandInfo; }
            set { SetValue("BrandInfo", ref _brandInfo, value); }
        }

        private CategoryVM _categoryInfo;

        public CategoryVM CategoryInfo
        {
            get { return _categoryInfo; }
            set { SetValue("CategoryInfo", ref _categoryInfo, value); }
        }

        private String _productModel;

        //[Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductModelEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        //[Validate(ValidateType.MaxLength, 100, ErrorMessageResourceName = "ProductMaintain_ProductModelInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        public String ProductModel
        {
            get { return _productModel; }
            set { SetValue("ProductModel", ref _productModel, value); }
        }

        private ProductType _productType;

        public ProductType ProductType
        {
            get { return _productType; }
            set { SetValue("ProductType", ref _productType, value); }
        }

        private VendorConsignFlag _productConsignFlag;

        public VendorConsignFlag ProductConsignFlag
        {
            get { return _productConsignFlag; }
            set { SetValue("ProductConsignFlag", ref _productConsignFlag, value); }
        }

        private PMVM _productManagerInfo;

        public PMVM ProductManagerInfo
        {
            get { return _productManagerInfo; }
            set { SetValue("ProductManagerInfo", ref _productManagerInfo, value); }
        }

        private ProductIsTakePicture _productIsTakePicture;

        public ProductIsTakePicture ProductIsTakePicture
        {
            get { return _productIsTakePicture; }
            set { SetValue("ProductIsTakePicture", ref _productIsTakePicture, value); }
        }

        private string _jdProductID;

        public string JDProductID
        {
            get { return _jdProductID; }
            set { SetValue("JDProductID", ref _jdProductID, value); }
        }

        private string _amProductID;

        public string AMProductID
        {
            get { return _amProductID; }
            set { SetValue("AMProductID", ref _amProductID, value); }
        }

        private string _BMCode;
        public string BMCode
        {
            get { return _BMCode; }
            set { SetValue("BMCode", ref _BMCode, value); }
        }

        private string _UPCCode;
        [Validate(ValidateType.Required)]
        public string UPCCode
        {
            get { return _UPCCode; }
            set { SetValue("UPCCode", ref _UPCCode, value); }
        }

        public bool HasJDAndAMItemNumberMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_JDAndAMItemNumberMaintain); }
        }

        public bool HasItemTakePicturesPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemTakePictures); }
        }

        public bool HasItemBasicInformationProductModelMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationProductModelMaintain); }
        }
    }
}
