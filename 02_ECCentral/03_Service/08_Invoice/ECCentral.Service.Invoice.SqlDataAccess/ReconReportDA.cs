using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Invoice.ReconReport;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IReconReportDA))]
    public class ReconReportDA : IReconReportDA
    {

        /// <summary>
        /// 重置SAP状态
        /// </summary>
        /// <param name="TransactionNumbers"></param>
        /// <returns></returns>
        public int UpdateSAPStatus(List<int> TransactionNumbers)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_UpdateSAPStatus");
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < TransactionNumbers.Count; i++)
            {
                if (i != TransactionNumbers.Count - 1)
                {
                    str.Append(TransactionNumbers[i] + ",");
                }
                else
                {
                    str.Append(TransactionNumbers[i]);
                }
            }
            string sql = command.CommandText.Replace("#StrWhere#", str.ToString());
            command.CommandText = sql;

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 生成总帐科目报表数据
        /// </summary>
        /// <param name="from">起始日期</param>
        /// <param name="to">结束日期</param>
        /// <returns></returns>
        public List<SAPInterfaceExchangeInfo> CreateExcel(DateTime? begin, DateTime? end)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_ComBineReportData");
            command.SetParameterValue("@PostingDate", end.Value);
            return AddMTD(command.ExecuteEntityList<SAPInterfaceExchangeInfo>(), begin.Value, end.Value);
        }

        /// <summary>
        /// 生成总帐科目MTD数据
        /// </summary>
        public List<SAPInterfaceExchangeInfo> AddMTD(List<SAPInterfaceExchangeInfo> list, DateTime begin, DateTime end)
        {
            List<SAPInterfaceExchangeInfo> lstResult = new List<SAPInterfaceExchangeInfo>();
            List<MTDInfo> lstMtd = new List<MTDInfo>();
            DataCommand command = DataCommandManager.GetDataCommand("Invoice_GetReportMTD");
            command.SetParameterValue("@BeginDate", begin);
            command.SetParameterValue("@EndDate", end.AddDays(1));
            lstMtd = command.ExecuteEntityList<MTDInfo>();

            foreach (MTDInfo mtd in lstMtd)
            {
                SAPInterfaceExchangeInfo result = new SAPInterfaceExchangeInfo();
                result.DocumentType = mtd.DOC_TYPE;
                result.GLAccount = mtd.GL_ACCOUNT;
                result.CompanyCode = mtd.CompanyCode;
                result.MTDData = mtd;
                foreach (SAPInterfaceExchangeInfo sap in list)
                {
                    if (sap.DocumentType == mtd.DOC_TYPE && sap.GLAccount == mtd.GL_ACCOUNT && sap.CompanyCode == mtd.CompanyCode)
                    {
                        result.PostingDate = sap.PostingDate;
                        result.Legacy_GLAmount = sap.Legacy_GLAmount;
                        result.SAP_GLAmount = sap.SAP_GLAmount;
                    }
                }

                lstResult.Add(result);
            }

            return lstResult;
        }

        /// <summary>
        /// 生成AR or AP科目报表数据
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<SAPInterfaceExchangeInfo> CreateOtherExcel(DateTime? begin, DateTime? end)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Invoice_ComBineOtherReportData");
            command.SetParameterValue("@PostingDate", end);
            return AddOtherMTD(command.ExecuteEntityList<SAPInterfaceExchangeInfo>(), begin.Value, end.Value);
        }

        /// <summary>
        /// 生成统驭科目MTD数据
        /// </summary>
        public List<SAPInterfaceExchangeInfo> AddOtherMTD(List<SAPInterfaceExchangeInfo> list, DateTime begin, DateTime end)
        {
            List<SAPInterfaceExchangeInfo> lstResult = new List<SAPInterfaceExchangeInfo>();
            List<MTDInfo> lstMtd = new List<MTDInfo>();
            DataCommand command = DataCommandManager.GetDataCommand("Invoice_GetOtherReportMTD");
            command.SetParameterValue("@BeginDate", begin);
            command.SetParameterValue("@EndDate", end.AddDays(1));
            lstMtd = command.ExecuteEntityList<MTDInfo>();

            foreach (MTDInfo mtd in lstMtd)
            {
                SAPInterfaceExchangeInfo result = new SAPInterfaceExchangeInfo();
                result.DocumentType = mtd.DOC_TYPE;
                result.CompanyCode = mtd.CompanyCode;
                result.AcctType = mtd.AcctType;
                result.MTDData = mtd;
                foreach (SAPInterfaceExchangeInfo sap in list)
                {
                    if (sap.DocumentType == mtd.DOC_TYPE && sap.CompanyCode == mtd.CompanyCode && sap.AcctType == mtd.AcctType)
                    {
                        result.PostingDate = sap.PostingDate;
                        result.Legacy_GLAmount = sap.Legacy_GLAmount;
                        result.SAP_GLAmount = sap.SAP_GLAmount;
                    }
                }
                lstResult.Add(result);
            }

            return lstResult;
        }

    }
}
