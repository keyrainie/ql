using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.SqlDataAccess
{
    public static class LegacyEnumMapper
    {
        /// <summary>
        ///转商品类型为int
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int ConvertProductType(ProductType p)
        {
            int getProductType = 0;
            switch (p)
            {
                case ProductType.Normal:
                    getProductType = 0;
                    break;
                case ProductType.OpenBox:
                    getProductType = 1;
                    break;
                case ProductType.Bad:
                    getProductType = 2;
                    break;
                case ProductType.Virtual:
                    getProductType = 3;
                    break;
                default:
                    break;
            }
            return getProductType;
        }
    }
}
