using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IVendorManufacturerDA
    {
        DataTable QueryManufacturers(VendorQueryManufacturerFilter queryRequest, out int totalCount);


        DataTable QueryBrands(VendorQueryManufacturerBrandFilter queryRequest, out int totalCount);
    }
}
