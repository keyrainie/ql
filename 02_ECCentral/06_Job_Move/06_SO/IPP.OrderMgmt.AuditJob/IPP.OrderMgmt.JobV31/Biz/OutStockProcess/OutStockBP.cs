using System;
using System.Collections.Generic;
using System.Configuration;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.OrderMgmt.JobV31.BusinessEntities.OutStock;
using IPP.OrderMgmt.JobV31.Dac.OutStockSO;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.DataContracts;
using Newegg.Oversea.Framework.Contract;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.ServiceContracts;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces;
using Newegg.Oversea.Framework.ServiceConsole.Client;

namespace IPP.OrderMgmt.JobV31.Biz.OutStockProcess
{
    public class OutStockBP 
    {
        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;

        //调用服务需要传入的公共信息
        private static string UserDisplayName;
        private static string UserLoginName;
        private static string CompanyCode;
        private static string StoreCompanyCode;
        private static string StoreSourceDirectoryKey;

        /// <summary>
        /// 调用服务需要传入的公共信息
        /// </summary>
        /// <returns></returns>
        private static void GetAutoAuditSendMessageLoginInfo()
        {
            UserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
            UserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
            CompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
            StoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
            StoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
        }
    
        //断货订单支持仓库
        private static List<string> OutStockList = new List<string>();

        #region Entry Point
        /// <summary>
        /// 开始检测订单的本地仓状态
        /// </summary>
        /// <param name="jobContext">Job运行上下文</param>
        public static void Run(JobContext jobContext)
        {
            BizLogFile = jobContext.Properties["BizLog"];

            List<SOEntity4OutStockEntity> list = OutStockSODA.GetOutStockSOList(CompanyCode);

            if (list == null || list.Count == 0)
            {
                jobContext.Message += "没有附合条件的记录";
                WriteLog("没有附合条件的记录");

                return;
            }
            else
            {
                jobContext.Message += string.Format(" 共获取到{0}条满足条件的记录",list.Count);
                WriteLog( string.Format(" 共获取到{0}条满足条件的记录",list.Count));
            }
            int successCount = 0;
            IMaintainSOManualChangeWHV31 service = ServiceBroker.FindService<IMaintainSOManualChangeWHV31>();
            GetAutoAuditSendMessageLoginInfo();
            foreach (SOEntity4OutStockEntity so in list)
            {
                
                // 如果3天没有备到货，改为超时未到货
                TimeSpan ts = DateTime.Now.Subtract(so.OrderDate);
                if (ts.TotalHours > 72)
                {
                    OutStockSODA.UpdateSOStockStatus(so.SysNo, 2);
                    continue;
                }
                
                // 如果已经到货，修改订单仓库并修改订单为到货可审
                if (OutStockSODA.IsItemAvail(so.SysNo, so.LocalWHSysNo))
                {
                    List<SOItem4OutStockEntity> soItems =  OutStockSODA.GetSOItem4OutStock(so.SysNo, so.LocalWHSysNo);
                    foreach(SOItem4OutStockEntity soitem in soItems)
                    {
                        try
                        {
                            UpdateSOStockStatus(so.SysNo, soitem.ProductID, soitem.ProductSysNo, soitem.Quantity,
                                int.Parse(soitem.WarehouseNumber), int.Parse(so.LocalWHSysNo), service);
                            
                            successCount++;
                            jobContext.Message += string.Format("成功修改订单仓库，单号：{0}\r\n", so.SysNo);
                            WriteLog(string.Format("成功修改订单仓库，单号：{0}", so.SysNo));
                        }
                        catch (Exception ex)
                        {
                            jobContext.Message += string.Format("修改仓库时发生异常,单号：{0},异常：{1}\r\n", so.SysNo, ex.Message);
                            WriteLog(string.Format("修改仓库时发生异常,单号：{0},异常：{1}", so.SysNo, ex.Message));
                        }
                    }
                }
            }
          
        }

        #endregion Entry Point


        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }

        private static void UpdateSOStockStatus(int soSysNO, string productID, int productSysNo,int Qty, int stockSysNo, 
            int targetStockSysNo, IMaintainSOManualChangeWHV31 service)
        {
            BatchMessageV31<SOManualChangeWHMsg> message = new BatchMessageV31<SOManualChangeWHMsg>();
            message.Header = new Newegg.Oversea.Framework.Contract.MessageHeader()
                {
                    OperationUser = new OperationUser(UserDisplayName, UserLoginName, StoreSourceDirectoryKey, CompanyCode),
                    CompanyCode = CompanyCode,
                    StoreCompanyCode = StoreCompanyCode,
                    FromSystem = "127.0.0.1"
                };
            message.Messages = new List<SOManualChangeWHMsg>();
            message.Messages.Add( new SOManualChangeWHMsg{ 
                ProductId = productID,
                ProductSysNo = productSysNo,
                Quantity = Qty,
                SOSysNo = soSysNO,
                StockSysNo = stockSysNo,
                TargetStockSysNo = targetStockSysNo
            });

            message = service.BatchMaintainSOWHUpdate(message);

            if (message.Faults != null
                && message.Faults.Count > 0)
            {
                string tmpSendMessageException = message.Faults[0].ErrorDescription + "\r\n" +
                   message.Faults[0].ErrorDetail;
                throw (new Exception(tmpSendMessageException));
            }
            else
            {
                OutStockSODA.UpdateSOStockStatus(soSysNO, 0);
            }
        }
    }
}