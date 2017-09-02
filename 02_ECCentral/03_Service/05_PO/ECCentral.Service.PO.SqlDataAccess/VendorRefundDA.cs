using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Data;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IVendorRefundDA))]
    public class VendorRefundDA : IVendorRefundDA
    {
        #region IVendorRefundDA Members

        public BizEntity.PO.VendorRefundInfo LoadVendorRefundInfo(int vendorRefundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorRefundMasterBySysNo");
            command.SetParameterValue("@SysNo", vendorRefundSysNo);
            return command.ExecuteEntity<VendorRefundInfo>();
        }

        public VendorRefundInfo UpdateVendorRefundInfo(VendorRefundInfo refundInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRMAVendorRefundMaster");
            command.SetParameterValue("@PMAuditTime", refundInfo.PMAuditTime);
            command.SetParameterValue("@PMUserSysNo", refundInfo.PMUserSysNo);
            command.SetParameterValue("@PMDAuditTime", refundInfo.PMDAuditTime);
            command.SetParameterValue("@PMDUserSysNo", refundInfo.PMDUserSysNo);
            command.SetParameterValue("@PMCCAuditTime", refundInfo.PMCCAuditTime);
            command.SetParameterValue("@PMCCUserSysNo", refundInfo.PMCCUserSysNo);
            command.SetParameterValue("@PMMemo", refundInfo.PMMemo);
            command.SetParameterValue("@PMDMemo", refundInfo.PMDMemo);
            command.SetParameterValue("@PMCCMemo", refundInfo.PMCCMemo);
            command.SetParameterValue("@Note", refundInfo.Note);
            command.SetParameterValue("@Status", refundInfo.Status);
            command.SetParameterValue("@SysNo", refundInfo.SysNo);
            if (command.ExecuteNonQuery() <= 0)
            {
                return null;
            }
            return refundInfo;
        }

        public List<VendorRefundItemInfo> LoadVendorRefundItems(int vendorRefundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorRefundItemByRMAVendorRefundSysNo");
            command.SetParameterValue("@RMAVendorRefundSysNo", vendorRefundSysNo);
            List<VendorRefundItemInfo> list = command.ExecuteEntityList<VendorRefundItemInfo>();
            return list;
        }

        #endregion

        #region IVendorRefundDA Members


        public List<int> GetPMUserSysNoByRMAVendorRefundSysNo(int vendorRefundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPMUserSysNoByRMAVendorRefundSysNo");
            command.SetParameterValue("@RMAVendorRefundSysNo", vendorRefundSysNo);
            DataTable dt = command.ExecuteDataSet().Tables[0];
            List<int> pmSyNo = new List<int>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    pmSyNo.Add(Convert.ToInt32(dr["PMUserSysNo"]));
                }
            }
            return pmSyNo;
        }

        #endregion
    }
}
