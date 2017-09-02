using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(ICurrencyDA))]
    class CurrencyDA : ICurrencyDA
    {
        [Caching(ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public List<CurrencyInfo> QueryCurrencyList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryCurrencyList");
            return cmd.ExecuteEntityList<CurrencyInfo>();
        }

        /// <summary>
        /// 创建币种
        /// </summary>
        /// <param name="currencyInfo"></param>
        /// <returns></returns>
        public CurrencyInfo Create(CurrencyInfo currencyInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCurrency");
            cmd.SetParameterValue("@CurrencyName", currencyInfo.CurrencyName);
            cmd.SetParameterValue("@CurrencySymbol", currencyInfo.CurrencySymbol);
            cmd.SetParameterValue("@IsLocal", currencyInfo.IsLocal);
            cmd.SetParameterValue("@ExchangeRate", currencyInfo.ExchangeRate);
            cmd.SetParameterValue("@ListOrder", currencyInfo.ListOrder);
            cmd.SetParameterValue("@Status", currencyInfo.Status);
            
            cmd.ExecuteNonQuery();
            currencyInfo.SysNo = (int)cmd.GetParameterValue("@SysNo");
            //currencyInfo.CurrencyName = cmd.GetParameterValue("@CurrencyName").ToString();
            return currencyInfo;
        }

        /// <summary>
        /// 更新币种
        /// </summary>
        /// <param name="areaInfo"></param>
        /// <returns></returns>
        public CurrencyInfo Update(CurrencyInfo currencyInfo)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateCurrency");
            string sql = cmd.CommandText;
            cmd.SetParameterValue("@SysNo", currencyInfo.SysNo);
            cmd.SetParameterValue("@CurrencyName", currencyInfo.CurrencyName);
            cmd.SetParameterValue("@CurrencySymbol", currencyInfo.CurrencySymbol);
            cmd.SetParameterValue("@ExchangeRate", currencyInfo.ExchangeRate);
            cmd.SetParameterValue("@IsLocal", currencyInfo.IsLocal);
            cmd.SetParameterValue("@Status", currencyInfo.Status);
            cmd.SetParameterValue("@ListOrder", currencyInfo.ListOrder);
            cmd.ExecuteNonQuery();
            return currencyInfo;
        }

        /// <summary>
        /// Load加载币种详情
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        [Caching(new string[] { "sysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public CurrencyInfo Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCurrency");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                CurrencyInfo currency = DataMapper.GetEntity<CurrencyInfo>(row);
                return currency;
            }
            return null;
        }
    }
}
