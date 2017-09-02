using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IProductLineDA
    {
        bool CheckOperateRightForCurrentUser(int productSysNo, int pmSysNo);
        List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo);
        List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo);
    }
}
