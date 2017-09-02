using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.BizProcessor;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using System.Text;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.BizEntity.IM;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.SO.AppService
{
    /// <summary>
    /// 订单服务
    /// </summary>
    [VersionExport(typeof(SOAppService))]
    public class SOAppService
    {
        SOProcessor soProcessor;
        public SOAppService()
        {
            soProcessor = ObjectFactory<SOProcessor>.Instance;
        }

        #region 取得订单信息相关方法

        /// <summary>
        /// 生成一个新的订单编号
        /// </summary>
        /// <returns></returns>
        public virtual int NewSOSysNo()
        {
            return soProcessor.NewSOSysNo();
        }

        /// <summary>
        /// 根据订单系统编号取得订单信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>订单信息</returns> 
        public virtual SOInfo GetSOBySOSysNo(int soSysNo)
        {
            return soProcessor.GetSOBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 取得订单基本信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public virtual SOBaseInfo GetSOBaseInfoBySOSysNo(int soSysNo)
        {
            return soProcessor.GetSOBaseInfoBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单的基本信息
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        public List<SOBaseInfo> GetSOBaseInfoBySOSysNoList(List<int> soSysNos)
        {
            return soProcessor.GetSOBaseInfoBySOSysNoList(soSysNos);
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        public List<SOInfo> GetSOBySOSysNoList(List<int> soSysNos)
        {
            return soProcessor.GetSOBySOSysNoList(soSysNos);
        }

        /// <summary>
        /// 根据客户编号获取客户对应的礼品卡信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        public List<GiftCardInfo> QueryGiftCardListInfo(int customerSysNo)
        {
            return soProcessor.QueryGiftCardListInfo(customerSysNo);
        }

        /// <summary>
        /// 根据客户编号获取订单对应的增值税发票
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        public virtual List<SOVATInvoiceInfo> QuerySOVATInvoiceInfo(int customerSysNo)
        {
            return soProcessor.QuerySOVATInvoiceInfo(customerSysNo);
        }

        /// <summary>
        /// 根据支付方式判断是否为货到付款 
        /// </summary>
        /// <param name="payTypeSysNo">支付方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public virtual bool IsPayWhenReceived(int payTypeSysNo)
        {
            return soProcessor.IsPayWhenReceived(payTypeSysNo);
        }

        /// <summary>
        /// 根据 礼品卡编号 和密码 获取 对应的礼品卡信息
        /// </summary>
        /// <param name="code">礼品卡 卡号</param>
        /// <param name="password">礼品卡 密码</param>
        public GiftCardInfo QueryGiftCardByCodeAndPassword(string code, string password)
        {
            return soProcessor.QueryGiftCardByCodeAndPassword(code, password);
        }

        /// <summary>
        /// 根据商品编号获取商品时间范围内的调价信息
        /// </summary>
        /// <param name="productSysNoList">商品编号List</param>
        /// <param name="startTime">下单时间</param>
        /// <param name="endTime">收货时间</param>
        /// <returns></returns>
        public List<PriceChangeLogInfo> GetPriceChangeLogsInfoByProductSysNoList(List<int> productSysNoList, DateTime startTime, DateTime endTime)
        {
            List<PriceChangeLogInfo> logsInfo = new List<PriceChangeLogInfo>();
            foreach (var item in productSysNoList)
            {
                PriceChangeLogInfo logInfo = ObjectFactory<IIMBizInteract>.Instance.GetPriceChangeLogInfoByProductSysNo(item, startTime, endTime);
                if (logInfo != null)
                {
                    logsInfo.Add(logInfo);
                }
            }
            return logsInfo;
        }

        /// <summary>
        /// 根据客户系统编号获取客户类型信息
        /// </summary>
        /// <param name="CustomerSysNo">客户系统编号</param>
        /// <returns>客户类型信息</returns>
        public KnownFraudCustomer GetKnownFraudCustomerInfoByCustomerSysNo(int CustomerSysNo)
        {
            return ObjectFactory<SOKFCProcessor>.Instance.GetKnownFraudCustomerInfoByCustomerSysNo(CustomerSysNo);   
        }        
        #endregion

        #region 订单操作：创建 更新 审核，作废 拆分...

        /// <summary>
        /// 计算(价格、费用)
        /// </summary>
        /// <param name="info">订单信息</param>
        public virtual SOInfo Calculate(SOInfo entity)
        {
            return soProcessor.Calculate(entity);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="info">订单信息</param>
        public virtual SOInfo CreateSO(SOInfo entity)
        {
            //记业务Log
            entity.SysNo = NewSOSysNo();
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog("Create SO", BizLogType.Sale_SO_Create, entity.BaseInfo.SysNo.Value, entity.CompanyCode);
            soProcessor.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Create, SOInfo = entity });
            return GetSOBySOSysNo(entity.SysNo.Value);
        }


        public virtual SOInfo CloneSO(SOInfo entity)
        {
            return soProcessor.CloneSO(entity);
        }

        /// <summary>
        /// 创建赠品订单
        /// </summary>
        /// <param name="info">订单信息</param>
        /// <param name="masterSOSysNo">主订单编号</param>
        public virtual SOInfo CreateGiftSO(SOInfo entity, int masterSOSysNo)
        {
            //记业务Log 
            entity.SysNo = NewSOSysNo();
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog("Create SO", BizLogType.Sale_SO_Create, entity.BaseInfo.SysNo.Value, entity.CompanyCode);
            soProcessor.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Create, SOInfo = entity, Parameter = new object[] { masterSOSysNo } });
            return entity;
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="info">订单信息</param>
        public virtual SOInfo UpdateSO(SOInfo entity)
        {
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog("Update SO", BizLogType.Sale_SO_Create, entity.BaseInfo.SysNo.Value, entity.CompanyCode);
            soProcessor.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Update, SOInfo = entity });
            return GetSOBySOSysNo(entity.SysNo.Value);
        }

        /// <summary>
        ///  订单出库后 普票改增票
        /// </summary>
        /// <param name="entity"></param> 
        public virtual void SetSOVATInvoiveWhenSOOutStock(SOInfo entity)
        {
            soProcessor.SetSOVATInvoiveWhenSOOutStock(entity);
        }

        /// <summary>
        /// 根据主订单编号，取得子订单列表
        /// </summary>
        /// <param name="masterSOSysNo"></param>
        /// <returns></returns>
        public virtual List<SOInfo> GetSubSOByMasterSOSysNo(int masterSOSysNo)
        {
            return soProcessor.GetSubSOByMasterSOSysNo(masterSOSysNo);
        }

        /// <summary>
        /// 拆分订单
        /// </summary>
        /// <param name="soSysNo">要拆分的订单编号</param>
        /// <returns>子订单列表</returns>
        public virtual List<SOInfo> SplitSO(int soSysNo)
        {
            SOInfo soInfo = GetSOBySOSysNo(soSysNo);
            soProcessor.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Split, SOInfo = soInfo });
            List<SOInfo> subSOInfoList = GetSubSOByMasterSOSysNo(soSysNo);
            return subSOInfoList;
        }

        /// <summary>
        /// 取消订单拆分
        /// </summary>
        /// <param name="soSysNo">主订单编号</param>
        /// <returns></returns>
        public virtual SOInfo CancelSplitSO(int soSysNo)
        {
            SOInfo soInfo = GetSOBySOSysNo(soSysNo);
            if (soInfo.BaseInfo.SplitType == SOSplitType.SubSO) //订单为拆分后的子订单
            {
                soInfo = soProcessor.GetMasterSOBySubSOSysNo(soSysNo);
            }
            else if (soInfo.BaseInfo.Status != SOStatus.Split) //订单不是拆分后的主订单
            {
                BizExceptionHelper.Throw("SO_Split_NotSplited");
            }
            soProcessor.ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.CancelSplit, SOInfo = soInfo });
            return soInfo;

        }

        public virtual bool IsAllSubSONotOutStockList(int soSysNo)
        {
            return soProcessor.IsAllSubSONotOutStockList(soSysNo);
        }

        /// <summary>
        /// 批量操作订单
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        /// <param name="command">操作命令</param>
        /// <param name="parameter">操作参数，可为null</param>
        protected virtual void BatchOperation(List<int> soSysNoList, SOAction.SOCommand command, object[] parameter, out List<SOInfo> soInfoList)
        {
            soInfoList = new List<SOInfo>();
            if (soSysNoList == null || soSysNoList.Count < 1)
            {
                BizExceptionHelper.Throw("SO_BatchOperation_IsNull");
            }
            else if (soSysNoList.Count == 1 && soSysNoList[0] > 0) //1条订单数据就直接操作
            {
                SOInfo soInfo = soProcessor.GetSOBySOSysNo(soSysNoList[0]);
                soInfoList.Add(soInfo);
                if (soInfo != null)
                {
                    soProcessor.ProcessSO(new SOAction.SOCommandInfo
                    {
                        Command = command,
                        SOInfo = soInfo,
                        Parameter = parameter
                    });
                }
            }
            else if (soSysNoList.Count <= 50) //50条数据以下，就先取出所有订单数据再操作
            {
                List<SOInfo> soList = GetSOBySOSysNoList(soSysNoList);

                if (soList != null && soList.Count > 0)
                {
                    soInfoList.AddRange(soList);
                    List<BatchActionItem<SOInfo>> actionItemList = new List<BatchActionItem<SOInfo>>();
                    foreach (SOInfo so in soList)
                    {
                        actionItemList.Add(new BatchActionItem<SOInfo> { ID = so.SysNo.Value.ToString(), Data = so });
                    }

                    BatchActionResult<SOInfo> result = BatchActionManager.DoBatchAction<SOInfo>(actionItemList, (so) =>
                    {
                        SOAction.SOCommandInfo commandInfo = new SOAction.SOCommandInfo();
                        commandInfo.Command = command;
                        commandInfo.SOInfo = so;
                        commandInfo.Parameter = parameter;
                        ObjectFactory<SOProcessor>.NewInstance().ProcessSO(commandInfo);
                    });

                    result.ThrowErrorException();
                }
            }
            else//50条数据以上，就在操作时实时取订单数据
            {
                List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                foreach (int no in soSysNoList)
                {
                    if (no > 0)
                    {
                        actionItemList.Add(new BatchActionItem<int> { ID = no.ToString(), Data = no });
                    }
                }
                if (actionItemList.Count > 0)
                {
                    List<SOInfo> soList = new List<SOInfo>();
                    BatchActionResult<int> result = BatchActionManager.DoBatchAction<int>(actionItemList, (soSysNo) =>
                    {
                        SOInfo soInfo = soProcessor.GetSOBySOSysNo(soSysNo);
                        if (soInfo != null)
                        {
                            soList.Add(soInfo);
                            soProcessor.ProcessSO(new SOAction.SOCommandInfo
                            {
                                Command = command,
                                SOInfo = soInfo,
                                Parameter = parameter
                            });
                        }
                    });
                    soInfoList = soList;
                    result.ThrowErrorException();
                }
            }
        }
        /// <summary>
        /// 批量审核订单
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        /// <param name="isForce">是否强制审核</param>
        /// <param name="isManagerAudit">是否是主管审核(审核待主管审核状态的订单时为true)</param>
        /// <param name="isAuditNetPay">是否同时审核网上支付</param>
        public virtual void AuditSO(List<int> soSysNoList, bool isForce, bool isManagerAudit, bool isAuditNetPay, out List<SOInfo> soInfoList)
        {
            BatchOperation(soSysNoList, SOAction.SOCommand.Audit, new object[] { isForce, isManagerAudit, isAuditNetPay }, out soInfoList);
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="info"></param>
        public virtual SOInfo CancelAuditSO(int soSysNo)
        {
            SOInfo info = GetSOBySOSysNo(soSysNo);
            soProcessor.ProcessSO(new SOAction.SOCommandInfo
            {
                Command = SOAction.SOCommand.CancelAudit,
                SOInfo = info
            });
            return info;
        }

        /// <summary>
        /// 批量作废订单
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <param name="immediatelyReturnInventory"></param>
        public virtual void AbandonSO(List<int> soSysNoList, bool immediatelyReturnInventory, out List<SOInfo> soInfoList)
        {
            BatchOperation(soSysNoList, SOAction.SOCommand.Abandon, new object[] { immediatelyReturnInventory, false, null, false }, out soInfoList);
        }
        /// <summary>
        /// 作废订单
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="immediatelyReturnInventory">是否立即返还库存</param>
        /// <param name="isCreateAO">是否先生成负收款单，现作废订单</param>
        /// <param name="refundInfo">负收款单信息</param>
        /// <param name="soInfo"></param>
        public virtual void AbandonSO(int soSysNo, bool immediatelyReturnInventory, bool isCreateAO, ECCentral.BizEntity.Invoice.SOIncomeRefundInfo refundInfo, out SOInfo soInfo)
        {
            soInfo = soProcessor.GetSOBySOSysNo(soSysNo);
            if (soInfo != null)
            {
                soProcessor.ProcessSO(new SOAction.SOCommandInfo
                {
                    Command = SOAction.SOCommand.Abandon,
                    SOInfo = soInfo,
                    Parameter = new object[] { immediatelyReturnInventory, isCreateAO, refundInfo }
                });
            }
        }

        /// <summary>
        /// 锁定订单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="note">锁单原因，备注</param>
        public virtual SOInfo HoldSO(int soSysNo, string note)
        {
            SOInfo soInfo = GetSOBySOSysNo(soSysNo);
            if (soInfo == null)
            {
                BizExceptionHelper.Throw("SO_SOIsNotExist");
            }
            soInfo.BaseInfo.HoldReason = note;
            soProcessor.ProcessSO(new SOAction.SOCommandInfo
            {
                Command = SOAction.SOCommand.Hold,
                SOInfo = soInfo
            });
            return soInfo;
        }

        /// <summary>
        /// 解锁已锁定的订单
        /// </summary>
        /// <param name="soBaseInfo"></param>
        public virtual SOInfo UnholdSO(int soSysNo, string note)
        {
            SOInfo soInfo = GetSOBySOSysNo(soSysNo);
            if (soInfo == null)
            {
                BizExceptionHelper.Throw("SO_SOIsNotExist");
            }
            soInfo.BaseInfo.HoldReason = note;
            soProcessor.ProcessSO(new SOAction.SOCommandInfo
            {
                Command = SOAction.SOCommand.Unhold,
                SOInfo = soInfo
            });
            return soInfo;
        }

        /// <summary>
        /// 设置订单增值税发票已开具
        /// </summary>
        /// <param name="infos"></param>
        public virtual void SOVATPrinted(List<int> soSysNoList)
        {
            soProcessor.SOVATPrinted(soSysNoList);
        }

        /// <summary>
        /// 拆分订单发票
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="invoiceItems"></param>
        public virtual void SplitSOInvoice(int soSysNo, List<ECCentral.BizEntity.Invoice.SubInvoiceInfo> invoiceItems)
        {
            soProcessor.SplitSOInvoice(soSysNo, invoiceItems);
        }

        /// <summary>
        /// 取消拆分订单发票
        /// </summary>
        /// <param name="soSysNo"></param>
        public virtual void CancelSplitSOInvoice(int soSysNo)
        {
            soProcessor.CancelSplitSOInvoice(soSysNo);
        }

        /// <summary>
        ///  发送内部邮件
        /// </summary>
        /// <param name="emailList"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<string> SendInternalEmail(List<string> emailList, string title, string content, string language)
        {
            return soProcessor.SendInternalEmail(emailList, title, content, language);
        }

        /// <summary>
        /// 手动更改订单仓库
        /// </summary>
        /// <param name="info"></param>
        public bool WHUpdateStock(SOWHUpdateInfo info)
        {
            return soProcessor.WHUpdateStock(info);
        }

        #endregion

        #region Job 相关方法

        /// <summary>
        /// 处理团购已完成了的订单
        /// </summary>
        /// <param name="companyCode"></param>
        public virtual void ProcessFinishedGroupBuySO(string companyCode)
        {
            List<ECCentral.BizEntity.MKT.GroupBuyingInfo> gbInfoList = ExternalDomainBroker.GetGroupBuyInfoForNeedProcess(companyCode);

            GroupBuySOProcessor groupBuySOProcessor = ObjectFactory<GroupBuySOProcessor>.Instance;

            List<BatchActionItem<ECCentral.BizEntity.MKT.GroupBuyingInfo>> actionItemList = new List<BatchActionItem<ECCentral.BizEntity.MKT.GroupBuyingInfo>>();
            foreach (ECCentral.BizEntity.MKT.GroupBuyingInfo gbInfo in gbInfoList)
            {
                actionItemList.Add(new BatchActionItem<ECCentral.BizEntity.MKT.GroupBuyingInfo> { ID = gbInfo.SysNo.Value.ToString(), Data = gbInfo });
            }
            BatchActionResult<ECCentral.BizEntity.MKT.GroupBuyingInfo> result = BatchActionManager.DoBatchAction<ECCentral.BizEntity.MKT.GroupBuyingInfo>(actionItemList, (gbInfo) =>
            {
                groupBuySOProcessor.ProcessorGroupBuySO(gbInfo);
            });

            result.ThrowErrorException();
        }
        /// <summary>
        /// 处理无效的团购订单
        /// </summary>
        public virtual void ProcessorInvalidGroupBuySO(string companyCode)
        {
            GroupBuySOProcessor groupBuySOProcessor = ObjectFactory<GroupBuySOProcessor>.Instance;
            List<int> soSysNoList = groupBuySOProcessor.GetInvalidGroupBuySOSysNoList(companyCode);
            if (soSysNoList != null)
            {
                if (soSysNoList.Count < 50)
                {
                    List<SOInfo> soInfoList = soProcessor.GetSOBySOSysNoList(soSysNoList);
                    List<BatchActionItem<SOInfo>> actionItemList = new List<BatchActionItem<SOInfo>>();
                    foreach (SOInfo soInfo in soInfoList)
                    {
                        actionItemList.Add(new BatchActionItem<SOInfo> { ID = soInfo.SysNo.Value.ToString(), Data = soInfo });
                    }
                    BatchActionResult<SOInfo> result = BatchActionManager.DoBatchAction<SOInfo>(actionItemList, (soInfo) =>
                    {
                        groupBuySOProcessor.ProcessorInvalidGroupBuySO(soInfo);
                    });
                    result.ThrowErrorException();
                }
                else
                {
                    List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                    foreach (int soSysNo in soSysNoList)
                    {
                        actionItemList.Add(new BatchActionItem<int> { ID = soSysNo.ToString(), Data = soSysNo });
                    }
                    BatchActionResult<int> result = BatchActionManager.DoBatchAction<int>(actionItemList, (soSysNo) =>
                    {
                        SOInfo soInfo = soProcessor.GetSOBySOSysNo(soSysNo);
                        groupBuySOProcessor.ProcessorInvalidGroupBuySO(soInfo);
                    });
                    result.ThrowErrorException();
                }
            }

        }

        /// <summary>
        /// 作废48小时内没有付款的团购订单
        /// </summary>
        /// <param name="companyCode"></param>
        public void AbandonNotPayGroupBuySO(string companyCode)
        {
            List<int> soSysNoList = ObjectFactory<GroupBuySOProcessor>.Instance.GetNotPayGroupBuySOSysNoList(companyCode);
            List<SOInfo> soInfoList = null;
            BatchOperation(soSysNoList, SOAction.SOCommand.Abandon, null, out soInfoList);
        }

        #endregion

        #region OPC

        public List<OPCOfflineTransactionInfo> GetTransactionsByMasterID(int masterId)
        {
            return ObjectFactory<OPCProcessor>.Instance.GetTransactionsByMasterID(masterId);
        }

        #endregion

        public List<SOItemInfo> BatchDealItemFromFile(byte[] fileContent)
        {
            return soProcessor.BatchDealItemFromFile(fileContent);
        }

        /// <summary>
        /// 发送订单出库评论提醒邮件
        /// </summary>
        /// <param name="soSysNo"></param>
        public void SendOrderCommentNotifyMail(int soSysNo)
        {
            var soInfo = soProcessor.GetSOBySOSysNo(soSysNo);
            if (soInfo != null && soInfo.BaseInfo.Status == SOStatus.OutStock)
            {
                ObjectFactory<SOSendMessageProcessor>.Instance.SendCommentNotifyEmailToCustomer(soInfo);
            }
        }

        /// <summary>
        /// 批量设置申报通过
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        public virtual void BatchOperationUpdateSOStatusToReported(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count < 1)
            {
                BizExceptionHelper.Throw("SO_BatchOperation_IsNull");
            }
            else 
            {
                List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                foreach (int sosysno in soSysNoList)
                {
                    BatchActionItem<int> item = new BatchActionItem<int>();
                    item.Data = sosysno;
                    item.ID = sosysno.ToString();
                    actionItemList.Add(item);
                }

                BatchActionResult<int> result = BatchActionManager.DoBatchAction<int>(actionItemList, (sosysno) =>
                    {
                        ObjectFactory<SOProcessor>.NewInstance().UpdateSOStatusToReported(sosysno);
                    });

                result.ThrowErrorException();

            }
        }

        /// <summary>
        /// 网关订单查询
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public TransactionQueryBill QueryBill(string soSysNo)
        {
            return ObjectFactory<SOProcessor>.Instance.QueryBill(soSysNo);
        }

        public ECCentral.BizEntity.Invoice.SOIncomeInfo GetValidSOIncomeInfo(int orderSysNo)
        {
            return ObjectFactory<SOProcessor>.Instance.GetValidSOIncomeInfo(orderSysNo);
        }

        /// <summary>
        /// 修改订单状态为 已申报待通关
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToReported(int sosysno)
        {
            return ObjectFactory<SOProcessor>.Instance.UpdateSOStatusToReported(sosysno);
        }

        /// <summary>
        /// 修改订单状态为 申报失败订单作废
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToReject(int sosysno)
        {
            var soInfo = soProcessor.GetSOBySOSysNo(sosysno);
            if (soInfo != null)
            {
                soProcessor.ProcessSO(new SOAction.SOCommandInfo
                {
                    Command = SOAction.SOCommand.Abandon,
                    SOInfo = soInfo,
                    Parameter = new object[] { true, true, null, false,SOStatus.Reject }
                });
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改订单状态为 已通关发往顾客
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToCustomsPass(int sosysno)
        {
            return ObjectFactory<SOProcessor>.Instance.UpdateSOStatusToCustomsPass(sosysno);
        }

        /// <summary>
        /// 修改订单状态为 通关失败订单作废
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToCustomsReject(int sosysno)
        {
            var soInfo = soProcessor.GetSOBySOSysNo(sosysno);
            if (soInfo != null)
            {
                soProcessor.ProcessSO(new SOAction.SOCommandInfo
                {
                    Command = SOAction.SOCommand.Abandon,
                    SOInfo = soInfo,
                    Parameter = new object[] { true, true, null, false, SOStatus.CustomsReject }
                });
                return true;
            }
            return false;
        }


        /// <summary>
        /// 批量设置 申报失败订单作废
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        public virtual void BatchOperationUpdateSOStatusToReject(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count < 1)
            {
                BizExceptionHelper.Throw("SO_BatchOperation_IsNull");
            }
            else
            {
                List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                foreach (int sosysno in soSysNoList)
                {
                    BatchActionItem<int> item = new BatchActionItem<int>();
                    item.Data = sosysno;
                    item.ID = sosysno.ToString();
                    actionItemList.Add(item);
                }

                BatchActionResult<int> result = BatchActionManager.DoBatchAction<int>(actionItemList, (sosysno) =>
                {
                    var process = ObjectFactory<SOProcessor>.NewInstance();
                    var soInfo = soProcessor.GetSOBySOSysNo(sosysno);
                    if (soInfo != null)
                    {
                        process.ProcessSO(new SOAction.SOCommandInfo
                        {
                            Command = SOAction.SOCommand.Abandon,
                            SOInfo = soInfo,
                            Parameter = new object[] { true, true, null, true, SOStatus.Reject }
                        });
                    }
                });

                result.ThrowErrorException();

            }
        }


        /// <summary>
        /// 批量设置 已通关发往顾客
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        public virtual void BatchOperationUpdateSOStatusToCustomsPass(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count < 1)
            {
                BizExceptionHelper.Throw("SO_BatchOperation_IsNull");
            }
            else
            {
                List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                foreach (int sosysno in soSysNoList)
                {
                    BatchActionItem<int> item = new BatchActionItem<int>();
                    item.Data = sosysno;
                    item.ID = sosysno.ToString();
                    actionItemList.Add(item);
                }

                BatchActionResult<int> result = BatchActionManager.DoBatchAction<int>(actionItemList, (sosysno) =>
                {
                    ObjectFactory<SOProcessor>.NewInstance().UpdateSOStatusToCustomsPass(sosysno);
                });

                result.ThrowErrorException();

            }
        }


        /// <summary>
        /// 批量设置 通关失败订单作废
        /// </summary>
        /// <param name="soSysNoList">订单编号列表</param>
        public virtual void BatchOperationUpdateSOStatusToCustomsReject(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count < 1)
            {
                BizExceptionHelper.Throw("SO_BatchOperation_IsNull");
            }
            else
            {
                List<BatchActionItem<int>> actionItemList = new List<BatchActionItem<int>>();
                foreach (int sosysno in soSysNoList)
                {
                    BatchActionItem<int> item = new BatchActionItem<int>();
                    item.Data = sosysno;
                    item.ID = sosysno.ToString();
                    actionItemList.Add(item);
                }

                BatchActionResult<int> result = BatchActionManager.DoBatchAction<int>(actionItemList, (sosysno) =>
                {
                    var process = ObjectFactory<SOProcessor>.NewInstance();
                    var soInfo = soProcessor.GetSOBySOSysNo(sosysno);
                    if (soInfo != null)
                    {
                        process.ProcessSO(new SOAction.SOCommandInfo
                        {
                            Command = SOAction.SOCommand.Abandon,
                            SOInfo = soInfo,
                            Parameter = new object[] { true, true, null, true, SOStatus.CustomsReject }
                        });
                    }
                });

                result.ThrowErrorException();

            }
        }

        public virtual void SOMaintainUpdateNote(SOInfo info)
        {
            ObjectFactory<SOProcessor>.Instance.SOMaintainUpdateNote(info);
        }
    }
}
