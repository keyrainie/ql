using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.Entity.Inventory;
using ECommerce.DataAccess.Inventory;
using ECommerce.DataAccess;
using ECommerce.Utility;
using ECommerce.Enums;
using System.Transactions;
using ECommerce.Entity.Product;
using ECommerce.Service.Product;
using System.Xml;
using ECommerce.Service.ControlPannel;

namespace ECommerce.Service.Inventory
{
    public static class InventoryService
    {

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<InventoryQueryInfo> QueryProductInventory(InventoryQueryFilter queryFilter)
        {
            if (queryFilter == null || string.IsNullOrEmpty(queryFilter.ProductSysNo))
            {
                throw new BusinessException("请输入商品编号");
            }
            if (string.IsNullOrEmpty(queryFilter.MerchantSysNo))
            {
                throw new BusinessException("商家编号不能为空");
            }

            queryFilter.CompanyCode = "8601";
            //查询总库存
            if (string.IsNullOrEmpty(queryFilter.StockSysNo))
            {
                return InventoryDA.QueryProductInventoryTotal(queryFilter);
            }
            else
            {
                QueryResult<InventoryQueryInfo> result = InventoryDA.QueryProductInventoryByStock(queryFilter);
                QueryResult<InventoryQueryInfo> resultTotal = InventoryDA.QueryProductInventoryTotal(queryFilter);
                if (resultTotal == null)
                {
                    resultTotal = new QueryResult<InventoryQueryInfo>();
                    resultTotal.ResultList = new List<InventoryQueryInfo>();
                    resultTotal.PageInfo = new PageInfo();
                }
                if (result != null && result.ResultList != null && result.ResultList.Count > 0)
                {
                    resultTotal.ResultList.AddRange(result.ResultList);
                    resultTotal.PageInfo.TotalCount += result.PageInfo.TotalCount;
                }
                return resultTotal;
            }
        }

        /// <summary>
        /// 查询库存变化单据
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<InventoryItemCardInfo> QueryCardItemOrders(InventoryItemCardQueryFilter queryFilter)
        {
            if (queryFilter == null || string.IsNullOrEmpty(queryFilter.ProductSysNo))
            {
                throw new BusinessException("请输入商品编号");
            }
            if (string.IsNullOrEmpty(queryFilter.MerchantSysNo))
            {
                throw new BusinessException("商家编号不能为空");
            }
            queryFilter.CompanyCode = "8601";

            //获取RMAInventoryOnlineDate:
            string rmaInventoryOnlineDate = CommonDA.GetSysConfigurationValue("RMAInventoryOnlineDate", queryFilter.CompanyCode);
            queryFilter.RMAInventoryOnlineDate = string.IsNullOrEmpty(rmaInventoryOnlineDate) ? new DateTime() : Convert.ToDateTime(rmaInventoryOnlineDate);

            return InventoryDA.QueryCardItemOrdersRelated(queryFilter);
        }

        /// <summary>
        /// 查询仓库
        /// </summary>
        /// <param name="merchantSysNo">商家编号</param>
        /// <returns></returns>
        public static List<StockInfo> GetStock(int merchantSysNo)
        {
            return InventoryDA.GetStock(merchantSysNo);
        }

        public static Entity.Inventory.ProductInventoryInfo PreCalculateInventoryAfterAdjust(Entity.Inventory.ProductInventoryInfo currentInfo, Entity.Inventory.ProductInventoryInfo adjustInfo)
        {
            return new Entity.Inventory.ProductInventoryInfo()
            {
                AllocatedQty = currentInfo.AllocatedQty + adjustInfo.AllocatedQty,
                AccountQty = currentInfo.AccountQty + adjustInfo.AccountQty,
                AvailableQty = currentInfo.AvailableQty + adjustInfo.AvailableQty,
                ConsignQty = currentInfo.ConsignQty + adjustInfo.ConsignQty,
                VirtualQty = currentInfo.VirtualQty + adjustInfo.VirtualQty,
                OrderQty = currentInfo.OrderQty + adjustInfo.OrderQty,
                PurchaseQty = currentInfo.PurchaseQty + adjustInfo.PurchaseQty,
                ReservedQty = currentInfo.ReservedQty + adjustInfo.ReservedQty,
                ShiftQty = currentInfo.ShiftQty + adjustInfo.ShiftQty,
                ShiftInQty = currentInfo.ShiftInQty + adjustInfo.ShiftInQty,
                ShiftOutQty = currentInfo.ShiftOutQty + adjustInfo.ShiftOutQty,
                ChannelQty = currentInfo.ChannelQty + adjustInfo.ChannelQty,
                StockInfo = currentInfo.StockInfo != null ? new StockInfo() { SysNo = currentInfo.StockInfo.SysNo } : null
            };
        }

