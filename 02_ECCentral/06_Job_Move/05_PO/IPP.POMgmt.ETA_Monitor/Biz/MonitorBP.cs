using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using System.ServiceModel;
using IPPOversea.POmgmt.Model;
using IPPOversea.Invoicemgmt.DAL;
using Newegg.Oversea.Framework.Contract;
using System.Transactions;
using IPPOversea.POmgmt.ETA.Model;
using IPPOversea.Invoicemgmt.AR_Invoice_SO_Monitor.DAL;
using ECCentral.BizEntity.Inventory;
using ECCentral.Job.Utility;


namespace IPPOversea.Invoicemgmt.Biz
{
    public static class MonitorBP
    {
        #region Field

        public delegate void ShowMsg(string info);
        public static ShowMsg ShowInfo;
        private static Mutex mut = new Mutex(false);
        private static ILog log = LogerManger.GetLoger();
        // private static int AllOK = 0;

        #endregion

        private static void OnShowInfo(string info)
        {
            mut.WaitOne();
            if (ShowInfo != null)
            {
                ShowInfo(info);
            }
            mut.ReleaseMutex();
        }

        public static void DoWork(AutoResetEvent are)
        {
            List<POEntity> list = MonitorDA.GetPO(Settings.CompanyCode);
            OnShowInfo(string.Format("找到{0}个审核通过(待入库)的PO单.", list.Count));
            foreach (var item in list)
            {
                if (item.ETATime.HasValue)
                {
                    TimeSpan ts = DateTime.Now - item.ETATime.Value;
                    if (ts.Days > 3)
                    {
                        //当前成本/当前价/最近一次调价时间/上次采购价/上次采购时间/京东价/有效库存/上月销售总量,
                        //以上字段在“待入库”、“已作废”、“系统作废”时记录到数据表中，
                        List<POItem> poItems = MonitorDA.GetPOItemBySys(item.SysNo, Settings.CompanyCode);

                        //系统作废PO单的Job（ETA Job）排除掉待入库的礼品卡PO单
                        if (!CheckHasGiftCard(poItems))
                        {
                            item.Items = poItems;
                            AbandonPO(item);
                        }
                    }
                }
            }
            are.Set();
        }

        public static void SendEmail(string mailSubject, string mailBody)
        {
            MailDA.SendEmail(Settings.MailAddress, mailSubject, mailBody);
        }

