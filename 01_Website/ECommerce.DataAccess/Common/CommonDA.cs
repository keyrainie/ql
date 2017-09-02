using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Common;
using System.Data;

namespace ECommerce.DataAccess.Common
{
    public class CommonDA
    {

        public static List<ProductStepPrice> GetProductStepPrice(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetProductStepPrice");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntityList<ProductStepPrice>();
        }

        /// <summary>
        /// 获取所有的省份
        /// </summary>
        /// <param name="regionSysNo"></param>
        /// <returns></returns>
        public static List<Area> GetAllProvince()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetAllProvice");
            return cmd.ExecuteEntityList<Area>();
        }

        /// <summary>
        /// 获取所有的省份下的城市
        /// </summary>
        /// <param name="proviceSysNo"></param>
        /// <returns></returns>
        public static List<Area> GetAllCity(int proviceSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetAllCity");
            cmd.SetParameterValue("ProvinceSysNo", proviceSysNo);
            return cmd.ExecuteEntityList<Area>();
        }

        /// <summary>
        /// 获取城市下所有的地区
        /// </summary>
        /// <returns></returns>
        public static List<Area> GetAllDistrict(int citySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetAllDistrict");
            cmd.SetParameterValue("@CitySysNo", citySysNo);
            return cmd.ExecuteEntityList<Area>();
        }

        public static Area GetAreaBySysNo(int sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetArea");
            cmd.SetParameterValue("@SysNo", sysno);
            return cmd.ExecuteEntity<Area>();
        }

        /// <summary>
        /// 创建短信
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool InsertNewSMS(SMSInfo item, out int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertNewSMS");
            item.CompanyCode = ConstValue.CompanyCode;
            item.StoreCompanyCode = ConstValue.StoreCompanyCode;
            item.LanguageCode = ConstValue.LanguageCode;
            cmd.SetParameterValue<SMSInfo>(item);
            int result = cmd.ExecuteNonQuery();
            sysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return result > 0 ? true : false;
        }

        public static bool InsertNewSMS(SMSInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertNewSMS");
            item.CompanyCode = ConstValue.CompanyCode;
            item.StoreCompanyCode = ConstValue.StoreCompanyCode;
            item.LanguageCode = ConstValue.LanguageCode;
            cmd.SetParameterValue<SMSInfo>(item);
            int result = cmd.ExecuteNonQuery();
            return result > 0 ? true : false;
        }

        /// <summary>
        /// 写系统Log
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool CreateApplicationEventLog(ApplicationEventLog entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_CreateApplicationEventLog");
            cmd.SetParameterValue<ApplicationEventLog>(entity);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 根据PageTypeID获取页面类型
        /// </summary>
        /// <param name="sysNo">编号</param>
        /// <returns></returns>
        public static PageType GetPageType(int pageTypeID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetPageType");
            cmd.SetParameterValue("@PageTypeID", pageTypeID);
            return cmd.ExecuteEntity<PageType>();
        }

        /// <summary>
        /// 添加积分
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static int AddPoint(Point point)
        {
            var cmd = DataCommandManager.GetDataCommand("AddPoint");
            cmd.SetParameterValue("@CustomerSysNo", point.CustomerSysNo);
            cmd.SetParameterValue("@Point", point.Points);
            cmd.SetParameterValue("@AvailablePoint", point.AvailablePoint);
            cmd.SetParameterValue("@ObtainType", point.ObtainType);
            cmd.SetParameterValue("@InDate", point.InDate);
            cmd.SetParameterValue("@InUser", point.InUser);
            cmd.SetParameterValue("@ExpireDate", point.ExpireDate);
            cmd.SetParameterValue("@Memo", point.Memo);
            cmd.SetParameterValue("@LanguageCode", point.LanguageCode);
            cmd.SetParameterValue("@CurrencyCode", point.CurrencyCode);
            cmd.SetParameterValue("@CompanyCode", point.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", point.StoreCompanyCode);
            cmd.SetParameterValue("@IsFromSysAccount", point.IsFromSysAccount);
            cmd.SetParameterValue("@SysAccount", point.SysAccount);

            var dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                var code = int.Parse(dt.Rows[0][0].ToString());
                if (code == 1000099)
                {
                    return 1;
                }
            }

            return 0;
        }


        public static int ExistsPoint(PointFilter filter)
        {
            var cmd = DataCommandManager.GetDataCommand("ExistsPoint");
            cmd.SetParameterValue("@CustomerSysNo", filter.CustomerSysNo);
            cmd.SetParameterValue("@ObtainType", filter.ObtainType);
            cmd.SetParameterValue("@LanguageCode", filter.LanguageCode);
            cmd.SetParameterValue("@CurrencyCode", filter.CurrencyCode);
            cmd.SetParameterValue("@CompanyCode", filter.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", filter.StoreCompanyCode);
            return cmd.ExecuteDataTable().Rows.Count;
        }

        /// <summary>
        /// 根据Key获取前台配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSysConfigByKey(string key)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Common_GetSysConfigByKey");
            cmd.SetParameterValue("@Key", key);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "0";
        }
    }
}
