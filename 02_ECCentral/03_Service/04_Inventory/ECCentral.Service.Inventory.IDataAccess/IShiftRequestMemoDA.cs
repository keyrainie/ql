using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IShiftRequestMemoDA
    {
        #region 移仓单日志维护

        /// <summary>
        /// 根据SysNo获取跟进日志信息
        /// </summary>
        /// <param name="memoSysNo"></param>
        /// <returns></returns>
        ShiftRequestMemoInfo GetShiftRequestMemoInfoBySysNo(int memoSysNo);

        /// <summary>
        /// 根据RequestSysNo获取跟进日志信息列表
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        List<ShiftRequestMemoInfo> GetShiftRequestMemoInfoListByRequestSysNo(int requestSysNo);

        /// <summary>
        /// 创建跟进日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ShiftRequestMemoInfo CreateShiftRequestMemo(ShiftRequestMemoInfo entity);

        /// <summary>
        /// 更新跟进日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ShiftRequestMemoInfo UpdateShiftRequestMemo(ShiftRequestMemoInfo entity);

        #endregion 移仓单日志维护
    }
}
