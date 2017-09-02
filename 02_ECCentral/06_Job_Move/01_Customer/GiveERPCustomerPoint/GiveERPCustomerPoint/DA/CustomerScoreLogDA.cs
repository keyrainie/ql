using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiveERPCustomerPoint.Entities;
using Newegg.Oversea.Framework.DataAccess;

namespace GiveERPCustomerPoint.DA
{
    public class CustomerScoreLogDA
    {
        public List<CustomerScoreEntity> LoadWaitGivePointList()
        {
            DataCommand command =  command = DataCommandManager.GetDataCommand("SelectWaitGivePointInfo");

            List<CustomerScoreEntity> list =  command.ExecuteEntityList<CustomerScoreEntity>();

            return list;
        }

        public void GivePointSuccess(int sysno)
        {
            DataCommand command = command = DataCommandManager.GetDataCommand("GivePointSuccess");
            command.SetParameterValue("@sysno", sysno);
            command.ExecuteNonQuery();
        }

        public void GivePointFaild(int sysno,string errorMark)
        {
            DataCommand command = command = DataCommandManager.GetDataCommand("GivePointFaild");
            command.SetParameterValue("@sysno", sysno);
            command.SetParameterValue("@ErrorMark", errorMark);
            command.ExecuteNonQuery();
        }

        public void InsertCRMLuckDrawLog(CRMLuckDrawLog entity)
        {
            DataCommand command = command = DataCommandManager.GetDataCommand("InsertCRMLuckDrawLog");
            command.SetParameterValue("@OrderSysNo", entity.OrderSysNo);
            command.SetParameterValue("@MembershipCardID", entity.MemberShipCardID);
            command.SetParameterValue("@LuckDrawName", entity.LuckDrawName);
            command.SetParameterValue("@LuckDrawID", entity.LuckDrawID);
            command.SetParameterValue("@LuckDrawCode", entity.LuckDrawCode);
            command.SetParameterValue("@LuckDrawMark", entity.LuckDrawMark);
            command.SetParameterValue("@PayMark", entity.PayMark);
            command.ExecuteNonQuery();
        }

        public List<CustomerScoreEntity> GetWaitReturnSOIDs()
        {
            DataCommand command = command = DataCommandManager.GetDataCommand("GetWaitReturnSOID");

            List<CustomerScoreEntity> list = command.ExecuteEntityList<CustomerScoreEntity>();

            return list;
        }

        public List<ReturnSoItemInfo> GetReturnSoItemInfoListBySOID(int soSysNo)
        {
            DataCommand command = command = DataCommandManager.GetDataCommand("GetSOItemtListBySoID");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<ReturnSoItemInfo>();
        }

        public void SetLuckTicketVoid(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SetLuckTicketVoid");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        public void ReturnPiontSuccess(int sysNo)
        {
            DataCommand command = command = DataCommandManager.GetDataCommand("GivePointSuccess");
            command.SetParameterValue("@SysNo", sysNo);
            command.ExecuteNonQuery();
        }
    }
}
