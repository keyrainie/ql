/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author       :  Kathy.Y.Gao
 * Create Date  :  8/4/2011 
 * Usage        :  
 * File         :  BizProcess.cs   
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.EcommerceMgmt.SendCouponCode.DA;
using IPP.EcommerceMgmt.SendCouponCode.Entities;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;
using System.Transactions;

namespace IPP.EcommerceMgmt.SendCouponCode.Biz
{
    public class BizProcess
    {
        public static JobContext jobContext = null;
        public static string imageText = "http://c1.neweggimages.com.cn/neweggpic2/neg/P80/{0}";
        public static string produceUrl = "http://www.newegg.com.cn/Product/{0}.htm?cm_mmc=emc-_-birthday-_-{1}"; 
        public static string BizLogFile;

        public static void Process()
        {
            int isTestZ = Convert.ToInt32(ConfigurationSettings.AppSettings["IsTestZ"].ToString());
            int isTestS = Convert.ToInt32(ConfigurationSettings.AppSettings["IsTestS"].ToString());
            int isTestR = Convert.ToInt32(ConfigurationSettings.AppSettings["IsTestR"].ToString());
            int isTestB = Convert.ToInt32(ConfigurationSettings.AppSettings["IsTestB"].ToString());
            int IsTestO = Convert.ToInt32(ConfigurationSettings.AppSettings["IsTestO"].ToString());

            //    int notInclude = Convert.ToInt32(ConfigurationSettings.AppSettings["NotInclude"].ToString());  
            try
            {
                List<Coupon> coupons = CouponDA.GetAvailableCoupon();
                WriteLog(DateTime.Now + "查询到优惠券活动" + coupons.Count + "条");
                foreach (Coupon item in coupons)
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    //A不限   R 注册   B 生日   Z 支付宝金账户   O购物送优惠卷
                    //获取自选用户列表
                    List<Customer> customerList = CouponDA.GetLimitCustomerForSpecial(item);
                    #region 
                    if (customerList != null && customerList.Count > 0 && item.BindCondition == "A"
                        && ((item.IsAutoBinding == "Y" && item.BindingDate.ToString("yyyy-MM-dd") == date)
                            || item.IsAutoBinding == "N")
                        && isTestS == 1)//自选年用户送优惠券
                    {
                        WriteLog(DateTime.Now + "为自选用户发放优惠券:" + item.CouponName);
                        ProcessEachForSpecial(item, customerList);
                    }

                    else if (item.BindCondition == "R"
                        && ((item.IsAutoBinding == "Y" && item.BindingDate.ToString("yyyy-MM-dd") == date)
                            || item.IsAutoBinding == "N")
                        && isTestR == 1)//注册送优惠券
                    {
                        WriteLog(string.Format("{0}为注册用户发放优惠券:{1} {2}", DateTime.Now, item.SysNo, item.CouponName));
                        ProcessEachForRigister(item);
                    }
                    else if (item.BindCondition == "B"
                        && ((item.IsAutoBinding == "Y" && item.BindingDate.ToString("yyyy-MM-dd") == date)
                            || item.IsAutoBinding == "N")
                        && isTestB == 1)//生日送优惠券
                    {
                        WriteLog(string.Format("{0}为生日用户发放优惠券:{1} {2}", DateTime.Now, item.SysNo, item.CouponName));
                        ProcessEachForBirthday(item);
                    }
                    else if (item.BindCondition == "Z"
                        && ((item.IsAutoBinding == "Y" && item.BindingDate.ToString("yyyy-MM-dd") == date)
                            || item.IsAutoBinding == "N")
                        && isTestZ == 1)//支付宝金账户优惠券
                    {
                        WriteLog(string.Format("{0}-（新）为支付宝金账户用户发放优惠券:{1} {2}", DateTime.Now, item.SysNo, item.CouponName));
                        ProcessEachForTBVip(item);
                    }
                    #endregion
                    else if (item.BindCondition == "O"
                        && ((item.IsAutoBinding == "Y" && item.BindingDate.ToString("yyyy-MM-dd") == date)
                            || item.IsAutoBinding == "N")
                        && IsTestO == 1)//购物送优惠卷
                    {
                        WriteLog(string.Format("{0}-为购物账户用户发放优惠券:{1} {2}", DateTime.Now, item.SysNo, item.CouponName));
                        ProcessEachForSO(item);
                    }
                }
                if (isTestZ == 1)//支付宝金账户优惠券
                {
                    List<Coupon> aliPayCoupons = CouponDA.GetAliPayCoupon();
                    foreach (Coupon aliItem in aliPayCoupons)
                    {
                        WriteLog(string.Format("{0}-(旧)为支付宝金账户用户发放优惠券:{1} {2}", DateTime.Now, aliItem.SysNo, aliItem.CouponName));
                        ProcessEachForTBVip_Old(aliItem);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("异常信息：" + ex.Message);
            }
            WriteLog(DateTime.Now + "优惠券发放结束。");
           

        }

        #region  支付宝金账户发放优惠券
        /// <summary>
        /// 支付宝金账户发放优惠券
        /// </summary>
        /// <param name="item"></param>
        private static void ProcessEachForTBVip(Coupon master)
        {
            string userType = "Z";//触发条件
            master.BindStatus = "S";//发放状态
            List<Customer> customerList = null;

            customerList = CouponDA.GetAlipayCustomer(userType, master);//获取未发放优惠券的顾客
            if (customerList.Count <= 0)
            {
                WriteLog("没有找到符合条件的支付宝金账户");
                return;
            }

            //更新绑定状态为S 
            CouponDA.UpdateBindRulesStatus(master);
            int num = 0;
            string userList = string.Empty;
            foreach (Customer customer in customerList)
            {
                #region CreateCode

                CouponCode codeEntity = new CouponCode();
                codeEntity.CouponSysNo = master.SysNo;
                GetCodeExpireDate(master, codeEntity);
                codeEntity = CouponDA.CreateCode(codeEntity);

                #endregion

                CouponCodeCustomerLog logEntity = new CouponCodeCustomerLog();
                logEntity.CouponCode = codeEntity.Code;
                logEntity.CouponSysNo = master.SysNo;
                logEntity.CustomerSysNo = customer.CustomerSysNo;
                logEntity.UserCodeType = userType;
                CouponDA.InsertCustomerLog(logEntity);

                #region sendMail
                MailEntity customerMail = new MailEntity();
                customerMail.MailSubject = master.CouponName;

                if (!string.IsNullOrEmpty(customer.MailAddress) && master.IsSendMail == "Y")
                {
                    customerMail.MailBody = ConstValues.MailTemplate.Replace("@InDate", DateTime.Now.ToString("yyyy-MM-dd"))
                            .Replace("@CustomerHello@", customer.CustomerID)
                            .Replace("@CouponCode@", codeEntity.Code)
                            .Replace("@nums@", "1")
                            .Replace("@EndDateStr@", codeEntity.EndDate.ToString("yyyy-MM-dd"));

                    customerMail.MailAddress = customer.MailAddress;
                    CouponDA.SendMailInfo(customerMail);
                }
                #endregion
                num++;
                userList += "顾客编号：" + customer.CustomerSysNo + "，顾客ID：" + customer.CustomerID + "\r\n";
            }
            WriteLog(DateTime.Now + "支付宝金账户送优惠券成功发送" + num + "个顾客，分别是：\r\n" + userList);

        }

        private static void ProcessEachForTBVip_Old(Coupon master)
        {
            string userType = "Z";//触发条件
            master.BindStatus = "S";//发放状态


            List<Customer> customerList = CouponDA.GetAlipayCustomer(userType, master);//获取未发放优惠券的顾客
            WriteLog("获取用户数" + customerList.Count + "个");
            if (customerList.Count <= 0)
            {
                WriteLog("没有找到符合条件的支付宝金账户");
            }
            else
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    string message = string.Format("({0}){1} -- ", master.SysNo, master.CouponName);
                    //通用型优惠券代码
                    CouponCode code = CouponDA.QueryBindingCode(master);

                    CouponDA.UpdateBindRulesStatus(master);

                    int index = 0;
                    string userList = string.Empty;
                    CouponCodeCustomerLog logEntity = new CouponCodeCustomerLog();
                    logEntity.CouponCode = code.Code;
                    logEntity.CouponSysNo = master.SysNo;

                    MailEntity customerMail = new MailEntity();
                    customerMail.MailSubject = master.CouponName;
                    foreach (Customer customer in customerList)
                    {
                        logEntity.CustomerSysNo = customer.CustomerSysNo;
                        logEntity.UserCodeType = userType;
                        CouponDA.InsertCustomerLog(logEntity);

                        if (!string.IsNullOrEmpty(customer.MailAddress) && master.IsSendMail == "Y")
                        {
                            customerMail.MailBody = ConstValues.MailTemplate.Replace("@Title@", master.CouponName)
                                    .Replace("@CustomerHello@", "您好，感谢您对大昌优品的支持。")
                                    .Replace("@CouponCode@", code.Code)
                                    .Replace("@nums@", code.CustomerMaxFrequency.ToString())
                                    .Replace("@EndDateStr@", code.EndDate.ToString("yyyy-MM-dd"));

                            customerMail.MailAddress = customer.MailAddress;
                            CouponDA.SendMailInfo(customerMail);
                        }
                        ++index;
                        userList += "顾客编号：" + customer.CustomerSysNo + "，顾客ID：" + customer.CustomerID + "\r\n";
                    }

                    WriteLog(DateTime.Now + "支付宝金账户优惠券成功发送" + index + "个顾客，分别是：\r\n" + userList);
                    ts.Complete();
                }
            }
        }

