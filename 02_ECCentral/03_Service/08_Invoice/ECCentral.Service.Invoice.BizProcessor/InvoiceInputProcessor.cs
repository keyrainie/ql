using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.EventMessage.VendorPortal;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor.ETPCalculator;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(InvoiceInputProcessor))]
    public class InvoiceInputProcessor
    {
        private IInvoiceInputDA DA = ObjectFactory<IInvoiceInputDA>.Instance;
        private ICommonBizInteract createLog = ObjectFactory<ICommonBizInteract>.Instance;

        #region Load

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取APInvoice全部信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo LoadAPInvoiceWithItemsBySysNo(int sysNo)
        {
            APInvoiceInfo apInvoice = this.GetAPInvoiceMasterBySysNo(sysNo);
            if (apInvoice != null)
            {
                apInvoice.POItemList = this.GetPOItemsByDocNo(sysNo);
                apInvoice.InvoiceItemList = this.GetInvoiceItemsByDocNo(sysNo);
            }
            return apInvoice;
        }

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取APInvoiceMaster信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo GetAPInvoiceMasterBySysNo(int sysNo)
        {
            return DA.GetAPInvoiceMasterBySysNo(sysNo);
        }

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取POItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> GetPOItemsByDocNo(int docNo)
        {
            List<APInvoicePOItemInfo> poItem = DA.GetPOItemsByDocNo(docNo);
            if (poItem != null && poItem.Count > 0)
            {
                poItem.ForEach(p =>
                {
                    List<EIMSInfo> EIMSlist = ExternalDomainBroker.LoadPurchaseOrderEIMSInfo(p.PoNo.Value);
                    if (EIMSlist != null && EIMSlist.Count > 0)
                    {
                        p.EIMSNoList = EIMSlist.Select(x => x.EIMSSysNo.ToString()).Join(",");
                    }
                });
            }
            return poItem;
        }

        /// <summary>
        /// 根据APInvoiceMaster系统编号获取InvoiceItems列表
        /// </summary>
        /// <param name="docNo"></param>
        /// <returns></returns>
        public virtual List<APInvoiceInvoiceItemInfo> GetInvoiceItemsByDocNo(int docNo)
        {
            return DA.GetInvoiceItemsByDocNo(docNo);
        }

        /// <summary>
        /// 加载供应商未录入的POItems
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> LoadNotInputPOItems(APInvoiceItemInputEntity request)
        {
            if (!request.VendorSysNo.HasValue || request.VendorSysNo.Value == 0)
            {
                ThrowBizException("InvoiceInput_VendorSysNoIsEmpty");
            }

            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();
            result = DA.LoadNotInputPOItems(request);

            if (result != null && result.Count > 0)
            {
                result.ForEach(p =>
                {
                    List<EIMSInfo> EIMSlist = ExternalDomainBroker.LoadPurchaseOrderEIMSInfo(p.PoNo.Value);
                    if (EIMSlist != null && EIMSlist.Count > 0)
                        p.EIMSNoList = EIMSlist.Select(x => x.EIMSSysNo.ToString()).Join(",");
                });
            }

            //保留原有的Items且去掉重复的
            if (request.POItemList.Count > 0)
            {
                result = request.POItemList.Union(result, new APInvoiceInputPOItemComparer()).ToList();

                result.ForEach(item =>
                {
                    APInvoicePOItemInfo ent = request.POItemList.Find(t =>
                    {
                        return t.PoNo == item.PoNo
                            && t.OrderType == item.OrderType
                            && t.BatchNumber == item.BatchNumber;
                    });
                });
            }

            result = result.OrderByDescending(x => x.SysNo)
                    .ThenBy(x => x.OrderType)
                    .ThenByDescending(x => x.PoBaselineDate)
                    .ToList();

            return result;
        }

        #endregion Load

        #region BatchAction

        /// <summary>
        /// 拒绝审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void RefuseAudit(int sysNo)
        {
            APInvoiceInfo entity = DA.GetAPInvoiceMasterBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("InvoiceInput_APInvoiceMasterNotFound", sysNo);
            }

            if (entity.Status != APInvoiceMasterStatus.NeedAudit)
            {
                ThrowBizException("InvoiceInput_RefuseAudit_StatusNotMatchNeedAudit");
            }

            entity.Status = APInvoiceMasterStatus.Refuse;

            DA.UpdateAPInvoiceStatusWithConfirmUser(entity);

            if (entity.VendorPortalSysNo.HasValue && entity.VendorPortalSysNo.Value != 0)
            {
                SSBSend(
                    entity.VendorPortalSysNo.Value,
                    VendorPortalType.D.ToString(),
                    string.Empty);
            }
            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_RefuseAudit", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.InvoiceInput_RefuseAudit
               , sysNo
               , entity.CompanyCode);
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Audit(int sysNo)
        {
            APInvoiceInfo entity = DA.GetAPInvoiceMasterBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("InvoiceInput_APInvoiceMasterNotFound", sysNo);
            }

            if (entity.Status != APInvoiceMasterStatus.NeedAudit)
            {
                ThrowBizException("InvoiceInput_Audit_StatusNotMatchNeedAudit");
            }

            entity.POItemList = DA.GetPOItemsByDocNo(sysNo);
            entity.InvoiceItemList = DA.GetInvoiceItemsByDocNo(sysNo);

            PayableInfo payable = new PayableInfo();
            payable.CompanyCode = entity.CompanyCode;
            payable.Note = string.Empty;
            var flag = false;
            entity.InvoiceItemList.ForEach(x =>
            {
                if (payable.Note.Length <= 480)
                {
                    payable.Note += x.InvoiceNo + ",";
                }
                else
                {
                    flag = true;
                }
            });

            if (flag)
            {
                payable.Note += "...";
                entity.Note = GetMessageString("InvoiceInput_InvoiceTooMany");
            }

            //去掉末位符号
            payable.Note = payable.Note.TrimEnd(',');

            entity.Status = APInvoiceMasterStatus.AuditPass;
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            string payableSysnos = string.Empty;

            foreach (var item in entity.POItemList)
            {
                //解决逐个审核造成的单据重复问题
                if (DA.CheckPOItemAudit(item))
                {
                    ThrowBizException("InvoiceInput_Audit_AuditFailed",
                            entity.SysNo,
                            item.OrderType,
                            item.PoNo,
                            item.BatchNumber);
                }

                if (item.OrderType == PayableOrderType.PO || item.OrderType == PayableOrderType.VendorSettleOrder)
                {
                    payable.OrderType = item.OrderType;
                    payable.OrderSysNo = item.PoNo;
                    payable.InvoiceStatus = PayableInvoiceStatus.Complete;
                    payable.BatchNumber = item.BatchNumber;
                    item.ETP = ETPCalculatorHelper.GetETPByPayPeriod(payable, DateTime.Now);
                }
                //else if (item.OrderType == PayableOrderType.SubInvoice)
                //{
                //    payable.OrderType = item.OrderType;
                //    payable.OrderSysNo = item.PoNo;
                //    payable.InvoiceStatus = PayableInvoiceStatus.Complete;
                //    payable.BatchNumber = item.BatchNumber;
                //    item.ETP = System.DateTime.Now;
                //}
            }

            var ssbflag = false;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                DA.UpdateAPInvoiceStatusWithConfirmUser(entity);
                DA.UpdatePOItemStatus(entity.SysNo.Value);
                DA.UpdateInvoItemStatus(entity.SysNo.Value);

                #region 同步更新Finance_Pay表

                entity.POItemList.ForEach(x =>
                {
                    payable.SysNo = DA.GetFinancePaySysNo(
                        x.PoNo.Value,
                        x.OrderType.Value,
                        x.BatchNumber);
                    //发票状态为完整
                    payable.InvoiceStatus = PayableInvoiceStatus.Complete;

                    //检查差异金额，如果没有差异，发票具体状态为正确；否则为金额不符
                    if (entity.DiffTaxAmt == 0)
                    {
                        payable.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                    }
                    else
                    {
                        payable.InvoiceFactStatus = PayableInvoiceFactStatus.MoneyWrong;
                    }

                    payable.UpdateInvoiceUserSysNo = entity.UpdateInvoiceUserSysNo;
                    payable.EstimatedTimeOfPay = x.ETP;
                    payableSysnos += payable.SysNo.ToString() + ",";

                    bool isAdvanced = ObjectFactory<PayItemProcessor>.Instance.IsAdvanced(PayableOrderType.PO, x.PoNo.Value);
                    if (isAdvanced)
                    {
                        ObjectFactory<PayableProcessor>.Instance.UpdatePayableInvoiceStatus(payable);
                    }
                    else
                    {
                        ObjectFactory<PayableProcessor>.Instance.UpdatePayableInvoiceStatusWithEtp(payable);
                    }
                });

                #endregion 同步更新Finance_Pay表

                scope.Complete();

                ssbflag = true;
            }

            if (ssbflag
                && entity.VendorPortalSysNo.HasValue
                && entity.VendorPortalSysNo.Value != 0)
            {
                SSBSend(
                    entity.VendorPortalSysNo.Value,
                    VendorPortalType.A.ToString(),
                    string.Empty);
            }

            payableSysnos = payableSysnos.TrimEnd(',');
            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_Audit", ServiceContext.Current.UserSysNo, sysNo, payableSysnos)
               , BizLogType.InvoiceInput_PassAudit
               , sysNo
               , entity.CompanyCode);
        }

        /// <summary>
        /// 撤销审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void CancelAudit(int sysNo)
        {
            APInvoiceInfo entity = DA.GetAPInvoiceMasterBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("InvoiceInput_APInvoiceMasterNotFound", sysNo);
            }
            if (entity.Status != APInvoiceMasterStatus.NeedAudit)
            {
                ThrowBizException("InvoiceInput_CancelAudit_StatusNotMatchNeedAudit");
            }
            entity.Status = APInvoiceMasterStatus.Origin;
            DA.UpdateAPInvoiceStatus(entity);

            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_CancelAudit", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.InvoiceInput_CancelAudit
               , sysNo
               , entity.CompanyCode);
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Submit(int sysNo)
        {
            APInvoiceInfo entity = DA.GetAPInvoiceMasterBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("InvoiceInput_APInvoiceMasterNotFound", sysNo);
            }

            if (entity.Status != APInvoiceMasterStatus.Origin)
            {
                ThrowBizException("InvoiceInput_Submit_StatusNotMatchOrigin");
            }
            entity.Status = APInvoiceMasterStatus.NeedAudit;
            DA.UpdateAPInvoiceStatus(entity);

            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_Submit", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.InvoiceInput_SubmitAudit
               , sysNo
               , entity.CompanyCode);
        }

        /// <summary>
        /// 退回供应商
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="vpCancelReason"></param>
        public virtual void VPCancel(int sysNo, string vpCancelReason)
        {
            APInvoiceInfo entity = DA.GetAPInvoiceMasterBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("InvoiceInput_APInvoiceMasterNotFound", sysNo);
            }

            if (entity.Status != APInvoiceMasterStatus.Origin)
            {
                ThrowBizException("InvoiceInput_VPCancel_StatusNotMatchOrigin");
            }
            entity.Status = APInvoiceMasterStatus.VPCancel;

            DA.UpdateAPInvoiceStatusWithConfirmUser(entity);

            if (entity.VendorPortalSysNo.HasValue && entity.VendorPortalSysNo.Value != 0)
            {
                SSBSend(
                    entity.VendorPortalSysNo.Value,
                    VendorPortalType.C.ToString(),
                    vpCancelReason);
            }

            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_VPCancel", ServiceContext.Current.UserSysNo, sysNo)
               , BizLogType.InvoiceInput_RefuseAudit
               , sysNo
               , entity.CompanyCode);
        }

        /// <summary>
        /// 退回发票时发送SSB消息给供应商
        /// </summary>
        /// <param name="vendorPortalSysNo"></param>
        /// <param name="status"></param>
        /// <param name="vpCancelReason"></param>
        public void SSBSend(int vendorPortalSysNo, string status, string vpCancelReason)
        {
            string editUserName = "";
            var editUser = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo);
            if (editUser != null)
            {
                editUserName = editUser.UserName;
            }

            VendorPortalInvoiceChangeStatusMessage msg = new VendorPortalInvoiceChangeStatusMessage()
            {
                Status = status,
                SysNo = vendorPortalSysNo,
                EditUser = editUserName,
                Note = vpCancelReason,
                MsgType = AppSettingManager.GetSetting("Invoice", "VendorPortal_SSB_Header_MsgType"),
            };
            EventPublisher.Publish<VendorPortalInvoiceChangeStatusMessage>(msg);
        }

        #endregion BatchAction

        #region Action--Create&Update

        /// <summary>
        /// 创建APInvoice
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo CreateAPInvoice(APInvoiceInfo entity)
        {
            //计算PayableAmt,PayableTaxAmt;初始化状态
            entity.POItemList.ForEach(x =>
            {
                CalculatePO(x);
                x.Status = APInvoiceItemStatus.Deactive;
                VendorFinanceInfo vendorFinance = ExternalDomainBroker.GetVendorFinanceInfoBySysNo(entity.VendorSysNo.Value);
                if (vendorFinance == null)
                {
                    throw new BizException(GetMessageString("InvoiceInput_CreateAPInvoice_VendorFinanceInfoInValid", x.PoNo));
                }

                x.PoPaymentTerm = vendorFinance.PayPeriod ?? -1;
            });

            entity.PoNetAmt = entity.POItemList.Sum(x =>
            {
                return x.PaymentAmt ?? 0;
            });
            entity.PoNetTaxAmt = entity.POItemList.Sum(x =>
            {
                return x.PayableTaxAmt ?? 0;
            });
            entity.Status = APInvoiceMasterStatus.Origin;
            entity.VendorTaxRate = InvoiceConst.VendorTaxRate - 1;

            //初始化状态
            entity.InvoiceItemList.ForEach(x =>
            {
                x.Status = APInvoiceItemStatus.Deactive;
            });

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                entity.SysNo = DA.InsertAPInvoiceMaster(entity);
                DA.InsertPOItem(entity.POItemList, entity.SysNo.Value);
                DA.InsertInvoItem(entity.InvoiceItemList, entity.SysNo.Value);

                scope.Complete();
            }

            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_CreateAPInvoice", ServiceContext.Current.UserSysNo,
                entity.SysNo), BizLogType.InvoiceInput_Add, entity.SysNo.Value, entity.CompanyCode);

            return entity;
        }

        /// <summary>
        /// 创建且提交审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo SubmitWithSaveAPInvoice(APInvoiceInfo entity)
        {
            if (entity.SysNo.HasValue && entity.SysNo.Value > 0)
            {
                UpdateAPInvoice(entity);
            }
            else
            {
                entity = CreateAPInvoice(entity);
            }
            entity.Status = APInvoiceMasterStatus.NeedAudit;
            DA.UpdateAPInvoiceStatus(entity);

            createLog.CreateOperationLog(GetMessageString("InvoiceInput_Log_SubmitWithSaveAPInvoice", ServiceContext.Current.UserSysNo, entity.SysNo)
               , BizLogType.InvoiceInput_SubmitAudit
               , entity.SysNo.Value
               , entity.CompanyCode);
            return entity;
        }

        /// <summary>
        /// 更新APInvoice信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual APInvoiceInfo UpdateAPInvoice(APInvoiceInfo entity)
        {
            #region Fields

            List<APInvoicePOItemInfo> poItems = new List<APInvoicePOItemInfo>();
            List<APInvoicePOItemInfo> transitionPOList = new List<APInvoicePOItemInfo>();
            List<APInvoicePOItemInfo> unionPOList = new List<APInvoicePOItemInfo>();
            List<POItemKeyEntity> exceptPO = new List<POItemKeyEntity>();
            List<POItemKeyEntity> intersectPO = new List<POItemKeyEntity>();
            List<POItemKeyEntity> deletePO = new List<POItemKeyEntity>();

            List<APInvoiceInvoiceItemInfo> invoItems = new List<APInvoiceInvoiceItemInfo>();
            List<APInvoiceInvoiceItemInfo> transitionInvoList = new List<APInvoiceInvoiceItemInfo>();
            List<string> exceptInvo = new List<string>();
            List<string> intersectInvo = new List<string>();
            List<string> deleteInvo = new List<string>();

            #endregion Fields

            #region 获得表中原始数据

            poItems = DA.GetPOItemsByDocNo(entity.SysNo.Value);
            invoItems = DA.GetInvoiceItemsByDocNo(entity.SysNo.Value);

            #endregion 获得表中原始数据

            #region 事务处理

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                #region 获得处理条件

                //获得需要insert的poNo集合
                exceptPO = ((from c in entity.POItemList
                             select new POItemKeyEntity
                             {
                                 PONo = c.PoNo.Value,
                                 OrderType = c.OrderType.Value,
                                 BatchNumber = c.BatchNumber
                             }).Except(
                                from e in poItems
                                select new POItemKeyEntity
                                {
                                    PONo = e.PoNo.Value,
                                    OrderType = e.OrderType.Value,
                                    BatchNumber = e.BatchNumber
                                },
                                new POItemKeyComparer())).ToList();

                //获得需要update的poNo集合
                intersectPO = ((from c in entity.POItemList
                                select new POItemKeyEntity
                                {
                                    PONo = c.PoNo.Value,
                                    OrderType = c.OrderType.Value,
                                    BatchNumber = c.BatchNumber
                                }).Intersect(
                                    from e in poItems
                                    select new POItemKeyEntity
                                    {
                                        PONo = e.PoNo.Value,
                                        OrderType = e.OrderType.Value,
                                        BatchNumber = e.BatchNumber
                                    },
                                    new POItemKeyComparer())).ToList();

                //获得需要delete的poNo集合
                deletePO = ((from c in poItems
                             select new POItemKeyEntity
                             {
                                 PONo = c.PoNo.Value,
                                 OrderType = c.OrderType.Value,
                                 BatchNumber = c.BatchNumber
                             }).Except(
                                from e in entity.POItemList
                                select new POItemKeyEntity
                                {
                                    PONo = e.PoNo.Value,
                                    OrderType = e.OrderType.Value,
                                    BatchNumber = e.BatchNumber
                                },
                                new POItemKeyComparer())).ToList();

                //获得需要insert的invoNo集合
                exceptInvo = ((from c in entity.InvoiceItemList
                               select c.InvoiceNo).Except(
                                    from e in invoItems
                                    select e.InvoiceNo)).ToList();

                //获得需要update的invoNo集合
                intersectInvo = ((from c in entity.InvoiceItemList
                                  select c.InvoiceNo).Intersect(
                                        from e in invoItems
                                        select e.InvoiceNo)).ToList();

                //获得需要delete的invoNo集合
                deleteInvo = ((from c in invoItems
                               select c.InvoiceNo).Except(
                                    from e in entity.InvoiceItemList
                                    select e.InvoiceNo)).ToList();

                #endregion 获得处理条件

                #region 处理PO items

                if (exceptPO != null && exceptPO.Count > 0)
                {   //根据PoNo,OrderType,BatchNumber查找出需要insert的items
                    transitionPOList = entity.POItemList
                        .Where(c =>
                        {
                            return exceptPO.Contains(
                                new POItemKeyEntity
                                {
                                    PONo = c.PoNo.Value,
                                    OrderType = c.OrderType.Value,
                                    BatchNumber = c.BatchNumber
                                },
                                new POItemKeyComparer());
                        }).ToList();

                    transitionPOList.ForEach(x =>
                    {
                        CalculatePO(x);
                        VendorFinanceInfo vendorFinance = ExternalDomainBroker.GetVendorFinanceInfoBySysNo(entity.VendorSysNo.Value);
                        x.PoPaymentTerm = vendorFinance.PayPeriod ?? -1;
                    });

                    unionPOList = transitionPOList;
                    DA.InsertPOItem(transitionPOList, entity.SysNo.Value);
                }

                if (intersectPO != null && intersectPO.Count > 0)
                {   //根据PoNo,OrderType,BatchNumber查找出需要update的实体集
                    transitionPOList = entity.POItemList
                        .Where(c =>
                        {
                            return intersectPO.Contains(
                                new POItemKeyEntity
                                {
                                    PONo = c.PoNo.Value,
                                    OrderType = c.OrderType.Value,
                                    BatchNumber = c.BatchNumber
                                },
                                new POItemKeyComparer());
                        }).ToList();

                    unionPOList = unionPOList.Union(transitionPOList, new APInvoiceInputPOItemComparer()).ToList();

                    transitionPOList.ForEach(x =>
                    {
                        x.Status = APInvoiceItemStatus.Deactive;

                        CalculatePO(x);

                        VendorFinanceInfo vendorFinance = ExternalDomainBroker.GetVendorFinanceInfoBySysNo(entity.VendorSysNo.Value);
                        x.PoPaymentTerm = vendorFinance.PayPeriod ?? -1;
                        DA.UpdatePOItem(x, entity.SysNo.Value);
                    });
                }

                //update时计算poNetAmt
                decimal poNetAmt;
                poNetAmt = 0;
                unionPOList.ForEach(x =>
                {
                    CalculatePO(x);
                    poNetAmt = poNetAmt + x.PoAmt.Value - x.EIMSAmt.Value;
                    x.Status = APInvoiceItemStatus.Deactive;
                    VendorFinanceInfo vendorFinance = ExternalDomainBroker.GetVendorFinanceInfoBySysNo(entity.VendorSysNo.Value);
                    x.PoPaymentTerm = vendorFinance.PayPeriod ?? -1;
                });
                entity.PoNetAmt = entity.POItemList.Sum(x => x.PaymentAmt);
                entity.PoNetTaxAmt = entity.POItemList.Sum(x => x.PayableTaxAmt);

                //更新Master
                DA.UpdateAPInvoiceMaster(entity);

                if (deletePO != null && deletePO.Count > 0)
                {
                    DA.DeletePOItems(deletePO);
                }

                #endregion 处理PO items

                #region 处理Invo items

                if (exceptInvo != null && exceptInvo.Count > 0)
                {
                    //根据InvoNo查找出需要insert的items
                    transitionInvoList = entity.InvoiceItemList
                        .Where(c =>
                        {
                            return exceptInvo.ToArray().Contains(c.InvoiceNo);
                        }).ToList();

                    DA.InsertInvoItem(transitionInvoList, entity.SysNo.Value);
                }

                if (intersectInvo != null && intersectInvo.Count > 0)
                {
                    //根据InvoNo查找出需要update的items
                    transitionInvoList = entity.InvoiceItemList
                        .Where(c =>
                        {
                            return intersectInvo.ToArray().Contains(c.InvoiceNo);
                        }).ToList();

                    transitionInvoList.ForEach(x =>
                    {
                        x.Status = APInvoiceItemStatus.Deactive;
                        DA.UpdateInvoItem(x, entity.SysNo.Value);
                    });
                }

                if (deleteInvo != null && deleteInvo.Count > 0)
                {
                    DA.DeleteInvoItems(deleteInvo);
                }

                #endregion 处理Invo items

                scope.Complete();
            }

            #endregion 事务处理

            return entity;
        }

        /// <summary>
        /// 保存前PoItem金额的计算
        /// </summary>
        /// <param name="x"></param>
        private void CalculatePO(APInvoicePOItemInfo x)
        {
            if (x.EIMSAmt.HasValue)
            {
                x.EIMSNetAmt = Math.Round(x.EIMSAmt.Value / InvoiceConst.VendorTaxRate, 2);
            }
            else
            {
                x.EIMSNetAmt = 0;
            }

            x.EIMSNetTaxAmt = x.EIMSAmt - Math.Round(x.EIMSAmt.Value / InvoiceConst.VendorTaxRate, 2);
            x.PoNetAmt = Math.Round(x.PaymentAmt.Value / InvoiceConst.VendorTaxRate, 2);
            x.PayableTaxAmt = x.PaymentAmt - Math.Round(x.PaymentAmt.Value / InvoiceConst.VendorTaxRate, 2);
        }

        #endregion Action--Create&Update

        #region InputStart

        #region InvoiceItemCheck

        /// <summary>
        /// 录入InvoiceItem
        /// </summary>
        /// <param name="inputEntity"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public virtual List<APInvoiceInvoiceItemInfo> InputInvoiceItem(APInvoiceItemInputEntity inputEntity, ref string errorMsg)
        {
            if (!inputEntity.VendorSysNo.HasValue || inputEntity.VendorSysNo.Value == 0)
            {
                ThrowBizException("InvoiceInput_InputInvoiceItem_VendorSysNoIsEmpty");
            }
            if (string.IsNullOrEmpty(inputEntity.ItemsNoList))
            {
                ThrowBizException("InvoiceInput_InputInvoiceItem_ItemsNoList");
            }

            List<APInvoiceInvoiceItemInfo> invoiceList = JudgeInvoice(inputEntity, ref errorMsg);

            //阻止加载数目过多
            if (invoiceList != null && invoiceList.Count > 500)
            {
                ThrowBizException("InvoiceInput_LoadItemTooMany");
            }
            return invoiceList;
        }

        /// <summary>
        /// 录入有效的InvItems
        /// </summary>
        /// <param name="inputEntity"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private List<APInvoiceInvoiceItemInfo> JudgeInvoice(APInvoiceItemInputEntity entity, ref string errorMsg)
        {
            InvoiceItemCheckEntity checkEntity = new InvoiceItemCheckEntity();
            List<APInvoiceInvoiceItemInfo> result = new List<APInvoiceInvoiceItemInfo>();
            if (entity.ItemsNoList.Contains("."))
            {
                checkEntity = GetInvoiceItemInvalid4Split(entity);
            }
            else if (entity.ItemsNoList.Contains("-"))
            {
                checkEntity = GetInvoiceItemInvalid4Range(entity);
            }
            else
            {
                checkEntity = GetInvoiceItemInvalid4Single(entity);
            }
            result = CheckInvoice(entity, checkEntity, out errorMsg);

            return result;
        }

        /// <summary>
        /// 获取有效的发票号码--单个
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private InvoiceItemCheckEntity GetInvoiceItemInvalid4Single(APInvoiceItemInputEntity entity)
        {
            List<string> condition = new List<string>();
            List<string> invalidList = new List<string>();
            InvoiceItemCheckEntity result = new InvoiceItemCheckEntity();
            int a = 0;

            if (!CheckNum(entity.ItemsNoList, ref  a))
            {
                ThrowBizException("InvoiceInput_ItemNoInValid");
            }
            else
            {
                condition.Add(entity.ItemsNoList);
            }
            invalidList = GetInvalidInvoiceNo(condition, entity);
            result.originalList = condition;
            result.invalidList = invalidList;
            return result;
        }

        /// <summary>
        /// 获取有效的发票号码--范围
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private InvoiceItemCheckEntity GetInvoiceItemInvalid4Range(APInvoiceItemInputEntity entity)
        {
            List<string> condition = new List<string>();
            List<string> invalidList = new List<string>();
            InvoiceItemCheckEntity result = new InvoiceItemCheckEntity();
            string[] entityArray = new string[2];
            int invoiceLength;
            string tempJoin;
            int a = 0;
            int b = 0;

            entityArray = entity.ItemsNoList.Split('-');
            invoiceLength = entityArray[0].Length;
            //处理不能转换和范围输入有误的异常
            if (CheckNum(entityArray[0], ref a) && CheckNum(entityArray[1], ref b))
            {
                if (Int32.Parse(entityArray[0]) >= Int32.Parse(entityArray[1]))
                {
                    ThrowBizException("InvoiceInput_OutOfDataRange");
                }

                if ((b - a + 1) > 500)
                {
                    ThrowBizException("InvoiceInput_LoadItemTooMany");
                }

                for (int i = a; i <= b; i++)
                {
                    string tempF = i.ToString();
                    //零补前位的处理
                    if (tempF.Length < entityArray[0].Length)
                    {
                        tempJoin = "";
                        for (int j = 0; j < invoiceLength - tempF.Length; j++)
                        {
                            tempJoin = tempJoin + "0";
                        }
                        condition.Add(tempJoin + tempF);
                    }
                    else
                    {
                        condition.Add(tempF);
                    }
                }
            }
            else
            {
                ThrowBizException("InvoiceInput_ItemNoInValid");
            }

            invalidList = GetInvalidInvoiceNo(condition, entity);
            result.originalList = condition;
            result.invalidList = invalidList;
            return result;
        }

        /// <summary>
        /// 获取有效的发票号码--数组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private InvoiceItemCheckEntity GetInvoiceItemInvalid4Split(APInvoiceItemInputEntity entity)
        {
            List<string> condition = new List<string>();
            List<string> invalidList = new List<string>();
            InvoiceItemCheckEntity result = new InvoiceItemCheckEntity();
            result.invalidList = new List<string>();
            result.originalList = new List<string>();
            string[] splitArray;
            int a = 0;

            splitArray = entity.ItemsNoList.Split('.');

            for (int i = 0; i < splitArray.Count(); i++)
            {
                if (!CheckNum(splitArray[i], ref a))
                {
                    ThrowBizException("InvoiceInput_ItemNoInValid");
                }
                else
                {
                    condition.Add(splitArray[i]);
                }
            }

            invalidList = GetInvalidInvoiceNo(condition, entity);
            result.originalList = condition;
            result.invalidList = invalidList;
            return result;
        }

        /// <summary>
        /// 获取无效的发票号码
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<string> GetInvalidInvoiceNo(List<string> condition, APInvoiceItemInputEntity entity)
        {
            return DA.GetInvalidInvoiceNo(condition, entity);
        }

        /// <summary>
        /// 录入有效的InvItems并保留原始的InvItems
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="checkEntity"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private List<APInvoiceInvoiceItemInfo> CheckInvoice(APInvoiceItemInputEntity entity, InvoiceItemCheckEntity checkEntity, out string errorMsg)
        {
            List<APInvoiceInvoiceItemInfo> result = new List<APInvoiceInvoiceItemInfo>();
            List<string> filterList = new List<string>();
            List<string> condition = new List<string>();
            List<string> intersectItems = new List<string>();

            filterList = checkEntity.originalList;
            errorMsg = string.Empty;

            if (checkEntity.invalidList != null && checkEntity.invalidList.Count > 0)
            {
                errorMsg = errorMsg + GetMessageString("InvoiceInput_CheckInvoice_LinkedInvoice");
                string invalidList = "";
                checkEntity.invalidList.ForEach(x =>
                {
                    invalidList = invalidList + x + ",";
                });
                errorMsg = errorMsg + invalidList.TrimEnd(',');
                //移除无效的发票号
                filterList = ((from c in checkEntity.originalList
                               select c).Except(
                                   from e in checkEntity.invalidList
                                   select e)).ToList();
            }

            if (entity.InvoiceItemList.Count > 0)
            {
                condition = ((from c in filterList
                              select c).Union(
                                    from e in entity.InvoiceItemList
                                    select e.InvoiceNo)).ToList();

                intersectItems = ((from c in filterList
                                   select c).Intersect(
                                        from e in entity.InvoiceItemList
                                        select e.InvoiceNo)).ToList();

                if (intersectItems.Count > 0)
                {
                    errorMsg = errorMsg + GetMessageString("InvoiceInput_CheckInvoice_DuplicatedInvoice");
                    string intersectList = "";
                    intersectItems.ForEach(x =>
                    {
                        intersectList = intersectList + x + ",";
                    });
                    errorMsg = errorMsg + intersectList.TrimEnd(',');
                }
            }
            else
            {
                condition = filterList;
            }

            //InvoiceNo存在则Load前台传过来的数据，否则添加默认值
            try
            {
                condition.ForEach(x =>
                {
                    APInvoiceInvoiceItemInfo findEntity = entity.InvoiceItemList.Find(e =>
                    {
                        return e.InvoiceNo == x;
                    });
                    if (findEntity != null)
                    {
                        result.Add(findEntity);
                    }
                    else
                    {
                        decimal tempInvociceAmt;
                        if (entity.InvoiceAmt.HasValue)
                        {
                            tempInvociceAmt = entity.InvoiceAmt.Value;
                        }
                        else
                        {
                            tempInvociceAmt = 0;
                        }

                        result.Add(new APInvoiceInvoiceItemInfo
                        {
                            InvoiceNo = x,
                            InvoiceDate = entity.InvoiceDate.HasValue ? entity.InvoiceDate.Value
                                            : DateTime.Now.AddDays(-1),
                            InvoiceAmt = tempInvociceAmt,
                            InvoiceNetAmt = Math.Round(tempInvociceAmt / InvoiceConst.VendorTaxRate, 2),
                            InvoiceTaxAmt = tempInvociceAmt - Math.Round(tempInvociceAmt / InvoiceConst.VendorTaxRate, 2)
                        });
                    }
                });
                result.ForEach(item =>
                {
                    APInvoiceInvoiceItemInfo ent = entity.InvoiceItemList.Find(t =>
                    {
                        return t.InvoiceNo == item.InvoiceNo;
                    });
                });
                result = result
                    .OrderByDescending(x => x.InvoiceNo)
                    .ThenBy(x => x.InvoiceDate).ToList();
            }
            catch (Exception x)
            {
                string error = x.Message;
            }
            return result;
        }

        #endregion InvoiceItemCheck

        #region POItemCheck

        /// <summary>
        /// 录入POItem
        /// </summary>
        /// <param name="inputEntity"></param>
        /// <param name="vendorName"></param>
        /// <param name="errorMsg"></param>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public virtual List<APInvoicePOItemInfo> InputPoItem(APInvoiceItemInputEntity inputEntity, ref string vendorName, ref string errorMsg, out int vendorSysNo)
        {
            if (string.IsNullOrEmpty(inputEntity.ItemsNoList))
            {
                ThrowBizException("InvoiceInput_InputPoItem_ItemsNoListIsEmpty");
            }
            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();
            //判断供应商是否存在
            if (inputEntity.VendorSysNo.HasValue && inputEntity.VendorSysNo.Value > 0)
            {
                result = JudgePO4Exist(inputEntity, ref vendorName, ref errorMsg, out vendorSysNo);
            }
            else
            {
                result = JudgePO4None(inputEntity, ref vendorName, ref errorMsg, out vendorSysNo);
            }
            return result;
        }

        /// <summary>
        /// 录入有效的POItems（未选供应商）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="vendorName"></param>
        /// <param name="errorMsg"></param>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        private List<APInvoicePOItemInfo> JudgePO4None(APInvoiceItemInputEntity entity, ref string vendorName, ref string errorMsg, out int vendorSysNo)
        {
            List<POItemCheckEntity> checkEntity = new List<POItemCheckEntity>();
            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();
            int iniValue;

            if (entity.ItemsNoList.Contains("."))
            {
                checkEntity = GetCheckEntity4Split(entity);
                iniValue = checkEntity.First().VendorSysNo;
                foreach (POItemCheckEntity item in checkEntity)
                {
                    if (item.VendorSysNo != iniValue)
                        ThrowBizException("InvoiceInput_JudgePO4None_ItemNotBelongToTheSameVendor");
                }
                entity.VendorSysNo = iniValue;
                result = CheckDealing(entity, checkEntity, ref errorMsg);
            }
            else if (entity.ItemsNoList.Contains("-"))
            {
                checkEntity = GetCheckEntity4Range(entity);
                iniValue = checkEntity.First().VendorSysNo;
                foreach (POItemCheckEntity item in checkEntity)
                {
                    if (item.VendorSysNo != iniValue)
                        ThrowBizException("InvoiceInput_JudgePO4None_ItemNotBelongToTheSameVendor");
                }
                entity.VendorSysNo = iniValue;
                result = CheckDealing(entity, checkEntity, ref errorMsg);
            }
            else
            {
                checkEntity = GetCheckEntiy4Single(entity);
                iniValue = checkEntity.First().VendorSysNo;
                foreach (POItemCheckEntity item in checkEntity)
                {
                    if (item.VendorSysNo != iniValue)
                        ThrowBizException("InvoiceInput_JudgePO4None_ItemNotBelongToTheSameVendor");
                }
                entity.VendorSysNo = iniValue;
                result = CheckDealing(entity, checkEntity, ref errorMsg);
            }
            vendorSysNo = iniValue;
            vendorName = ExternalDomainBroker.GetVendorBasicInfo(iniValue).VendorBasicInfo.VendorNameLocal;
            return result;
        }

        /// <summary>
        /// 录入有效的POItems（已选供应商）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="vendorName"></param>
        /// <param name="errorMsg"></param>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        private List<APInvoicePOItemInfo> JudgePO4Exist(APInvoiceItemInputEntity entity, ref string vendorName, ref string errorMsg, out int vendorSysNo)
        {
            vendorSysNo = 0;
            List<POItemCheckEntity> checkEntity = new List<POItemCheckEntity>();
            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();

            if (entity.ItemsNoList.Contains("."))
            {
                checkEntity = GetCheckEntity4Split(entity);
                result = CheckDealing(entity, checkEntity, ref errorMsg);
            }
            else if (entity.ItemsNoList.Contains("-"))
            {
                checkEntity = GetCheckEntity4Range(entity);
                result = CheckDealing(entity, checkEntity, ref errorMsg);
            }
            else
            {
                checkEntity = GetCheckEntiy4Single(entity);
                result = CheckDealing(entity, checkEntity, ref errorMsg);
            }

            if (entity.VendorSysNo.HasValue && entity.VendorSysNo.Value != 0)
            {
                vendorSysNo = entity.VendorSysNo.Value;
                vendorName = ExternalDomainBroker.GetVendorBasicInfo(entity.VendorSysNo.Value).VendorBasicInfo.VendorNameLocal;
            }
            return result;
        }

        private List<POItemCheckEntity> GetCheckEntity4Split(APInvoiceItemInputEntity entity)
        {
            List<POItemCheckEntity> result = new List<POItemCheckEntity>();
            List<int> condition = new List<int>();
            string[] splitArray;
            int a = 0;

            splitArray = entity.ItemsNoList.Split('.');
            for (int i = 0; i < splitArray.Count(); i++)
            {
                if (!CheckNum(splitArray[i], ref a))
                {
                    ThrowBizException("InvoiceInput_ItemNoInValid");
                }
                else
                {
                    condition.Add(a);
                }
            }

            result = GetPOCheckList(condition, entity);

            if (result.Count == 0)
                ThrowBizException("InvoiceInput_AllItemsNotFound");

            return result;
        }

        private List<POItemCheckEntity> GetCheckEntity4Range(APInvoiceItemInputEntity entity)
        {
            List<POItemCheckEntity> result = new List<POItemCheckEntity>();
            string[] entityArray = new string[2];
            int a = 0;
            int b = 0;

            entityArray = entity.ItemsNoList.Split('-');
            List<int> condition = new List<int>();
            if (CheckNum(entityArray[0], ref a) && CheckNum(entityArray[1], ref b))
            {
                if (Int32.Parse(entityArray[0]) >= Int32.Parse(entityArray[1]))
                {
                    ThrowBizException("InvoiceInput_OutOfDataRange");
                }

                if ((b - a + 1) > 500)
                {
                    ThrowBizException("InvoiceInput_LoadItemTooMany");
                }

                for (int i = a; i <= b; i++)
                {
                    condition.Add(i);
                }
            }
            else
            {
                ThrowBizException("InvoiceInput_ItemNoInValid");
            }

            result = GetPOCheckList(condition, entity);

            if (result.Count == 0)
                ThrowBizException("InvoiceInput_AllItemsNotFound");

            return result;
        }

        private List<POItemCheckEntity> GetCheckEntiy4Single(APInvoiceItemInputEntity entity)
        {
            List<POItemCheckEntity> result = new List<POItemCheckEntity>();
            List<int> condition = new List<int>();
            int a = 0;

            if (!CheckNum(entity.ItemsNoList, ref a))
            {
                ThrowBizException("InvoiceInput_ItemNoInValid");
            }
            else
            {
                condition.Add(a);
            }

            result = GetPOCheckList(condition, entity);

            if (result.Count == 0)
                ThrowBizException("InvoiceInput_AllItemsNotFound");

            return result;
        }

        /// <summary>
        /// 根据输入的PO单据号获取应付款中的PO单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<POItemCheckEntity> GetPOCheckList(List<int> condition, APInvoiceItemInputEntity entity)
        {
            return DA.GetPOCheckList(condition, entity);
        }

        /// <summary>
        /// 录入新POItems并保留原来的Poitems
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="checkEntity"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private List<APInvoicePOItemInfo> CheckDealing(APInvoiceItemInputEntity entity, List<POItemCheckEntity> checkEntity, ref string errorMsg)
        {
            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();

            List<POItemCheckEntity> filterList = new List<POItemCheckEntity>();
            List<POItemCheckEntity> errorList = new List<POItemCheckEntity>();
            List<POItemCheckEntity> errorMiddle = new List<POItemCheckEntity>();
            List<POItemKeyEntity> condition = new List<POItemKeyEntity>();
            List<POItemKeyEntity> intersectItems = new List<POItemKeyEntity>();
            string stockFlag;//？不知道用处

            #region 获得符合条件的录入

            //Vendor相符；
            //PO已确认入库,或者RMA通过PMD审核；
            //PO没有被关联过发票 或者 PO有关联发票但该关联仍在可编辑状态（仅保存未提交 或者 审批被退回）；
            //PO不在当前列表中
            if (entity.OrderType == PayableOrderType.PO
                || entity.OrderType == PayableOrderType.POAdjust)
            {
                //PO单已入库
                stockFlag = "46789";
            }
            //else if (entity.OrderType == PayableOrderType.SubInvoice)
            //{
            //    stockFlag = "123"; //票扣
            //}
            else
            {
                //PMD已审核&PMCC已审核
                stockFlag = "35";
            }

            filterList = checkEntity
                .Where(c => c.VendorSysNo == entity.VendorSysNo
                            && stockFlag.IndexOf(c.StockStatus.ToString()) != -1
                            && c.InvoiceStatus == PayableInvoiceStatus.Absent
                            && (c.POItemStatus == null || c.POItemStatus == APInvoiceItemStatus.Deactive &&
                            (c.MasterStatus == APInvoiceMasterStatus.Refuse
                            || c.MasterStatus == APInvoiceMasterStatus.VPCancel))
                          ).Except(checkEntity.Where(c => c.POItemStatus == APInvoiceItemStatus.Active), new POItemCheckComparer()).ToList()
                          .Except(checkEntity.Where(c => (c.MasterStatus == APInvoiceMasterStatus.Origin ||
                              c.MasterStatus == APInvoiceMasterStatus.NeedAudit ||
                              c.MasterStatus == APInvoiceMasterStatus.AuditPass)), new POItemCheckComparer()).ToList();

            #endregion 获得符合条件的录入

            #region 处理不能录入的订单并反馈信息

            if (filterList.Count < checkEntity.Count)
            {
                errorList = checkEntity.Except(filterList, new POItemCheckComparer()).ToList();

                errorMiddle = (from c in errorList
                               where c.VendorSysNo != entity.VendorSysNo
                               select c).ToList();
                if (errorMiddle.Count > 0)
                {
                    string typeString = entity.OrderType.ToString();
                   
                    errorMsg = errorMsg
                            + GetErrorStr4PO(typeString, errorMiddle, GetMessageString("InvoiceInput_CheckDealing_OrderNoInValid"));

                    errorList = GetErrorList4PO(errorList, errorMiddle);
                }

                if (errorList.Count > 0)
                {
                    errorMiddle = (from c in errorList
                                   where stockFlag.IndexOf(c.StockStatus.ToString()) == -1
                                   select c).ToList();
                    if (errorMiddle.Count > 0)
                    {
                        string typeString = entity.OrderType.ToString();
                       
                        errorMsg = errorMsg
                            + GetErrorStr4PO(typeString, errorMiddle, GetMessageString("InvoiceInput_CheckDealing_OrderStatusInValid"));
                        errorList = GetErrorList4PO(errorList, errorMiddle);
                    }
                }

                if (errorList.Count > 0)
                {
                    errorMiddle = (from c in errorList
                                   where c.InvoiceStatus != PayableInvoiceStatus.Absent
                                   select c).ToList();
                    if (errorMiddle.Count > 0)
                    {
                        string typeString = entity.OrderType.ToString();
                      
                        errorMsg = errorMsg + GetErrorStr4PO(typeString, errorMiddle, GetMessageString("InvoiceInput_CheckDealing_OrderExist"));
                        errorList = GetErrorList4PO(errorList, errorMiddle);
                    }
                }
                if (errorList.Count > 0)
                {
                    string typeString = entity.OrderType.ToString();
                   
                    errorMsg = errorMsg
                            + GetErrorStr4PO(typeString, errorList, GetMessageString("InvoiceInput_CheckDealing_OrderLinkedOrEditable"));
                }
            }

            #endregion 处理不能录入的订单并反馈信息

            if (filterList.Count > 0)
            {
                //获得重复录入后的记录集
                if (entity.POItemList.Count > 0)
                {
                    //再次录入记录，比较是否属于同一供应商
                    if (entity.VendorSysNo != checkEntity.First().VendorSysNo)
                    {
                        ThrowBizException("InvoiceInput_CheckDealing_OrderNotBelongToTheSameVendor");
                    }

                    condition = ((from c in filterList
                                  select new POItemKeyEntity
                                  {
                                      PONo = c.PONo
                                      ,
                                      OrderType = c.OrderType
                                      ,
                                      BatchNumber = c.BatchNumber
                                  }).Union(from e in entity.POItemList
                                           select new POItemKeyEntity
                                           {
                                               PONo = e.PoNo.Value
                                               ,
                                               OrderType = e.OrderType.Value
                                               ,
                                               BatchNumber = e.BatchNumber
                                           }
                                           , new POItemKeyComparer())).ToList();

                    //取相同项，通知前台
                    intersectItems = ((from c in filterList
                                       select new POItemKeyEntity
                                       {
                                           PONo = c.PONo
                                           ,
                                           OrderType = c.OrderType
                                           ,
                                           BatchNumber = c.BatchNumber
                                       }).Intersect(from e in entity.POItemList
                                                    select new POItemKeyEntity
                                                    {
                                                        PONo = e.PoNo.Value
                                                        ,
                                                        OrderType = e.OrderType.Value
                                                        ,
                                                        BatchNumber = e.BatchNumber
                                                    }
                                                    , new POItemKeyComparer())).ToList();

                    if (intersectItems.Count > 0)
                    {
                        errorMsg = errorMsg + GetMessageString("InvoiceInput_CheckDealing_DuplicatedOrderNo");
                        string intersectList = "";
                        intersectItems.ForEach(x =>
                        {
                            string typeString = entity.OrderType.ToString();

                            
                            intersectList = intersectList + typeString + ":" + x.PONo + "_" + x.BatchNumber.ToString() + ",";
                        });
                        errorMsg = errorMsg + intersectList;
                        errorMsg = errorMsg.TrimEnd(',');
                    }
                }
                //首次录入的记录集
                else
                {
                    condition = (from c in filterList
                                 select new POItemKeyEntity
                                 {
                                     PONo = c.PONo,
                                     OrderType = c.OrderType,
                                     BatchNumber = c.BatchNumber
                                 }).ToList();
                }

                //返回前台比较后的结果集
                //为了与编辑界面统一，需要保留编辑入库项
                var conditionKeep = ((from e in condition
                                      select new POItemKeyEntity
                                      {
                                          PONo = e.PONo
                                          ,
                                          OrderType = e.OrderType
                                          ,
                                          BatchNumber = e.BatchNumber
                                      }).Except(from c in entity.POItemList
                                                select new POItemKeyEntity
                                                {
                                                    PONo = c.PoNo.Value
                                                    ,
                                                    OrderType = c.OrderType.Value
                                                    ,
                                                    BatchNumber = c.BatchNumber
                                                }
                                                , new POItemKeyComparer())).ToList();
                if (conditionKeep != null && conditionKeep.Count != 0)
                {
                    result = POItemsInputQueryHand(conditionKeep);
                    result = ((from c in entity.POItemList
                               select c).Union(from e in result
                                               select e)).ToList();
                }
                else
                {
                    result = POItemsInputQueryHand(condition);
                }

                result.ForEach(item =>
                {
                    APInvoicePOItemInfo ent = entity.POItemList.Find(t =>
                    {
                        return t.PoNo == item.PoNo
                            && t.OrderType == item.OrderType
                            && t.BatchNumber == item.BatchNumber;
                    });
                });
                result = result
                    .OrderByDescending(x => x.PoNo)
                    .ThenBy(x => x.OrderType)
                    .ThenByDescending(x => x.PoBaselineDate)
                    .ToList();
            }
            else
            {
                result = entity.POItemList;
            }
            return result;
        }

        /// <summary>
        ///POItems录入处理
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private List<APInvoicePOItemInfo> POItemsInputQueryHand(List<POItemKeyEntity> condition)
        {
            List<APInvoicePOItemInfo> result4PO = new List<APInvoicePOItemInfo>();
            List<APInvoicePOItemInfo> result4RMA = new List<APInvoicePOItemInfo>();
            List<APInvoicePOItemInfo> result = new List<APInvoicePOItemInfo>();

            List<int> POCondition = new List<int>();
            List<int> RMACondtion = new List<int>();
            List<int> VendorSettleCondition = new List<int>();
            List<int> SubInvoiceCondition = new List<int>();

            POCondition = (from c in condition
                           where c.OrderType == PayableOrderType.PO
                           || c.OrderType == PayableOrderType.POAdjust
                           select c.PONo).ToList();

            RMACondtion = (from c in condition
                           where c.OrderType == PayableOrderType.RMAPOR
                           select c.PONo).ToList();

            VendorSettleCondition = (from c in condition
                                     where c.OrderType == PayableOrderType.VendorSettleOrder
                                     select c.PONo).ToList();

            //SubInvoiceCondition = (from c in condition
            //                       where c.OrderType == PayableOrderType.SubInvoice
            //                       select c.PONo).ToList();

            if (POCondition != null && POCondition.Count > 0)
            {
                result = (from c in result
                          select c).Union(
                                 from e in GetPOInputItemsHand(POCondition, PayableOrderType.PO)
                                 select e, new APInvoiceInputPOItemComparer()).ToList();
            }
            if (RMACondtion != null && RMACondtion.Count > 0)
            {
                result = (from c in result
                          select c).Union(
                                 from e in GetPOInputItemsHand(RMACondtion, PayableOrderType.RMAPOR)
                                 select e, new APInvoiceInputPOItemComparer()).ToList();
            }
            if (VendorSettleCondition != null && VendorSettleCondition.Count > 0)
            {
                result = (from c in result
                          select c).Union(
                                 from e in GetPOInputItemsHand(VendorSettleCondition, PayableOrderType.VendorSettleOrder)
                                 select e, new APInvoiceInputPOItemComparer()).ToList();
            }

            //if (SubInvoiceCondition != null && SubInvoiceCondition.Count > 0)
            //{
            //    result = (from c in result
            //              select c).Union(
            //                     from e in GetPOInputItemsHand(SubInvoiceCondition, PayableOrderType.SubInvoice)
            //                     select e, new APInvoiceInputPOItemComparer()).ToList();
            //}

            result = result.Where(c =>
            {
                return condition.Contains(new POItemKeyEntity
                {
                    PONo = c.PoNo.Value,
                    OrderType = c.OrderType.Value,
                    BatchNumber = c.BatchNumber
                }, new POItemKeyComparer());
            }).ToList();
            if (result != null && result.Count > 0)
            {
                result.ForEach(p =>
                {
                    if (p.OrderType == PayableOrderType.PO)
                    {
                        List<EIMSInfo> EIMSlist = ExternalDomainBroker.LoadPurchaseOrderEIMSInfo(p.PoNo.Value);
                        if (EIMSlist.Count > 0)
                            p.EIMSNoList = EIMSlist.Select(x => x.EIMSSysNo.ToString()).Join(",");
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// 根据输入的PONO获取对应的POItems信息
        /// </summary>
        /// <param name="POCondition"></param>
        /// <param name="payableOrderType"></param>
        /// <returns></returns>
        private List<APInvoicePOItemInfo> GetPOInputItemsHand(List<int> POCondition, PayableOrderType payableOrderType)
        {
            return DA.GetPOInputItemsHand(POCondition, payableOrderType);
        }

        private List<POItemCheckEntity> GetErrorList4PO(List<POItemCheckEntity> errorList, List<POItemCheckEntity> errorMiddle)
        {
            errorList = errorList.Except(errorMiddle, new POItemCheckComparer()).ToList();
            return errorList;
        }

        private string GetErrorStr4PO(string orderType, List<POItemCheckEntity> errorMiddle, string title)
        {
            errorMiddle.ForEach(x =>
            {
                title = title + orderType.ToString() + ":" + x.PONo + (x.BatchNumber.HasValue ? "_" + x.BatchNumber.ToString() : "") + ",";
            });
            title = title.TrimEnd(',');

            return title;
        }

        #endregion POItemCheck

        private bool CheckNum(string str, ref int num)
        {
            int p = 0;
            int.TryParse(str, out p);
            num = p;
            return p > 0;
        }

        #endregion InputStart

        #region Private Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.InvoiceInput, msgKeyName), args);
        }

        #endregion Private Helper Methods
    }
}