        public static void PreCheckGeneralRules(Entity.Inventory.ProductInventoryInfo inventoryInfo, ref bool isNeedCompareAvailableQtyAndAccountQty)
        {
            string commError = "不能将可用库存调整为负数，也不能使可卖数量为负数，可卖数量=可用库存+虚拟库存+代销库存.";
            #region 检查库存量是否小于0

            //if (inventoryInfo.con < 0)
            //{
            //    throw new BusinessException("财务库存不能小于0!");
            //}

            //if (inventoryInfo.AvailableQty < 0)
            //{
            //    throw new BusinessException(commError);
            //}

            if (inventoryInfo.OrderQty < 0)
            {
                throw new BusinessException("已订购数量不能小于0!");
            }

            if (inventoryInfo.ConsignQty < 0)
            {
                throw new BusinessException(commError);
            }

            if (inventoryInfo.AllocatedQty < 0)
            {
                throw new BusinessException("已分配库存不能小于0！");
            }

            #endregion  检查库存量是否小于0

            #region 检查相关库存量之间的逻辑规则

            if (inventoryInfo.AvailableQty + inventoryInfo.VirtualQty + inventoryInfo.ConsignQty < 0)
            {
                throw new BusinessException(commError);
            }

            //if (inventoryInfo.StockInfo != null && inventoryInfo.StockInfo.SysNo > 0)
            //{
            //    //if product is in MKT's Stock, need not run this way
            //    var mktStockList = AppSettingManager.GetSetting("Inventory", "MKTVirtualInventory").Split(',').Select(p => int.Parse(p));
            //    if (mktStockList.Contains(inventoryInfo.StockInfo.SysNo.Value))
            //    {
            //        isNeedCompareAvailableQtyAndAccountQty = false;
            //    }
            //}

            //if (isNeedCompareAvailableQtyAndAccountQty && inventoryInfo.AvailableQty > inventoryInfo.ConsignQty)
            //{
            //    throw new BusinessException("财务库存不能小于可用库存！");
            //}

            #endregion 检查相关库存量之间的逻辑规则
        }

        public static AdjustRequestInfo GetAdjustRequestInfoBySysNo(int adjustSysNo)
        {
            return InventoryDA.GetAdjustRequestInfoBySysNo(adjustSysNo);
        }

