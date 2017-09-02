using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IAreaDeliveryDA))]
    public class AreaDeliveryDA:IAreaDeliveryDA
    {
        public AreaDeliveryInfo Create(AreaDeliveryInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAreaDeliveryInfo");
            cmd.SetParameterValue<AreaDeliveryInfo>(entity);
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntity<AreaDeliveryInfo>();
        }

        public AreaDeliveryInfo Update(AreaDeliveryInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAreaDeliveryInfo");
            cmd.SetParameterValue<AreaDeliveryInfo>(entity);
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntity<AreaDeliveryInfo>();
        }

        public void Delete(int transactionNumber)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteAreaDeliveryInfo");
            cmd.SetParameterValue("@SysNo", transactionNumber);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
        }

        public AreaDeliveryInfo GetAreaDeliveryInfoByID(int transactionNumber)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAreaDeliveryInfoByID");
            cmd.SetParameterValue("@SysNo", transactionNumber);
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntity<AreaDeliveryInfo>();
        }

        public List<AreaDeliveryInfo> GetAreaDeliveryList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllAreaDelivery");
            cmd.SetParameterValue("@CompanyCode", "8601");
            return cmd.ExecuteEntityList<AreaDeliveryInfo>();
        }
    }
}
