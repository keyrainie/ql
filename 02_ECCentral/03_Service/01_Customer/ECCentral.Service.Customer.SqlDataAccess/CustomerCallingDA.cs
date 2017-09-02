using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ICallsEventsDA))]
    public class CustomerCallingDA : ICallsEventsDA
    {

        public virtual CallsEvents CreateEvents(CallsEvents entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCustomerCallsEvents");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity;
        }

        public virtual void UpdateEvents(CallsEvents entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerCallsEvents");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual void CreateFollowUpLog(CallsEventsFollowUpLog entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCallsEventsFollowUpLog");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }



        public virtual void CloseCallsEvents(CallsEvents entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CloseCallsEvents");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }


        public virtual CallsEvents Load(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadCallsEvents");
            cmd.SetParameterValue("@SysNo", SysNo);
            return cmd.ExecuteEntity<CallsEvents>();
        }

        public virtual CallsEvents Load(CallingReferenceType ReferenceType, int ReferenceSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadCallsEventsByReference");
            cmd.SetParameterValue("@ReferenceType", ReferenceType);
            cmd.SetParameterValue("@ReferenceSysNo", ReferenceSysNo);
            return cmd.ExecuteEntity<CallsEvents>();
        }



        public virtual void UpdateReference(CallsEvents entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateReference");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }



        public virtual void CloseComplain(CallsEvents entity)
        {
            throw new NotImplementedException();
        }




        public virtual List<CallsEventsFollowUpLog> GetLogsByEventsSysNo(int callEventsSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetLogsByEventsSysNo");
            cmd.SetParameterValue("@CallEventsSysNo", callEventsSysNo);
            return cmd.ExecuteEntityList<CallsEventsFollowUpLog>();
        }



        public virtual void UpdateStatus(int callEventsSysNo, CallsEventsStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCallsEventsStatus");
            cmd.SetParameterValue("@SysNo", callEventsSysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValueAsCurrentUserSysNo("@LastEditUserSysNo");
            cmd.ExecuteNonQuery();
        }




    }
}