        /// <summary>
        /// 损益单出库 (确认成功):
        /// </summary>
        /// <param name="adjustRequestSysNo">原始损益单编号</param>
        /// <param name="productSysNo">损益商品编号</param>
        /// <param name="realAdjustQty">该商品的实际损益数量</param>
        /// <returns></returns>
        public static AdjustRequestInfo AdjustOutStock(int adjustRequestSysNo, int? productSysNo, int? realAdjustQty, int sellerSysNo)
        {
            var adjustRequestInfo = GetAdjustRequestInfoBySysNo(adjustRequestSysNo);


            #region Check操作 :
            if (null == adjustRequestInfo || adjustRequestInfo.SysNo <= 0)
            {
                throw new BusinessException(string.Format("找不到编号为{0}的损益单据信息!", adjustRequestSysNo));
            }

            if (productSysNo.HasValue && realAdjustQty.HasValue)
            {
                var productItemInfo = adjustRequestInfo.AdjustItemInfoList.FirstOrDefault(x => x.ProductSysNo == productSysNo);
                if (productItemInfo == null)
                {
                    throw new BusinessException(string.Format("编号为{0}的商品不存在于该损益单中!损益单编号 ：{1}", productSysNo, adjustRequestSysNo));
                }
                if (realAdjustQty >= 0)
                {
                    if (realAdjustQty > productItemInfo.AdjustQuantity)
                    {
                        throw new BusinessException(string.Format("编号为{0}的商品实际损益的数量大于预损益的数量!损益单编号 ：{1}", productSysNo, adjustRequestSysNo));
                    }
                }
                else
                {
                    if (realAdjustQty < productItemInfo.AdjustQuantity)
                    {
                        throw new BusinessException(string.Format("编号为{0}的商品实际损益的数量大于预损益的数量!损益单编号 ：{1}", productSysNo, adjustRequestSysNo));
                    }
                }
            }

            var stockInfo = StockService.LoadStock(adjustRequestInfo.Stock.SysNo);
            if (null == stockInfo)
            {
                throw new BusinessException("损益单据关联的仓库编号无效!");
            }
            if (stockInfo.MerchantSysNo != sellerSysNo)
            {
                throw new BusinessException("此商家无权操作此单据!");
            }

            //增加损益单状态Check (已申报状态):
            if (adjustRequestInfo.RequestStatus != AdjustRequestStatus.Reported)
            {
                throw new BusinessException(string.Format("损益单编号 ：{1},当前单据的状态不是'已申报'状态，不能进行损益单确认操作", productSysNo, adjustRequestSysNo));

            }

            #endregion

            bool isConsign = false;
            adjustRequestInfo.OutStockDate = DateTime.Now;
            adjustRequestInfo.RequestStatus = Enums.AdjustRequestStatus.OutStock;
            isConsign = (adjustRequestInfo.ConsignFlag == RequestConsignFlag.Consign || adjustRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay);
            var inventoryAdjustContract = new InventoryAdjustContractInfo
            {
                SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_AdjustRequest,
                SourceActionName = InventoryAdjustSourceAction.OutStock,
                ReferenceSysNo = adjustRequestInfo.SysNo.ToString(),
                AdjustItemList = new List<InventoryAdjustItemInfo>()
            };

            using (TransactionScope scope = new TransactionScope())
            {
                if (adjustRequestInfo.AdjustItemInfoList != null && adjustRequestInfo.AdjustItemInfoList.Count > 0)
                {
                    adjustRequestInfo.AdjustItemInfoList.ForEach(adjustItem =>
                    {
                        //if (adjustItem.AdjustProduct.ProductPriceInfo == null)
                        //{
                        //    BizExceptionHelper.Throw("Common_CannotFindPriceInformation");
                        //    throw new BusinessException("损益数量只能为非0的整数!");
                        //}
                        if (adjustItem.AdjustQuantity == 0)
                        {
                            throw new BusinessException("损益数量只能为非0的整数!");
                        }

                        var cost = InventoryDA.GetItemCost(adjustItem.ProductSysNo.Value);
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = adjustItem.AdjustQuantity.Value,
                            ProductSysNo = adjustItem.ProductSysNo.Value,
                            StockSysNo = (int)adjustRequestInfo.Stock.SysNo,
                            AdjustUnitCost = cost,
                        });

                        //update flash item unit cost
                        if (adjustItem.AdjustCost != cost)
                        {
                            adjustItem.AdjustCost = cost;
                            InventoryDA.UpdateAdjustItemCost(adjustItem);
                        }
                    });
                }
                InventoryDA.UpdateAdjustRequestStatus(adjustRequestInfo);

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    //string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    //if (!string.IsNullOrEmpty(adjustResult))
                    //{
                    //    throw new BizException("库存调整失败: " + adjustResult);
                    //}


                    #region 调整库存:

