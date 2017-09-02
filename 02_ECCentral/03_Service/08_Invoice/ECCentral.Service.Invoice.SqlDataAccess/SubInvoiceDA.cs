using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(ISubInvoiceDA))]
    public class SubInvoiceDA : ISubInvoiceDA
    {
        #region ISubInvoiceDA Members

        public SubInvoiceInfo Create(SubInvoiceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertSubInvoice");
            command.SetParameterValue("@SOSysNo", entity.SOSysNo);
            command.SetParameterValue("@StockSysNo", entity.StockSysNo);
            command.SetParameterValue("@InvoiceSeq", entity.InvoiceSeq);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@SplitQty", entity.SplitQty);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.SetParameterValue("@IsExtendWarrantyItem", entity.IsExtendWarrantyItem);
            string masterProductSysNoStr = "";
            if (entity.MasterProductSysNo != null && entity.MasterProductSysNo.Count > 0)
            {
                for (int i = 0; i < entity.MasterProductSysNo.Count; i++)
                {
                    masterProductSysNoStr += entity.MasterProductSysNo[i] + (i < entity.MasterProductSysNo.Count - 1 ? "," : "");
                }
            }
            command.SetParameterValue("@MasterProductSysNo", masterProductSysNoStr);

            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return LoadBySysNo(entity.SysNo.Value);
        }

        public void DeleteBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteSubInvoice");
            command.SetParameterValue("@SOSysNo", soSysNo);

            command.ExecuteNonQuery();
        }

        public SubInvoiceInfo LoadBySysNo(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetSubInvoiceBySysNo");
            dataCommand.SetParameterValue("@SysNo", sysNo);

            return dataCommand.ExecuteEntity<SubInvoiceInfo>((r, s) =>
            {
                string masterProductSysNoStr = r["MasterProductSysNoStr"].ToString();
                if (!string.IsNullOrEmpty(masterProductSysNoStr))
                {
                    s.MasterProductSysNo = masterProductSysNoStr
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList().ConvertAll(sysNoStr => int.Parse(sysNoStr));
                }
            }, true, true);
        }

        public List<SubInvoiceInfo> GetListByCriteria(SubInvoiceInfo query)
        {
            List<SubInvoiceInfo> result = new List<SubInvoiceInfo>();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSubInvoiceList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
               dataCommand.CommandText, dataCommand, null, "SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SOSysNo",
                  DbType.Int32, "@SOSysNo", QueryConditionOperatorType.Equal, query.SOSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "WarehouseNo",
                  DbType.Int32, "@WarehouseNo", QueryConditionOperatorType.Equal, query.StockSysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = dataCommand.ExecuteEntityList<SubInvoiceInfo>((r, s) =>
                {
                    string masterProductSysNoStr = r["MasterProductSysNoStr"].ToString();
                    if (!string.IsNullOrEmpty(masterProductSysNoStr))
                    {
                        s.MasterProductSysNo = masterProductSysNoStr
                            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList().ConvertAll(sysNoStr => int.Parse(sysNoStr));
                    }
                }, true, true);
            }

            return result;
        }

        #endregion ISubInvoiceDA Members
    }
}