        #endregion

        #region 自选用户发放
        private static void ProcessEachForSpecial(Coupon master, List<Customer> customerList)
        {
            if (customerList == null || customerList.Count <= 0)
            {
                WriteLog("没能查询到符合条件的用户");
                return;
            }
            using (TransactionScope ts = new TransactionScope())
            {
                string message = string.Format("({0}){1} -- ", master.SysNo, master.CouponName);
                CouponCode code = null;

                //通用型优惠券代码 只有一条记录
                code = CouponDA.QueryBindingCode(master);

                if (code != null && code.TotalCount > 0)
                {
                    int availCustomerCount = 0;
                    if (code.CustomerMaxFrequency != 0 && code.TotalCount != 0)
                    {
                        availCustomerCount = code.TotalCount / code.CustomerMaxFrequency;
                    }

                    if (availCustomerCount == 0 || availCustomerCount < code.CustomerMaxFrequency)
                    {
                        message += string.Format("（通用型）优惠券不够发放({0})只够发放{1}个客户，需要发放{2}个客户。"
                            , code.Code, availCustomerCount, customerList.Count);
                    }
                }
                WriteLog(message);

                //发放特殊用户的优惠券，直接发放到用户中心
                PCodeForSpecialUser(master, code, customerList);
                ts.Complete();
            }
        }