                    foreach (InventoryAdjustItemInfo adjustItem in inventoryAdjustContract.AdjustItemList)
                    {

                        ProductQueryInfo productInfo = ProductService.GetProductBySysNo(adjustItem.ProductSysNo);
                        if (productInfo == null || productInfo.SysNo <= 0)
                        {
                            throw new BusinessException(string.Format("欲调库存的商品不存在，商品编号：{0}", adjustItem.ProductSysNo));
                        }
                        InventoryDA.InitProductInventoryInfo(adjustItem.ProductSysNo, adjustItem.StockSysNo);
                        var inventoryType = InventoryDA.GetProductInventroyType(adjustItem.ProductSysNo);
                        ECommerce.Entity.Inventory.ProductInventoryInfo stockInventoryCurrentInfo = InventoryDA.GetProductInventoryInfoByStock(adjustItem.ProductSysNo, adjustItem.StockSysNo);
                        ECommerce.Entity.Inventory.ProductInventoryInfo totalInventoryCurrentInfo = InventoryDA.GetProductTotalInventoryInfo(adjustItem.ProductSysNo);

                        ECommerce.Entity.Inventory.ProductInventoryInfo stockInventoryAdjustInfo = new Entity.Inventory.ProductInventoryInfo()
                        {
                            ProductSysNo = adjustItem.ProductSysNo,
                            StockSysNo = adjustItem.StockSysNo
                        };

                        ECommerce.Entity.Inventory.ProductInventoryInfo totalInventoryAdjustInfo = new ECommerce.Entity.Inventory.ProductInventoryInfo()
                        {
                            ProductSysNo = adjustItem.ProductSysNo
                        };


                        if (adjustItem.AdjustQuantity < 0)
                        {
                            //损单出库
                            if (adjustRequestInfo.ConsignFlag == RequestConsignFlag.Consign || adjustRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
                            {
                                //代销商品, 恢复可用库存, 减少已分配库存/代销库存                   
                                stockInventoryAdjustInfo.AvailableQty = -adjustItem.AdjustQuantity;
                                totalInventoryAdjustInfo.AvailableQty = -adjustItem.AdjustQuantity;

                                if (adjustItem.AdjustQuantity < 0)
                                {
                                    //AllocatedQty(-,->0),小于0则自动调为0。       
                                    if (stockInventoryCurrentInfo.AllocatedQty + adjustItem.AdjustQuantity < 0)
                                    {
                                        stockInventoryAdjustInfo.AllocatedQty = -stockInventoryCurrentInfo.AllocatedQty;
                                    }
                                    else
                                    {
                                        stockInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                    }

                                    if (totalInventoryCurrentInfo.AllocatedQty + adjustItem.AdjustQuantity < 0)
                                    {
                                        totalInventoryAdjustInfo.AllocatedQty = -totalInventoryCurrentInfo.AllocatedQty;
                                    }
                                    else
                                    {
                                        totalInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                    }

                                }
                                else
                                {
                                    stockInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                    totalInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                }

                                stockInventoryAdjustInfo.ConsignQty = adjustItem.AdjustQuantity;
                                totalInventoryAdjustInfo.ConsignQty = adjustItem.AdjustQuantity;
                            }
                            else
                            {
                                //非代销商品, 减少财务库存/已分配库存
                                stockInventoryAdjustInfo.AccountQty = adjustItem.AdjustQuantity;
                                totalInventoryAdjustInfo.AccountQty = adjustItem.AdjustQuantity;

                                if (adjustItem.AdjustQuantity < 0)
                                {
                                    //AllocatedQty(-,->0),小于0则自动调为0。       
                                    if (stockInventoryCurrentInfo.AllocatedQty + adjustItem.AdjustQuantity < 0)
                                    {
                                        stockInventoryAdjustInfo.AllocatedQty = -stockInventoryCurrentInfo.AllocatedQty;
                                    }
                                    else
                                    {
                                        stockInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                    }

                                    if (totalInventoryCurrentInfo.AllocatedQty + adjustItem.AdjustQuantity < 0)
                                    {
                                        totalInventoryAdjustInfo.AllocatedQty = -totalInventoryCurrentInfo.AllocatedQty;
                                    }
                                    else
                                    {
                                        totalInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                    }

                                }
                                else
                                {
                                    stockInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                    totalInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                                }
                            }
                        }
                        else
                        {
                            //溢单出库
                            if (adjustRequestInfo.ConsignFlag == RequestConsignFlag.Consign || adjustRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
                            {
                                //代销商品, 增加代销库存 (损/溢单都增加代销库存?)
                                stockInventoryAdjustInfo.ConsignQty = adjustItem.AdjustQuantity;
                                totalInventoryAdjustInfo.ConsignQty = adjustItem.AdjustQuantity;
                            }
                            else
                            {
                                //非代销商品, 增加财务库存/可用库存
                                stockInventoryAdjustInfo.AccountQty = adjustItem.AdjustQuantity;
                                totalInventoryAdjustInfo.AccountQty = adjustItem.AdjustQuantity;
                                stockInventoryAdjustInfo.AvailableQty = adjustItem.AdjustQuantity;
                                totalInventoryAdjustInfo.AvailableQty = adjustItem.AdjustQuantity;
                            }
                        }

                        //预检调整后的商品库存是否合法  
                        Entity.Inventory.ProductInventoryInfo stockInventoryAdjustAfterAdjust = InventoryService.PreCalculateInventoryAfterAdjust(stockInventoryCurrentInfo, stockInventoryAdjustInfo);
                        Entity.Inventory.ProductInventoryInfo totalInventoryAdjustAfterAdjust = InventoryService.PreCalculateInventoryAfterAdjust(totalInventoryCurrentInfo, totalInventoryAdjustInfo);

                        bool isNeedCompareAvailableQtyAndAccountQty = true;
                        InventoryService.PreCheckGeneralRules(stockInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);
                        InventoryService.PreCheckGeneralRules(totalInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);

                        //调整商品库存:
                        InventoryDA.AdjustProductStockInventoryInfo(stockInventoryAdjustInfo);
                        InventoryDA.AdjustProductTotalInventoryInfo(totalInventoryAdjustInfo);
                    }
                    #endregion

                    #region 损益单为代销类型，出库时需要写入代销转财务日志
                    if (isConsign)
                    {

                        List<ConsignToAcctLogInfo> acctLogInfoList = new List<ConsignToAcctLogInfo>();
                        adjustRequestInfo.AdjustItemInfoList.ForEach(x =>
                        {
                            ConsignToAcctLogInfo acctLog = new ConsignToAcctLogInfo();
                            acctLog.ProductSysNo = x.ProductSysNo;
                            acctLog.StockSysNo = adjustRequestInfo.Stock.SysNo;
                            acctLog.VendorSysNo = InventoryDA.GetProductBelongVendorSysNo(x.ProductSysNo.Value);
                            acctLog.ProductQuantity = -x.AdjustQuantity;
                            acctLog.OutStockTime = adjustRequestInfo.OutStockDate;
                            acctLog.CreateCost = x.AdjustCost;
                            acctLog.OrderSysNo = adjustRequestInfo.SysNo;
                            acctLog.CompanyCode = adjustRequestInfo.CompanyCode;
                            acctLog.StoreCompanyCode = adjustRequestInfo.CompanyCode;
                            acctLog.IsConsign = (int)adjustRequestInfo.ConsignFlag;
                            acctLogInfoList.Add(acctLog);
                        });

                        if (acctLogInfoList.Count > 0)
                        {
                            foreach (var item in acctLogInfoList)
                            {
                                InventoryDA.CreatePOConsignToAccLogForInventory(item);
                            }
                        }

                    }
                    #endregion

                    #region
                    string InUser = "接口API调用";
                    List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = InventoryDA.GetBatchDetailsInfoEntityListByNumber(adjustRequestInfo.SysNo.Value);
                    if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
                    {
                        #region 构建损益单 出库调整批次库存表的SSB消息 调整库存
                        List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                        foreach (var item in batchDetailsInfoEntitylist)
                        {
                            ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                            itemBatchInfo.BatchNumber = item.BatchNumber;
                            itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();
                            Stock stock = new Stock();
                            AdjustRequestItemInfo aEntity = new AdjustRequestItemInfo();
                            aEntity = adjustRequestInfo.AdjustItemInfoList.Find(x => { return x.ProductSysNo == item.ProductSysNo; });
                            if (aEntity != null && aEntity.AdjustQuantity > 0)
                            {
                                stock.Quantity = item.Quantity.ToString(); //单据出库两个数量都要调整
                                stock.AllocatedQty = string.Empty; //益单不调 占用库存 作废 取消作废 单据 只调整 占用库存
                            }
                            else
                            {
                                stock.Quantity = item.Quantity.ToString(); //单据出库两个数量都要调整
                                stock.AllocatedQty = item.Quantity.ToString(); //损单 需要调整 占用库存 作废 取消作废 单据 只调整 占用库存
                            }
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
                                NameSpace = "http://soa.ECommerce.com/InventoryProfile",
                                Action = "OutStock",
                                Version = "V10",
                                Type = "Adjust",
                                CompanyCode = "8601",
                                Tag = "AdjustOutStock",
                                Language = "zh-CN",
                                From = "IPP",
                                GlobalBusinessType = "Listing",
                                StoreCompanyCode = "8601",
                                TransactionCode = adjustRequestInfo.SysNo.ToString()
                            },
                            Body = new InventoryBody
                            {
                                InUser = InUser,
                                Number = adjustRequestInfo.SysNo.ToString(),
                                ItemBatchInfo = itemBatchInfoList
                            }
                        };
                        string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                        XmlDocument xmlD = new XmlDocument();
                        xmlD.LoadXml(paramXml);
                        paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                        InventoryDA.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量
                        #endregion
                    }
                    List<InventoryAdjustItemInfo> adjustCaseEntityList = new List<InventoryAdjustItemInfo>();
                    foreach (var item in inventoryAdjustContract.AdjustItemList)
                    {
                        if (!InventoryDA.CheckISBatchNumberProduct(item.ProductSysNo))
                        {
                            item.AdjustQuantity = item.AdjustQuantity;

                            adjustCaseEntityList.Add(item);
                        }
                    }
                    AdjustSendSSBToWMS(adjustRequestInfo.SysNo.Value, adjustRequestInfo.Stock.SysNo.ToString(), batchDetailsInfoEntitylist, adjustCaseEntityList);//损益单出库向仓库发送SSB消息                                                                    

                    #endregion
                }

