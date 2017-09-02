using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Newegg.Oversea.Framework.DataAccess;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.Oversea.CN.CustomerMgmt.SplitPhone
{
    public class CustomerBIZ: IJobAction
    {
        private const int pageSize = 500;
        private  string path = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
        private const string path1 = "TimeStamp1.txt";
        private const string path2 = "TimeStamp2.txt";
        Int64 timeStampPara1 = 0;
        Int64 timeStampPara2 = 0;
        private JobContext _context = null;
        //public CustomerBIZ()
        //{
        //    GetTimestampParmeter();
        //}

        public void ShowMessage(string message)
        {
            if (_context == null)
            {
                Console.WriteLine(message);
            }
            else
            {
                _context.Message += message + Environment.NewLine;
            }
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            _context = context;
            timeStampPara1 = 0;
            timeStampPara2 = 0;
            ShowMessage("Starting....");
            #region init
            GetTimestampParmeter();
            #endregion
            StartWork();
            ShowMessage("Finish");
            Console.ReadLine();
        }

        #endregion

        private void GetTimestampParmeter()
        {
            string timeStamp = null;

            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                fs = new FileStream(Path.Combine(path,path1), FileMode.Open);
                sr = new StreamReader(fs);
                timeStamp = sr.ReadLine();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
                fs.Close();
            }

            if (string.IsNullOrEmpty(timeStamp))
            {
                timeStamp = "0";
            }

            Int64 dd = Int64.Parse(timeStamp);

            timeStampPara1 = dd;
           
            try
            {
                fs = new FileStream(Path.Combine(path, path2), FileMode.Open);
                sr = new StreamReader(fs);
                timeStamp = sr.ReadLine();
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
                fs.Close();
            }

            if (string.IsNullOrEmpty(timeStamp))
            {
                timeStamp = "0";
            }

            dd = Int64.Parse(timeStamp);
            timeStampPara2 = dd;
        }

        public void StartWork()
        {
            try
            {
                int totalCount = 0;
                int startNumber = 0;
                List<Customer> customers = null;

                do
                {
                    customers = GetCustomer(startNumber, pageSize, timeStampPara1);
                    totalCount = customers.Count;
                    //var result = customers.Find(c => c.Phone == "15982858081");
                    //if (result != null)
                    //{
                    //    ShowMessage(result.CustomerID);
                    //}
                    if (customers != null || customers.Count > 0)
                    {
                        DoWork(customers);
                        startNumber += pageSize;
                        ShowMessage("Customer " + startNumber.ToString() + " done!");
                    }
                    else
                    {
                        break;
                    }
                }
                while (totalCount == pageSize);

                totalCount = 0;
                startNumber = 0;
                customers = null;

                do
                {
                    customers = GetCustomerPhone(startNumber, pageSize, timeStampPara2);
                    totalCount = customers.Count;

                    if (customers != null || customers.Count > 0)
                    {
                        DoWork(customers);
                        startNumber += pageSize;
                        ShowMessage("ShippingAddress " + startNumber.ToString() + " done!");
                    }
                    else
                    {
                        break;
                    }
                }
                while (totalCount == pageSize);

                UpdateTimeStamp();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                Console.ReadLine();
            }

        }

        private void DoWork(List<Customer> customers)
        {
            List<Customer> parsedCustomers = null;
            string sql = string.Empty;

            parsedCustomers = ParsePhone(customers);
            RemoveRepeatCustomerPhone(parsedCustomers);
            RemoveExistedCustomerPhone(parsedCustomers);
            sql = BuildInsertSql(parsedCustomers);
            CreateCustomerPhone(sql);
        }

        private List<Customer> GetCustomer(int pageIndex, int pageSize, Int64 sqlTimeStamp)
        {
            List<Customer> result = null;

            DataCommand dc = DataCommandManager.GetDataCommand("GetCustomerPhone");
            dc.SetParameterValue("@StartNumber", pageIndex);
            dc.SetParameterValue("@EndNumber", pageIndex + pageSize);
            dc.SetParameterValue("@TimeStamp", sqlTimeStamp);
            result = dc.ExecuteEntityList<Customer>();
            return result;
        }

        private List<Customer> GetCustomerPhone(int pageIndex, int pageSize, Int64 sqlTimeStamp)
        {
            List<Customer> result = null;

            DataCommand dc = DataCommandManager.GetDataCommand("GetCustomerReceivePhone");
            dc.SetParameterValue("@StartNumber", pageIndex);
            dc.SetParameterValue("@EndNumber", pageIndex + pageSize);
            dc.SetParameterValue("@TimeStamp", sqlTimeStamp);
            result = dc.ExecuteEntityList<Customer>();
            return result;
        }

        private List<Customer> ParsePhone(List<Customer> customers)
        {
            List<Customer> result = new List<Customer>();
            Hashtable tmpPhones = new Hashtable();
            char[] spliter = { '，', ',', '；', ';', ' ' };
            string[] phones = null;
            Customer phoneCustomer = null;

            foreach (Customer customer in customers)
            {
                if (!string.IsNullOrEmpty(customer.Phone) || !string.IsNullOrEmpty(customer.CellPhone))
                {
                    if (string.IsNullOrEmpty(customer.Phone))
                    {
                        customer.Phone = string.Empty;
                    }

                    if (string.IsNullOrEmpty(customer.CellPhone))
                    {
                        customer.CellPhone = string.Empty;
                    }

                    phones = (customer.Phone + "," + customer.CellPhone).Split(spliter);
                }


                if (phones != null && phones.Length > 0)
                {
                    foreach (string phone in phones)
                    {
                        if (((Regex.IsMatch(phone.Trim(), @"^(13|15|18)[0-9]\d{8}$")
                            || Regex.IsMatch(phone.Trim(), @"^\d{2,4}[-]?\d{7,8}([-]?\d{1,4})?$")))
                            && Regex.IsMatch(phone.Trim(), @"^[^\u4e00-\u9fa5]+$"))
                        {
                            phoneCustomer = new Customer();
                            phoneCustomer.CustomerID = customer.CustomerID;
                            phoneCustomer.CustomerSysNo = customer.CustomerSysNo;
                            phoneCustomer.Phone = phone;
                            if (phoneCustomer.Phone.Length > 20)
                            {
                                phoneCustomer.Phone = phoneCustomer.Phone.Substring(0, 20);
                            }
                            if (Encoding.Default.GetByteCount(phoneCustomer.Phone) > 20)
                            {
                                continue;
                            }
                            result.Add(phoneCustomer);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            return result;
        }

        private void RemoveRepeatCustomerPhone(List<Customer> customers)
        {
            List<Customer> result = new List<Customer>();
            customers.ForEach(item =>
            {
                if (!result.Exists(r=>r.CustomerSysNo==item.CustomerSysNo && r.Phone == item.Phone))
                {
                    result.Add(item);
                }
            });
            customers.Clear();
            customers.AddRange(result);
        }

        private void RemoveExistedCustomerPhone(List<Customer> customers)
        {
            string xml = BuildCustomersXML(customers);
            List<Customer> existedCustomers = GetExistedCustomers(xml);
            customers.RemoveAll(c => existedCustomers.Exists(ec => ec.CustomerSysNo == c.CustomerSysNo && ec.Phone == c.Phone));
        }

        private string BuildInsertSql(List<Customer> customers)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            
            if (customers != null && customers.Count > 0)
            {
                string sqlCustomerPhoneColumns = @"INSERT INTO [dbo].[CustomerPhone] ([CustomerSysNo],[Phone],[CustomerID])  ";

                sqlBuilder.Append(sqlCustomerPhoneColumns);
                foreach (Customer entity in customers)
                {
                    if (string.IsNullOrEmpty(entity.Phone))
                    {
                        continue;
                    }
                    else
                    {
                        if (entity.Phone.Replace("'", "").Replace(" ", "").Length != 0)
                        {
                            entity.Phone = entity.Phone.Replace("'", "''");
                        }
                        else
                        {
                            continue;
                        }

                        if (entity.CustomerID.Replace("'", "").Replace(" ", "").Length != 0)
                        {
                            entity.CustomerID = entity.CustomerID.Replace("'", "''");
                        }
                        else
                        {
                            continue;
                        }
                    }
                    
                    sqlBuilder.Append("\r\n SELECT ");
                    sqlBuilder.Append(entity.CustomerSysNo.ToString());
                    sqlBuilder.Append(",'");
                    sqlBuilder.Append(entity.Phone.Trim());
                    sqlBuilder.Append("','");
                    sqlBuilder.Append(entity.CustomerID.Trim());
                    sqlBuilder.Append("'");
                    sqlBuilder.Append("\r\n");
                    sqlBuilder.Append("UNION ALL \r\n");
                    
                }
            }

            int index = 0;
            string sql = sqlBuilder.ToString();
            index = sql.LastIndexOf("UNION ALL");
            if (index > -1)
            {
                sql = sql.Substring(0, index);
            }
            else
            {
                sql = string.Empty;
            }
            return sql;
        }

        private void CreateCustomerPhone(string sqlInsert)
        {
            if (!string.IsNullOrEmpty(sqlInsert))
            {
                try
                {
                    DataCommand dc = DataCommandManager.CreateCustomDataCommand("OverseaCustomerManagement", CommandType.Text, sqlInsert);
                    dc.ExecuteNonQuery();
                }
                catch 
                {

                }                
            }
        }

        private void GetMaxUpdateTime(out string max1, out string max2)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetMaxUpdateTime");
            dc.SetParameterValue("TimeStamp1", timeStampPara1);
            dc.SetParameterValue("TimeStamp2", timeStampPara2);
            dc.ExecuteNonQuery();
            max1 = dc.GetParameterValue("UpdateTime1").ToString();
            max2 = dc.GetParameterValue("UpdateTime2").ToString();
        }
        private void WriteException(string message)
        { }
        private void UpdateTimeStamp()
        {
            string timeStamp1 = string.Empty;
            string timeStamp2 = string.Empty;
            GetMaxUpdateTime(out timeStamp1, out timeStamp2);

            FileStream fs = null;
            StreamWriter sw = null;
            if (!string.IsNullOrEmpty(timeStamp1))
            {
                try
                {
                    fs = new FileStream(Path.Combine(path, path1), FileMode.Truncate);
                    sw = new StreamWriter(fs);
                    sw.Write(timeStamp1);
                }
                catch (IOException ex)
                {
                    throw ex;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }
            if (!string.IsNullOrEmpty(timeStamp2))
            {
                try
                {
                    fs = new FileStream(Path.Combine(path, path2), FileMode.Truncate);
                    sw = new StreamWriter(fs);
                    sw.Write(timeStamp2);
                }
                catch (IOException ex)
                {
                    throw ex;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }
        }

        private string BuildCustomersXML(List<Customer> customers)
        {
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");

            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                writer.WriteStartElement("Customers");

                foreach (Customer customer in customers)
                {
                    writer.WriteStartElement("Customer");
                    writer.WriteElementString("CustomerSysNo", customer.CustomerSysNo.ToString());
                    writer.WriteElementString("Phone", customer.Phone);
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("Customers");
                writer.Flush();
            }

            return builder.ToString();
        }

        private List<Customer> GetExistedCustomers(string xml)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetExistedCustomerPhone");
            dc.SetParameterValue("@Customers", xml);
            List<Customer> existedCustomers = dc.ExecuteEntityList<Customer>();
            return existedCustomers;
        }

        
    }
}