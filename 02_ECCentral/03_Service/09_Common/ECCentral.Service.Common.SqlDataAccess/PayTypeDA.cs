using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IPayTypeDA))]
    public class PayTypeDA : IPayTypeDA
    {
        public PayType Create(PayType entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreatePayType");
            cmd.SetParameterValue<PayType>(entity);
            cmd.SetParameterValue("@IsNet", entity.IsNet.GetHashCode());
            cmd.SetParameterValue("@IsPayWhenRecv", entity.IsPayWhenRecv.GetHashCode());
            cmd.SetParameterValue("@IsOnlineShow", entity.IsOnlineShow.GetHashCode());
            cmd.SetParameterValue("@NetPayType", entity.NetPayType.GetHashCode());
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteEntity<PayType>();
        }

        public PayType Update(PayType entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePayType");
            cmd.SetParameterValue<PayType>(entity);
            cmd.SetParameterValue("@IsNet", entity.IsNet.GetHashCode());
            cmd.SetParameterValue("@IsPayWhenRecv", entity.IsPayWhenRecv.GetHashCode());
            cmd.SetParameterValue("@IsOnlineShow", entity.IsOnlineShow.GetHashCode());
            cmd.SetParameterValue("@NetPayType", entity.NetPayType.GetHashCode());
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteEntity<PayType>();
        }

        public PayType LoadPayType(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadPayType");
            cmd.SetParameterValue("@SysNo", sysNo);
            PayType item = cmd.ExecuteEntity<PayType>();
            return item;
        }


        public bool IsExistPayTypeID(string payTypeID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistPayTypeID");
            cmd.SetParameterValue("@PayTypeID", payTypeID);
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteScalar().ToInteger() == 0 ? false : true;
        }

        public bool IsExistPayTypeName(string payTypeName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistPayTypeName");
            cmd.SetParameterValue("@PayTypeName", payTypeName);
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteScalar().ToInteger() == 0 ? false : true;
        }
    }
}
