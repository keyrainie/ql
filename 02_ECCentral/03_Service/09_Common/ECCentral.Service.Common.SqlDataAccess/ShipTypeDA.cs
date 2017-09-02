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
    [VersionExport(typeof(IShipTypeDA))]
    public class ShipTypeDA:IShipTypeDA
    {
        /// <summary>
        /// 获取配送方式的SysNo
        /// </summary>
        /// <returns></returns>
        public int GetShipTypeSequence()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipType_GetShipTypeSequence");
            cmd.ExecuteNonQuery();
            return Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
        }

        /// <summary>
        /// 检查创建配送方式是否存在
        /// </summary>
        /// <param name="item"></param>
        public bool GetShipTypeforCreate(ShippingType item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipType_GetShipTypeforCreate");
            cmd.SetParameterValue<ShippingType>(item);
            cmd.SetParameterValue("@CompanyCode", "8601");

            DataTable dt = cmd.ExecuteDataTable();//<ADStatus>("Status");
            if (dt != null&& dt.Rows.Count>0)
            {
                return DataMapper.GetEntity<ShippingType>(dt.Rows[0]) == null ? false : true;
            }
            return false;
        }

        /// <summary>
        /// 创建配送方式
        /// </summary>
        /// <param name="item"></param>
        public void CreateShipType(ShippingType item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipType_CreateShipType");                      
            cmd.SetParameterValue<ShippingType>(item);
            cmd.SetParameterValue("@DeliveryType", item.DeliveryType.GetHashCode());           
            cmd.SetParameterValue("@IsOnlineShow", item.IsOnlineShow.GetHashCode());
            cmd.SetParameterValue("@IsWithPackFee", item.IsWithPackFee.GetHashCode());
            cmd.SetParameterValue("@PackStyle", item.PackStyle.GetHashCode());
            if (!item.AreaSysNo.HasValue)
            {
                cmd.SetParameterValue("@AreaSysNo", 0);
            }
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 加载配送方式
        /// </summary>
        /// <param name="item"></param>
        public ShippingType LoadShipType(int  sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipType_LoadShipType");
            cmd.SetParameterValue("@SysNo",sysNo);
            ShippingType item=  cmd.ExecuteEntity <ShippingType>();
            return item;
        }
        /// <summary>
        /// 更新配送方式
        /// </summary>
        /// <param name="item"></param>
        public void UpdateShipType(ShippingType item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipType_UpdateShipType");
            cmd.SetParameterValue<ShippingType>(item);            
            cmd.SetParameterValue("@DeliveryType", item.DeliveryType.GetHashCode());                       
            cmd.SetParameterValue("@IsOnlineShow", item.IsOnlineShow.GetHashCode());                        
            cmd.SetParameterValue("@IsWithPackFee", item.IsWithPackFee.GetHashCode());                      
            cmd.SetParameterValue("@PackStyle", item.PackStyle.GetHashCode());
            if (!item.AreaSysNo.HasValue)
            {
                cmd.SetParameterValue("@AreaSysNo", 0);
            }
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
        }

    }
}
