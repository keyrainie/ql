using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ECommerce.DataAccess.Member;
using ECommerce.Entity.Member;
using ECommerce.Facade.Common;
using ECommerce.Facade.Common.RestClient;
using ECommerce.Facade.Member.Models;
using ECommerce.Entity.Order;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.WebFramework;
using ECommerce.WebFramework.Mail;
using ECommerce.DataAccess.Common;
using System.Xml.Linq;
using ECommerce.Utility;
using SolrNet;
using ECommerce.Entity.Product;
using ECommerce.Entity.Shipping;
using ECommerce.DataAccess;
using ECommerce.DataAccess.Product;

namespace ECommerce.Facade.Member
{
    public class CustomerFacade
    {
        #region 获取用户信息及更新用户信息
        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="sysNo">客户SysNo</param>
        /// <returns>客户信息</returns>
        public static CustomerInfo GetCustomerInfo(int sysNo)
        {
            CustomerInfo item = CustomerDA.GetCustomerInfo(sysNo);
            if (item != null)
                item.ExtendInfo = CustomerDA.GetCustomerExtendInfo(sysNo);
            return item;
        }
        /// <summary>
        /// Gets the customer information center database.
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static CustomerInfo GetCustomerInfoCenterDB(int sysNo)
        {
            CustomerInfo item = CustomerDA.GetCustomerInfoCenterDB(sysNo);
            if (item != null)
                item.ExtendInfo = CustomerDA.GetCustomerExtendInfoCenterDB(sysNo);
            return item;
        }
        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="email">客户Email</param>
        /// <returns>客户信息</returns>
        public static CustomerInfo GetCustomerByEmail(string email)
        {
            return CustomerDA.GetCustomerByEmail(email);
        }


        public static CustomerInfo GetCustomerByID(string customerid)
        {
            CustomerInfo item = CustomerDA.GetCustomerByID(customerid);
            if (item != null)
                item.ExtendInfo = CustomerDA.GetCustomerExtendInfo(item.SysNo);
            return item;
        }


        public static CustomerInfo GetCustomerByPhone(string phone)
        {
            return CustomerDA.GetCustomerByPhone(phone);
        }


        /// 更新用户最后登录时间
        /// </summary>
        /// <param name="sysNo">用户的sysNo</param>
        /// <returns></returns>
        public static void UpdateCustomerLastLoginDate(int sysNo)
        {
            CustomerDA.UpdateCustomerLastLoginDate(sysNo);
        }

        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool UpdateCustomerPersonInfo(CustomerInfo info)
        {
            return CustomerDA.UpdateCustomerPersonInfo(info);
        }

        /// <summary>
        /// 获取用户最后登录时间
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static DateTime GetLastLoginDate(int sysNo)
        {
            return CustomerDA.GetLastLoginDate(sysNo);
        }
        /// <summary>
        /// 创建手机号码验证对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static CellPhoneConfirm CreateCellPhoneConfirm(CellPhoneConfirm entity)
        {
            var temp = CustomerDA.CreateCellPhoneConfirm(entity);
            SMSInfo item = new SMSInfo(); //发送验证码短信
            item.CreateUserSysNo = entity.CustomerSysNo;
            item.CellNumber = entity.CellPhone;
            item.Status = SMSStatus.NoSend;
            item.Type = SMSType.VerifyPhone;
            item.Priority = 100;
            item.RetryCount = 0;
            item.SMSContent = string.Format(AppSettingManager.GetSetting("SMSTemplate", "CreateConfirmPhoneCode"), DateTime.Now.ToString("MM月dd日 HH:mm"), entity.ConfirmKey);
            int smsResult = 0;
            CommonFacade.InsertNewSMS(item, out smsResult);
            if(smsResult == -99999)//表明非法发送验证码
            {
                temp = new CellPhoneConfirm();
                temp.SysNo = -99999;
            }
            return temp;
        }

        /// <summary>
        /// 更新用户的邮件地址
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool UpdateCustomerEmailAddress(string customerID, string email)
        {
            return CustomerDA.UpdateCustomerEmailAddress(customerID, email);
        }

