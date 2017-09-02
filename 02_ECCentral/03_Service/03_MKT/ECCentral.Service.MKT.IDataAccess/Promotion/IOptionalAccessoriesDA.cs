using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IOptionalAccessoriesDA
    {
        OptionalAccessoriesInfo Load(int sysNo);

        int CreateMaster(OptionalAccessoriesInfo info);

        void UpdateMaster(OptionalAccessoriesInfo info);

        void UpdateStatus(int? sysNo, ComboStatus targetStatus);

        int AddOptionalAccessoriesItem(OptionalAccessoriesItem item);

        void DeleteOptionalAccessoriesAllItem(int optionalAccessoriesSysNo);

        List<OptionalAccessoriesItem> GetOptionalAccessoriesItemListByOASysNo(int oaSysNo);

        List<OptionalAccessoriesItem> GetOptionalAccessoriesItemListByProductSysNo(int productSysNo);

        List<OptionalAccessoriesItem> GetOptionalAccessoriesItemListByProductSysNo(int productSysNo, int oaiSysNo);
    }
}
