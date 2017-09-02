using ECCentral.BizEntity.MKT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IECDynamicCategoryDA
    {
        ECDynamicCategory Create(ECDynamicCategory info);

        void Update(ECDynamicCategory info);

        void Delete(int sysNo);

        void DeleteCategoryMapping(int dynamicCategorySysNo);

        List<ECDynamicCategory> GetDynamicCategories(DynamicCategoryStatus? status, DynamicCategoryType? categoryType);

        bool CheckNameDuplicate(string name, int excludeSysNo, int level, string companyCode);

        bool CheckSubCategoryExists(int dynamicCategorySysNo);

        void InsertCategoryProductMapping(int dynamicCategorySysNo, int productSysNo);

        void DeleteCategoryProductMapping(int dynamicCategorySysNo, int productSysNo);

        bool ExistCategoryProductMapping(int dynamicCategorySysNo, int productSysNo);
    }
}
