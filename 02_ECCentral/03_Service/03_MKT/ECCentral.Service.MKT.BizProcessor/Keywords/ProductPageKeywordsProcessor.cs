using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using System.Collections;
using ECCentral.BizEntity.Common;
using System.Transactions;
using System.IO;
using System.Data;
using System.Threading;
using ECCentral.Service.IBizInteract;


namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductPageKeywordsProcessor))]
    public class ProductPageKeywordsProcessor
    {

        private IProductPageKeywordsDA keywordDA = ObjectFactory<IProductPageKeywordsDA>.Instance;

        #region 产品页面关键字（ProductPageKeywords）
        #region  JOB
        
        /// <summary>
        /// JOB更新产品页面关键字
        /// </summary>
        public virtual void BatchUpdateKeywordsFromQueue(string companyCode)
        {
            //string companyCode = "8601";
            int index = 0;
            // 从Queue中得到所有要更新的产品
            List<ProductKeywordsQueue> productIDs = keywordDA.GetProductIDsFromQueue(companyCode);
            BatchUpdateProductPageKeywords(companyCode, productIDs, ref index);
        }


        /// <summary>
        /// JOB更新产品页面关键字
        /// </summary>
        public virtual void BatchUpdateKeywordsForKeywords0(string companyCode)
        {
            List<ProductKeywordsQueue> categoryList = keywordDA.GetAllC3Categories(companyCode);//得到所以categroy3
            var index = 0; 
            
            //遍历所有categroy每次拿出一个categoryId 找到其下所有product 并执行更新方法
            foreach (ProductKeywordsQueue catgory in categoryList)
            {
                List<ProductKeywordsQueue> productList = keywordDA.GetProductByC3SysNo(companyCode,catgory.C3SysNo);

                //WriteLog("正在更新S3SysNo为{0}的Product..", catgory.C3SysNo);
                BatchUpdateProductPageKeywords(companyCode,productList, ref index);
                //WriteLog("更新S3SysNo为{0}的Product 成功!", catgory.C3SysNo);
            }          
        }

        public virtual void BatchUpdateKeywordsFromProduct(ECCentral.BizEntity.IM.ProductInfo product)
        {
            ProductKeywordsQueue productInfo = keywordDA.GetSingleProduct(product.CompanyCode,product.SysNo);
            int index = 0;
            if (productInfo == null)
                return;
                //WriteLog("没有找到Product SysNo 为{0}的数据", productId);
            //WriteLog("正在更新的Product SysNo为{0}的keywords", productId);
            BatchUpdateProductPageKeywords(product.CompanyCode, new List<ProductKeywordsQueue> { productInfo }, ref index);
        }

        public virtual void BatchUpdateProductPageKeywords(string companyCode, List<ProductKeywordsQueue> productIDs, ref int index)
        {
            if (productIDs.Count() > 0)
            {
                //得到所以产品的cageoryId
                var categoryIds = from e in productIDs
                                  where e.C3SysNo.HasValue
                                  group e by e.C3SysNo into g
                                  select g.Key.Value;

                List<CategoryKeywords> cateList = keywordDA.GetCategroyKeywordsForBatchUpdate(companyCode, categoryIds.Join(", "));
                var dictCaregroyKeywords = (from e in cateList group e by e.Category3SysNo).ToDictionary(e => e.Key.Value, e => e.First());
                for (int i = 0; i < productIDs.Count; i++)
                {
                    //执行每2000条停5秒
                    if (++index % 2000 == 0)
                    {
                        //WriteLog("更新条数超过2000条,等待中...!");
                        Thread.Sleep(5 * 1000);
                    }
                    //如果执行失败不中断任务
                    try
                    {
                        var product = productIDs[i];
                        var keywords0 = string.Empty;
                        //如果Product在CategoryKeywords中没有, 更新keywords0为空字符串
                        if (dictCaregroyKeywords.ContainsKey(product.C3SysNo.Value))
                        {
                            var category = dictCaregroyKeywords[product.C3SysNo.Value];
                            //keywords0 = keywordDA.GetProductKeywords0(category, product);

                            //因为要不重复, 所以使用HashSet
                            var distinctedKeywordsList = new HashSet<string>();
                            //如果公共关键字不为空
                            if (!string.IsNullOrEmpty(category.CommonKeywords.Content))
                                distinctedKeywordsList.AddRange(category.CommonKeywords.Content.Split(' '));

                            //得到属性关键字
                            List<PropertyInfo> propertyInfoList = ObjectFactory<ICategoryKeywordsDA>.Instance.GetPropertyInfo(companyCode, product.ProductSysNo.Value, product.C3SysNo.Value);
                            foreach (var propertyInfo in propertyInfoList)
                            {
                                var keywords = string.IsNullOrEmpty(propertyInfo.ManualInput) ? propertyInfo.ValueDescription : propertyInfo.ManualInput;
                                keywords = (keywords ?? string.Empty).Trim();

                                if (!string.IsNullOrEmpty(keywords))
                                    distinctedKeywordsList.AddRange(keywords.Split(' '));

                            }
                            keywords0 = distinctedKeywordsList.Join(" ");
                        }

                        keywordDA.UpdateKeyWords0ByProductSysNo(companyCode, product.ProductSysNo, keywords0);
                        if (!string.IsNullOrEmpty(keywords0))
                            //Console.WriteLine("成功更新SysNo为{0} 的产品 Keywords0 到 '{1}'! ", product.ProductSysNo, keywords0);
                            Console.WriteLine(ResouceManager.GetMessageString("MKT.Keywords","Keywords_UpdateSucess"), product.ProductSysNo, keywords0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_UpdateFailed") + ex.Message);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 加载产品页面关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        //public virtual ProductPageKeywords LoadProductPageKeywords(int sysNo)
        //{
        //    return keywordDA.LoadProductPageKeywords(sysNo);
        //}

        /// <summary>
        /// 获取页面关键字编辑人员列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<UserInfo> GetProductKeywordsEditUserList(string companyCode)
        {
            return keywordDA.GetProductKeywordsEditUserList(companyCode);
        }

        /// <summary>
        /// 更新产品Keywords0
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateProductPageKeywords(ProductPageKeywords item)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                keywordDA.UpdateProductPageKeywords(item);
                /*2012-12-31 update bug95058  jack.g.tang
                 * 修改目的：ProductSysNo可能为null
                 * 解决办法：为null时 根据productId取得ProducuSysNo
                 */
                 int? productSysNo=item.ProductSysNo;
                if (item.ProductSysNo == null || item.ProductSysNo == 0)
                {
                     productSysNo = ExternalDomainBroker.GetProductInfo(item.ProductId).SysNo;
                }
                ExternalDomainBroker.UpdateProductExKeyKeywords(
                        (int)productSysNo , item.Keywords.ToString()
                          , item.Keywords0.ToString(), ServiceContext.Current.UserSysNo, item.CompanyCode);
                scope.Complete();
            }
            
        }

        /// <summary>
        /// 删除或添加产品页面关键字
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="batchAdd"></param>
        /// <param name="key0"></param>
        /// <param name="key1"></param>
        /// <param name="languageCode"></param>
        /// <param name="companyCode"></param>
        public virtual void BatchUpdateProductPageKeywords(List<string> productList, bool batchAdd, string key0, string key1, string languageCode, string companyCode)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (string productSysNo in productList)
                {
                    string[] strList = new string[3];
                    //先拆分
                    strList[0] = productSysNo.Split(',')[0].Trim();
                    strList[1] = productSysNo.Split(',')[1].Trim();
                    strList[2] = productSysNo.Split(',')[2].Trim();

                    //判断是更新k1还是k0
                    if (key1 != null)
                    {
                        strList[1] = SetKeywordsvValue(strList[1], batchAdd, key1.Trim());
                        if (String.IsNullOrEmpty(strList[0]) || strList[1].Length > 400)
                            continue;
                    }

                    if (key0 != null)
                    {
                        strList[2] = SetKeywordsvValue(strList[2], batchAdd, key0.Trim());


                        if (String.IsNullOrEmpty(strList[0]) || strList[2].Length > 200)
                            continue;
                    }
                    ProductPageKeywords keywords = new ProductPageKeywords();
                    keywords.Keywords = new LanguageContent(languageCode, strList[1].ToString());
                    keywords.Keywords0 = new LanguageContent(languageCode, strList[2].ToString());
                    keywords.ProductSysNo = int.Parse(strList[0].ToString());
                    keywords.CompanyCode = companyCode;
                    keywordDA.UpdateProductPageKeywords(keywords);
                    ExternalDomainBroker.UpdateProductExKeyKeywords(int.Parse(keywords.ProductSysNo.ToString()), keywords.Keywords.ToString()
                        , keywords.Keywords0.ToString(), ServiceContext.Current.UserSysNo, keywords.CompanyCode);

                }
                scope.Complete();
            }
        }

        private static string SetKeywordsvValue(string kewords1, bool Type, string replKeywords)
        {
            string value = kewords1;

            if (Type)
            {
                string[] kewordsList = kewords1.Split(' ');
                string[] replKeywordsList = replKeywords.Split(' ');
                ArrayList arList = new ArrayList();
                arList.AddRange(kewordsList);

                foreach (string strKey in replKeywordsList)
                {
                    if (String.IsNullOrEmpty(strKey))
                        continue;
                    if (!arList.Contains(strKey))
                    {
                        arList.Add(strKey);
                        value = value + " " + strKey;
                    }
                }
            }
            else
            {
                value = string.Empty;
                if (!String.IsNullOrEmpty(kewords1))
                {
                    string[] kewordsList = kewords1.Split(' ');
                    string[] replKeywordsList = replKeywords.Split(' ');
                    ArrayList arList = new ArrayList();
                    arList.AddRange(kewordsList);

                    foreach (string keywords in arList)
                    {
                        if (String.IsNullOrEmpty(keywords))
                            continue;
                        bool rflag = true;
                        foreach (string strKey in replKeywordsList)
                        {
                            if (String.IsNullOrEmpty(strKey))
                                continue;

                            if (keywords == strKey)
                            {
                                rflag = false;
                                break;
                            }
                        }
                        if (rflag)
                            value += keywords + " ";
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 批量添加产品页面关键字
        /// </summary>
        /// <param name="item"></param>
        //public virtual void AddProductPageKeywords(ProductPageKeywords item)
        //{
        //    keywordDA.AddProductPageKeywords(item);
        //}

        /// <summary>
        /// 上传批量添加产品页面关键字
        /// </summary>
        /// <param name="uploadFileInfo"></param>
        public virtual void BatchImportProductKeywords(string uploadFileInfo)
        {
            if (FileUploadManager.FileExists(uploadFileInfo))
            {
             
                string configPath = AppSettingManager.GetSetting("MKT", "PostProductKeywordsFilePath");
                if (!Path.IsPathRooted(configPath))
                {
                    configPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configPath);
                }
                string ExtensionName = FileUploadManager.GetFilePhysicalFullPath(uploadFileInfo);
                string destinationPath = Path.Combine(configPath, ExtensionName);
                string folder = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                FileUploadManager.MoveFile(uploadFileInfo, destinationPath);

               
                    DataTable table = keywordDA.ReadExcelFileToDataTable(destinationPath);
                    if (table != null && table.Rows != null && table.Rows.Count > 0)
                    {
                        if (table.Columns[0].ColumnName == "Item No#")
                        {
                            string ProductID = string.Empty;
                            List<string> ProductList = new List<string>();
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                if (table.Rows[i]["Item No#"] != DBNull.Value
                                    && !string.IsNullOrEmpty(table.Rows[i]["Item No#"].ToString()))
                                {
                                    ProductID = table.Rows[i]["Item No#"] == null ? string.Empty : table.Rows[i]["Item No#"].ToString().Trim();

                                    if (!ProductList.Contains(ProductID))
                                    {
                                        ProductList.Add(ProductID);
                                    }
                                }
                            }
                            int count = string.IsNullOrEmpty(AppSettingManager.GetSetting("MKT", "ProductKeywordsExcelBatchCount")) ? 100 : int.Parse(AppSettingManager.GetSetting("MKT", "ProductKeywordsExcelBatchCount"));
                            if (ProductList.Count > count)
                                //throw new BizException(string.Format("导入的条数超过限制，最大{0}条！", count.ToString()));
                                throw new BizException(string.Format(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_LimitCount"), count.ToString()));
                            else if (ProductList.Count == 0)
                                //throw new BizException("导入的Excel无有效数据，导入失败！");
                                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_HasNotActiveData"));
                            else
                            {
                                foreach (string id in ProductList)
                                {
                                    //[Mark][Alan.X.Luo 硬编码]
                                    keywordDA.InsertProductKeywordsListBatch(id, "8601");
                                }
                            }
                        }
                        else
                            //throw new BizException("导入的Excel文件列名无效！");
                            throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_HasNotActiveData"));
                    }
                    else 
                    {
                        //throw new BizException("Execl中没有数据,或者工作簿名称不是Sheet1或者格式不正确！");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_HasNotActiveData"));
                    }
                }
        }

        #endregion

    }
}
