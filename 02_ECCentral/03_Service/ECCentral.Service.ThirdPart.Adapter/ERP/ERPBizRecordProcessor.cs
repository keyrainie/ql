using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.ThirdPart.Interface;
using ECCentral.Service.Utility;
using ECCentral.Service.ThirdPart.SqlDataAccess;

namespace ECCentral.Service.ThirdPart.Adapter
{
    [VersionExport(typeof(ISyncERPBizRecord))]
    public class ERPBizRecordProcessor : ISyncERPBizRecord
    {
        private ERPBizOrderInfoDA erpDA = new ERPBizOrderInfoDA();

        /// <summary>
        /// 送货单
        /// </summary>
        /// <param name="shdInfo"></param>
        /// <returns></returns>
        public ERPSHDInfo CreateSHD(ERPSHDInfo shdInfo)
        {
            shdInfo.JLBH = erpDA.GetJLBH("SHD_SH");
            shdInfo.DJLX = 0;
            shdInfo.SHDTypeMemo = "送货单";
            if(shdInfo.SHRQ == null)
            {
                shdInfo.SHRQ = DateTime.Now;
            }
            if (shdInfo.SHSJ == null)
            {
                shdInfo.SHSJ = "全天";
            }
            shdInfo.BZ = "B2C_" + shdInfo.RefOrderNo + " " + shdInfo.RefOrderType + " " + shdInfo.SHDTypeMemo;

            erpDA.CreateSHD(shdInfo);

            int itemINX = 1;
            foreach (ERPSHDItem shdItem in shdInfo.SHDItemList)
            {
                shdItem.JLBH = shdInfo.JLBH;
                shdItem.INX = itemINX++;
                shdItem.ERP_SP_ID = shdItem.ERP_SP_ID.HasValue ? shdItem.ERP_SP_ID : 0;
                erpDA.CreateSHDItem(shdItem);
            }

            ERPSHDInfo result = erpDA.GetSHDInfoByJLBH((int)shdInfo.JLBH);
            return shdInfo;
        }

        /// <summary>
        /// 同城往返单
        /// </summary>
        /// <param name="shdInfo"></param>
        /// <returns></returns>
        public ERPSHDInfo CreateSHD_TCWF(ERPSHDInfo shdInfo)
        {
            shdInfo.JLBH = erpDA.GetJLBH("SHD_SH");
            shdInfo.DJLX = 0;
            shdInfo.SHDTypeMemo = "同城往返";
            if(shdInfo.SHRQ == null)
            {
                shdInfo.SHRQ = DateTime.Now;
            }
            if (shdInfo.SHSJ == null)
            {
                shdInfo.SHSJ = "全天";
            }
            shdInfo.BZ = "B2C_" + shdInfo.RefOrderNo + " " + shdInfo.RefOrderType + " " + shdInfo.SHDTypeMemo;

            erpDA.CreateSHD(shdInfo);

            int itemINX = 1;
            foreach (ERPSHDItem shdItem in shdInfo.SHDItemList)
            {
                shdItem.JLBH = shdInfo.JLBH;
                shdItem.INX = itemINX++;
                shdItem.ERP_SP_ID = shdItem.ERP_SP_ID.HasValue ? shdItem.ERP_SP_ID : 0;
                erpDA.CreateSHDItem(shdItem);
            }

            ERPSHDInfo result = erpDA.GetSHDInfoByJLBH((int)shdInfo.JLBH);
            return shdInfo;
        }

        /// <summary>
        /// 返仓单
        /// </summary>
        /// <param name="shdInfo"></param>
        /// <returns></returns>
        public ERPSHDInfo CreateSHD_FC(ERPSHDInfo shdInfo)
        {
            shdInfo.JLBH = erpDA.GetJLBH("SHD_SH");
            shdInfo.DJLX = 0;
            shdInfo.SHDTypeMemo = "返仓单";
            if(shdInfo.SHRQ == null)
            {
                shdInfo.SHRQ = DateTime.Now;
            }
            if (shdInfo.SHSJ == null)
            {
                shdInfo.SHSJ = "全天";
            }
            shdInfo.BZ = "B2C_" + shdInfo.RefOrderNo + " " + shdInfo.RefOrderType + " " + shdInfo.SHDTypeMemo;

            erpDA.CreateSHD(shdInfo);

            int itemINX = 1;
            foreach (ERPSHDItem shdItem in shdInfo.SHDItemList)
            {
                shdItem.JLBH = shdInfo.JLBH;
                shdItem.INX = itemINX++;
                shdItem.ERP_SP_ID = shdItem.ERP_SP_ID.HasValue ? shdItem.ERP_SP_ID : 0;
                erpDA.CreateSHDItem(shdItem);
            }

            ERPSHDInfo result = erpDA.GetSHDInfoByJLBH((int)shdInfo.JLBH);
            return shdInfo;
        }

        /// <summary>
        /// 获取送货单据信息
        /// </summary>
        /// <param name="jlbh"></param>
        /// <returns></returns>
        public ERPSHDInfo GetSHDInfoByJLBH(int jlbh)
        {
            ERPSHDInfo shdInfo = erpDA.GetSHDInfoByJLBH(jlbh);
            if (shdInfo != null)
            {
                shdInfo.SHDItemList = erpDA.GetSHDItemByJLBH(jlbh);
            }

            return shdInfo;
        }

        /// <summary>
        /// 根据关联单据获取送货单状态
        /// </summary>
        /// <param name="refOrderList"></param>
        /// <returns></returns>
        public List<ERPShippingInfo> GetERPShippingInfoByRefOrder(List<ERPShippingInfo> refOrderList)
        {
            foreach (ERPShippingInfo item in refOrderList)
            {
                item.SHDSysNo = erpDA.GetSHDSysNoByRefOrder(item);
                item.ShippingStatus = erpDA.GetSHDShippingStatus((int)item.SHDSysNo);
            }

            return refOrderList;
        }
    }
}
