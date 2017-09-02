using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService
{
      [VersionExport(typeof(ProductLineAppService))]
   public class ProductLineAppService
    {
          private ProductLineProcessor processor = ObjectFactory<ProductLineProcessor>.Instance;
          /// <summary>
          /// 创建产品线信息
          /// </summary>
          /// <param name="info"></param>
          public void CreateProductLine(ProductLineInfo info)
          {
              processor.CreateProductLine(info);
          }
          /// <summary>
          /// 更新产品线信息
          /// </summary>
          /// <param name="info"></param>
          public void UpdateProductLine(ProductLineInfo info)
          {
              processor.UpdateProductLine(info);
          }

          /// <summary>
          /// 删除产品线信息
          /// </summary>
          /// <param name="SysNo"></param>
          public void DeleteProductLine(int SysNo)
          {
              processor.DeleteProductLine(SysNo);
          }
    }
}
