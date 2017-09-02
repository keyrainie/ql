using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IAdvertisingDA
    {
        AdvertisingInfo Load(int sysNo);

        List<AdvertisingInfo> LoadByProductLineSysNoAndType(AdvertisingInfo entity);

       /// <summary>
       /// 创建
       /// </summary>
       /// <param name="info"></param>
        AdvertisingInfo CreateAdvertising(AdvertisingInfo info);

       /// <summary>
       /// 更新
       /// </summary>
       /// <param name="info"></param>
        void UpdateAdvertising(AdvertisingInfo info);


       /// <summary>
       /// 删除
       /// </summary>
       /// <param name="SysNo"></param>
        void DeleteAdvertising(int SysNo);

    }
}
