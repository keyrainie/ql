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
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.EventMessage.Inventory;


namespace ECCentral.Service.Inventory.BizProcessor
{
    /// <summary>
    /// 移仓单
    /// </summary>
    [VersionExport(typeof(ShiftRequestProcessor))]
    public class ShiftRequestProcessor
    {
        private IShiftRequestDA shiftRequestDA = ObjectFactory<IShiftRequestDA>.Instance;

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo GetProductLineInfo(int ProductSysNo)
        {
            ShiftRequestInfo shiftRequest = shiftRequestDA.GetProductLineInfo(ProductSysNo);
            return shiftRequest;
        }

        /// <summary>
        /// 根据SysNo获取移仓单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo GetShiftRequestInfoBySysNo(int requestSysNo)
        {
            ShiftRequestInfo shiftRequest = shiftRequestDA.GetShiftRequestInfoBySysNo(requestSysNo);
            if (shiftRequest != null)
            {
                shiftRequest.ShiftItemInfoList = shiftRequestDA.GetShiftItemListByRequestSysNo(requestSysNo);
            }

            return shiftRequest;
        }

        /// <summary>
        /// 创建移仓单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestInfo> CreateRequest(ShiftRequestInfo entityToCreate)
        {
            PreCheckShiftRequestInfoForCreate(entityToCreate);

            var result = new List<ShiftRequestInfo>();

            //区分移仓单中的代销商品和非代销商品，将原移仓单拆分为两个单据
            var consignItemList = entityToCreate.ShiftItemInfoList.FindAll(p => { return p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.Consign || p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.GatherPay; });
            var unConsignItemList = entityToCreate.ShiftItemInfoList.FindAll(p => { return p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.Sell; });

             TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                if ((consignItemList != null && consignItemList.Count > 0)
                    && (unConsignItemList != null && unConsignItemList.Count > 0))
                {

                    entityToCreate.ShiftItemInfoList = null;

                    ShiftRequestInfo consignEntity = CloneRequestCommonInfo(entityToCreate);
                    consignEntity.ShiftItemInfoList = consignItemList;
                    consignEntity.ConsignFlag = RequestConsignFlag.Consign;

                    var unConsignEntity = entityToCreate;
                    entityToCreate.ShiftItemInfoList = unConsignItemList;
                    entityToCreate.ConsignFlag = RequestConsignFlag.NotConsign;


                    result.Add(CreateSingleRequest(consignEntity));
                    result.Add(CreateSingleRequest(unConsignEntity));
                    ts.Complete();

                }
                else
                {
                    //仅包含代销商品或非代销商品
                    result.Add(CreateSingleRequest(entityToCreate));
                }

                //发送创建移仓单据消息
                EventPublisher.Publish(new CreateShiftRequestInfoMessage()
                {
                    ShiftRequestInfoSysNo = entityToCreate.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                ts.Complete();
            }

            return result;
        }

        /// <summary>
        /// 创建单个移仓单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo CreateSingleRequest(ShiftRequestInfo entityToCreate)
        {

            PreCheckDuplicatedShiftItem(entityToCreate);

            ShiftRequestInfo result;

            List<ShiftRequestItemInfo> shiftItemList = entityToCreate.ShiftItemInfoList;

            bool isConsign = false;
            if (shiftItemList != null && shiftItemList.Count > 0)
            {
                isConsign = (shiftItemList[0].ShiftProduct.ProductConsignFlag == VendorConsignFlag.Consign || shiftItemList[0].ShiftProduct.ProductConsignFlag == VendorConsignFlag.GatherPay);
                VendorConsignFlag productConsign = shiftItemList[0].ShiftProduct.ProductConsignFlag;
                //entityToCreate.ConsignFlag = productConsign == VendorConsignFlag.Consign ? RequestConsignFlag.Consign : RequestConsignFlag.NotConsign;
                entityToCreate.ConsignFlag = productConsign == VendorConsignFlag.Consign ? RequestConsignFlag.Consign : RequestConsignFlag.NotConsign;
            }

            using (var scope = new TransactionScope())
            {
                entityToCreate.RequestStatus = ShiftRequestStatus.Origin;
                entityToCreate.CreateDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();

                int requestSysNo = shiftRequestDA.GetShiftRequestSequence();
                entityToCreate.SysNo = requestSysNo;
                entityToCreate.RequestID = "57" + requestSysNo.ToString().PadLeft(8, '0');

                result = shiftRequestDA.CreateShiftRequest(entityToCreate);

                //初始化统一库存调整实体
                var inventoryAdjustContract = InitInventoryAdjustContract(entityToCreate, InventoryAdjustSourceAction.Create);

                if (shiftItemList != null && shiftItemList.Count > 0)
                {
                    foreach (var shiftItem in shiftItemList)
                    {
                        shiftItem.ShiftProduct = ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(shiftItem.ShiftProduct.SysNo);

                        PreCheckShiftItemInfo(shiftItem);

                        PreCheckShiftItemInfoForInventoryAdjust(shiftItem);

                        shiftItem.ShiftUnitCost = shiftItem.ShiftProduct.ProductPriceInfo.UnitCost;
                        shiftItem.ShiftUnitCostWithoutTax = shiftItem.ShiftProduct.ProductPriceInfo.UnitCostWithoutTax;

                        shiftRequestDA.CreateShiftItem(shiftItem, (int)entityToCreate.SysNo);

                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -shiftItem.ShiftQuantity,
                            ProductSysNo = shiftItem.ShiftProduct.SysNo,
                            StockSysNo = (int)entityToCreate.SourceStock.SysNo,
                        });
                    }

                    if (inventoryAdjustContract.AdjustItemList.Count > 0)
                    {
                        string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                        if (!string.IsNullOrEmpty(adjustResult))
                        {
                            throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                        }
                    }
                }

                scope.Complete();
            }