        public static bool ValidateCustomerPhone(string cellNumber, string confimKey, Point point)
        {
            using (var tran = TransactionManager.Create())
            {
                if (CustomerDA.ValidateCustomerPhone(cellNumber, confimKey) == false)
                {
                    return false;
                }
                PointFilter filter = new PointFilter
                {
                    CustomerSysNo = point.CustomerSysNo,
                    ObtainType = point.ObtainType,
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    CurrencyCode = ConstValue.CurrencySysNo.ToString(),
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                if (CommonDA.ExistsPoint(filter) <= 0)
                {
                    CommonFacade.AddPoint(point);
                }
                tran.Complete();
                return true;
            }
        }
        public static bool ValidateCustomerPhoneWithoutPoint(string cellNumber, string confimKey)
        {
            using (var tran = TransactionManager.Create())
            {
                if (CustomerDA.ValidateCustomerPhone(cellNumber, confimKey) == false)
                {
                    return false;
                }
                tran.Complete();
                return true;
            }
        }
        /// <summary>
        /// 更改用户密码
        /// </summary>
        /// <param name="customerId">用户Id</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>更新结果</returns>
        public static bool UpdateCustomerPassword(string customerId, string oldPassword, string newPassword)
        {
            var result = CustomerDA.UpdateCustomerPassword(customerId, oldPassword, newPassword);
            if (result)
            {
                //密码修改成功后发送短信
                var customer = CustomerFacade.GetCustomerByID(customerId);
                if (customer.IsPhoneValided == 1)
                {
                    var sms = new SMSInfo();
                    sms.CellNumber = customer.CellPhone;
                    sms.CompanyCode = ConstValue.CompanyCode;
                    sms.CreateTime = DateTime.Now;
                    sms.CreateUserSysNo = customer.SysNo;
                    sms.LanguageCode = ConstValue.LanguageCode;
                    sms.Priority = 1;
                    sms.SMSContent = AppSettingManager.GetSetting("SMSTemplate", "AlertUpdatePassword");
                    sms.Status = SMSStatus.NoSend;
                    sms.StoreCompanyCode = ConstValue.StoreCompanyCode;
                    sms.Type = SMSType.AlertUpdatePassword;
                    sms.RetryCount = 0;
                    CommonDA.InsertNewSMS(sms);
                }
            }
            return result;
        }
        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="avatarimg"></param>
        /// <param name="customersysno"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool ChangeCustomerAvatarImg(string avatarimg, int customersysno, AvtarImageStatus status)
        {
            return CustomerDA.ChangeCustomerAvatarImg(avatarimg, customersysno, status);
        }

        public static bool CheckCustomerPhoneValided(int customerSysNo)
        {
            return CustomerDA.CheckCustomerPhoneValided(customerSysNo);
        }

        public static int GetOrderCountByCustomerAndStatus(int customerSysNo, int orderStatus)
        {
            return CustomerDA.GetOrderCountByCustomerAndStatus(customerSysNo, orderStatus);
        }

        public static List<WishProductInfo> GetByWishList(int sysNo, int topCount)
        {
            return CustomerDA.GetMyWishTopList(sysNo, topCount);
        }

        public static List<OrderInfo> GetTopOrderMasterList(int customerSysNo, int top)
        {
            List<OrderInfo> result = CustomerDA.GetTopOrderMasterList(customerSysNo, top);
            //处理当前会话已经作废了的订单
            var voidedOrderSysNos = GetVoidedOrderFromCurrentSession();
            if (result != null && result.Count > 0)
            {
                result.ForEach(p =>
                {
                    if (voidedOrderSysNos.Any(q => q == p.SoSysNo))
                    {
                        p.Status = SOStatus.EmployeeCancel;
                    }
                });
            }
            return result;
        }

        public static QueryResult<OrderInfo> GetOrderList(SOQueryInfo queryInfo, bool isCenter)
        {
            var result = CustomerDA.GetOrderList(queryInfo, isCenter);

            //处理当前会话已经作废了的订单
            var voidedOrderSysNos = GetVoidedOrderFromCurrentSession();
            if (result != null && result.ResultList != null && result.ResultList.Count > 0)
            {
                result.ResultList.ForEach(p =>
                {
                    if (voidedOrderSysNos.Any(q => q == p.SoSysNo))
                    {
                        p.Status = SOStatus.EmployeeCancel;
                    }
                });
            }

            return result;
        }

        public static QueryResult<OrderInfo> GetOrderList(SOQueryInfo queryInfo)
        {
            return GetOrderList(queryInfo, false);
        }

        public static List<OrderInfo> GetQueryOrderMasterList(List<string> sosysnos)
        {
            var result = CustomerDA.GetQueryOrderMasterList(sosysnos);

            //处理当前会话已经作废了的订单
            var voidedOrderSysNos = GetVoidedOrderFromCurrentSession();
            result.ForEach(p =>
            {
                if (voidedOrderSysNos.Any(q => q == p.SoSysNo))
                {
                    p.Status = SOStatus.EmployeeCancel;
                }
            });

            return result;
        }

        public static List<OrderInfo> GetCenterOrderMasterList(int customerSysNo, List<string> sosysnos)
        {
            if (sosysnos.Count > 0)
                return CustomerDA.GetCenterOrderMasterList(customerSysNo, sosysnos);
            return new List<OrderInfo>();
        }

        public static OrderInfo GetQuerySODetailInfo(int customerSysNo, int sosysno)
        {

            var result = CustomerDA.GetQuerySODetailInfo(customerSysNo, sosysno);
            if (result != null)
            {
                //处理当前会话已经作废了的订单
                var voidedOrderSysNos = GetVoidedOrderFromCurrentSession();
                if (voidedOrderSysNos.Any(q => q == result.SoSysNo))
                {
                    result.Status = SOStatus.EmployeeCancel;
                }
            }
            return result;
        }

        public static OrderInfo GetCenterSODetailInfo(int customerSysNo, int sosysno)
        {
            return CustomerDA.GetCenterSODetailInfo(customerSysNo, sosysno);
        }

        public static List<SOLog> GetOrderLogBySOSysNo(int sosysno)
        {
            List<SOLog> list = CustomerDA.GetOrderLogBySOSysNo(sosysno);
            List<SOLog> logisticsLogs = new List<SOLog>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Note.IndexOf("<ActionName>") > 0)
                    {
                        try
                        {
                            XDocument xmlDoc = XDocument.Parse(item.Note);
                            var actionNameNotes = xmlDoc.Descendants("ActionName");
                            item.Note = actionNameNotes.First().Value;
                        }
                        catch
                        {
                            //简单处理屏蔽异常
                            item.Note = string.Empty;
                        }
                    }
                    else if (item.OptType == 600623)
                    {
                        //物流信息 
                        try
                        {
                            if (item.Note.StartsWith("http://www.kuaidi100.com"))
                            {
                                logisticsLogs.Add(new SOLog()
                                {
                                    Note = item.Note,
                                    OptType = item.OptType
                                });
                            }
                            else
                            {
                                List<SOLogisticsInfo> infos = SerializationUtility.XmlDeserialize<List<SOLogisticsInfo>>(item.Note);
                                if (infos != null && infos.Count > 0)
                                {
                                    DateTime acceptTime;
                                    foreach (var info in infos)
                                    {
                                        SOLog log = new SOLog();
                                        log.OptType = item.OptType;
                                        if (info.AcceptTime != null && info.AcceptTime.Length > 19)
                                        {
                                            info.AcceptTime = info.AcceptTime.Substring(0, 19);
                                        }
                                        if (DateTime.TryParse(info.AcceptTime, out acceptTime))
                                        {
                                            log.OptTime = acceptTime;
                                        }
                                        log.Note = info.Remark;
                                        if (info.Type == ExpressType.YT)
                                        {
                                            log.Note = string.Format("{0} {1} {2}", info.AcceptAddress, info.Remark, (info.Name.IndexOf(":") > 0 ? "" : "操作人: ") + info.Name);
                                        }
                                        else if (info.Type == ExpressType.SF)
                                        {
                                            log.Note = string.Format("{0} {1}", info.AcceptAddress, info.Remark);
                                        }
                                        logisticsLogs.Add(log);
                                    }

                                    //logisticsLogs = infos.ConvertAll<SOLog>(c =>
                                    //{
                                    //    SOLog log = new SOLog();
                                    //    log.OptType = item.OptType;
                                    //    //string timeStr = c.AcceptTime;
                                    //    if (c.AcceptTime != null && c.AcceptTime.Length > 19)
                                    //    {
                                    //        c.AcceptTime = c.AcceptTime.Substring(0, 19);
                                    //    }
                                    //    if (DateTime.TryParse(c.AcceptTime, out acceptTime))
                                    //    {
                                    //        log.OptTime = acceptTime;
                                    //    }
                                    //    log.Note = c.Remark;
                                    //    if (c.Type == ExpressType.YT)
                                    //    {
                                    //        log.Note = string.Format("{0} {1} {2}", c.AcceptAddress, c.Remark, (c.Name.IndexOf(":") > 0 ? "" : "操作人: ") + c.Name);
                                    //    }
                                    //    else if (c.Type == ExpressType.SF)
                                    //    {
                                    //        log.Note = string.Format("{0} {1}", c.AcceptAddress, c.Remark);
                                    //    }
                                    //    return log;
                                    //});
                                }
                            }
                        }
                        catch
                        {
                            //简单处理屏蔽异常
                            item.Note = string.Empty;
                        }
                    }
                }
            }
            if (list != null && logisticsLogs != null)
            {
                list.RemoveAll(x => x.OptType == 600623);
                list.AddRange(logisticsLogs);
            }
            return list;
        }

        public static List<SOLog> GetOrderDetailLogBySOSysNo(int sosysno)
        {
            int SOSysNo = sosysno;
            var log = CustomerFacade.GetOrderLogBySOSysNo(SOSysNo).Where(p => p.OptType > 0).ToList();
            for (var i = 0; i < log.Count; i++)
            {
                if (log[i].OptType == 600606 && (i + 1) < log.Count && log[i + 1].OptType == 201)
                {
                    log[i].Note += string.Format(" {0}", log[i + 1].Note);
                    log.Remove(log[i + 1]);
                }
            }
            return log;
        }

        /// <summary>
        /// 作废订单,调用ECCservice进行作废
        /// </summary>
        /// <param name="orderSysNo">订单编号</param>
        /// <param name="message"></param>
        /// <param name="userSysNo"></param>
        /// <returns>如果返回的空字符串则作废成功,否则显示返回的字符串</returns>
        public static string VoidedOrder(int orderSysNo, string message, int userSysNo)
        {
            var orderInfo= CustomerFacade.GetCenterSODetailInfo(userSysNo, orderSysNo);
            if (orderInfo == null)
            {
                throw new BusinessException("订单不存在");
            }
            if (orderInfo.HoldMark)
            {
                throw new BusinessException("订单已被锁定不能作废。如有疑问，请联系客服人员。");
            }
            var client = new Common.RestClient.RestClient(ConstValue.ECCServiceBaseUrl, ConstValue.LanguageCode);
            var serviceUrl = "/SOService/SO/Abandon";
            RestServiceError error;
            client.Update(serviceUrl, new
            {
                IsCreateAO = false,
                ImmediatelyReturnInventory = false,
                SOSysNoList = new List<int> { orderSysNo }
            }, out error);
            if (error != null)
            {
                var sb = new StringBuilder();
                error.Faults.ForEach(e => sb.AppendLine(e.ErrorDescription));

                if (error.Faults.All(e => e.IsBusinessException))
                {
                    return sb.ToString();
                }
                throw new ApplicationException(sb.ToString());
            }
            CustomerDA.InsertSOLog(userSysNo, CommonFacade.GetIP(), orderSysNo, message, 201);

            //将作废的订单的SOSysNo保存到Cookies中
            var values = CookieHelper.GetCookie<List<int>>(ConstValue.Cookie_Name_VoidedOrder);
            if (values != null)
            {
                values.Add(orderSysNo);
            }
            else
            {
                values = new List<int>();
                values.Add(orderSysNo);
            }
            CookieHelper.SaveCookie<List<int>>(ConstValue.Cookie_Name_VoidedOrder, values);

            return "";
        }
        /// <summary>
        /// 获得当前会话已经作废了的订单
        /// </summary>
        /// <returns></returns>
        public static List<int> GetVoidedOrderFromCurrentSession()
        {
            var values = CookieHelper.GetCookie<List<int>>(ConstValue.Cookie_Name_VoidedOrder);
            if (values == null)
            {
                values = new List<int>();
            }
            return values;
        }

        /*
       public static CreateDynamicStatus CreateDynamicConfirmInfo(PhoneDynamicValidationInfo validationInfo)
       {
           return CustomerDA.CreateDynamicConfirmInfo(validationInfo);
       }

       public static ValidateDynamicStatus ValidateDynamicConfirmInfo(PhoneDynamicValidationInfo validationInfo)
       {
           return CustomerDA.ValidateDynamicConfirmInfo(validationInfo);
       }
       */

        #endregion

        #region 收货地址

        /// <summary>
        /// 获取收货地址
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        //public static List<ShippingContactInfo> GetShippingAddress(int customerSysNo)
        //{
        //    return CustomerShippingAddressDA.GetCustomerShippingAddressList(customerSysNo);
        //}

        #endregion

        #region 操作用户扩展信息

        /// <summary>
        /// 更新或创建用户扩展信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool UpdateCustomerPersonExtendInfo(CustomerExtendPersonInfo item)
        {
            return CustomerDA.UpdateCustomerPersonExtendInfo(item);
        }

        /// <summary>
        /// 获取用户的扩展信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static CustomerExtendPersonInfo GetCustomerPersonExtendInfo(int customerSysNo)
        {
            return CustomerDA.GetCustomerPersonExtendInfo(customerSysNo);
        }

        #endregion
        /// <summary>
        /// 创建验证邮箱地址的邮件
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="ImgBaseUrl"></param>
        /// <returns></returns>
        public static bool SendEmailValidatorMail(string customerID, string newEmail, string ImgBaseUrl, string doamin)
        {
            CustomerInfo customer = CustomerDA.GetCustomerByID(customerID);
            AsyncEmail email = new AsyncEmail();
            email.MailAddress = newEmail;
            email.CustomerID = customerID;

            email.ImgBaseUrl = ImgBaseUrl;
            email.Status = (int)EmailStatus.NotSend;
            string token = Guid.NewGuid().ToString("N");
            email.SetNewTokenUrl = "/EmailVerifySucceed?token=" + token + "&sysno=" + customer.SysNo.ToString() + "&email=" + System.Web.HttpUtility.HtmlEncode(newEmail);
            string sub = string.Empty;
            email.MailBody = MailHelper.GetMailTemplateBody("ValidateEmail", out sub);
            email.MailBody = email.MailBody.Replace("[CustomerID]", customerID).Replace("[ImgBaseUrl]", ImgBaseUrl)
                .Replace("[WebBaseUrl]", ConstValue.WebDomain).Replace("[url]", doamin + email.SetNewTokenUrl).Replace("[CurrentDateTime]", DateTime.Now.ToString("yyyy-MM-dd")).Replace("[Year]", DateTime.Now.Year.ToString());
            email.MailSubject = sub;
            return EmailDA.SendEmail(email);
        }

        public static bool CheckCustomerEmail(int sysno, string email)
        {
            return CustomerDA.CheckCustomerEmail(sysno, email);
        }
        /// <summary>
        /// 用户邮箱地址通过验证
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public static bool CustomerEmailValidated(int sysno, Point point)
        {
            using (var tran = TransactionManager.Create())
            {
                CustomerDA.CustomerEmailValidated(sysno);
                PointFilter filter = new PointFilter
                {
                    CustomerSysNo = point.CustomerSysNo,
                    ObtainType = point.ObtainType,
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    CurrencyCode = ConstValue.CurrencySysNo.ToString(),
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                if (CommonDA.ExistsPoint(filter) <= 0)
                {
                    CommonFacade.AddPoint(point);
                }
                tran.Complete();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得作废订单的原因
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePair> GetToVoidedOrderReasons()
        {
            return CodeNamePairManager.GetList("VoidedOrderReasons", "VoidedOrderReasons");
        }

        /// <summary>
        /// 获得寄回方式
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePair> GetShipTypes()
        {
            return CodeNamePairManager.GetList("RMA", "ShipType");
        }
        /// <summary>
        /// 获得申请理由
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePair> GetRMAReasons()
        {
            return CodeNamePairManager.GetList("RMA", "RMAReason");
        }
        /// <summary>
        /// 获得售后请求
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePair> GetRequests()
        {
            return CodeNamePairManager.GetList("RMA", "Request");
        }
        /// <summary>
        /// 获得售后服务状态
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePair> GetRMARequestStatus()
        {
            return CodeNamePairManager.GetList("RMA", "RMARequestStatus");
        }
        /// <summary>
        /// 获得返修状态
        /// </summary>
        /// <returns></returns>
        public static List<CodeNamePair> GetRMARevertStatus()
        {
            return CodeNamePairManager.GetList("RMA", "RMARevertStatus");
        }

        public static bool IsShowPay(OrderInfo orderInfo)
        {
            var result = false;
            if (orderInfo.Status == SOStatus.Original
                    || orderInfo.Status == SOStatus.WaitingManagerAudit
                    || orderInfo.Status == SOStatus.WaitingPay)
            {
                if (orderInfo.IsNetPayed != 1
                    && orderInfo.IsPayWhenRecv != 1
                    && orderInfo.IsNet == 1)
                {
                    result = true;
                }
            }
            return result;
        }

        public static string ModifyOrderMemo(int soSysNo, string memo)
        {
            if (CustomerDA.UpdateOrderMemo(soSysNo, memo) > 0)
            {
                return "修改成功";
            }
            return "修改失败";
        }

        #region 我的商品收藏

        public static QueryResult<ProductFavorite> GetMyFavoriteProductList(int CustomerSysNo, PageInfo pagingInfo)
        {
            return CustomerDA.GetMyFavoriteProductList(CustomerSysNo, pagingInfo);
        }

        public static void AddProductToWishList(int customerSysNo, int productSysNo)
        {
            CustomerDA.AddProductToWishList(customerSysNo, productSysNo);
        }

        public static void DeleteMyFavorite(int wishSysNo)
        {
            CustomerDA.DeleteMyFavorite(wishSysNo);
        }

        /// <summary>
        /// 清空我的收藏
        /// </summary>
        /// <param name="customerSysNo"></param>
        public static void DeleteMyFavoriteAll(int customerSysNo)
        {
            CustomerDA.DeleteMyFavoriteAll(customerSysNo);
        }

        public static int GetRecentListCountByCustomerID(int customerSysNo, int day)
        {
            return CustomerDA.GetRecentListCountByCustomerID(customerSysNo, day);
        }

        #endregion

        #region 我的店铺收藏

        public static QueryResult<MyFavoriteSeller> GetMyFavoriteSeller(int customerSysNo, PageInfo pagingInfo)
        {
            return CustomerDA.GetMyFavoriteSeller(customerSysNo, pagingInfo);
        }

        /// <summary>
        /// 添加店铺收藏
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="sellerSysNo"></param>
        public static void AddMyFavoriteSeller(int customerSysNo, int sellerSysNo)
        {
            CustomerDA.AddFavoriteSeller(customerSysNo, sellerSysNo);
        }

        /// <summary>
        /// 判断当前店铺是否已收藏
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="sellerSysNo"></param>
        public static bool IsMyFavoriteSeller(int customerSysNo, int sellerSysNo)
        {
            return CustomerDA.IsMyFavoriteSeller(customerSysNo, sellerSysNo);
        }

        public static void DeleteMyFavoriteSeller(int wishSysNo)
        {
            CustomerDA.DeleteMyFavoriteSeller(wishSysNo);
        }

        /// <summary>
        /// 清空我的店铺收藏
        /// </summary>
        /// <param name="customerSysNo"></param>
        public static void DeleteMyFavoriteSellerAll(int customerSysNo)
        {
            CustomerDA.DeleteMyFavoriteSellerAll(customerSysNo);
        }

        /// <summary>
        /// 获取最近几天收藏的店铺
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int GetRecentFavoriteSellerCountByCustomerID(int customerSysNo, int day)
        {
            return CustomerDA.GetRecentFavoriteSellerCountByCustomerID(customerSysNo, day);
        }

        #endregion

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static PrepayQueryResultInfo GetPrepayList(PrepayQueryInfoFilter filter)
        {
            return CustomerDA.GetPrepayList(filter);
        }

        public static ExperienceQueryResultInfo GetExperienceList(ExperienceQueryInfoFilter filter)
        {
            return CustomerDA.GetExperienceList(filter);
        }
        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static PointListView GetPointListView(PointQueryInfoFilter filter1, PointQueryInfoFilter filter2)
        {
            PointListView listView = new PointListView();
            listView.PointObtainList = CustomerDA.GetPointObtainList(filter1);// (ConstructQueryInfo(customerID, "Obtain"));
            listView.PointConsumeList = CustomerDA.GetPointConsumeList(filter2);// (ConstructQueryInfo(customerID, "Consume"));
            return listView;
        }

        /// <summary>
        /// 积分获得记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult<PointObtainInfo> GetPointObtainList(PointQueryInfoFilter filter)
        {
            return CustomerDA.GetPointObtainList(filter);
        }

        /// <summary>
        /// 积分使用记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult<PointConsumeInfo> GetPointConsumeList(PointQueryInfoFilter filter)
        {
            return CustomerDA.GetPointConsumeList(filter);
        }

        /// <summary>
        /// 查询用户优惠券
        /// </summary>
        /// <param name="query">查询信息</param>
        /// <returns></returns>
        public static QueryResult<CustomerCouponInfo> QueryCouponCode(CustomerCouponCodeQueryInfo query)
        {
            var memberInfo = CustomerDA.GetCustomerInfo(query.CustomerSysNo);
            query.CustomerRank = (int)memberInfo.CustomerRank;
            return CustomerDA.QueryCouponCode(query);
        }

        /// <summary>
        /// 得到账户中心的晒单列表
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static PagedResult<OrderShowMaster> GetMyOrderShow(Product_ReviewQueryInfo queryInfo)
        {
            return CustomerDA.GetMyOrderShow(queryInfo);
        }

        /// <summary>
        /// 判断手机是否被验证过
        /// true:验证过,false:没有验证
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool PhoneIsValidate(string phoneNumber)
        {
            return CustomerDA.PhoneIsValidate(phoneNumber);
        }

        public static void CancelCustomerPhone(string cellPhone, int customerSysNo)
        {
            CustomerDA.CancelCustomerPhone(cellPhone, customerSysNo);
        }

        /// <summary>
        /// 获取账户中心我的评论
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static PagedResult<MyReview> GetMyReview(Product_ReviewQueryInfo queryInfo)
        {
            return CustomerDA.GetMyReview(queryInfo);
        }

        /// <summary>
        /// 检查订单是否发表过几次评论，若已发表1次以上，则不能再发表
        /// </summary>
        /// <param name="sosysno"></param>
        /// <param name="productSysNo"></param>
        /// <returns>0从来就没有评论过；1评论了一次，还可以评论一次；2不能在进行评论</returns>
        public static int CheckReviewedBySoSysNo(int sosysno, int productSysNo)
        {
            return ReviewDA.CheckReviewedBySoSysNo(sosysno, productSysNo);
        }

        /// <summary>
        /// 获取账户中心 我的咨询
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static PagedResult<ConsultationInfo> GetConsultListByCustomerSysNo(ConsultQueryInfo queryInfo)
        {
            return CustomerDA.GetConsultListByCustomerSysNo(queryInfo);
        }

        /// <summary>
        ///  检测邮箱是否已经被通过验证
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool CheckEmail(string email)
        {
            return CustomerDA.CheckEmail(email);
        }

        public static bool InsertCustomerLeaveWords(CustomerLeaveWords customerLeaveWords)
        {
            return CustomerDA.InsertCustomerLeaveWords(customerLeaveWords);
        }

        /// <summary>
        /// 取得用户实名认证信息
        /// </summary>
        /// <param name="customerSysno"></param>
        /// <returns></returns>
        public static CustomerAuthenticationInfo GetCustomerAuthenticationInfo(int customerSysno)
        {
            return CustomerDA.GetCustomerAuthenticationInfo(customerSysno);
        }
        /// <summary>
        ///  保存用户实名认证信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static CustomerAuthenticationInfo SaveCustomerAuthenticationInfo(CustomerAuthenticationInfo info)
        {
            if (!info.CustomerSysNo.HasValue || info.CustomerSysNo.Value <= 0)
            {
                throw new BusinessException("此用户不存在");
            }
            if (string.IsNullOrWhiteSpace(info.Name))
            {
                throw new BusinessException("实名认证信息姓名不能为空");
            }
            if (!info.IDCardType.HasValue)
            {
                throw new BusinessException("实名认证信息必须选择一种证件类型");
            }
            if (string.IsNullOrWhiteSpace(info.IDCardNumber))
            {
                throw new BusinessException("实名认证信息证件号不能为空");
            }
            if (!info.Birthday.HasValue)
            {
                throw new BusinessException("实名认证信息出生日期不能为空");
            }
            if (info.IDCardType.Value == IDCardType.IdentityCard)
            {
                string errorMsg;
                bool checkResult = CheckIDCard(info.IDCardNumber, info.Birthday.Value, out errorMsg);
                if (!checkResult)
                {
                    throw new BusinessException(errorMsg);
                }
            }

            var authInfo = CustomerDA.GetCustomerAuthenticationInfo(info.CustomerSysNo.Value);
            if (authInfo == null)
            {
                return CustomerDA.InsertCustomerAuthenticationInfo(info);
            }
            else
            {
                info.SysNo = authInfo.SysNo;
                return CustomerDA.UpdateCustomerAuthenticationInfo(info);
            }
        }

        public static bool CheckIDCard(string Id, DateTime birthDay, out string errorMsg)
        {
            bool checkResult = CheckIDCard(Id);
            if (!checkResult)
            {
                errorMsg = "身份证格式不正确";
                return false;
            }

            DateTime cardBirthDay = DateTime.MinValue;
            string birth = string.Empty;
            if (Id.Length == 15)
            {
                birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                cardBirthDay = DateTime.Parse(birth);
            }
            else if (Id.Length == 18)
            {
                birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                cardBirthDay = DateTime.Parse(birth);
            }
            if (cardBirthDay.ToString("yyyyMMdd") != birthDay.ToString("yyyyMMdd"))
            {
                errorMsg = "出生日期与身份证不一致";
                return false;
            }
            errorMsg = null;
            return true;
        }

        private static bool CheckIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }

            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();

            int sum = 0;

            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }

            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }

            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        /// <summary>
        /// Gets the query database so authentication information.
        /// </summary>
        /// <param name="customerSysNo">The customer system no.</param>
        /// <param name="sosysno">The sosysno.</param>
        /// <returns></returns>
        public static SOAuthenticationInfo GetQueryDbSOAuthenticationInfo(int customerSysNo, int sosysno)
        {
            return CustomerDA.GetQueryDbSOAuthenticationInfo(customerSysNo, sosysno);
        }

        /// <summary>
        /// Gets the center database so authentication information.
        /// </summary>
        /// <param name="customerSysNo">The customer system no.</param>
        /// <param name="sosysno">The sosysno.</param>
        /// <returns></returns>
        public static SOAuthenticationInfo GetCenterDbSOAuthenticationInfo(int customerSysNo, int sosysno)
        {
            return CustomerDA.GetCenterDbSOAuthenticationInfo(customerSysNo, sosysno); ;
        }

        /// <summary>
        /// 取得用户购物发票信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static CustomerInvoiceInfo GetCustomerInvoiceInfo(int customerSysNo)
        {
            return CustomerDA.GetCustomerInvoiceInfo(customerSysNo);
        }
        /// <summary>
        ///  新增或更新用户购物发票信息
        /// </summary>
        /// <param name="customerInvoiceInfo"></param>
        public static void UpdateCustomerInvoice(CustomerInvoiceInfo customerInvoiceInfo)
        {
            CustomerDA.UpdateCustomerInvoiceInfo(customerInvoiceInfo);
        }

        /// <summary>
        /// Updates the customer last order pay type identifier.
        /// </summary>
        /// <param name="customerSysNo">The customer system no.</param>
        /// <param name="paytypeID">The paytype identifier.</param>
        /// <returns></returns>
        public static bool UpdateCustomerLastOrderPayTypeID(int customerSysNo, int paytypeID)
        {
            return CustomerDA.UpdateCustomerLastOrderPayTypeID(customerSysNo, paytypeID);
        }
        /// <summary>
        /// 由于注册时手机验证无法关联还没有注册的用户，所以需要在用户注册成功后将注册的手机添加上去
        /// </summary>
        /// <cellPhoneSysNo>手机表ID</cellPhoneSysNo>
        /// <CustomerSysno>用户ID</CustomerSysno>
        /// <returns></returns>
        public static bool UpdateCellPhoneCustomerSysNoByID(int cellPhoneSysNo,int CustomerSysno)
        {
            return CustomerDA.UpdateCustomerIDByCellPhoneSysNo(cellPhoneSysNo, CustomerSysno);
        }
    }
}
