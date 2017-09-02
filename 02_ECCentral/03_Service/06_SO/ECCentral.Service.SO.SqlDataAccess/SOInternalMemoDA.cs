using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOInternalMemoDA))]
    public class SOInternalMemoDA : ISOInternalMemoDA
    {
        /// <summary>
        /// 获取所有日志创建者列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "00:05:00")]
        public List<CSInfo> GetSOLogCreater(string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_AllCreateLogUser");
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, companyCode);
            return command.ExecuteEntityList<CSInfo>();
        }

        /// <summary>
        /// 获取所有日志更新者列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [Caching("local", new string[] { "companyCode" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "00:05:00")]
        public List<CSInfo> GetSOLogUpdate(string companyCode)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("SO_Query_AllUpdaterLogUser");
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, companyCode);
            return command.ExecuteEntityList<CSInfo>();
        }

        /// <summary>
        /// 添加订单跟进日志
        /// </summary>
        /// <param name="info"></param>  
        /// <param name="companyCode"></param>  
        public void AddSOInternalMemoInfo(SOInternalMemoInfo info, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateSO_InternalMemo");
            command.SetParameterValue("@SOSysNo", info.SOSysNo);
            command.SetParameterValue("@CreateUserSysNo", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@CreateTime", DateTime.Now);
            command.SetParameterValue("@SourceSysNo", info.SourceSysNo);
            command.SetParameterValue("@ReasonCodeSysNo", info.ReasonCodeSysNo);
            command.SetParameterValue("@Content", info.Content);
            command.SetParameterValue("@UpdateUserSysNo", DBNull.Value);
            command.SetParameterValue("@UpdateTime", DBNull.Value);
            command.SetParameterValue("@Status", info.Status);
            command.SetParameterValue("@RemindTime", info.RemindTime);
            command.SetParameterValue("@Note", info.Note);
            command.SetParameterValue("@CallType", DBNull.Value);
            command.SetParameterValue("@ResponsibleDepCode", DBNull.Value);
            command.SetParameterValue("@Importance", info.Importance);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新 订单更近日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void UpdateSOInternalMemoInfo(SOInternalMemoInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSO_InternalMemo");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@SOSysNo", info.SOSysNo);
            command.SetParameterValue("@Content", info.Content);
            command.SetParameterValue("@UpdateUserSysNo", info.OperatorSysNo);//处理人
            command.SetParameterValue("@Status", info.Status);
            command.SetParameterValue("@UpdateTime", DateTime.Now);
            command.SetParameterValue("@RemindTime", info.RemindTime);
            if (info.Status == SOInternalMemoStatus.Complete)
            {
                command.SetParameterValue("@RemindTime", DBNull.Value);
                command.SetParameterValue("@UpdateTime", DBNull.Value);
            }
            command.SetParameterValue("@Note", info.Note);
            command.SetParameterValue("@CallType", info.CallType);
            command.SetParameterValue("@ResponsibleDepCode", info.DepartmentCode);
            command.SetParameterValue("@Importance", info.Importance);
            command.ExecuteNonQuery();
        }
        /// <summary>
        ///派发 取消派发 订单更近日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Update_AssignInfo(SOInternalMemoInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Update_InternalMemoAssignInfo");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@UpdateUserSysNo", info.OperatorSysNo);//处理人           
            command.ExecuteNonQuery();
        }
        /// <summary>
        ///关闭 订单更近日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Update_StatusInfo(SOInternalMemoInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Update_InternalMemoStatusInfo");
            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@Status", info.Status);//处理人           
            command.SetParameterValue("@Note", info.Note);
            command.SetParameterValue("@UpdateTime", DateTime.Now);
            command.SetParameterValue("@UpdateUserSysNo", ServiceContext.Current.UserSysNo);
            command.SetParameterValue("@RemindTime", info.RemindTime);
            command.ExecuteNonQuery();
        }

        public SOInternalMemoInfo GetBySysNo(int sysNo)
        {
            SOInternalMemoQueryFilter queryFilter = new SOInternalMemoQueryFilter();
            queryFilter.SysNo = sysNo;
            int dataCount = 0;
            DataTable dt = ObjectFactory<ISOQueryDA>.Instance.InternalMemoQuery(queryFilter, out dataCount, false);
            if (dataCount == 1)
            {
                return DataMapper.GetEntity<SOInternalMemoInfo>(dt.Rows[0], true, true, (o, e) =>
                {
                    if (o["LastEditUserSysNo"] != DBNull.Value)
                    {
                        e.OperatorSysNo = Convert.ToInt32(o["LastEditUserSysNo"]);
                    }
                });
            }
            return null;
        }

        public List<SOInternalMemoInfo> GetBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSO_InternalMemo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<SOInternalMemoInfo>();
        }
    }
}