            result = GetShiftRequestInfoBySysNo((int)result.SysNo);

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"创建了编号为\"{1}\"的移仓单", entityToCreate.CreateUser.SysNo, entityToCreate.RequestID)
                    , BizLogType.St_Shift_Master_Insert, (int)entityToCreate.SysNo, entityToCreate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 从备货中心批量创建移仓单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>        
        public virtual ShiftRequestInfo BatchCreateRequest(ShiftRequestInfo entityToCreate, out int faults)
        {
            PreCheckShiftRequestInfoForCreate(entityToCreate);
            PreCheckDuplicatedShiftItem(entityToCreate);
            faults = 0;
            ShiftRequestInfo result = null;
            try
            {
                result = CreateSingleRequest(entityToCreate);
            }
            catch (BizException ex)
            {
                var fault = new MessageFault { ErrorCode = ex.Source, ErrorDescription = ex.Message };
                faults = 1;
            }
            return result;
        }

        /// <summary>
        /// 更新移仓单
        /// </summary>
        /// <param name="entityToCreate"></param>
        /// <returns></returns>    
        public virtual ShiftRequestInfo UpdateRequest(ShiftRequestInfo entityToUpdate)
        {
            if (entityToUpdate.ShiftItemInfoList == null || entityToUpdate.ShiftItemInfoList.Count == 0)
            {
                throw new BizException("操作失败，请提供移仓单商品！");
            }

            var consignItems = entityToUpdate.ShiftItemInfoList.FindAll(p => { return p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.Consign || p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.GatherPay; });
            var unConsignItems = entityToUpdate.ShiftItemInfoList.FindAll(p => { return p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.Sell || p.ShiftProduct.ProductConsignFlag == VendorConsignFlag.Gather; });
            if ((consignItems != null && consignItems.Count > 0)
                && entityToUpdate.ConsignFlag == RequestConsignFlag.NotConsign)
            {
                throw new BizException("非代销移仓单，不允许包含代销商品");
            }

            if ((unConsignItems != null && unConsignItems.Count > 0)
                && entityToUpdate.ConsignFlag == RequestConsignFlag.Consign)
            {
                throw new BizException("代销移仓单，不允许包含非代销商品");
            }

            PreCheckShiftRequestInfoForUpdate(entityToUpdate);

            PreCheckDuplicatedShiftItem(entityToUpdate);

            ShiftRequestInfo originalEntity = GetShiftRequestInfoBySysNo((int)entityToUpdate.SysNo);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Update);

            var originalItems = originalEntity.ShiftItemInfoList;

            if (originalItems != null && originalItems.Count == 0)
            {
                originalItems = null;
            }

            var newItems = entityToUpdate.ShiftItemInfoList;

