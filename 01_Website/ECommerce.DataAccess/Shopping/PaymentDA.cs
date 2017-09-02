using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Payment;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Shopping
{
    public class PaymentDA
    {
        public static List<PayTypeInfo> GetAllPayTypeList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Payment_GetAllPayTypeList");
            List<PayTypeInfo> paytypeList =  cmd.ExecuteEntityList<PayTypeInfo>();

            return paytypeList;
        }


        /// <summary>
        /// Sets the type of the so pay.
        /// </summary>
        /// <param name="soSysNo">The so system no.</param>
        /// <param name="payTypeSysNo">The pay type system no.</param>
        public static void UpdateOrderPayType(int soSysNo, int payTypeSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Payment_UpdateOrderPayType");
            dataCommand.SetParameterValue("@SOSysNo", soSysNo);
            dataCommand.SetParameterValue("@PayTypeSysNo", payTypeSysNo);
            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建一条网银积分消费记录
        /// </summary>
        /// <param name="tlPoint"></param>
        /// <returns></returns>
        public static bool CreateUseBankPointRecord(TLPoint tlPoint)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Payment_CreateUseBankPointRecord");
            dataCommand.SetParameterValue("@SOSysNo", tlPoint.SoSysNO);
            dataCommand.SetParameterValue("@Point", tlPoint.Point);
            dataCommand.SetParameterValue("@Type", tlPoint.Type);
            dataCommand.SetParameterValue("@InDate", tlPoint.InDate);
            dataCommand.SetParameterValue("@InUser", tlPoint.InUser);
            dataCommand.SetParameterValue("@LastEditDate", tlPoint.LastEditDate);
            dataCommand.SetParameterValue("@LastEditUser", tlPoint.LastEditUser);
            int effectRow=dataCommand.ExecuteNonQuery();
            return effectRow == 1;
        }

        /// <summary>
        /// 根据soSysNO获取该so使用了的网银积分数量
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static int GetUseBankPointBySoNo(int soSysNo)
        {
            int usePoint = 0;
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Payment_GetUseBankPointBySoNo");
            dataCommand.SetParameterValue("@SOSysNo", soSysNo);
            usePoint=dataCommand.ExecuteScalar<int>();
            return usePoint;
        }
    }
}
