using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IProductPriceCompareDA
    {
        //价格举报有效
        void UpdateProductPriceCompareValid(ProductPriceCompareEntity entity);

        //价格举报无效
        void UpdateProductPriceCompareInvalid(ProductPriceCompareEntity entity);

        //价格举报恢复
        void UpdateProductPriceCompareResetLinkShow(ProductPriceCompareEntity entity);

        ProductPriceCompareEntity Load(int sysNo);
    }
}