            if (newItems != null && newItems.Count == 0)
            {
                newItems = null;
            }

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Update);

            ShiftRequestInfo result;

            using (var scope = new TransactionScope())
            {
                result = shiftRequestDA.UpdateShiftRequest(entityToUpdate);

                if (originalItems != null)
                {
                    //删除原移仓单中的移仓商品
                    shiftRequestDA.DeleteShiftItemByRequestSysNo((int)entityToUpdate.SysNo);
                }

                if (newItems != null)
                {
                    //保存新移仓单中的移仓商品
                    result.ShiftItemInfoList = new List<ShiftRequestItemInfo>();

                    foreach (var newItem in newItems)
                    {
                        PreCheckShiftItemInfo(newItem);

                        newItem.ShiftUnitCost = newItem.ShiftProduct.ProductPriceInfo.UnitCost;
                        newItem.ShiftUnitCostWithoutTax = newItem.ShiftProduct.ProductPriceInfo.UnitCostWithoutTax;

                        shiftRequestDA.CreateShiftItem(newItem, (int)entityToUpdate.SysNo);

                        result.ShiftItemInfoList.Add(newItem);
                    }
                }

                #region 计算移仓数量差异, 设置库存调整量

                if (originalItems == null && newItems != null)
                {
                    //原移仓单中无商品，新移仓单中的商品均为新增
                    foreach (var newItem in newItems)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -newItem.ShiftQuantity,
                            ProductSysNo = newItem.ShiftProduct.SysNo,
                            StockSysNo = (int)originalEntity.SourceStock.SysNo
                        });
                    }
                }
                else if (originalItems != null && newItems == null)
                {
                    //新移仓单中无商品，原移仓单中的商品均为删除
                    foreach (var originalItem in originalItems)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = originalItem.ShiftQuantity,
                            ProductSysNo = originalItem.ShiftProduct.SysNo,
                            StockSysNo = (int)originalEntity.SourceStock.SysNo
                        });
                    }
                }
                else if (originalItems != null)
                {
                    //新旧移仓单中都有移仓商品， 需要逐条对比
                    foreach (var originalItem in originalItems)
                    {
                        //var entity = originalItem;
                        var newItem = newItems.Find(p => p.ShiftProduct.SysNo == originalItem.ShiftProduct.SysNo);

                        if (newItem != null)
                        {
                            //新旧移仓单都包含的同一商品
                            var adjustQty = originalItem.ShiftQuantity - newItem.ShiftQuantity;

                            if (adjustQty != 0)
                            {
                                inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                                {
                                    AdjustQuantity = adjustQty,
                                    ProductSysNo = originalItem.ShiftProduct.SysNo,
                                    StockSysNo = (int)originalEntity.SourceStock.SysNo,
                                });
                            }
                            //continue;
                        }
                        else
                        {
                            //只在原移仓单中包含的商品，为删除
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = originalItem.ShiftQuantity,
                                ProductSysNo = originalItem.ShiftProduct.SysNo,
                                StockSysNo = (int)originalEntity.SourceStock.SysNo
                            });
                        }
                    }

                    foreach (var newItem in newItems)
                    {
                        //var entity = newItem;
                        var originalItem = originalItems.Find(p => p.ShiftProduct.SysNo == newItem.ShiftProduct.SysNo);

                        if (originalItem == null)
                        {
                            //只在新移仓单中包含的商品，为新增
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = -newItem.ShiftQuantity,
                                ProductSysNo = newItem.ShiftProduct.SysNo,
                                StockSysNo = (int)originalEntity.SourceStock.SysNo
                            });
                        }
                    }
                }

                #endregion

                if (inventoryAdjustContract.AdjustItemList.Count > 0)
                {
                    string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                    if (!string.IsNullOrEmpty(adjustResult))
                    {
                        throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                    }
                }

                scope.Complete();
            }

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"更新了编号为\"{1}\"的移仓单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Shift_Master_Update, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);


            #endregion

            return result;
        }

        /// <summary>
        /// 更新移仓单状态
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo UpdateRequestStatus(ShiftRequestInfo entityToUpdate)
        {
            return shiftRequestDA.UpdateShiftRequestStatus(entityToUpdate);
        }

        /// <summary>
        /// 批量更新移仓单特殊状态
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestInfo> UpdateSpecialShiftTypeBatch(List<ShiftRequestInfo> entityListToUpdate)
        {
            if (entityListToUpdate == null || entityListToUpdate.Count <= 0)
            {
                return null;
            }
            List<ShiftRequestInfo> resultList = new List<ShiftRequestInfo>();
            entityListToUpdate.ForEach(entity =>
            {
                ShiftRequestInfo request = UpdateSpecialShiftType(entity);
                resultList.Add(GetShiftRequestInfoBySysNo((int)request.SysNo));
            });
            return resultList;
        }

        /// <summary>
        /// 更新移仓单特殊状态
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo UpdateSpecialShiftType(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateSpecialShiftType(entityToUpdate);
            entityToUpdate.SpecialShiftSetDate = DateTime.Now;
            return shiftRequestDA.UpdateSpecialShiftType(entityToUpdate);
        }

        /// <summary>
        /// 审核移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo VerifyRequest(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Audit);

            var originalItems = entityToUpdate.ShiftItemInfoList;

            ShiftRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ShiftRequestStatus.Verified;
                entityToUpdate.AuditDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();

                result = UpdateRequestStatus(entityToUpdate);


                //发送审核移仓单据消息
                EventPublisher.Publish(new AuditShiftRequestInfoMessage()
                {
                    ShiftRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            result.ShiftItemInfoList = originalItems;

            #region send email

            //if (result.ShipViaTerm != null && result.ShipViaTerm.IndexOf("紧急") != -1)
            //{
            //    var dt = shiftRequestDA.GetInventoryTransferItemforEmail(newEntity.InventoryTransferSysNumber, newEntity.CompanyCode);

            //    var body = new StringBuilder();
            //    body.Append("<Center>移仓单  " + dt.Rows[0]["ShiftID"]);
            //    body.Append("<table border =1>");
            //    body.Append("<tr>");
            //    body.Append("<td>商品ID</td>");
            //    body.Append("<td>商品名称</td>");
            //    body.Append("<td>数量</td>");
            //    body.Append("</tr>");

            //    foreach (DataRow row in dt.Rows)
            //    {
            //        body.Append("<tr><td>" + row["ProductID"] + "</td>");
            //        body.Append("<td>" + row["ProductName"] + "</td>");
            //        body.Append("<td>" + row["ShiftQty"] + "</td></tr>");
            //    }

            //    body.Append("</table></Center>");

            //    var from = BusinessConfig.Default.Message_Email_From;
            //    var to = BusinessConfig.Default.Lend_Message_Email_To;
            //    var subject = dt.Rows[0]["StockNameA"] + "到" + dt.Rows[0]["StockNameB"] + "(提醒移仓单待出库邮件)";

            //    MailService.SendMail2IPP3Internal(from, to, subject, body.ToString());
            //}

            #endregion

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"审核了编号为\"{1}\"的移仓单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Shift_Verify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 取消审核移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo CancelVerifyRequest(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAudit);

            ShiftRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ShiftRequestStatus.Origin;
                entityToUpdate.AuditDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();

                result = UpdateRequestStatus(entityToUpdate);


                //发送审核移仓单据消息
                EventPublisher.Publish(new CancelAuditShiftRequestInfoMessage()
                {
                    ShiftRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            result.ShiftItemInfoList = entityToUpdate.ShiftItemInfoList;

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消审核了编号为\"{1}\"的移仓单", entityToUpdate.AuditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Shift_CancelVerify, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// PO作废移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo AbandonRequestForPO(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.AbandonForPO);

            var originalItems = entityToUpdate.ShiftItemInfoList;

            ShiftRequestInfo result;

            if (entityToUpdate.RequestStatus == ShiftRequestStatus.Abandon)
            {
                result = entityToUpdate;
            }
            else
            {
                //初始化统一库存调整实体
                var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.AbandonForPO);

                using (var scope = new TransactionScope())
                {
                    entityToUpdate.RequestStatus = ShiftRequestStatus.Abandon;

                    result = UpdateRequestStatus(entityToUpdate);

                    if (originalItems != null && originalItems.Count > 0)
                    {
                        result.ShiftItemInfoList = originalItems;

                        foreach (var item in originalItems)
                        {
                            inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                            {
                                AdjustQuantity = item.ShiftQuantity,
                                ProductSysNo = item.ShiftProduct.SysNo,
                                StockSysNo = (int)entityToUpdate.SourceStock.SysNo
                            });
                        }

                        if (inventoryAdjustContract.AdjustItemList.Count > 0)
                        {
                            string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                            if (!string.IsNullOrEmpty(adjustResult))
                            {
                                throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                            }
                        }
                    }

                    scope.Complete();
                }

                #region write log

                ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"作废了编号为\"{1}\"的移仓单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                , BizLogType.St_Shift_Abandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

                #endregion
            }

            return result;
        }

        /// <summary>
        /// 作废移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo AbandonRequest(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            var originalItems = entityToUpdate.ShiftItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.Abandon);

            ShiftRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ShiftRequestStatus.Abandon;

                result = UpdateRequestStatus(entityToUpdate);

                if (originalItems != null && originalItems.Count > 0)
                {
                    result.ShiftItemInfoList = originalItems;

                    foreach (var item in originalItems)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = item.ShiftQuantity,
                            ProductSysNo = item.ShiftProduct.SysNo,
                            StockSysNo = (int)entityToUpdate.SourceStock.SysNo
                        });
                    }

                    if (inventoryAdjustContract.AdjustItemList.Count > 0)
                    {
                        string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                        if (!string.IsNullOrEmpty(adjustResult))
                        {
                            throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                        }
                    }
                }

                ///发送作废移仓单message
                EventPublisher.Publish(new VoidShiftRequestInfoMessage() 
                {
                    ShiftRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });


                scope.Complete();
            }

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"作废了编号为\"{1}\"的移仓单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                    , BizLogType.St_Shift_Abandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 取消作废移仓单
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo CancelAbandonRequest(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            var originalItems = entityToUpdate.ShiftItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.CancelAbandon);

            ShiftRequestInfo result;
            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ShiftRequestStatus.Origin;

                result = UpdateRequestStatus(entityToUpdate);

                if (originalItems != null && originalItems.Count > 0)
                {
                    result.ShiftItemInfoList = originalItems;

                    foreach (var item in originalItems)
                    {
                        inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                        {
                            AdjustQuantity = -item.ShiftQuantity,
                            ProductSysNo = item.ShiftProduct.SysNo,
                            StockSysNo = (int)entityToUpdate.SourceStock.SysNo
                        });
                    }

                    if (inventoryAdjustContract.AdjustItemList.Count > 0)
                    {
                        string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContract);
                        if (!string.IsNullOrEmpty(adjustResult))
                        {
                            throw new BizException("Inventory Adjust ERROR: " + adjustResult);
                        }
                    }
                }

                //发送取消作废移仓单message
                EventPublisher.Publish(new CancelVoidShiftRequestInfoMessage() 
                {
                    ShiftRequestInfoSysNo = entityToUpdate.SysNo.Value,
                    CurrentUserSysNo=ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"取消作废了编号为\"{1}\"的移仓单", entityToUpdate.EditUser.SysNo, entityToUpdate.RequestID)
                  , BizLogType.St_Shift_CancelAbandon, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 移仓单入库, PS.出库操作的逻辑/是否由WMS触发？是否需要调用Inventory的接口? 需要进一步确认。
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo OutStockRequest(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);

            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.OutStock);

            var originalItems = entityToUpdate.ShiftItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.OutStock);

            ShiftRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ShiftRequestStatus.OutStock;
                entityToUpdate.InStockDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();
                result = UpdateRequestStatus(entityToUpdate);
                result.ShiftItemInfoList = new List<ShiftRequestItemInfo>();

                foreach (var item in originalItems)
                {
                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = -item.ShiftQuantity,
                        ProductSysNo = item.ShiftProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.SourceStock.SysNo
                    });

                }

                //TODO: Check the following 2 methods
                //InventoryAdjustBP.AdjustInventory(inventoryAdjustEntity);
                //InventoryAdjustBP.AdjustInventoryShiftQty(adjustShiftQtyEntity);

                scope.Complete();
            }

            #region send email

            //if (result.ShipViaTerm != null && result.ShipViaTerm.IndexOf("紧急") != -1)
            //{
            //    var dt = shiftRequestDA.GetInventoryTransferItemforEmail(newEntity.InventoryTransferSysNumber, newEntity.CompanyCode);

            //    var body = new StringBuilder();
            //    body.Append("<Center>移仓单  " + dt.Rows[0]["ShiftID"]);
            //    body.Append("<table border =1>");
            //    body.Append("<tr>");
            //    body.Append("<td>商品ID</td>");
            //    body.Append("<td>商品名称</td>");
            //    body.Append("<td>数量</td>");
            //    body.Append("</tr>");

            //    foreach (DataRow row in dt.Rows)
            //    {
            //        body.Append("<tr><td>" + row["ProductID"] + "</td>");
            //        body.Append("<td>" + row["ProductName"] + "</td>");
            //        body.Append("<td>" + row["ShiftQty"] + "</td></tr>");
            //    }

            //    body.Append("</table></Center>");

            //    var from = BusinessConfig.Default.Message_Email_From;
            //    var to = BusinessConfig.Default.Lend_Message_Email_To;
            //    var subject = dt.Rows[0]["StockNameA"] + "到" + dt.Rows[0]["StockNameB"] + "(提醒移仓单入库邮件)";

            //    MailService.SendMail2IPP3Internal(from, to, subject, body.ToString());
            //}

            #endregion

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"出库了编号为\"{1}\"的移仓单", entityToUpdate.OutStockUser.SysNo, entityToUpdate.RequestID)
              , BizLogType.St_Shift_OutStock, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        /// <summary>
        /// 移仓单入库, PS.出库操作的逻辑/是否由WMS触发？是否需要调用Inventory的接口? 需要进一步确认。
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public virtual ShiftRequestInfo InStockRequest(ShiftRequestInfo entityToUpdate)
        {
            PreCheckShiftRequestInfoForUpdateStatus(entityToUpdate);
            PreCheckOriginShiftRequestInfo(entityToUpdate, InventoryAdjustSourceAction.InStock);

            var originalItems = entityToUpdate.ShiftItemInfoList;

            //初始化统一库存调整实体
            var inventoryAdjustContract = InitInventoryAdjustContract(entityToUpdate, InventoryAdjustSourceAction.InStock);

            ShiftRequestInfo result;

            using (var scope = new TransactionScope())
            {
                entityToUpdate.RequestStatus = ShiftRequestStatus.InStock;
                entityToUpdate.InStockDate = DateTime.Now;//BusinessUtility.GetCurrentDateTime();
                result = UpdateRequestStatus(entityToUpdate);
                result.ShiftItemInfoList = new List<ShiftRequestItemInfo>();

                foreach (var item in originalItems)
                {
                    item.InStockQuantity = item.ShiftQuantity;

                    result.ShiftItemInfoList.Add(shiftRequestDA.UpdateShiftItem(item));

                    inventoryAdjustContract.AdjustItemList.Add(new InventoryAdjustItemInfo
                    {
                        AdjustQuantity = item.ShiftQuantity,
                        ProductSysNo = item.ShiftProduct.SysNo,
                        StockSysNo = (int)entityToUpdate.SourceStock.SysNo
                    });

                }

                //TODO: Check the following 2 methods
                //InventoryAdjustBP.AdjustInventory(inventoryAdjustEntity);
                //InventoryAdjustBP.AdjustInventoryShiftQty(adjustShiftQtyEntity);

                scope.Complete();
            }

            #region send email

            //if (result.ShipViaTerm != null && result.ShipViaTerm.IndexOf("紧急") != -1)
            //{
            //    var dt = shiftRequestDA.GetInventoryTransferItemforEmail(newEntity.InventoryTransferSysNumber, newEntity.CompanyCode);

            //    var body = new StringBuilder();
            //    body.Append("<Center>移仓单  " + dt.Rows[0]["ShiftID"]);
            //    body.Append("<table border =1>");
            //    body.Append("<tr>");
            //    body.Append("<td>商品ID</td>");
            //    body.Append("<td>商品名称</td>");
            //    body.Append("<td>数量</td>");
            //    body.Append("</tr>");

            //    foreach (DataRow row in dt.Rows)
            //    {
            //        body.Append("<tr><td>" + row["ProductID"] + "</td>");
            //        body.Append("<td>" + row["ProductName"] + "</td>");
            //        body.Append("<td>" + row["ShiftQty"] + "</td></tr>");
            //    }

            //    body.Append("</table></Center>");

            //    var from = BusinessConfig.Default.Message_Email_From;
            //    var to = BusinessConfig.Default.Lend_Message_Email_To;
            //    var subject = dt.Rows[0]["StockNameA"] + "到" + dt.Rows[0]["StockNameB"] + "(提醒移仓单入库邮件)";

            //    MailService.SendMail2IPP3Internal(from, to, subject, body.ToString());
            //}

            #endregion

            #region write log

            ExternalDomainBroker.CreateOperationLog(string.Format("用户\"{0}\"入库了编号为\"{1}\"的移仓单", entityToUpdate.InStockUser.SysNo, entityToUpdate.RequestID)
              , BizLogType.St_Shift_InStock, (int)entityToUpdate.SysNo, entityToUpdate.CompanyCode);

            #endregion

            return result;
        }

        public virtual bool EditGoldenTaxNo(string GoldenTaxNo, int stSysNo)
        {
            shiftRequestDA.UpdateStshiftItemGoldenTaxNo(GoldenTaxNo, stSysNo);
            return true;
        }

        #region 私有方法

        private ShiftRequestInfo CloneRequestCommonInfo(ShiftRequestInfo entity)
        {
            return new ShiftRequestInfo()
            {
                CreateDate = entity.CreateDate,
                CreateUser = entity.CreateUser,
                EditUser = entity.EditUser,
                EditDate = entity.EditDate,
                AuditUser = entity.AuditUser,
                AuditDate = entity.AuditDate,
                InStockUser = entity.InStockUser,
                InStockDate = entity.InStockDate,
                OutStockUser = entity.OutStockUser,
                OutStockDate = entity.OutStockDate,
                SourceStock = entity.SourceStock,
                TargetStock = entity.TargetStock,
                Note = entity.Note,
                ShiftShippingType = entity.ShiftShippingType,
                RequestStatus = entity.RequestStatus,
                ProductLineSysno = entity.ProductLineSysno,
                CompanyCode = entity.CompanyCode
            };
        }

        private InventoryAdjustContractInfo InitInventoryAdjustContract(ShiftRequestInfo entity, InventoryAdjustSourceAction sourceAction)
        {
            var inventoryAdjustContract = new InventoryAdjustContractInfo
            {
                SourceBizFunctionName = InventoryAdjustSourceBizFunction.Inventory_ShiftRequest,
                SourceActionName = sourceAction,
                ReferenceSysNo = entity.SysNo.ToString(),
                AdjustItemList = new List<InventoryAdjustItemInfo>()
            };
            return inventoryAdjustContract;
        }

        private void PreCheckShiftRequestInfoForCreate(ShiftRequestInfo entityToCreate)
        {
            if (entityToCreate == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToCreate.ShiftItemInfoList == null || entityToCreate.ShiftItemInfoList.Count == 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_hasNotItem");
            }

            if (entityToCreate.SourceStock == null || entityToCreate.SourceStock.SysNo == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToCreate.TargetStock == null || entityToCreate.TargetStock.SysNo == null || entityToCreate.TargetStock.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToCreate.SourceStock.SysNo == entityToCreate.TargetStock.SysNo)
            {
                BizExceptionHelper.Throw("InventoryTransfer_StockAandStockBCannotBetheSame");
            }

            if (entityToCreate.ShiftType != ShiftRequestType.Positive)
            {
                BizExceptionHelper.Throw("InventoryTransfer_TransferTypeMustBePositiveAndCreateByPO");
            }

        }

        private void PreCheckShiftRequestInfoForUpdate(ShiftRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }
        }

        private void PreCheckShiftRequestInfoForUpdateStatus(ShiftRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }
        }

        private void PreCheckShiftRequestInfoForUpdateSpecialShiftType(ShiftRequestInfo entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToUpdate.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToUpdate.SpecialShiftType == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (entityToUpdate.SpecialShiftType != SpecialShiftRequestType.Default && entityToUpdate.SpecialShiftType != SpecialShiftRequestType.HandWork)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            //TODO: Load UserInfo
            //if (string.IsNullOrEmpty(entityToUpdate.CreateUser.EmailAddress))
            //{
            //    BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            //}
        }

        private void PreCheckOriginShiftRequestInfo(ShiftRequestInfo entity, InventoryAdjustSourceAction actionType)
        {
            if (entity == null)
            {
                BizExceptionHelper.Throw("InventoryTransfer_cannotfindOriginalInventoryTransfer");
            }

            //if (entity.CreateUser != null && entity.CreateUser.SysNo == ServiceContext.Current.UserSysNo
            //    && actionType == InventoryAdjustSourceAction.Audit)
            //{
            //    throw new BizException("创建人和审核人不能相同");
            //}


            //移仓单当前状态检查
            if ((actionType == InventoryAdjustSourceAction.Update || actionType == InventoryAdjustSourceAction.Audit
                || actionType == InventoryAdjustSourceAction.Abandon) && entity.RequestStatus != ShiftRequestStatus.Origin)
            {
                BizExceptionHelper.Throw("InventoryTransfer_StatusMustBeOriginal");
            }
            else if (actionType == InventoryAdjustSourceAction.AbandonForPO)
            {
                if (entity.RequestStatus == ShiftRequestStatus.OutStock)
                {
                    BizExceptionHelper.Throw("InventoryTransfer_StatusIsOutStock", entity.RequestID);
                }
                else if (entity.RequestStatus == ShiftRequestStatus.PartlyInStock)
                {
                    BizExceptionHelper.Throw("InventoryTransfer_StatusIsPartlyInStock", entity.RequestID);
                }
                else if (entity.RequestStatus == ShiftRequestStatus.InStock)
                {
                    BizExceptionHelper.Throw("InventoryTransfer_StatusIsInStock", entity.RequestID);
                }
            }
            else if (actionType == InventoryAdjustSourceAction.CancelAbandon && entity.RequestStatus != ShiftRequestStatus.Abandon)
            {
                BizExceptionHelper.Throw("InventoryTransfer_StatusMustBeAbandon");
            }
            else if ((actionType == InventoryAdjustSourceAction.CancelAudit || actionType == InventoryAdjustSourceAction.OutStock)
                && entity.RequestStatus != ShiftRequestStatus.Verified)
            {
                BizExceptionHelper.Throw("InventoryTransfer_StatusMustBeVerified");
            }
            else if (actionType == InventoryAdjustSourceAction.CancelAudit && entity.IsScanned == true)
            {
                BizExceptionHelper.Throw("InventoryTransfer_StatusIsScanned");
            }
            else if (actionType == InventoryAdjustSourceAction.InStock && entity.RequestStatus != ShiftRequestStatus.OutStock)
            {
                BizExceptionHelper.Throw("InventoryTransfer_StatusMustBeOutStock");
            }

            if (entity.ShiftItemInfoList == null || entity.ShiftItemInfoList.Count <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_hasNotItem");
            }

        }

        private void PreCheckDuplicatedShiftItem(ShiftRequestInfo entityToUpdate)
        {
            if (entityToUpdate.ShiftItemInfoList != null && entityToUpdate.ShiftItemInfoList.Count > 0)
            {
                var itemList = new List<ShiftRequestItemInfo>();

                foreach (var si in entityToUpdate.ShiftItemInfoList)
                {
                    if (itemList.Find(p => p.ShiftProduct.SysNo == si.ShiftProduct.SysNo) != null)
                    {
                        BizExceptionHelper.Throw("InventoryTransfer_ItemDuplicatedValue", si.ShiftProduct.SysNo.ToString());
                    }

                    itemList.Add(si);
                }
            }
        }

        private void PreCheckShiftItemInfo(ShiftRequestItemInfo shiftItemInfo)
        {
            if (shiftItemInfo.ShiftProduct == null || shiftItemInfo.ShiftProduct.SysNo <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_Incomplete");
            }

            if (shiftItemInfo.ShiftQuantity <= 0)
            {
                BizExceptionHelper.Throw("InventoryTransfer_ItemInComplete");
            }

            if (shiftItemInfo.ShiftProduct.ProductPriceInfo == null)
            {
                BizExceptionHelper.Throw("Common_CannotFindPriceInformation", shiftItemInfo.ShiftProduct.SysNo.ToString());
            }
        }

        private void PreCheckShiftItemInfoForInventoryAdjust(ShiftRequestItemInfo shiftItemInfo)
        {
            //所有与库存调整的逻辑都移至库存统一调整接口
        }

        #endregion 私有方法

        #region 需要确认的业务逻辑

        public virtual ShiftRequestInfo SetSpecialStatus(ShiftRequestInfo newEntity)
        {
            //Need to confirm
            return null;
        }

        public virtual ShiftRequestInfo VerifyAutoShift(string autoShiftSysNo, int editUsersysNo)
        {
            //Need to confirm
            return null;
        }

        public virtual ShiftRequestInfo CancelVerifyAutoShift(string autoShiftSysNo, int editUserSysNo)
        {
            //Need to confirm
            return null;
        }

        public virtual void SyncSAP()
        {

        }

        #endregion 需要确认的业务逻辑


        #region 仓库移仓配置

        private void ThrowBizException(string key)
        {
            throw new BizException(ResouceManager.GetMessageString("Inventory.ShiftRequest", key));
        }
        private void StockShiftConfigCheck(StockShiftConfigInfo info)
        {
            if (info.OutStockSysNo < 1 || info.InStockSysNo < 1 || info.InStockSysNo == info.OutStockSysNo)
            {
                ThrowBizException("Inventory_StockShiftConfig_StockIsError");
            }
            if (info.SPLInterval < 0 || info.ShipInterval < 0)
            {
                ThrowBizException("Inventory_StockShiftConfig_IntervalIsError");
            }
        }

        public StockShiftConfigInfo CreateStockShiftConfig(StockShiftConfigInfo info)
        {
            StockShiftConfigCheck(info);
            if (IsExistStockShiftConfig(info))
            {
                ThrowBizException("Inventory_StockShiftConfig_IsExist");
            }
            return shiftRequestDA.CreateStockShiftConfig(info);
        }
        /// <summary>
        /// 返回true表示修改成功，否则修改失败
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public void UpdateStockShiftConfig(StockShiftConfigInfo info)
        {
            StockShiftConfigCheck(info);
            if (!info.SysNo.HasValue || !shiftRequestDA.UpdateStockShiftConfig(info))
            {
                ThrowBizException("Inventory_StockShiftConfig_IsNotExist");
            }
        }
        public StockShiftConfigInfo GetStockShiftConfigBySysNo(int sysNo)
        {
            return shiftRequestDA.GetStockShiftConfigBySysNo(sysNo);
        }

        /// <summary>
        /// 根据移入移出仓库编号和移仓类型判断是否已经存在相应记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool IsExistStockShiftConfig(StockShiftConfigInfo info)
        {
            return shiftRequestDA.IsExistStockShiftConfig(info);
        }
        #endregion
    }
}
