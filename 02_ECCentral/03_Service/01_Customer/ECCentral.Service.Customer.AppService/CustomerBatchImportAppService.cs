using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Utility;
using System.IO;
using System.Data;
using ECCentral.BizEntity;
using ECCentral.Service.Customer.BizProcessor;
using System.Data.OleDb;

namespace ECCentral.Service.Customer.AppService
{
    [VersionExport(typeof(CustomerBatchImportAppService))]
    public class CustomerBatchImportAppService
    {
        /// <summary>
        /// 批量导入用户
        /// </summary>
        /// <param name="customers">用户列表</param>
        /// <returns>执行批量操作返回的错误信息</returns>
        public virtual string BatchImportCustomer(CustomerBatchImportInfo batchInfo)
        {
            StringBuilder sb = new StringBuilder();
            DataTable excelData = null;
            List<CustomerInfo> customers = null;

            string tempFilePath = Path.Combine(FileUploadManager.BaseFolder, Encoding.UTF8.GetString(Convert.FromBase64String(batchInfo.ImportFilePath)));
            try
            {
                excelData = ReadExcelFileToDataTable(tempFilePath);
                if (excelData != null && excelData.Rows != null && excelData.Rows.Count > 0)
                {
                    customers = ParseExcelData(excelData, batchInfo);
                }
            }
            catch (Exception ex)
            {
                //读取文件格式异常
                sb.AppendLine(ResouceManager.GetMessageString("Customer.BatchImportCustomer", "ExcelFormatError") + "\n" + ex.Message);
                throw new BizException(sb.ToString());
            }

            sb.AppendLine(ResouceManager.GetMessageString("Customer.BatchImportCustomer", "PromptingMessage"));
            int successCount = 0;
            int failedCount = 0;
            if (customers != null && customers.Count > 0)
            {
                var processor = ObjectFactory<CustomerProcessor>.Instance;
                foreach (var customer in customers)
                {
                    try
                    {
                        processor.CreateCustomer(customer);
                        successCount++;
                    }
                    catch (Exception exp)
                    {
                        sb.AppendLine(exp.Message);
                        failedCount++;
                    }
                }
                return string.Format(sb.ToString(), customers.Count, successCount, failedCount);
            }
            return ResouceManager.GetMessageString("Customer.BatchImportCustomer", "NullExcel");
        }


        private DataTable ReadExcelFileToDataTable(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;";
                string SQL = "select * from [sheet1$]";
                OleDbConnection cn = new OleDbConnection(string.Format(strConn, fileName));
                OleDbDataAdapter da = new OleDbDataAdapter(SQL, string.Format(strConn, fileName));

                try
                {
                    DataTable dt = new DataTable();
                    cn.Open();
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (cn != null)
                    {
                        cn.Close();
                    }
                    if (da != null)
                    {
                        da.Dispose();
                    }
                }
            }

            return null;
        }

        private List<CustomerInfo> ParseExcelData(DataTable table, CustomerBatchImportInfo batchInfo)
        {
            BuildDataDelegate buildDataDelegate = GetBuildDelegate(batchInfo.TemplateType.Value);
            if (buildDataDelegate != null)
            {
                List<CustomerInfo> customers = buildDataDelegate.Invoke(table, batchInfo);
                if (customers != null && customers.Count > 0)
                {
                    CommonUtility.RemoveRepeatEntity<CustomerInfo>(customers, (customer1, customer2) =>
                    {
                        if (customer1 == null)
                        {
                            return customer2 == null ? 0 : -1;
                        }
                        else if (customer2 == null)
                        {
                            return 1;
                        }
                        else
                        {
                            return string.Compare(customer1.BasicInfo.CustomerID, customer2.BasicInfo.CustomerID, true);
                        }
                    });
                    CustomerInfo customer = new CustomerInfo();
                    customer.BasicInfo.CustomerID = string.Empty;
                    //以下比较的作用？
                    //int index = customers.BinarySearch(customer);
                    //if (index > -1)
                    //{
                    //    customers.RemoveAt(index);
                    //}
                    return customers;
                }
            }
            return null;
        }

