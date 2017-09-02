using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(TrackingInfoProcessor))]
    public class TrackingInfoProcessor
    {
        private ITrackingInfoDA m_TrackInfoDA = ObjectFactory<ITrackingInfoDA>.Instance;

        #region TrackingInfo

        public virtual TrackingInfo GetTrackingInfoByOrderSysNo(int orderSysNo, SOIncomeOrderType orderType)
        {
            return m_TrackInfoDA.LoadTrackingInfoByOrderSysNo(orderSysNo, orderType);
        }

        public virtual void UpdateTrackingInfoStatus(TrackingInfo entity)
        {
            m_TrackInfoDA.UpdateTrackingInfoStatus(entity);
        }

        public virtual TrackingInfo CreateTrackingInfo(TrackingInfo entity)
        {
            PreCheckForCreate(entity);

            entity.ResponsibleUserName = GetResponsibleUserName(entity);
            entity.IncomeAmt = m_TrackInfoDA.GetIncomeAmt(entity.OrderSysNo.Value, entity.OrderType.Value);

            m_TrackInfoDA.CreateTrackingInfo(entity);

            return entity;
        }

        protected void PreCheckForCreate(TrackingInfo entity)
        {
            if (!entity.OrderSysNo.HasValue)
            {
                ThrowBizException("TrackingInfo_RequiredOrderSysNo");
            }

            if (!entity.OrderType.HasValue)
            {
                ThrowBizException("TrackingInfo_RequiredOrderType");
            }

            if (m_TrackInfoDA.ExistsTrackingInfo(entity.OrderSysNo.Value, entity.OrderType.Value))
            {
                ThrowBizException("TrackingInfo_ExistTrackingInfo", entity.OrderSysNo);
            }
        }

        public virtual List<ResponsibleUserInfo> GetAllResponsibleUsers(string companyCode)
        {
            return m_TrackInfoDA.GetAllResponsibleUsers(companyCode);
        }

        protected virtual string GetResponsibleUserName(TrackingInfo entity)
        {
            if (!entity.OrderSysNo.HasValue)
            {
                throw new ArgumentNullException("entity.OrderSysNo");
            }

            if (!entity.OrderType.HasValue)
            {
                throw new ArgumentNullException("entity.OrderType");
            }

            var allResponseUsers = GetAllResponsibleUsers(entity.CompanyCode);

            //fixbug:Dictionary键重复问题
            //原来的IPP在配置责任人的时候有bug，不能判断勾选了“特殊责任人”，但是实际条件又不满足“特殊责任人”的情况，导致生成重复的责任人数据（配送方式、支付方式、客户编号相同的数据）。
            //ECCentral不会产生这样的数据，但为了兼容之前的数据，作出下面的调整。
            var vipDict = allResponseUsers.Where(x => x.CustomerSysNo.HasValue)
                .GroupBy(g => g.CustomerSysNo).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var shipTypeDict = allResponseUsers.Where(x => x.ShipTypeSysNo.HasValue)
                .GroupBy(g => g.ShipTypeSysNo).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var payTypeDict = allResponseUsers.Where(x => x.PayTypeSysNo.HasValue)
                .GroupBy(g => g.PayTypeSysNo).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var incomeStyleDict = allResponseUsers.Where(x => x.IncomeStyle.HasValue
                                        && !x.PayTypeSysNo.HasValue
                                        && !x.ShipTypeSysNo.HasValue
                                        && !x.CustomerSysNo.HasValue
                                        && !x.SourceType.HasValue).ToDictionary(x => x.IncomeStyle);

            if (entity.OrderType == SOIncomeOrderType.SO)
            {
                var soInfo = ExternalDomainBroker.GetSOInfo(entity.OrderSysNo.Value);

                if (soInfo == null)
                {
                    ThrowBizException("TrackingInfo_NotExistSORecord", entity.OrderSysNo);
                }

                var soIncomeInfo = ObjectFactory<SOIncomeProcessor>.Instance.GetListByCriteria(null,soInfo.SysNo,SOIncomeOrderType.SO,null)
                                        .Select(s => s).FirstOrDefault();

                var netpayInfo = ObjectFactory<NetPayProcessor>.Instance.GetListByCriteria(
                    new NetPayInfo()
                    {
                        SOSysNo = soInfo.SysNo,
                    }).Where(w => w.Status > NetPayStatus.Abandon).Select(s => s).FirstOrDefault();

                //责任人来源方式
                ResponsibleSource? souceType = null;
                if (soIncomeInfo.IncomeStyle == SOIncomeOrderStyle.Advanced)
                {
                    if (netpayInfo.Source == NetPaySource.Bank)
                    {
                        souceType = ResponsibleSource.NetPay;
                    }
                    else
                    {
                        souceType = ResponsibleSource.Manual;
                    }
                }

                //款到发货
                if (soIncomeInfo.IncomeStyle == SOIncomeOrderStyle.Advanced)
                {
                    #region CRL10309 By Kilin 根据来源方式(网关、手动)匹配责任人

                    //根据来源方式匹配
                    ResponsibleUserInfo responsibleUser = allResponseUsers
                        .Where(w => w.IncomeStyle == ResponseUserOrderStyle.Advanced
                            && w.SourceType.HasValue
                            && souceType.HasValue
                            && w.SourceType == souceType.Value)
                        .Select(s => s)
                        .FirstOrDefault();

                    if (responsibleUser != null)
                    {
                        return responsibleUser.ResponsibleUserName;
                    }

                    //如果是 银行电汇
                    if (soInfo.BaseInfo.PayTypeSysNo.HasValue)
                    {
                        if (payTypeDict.ContainsKey(soInfo.BaseInfo.PayTypeSysNo.Value))
                        {
                            return payTypeDict[soInfo.BaseInfo.PayTypeSysNo.Value].ResponsibleUserName;
                        }
                    }

                    //最后一步取支付方式和来源方式都为空的数据
                    responsibleUser = allResponseUsers
                        .Where(w => w.IncomeStyle == ResponseUserOrderStyle.Advanced
                            && !w.PayTypeSysNo.HasValue
                            && !w.SourceType.HasValue)
                        .Select(s => s)
                        .FirstOrDefault();

                    if (responsibleUser != null)
                    {
                        return responsibleUser.ResponsibleUserName;
                    }

                    #endregion CRL10309 By Kilin 根据来源方式(网关、手动)匹配责任人
                }
                //货到付款
                else if (soIncomeInfo.IncomeStyle == SOIncomeOrderStyle.Normal)
                {
                    //优先按照客户编号匹配责任人
                    if (vipDict.ContainsKey(soInfo.BaseInfo.CustomerSysNo.Value))
                    {
                        return vipDict[soInfo.BaseInfo.CustomerSysNo.Value].ResponsibleUserName;
                    }

                    //其次是按照配送方式匹配责任人
                    if (shipTypeDict.ContainsKey(soInfo.ShippingInfo.ShipTypeSysNo.Value))
                    {
                        return shipTypeDict[soInfo.ShippingInfo.ShipTypeSysNo.Value].ResponsibleUserName;
                    }
                }

                ResponseUserOrderStyle incomeStyle = ResponseUserOrderStyle.Normal;
                if (!soInfo.BaseInfo.PayWhenReceived.Value)
                {
                    incomeStyle = ResponseUserOrderStyle.Advanced;
                }

                //最后按照是否货到付款确定责任人
                if (incomeStyleDict.ContainsKey(incomeStyle))
                {
                    return incomeStyleDict[incomeStyle].ResponsibleUserName;
                }
            }
            else
            {
                if (incomeStyleDict.ContainsKey(ResponseUserOrderStyle.RefundException))
                {
                    return incomeStyleDict[ResponseUserOrderStyle.RefundException].ResponsibleUserName;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 提交报损
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual void SubmitTrackingInfo(int sysNo)
        {
            var entity = GetTrackingInfoBySysNo(sysNo);

            if (entity.Status != TrackingInfoStatus.Follow)
            {
                ThrowBizException("TrackingInfo_Submit_StatusNotMatchFollow");
            }

            entity.Status = TrackingInfoStatus.Submit;
            m_TrackInfoDA.UpdateTrackingInfoStatus(entity);
        }

        /// <summary>
        /// 关闭跟踪单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual void CloseTrackingInfo(int sysNo)
        {
            var entity = GetTrackingInfoBySysNo(sysNo);

            if (entity.Status == TrackingInfoStatus.Confirm)
            {
                ThrowBizException("TrackingInfo_Close_ConfirmStatusCanNotClose");
            }

            entity.Status = TrackingInfoStatus.Confirm;
            m_TrackInfoDA.UpdateTrackingInfoStatus(entity);
        }

        /// <summary>
        /// 修改跟踪单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateTrackingInfo(TrackingInfo entity)
        {
            var trackingInfo = GetTrackingInfoBySysNo(entity.SysNo.Value);

            if (trackingInfo.Status == TrackingInfoStatus.Confirm)
            {
                ThrowBizException("TrackingInfo_Update_ConfirmStatusCanNotUpdate");
            }

            //修改责任人姓名
            ChangeResponsibleUserName(entity, trackingInfo, string.Empty);

            //多次备注之间用分号隔开
            string note = string.Concat(trackingInfo.Note, ";", entity.Note).TrimStart(';').TrimEnd(';');

            //备注内容查过500，需要记录日志。
            if (note.Length > 500)
            {
                //记录操作日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("TrackingInfo_Log_UpdateTrackingInfo", ServiceContext.Current.UserSysNo, entity.SysNo, entity.Note)
                    , BizLogType.Invoice_TrackingInfo_UpdateTrackingInfo
                    , entity.SysNo.Value
                    , entity.CompanyCode);

                entity.Note = trackingInfo.Note;
            }
            else
            {
                entity.Note = note;
            }

            m_TrackInfoDA.UpdateTrackingInfo(entity);
        }

        public virtual TrackingInfo GetTrackingInfoBySysNo(int sysNo)
        {
            return m_TrackInfoDA.LoadTrackingInfoBySysNo(sysNo);
        }

        #endregion TrackingInfo

        #region ResponsibleUser

        /// <summary>
        /// 创建跟踪单负责人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ResponsibleUserInfo CreateResponsibleUser(ResponsibleUserInfo entity)
        {
            PreCheckResponsibleUser(entity);

            return m_TrackInfoDA.CreateResponsibleUser(entity);
        }

        /// <summary>
        /// 更新跟踪单负责人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ResponsibleUserInfo UpdateResponsibleUser(ResponsibleUserInfo entity)
        {
            PreCheckResponsibleUser(entity);

            var responsibleUser = m_TrackInfoDA.LoadResponsibleUserBySysNo(entity.SysNo.Value);

            if (string.Compare(responsibleUser.ResponsibleUserName, entity.ResponsibleUserName, true) != 0)
            {
                var notClosed = m_TrackInfoDA.GetNotClosedTrackingInfoBelongToCertainUser(responsibleUser.ResponsibleUserName);

                foreach (var trackingInfo in notClosed)
                {
                    if (IsRuleMatch(responsibleUser, trackingInfo))
                    {
                        trackingInfo.ResponsibleUserName = entity.ResponsibleUserName;
                        UpdateTrackingInfoResponsibleUserName(trackingInfo, entity.EmailAddress);
                    }
                }
            }

            m_TrackInfoDA.UpdateResponsibleUser(entity);

            return entity;
        }

        /// <summary>
        /// 取得已经存在的符合条件的跟踪单负责人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ResponsibleUserInfo GetExistedResponsibleUser(ResponsibleUserInfo entity)
        {
            return m_TrackInfoDA.GetExistedResponsibleUser(entity);
        }

        /// <summary>
        /// 作废跟踪单责任人
        /// </summary>
        /// <param name="entity"></param>
        public virtual void AbandonResponsibleUser(int sysNo)
        {
            m_TrackInfoDA.AbandonResponsibleUser(sysNo);
        }

        protected void PreCheckResponsibleUser(ResponsibleUserInfo entity)
        {
            if (!entity.IncomeStyle.HasValue && !entity.PayTypeSysNo.HasValue && !entity.ShipTypeSysNo.HasValue && !entity.CustomerSysNo.HasValue)
            {
                ThrowBizException("TrackingInfo_ValueCantAllEmpty");
            }
            if (String.IsNullOrEmpty(entity.ResponsibleUserName))
            {
                ThrowBizException("TrackingInfo_UserNameIsRequird");
            }
            if (!entity.Status.HasValue)
            {
                ThrowBizException("TrackingInfo_StatusIsRequird");
            }
            if (String.IsNullOrEmpty(entity.EmailAddress))
            {
                ThrowBizException("TrackingInfo_EmailAddressIsRequird");
            }
            if (entity.CustomerSysNo.HasValue)
            {
                if (!ExternalDomainBroker.ExistsCustomer(entity.CustomerSysNo.Value))
                {
                    ThrowBizException("TrackingInfo_CustomerNotFound");
                }
            }
        }

        private bool IsRuleMatch(ResponsibleUserInfo responsibleUser, TrackingInfo trackingInfo)
        {
            var soIncomeBL = ObjectFactory<SOIncomeProcessor>.Instance;
            if (trackingInfo.OrderType == SOIncomeOrderType.SO)
            {
                //订单不存在直接抛出异常，这个是数据错误。
                var soInfo = ExternalDomainBroker.GetSOInfo(trackingInfo.OrderSysNo.Value);

                if (responsibleUser.CustomerSysNo == soInfo.BaseInfo.CustomerSysNo
                    || responsibleUser.ShipTypeSysNo == soInfo.ShippingInfo.ShipTypeSysNo
                    || responsibleUser.PayTypeSysNo == soInfo.BaseInfo.PayTypeSysNo)
                {
                    return true;
                }

                //比较收款类型，SO单只有货到付款和款到发货两种。
                if (!responsibleUser.CustomerSysNo.HasValue
                    && !responsibleUser.ShipTypeSysNo.HasValue
                    && !responsibleUser.PayTypeSysNo.HasValue
                    && (int)responsibleUser.IncomeStyle.Value == (int)trackingInfo.IncomeStyle.Value
                    )
                {
                    return true;
                }
            }
            else
            {
                if (!responsibleUser.CustomerSysNo.HasValue
                    && !responsibleUser.ShipTypeSysNo.HasValue
                    && !responsibleUser.PayTypeSysNo.HasValue
                    && responsibleUser.IncomeStyle == ResponseUserOrderStyle.RefundException)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 修改跟踪单责任人姓名
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="emailAddress"></param>
        public virtual void UpdateTrackingInfoResponsibleUserName(TrackingInfo entity, string emailAddress)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (string.IsNullOrEmpty(entity.ResponsibleUserName))
            {
                ThrowBizException("TrackingInfo_UserNameIsRequird");
            }
            var trackingInfo = m_TrackInfoDA.LoadTrackingInfoBySysNo(entity.SysNo.Value);

            if (trackingInfo.Status == TrackingInfoStatus.Confirm)
            {
                ThrowBizException("TrackingInfo_StatusCantConfirm");
            }

            ChangeResponsibleUserName(entity, trackingInfo, emailAddress);

            m_TrackInfoDA.UpdateTrackingInfoResponsibleUserName(entity);
        }

        /// <summary>
        /// 修改跟踪单损失类型
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateTrackingInfoLossType(TrackingInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (!entity.LossType.HasValue)
            {
                ThrowBizException("TrackingInfo_LossTypeRequired");
            }

            var trackingInfo = m_TrackInfoDA.LoadTrackingInfoBySysNo(entity.SysNo.Value);
            if (trackingInfo.Status == TrackingInfoStatus.Confirm)
            {
                ThrowBizException("TrackingInfo_StatusCantConfirm");
            }

            m_TrackInfoDA.UpdateTrackingInfoLossType(entity);
        }

        protected virtual void ChangeResponsibleUserName(TrackingInfo entity, TrackingInfo trackingInfo, string emailAddr)
        {
            if (string.Compare(trackingInfo.ResponsibleUserName, entity.ResponsibleUserName, true) != 0)
            {
                string fromEmail = string.Empty;        //邮件发送人
                string emailAddress = string.Empty;     //收件人
                string ccEmailAddress = string.Empty;   //抄送人 = 发送人
                string orgEmail = string.Empty;         //原责任人邮件地址

                List<string> emailAddrList = m_TrackInfoDA.GetEmailAddressByResponsibleUserName(entity.ResponsibleUserName);
                List<string> orgEmailAddrList = m_TrackInfoDA.GetEmailAddressByResponsibleUserName(trackingInfo.ResponsibleUserName);

                //从逾期未付款修改责任人
                if (string.IsNullOrEmpty(emailAddr))
                {
                    emailAddress = string.Join(";", this.CheckEmailAddress(emailAddrList));
                }
                else//从责任人配置修改责任人
                {
                    emailAddress = emailAddr;
                }

                orgEmail = string.Join(";", this.CheckEmailAddress(orgEmailAddrList));
                fromEmail = m_TrackInfoDA.GetEmailAddressByUserSysNo(ServiceContext.Current.UserSysNo);
                ccEmailAddress = string.Join(";", this.CheckEmailAddress(new List<string>() { fromEmail }));
                //记录操作日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("TrackingInfo_Log_ChangeResponsibleUserName", ServiceContext.Current.UserSysNo, entity.SysNo, entity.Note)
                    , BizLogType.Invoice_TrackingInfo_ChangeResponsibleUserName
                    , entity.SysNo.Value
                    , entity.CompanyCode);

                KeyValueVariables replaceVariables = new KeyValueVariables();
                replaceVariables.Add("Year", DateTime.Now.Year);
                replaceVariables.Add("Month", DateTime.Now.Month);
                replaceVariables.Add("Day", DateTime.Now.Day);
                replaceVariables.Add("UserSysNo", ServiceContext.Current.UserSysNo);
                replaceVariables.Add("OrderSysNo", entity.OrderSysNo);
                replaceVariables.Add("FromEmail", fromEmail);
                EmailHelper.SendEmailByTemplate(emailAddress, "ResponsibleUser_Change_Notify", replaceVariables, true);
            }
        }

        #endregion ResponsibleUser

        #region Private Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            string msg = GetMessageString(msgKeyName, args);
            throw new BizException(msg);
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.TrackingInfo, msgKeyName), args);
        }

        /// <summary>
        /// 如果配置的邮件地址有误，则将有误的邮件地址移除
        /// </summary>
        /// <param name="mailAddressList"></param>
        /// <returns></returns>
        private List<string> CheckEmailAddress(List<string> mailAddressList)
        {
            return mailAddressList.Where(w => StringUtility.IsEmailAddress(w)).Select(s => s).ToList();
        }

        #endregion Private Helper Methods
    }
}