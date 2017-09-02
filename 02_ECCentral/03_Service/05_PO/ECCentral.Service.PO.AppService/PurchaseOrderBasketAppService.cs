using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using System.IO;
using System.Data;
using ECCentral.Service.IBizInteract;
using System.Data.OleDb;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(PurchaseOrderBasketAppService))]
    public class PurchaseOrderBasketAppService
    {
        #region [Fields]
        private PurchaseOrderBasketProcessor m_PurchaseOrderBasketProcessor;
        private IInventoryBizInteract m_InventoryBizInteract;

        public PurchaseOrderBasketProcessor PurchaseOrderBasketProcessor
        {
            get
            {
                if (null == m_PurchaseOrderBasketProcessor)
                {
                    m_PurchaseOrderBasketProcessor = ObjectFactory<PurchaseOrderBasketProcessor>.Instance;
                }
                return m_PurchaseOrderBasketProcessor;
            }
        }

        public IInventoryBizInteract InventoryBizInteract
        {
            get
            {
                if (null == m_InventoryBizInteract)
                {
                    m_InventoryBizInteract = ObjectFactory<IInventoryBizInteract>.Instance;
                }
                return m_InventoryBizInteract;
            }
        }
        #endregion


        public BatchCreateBasketResultInfo BatchCreatePurchaseOrder(List<BasketItemsInfo> list)
        {
            return PurchaseOrderBasketProcessor.BatchCreatePurchaseOrder(list);
        }

        public BasketItemsInfo LoadBasketItem(int? itemSysNo)
        {
            return PurchaseOrderBasketProcessor.LoadBasketItemInfoBySysNo(itemSysNo);
        }

        public virtual void BatchCreateGiftForBasket(List<BasketItemsInfo> list)
        {
            PurchaseOrderBasketProcessor.BatchAddGift(list);
        }

        public virtual void BatchUpdateBasketItems(List<BasketItemsInfo> basketList)
        {
            PurchaseOrderBasketProcessor.BatchUpdateBasketItems(basketList);
        }

        public virtual void BatchDeleteBasketItems(List<BasketItemsInfo> basketList)
        {
            PurchaseOrderBasketProcessor.BatchDeleteBasketItems(basketList);
        }

        public void ConvertBasketTemplateFileToEntityList(string fileIdentity, out int successCount, out int failedCount, out string errorMsg,out List<BasketItemsInfo> failedList)
        {

            //1.移到一个新的文件夹中:
            string getConfigPath = AppSettingManager.GetSetting("PO", "VendorAttachmentFilesPath");
            if (!Path.IsPathRooted(getConfigPath))
            {
                //是相对路径:
                getConfigPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, getConfigPath);
            }
            string fileName = Guid.NewGuid().ToString() + FileUploadManager.GetFileExtensionName(fileIdentity);
            string getDestinationPath = Path.Combine(getConfigPath, fileName);
            string getFolder = Path.GetDirectoryName(getDestinationPath);
            if (!Directory.Exists(getFolder))
            {
                Directory.CreateDirectory(getFolder);
            }
            //将上传的文件从临时文件夹剪切到目标文件夹:
            FileUploadManager.MoveFile(fileIdentity, getDestinationPath);

            //2.解析Excel:
            DataTable dt = new DataTable();
            int sCount = 0;
            int fCount = 0;
            string returnErrorMessage = string.Empty;

            List<string> stockNames = new List<string>();


            //获取仓库信息:
            List<string> validStockSysNo = new List<string>();
            var configString = AppSettingManager.GetSetting("PO", "ValidStockSysNo");

            if (!string.IsNullOrEmpty(configString))
            {
                validStockSysNo = configString.Split(',').ToList();
            }

            validStockSysNo.ForEach(x =>
            {
                WarehouseInfo winfo = InventoryBizInteract.GetWarehouseInfoBySysNo(Convert.ToInt32(x));

                if (winfo != null)
                    stockNames.Add(winfo.WarehouseName);
            });

            var connectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", Path.GetFullPath(getDestinationPath));

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("select * from [sheet1$]", conn);

                    OleDbDataAdapter adp = new OleDbDataAdapter(cmd);

                    adp.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new BizException("文件格式不正确！" + "\n" + ex.Message);
                }

            }

            if (dt.Rows.Count == 0)
            {
                throw new BizException("没有可导入的数据");
            }
            else if (dt.Rows.Count > 1000)
            {
                throw new BizException("商品信息超过1000条，请将文件拆分后重新进行上传！");
            }

            List<BasketItemsInfo> importList = new List<BasketItemsInfo>(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                int vendorSysNo = -1;
                int quantity = -1;
                decimal orderPrice = -1;

                int.TryParse(row[0].ToString().Trim(), out vendorSysNo);
                int.TryParse(row[2].ToString().Trim(), out quantity);
                decimal.TryParse(row[3].ToString().Trim(), out orderPrice);

                BasketItemsInfo importModel = new BasketItemsInfo
                {
                    VendorSysNo = vendorSysNo,
                    ProductID = row[1].ToString().Trim(),
                    Quantity = quantity,
                    OrderPrice = orderPrice,
                    StockName = row[4].ToString().Trim(),
                    IsTransfer = row[5].ToString().Trim() != "是" && row[5].ToString().Trim() != "否" ? (int?)null : (row[5].ToString().Trim() == "是" ? 1 : 0)
                };

                importList.Add(importModel);
            }

            var query = from g in
                            (from item in importList
                             group item by new { item.VendorSysNo, item.ProductID, item.StockName }
                                 into importGroup
                                 select importGroup)
                        where g.Count() > 1
                        select g;

            if (query.Count() > 0)
            {
                var message = new StringBuilder("上传文件中存在重复记录:" + Environment.NewLine);

                foreach (var g in query)
                {
                    message.AppendFormat("供应商编号:{0}  商品编号:{1}  目标分仓:{2}{3}", g.Key.VendorSysNo, g.Key.ProductID, g.Key.StockName, Environment.NewLine);
                }

                throw new BizException(message.ToString());
            }

            List<BasketItemsInfo> messageList = new List<BasketItemsInfo>();
            failedList = new List<BasketItemsInfo>();

            foreach (var item in importList)
            {
                try
                {
                    BasketItemsInfo basketItem = new BasketItemsInfo();

                    if (item.VendorSysNo > 0)
                    {
                        int vendorSysNo;

                        if (Int32.TryParse(item.VendorSysNo.ToString(), out vendorSysNo))
                        {
                            basketItem.LastVendorSysNo = vendorSysNo;
                        }
                        else
                        {
                            throw new BizException("【供应商编号】有非法的值,必须是有效的供应商");
                        }
                    }
                    else
                    {
                        throw new BizException("【供应商编号】有非法的值,必须是有效的供应商");
                    }

                    if (string.IsNullOrEmpty(item.ProductID))
                    {
                        throw new BizException("【商品编号】有非法的值,必须是有效的商品编号");
                    }
                    else
                    {
                        basketItem.ProductID = item.ProductID;
                    }

                    if (item.OrderPrice < 0)
                    {
                        throw new BizException("【采购价格】不能为空");
                    }
                    else
                    {
                        basketItem.OrderPrice = Convert.ToDecimal(item.OrderPrice);
                    }

                    if (!item.IsTransfer.HasValue)
                    {
                        throw new BizException("【是否中转】有非法的值,必须为'是'或者'否'");
                    }
                    else
                    {
                        basketItem.IsTransfer = item.IsTransfer;
                    }

                    if (item.Quantity < 0)
                    {
                        throw new BizException("【数量】有非法的值,必须输入大于零的整数");
                    }
                    else
                    {
                        basketItem.Quantity = item.Quantity;
                    }

                    if (item.OrderPrice >= 0)
                    {
                        decimal orderPrice;

                        if (decimal.TryParse(item.OrderPrice.ToString(), out orderPrice))
                        {
                            if (orderPrice > 0)
                            {
                                basketItem.OrderPrice = orderPrice;
                            }
                            else
                            {
                                throw new BizException("【订购价格】有非法的值,必须为大于零的数");
                            }
                        }
                        else
                        {
                            throw new BizException("【订购价格】有非法的值,必须为大于零的数");
                        }
                    }

                    if (string.IsNullOrEmpty(item.StockName) ||
                        !stockNames.Contains(item.StockName))
                    {
                        throw new BizException(string.Format("【目标分仓】有非法的值,必须为:{0}", String.Join("、", stockNames.ToArray())));
                    }
                    else
                    {
                        basketItem.StockName = item.StockName;
                    }

                    messageList.Add(basketItem);
                }
                catch (BizException ex)
                {
                    item.ErrorMessage = ex.Message;
                    failedList.Add(item);
                }
                catch (Exception)
                {
                    //TODO:定message
                    item.ErrorMessage = "验证数据时出现未知错误";
                }
            }

            //调用服务,写入到采购篮中:
            List<BasketItemsInfo> returnItemsList = new List<BasketItemsInfo>();
            if (messageList.Count > 0)
            {
                returnItemsList = PurchaseOrderBasketProcessor.BatchImportAndCreateBasketItem(messageList, false);
                //构建失败列表并返回:
                for (int i = 0; i < returnItemsList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(returnItemsList[i].ErrorMessage))
                    {
                        fCount++;
                        failedList.Add(returnItemsList[i]);
                    }
                    else
                        sCount++;
                };
            }

            for (int i = 0; i < failedList.Count; i++)
            {
                returnErrorMessage += (i + 1) + ":" + failedList[i].ErrorMessage.ToString() + Environment.NewLine;
            }

            successCount = sCount;
            failedCount = failedList.Count;
            errorMsg = returnErrorMessage;
        }

        public List<WarehouseInfo> QueryBasketTargetWarehouseList(string companyCode)
        {
            return InventoryBizInteract.GetWarehouseList(companyCode);
        }

        public virtual List<BasketItemsInfo> GetGiftBasketItems(List<int> productSysNoList)
        {
            return PurchaseOrderBasketProcessor.GetGiftBasketItems(productSysNoList);
        }
    }
}
