using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IAccessoryDA))]
    public class AccessoryDA : IAccessoryDA
    {
        public AccessoryInfo GetBySysNo(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetBySysNo");
            dc.SetParameterValue("@SysNo", sysNo);
            return dc.ExecuteEntity<AccessoryInfo>();
        }

        public AccessoryInfo Insert(AccessoryInfo accessoryInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Insert");

            dc.SetParameterValue("@SysNo", accessoryInfo.SysNo);
            dc.SetParameterValue("@AccessoryName", accessoryInfo.AccessoryName.Content);
            dc.ExecuteNonQuery();
            accessoryInfo.SysNo = (int)dc.GetParameterValue("@SysNo");
            return accessoryInfo;
        }

        public AccessoryInfo Update(AccessoryInfo accessoryInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Update");

            dc.SetParameterValue("@SysNo", accessoryInfo.SysNo);
            dc.SetParameterValue("@AccessoryID", accessoryInfo.SysNo);
            dc.SetParameterValue("@AccessoryName", accessoryInfo.AccessoryName.Content);
            dc.ExecuteNonQuery();
            return accessoryInfo;
        }

        public IList<AccessoryInfo> GetList(string accessoryName)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetListByAccessoryName");
            dc.SetParameterValue("@AccessoryName", accessoryName);
            return dc.ExecuteEntityList<AccessoryInfo>();
        }

        public IList<AccessoryInfo> GetAll()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetAll");
            return dc.ExecuteEntityList<AccessoryInfo>();
        }

        public IList<AccessoryInfo> GetListByID(string accessoryID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetListByID");
            dc.SetParameterValue("@AccessoriesID", accessoryID);
            return dc.ExecuteEntityList<AccessoryInfo>();
        }
    }
}
