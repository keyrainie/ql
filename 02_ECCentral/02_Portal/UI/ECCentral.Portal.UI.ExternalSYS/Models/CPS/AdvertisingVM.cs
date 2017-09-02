using System;
using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class AdvertisingVM : ModelBase
    {
        public AdvertisingVM()
        {
            this.AdvertisingTypeList = EnumConverter.GetKeyValuePairs<AdvertisingType>();
            this.AdvertisingTypeList.RemoveAll(at => at.Key == AdvertisingType.Custom);

            this.ImageSizeList = new List<CodeNamePair>();
            //ImageSizeList.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });

            this.ProductLineCategoryList = new List<CodeNamePair>();
            ProductLineCategoryList.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });

            this.ProductLineList = new List<CodeNamePair>();
            ProductLineList.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
        }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }

        private int? productLineCategorySysNo;
        public int? ProductLineCategorySysNo
        {
            get { return productLineCategorySysNo; }
            set { SetValue("ProductLineCategorySysNo", ref productLineCategorySysNo, value); }
        }

        private int? productLineSysNo;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public int? ProductLineSysNo
        {
            get { return productLineSysNo; }
            set { SetValue("ProductLineSysNo", ref productLineSysNo, value); }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { SetValue("Text", ref text, value); }
        }

        private string sharedText;
        [Validate(ValidateType.Required)]
        public string SharedText
        {
            get { return sharedText; }
            set { SetValue("SharedText", ref sharedText, value); }
        }

        private string eventCode;
        [Validate(ValidateType.Required)]
        public string EventCode
        {
            get { return eventCode; }
            set { SetValue("EventCode", ref eventCode, value); }
        }

        private string adCode;
        public string AdCode
        {
            get { return adCode; }
            set { SetValue("AdCode", ref adCode, value); }
        }

        private string url;
        [Validate(ValidateType.Required)]
        public string Url
        {
            get { return url; }
            set {
               
                SetValue("Url", ref url, value);
                }
        }

        private string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl; }

            set {
                if (!value.Contains("http://")&& !string.IsNullOrEmpty(value))
                {
                    //Ocean.20130514, Move to ControlPanelConfiguration
                    string cpsImageUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.ConfigKey_External_CPSAdvertisingImageUrl);
                    value = string.Format(cpsImageUrl, value);
                }
                SetValue("ImageUrl", ref imageUrl, value); 
            }
        }

        private int? imageWidth;
        public int? ImageWidth
        {
            get { return imageWidth; }
            set { SetValue("ImageWidth", ref imageWidth, value); }
        }

        private int? imageHeight;
        public int? ImageHeight
        {
            get { return imageHeight; }
            set { SetValue("ImageHeight", ref imageHeight, value); }
        }

        private AdvertisingType? type;
        public AdvertisingType? Type
        {
            get { return type; }
            set { base.SetValue("Type", ref type, value); }
        }

        private ValidStatus? status;
        public ValidStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private string inUser;
        public string InUser
        {
            get { return inUser; }
            set { SetValue("InUser", ref inUser, value); }
        }

        public List<CodeNamePair> ProductLineCategoryList { get; set; }

        public List<CodeNamePair> ProductLineList { get; set; }

        public List<KeyValuePair<AdvertisingType?, string>> AdvertisingTypeList { get; set; }

        public List<CodeNamePair> ImageSizeList { get; set; }

        public string ImageSize
        {
            get
            {
                return this.ImageWidth + "*" + this.ImageHeight;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "*")
                {
                    string[] dimission = value.Split('*');
                    this.ImageWidth = int.Parse(dimission[0]);
                    this.ImageHeight = int.Parse(dimission[1]);
                }
            }
        }

    }
}
