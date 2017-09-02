using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.Utilities;

namespace ECCentral.Job.Invoice.AutoRunSOFreight.Biz
{
    public class FreightDA
    {

        public static List<SOFreightStatDetail> GetSOList()
        {
            var command = DataCommandManager.GetDataCommand("GetSOList");
            return command.ExecuteEntityList<SOFreightStatDetail>();
        }


        public static SOFreightStatDetail GetTrackingNumber(int soSysNo, int localWHSysNo)
        {
            string cmdName = string.Empty;
            switch (localWHSysNo)
            {
                case 51:
                    cmdName = "GetTrackingNumber-JP";
                    break;
                case 52:
                    cmdName = "GetTrackingNumber-HK";
                    break;
                case 53:
                    cmdName = "GetTrackingNumber-SH";
                    break;
            }

            if (string.IsNullOrEmpty(cmdName))
            {
                return null;
            }

            var command = DataCommandManager.GetDataCommand(cmdName);
            command.SetParameterValue("@SONumber", soSysNo);
            return command.ExecuteEntity<SOFreightStatDetail>();
        }

        public static List<ShippingInfo> GetAllShippingFee(List<ShippingFeeQueryInfo> qryList)
        {
            string xml = SerializationUtility.XmlSerialize(qryList);
            DataCommand command = DataCommandManager.GetDataCommand("Shipping_GetAllShippingFee");
            command.SetParameterValue("@ReqMsg", xml);
            return command.ExecuteEntityList<ShippingInfo>();
        }


        public static SOFreightStatDetail GetSOFreightStat(int soSysNo)
        {
            var command = DataCommandManager.GetDataCommand("GetSOFreightStat");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<SOFreightStatDetail>();
        }

        public static void CreateSOFreightStat(SOFreightStatDetail so)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateSOFreightStatDetail");
            command.SetParameterValue("@SOSysNo", so.SOSysNo);
            command.SetParameterValue("@TrackingNumber", so.TrackingNumber);
            command.SetParameterValue("@ShipTypeSysNo", so.ShipTypeSysNo);
            command.SetParameterValue("@SOWeight", so.SOWeight);
            command.SetParameterValue("@SOFreight", so.SOFreight);
            command.SetParameterValue("@RealWeight", so.RealWeight);
            command.SetParameterValue("@RealPayFreight", so.RealPayFreight);
            command.SetParameterValue("@RealOutFreight", 0);//实际支出运费，后期使用
            command.SetParameterValue("@OrderDate", so.OrderDate);
            command.SetParameterValue("@OutDate", so.OutDate);
            command.SetParameterValue("@CurrencySysNo", so.CurrencySysNo);
            command.ExecuteNonQuery();
        }

        public static VendorInfo GetVendorInfo(int merchantSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetVendorInfo");
            cmd.SetParameterValue("@SysNo", merchantSysNo);
            return cmd.ExecuteEntity<VendorInfo>();
        }
    }
}
