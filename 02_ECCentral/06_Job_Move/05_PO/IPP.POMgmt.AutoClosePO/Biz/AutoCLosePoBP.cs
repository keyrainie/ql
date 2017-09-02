using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;
using AutoClose.Model;
using AutoClose.DAL;
using System.Xml;
using ECCentral.BizEntity.Invoice;
using ECCentral.Job.Utility;
using ECCentral.BizEntity.Inventory;

namespace AutoClose.Biz
{
    public static class AutoCLosePoBP
    {
        #region Field
        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static ILog log = LogerManger.GetLoger();
        private static JobContext CurrentContext;

        // private static int AllOK = 0;
        #endregion
        public static void DoWork(JobContext currentContext)
        {
            CurrentContext = currentContext;
            List<PoSysNoItem> listPo = AutoCloseDA.GetNeedClesePoSysno();
            foreach (PoSysNoItem item in listPo)
            {
                SetPoInfo(item);
                OnShowInfo(string.Format("编号为{0}的PO单进行了关闭操作！", item.PoSysNo));
            }
        }
        private static void OnShowInfo(string info)
        {
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            Console.WriteLine(info);
            log.WriteLog(info);
            if (CurrentContext != null)
            {
                CurrentContext.Message += info + Environment.NewLine;
            }
        }
        public static void SetPoInfo(PoSysNoItem item)
        {
            decimal noUseReturnPoint = GetNoUseReturnPoint(item);
            bool result = false;
            result = CreateNewItemPay(item, noUseReturnPoint);      //创建财务调整单
            if (result == false) { return; }
            //SetReturnPoint(item);       //修改使用返点
            SetToAmt(item);             //设置总金额
            InventoryPurQtyChange(item);//修改在途库存
            SendMail(item);                 //发送邮件
            SetPoState6To8(item);       //修改PO状态

            //CRL17821 发送关闭SSB消息
            SendCloseMessage(item.PoSysNo, 493);
        }
        public static decimal GetNoUseReturnPoint(PoSysNoItem item)
        {
            decimal noUseReturnPoint = 0;
            List<POEimsEntity> eimsInfo = AutoCloseDA.GetPOEimsRelevanceInfo(item.PoSysNo);
            foreach (POEimsEntity eims in eimsInfo)
            {
                noUseReturnPoint += eims.LeftAmt;
            }
            return noUseReturnPoint;
        }
        private static bool CreateNewItemPay(PoSysNoItem item, decimal noUseReturnPoint)
        {
            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["InvoiceRestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];

            PayableInfo payableInfo = new PayableInfo();
            List<POItem> items = AutoCloseDA.QueryPOItemsForPrint(item.PoSysNo);
            decimal? totalAmt = GettoTalAmt(items);
            decimal? trueAmt = GettrueAmt(items);

            payableInfo.OrderSysNo = item.PoSysNo;
            payableInfo.BatchNumber = 1;
            payableInfo.OrderStatus = 6;
            payableInfo.OrderType = PayableOrderType.POAdjust;
            payableInfo.OperationUserFullName = "Job User";
            payableInfo.InStockAmt = totalAmt - trueAmt - noUseReturnPoint;
            payableInfo.CompanyCode = companyCode;

            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update("/Payable/CreateByPO", payableInfo , out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");

                OnShowInfo(errorMsg);
                OnShowInfo("PO单编号为：" + item.PoSysNo.ToString() + "财务收款单调整失败");
                return false;
            }
            else
            {
                OnShowInfo("PO单编号为：" + item.PoSysNo.ToString() + "财务收款单调整成功");
                return true;
            }
        }
        private static decimal? GettrueAmt(List<POItem> entitys)
        {
            return entitys.Sum(p => p.Quantity * p.OrderPrice);
        }

        private static decimal? GettoTalAmt(List<POItem> entitys)
        {
            return entitys.Sum(p => p.PurchaseQty * p.OrderPrice);
        }
        private static void SendMail(PoSysNoItem item)
        {
            List<POMailInfo> list = AutoCloseDA.GetNeedSendMailPo(item.PoSysNo);
            if (list != null)
            {
                foreach (POMailInfo poMailInfo in list)
                {
                    CreateEmailContent cretet = new CreateEmailContent(poMailInfo.Sysno, "");
                    string mailCountent = cretet.EmailInfo();
                    cretet.SendMail(mailCountent, poMailInfo.MailAddress + ";" + poMailInfo.PMEmail + ";" + poMailInfo.CreateEmail);
                }
            }
            OnShowInfo("邮件发送完成");
        }
        //如果PO单有使用EIMS，当EIMS金额(PO_Master. UsingReturnPoint)超过该PO各批次入库总金额（具体表和字段需WMS提供）调用EIMS接口将多余EIMS金额返还，并将PO总价格TotalAmt更新为各批次入库总金额。
        public static void SetReturnPoint(PoSysNoItem item)
        {
            //List<POEmisInfo> eimsInfo = AutoCloseDA.GetPoEmisInfo(item.PoSysNo);
            //foreach (POEmisInfo emis in eimsInfo)
            //{
            //    if (emis.sumEimsCount < emis.UsingReturnPoint)
            //    {
            //        ResumePMRemnantReturnPoint(emis.SysNo, emis.PM_ReturnPointSysNo, emis.UsingReturnPoint - emis.sumEimsCount, 500, emis.ReturnPointC3SysNo, item.PoStatus);
            //        AutoCloseDA.UpdateUseReturnPoint(emis);
            //        OnShowInfo("返点操作成功");
            //    }
            //}

            //调EIMS接口返还多余返点                          
            //int operationUserSysNumber = 493;

            //List<POAttachInfo> poAttachEntitys = new List<POAttachInfo>();
            //List<POEimsEntity> poEimsEntitys = GetPOEimsRelevanceInfo(item.PoSysNo);
            //foreach (POEimsEntity poEims in poEimsEntitys)
            //{
            //    if (poEims.LeftAmt > 0.00m)
            //    {
            //        POAttachInfo poAttachInfo = new POAttachInfo()
            //        {
            //            PONumber = item.PoSysNo,
            //            InvoiceNumber = poEims.EIMSNo,
            //            PostTime = System.DateTime.Now,
            //            UseTime = System.DateTime.Now,
            //            POStatus = (int)item.PoStatus,
            //            C3SysNo = -1
            //        };
            //        poAttachEntitys.Add(poAttachInfo);
            //    }
            //}
            //ResumePMRemnantReturnPoint(poAttachEntitys, operationUserSysNumber.ToString());
            //OnShowInfo("返点操作成功");

        }
        public static List<POEimsEntity> GetPOEimsRelevanceInfo(int poSysNo)
        {
            List<POEimsEntity> poEimsRelevanceList = new List<POEimsEntity>();
            poEimsRelevanceList = AutoCloseDA.GetPOEimsRelevanceInfo(poSysNo);
            return poEimsRelevanceList;
        }
        /// <summary>
        /// 恢复返点金额
        /// </summary>
        /// <param name="POSysNo">PO编号</param>
        /// <param name="InvoiceNumber ">返点编号</param>
        /// <param name="useInvoiceAmount">当前使用金额</param>
        /// <returns></returns>
        //public static void ResumePMRemnantReturnPoint(List<POAttachInfo> poAttachEntitys, string userSysNo)
        //{
        //    EIMSMessageResult returnValue = null;
        //    IEIMSInterfaceService service = null;
        //    try
        //    {
        //        EIMSMessage<List<POAttachInfo>> entity = new EIMSMessage<List<POAttachInfo>>()
        //        {
        //            Header = new EIMSMessageHeader()
        //            {
        //                UserID = userSysNo,
        //                CompanyCode = Settings.CompanyCode
        //            },
        //            Body = poAttachEntitys
        //        };
        //        service = ServiceBroker.FindService<IEIMSInterfaceService>(Settings.ConsumerName, Settings.EIMSLocationName);
        //        returnValue = service.PostPOConfirm(entity);
        //    }
        //    finally
        //    {
        //        ServiceBroker.DisposeService<IEIMSInterfaceService>(service);
        //    }

        //}


        //private static void ResumePMRemnantReturnPoint(int poSysNo, int invoiceNumber, decimal useInvoiceAmount, int userSysNo, int c3SysNo, int poStatus)
        //{
        //    EIMSMessageResult returnValue = null;
        //    IEIMSInterfaceService service = null;
        //    try
        //    {
        //        EIMSMessage<POAttachInfo> entity = new EIMSMessage<POAttachInfo>()
        //        {
        //            Header = new EIMSMessageHeader()
        //            {
        //                UserID = userSysNo.ToString(),
        //                CompanyCode = Settings.CompanyCode
        //            },
        //            Body = new POAttachInfo()
        //            {
        //                PONumber = poSysNo,
        //                InvoiceNumber = invoiceNumber,
        //                PostTime = System.DateTime.Now,
        //                UseTime = System.DateTime.Now,
        //                C3SysNo = c3SysNo,
        //                POStatus = poStatus
        //            }
        //        };
        //        service = ServiceBroker.FindService<IEIMSInterfaceService>(Settings.ConsumerName, Settings.EIMSLocationName);
        //        returnValue = service.PostPOConfirm(entity);
        //    }
        //    finally
        //    {
        //        ServiceBroker.DisposeService<IEIMSInterfaceService>(service);
        //    }
        //}
        //入库总金额=该PO各批次入库总金额
        public static void SetToAmt(PoSysNoItem item)
        {
            AutoCloseDA.SetCountToamt(item.PoSysNo);
            OnShowInfo("PO单总金额调整完成");
        }
        //建立Job，对于最后一次Task打印时间起超过30天未继续到货的“部分入库（status=6）”状态PO，系统则自动关闭该PO，将PO的状态修改为“系统关闭 8”
        public static void SetPoState6To8(PoSysNoItem item)
        {
            string closeUser = "493/自动关闭";
            AutoCloseDA.SetStatus6To8(item.PoSysNo, closeUser);
            OnShowInfo("PO单状态修改完成");
        }
        //（PO中该商品采购数量 –实际入库数量） 进行扣减采购在途库存（inventory .purQty）；
        //负po不需要进行采购在途库存设置。关闭时，需要把占用库存剩下的还给可用库存
        public static void InventoryPurQtyChange(PoSysNoItem poInfo)
        {
            List<PoPurQtyInfo> listPurQty = AutoCloseDA.GetPoPurQtyInfoList(poInfo.PoSysNo);
            //var data = listPurQty.GroupBy(p => p.poSysno);
            //int storySyno = 0;
            //foreach (var d in data)
            //{
            //    List<KeyValuePair<int, int>> kv = new List<KeyValuePair<int, int>>();
            //    foreach (var f in d)
            //    {
            //        storySyno = f.StockSysNo;
            //        kv.Add(new KeyValuePair<int, int>(f.productSysno, f.pruCount));
            //    }
            //    if (storySyno != 0)
            //    {
            //        InventoryPurQty(storySyno, kv, d.Key, 2);//0 审核； 1 撤销审核；2 中止入库
            //        storySyno = 0;
            //    }
            //}
            InventoryAdjustContractInfo info = new InventoryAdjustContractInfo();
            info.SourceActionName = InventoryAdjustSourceAction.StopInStock;
            info.SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order;
            info.AdjustItemList = new List<InventoryAdjustItemInfo>();
            info.ReferenceSysNo = poInfo.PoSysNo.ToString();
            foreach (var item in listPurQty)
            {
                info.AdjustItemList.Add(new InventoryAdjustItemInfo
                {
                    AdjustQuantity = -1 * item.pruCount,
                    ProductSysNo = item.productSysno,
                    StockSysNo = item.StockSysNo
                });
            }

            string baseUrl = System.Configuration.ConfigurationManager.AppSettings["PORestFulBaseUrl"];
            string languageCode = System.Configuration.ConfigurationManager.AppSettings["LanguageCode"];
            string companyCode = System.Configuration.ConfigurationManager.AppSettings["CompanyCode"];
            ECCentral.Job.Utility.RestClient client = new ECCentral.Job.Utility.RestClient(baseUrl, languageCode);
            ECCentral.Job.Utility.RestServiceError error;
            var ar = client.Update("/PurchaseOrder/AdjustPurchaseOrderQtyInventory", info, out error);
            if (error != null && error.Faults != null && error.Faults.Count > 0)
            {
                string errorMsg = "";
                foreach (var errorItem in error.Faults)
                {
                    errorMsg += errorItem.ErrorDescription;
                }
                Logger.WriteLog(errorMsg, "JobConsole");

                OnShowInfo(errorMsg);
                OnShowInfo("PO单编号为：" + poInfo.PoSysNo.ToString() + "在途数量修改失败");
                AutoCloseDA.SendEmail(poInfo.PoSysNo, Settings.ExceptionMeil, "PO单编号为：" + poInfo.PoSysNo.ToString() + "在途数量修改失败", errorMsg);
            }
            else
            {
                OnShowInfo("PO单编号为：" + poInfo.PoSysNo.ToString() + "在途数量修改成功");
            }
        }
        private static bool IsSSBEnabled()
        {
            var status = AutoCloseDA.GetPOOfflineStatus();

            return status == "Y";
        }

        public static int SendCloseMessage(int poSysNo, int userSysNo)
        {
            #region 模版

            string poTemplate = @"<Publish xmlns=""http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService"">
	                                    <FromService>http://soa.newegg.com/SOA/USA/InventoryManagement/V30/E5DBS01/NeweggOZZOService</FromService>
	                                    <ToService>http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService</ToService>
	                                    <RouteTable>
		                                    <Article xmlns=""http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService"">
			                                    <ArticleCategory>WMS</ArticleCategory>
			                                    <ArticleType1>Download</ArticleType1>
			                                    <ArticleType2>Newegg</ArticleType2>
		                                    </Article>
	                                    </RouteTable>
	                                    <Node>
		                                    <MessageHead>
			                                    <!-- 确定由哪一个SP来处理当前逻辑 -->
			                                    <MessageType>POClose</MessageType>
			                                    <!-- 版本号 -->
			                                    <Version>1.0</Version>
			                                    <!-- 商家编号 -->
			                                    <CompanyCode>{CompanyCode}</CompanyCode>
			                                    <!-- Merchant PO号(PONumber) -->
			                                    <ReferenceNumber>{PONumber}</ReferenceNumber>
		                                    </MessageHead>
		                                    <Body>
			                                    <!-- PONumber 商户PO编号 -->
			                                    <PONumber>{PONumber}</PONumber>
			                                    <!-- 关闭备注 -->
			                                    <Memo>{Memo}</Memo>
		                                    </Body>
	                                    </Node>
                                    </Publish>";

            #endregion

            if (!IsSSBEnabled())
            {
                return 0;
            }

            NewPOEntity po = AutoCloseDA.GetPOMaster(poSysNo);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(poTemplate);
            doc.GetElementsByTagName("Memo")[0].InnerText = po.Memo;
            doc.GetElementsByTagName("PONumber")[0].InnerText = po.SysNo.ToString();
            doc.GetElementsByTagName("ReferenceNumber")[0].InnerText = po.SysNo.ToString();
            doc.GetElementsByTagName("CompanyCode")[0].InnerText = po.CompanyCode;
            //string message = poTemplate.Replace("{PONumber}", po.SysNo.ToString())
            //                            .Replace("{CompanyCode}", po.CompanyCode)
            //                            .Replace("{Memo}", po.Memo);
            string message = doc.InnerXml;


            AutoCloseDA.CreatePOSSBLog(new POSSBLogEntity
            {
                POSysNo = poSysNo,
                Content = message,
                ActionType = "C",
                InUser = userSysNo,
                SendErrMail = "N",
                CompanyCode = Settings.CompanyCode,
                LanguageCode = Settings.LanguageCode,
                StoreCompanyCode = Settings.StoreCompanyCode
            });

            return AutoCloseDA.CallSSBMessageSP(message);
        }
    }
}
