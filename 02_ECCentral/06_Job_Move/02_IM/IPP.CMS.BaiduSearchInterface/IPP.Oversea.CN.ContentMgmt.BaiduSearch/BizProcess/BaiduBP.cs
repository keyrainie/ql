using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using IPP.Oversea.CN.ContentMgmt.Baidu.Entities;
using IPP.Oversea.CN.ContentMgmt.BaiduSearch.DataAccess;
using Newegg.Oversea.Framework.ExceptionBase;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.BizProcess
{
    public class BaiduBP
    {
        private string IndexFileName;
        private string ItemFileName;
        private int ItemCountPerFile;

        private static List<CategoryEntity> CategoryList { get; set; }
        private static List<BaiduCategoryEntity> BaiduCategoryList { get; set; }

        //public void Init()
        //{
        //    try
        //    {
        //        IndexFileName = AppConfig.IndexFileName;
        //        ItemFileName = AppConfig.ItemFileName;
        //        ItemCountPerFile = AppConfig.ItemCountPerFile;

        //        BaiduCategoryList = BaiduDA.GetBaiduCategoryList();
        //        CategoryList = BaiduDA.GetCategoryList(AppConfig.CompanyCode);
        //    }
        //    catch
        //    {
        //        throw new BusinessException(@"配置出错，请修改配置文件！");
        //    }
        //}

        public BaiduBP()
        {
            try
            {
                IndexFileName = AppConfig.IndexFileName;
                ItemFileName = AppConfig.ItemFileName;
                ItemCountPerFile = AppConfig.ItemCountPerFile;

                BaiduCategoryList = BaiduDA.GetBaiduCategoryList();
                CategoryList = BaiduDA.GetCategoryList(AppConfig.CompanyCode);
            }
            catch
            {
                throw new BusinessException(@"配置出错，请修改配置文件！");
            }
        }

        public void Process()
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Encoding = System.Text.Encoding.UTF8;

            GenerateIndexFile(IndexFileName, setting);
            GenerateItemFile(ItemFileName, setting);
        }

        private void GenerateIndexFile(string fileName, XmlWriterSettings setting)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fs, setting))
                {
                    xmlWriter.WriteStartElement("urlset");

                    foreach (CategoryEntity entity in CategoryList)
                    {
                        entity.WriteCategory(xmlWriter);
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        private void GenerateItemFile(string fileName, XmlWriterSettings setting)
        {
            List<ItemEntity> itemList = BaiduDA.GetProductList(AppConfig.CompanyCode);
            List<List<ItemEntity>> itemListArray = GetItemListArray(itemList);

            for (int i = 0; i < itemListArray.Count; i++)
            {
                GenerateEachItemFile(GetItemFileName(fileName, i), setting, itemListArray[i]);
            }
        }

        private static string GetItemFileName(string fileName, int i)
        {
            return fileName + (i + 1).ToString() + ".xml";
        }

        private List<List<ItemEntity>> GetItemListArray(List<ItemEntity> itemList)
        {
            List<List<ItemEntity>> result = new List<List<ItemEntity>>();

            for (int i = 0; i < Math.Ceiling((decimal)itemList.Count / ItemCountPerFile); i++)
            {
                result.Add(itemList.Skip(ItemCountPerFile * i).Take(ItemCountPerFile).ToList());
            }

            return result;
        }

        private void GenerateEachItemFile(string fileName, XmlWriterSettings setting, List<ItemEntity> itemList)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(fs, setting))
                {
                    xmlWriter.WriteStartElement("urlset");

                    foreach (ItemEntity entity in itemList)
                    {
                        entity.WriteItem(xmlWriter);
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        public static BaiduCategoryEntity GetBaiduCategory(int categorySysNo)
        {
            BaiduCategoryEntity category = BaiduCategoryList.FirstOrDefault(p => p.CategorySysNo == categorySysNo);
            
            if (category == null)
            {
                CategoryEntity categoryEntity = CategoryList.FirstOrDefault(p => p.CategoryId == categorySysNo);
            
                if (categoryEntity == null)
                {
                    return new BaiduCategoryEntity();
                }
                else
                {
                    category = new BaiduCategoryEntity() { CategoryName = categoryEntity.CategoryName };
                }
            }

            return category;
        }
    }
}