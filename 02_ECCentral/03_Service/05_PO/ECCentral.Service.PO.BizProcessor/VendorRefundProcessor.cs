using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.Service.PO.IDataAccess;
using System.Data;
using System.Transactions;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.EventMessage.PO;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 供应商退款 - BizProcessor
    /// </summary>
    [VersionExport(typeof(VendorRefundProcessor))]
    public class VendorRefundProcessor
    {

        #region [Fields]
        private IVendorRefundDA m_VendorRefundDA;
        private IIMBizInteract m_IMBizInteract;

        public IIMBizInteract IMBizInteract
        {
            get
            {
                if (null == m_IMBizInteract)
                {
                    m_IMBizInteract = ObjectFactory<IIMBizInteract>.Instance;
                }
                return m_IMBizInteract;
            }
        }

        public IVendorRefundDA VendorRefundDA
        {
            get
            {
                if (null == m_VendorRefundDA)
                {
                    m_VendorRefundDA = ObjectFactory<IVendorRefundDA>.Instance;
                }
                return m_VendorRefundDA;
            }
        }
        #endregion

        public virtual VendorRefundInfo LoadVendorRefundInfo(int sysNo)
        {
            VendorRefundInfo refundInfo = new VendorRefundInfo() { NotPMAndPMD = false };
            //加载供应商退款主信息:
            refundInfo = VendorRefundDA.LoadVendorRefundInfo(sysNo);
            //加载供应商退款Items:
            refundInfo.ItemList = new List<VendorRefundItemInfo>();
            refundInfo.ItemList = VendorRefundDA.LoadVendorRefundItems(sysNo);

            //TODO:权限判断:PM,PMD,PMCC
            bool isPM = IMBizInteract.GetPMListByUserSysNo(ServiceContext.Current.UserSysNo).SysNo.HasValue ? true : false;
            if (isPM)
            {
                refundInfo.UserRole = "PM";
            }
            if (0 < refundInfo.ItemList.Count && isPM)
            {
                int userSysNo = ServiceContext.Current.UserSysNo;
                //获取PM List:
                List<int> pmSysNo = VendorRefundDA.GetPMUserSysNoByRMAVendorRefundSysNo(refundInfo.SysNo.Value);
                //获得备份的PM:

                List<ProductManagerInfo> bankupPMSysNo = ExternalDomainBroker.GetPMList(userSysNo).ProductManagerInfoList;
                bankupPMSysNo.Add(new ProductManagerInfo() { UserInfo = new BizEntity.Common.UserInfo() { SysNo = userSysNo } });
                foreach (var sysNoItem in pmSysNo)
                {
                    bool temp = false;
                    foreach (var pm in bankupPMSysNo)
                    {
                        if (sysNoItem == pm.UserInfo.SysNo.Value)
                        {
                            temp = true;
                        }
                    }
                    if (!temp)
                    {
                        refundInfo.NotPMAndPMD = true;
                        break;
                    }
                }


            }
            return refundInfo;
        }

        /// <summary>
        /// 供应商退款 - PM拒绝
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo PMReject(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }

            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);
            if (localEntity == null)
            {
                //供应商退款单在数据中不存在
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }
            //if (localEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    //供应商退款单中，创建人，PM审核人，PMD审核人，PMCC审核人，不能相同
            //    throw new BizException(GetMessageString("VendorRefund_OperatorCannotSame_Invalid"));
            //}

            localEntity.PMUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMAuditTime = System.DateTime.Now;
            localEntity.Status = VendorRefundStatus.Origin;
            localEntity.PMMemo = refundInfo.PMMemo;


            // 检查PM:您不是当前产品的PM，也不是当前产品PM的备份PM，无法操作！
            CheckRefundPM(localEntity);

            using (TransactionScope scope = new TransactionScope())
            {
                //获取当前的VendorRefundInfo:
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoRejectMessage>(new VendorRefundInfoRejectMessage()
                {
                    RejectUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = refundInfo.SysNo.Value
                });

                scope.Complete();
            }

            return localEntity;
        }

        /// <summary>
        /// 供应商退款 - PMD审核拒绝
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo PMDReject(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }
            //获取当前的VendorRefundInfo:
            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);
            if (localEntity == null)
            {
                //供应商退款单在数据中不存在
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }
            //if (localEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    //供应商退款单中，创建人，PM审核人，PMD审核人，PMCC审核人，不能相同
            //    throw new BizException(GetMessageString("VendorRefund_OperatorCannotSame_Invalid"));
            //}

            ///更新操作:
            localEntity.PMDUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMDAuditTime = System.DateTime.Now;
            localEntity.PMDMemo = refundInfo.PMDMemo;
            localEntity.Status = VendorRefundStatus.Origin;

            using (TransactionScope scope = new TransactionScope())
            {
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoRejectMessage>(new VendorRefundInfoRejectMessage()
                {
                    RejectUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = refundInfo.SysNo.Value
                });

                scope.Complete();
            }

            return localEntity;
        }

        /// <summary>
        /// 供应商退款 - PM审核通过
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo PMApproveVendorRefund(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }

            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);
            if (localEntity == null)
            {
                //供应商退款单在数据中不存在!
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }

            //if (localEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    //供应商退款单中，创建人，PM审核人，PMD审核人，PMCC审核人，不能相同
            //    throw new BizException(GetMessageString("VendorRefund_OperatorCannotSame_Invalid"));
            //}

            localEntity.PMUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMAuditTime = System.DateTime.Now;
            localEntity.PMMemo = refundInfo.PMMemo;
            localEntity.Status = VendorRefundStatus.PMVerify;

            //您不是当前产品的PM，也不是当前产品PM的备份PM，无法操作！
            CheckRefundPM(localEntity);

            using (TransactionScope scope = new TransactionScope())
            {
                //更新操作:
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoAuditMessage>(new VendorRefundInfoAuditMessage()
                {
                    AuditUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = refundInfo.SysNo.Value
                });

                scope.Complete();
            }
            return localEntity;
        }

        /// <summary>
        /// 供应商退款 - PMD审核通过
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo PMDApproveVendorRefund(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }
            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);
            if (localEntity == null)
            {
                //供应商退款单在数据中不存在
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }
            //if (localEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    //供应商退款单中，创建人，PM审核人，PMD审核人，PMCC审核人，不能相同
            //    throw new BizException(GetMessageString("VendorRefund_OperatorCannotSame_Invalid"));
            //}


            localEntity.PMDUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMDAuditTime = System.DateTime.Now;
            localEntity.PMDMemo = refundInfo.PMDMemo;
            localEntity.Status = VendorRefundStatus.PMDVerify;

            //加载当前供应商退款单Items:
            List<VendorRefundItemInfo> localItem = VendorRefundDA.LoadVendorRefundItems(localEntity.SysNo.Value);

            #region 调用RMA接口:判断单件是否可以关闭
            List<int> RegisterSysNo = new List<int>();
            localItem.ForEach(a => RegisterSysNo.Add(a.RegisterSysNo.Value));
            //调用RMA接口,判断单件是否可以关闭
            List<RMARegisterInfo> getList = ExternalDomainBroker.GetRMARegisterList(RegisterSysNo);
            foreach (var item in getList)
            {
                if (item.BasicInfo.Status != RMARequestStatus.Handling)  //RMARequestStatus.Handling 1 表示 处理中
                {
                    throw new BizException(string.Format(GetMessageString("VendorRefund_CloseInvalid"), item.SysNo.Value));
                }
            }
            #endregion

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            List<VendorRefundItemInfo> list = new List<VendorRefundItemInfo>();
            List<KeyValuePair<int, int>> registerList = new List<KeyValuePair<int, int>>();
            List<VendorRefundInfo> deductOnVendor = new List<VendorRefundInfo>();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //更新状态 ：
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);
                //关闭单件并扣减RMA库存中OnVendorQty数量：
                list = VendorRefundDA.LoadVendorRefundItems(localEntity.SysNo.Value);
                foreach (var item in list)
                {
                    int registerNo = item.RegisterSysNo.Value;
                    registerList.Add(new KeyValuePair<int, int>(registerNo, localEntity.PMDUserSysNo.Value));

                    ////调用RMA接口，根据单件号获取接收仓库:
                    string[] str = ExternalDomainBroker.GetReceiveWarehouseByRegisterSysNo(registerNo);
                    if (!string.IsNullOrEmpty(str[0]) && !string.IsNullOrEmpty(str[1]))
                    {
                        deductOnVendor.Add(new VendorRefundInfo
                        {
                            WarehouseSysNo = Convert.ToInt32(str[0]),
                            ProductSysNo = Convert.ToInt32(str[1]),
                            RegisterSysNo = registerNo
                        });
                    }
                }

                ////调用RMA接口，关闭送修单:
                List<int> OutBound = ExternalDomainBroker.GetOutBoundSysNoListByRegisterSysNo(registerList.ToListString("Key"));
                if (OutBound.Count > 0)
                    ExternalDomainBroker.UpdateOutBound(OutBound.ToListString());
                ////调用RMA接口:关闭单件
                List<int> registerSysNos = new List<int>();
                registerList.ForEach(x =>
                {
                    registerSysNos.Add(x.Key);
                });

                ExternalDomainBroker.BatchCloseRegisterForVendorRefund(registerSysNos);

                //扣减RMA库存中OnVendorQty数量

                ExternalDomainBroker.BatchDeductOnVendorQty(deductOnVendor);

                PayableInfo createdPayableInfo = null;

                ////调用Invoice接口,生成财务POR记录
                PayableInfo payableInfo = new PayableInfo()
                {
                    OrderSysNo = localEntity.SysNo.Value,
                    CurrencySysNo = 1,
                    OrderAmt = -1 * localEntity.RefundCashAmt.Value,
                    PayStatus = 0,
                    InvoiceStatus = 0,
                    AlreadyPayAmt=0m,
                    OrderType = PayableOrderType.RMAPOR, //9
                    InvoiceUpdateTime = DateTime.Parse("1900-1-1"),
                    InvoiceFactStatus = PayableInvoiceFactStatus.Others,// 3
                    CompanyCode="8601"
                };
                createdPayableInfo = ExternalDomainBroker.CreatePayable(payableInfo);
                if (null != createdPayableInfo && 0 < createdPayableInfo.SysNo)
                {
                    int financePaySysNo = createdPayableInfo.SysNo.Value;

                    List<PayItemInfo> payItemList = new List<PayItemInfo>();
                    list.ForEach(x =>
                    {
                        PayItemInfo item = new PayItemInfo()
                        {
                            PaySysNo = financePaySysNo,
                            PayStyle = PayItemStyle.Normal,
                            Status = PayItemStatus.Origin,
                            PayAmt = x.RefundCash.HasValue ? (-1 * x.RefundCash.Value) : -999999,
                            OrderType = PayableOrderType.RMAPOR,
                            OrderSysNo = x.RefundSysNo.Value,
                            CompanyCode="8601"
                        };
                        payItemList.Add(item);
                    });

                    ExternalDomainBroker.BatchCreatePayItem(payItemList);
                }

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoAuditMessage>(new VendorRefundInfoAuditMessage()
                {
                    AuditUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = refundInfo.SysNo.Value
                });

                scope.Complete();
            }
            return localEntity;
        }

        /// <summary>
        /// PMCC审核通过
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo PMCCApproveVendorRefund(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }
            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);
            if (localEntity == null)
            {
                //供应商退款单在数据中不存在
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }
            if (localEntity.Status.Value == VendorRefundStatus.PMCCVerify)
            {
                throw new BizException("该供应商退款单为PMCC审核状态,不允许进行当前操作!");
            }

            //if (localEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    //供应商退款单中，创建人，PM审核人，PMD审核人，PMCC审核人，不能相同
            //    throw new BizException(GetMessageString("VendorRefund_OperatorCannotSame_Invalid"));
            //}

            localEntity.PMCCUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMCCAuditTime = System.DateTime.Now;
            localEntity.PMCCMemo = refundInfo.PMCCMemo;
            localEntity.Status = VendorRefundStatus.PMCCVerify;

            //加载当前商家退款单Items:
            List<VendorRefundItemInfo> localItem = VendorRefundDA.LoadVendorRefundItems(localEntity.SysNo.Value);

            #region 调用RMA接口:判断单件是否可以关闭
            List<int> RegisterSysNo = new List<int>();
            localItem.ForEach(a => RegisterSysNo.Add(a.RegisterSysNo.Value));
            //调用RMA接口,判断单件是否可以关闭
            List<RMARegisterInfo> getList = ExternalDomainBroker.GetRMARegisterList(RegisterSysNo);
            foreach (var item in getList)
            {
                if (item.BasicInfo.Status != RMARequestStatus.Handling)  //RMARequestStatus.Handling 1 表示 处理中
                {
                    throw new BizException(string.Format(GetMessageString("VendorRefund_CloseInvalid"), item.SysNo.Value));
                }
            }
            #endregion

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            List<VendorRefundItemInfo> list = new List<VendorRefundItemInfo>();
            List<KeyValuePair<int, int>> registerList = new List<KeyValuePair<int, int>>();
            List<VendorRefundInfo> deductOnVendor = new List<VendorRefundInfo>();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //更新状态 ：
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);
                //关闭单件并扣减RMA库存中OnVendorQty数量：
                list = VendorRefundDA.LoadVendorRefundItems(localEntity.SysNo.Value);
                foreach (var item in list)
                {
                    int registerNo = item.RegisterSysNo.Value;
                    registerList.Add(new KeyValuePair<int, int>(registerNo, localEntity.PMDUserSysNo.Value));

                    ////调用RMA接口，根据单件号获取接收仓库:
                    string[] str = ExternalDomainBroker.GetReceiveWarehouseByRegisterSysNo(registerNo);
                    if (!string.IsNullOrEmpty(str[0]) && !string.IsNullOrEmpty(str[1]))
                    {
                        deductOnVendor.Add(new VendorRefundInfo
                        {
                            WarehouseSysNo = Convert.ToInt32(str[0]),
                            ProductSysNo = Convert.ToInt32(str[1]),
                            RegisterSysNo = registerNo
                        });
                    }
                }

                ////调用RMA接口，关闭送修单:
                List<int> OutBound = ExternalDomainBroker.GetOutBoundSysNoListByRegisterSysNo(registerList.ToListString("Key"));
                ExternalDomainBroker.UpdateOutBound(OutBound.ToListString());

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoAuditMessage>(new VendorRefundInfoAuditMessage()
                {
                    AuditUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = refundInfo.SysNo.Value
                });

                scope.Complete();
            }
            ////调用RMA接口:关闭单件
            List<int> registerSysNos = new List<int>();
            registerList.ForEach(x =>
            {
                registerSysNos.Add(x.Key);
            });

            ExternalDomainBroker.BatchCloseRegisterForVendorRefund(registerSysNos);

            //扣减RMA库存中OnVendorQty数量

            ExternalDomainBroker.BatchDeductOnVendorQty(deductOnVendor);

            ////调用Invoice接口,生成财务POR记录
            PayableInfo payableInfo = new PayableInfo()
            {
                OrderSysNo = localEntity.SysNo.Value,
                CurrencySysNo = 1,
                OrderAmt = -1 * localEntity.RefundCashAmt.Value,
                PayStatus = 0,
                InvoiceStatus = 0,
                OrderType = PayableOrderType.RMAPOR, //9
                InvoiceUpdateTime = DateTime.Parse("1900-1-1"),
                InvoiceFactStatus = PayableInvoiceFactStatus.Others// 3
            };
            PayableInfo createdPayableInfo = ExternalDomainBroker.CreatePayable(payableInfo);
            if (null != createdPayableInfo && 0 < createdPayableInfo.SysNo)
            {
                int financePaySysNo = createdPayableInfo.SysNo.Value;

                List<PayItemInfo> payItemList = new List<PayItemInfo>();
                list.ForEach(x =>
                {
                    PayItemInfo item = new PayItemInfo()
                    {
                        PaySysNo = financePaySysNo,
                        PayStyle = PayItemStyle.Normal,
                        Status = PayItemStatus.Origin,
                        PayAmt = x.RefundCash.HasValue ? (-1 * x.RefundCash.Value) : -999999,
                        OrderType = PayableOrderType.RMAPOR,
                        OrderSysNo = x.RefundSysNo.Value
                    };
                    payItemList.Add(item);
                });

                ExternalDomainBroker.BatchCreatePayItem(payItemList);
            }

            return localEntity;
        }

        /// <summary>
        /// PMCC审核拒绝
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo PMCCReject(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }
            //获取当前的VendorRefundInfo:
            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);
            if (localEntity == null)
            {
                //供应商退款单在数据中不存在
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }
            if (localEntity.Status.Value == VendorRefundStatus.PMCCVerify)
            {
                throw new BizException("该供应商退款单为PMCC审核状态,不允许进行当前操作!");
            }
            //if (localEntity.CreateUserSysNo == ServiceContext.Current.UserSysNo)
            //{
            //    //供应商退款单中，创建人，PM审核人，PMD审核人，PMCC审核人，不能相同
            //    throw new BizException(GetMessageString("VendorRefund_OperatorCannotSame_Invalid"));
            //}
            ///更新操作:
            localEntity.PMCCUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMDAuditTime = System.DateTime.Now;
            localEntity.PMCCMemo = refundInfo.PMCCMemo;
            localEntity.Status = VendorRefundStatus.Origin;

            using (TransactionScope scope = new TransactionScope())
            {
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoRejectMessage>(new VendorRefundInfoRejectMessage()
                {
                    RejectUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = refundInfo.SysNo.Value
                });

                scope.Complete();
            }

            return localEntity;
        }

        /// <summary>
        /// 更新供应商退款申请
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public virtual VendorRefundInfo UpdateVendorRefund(VendorRefundInfo refundInfo)
        {
            //编号不能为空
            if (!refundInfo.SysNo.HasValue || refundInfo.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }

            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(refundInfo.SysNo.Value);

            if (localEntity == null)
            {
                //"供应商退款单在数据中不存在!"
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            localEntity.PMMemo = refundInfo.PMMemo;
            localEntity.PMDMemo = refundInfo.PMDMemo;
            localEntity.PMCCMemo = refundInfo.PMCCMemo;
            //更新操作:
            localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);
            return localEntity;
        }

        /// <summary>
        /// 检查PM信息
        /// </summary>
        /// <param name="refundInfo"></param>
        public void CheckRefundPM(VendorRefundInfo refundInfo)
        {
            int userSysNo = refundInfo.PMUserSysNo.Value;
            //获取PM List:
            List<int> pmSysNo = VendorRefundDA.GetPMUserSysNoByRMAVendorRefundSysNo(refundInfo.SysNo.Value);
            //获得备份的PM:

            List<ProductManagerInfo> getPMList = ExternalDomainBroker.GetPMList(userSysNo).ProductManagerInfoList;


            List<int> userSysNoList = new List<int>();
            foreach (var dr in getPMList)
            {
                userSysNoList.Add(dr.UserInfo.SysNo.Value);
            }

            userSysNoList.Add(userSysNo);

            foreach (var sysNo in pmSysNo)
            {
                bool temp = false;
                foreach (var pm in userSysNoList)
                {
                    if (sysNo == pm)
                    {
                        temp = true;
                    }
                }
                if (!temp)
                {
                    //您不是当前产品的PM，也不是当前产品PM的备份PM，无法操作！
                    throw new BizException(GetMessageString("VendorRefund_CannotOperate"));
                }
            }
        }

        /// <summary>
        /// 提交PMCC审核
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VendorRefundInfo SubmitToPMCC(VendorRefundInfo entity)
        {
            //编号不能为空
            if (!entity.SysNo.HasValue || entity.SysNo.Value <= 0)
            {
                //供应商退款编号无效
                throw new BizException(GetMessageString("VendorRefund_SysNoEmpty"));
            }

            VendorRefundInfo localEntity = VendorRefundDA.LoadVendorRefundInfo(entity.SysNo.Value);
            if (localEntity == null)
            {
                throw new BizException(GetMessageString("VendorRefund_RefundNotExist"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Abandon)
            {
                //该供应商退款单为作废状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Abandon_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.Origin)
            {
                //该供应商退款单为初始状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_Origin_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMDVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException(GetMessageString("VendorRefund_PMDVerify_Invalid"));
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMCCToVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException("该供应商退款单为待PMCC审核状态,不允许进行当前操作！");
            }

            if (localEntity.Status.Value == VendorRefundStatus.PMCCVerify)
            {
                //该供应商退款单为PMD审核状态,不允许进行当前操作!
                throw new BizException("该供应商退款单为PMCC已审核状态,不允许进行当前操作！");
            }
            localEntity.PMDUserSysNo = ServiceContext.Current.UserSysNo;
            localEntity.PMDAuditTime = System.DateTime.Now;
            localEntity.PMDMemo = entity.PMDMemo;
            localEntity.Status = VendorRefundStatus.PMCCToVerify;

            using (TransactionScope scope = new TransactionScope())
            {
                localEntity = VendorRefundDA.UpdateVendorRefundInfo(localEntity);

                //发送ESB消息
                EventPublisher.Publish<VendorRefundInfoSubmitMessage>(new VendorRefundInfoSubmitMessage()
                {
                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                    SysNo = entity.SysNo.Value
                });

                scope.Complete();
            }

            return localEntity;
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.VendorRefund", key);
        }

    }
}
