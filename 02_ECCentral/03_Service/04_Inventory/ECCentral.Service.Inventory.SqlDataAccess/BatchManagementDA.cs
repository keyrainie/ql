using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IBatchManagementDA))]
    public class BatchManagementDA : IBatchManagementDA
    {

        #region IBatchManagementDA Members

        public void UpdateProductRingInfo(BizEntity.Inventory.ProductRingDayInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("UpdateProductRing");
            command.SetParameterValue("@SysNo", entity.SysNo.Value);
            command.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            command.SetParameterValue("@C3SysNo", entity.C3SysNo);
            command.SetParameterValue("@RingDay", entity.RingDay);
            command.SetParameterValue("@Email", entity.Email);
            command.SetParameterValue("@EditUser", entity.EditUser);
            command.ExecuteNonQuery();
        }

        public void InsertProductRingInfo(BizEntity.Inventory.ProductRingDayInfo entity)
        {
            var command = DataCommandManager.GetDataCommand("InsertProductRing");
            command.SetParameterValue("@BrandSysNo", entity.BrandSysNo);
            command.SetParameterValue("@C3SysNo", entity.C3SysNo);
            command.SetParameterValue("@RingDay", entity.RingDay);
            command.SetParameterValue("@Email", entity.Email);
            command.SetParameterValue("@InUser", entity.InUser);
            command.SetParameterValue("@InDate", entity.InDate);
            command.SetParameterValue("@EditUser", entity.EditUser);
            command.SetParameterValue("@EditDate", entity.EditDate);
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
