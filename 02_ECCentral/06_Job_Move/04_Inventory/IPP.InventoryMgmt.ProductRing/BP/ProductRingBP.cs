using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECCentral.BizEntity.Inventory;
using ECCentral.Job.Inventory.ProductRing.Components;
using ECCentral.Job.Inventory.ProductRing.DAL;
using ECCentral.Job.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.Utilities;
using System.Xml;
using ECCentral.Job.Inventory.ProductRing.Model;

namespace ECCentral.Job.Inventory.ProductRing.BP
{
    public static class ProductRingBP
    {
        #region Field

        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);

        #endregion

        public static void DoWork(JobContext context)
        {
            try
            {
                OnShowInfo("开始更新批次商品状态...");
                UpdateProductBatchStatus();
                OnShowInfo("更新批次商品状态结束！");
                OnShowInfo("开始执行临期商品报警...");
                CallProductRingService();
                OnShowInfo("临期商品报警结束！");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新批次商品状态
        /// </summary>
        private static void UpdateProductBatchStatus()
        {
            List<ProductBatchInfo> lstProductBatchInfo = ProductRingDA.QueryProductBatchModified();  //需要修改批次状态的数据
            var productBatchInfoGroup = lstProductBatchInfo.GroupBy(p => new { p.ProductSysNo});

            List<ProductBatchInfo> lstProductBatchGroup;
            List<ItemBatchInfo> itemBatchInfoList;
            ItemBatchInfo itemBatchInfo;
            foreach (var itemGroup in productBatchInfoGroup)
            {
                lstProductBatchGroup = itemGroup.ToList();
                itemBatchInfoList = new List<ItemBatchInfo>();
                foreach(ProductBatchInfo item in lstProductBatchGroup)
                {
                    itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();
                    itemBatchInfo.Status = item.NewStatus;
                    itemBatchInfoList.Add(itemBatchInfo);
                }

                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new InventoryHeader()
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = "Status",
                        Version = "V10",
                        Type = "Update",
                        CompanyCode = "8601",
                        Tag = "UpdateStatus",
                        Language = "zh-CN",
                        From = "Job",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = ""
                    },
                    Body = new InventoryBody()
                    {
                        Number = "",
                        InUser = "Job",
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                //给仓库发消息，调整批次信息
                ProductRingDA.UpdateBatchInfo(paramXml);
            }
        }

        /// <summary>
        /// 临期商品报警
        /// </summary>
        private static void CallProductRingService()
        {
            RestClient client;
            RestServiceError error;
            client = new RestClient(Settings.RestFulBaseUrl, Settings.LanguageCode);
            client.Create("/Inventory/Job/ProductRing", "", out error);
            var messageBuilder = new StringBuilder();
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                foreach (var errorItem in error.Faults)
                {
                    messageBuilder.AppendFormat(" {0} <br/>", errorItem.ErrorDescription);
                }

                throw new Exception(messageBuilder.ToString());
            }
        }

        public static void OnShowInfo(string info)
        {
            mut.WaitOne();
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            mut.ReleaseMutex();
        }
    }
}
