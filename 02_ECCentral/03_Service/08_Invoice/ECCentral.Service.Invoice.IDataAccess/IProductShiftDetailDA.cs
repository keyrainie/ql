using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IProductShiftDetailDA
    {
        int InsertProductShiftDetails(List<ProductShiftDetailEntity> listProductShiftDetails,string groubText);

        List<ProductShiftDetailEntity> GetProductShiftDetail(ProductShiftDetailQueryEntity productShiftDetailQueryEntity);

        List<SysConfigEntity> GetSysysProductList(int? stockA);

        void WriteLog(GoldenTaxInvoiceLogEntity goldenTaxLog);

        ProductShiftDetailEntity GetProductShiftDetail(int shiftSysno, int productSysno);

        void InsertProductShiftCompany(List<ProductShiftDetailEntity> productList, string GoldenTaxNo, int createUser);
    }
}
