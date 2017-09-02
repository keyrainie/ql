using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductBrandWarrantyDA
    {
         //获取品牌维护信息
        DataTable GetProductBrandWarrantyByQuery(ProductBrandWarrantyQueryFilter query, out int totalCount);
        //插入品牌维护
        int InsertBrandWarrantyInfo(ProductBrandWarrantyInfo productBrandWarranty);
        //更新品牌维护通过品牌和三级类别
        void UpdateBrandWarrantyInfoByBrandSysNoAndC3SysNo(ProductBrandWarrantyInfo productBrandWarranty);
        //更新品牌維護
        void UpdateBrandWarrantyInfoBySysNo(ProductBrandWarrantyInfo productBrandWarranty);
        //查询三级别类别
        List<ProductBrandWarrantyInfo> GetC3SysNo(ProductBrandWarrantyInfo productBrandWarranty);
        //获取所有品牌保修
        List<ProductBrandWarrantyInfo> GetBrandWarrantyInfoByAll();
        //删除品牌保修
        void DelBrandWarrantyInfoBySysNo(Int32 SysNo);

        ProductBrandWarrantyInfo GetAllowDeleteBrandWarranty(int c3sysno, int brandsysno);

        ProductBrandWarrantyInfo GetBrandWarranty(int c3sysno, int brandsysno);
    }
}
