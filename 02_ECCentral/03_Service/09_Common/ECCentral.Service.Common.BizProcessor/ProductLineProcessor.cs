using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(ProductLineProcessor))]
    public class ProductLineProcessor
    {
        public virtual bool CheckOperateRightForCurrentUser(int productSysNo, int pmSysNo)
        {
            return ObjectFactory<IProductLineDA>.Instance.CheckOperateRightForCurrentUser(productSysNo, pmSysNo);
        }

          public virtual List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo)
          {
              List<ProductPMLine> tList = ObjectFactory<IProductLineDA>.Instance.GetProductLineSysNoByProductList(productSysNo);
              if (productSysNo.Length != tList.Count)
              {
                  productSysNo.ForEach(item =>
                  {
                      if (tList.SingleOrDefault(x => x.ProductSysNo == item) == null)
                          tList.Add(new ProductPMLine() { ProductSysNo = item });
                  });
              }
              return tList;
          }

          public List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo)
          {
              return ObjectFactory<IProductLineDA>.Instance.GetProductLineInfoByPM(pmSysNo);
          }
    }
}
