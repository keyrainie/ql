using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using ContentMgmt.GiftCardPoolInterface.Entities;

namespace ContentMgmt.GiftCardPoolInterface.DA
{
    public static class GiftCardPoolDA
    {
        private static void SetCommand(DataCommand command, GiftCardPoolEntity entity)
        {
            command.SetParameterValue("AmountType", entity.AmountType);
            command.SetParameterValue("Barcode", entity.Barcode);
            command.SetParameterValue("Code", entity.Code);
            command.SetParameterValue("Password", entity.Password);
            command.SetParameterValue("Status", entity.Status);
            command.SetParameterValue("SysNo", entity.SysNo);
        }

        public static int Insert(GiftCardPoolEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Item_InsertGiftCardPool");
            SetCommand(command, entity);
            int result = command.ExecuteNonQuery();
            entity.SysNo = (int)command.GetParameterValue("@SysNo");
            return result;
        }

        public static int Update(GiftCardPoolEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Item_UpdateGiftCardPool");
            SetCommand(command, entity);
            return command.ExecuteNonQuery();
        }

        public static int Delete(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Item_DeleteGiftCardPool");
            command.SetParameterValue("SysNo", sysNo);
            return command.ExecuteNonQuery();
        }

        public static int GetActiveCount(string barPrefix,decimal amt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Item_GetGiftCardPool_ActiveCountByBarcode");
            command.SetParameterValue("Barcode", barPrefix);
            command.SetParameterValue("AmountType", amt);
            return command.ExecuteScalar<int>();
        }

        public static string GetMaxBarcode(string barPrefix)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Item_GetGiftCardPool_MaxBarcode");
            command.SetParameterValue("Barcode", barPrefix);
            return command.ExecuteScalar<string>();
        }

        public static int SendMailInfo(string subject, string body)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("MailAddress", ConstValues.ToMailAddress);
            command.SetParameterValue("CCMailAddress", ConstValues.CcMailAddress);
            command.SetParameterValue("MailSubject", subject);
            command.SetParameterValue("MailBody", body);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            command.SetParameterValue("LanguageCode", ConstValues.LanguageCode);
            return command.ExecuteNonQuery();
        }

        public static List<GCItemEntity> GetGCItemList()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGCItemList");
            return command.ExecuteEntityList<GCItemEntity>();
        }

    }
}