        public static void PCodeForSpecialUser(Coupon master, CouponCode code, List<Customer> customerList)
        {
            // 自选用户
            string userType = "S";
            master.BindStatus = "E";//字段
            CouponDA.UpdateBindRulesStatus(master);

            int index = 0;
            string userList = string.Empty;
            CouponCodeCustomerLog logEntity = new CouponCodeCustomerLog();
            logEntity.CouponCode = code.Code;
            logEntity.CouponSysNo = master.SysNo;

            MailEntity customerMail = new MailEntity();
            customerMail.MailSubject = master.CouponName;
            foreach (Customer customer in customerList)
            {
                logEntity.CustomerSysNo = customer.CustomerSysNo;
                logEntity.UserCodeType = userType;
                CouponDA.InsertCustomerLog(logEntity);

                //if (!string.IsNullOrEmpty(customer.MailAddress) && master.IsSendMail == "Y")
                //{
                //    customerMail.MailBody = ConstValues.MailTemplate.Replace("@InDate", DateTime.Now.ToString("yyyy-MM-dd"))
                //            .Replace("@CustomerHello@", customer.CustomerID)
                //            .Replace("@CouponCode@", code.Code)
                //            .Replace("@nums@", code.CustomerMaxFrequency.ToString())
                //            .Replace("@EndDateStr@", code.EndDate.ToString("yyyy-MM-dd"));

                //    customerMail.MailAddress = customer.MailAddress;
                //    CouponDA.SendMailInfo(customerMail);
                //}
                ++index;
                userList += "顾客编号：" + customer.CustomerSysNo + "，顾客ID：" + customer.CustomerID + "\r\n";
            }
            CouponDA.UpdateCouponSaleRulesCustomerStatus(master);//自选用户标识已发放
            WriteLog(DateTime.Now + "自选用户优惠券成功发送" + index + "个顾客，分别是：\r\n" + userList);
        }