        delegate List<CustomerInfo> BuildDataDelegate(DataTable table, object extend);

        private List<CustomerInfo> BuildAstraZenecaCustomerData(DataTable table, object extend)
        {
            CustomerInfo customer = null;
            List<CustomerInfo> customers = new List<CustomerInfo>();
            string custExcelGender = string.Empty;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i]["Employee ID"] != DBNull.Value
                    && !string.IsNullOrEmpty(table.Rows[i]["Employee ID"].ToString()))
                {
                    customer = new CustomerInfo();
                    customer.BasicInfo.CustomerID = "AstraZeneca-" + table.Rows[i]["Employee ID"].ToString().Trim();

                    if (table.Rows[i]["Chinese Name"] != DBNull.Value
                         && !string.IsNullOrEmpty(table.Rows[i]["Chinese Name"].ToString()))
                    {
                        customer.BasicInfo.CustomerName = table.Rows[i]["Chinese Name"].ToString().Trim();
                    }
                    else
                    {
                        customer.BasicInfo.CustomerName = StringUtility.TrimNull(table.Rows[i]["First Name"]) + StringUtility.TrimNull(table.Rows[i]["Last Name"]);
                    }

                    if (table.Rows[i]["Email"] != DBNull.Value
                        && !string.IsNullOrEmpty(table.Rows[i]["Email"].ToString().Trim()))
                    {
                        customer.BasicInfo.Email = table.Rows[i]["Email"].ToString().Trim();
                    }
                    else
                    {
                        customer.BasicInfo.Email = table.Rows[i]["Employee ID"] + "@abcAZ.com";
                    }
                    if (table.Rows[i]["Gender"] != DBNull.Value
                        && !string.IsNullOrEmpty(table.Rows[i]["Gender"].ToString()))
                    {
                        custExcelGender = table.Rows[i]["Gender"].ToString().Trim();
                        custExcelGender = ((custExcelGender.ToUpper() == "男") || (custExcelGender.ToUpper() == "M")) ? "1" : "0";
                        customer.BasicInfo.Gender = (Gender)int.Parse(custExcelGender);
                    }
                    customer.BasicInfo.DwellAddress = StringUtility.TrimNull(table.Rows[i]["Office Address"]);
                    customer.BasicInfo.DwellZip = StringUtility.TrimNull(table.Rows[i]["Post Code"]);
                    customer.BasicInfo.CellPhone = StringUtility.TrimNull(table.Rows[i]["Mobile Phone Number"]);
                    customer.BasicInfo.Phone = customer.BasicInfo.CellPhone;
                    ApplyAstraZenecaDefaultValue(customer);
                    customers.Add(customer);
                }
            }

            return customers;
        }

        private List<CustomerInfo> BuildRicoherCustomerData(DataTable table, object extend)
        {
            CustomerInfo customer = null;
            List<CustomerInfo> customers = new List<CustomerInfo>();
            string custExcelGender = string.Empty;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                custExcelGender = string.Empty;
                if (table.Rows[i]["Employee ID"] != DBNull.Value
                    && !string.IsNullOrEmpty(table.Rows[i]["Employee ID"].ToString()))
                {
                    customer = new CustomerInfo();
                    customer.BasicInfo.CustomerID = "Ricoh-" + table.Rows[i]["Employee ID"].ToString().Trim();

                    if (table.Rows[i]["Chinese Name"] != DBNull.Value
                        && !string.IsNullOrEmpty(table.Rows[i]["Chinese Name"].ToString()))
                    {
                        customer.BasicInfo.CustomerName = table.Rows[i]["Chinese Name"].ToString().Trim();
                    }
                    else
                    {
                        customer.BasicInfo.CustomerName = StringUtility.TrimNull(table.Rows[i]["First Name"]) + StringUtility.TrimNull(table.Rows[i]["Last Name"]);
                    }

                    if (table.Rows[i]["Email"] != DBNull.Value
                        && StringUtility.IsEmailAddress(table.Rows[i]["Email"].ToString().Trim()))
                    {
                        customer.BasicInfo.Email = table.Rows[i]["Email"].ToString().Trim();
                    }
                    else
                    {
                        customer.BasicInfo.Email = customer.BasicInfo.CustomerID + "@Ricoh.com";
                    }

                    if (table.Rows[i]["Gender"] != DBNull.Value
                        && !string.IsNullOrEmpty(table.Rows[i]["Gender"].ToString()))
                    {
                        custExcelGender = table.Rows[i]["Gender"].ToString().Trim();
                        custExcelGender = ((custExcelGender.ToUpper() == "男")
                            || (custExcelGender.ToUpper() == "M")) ? "1" : "0";
                        customer.BasicInfo.Gender = (Gender)int.Parse(custExcelGender);
                    }

                    customer.BasicInfo.DwellAddress = StringUtility.TrimNull(table.Rows[i]["Office Address"]);
                    customer.BasicInfo.DwellZip = StringUtility.TrimNull(table.Rows[i]["Post Code"]);
                    customer.BasicInfo.CellPhone = StringUtility.TrimNull(table.Rows[i]["Mobile Phone Number 1"]);
                    customer.BasicInfo.CellPhone = customer.BasicInfo.CellPhone;
                    ApplyRicohDefaultValue(customer);
                    customers.Add(customer);
                }
            }

            return customers;
        }

        private List<CustomerInfo> BuildVipCustomerData(DataTable table, object extend)
        {
            CustomerBatchImportInfo batchInfo = (CustomerBatchImportInfo)extend;
            CustomerInfo customer = null;
            List<CustomerInfo> customers = new List<CustomerInfo>();
            string custExcelName = string.Empty;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i]["CustomerID"] != DBNull.Value
                    && !string.IsNullOrEmpty(table.Rows[i]["CustomerID"].ToString()))
                {
                    customer = new CustomerInfo();

                    customer.BasicInfo.CustomerID = StringUtility.TrimNull(table.Rows[i]["CustomerID"]);
                    if (table.Rows[i]["Email"] != DBNull.Value
                      && StringUtility.IsEmailAddress(table.Rows[i]["Email"].ToString().Trim()))
                    {
                        customer.BasicInfo.Email = table.Rows[i]["Email"].ToString().Trim();
                    }
                    else
                    {
                        customer.BasicInfo.Email = string.Empty;
                    }
                    customer.BasicInfo.CustomerName = StringUtility.TrimNull(table.Rows[i]["联系人名称"]);
                    customer.BasicInfo.Phone = StringUtility.TrimNull(table.Rows[i]["电话/手机"]);
                    customer.BasicInfo.CellPhone = customer.BasicInfo.Phone;
                    customer.BasicInfo.FromLinkSource = batchInfo.FromLinkSource.ToString();
                    customer.BasicInfo.DwellAddress = StringUtility.TrimNull(table.Rows[i]["客户名称"]);
                    ApplyVIPDefaultValue(customer, i);
                    customers.Add(customer);
                }
            }

            return customers;
        }

        private void ApplyRicohDefaultValue(CustomerInfo data)
        {
            if (data != null)
            {
                data.BasicInfo.Status = 0;
                data.BasicInfo.IsEmailConfirmed = false;
                data.IsSubscribe = true;
                data.ValidScore = 0;
                data.TotalScore = 0;
                data.Rank = CustomerRank.Ferrum;//初级
                data.AuctionRank = 0;
                data.BasicInfo.DwellAreaSysNo = -999999;
                data.BasicInfo.RegisterTime = DateTime.Now;

                data.PasswordInfo.Password = "1234";
                data.ValidPrepayAmt = 0.00M;
                data.AccountPeriodInfo.IsAllowComment = false;
                data.TotalSOMoney = 0.00M;
                data.VIPRank = VIPRank.NormalManual;
                data.BasicInfo.FromLinkSource = "Ricoh";
                data.AccountPeriodInfo.IsUseChequesPay = true;
                data.CustomersType = CustomerType.Enterprise;//Ricoh

                //账期默认信息
                data.AccountPeriodInfo.AccountPeriodDays = 0;
                data.AccountPeriodInfo.AvailableCreditLimit = 0.0m;
                data.AccountPeriodInfo.TotalCreditLimit = 0.0m;
            }
        }

        private void ApplyAstraZenecaDefaultValue(CustomerInfo data)
        {
            if (data != null)
            {
                data.BasicInfo.Status = 0;
                data.BasicInfo.IsEmailConfirmed = true;
                data.IsSubscribe = true;
                data.ValidScore = 0;
                data.TotalScore = 0;
                data.Rank = CustomerRank.Copper;//青铜
                data.AuctionRank = 0;
                data.BasicInfo.DwellAreaSysNo = -999999;
                data.BasicInfo.RegisterTime = DateTime.Now;
                data.CustomersType = CustomerType.Enterprise;

                data.PasswordInfo.Password = "1234";
                data.ValidPrepayAmt = 0.00M;
                data.AccountPeriodInfo.IsAllowComment = false;
                data.TotalSOMoney = 0.00M;
                data.VIPRank = VIPRank.NormalAuto;
                data.BasicInfo.FromLinkSource = "AstraZeneca";
                data.PointExpiringDate = DateTime.Now;
                data.AccountPeriodInfo.IsUseChequesPay = true;

                //账期默认信息
                data.AccountPeriodInfo.AccountPeriodDays = 0;
                data.AccountPeriodInfo.AvailableCreditLimit = 0.0m;
                data.AccountPeriodInfo.TotalCreditLimit = 0.0m;
            }
        }

        private void ApplyVIPDefaultValue(CustomerInfo data, int i)
        {
            if (data != null)
            {
                data.BasicInfo.Status = 0;
                data.BasicInfo.IsEmailConfirmed = false;
                data.IsSubscribe = true;
                data.ValidScore = 0;
                data.TotalScore = 0;
                data.Rank = CustomerRank.Ferrum;
                data.AuctionRank = 0;
                data.BasicInfo.DwellAreaSysNo = -999999;
                data.BasicInfo.RegisterTime = DateTime.Now;
                data.ValidPrepayAmt = 0.00M;
                data.AccountPeriodInfo.IsAllowComment = false;
                data.TotalSOMoney = 0.00M;
                data.VIPRank = VIPRank.NormalAuto;
                data.PasswordInfo.Password = ReturnPWDString(i);
                data.PointExpiringDate = DateTime.Now;
                data.AccountPeriodInfo.IsUseChequesPay = true;
                data.CustomersType = CustomerType.Personal;

                //账期默认信息
                data.AccountPeriodInfo.AccountPeriodDays = 0;
                data.AccountPeriodInfo.AvailableCreditLimit = 0.0m;
                data.AccountPeriodInfo.TotalCreditLimit = 0.0m;
            }
        }

        private string ReturnPWDString(int seed)
        {
            Random rand = new Random(seed);
            int n = rand.Next(100000, 999999);
            string sn = n.ToString();
            for (int j = 0; j < 6; j++)
            {
                char ch = Convert.ToChar(rand.Next(65, 90));
                if (ch == 'E' || ch == 'O' || ch == 'S' || ch == 'B' || ch == 'I' || ch == 'L' || ch == 'D' || ch == 'C' || ch == 'G')
                    break;
                sn = sn.Replace(rand.Next(1, 9).ToString(), ch.ToString());
            }
            sn = sn.Replace("0", rand.Next(2, 9).ToString());
            sn = sn.Replace("1", rand.Next(2, 9).ToString());
            sn = sn.Replace("5", rand.Next(6, 9).ToString());
            return sn;
        }

        private BuildDataDelegate GetBuildDelegate(TemplateType templateType)
        {
            switch (templateType)
            {
                case TemplateType.AstraZeneca:
                    return BuildAstraZenecaCustomerData;
                case TemplateType.Ricoh:
                    return BuildRicoherCustomerData;
                case TemplateType.VIP:
                    return BuildVipCustomerData;
                default:
                    return null;
            }
        }
    }
}
