using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess.CPS
{
    [VersionExport(typeof(IAdvertisingDA))]
    public class AdvertisingDA : IAdvertisingDA
    {
        public AdvertisingInfo Load(int sysNo)
        {

            AdvertisingInfo info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("LoadAdInfo");
            cmd.SetParameterValue("@SysNo", sysNo);

            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("Type", typeof(AdvertisingType));
            enumList.Add("Status", typeof(ValidStatus));
            DataTable dtMaster = cmd.ExecuteDataTable(enumList);
            if (dtMaster == null || dtMaster.Rows.Count == 0)
            {
                return null;
            }

            info = DataMapper.GetEntity<AdvertisingInfo>(dtMaster.Rows[0], null);

            return info;
        }

        public List<AdvertisingInfo> LoadByProductLineSysNoAndType(AdvertisingInfo entity)
        {
            List<AdvertisingInfo> info = null;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAdInfoByProductLineSysNoAndType");
            cmd.SetParameterValue("@SysNo", entity.ProductLineSysNo);
            cmd.SetParameterValue("@Type", entity.Type);

            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("Type", typeof(AdvertisingType));
            enumList.Add("Status", typeof(ValidStatus));
            DataTable dtMaster = cmd.ExecuteDataTable(enumList);

            if (dtMaster != null && dtMaster.Rows.Count > 0)
            {
                info = DataMapper.GetEntityList<AdvertisingInfo, List<AdvertisingInfo>>(dtMaster.Rows, null);
            }

            return info;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        public AdvertisingInfo CreateAdvertising(AdvertisingInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAd");
            cmd.SetParameterValue("@ProductLineSysNo", info.ProductLineSysNo);
            cmd.SetParameterValue("@Url", info.Url);
            cmd.SetParameterValue("@ImageUrl", info.ImageUrl);
            cmd.SetParameterValue("@Type", info.Type);
            cmd.SetParameterValue("@Text", info.Text);
            cmd.SetParameterValue("@SharedText", info.SharedText);
            cmd.SetParameterValue("@EventCode", info.EventCode);
            cmd.SetParameterValue("@AdCode", info.AdCode);
            //cmd.SetParameterValue("@InUser", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@InUser");
            cmd.SetParameterValue("@ImageWidth", info.ImageWidth);
            cmd.SetParameterValue("@ImageHeight", info.ImageHeight);

            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return info;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="info"></param>
        public void UpdateAdvertising(AdvertisingInfo info)
        {
             DataCommand cmd = DataCommandManager.GetDataCommand("UpdataAd");
             cmd.SetParameterValue("@Url", info.Url);
             cmd.SetParameterValue("@ImageUrl", info.ImageUrl);
             cmd.SetParameterValue("@Text", info.Text);
             cmd.SetParameterValue("@SharedText", info.SharedText);
             cmd.SetParameterValue("@EventCode", info.EventCode);
             cmd.SetParameterValue("@AdCode", info.AdCode);
             cmd.SetParameterValue("@Type", info.Type);
             cmd.SetParameterValue("@ImageHeight", info.ImageHeight);
             cmd.SetParameterValue("@ImageWidth", info.ImageWidth);
             cmd.SetParameterValue("@SysNo", info.SysNo);
             cmd.SetParameterValue("@ProductLineSysNo", info.ProductLineSysNo);
             cmd.SetParameterValueAsCurrentUserAcct("@EditUser");
             cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteAdvertising(int SysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteAd");
            cmd.SetParameterValue("@SysNo", SysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");
            cmd.ExecuteNonQuery();
        }

    }
}
