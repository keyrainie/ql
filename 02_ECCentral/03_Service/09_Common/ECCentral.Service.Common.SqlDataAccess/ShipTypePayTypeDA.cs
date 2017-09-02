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
    [VersionExport(typeof(IShipTypePayTypeDA))]
    public class ShipTypePayTypeDA:IShipTypePayTypeDA
    {
        public ShipTypePayTypeInfo Create(ShipTypePayTypeInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypePayType_Create");
            cmd.SetParameterValue<ShipTypePayTypeInfo>(entity);
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteEntity<ShipTypePayTypeInfo>();
        }

        public void Delete(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypePayType_Delete");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
        }

        public bool IsExistShipPayType(ShipTypePayTypeInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistShipTypePayType");
            cmd.SetParameterValue<ShipTypePayTypeInfo>(entity);

            return cmd.ExecuteScalar().ToInteger() == 0 ? false : true;
        }
    }
}
