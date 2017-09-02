using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Common;
using ECommerce.DataAccess;
using ECommerce.Enums;
using ECommerce.Entity.Store.Vendor;

namespace ECommerce.Service.Common
{
    public class CommonService
    {

        /// <summary>
        /// 构造缓存的Key
        /// </summary>
        /// <param name="baseKey"></param>
        /// <param name="paramlist"></param>
        /// <returns></returns>
        public static string GenerateKey(string baseKey, params string[] paramlist)
        {
            string key = baseKey;
            if (paramlist != null && paramlist.Length > 0)
            {
                foreach (string param in paramlist)
                {
                    key += "_" + param;
                }
            }
            return key;
        }


        public static ECConfig GetECConfig(string key)
        {
            return null;
        }
        /// <summary>
        /// 获取所有省份
        /// </summary>
        /// <returns></returns>
        public static List<AreaInfo> GetAllProvince()
        {
            AreaInfoQueryFilter queryFilter = new AreaInfoQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                OnlyProvince = true,
            };
            return CommonDA.QueryArea(queryFilter).ResultList;
        }
        /// <summary>
        /// 获取省份下面所有城市
        /// </summary>
        /// <param name="provinceSysNo"></param>
        /// <returns></returns>
        public static List<AreaInfo> GetAllCity(string provinceSysNo)
        {
            AreaInfoQueryFilter queryFilter = new AreaInfoQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                OnlyCity = true,
                ProvinceSysNo = provinceSysNo,
            };
            return CommonDA.QueryArea(queryFilter).ResultList;
        }
        /// <summary>
        /// 获取城市下面所有区域
        /// </summary>
        /// <param name="citySysNo"></param>
        /// <returns></returns>
        public static List<AreaInfo> GetAllDistrict(int citySysNo)
        {
            AreaInfoQueryFilter queryFilter = new AreaInfoQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                CitySysNo = citySysNo,
            };
            return CommonDA.QueryArea(queryFilter).ResultList;
        }
        /// <summary>
        /// 加载一个区域信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static AreaInfo LoadArea(int sysNo)
        {
            AreaInfoQueryFilter queryFilter = new AreaInfoQueryFilter()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                DistrictSysNo = sysNo,
            };
            return CommonDA.QueryArea(queryFilter).ResultList.FirstOrDefault();
        }

        public static QueryResult<AreaInfo> QueryArea(AreaInfoQueryFilter queryFilter)
        {
            return CommonDA.QueryArea(queryFilter);
        }

        public static bool SendEmail(AsyncEmail email)
        {
            return CommonDA.SendEmail(email);
        }

        public static List<CurrencyInfo> GetCurrencyList()
        {
            return CommonDA.GetCurrencyList();
        }

        public static CurrencyInfo GetCurrencyBySysNo(int sysNo)
        {
            List<CurrencyInfo> list = GetCurrencyList();
            if (list != null)
            {
                return list.Find(x => x.SysNo.Value == sysNo);
            }
            return null;
        }

        public static QueryResult QueryCustomers(CustomerQueryFilter queryFilter)
        {
            return CommonDA.QueryCustomers(queryFilter);
        }

        public static QueryResult<BrandInfo> QueryBrandList(BrandQueryFilter queryFilter)
        {
            return CommonDA.QueryBrandList(queryFilter);
        }

        public static QueryResult<ManufacturerInfo> QueryManufacturerList(ManufacturerQueryFilter queryFilter)
        {
            return CommonDA.QueryManufacturerList(queryFilter);
        }

        /// <summary>
        /// 生成商检号码
        /// </summary>
        /// <returns></returns>
        public static string GenerateInspectionNo()
        {
            //TODO 根据泰隆优选提供的规则生成

            return Guid.NewGuid().ToString();
        }

        public static List<PayTypeInfo> GetAllPayType()
        {
            return CommonDA.GetAllPayType();
        }

        public static ShippingType GetShippingTypeBySysNo(int shipTypeSysNo)
        {
            return CommonDA.GetShippingTypeBySysNo(shipTypeSysNo);
        }

        /// <summary>
        /// 根据用户ID获取vendor user cellphone
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static string GetVendorCellPhone(string customerID)
        {
            string vendorCellPhone = CommonDA.GetVendorCellPhone(customerID);
            return vendorCellPhone;
        }

    }
}