        #endregion

        #region 注册送优惠券
        private static void ProcessEachForRigister(Coupon master)
        {
            //获取注册未发放优惠券的顾客  （实时发）5分钟一次，造优惠券，发给顾客
            string userType = "R";
            List<Customer> customerList = CouponDA.GetRegisterUser(userType,master);//获取注册未发放优惠券的顾客 
            if (customerList == null || customerList.Count <= 0)
            {
                WriteLog("没能查询到符合条件的用户");
                return;
            }
            WriteLog("查询到新注册的用户" + customerList.Count + "个");
            
            master.BindStatus = "S";//字段       
            CouponDA.UpdateBindRulesStatus(master);
            int num = 0;
            string userList = string.Empty;
            foreach (Customer customer in customerList)
            {
                #region CreateCode

                CouponCode codeEntity = new CouponCode();
                codeEntity.CouponSysNo = master.SysNo;
                codeEntity.CodeType = "C";
                codeEntity.CustomerMaxFrequency = 1;
                GetCodeExpireDate(master, codeEntity);
                codeEntity = CouponDA.CreateCode(codeEntity);

                #endregion

                CouponCodeCustomerLog logEntity = new CouponCodeCustomerLog();
                logEntity.CouponCode = codeEntity.Code;
                logEntity.CouponSysNo = master.SysNo;
                logEntity.CustomerSysNo = customer.CustomerSysNo;
                logEntity.UserCodeType = userType;
                CouponDA.InsertCustomerLog(logEntity);

                #region sendMail
                MailEntity customerMail = new MailEntity();
                customerMail.MailSubject = master.CouponName;

                if (!string.IsNullOrEmpty(customer.MailAddress) && master.IsSendMail == "Y")
                {
                    customerMail.MailBody = ConstValues.MailTemplate.Replace("@InDate", DateTime.Now.ToString("yyyy-MM-dd"))
                            .Replace("@CustomerHello@", customer.CustomerID)
                            .Replace("@CouponCode@", codeEntity.Code)
                            .Replace("@nums@", "1")
                            .Replace("@EndDateStr@", codeEntity.EndDate.ToString("yyyy-MM-dd"));

                    customerMail.MailAddress = customer.MailAddress;
                    CouponDA.SendMailInfo(customerMail);
                }
                #endregion
                num++;
                userList += "顾客编号：" + customer.CustomerSysNo + "，顾客ID：" + customer.CustomerID + "\r\n";
            }
            WriteLog(DateTime.Now + "注册送优惠券成功发送" + num + "个顾客，分别是：\r\n" + userList);
        }
        #endregion

