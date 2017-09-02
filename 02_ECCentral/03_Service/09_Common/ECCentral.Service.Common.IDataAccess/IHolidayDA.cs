using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IHolidayDA
    {
        /// <summary>
        /// 根据服务标签获取节假日时间列表
        /// </summary>
        /// <param name="blockedService">服务标签</param>
        /// <param name="CompanyCode">公司编码</param>
        /// <returns>节假日时间列表</returns>
        List<DateTime> GetHolidayList(string blockedService, string companyCode);

        /// <summary>
        /// 获取所有今天以及今天以后的节假日
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>节假日列表</returns>
        List<Holiday> GetHolidaysAfterToday(string companyCode);

        Holiday Create(Holiday entity);

        void Delete(int sysNo);

        /// <summary>
        /// 特殊获取记录列表；用于创建业务中的特殊重复验证
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        List<Holiday> GetHolidaysByEntity(int? shipTypeSysNo, DateTime? holidayDate);
    }
}
