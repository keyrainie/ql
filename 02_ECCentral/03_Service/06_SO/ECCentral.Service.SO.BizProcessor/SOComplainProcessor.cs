using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.IDataAccess;
using System;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 订单投诉处理
    /// </summary>
    [VersionExport(typeof(SOComplainProcessor))]
    public class SOComplainProcessor
    {
        ISOComplainDA m_da = ObjectFactory<ISOComplainDA>.Instance;


        //添加投诉
        /// <summary>
        /// 添加投诉
        /// </summary>
        /// <param name="info">请求的实体</param>
        public virtual SOComplaintInfo Create(SOComplaintCotentInfo info)
        {
            if (info.Subject != null)
            {
                info.Subject = info.Subject.Trim();
            }
            SOInfo soEntity = null;
            if (info.SOSysNo.HasValue)
            {
                soEntity = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(info.SOSysNo.Value);
                if (null == soEntity)
                {
                    BizExceptionHelper.Throw("SO_SOIsNotExist");
                }
                info.CustomerSysNo = soEntity.BaseInfo.CustomerSysNo;
                //如果传入的邮箱和电话都是空的，需要获取用户邮箱和电话
                if (string.IsNullOrEmpty(info.CustomerPhone) && string.IsNullOrEmpty(info.CustomerEmail))
                {
                    CustomerInfo customerInfo = ExternalDomainBroker.GetCustomerInfo(info.CustomerSysNo.Value);
                    info.CustomerEmail = customerInfo.BasicInfo.Email;
                    info.CustomerPhone = customerInfo.BasicInfo.Phone;
                }
            }
            if (string.IsNullOrEmpty(info.ComplainSourceType))
            {
                info.ComplainSourceType = AppSettingHelper.ComplainSourceTypeDefault;
            }

            object oldComplainID = null;
            SOComplaintInfo soComplaintInfo = new SOComplaintInfo();
            soComplaintInfo.ComplaintCotentInfo = info;
            if (ObjectFactory<ISOComplainDA>.Instance.IsSameCaseExist(info, ref oldComplainID))
            {
                soComplaintInfo.ProcessInfo.Status = SOComplainStatus.Abandoned;
                soComplaintInfo.ProcessInfo.ComplainNote = ResourceHelper.Get("SO_Complain_SameOrder", oldComplainID);
            }
            if (!string.IsNullOrEmpty(soComplaintInfo.ComplaintCotentInfo.ComplainContent))
            {
                soComplaintInfo.ComplaintCotentInfo.ComplainContent = ResourceHelper.Get("SO_Complain_ContentFormat"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + "\r\n"
                                                                + soComplaintInfo.ComplaintCotentInfo.ComplainContent;
            }

            soComplaintInfo.ComplaintCotentInfo.ComplainTime = DateTime.Now;

            m_da.InsertComplainMaster(soComplaintInfo);

            AddHistory(soComplaintInfo, ReplyOperatorType.Add);

            return soComplaintInfo;
        }

        //添加回复历史记录
        /// <summary>
        /// 添加回复历史记录
        /// </summary>
        /// <param name="soComplaintInfo">投诉实体</param>
        /// <param name="isAdd">是否是添加,会影响history属性的赋值</param>
        private void AddHistory(SOComplaintInfo soComplaintInfo, ReplyOperatorType optType)
        {
            SOComplaintReplyInfo info = new SOComplaintReplyInfo();
            info.ComplainSysNo = soComplaintInfo.SysNo.Value;
            //以后的内容不同，最好还是分开写
            switch (optType)
            {
                case ReplyOperatorType.Add:
                    info.HistoryContent = ResourceHelper.Get("SO_Complain_CreateHistoryBy", ServiceContext.Current.UserSysNo, DateTime.Now);
                    break;
                case ReplyOperatorType.Update:
                    info.HistoryContent = ResourceHelper.Get("SO_Complain_UpdateHistoryBy", ServiceContext.Current.UserSysNo, DateTime.Now);
                    break;
                case ReplyOperatorType.SendMain:
                    info.HistoryContent = ResourceHelper.Get("SO_Complain_ReplyHistoryBy", ServiceContext.Current.UserSysNo, DateTime.Now);
                    break;
            }

            info.ReplyContent = soComplaintInfo.ProcessInfo.ReplyContent;
            info.ReplyType = soComplaintInfo.ProcessInfo.ReplyType;
            info.Status = soComplaintInfo.ProcessInfo.Status;
            m_da.InsertHistory(info);
        }

        //更新 订单投诉信息
        /// <summary>
        /// 更新 订单投诉信息
        /// </summary>
        /// <param name="info">请求的实体</param>
        /// <returns>修改后的订单</returns>
        public virtual SOComplaintInfo Update(SOComplaintInfo info)
        {
            if (!info.ProcessInfo.IsSure.HasValue
                && (info.ProcessInfo.Status == SOComplainStatus.Review
                    || info.ProcessInfo.Status == SOComplainStatus.Dealing
                    || info.ProcessInfo.Status == SOComplainStatus.Complete))
            {
                BizExceptionHelper.Throw("SO_Complain_UpdateAuditError");
            }

            if (info.ProcessInfo.ReasonCodeSysNo.HasValue)
            {
                var reasonPath = ExternalDomainBroker.GetReasonCodePath(info.ProcessInfo.ReasonCodeSysNo.Value, info.ComplaintCotentInfo.CompanyCode);
                //spilt reasonPath
                var reasons = reasonPath.Split('>');
                if (reasons.Length > 1)
                {
                    //构造投诉大类
                    info.ProcessInfo.CSConfirmComplainType = reasons[1];
                    if (reasons.Length > 2)
                    {
                        //构造投诉类别
                        info.ProcessInfo.CSConfirmComplainTypeDetail = reasons[2];
                        if (reasons.Length > 3)
                        {
                            info.ProcessInfo.ResponsibleDepartment = CodeNamePairManager.GetCode(SOConst.DomainName, "ResponsibleDept", reasons[3]);
                        }
                    }
                }
            }

            //处理情况加入到投诉内容中
            if (!string.IsNullOrEmpty(info.ProcessInfo.ProcessedNote))
            {
                info.ComplaintCotentInfo.ComplainContent = info.ComplaintCotentInfo.ComplainContent
                                                            + "\r\n\r\n"
                                                            + ResourceHelper.Get("SO_Complain_ContentFormat"
                                                                            , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                                                            )
                                                            + "\r\n"
                                                            + info.ProcessInfo.ProcessedNote.Trim();
                info.ProcessInfo.ProcessedNote = null;
            }

            //复核审查
            if (info.ProcessInfo.Status == SOComplainStatus.Review)
            {
                //如果是复核状态
                info.ProcessInfo.ReopenCount++;
            }
            else if (info.ProcessInfo.Status == SOComplainStatus.Complete)
            {
                //如果是处理完毕，更新所用工时
                double worktime = CommonUtility.CalWorkMinute(info.ComplaintCotentInfo.ComplainTime.Value, DateTime.Now, 0);
                info.ProcessInfo.SpendHours = Convert.ToInt32(worktime / 60);
            }

            info = m_da.UpdateComplainMaster(info);

            AddHistory(info, ReplyOperatorType.Update);
            //重新获取日志
            info.ReplyHistory = m_da.GetHistory(info);

            //处理完毕和作废的最终状态通知客户
            if (info.ProcessInfo.Status == SOComplainStatus.Abandoned
                || info.ProcessInfo.Status == SOComplainStatus.Complete)
            {
                //更新CallingStatus
                ExternalDomainBroker.CallingCustomStatus(info.SysNo.Value
                                                            , CallingReferenceType.Complain
                                                            , info.ProcessInfo.ComplainNote);
            }
            return info;
        }

        //批量派发投诉
        /// <summary>
        /// 批量派发投诉
        /// </summary>
        /// <param name="entity">待派发的实体集合</param>
        public virtual void BatchAssign(List<SOComplaintProcessInfo> infoList)
        {
            foreach (var item in infoList)
            {
                if (item.OperatorSysNo == 0)
                {
                    BizExceptionHelper.Throw("SO_AssignOperator_Unknow");
                }
                item.AssignerSysNo = ServiceContext.Current.UserSysNo;
                item.AssignDate = DateTime.Now;
                m_da.Update_AssignInfo(item);
            }
        }

        //批量撤销派发投诉
        /// <summary>
        /// 撤销派发投诉
        /// </summary>
        /// <param name="entity">待取消派发的实体集合</param>
        public virtual void BatchCancelAssign(List<SOComplaintProcessInfo> infoList)
        {
            foreach (var item in infoList)
            {
                item.OperatorSysNo = null;
                item.AssignDate = null;
                item.AssignerSysNo = null;
                m_da.Update_AssignInfo(item);
            }
        }

        //获取根据编号投诉
        /// <summary>
        /// 获取根据编号投诉
        /// </summary>
        /// <param name="sysNo">投诉的唯一编号</param>
        /// <returns>投诉实体</returns>
        public virtual SOComplaintInfo GetInfo(int sysNo)
        {
            SOComplaintInfo info = m_da.GetBySysNo(sysNo);

            if (info == null)
            {
                BizExceptionHelper.Throw("SO_Complain_IsNotExist");
            }

            return info;
        }

        //根据商品ID获取商品所属domain
        /// <summary>
        /// 根据商品ID获取商品所属domain
        /// </summary>
        /// <param name="productID">商品ID</param>
        /// <returns>商品所属domain</returns>
        public virtual ProductDomain GetProductDomain(string productID)
        {
            var info = ExternalDomainBroker.GetDomainByProductID(productID);
            if (!info.SysNo.HasValue)
            {
                BizExceptionHelper.Throw("SO_Complain_UnkownDomain");
            }
            return info;
        }

        /// <summary>
        /// 发送投诉邮件通知
        /// </summary>
        /// <param name="complainSysNo">投诉单号</param>
        public void SendMain(int complainSysNo)
        {
            var info = GetInfo(complainSysNo);
            //更新操作人
            info.ProcessInfo.OperatorSysNo = ServiceContext.Current.UserSysNo;
            m_da.UpdateCompainOperatorUser(info.ProcessInfo);

            //发送邮件
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("Title", info.ComplaintCotentInfo.Subject);
            vars.Add("SysNo", info.ComplaintCotentInfo.SysNo);
            vars.Add("Content", info.ProcessInfo.ReplyContent.Replace(Environment.NewLine, "<BR/>"));

            CustomerInfo customerInfo = null;
            if (info.ComplaintCotentInfo.CustomerSysNo.HasValue)
            {
                customerInfo = ExternalDomainBroker.GetCustomerInfo(info.ComplaintCotentInfo.CustomerSysNo.Value);
            }

            if (customerInfo != null)
            {
                ExternalDomainBroker.SendExternalEmail(customerInfo.BasicInfo.Email, "SO_ReplyComplainEmail", vars, customerInfo.BasicInfo.FavoriteLanguageCode);
            }
            //写入日志
            AddHistory(info, ReplyOperatorType.SendMain);

        }

        /// <summary>
        /// 投诉回复操作类型
        /// </summary>
        enum ReplyOperatorType
        {
            Add,
            Update,
            SendMain
        }
    }
}
