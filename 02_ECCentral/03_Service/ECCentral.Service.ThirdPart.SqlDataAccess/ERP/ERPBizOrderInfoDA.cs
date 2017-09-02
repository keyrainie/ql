using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ThirdPart.SqlDataAccess
{
    public class ERPBizOrderInfoDA
    {
        public int GetJLBH(string tableName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_GetJLBH_ByTableName");
            cmd.SetParameterValue("@TableName", tableName);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 创建送货单
        /// </summary>
        /// <param name="adjustInfo"></param>
        /// <returns></returns>
        public void CreateSHD(ERPSHDInfo shdInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_CreateSHD");
            cmd.SetParameterValue("@JLBH", shdInfo.JLBH);
            cmd.SetParameterValue("@BDJK", shdInfo.BDJK);
            cmd.SetParameterValue("@BZ", shdInfo.BZ);
            cmd.SetParameterValue("@DEPTID", shdInfo.DEPTID);
            cmd.SetParameterValue("@DEPTID_BC", shdInfo.DEPTID_BC);
            cmd.SetParameterValue("@DJLX", shdInfo.DJLX);
            cmd.SetParameterValue("@KCDD_BR", shdInfo.KCDD_BR);
            cmd.SetParameterValue("@KHID", shdInfo.KHID);
            cmd.SetParameterValue("@QHBZ", shdInfo.QHBZ);
            cmd.SetParameterValue("@SHRQ", shdInfo.SHRQ);
            cmd.SetParameterValue("@SHSJ", shdInfo.SHSJ);
            cmd.SetParameterValue("@WGBJ", shdInfo.WGBJ);
            cmd.SetParameterValue("@XHHD", shdInfo.XHHD);
            cmd.SetParameterValue("@ZDR", shdInfo.ZDR);
            cmd.SetParameterValue("@ZDSJ", shdInfo.ZDSJ);
            cmd.SetParameterValue("@ZXR", shdInfo.ZXR);
            cmd.SetParameterValue("@ZXSJ", shdInfo.ZXSJ);
            
            cmd.ExecuteNonQuery();

            DataCommand cmd2 = DataCommandManager.GetDataCommand("ECC_InsertSHDMapping");
            cmd2.SetParameterValue("@SHDSysNo", shdInfo.JLBH);
            cmd2.SetParameterValue("@RefOrderNo", shdInfo.RefOrderNo);
            cmd2.SetParameterValue("@RefOrderType", shdInfo.RefOrderType);

            cmd2.ExecuteNonQuery();

        }

        public void CreateSHDItem(ERPSHDItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_CreateSHDItem");
            cmd.SetParameterValue("@BZ", item.BZ);
            cmd.SetParameterValue("@ERP_SP_ID", item.ERP_SP_ID);
            cmd.SetParameterValue("@INX", item.INX);
            cmd.SetParameterValue("@JLBH", item.JLBH);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@SL", item.SL);

            cmd.ExecuteNonQuery();
        }

        public ERPSHDInfo GetSHDInfoByJLBH(int jlbh)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ERP_GetSHD_ByJLBH");
            dc.SetParameterValue("@JLBH", jlbh);
            return dc.ExecuteEntity<ERPSHDInfo>();
        }

        public virtual List<ERPSHDItem> GetSHDItemByJLBH(int jlbh)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ERP_GetSHDItem_ByJLBH");
            dc.SetParameterValue("@JLBH", jlbh);

            using (IDataReader reader = dc.ExecuteDataReader())
            {
                var list = DataMapper.GetEntityList<ERPSHDItem, List<ERPSHDItem>>(reader);
                return list;
            }
        }

        public int GetSHDSysNoByRefOrder(ERPShippingInfo shipInfo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ECC_GetSHDSysNoByRefOrder");
            dc.SetParameterValue("@RefOrderNo", shipInfo.RefOrderNo);
            dc.SetParameterValue("RefOrderType", shipInfo.RefOrderType);
            return dc.ExecuteScalar<int>();
        }

        public int GetSHDShippingStatus(int shdSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ECC_GetSHDShippingStatus");
            dc.SetParameterValue("@SHDSysNo", shdSysNo);
            return dc.ExecuteScalar<int>();
        }
    }
}
