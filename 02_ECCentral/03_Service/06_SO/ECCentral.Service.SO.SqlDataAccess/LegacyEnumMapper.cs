using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.SO.SqlDataAccess
{
    public static class LegacyEnumMapper
    {
        /// <summary>
        /// 转商品类型为int
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int? ConvertSYNStatus(SYNStatus? p)
        {
            int ? result = new Nullable<int>();
            if (p.HasValue)
            {
                switch (p)
                {
                    case SYNStatus.Yes:
                        result = 1;
                        break;
                    case SYNStatus.No:
                        result = 0;
                        break;
                }
            }
            return result;
        }

        public static string ConvertSOProductActivityType(SOProductActivityType? p)
        {
            string result = null;

            if (p.HasValue)
            {
                switch (p)
                {
                    case SOProductActivityType.E_Promotion:
                        result = "E";
                        break;
                    case SOProductActivityType.GroupBuy:
                        result = "G";
                        break;
                    case SOProductActivityType.SpecialAccount:
                        result = "C";
                        break;
                }
            }

            return result;
        }

        public static string ConvertSettlementStatus(SettlementStatus? p)
        {
            string result = null;

            if (p.HasValue)
            {
                switch (p)
                {
                    case SettlementStatus.Fail:
                        result = "F";
                        break;
                    case SettlementStatus.PlanFail:
                        result = "P";
                        break;
                    case SettlementStatus.Success:
                        result = "S";
                        break;
                }
            }

            return result;
        }
    }
}
