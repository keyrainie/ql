using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IFloorDA))]
    public class FloorDA : IFloorDA
    {
        public List<FloorTemplate> GetFloorTemplateList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_GetFloorTemplateList");
            return cmd.ExecuteEntityList<FloorTemplate>();
        }

        public List<FloorMaster> GetAllFloorMasterList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_GetAllFloorMasterList");
            return cmd.ExecuteEntityList<FloorMaster>();
        }

        public DataTable QueryFloorMaster(FloorMasterQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            if (filter.PageInfo == null)
                pagingEntity = null;
            else
            {
                pagingEntity.SortField = filter.PageInfo.SortBy;
                pagingEntity.MaximumRows = filter.PageInfo.PageSize;
                pagingEntity.StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize;
            }
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Floor_QueryFloorMaster");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "PageCode ASC, Priority ASC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "PageCode", DbType.String, "@PageCode",
                    QueryConditionOperatorType.Equal, filter.PageCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "PageType", DbType.Int32, "@PageType",
                    QueryConditionOperatorType.Equal, filter.PageType);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("PageType", typeof(PageCodeType));
                enumConfig.Add("Status", typeof(ADStatus));

                var dt = cmd.ExecuteDataTable(enumConfig);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public FloorMaster GetFloorMaster(string sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_GetFloorMaster");
            cmd.SetParameterValue("@SysNo", int.Parse(sysno));
            return cmd.ExecuteEntity<FloorMaster>();
        }

        public int CreateFloorMaster(FloorMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_CreateFloorMaster");
            cmd.SetParameterValue<FloorMaster>(entity);
            cmd.SetParameterValue("@Status", (int)entity.Status);
            cmd.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity.SysNo.Value;
        }

        public void UpdateFloorMaster(FloorMaster entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_UpdateFloorMaster");
            cmd.SetParameterValue<FloorMaster>(entity);
            cmd.SetParameterValue("@Status", (int)entity.Status);
            cmd.ExecuteNonQuery();
        }

        public void DeleteFloor(string sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_DeleteFloor");
            cmd.SetParameterValue("@SysNo", int.Parse(sysno));
            cmd.ExecuteNonQuery();
        }

        public List<FloorSection> GetFloorSectionList(string floorMasterSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_GetFloorSectionList");
            cmd.SetParameterValue("@FloorMasterSysNo", int.Parse(floorMasterSysNo));
            return cmd.ExecuteEntityList<FloorSection>();
        }

        public FloorSection GetFloorSection(string floorSectionSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_GetFloorSection");
            cmd.SetParameterValue("@SysNo", int.Parse(floorSectionSysNo));
            return cmd.ExecuteEntity<FloorSection>();
        }

        public int CreateFloorSection(FloorSection entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_CreateFloorSection");
            cmd.SetParameterValue<FloorSection>(entity);
            cmd.SetParameterValue("@Status", (int)entity.Status);
            cmd.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity.SysNo.Value;
        }

        public void UpdateFloorSection(FloorSection entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_UpdateFloorSection");
            cmd.SetParameterValue<FloorSection>(entity);
            cmd.SetParameterValue("@Status", (int)entity.Status);
            cmd.ExecuteNonQuery();
        }

        public void DeleteFloorSection(string sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_DeleteFloorSection");
            cmd.SetParameterValue("@SysNo", int.Parse(sysno));
            cmd.ExecuteNonQuery();
        }

        public List<FloorSectionItem> GetFloorSectionItemList(string floorSectionSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_GetFloorSectionItemList");
            cmd.SetParameterValue("@FloorSectionSysNo", int.Parse(floorSectionSysNo));
            return cmd.ExecuteEntityList<FloorSectionItem>();
        }

        public int CreateFloorSectionItem(FloorSectionItem entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_CreateFloorSectionItem");
            cmd.SetParameterValue<FloorSectionItem>(entity);
            cmd.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
            return entity.SysNo.Value;
        }

        public void UpdateFloorSectionItem(FloorSectionItem entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_UpdateFloorSectionItem");
            cmd.SetParameterValue<FloorSectionItem>(entity);
            cmd.ExecuteNonQuery();
        }

        public void DeleteFloorSectionItem(string sysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Floor_DeleteFloorSectionItem");
            cmd.SetParameterValue("@SysNo", int.Parse(sysno));
            cmd.ExecuteNonQuery();
        }
    }
}
