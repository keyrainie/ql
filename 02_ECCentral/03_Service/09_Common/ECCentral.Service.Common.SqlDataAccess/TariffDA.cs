using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(ITariffDA))]
    public class TariffDA : ITariffDA
    {
        public List<TariffInfo> QueryTariffCategory(string tariffcode)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("QueryTariffCategory");
            dc.SetParameterValue("@Tariffcode", tariffcode);
            return dc.ExecuteEntityList<TariffInfo>();
        }


        public TariffInfo CreateTariffInfo(TariffInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateTariff");
            cmd.SetParameterValue("@Tcode", entity.Tcode);
            cmd.SetParameterValue("@TariffCode", entity.Tariffcode);
            cmd.SetParameterValue("@ItemCategoryName", entity.ItemCategoryName);
            cmd.SetParameterValue("@Status", entity.Status.Value);
            cmd.SetParameterValue("@Unit", entity.Unit);
            cmd.SetParameterValue("@TariffPrice", entity.TariffPrice.Value);
            cmd.SetParameterValue("@TariffRate", entity.TariffRate.Value);
            cmd.SetParameterValue("@InUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@InUserName", ServiceContext.Current.UserDisplayName);
            cmd.SetParameterValue("@InDate", DateTime.Now);
            cmd.SetParameterValue("@EditUserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@EditUserName", ServiceContext.Current.UserDisplayName);
            cmd.SetParameterValue("@EditDate", DateTime.Now);
            cmd.SetParameterValue("@LanguageCode", "ch-zn");

            return cmd.ExecuteEntity<TariffInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public TariffInfo GetTariffInfo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTariffInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<TariffInfo>();
        }


        public bool UpdateTariffInfo(TariffInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTariffInfo");
            cmd.SetParameterValue("@SysNo", entity.SysNo.Value);
            cmd.SetParameterValue("@Tcode", entity.Tcode);
            cmd.SetParameterValue("@TariffCode", entity.Tariffcode);
            cmd.SetParameterValue("@ItemCategoryName", entity.ItemCategoryName);
            cmd.SetParameterValue("@Status", entity.Status.Value);
            cmd.SetParameterValue("@Unit", entity.ParentSysNo.Value);
            cmd.SetParameterValue("@TariffPrice", entity.ParentSysNo.Value);
            cmd.SetParameterValue("@TariffRate", entity.ParentSysNo.Value);
            cmd.SetParameterValue("@EditUserSysNo", entity.ParentSysNo.Value);
            cmd.SetParameterValue("@EditUserName", entity.ParentSysNo.Value);
            cmd.SetParameterValue("@EditDate", entity.ParentSysNo.Value);
            cmd.SetParameterValue("@Tcode", entity.Tcode);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public TariffInfo GetTariffInfoByName(string itemCategoryName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTariffInfoByName");
            cmd.SetParameterValue("@ItemCategoryName", itemCategoryName);
            return cmd.ExecuteEntity<TariffInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public TariffInfo GetTariffInfoByTariffCode(string tariffCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTariffInfoByTariffCode");
            cmd.SetParameterValue("@TariffCode", tariffCode);
            return cmd.ExecuteEntity<TariffInfo>();
        }


 
    }
}
