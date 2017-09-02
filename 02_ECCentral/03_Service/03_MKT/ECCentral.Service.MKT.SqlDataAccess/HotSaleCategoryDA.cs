using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IHotSaleCategoryDA))]
    public class HotSaleCategoryDA : IHotSaleCategoryDA
    {
        /// <summary>
        /// 获取同位置同组下其它的记录-
        /// </summary>
        /// <param name="relatedSysNo">参照记录的系统编号</param>
        /// <returns></returns>
        public List<HotSaleCategory> GetSameGroupOtherRecords(int relatedSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("HotSaleCategory_GetSameGroupOtherRecords");
            dataCommand.SetParameterValue("@SysNo", relatedSysNo);

            return dataCommand.ExecuteEntityList<HotSaleCategory>();
        }

        //验证同位置同组下是否存在重复的分类设置
        public bool CheckDuplicateCategory(HotSaleCategory query)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_ExistHotSaleCategoryByCategory2");
            dataCommand.SetParameterValue("@Position", query.Position);
            dataCommand.SetParameterValue("@CompanyCode", query.CompanyCode);
            dataCommand.SetParameterValue("@GroupName", query.GroupName);
            dataCommand.SetParameterValue("@C3SysNo", query.C3SysNo);
            dataCommand.SetParameterValue("@ExcludeSysNo", query.SysNo);
            return dataCommand.ExecuteScalar<int>() > 0;
        }

        public List<HotSaleCategory> GetByPosition(HotSaleCategory query)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_GetHotSaleCategoryByPosition");
            dataCommand.SetParameterValue("@Position", query.Position);
            dataCommand.SetParameterValue("@CompanyCode", query.CompanyCode);
            dataCommand.SetParameterValue("@GroupName", query.GroupName);
            return dataCommand.ExecuteEntityList<HotSaleCategory>();
        }

        //验证：同一位置有效记录的组名必须相同
        public string GetExistsGroupNameByPosition(HotSaleCategory query)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_ExistHotSaleCategoryByPosition");
            dataCommand.SetParameterValue("@Position", query.Position);
            dataCommand.SetParameterValue("@CompanyCode", query.CompanyCode);
            dataCommand.SetParameterValue("@GroupName", query.GroupName);
            dataCommand.SetParameterValue("@SysNo", query.SysNo);
            return dataCommand.ExecuteScalar<string>();
        }

        public void Delete(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_DeleteHotSaleCategory");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            dataCommand.ExecuteNonQuery();
        }

        private void SetCommand(DataCommand command, HotSaleCategory entity)
        {
            command.SetParameterValue("@C3SysNo", entity.C3SysNo);
            command.SetParameterValue("@GroupName", entity.GroupName);
            command.SetParameterValue("@ItemName", entity.ItemName);
            command.SetParameterValue("@PageId", entity.PageId);
            command.SetParameterValue("@PageType", entity.PageType);
            command.SetParameterValue("@Position", entity.Position);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValueAsCurrentUserAcct("@LogUser");
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
        }

        public void Insert(HotSaleCategory entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_InsertHotSaleCategory");
            SetCommand(dataCommand, entity);
            dataCommand.ExecuteNonQuery();
        }

        public void Update(HotSaleCategory entity)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_UpdateHotSaleCategory");
            SetCommand(dataCommand, entity);
            dataCommand.ExecuteNonQuery();
        }

        public HotSaleCategory Load(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("WebSite_GetHotSaleCategory");
            dataCommand.SetParameterValue("@SysNo", sysNo);
            return dataCommand.ExecuteEntity<HotSaleCategory>();
        }
    }
}