        #region 生日送优惠券
        private static void ProcessEachForBirthday(Coupon master)
        {
            //获取要过生日未发放生日优惠券的顾客 （一天发一次）造优惠券，发给顾客
            string userType = "B";
            List<Customer> customerList = CouponDA.GetBirthdayUser(master.SysNo);//获取要过生日未发放生日优惠券的顾客
            if (customerList == null || customerList.Count <= 0)
            {
                WriteLog("没能查询到符合条件的用户");
                return;
            }
            WriteLog("查询到今天过生日的用户" + customerList.Count + "个");
            master.BindStatus = "S";//字段
            CouponDA.UpdateBindRulesStatus(master);

            int num = 0;
            string userList = string.Empty;
            foreach (Customer customer in customerList)
            {
                #region CreateCode
                CouponCode codeEntity = new CouponCode();
                codeEntity.CouponSysNo = master.SysNo;
                codeEntity.CodeType = "T";
                codeEntity.CustomerMaxFrequency = 1;
                GetCodeExpireDate(master, codeEntity);
                codeEntity = CouponDA.CreateCode(codeEntity);
                #endregion

                CouponCodeCustomerLog logEntity = new CouponCodeCustomerLog();
                logEntity.CouponCode = codeEntity.Code;
                logEntity.CouponSysNo = master.SysNo;
                logEntity.CustomerSysNo = customer.CustomerSysNo;
                logEntity.UserCodeType = userType;
                CouponDA.InsertCustomerLog(logEntity);

                #region sendMail
                MailEntity customerMail = new MailEntity();
                customerMail.MailSubject = string.Format("亲爱的{0}，生日将际，大昌优品为你送上一份生日礼物",customer.CustomerID);//master.CouponName;

                if (!string.IsNullOrEmpty(customer.MailAddress) && master.IsSendMail == "Y")
                {
                    int _num = 1;
                    StringBuilder result = new StringBuilder();
                    IList<ProductTop6Entity> product = CouponDA.GetProductTop6();
                    result.Append(ConstValues.BirthdayMailTemplate.Replace("@CustomerHello@", customer.CustomerID));
                    foreach (ProductTop6Entity item in product)
                    {
                        result.Replace(String.Format("@ProductImage{0}@", _num), String.Format(imageText, item.DefaultImage));
                        result.Replace(String.Format("@ProductTitle{0}@", _num), item.ProductTitle);
                        result.Replace(String.Format("@ProductUrl{0}@", _num), String.Format(produceUrl, item.ProductID, DateTime.Now.ToString("yyyyMMdd")));
                        result.Replace(String.Format("@ProductPrice{0}@", _num),item.CurrentPrice.ToString("0.00"));
                        _num += 1;
                    }
                    result.Replace("@DateTime@",DateTime.Now.ToString("yyyy.MM.dd"));

                    result.Replace("@Birthday@", DateTime.Now.ToString("yyyyMMdd"));

                    customerMail.MailBody = result.ToString();
                    customerMail.MailAddress = customer.MailAddress;
                    CouponDA.SendMailInfo(customerMail);
                }
                #endregion
                num++;
                userList += "顾客编号：" + customer.CustomerSysNo + "，顾客ID：" + customer.CustomerID + "\r\n";
            }

            WriteLog(DateTime.Now + "生日送优惠券成功发送" + num + "个顾客，分别是：\r\n" + userList);
        }
        #endregion

