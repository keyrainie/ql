using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using System.Data;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IShipTypeSMSDA))]
    public class ShipTypeSMSDA : IShipTypeSMSDA
    {
        #region IShipTypeSMSDA Members

        public virtual BizEntity.Customer.ShipTypeSMS Create(BizEntity.Customer.ShipTypeSMS entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateShipTypeSMS");
            cmd.SetParameterValue<ShipTypeSMS>(entity);
            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity;
        }

        public virtual void Update(BizEntity.Customer.ShipTypeSMS entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateShipTypeSMS");
            cmd.SetParameterValue<ShipTypeSMS>(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual ShipTypeSMS Load(int sysNo)
        {
            ShipTypeSMS entity = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetShipTypeSMSBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                entity = DataMapper.GetEntity<ShipTypeSMS>(row);
            }
            return entity;
        }



        public virtual ShipTypeSMS Load(int shipTypeSysNo, SMSType smsType, string WebChannelID)
        {
            ShipTypeSMS entity = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryShipTypeSMS");
            cmd.SetParameterValue("@SMSType", smsType);
            cmd.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            entity = cmd.ExecuteEntity<ShipTypeSMS>();
            return entity;
        }

        #endregion
    }
}
