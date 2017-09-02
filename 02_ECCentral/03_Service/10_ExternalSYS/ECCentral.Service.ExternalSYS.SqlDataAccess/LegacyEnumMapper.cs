using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    public static class LegacyEnumMapper
    {
        #region EIMS

        #endregion

        #region VendorPortal

        /// <summary>
        /// 转商品类型为string
        /// </summary>
        /// <param name="p">待转换的类型</param>
        /// <returns></returns>
        public static string ConvertValidStatus(ValidStatus? p)
        {
            string result = null;
            if (p.HasValue)
            {
                switch (p)
                {
                    case ValidStatus.Active:
                        result = "A";
                        break;
                    case ValidStatus.DeActive:
                        result = "D";
                        break;
                }
            }
            return result;
        }

        #endregion
    }
}
