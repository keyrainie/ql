using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductResourceDA
    {
        void DeleteProductResource(Int32 resourceSysNo, Int32 productCommonInfoSysNo);

        void InsertProductResource(ProductResourceForNewegg resource, Int32 productCommonInfoSysNo);

        void UpdateProductResource(ProductResourceForNewegg resource, Int32 productCommonInfoSysNo);

        List<ProductResourceForNewegg> GetNeweggProductResourceListByProductCommonInfoSysNo(int productCommonInfoSysNo);

        void UpdateResource(int resourceSysNo, int productGroupSysNo,UserInfo operationUser);

        int GetProductGroupInfoImageSysNoByFileName(string fileName);

    }
}
