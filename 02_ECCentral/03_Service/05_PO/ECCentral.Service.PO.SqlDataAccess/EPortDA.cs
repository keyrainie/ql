using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.PO.SqlDataAccess
{

    [VersionExport(typeof(IEPortDA))]
    public class EPortDA : IEPortDA
    {
        /// <summary>
        /// 创建电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity CreateEPort(EPortEntity entity)
        {
            entity.Indate = DateTime.Now;
            entity.LastEditdate = entity.Indate;
            entity.LastEditUser = entity.InUser;
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateEPortInfo");
            cmd.SetParameterValue<EPortEntity>(entity);
            return cmd.ExecuteEntity<EPortEntity>();
        }

        /// <summary>
        /// 保存电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity SaveEPort(EPortEntity entity)
        {
            entity.LastEditdate = DateTime.Now;
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateEPortInfo");
            cmd.SetParameterValue<EPortEntity>(entity);
            return cmd.ExecuteEntity<EPortEntity>();
        }

        /// <summary>
        /// 将电子口岸职位无效
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public int DeleteEPort(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteEPortInfo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public EPortEntity GetEPort(string sysNo)
        {
            EPortEntity result = new EPortEntity();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetEPortInfo");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<EPortEntity>();
        }

        /// <summary>
        /// 获取电子口岸列表
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public DataTable QueryEPort(EPortFilter filter, out int totalCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QeryEPortList");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = filter.PageInfo.SortBy,
                StartRowIndex = filter.PageInfo.PageIndex * filter.PageInfo.PageSize,
                MaximumRows = filter.PageInfo.PageSize
            };

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "c.InDate Desc"))
            {
                if(filter.SysNo>0)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
"c.SysNo", DbType.Int32,
"@SysNo", QueryConditionOperatorType.Equal,
filter.SysNo);
                }

                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
    "c.ePortName", DbType.String,
    "@ePortName", QueryConditionOperatorType.Equal,
    filter.Name);
                }

                if (filter.TaxFreeLimit > 0)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
"c.TaxFreeLimit", DbType.Int32,
"@TaxFreeLimit", QueryConditionOperatorType.Equal,
filter.TaxFreeLimit);
                }

//                if (filter.CreateTime !=null)
//                {
//                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
//"c.Indate", DbType.DateTime,
//"@Indate", QueryConditionOperatorType.Equal,
//filter.CreateTime);
//                }

                if (filter.DateFrom.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.Indate",
                    DbType.DateTime, "@DateFrom", QueryConditionOperatorType.MoreThanOrEqual, filter.DateFrom.Value);
                }
                if (filter.DataTo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.Indate",
                        DbType.DateTime, "@DataTo", QueryConditionOperatorType.LessThanOrEqual, filter.DataTo.Value);
                }

                if (filter.Status != null)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
"c.[Status]", DbType.Int32,
"@Status", QueryConditionOperatorType.Equal,
filter.Status);
                }
                command.CommandText = builder.BuildQuerySql();
                DataTable dt = command.ExecuteDataTable(new EnumColumnList { 
                {"Status", typeof(EPortStatusENUM)},
                {"ShippingType",typeof(EPortShippingTypeENUM)}});

                totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// 获取电子口岸
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public List<EPortEntity> GetEPort()
        {
            List<EPortEntity> result = new List<EPortEntity>();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetALLEPort");
            result= cmd.ExecuteEntityList<EPortEntity>();
            return result;
        }
    }
}
