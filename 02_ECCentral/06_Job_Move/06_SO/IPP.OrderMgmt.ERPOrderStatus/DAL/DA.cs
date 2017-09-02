using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.ERPOrderStatus;

namespace ERPOrderStatus.DAL
{


    public class DA
    {
        public static List<ErpOrder> GetERPOrder()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetERPOrder");
            return command.ExecuteEntityList<ErpOrder>();
        }

        public static bool UpdateErpOrder(ErpOrder entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateERPOrder");
            command.SetParameterValue("@SOSysNo", Convert.ToInt32(entity.RefOrderNo));                 
            return command.ExecuteNonQuery()>0;              
        }
        public static List<ErpOrder> GetHuoDao(List<ErpOrder> list)
        {
            string listStr = string.Empty;
            foreach (ErpOrder item in list)
            {
                listStr += item.RefOrderNo + ",";
            }
            listStr = listStr.TrimEnd(',');
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetHuoDao");
            if (string.IsNullOrEmpty(listStr)) //by nolan: 0条数据的情况
            {
                listStr = "0";
            }
            command.CommandText = command.CommandText.Replace("#SysNoList#", listStr);
            return command.ExecuteEntityList<ErpOrder>();
        }   
    }       
}
