using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Common.SqlDataAccess
{

    [VersionExport(typeof(IBizLanguageDescDA))]
    public class BizLanguageDescSettingDA : IBizLanguageDescDA
    {
        public bool InsertBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertBizObjectLanguageDesc");
            cmd.SetParameterValue("@BizObjectType", entity.BizObjectType);
            cmd.SetParameterValue("@BizObjectSysNo", entity.BizObjectSysNo);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.SetParameterValue("@BizObjectId", string.IsNullOrWhiteSpace(entity.BizObjectId) ? string.Empty : entity.BizObjectId);
            cmd.SetParameterValue("@Description", entity.Description);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool UpdateBizObjectLanguageDesc(BizObjectLanguageDesc entity)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateBizObjectLanguageDesc");
            cmd.SetParameterValue("@BizObjectType", entity.BizObjectType);


            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.SetParameterValue("@Description", entity.Description);

            if (entity.BizObjectSysNo.HasValue && entity.BizObjectSysNo > 0)
            {
                cmd.CommandText = cmd.CommandText.Replace("#ObjId#", " AND BizObjectSysNo=@BizObjectSysNo");

                cmd.AddInputParameter("@BizObjectSysNo", DbType.Int32, entity.BizObjectSysNo);
            }
            else if (!string.IsNullOrEmpty(entity.BizObjectId))
            {
                cmd.CommandText = cmd.CommandText.Replace("#ObjId#", " AND BizObjectId=@BizObjectId"); 
                cmd.AddInputParameter("@BizObjectId", DbType.String, entity.BizObjectId);
            }
            return cmd.ExecuteNonQuery() > 0;
        }


        public List<BizObjectLanguageDesc> GetBizObjectLanguageDescByBizTypeAndBizSysNo(string bizObjectType, string bizObjectSysNo, string bizObjectId)
        {
            List<BizObjectLanguageDesc> result = new List<BizObjectLanguageDesc>();
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetBizObjectLanguageDesc");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, new PagingInfoEntity(), "ORDER BY SysNo"))
            {
                if (!string.IsNullOrEmpty(bizObjectType))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BizObjectType",
                        DbType.String, "@BizObjectType", QueryConditionOperatorType.Equal, bizObjectType);
                }
                if (bizObjectSysNo != "0")
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BizObjectSysNo",
                                          DbType.Int32, "@BizObjectSysNo", QueryConditionOperatorType.Equal, int.Parse(bizObjectSysNo));
                }
                if (!string.IsNullOrEmpty(bizObjectId))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "bizObjectId",
                                          DbType.String, "@bizObjectId", QueryConditionOperatorType.Equal, bizObjectId);
                }

                cmd.CommandText = sb.BuildQuerySql();
                result = cmd.ExecuteEntityList<BizObjectLanguageDesc>();
            }
            return result;
        }


        public BizObjectLanguageDesc GetBizObjectLanguageInfo(string bizObjectType,string languageCode, int? bizObjectSysNo, string bizObjectId )
        {
            BizObjectLanguageDesc result = null;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetBizObjectLanguageInfo");
            using (var sb = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, new PagingInfoEntity(), "ORDER BY SysNo"))
            {
                if (!string.IsNullOrEmpty(bizObjectType))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BizObjectType",
                        DbType.String, "@BizObjectType", QueryConditionOperatorType.Equal, bizObjectType);
                }
                if (!string.IsNullOrEmpty(languageCode))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "LanguageCode",
                        DbType.String, "@LanguageCode", QueryConditionOperatorType.Equal, languageCode);
                }
                if (bizObjectSysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "BizObjectSysNo",
                                          DbType.Int32, "@BizObjectSysNo", QueryConditionOperatorType.Equal, bizObjectSysNo);
                }
                if (!string.IsNullOrEmpty(bizObjectId))
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "bizObjectId",
                                          DbType.String, "@bizObjectId", QueryConditionOperatorType.Equal, bizObjectId);
                }

                cmd.CommandText = sb.BuildQuerySql();
                result = cmd.ExecuteEntity<BizObjectLanguageDesc>();
            }
            return result;
        }

    }
}
