using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IShiftRequestMemoDA))]
    public class ShiftRequestMemoDA : IShiftRequestMemoDA
    {
        #region 移仓单日志维护

        /// <summary>
        /// 根据SysNo获取跟进日志信息
        /// </summary>
        /// <param name="memoSysNo"></param>
        /// <returns></returns>
        public virtual ShiftRequestMemoInfo GetShiftRequestMemoInfoBySysNo(int memoSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetShiftRequestMemoBySysNo");
            dc.SetParameterValue("@MemoSysNo", memoSysNo);
            return dc.ExecuteEntity<ShiftRequestMemoInfo>();
        }

        /// <summary>
        /// 根据RequestSysNo获取跟进日志信息列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        public virtual List<ShiftRequestMemoInfo> GetShiftRequestMemoInfoListByRequestSysNo(int requestSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetShiftRequestMemoListByRequestSysNo");
            dc.SetParameterValue("@RequestSysNo", requestSysNo);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<ShiftRequestMemoInfo, List<ShiftRequestMemoInfo>>(reader);
                return list;
            }
        }

        /// <summary>
        /// 创建跟进日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestMemoInfo CreateShiftRequestMemo(ShiftRequestMemoInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_CreateShiftRequestMemo");

            command.SetParameterValue("@RequestSysNo", entity.RequestSysNo);
            command.SetParameterValue("@CreateUserSysNo", entity.CreateUser.SysNo);
            command.SetParameterValue("@CreateDate", entity.CreateDate);
            command.SetParameterValue("@Content",  entity.Content);
            command.SetParameterValue("@MemoStatus",  (int)entity.MemoStatus);
            command.SetParameterValue("@RemindTime",  entity.RemindTime);
            command.SetParameterValue("@EditDate",  entity.EditDate);            

            return command.ExecuteEntity<ShiftRequestMemoInfo>();
        }

        /// <summary>
        /// 更新跟进日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual ShiftRequestMemoInfo UpdateShiftRequestMemo(ShiftRequestMemoInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("Inventory_UpdateShiftRequestMemo");

            command.SetParameterValue("@MemoSysNo", entity.SysNo);
            command.SetParameterValue("@EditDate", entity.EditDate);
            command.SetParameterValue("@EditUserSysNo", entity.EditUser.SysNo);
            command.SetParameterValue("@MemoStatus", entity.MemoStatus);
            command.SetParameterValue("@Note", entity.Note);

            return command.ExecuteEntity<ShiftRequestMemoInfo>();
        }

        #endregion 移仓单日志维护
    }
}
