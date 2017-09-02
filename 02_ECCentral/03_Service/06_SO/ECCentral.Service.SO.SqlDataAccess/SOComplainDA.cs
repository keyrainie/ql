using System;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using System.Collections.Generic;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.SO;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOComplainDA))]
    public class SOComplainDA : ISOComplainDA
    {
        #region ISOComplainDA Members

        /// <summary>
        /// 添加投诉信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public SOComplaintInfo InsertComplainMaster(SOComplaintInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateComplainMaster");
            command.SetParameterValue("@CompanyCode", info.ComplaintCotentInfo.CompanyCode);
            command.SetParameterValue("@SOSysNo", info.ComplaintCotentInfo.SOSysNo);
            command.SetParameterValue("@ComplainType", info.ComplaintCotentInfo.ComplainType);
            command.SetParameterValue("@ComplainSourceType",info.ComplaintCotentInfo.ComplainSourceType);
            command.SetParameterValue("@Subject", info.ComplaintCotentInfo.Subject);
            command.SetParameterValue("@ComplainContent", info.ComplaintCotentInfo.ComplainContent);
            command.SetParameterValue("@CustomerEmail", info.ComplaintCotentInfo.CustomerEmail);
            command.SetParameterValue("@CustomerPhone", info.ComplaintCotentInfo.CustomerPhone);
            command.SetParameterValue("@ReplyContent", info.ProcessInfo.ReplyContent);
            command.SetParameterValue("@CreateCustomerSysNo", info.ComplaintCotentInfo.CustomerSysNo);
            command.SetParameterValue("@CreateDate", info.ComplaintCotentInfo.ComplainTime);
            command.SetParameterValue("@Status", info.ProcessInfo.Status);
            command.SetParameterValue("@ReplySourceType", info.ProcessInfo.ReplyType);
            command.SetParameterValue("@ResponsibleDept", info.ProcessInfo.ResponsibleDepartment);
            command.SetParameterValue("@ComplainNote", info.ProcessInfo.ComplainNote);
            command.SetParameterValue("@AssignerSysNo", info.ProcessInfo.AssignerSysNo);
            command.SetParameterValue("@AssignDate", info.ProcessInfo.AssignDate);
            command.SetParameterValue("@OperatorSysNo", info.ProcessInfo.OperatorSysNo);
            command.SetParameterValue("@CSConfirmComplainType", info.ProcessInfo.CSConfirmComplainType);
            command.SetParameterValue("@CSConfirmComplainTypeDetail", info.ProcessInfo.CSConfirmComplainTypeDetail);
            command.SetParameterValue("@ResponsibleUser", info.ProcessInfo.ResponsibleUser);
            command.ExecuteNonQuery();
            info.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));
            return info;
        }

        /// <summary>
        /// 更新投诉信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public SOComplaintInfo UpdateComplainMaster(SOComplaintInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateComplain_Master");          
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@SOSysNo", info.ComplaintCotentInfo.SOSysNo);
            command.SetParameterValue("@ComplainType", info.ComplaintCotentInfo.ComplainType);
            command.SetParameterValue("@ComplainSourceType", info.ComplaintCotentInfo.ComplainSourceType);
            command.SetParameterValue("@Subject", info.ComplaintCotentInfo.Subject);
            command.SetParameterValue("@ComplainContent", info.ComplaintCotentInfo.ComplainContent);
            command.SetParameterValue("@CustomerEmail", info.ComplaintCotentInfo.CustomerEmail);
            command.SetParameterValue("@CustomerPhone", info.ComplaintCotentInfo.CustomerPhone);
            command.SetParameterValue("@ReplyContent", info.ProcessInfo.ReplyContent);         
            command.SetParameterValue("@Status", info.ProcessInfo.Status);
            command.SetParameterValue("@ReplySourceType", info.ProcessInfo.ReplyType);
            command.SetParameterValue("@ResponsibleDept", info.ProcessInfo.ResponsibleDepartment);
            command.SetParameterValue("@ProductID", info.ProcessInfo.ProductID);
            command.SetParameterValue("@DomainSysNo", info.ProcessInfo.DomainSysNo);            
            command.SetParameterValue("@ComplainNote", info.ProcessInfo.ComplainNote);
            command.SetParameterValue("@AssignerSysNo", info.ProcessInfo.AssignerSysNo);
            command.SetParameterValue("@AssignDate", info.ProcessInfo.AssignDate);
            command.SetParameterValue("@OperatorSysNo", info.ProcessInfo.OperatorSysNo);
            command.SetParameterValue("@CSConfirmComplainType", info.ProcessInfo.CSConfirmComplainType);
            command.SetParameterValue("@CSConfirmComplainTypeDetail", info.ProcessInfo.CSConfirmComplainTypeDetail);
            command.SetParameterValue("@ResponsibleUser", info.ProcessInfo.ResponsibleUser);

            command.SetParameterValue("@HistoryContent", info.ProcessInfo.ReplyContent);
            command.SetParameterValue("@SpendHours", info.ProcessInfo.SpendHours);
            if (info.ProcessInfo.IsSure.HasValue)
            {
                command.SetParameterValue("@DutyIdentification", info.ProcessInfo.IsSure.Value ? 'Y' : 'N');
            }
            else
            {
                command.SetParameterValue("@DutyIdentification", DBNull.Value);
            }
            command.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);

            command.SetParameterValue("@OrderType", info.ProcessInfo.OrderType);
            command.SetParameterValue("@ExtensionFlag", info.ProcessInfo.ExtensionFlag);
            command.SetParameterValue("@ReopenCount", info.ProcessInfo.ReopenCount);

            if (info.ProcessInfo.Status == SOComplainStatus.Complete)
            {
                command.SetParameterValue("@UpdateDate", info.ProcessInfo.ProcessTime);//投诉处理时间
            }
            else
            {
                command.SetParameterValue("@UpdateDate", System.DBNull.Value);
            }
            command.SetParameterValue("@ReasonCodeSysNo", info.ProcessInfo.ReasonCodeSysNo);
            command.ExecuteNonQuery();
            info.SysNo = Convert.ToInt32(command.GetParameterValue("@SysNo"));
            return info;
        }

        /// <summary>
        /// 指派投诉
        /// </summary>
        /// <param name="info"></param>
        public void Update_AssignInfo(SOComplaintProcessInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Update_ComplainAssignInfo");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@AssignerSysNo", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@AssignDate", info.AssignDate);
            command.SetParameterValue("@OperatorSysNo", info.OperatorSysNo);
            command.SetParameterValue("@UpdateDate", DateTime.Now);
            command.ExecuteNonQuery();         
        }

        /// <summary>
        /// 是否已存在相同的Case
        /// </summary>
        /// <param name="info">投诉内容实体</param>
        /// <param name="oldComplainID">已存在的相关投诉</param>
        /// <returns>如果存在返回真，否则返回假</returns>
        public bool IsSameCaseExist(SOComplaintCotentInfo info, ref object oldComplainID)
        {
            // 距当前时间一个工作日的时间点
            DateTime tmpStarttime = CommonUtility.AddWorkMinute(DateTime.Now, -1 * 60 * 9);
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("IsSameComplainCaseExist");

            cmd.AddInputParameter("@SOSysNo",DbType.Int32, info.SOSysNo);
            cmd.AddInputParameter("@CustomerSysno", DbType.Int32, info.CustomerSysNo);
            cmd.AddInputParameter("@Subject",DbType.String, info.Subject.Trim());
            cmd.AddInputParameter("@StartTime",DbType.DateTime, tmpStarttime);
            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, info.CompanyCode);
            oldComplainID = cmd.ExecuteScalar();
            return oldComplainID != null;
        }

        /// <summary>
        /// 获取历史记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<SOComplaintReplyInfo> GetHistory(SOComplaintInfo info)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_ComplainHistory");
            cmd.AddInputParameter("@ComplainSysNo", DbType.Int32, info.SysNo.Value);
            return cmd.ExecuteEntityList<SOComplaintReplyInfo>();
        }

        public void InsertHistory(SOComplaintReplyInfo info)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertComplainHistory");
            command.AddInputParameter("@SysNo",DbType.Int32, info.ComplainSysNo);
            command.AddInputParameter("@HistoryContent",DbType.String, info.HistoryContent);
            command.AddInputParameter("@CreateUserSysNo",DbType.Int32, ServiceContext.Current.UserSysNo);
            command.AddInputParameter("@ReplyContent",DbType.String, info.ReplyContent);
            command.AddInputParameter("@Status",DbType.Int32, info.Status);
            command.AddInputParameter("@ReplySourceType",DbType.String, info.ReplyType);
            command.ExecuteNonQuery();    
        }

        public SOComplaintInfo GetBySysNo(int sysNo)
        {
            SOComplaintInfo info = new SOComplaintInfo();

            //加载主实体
            var c = new ComplainQueryFilter();
            c.PagingInfo = new PagingInfo();
            c.SystemNumber = sysNo;
            //无需过滤无效订单号
            c.ValidCase = false;

            int count = 0;
            var dt = ObjectFactory<ISOQueryDA>.Instance.ComplainQuery(c, out count,false);
            if (count == 1)
            {
                //映射订单投诉内容信息
                info.ComplaintCotentInfo = DataMapper.GetEntity<SOComplaintCotentInfo>(dt.Rows[0], true, true, (o, e) => {
                    e.ComplainTime = Convert.ToDateTime(o["CreateDate"]);
                });
                info.SysNo = info.ComplaintCotentInfo.SysNo;

                //映射订单投诉处理信息
                info.ProcessInfo = DataMapper.GetEntity<SOComplaintProcessInfo>(dt.Rows[0], true, true, (o, e) =>
                {
                    if (o["DutyIdentification"].ToString().Length > 0)
                    {
                        e.IsSure = o["DutyIdentification"].ToString() == "Y" ? true : false;
                    }
                });
            }
            else
            {
                return null;
            }

            //加载历史回复
            info.ReplyHistory = GetHistory(info);

            return info;
        }

        public void UpdateCompainOperatorUser(SOComplaintProcessInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCompainOperatorUser");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@UpdateDate", DateTime.Now);
            command.SetParameterValue("@OperatorSysNo", entity.OperatorSysNo);
            command.ExecuteNonQuery();
        }

        #endregion
    }
}