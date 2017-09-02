using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.IDataAccess;
using System.Xml;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using System.Data;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IProductShiftDetailDA))]
    public class ProductShiftDetailDA : IProductShiftDetailDA
    {
        #region IProductShiftDetailDA Members

        public int InsertProductShiftDetails(List<BizEntity.Invoice.ProductShiftDetailEntity> listProductShiftDetails, string groubText)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlProductShift xmlEntity = new XmlProductShift();
            if (listProductShiftDetails.Count > 0)
            {
                xmlEntity.InvoiceNode = new InvoiceNode
                {
                    OrderSysNo = groubText,
                    OrderType = "SHIFT",
                    WarehouseNumber = listProductShiftDetails[0].WarehouseNumber.ToString(),
                    ComputerNo = 0,
                    Items = new Items
                    {
                        Item = new List<ItemForProductShift>()
                    }
                };
            }

            var productShiftDetailEntitys = listProductShiftDetails.GroupBy(p => p.SysNo);

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("InsertProductShiftDetails");
            //            command.CommandText = @"           
            //                EXEC WMS.dbo.UP_SendShiftInvoiceMessage
            //                @WarehouseNumber = @WarehouseNumber,
            //                @Body=@Body,
            //                @ReturnMsg= @ReturnMsg output
            //                ";

            foreach (ProductShiftDetailEntity entity in listProductShiftDetails)
            {
                xmlEntity.InvoiceNode.Items.Item.Add(
                    new ItemForProductShift(entity, groubText)
                );
            }
            var serialResult = "";
            int? whereHoseNumber = listProductShiftDetails[0].WarehouseNumber;
            if (whereHoseNumber == 50 || whereHoseNumber == 59)
            {
                whereHoseNumber = 51;
            }
            serialResult = SerializationUtility.XmlSerialize(xmlEntity, true);
            //serialResult = serialResult.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");
            //command.AddInputParameter("@WarehouseNumber", DbType.Int32, whereHoseNumber);
            //command.AddInputParameter("@Body", DbType.String, serialResult);
            command.SetParameterValue("@WarehouseNumber", whereHoseNumber);
            command.SetParameterValue("@Body", serialResult);

            command.AddOutParameter("@ReturnMsg", DbType.String, 50);
            command.ExecuteNonQuery();
            string result = command.GetParameterValue("@ReturnMsg").ToString();
            if (!result.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
                //try
                //{
                //    CommonService.SendMail(new MailBodyMsg
                //    {
                //        CreateDate = DateTime.Now,
                //        Subjuect = "导入金税失败",
                //        MailBody = serialResult,
                //        MailFrom = "OYSD Support",
                //        MailTo = " Prince.W.Ma@newegg.com; Tony.Y.Ji@newegg.com; Allan.K.Li@newegg.com;Kathy.Y.Gao@newegg.com;Tom.D.Zhou@newegg.com",
                //        Status = 0
                //    });
                //}
                //catch
                //{
                //    throw new BusinessException("发送SSB消息失败！");
                //}
            }
            else
            {
                return 0;
            }
        }

        public List<ProductShiftDetailEntity> GetProductShiftDetail(ProductShiftDetailQueryEntity productShiftDetailQueryEntity)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductShiftDetails");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "st.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.StockSysNoA",
                  DbType.Int32, "@StockSysNoA", QueryConditionOperatorType.Equal, productShiftDetailQueryEntity.StockSysNoA);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.StockSysNoB",
                 DbType.Int32, "@StockSysNoB", QueryConditionOperatorType.Equal, productShiftDetailQueryEntity.StockSysNoB);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                 DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, productShiftDetailQueryEntity.OutTimeEnd);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                 DbType.DateTime, "@OutTimeBegin", QueryConditionOperatorType.MoreThanOrEqual, productShiftDetailQueryEntity.OutTimeBegin);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "stItem.UnitCost",
                    DbType.Decimal, "@UnitCost", QueryConditionOperatorType.NotEqual, 0M);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, productShiftDetailQueryEntity.CompanyCode);
                if (productShiftDetailQueryEntity.GoldenTaxNo != "")
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.GoldenTaxNo",
                   DbType.String, "@GoldenTaxNo", QueryConditionOperatorType.Equal, productShiftDetailQueryEntity.GoldenTaxNo);
                }
                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "st.ShiftType",
                QueryConditionOperatorType.In, "0,2");
                if (productShiftDetailQueryEntity.StItemSysNos.Count > 0)
                {
                    string strs = "-99999";
                    foreach (int data in productShiftDetailQueryEntity.StItemSysNos)
                    {
                        strs = strs + "," + data.ToString();
                    }
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "stItem.SysNo", QueryConditionOperatorType.In, strs);
                }
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var result = dataCommand.ExecuteEntityList<ProductShiftDetailEntity>();
                return result;
            }
        }

        public List<SysConfigEntity> GetSysysProductList(int? stockA)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSysConfigProductDetial");
            var result = dataCommand.ExecuteEntityList<SysConfigEntity>();

            if (result.Count > 0)
            {
                List<SysConfigEntity> list = new List<SysConfigEntity>();
                string[] strs = result[0].Value.Split(new char[] { ',' });
                foreach (string str in strs)
                {
                    string[] srs = str.Split(new char[] { ':' });
                    SysConfigEntity ent = new SysConfigEntity();
                    ent.Key = srs[0];
                    ent.Value = srs[1];
                    list.Add(ent);
                }
                return list;
            }
            return result;
        }

        public void WriteLog(GoldenTaxInvoiceLogEntity goldenTaxLog)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertProductShiftDetailLog");
            command.SetParameterValue("@CompanyCode", goldenTaxLog.CompanyCode);
            command.SetParameterValue("@LogDescription", goldenTaxLog.LogDescription);
            command.SetParameterValue("@LogStatus", goldenTaxLog.LogStatus);
            command.SetParameterValue("@LogTime", goldenTaxLog.LogTime);
            command.SetParameterValue("@OrderID", goldenTaxLog.OrderID);
            command.SetParameterValue("@OrderType", goldenTaxLog.OrderType);
            command.SetParameterValue("@WarehouseNumber", goldenTaxLog.WarehouseNumber);
            command.ExecuteNonQuery();
        }

        public ProductShiftDetailEntity GetProductShiftDetail(int shiftSysno, int productSysno)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductShiftDetails");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
               dataCommand.CommandText, dataCommand, null, "st.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.SysNo",
                 DbType.Int32, "@ShiftSysno", QueryConditionOperatorType.Equal, shiftSysno);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "stItem.ProductSysNo",
                 DbType.Int32, "@ProdcutSysno", QueryConditionOperatorType.Equal, productSysno);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
            }
            return dataCommand.ExecuteEntity<ProductShiftDetailEntity>();

        }

        public void InsertProductShiftCompany(List<ProductShiftDetailEntity> productList, string GoldenTaxNo, int createUser)
        {
            foreach (ProductShiftDetailEntity productShift in productList)
            {
                DataCommand command = DataCommandManager.GetDataCommand("InnersertShiftDetailToInvoice");

                command.SetParameterValue("@ShiftSysNo", productShift.SysNo);
                command.SetParameterValue("@ProductSysNo", productShift.ProductSysNo);
                command.SetParameterValue("@Quantity", productShift.Quantity);

                command.SetParameterValue("@Price", productShift.Price);
                command.SetParameterValue("@UnitCost", productShift.UnitCost);
                command.SetParameterValue("@TaxPrice", productShift.Tax);
                command.SetParameterValue("@StockA", productShift.WarehouseNumber);
                command.SetParameterValue("@StockB", productShift.StockSysNoB);
                command.SetParameterValue("@TaxNO", GoldenTaxNo);
                command.SetParameterValue("@OutCompany", productShift.OutCompany);
                command.SetParameterValue("@InCompany", productShift.InCompany);

                command.SetParameterValue("@AtTotalAmt", productShift.AtTotalAmt);
                command.SetParameterValue("@CreateUserSysNo", createUser);

                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
