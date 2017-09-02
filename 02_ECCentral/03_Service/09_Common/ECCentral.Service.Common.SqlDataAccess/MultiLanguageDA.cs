using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Common.IDataAccess
{
   [VersionExport(typeof(IMultiLanguageDA))]
    public class MultiLanguageDA:IMultiLanguageDA
    {
        

        public List<MultiLanguageBizEntity> GetMultiLanguageBizEntityList(MultiLanguageBizEntity entity)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetMultiLanguageTemplate");
            string columns = "SysNo,LanguageCode";
            string tableName = entity.MappingTable;
            string pkcriteria = string.Format(" SysNo={0}", entity.SysNo); 

            foreach (PropertyItem propItem in entity.PropertyItemList)
            {
                columns += string.Format(",{0}", propItem.Field.Trim());
            }
            cmd.CommandText = cmd.CommandText.Replace("#tableName", tableName).Replace("#pkcriteria", pkcriteria).Replace("#columns", columns);
            DataTable dt = cmd.ExecuteDataTable();
            List<MultiLanguageBizEntity> list = new List<MultiLanguageBizEntity>();
            
            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                MultiLanguageBizEntity rowEntity = new MultiLanguageBizEntity();
                rowEntity.BizEntityType = entity.BizEntityType;
                rowEntity.LanguageCode = row["LanguageCode"].ToString().Trim();
                rowEntity.SysNo = entity.SysNo;
                rowEntity.MappingTable = entity.MappingTable;
                rowEntity.PropertyItemList = new List<PropertyItem>();
                foreach (PropertyItem propItem in entity.PropertyItemList)
                {
                    PropertyItem newPropItem = new PropertyItem();
                    newPropItem.Field = propItem.Field;
                    newPropItem.InputType = propItem.InputType;
                    newPropItem.Label = propItem.Label;
                    newPropItem.MaxLength = propItem.MaxLength;
                    newPropItem.IsRequired = propItem.IsRequired;
                    newPropItem.Value = row[propItem.Field] == null ? string.Empty : row[propItem.Field].ToString().Trim();
                    rowEntity.PropertyItemList.Add(newPropItem);
                }
                list.Add(rowEntity);
            }

            return list;
        }

        public void SetMultiLanguageBizEntity(MultiLanguageBizEntity entity)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("SetMultiLanguageTemplate");
            string tableName = entity.MappingTable;
            string pkcriteria = string.Format(" SysNo={0} AND LanguageCode='{1}'", entity.SysNo, entity.LanguageCode.Replace("'","''"));

            string insertColumns = string.Empty;
            string insertValues = string.Empty;
            string updateColumns = string.Empty;

            insertColumns = " LanguageCode,SysNo,";
            insertValues = " @LanguageCode,@SysNo,";
            cmd.AddInputParameter("@LanguageCode", System.Data.DbType.AnsiStringFixedLength);
            cmd.SetParameterValue("@LanguageCode", entity.LanguageCode);
            cmd.AddInputParameter("@SysNo", System.Data.DbType.AnsiStringFixedLength);
            cmd.SetParameterValue("@SysNo", entity.SysNo);

            foreach (PropertyItem propItem in entity.PropertyItemList)
            {
                insertColumns += " " + propItem.Field + ",";
                insertValues += " @" + propItem.Field + ",";
                updateColumns += string.Format(" {0}=@{0},", propItem.Field);
                cmd.AddInputParameter("@" + propItem.Field, System.Data.DbType.AnsiStringFixedLength);
                cmd.SetParameterValue("@" + propItem.Field, propItem.Value);
            }

            insertColumns = insertColumns.TrimEnd(',');
            insertValues = insertValues.TrimEnd(',');
            updateColumns = updateColumns.TrimEnd(',');            
            cmd.CommandText = cmd.CommandText.Replace("#tableName", tableName).Replace("#pkcriteria", pkcriteria).Replace("#insertColumns", insertColumns).Replace("#insertValues", insertValues).Replace("#updateColumns", updateColumns);

            cmd.ExecuteNonQuery();
        }




        public List<Language> GetAllLanguageList()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllLanguageList");
            List<Language> list = cmd.ExecuteEntityList<Language>();
            return list;
        }
    }
}
