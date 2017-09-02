using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.Inventory;
using System.Xml;

namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 转换单 - BizProcessor
    /// </summary>
    [VersionExport(typeof(ConvertRequestProcessor))]
    public class ConvertRequestProcessor
    {
        private IConvertRequestDA convertRequestDA = ObjectFactory<IConvertRequestDA>.Instance;

        /// <summary>
        /// 加载单个转换单据
        /// </summary>
        /// <param name="convertRequestSysNo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo GetConvertRequestInfoBySysNo(int requestSysNo)
        {
            ConvertRequestInfo returnEntity = new ConvertRequestInfo();
            returnEntity = ObjectFactory<IConvertRequestDA>.Instance.GetConvertRequestInfoBySysNo(requestSysNo);
            if (null != returnEntity)
            {
                returnEntity.ConvertItemInfoList = ObjectFactory<IConvertRequestDA>.Instance.GetConvertItemListByRequestSysNo(requestSysNo);
            }
            return returnEntity;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo GetProductLineInfo(int ProductSysNo)
        {
            ConvertRequestInfo shiftRequest = convertRequestDA.GetProductLineInfo(ProductSysNo);
            return shiftRequest;
        }

        /// <summary>
        /// 转换单 - 创建操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo CreateRequest(ConvertRequestInfo entityToCreate)
        {
            PreCheckConvertRequestInfoForCreate(entityToCreate);

            PreCheckDuplicatedConvertItem(entityToCreate);
            
            ConvertRequestInfo result;

            using (var scope = new TransactionScope())
            {   
                entityToCreate.RequestStatus = ConvertRequestStatus.Origin;
                entityToCreate.CreateDate = DateTime.Now; //BusinessUtility.GetCurrentDateTime();


                int requestSysNo = convertRequestDA.GetConvertRequestSequence();                
                entityToCreate.SysNo = requestSysNo;
                entityToCreate.RequestID = "58" + requestSysNo.ToString().PadLeft(8, '0');

                result = convertRequestDA.CreateConvertRequest(entityToCreate);

                result.CreateUser = new UserInfo { SysNo = entityToCreate.CreateUser.SysNo };//entityToCreate.CreateUserName;
                result.Stock = new StockInfo { SysNo = entityToCreate.Stock.SysNo };//entityToCreate.StockName;

                var inventoryAdjustContract = InitInventoryAdjustContract(entityToCreate, InventoryAdjustSourceAction.Create);

                if (entityToCreate.ConvertItemInfoList != null && entityToCreate.ConvertItemInfoList.Count > 0)
                {
                    result.ConvertItemInfoList = new List<ConvertRequestItemInfo>();

                    // product batch info
                    List<InventoryBatchDetailsInfo> sourceBatchList = new List<InventoryBatchDetailsInfo>();
                    List<InventoryBatchDetailsInfo> targetBatchList = new List<InventoryBatchDetailsInfo>();


                    entityToCreate.ConvertItemInfoList.ForEach(convertItem =>
                    {
                        PreCheckConvertItemInfo(convertItem);

                        if (convertItem.ConvertType == ConvertProductType.Target)
                        {
                            //目标商品
                            //中蛋特殊逻辑，51上海仓目标商品去税成本特殊计算
                            //[Mark][Alan.X.Luo 特有逻辑]
                            if (entityToCreate.Stock.SysNo == 51)
                            {
                                convertItem.ConvertUnitCostWithoutTax = convertItem.ConvertUnitCost / (decimal)1.17;
                            }
                            else
                            {
                                convertItem.ConvertUnitCostWithoutTax = convertItem.ConvertUnitCost;
                            }
                            if (convertItem.BatchDetailsInfoList != null && convertItem.BatchDetailsInfoList.Count > 0)
                            {
                                targetBatchList.AddRange(convertItem.BatchDetailsInfoList);
                            }
                        }
                        else
                        {
                            //源商品
                            convertItem.ConvertUnitCost = convertItem.ConvertProduct.ProductPriceInfo.UnitCost;
                            convertItem.ConvertUnitCostWithoutTax = convertItem.ConvertProduct.ProductPriceInfo.UnitCostWithoutTax;
                            if (convertItem.BatchDetailsInfoList != null && convertItem.BatchDetailsInfoList.Count > 0)
                            {
                                sourceBatchList.AddRange(convertItem.BatchDetailsInfoList);
                            }
                        }

                        //convertItem.ConvertSysNumber = result.ConvertSysNumber;
                        
                        result.ConvertItemInfoList.Add(convertRequestDA.CreateConvertItem(convertItem, (int)entityToCreate.SysNo));

                        if (convertItem.ConvertType == ConvertProductType.Source)
                        {
                            //源商品库存调整
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = -convertItem.ConvertQuantity,
                                ProductSysNo = convertItem.ConvertProduct.SysNo,
                                StockSysNo = (int)entityToCreate.Stock.SysNo
                            });
                        }

                    });

                    //InventoryAdjustBP.AdjustInventory(inventoryAdjustEntity);
                    //调整库存统一接口
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }

                    // batch info
                    if (sourceBatchList != null && sourceBatchList.Count > 0)
                    {
                        ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(sourceBatchList, result.SysNo.Value, "Convert", "Create");
                    }
                    if (targetBatchList != null && targetBatchList.Count > 0)
                    {
                        ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(targetBatchList, result.SysNo.Value, "Convert", "Target");
                    }
                }

                ///创建转换单发送message
                EventPublisher.Publish(new CreateConvertRequestInfoMessage() {
                    ConvertRequestInfoSysNo = entityToCreate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            //SetEntityBase(newEntity, result);

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"创建了编号为\"{1}\"的转换单", entityToCreate.CreateUser.SysNo, entityToCreate.RequestID)
            , BizLogType.St_Transfer_Master_Insert, (int)entityToCreate.SysNo, entityToCreate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 转换单 - 更新操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo UpdateRequest(ConvertRequestInfo entityToUpdate)
        {
            PreCheckConvertRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedConvertItem(entityToUpdate);

            var originalEntity = GetConvertRequestInfoBySysNo((int)entityToUpdate.SysNo);

            PreCheckOriginConvertRequestInfo(originalEntity, InventoryAdjustSourceAction.Update);

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Update);

            var originalItems = originalEntity.ConvertItemInfoList;

            var newItems = entityToUpdate.ConvertItemInfoList;

            #region 分别获取新旧转换单中的源商品

            List<ConvertRequestItemInfo> originalSourceItems = null;

            if (originalItems != null && originalItems.Count > 0)
            {
                originalSourceItems = new List<ConvertRequestItemInfo>();
                foreach (var item in originalItems)
                {
                    if (item.ConvertType == ConvertProductType.Source)
                    {
                        originalSourceItems.Add(item);
                    }
                }
                if (originalSourceItems.Count == 0)
                {
                    originalSourceItems = null;
                }
            }

            List<ConvertRequestItemInfo> newSourceItems = null;

            if (newItems != null && newItems.Count > 0)
            {
                newSourceItems = new List<ConvertRequestItemInfo>();
                foreach (var item in newItems)
                {
                    if (item.ConvertType == ConvertProductType.Source)
                    {
                        newSourceItems.Add(item);
                    }
                }

                if (newSourceItems.Count == 0)
                {
                    newSourceItems = null;
                }
            }

            #endregion 分别获取新旧转换单中的源商品

            #region 库存调整逻辑

            //对比新/旧转换单中的源商品
            if (originalSourceItems != null && newSourceItems == null)
            {
                //原转换单有源商品，新单中无源商品, 移除源商品对应库存操作
                foreach (var item in originalSourceItems)
                {
                    //取消源商品库存调整                        
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = item.ConvertQuantity,
                        ProductSysNo = item.ConvertProduct.SysNo,
                        StockSysNo = (int)originalEntity.Stock.SysNo
                    });
                }
            }
            else if (originalSourceItems == null && newSourceItems != null)
            {
                //原转换单有源商品，新单中无源商品, 新增源商品对应库存操作
                foreach (var item in newSourceItems)
                {
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = -item.ConvertQuantity,
                        ProductSysNo = item.ConvertProduct.SysNo,
                        StockSysNo = (int)originalEntity.Stock.SysNo
                    });
                }
            }
            else if (originalSourceItems != null)
            {
                //新/旧转换单中都有源商品，需要逐个对比
                foreach (var originalItem in originalSourceItems)
                {
                    //var entity = originalItem;
                    var newItem = newSourceItems.Find(p => p.ConvertProduct.SysNo == originalItem.ConvertProduct.SysNo);

                    if (newItem != null)
                    {
                        //新旧单中都包含的同一源商品
                        var adjustQty = originalItem.ConvertQuantity - newItem.ConvertQuantity;
                        if (adjustQty != 0)
                        {
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = adjustQty,
                                ProductSysNo = originalItem.ConvertProduct.SysNo,
                                StockSysNo = (int)originalEntity.Stock.SysNo                                
                            });
                        }
                        //continue;
                    }
                    else
                    {
                        //只在原转换单中存在的源商品，被移除
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = originalItem.ConvertQuantity,
                            ProductSysNo = originalItem.ConvertProduct.SysNo,
                            StockSysNo = (int)originalEntity.Stock.SysNo
                        });
                    }
                }

                foreach (var newItem in newSourceItems)
                {
                    var originaltem = originalSourceItems.Find(p => p.ConvertProduct.SysNo == newItem.ConvertProduct.SysNo);

                    if (originaltem == null)
                    {
                        //只在新转换单中存在的源商品, 新增
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -newItem.ConvertQuantity,
                            ProductSysNo = newItem.ConvertProduct.SysNo,
                            StockSysNo = (int)originalEntity.Stock.SysNo
                        });
                    }
                }
            }          

            #endregion 库存调整逻辑

            ConvertRequestInfo result;
            
            using (var scope = new TransactionScope())
            {
                //更新转换单主信息
                result = convertRequestDA.UpdateConvertRequest(entityToUpdate);

                #region 更新转换单中的商品列表

                //清除原商品列表
                convertRequestDA.DeleteConvertItemByRequestSysNo((int)entityToUpdate.SysNo);

                //添加新转换单中商品
                if (newItems != null && newItems.Count > 0)
                {
                    result.ConvertItemInfoList = new List<ConvertRequestItemInfo>();

                    // product batch info
                    List<InventoryBatchDetailsInfo> sourceBatchList = new List<InventoryBatchDetailsInfo>();
                    List<InventoryBatchDetailsInfo> targetBatchList = new List<InventoryBatchDetailsInfo>();

                    newItems.ForEach(convertItem =>
                    {
                        PreCheckConvertItemInfo(convertItem);
                       
                        //Set ConvertItem UnitCost/UnitCostW/oTax
                        if (convertItem.ConvertType == ConvertProductType.Target)
                        {
                            //目标商品
                            //中蛋特殊逻辑，51上海仓目标商品去税成本特殊计算
                            //[Mark][Alan.X.Luo 特有逻辑]
                            if (entityToUpdate.Stock.SysNo == 51)
                            {
                                convertItem.ConvertUnitCostWithoutTax = convertItem.ConvertUnitCost / (decimal)1.17;
                            }
                            else
                            {
                                convertItem.ConvertUnitCostWithoutTax = convertItem.ConvertUnitCost;
                            }
                            targetBatchList.AddRange(convertItem.BatchDetailsInfoList);
                        }
                        else
                        {
                            //源商品
                            convertItem.ConvertUnitCost = convertItem.ConvertProduct.ProductPriceInfo.UnitCost;
                            convertItem.ConvertUnitCostWithoutTax = convertItem.ConvertProduct.ProductPriceInfo.UnitCostWithoutTax;
                            sourceBatchList.AddRange(convertItem.BatchDetailsInfoList);
                        }

                        result.ConvertItemInfoList.Add(convertRequestDA.CreateConvertItem(convertItem, (int)entityToUpdate.SysNo));
                    });

                    // batch info
                    if (sourceBatchList != null && sourceBatchList.Count > 0)
                    {
                        ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(sourceBatchList, result.SysNo.Value, "Convert", "Create");
                    }
                    if (targetBatchList != null && targetBatchList.Count > 0)
                    {
                        ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(targetBatchList, result.SysNo.Value, "Convert", "Target");
                    }
                }

                #endregion 更新转换单中的商品列表      
          
                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                scope.Complete();
            }

            //SetEntityBase(newEntity, result);

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"更新了编号为\"{1}\"的转换单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
            , BizLogType.St_Transfer_Master_Update, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);
  
            #endregion

            return result;
        }

        /// <summary>
        /// 更新转换单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo UpdateRequestStatus(ConvertRequestInfo entity)
        {
            return convertRequestDA.UpdateConvertRequestStatus(entity);
        }

        /// <summary>
        /// 转换单 - 作废操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo AbandonRequest(ConvertRequestInfo entityToUpdate)
        {
            PreCheckConvertRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedConvertItem(entityToUpdate);

            PreCheckOriginConvertRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            var originalItems = entityToUpdate.ConvertItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            if (originalItems != null && originalItems.Count > 0)
            {                
                foreach (var item in originalItems)
                {
                    if (item.ConvertType == ConvertProductType.Source)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = item.ConvertQuantity,
                            ProductSysNo = item.ConvertProduct.SysNo,
                            StockSysNo = (int)entityToUpdate.Stock.SysNo
                        });
                    }
                }

            }
            ConvertRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ConvertRequestStatus.Abandon;
                result = UpdateRequestStatus(entityToUpdate);

                result.ConvertItemInfoList = originalItems;

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                List<InventoryBatchDetailsInfo> InventoryBatchDetailsInfolist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(result.SysNo.Value).FindAll(x => { return x.ReturnQty == 0 && x.Quantity != 0; });
                if (InventoryBatchDetailsInfolist != null && InventoryBatchDetailsInfolist.Count > 0)
                {
                    #region 构建转换单作废调整批次库存表的SSB消息 调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in InventoryBatchDetailsInfolist)
                    {
                        ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                        itemBatchInfo.BatchNumber = item.BatchNumber;
                        itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                        Stock stock = new Stock();
                        stock.AllocatedQty = (-item.Quantity).ToString(); //作废 取消作废 单据 只调整 占用库存
                        stock.WarehouseNumber = item.StockSysNo.ToString();
                        List<Stock> StockList = new List<Stock>();
                        StockList.Add(stock);

                        Stocks stocks = new Stocks();
                        stocks.Stock = StockList;

                        itemBatchInfo.Stocks = stocks;
                        itemBatchInfoList.Add(itemBatchInfo);
                    }
                    string InUser = ServiceContext.Current.UserDisplayName;
                    BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                    {
                        Header = new InventoryHeader()
                        {
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "Abandon",
                            Version = "V10",
                            Type = "Convert",
                            CompanyCode = "8601",
                            Tag = "ConvertAbandon",
                            Language = "zh-CN",
                            From = "IPP",
                            GlobalBusinessType = "Listing",
                            StoreCompanyCode = "8601",
                            TransactionCode = result.SysNo.ToString()
                        },
                        Body = new InventoryBody
                        {
                            InUser = InUser,
                            Number = result.SysNo.ToString(),
                            ItemBatchInfo = itemBatchInfoList
                        }
                    };
                    string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                    XmlDocument xmlD = new XmlDocument();
                    xmlD.LoadXml(paramXml);
                    paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                    ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量
                    #endregion
                }                           

                //作废转换单发送Message
                EventPublisher.Publish(new VoidConvertRequestInfoMessage() 
                {
                    ConvertRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });


                scope.Complete();
            }

            //SetEntityBase(newEntity, result);
            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"作废了编号为\"{1}\"的转换单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
            , BizLogType.St_Transfer_Abandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 转换单 - 取消作废操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo CancelAbandonRequest(ConvertRequestInfo entityToUpdate)
        {
            PreCheckConvertRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedConvertItem(entityToUpdate);

            PreCheckOriginConvertRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            var originalItems = entityToUpdate.ConvertItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);
            if (originalItems != null && originalItems.Count > 0)
            {
                foreach (var item in originalItems)
                {
                    if (item.ConvertType == ConvertProductType.Source)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -item.ConvertQuantity,
                            ProductSysNo = item.ConvertProduct.SysNo,
                            StockSysNo = (int)entityToUpdate.Stock.SysNo
                        });
                    }
                }
            }
            ConvertRequestInfo result;

            using (var scope = new TransactionScope())
            {
                //result = convertRequestDA.AbandonOrCancelAbandonConvert(newEntity);
                entityToUpdate.RequestStatus = ConvertRequestStatus.Origin;
                result = UpdateRequestStatus(entityToUpdate);
                result.ConvertItemInfoList = originalItems;

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }
                List<InventoryBatchDetailsInfo> InventoryBatchDetailsInfolist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(result.SysNo.Value).FindAll(x => { return x.ReturnQty == 0 && x.Quantity != 0; });
                if (InventoryBatchDetailsInfolist != null && InventoryBatchDetailsInfolist.Count > 0)
                {
                    #region 构建转换单取消作废调整批次库存表的SSB消息 调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in InventoryBatchDetailsInfolist)
                    {
                        ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                        itemBatchInfo.BatchNumber = item.BatchNumber;
                        itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                        Stock stock = new Stock();
                        stock.AllocatedQty = item.Quantity.ToString(); //作废 取消作废 单据 只调整 占用库存
                        stock.WarehouseNumber = item.StockSysNo.ToString();
                        List<Stock> StockList = new List<Stock>();
                        StockList.Add(stock);

                        Stocks stocks = new Stocks();
                        stocks.Stock = StockList;

                        itemBatchInfo.Stocks = stocks;
                        itemBatchInfoList.Add(itemBatchInfo);
                    }
                    string InUser = ServiceContext.Current.UserDisplayName;
                    BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                    {
                        Header = new InventoryHeader()
                        {
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "CancelAbandon",
                            Version = "V10",
                            Type = "Convert",
                            CompanyCode = "8601",
                            Tag = "ConvertCancelAbandon",
                            Language = "zh-CN",
                            From = "IPP",
                            GlobalBusinessType = "Listing",
                            StoreCompanyCode = "8601",
                            TransactionCode = result.SysNo.ToString()
                        },
                        Body = new InventoryBody
                        {
                            InUser = InUser,
                            Number = result.SysNo.ToString(),
                            ItemBatchInfo = itemBatchInfoList
                        }
                    };
                    string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                    XmlDocument xmlD = new XmlDocument();
                    xmlD.LoadXml(paramXml);
                    paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                    ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量
                    #endregion
                }         
                //取消作废发送message
                EventPublisher.Publish(new CancelVoidConvertRequestInfoMessage() 
                {
                    ConvertRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });
                scope.Complete();
            }

            //SetEntityBase(newEntity, result);
            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消作废了编号为\"{1}\"的转换单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
            , BizLogType.St_Transfer_CancelAbandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);
    
            #endregion

            return result;
        }

        /// <summary>
        /// 转换单 - 审核操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo VerifyRequest(ConvertRequestInfo entityToUpdate)
        {
            PreCheckConvertRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedConvertItem(entityToUpdate);

            PreCheckOriginConvertRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Audit);

            PreCheckConvertCost(entityToUpdate);

            ConvertRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.AuditDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();
                entityToUpdate.RequestStatus = ConvertRequestStatus.Verified;
                result = UpdateRequestStatus(entityToUpdate);

                //审核转换单发送message
                EventPublisher.Publish(new AuditConvertRequestInfoMessage() { 
                    ConvertRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            result.ConvertItemInfoList = entityToUpdate.ConvertItemInfoList;

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"审核了编号为\"{1}\"的转换单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
              , BizLogType.St_Transfer_Verify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);
          
            #endregion

            return result;
        }

        /// <summary>
        /// 转换单 - 取消审核操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo CancelVerifyRequest(ConvertRequestInfo entityToUpdate)
        {
            PreCheckConvertRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedConvertItem(entityToUpdate);

            PreCheckOriginConvertRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAudit);

            ConvertRequestInfo result;

            using (var scope = new TransactionScope())
            {   
                entityToUpdate.AuditDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();
                entityToUpdate.RequestStatus = ConvertRequestStatus.Origin;
                result = UpdateRequestStatus(entityToUpdate);

                ///取消审核发送message
                EventPublisher.Publish(new CancelAuditConvertRequestInfoMessage()
                { 
                    ConvertRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            result.ConvertItemInfoList = entityToUpdate.ConvertItemInfoList;

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消审核了编号为\"{1}\"的转换单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Transfer_CancelVerify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);
            
            #endregion

            return result;
        }

        /// <summary>
        /// 转换单 - 出库操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public virtual ConvertRequestInfo OutStockRequest(ConvertRequestInfo entityToUpdate)
        {
            PreCheckConvertRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedConvertItem(entityToUpdate);

            PreCheckOriginConvertRequestInfo(entityToUpdate, InventoryAdjustSourceAction.OutStock);

            PreCheckConvertCost(entityToUpdate);

            var originalItems = entityToUpdate.ConvertItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.OutStock);
            
            foreach (var originalItem in originalItems)
            {
                if (originalItem.ConvertType == ConvertProductType.Target)
                {                    
                    //TODO:需要Check更新UnitCost的逻辑，并入库存调整接口中
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = originalItem.ConvertQuantity,
                        ProductSysNo = originalItem.ConvertProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.Stock.SysNo,
                        AdjustItemBizFlag = (int)originalItem.ConvertType,
                        AdjustUnitCost = originalItem.ConvertUnitCost,
                    });
                }
                else
                {
                    //TODO:需要Check更新UnitCost的逻辑，并入库存调整接口中
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = -originalItem.ConvertQuantity,
                        ProductSysNo = originalItem.ConvertProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.Stock.SysNo,
                        AdjustItemBizFlag = (int)originalItem.ConvertType,
                        AdjustUnitCost = originalItem.ConvertUnitCost,
                    });
                }
            }

            foreach (var originalItem in originalItems)
            {                
                if (originalItem.ConvertType == ConvertProductType.Source)
                {
                    if (originalItem.ConvertProduct.ProductPriceInfo == null)
                    {
                        BizExceptionHelper.Throw("Common_CannotFindPriceInformation", originalItem.ConvertProduct.SysNo.ToString());
                    }

                    if (originalItem.ConvertUnitCost != originalItem.ConvertProduct.ProductPriceInfo.UnitCost)
                    {
                        BizExceptionHelper.Throw("InventoryConvert_UnitCostbeChanged", originalItem.ConvertProduct.ProductID);
                    }
                }
            }
            
            ConvertRequestInfo result;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                entityToUpdate.OutStockDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();
                entityToUpdate.RequestStatus = ConvertRequestStatus.OutStock;
                result = UpdateRequestStatus(entityToUpdate);

                string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                if (!string.IsNullOrEmpty(adjustResult))
                {
                    throw new BizException("调整库存失败: " + adjustResult);
                }

                string InUser = ServiceContext.Current.UserDisplayName;
                //源商品调整库存
                List<InventoryBatchDetailsInfo> InventoryBatchDetailsInfolist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(result.SysNo.Value).FindAll(x => { return x.ReturnQty == 0 && x.Quantity != 0; });
                if (InventoryBatchDetailsInfolist != null && InventoryBatchDetailsInfolist.Count > 0)
                {
                    #region 构建转换单出库调整批次库存表的SSB消息 调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in InventoryBatchDetailsInfolist)
                    {
                        ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                        itemBatchInfo.BatchNumber = item.BatchNumber;
                        itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                        Stock stock = new Stock();
                        stock.Quantity = (-item.Quantity).ToString(); //单据出库两个数量都要调整
                        stock.AllocatedQty = (-item.Quantity).ToString(); //单据出库两个数量都要调整
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
                        Header = new InventoryHeader()
                        {
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "OutStock",
                            Version = "V10",
                            Type = "Convert",
                            CompanyCode = "8601",
                            Tag = "ConvertOutStock",
                            Language = "zh-CN",
                            From = "IPP",
                            GlobalBusinessType = "Listing",
                            StoreCompanyCode = "8601",
                            TransactionCode = result.SysNo.ToString()
                        },
                        Body = new InventoryBody
                        {
                            InUser = InUser,
                            Number = result.SysNo.ToString(),
                            ItemBatchInfo = itemBatchInfoList
                        }
                    };
                    string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                    XmlDocument xmlD = new XmlDocument();
                    xmlD.LoadXml(paramXml);
                    paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                    ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);//调整批次库存表 占用数量
                    #endregion

                    //目标商品调整库存
                    List<InventoryBatchDetailsInfo> TargetInventoryBatchDetailsInfolist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(result.SysNo.Value).FindAll(x => { return x.ReturnQty != 0 && x.Quantity == 0; });
                    if (TargetInventoryBatchDetailsInfolist != null && TargetInventoryBatchDetailsInfolist.Count > 0)
                    {
                        #region 构建转换单出库调整批次库存表的SSB消息 调整库存
                        List<ItemBatchInfo> targetItemBatchInfoList = new List<ItemBatchInfo>();
                        foreach (var item in TargetInventoryBatchDetailsInfolist)
                        {
                            ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                            itemBatchInfo.BatchNumber = item.BatchNumber;
                            itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                            Stock stock = new Stock();
                            stock.Quantity = item.ReturnQty.ToString(); //单据出库 目标商品库存增加
                            stock.WarehouseNumber = item.StockSysNo.ToString();
                            List<Stock> StockList = new List<Stock>();
                            StockList.Add(stock);

                            Stocks stocks = new Stocks();
                            stocks.Stock = StockList;

                            itemBatchInfo.Stocks = stocks;
                            targetItemBatchInfoList.Add(itemBatchInfo);
                        }
                        BatchXMLMessage targetBatchXMLMessage = new BatchXMLMessage()
                        {
                            Header = new InventoryHeader()
                            {
                                NameSpace = "http://soa.newegg.com/InventoryProfile",
                                Action = "OutStock",
                                Version = "V10",
                                Type = "Convert",
                                CompanyCode = "8601",
                                Tag = "ConvertOutStock",
                                Language = "zh-CN",
                                From = "IPP",
                                GlobalBusinessType = "Listing",
                                StoreCompanyCode = "8601",
                                TransactionCode = result.SysNo.ToString()
                            },
                            Body = new InventoryBody
                            {
                                InUser = InUser,
                                Number = result.SysNo.ToString(),
                                ItemBatchInfo = targetItemBatchInfoList
                            }
                        };
                        string targetParamXml = SerializationUtility.XmlSerialize(targetBatchXMLMessage);
                        XmlDocument targetXMLD = new XmlDocument();
                        targetXMLD.LoadXml(targetParamXml);
                        targetParamXml = "<" + targetXMLD.DocumentElement.Name + ">" + targetXMLD.DocumentElement.InnerXml + "</" + targetXMLD.DocumentElement.Name + ">";

                        ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(targetParamXml);//调整批次库存表 占用数量
                        #endregion
                    }
                }
              

                //获取 非批次商品更改的信息, Send SSB to WMS
                List<InventoryAdjustItemInfo> adjustCaseEntityList = new List<InventoryAdjustItemInfo>();
                foreach (var item in inventoryAdjustContract.AdjustItemList)
                {
                    if (!ObjectFactory<IAdjustRequestDA>.Instance.CheckISBatchNumberProduct(item.ProductSysNo))
                    {
                        item.AdjustQuantity = item.AdjustQuantity;

                        adjustCaseEntityList.Add(item);
                    }
                }

                ConvertSendSSBToWMS(result.SysNo.Value, inventoryAdjustContract.AdjustItemList[0].StockSysNo.ToString(), adjustCaseEntityList);//转换单出库向仓库发送SSB消息

                SendSSBToWMS();

                //出库发送message
                EventPublisher.Publish(new OutStockConvertRequestInfoMessage() 
                {
                    ConvertRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            //检查成本更新逻辑
            //adjustBP.TryUpdateItemUnitCost();

            result.ConvertItemInfoList = originalItems;

            //SetEntityBase(newEntity, result);

            #region write log
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"出库了编号为\"{1}\"的转换单", entityToUpdate.OutStockUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Transfer_OutStock, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);
            
            #endregion

            return result;
        }

        #region 私有方法

        #region 库存调整

        private InventoryAdjustContractInfo InitInventoryAdjustContract(ConvertRequestInfo entity, InventoryAdjustSourceAction sourceAction)
        {
            var inventoryAdjustContract = new InventoryAdjustContractInfo
            {
                SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_ConvertRequest,
                SourceActionName = sourceAction,
                ReferenceSysNo = entity.SysNo.ToString(),
                AdjustItemList = new List<InventoryAdjustItemInfo>()
            };
            return inventoryAdjustContract;
        }

        #endregion 库存调整

        #region PreCheck

        private void PreCheckConvertRequestInfoForCreate(ConvertRequestInfo entityToCreate)
        {
            if (entityToCreate == null)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertSheetNotCompletelyforAdd");
            }

            if (entityToCreate.Stock == null || entityToCreate.Stock.SysNo == null ||entityToCreate.Stock.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertSheetNotCompletelyforAdd");
            }

            PreCheckConvertProduct(entityToCreate);
        }

        private void PreCheckConvertRequestInfoForUpdate(ConvertRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertSheetNotCompletelyforAdd");
            }

            if (entityToUpdate.SysNo == null || entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertSheetNotCompletelyforAdd");
            }

            PreCheckConvertProduct(entityToUpdate);
        }

        private void PreCheckConvertProduct(ConvertRequestInfo entity)
        {
            var consignItems = entity.ConvertItemInfoList.FindAll(p => { return p.ConvertProduct.ProductConsignFlag == VendorConsignFlag.Consign; });
            if (consignItems != null && consignItems.Count > 0)
            {
                throw new BizException("转换单中不能包含代销商品！");
            }
            var sourceItems = entity.ConvertItemInfoList.FindAll(p => { return p.ConvertType == ConvertProductType.Source; });
            if (sourceItems == null || sourceItems.Count <= 0)
            {
                throw new BizException("请选择要转换的源商品！");
            }
            var targetItems = entity.ConvertItemInfoList.FindAll(p => { return p.ConvertType == ConvertProductType.Target; });
            if (targetItems == null || targetItems.Count <= 0)
            {
                throw new BizException("请选择要转换的目标商品！");
            }
        }

        private void PreCheckConvertRequestInfoForUpdateStatus(ConvertRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertSheetNotCompletelyforAdd");
            }

            if (entityToUpdate.SysNo == null || entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertSheetNotCompletelyforAdd");
            }
        }

        private void PreCheckOriginConvertRequestInfo(ConvertRequestInfo entity, InventoryAdjustSourceAction actionType)
        {
            if (entity == null)
            {
                BizExceptionHelper.Throw("InventoryLend_cannotFindOriginalLendSheet");
            }

            //借货单当前状态检查
            if ((actionType == InventoryAdjustSourceAction.Update || actionType == InventoryAdjustSourceAction.Audit
                || actionType == InventoryAdjustSourceAction.Abandon) && entity.RequestStatus != ConvertRequestStatus.Origin)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotOrigin");            
            }
            else if (actionType == InventoryAdjustSourceAction.CancelAbandon && entity.RequestStatus != ConvertRequestStatus.Abandon)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotAbandon");                
            }
            else if ((actionType == InventoryAdjustSourceAction.CancelAudit || actionType == InventoryAdjustSourceAction.OutStock)
                && entity.RequestStatus != ConvertRequestStatus.Verified)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotVerify");                
            }

            //if (entity.CreateUser != null && entity.CreateUser.SysNo == ServiceContext.Current.UserSysNo&&actionType== InventoryAdjustSourceAction.Audit)
            //{
            //    throw new BizException("创建人和审核人不能相同");
            //}

        }

        private void PreCheckDuplicatedConvertItem(ConvertRequestInfo entityToUpdate)
        {
            List<ConvertRequestItemInfo> itemEntities;
            
            //--------------------------remove the duplicated item---------------------------------------------------------------------------------
            if (entityToUpdate.ConvertItemInfoList != null && entityToUpdate.ConvertItemInfoList.Count > 0)
            {
                itemEntities = new List<ConvertRequestItemInfo>();

                foreach (ConvertRequestItemInfo ci in entityToUpdate.ConvertItemInfoList)
                {   
                    if (itemEntities.Find(p => p.ConvertProduct.SysNo == ci.ConvertProduct.SysNo) != null)
                    {
                        BizExceptionHelper.Throw("InventoryConvert_ItemCannotSame", ci.ConvertProduct.SysNo.ToString());
                    }

                    itemEntities.Add(ci);
                }
            }
        }

        private void PreCheckConvertItemInfo(ConvertRequestItemInfo convertItemInfo)
        {
            if (convertItemInfo.ConvertProduct == null || convertItemInfo.ConvertProduct.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_ItemIdCannotLessThanZero");
            }

            if (convertItemInfo.ConvertType != ConvertProductType.Target && convertItemInfo.ConvertType != ConvertProductType.Source)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertTypeIsNotValid");
            }

            if (convertItemInfo.ConvertQuantity <= 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_ConvertQtyCannotLessThanZero", convertItemInfo.ConvertProduct.SysNo.ToString());
            }

            if (convertItemInfo.ConvertType == ConvertProductType.Target)
            {
                if (convertItemInfo.ConvertUnitCost < 0)
                {
                    BizExceptionHelper.Throw("InventoryConvert_ConvertCostCannotLessThanZero", convertItemInfo.ConvertProduct.SysNo.ToString());
                }              
            }
            else
            {
                if (convertItemInfo.ConvertProduct.ProductPriceInfo == null)
                {
                    BizExceptionHelper.Throw("Common_CannotFindPriceInformation", convertItemInfo.ConvertProduct.SysNo.ToString());
                }
            }
        }

        private void PreCheckConvertCost(ConvertRequestInfo entityToUpdate)
        {
            var originalItems = entityToUpdate.ConvertItemInfoList;

            var sourceTotalCost = (decimal)0;
            var targetTotalCost = (decimal)0;

            var totalSourcItems = 0;
            var totalTargetItems = 0;

            foreach (var itemEntity in originalItems)
            {
                if (itemEntity.ConvertType == ConvertProductType.Target)
                {
                    totalTargetItems++;
                    targetTotalCost += itemEntity.ConvertUnitCost * itemEntity.ConvertQuantity;
                }
                else
                {
                    totalSourcItems++;
                    sourceTotalCost += itemEntity.ConvertUnitCost * itemEntity.ConvertQuantity;
                }
            }

            if (totalSourcItems == 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_LackSourceItems", entityToUpdate.RequestID);
            }

            if (totalTargetItems == 0)
            {
                BizExceptionHelper.Throw("InventoryConvert_LackTargetItems",  entityToUpdate.RequestID);
            }

            if (Math.Abs(sourceTotalCost - targetTotalCost) >= 0.09M)
            {
                BizExceptionHelper.Throw("InventoryConvert_CostNotEqual");
            }          
        }
        #endregion PreCheck

        #region Interact with Other Domain

        private void SendSSBToWMS()
        {

        }

        #endregion Interact with Other Domain

        private void ConvertSendSSBToWMS(int sysNo, string inWarehouseNumber, List<InventoryAdjustItemInfo> adjustCaseEntityList)
        {
            List<ECCentral.BizEntity.Inventory.Item> itemList = new List<BizEntity.Inventory.Item>();
            List<InventoryBatchDetailsInfo> batchDetailsInfoEntity =  ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(sysNo);

            if ((batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0) || (adjustCaseEntityList != null && adjustCaseEntityList.Count > 0))
            {
                if (batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0)
                {
                    foreach (var BatchItem in batchDetailsInfoEntity)
                    {
                        ProductBatch pbEntity = new ProductBatch();
                        pbEntity.BatchNumber = BatchItem.BatchNumber;
                        if (BatchItem.Quantity > 0 && BatchItem.ReturnQty == 0)
                        {
                            pbEntity.Quantity = (-BatchItem.Quantity).ToString();
                        }
                        if (BatchItem.Quantity == 0 && BatchItem.ReturnQty > 0)
                        {
                            pbEntity.Quantity = BatchItem.ReturnQty.ToString();
                        }
                        ECCentral.BizEntity.Inventory.Item item = new ECCentral.BizEntity.Inventory.Item()
                        {
                            ProductBatch = pbEntity,
                            ProductSysNo = BatchItem.ProductSysNo.ToString(),
                            Quantity = pbEntity.Quantity.ToString()
                        };
                        itemList.Add(item);
                    }
                }
                if (adjustCaseEntityList != null && adjustCaseEntityList.Count > 0)
                {
                    //初始非批次商品调整信息
                    foreach (var NOTBatchItem in adjustCaseEntityList)
                    {
                        ECCentral.BizEntity.Inventory.Item item = new ECCentral.BizEntity.Inventory.Item()
                        {
                            ProductSysNo = NOTBatchItem.ProductSysNo.ToString()
                            ,
                            Quantity = NOTBatchItem.AdjustQuantity.ToString()
                        };
                        itemList.Add(item);
                    }
                }

                SendToWMSSSBXMLMessage sendTOWMSSSBXMLMessage = new SendToWMSSSBXMLMessage()
                {
                    RequestRoot = new RequestRoot()
                    {
                        MessageHeader = new BizEntity.Inventory.MessageHeader
                        {
                            Language = "CH",
                            Sender = "IPP",
                            CompanyCode = "8601",
                            Action = "Adjust",
                            Version = "0.1",
                            Type = "InventoryAdjust",
                            OriginalGUID = ""
                        },
                        Body = new BizEntity.Inventory.Body
                        {
                            Operation = new Operation()
                            {
                                Type = "80",
                                Number = sysNo.ToString(),
                                User = ServiceContext.Current.UserDisplayName,
                                Memo = "转换单出库给仓库发送SSB",
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
                    ObjectFactory<IAdjustRequestDA>.Instance.SendSSBToWMS(paramXml);
                }
            }
        }        

        #endregion

    }
}
