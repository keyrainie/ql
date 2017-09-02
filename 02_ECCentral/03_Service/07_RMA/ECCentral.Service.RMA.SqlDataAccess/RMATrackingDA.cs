using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRMATrackingDA))]
    public class RMATrackingDA : IRMATrackingDA
    {
        #region IRMATrackingDA Members

        /// <summary>
        /// 派发RMA跟进单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Dispatch(int sysNo, int handlerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DispatchRMATracking");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@UpdateUserSysNo", handlerSysNo);
            return command.ExecuteNonQuery() == 0 ? false : true;
        }
        /// <summary>
        /// 取消派发RMA跟进单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool CancelDispatch(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CancelDispatchRMATracking");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteNonQuery() == 0 ? false : true;
        }

        public virtual void Close(InternalMemoInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CloseRMATracking");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValueAsCurrentUserSysNo("@UpdateUserSysNo");
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 创建跟进日志有效性检查---检查是否存在单件号码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool IsExistRegisterSysNo(int RegisterSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetNewTrackingByRegisterSysNo");
            command.SetParameterValue("@RegisterSysNo", RegisterSysNo);
            object o = command.ExecuteScalar();
            return (o != null && Convert.ToInt32(o) > 0) ? true : false;

        }
        /// <summary>
        /// 创建RMA跟进日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual InternalMemoInfo Create(InternalMemoInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateRMATracking");

            command.SetParameterValue("@RegisterSysNo", entity.RegisterSysNo);
            command.SetParameterValue("@Content", entity.Content);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@RemindTime", entity.RemindTime);
            command.SetParameterValue("@Note", entity.Note);

            if (entity.Status == InternalMemoStatus.Close)
            {
                command.SetParameterValue("@UpdateTime", DateTime.Now);
                command.SetParameterValue("@UpdateUserSysNo", ServiceContext.Current.UserSysNo);
            }
            else
            {
                command.SetParameterValue("@UpdateTime", DBNull.Value);
                command.SetParameterValue("@UpdateUserSysNo", DBNull.Value);
            }
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@DepartmentCode", entity.DepartmentCode);
            command.SetParameterValue("@Urgency", entity.Urgency);
            command.SetParameterValue("@ReasonCodeSysNo", entity.ReasonCodeSysNo);
            command.SetParameterValue("@SourceSysNo", entity.SourceSysNo);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.ExecuteNonQuery();
            return entity;
        }

        public InternalMemoInfo GetRMATrackingBySysNo(int sysNo) 
        {
            InternalMemoInfo result = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetRMATrackingBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            result = command.ExecuteEntity<InternalMemoInfo>();

            return result;
        }
        #endregion
    }
}
