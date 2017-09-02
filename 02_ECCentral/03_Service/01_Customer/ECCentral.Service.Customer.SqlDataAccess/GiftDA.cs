using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.Service.Utility;
using System.ComponentModel.Composition;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IGiftDA))]
    public class GiftDA : IGiftDA
    {
        public virtual CustomerGift Insert(CustomerGift entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Gift_Insert");
            cmd.SetParameterValue<CustomerGift>(entity);
            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity;
        }

        public virtual int UpdateStatus(CustomerGift entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Gift_UpdateStatus");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@Status", (int)entity.Status);
            return cmd.ExecuteNonQuery();
        }

        public virtual CustomerGift Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Gift_Load");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<CustomerGift>();
        }

        public virtual CustomerGift Load(int customerSysNo, int productSysNo, CustomerGiftStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Gift_LoadByParams");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@Status", status);
            return cmd.ExecuteEntity<CustomerGift>();
        }



        #region IGiftDA Members


        public virtual void Update(CustomerGift entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Gift_Update");
            cmd.SetParameterValue<CustomerGift>(entity);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region IGiftDA Members


        public void ReturnGiftForSO(int soSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ReturnGiftForSO");
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
