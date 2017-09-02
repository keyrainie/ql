using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using IPP.OrderMgmt.JobV31.BusinessEntities.FetchDubiousUser;
using IPP.OrderMgmt.JobV31.Dac.FetchDubiousUser;
using IPP.OrderMgmt.JobV31.AppEnum;
using IPP.OrderMgmt.JobV31.Dac.Common;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Biz.FetchDubiousUser
{
    public class SOFetchDubiousUserBP 
    {
        private const string CCPAYTYPE = "CREDITCARD_PAYTYPE";
        private const string INTERORDER = "PromotionIntel2009Q4";
        
        #region PreData List

        /// <summary>
        /// 自动审核一次最多提取的单数
        /// </summary>
        private static int TopCount = 2000;
        /// <summary>
        /// 是否检查串货标识
        /// </summary>
        private static bool IsCheckChuanDan = false;

        /// <summary>
        /// 是否检查串货标识
        /// </summary>
        private static bool IsCheckChaoHuo = false;
        /// <summary>
        /// 一个月内被物流拒收过单拒的客户编号
        /// </summary>
        private static List<int> AutoRMACustomerSysNos;
        /// <summary>
        /// 业务日志文件
        /// </summary>
        private static string BizLogFile;
        /// <summary>
        /// 支付方式列表
        /// </summary>
//        private static List<PayTypeEntity> payTypeList;

        /// <summary>
        /// 用户一年内拒收订单比例
        /// </summary>
        private static float RejectionPercent;

        private static List<UsersOfOrderEntity> userList;
        private static List<int> customerSysNoList;

        //断货订单支持仓库
        private static List<string> OutStockList = new List<string>();
        #endregion

        public static void GetOprationUserInfo()
        {
            SOFetchDubiousUserDA.UserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
            SOFetchDubiousUserDA.UserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
            SOFetchDubiousUserDA.CompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
            SOFetchDubiousUserDA.StoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
            SOFetchDubiousUserDA.StoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
        }

        /// <summary>
        /// 从配置文件获取判断恶意用户时的一些参数
        /// </summary>
        /// <returns></returns>
        public static void GetParameterFromContext(JobContext jc)
        {
            //UserDisplayName = ConfigurationManager.AppSettings["SendMailUserDisplayName"];
            //UserLoginName = ConfigurationManager.AppSettings["SendMailUserLoginName"];
            //CompanyCode = ConfigurationManager.AppSettings["SendMailCompanyCode"];
            //StoreCompanyCode = ConfigurationManager.AppSettings["SendMailStoreCompanyCode"];
            //StoreSourceDirectoryKey = ConfigurationManager.AppSettings["SendMailStoreSourceDirectoryKey"];
            int soRejectionPercentFromConfig;

            try
            {
                soRejectionPercentFromConfig = int.Parse(jc.Properties["RejectionPercent"]);
                if ((soRejectionPercentFromConfig < 0) && (soRejectionPercentFromConfig > 100))
                {
                    soRejectionPercentFromConfig = 30;
                }
            }
            catch (Exception e)
            {
                soRejectionPercentFromConfig = 30;
            }

            RejectionPercent = soRejectionPercentFromConfig / 100F;
  //          WriteLog(string.Format("读取系统默认拒收率:{0}", RejectionPercent));
        }

        private static void FetchRejectionUserList()
        {
            //提取所有一年内下过订单的Customer SysNo
             userList = SOFetchDubiousUserDA.GetDistinctUsersFromSOList();

             customerSysNoList = userList.Select(csys=>csys.CustomerSysNo).Distinct().ToList<int>();

  //          WriteLog(string.Format("提取待审查拒收用户共计:{0}", userList.Count));

            foreach (int customerSysNo in customerSysNoList)
            {
                float UserRP = SOFetchDubiousUserDA.GetUserRP(customerSysNo);

                if (UserRP >= RejectionPercent)
                {
   //                 WriteLog(string.Format("顾客:{0} 拒收率为:{1}", x.CustomerSysNo, UserRP));

                    SOFetchDubiousUserDA.AddRejectionUsers(customerSysNo, 0, "CustomerSysNo");

  //                  WriteLog(string.Format("顾客:{0} 添加顾客编号到拒收用户列表", x.CustomerSysNo));

                    SOFetchDubiousUserDA.AddRejectionUsers(customerSysNo, 1, "ReceiveCellPhone");

  //                  WriteLog(string.Format("顾客:{0} 添加手机号到拒收用户列表", x.CustomerSysNo));

                 //   SOFetchDubiousUserDA.AddRejectionUsers(x.CustomerSysNo, 2, "ReceivePhone");
                    List<SingleValueEntity> listUserPhone = SOFetchDubiousUserDA.GetRejectionUserPhone(customerSysNo);

                    foreach (SingleValueEntity up in listUserPhone)
                    {
                        if (!string.IsNullOrEmpty(up.StringValue))
                        {
                            if (up.StringValue.IndexOf(",") >= 0)
                            {
                                string[] singlePhone = up.StringValue.Split(',');
                                foreach (string sp in singlePhone)
                                {
                                    SOFetchDubiousUserDA.AddSinglePhoneNumber(sp, 1);
                                    //                              WriteLog(string.Format("顾客:{0} 添加拆分固话到拒收用户列表", x.CustomerSysNo));
                                }
                            }
                            else if (up.StringValue.IndexOf("，") >= 0)
                            {
                                string[] singlePhone = up.StringValue.Split('，');
                                foreach (string sp in singlePhone)
                                {
                                    SOFetchDubiousUserDA.AddSinglePhoneNumber(sp, 1);
                                    //                              WriteLog(string.Format("顾客:{0} 添加拆分固话到拒收用户列表", x.CustomerSysNo));
                                }
                            }
                            else
                            {
                                //                           WriteLog(string.Format("顾客:{0} 添加固定电话号到拒收用户列表", x.CustomerSysNo));
                                SOFetchDubiousUserDA.AddSinglePhoneNumber(up.StringValue, 1);
                            }
                        }
                    }

                    SOFetchDubiousUserDA.AddRejectionUsers(customerSysNo, 3, "ReceiveAddress");
 //                   WriteLog(string.Format("顾客:{0} 添加收货地址到拒收用户列表", x.CustomerSysNo));
                }
                else
                {
  //                  WriteLog(string.Format("顾客:{0} 从拒收用户列表移除", x.CustomerSysNo));
                    SOFetchDubiousUserDA.RemoveRejectionUser(customerSysNo);
                }
            }   

        }

        private static void FetchRejectionUserList2()
        {
            //提取所有一年内下过订单的Customer SysNo
            List<AddressOfOrderEntity> addressList = SOFetchDubiousUserDA.GetDistinctAddressFromSOList();

   //         WriteLog(string.Format("提取待审查拒收地址共计:{0}", addressList.Count));

            foreach (AddressOfOrderEntity x in addressList)
            {
                if (string.IsNullOrEmpty(x.ReceiveAddress))
                    continue;

                SOFetchDubiousUserDA.AddRejectionUsers2(x.ReceiveAddress);

 //               WriteLog(string.Format("地址:{0} 被添加相关信息到拒收用户列表", x.ReceiveAddress));

                List<SingleValueEntity> listUserPhone = SOFetchDubiousUserDA.GetRejectionUserPhone2(x.ReceiveAddress);

                foreach (SingleValueEntity up in listUserPhone)
                {
                    if (!string.IsNullOrEmpty(up.StringValue))
                    {
                        if (up.StringValue.IndexOf(",") >= 0)
                        {
                            string[] singlePhone = up.StringValue.Split(',');
                            foreach (string sp in singlePhone)
                            {
                                SOFetchDubiousUserDA.AddSinglePhoneNumber(sp, 1);
                            }
                        }
                        else if (up.StringValue.IndexOf("，") >= 0)
                        {
                            string[] singlePhone = up.StringValue.Split('，');
                            foreach (string sp in singlePhone)
                            {
                                SOFetchDubiousUserDA.AddSinglePhoneNumber(sp, 1);
                            }
                        }
                        else
                        {
                            SOFetchDubiousUserDA.AddSinglePhoneNumber(up.StringValue, 1);
                        }
                    }
                }

            }

        }

        private static void FetchOccupyStockUserList()
        {
            
//            List<OccupyStockUserEntity> userList = SOFetchDubiousUserDA.GetOccupyStockUserList();

//            WriteLog(string.Format("提取待审查占库存用户共计:{0} 条", userList.Count));


                List<int> OccupyCustomerList = new List<int>();
                List<string> RemoveCustomerList = new List<string>();
                List<OccupyStockUserEntity> occupyList = new List<OccupyStockUserEntity>();    

                int ExpiredSOCount = 0;
                int CurrentUsrNo = 0;
                int maxExpiredSOCount = 0;
                StringBuilder strbRemoveCustomerSyNo = new StringBuilder();

                foreach (int customerSysNo in customerSysNoList)
                {
                    List<OccupyStockUserEntity> curruserList = SOFetchDubiousUserDA.GetOccupyStockUserList(customerSysNo);

                    if (curruserList.Count > 0)
                        CurrentUsrNo = curruserList[0].CustomerSysNo;

                    ExpiredSOCount = 0;
                    maxExpiredSOCount = 0;

                    foreach (OccupyStockUserEntity x in curruserList)
                    {

                        if (OccupyCustomerList.Contains(x.CustomerSysNo))
                            continue;

                        if (CurrentUsrNo != x.CustomerSysNo)
                        {
                            if (!OccupyCustomerList.Contains(CurrentUsrNo) && (maxExpiredSOCount < 3))
                            {
                                RemoveCustomerList.Add("'" + CurrentUsrNo.ToString() + "'");
                                //SOFetchDubiousUserDA.RemoveOccupyStockUser(CurrentUsrNo);
                            }
                            ExpiredSOCount = 0;
                            maxExpiredSOCount = 0;
                            CurrentUsrNo = x.CustomerSysNo;
                        }

                        if ((x.Status < 0) && (x.Status > -5))
                        {
                            ExpiredSOCount++;
                            if (((ExpiredSOCount >= 3) && (x.totalAMT <= 0)) || ((ExpiredSOCount >= 6) && (x.totalAMT > 0)))
                            {
                                OccupyCustomerList.Add(CurrentUsrNo);
                                occupyList.AddRange(curruserList.Where(cl => (cl.CustomerSysNo == x.CustomerSysNo && cl.Status > -5 && cl.Status < 0)).ToList());
                                break;
                            }
                        }
                        else
                        {
                            //Added the codes below for identify the customers who
                            //have less than 6 exp orders but greater than 2.
                            //They would be maintain the status quo and don't be
                            //removed from dubious user list. --20100528S1
                            if (ExpiredSOCount > maxExpiredSOCount)
                            {
                                maxExpiredSOCount = ExpiredSOCount;
                            }
                            //--20100528S1

                            ExpiredSOCount = 0;
                        }

                    }

                    if (!OccupyCustomerList.Contains(CurrentUsrNo) && (maxExpiredSOCount < 3))
                    {
                        RemoveCustomerList.Add("'" + CurrentUsrNo.ToString() + "'");
                        //SOFetchDubiousUserDA.RemoveOccupyStockUser(CurrentUsrNo);
                    }

                }

                if (RemoveCustomerList.Count > 0)
                {
                    strbRemoveCustomerSyNo.Append("(");
                    strbRemoveCustomerSyNo.Append(string.Join(",", RemoveCustomerList.ToArray()));
                    strbRemoveCustomerSyNo.Append(")");
                    SOFetchDubiousUserDA.RemoveOccupyStockUser(strbRemoveCustomerSyNo.ToString());
                }


                var distinctList = occupyList.Select(ocItem => new { ocItem.CustomerSysNo, ocItem.ReceiveAddress, ocItem.ReceiveCellPhone, ocItem.ReceivePhone }).Distinct().ToList();


                //            WriteLog(string.Format("下列用户将从占库存用户列表中移除ID：{0}", strbRemoveCustomerSyNo.ToString()));

                //            WriteLog(string.Format("共有{0}名用户相关信息将被加入占库存用户", OccupyCustomerList.Count));

                //occupyList.ForEach(item =>
                //{

                   // var singleOSUser = curruserList.Where(x => (x.CustomerSysNo == item && x.Status > -5 && x.Status < 0)).ToList();

                    //                WriteLog(string.Format("顾客:{0} 共有:{1}条订单信息将被加入占库存用户", item, singleOSUser.Count));
                    //if (singleOSUser != null)
                    //{
                        distinctList.ForEach(su =>
                        {

                            SOFetchDubiousUserDA.AddOccupyStockUsers(0, su.CustomerSysNo.ToString());
                            //                    WriteLog(string.Format("顾客:{0} 添加顾客编号到占库存用户列表", su.CustomerSysNo));

                            if (!String.IsNullOrEmpty(su.ReceiveCellPhone))
                            {
                                SOFetchDubiousUserDA.AddOccupyStockUsers(1, su.ReceiveCellPhone);
                                //                        WriteLog(string.Format("顾客:{0} 添加手机号码到占库存用户列表", su.CustomerSysNo));
                            }

                            if (!String.IsNullOrEmpty(su.ReceivePhone))
                            {

                                if (su.ReceivePhone.IndexOf(",") >= 0)
                                {
                                    string[] singlePhone = su.ReceivePhone.Split(',');
                                    foreach (string sp in singlePhone)
                                    {
                                        if (!string.IsNullOrEmpty(sp))
                                            SOFetchDubiousUserDA.AddSinglePhoneNumber(sp, 0);
                                    }
                                }
                                else if (su.ReceivePhone.IndexOf("，") >= 0)
                                {
                                    string[] singlePhone = su.ReceivePhone.Split('，');
                                    foreach (string sp in singlePhone)
                                    {
                                        if (!string.IsNullOrEmpty(sp))
                                            SOFetchDubiousUserDA.AddSinglePhoneNumber(sp, 0);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(su.ReceivePhone))
                                    {
                                        SOFetchDubiousUserDA.AddSinglePhoneNumber(su.ReceivePhone, 0);
                                        //                                WriteLog(string.Format("顾客:{0} 添加固话号码到占库存用户列表", su.CustomerSysNo));
                                    }
                                }

                            }

                            if (!String.IsNullOrEmpty(su.ReceiveAddress))
                            {
                                SOFetchDubiousUserDA.AddOccupyStockUsers(3, su.ReceiveAddress);
                                //                        WriteLog(string.Format("顾客:{0} 添加收货地址到占库存用户列表", su.CustomerSysNo));
                            }

                        });
                    //}

                //});
        }

        private static void FetchSpiteUserList()
        {
            SOFetchDubiousUserDA.AddSpiteUsers();

            List<ExpiredSpiteCustomerEntity> sc =  SOFetchDubiousUserDA.GetExpiredSpiteCustomers();

        //    string idStr = "";
            StringBuilder strbSpiteCustomersIDList = new StringBuilder();

            strbSpiteCustomersIDList.Append("(");

            foreach (ExpiredSpiteCustomerEntity esc in sc)
            {
                strbSpiteCustomersIDList.Append("'");
                strbSpiteCustomersIDList.Append(esc.Content);
                strbSpiteCustomersIDList.Append("'");
                strbSpiteCustomersIDList.Append(",");
                //idStr += "'" + esc.Content + "',";
            }     

            if (strbSpiteCustomersIDList.Length <= 1) return;

            strbSpiteCustomersIDList.Remove(strbSpiteCustomersIDList.Length - 1, 1);

            strbSpiteCustomersIDList.Append(")");

          //  idStr = "(" + idStr.Substring(0, idStr.Length - 1) + ")" ;

            SOFetchDubiousUserDA.RemoveSpiteCustomers(strbSpiteCustomersIDList.ToString());
        }

        /// <summary>
        /// 开始获取可疑用户
        /// </summary>
        /// <param name="jobContext">Job运行上下文</param>
        public static void FetchUser(JobContext jobContext)
        {
            BizLogFile = jobContext.Properties["BizLog"];
            
            GetParameterFromContext(jobContext);            

            GetOprationUserInfo();

            //Retrive the rejection users
            StartCheckAll(1, jobContext);

            FetchRejectionUserList();

            FetchRejectionUserList2();
            
            EndCheckAll(1, jobContext);

            //Retrive the Occupy stock users
            StartCheckAll(0, jobContext);
            
            FetchOccupyStockUserList();
            
            EndCheckAll(0, jobContext);

            //Retrive Spite Users
            StartCheckAll(2, jobContext);

            FetchSpiteUserList();

            EndCheckAll(2, jobContext);

            //提前取得所有支付方式的列表
   //         payTypeList = CommonDA.GetPayTypeList();            



            EndCheckAll(4, jobContext);
        }

        /// <summary>
        /// 本批全部检查结束
        /// </summary>
        /// <param name="entity"></param>
        private static void EndCheckAll(int nType, JobContext jobContext)
        {

            switch (nType)
            {
                case 1:
                    WriteLog("完成提取拒收用户列表:" + DateTime.Now.ToString());
                break;
                case 0:
                    WriteLog("完成提取恶意占库存用户列表:" + DateTime.Now.ToString());
                break;
                case 2:
                    WriteLog("完成提取恶意用户列表:" + DateTime.Now.ToString());
                break;
                case 4:
                    jobContext.Message += "本次检查结束:" + DateTime.Now.ToString() + Environment.NewLine;
                    WriteLog("本次Job执行结束:" + DateTime.Now.ToString());
                break;
            }

        }

        /// <summary>
        /// 本批全部检查结束
        /// </summary>
        /// <param name="entity"></param>
        private static void StartCheckAll(int nType, JobContext jobContext)
        {
            switch(nType)
            {
                case 1:
                    WriteLog("开始提取拒收用户列表:" + DateTime.Now.ToString());
                break;
                case 0:
                    WriteLog("开始提取恶意占库存用户列表:" + DateTime.Now.ToString());
                break;
                case 2:
                    WriteLog("开始提取恶意用户列表:" + DateTime.Now.ToString());
                break;
                case 4:
                    WriteLog("开始本次Job执行:" + DateTime.Now.ToString());
                break;
            }
        }

        private static void WriteLog(string content)
        {
            Log.WriteLog(content, BizLogFile);
            Console.WriteLine(content);
        }

    }
}
