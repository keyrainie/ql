using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using AutoSetResponsibleUser.Entities;

namespace AutoSetResponsibleUser.DataAccessLayer
{
    internal sealed class AutoSetResponsibleUserDAL
    {
        public void SendEmail(string mailAddress, string mailSubject, string mailBody, int status, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");

            command.SetParameterValue("@MailAddress", mailAddress);
            command.SetParameterValue("@MailSubject", mailSubject);
            command.SetParameterValue("@MailBody", mailBody);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@CompanyCode", companyCode);

            command.ExecuteNonQuery();
        }

        public List<TrackingInfoEntity> GetTrackingInfo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetTrackingInfo");
            command.SetParameterValue("@BeginDate", AutoSetResponsibleUser.Common.GlobalSettings.BeginDate);
            return command.ExecuteEntityList<TrackingInfoEntity>();
        }
    }
}
