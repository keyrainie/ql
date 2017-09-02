using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(BatchInventoryProcessor))]
    public class BatchInventoryProcessor
    {
        public void AdjustBatchNumberInventory(string xml)
        {
            ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(xml);
        }

        public void UpdateAdjustQuantityOfSTB(List<InventoryBatchDetailsInfo> model, int inputDocumentNumber, string inputType, string inputAction)
        {
            if (model == null || model.Count == 0)
            {
                AbandonSTBInfo(inputDocumentNumber, inputType, inputAction);
            }
            else
            {                
                //获取数据库重更新前的数量
                List<InventoryBatchDetailsInfo> originalInventoryBatchDetailsInfolist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(Convert.ToInt32(inputDocumentNumber));
               
                List<int> deleteItemSysNoList = new List<int>();
                List<InventoryBatchDetailsInfo> deleteBatchInfoList = new List<InventoryBatchDetailsInfo>();
                if (originalInventoryBatchDetailsInfolist != null)
                {
                    originalInventoryBatchDetailsInfolist.ForEach(p =>
                    {
                        if (model != null)
                        {
                            var count = model.Where(k => k.ProductSysNo == p.ProductSysNo).Count();
                            if (count == 0)
                            {
                                deleteItemSysNoList.Add(p.ProductSysNo);
                            }
                        }
                        var c = model.Where(k => k.ProductSysNo == p.ProductSysNo && k.BatchNumber == p.BatchNumber && k.StockSysNo==p.StockSysNo).Count();
                        if (c == 0)
                        {
                            var clone = SerializationUtility.DeepClone<InventoryBatchDetailsInfo>(p);
                            clone.Quantity = 0;
                            deleteBatchInfoList.Add(clone);
                        }
                    });
                }
                //把需要删除的批次加入
               
                model.AddRange(deleteBatchInfoList);                

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    //损益单 调整需要 特殊处理
                    if (inputType.Equals("Adjust") && inputAction.Equals("Create"))//损益单创建  输入正值 不调占用库存
                    {
                        InnerAdjustMethod(model, inputDocumentNumber, deleteItemSysNoList, "AD", "Create", originalInventoryBatchDetailsInfolist);
                    }
                    else if (inputType.Equals("Lend") && inputAction.Equals("Create"))//借货单创建  更新
                    {
                        InnerLendMethod(model, inputDocumentNumber, deleteItemSysNoList, "LD", "Create", originalInventoryBatchDetailsInfolist);
                    }
                    else if (inputType.Equals("Lend") && inputAction.Equals("Return"))//借货单归还  更新实际库存
                    {
                        //更新 ST_Batch Quantity 和ReturnQty
                        ObjectFactory<IInventoryAdjustDA>.Instance.UpdateSTBInfo(model, inputDocumentNumber, "LD", "Return");
                        //拼XML 调用TankSP调整 实际库存
                       InnerReturndMethod(model, inputDocumentNumber, inputType, inputAction);
                        //发送SSB 到仓库                    
                        ObjectFactory<LendRequestProcessor>.Instance.ReturnSendSSBToWMS(inputDocumentNumber, model[0].StockSysNo.ToString(), model, null, "借货单归还向仓库发送SSB");
                    }
                    else if (inputType.Equals("Convert") && inputAction.Equals("Create"))//转换单创建  目标商品不调占用库存
                    {
                        List<InventoryBatchDetailsInfo> OriginalSourceInventoryBatchDetailsInfolist = new List<InventoryBatchDetailsInfo>();
                        if (originalInventoryBatchDetailsInfolist != null && originalInventoryBatchDetailsInfolist.Count > 0)
                        {
                            OriginalSourceInventoryBatchDetailsInfolist = originalInventoryBatchDetailsInfolist.FindAll(x => { return x.ReturnQty == 0; });
                        }

                        //找到源商品中被删除的商品
                        deleteItemSysNoList.Clear();
                        deleteBatchInfoList.Clear();
                        if (originalInventoryBatchDetailsInfolist != null)
                        {
                            var originSourceBatchDetailsInfoList = originalInventoryBatchDetailsInfolist.Where(p => p.ReturnQty == 0).ToList();
                            originSourceBatchDetailsInfoList.ForEach(p =>
                            {
                                if (model != null)
                                {
                                    var count = model.Where(k => k.ProductSysNo == p.ProductSysNo).Count();
                                    if (count == 0)
                                    {
                                        deleteItemSysNoList.Add(p.ProductSysNo);
                                    }
                                    var c = model.Where(k => k.ProductSysNo == p.ProductSysNo && k.BatchNumber == p.BatchNumber && k.StockSysNo == p.StockSysNo).Count();
                                    if (c == 0)
                                    {
                                        var clone = SerializationUtility.DeepClone<InventoryBatchDetailsInfo>(p);
                                        clone.Quantity = 0;
                                        deleteBatchInfoList.Add(p);
                                    }
                                }
                            });
                        }
                        model.AddRange(deleteBatchInfoList);
                       InnerConvertMethod(model, inputDocumentNumber, deleteItemSysNoList, "TR", "Create", OriginalSourceInventoryBatchDetailsInfolist);
                    }
                    else if (inputType.Equals("Convert") && inputAction.Equals("Target"))//转换单创建  目标商品不调占用库存
                    {
                        //找到目标商品中被删除的商品
                        deleteItemSysNoList.Clear();
                        deleteBatchInfoList.Clear();
                        if (originalInventoryBatchDetailsInfolist != null)
                        {
                            var originTargetBatchDetailsInfoList = originalInventoryBatchDetailsInfolist.Where(p => p.ReturnQty != 0).ToList();
                            originTargetBatchDetailsInfoList.ForEach(p =>
                            {
                                if (model != null)
                                {
                                    var count = model.Where(k => k.ProductSysNo == p.ProductSysNo).Count();
                                    if (count == 0)
                                    {
                                        deleteItemSysNoList.Add(p.ProductSysNo);
                                    }
                                    var c = model.Where(k => k.ProductSysNo == p.ProductSysNo && k.BatchNumber == p.BatchNumber && k.StockSysNo == p.StockSysNo).Count();
                                    if (c == 0)
                                    {
                                        var clone = SerializationUtility.DeepClone<InventoryBatchDetailsInfo>(p);
                                        clone.Quantity = 0;
                                        deleteBatchInfoList.Add(p);
                                    }
                                }
                            });
                        }

                        model.AddRange(deleteBatchInfoList);

                        if (deleteItemSysNoList.Count > 0)
                        {
                            foreach (var item in deleteItemSysNoList)
                            {
                                ObjectFactory<IInventoryAdjustDA>.Instance.DeleteBatchItemOfSTB(item, inputDocumentNumber);
                            }
                        }
                        ObjectFactory<IInventoryAdjustDA>.Instance.TargetUpdateSTBInfo(model, inputDocumentNumber, "TR");
                    }
                    scope.Complete();
                }                
            }
        }

        /// <summary>
        /// 损益单 创建更新 调整 Allocated 的方法
        /// </summary>
        /// <param name="model">传入的批次信息</param>
        /// <param name="InUser">操作人</param>
        /// <param name="DocumentNumber">操作的单据号</param>
        /// <param name="Type">操作的单据类型</param>
        /// <param name="Action">操作的单据的动作</param>
        /// <param name="result">操作是否成功</param>
        /// <param name="OriginalInventoryBatchDetailsInfolist">ST表对应的原由单据信息</param>
        /// <returns>返回值</returns>
        private void InnerAdjustMethod(List<InventoryBatchDetailsInfo> model, int inputDocumentNumber, List<int> deleteItemSysNoList, string inputType, string inputAction, List<InventoryBatchDetailsInfo> OriginalInventoryBatchDetailsInfolist)
        {            
            #region 将原由的数据作废
            List<InventoryBatchDetailsInfo> OriginalYiDanList = new List<InventoryBatchDetailsInfo>();
            List<InventoryBatchDetailsInfo> OriginalSunDanList = new List<InventoryBatchDetailsInfo>();

            if (OriginalInventoryBatchDetailsInfolist != null && OriginalInventoryBatchDetailsInfolist.Count > 0)
            {
                OriginalYiDanList = OriginalInventoryBatchDetailsInfolist.FindAll(x => { return x.Quantity >= 0; });
                OriginalSunDanList = OriginalInventoryBatchDetailsInfolist.FindAll(x => { return x.Quantity < 0; });
            }
            //将原有的单据 占用的库存调回去（ 只调整损单占用的库存就可以了  因为 益单是不占用库存的）
            if (OriginalSunDanList != null && OriginalSunDanList.Count > 0)
            {
                #region 拼接 损单调整占用库存的XML 消息
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in OriginalSunDanList)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.AllocatedQty = item.Quantity.ToString(); // 损单作废  将占用库存 调回去                               
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new  InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Adjust",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new  InventoryBody
                    {
                        InUser = ServiceContext.Current.UserDisplayName,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
            }
            #endregion
            #region 作废后 就相当于创建新单据了

            List<InventoryBatchDetailsInfo> NowYiDanList = new List<InventoryBatchDetailsInfo>();
            List<InventoryBatchDetailsInfo> NowSunDanList = new List<InventoryBatchDetailsInfo>();
            if (model != null && model.Count > 0)
            {
                NowYiDanList = model.FindAll(x => { return x.Quantity >= 0; });
                NowSunDanList = model.FindAll(x => { return x.Quantity < 0; });
            }
            ObjectFactory<IInventoryAdjustDA>.Instance.UpdateSTBInfo(model, inputDocumentNumber, inputType, inputAction);//益单执行了此动作后就 结束了。
            if (deleteItemSysNoList!=null)
            {
                foreach (var item in deleteItemSysNoList)
                {
                    ObjectFactory<IInventoryAdjustDA>.Instance.DeleteBatchItemOfSTB(item, inputDocumentNumber);
                }
            }
            if (NowSunDanList != null && NowSunDanList.Count > 0)
            {
                #region 损单调整 占用库存 XML
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in NowSunDanList)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.AllocatedQty = (-item.Quantity).ToString(); //创建单据 只调整 占用库存                                   
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new  InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Adjust",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new InventoryBody()
                    {
                        InUser = ServiceContext.Current.UserDisplayName,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量    
            }
            #endregion            
        }

        /// <summary>
        /// 借货单 创建更新 调整 Allocated 的方法
        /// </summary>
        /// <param name="model">传入的批次信息</param>
        /// <param name="InUser">操作人</param>
        /// <param name="DocumentNumber">操作的单据号</param>
        /// <param name="Type">操作的单据类型</param>
        /// <param name="Action">操作的单据的动作</param>
        /// <param name="result">操作是否成功</param>
        /// <param name="OriginalInventoryBatchDetailsInfolist">ST表对应的原由单据信息</param>
        /// <returns>返回值</returns>
        private void InnerLendMethod(List<InventoryBatchDetailsInfo> model, int inputDocumentNumber, List<int> deleteItemSysNoList, string inputType, string inputAction, List<InventoryBatchDetailsInfo> OriginalInventoryBatchDetailsInfolist)
        {            
            #region 将原由的数据作废
            //将原有的单据 占用的库存调回去（ 只调整损单占用的库存就可以了  因为 益单是不占用库存的）
            if (OriginalInventoryBatchDetailsInfolist != null && OriginalInventoryBatchDetailsInfolist.Count > 0)
            {
                #region 拼接 损单调整占用库存的XML 消息
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in OriginalInventoryBatchDetailsInfolist)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.AllocatedQty = (-item.Quantity).ToString(); // 原由借货单作废  将占用库存 调回去                               
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Lend",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new InventoryBody
                    {
                        InUser = ServiceContext.Current.UserDisplayName,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
            }
            #endregion
            #region 作废后 就相当于创建新单据了
            if (deleteItemSysNoList!=null)
            {
                foreach (var item in deleteItemSysNoList)
                {
                    ObjectFactory<IInventoryAdjustDA>.Instance.DeleteBatchItemOfSTB(item, inputDocumentNumber);
                }
            }
            ObjectFactory<IInventoryAdjustDA>.Instance.UpdateSTBInfo(model, inputDocumentNumber, inputType, inputAction);//调整批次库存表 占用数量。          
            if (model != null && model.Count > 0)
            {
                #region 损单调整 占用库存 XML
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in model)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.AllocatedQty = item.Quantity.ToString(); //创建单据 只调整 占用库存                                   
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Lend",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new InventoryBody
                    {
                        InUser = ServiceContext.Current.UserDisplayName,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量    
            }
            #endregion            
        }


        /// <summary>
        /// 转换单 创建更新 调整 Allocated 的方法
        /// </summary>
        /// <param name="model">传入的批次信息</param>
        /// <param name="InUser">操作人</param>
        /// <param name="DocumentNumber">操作的单据号</param>
        /// <param name="Type">操作的单据类型</param>
        /// <param name="Action">操作的单据的动作</param>
        /// <param name="result">操作是否成功</param>
        /// <param name="OriginalInventoryBatchDetailsInfolist">ST表对应的原由单据信息</param>
        /// <returns>返回值</returns>
        private void InnerConvertMethod(List<InventoryBatchDetailsInfo> model, int inputDocumentNumber, List<int> deleteItemSysNoList, string inputType, string inputAction, List<InventoryBatchDetailsInfo> OriginalSourceInventoryBatchDetailsInfolist)
        {            
            #region 将原由的数据作废
            //将原有的单据 占用的库存调回去（ 只调整损单占用的库存就可以了  因为 益单是不占用库存的）
            if (OriginalSourceInventoryBatchDetailsInfolist != null && OriginalSourceInventoryBatchDetailsInfolist.Count > 0)
            {
                #region 拼接 损单调整占用库存的XML 消息
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in OriginalSourceInventoryBatchDetailsInfolist)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.AllocatedQty = (-item.Quantity).ToString(); // 原由借货单作废  将占用库存 调回去                               
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new  InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Convert",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new InventoryBody
                    {
                        InUser = ServiceContext.Current.UserDisplayName,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
            }
            #endregion
            #region 作废后 就相当于创建新单据了
            if (deleteItemSysNoList!=null)
            {
                foreach (var item in deleteItemSysNoList)
                {
                    ObjectFactory<IInventoryAdjustDA>.Instance.DeleteBatchItemOfSTB(item, inputDocumentNumber);
                }
            }
            ObjectFactory<IInventoryAdjustDA>.Instance.SourceUpdateSTBInfo(model, inputDocumentNumber, inputType);//调整批次库存表 占用数量。          
            if (model != null && model.Count > 0)
            {
                #region 源商品转换单调整 占用库存 XML
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in model)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.AllocatedQty = item.Quantity.ToString(); //创建单据 只调整 占用库存                                   
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Convert",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new InventoryBody
                    {
                        InUser = ServiceContext.Current.UserDisplayName,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量    
            }
            #endregion            
        }

        /// <summary>
        /// 作废原来单据
        /// </summary>
        /// <param name="inputInUser"></param>
        /// <param name="inputDocumentNumber"></param>
        /// <param name="deleteItemSysNoList"></param>
        /// <param name="inputType"></param>
        /// <param name="inputAction"></param>
        /// <returns></returns>        
        private void AbandonSTBInfo(int inputDocumentNumber, string inputType, string inputAction)
        {
            string inputInUser = ServiceContext.Current.UserDisplayName;            
            //获取数据库重更新前的数量
            List<InventoryBatchDetailsInfo> OriginalInventoryBatchDetailsInfolist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(Convert.ToInt32(inputDocumentNumber));
            if (OriginalInventoryBatchDetailsInfolist != null && OriginalInventoryBatchDetailsInfolist.Count > 0)
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    //损益单 调整需要 特殊处理
                    if (inputType.Equals("Adjust") && inputAction.Equals("Create"))//损益单创建  输入正值 不调占用库存
                    {
                        #region 将原由的数据作废
                        List<InventoryBatchDetailsInfo> OriginalYiDanList = new List<InventoryBatchDetailsInfo>();
                        List<InventoryBatchDetailsInfo> OriginalSunDanList = new List<InventoryBatchDetailsInfo>();

                        OriginalYiDanList = OriginalInventoryBatchDetailsInfolist.FindAll(x => { return x.Quantity >= 0; });
                        OriginalSunDanList = OriginalInventoryBatchDetailsInfolist.FindAll(x => { return x.Quantity < 0; });

                        //将原有的单据 占用的库存调回去（ 只调整损单占用的库存就可以了  因为 益单是不占用库存的）
                        if (OriginalSunDanList != null && OriginalSunDanList.Count > 0)
                        {
                            #region 拼接 损单调整占用库存的XML 消息
                            List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                            foreach (var item in OriginalSunDanList)
                            {
                                ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                                itemBatchInfo.BatchNumber = item.BatchNumber;
                                itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                                Stock stock = new Stock();
                                stock.AllocatedQty = item.Quantity.ToString(); // 损单作废  将占用库存 调回去                               
                                stock.WarehouseNumber = item.StockSysNo.ToString();
                                List<Stock> StockList = new List<Stock>();
                                StockList.Add(stock);

                                Stocks stocks = new Stocks();
                                stocks.Stock = StockList;

                                itemBatchInfo.Stocks = stocks;
                                itemBatchInfoList.Add(itemBatchInfo);
                            }
                            BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                            {
                                Header = new  InventoryHeader
                                {
                                    NameSpace = "http://soa.newegg.com/InventoryProfile",
                                    Action = inputAction,
                                    Version = "V10",
                                    Type = "Adjust",
                                    CompanyCode = "8601",
                                    Tag = inputType + "" + inputAction,
                                    Language = "zh-CN",
                                    From = "IPP",
                                    GlobalBusinessType = "Listing",
                                    StoreCompanyCode = "8601",
                                    TransactionCode = inputDocumentNumber.ToString()
                                },
                                Body = new InventoryBody
                                {
                                    InUser = inputInUser,
                                    Number = inputDocumentNumber.ToString(),
                                    ItemBatchInfo = itemBatchInfoList
                                }
                            };
                            string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                            XmlDocument xmlD = new XmlDocument();
                            xmlD.LoadXml(paramXml);
                            paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                            #endregion
                            ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
                        }
                        ObjectFactory<IInventoryAdjustDA>.Instance.DeleteAllBatchItem(inputDocumentNumber, "");
                        #endregion
                    }
                    else if (inputType.Equals("Lend") && inputAction.Equals("Create"))//借货单创建  更新
                    {
                        #region 将原由的数据作废
                        //将原有的单据 占用的库存调回去（ 只调整损单占用的库存就可以了  因为 益单是不占用库存的）
                        if (OriginalInventoryBatchDetailsInfolist != null && OriginalInventoryBatchDetailsInfolist.Count > 0)
                        {
                            #region 拼接 损单调整占用库存的XML 消息
                            List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                            foreach (var item in OriginalInventoryBatchDetailsInfolist)
                            {
                                ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                                itemBatchInfo.BatchNumber = item.BatchNumber;
                                itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                                Stock stock = new Stock();
                                stock.AllocatedQty = (-item.Quantity).ToString(); // 原由借货单作废  将占用库存 调回去                               
                                stock.WarehouseNumber = item.StockSysNo.ToString();
                                List<Stock> StockList = new List<Stock>();
                                StockList.Add(stock);

                                Stocks stocks = new Stocks();
                                stocks.Stock = StockList;

                                itemBatchInfo.Stocks = stocks;
                                itemBatchInfoList.Add(itemBatchInfo);
                            }
                            BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                            {
                                Header = new  InventoryHeader
                                {
                                    NameSpace = "http://soa.newegg.com/InventoryProfile",
                                    Action = inputAction,
                                    Version = "V10",
                                    Type = "Lend",
                                    CompanyCode = "8601",
                                    Tag = inputType + "" + inputAction,
                                    Language = "zh-CN",
                                    From = "IPP",
                                    GlobalBusinessType = "Listing",
                                    StoreCompanyCode = "8601",
                                    TransactionCode = inputDocumentNumber.ToString()
                                },
                                Body = new InventoryBody()
                                {
                                    InUser = inputInUser,
                                    Number = inputDocumentNumber.ToString(),
                                    ItemBatchInfo = itemBatchInfoList
                                }
                            };
                            string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                            XmlDocument xmlD = new XmlDocument();
                            xmlD.LoadXml(paramXml);
                            paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                            #endregion
                            ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
                        }
                        ObjectFactory<IInventoryAdjustDA>.Instance.DeleteAllBatchItem(inputDocumentNumber, "");
                        #endregion
                    }
                    else if (inputType.Equals("Convert") && inputAction.Equals("DeleteAllSourceBatchItem"))//转换单创建  目标商品不调占用库存
                    {
                        List<InventoryBatchDetailsInfo> OriginalSourceInventoryBatchDetailsInfolist = OriginalInventoryBatchDetailsInfolist.FindAll(x => { return x.ReturnQty == 0; });
                        if (OriginalSourceInventoryBatchDetailsInfolist != null && OriginalSourceInventoryBatchDetailsInfolist.Count > 0)
                        {
                            #region 拼接 损单调整占用库存的XML 消息
                            List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                            foreach (var item in OriginalSourceInventoryBatchDetailsInfolist)
                            {
                                ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                                itemBatchInfo.BatchNumber = item.BatchNumber;
                                itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                                Stock stock = new Stock();
                                stock.AllocatedQty = (-item.Quantity).ToString(); // 原由借货单作废  将占用库存 调回去                               
                                stock.WarehouseNumber = item.StockSysNo.ToString();
                                List<Stock> StockList = new List<Stock>();
                                StockList.Add(stock);

                                Stocks stocks = new Stocks();
                                stocks.Stock = StockList;

                                itemBatchInfo.Stocks = stocks;
                                itemBatchInfoList.Add(itemBatchInfo);
                            }
                            BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                            {
                                Header = new  InventoryHeader
                                {
                                    NameSpace = "http://soa.newegg.com/InventoryProfile",
                                    Action = "Create",
                                    Version = "V10",
                                    Type = "Convert",
                                    CompanyCode = "8601",
                                    Tag = inputType + "" + "Create",
                                    Language = "zh-CN",
                                    From = "IPP",
                                    GlobalBusinessType = "Listing",
                                    StoreCompanyCode = "8601",
                                    TransactionCode = inputDocumentNumber.ToString()
                                },
                                Body = new  InventoryBody
                                {
                                    InUser = inputInUser,
                                    Number = inputDocumentNumber.ToString(),
                                    ItemBatchInfo = itemBatchInfoList
                                }
                            };
                            string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                            XmlDocument xmlD = new XmlDocument();
                            xmlD.LoadXml(paramXml);
                            paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                            #endregion
                            ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
                        }
                        ObjectFactory<IInventoryAdjustDA>.Instance.DeleteAllBatchItem(inputDocumentNumber, "DeleteAllSourceBatchItem");
                    }
                    else if (inputType.Equals("Convert") && inputAction.Equals("DeleteAllTargetBatchItem"))//转换单创建  目标商品不调占用库存
                    {
                        ObjectFactory<IInventoryAdjustDA>.Instance.DeleteAllBatchItem(inputDocumentNumber, "DeleteAllTargetBatchItem");
                    }
                    scope.Complete();
                }
            }            
        }


        /// <summary>
        /// 借货单归还调整 Allocated 的方法
        /// </summary>
        /// <param name="model">传入的批次信息</param>
        /// <param name="InUser">操作人</param>
        /// <param name="DocumentNumber">操作的单据号</param>
        /// <param name="Type">操作的单据类型</param>
        /// <param name="Action">操作的单据的动作</param>
        /// <param name="result">操作是否成功</param>
        /// <param name="OriginalInventoryBatchDetailsInfolist">ST表对应的原由单据信息</param>
        /// <returns>返回值</returns>
        private void InnerReturndMethod(List<InventoryBatchDetailsInfo> model, int inputDocumentNumber, string inputType, string inputAction)
        {
            string inputInUser = ServiceContext.Current.UserDisplayName;            
            #region 将原由的数据作废
            //将原有的单据 占用的库存调回去（ 只调整损单占用的库存就可以了  因为 益单是不占用库存的）
            if (model != null && model.Count > 0)
            {
                #region 拼接 损单调整占用库存的XML 消息
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in model)
                {
                    ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                    itemBatchInfo.BatchNumber = item.BatchNumber;
                    itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                    Stock stock = new Stock();
                    stock.Quantity = item.ReturnQty.ToString();    //归还调整 批次商品 的库存   
                    stock.WarehouseNumber = item.StockSysNo.ToString();
                    List<Stock> StockList = new List<Stock>();
                    StockList.Add(stock);

                    Stocks stocks = new Stocks();
                    stocks.Stock = StockList;

                    itemBatchInfo.Stocks = stocks;
                    itemBatchInfoList.Add(itemBatchInfo);
                }
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new InventoryHeader
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = inputAction,
                        Version = "V10",
                        Type = "Lend",
                        CompanyCode = "8601",
                        Tag = inputType + "" + inputAction,
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = "8601",
                        TransactionCode = inputDocumentNumber.ToString()
                    },
                    Body = new InventoryBody
                    {
                        InUser = inputInUser,
                        Number = inputDocumentNumber.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                #endregion
                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量   
            }
            #endregion            
        }
    }
}
