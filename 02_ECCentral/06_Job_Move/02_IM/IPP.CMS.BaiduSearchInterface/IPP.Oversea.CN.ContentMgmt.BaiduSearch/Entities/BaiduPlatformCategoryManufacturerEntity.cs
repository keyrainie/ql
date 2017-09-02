using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using System.Xml;

namespace IPP.Oversea.CN.ContentMgmt.BaiduSearch.Entities
{
    public class BaiduPlatformCategoryManufacturerEntity
    {
        public BaiduPlatformCategoryManufacturerEntity(int categorySysNo, string categoryName, string pathSegment,
            string brandSegment, string manufacturerName, int number, decimal price)
        {
            this.CategorySysNo = categorySysNo;
            this.PathSegment = pathSegment;
            this.BrandSegment = brandSegment;

            this.DataList = new List<BaiduPlatformCategoryManufacturerData>();
            DataList.Add(new BaiduPlatformCategoryManufacturerData(categoryName, manufacturerName, number, price));
        }

        private int CategorySysNo
        {
            get;
            set;
        }
        private string PathSegment
        {
            get;
            set;
        }
        private string BrandSegment
        {
            get;
            set;
        }

        public string Loc
        {
            get
            {
                return string.Format(AppConfig.CategoryBrandAddress, this.CategorySysNo.ToString(), this.PathSegment, this.BrandSegment);
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

        private IList<BaiduPlatformCategoryManufacturerData> DataList
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

    public class BaiduPlatformCategoryManufacturerData
    {
        public BaiduPlatformCategoryManufacturerData(string categoryName, string manufacturerName, int number, decimal price)
        {
            this.ManufacturerName = manufacturerName;
            this.Number = number;
            this.Price = price;

            this.Tags = categoryName.Replace('、', '\\').Replace('/', '\\');// + "\\" + this.ManufacturerName;
        }

        public string ManufacturerName
        {
            get;
            set;
        }

        public string Tags
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
