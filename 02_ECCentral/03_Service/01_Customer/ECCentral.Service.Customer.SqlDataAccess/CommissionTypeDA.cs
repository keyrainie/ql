using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Customer.Society;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ICommissionTypeDA))]
    public class CommissionTypeDA : ICommissionTypeDA
    {
        public CommissionType Create(CommissionType entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCommissionType");
            //cmd.SetParameterValue<CommissionType>(entity);
            //cmd.SetParameterValue("@IsNet", entity.IsNet.GetHashCode());
            cmd.SetParameterValue("@CommissionTypeID", entity.CommissionTypeID);
            cmd.SetParameterValue("@CommissionTypeName", entity.CommissionTypeName);
            cmd.SetParameterValue("@CommissionTypeDesc", entity.CommissionTypeDesc);
            cmd.SetParameterValue("@LowerLimit", entity.LowerLimit);
            cmd.SetParameterValue("@UpperLimit", entity.UpperLimit);
            cmd.SetParameterValue("@CommissionRate", entity.CommissionRate);
            cmd.SetParameterValue("@CommissionStatus", entity.CommissionStatus);
            cmd.SetParameterValue("@CommissionOrder", entity.CommissionOrder);
            return cmd.ExecuteEntity<CommissionType>();
        }

        public CommissionType Update(CommissionType entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePayType");
            //cmd.SetParameterValue<CommissionType>(entity);
            //cmd.SetParameterValue("@IsNet", entity.IsNet.GetHashCode());
            //cmd.SetParameterValue("@IsPayWhenRecv", entity.IsPayWhenRecv.GetHashCode());
            //cmd.SetParameterValue("@IsOnlineShow", entity.IsOnlineShow.GetHashCode());
            //cmd.SetParameterValue("@NetPayType", entity.NetPayType.GetHashCode());
            //cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteEntity<CommissionType>();
        }

        public CommissionType QueryCommissionType(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryCommissionType");
            cmd.SetParameterValue("@SysNo", sysNo);
            CommissionType item = cmd.ExecuteEntity<CommissionType>();
            return item;
        }


        public bool IsExistCommissionTypeID(string commissionTypeID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistCommissionTypeID");
            cmd.SetParameterValue("@CommissionTypeID", commissionTypeID);
            return cmd.ExecuteScalar().ToInteger() == 0 ? false : true;
        }

        public bool IsExistCommissionTypeName(string commissionTypeName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistCommissionTypeName");
            cmd.SetParameterValue("@CommissionTypeName", commissionTypeName);
            return cmd.ExecuteScalar().ToInteger() == 0 ? false : true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<CommissionType> GetCommissionTypeListBySysNo(string sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryCommissionType");
            cmd.SetParameterValue("@SysNo", sysNo);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<CommissionType, List<CommissionType>>(reader);
            }
        }
        #region 扩展属性
        public CommissionType QueryCommissionType(string societyID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SocietyCommissionQuery");
            cmd.SetParameterValue("@SocietyID", societyID);
            CommissionType item = cmd.ExecuteEntity<CommissionType>();
            return item;
        }
        #endregion
    }
}
