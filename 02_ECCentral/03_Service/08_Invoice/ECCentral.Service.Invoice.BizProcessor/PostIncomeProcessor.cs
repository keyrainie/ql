using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(PostIncomeProcessor))]
    public class PostIncomeProcessor
    {
        private IPostIncomeDA m_PostIncomeDA = ObjectFactory<IPostIncomeDA>.Instance;
        TransactionOptions options = new TransactionOptions();

        /// <summary>
        /// 创建电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="needValidate">是否需要验证SO#和OrderType</param>
        /// <returns></returns>
        public virtual PostIncomeInfo Create(PostIncomeInfo entity, bool needValidate)
        {
            PostIncomeInfo result = new PostIncomeInfo();
            PreCheckForCreateOrUpdate(entity, needValidate);

            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                entity.HandleStatus = PostIncomeHandleStatus.WaitingHandle;
                entity.ConfirmStatus = PostIncomeStatus.Origin;

                result = m_PostIncomeDA.Create(entity);

                //发送创建电汇邮局收款单Message
                EventPublisher.Publish(new CreatePostIncomeInfoMessage()
                {
                    PostIncomeInfoSysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }

            return result;
        }

        /// <summary>
        /// 更新电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Update(PostIncomeInfo entity, string confirmedSOSysNo)
        {
            PreCheckForCreateOrUpdate(entity, false);

            var origin = LoadBySysNo(entity.SysNo.Value);

            if (origin.ConfirmStatus == PostIncomeStatus.Confirmed)
            {
                Handle(entity, confirmedSOSysNo);
            }

            m_PostIncomeDA.Update(entity);
        }

        /// <summary>
        /// 处理电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Handle(PostIncomeInfo entity, string confirmedSOSysNoStr)
        {
            PreCheckForHandle(entity, confirmedSOSysNoStr);

            var postIncomeInfo = LoadBySysNo(entity.SysNo.Value);
            if (postIncomeInfo.ConfirmStatus.Value != PostIncomeStatus.Confirmed)
            {
                ThrowBizException("PostIncome_Handle_ConfirmStatusNotMatch");
            }

            //保证下面对ConfirmedSOSysNoList操作时不为NULL
            if (string.IsNullOrEmpty(confirmedSOSysNoStr))
            {
                confirmedSOSysNoStr = string.Empty;
            }
            string[] confirmedSOSysNoList = confirmedSOSysNoStr.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var confirmedSOSysNo in confirmedSOSysNoList)
            {
                //验证订单号是否有效
                var so = ExternalDomainBroker.GetSOBaseInfo(Convert.ToInt32(confirmedSOSysNo));
                if (!ObjectFactory<PostPayProcessor>.Instance.IsBankOrPostPayType(so.PayTypeSysNo.Value))
                {
                    ThrowBizException("PostIncome_NotMatchSOPayTypeSysNo", confirmedSOSysNo);
                }
            }

            List<int> abandonList = new List<int>();
            var postIncomeConfirm = postIncomeInfo.ConfirmInfoList;
            postIncomeConfirm.ForEach(p =>
            {
                int idx = confirmedSOSysNoStr.IndexOf(p.ConfirmedSoSysNo.ToString());
                if (idx < 0)
                {
                    if (p.Status == PostIncomeConfirmStatus.Audit)
                    {
                        ThrowBizException("PostIncome_StatusNotMatchAudit", p.ConfirmedSoSysNo);
                    }
                    if (p.Status == PostIncomeConfirmStatus.Related)
                    {
                        abandonList.Add(p.SysNo.Value);
                    }
                }
            });

            List<int> updateList = new List<int>();
            List<string> errorList = new List<string>();
            postIncomeConfirm = GetConfirmedListBySOSysNo(string.Join(",", confirmedSOSysNoList));
            postIncomeConfirm.ForEach(p =>
            {
                if (p.Status == PostIncomeConfirmStatus.Audit || (p.Status == PostIncomeConfirmStatus.Related && p.PostIncomeSysNo != entity.SysNo))
                {
                    errorList.Add(p.ConfirmedSoSysNo.ToString());
                }
                else
                {
                    updateList.Add(p.SysNo.Value);
                }
            });

            if (errorList.Count > 0)
            {
                ThrowBizException("PostIncome_StatusNotMatchRelated", string.Join(".", errorList.ToArray()));
            }

            List<PostIncomeConfirmInfo> insertEntityList = new List<PostIncomeConfirmInfo>();
            string[] confirmedSoList = confirmedSOSysNoList;
            foreach (string confirmedSo in confirmedSoList)
            {
                int soNo = Convert.ToInt32(confirmedSo);
                bool exist = postIncomeConfirm.Exists(p => p.ConfirmedSoSysNo == soNo);
                if (!exist)
                {
                    insertEntityList.Add(new PostIncomeConfirmInfo()
                    {
                        PostIncomeSysNo = entity.SysNo,
                        ConfirmedSoSysNo = soNo
                    });
                }
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                abandonList.ForEach(p =>
                {
                    UpdatePostIncomeConfirmStatus(p, PostIncomeConfirmStatus.Cancel);//作废CS确认的订单
                });

                updateList.ForEach(p =>
                {
                    UpdatePostIncomeConfirmStatus(p, PostIncomeConfirmStatus.Related);//更新CS确认的订单
                });

                insertEntityList.ForEach(p =>
                {
                    CreatePostIncomeConfirm(p);//添加CS确认的订单
                });

                m_PostIncomeDA.Handle(entity);

                scope.Complete();
            }

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostIncome_Log_Handle", ServiceContext.Current.UserSysNo, postIncomeInfo.SysNo)
                , BizLogType.Invoice_PostIncome_Handle
                , postIncomeInfo.SysNo.Value
                , postIncomeInfo.CompanyCode);
        }

        /// <summary>
        /// 根据订单编号获取电汇邮局收款单确认信息列表，多个订单编号之间用逗号(,)隔开
        /// </summary>
        /// <param name="soSysNoStr"></param>
        /// <returns></returns>
        public virtual List<PostIncomeConfirmInfo> GetConfirmedListBySOSysNo(string soSysNoStr)
        {
            return m_PostIncomeDA.GetConfirmedListBySOSysNo(soSysNoStr);
        }

        /// <summary>
        /// 根据电汇邮局收款单系统编号获取收款单确认信息列表
        /// </summary>
        /// <param name="postIncomeSysNo">电汇邮局收款单系统编号</param>
        /// <returns></returns>
        public virtual List<PostIncomeConfirmInfo> GetConfirmedListByPostIncomeSysNo(int postIncomeSysNo)
        {
            return m_PostIncomeDA.GetConfirmListByPostIncomeSysNo(postIncomeSysNo);
        }

        /// <summary>
        /// 确认电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">待确认的电汇邮局收款单系统编号</param>
        public virtual void Confirm(int sysNo)
        {
            var postIncomeInfo = LoadBySysNo(sysNo);

            if (postIncomeInfo.ConfirmStatus.Value != PostIncomeStatus.Origin)
            {
                ThrowBizException("PostIncome_Confirm_StatusNotMatchOrigin");
            }

            //更新收款单状态为已确认状态
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_PostIncomeDA.UpdateConfirmStatus(postIncomeInfo.SysNo.Value, PostIncomeStatus.Confirmed);

                //发送确认电汇邮局收款单Message
                EventPublisher.Publish(new ConfirmPostIncomeInfoMessage()
                {
                    PostIncomeInfoSysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                scope.Complete();
            }


            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostIncome_Log_Confirm", ServiceContext.Current.UserSysNo, postIncomeInfo.SysNo)
                , BizLogType.Invoice_PostIncome_Confirm
                , postIncomeInfo.SysNo.Value
                , postIncomeInfo.CompanyCode);
        }

        /// <summary>
        /// 取消确认电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">待取消确认的电汇邮局收款单系统编号</param>
        public virtual void CancelConfirm(int sysNo)
        {
            var postIncomeInfo = LoadBySysNo(sysNo);

            if (postIncomeInfo.ConfirmStatus.Value != PostIncomeStatus.Confirmed || postIncomeInfo.HandleStatus.Value == PostIncomeHandleStatus.Handled)
            {
                ThrowBizException("PostIncome_CancelConfirm_StatusNotMatchConfirmed");
            }

            //更新收款单状态为初始状态
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_PostIncomeDA.UpdateConfirmStatus(postIncomeInfo.SysNo.Value, PostIncomeStatus.Origin);


                //发送取消确认电汇邮局收款单Message
                EventPublisher.Publish(new CancelConfirmPostIncomeMessage()
                {
                    PostIncomeInfoSysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                scope.Complete();
            }

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostIncome_Log_CancelConfirm", ServiceContext.Current.UserSysNo, postIncomeInfo.SysNo)
                , BizLogType.Invoice_PostIncome_CancelConfirm
                , postIncomeInfo.SysNo.Value
                , postIncomeInfo.CompanyCode);
        }

        /// <summary>
        /// 作废电汇邮局收款单
        /// </summary>
        /// <param name="sysNo">待作废的电汇邮局收款单系统编号</param>
        public virtual void Abandon(int sysNo)
        {
            var postIncomeInfo = LoadBySysNo(sysNo);

            if (postIncomeInfo.ConfirmStatus.Value != PostIncomeStatus.Origin)
            {
                ThrowBizException("PostIncome_Abandon_StatusNotMatchOrigin");
            }

            var postIncomeConfirm = postIncomeInfo.ConfirmInfoList;
            foreach (var confirmedSO in postIncomeConfirm)
            {
                if (confirmedSO.Status == PostIncomeConfirmStatus.Audit)
                {
                    ThrowBizException("PostIncome_Abandon_RelatedStatusNotMatch", postIncomeInfo.SysNo, confirmedSO.ConfirmedSoSysNo);
                }
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                foreach (var confirmedSO in postIncomeConfirm)
                {
                    if (confirmedSO.Status == PostIncomeConfirmStatus.Related)
                    {
                        UpdatePostIncomeConfirmStatus(confirmedSO.SysNo.Value, PostIncomeConfirmStatus.Cancel);
                    }
                }
                //更新收款单状态为作废状态

                m_PostIncomeDA.UpdateConfirmStatus(postIncomeInfo.SysNo.Value, PostIncomeStatus.Abandon);

                //发送作废电汇邮局收款单Message
                EventPublisher.Publish(new VoidPostIncomeInfoMessage()
                {
                    PostIncomeInfoSysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                scope.Complete();
            }

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostIncome_Log_Abandon", ServiceContext.Current.UserSysNo, postIncomeInfo.SysNo)
                , BizLogType.Invoice_PostIncome_Abandon
                , postIncomeInfo.SysNo.Value
                , postIncomeInfo.CompanyCode);
        }

        /// <summary>
        /// 取消作废电汇邮局收款单
        /// </summary>
        /// <param name="entity">待取消作废的电汇邮局收款单系统编号</param>
        public virtual void CancelAbandon(int sysNo)
        {
            var postIncomeInfo = LoadBySysNo(sysNo);

            if (postIncomeInfo.ConfirmStatus.Value != PostIncomeStatus.Abandon)
            {
                ThrowBizException("PostIncome_CancelAbandon_StatusNotMatchAbandon");
            }

            //更新收款单状态为初始状态
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_PostIncomeDA.UpdateConfirmStatus(postIncomeInfo.SysNo.Value, PostIncomeStatus.Origin);

                //发送取消作废电汇邮局收款单Message
                EventPublisher.Publish(new CancelVoidPostIncomeInfoMessage()
                {
                    PostIncomeInfoSysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                scope.Complete();
            }

            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostIncome_Log_CancelAbandon", ServiceContext.Current.UserSysNo, postIncomeInfo.SysNo)
                , BizLogType.Invoice_PostIncome_CancelAbandon
                , postIncomeInfo.SysNo.Value
                , postIncomeInfo.CompanyCode);
        }

        /// <summary>
        /// 加载电汇邮局收款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual PostIncomeInfo LoadBySysNo(int sysNo)
        {
            var entity = m_PostIncomeDA.LoadBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("PostIncome_RecordNotExist", sysNo);
            }
            return entity;
        }

        /// <summary>
        /// 根据订单系统编号列表取得已和订单关联的PostIncome列表
        /// </summary>
        /// <param name="confirmedSOSysNo">订单系统编号列表</param>
        /// <returns></returns>
        public virtual List<PostIncomeInfo> GetListBySOSysNoList(List<int> soSysNoList)
        {
            if (soSysNoList == null || soSysNoList.Count == 0)
                return null;

            string soSysNoStr = string.Join(",", soSysNoList.Distinct());
            return m_PostIncomeDA.GetListByConfirmedSOSysNo(soSysNoStr);
        }

        /// <summary>
        /// 更新电汇邮局收款单确认信息状态
        /// </summary>
        /// <param name="postIncomeSysNo"></param>
        /// <param name="postIncomeConfirmStatus"></param>
        public virtual void UpdatePostIncomeConfirmStatus(int postIncomeSysNo, PostIncomeConfirmStatus postIncomeConfirmStatus)
        {
            m_PostIncomeDA.UpdatePostIncomeConfirmStatus(postIncomeSysNo, postIncomeConfirmStatus);
        }

        /// <summary>
        /// Create Or Update前检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForCreateOrUpdate(PostIncomeInfo entity, bool needValidate)
        {
            entity.Validate(p => p.IncomeDate != null, () => ThrowBizException("PostIncome_IncomeDateRequired"))
                .Validate(p => !string.IsNullOrEmpty(p.IncomeBank), () => ThrowBizException("PostIncome_IncomeBankRequired"))
                .Validate(p => p.IncomeAmt != null, () => ThrowBizException("PostIncome_IncomeAmtRequired"))
                .Validate(p => p.IncomeAmt >= 0, () => ThrowBizException("PostIncome_IncomeAmtShouldNotLessThanZero"))
                .Validate(p =>
                {
                    if (p.SOSysNo != null && needValidate)
                    {
                        var so = ObjectFactory<ISOBizInteract>.Instance.GetSOBaseInfo(p.SOSysNo.Value);
                        if (so == null)
                        {
                            ThrowBizException("PostIncome_SONotExistFormat", p.SOSysNo);
                        }
                        return ObjectFactory<PostPayProcessor>.Instance.IsBankOrPostPayType(so.PayTypeSysNo.Value);
                    }
                    return true;
                }, () => ThrowBizException("PostIncome_NotMatchSOPayTypeSysNo"));
        }

        /// <summary>
        /// Handle前检查
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void PreCheckForHandle(PostIncomeInfo entity, string confirmedSOSysNoStr)
        {
            entity.Validate(p => p.SysNo != null, () =>
            {
                throw new ArgumentNullException("entity.SysNo");
            })
            .Validate(p => p.HandleStatus != null, () =>
            {
                throw new ArgumentNullException("entity.HandleStatus");
            });

            var soSysNoList = confirmedSOSysNoStr.Replace(",", ".")
                .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToList()
                .ConvertAll(s => Int32.Parse(s));
            var soList = ExternalDomainBroker.GetSOBaseInfoList(soSysNoList);
            if (soList.Count != soSysNoList.Count)
            {
                var exceptSysNoList = soSysNoList.Except(soList.Select(s => s.SysNo.Value)).ToList();
                ThrowBizException("PostIncome_InvalidSOSysNo", string.Join(".", exceptSysNoList));
            }
            else
            {
                var recTotal = soList.Sum(s => s.ReceivableAmount);
                if (recTotal > entity.IncomeAmt.Value)
                {
                    ThrowBizException("PostIncome_ReceivableAmtMoreThanIncomeAmt");
                }
            }
        }

        /// <summary>
        /// 建电汇邮局收款单确认信息
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CreatePostIncomeConfirm(PostIncomeConfirmInfo entity)
        {
            entity.Status = PostIncomeConfirmStatus.Related;

            m_PostIncomeDA.CreatePostIncomeConfirm(entity);
        }

        #region [For SO Domain]

        public virtual void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            if (master == null || subList == null)
            {
                throw new ArgumentNullException("entity");
            }

            var postIncomeConfirmList = m_PostIncomeDA.GetConfirmedListBySOSysNo(master.SysNo.ToString());
            if (postIncomeConfirmList == null || postIncomeConfirmList.Count == 0)
            {
                ThrowBizException("PostIncome_PostIncomeNotFound");
            }

            var origin = m_PostIncomeDA.LoadBySysNo(postIncomeConfirmList[0].PostIncomeSysNo.Value);
            origin.ConfirmStatus = PostIncomeStatus.Abandon;
            origin.HandleStatus = PostIncomeHandleStatus.Handled;

            foreach (var postIncomeConfirm in postIncomeConfirmList)
            {
                m_PostIncomeDA.UpdatePostIncomeConfirmStatus(postIncomeConfirm.SysNo.Value, PostIncomeConfirmStatus.Cancel);
            }
            m_PostIncomeDA.AbandonSplitForSO(origin);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("PostIncome_Log_Abandon", ServiceContext.Current.UserSysNo, origin.SysNo)
                , BizLogType.Invoice_PostIncome_Abandon
                , origin.SysNo.Value
                , origin.CompanyCode);
        }

        #endregion [For SO Domain]

        #region Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PostIncome, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}