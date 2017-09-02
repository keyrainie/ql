using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity;
using System.IO;
using System.Data;
using System.Transactions;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(ProductShiftProcessor))]
    public class ProductShiftProcessor
    {
        private IProductShiftDetailDA DA;

        public ProductShiftProcessor()
        {
            DA = ObjectFactory<IProductShiftDetailDA>.Instance;
        }

        public int CreateProductShiftDetail(List<ProductShiftDetailQueryEntity> entity)
        {
            int count = 0;
            if (entity != null && entity.Count > 0)
            {
                string GoldenTaxNo = DateTime.Now.ToString("yyyyMMddHHmmss");
                List<ProductShiftDetailEntity> listProductShiftDetails = DA.GetProductShiftDetail(entity[0]);
                if (listProductShiftDetails.Count > 0)
                {
                    int? StockSysNoA = listProductShiftDetails[0].WarehouseNumber;
                }
                if (listProductShiftDetails.Count > 5000)
                {
                    //throw new BizException("导入数据不能超过5000条！");
                    ThrowBizException("ProductShift_ImportDataCountError");
                }
                count = listProductShiftDetails.Count;
                if (listProductShiftDetails != null && listProductShiftDetails.Count > 0)
                {
                    SetProductShiftDetailGoldenTaxNo(listProductShiftDetails, GoldenTaxNo, entity[0].StockSysNoA);
                    //导入的金税
                    DA.InsertProductShiftDetails(listProductShiftDetails, GoldenTaxNo);
                    //update goldenTaxNo
                    foreach (ProductShiftDetailEntity productshift in listProductShiftDetails)
                    {
                        ExternalDomainBroker.EditGoldenTaxNo(GoldenTaxNo, productshift.StItemSysNo.Value);
                    }

                    string strs = ":";
                    foreach (ProductShiftDetailEntity ent in listProductShiftDetails)
                    {
                        strs += "," + ent.StItemSysNo.Value.ToString();
                    }

                    //record log
                    GoldenTaxInvoiceLogEntity goldenTaxLog = new GoldenTaxInvoiceLogEntity();
                    goldenTaxLog.WarehouseNumber = entity[0].StockSysNoA;
                    goldenTaxLog.OrderType = "SHIFT";
                    goldenTaxLog.OrderID = GoldenTaxNo;
                    goldenTaxLog.LogTime = DateTime.Now;
                    goldenTaxLog.LogStatus = 1;
                    goldenTaxLog.LogDescription = "Insert Golden Tax Successfully" + strs;
                    goldenTaxLog.StoreCompanyCode = entity[0].CompanyCode;
                    goldenTaxLog.LanguageCode = entity[0].LanguageCode;
                    goldenTaxLog.CompanyCode = entity[0].CompanyCode;
                    DA.WriteLog(goldenTaxLog);
                }
            }
            else
            {
                //throw new BizException("当前没要导入的金税信息");
                ThrowBizException("ProductShift_ImportDataNotNull");
            }
            return count;
        }

        /// <summary>
        /// 导入金税明细
        /// </summary>
        /// <param name="serverFilePath"></param>
        /// <returns></returns>
        public bool ImportProductShiftDetail(string serverFilePath)
        {
            string tempFilePath = Path.Combine(FileUploadManager.BaseFolder, Encoding.UTF8.GetString(Convert.FromBase64String(serverFilePath)));

            DataTable table = ExcelHelper.Read(tempFilePath, 0);

            if (table == null
                || table.Rows.Count == 0)
            {
                //throw new BizException("没有可导入的数据");
                ThrowBizException("ProductShift_MustHaveImportData");
            }
            if (table.Rows.Count > 5001)
            {
                //throw new BizException("导入的数据不能超过5000条，请将文件拆分后重新进行上传！");
                ThrowBizException("ProductShift_ImportDataCountError");
            }

            List<ShiftProductCompanyEntity> source = Parse(table);
            List<ProductShiftDetailEntity> productList = new List<ProductShiftDetailEntity>();
            string GoldenTaxNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (ShiftProductCompanyEntity shitProductCompany in source)
            {
                ProductShiftDetailEntity listProductShiftDetails = DA.GetProductShiftDetail(shitProductCompany.ShiftSysNo.Value, shitProductCompany.ProductSysNo.Value);
                if (listProductShiftDetails != null)
                {
                    listProductShiftDetails.Quantity = shitProductCompany.ShiftQty;
                    listProductShiftDetails.PriceWithoutTax = 0.0m;
                    listProductShiftDetails.Tax = 0.0m;
                    listProductShiftDetails.Price = shitProductCompany.AmtProductCost;
                    listProductShiftDetails.ProductSysNo = shitProductCompany.ProductSysNo;
                    listProductShiftDetails.OutCompany = shitProductCompany.SapCoCodeFrom;
                    listProductShiftDetails.InCompany = shitProductCompany.SapCoCodeTo;
                    listProductShiftDetails.AtTotalAmt = shitProductCompany.AmtProductCost;
                    listProductShiftDetails.UnitCost = decimal.Parse((listProductShiftDetails.Price.Value / listProductShiftDetails.Quantity.Value).ToString("#.00"));
                    productList.Add(listProductShiftDetails);
                }
            }
            if (productList.Count > 0)
            {
                //对需要上传的移仓单按照仓库进行排序/////////////////////////////////////
                productList = productList.OrderBy(item => item.WarehouseNumber).ToList();
                /////////////////////////////////////////////////////////////////////////
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = TransactionManager.DefaultTimeout;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {

                    DA.InsertProductShiftDetails(productList, GoldenTaxNo);

                    DA.InsertProductShiftCompany(productList, GoldenTaxNo, ServiceContext.Current.UserSysNo);

                    scope.Complete();
                }
                return true;
            }
            return false;
        }

        private List<ShiftProductCompanyEntity> Parse(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            List<ShiftProductCompanyEntity> result = new List<ShiftProductCompanyEntity>((int)(table.Rows.Count * 1.3));
            DataRow row;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                ShiftProductCompanyEntity entity = new ShiftProductCompanyEntity();
                try
                {
                    entity.ShiftSysNo = string.IsNullOrEmpty(row[0].ToString().Trim()) ? 0 : int.Parse(row[0].ToString().Trim());
                    entity.ProductSysNo = string.IsNullOrEmpty(row[1].ToString().Trim()) ? 0 : int.Parse(row[1].ToString().Trim());
                    entity.ProductID = row[2].ToString().Trim();
                    entity.ProductName = row[3].ToString().Trim();
                    entity.ShiftQty = string.IsNullOrEmpty(row[4].ToString().Trim()) ? 0 : int.Parse(row[4].ToString().Trim());
                    entity.UnitCostCount = string.IsNullOrEmpty(row[5].ToString().Trim()) ? 0.00m : decimal.Parse(row[5].ToString().Trim());
                    entity.AmtProductCost = string.IsNullOrEmpty(row[6].ToString().Trim()) ? 0.00m : decimal.Parse(row[6].ToString().Trim());
                    entity.SapCoCodeFrom = row[7].ToString().Trim();
                    entity.SapCoCodeTo = row[8].ToString().Trim();
                }
                catch (FormatException)
                {
                    if (i == 0)
                    {
                        continue;
                    }
                    //throw new BizException("模板数据格式不正确，无法识别");
                    ThrowBizException("ProductShift_TempFormatError");
                }
                catch (OverflowException)
                {
                    //throw new BizException("模板数据格式不正确，无法识别");
                    ThrowBizException("ProductShift_TempFormatError");
                }
                entity.AmtCount = 0.0m;
                entity.AmtTaxItem = 0.0m;
                entity.AtTotalAmt = 0.0m;

                result.Add(entity);
            }
            return result;
        }

        private void SetProductShiftDetailGoldenTaxNo(List<ProductShiftDetailEntity> listProductShiftDetails, string GoldenTaxNo, int? stockA)
        {
            for (int i = 0; i < listProductShiftDetails.Count; i++)
            {
                listProductShiftDetails[i].OrderID = GoldenTaxNo;
                listProductShiftDetails[i].IsSplit = 0;
                listProductShiftDetails[i].PayType = "";
                listProductShiftDetails[i].OutTime = DateTime.Now;
                listProductShiftDetails[i].Status = 0;
                listProductShiftDetails[i].OrginComputerNo = GetOrginComputerNo(stockA);
                listProductShiftDetails[i].InvoiceType = 0;
                listProductShiftDetails[i].Discount = 0;
                listProductShiftDetails[i].Unit = "";
                listProductShiftDetails[i].OrderType = "SHIFT";
            }
        }

        private string GetOrginComputerNo(int? stockA)
        {
            string result = "";
            List<SysConfigEntity> sysList = DA.GetSysysProductList(stockA);
            foreach (SysConfigEntity entity in sysList)
            {
                if (entity.Key == stockA.ToString().Trim())
                {
                    result = entity.Value;
                    break;
                }
            }
            if (result == "")
            {
                //throw new BizException("未配置key值为：" + stockA.Value.ToString() + "的value值");
                ThrowBizException("ProductShift_ValueNotSetting", stockA.Value.ToString());
                return string.Empty;
            }
            else
            {
                return result;
            }
        }


        internal void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        internal string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.ProductShift, msgKeyName), args);
        }
    }
}
