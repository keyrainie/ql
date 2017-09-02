using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess;
using System.Data;
using System.Xml;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using System.IO;

namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 商品库存
    /// </summary>
    [VersionExport(typeof(ProductInventoryProcessor))]
    public class ProductInventoryProcessor
    {
        private IProductInventoryDA productInventoryDA = ObjectFactory<IProductInventoryDA>.Instance;
        private IProductCostQueryDA ProductCostQueryDA = ObjectFactory<IProductCostQueryDA>.Instance;
        private IInventoryAdjustDA InventoryAdjustDA = ObjectFactory<IInventoryAdjustDA>.Instance;
        private IStockDA stockDA = ObjectFactory<IStockDA>.Instance;

        /// <summary>
        /// 初始化商品库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param> 
        /// <param name="webChannelID">销售渠道编号</param> 
        /// <returns></returns>
        public virtual void InitProductInventoryInfo(int productSysNo, string webChannelID)
        {
            List<StockInfo> stockList = ObjectFactory<StockProcessor>.Instance.GetStockList(webChannelID);
            foreach (StockInfo stock in stockList)
            {
                productInventoryDA.InitProductInventoryInfo(productSysNo, (int)stock.SysNo);
            }
        }

        #region 商品库存更新

        /// <summary>
        /// 设置商品分仓库预留库存
        /// </summary>
        /// <param name="ProductID">商品编号</param>
        /// <param name="StockSysNo">渠道仓库编号</param>
        /// <param name="ReservedQty">预留库存</param>
        /// <returns></returns>
        public virtual void AdjustProductInventoryInfo(ProductInventoryInfo inventoryAdjustInfo)
        {
            productInventoryDA.AdjustProductStockInventoryInfo(inventoryAdjustInfo);
            productInventoryDA.AdjustProductTotalInventoryInfo(inventoryAdjustInfo);
        }

        /// <summary>
        /// 设置商品分仓库预留库存
        /// </summary>
        /// <param name="ProductID">商品编号</param>
        /// <param name="StockSysNo">渠道仓库编号</param>
        /// <param name="ReservedQty">预留库存</param>
        /// <returns></returns>
        public virtual void AdjustProductReservedQty(int productSysNo, int stockSysNo, int reservedQty)
        {
            //TODO: Call AdjustProductInventoryInfo MKT_CountDown/SetReservedQty
            ProductInventoryInfo inventoryAdjustInfo = new ProductInventoryInfo()
            {
                ProductSysNo = productSysNo,
                StockSysNo = stockSysNo,
                ReservedQty = reservedQty,
                AvailableQty = -reservedQty

            };

            AdjustProductInventoryInfo(inventoryAdjustInfo);

        }
        #endregion 商品库存更新

        #region 商品库存查询

        /// <summary>
        /// 取得商品各销售渠道的商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo)
        {
            List<ProductInventoryInfo> inventoryList = productInventoryDA.GetProductInventoryInfo(productSysNo);
            List<StockInfo> stocks = stockDA.QueryStockAll();
            foreach (ProductInventoryInfo inventory in inventoryList)
            {
                inventory.StockInfo = stocks.Find(s => s.SysNo == inventory.StockSysNo);
            }

            return inventoryList;
        }

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productID">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public virtual ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            return productInventoryDA.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        /// 获取指定商品的总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            return productInventoryDA.GetProductTotalInventoryInfo(productSysNo);
        }

        /// <summary>
        ///  获取指定商品列表的总库存
        /// </summary>
        /// <param name="productIDList">商品编号列表</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count == 0)
            {
                return null;
            }

            return productInventoryDA.GetProductTotalInventoryInfoByProductList(productSysNoList);

        }

        #endregion 商品库存查询

        #region 销售数量
        /// <summary>
        ///  获取指定商品的渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓编号</param>
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoByStock(int productSysNo, int stockSysNo)
        {
            return productInventoryDA.GetProductSalesTrendInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        ///  获取指定商品的总销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int productSysNo)
        {
            return productInventoryDA.GetProductSalesTrendInfoTotal(productSysNo);
        }

        /// <summary>
        ///  获取指定商品的各渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo)
        {
            return productInventoryDA.GetProductSalesTrendInfo(productSysNo);
        }
        #endregion 销售数量

        #region 库存成本
        /// <summary>
        /// 批量设置库存成本序列优先级
        /// </summary>
        /// <param name="list"></param>
        public virtual void UpdateProductCostPriority(List<ProductCostInfo> list)
        {
             productInventoryDA.UpdateProductCostPriority(list);
        }

        /// <summary>
        /// 成本变价单更新库存成本
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        public virtual void UpdateCostInWhenCostChange(CostChangeInfo costChangeInfo)
        {
            //预校验
            if (!PreCheck(costChangeInfo))
            {
                throw new BizException("存在有效库存成本数量不够变价数量的商品，无法审核通过！");
            }

            TransactionOptions option = new TransactionOptions();
            //option.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, option))
            {
                string msg = SerializationUtility.XmlSerialize(costChangeInfo);
                productInventoryDA.WriteCostLog((int)ECCentral.BizEntity.Inventory.InventoryAdjustContractInfo.CostBillType.CostAdjust, costChangeInfo.SysNo.Value, msg);

                List<ProductCostIn> ProductCostInList = new List<ProductCostIn>();
                List<ProductCostOut> ProductCostOutList = new List<ProductCostOut>();
                ProductCostIn costIn; 
                ProductCostOut costout;
                foreach (CostChangeItemsInfo item in costChangeInfo.CostChangeItems)
                {
                    costout = new ProductCostOut
                    {
                        Quantity = item.ChangeCount,
                        Cost = item.OldPrice,
                        BillSysNo = item.POSysNo,
                        BillType = (int)ECCentral.BizEntity.Inventory.InventoryAdjustContractInfo.CostBillType.PO,
                        ProductSysNo = item.ProductSysNo
                    };
                    ProductCostOutList.Add(costout);
                    if (item.ChangeCount > 0)   //库存溢出表示为入库
                    {
                        costIn = new ProductCostIn();
                        costIn.BillType = (int)ECCentral.BizEntity.Inventory.InventoryAdjustContractInfo.CostBillType.CostAdjust;
                        costIn.BillSysNo = costChangeInfo.SysNo.Value;
                        costIn.Quantity = item.ChangeCount;
                        costIn.LeftQuantity = item.ChangeCount;
                        costIn.LockQuantity = 0;
                        costIn.ProductSysNo = item.ProductSysNo;
                        costIn.Cost = item.NewPrice;
                        ProductCostInList.Add(costIn);
                    }
                }

                //先出
                productInventoryDA.UpdateProductCostForCostChange(ProductCostOutList);
                //后进
                productInventoryDA.WriteProductCost(ProductCostInList);

                tran.Complete();
            }        
        }

        /// <summary>
        /// 预校验库存成本变更
        /// </summary>
        /// <param name="costChangeInfo"></param>
        private bool PreCheck(CostChangeInfo costChangeInfo)
        {
            return ProductCostQueryDA.PreCheckCostChange(costChangeInfo.SysNo.Value);
        }
        #endregion

        /// <summary>
        /// 获取商品当前库龄
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public int GetInStockDaysByProductSysNo(int productSysNo)
        {
            return productInventoryDA.GetInStockDaysByProductSysNo(productSysNo);
        }

        public List<InventoryBatchDetailsInfo> GetProdcutBatchesInfo(string type, int number, int productSysNo)
        {
            return productInventoryDA.GetProdcutBatchesInfo(type, number, productSysNo);
        }

        public void GetProductRingAndSendEmail()
        {
            List<ProductRingDetailInfo> ringDetails = productInventoryDA.GetProductRingDetails();
            if (ringDetails != null && ringDetails.Count > 0)
            {
                var brandAndC3List = from p in ringDetails
                                     group p by new
                                     {
                                         p.BrandSysNo,
                                         p.C3SysNo
                                     }
                                         into g
                                         select new
                                         {
                                             g.Key,
                                             g
                                         };



                foreach (var item in brandAndC3List)
                {
                    var temp = ringDetails.Where(p =>
                    {
                        return item.g.Key.C3SysNo == p.C3SysNo && item.g.Key.BrandSysNo == p.BrandSysNo;
                    });
                    SendProductRingEmail("Inventory_ProductRing", temp.ToList<ProductRingDetailInfo>());
                }
            }

        }

        public void SendProductRingEmail(string emailTemplate, List<ProductRingDetailInfo> detailList)
        {
            if (null != detailList&&detailList.Count > 0)
            {
                DataTable dtProduct = new DataTable();
                dtProduct.Columns.AddRange(new DataColumn[]
                {
                    new DataColumn("ProductID"),
                    new DataColumn("ProductName"),
                    new DataColumn("BatchNumber"),
                    new DataColumn("ActualQty"),
                    new DataColumn("LeftRingDays"),
                });

                KeyTableVariables keyTablesVariables = new KeyTableVariables();
                KeyValueVariables keyValueVariables = new KeyValueVariables();

                keyValueVariables.Add("Receiver", detailList[0].Email);

                detailList.ForEach(p => 
                {
                    DataRow dr = dtProduct.NewRow();
                    dr["ProductID"] = p.ProductID;
                    dr["ProductName"] = p.ProductName;
                    dr["BatchNumber"] = p.BatchNumber;
                    dr["ActualQty"] = p.ActualQty;
                    dr["LeftRingDays"] = p.LeftRingDays;

                    dtProduct.Rows.Add(dr);
                });

                keyTablesVariables.Add("ProductList", dtProduct);

                ExternalDomainBroker.SendInternalEmail(detailList[0].Email.Trim(),emailTemplate, keyValueVariables, keyTablesVariables);
                //ExternalDomainBroker.SendExternalEmail(detailList[0].Email, emailTemplate, keyValueVariables, keyTablesVariables,"zh-CN");
            }
        }

        /// <summary>
        /// 发送月底库存邮件
        /// </summary>
        /// <param name="emailList">发送地址列表</param>
        /// <param name="title">标题可以不填</param>
        /// <param name="content">发送内容</param>
        /// <returns>返回成功发送列表</returns>
        public void SendInventoryEmailEndOfMonth(string address, string language, string downloadPath, string savePath)
        {
            DataTable dt = ObjectFactory<IInventoryQueryDA>.Instance.QueryInventoryListEndOfMouth();

            KeyTableVariables keyTablesVariables = new KeyTableVariables();
            KeyValueVariables keyValueVariables = new KeyValueVariables();
            List<DataTable> data = new List<DataTable>();
            data.Add(dt);
            //keyTablesVariables.Add("InventoryList", dt);
            List<ColumnData> columndatalist = new List<ColumnData> ();
            ColumnData columndata = null;
            columndata = new ColumnData() { FieldName = "StockName", FooterType = FooterType.None, Width = 30, Title = "渠道仓库" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductID", FooterType = FooterType.None, Width = 30, Title = "商品ID" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ProductName", FooterType = FooterType.None, Width = 30, Title = "商品名称" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "VendorName", FooterType = FooterType.None, Width = 30, Title = "供应商" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "AccountQty", FooterType = FooterType.None, Width = 30, Title = "财务库存" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "CostAmount", FooterType = FooterType.None, Width = 30, Title = "成本金额" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "AvailableQty", FooterType = FooterType.None, Width = 30, Title = "可用库存" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "AllocatedQty", FooterType = FooterType.None, Width = 30, Title = "被占用库存" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "OrderQty", FooterType = FooterType.None, Width = 30, Title = "被订购数量" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "VirtualQty", FooterType = FooterType.None, Width = 30, Title = "虚库数量" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ConsignQty", FooterType = FooterType.None, Width = 30, Title = "代销库存" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "PurchaseQty", FooterType = FooterType.None, Width = 30, Title = "采购在途数量" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ShiftInQty", FooterType = FooterType.None, Width = 30, Title = "移入在途数量" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "ShiftOutQty", FooterType = FooterType.None, Width = 30, Title = "移出在途数量" }; columndatalist.Add(columndata);
            columndata = new ColumnData() { FieldName = "PositionInWarehouse", FooterType = FooterType.None, Width = 30, Title = "库位" }; columndatalist.Add(columndata);

            List<List<ColumnData>> columnList = new List<List<ColumnData>>();
            columnList.Add(columndatalist);

            string fileName = "";
            byte[] excelByte = new ExcelFileExporter().CreateFile(data, columnList, null,out fileName, "月底库存报表");

            string excelName = System.Guid.NewGuid().ToString();
            FileStream fs = new FileStream(String.Format("{0}//{1}.xls", savePath, excelName), FileMode.Create, FileAccess.Write);
            
            fs.Write(excelByte, 0, excelByte.Length);
            fs.Close();

            

            keyValueVariables.Add("Titel", String.Format("月底库存报表，总数{0}条", dt.Rows.Count));
            keyValueVariables.Add("Content", String.Format("库存数据已生成excel文件，请点击<a href=\"{0}/{1}.xls\">下载</a>", downloadPath, excelName));

            ExternalDomainBroker.SendExternalEmail(address, "Inventory_EndOfMonth", keyValueVariables, keyTablesVariables, language);
        }

        /// <summary>
        /// 修改批次信息状态
        /// </summary>
        /// <param name="productBatchInfo"></param>
        public virtual void UpdateProductBatchStatus(InventoryBatchDetailsInfo productBatchInfo)
        {
            List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
            ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
            itemBatchInfo.BatchNumber = productBatchInfo.BatchNumber;
            itemBatchInfo.ProductNumber = productBatchInfo.ProductSysNo.ToString();
            itemBatchInfo.Status = productBatchInfo.Status;
            itemBatchInfoList.Add(itemBatchInfo);

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
                    From = "IPP",
                    GlobalBusinessType = "Listing",
                    StoreCompanyCode = "8601",
                    TransactionCode = ""
                },
                Body = new InventoryBody()
                {
                    Number = "",
                    InUser = ServiceContext.Current.UserDisplayName,
                    ItemBatchInfo = itemBatchInfoList
                }
            };
            string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
            XmlDocument xmlD = new XmlDocument();
            xmlD.LoadXml(paramXml);
            paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

            //给仓库发消息，调整批次信息
            InventoryAdjustDA.AdjustBatchNumberInventory(paramXml);

            //SendSSBToWMS(query, InUser);

        }

    }
}
