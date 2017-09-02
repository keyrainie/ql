using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using System.Xml;

namespace IPP.Oversea.CN.ContentMgmt.BaiduSearch.Entities
{
    public class BaiduPlatformManufacturerEntity
    {
        public BaiduPlatformManufacturerEntity(BaiduManufacturerEntity brand)
        {
            this.ManufacturerSysNo = brand.ManufacturerSysNo;
            this.ManufacturerType = brand.Type;
            this.InitialPingYin = brand.InitialPingYin;

            this.DataList = new List<BaiduPlatformManufacturerData>();
            DataList.Add(new BaiduPlatformManufacturerData(brand.ManufacturerName, brand.ProductCount, brand.MinPrice));
        }

        private int ManufacturerSysNo
        {
            get;
            set;
        }

        private int ManufacturerType
        {
            get;
            set;
        }

        private string InitialPingYin
        {
            get;
            set;
        }

        public string Loc
        {
            get
            {
                if (this.ManufacturerType == 2)
                {
                    return string.Format(AppConfig.FlagshipBrandAddress, this.ManufacturerSysNo.ToString(), this.InitialPingYin);
                }
                else
                {
                    return string.Format(AppConfig.CommonBrandAddress, (800000000 + this.ManufacturerSysNo).ToString());
                }
            }
            set
            {
            }
        }

        public string LastModifiedDateTime
        {
            get
            {
                return AppConfig.LastModifiedDateTime;
            }
        }

        public string ChangeFrequency
        {
            get
            {
                return AppConfig.ChangeFrequency;
            }
        }

        public string Priority
        {
            get
            {
                return AppConfig.Priority;
            }
        }

        private IList<BaiduPlatformManufacturerData> DataList
        {
            get;
            set;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            if (DataList != null)
            {
                bool hasProduct = false;

                foreach (var data in this.DataList)
                {
                    if (data.Number > 0)
                    {
                        hasProduct = true;
                        break;
                    }
                }

                if (!hasProduct)
                {
                    return;
                }
            }

            xmlWriter.WriteStartElement("url");

            xmlWriter.WriteElementString("loc", this.Loc);
            xmlWriter.WriteElementString("lastmod", this.LastModifiedDateTime);
            xmlWriter.WriteElementString("changefreq", this.ChangeFrequency);
            xmlWriter.WriteElementString("priority", this.Priority);

            if (this.DataList != null && this.DataList.Count > 0)
            {
                xmlWriter.WriteStartElement("data");

                foreach (var data in this.DataList)
                {
                    data.WriteXml(xmlWriter);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }

    public class BaiduPlatformManufacturerData
    {
        public BaiduPlatformManufacturerData(string manufacturerName, int number, decimal price)
        {
            this.ManufacturerName = manufacturerName;
            this.Number = number;
            this.Price = price;
        }

        public string Tags
        {
            get
            {
                return "品牌";
            }
        }

        public string ManufacturerName
        {
            get;
            set;
        }

        public string Store
        {
            get
            {
                return AppConfig.StoreName;
            }
        }

        public string StorePicture
        {
            get
            {
                return AppConfig.StorePic;
            }
        }

        public string Services
        {
            get
            {
                return AppConfig.Service;
            }
        }

        public int Number
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
            if (this.Number < 1)
            {
                return;
            }

            xmlWriter.WriteStartElement("display");

            xmlWriter.WriteElementString("store", this.Store);
            xmlWriter.WriteElementString("store_pic", this.StorePicture);
            xmlWriter.WriteElementString("tags", this.Tags);
            xmlWriter.WriteElementString("services", this.Services);
            xmlWriter.WriteElementString("brand", this.ManufacturerName);
            xmlWriter.WriteElementString("number", this.Number.ToString());
            xmlWriter.WriteElementString("price", this.Price.ToString("0.00"));

            xmlWriter.WriteEndElement();
        }
    }
}
