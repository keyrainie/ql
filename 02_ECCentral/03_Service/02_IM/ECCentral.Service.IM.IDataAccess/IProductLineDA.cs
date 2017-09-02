using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using System.Data;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductLineDA
    {
        ProductLineInfo GetProductLine(int sysno);

        ProductLineInfo Create(ProductLineInfo entity);

        ProductLineInfo Update(ProductLineInfo entity);

        void CreateChangePool(int linesysno,int? c2sysno,int? brandsysno);

        void Delete(int sysNo);

        void DeleteByPMUserSysNo(int pmusersysno);

        bool IsExists(int c2sysno,int? brandsysno);

        bool HasSameChange(int linesysno, int pmusersysno);

        bool BrandIsUsing(int brandsysno);

        bool ManufacturerIsUsing(int manufacturersysno);

        bool Category2IsUsing(int csysno,int categorytype);

        bool PMIsUsing(int pmusersysno);

        void DeleteByBrand(int brandsysno);

        void DeleteByCategory(int csysno, int categorytype);

        void DeleteByManufacturer(int manufacturersysno);

        bool C2IsValid(int c2sysno);

        bool BrandIsValid(int brandsysno);

        bool PMIsValid(int pmusersysno);

        bool IsProductLosePM(int c2sysno, int? brandsysno);

        ProductManagerInfo GetPMByC3(int c3SysNo, int brandSysNo);

        ProductLineInfo GetPMByProductSysNo(int productsysno);

        bool HasRightByPMUser(ProductLineInfo entity);

        bool HasProduct(int? c2sysno, int? brandsysno);

        List<ProductLineInfo> GetProductLineByPMSysNo(int pmusersysno);

        List<ProductLineInfo> GetByBakPMUserSysNo(int pmusersysno);
    }
}
