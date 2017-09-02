using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.BizEntity.PO;
using ECCentral.Service.EventMessage.Inventory;
using System.Xml;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;

namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 损益单 - AdjustRequestProcessor
    /// </summary>
    [VersionExport(typeof(AdjustRequestProcessor))]
    public class AdjustRequestProcessor
    {
        private IAdjustRequestDA adjustRequestDA = ObjectFactory<IAdjustRequestDA>.Instance;
        private IInventoryQueryDA inventoryQueryDA = ObjectFactory<IInventoryQueryDA>.Instance;

        /// <summary>
        /// 根据SysNo获取损益单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo GetAdjustRequestInfoBySysNo(int requestSysNo)
        {
            AdjustRequestInfo adjustRequest = adjustRequestDA.GetAdjustRequestInfoBySysNo(requestSysNo);
            if (adjustRequest != null)
            {
                adjustRequest.AdjustItemInfoList = adjustRequestDA.GetAdjustItemListByRequestSysNo(requestSysNo);
                adjustRequest.InvoiceInfo = adjustRequestDA.GetInvoiceInfoByRequestSysNo(requestSysNo);
                
            }

            return adjustRequest;
        }

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo GetProductLineInfo(int ProductSysNo)
        {
            AdjustRequestInfo shiftRequest = adjustRequestDA.GetProductLineInfo(ProductSysNo);
            return shiftRequest;
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual List<AdjustRequestInfo> CreateRequest(AdjustRequestInfo entityToCreate)
        {
            if (entityToCreate.AdjustItemInfoList == null || entityToCreate.AdjustItemInfoList.Count == 0)
            {
                throw new BizException("操作失败，请提供损益单商品！");
            }

            List<AdjustRequestInfo> result = new List<AdjustRequestInfo>();

            //区分损益商品中的商品属性
            //代销商品
            var consignItems = entityToCreate.AdjustItemInfoList.FindAll(p =>( p.AdjustProduct.ProductConsignFlag == VendorConsignFlag.Consign));
            //代收代付商品
            var consign4Items = entityToCreate.AdjustItemInfoList.FindAll(p =>(p.AdjustProduct.ProductConsignFlag == VendorConsignFlag.GatherPay));
            //剩下的商品
            var restItems = entityToCreate.AdjustItemInfoList.FindAll(p => ( p.AdjustProduct.ProductConsignFlag != VendorConsignFlag.Consign
                                                                          && p.AdjustProduct.ProductConsignFlag != VendorConsignFlag.GatherPay));

            using (TransactionScope scope = new TransactionScope())
            {

                if (consignItems != null && consignItems.Count > 0)
                {
                    AdjustRequestInfo consignEntity = CloneRequestCommonInfo(entityToCreate);
                    consignEntity.AdjustItemInfoList = consignItems;
                    consignEntity.ConsignFlag = RequestConsignFlag.Consign;
                    result.Add(CreateSingleRequest(consignEntity));
                }

                if (consign4Items != null && consign4Items.Count > 0)
                {
                    AdjustRequestInfo consign4Entity = CloneRequestCommonInfo(entityToCreate);
                    consign4Entity.AdjustItemInfoList = consign4Items;
                    consign4Entity.ConsignFlag = RequestConsignFlag.GatherPay;
                    result.Add(CreateSingleRequest(consign4Entity));
                }

                if (restItems != null && restItems.Count > 0)
                {
                    AdjustRequestInfo restItemsEntity = CloneRequestCommonInfo(entityToCreate);
                    restItemsEntity.AdjustItemInfoList = restItems;
                    restItemsEntity.ConsignFlag = RequestConsignFlag.GatherPay;
                    result.Add(CreateSingleRequest(restItemsEntity));

                }               

                //创建损益单发送message
                //EventPublisher.Publish(new CreateAdjustRequestInfoMessage() {
                //    AdjustRequestInfoSysNo = entityToCreate.SysNo.Value,
                //    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                //});

                scope.Complete();
            }


            //if ((consignItems != null && consignItems.Count > 0)
            //    && (unConsignItems != null && unConsignItems.Count > 0)
            //    && (consign4Items!=null && consign4Items.Count>0)
            //    )
            //{
            //    //既有代销商品也有非代销商品, 将原损益单拆分为两个单据
            //    entityToCreate.AdjustItemInfoList = null;

            //    AdjustRequestInfo consignEntity = CloneRequestCommonInfo(entityToCreate);
            //    consignEntity.AdjustItemInfoList = consignItems;
            //    consignEntity.ConsignFlag = RequestConsignFlag.Consign;

            //    AdjustRequestInfo unConsignEntity = entityToCreate;
            //    entityToCreate.AdjustItemInfoList = unConsignItems;
            //    entityToCreate.ConsignFlag = RequestConsignFlag.NotConsign;

            //    AdjustRequestInfo consign4Entity = CloneRequestCommonInfo(entityToCreate);
            //    consignEntity.AdjustItemInfoList = consign4Items;
            //    consignEntity.ConsignFlag = RequestConsignFlag.GatherPay;

                
            //        result.Add(CreateSingleRequest(consignEntity));
            //        result.Add(CreateSingleRequest(unConsignEntity));
            //        result.Add(CreateSingleRequest(consign4Entity));
            //        scope.Complete();
            //    }
            //}
            //else
            //{
            //    //仅包含代销商品或非代销商品
            //    result.Add(CreateSingleRequest(entityToCreate));
            //}

            #region WriteLog & Send Email

            //Log & Send Email

            //Load CreateUserInfo/Stock Info
            UserInfo createUser = ExternalDomainBroker.GetUserInfo((int)entityToCreate.CreateUser.SysNo);
            WarehouseInfo warehouseInfo = ObjectFactory<StockProcessor>.Instance.GetWarehouseInfo(entityToCreate.Stock.SysNo.Value);
            foreach (var item in result)
            {
                ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"创建了编号为\"{1}\"的损益单", item.CreateUser.SysNo, item.RequestID)
                    , BizLogType.St_Adjust_Master_Insert, (int)item.SysNo, item.CompanyCode);

                KeyValueVariables vars = new KeyValueVariables();
                vars.Add("RequestID", item.RequestID);
                vars.Add("CreateUserName", createUser.UserDisplayName);
                vars.Add("StockName", warehouseInfo.WarehouseName);
                vars.Add("CreateDate", item.CreateDate);
                vars.Add("Note", item.Note);
                vars.Add("RequestSysNo", item.SysNo);

                ExternalDomainBroker.SendInternalEmail("Inventory_AdjustRequest_Create", vars);                
            
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo CreateSingleRequest(AdjustRequestInfo entityToCreate)
        {
            AdjustRequestInfo result = new AdjustRequestInfo();
            
            bool isConsign = false;
            if (entityToCreate.AdjustItemInfoList != null && entityToCreate.AdjustItemInfoList.Count > 0)
            {
                if (entityToCreate.AdjustItemInfoList[0].AdjustProduct.ProductConsignFlag == VendorConsignFlag.Consign || entityToCreate.AdjustItemInfoList[0].AdjustProduct.ProductConsignFlag == VendorConsignFlag.GatherPay)
                {
                    isConsign = true;
                }
                //isConsign = entityToCreate.AdjustItemInfoList[0].AdjustProduct.ProductConsignFlag == VendorConsignFlag.Consign;
                VendorConsignFlag RequestConsignFlag = entityToCreate.AdjustItemInfoList[0].AdjustProduct.ProductConsignFlag;
                //entityToCreate.ConsignFlag = productConsign == VendorConsignFlag.Consign ? RequestConsignFlag.Consign : RequestConsignFlag.NotConsign;
                entityToCreate.ConsignFlag = (RequestConsignFlag)((int)RequestConsignFlag);
            }

            using (TransactionScope scope = new TransactionScope())
            {
                entityToCreate.CreateDate = DateTime.Now; //BusinessUtility.GetCurrentDateTime();
                int requestSysNo = adjustRequestDA.GetAdjustRequestSequence();
                entityToCreate.SysNo = requestSysNo;
                entityToCreate.RequestID = "56" + requestSysNo.ToString().PadLeft(8, '0');
                if(entityToCreate.CreateUser==null)
                {
                    throw new BizException("无法获取当前用户信息。");
                }
                adjustRequestDA.CreateAdjustRequest(entityToCreate);

                //初始化统一库存调整实体
                var inventoryAdjustContract = InitInventoryAdjustContract(entityToCreate, InventoryAdjustSourceAction.Create);

                if (entityToCreate.AdjustItemInfoList != null && entityToCreate.AdjustItemInfoList.Count > 0)
                {
                    entityToCreate.AdjustItemInfoList.ForEach( adjustItem =>
                    {
                        PreCheckAdjustItemInfo(adjustItem);

                        if (adjustItem.AdjustQuantity < 0)
                        {
                            //add item which need adjust quantity
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = adjustItem.AdjustQuantity,
                                ProductSysNo = adjustItem.AdjustProduct.SysNo,                                
                                StockSysNo = (int)entityToCreate.Stock.SysNo
                            });

                            PreCheckAdjustItemInfoForInventoryAdjust(adjustItem);

                        }
                        //update flash item unit cost
                        adjustItem.AdjustCost = adjustItem.AdjustProduct.ProductPriceInfo.UnitCost;
                        adjustRequestDA.CreateAdjustItem(adjustItem, (int)entityToCreate.SysNo);
                    });
                }

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                result = GetAdjustRequestInfoBySysNo(requestSysNo);

                // product batch info
                List<InventoryBatchDetailsInfo> batchList = new List<InventoryBatchDetailsInfo>();
                entityToCreate.AdjustItemInfoList.ForEach(p =>
                {
                    if (p.BatchDetailsInfoList != null)
                    {
                        batchList.AddRange(p.BatchDetailsInfoList);
                    }
                });
                // batch info
                if (batchList != null && batchList.Count > 0)
                {
                    ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(batchList, result.SysNo.Value, "Adjust", "Create");
                }


                scope.Complete();
            }

            //result.OperationUserUniqueName = entity.OperationUserUniqueName;

            return result;
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo UpdateRequest(AdjustRequestInfo entityToUpdate)
        {
            if (entityToUpdate.AdjustItemInfoList == null || entityToUpdate.AdjustItemInfoList.Count == 0)
            {
                throw new BizException("操作失败，请提供损益单商品！");
            }

            var consignItems = entityToUpdate.AdjustItemInfoList.FindAll(p => { return p.AdjustProduct.ProductConsignFlag == VendorConsignFlag.Consign; });
            var unConsignItems = entityToUpdate.AdjustItemInfoList.FindAll(p => { return p.AdjustProduct.ProductConsignFlag != VendorConsignFlag.Consign; });
            if ((consignItems != null && consignItems.Count > 0) 
                && entityToUpdate.ConsignFlag == RequestConsignFlag.NotConsign)
            {
                throw new BizException("非代销损益单，不允许包含代销商品");
            }

            if ((unConsignItems != null && unConsignItems.Count > 0) 
                && entityToUpdate.ConsignFlag == RequestConsignFlag.Consign)
            {
                throw new BizException("代销损益单，不允许包含非代销商品");
            }
            
            AdjustRequestInfo originalEntity = GetAdjustRequestInfoBySysNo((int)entityToUpdate.SysNo);

            PreCheckOriginAdjustRequestInfo(originalEntity, entityToUpdate, InventoryAdjustSourceAction.Update);

            AdjustRequestInfo result = new AdjustRequestInfo();
            
            //TODO: Load AdjustItem ProductInfo

            using (TransactionScope scope = new TransactionScope())
            {
                adjustRequestDA.UpdateAdjustRequest(entityToUpdate);
                //delete current adjust item, and invoke adjust inventory of self domain to back available quantity
                DeleteAllAdjustItem(entityToUpdate);

                //初始化统一库存调整实体
                var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Update);

                
                if (entityToUpdate.AdjustItemInfoList != null && entityToUpdate.AdjustItemInfoList.Count > 0)
                {
                    entityToUpdate.AdjustItemInfoList.ForEach(adjustItem =>
                    {
                        PreCheckAdjustItemInfo(adjustItem);

                        if (adjustItem.AdjustQuantity < 0)
                        {
                            //add item which need adjust quantity
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = adjustItem.AdjustQuantity,
                                ProductSysNo = adjustItem.AdjustProduct.SysNo,
                                StockSysNo = (int)entityToUpdate.Stock.SysNo
                            });

                            PreCheckAdjustItemInfoForInventoryAdjust(adjustItem);

                        }
                        //update flash item unit cost
                        adjustItem.AdjustCost = adjustItem.AdjustProduct.ProductPriceInfo.UnitCost;
                        adjustRequestDA.CreateAdjustItem(adjustItem, (int)entityToUpdate.SysNo);
                    });
                }
                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                result = GetAdjustRequestInfoBySysNo((int)entityToUpdate.SysNo);

                // product batch info
                List<InventoryBatchDetailsInfo> batchList = new List<InventoryBatchDetailsInfo>();
                entityToUpdate.AdjustItemInfoList.ForEach(p =>
                {
                    if (p.BatchDetailsInfoList != null)
                    {
                        batchList.AddRange(p.BatchDetailsInfoList);
                    }
                });
                // batch info
                if (batchList != null && batchList.Count > 0)
                {
                    ObjectFactory<BatchInventoryProcessor>.Instance.UpdateAdjustQuantityOfSTB(batchList, result.SysNo.Value, "Adjust", "Create");
                }

                scope.Complete();
            }

            //写日志
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"更新了编号为\"{1}\"的损益单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Adjust_Master_Update, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);            

            return result;
        }

        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo UpdateRequestStatus(AdjustRequestInfo entity)
        {
            return adjustRequestDA.UpdateAdjustRequestStatus(entity);
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo AbandonRequest(AdjustRequestInfo entityToUpdate)
        {            

            PreCheckOriginAdjustRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Abandon);
            
            entityToUpdate.RequestStatus = AdjustRequestStatus.Abandon;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            using (TransactionScope scope = new TransactionScope())
            {
                if (entityToUpdate.AdjustItemInfoList != null && entityToUpdate.AdjustItemInfoList.Count > 0)
                {
                    entityToUpdate.AdjustItemInfoList.ForEach(adjustItem =>
                    {
                        PreCheckAdjustItemInfo(adjustItem);

                        if (adjustItem.AdjustQuantity < 0)
                        {
                            //add item which need adjust quantity
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = -adjustItem.AdjustQuantity,
                                ProductSysNo = adjustItem.AdjustProduct.SysNo,
                                StockSysNo = (int)entityToUpdate.Stock.SysNo
                            });
                        }
                    });
                }
                
                UpdateRequestStatus(entityToUpdate);

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(entityToUpdate.SysNo.Value);
                if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
                {
                    #region 构建损益单作废调整批次库存表的SSB消息  调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in batchDetailsInfoEntitylist)
                    {
                        ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                        itemBatchInfo.BatchNumber = item.BatchNumber;
                        itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                        Stock stock = new Stock();
                        AdjustRequestItemInfo aEntity = new AdjustRequestItemInfo();
                        aEntity = entityToUpdate.AdjustItemInfoList.Find(x => { return x.AdjustProduct.SysNo == item.ProductSysNo; });
                        if (aEntity != null && aEntity.AdjustQuantity > 0)
                        {
                            stock.AllocatedQty = string.Empty; //益单不调 占用库存 作废 取消作废 单据 只调整 占用库存
                        }
                        else
                        {
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
                    string InUser = ServiceContext.Current.UserDisplayName;
                    BatchXMLMessage batchXMLMessage = new BatchXMLMessage()
                    {
                        Header = new InventoryHeader
                        {
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "Abandon",
                            Version = "V10",
                            Type = "Adjust",
                            CompanyCode = "8601",
                            Tag = "AdjustAbandon",
                            Language = "zh-CN",
                            From = "IPP",
                            GlobalBusinessType = "Listing",
                            StoreCompanyCode = "8601",
                            TransactionCode = entityToUpdate.SysNo.ToString()
                        },
                        Body = new InventoryBody
                        {
                            InUser = InUser,
                            Number = entityToUpdate.SysNo.ToString(),
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

                //作废损益单发送message
                EventPublisher.Publish(new VoidAdjustRequestInfoMessage() { 
                    AdjustRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            //写日志
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"作废了编号为\"{1}\"的损益单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Adjust_Abandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            return entityToUpdate;
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo CancelAbandonRequest(AdjustRequestInfo entityToUpdate)
        {
            //CreateUser&&EditUser
            PreCheckOriginAdjustRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            entityToUpdate.RequestStatus = AdjustRequestStatus.Origin;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            using (TransactionScope scope = new TransactionScope())
            {
                if (entityToUpdate.AdjustItemInfoList != null && entityToUpdate.AdjustItemInfoList.Count > 0)
                {
                    entityToUpdate.AdjustItemInfoList.ForEach( adjustItem =>
                    {
                        PreCheckAdjustItemInfo(adjustItem);

                        if (adjustItem.AdjustQuantity < 0)
                        {
                            //add item which need adjust quantity
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = adjustItem.AdjustQuantity,
                                ProductSysNo = adjustItem.AdjustProduct.SysNo,
                                StockSysNo = (int)entityToUpdate.Stock.SysNo
                            });
                        }
                    });
                }

                UpdateRequestStatus(entityToUpdate);

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(entityToUpdate.SysNo.Value);
                if (batchDetailsInfoEntitylist != null && batchDetailsInfoEntitylist.Count > 0)
                {
                    #region 构建损益单 取消作废调整批次库存表的SSB消息  调整库存
                    List<ItemBatchInfo> itemBatchInfoList = new List<ItemBatchInfo>();
                    foreach (var item in batchDetailsInfoEntitylist)
                    {
                        ItemBatchInfo itemBatchInfo = new ItemBatchInfo();
                        itemBatchInfo.BatchNumber = item.BatchNumber;
                        itemBatchInfo.ProductNumber = item.ProductSysNo.ToString();

                        Stock stock = new Stock();
                        AdjustRequestItemInfo aEntity = new AdjustRequestItemInfo();
                        aEntity = entityToUpdate.AdjustItemInfoList.Find(x => { return x.AdjustProduct.SysNo == item.ProductSysNo; });
                        if (aEntity != null && aEntity.AdjustQuantity > 0)                        
                        {
                            stock.AllocatedQty = string.Empty; //益单不调 占用库存 作废 取消作废 单据 只调整 占用库存
                        }
                        else
                        {
                            stock.AllocatedQty = (-item.Quantity).ToString(); //损单 需要调整 占用库存 作废 取消作废 单据 只调整 占用库存
                        }
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
                            Action = "CancelAbandon",
                            Version = "V10",
                            Type = "Adjust",
                            CompanyCode = "8601",
                            Tag = "AdjustCancelAbandon",
                            Language = "zh-CN",
                            From = "IPP",
                            GlobalBusinessType = "Listing",
                            StoreCompanyCode = "8601",
                            TransactionCode = entityToUpdate.SysNo.ToString()
                        },
                        Body = new InventoryBody
                        {
                            InUser = InUser,
                            Number = entityToUpdate.SysNo.ToString(),
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

                //取消作废损益单发送message
                EventPublisher.Publish(new CancelVoidAdjustRequestInfoMessage() { 
                    AdjustRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            //写日志
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消作废了编号为\"{1}\"的损益单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Adjust_CancelAbandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            return entityToUpdate;
        }


        public virtual AdjustRequestInfo VerifyRequest(AdjustRequestInfo entityToUpdate)
        {
            PreCheckOriginAdjustRequestStatus(entityToUpdate, InventoryAdjustSourceAction.Audit);

            entityToUpdate.AuditDate = DateTime.Now;// BusinessUtility.GetCurrentDateTime();
            entityToUpdate.RequestStatus = AdjustRequestStatus.Verified;

            using (TransactionScope scope = new TransactionScope())
            {

                //adjustRequestDA.VerifyAdjust(entity.SysNumber, (int)entity.Status, entity.AuditUserSysNumber.Value, entity.AuditTime.Value, entity.CompanyCode);
                UpdateRequestStatus(entityToUpdate);

                //审核损益单发送message
                EventPublisher.Publish(new AuditAdjustRequestInfoMessage() 
                {
                    AdjustRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });
                scope.Complete();
            }

            //写日志
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"审核了编号为\"{1}\"的损益单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Adjust_Verify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            return entityToUpdate;
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo CancelVerifyRequest(AdjustRequestInfo entityToUpdate)
        {
            PreCheckOriginAdjustRequestStatus(entityToUpdate, InventoryAdjustSourceAction.CancelAudit);

            entityToUpdate.AuditDate = DateTime.Now;// BusinessUtility.GetCurrentDateTime();
            entityToUpdate.RequestStatus = AdjustRequestStatus.Origin;

            using (TransactionScope ts = new TransactionScope())
            {
                //adjustRequestDA.VerifyAdjust(entity.SysNumber, (int)entity.Status, entity.AuditUserSysNumber.Value, entity.AuditTime.Value, entity.CompanyCode);
                UpdateRequestStatus(entityToUpdate);

                //取消审核发送message
                EventPublisher.Publish(new CancelAuditAdjustRequestInfoMessage() { 
                    AdjustRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });
            }
            //写日志
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消审核了编号为\"{1}\"的损益单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Adjust_CancelVerify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            return entityToUpdate;
        }

        public virtual AdjustRequestInfo OutStockRequest(AdjustRequestInfo entityToUpdate)
        {
    
            PreCheckOriginAdjustRequestStatus(entityToUpdate, InventoryAdjustSourceAction.OutStock);

           
            bool isConsign = false;

            entityToUpdate.OutStockDate = DateTime.Now;// BusinessUtility.GetCurrentDateTime();
            entityToUpdate.RequestStatus = AdjustRequestStatus.OutStock;
            isConsign = (entityToUpdate.ConsignFlag == RequestConsignFlag.Consign || entityToUpdate.ConsignFlag==RequestConsignFlag.GatherPay);

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.OutStock);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
            {
                if (entityToUpdate.AdjustItemInfoList != null && entityToUpdate.AdjustItemInfoList.Count > 0)
                {
                    entityToUpdate.AdjustItemInfoList.ForEach( adjustItem =>
                    {
                        PreCheckAdjustItemInfo(adjustItem);         

                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = adjustItem.AdjustQuantity,
                            ProductSysNo = adjustItem.AdjustProduct.SysNo,
                            StockSysNo = (int)entityToUpdate.Stock.SysNo,
                            AdjustUnitCost = adjustRequestDA.GetItemCost(adjustItem.AdjustProduct.SysNo),
                        });

                        //update flash item unit cost
                        if (adjustItem.AdjustCost != adjustItem.AdjustProduct.ProductPriceInfo.UnitCost)
                        {
                            adjustItem.AdjustCost = adjustItem.AdjustProduct.ProductPriceInfo.UnitCost;
                            adjustRequestDA.UpdateAdjustItemCost(adjustItem);
                        }
                    });
                }
                
                //adjustRequestDA.OutWarehouseAdjust(entity.SysNumber, (int)entity.Status, entity.OutStockUserSysNumber.Value, entity.OutStockTime.Value, entity.CompanyCode);
                UpdateRequestStatus(entityToUpdate);

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("库存调整失败: " + adjustResult);
                    }
                }

                //损益单为代销类型，出库时需要写入代销转财务日志
                if (isConsign)
                {
                    List<ConsignToAcctLogInfo> acctLogInfoList = new List<ConsignToAcctLogInfo>();
                    entityToUpdate.AdjustItemInfoList.ForEach(x => {
                        ConsignToAcctLogInfo acctLog = new ConsignToAcctLogInfo();
                        acctLog.ProductSysNo = x.AdjustProduct.SysNo;
                        acctLog.StockSysNo = entityToUpdate.Stock.SysNo;
                        acctLog.VendorSysNo = inventoryQueryDA.GetProductBelongVendorSysNo(x.AdjustProduct.SysNo);
                        acctLog.ProductQuantity = -x.AdjustQuantity;
                        acctLog.OutStockTime = entityToUpdate.OutStockDate;
                        acctLog.CreateCost = x.AdjustCost;                        
                        acctLog.OrderSysNo = entityToUpdate.SysNo;
                        acctLog.CompanyCode = entityToUpdate.CompanyCode;
                        acctLog.StoreCompanyCode = entityToUpdate.CompanyCode;
                        acctLog.IsConsign = (int)entityToUpdate.ConsignFlag;                  
                        acctLogInfoList.Add(acctLog);
                    });

                    if (acctLogInfoList.Count > 0)
                    {
                        ExternalDomainBroker.BatchCreateConsignToAcctLogsInventory(acctLogInfoList);
                    }                   

                }

                string InUser = ServiceContext.Current.UserDisplayName;
                List<InventoryBatchDetailsInfo> batchDetailsInfoEntitylist = ObjectFactory<IInventoryAdjustDA>.Instance.GetBatchDetailsInfoEntityListByNumber(entityToUpdate.SysNo.Value);
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
                        aEntity = entityToUpdate.AdjustItemInfoList.Find(x => { return x.AdjustProduct.SysNo == item.ProductSysNo; });
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
                            NameSpace = "http://soa.newegg.com/InventoryProfile",
                            Action = "OutStock",
                            Version = "V10",
                            Type = "Adjust",
                            CompanyCode ="8601",
                            Tag = "AdjustOutStock",
                            Language = "zh-CN",
                            From = "IPP",
                            GlobalBusinessType = "Listing",
                            StoreCompanyCode = "8601",
                            TransactionCode = entityToUpdate.SysNo.ToString()
                        },
                        Body = new InventoryBody
                        {
                            InUser = InUser,
                            Number = entityToUpdate.SysNo.ToString(),
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
                //#region  获取 非批次商品更改的信息 给仓库发送SSB
                List<InventoryAdjustItemInfo> adjustCaseEntityList = new List<InventoryAdjustItemInfo>();
                foreach (var item in inventoryAdjustContract.AdjustItemList)
                {
                    if (!ObjectFactory<IAdjustRequestDA>.Instance.CheckISBatchNumberProduct(item.ProductSysNo))
                    {
                        item.AdjustQuantity = item.AdjustQuantity;

                        adjustCaseEntityList.Add(item);
                    }
                }
                AdjustSendSSBToWMS(entityToUpdate.SysNo.Value, entityToUpdate.Stock.SysNo.ToString(), batchDetailsInfoEntitylist, adjustCaseEntityList);//损益单出库向仓库发送SSB消息                                                                    

                SendSSBToWMS();

                ///出库发送message
                EventPublisher.Publish(new OutStockAdjustRequestInfoMessage() { 
                    AdjustRequestInfoSysNo=entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            //写日志
            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"出库了编号为\"{1}\"的损益单", entityToUpdate.OutStockUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Adjust_OutStock, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            return entityToUpdate;
        }

        /// <summary>
        /// 损益单 - 维护损益单发票信息
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual AdjustRequestInfo MaintainAdjustInvoiceInfo(AdjustRequestInfo entityToUpdate)
        {
            AdjustRequestInfo originEntity = GetAdjustRequestInfoBySysNo((int)entityToUpdate.SysNo);
            
            if (originEntity.InvoiceInfo == null || originEntity.InvoiceInfo.SysNo == null)
            {
                CreateAdjustInvoiceInfo(entityToUpdate.InvoiceInfo, (int)entityToUpdate.SysNo);
            }
            else
            {
                UpdateAdjustInoviceInfo(entityToUpdate.InvoiceInfo);
            }

            entityToUpdate = GetAdjustRequestInfoBySysNo((int)entityToUpdate.SysNo);

            return entityToUpdate;
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual void CreateAdjustInvoiceInfo(AdjustRequestInvoiceInfo adjustInvoice, int requestSysNo)
        {
            adjustRequestDA.CreateAdjustRequestInvoice(adjustInvoice, requestSysNo);
        }

        /// <summary>
        /// 损益单 - 更新操作
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual void UpdateAdjustInoviceInfo(AdjustRequestInvoiceInfo adjustInvoice)
        {
            adjustRequestDA.UpdateAdjustRequestInvoice(adjustInvoice);
        }

        #region 私有方法

        private AdjustRequestInfo CloneRequestCommonInfo(AdjustRequestInfo entity)
        {
            return new AdjustRequestInfo() {
                CreateDate = entity.CreateDate,
                CreateUser = entity.CreateUser,
                EditUser = entity.EditUser,
                EditDate = entity.EditDate,
                AuditUser = entity.AuditUser,
                AuditDate = entity.AuditDate,
                OutStockUser = entity.OutStockUser,
                OutStockDate = entity.OutStockDate,
                AdjustProperty = entity.AdjustProperty,
                Note = entity.Note,
                RequestStatus = entity.RequestStatus,
                Stock = entity.Stock,
                CompanyCode = entity.CompanyCode
            };
        }

        #region 库存调整

        private InventoryAdjustContractInfo InitInventoryAdjustContract(AdjustRequestInfo entity, InventoryAdjustSourceAction sourceAction)
        {
            var inventoryAdjustContract = new InventoryAdjustContractInfo
            {
                SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_AdjustRequest,
                SourceActionName = sourceAction,
                ReferenceSysNo = entity.SysNo.ToString(),
                AdjustItemList = new List<InventoryAdjustItemInfo>()
            };
            return inventoryAdjustContract;
        }

        private void DeleteAllAdjustItem(AdjustRequestInfo entityToUpdate)
        {
            AdjustRequestInfo originEntity = GetAdjustRequestInfoBySysNo((int)entityToUpdate.SysNo);
            List<AdjustRequestItemInfo> originItems = originEntity.AdjustItemInfoList;
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Update);

            if (originItems != null && originItems.Count > 0)
            {
                foreach (var adjustItem in originItems)
                {
                    if (adjustItem.AdjustQuantity < 0)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -adjustItem.AdjustQuantity,
                            ProductSysNo = adjustItem.AdjustProduct.SysNo,
                            StockSysNo = (int)originEntity.Stock.SysNo
                        });                       
                    }
                }
            }

            adjustRequestDA.DeleteAdjustItemByRequestSysNo((int)originEntity.SysNo);
            if (inventoryAdjustContract.AdjustItemList.Count > 0)
            {
                string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                if (!string.IsNullOrEmpty(adjustResult))
                {
                    throw new BizException("库存调整失败: " + adjustResult);
                }
            }     
        }
        #endregion 库存调整

        #region PreCheck

        private void PreCheckOriginAdjustRequestInfo(AdjustRequestInfo originalEntity, AdjustRequestInfo entityToUpdate, InventoryAdjustSourceAction actionType)
        {            
            if (originalEntity == null)
            {
                BizExceptionHelper.Throw("Adjust_CanNotFoundAdjust");
            }          
            
            if (originalEntity.Stock.SysNo != entityToUpdate.Stock.SysNo)
            {
                BizExceptionHelper.Throw("Adjust_CurrentPropertyNotEqualPreviousIt");
            }
            
            if (originalEntity.AdjustProperty != entityToUpdate.AdjustProperty)
            {
                BizExceptionHelper.Throw("Adjust_CurrentWarehouseNotEqualPreviousIt");
            }

            if (originalEntity.CreateUser.SysNo != entityToUpdate.EditUser.SysNo && !entityToUpdate.IsShippingMaster)
            {
                //BizExceptionHelper.Throw("NotShippingMasterOrNotCreater");
                //throw new BizException("WarningMessage.NotShippingMasterOrNotCreaterCode, WarningMessage.NotShippingMasterOrNotCreaterValue, false");
            }

            PreCheckOriginAdjustRequestStatus(originalEntity, actionType);
        }

        private void PreCheckOriginAdjustRequestInfo(AdjustRequestInfo originalEntity, InventoryAdjustSourceAction actionType)
        {
            if (originalEntity == null)
            {
                BizExceptionHelper.Throw("Adjust_CanNotFoundAdjust");
            }
     
            if (originalEntity.CreateUser.SysNo != originalEntity.EditUser.SysNo && !originalEntity.IsShippingMaster)
            {
                //BizExceptionHelper.Throw("NotShippingMasterOrNotCreater");
                //throw new BizException("WarningMessage.NotShippingMasterOrNotCreaterCode, WarningMessage.NotShippingMasterOrNotCreaterValue, false");
            }

            PreCheckOriginAdjustRequestStatus(originalEntity, actionType);
        }

        private void PreCheckOriginAdjustRequestStatus(AdjustRequestInfo originalEntity, InventoryAdjustSourceAction actionType)
        {
            if (originalEntity == null)
            {
                BizExceptionHelper.Throw("Adjust_CanNotFoundAdjust");
            }

            //损益单当前状态检查
            if ((actionType == InventoryAdjustSourceAction.Update || actionType == InventoryAdjustSourceAction.Audit
                || actionType == InventoryAdjustSourceAction.Abandon) && originalEntity.RequestStatus != AdjustRequestStatus.Origin)
            {
                BizExceptionHelper.Throw("Adjust_ErrorStatusWithUpdate");
            }
            else if (actionType == InventoryAdjustSourceAction.CancelAbandon && originalEntity.RequestStatus != AdjustRequestStatus.Abandon)
            {
                BizExceptionHelper.Throw("Adjust_ErrorStatusWithUpdate");
            }
            else if ((actionType == InventoryAdjustSourceAction.CancelAudit || actionType == InventoryAdjustSourceAction.OutStock)
                && originalEntity.RequestStatus != AdjustRequestStatus.Verified)
            {
                BizExceptionHelper.Throw("Adjust_ErrorStatusWithUpdate");
            }

            //if (originalEntity.CreateUser != null && originalEntity.CreateUser.SysNo == ServiceContext.Current.UserSysNo&&actionType == InventoryAdjustSourceAction.Audit)
            //{
            //    throw new BizException("创建人和审核人不能相同");
            //}
        }

        private void PreCheckAdjustItemInfo(AdjustRequestItemInfo adjustItem)
        {
            if (adjustItem.AdjustProduct.ProductPriceInfo == null)
            {
                BizExceptionHelper.Throw("Common_CannotFindPriceInformation");
            }

            if (adjustItem.AdjustQuantity == 0)
            {
                BizExceptionHelper.Throw("Adjust_AdjustQtyCannotBeZero");
            }
        }

        private void PreCheckAdjustItemInfoForInventoryAdjust(AdjustRequestItemInfo adjustItemInfo)
        {            
            //将库存调整相关的检查逻辑移至库存统一调整接口中
            if (adjustItemInfo.AdjustProduct.ProductConsignFlag == VendorConsignFlag.Consign)
            {
                ////代销商品，检查限购量和预留库存
                //int checkResultLimitedQtyORIsReservedQty = adjustRequestDA.CheckBuyLimitAndIsLimitedQtyORIsReservedQty(adjustItem.ItemSysNumber);
                //if (checkResultLimitedQtyORIsReservedQty > 0)
                //{
                //    throw new BizException("商品编号为" + adjustItem.ItemSysNumber + "存在有限量或预留库存,且状态是就绪或运行的限时抢购记录。请先作废限时抢购记录");
                //}

                //int checkResultIsNotLimitedQtyANDIsNotReservedQty = adjustRequestDA.CheckBuyLimitAndIsNotLimitedQtyANDIsNotReservedQty(adjustItem.ItemSysNumber, adjustItem.AdjustQty, entity.WarehouseSysNumber, entity.CompanyCode);
                //if (checkResultIsNotLimitedQtyANDIsNotReservedQty > 0)
                //{
                //    throw new BizException("商品编号为" + adjustItem.ItemSysNumber + "损单数量不能大于“代销-被占用-被订购”数量");
                //}
            }
        }

        #endregion PreCheck

        #region Interact with Other Domain

        private void SendSSBToWMS()
        {

        }

        private void AdjustSendSSBToWMS(int sysNo, string inWarehouseNumber, List<InventoryBatchDetailsInfo> batchDetailsInfoEntity, List<InventoryAdjustItemInfo> adjustCaseEntityList)
        {
            List<Item> itemList = new List<Item>();
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
                        Item item = new Item()
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
                        Item item = new Item()
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
                        Body = new Body
                        {
                            Operation = new Operation()
                            {
                                Type = "60",
                                Number = sysNo.ToString(),
                                User = ServiceContext.Current.UserDisplayName,
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
                    ObjectFactory<IAdjustRequestDA>.Instance.SendSSBToWMS(paramXml);
                }               
            }            
        }       

        #endregion Interact with Other Domain

        #endregion

    }
}
