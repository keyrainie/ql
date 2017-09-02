using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService
{
      [VersionExport(typeof(AdvertisingAppService))]
    public class AdvertisingAppService
    {
          private AdvertisingProcessor processor = ObjectFactory<AdvertisingProcessor>.Instance;

          public AdvertisingInfo Load(int? sysNo)
          {
              return processor.Load(sysNo);
          }

          /// <summary>
          /// 创建
          /// </summary>
          /// <param name="info"></param>
          public int? CreateAdvertising(AdvertisingInfo info)
          {
              AdvertisingInfo result = processor.CreateAdvertising(info);
              return result.SysNo;
          }
          /// <summary>
          /// 更新
          /// </summary>
          /// <param name="info"></param>
          public void UpdateAdvertising(AdvertisingInfo info)
          {
              processor.UpdateAdvertising(info);
          }

          /// <summary>
          /// 删除
          /// </summary>
          /// <param name="SysNo"></param>
          public void DeleteAdvertising(int SysNo)
          {
              processor.DeleteAdvertising(SysNo);
          }
    }
}
