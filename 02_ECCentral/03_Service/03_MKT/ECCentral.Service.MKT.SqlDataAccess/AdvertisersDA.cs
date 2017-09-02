using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IAdvertisersDA))]
    public class AdvertisersDA : IAdvertisersDA
    {
        #region 广告商
        /// <summary>
        /// 创建广告商
        /// </summary>
        /// <param name="item"></param>
        public void CreateAdvertisers(Advertisers item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Advertisers_CreateAdvertisers");
            dc.SetParameterValue<Advertisers>(item);
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 是否存在该广告商
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckExistAdvertisers(Advertisers item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Advertisers_CheckDuplicateAdvertiser");
            cmd.SetParameterValue("@SysNo", item.SysNo);
            cmd.SetParameterValue("@CompanyCode", item.CompanyCode);
            cmd.SetParameterValue("@AdvertiserName", item.AdvertiserName);
            cmd.SetParameterValue("@MonitorCode", item.MonitorCode);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 是否存在该广告商监测代码
        /// </summary>
        /// <param name="monitorCode"></param>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool CheckExistAdvertiserMonitorCode(string companyCode, string monitorCode, int? sysNo)
        {
            DataCommand cmd;
            if (sysNo == null)
                cmd = DataCommandManager.GetDataCommand("Advertisers_CheckDuplicateMonitorCode");
            else
            {
                cmd = DataCommandManager.GetDataCommand("Advertisers_CheckDuplicateMonitorCodeBySysNo");
                cmd.SetParameterValue("@SysNo", sysNo);
            }
            cmd.SetParameterValue("@MonitorCode", monitorCode);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// 加载广告商
        /// </summary>
        /// <param name="sysNo"></param>
        public Advertisers LoadAdvertiser(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Advertisers_GetAdvertiser");
            dc.SetParameterValue("@SysNo", sysNo);
            DataTable dt = dc.ExecuteDataTable<ADStatus>("Status");
            return DataMapper.GetEntity<Advertisers>(dt.Rows[0]);
        }

        /// <summary>
        /// 批量设置广告商监视状态
        /// </summary>
        /// <param name="item"></param>
        public void SetAdvertiserValid(List<int> item)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in item)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Advertisers_UpdateAdvertiserStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Active);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="item"></param>
        public void SetAdvertiserInvalid(List<int> item)
        {
            StringBuilder message = new StringBuilder();
            foreach (var i in item)
            {
                message.Append(i.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("Advertisers_UpdateAdvertiserStatus");
            dc.SetParameterValue("@SysNoString", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", ADStatus.Deactive);
            dc.SetParameterValueAsCurrentUserAcct("EditUser");
            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 编辑广告商
        /// </summary>
        /// <param name="item"></param>
        public void UpdateAdvertisers(Advertisers item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Advertisers_UpdateAdvertisers");
            dc.SetParameterValue<Advertisers>(item);
            dc.ExecuteNonQuery();
        }

        #endregion

        #region 广告效果监视

        /// <summary>
        /// 导出订阅用户Email列表
        /// </summary>
        public void ExportToExcel()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Advertisers_InsertCustomerRight");
            dc.ExecuteNonQuery();
        }
        #endregion
    }
}
