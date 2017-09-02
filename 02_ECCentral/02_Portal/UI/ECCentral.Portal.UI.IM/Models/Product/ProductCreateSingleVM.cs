using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductCreateSingleVM : ModelBase
    {
        public ProductCreateSingleVM()
        {
            var productTypeList = new List<KeyValuePair<string, string>>();
            var consignFlagList = new List<KeyValuePair<int, string>>();

            productTypeList.Add(new KeyValuePair<string, string>("0", ResProductCreate.NormalGoods));
           // productTypeList.Add(new KeyValuePair<string, string>("1", ResProductCreate.Secondhand));
            productTypeList.Add(new KeyValuePair<string, string>("3", ResProductCreate.VirtualProduct));

            consignFlagList.Add(new KeyValuePair<int, string>(0, ResProductCreate.Distribute));
            consignFlagList.Add(new KeyValuePair<int, string>(5, ResProductCreate.GroupBuying));
            consignFlagList.Add(new KeyValuePair<int, string>(1, ResProductCreate.Consignment));
            consignFlagList.Add(new KeyValuePair<int, string>(3, ResProductCreate.Collection));
           // consignFlagList.Add(new KeyValuePair<int, string>(4, ResProductCreate.AgentBusiness));
            ProductTypeList = productTypeList;
            ConsignFlagList = consignFlagList;
            ProductModel = string.Empty;
        }

        //商品类型
        public List<KeyValuePair<string, string>> ProductTypeList { get; set; }
        //代销属性
        public List<KeyValuePair<int, string>> ConsignFlagList { get; set; }


        
        

        /// <summary>
        /// 商品标题
        /// </summary>
        private string _productTitle;

        [Validate(ValidateType.Required)]
        public string ProductTitle
        {
            get { return _productTitle; }
            set { SetValue("ProductTitle", ref _productTitle, value); }
        }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 品牌SysNo
        /// </summary>
        private int? _brandSysNo;
        public int? BrandSysNo
        {
            get { return _brandSysNo; }
            set { SetValue("BrandSysNo", ref _brandSysNo, value); }
        }

        /// <summary>
        /// C2SysNo
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// C3SysNo
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        private string _productModel;

        [Validate(ValidateType.Required)]
        public string ProductModel
        {
            get { return _productModel; }
            set { SetValue("ProductModel", ref _productModel, value); }
        }

        private string productType;
        /// <summary>
        /// 商品类型
        /// </summary>
        public string ProductType
        {
            get
            {
                return productType;
            }
            set
            {
                SetValue("ProductType", ref productType, value);
            }
        }

        /// <summary>
        /// 代销属性
        /// </summary>
        public int ConsignFlag { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// UPCCode
        /// </summary>
        public string UPCCode { get; set; }

        /// <summary>
        /// BM编号
        /// </summary>
        public string BMCode { get; set; }

        /// <summary>
        /// 正常采购价
        /// </summary>
        private string _virtualPrice;

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"\d+(\.\d\d)?", ErrorMessageResourceType=typeof(ResProductCreate),ErrorMessageResourceName="Error_VirtualPriceMessage")]
        public string VirtualPrice
        {
            get { return _virtualPrice; }
            set { SetValue("VirtualPrice", ref _virtualPrice, value); }
        }

        private string _productID;

        public string ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }

        /// <summary>
        /// 是否拍照
        /// </summary>
        private bool isTakePictures;
        public bool IsTakePictures
        {
            get
            {
                return isTakePictures;
            }
            set
            {
                SetValue("IsTakePictures", ref isTakePictures, value);
            }
        }

        private string _OrginCode;
        [Validate(ValidateType.Required)]
        public string OrginCode
        {
            get { return _OrginCode; }
            set { SetValue("OrginCode", ref _OrginCode, value); }
        }
    }
}
