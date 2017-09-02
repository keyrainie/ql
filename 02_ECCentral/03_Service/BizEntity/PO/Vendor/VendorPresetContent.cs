using System.Xml.Serialization;

namespace ECCentral.BizEntity.PO
{
    [XmlRoot("vendorPresetContent")]
    public class VendorPresetContent
    {
        [XmlElement("storePageInfo")]
        public StorePageInfoElement[] StorePageInfos { get; set; }
        [XmlElement("storePageHeader")]
        public StorePageHeaderElement StorePageHeader { get; set; }

        [XmlElement("userRoleId")]
        public string[] UserRoleIds { get; set; }

        public class StorePageHeaderElement
        {
            [XmlElement("headerContent")]
            public string HeaderContent { get; set; }
        }

        public class StorePageInfoElement
        {
            [XmlElement("templateKey")]
            public string TemplateKey { get; set; }
            [XmlElement("pageTypeKey")]
            public string PageTypeKey { get; set; }
            [XmlElement("pageName")]
            public string PageName { get; set; }
            [XmlElement("dataValue")]
            public string DataValue { get; set; }
            [XmlElement("linkUrl")]
            public string LinkUrl { get; set; }
            [XmlElement("previewLinkUrl")]
            public string PreviewLinkUrl { get; set; }


            /// <summary>
            /// 获得转换后预设DataValue内容为StorePageInfo或PublishedStorePageInfo使用的值
            /// </summary>
            /// <param name="sellerSysNo"></param>
            /// <param name="storePageSysNo"></param>
            /// <returns></returns>
            public string GetDataValueForStorePageInfo(string sellerSysNo, string storePageSysNo = null)
            {
                if (string.IsNullOrWhiteSpace(DataValue))
                    return string.Empty;

                string result = DataValue;

                if (sellerSysNo != null)
                {
                    result = result.Replace("{SellerSysNo}", sellerSysNo);
                }

                if (storePageSysNo != null)
                {
                    result = result.Replace("{StorePageSysNo}", storePageSysNo);
                }

                return result;
            }

            /// <summary>
            /// 获得转换后预设LinkUrl内容为StorePageInfo或PublishedStorePageInfo使用的值
            /// </summary>
            /// <param name="sellerSysNo"></param>
            /// <param name="storePageSysNo"></param>
            /// <returns></returns>
            public string GetLinkUrlForStorePageInfo(string sellerSysNo, string storePageSysNo = null)
            {
                if (string.IsNullOrWhiteSpace(LinkUrl))
                    return string.Empty;

                string result = LinkUrl;

                if (sellerSysNo != null)
                {
                    result = result.Replace("{SellerSysNo}", sellerSysNo);
                }

                if (storePageSysNo != null)
                {
                    result = result.Replace("{StorePageSysNo}", storePageSysNo);
                }

                return result;
            }

            /// <summary>
            /// 获得转换后预设LinkUrl内容为StorePageInfo或PublishedStorePageInfo使用的值
            /// </summary>
            /// <param name="sellerSysNo"></param>
            /// <param name="storePageSysNo"></param>
            /// <returns></returns>
            public string GetPreviewLinkUrlForStorePageInfo(string sellerSysNo, string storePageSysNo = null)
            {
                if (string.IsNullOrWhiteSpace(PreviewLinkUrl))
                    return string.Empty;

                string result = PreviewLinkUrl;

                if (sellerSysNo != null)
                {
                    result = result.Replace("{SellerSysNo}", sellerSysNo);
                }

                if (storePageSysNo != null)
                {
                    result = result.Replace("{StorePageSysNo}", storePageSysNo);
                }

                return result;
            }
        }
    }
}