                scope.Complete();
            }

            return adjustRequestInfo;
        }

        private static void AdjustSendSSBToWMS(int sysNo, string inWarehouseNumber, List<InventoryBatchDetailsInfo> batchDetailsInfoEntity, List<InventoryAdjustItemInfo> adjustCaseEntityList)
        {
            List<ECommerce.Entity.Inventory.Item> itemList = new List<ECommerce.Entity.Inventory.Item>();
            // List<BatchDetailsInfoEntity> batchDetailsInfoEntity = AdjustDAL.GetBatchDetailsInfoEntityListByNumber(SysNumber);
            if ((batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0) || (adjustCaseEntityList != null && adjustCaseEntityList.Count > 0))
            {
                if (batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0)
                {
                    foreach (var BatchItem in batchDetailsInfoEntity)
                    {
                        ProductBatch pbEntity = new ProductBatch
                        {
                            BatchNumber = BatchItem.BatchNumber
                            ,
                            Quantity = BatchItem.Quantity.ToString()
                        };
                        ECommerce.Entity.Inventory.Item item = new ECommerce.Entity.Inventory.Item()
                        {
                            ProductBatch = pbEntity
                            ,
                            ProductSysNo = BatchItem.ProductSysNo.ToString()
                            ,
                            Quantity = BatchItem.Quantity.ToString()
                        };
                        itemList.Add(item);
                    }
                }
                if (adjustCaseEntityList != null && adjustCaseEntityList.Count > 0)
                {
                    //初始非批次商品调整信息
                    foreach (var NOTBatchItem in adjustCaseEntityList)
                    {
                        ECommerce.Entity.Inventory.Item item = new ECommerce.Entity.Inventory.Item()
                        {
                            ProductSysNo = NOTBatchItem.ProductSysNo.ToString(),
                            Quantity = NOTBatchItem.AdjustQuantity.ToString()
                        };
                        itemList.Add(item);
                    }
                }

                SendToWMSSSBXMLMessage sendTOWMSSSBXMLMessage = new SendToWMSSSBXMLMessage()
                {
                    RequestRoot = new RequestRoot()
                    {
                        MessageHeader = new MessageHeader
                        {
                            Language = "CH",
                            Sender = "IPP",
                            CompanyCode = "8601",
                            Action = "Adjust",
                            Version = "0.1",
                            Type = "InventoryAdjust",
                            OriginalGUID = ""
                        },
                        Body = new ECommerce.Entity.Inventory.Body
                        {
                            Operation = new Operation()
                            {
                                Type = "60",
                                Number = sysNo.ToString(),
                                User = "API接口调用",//ServiceContext.Current.UserDisplayName,
                                Memo = "损益单出库给仓库发送SSB",
                                Item = itemList,
                                WarehouseNumber = inWarehouseNumber
                            }
                        }
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(sendTOWMSSSBXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                if (itemList != null && itemList.Count > 0)
                {
                    InventoryDA.SendSSBToWMS(paramXml);
                }
            }
        }
    }
}