        /// <summary>
        ///  判断该po单的item中是否包含礼品卡
        /// </summary>
        /// <param name="poItems"></param>
        /// <returns></returns>
        public static bool CheckHasGiftCard(List<POItem> poItems)
        {
            string cardlistStr = Settings.GiftCardIDList;
            var cardlist = cardlistStr.Split(',');

            foreach (POItem pitem in poItems)
            {
                foreach (string pID in cardlist)
                {
                    if (string.Equals(pitem.ProductID, pID))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void AbandonPO(POEntity PO)
        {
            string logInfo = string.Empty;
            string halfday = string.Empty;
            if (PO.ETAHalfDay == "AM")
            {
                halfday = "上午";
            }
            else if (PO.ETAHalfDay == "PM")
            {
                halfday = "下午";
            }
            logInfo = string.Format("POSysNo：{0},StockSysNo：{1} ,ETATime：{2}{3}", PO.SysNo, PO.StockSysNo, PO.ETATime.Value.ToShortDateString(), halfday);
            try
            {
                //if (PO.PM_ReturnPointSysNo > 0)
                //{
                //    EIMSOper(PO);
                //}
                int userSysNo = 493;
                #region 返还返点信息
                //List<POAttachInfo> poAttachEntitys = new List<POAttachInfo>();
                //List<POEimsEntity> poEimsEntitys = GetPOEimsRelevanceInfo(PO.SysNo);
                //if (poEimsEntitys != null && poEimsEntitys.Count > 0)
                //{
                //    foreach (POEimsEntity poEims in poEimsEntitys)
                //    {
                //        POAttachInfo poAttachInfo = new POAttachInfo()
                //        {
                //            PONumber = PO.SysNo,
                //            InvoiceNumber = poEims.EIMSNo,
                //            UseInvoiceAmount = poEims.EIMSAmt * -1,
                //            PostTime = System.DateTime.Now,
                //            UseTime = System.DateTime.Now,
                //            C3SysNo = -1
                //        };
                //        poAttachEntitys.Add(poAttachInfo);
                //    }
                //    EIMSOper(poAttachEntitys, userSysNo.ToString(), PO.SysNo);
                //}
                #endregion

                InventoryOper(PO);
                
                //但放到前面则会报：等待入库和等待分配的采购单只能添加预付款单！
                //为已付款的PO产生POAjust时需在作废PO单之前，否则报业务异常：已作废、关闭、初始状态的采购单不能添加付款单！
                var fpItem = MonitorDA.GetFinacePayItemByPOSysNo(PO.SysNo);
                if (fpItem != null && fpItem.AvailableAmt>0) //如果找到已付款的则调用财务接品生成POAjust单
                {
                    MonitorDA.InsertFinancePayAndItem( fpItem,userSysNo);
                }

                MonitorDA.AbandonPO(PO.SysNo, Settings.CompanyCode);
                MonitorDA.AbandonETA(PO.SysNo);
                foreach (var poItem in PO.Items)
                {
                    MonitorDA.UpdateExtendPOInfo(poItem.SysNo, Settings.CompanyCode, poItem.ProductSysNo);
                }

                //CRL17821 发送关闭消息
                SendCloseMessage(PO.SysNo, userSysNo);

                Logger.WriteLog("JOB自动作废PO单Success" + logInfo, "JobConsole", PO.SysNo.ToString());
                OnShowInfo(string.Format("采购单{0}作废成功.", PO.SysNo));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(DateTime.Now + "JOB自动作废PO单Failed。" + logInfo + "\r\n" + ex.Message.ToString(),
                    "JobConsole",
                    PO.SysNo.ToString()
                    );
                OnShowInfo(string.Format("采购单{0}作废失败:\r\n{1}.", PO.SysNo, ex.Message));
            }
        }
        public static List<POEimsEntity> GetPOEimsRelevanceInfo(int poSysNo)
        {
            List<POEimsEntity> poEimsRelevanceList = new List<POEimsEntity>();
            poEimsRelevanceList = MonitorDA.GetPOEimsRelevanceInfo(poSysNo);
            return poEimsRelevanceList;
        }
        //private static void EIMSOper(POEntity PO)
        //{
        //    IEIMSInterfaceService service = null;
        //    try
        //    {
        //        service = ServiceBroker.FindService<IEIMSInterfaceService>(Settings.ConsumerName, Settings.EIMSLocationName);
        //        EIMSMessage<POAttachInfo> entity = new EIMSMessage<POAttachInfo>()
        //        {
        //            Header = new EIMSMessageHeader()
        //            {
        //                UserID = "493",
        //                CompanyCode = Settings.CompanyCode
        //            },
        //            Body = new POAttachInfo()
        //            {
        //                PONumber = PO.SysNo,
        //                InvoiceNumber = PO.PM_ReturnPointSysNo,
        //                UseInvoiceAmount = PO.UsingReturnPoint * -1,
        //                PostTime = System.DateTime.Now,
        //                UseTime = System.DateTime.Now,
        //                C3SysNo = PO.ReturnPointC3SysNo
        //            }
        //        };
        //        var result = service.PostPOCancel(entity);
        //        if (!result.IsSucceed)
        //        {
        //            throw new Exception(result.Error);
        //        }
        //        OnShowInfo(string.Format("采购单{0}需要调整返点成功", PO.SysNo));
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        ServiceBroker.DisposeService<IEIMSInterfaceService>(service);
        //    }
        //}
        //public static void EIMSOper(List<POAttachInfo> poAttachEntitys, string userSysNo, int poSysNo)
        //{
        //    EIMSMessageResult returnValue = null;
        //    IEIMSInterfaceService service = null;
        //    try
        //    {
        //        EIMSMessage<List<POAttachInfo>> entity = new EIMSMessage<List<POAttachInfo>>
        //        {
        //            Header = new EIMSMessageHeader()
        //            {
        //                UserID = userSysNo,
        //                CompanyCode = Settings.CompanyCode
        //            },
        //            Body = poAttachEntitys
        //        };

        //        service = ServiceBroker.FindService<IEIMSInterfaceService>(Settings.ConsumerName, Settings.EIMSLocationName);
        //        returnValue = service.PostPOCancel(entity);
        //        if (!returnValue.IsSucceed)
        //        {
        //            throw new Exception(returnValue.Error);
        //        }
        //        OnShowInfo(string.Format("采购单{0}需要调整返点成功", poSysNo));
        //    }
        //    finally
        //    {
        //        ServiceBroker.DisposeService<IEIMSInterfaceService>(service);
        //    }
        //}

        private static void InventoryOper(POEntity poInfo)
        {
            InventoryAdjustContractInfo info = new InventoryAdjustContractInfo();
            info.SourceActionName = InventoryAdjustSourceAction.AbandonForPO;
            info.SourceBizFunctionName = InventoryAdjustSourceBizFunction.PO_Order;
            info.AdjustItemList = new List<InventoryAdjustItemInfo>();
            info.ReferenceSysNo = poInfo.SysNo.ToString();
            foreach (var item in poInfo.Items)
            {
                info.AdjustItemList.Add(new InventoryAdjustItemInfo
                {
                    AdjustQuantity = -1 * item.PurchaseQty,
                    ProductSysNo = item.ProductSysNo,
                    StockSysNo = poInfo.StockSysNo
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
            }
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

            NewPOEntity po = MonitorDA.GetPOMaster(poSysNo);

            string message = poTemplate.Replace("{PONumber}", po.SysNo.ToString())
                                        .Replace("{CompanyCode}", po.CompanyCode)
                                        .Replace("{Memo}", po.Memo);


            MonitorDA.CreatePOSSBLog(new POSSBLogEntity
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

            return MonitorDA.CallSSBMessageSP(message);
        }

        public static void CreateFinancePayAndItem(POEntity entity, decimal payAmt, int userSysNo)
        {
            
        }

        private static bool IsSSBEnabled()
        {
            var status = MonitorDA.GetPOOfflineStatus();

            return status == "Y";
        }
    }
}