        private static void ProcessEachForSO(Coupon master)
        {
            string userType = "O";
            List<Customer> customernewList = new List<Customer>();
            List<Customer> customerCheckList = new List<Customer>();
            //（所有商品）LimitType = "A";
            if (master.LimitType == "A")
            {
                //根据商家SysNo获取符合条件的订单信息和顾客信息
                customernewList = CouponDA.GetCustomerAndSO(master);
            }
            //（限定商品）LimitType = "I";
            if (master.LimitType == "I")
            {
                //限定商品---是否指定商品（true是）
                if (CouponDA.CheckIfRelationTypeY(master.SysNo))
                {
                    //（指定商品）根据商家SysNo获取符合条件的订单信息和顾客信息
                    customernewList = CouponDA.GetCustomerAndSoByTypeI(master);
                }
                else
                {
                    //（排他商品）根据商家SysNo获取符合条件的订单信息和顾客信息
                    customernewList = CouponDA.GetCustomerAndSoByTypeINO(master);
                }
            }


            if (customernewList == null || customernewList.Count <= 0)
            {
                WriteLog("没能查询到符合条件的用户");
                return;
            }
            else
            {
                foreach (Customer customer in customernewList)
                {
                    //排除已经发送优惠卷的订单信息和顾客信息
                    if (!CouponDA.CheckIfSendCustomerCouponCode(master.SysNo, customer.SOSysNo))
                    {
                        customerCheckList.Add(customer);
                    }
                }
            }
            if (customerCheckList == null || customerCheckList.Count <= 0)
            {
                WriteLog("没能查询到符合条件的用户");
                return;
            }
            WriteLog("查询到可发的用户" + customerCheckList.Count + "个");

            master.BindStatus = "S";//字段
            CouponDA.UpdateBindRulesStatus(master);

            int num = 0;
            string userList = string.Empty;
            foreach (Customer customer in customerCheckList)
            {
                if (!CouponDA.CheckIfSendCustomerCouponCode(master.SysNo, customer.SOSysNo))
                {
                    #region CreateCode
                    CouponCode codeEntity = new CouponCode();
                    codeEntity.CouponSysNo = master.SysNo;
                    codeEntity.CodeType = "C";
                    codeEntity.CustomerMaxFrequency = 1;
                    GetCodeExpireDate(master, codeEntity);
                    codeEntity = CouponDA.CreateCode(codeEntity);
                    #endregion

                    CouponCodeCustomerLog logEntity = new CouponCodeCustomerLog();
                    logEntity.CouponCode = codeEntity.Code;
                    logEntity.CouponSysNo = master.SysNo;
                    logEntity.CustomerSysNo = customer.CustomerSysNo;
                    logEntity.SOSysNo = customer.SOSysNo;
                    logEntity.UserCodeType = userType;
                    CouponDA.InsertCustomerLog(logEntity);
                    num++;
                    userList += "顾客编号：" + customer.CustomerSysNo + "\r\n";
                }
            }
            WriteLog(DateTime.Now + "购买商品送优惠券成功发送" + num + "次，分别是：\r\n" + userList);
            
        }

        #region 计算优惠券code有效期
        /// <summary>
        /// 计算优惠券code有效期
        /// </summary>
        /// <param name="master"></param>
        /// <param name="codeEntity"></param>
        public static void GetCodeExpireDate(Coupon master, CouponCode codeEntity)
        {
            #region beginDate,EndDate
            switch (master.ValidPeriod)
            {
                case (int)ValidPeriodType.ALL:
                    codeEntity.BeginDate = master.BeginDate;
                    codeEntity.EndDate = master.EndDate;
                    break;
                case (int)ValidPeriodType.OneWeek:
                    codeEntity.BeginDate =Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(7);
                    break;
                case (int)ValidPeriodType.OneMonth:
                    codeEntity.BeginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(1);
                    break;
                case (int)ValidPeriodType.TwoMonth:
                    codeEntity.BeginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(2);
                    break;
                case (int)ValidPeriodType.Threemonth:
                    codeEntity.BeginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(3);
                    break;
                case (int)ValidPeriodType.SixMonth:
                    codeEntity.BeginDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    codeEntity.EndDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).AddMonths(6);
                    break;
                case (int)ValidPeriodType.Customerize:
                    codeEntity.BeginDate = master.BindBeginDate;
                    codeEntity.EndDate = master.BindEndDate;
                    break;
                default:
                    codeEntity.BeginDate = master.BeginDate;
                    codeEntity.EndDate = master.EditDate;
                    break;
            }

            #endregion
        }
        #endregion

        public static void WriteLog(string content)
        {
            Console.WriteLine(content);
            Log.WriteLog(content, BizLogFile);
            if (jobContext != null)
            {
                jobContext.Message += content + "\r\n";
            }
        }
    }
}
