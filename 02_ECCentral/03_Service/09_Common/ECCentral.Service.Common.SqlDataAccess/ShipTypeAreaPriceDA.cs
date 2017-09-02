using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.Common.IDataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IShipTypeAreaPriceDA))]
  public  class ShipTypeAreaPriceDA:IShipTypeAreaPriceDA
    {
        /// <summary>
        /// 删除配送方式-地区-价格（非）
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public void VoidShipTypeAreaPrice(List<int> sysNoList)
        {
            if (sysNoList != null && sysNoList.Count > 0)
            {
                CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeAreaPrice_Void");
                cmd.CommandText = cmd.CommandText.Replace("@SysNo", String.Join(",", sysNoList));
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 删除配送方式-地区-价格（非）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ShipTypeAreaPriceInfo CreateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            if (entity != null)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypeAreaPrice_Create");
                cmd.SetParameterValue("@AreaSysNo", entity.AreaSysNo);
                cmd.SetParameterValue("@ShipTypeSysNo", entity.ShipTypeSysNo);
                cmd.SetParameterValue("@BaseWeight", entity.BaseWeight);
                cmd.SetParameterValue("@TopWeight", entity.TopWeight);
                cmd.SetParameterValue("@UnitWeight", entity.UnitWeight);
                cmd.SetParameterValue("@UnitPrice", entity.UnitPrice);
                cmd.SetParameterValue("@MaxPrice", entity.MaxPrice);
                cmd.SetParameterValue("@CompanyCode", "8601");
                cmd.SetParameterValue("@MerchantSysNo", entity.VendorSysNo);
                cmd.ExecuteNonQuery();
                entity.SysNo =(int?)cmd.GetParameterValue("@SysNO");
                return entity;
            }
            return null;
        }
        /// <summary>
        /// 更新配送方式-地区-价格（非）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void UpdateShipTypeAreaPrice(ShipTypeAreaPriceInfo entity)
        {
            if (entity != null)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypeAreaPrice_Update");
                cmd.SetParameterValue("@SysNo", entity.SysNo);
                cmd.SetParameterValue("@AreaSysNo", entity.AreaSysNo);
                cmd.SetParameterValue("@ShipTypeSysNo", entity.ShipTypeSysNo);
                cmd.SetParameterValue("@BaseWeight", entity.BaseWeight);
                cmd.SetParameterValue("@TopWeight", entity.TopWeight);
                cmd.SetParameterValue("@UnitWeight", entity.UnitWeight);
                cmd.SetParameterValue("@UnitPrice", entity.UnitPrice);
                cmd.SetParameterValue("@MaxPrice", entity.MaxPrice);
                cmd.SetParameterValue("@MerchantSysNo", entity.VendorSysNo);
                cmd.ExecuteNonQuery();

            }
        }
    }
}
