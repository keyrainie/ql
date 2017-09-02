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
    [VersionExport(typeof(IShipTypeProductDA))]
    public class ShipTypeProductDA : IShipTypeProductDA
    {
        /// <summary>
        /// 删除配送方式-产品
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <returns></returns>
        public void VoidShipTypeProduct(List<int?> sysNoList)
        {
            if (sysNoList != null && sysNoList.Count > 0)
            {
                CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeProduct_Void");
                cmd.CommandText = cmd.CommandText.Replace("@SysNo", String.Join(",", sysNoList));
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 添加配送方式-产品
        /// </summary>
        /// <param name="ShipTypeProductInfo"></param>
        /// <returns></returns>
        public void CreateShipTypeProduct(ShipTypeProductInfo ShipTypeProductInfo)
        {
            object obj = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypeProduct_Master_Create");
            cmd.SetParameterValue("@StockSysNo", ShipTypeProductInfo.WareHouse);
            cmd.SetParameterValue("@ShipTypeSysNo", ShipTypeProductInfo.ShippingType);
            if (!ShipTypeProductInfo.CitySysNo.HasValue && !ShipTypeProductInfo.ProvinceSysNo.HasValue)
            {
                cmd.SetParameterValue("@AreaSysNo", null);
            }
            if (!ShipTypeProductInfo.CitySysNo.HasValue && ShipTypeProductInfo.ProvinceSysNo.HasValue)
            {
                cmd.SetParameterValue("@AreaSysNo", ShipTypeProductInfo.ProvinceSysNo);
            }
            if (ShipTypeProductInfo.CitySysNo.HasValue && !ShipTypeProductInfo.DistrictSysNo.HasValue)
            {
                cmd.SetParameterValue("@AreaSysNo", ShipTypeProductInfo.CitySysNo);
            }
            if (ShipTypeProductInfo.DistrictSysNo.HasValue)
            {
                cmd.SetParameterValue("@AreaSysNo", ShipTypeProductInfo.AreaSysNo);
            }
            cmd.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@Status", "A");
            cmd.SetParameterValue("@CompanyCode", ShipTypeProductInfo.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ShipTypeProductInfo.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", "zh-CN");
            cmd.ExecuteNonQuery();
            ShipTypeProductInfo.SysNo = (int)cmd.GetParameterValue("@SysNo");
            CustomDataCommand cmd_item = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeProduct_Item_Create");
            StringBuilder builersql = new StringBuilder(); ;
            if (ShipTypeProductInfo.ListProductInfo == null && ShipTypeProductInfo.ListCategoryInfo == null)
            {
                return;
            }
            if (ShipTypeProductInfo.ListProductInfo != null && ShipTypeProductInfo.ListCategoryInfo == null)
            {
                for (int i = 0; i < ShipTypeProductInfo.ListProductInfo.Count; i++)
                {

                    cmd_item.SetParameterValue("@MasterSysNo", ShipTypeProductInfo.SysNo);

                    cmd_item.SetParameterValue("@CompanyCode", ShipTypeProductInfo.CompanyCode);
                    cmd_item.SetParameterValue("@Status", "A");
                    cmd_item.SetParameterValue("@Description", ShipTypeProductInfo.Description);
                    if (EnumCodeMapper.TryGetCode(ShipTypeProductInfo.ShipTypeProductType, out obj))
                    {
                        cmd_item.SetParameterValue("@Type", obj);
                    }
                    if (EnumCodeMapper.TryGetCode(ShipTypeProductInfo.ProductRange, out obj))
                    {
                        cmd_item.SetParameterValue("@ItemRange", obj);
                    }
                    cmd_item.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
                    cmd_item.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);
                    cmd_item.SetParameterValue("@StoreCompanyCode", ShipTypeProductInfo.CompanyCode);
                    cmd_item.SetParameterValue("@LanguageCode", "zh-CN");
                    cmd_item.SetParameterValue("@CompanyCustomer", 0);
                    builersql.Append(cmd_item.CommandText.Replace("#DynamicData#", ShipTypeProductInfo.ListProductInfo[i].SysNo.ToString() + ",'" + ShipTypeProductInfo.ListProductInfo[i].ProductID.ToString() + "'"));
                }
                cmd_item.CommandText = builersql.ToString();
                cmd_item.ExecuteNonQuery();
            }
            if (ShipTypeProductInfo.ListProductInfo == null && ShipTypeProductInfo.ListCategoryInfo != null)
            {
                for (int i = 0; i < ShipTypeProductInfo.ListCategoryInfo.Count; i++)
                {

                    cmd_item.SetParameterValue("@MasterSysNo", ShipTypeProductInfo.SysNo);

                    cmd_item.SetParameterValue("@CompanyCode", ShipTypeProductInfo.CompanyCode);
                    cmd_item.SetParameterValue("@Status", "A");
                    cmd_item.SetParameterValue("@Description", ShipTypeProductInfo.Description);
                    if (EnumCodeMapper.TryGetCode(ShipTypeProductInfo.ShipTypeProductType, out obj))
                    {
                        cmd_item.SetParameterValue("@Type", obj);
                    }                  
                    if (EnumCodeMapper.TryGetCode(ShipTypeProductInfo.ProductRange, out obj))
                    {
                        cmd_item.SetParameterValue("@ItemRange", obj);
                    }
                    cmd_item.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
                    cmd_item.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);
                    cmd_item.SetParameterValue("@StoreCompanyCode", ShipTypeProductInfo.CompanyCode);
                    cmd_item.SetParameterValue("@LanguageCode", "zh-CN");
                    cmd_item.SetParameterValue("@CompanyCustomer", 0);
                    builersql.Append(cmd_item.CommandText.Replace("#DynamicData#", ShipTypeProductInfo.ListCategoryInfo[i].SysNo + ",null"));
                   
                }
                cmd_item.CommandText = builersql.ToString();
                cmd_item.ExecuteNonQuery();
            }
        }
    }
}
