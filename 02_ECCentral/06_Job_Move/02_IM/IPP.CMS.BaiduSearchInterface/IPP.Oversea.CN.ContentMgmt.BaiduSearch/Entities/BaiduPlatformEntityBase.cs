using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;

namespace IPP.Oversea.CN.ContentMgmt.BaiduSearch.Entities
{
    public abstract class BaiduPlatformEntityBase
    {
        public abstract string Loc
        {
            get;
            set;
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

        public IList<BaiduPlatformDataBase> DataList
        {
            get;
            set;
        }

        public void WriteXml(XmlWriter xmlWriter)
        {
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

    public abstract class BaiduPlatformDataBase
    {
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

        public abstract string Tags
        {
            get;
            set;
        }

        public abstract string Brand
        {
            get;
            set;
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
            xmlWriter.WriteStartElement("display");

            xmlWriter.WriteElementString("store", this.Store);
            xmlWriter.WriteElementString("store_pic", this.StorePicture);
            xmlWriter.WriteElementString("tags", this.Tags);
            xmlWriter.WriteElementString("services", this.Services);
            xmlWriter.WriteElementString("brand", this.Brand);
            xmlWriter.WriteElementString("number", this.Number.ToString());
            xmlWriter.WriteElementString("price", this.Price.ToString());

            xmlWriter.WriteEndElement();
        }
    }

}
