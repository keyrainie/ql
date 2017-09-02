using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice.InvoiceReport;
using ECCentral.BizEntity.Invoice.Report;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(InvoiceReportAppService))]
    public class InvoiceReportAppService
    {
        public virtual void ImportTrackingNumber(string fileIdentity, int stockSysNo,
            out List<TrackingNumberInfo> successResult, out List<TrackingNumberInfo> failedResult)
        {
            string destinationPath;
            MoveFile(fileIdentity, out destinationPath);

            var importTable = ConvertExcel2DataTable(destinationPath);

            CheckImportData(importTable, stockSysNo, out successResult, out failedResult);

            if (successResult.Count > 0)
            {
                var importResult = BatchCreateTrackingNumber(successResult);

                successResult.Clear();
                successResult.AddRange(importResult.SuccessList.ToEntityList().Where(w => w.SysNo.Value != 0).ToList());

                //运单号的系统编号等于0说明这条运单号记录已经存在
                failedResult.AddRange(importResult.SuccessList.ToEntityList().Where(w => w.SysNo.Value == 0).ToList());
                failedResult.AddRange(importResult.FaultList.ToEntityList());
            }
        }

        public virtual BatchActionResult<TrackingNumberInfo> BatchCreateTrackingNumber(List<TrackingNumberInfo> entityList)
        {
            List<BatchActionItem<TrackingNumberInfo>> request = entityList.Select(x => new BatchActionItem<TrackingNumberInfo>()
            {
                ID = x.SysNo.ToString(),
                Data = x
            }).ToList();

            return BatchActionManager.DoBatchAction(request, entity => ObjectFactory<InvoiceReportProcessor>.Instance.CreateTrackingNumber(entity));
        }

        private void MoveFile(string fileIdentity, out string destinationPath)
        {
            string configPath = AppSettingManager.GetSetting("Invoice", "InvoiceReportFilesPath");
            if (!Path.IsPathRooted(configPath))
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configPath);
            }
            destinationPath = Path.Combine(configPath, fileIdentity);
            string folder = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            FileUploadManager.MoveFile(fileIdentity, destinationPath);
        }

        private DataTable ConvertExcel2DataTable(string destinationPath)
        {
            string strConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + destinationPath + ";Extended Properties=Excel 12.0;";
            DataSet ds = new DataSet();
            using (OleDbConnection Conn = new OleDbConnection(strConn))
            {
                string SQL = "select * from [sheet1$]";
                Conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(SQL, strConn);
                da.Fill(ds);
            }
            File.Delete(destinationPath);

            if (ds.Tables[0].Rows.Count > 1000)
            {
                throw new BizException(ResouceManager.GetMessageString("Invoice.InvoiceReport", "InvoiceReport_ImportDataLimitError"));
            }
            string[] columnNameArray = new string[] { "OrderID", "OrderType", "InvoiceNumber", "TrackingNumber" };
            for (int i = 0; i < columnNameArray.Length; i++)
            {
                if (!ds.Tables[0].Columns.Contains(columnNameArray[i]))
                {
                    throw new BizException(ResouceManager.GetMessageString("Invoice.InvoiceReport", "InvoiceReport_ExcelFormatError"));
                }
            }

            return ds.Tables[0];
        }

        private void CheckImportData(DataTable importTable, int stockSysNo, out List<TrackingNumberInfo> successResult, out List<TrackingNumberInfo> failedResult)
        {
            successResult = new List<TrackingNumberInfo>();
            failedResult = new List<TrackingNumberInfo>();

            foreach (DataRow row in importTable.Rows)
            {
                TrackingNumberInfo entity = new TrackingNumberInfo()
                {
                    OrderID = Convert.ToString(row["OrderID"]),
                    OrderType = Convert.ToString(row["OrderType"]),
                    InvoiceNumber = Convert.ToString(row["InvoiceNumber"]),
                    TrackingNumber = Convert.ToString(row["TrackingNumber"]),
                    StockSysNo = stockSysNo
                };
                if (string.IsNullOrEmpty(entity.OrderID))
                {
                    if (string.IsNullOrEmpty(entity.OrderID) && string.IsNullOrEmpty(entity.OrderType)
                        && string.IsNullOrEmpty(entity.InvoiceNumber) && string.IsNullOrEmpty(entity.TrackingNumber))
                    {
                        continue;
                    }
                    failedResult.Add(entity);
                }
                else
                {
                    successResult.Add(entity);
                }
            }
        }

        public SOInvoiceInfo GetNew(SOInvoiceInfo entity)
        {
            return ObjectFactory<InvoiceReportProcessor>.Instance.GetNew(entity);
        }
    }
}