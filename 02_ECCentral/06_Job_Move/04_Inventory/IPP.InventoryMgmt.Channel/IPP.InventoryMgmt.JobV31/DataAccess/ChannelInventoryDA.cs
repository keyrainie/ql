using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.DataAccess
{
    public class ChannelInventoryDA
    {
        public static int GetPercentListCountByChannelSysNo(int ChannelSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPercentListCountByChannelSysNo");
            command.SetParameterValue("@ChannelSysNo", ChannelSysNo);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static List<ChannelProductPercentEntity> GetAllPercentListByChannelSysNo(int page, int pageSize, int ChannelSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllPercentListByChannelSysNo");
            command.SetParameterValue("@Page", page);
            command.SetParameterValue("@PageSize", pageSize);
            command.SetParameterValue("@ChannelSysNo", ChannelSysNo);
            return command.ExecuteEntityList<ChannelProductPercentEntity>();
        }

        public static int GetPercentListCount()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPercentListCount");

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static List<ChannelProductPercentEntity> GetAllPercentList(int page,int pageSize)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllPercentList");

            command.SetParameterValue("@Page",page);
            command.SetParameterValue("@PageSize", pageSize);

            return command.ExecuteEntityList<ChannelProductPercentEntity>();
        }

        public static int GetAppointListCount()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAppointListCount");

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static List<ChannelProductAppointEntity> GetAllAppointList(int page, int pageSize)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAllAppointList");

            command.SetParameterValue("@Page", page);
            command.SetParameterValue("@PageSize", pageSize);

            return command.ExecuteEntityList<ChannelProductAppointEntity>();
        }

        public static int ChangeAppointSynStatus(int productSysNo, int channelSysNo, string synStatus,string userName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangeAppointSynStatus");

            command.SetParameterValue("@ProductSysNo", productSysNo);
            command.SetParameterValue("@ChannelSysNo", channelSysNo);
            command.SetParameterValue("@IsNeedSyn", synStatus);
            command.SetParameterValue("@EditUser", userName);
            command.SetParameterValue("@EditDate", DateTime.Now);

            return command.ExecuteNonQuery();
        }

        public static int ChangePercentSynQty(ChannelProductPercentEntity entity, string userName)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangePercentSynQty");

            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@ChannelSysNo", entity.ChannelSysNo);
            command.SetParameterValue("@OnlineQty", entity.MaxOnlineQty);
            command.SetParameterValue("@InventoryPercent", entity.InventoryPercent);
            command.SetParameterValue("@SafeInventoryQty", entity.SafeInventoryCount);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@EditUser", userName);
            command.SetParameterValue("@EditDate", DateTime.Now);

            return command.ExecuteNonQuery();
        }
    }
}
