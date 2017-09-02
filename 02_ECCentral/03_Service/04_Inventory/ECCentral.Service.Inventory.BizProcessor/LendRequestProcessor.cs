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
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.Inventory;
using System.Xml;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(LendRequestProcessor))]
    public class LendRequestProcessor
    {
        private ILendRequestDA lendRequestDA = ObjectFactory<ILendRequestDA>.Instance;

        /// <summary>
        /// 根据SysNo获取借货单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo GetLendRequestInfoBySysNo(int requestSysNo)
        {
            LendRequestInfo lendRequest = lendRequestDA.GetLendRequestInfoBySysNo(requestSysNo);
            if (lendRequest != null)
            {
                lendRequest.LendItemInfoList = lendRequestDA.GetLendItemListByRequestSysNo(requestSysNo);
                lendRequest.ReturnItemInfoList = lendRequestDA.GetReturnItemListByRequestSysNo(requestSysNo);
            }

            return lendRequest;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual LendRequestInfo GetProductLineInfo(int ProductSysNo)
        {
            LendRequestInfo lendRequest = lendRequestDA.GetProductLineInfo(ProductSysNo);
            return lendRequest;
        }

        /// <summary>
        /// 创建借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CreateRequest(LendRequestInfo entityToCreate)
        {
            PreCheckLendRequestInfoForCreate(entityToCreate);

            PreCheckDuplicatedLendItem(entityToCreate);

            LendRequestInfo result;
            var newItems = entityToCreate.LendItemInfoList;

            using (var scope = new TransactionScope())
            {
                entityToCreate.RequestStatus = LendRequestStatus.Origin;
                entityToCreate.CreateDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();                

                int requestSysNo = lendRequestDA.GetLendRequestSequence();
                entityToCreate.SysNo = requestSysNo;

                //TODO: 特殊逻辑，枚举化
                entityToCreate.RequestID = "55" + requestSysNo.ToString().PadLeft(8, '0');

                result = lendRequestDA.CreateLendRequest(entityToCreate);

                var inventoryAdjustContract = InitInventoryAdjustContract(entityToCreate, InventoryAdjustSourceAction.Create);

                if (newItems != null && newItems.Count > 0)
                {
                    // product batch info
                    List<InventoryBatchDetailsInfo> batchList = new List<InventoryBatchDetailsInfo>();

                    result.LendItemInfoList = new List<LendRequestItemInfo>();
                    newItems.ForEach(item =>
                    {
                        
                        // batch info
                        if (item.BatchDetailsInfoList != null && item.BatchDetailsInfoList.Count > 0)
                        {
                            batchList.AddRange(item.BatchDetailsInfoList);
                        }

                        PreCheckLendItemInfo(item);

                        //item.LendSysNumber = entity.LendSysNumber;
                        item.LendUnitCost = item.LendProduct.ProductPriceInfo.UnitCost;//itemPrice.UnitCost;
                        item.LendUnitCostWhenCreate = item.LendProduct.ProductPriceInfo.UnitCost;//itemPrice.UnitCost;
                        item.LendUnitCostWithoutTax = item.LendProduct.ProductPriceInfo.UnitCostWithoutTax;//itemPrice.UnitCostWithoutTax;

                        result.LendItemInfoList.Add(lendRequestDA.CreateLendItem(item, (int)entityToCreate.SysNo));

                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo()
                        {
                            AdjustQuantity = -item.LendQuantity,
                            ProductSysNo = item.LendProduct.SysNo,
                            StockSysNo = (int)entityToCreate.Stock.SysNo
                        });
                    });

                    // batch info
                    if (batchList != null && batchList.Count > 0)
                    {
                        ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(batchList, result.SysNo.Value, "Lend", "Create");
                    }

                    //InventoryAdjustBP.AdjustInventory(inventoryAdjustEntity);
                    //调整库存统一接口
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                //创建借货单发送message
                //EventPublisher.Publish(new CreateLendRequestInfoMessage()
                //{
                //    LendRequestInfoSysNo = entityToCreate.SysNo.Value,
                //    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                //});

                scope.Complete();
            }

            #region send email

            //var from = BusinessConfig.Default.Message_Email_From;
            //var to = BusinessConfig.Default.Lend_Message_Email_To;
            //var subject = InfoMessage.Lend_Message_Email_Subject;
            //var description = string.Format(BusinessConfig.Default.Lend_Message_Email_Body, entity.LendSysNumber);
            //var body = string.Format(InfoMessage.Message_Email_Body, entity.LendCode, result.CreateUserName, result.StockName, BusinessUtility.GetCurrentDateTime(), result.Note, description);

            //MailService.SendMail2IPP3Internal(from, to, subject, body);

            #endregion

            //SetEntityBase(entity, result);

            #region write log
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"创建了编号为\"{1}\"的借货单", entityToCreate.CreateUser.SysNo, entityToCreate.RequestID)
                , BizLogType.St_Lend_Master_Insert, (int)entityToCreate.SysNo, entityToCreate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 更新借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo UpdateRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedLendItem(entityToUpdate);

            #region check params

            var originalEntity = GetLendRequestInfoBySysNo((int)entityToUpdate.SysNo);

            PreCheckOriginLendRequestInfo(originalEntity, InventoryAdjustSourceAction.Update);

            #endregion

            var originalItems = lendRequestDA.GetLendItemListByRequestSysNo((int)entityToUpdate.SysNo);
            if (originalItems.Count == 0)
            {
                originalItems = null;
            }

            var newItems = entityToUpdate.LendItemInfoList;
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Update);

            #region 库存调整逻辑
            if (originalItems == null && newItems != null)
            {
                //原借货单中没有商品， 全部为新增商品
                foreach (var newItem in newItems)
                {
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = -newItem.LendQuantity,
                        ProductSysNo = newItem.LendProduct.SysNo,
                        StockSysNo = (int)originalEntity.Stock.SysNo
                    });
                }
            }
            else if (originalItems != null && newItems == null)
            {
                //原借货单中的商品全被移除
                foreach (var originalItem in originalItems)
                {
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = originalItem.LendQuantity,
                        ProductSysNo = originalItem.LendProduct.SysNo,
                        StockSysNo = (int)originalEntity.Stock.SysNo
                    });
                }
            }
            else if (originalItems != null)
            {
                //原借货单与新的借货单均有商品，需逐条对比
                foreach (var originalItem in originalItems)
                {
                    var entity = originalItem;
                    var newItem = newItems.Find(p => p.LendProduct.SysNo == entity.LendProduct.SysNo);

                    if (newItem != null)
                    {
                        //新/旧借货单均包含的借货商品

                        var adjustQty = originalItem.LendQuantity - newItem.LendQuantity;
                        if (adjustQty != 0)
                        {
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = adjustQty,
                                ProductSysNo = originalItem.LendProduct.SysNo,
                                StockSysNo = (int)originalEntity.Stock.SysNo
                            });
                        }
                        //continue;
                    }
                    else
                    {
                        //原借货单包含但在新借货单移除的商品
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = originalItem.LendQuantity,
                            ProductSysNo = originalItem.LendProduct.SysNo,
                            StockSysNo = (int)originalEntity.Stock.SysNo
                        });
                    }
                }

                foreach (var newItem in newItems)
                {
                    var entity = newItem;
                    var originalLendItem = originalItems.Find(p => p.LendProduct.SysNo == entity.LendProduct.SysNo);

                    if (originalLendItem == null)
                    {
                        //原借货单不包含，新借货单中新增的商品
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -newItem.LendQuantity,
                            ProductSysNo = newItem.LendProduct.SysNo,
                            StockSysNo = (int)originalEntity.Stock.SysNo
                        });
                    }
                }
            }
            #endregion 库存调整逻辑

            LendRequestInfo result;

            using (var scope = new TransactionScope())
            {
                // product batch info
                List<InventoryBatchDetailsInfo> batchList = new List<InventoryBatchDetailsInfo>();

                result = lendRequestDA.UpdateLendRequest(entityToUpdate);

                if (originalItems != null)
                {
                    lendRequestDA.DeleteLendItemByRequestSysNo((int)entityToUpdate.SysNo);
                }

                #region update lend item's price

                if (newItems != null)
                {
                    result.LendItemInfoList = new List<LendRequestItemInfo>();

                    
                    StringBuilder deleteItemSysNoList = new StringBuilder();

                    foreach (var item in newItems)
                    {
                        PreCheckLendItemInfo(item);

                        // batch info
                        deleteItemSysNoList.AppendFormat("{0},",item.LendProduct.ProductID);

                        if (item.BatchDetailsInfoList != null && item.BatchDetailsInfoList.Count > 0)
                        {
                            batchList.AddRange(item.BatchDetailsInfoList);
                        }


                        LendRequestItemInfo oldItem = null;
                        if (originalItems != null)
                        {
                            oldItem = originalItems.Find(p => p.LendProduct.SysNo == item.LendProduct.SysNo);
                        }

                        //item.LendSysNumber = entity.LendSysNumber;
                        item.LendUnitCost = item.LendProduct.ProductPriceInfo.UnitCost;//itemPrice.UnitCost;
                        item.LendUnitCost = oldItem != null ? oldItem.LendUnitCostWhenCreate : item.LendProduct.ProductPriceInfo.UnitCost; //itemPrice.UnitCost;
                        item.LendUnitCostWithoutTax = item.LendProduct.ProductPriceInfo.UnitCostWithoutTax; //itemPrice.UnitCostWithoutTax;

                        result.LendItemInfoList.Add(lendRequestDA.CreateLendItem(item, (int)entityToUpdate.SysNo));
                    }

                    // batch info
                    if (batchList != null && batchList.Count > 0)
                    {
                        ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(batchList, result.SysNo.Value, "Lend", "Create");
                    }

                }

                #endregion

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

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"更新了编号为\"{1}\"的借货单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Lend_Master_Update, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            return result;
            #endregion
        }

        /// <summary>
        /// 更新借货单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo UpdateRequestStatus(LendRequestInfo entity)
        {
            //PreCheck
            return lendRequestDA.UpdateLendRequestStatus(entity);
        }

        /// <summary>
        /// 审核借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo VerifyRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdateStatus(entityToUpdate);

            //var originalEntity = GetLendRequestInfoBySysNo((int)entityToUpdate.SysNo);
            PreCheckOriginLendRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Audit);

            var originalItems = lendRequestDA.GetLendItemListByRequestSysNo((int)entityToUpdate.SysNo);

            if (originalItems == null || originalItems.Count <= 0)
            {
                throw new BizException("WarningMessage.InventoryLend_hasNotItemValue");
            }

            LendRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.AuditDate = DateTime.Now;
                entityToUpdate.RequestStatus = LendRequestStatus.Verified;
                result = UpdateRequestStatus(entityToUpdate);

                //EventPublisher.Publish(new AuditLendRequestInfoMessage()
                //{
                //    LendRequestInfoSysNo = entityToUpdate.SysNo.Value,
                //    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                //});

                scope.Complete();
            }

            result.LendItemInfoList = originalItems;

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"审核了编号为\"{1}\"的借货单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Lend_Verify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 取消审核借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CancelVerifyRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginLendRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAudit);

            LendRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = LendRequestStatus.Origin;
                entityToUpdate.AuditDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();

                result = UpdateRequestStatus(entityToUpdate);

                //取消审核发送message
                EventPublisher.Publish(new CancelAuditLendRequestInfoMessage()
                {
                    LendRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();

            }

            result.LendItemInfoList = lendRequestDA.GetLendItemListByRequestSysNo((int)entityToUpdate.SysNo);

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消审核了编号为\"{1}\"的借货单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Lend_CancelVerify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);


            #endregion

            return result;
        }

        /// <summary>
        /// 作废借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo AbandonRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginLendRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            var originalItems = entityToUpdate.LendItemInfoList;

            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            if (originalItems != null && originalItems.Count > 0)
            {
                foreach (var lendItemEntity in originalItems)
                {
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = lendItemEntity.LendQuantity,
                        ProductSysNo = lendItemEntity.LendProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.Stock.SysNo
                    });
                }

            }

            LendRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = LendRequestStatus.Abandon;

                result = UpdateRequestStatus(entityToUpdate);
                result.LendItemInfoList = originalItems;

                //作废借货单
                //EventPublisher.Publish(new VoidLendRequestInfoMessage()
                //{
                //    LendRequestInfoSysNo = entityToUpdate.SysNo.Value,
                //    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                //});

                string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                if (!string.IsNullOrEmpty(adjustResult))
                {
                    throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                }

                //BatchProduct SSB to WMS

                List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(entityToUpdate.SysNo.Value);
                if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
                {
                    #region 构建借货单作废调整批次库存表的SSB消息 调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in batchDetailsInfoEntitylist)
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
                        Header = new InventoryHeader
                        {
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "Abandon",
                            Version = "V10",
                            Type = "Lend",
                            CompanyCode = "8601",
                            Tag = "LendAbandon",
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

                scope.Complete();
            }

            //SetEntityBase(newEntity, result);

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"作废了编号为\"{1}\"的借货单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Lend_Abandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 取消作废借货单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo CancelAbandonRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginLendRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            var originalItems = entityToUpdate.LendItemInfoList;

            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            if (originalItems != null && originalItems.Count > 0)
            {
                foreach (var lendItemEntity in originalItems)
                {
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = -lendItemEntity.LendQuantity,
                        ProductSysNo = lendItemEntity.LendProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.Stock.SysNo
                    });
                }
            }

            LendRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = LendRequestStatus.Origin;

                result = UpdateRequestStatus(entityToUpdate);
                result.LendItemInfoList = originalItems;

                //取消作废
                EventPublisher.Publish(new CancelVoidLendRequestInfoMessage()
                {
                    LendRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                if (!string.IsNullOrEmpty(adjustResult))
                {
                    throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                }

                //BatchProduct SSB to WMS
                List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(result.SysNo.Value);
                if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
                {
                    #region 构建借货单取消作废调整批次库存表的SSB消息 调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in batchDetailsInfoEntitylist)
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
                            Type = "Lend",
                            CompanyCode = "8601",
                            Tag = "LendCancelAbandon",
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

                scope.Complete();
            }

            #region write log


            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消作废了编号为\"{1}\"的借货单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Lend_CancelAbandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 借货出库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual LendRequestInfo OutStockRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginLendRequestInfo(entityToUpdate, InventoryAdjustSourceAction.OutStock);

            var originalItems = entityToUpdate.LendItemInfoList;

            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.OutStock);

            LendRequestInfo result;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                entityToUpdate.RequestStatus = LendRequestStatus.OutStock;

                entityToUpdate.OutStockDate = DateTime.Now;// BusinessUtility.GetCurrentDateTime();

                result = UpdateRequestStatus(entityToUpdate);

                result.LendItemInfoList = new List<LendRequestItemInfo>();

                foreach (var item in originalItems)
                {
                    PreCheckLendItemInfo(item);

                    var oldItem = originalItems.Find(p => p.LendProduct.SysNo == item.LendProduct.SysNo);

                    item.LendUnitCost = item.LendProduct.ProductPriceInfo.UnitCost;//itemPrice.UnitCost;
                    item.LendUnitCost = oldItem != null ? oldItem.LendUnitCostWhenCreate : item.LendProduct.ProductPriceInfo.UnitCost; //itemPrice.UnitCost;
                    item.LendUnitCostWithoutTax = item.LendProduct.ProductPriceInfo.UnitCostWithoutTax; //itemPrice.UnitCostWithoutTax;

                    lendRequestDA.UpdateLendItem(item);

                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = -item.LendQuantity,
                        ProductSysNo = item.LendProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.Stock.SysNo,
                        AdjustUnitCost = item.LendUnitCost,
                    });

                    result.LendItemInfoList.Add(item);
                }

                string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                if (!string.IsNullOrEmpty(adjustResult))
                {
                    throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                }

                //For BatchProduct, SSB to WMS
                string InUser = ServiceContext.Current.UserDisplayName;
                List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(result.SysNo.Value);
                if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
                {
                    #region 构建借货单出库调整批次库存表的SSB消息 并调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in batchDetailsInfoEntitylist)
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
                        Header = new InventoryHeader
                        {
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "OutStock",
                            Version = "V10",
                            Type = "Lend",
                            CompanyCode = "8601",
                            Tag = "LendOutStock",
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

                #region  获取 非批次商品更改的信息 并给仓库发送SSB
               
                List<InventoryAdjustItemInfo> adjustCaseEntityList = new List<InventoryAdjustItemInfo>();
                foreach (var item in inventoryAdjustContract.AdjustItemList)
                {                                                                                  
                    if (!ObjectFactory<IAdjustRequestDA>.Instance.CheckISBatchNumberProduct(item.ProductSysNo))
                    {
                        item.AdjustQuantity = -item.AdjustQuantity;
                        
                        adjustCaseEntityList.Add(item);
                    }
                }
                #endregion
                LendSendSSBToWMS(result.SysNo.Value, inventoryAdjustContract.AdjustItemList[0].StockSysNo.ToString(), batchDetailsInfoEntitylist, adjustCaseEntityList, "借货单出库向仓库发送SSB");//借货单出库向仓库发送SSB                    

                //TODO: Send SSB to WMS for OutStock
                SendSSBToWMS();

                //借货出库发送message
                //EventPublisher.Publish(new OutStockLendRequestInfoMessage()
                //{
                //    LendRequestInfoSysNo = entityToUpdate.SysNo.Value,
                //    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                //});

                scope.Complete();
            }

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"出库了编号为\"{1}\"的借货单", entityToUpdate.OutStockUser.SysNo, entityToUpdate.RequestID)
              , BizLogType.St_Lend_OutStock, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            LendRequestInfo resultRequest = GetLendRequestInfoBySysNo((int)entityToUpdate.SysNo);

            return resultRequest;
        }

        /// <summary>
        /// 借货归还
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual LendRequestInfo ReturnRequest(LendRequestInfo entityToUpdate)
        {
            PreCheckLendRequestInfoForUpdateStatus(entityToUpdate);

            List<LendRequestReturnItemInfo> newReturnItems = new List<LendRequestReturnItemInfo>();
            List<LendRequestItemInfo> lendItemsWithReturnQty = entityToUpdate.LendItemInfoList.FindAll(i => { return i.ToReturnQuantity.HasValue && i.ToReturnQuantity > 0; });
            if (lendItemsWithReturnQty == null || lendItemsWithReturnQty.Count == 0)
            {
                BizExceptionHelper.Throw("InventoryLend_ReturnItemIsEmpty");
            }
            else
            {
                lendItemsWithReturnQty.ForEach(i =>
                {
                    LendRequestReturnItemInfo ri = new LendRequestReturnItemInfo
                    {
                        ReturnProduct = new ProductInfo { SysNo = i.LendProduct.SysNo },
                        ReturnQuantity = (int)i.ToReturnQuantity,
                        ReturnDate = DateTime.Now,
                        BatchDetailsInfoList = i.BatchDetailsInfoList
                    };

                    newReturnItems.Add(ri);
                });
            }


            #region remove dumplicated item

            var returnItemEntities = new List<LendRequestReturnItemInfo>();
            foreach (var returnItemEntity in newReturnItems)
            {
                var entity = returnItemEntity;
                if (returnItemEntities.Find(p => p.ReturnProduct.SysNo == entity.ReturnProduct.SysNo) != null)
                {
                    BizExceptionHelper.Throw("InventoryLend_ReturnItemDuplicated", returnItemEntity.ReturnProduct.SysNo.ToString());
                }
                returnItemEntities.Add(returnItemEntity);
            }

            #endregion

            #region check status
            var originalEntity = GetLendRequestInfoBySysNo((int)entityToUpdate.SysNo);

            PreCheckOriginLendRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Return);
            #endregion

            var originalLendItems = originalEntity.LendItemInfoList;
            var originalReturnItems = originalEntity.ReturnItemInfoList;

            entityToUpdate.LendItemInfoList = originalLendItems;

            var nowReturnItems = originalReturnItems;

            List<InventoryBatchDetailsInfo> batchList = new List<InventoryBatchDetailsInfo>();
            foreach (var newReturnItem in newReturnItems)
            {
                var totalReturnQty = 0;
                var itemEntity = newReturnItem;
                var originalLendItem = originalLendItems.Find(entity => entity.LendProduct.SysNo == itemEntity.ReturnProduct.SysNo);

                if (originalLendItem == null)
                {
                    BizExceptionHelper.Throw("InventoryLend_ReturnItemIsNotValid", newReturnItem.ReturnProduct.SysNo.ToString());
                }

                //return qty must be greater than zero
                if (newReturnItem.ReturnQuantity <= 0)
                {
                    BizExceptionHelper.Throw("InventoryLend_ReturnQtyMustMoreThanZero", newReturnItem.ReturnProduct.SysNo.ToString());
                }

                foreach (var originalReturnItem in originalReturnItems)
                {
                    //originalReturnItem.LendSysNumber == newEntity.LendSysNumber &&
                    if (originalReturnItem.ReturnProduct.SysNo == newReturnItem.ReturnProduct.SysNo)
                    {
                        totalReturnQty += originalReturnItem.ReturnQuantity;
                    }
                }

                totalReturnQty += newReturnItem.ReturnQuantity;

                //return qty must be less than lend qty
                if (totalReturnQty > originalLendItem.LendQuantity)
                {
                    BizExceptionHelper.Throw("InventoryLend_ReturnQtyIsNotValid", newReturnItem.ReturnProduct.SysNo.ToString(), originalLendItem.LendQuantity.ToString());
                }

                if (newReturnItem.BatchDetailsInfoList != null && newReturnItem.BatchDetailsInfoList.Count > 0)
                {
                    batchList.AddRange(newReturnItem.BatchDetailsInfoList);
                }
            }

            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Return);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                #region 生成新的归还记录，并调整库存
                foreach (var returnItem in newReturnItems)
                {
                    var originalLendItem = originalLendItems.Find(p => p.LendProduct.SysNo == returnItem.ReturnProduct.SysNo);
                    lendRequestDA.CreateReturnItem(returnItem, (int)entityToUpdate.SysNo);
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = returnItem.ReturnQuantity,
                        ProductSysNo = returnItem.ReturnProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.Stock.SysNo,
                        AdjustUnitCost = originalLendItem.LendUnitCost,
                    });

                    nowReturnItems.Add(returnItem);
                }

                string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                if (!string.IsNullOrEmpty(adjustResult))
                {
                    throw new BizException("库存调整失败: " + adjustResult);
                }

                #endregion

                #region update status

                var totalLendItemQty = 0;

                var totalReturnItemQty = 0;

                foreach (var lendItemEntity in originalLendItems)
                {
                    totalLendItemQty += lendItemEntity.LendQuantity;
                }

                foreach (var returnItemEntity in nowReturnItems)
                {
                    totalReturnItemQty += returnItemEntity.ReturnQuantity;
                }

                entityToUpdate.RequestStatus = totalLendItemQty == totalReturnItemQty ? LendRequestStatus.ReturnAll : LendRequestStatus.ReturnPartly;

                UpdateRequestStatus(entityToUpdate);


                #endregion

                #region 借货单归还 向仓库发送SSB
                #region  获取 非批次商品更改的信息
                List<InventoryAdjustItemInfo> notBatchAdjustCaseEntityList = new List<InventoryAdjustItemInfo>();
                foreach (var item in inventoryAdjustContract.AdjustItemList)
                {
                    if (!ObjectFactory<IAdjustRequestDA>.Instance.CheckISBatchNumberProduct(item.ProductSysNo))
                    {
                        item.AdjustQuantity = item.AdjustQuantity;

                        notBatchAdjustCaseEntityList.Add(item);
                    }
                }
                #endregion
                if (notBatchAdjustCaseEntityList != null && notBatchAdjustCaseEntityList.Count > 0)
                {
                    ReturnSendSSBToWMS(entityToUpdate.SysNo.Value, inventoryAdjustContract.AdjustItemList[0].StockSysNo.ToString(), null, notBatchAdjustCaseEntityList, "借货单归还向仓库发送SSB");//借货单归还项仓库发送SSB
                }
                
                ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(batchList, entityToUpdate.SysNo.Value, "Lend", "Return");                

                SendSSBToWMS();
                #endregion
                scope.Complete();

            }

            //adjustBP.TryUpdateItemUnitCost();

            entityToUpdate.ReturnItemInfoList = nowReturnItems;
            LendRequestInfo resultRequest = GetLendRequestInfoBySysNo((int)entityToUpdate.SysNo);

            #region write log

            string logMsg = entityToUpdate.RequestStatus == LendRequestStatus.ReturnPartly ? string.Format("用户\"{0}\"部份归还了编号为\"{1}\"的借货单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                                                                                            : string.Format("用户\"{0}\"归还了编号为\"{1}\"的借货单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID);
            ExternalDomainBroker.CreateOperationLog(logMsg, BizLogType.St_Lend_Return_Insert, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return resultRequest;
        }

        #region 私有方法

        private InventoryAdjustContractInfo InitInventoryAdjustContract(LendRequestInfo entity, InventoryAdjustSourceAction sourceAction)
        {
            var inventoryAdjustContract = new InventoryAdjustContractInfo
            {
                SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_LendRequest,
                SourceActionName = sourceAction,
                ReferenceSysNo = entity.SysNo.ToString(),
                AdjustItemList = new List<InventoryAdjustItemInfo>()
            };
            return inventoryAdjustContract;
        }

        private void PreCheckLendRequestInfoForCreate(LendRequestInfo entityToCreate)
        {
            if (entityToCreate == null)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetNotCompletelyforAdd");
            }

            if (entityToCreate.LendUser == null || entityToCreate.LendUser.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetNotCompletelyforAdd");
            }

            if (entityToCreate.Stock == null || entityToCreate.Stock.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetNotCompletelyforAdd");
            }
        }

        private void PreCheckLendRequestInfoForUpdate(LendRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetNotCompletelyforUpdate");
            }

            if (entityToUpdate.SysNo == null || entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetNotCompletelyforUpdate");
            }

            if (entityToUpdate.LendUser == null || entityToUpdate.LendUser.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetNotCompletelyforUpdate");
            }
        }

        private void PreCheckLendRequestInfoForUpdateStatus(LendRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryLend_sysNumberIsNotValid");
            }

            if (entityToUpdate.SysNo == null || entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_sysNumberIsNotValid");
            }
        }

        private void PreCheckOriginLendRequestInfo(LendRequestInfo entity, InventoryAdjustSourceAction actionType)
        {
            if (entity == null)
            {
                BizExceptionHelper.Throw("InventoryLend_cannotFindOriginalLendSheet");
            }

            //借货单当前状态检查
            if ((actionType == InventoryAdjustSourceAction.Update || actionType == InventoryAdjustSourceAction.Audit
                || actionType == InventoryAdjustSourceAction.Abandon) && entity.RequestStatus != LendRequestStatus.Origin)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotOrigin");
            }
            else if (actionType == InventoryAdjustSourceAction.CancelAbandon && entity.RequestStatus != LendRequestStatus.Abandon)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotAbandon");
            }
            else if ((actionType == InventoryAdjustSourceAction.CancelAudit || actionType == InventoryAdjustSourceAction.OutStock)
                && entity.RequestStatus != LendRequestStatus.Verified)
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotVerify");
            }
            else if ((actionType == InventoryAdjustSourceAction.Return)
               && (entity.RequestStatus != LendRequestStatus.OutStock && entity.RequestStatus != LendRequestStatus.ReturnPartly))
            {
                BizExceptionHelper.Throw("InventoryLend_LendSheetIsNotOutStockOrReturnPartly", entity.SysNo.ToString());
            }

            //if (entity.CreateUser != null && entity.CreateUser.SysNo == ServiceContext.Current.UserSysNo&&actionType == InventoryAdjustSourceAction.Audit)
            //{
            //    throw new BizException("创建人和审核人不能相同");

            //}

        }

        private void PreCheckDuplicatedLendItem(LendRequestInfo entityToUpdate)
        {
            List<LendRequestItemInfo> lendItemEntities;

            if (entityToUpdate.LendItemInfoList != null && entityToUpdate.LendItemInfoList.Count > 0)
            {
                lendItemEntities = new List<LendRequestItemInfo>();
                foreach (var li in entityToUpdate.LendItemInfoList)
                {
                    if (lendItemEntities.Find(p => p.LendProduct.SysNo == li.LendProduct.SysNo) != null)
                    {
                        BizExceptionHelper.Throw("InventoryLend_ItemDuplicated", li.LendProduct.SysNo.ToString());
                    }

                    lendItemEntities.Add(li);
                }
            }
        }

        private void PreCheckLendItemInfo(LendRequestItemInfo lendItemInfo)
        {
            if (lendItemInfo.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_LendItemNotCompletely");
            }

            if (lendItemInfo.LendQuantity <= 0)
            {
                BizExceptionHelper.Throw("InventoryLend_LendItemNotCompletely");
            }

            if (lendItemInfo.LendProduct.ProductPriceInfo == null)
            {
                BizExceptionHelper.Throw("Common_CannotFindPriceInformation", lendItemInfo.LendProduct.SysNo.ToString());
            }
        }

        private void SendSSBToWMS()
        {
           
        }

        private void AdjustBatchNumberInventory(int number,string companyCode)
        {
            List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(number);
            if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
            {                
                List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                foreach (var item in batchDetailsInfoEntitylist)
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
                string inUser = ServiceContext.Current.UserDisplayName;
                BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                {
                    Header = new InventoryHeader()
                    {
                        NameSpace = "http://soa.newegg.com/InventoryProfile",
                        Action = "Abandon",
                        Version = "V10",
                        Type = "Lend",
                        
                        CompanyCode = companyCode,
                        Tag = "LendAbandon",
                        Language = "zh-CN",
                        From = "IPP",
                        GlobalBusinessType = "Listing",
                        StoreCompanyCode = companyCode,
                        TransactionCode = number.ToString()
                    },
                    Body = new InventoryBody
                    {
                        InUser = inUser,
                        Number = number.ToString(),
                        ItemBatchInfo = itemBatchInfoList
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(batchXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";

                ObjectFactory<IInventoryAdjustDA>.Instance.AdjustBatchNumberInventory(paramXml);
            }

        }

        private void LendSendSSBToWMS(int sysNo, string inWarehouseNumber, List<InventoryBatchDetailsInfo> batchDetailsInfoEntity, List<InventoryAdjustItemInfo> adjustCaseEntityList, string inMemo)
        {
            if ((batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0) || (adjustCaseEntityList != null && adjustCaseEntityList.Count > 0))
            {
                List<ECCentral.BizEntity.Inventory.Item> itemList = new List<ECCentral.BizEntity.Inventory.Item>();
                //List<BatchDetailsInfoEntity> batchDetailsInfoEntity = AdjustDAL.GetBatchDetailsInfoEntityListByNumber(SysNumber);
                if (batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0)
                {
                    foreach (var BatchItem in batchDetailsInfoEntity)
                    {
                        ProductBatch pbEntity = new ProductBatch
                        {
                            BatchNumber = BatchItem.BatchNumber,
                            Quantity = (-BatchItem.Quantity).ToString()
                        };
                        ECCentral.BizEntity.Inventory.Item item = new ECCentral.BizEntity.Inventory.Item()
                        {
                            ProductBatch = pbEntity,
                            ProductSysNo = BatchItem.ProductSysNo.ToString(),
                            Quantity = (-BatchItem.Quantity).ToString()
                        };
                        itemList.Add(item);
                    }
                }
                //初始非批次商品调整信息
                if (adjustCaseEntityList != null && adjustCaseEntityList.Count > 0)
                {
                    foreach (var NOTBatchItem in adjustCaseEntityList)
                    {
                        ECCentral.BizEntity.Inventory.Item item = new ECCentral.BizEntity.Inventory.Item()
                        {
                            ProductSysNo = NOTBatchItem.ProductSysNo.ToString(),
                            Quantity = (-NOTBatchItem.AdjustQuantity).ToString()
                        };
                        itemList.Add(item);
                    }
                }
                SendToWMSSSBXMLMessage SendToWMSSSBXMLMessage = new SendToWMSSSBXMLMessage()
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
                                Type = "70",
                                Number = sysNo.ToString(),
                                User = ServiceContext.Current.UserDisplayName,
                                Memo = inMemo,
                                Item = itemList,
                                WarehouseNumber = inWarehouseNumber
                            }
                        }
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(SendToWMSSSBXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                ObjectFactory<IAdjustRequestDA>.Instance.SendSSBToWMS(paramXml);
            }
        }

        public void ReturnSendSSBToWMS(int sysNo, string inWarehouseNumber, List<InventoryBatchDetailsInfo> batchDetailsInfoEntity,  List<InventoryAdjustItemInfo> notBatchAdjustCaseEntityList, string inMemo)
        {
            if ((batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0) || (notBatchAdjustCaseEntityList != null && notBatchAdjustCaseEntityList.Count > 0))
            {
                List<ECCentral.BizEntity.Inventory.Item> itemList = new List<ECCentral.BizEntity.Inventory.Item>();
                //List<BatchDetailsInfoEntity> batchDetailsInfoEntity = AdjustDAL.GetBatchDetailsInfoEntityListByNumber(SysNumber);
                if (batchDetailsInfoEntity != null && batchDetailsInfoEntity.Count > 0)
                {
                    foreach (var BatchItem in batchDetailsInfoEntity)
                    {
                        ProductBatch pbEntity = new ProductBatch
                        {
                            BatchNumber = BatchItem.BatchNumber,
                            Quantity = BatchItem.ReturnQty.ToString()
                        };
                        ECCentral.BizEntity.Inventory.Item item = new ECCentral.BizEntity.Inventory.Item()
                        {
                            ProductBatch = pbEntity,
                            ProductSysNo = BatchItem.ProductSysNo.ToString(),
                            Quantity = BatchItem.ReturnQty.ToString()
                        };
                        itemList.Add(item);
                    }
                }
                if (notBatchAdjustCaseEntityList != null && notBatchAdjustCaseEntityList.Count > 0)
                {
                    //初始非批次商品调整信息
                    foreach (var NOTBatchItem in notBatchAdjustCaseEntityList)
                    {
                        ECCentral.BizEntity.Inventory.Item item = new ECCentral.BizEntity.Inventory.Item()
                        {
                            ProductSysNo = NOTBatchItem.ProductSysNo.ToString(),
                            Quantity = NOTBatchItem.AdjustQuantity.ToString()
                        };
                        itemList.Add(item);
                    }
                }
                SendToWMSSSBXMLMessage SendToWMSSSBXMLMessage = new SendToWMSSSBXMLMessage()
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
                                Type = "71",
                                Number = sysNo.ToString(),
                                User = ServiceContext.Current.UserDisplayName,
                                Memo = inMemo,
                                Item = itemList,
                                WarehouseNumber = inWarehouseNumber
                            }
                        }
                    }
                };
                string paramXml = SerializationUtility.XmlSerialize(SendToWMSSSBXMLMessage);
                XmlDocument xmlD = new XmlDocument();
                xmlD.LoadXml(paramXml);
                paramXml = "<" + xmlD.DocumentElement.Name + ">" + xmlD.DocumentElement.InnerXml + "</" + xmlD.DocumentElement.Name + ">";
                if (itemList != null && itemList.Count > 0)
                {
                    ObjectFactory<IAdjustRequestDA>.Instance.SendSSBToWMS(paramXml);
                }               
            }            
        }        

        #endregion 私有方法
    }
}
