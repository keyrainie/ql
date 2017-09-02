using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentMgmt.BaiduSearch.Entities;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using IPP.Oversea.CN.ContentMgmt.BaiduSearch.DataAccess;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using System.Xml;
using System.IO;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.BizProcess
{
    public class BaiduBPV2
    {
        List<BaiduPlatformCategory1Entity> C1EngityList;
        List<BaiduPlatformCategory2Entity> C2EngityList;
        List<BaiduPlatformCategory3Entity> C3EngityList;

        List<BaiduPlatformManufacturerEntity> ManufacturerEntityList;
        List<BaiduPlatformCategoryManufacturerEntity> CategoryManufacturerEntityList;

        List<Category2or3ConfigurationEntity> category3List;
        List<BaiduManufacturerEntity> manufacturerList;

        TxtFileLogger txtFileLogger;

        public BaiduBPV2(TxtFileLogger logger)
        {
            txtFileLogger = logger;
        }

        public void Process()
        {
            txtFileLogger.WriteLog("开始准备数据。");
            this.Init();
            txtFileLogger.WriteLog("准备数据结束。");

            txtFileLogger.WriteLog("开始写XML文件。");
            this.WriteXml();
            txtFileLogger.WriteLog("写XML文件结束。");
        }

        private void Init()
        {
            txtFileLogger.WriteLog("准备C1List");
            InitC1List();

            txtFileLogger.WriteLog("准备C2List");
            InitC2List();

            txtFileLogger.WriteLog("准备C3List");
            InitC3List();

            txtFileLogger.WriteLog("准备ManufacturerList");
            InitManufacturerList();

            txtFileLogger.WriteLog("准备CategoryManufacturerList");
            InitCategoryManufacturerList();
        }

        private void WriteXml()
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Encoding = System.Text.Encoding.UTF8;

            GenerateIndexFile(AppConfig.IndexFileName, setting);
        }

        private void GenerateIndexFile(string fileName, XmlWriterSettings setting)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fs, setting))
                {
                    xmlWriter.WriteStartElement("urlset");

                    txtFileLogger.WriteLog("写C1List");
                    foreach (BaiduPlatformCategory1Entity entity1 in C1EngityList)
                    {
                        entity1.WriteXml(xmlWriter);
                    }

                    txtFileLogger.WriteLog("写C2List");
                    foreach (BaiduPlatformCategory2Entity entity2 in C2EngityList)
                    {
                        entity2.WriteXml(xmlWriter);
                    }

                    txtFileLogger.WriteLog("写C3List");
                    foreach (BaiduPlatformCategory3Entity entity3 in C3EngityList)
                    {
                        entity3.WriteXml(xmlWriter);
                    }

                    txtFileLogger.WriteLog("写ManufacturerList");
                    foreach (BaiduPlatformManufacturerEntity manufacturerEntity in this.ManufacturerEntityList)
                    {
                        manufacturerEntity.WriteXml(xmlWriter);
                    }

                    txtFileLogger.WriteLog("写CategoryManufacturerList");
                    foreach (BaiduPlatformCategoryManufacturerEntity categoryManufacturerEntity in this.CategoryManufacturerEntityList)
                    {
                        categoryManufacturerEntity.WriteXml(xmlWriter);
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        private void InitC1List()
        {
            this.C1EngityList = new List<BaiduPlatformCategory1Entity>();
            List<Category1ConfigurationEntity> category1ConfigList = BaiduDA.GetCategory1ConfigList();

            if (category1ConfigList != null)
            {
                foreach (var c1 in category1ConfigList)
                {
                    ProductNumberAndMinPriceEntity productCountEntity = BaiduDA.GetProductCountByFrontEndCategory1SysNo(c1.CategorySysNo, AppConfig.CompanyCode);

                    BaiduPlatformCategory1Entity c1Entity = new BaiduPlatformCategory1Entity(c1.CategorySysNo, c1.CategoryName, c1.CategoryAddress, productCountEntity.ProductCount, productCountEntity.MinPrice);
                    this.C1EngityList.Add(c1Entity);
                }
            }
        }

        private void InitC2List()
        {
            this.C2EngityList = new List<BaiduPlatformCategory2Entity>();
            List<Category2or3ConfigurationEntity> category2List = BaiduDA.GetCategory2or3List("M", AppConfig.CompanyCode);

            if (category2List != null)
            {
                foreach (var c2 in category2List)
                {
                    ProductNumberAndMinPriceEntity productCountEntity = BaiduDA.GetProductCountByFrontEndCategory2SysNo(c2.CategorySysNo, AppConfig.CompanyCode);

                    BaiduPlatformCategory2Entity c2Entity = new BaiduPlatformCategory2Entity(c2.CategorySysNo, c2.CategoryName, productCountEntity.ProductCount, productCountEntity.MinPrice);
                    this.C2EngityList.Add(c2Entity);
                }
            }
        }

        private void InitC3List()
        {
            this.C3EngityList = new List<BaiduPlatformCategory3Entity>();
            category3List = BaiduDA.GetCategory2or3List("L", AppConfig.CompanyCode);

            if (category3List != null)
            {
                foreach (var c3 in category3List)
                {
                    ProductNumberAndMinPriceEntity productCountEntity = BaiduDA.GetProductCountByFrontEndCategory3SysNo(c3.CategorySysNo, AppConfig.CompanyCode);

                    BaiduPlatformCategory3Entity c3Entity = new BaiduPlatformCategory3Entity(c3.CategorySysNo, c3.CategoryName, productCountEntity.ProductCount, productCountEntity.MinPrice);
                    this.C3EngityList.Add(c3Entity);
                }
            }
        }

        private void InitManufacturerList()
        {
            this.ManufacturerEntityList = new List<BaiduPlatformManufacturerEntity>();
            manufacturerList = BaiduDA.GetBrandList(AppConfig.CompanyCode);

            if (manufacturerList != null)
            {
                foreach (var brand in manufacturerList)
                {
                    BaiduPlatformManufacturerEntity brandEntity = new BaiduPlatformManufacturerEntity(brand);
                    this.ManufacturerEntityList.Add(brandEntity);
                }
            }
        }

        private void InitCategoryManufacturerList()
        {
            this.CategoryManufacturerEntityList = new List<BaiduPlatformCategoryManufacturerEntity>();

            foreach (Category2or3ConfigurationEntity category in category3List)
            {
                foreach (BaiduManufacturerEntity manufacturerEntity in manufacturerList)
                {
                    CategoryManufacturerPathEntity path = BaiduDA.GetCategoryManufacturerPath(manufacturerEntity.ManufacturerSysNo, category.CategorySysNo);
                    ProductNumberAndMinPriceEntity productCountEntity = BaiduDA.GetProductCountByFrontEndCategory3SysNoAndManufacturerSysNo(category.CategorySysNo, manufacturerEntity.ManufacturerSysNo, AppConfig.CompanyCode);

                    if (path != null)
                    {
                        BaiduPlatformCategoryManufacturerEntity categoryBrandEntity = new BaiduPlatformCategoryManufacturerEntity(category.CategorySysNo, category.CategoryName,
                               path.PathSegment, path.ManufacturerSegment, manufacturerEntity.ManufacturerName, productCountEntity.ProductCount, productCountEntity.MinPrice);

                        this.CategoryManufacturerEntityList.Add(categoryBrandEntity);
                    }
                }
            }
        }
    }
}
