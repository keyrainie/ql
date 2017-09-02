using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECCentral.Service.IM.IDataAccess
{
  public interface IProductCommonSkuNumberConvertorDA
    {
      /// <summary>
      /// 根据ProductIDs得到CommonSkuNumbers
      /// </summary>
      /// <param name="list"></param>
      /// <returns></returns>
     DataTable GetCommonSkuNumbersByProductIDs(List<string> list);
      /// <summary>
      /// 根据CommonSkuNumbers得到ProductIDs
      /// </summary>
      /// <param name="list"></param>
      /// <returns></returns>
     DataTable GetProductIDsByCommonSkuNumbers(List<string> list);
    }
}
