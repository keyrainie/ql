using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ECommerce.DataAccess.Member;
using ECommerce.Entity.Member;
using ECommerce.Entity.Common;
using ECommerce.Enums;
using ECommerce.WebFramework.Mail;
using ECommerce.DataAccess.Common;
using System.Configuration;
using ECommerce.Entity;
using System.Web;
using System.Web.Caching;
using System.Threading;
using ECommerce.Facade.Common.RestClient;
using ECommerce.Facade.Passport.Partner;
using System.Collections.Specialized;
using ECommerce.Utility;
using ECommerce.Facade.Member.Models;

namespace ECommerce.Facade.Member
{
    public class LoginFacade
    {
        ///// <summary>
        ///// 对字符串进行加密
        ///// </summary>
        ///// <param name="oldstring"></param>
        ///// <returns></returns>
        //public static string GetEncryptedPassword(string oldstring)
        //{
        //    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        //    byte[] source = Encoding.UTF8.GetBytes(oldstring);
        //    byte[] destination = sha1.ComputeHash(source);
        //    sha1.Clear();
        //    (sha1 as IDisposable).Dispose();
        //    return Convert.ToBase64String(destination);
        //}



        #region 检查用户注册状态

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="custLoginName">客户登录名</param>
        /// <returns>是否存在</returns>
        public static bool IsExistCustomer(string custLoginName)
        {
            return CustomerDA.IsExistCustomer(custLoginName);
        }

        /// <summary>
        /// Determines whether [is exist customer email] [the specified email].
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool IsCustomerEmailExist(string email)
        {
            CustomerInfo customer = CustomerDA.GetCustomerByEmail(email);

            return customer != null;
        }

        /// <summary>
        /// Determines whether [is exist customer email] [the specified email].
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool IsCustomerPhoneExist(string phoneNumber)
        {
            CustomerInfo customer = CustomerDA.GetCustomerByPhone_ForCheck(phoneNumber);

            return customer != null;
        }

        /// <summary>
        /// 判断手机号是否已被绑定
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static bool IsCustomerPhoneExist_Confirm(string phoneNumber)
        {
            CustomerInfo customer = CustomerDA.GetCustomerConfirmByPhone_ForCheck(phoneNumber);

            return customer != null;
        }
        /// <summary>
        /// 是否存在相同邮件地址
        /// </summary>
        /// <param name="customerSysNo">客户ID</param>
        /// <param name="email">客户Email</param>
        /// <returns>bool</returns>
        public static bool IsExistsEmail(int customerSysNo, string email)
        {
            return CustomerDA.IsExistsEmail(customerSysNo, email);
        }


        //public static string GetCustomerPasswordSalt(string customerID)
        //{
        //    return CustomerDA.GetCustomerPasswordSalt(customerID);
        //}

        public static EncryptMetaInfo GetCustomerEncryptMeta(string customerID)
        {
            return CustomerDA.GetCustomerEncryptMeta(customerID);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static CustomerInfo CustomerLogin(string customerid, string password)
        {
            return CustomerDA.CustomerLogin(customerid, password);
        }

        /// <summary>
        /// 泰隆网银账户登录
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //public static CustomerInfo TLYHCustomerLogin(string customerid, string password)
        //{
        //    //Key Test
        //    CustomerInfo customer = null;
        //    try
        //    {
        //        string tlyhHost = ECommerce.Facade.Payment.Charge.Charges.GetPaymentInfo(201).PaymentMode.PaymentUrl;
        //        //第一步，获取授权码
        //        NameValueCollection postDataAccessToken = new NameValueCollection();
        //        postDataAccessToken.Add("trxCode", "PB0004");
        //        string AccessTokenUri = "PB0004_getB2CCode_Ajax.do";
        //        string url = string.Format("{0}/{1}", tlyhHost, AccessTokenUri);
        //        string param = Partners.BuildStringFromNameValueCollection(postDataAccessToken);
        //        string responseData = Partners.HttpPostRequest(url, param, "application/x-www-form-urlencoded", "UTF-8");
        //        Logger.WriteLog(responseData, "第一步，获取授权码");
        //        TLYHAccessToken accessToken = SerializationUtility.JsonDeserialize<TLYHAccessToken>(responseData);
        //        //第二步，登陆网关
        //        NameValueCollection postData = new NameValueCollection();
        //        postData.Add("userName", customerid);
        //        postData.Add("password", password);
        //        postData.Add("checkCode", null);
        //        postData.Add("isCheckCode", "0");//1：使用验证码
        //        postData.Add("agreement", "0");//联合登录标识（“0”用户信息部分授权 ，1”用户信息全部授权）
        //        postData.Add("ebankId", "020200");
        //        postData.Add("clientIp", ClientHelper.getLocalIP());
        //        postData.Add("mac", ClientHelper.getLocalMac());
        //        postData.Add("log_opType", "13");
        //        postData.Add("trxCode", "PB0004");
        //        postData.Add("uniteLoginCode", accessToken.uniteLoginCode);
        //        postData.Add("authorizedSerial", accessToken.authorizedSerial);
        //        postData.Add("comparsys_time", DateTime.Now.ToString());
        //        string loginUri = "PB0004_uniteLoginOAuth.do";
        //        url = string.Format("{0}/{1}", tlyhHost, loginUri);
        //        param = Partners.BuildStringFromNameValueCollection(postData);
        //        responseData = Partners.HttpPostRequest(url, param, "application/x-www-form-urlencoded", "UTF-8");
        //        Logger.WriteLog(responseData, "第二步，登陆网关");
        //        TLYHUserInfo userInfo = SerializationUtility.JsonDeserialize<TLYHUserInfo>(responseData);
        //        if (userInfo == null)
        //        {
        //            Logger.WriteLog(string.Format("获取AccessToken 失败：{0}", responseData), "PassportTLYH", "TLXHCustomerLogin");
        //            throw new BusinessException("登录失败！");
        //        }

        //        DateTime customerBirthday = DateTime.Now;
        //        DateTime.TryParse(userInfo.Birthday, out customerBirthday);
        //        //第三步，写入优选网站Customer
        //        customer = new CustomerInfo()
        //        {
        //            CustomerID = userInfo.CustomerId,
        //            CustomerName = userInfo.CustomerNameCN,
        //            CustomersType = (int)CustomerSourceType.TLYH,
        //            InitRank = 1,
        //            Password = password,
        //            CellPhone = userInfo.PhoneNo,
        //            Email = userInfo.Email,
        //            Birthday = customerBirthday,
        //            //Gender = userInfo.CustomerSex,
        //            SourceType = CustomerSourceType.TLYH
        //        };

        //        var existsCustomer = CustomerFacade.GetCustomerByID(customer.CustomerID);
        //        if (existsCustomer == null)
        //        {
        //            int customerSysNo = LoginFacade.CreateCustomer(customer).SysNo;
        //            if (customerSysNo <= 0)
        //            {
        //                Logger.WriteLog(string.Format("第三方登录回调注册用户失败，第三方标识：{0}", CustomerSourceType.TLYH), "Passport", "LoginBack");
        //                throw new BusinessException("第三方登录注册用户失败！");
        //            }
        //            customer.SysNo = customerSysNo;
        //        }
        //        else
        //        {
        //            customer.SysNo = existsCustomer.SysNo;
        //        }

        //        //获取网银积分
        //        //customer.BankAccountPoint = GetTLYHCustomerPoint(customerid, password);

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(ex.Message, "异常");
        //    }

        //    return customer;
        //    //第一步，获取AccessToken
           

        //    //string loginUri = "PB0004_uniteLoginOAuth.do";
        //    //string tlyhHost = ECommerce.Facade.Payment.Charge.Charges.GetPaymentInfo(201).PaymentMode.PaymentUrl;

        //    //string tlyhAuthUrl = string.Format("{0}/{1}", tlyhHost, loginUri);
        //    //Logger.WriteLog(tlyhAuthUrl, "PassportTLYH", "TLXHCustomerLoginUrl");

        //    //string tlyhAuthParam = Partners.BuildStringFromNameValueCollection(postData);
        //    //Logger.WriteLog(tlyhAuthParam, "PassportTLYH", "TLXHCustomerLoginParam");

        //    //string responseData = Partners.HttpPostRequest(tlyhAuthUrl,
        //    //     tlyhAuthParam,
        //    //     "application/x-www-form-urlencoded", "UTF-8");

        //    //Logger.WriteLog(responseData, "PassportTLYH", "TLXHCustomerLoginResponse");

        //    //TLYHUserInfo userInfo = SerializationUtility.JsonDeserialize<TLYHUserInfo>(responseData);

        //    //if (userInfo == null)
        //    //{
        //    //    Logger.WriteLog(string.Format("获取AccessToken 失败：{0}", responseData), "PassportTLYH", "TLXHCustomerLogin");
        //    //    throw new BusinessException("登录失败！");
        //    //}

        //    //DateTime customerBirthday = DateTime.Now;
        //    //DateTime.TryParse(userInfo.Birthday, out customerBirthday);
        //    ////第三步，写入优选网站Customer
        //    //var customer = new CustomerInfo()
        //    //{
        //    //    CustomerID = userInfo.CustomerId,
        //    //    CustomerName = userInfo.CustomerNameCN,
        //    //    CustomersType = (int)CustomerSourceType.TLYH,
        //    //    InitRank = 1,
        //    //    Password = password,
        //    //    CellPhone = userInfo.PhoneNo,
        //    //    Email = userInfo.Email,
        //    //    Birthday = customerBirthday,
        //    //    //Gender = userInfo.CustomerSex,
        //    //    SourceType = CustomerSourceType.TLYH
        //    //};

        //    //var existsCustomer = CustomerFacade.GetCustomerByID(customer.CustomerID);
        //    //if (existsCustomer == null)
        //    //{
        //    //    int customerSysNo = LoginFacade.CreateCustomer(customer).SysNo;
        //    //    if (customerSysNo <= 0)
        //    //    {
        //    //        Logger.WriteLog(string.Format("第三方登录回调注册用户失败，第三方标识：{0}", CustomerSourceType.TLYH), "Passport", "LoginBack");
        //    //        throw new BusinessException("第三方登录注册用户失败！");
        //    //    }
        //    //    customer.SysNo = customerSysNo;
        //    //}
        //    //else
        //    //{
        //    //    customer.SysNo = existsCustomer.SysNo;
        //    //}


        //    ////获取网银积分
        //    //customer.BankAccountPoint = GetTLYHCustomerPoint(customerid, password);

        //    return customer;
        //}

        public static bool UpdateLastLoginTime(int sysNo)
        {
            return CustomerDA.UpdateLastLoginTime(sysNo);
        }

        /// <summary>
        /// 获取泰隆银行网银账号积分
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //public static int GetTLYHCustomerPoint(string customerId, string password)
        //{
        //    int userPoint = 0;
        //    try
        //    {
        //        NameValueCollection postData = new NameValueCollection();
        //        postData.Add("customerId", customerId);
        //        postData.Add("password", password);
        //        //postData.Add("checkCode", null);
        //        postData.Add("actionFlag", "0");

        //        string loginUri = "signIn.do";
        //        string tlyhHost = ECommerce.Facade.Payment.Charge.Charges.GetPaymentInfo(201).PaymentBase.BaseUrl;
        //        string tlyhAuthUrl = string.Format("{0}/{1}", tlyhHost, loginUri);

        //        Logger.WriteLog(tlyhAuthUrl, "GetTLYHCustomerPoint", "TLXHCustomerPointUrl");

        //        Logger.WriteLog(Partners.BuildStringFromNameValueCollection(postData), "GetTLYHCustomerPoint", "TLXHCustomerPointParams");

        //        string responseData = Partners.HttpPostRequest(tlyhAuthUrl,
        //             Partners.BuildStringFromNameValueCollection(postData),
        //             "application/x-www-form-urlencoded", "UTF-8");

        //        Logger.WriteLog(responseData, "GetTLYHCustomerPoint", "GetTLYHCustomerPointResponse");
        //        TLYHSignInResponse userSignInResponse = SerializationUtility.JsonDeserialize<TLYHSignInResponse>(responseData);
        //        userPoint = userSignInResponse.scoreMount;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog("获取泰隆网银账户积分失败:" + ex.Message);
        //    }
        //    return userPoint;
        //}

        /// <summary>
        /// 检查用户邮件和用户名是否存在
        /// </summary>
        /// <param name="customer">用户</param>
        /// <returns>用户信息</returns>
        //public static CustomerInfo CheckCustomerData(CustomerInfo customer)
        //{
        //    return CustomerDA.CheckCustomerData(customer);
        //}


        ///// <summary>
        ///// 检查用户绑定手机是否存在
        ///// </summary>
        ///// <param name="customer">用户</param>
        ///// <returns>是否存在</returns>
        //public static bool CheckCustomerIsExistsPhone(string phone)
        //{
        //    return CustomerDA.CheckCustomerIsExistsPhone(phone);
        //}


        #endregion

        #region PasswordToken

        /// <summary>
        /// 创建设置新密码token
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CreatePasswordToken(int customerSysNo, string token, string tokenType)
        {
            return CustomerDA.CreatePasswordToken(customerSysNo, token, tokenType);
        }

        /// <summary>
        /// 获取token对应的customer sysno
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static CustomerPasswordTokenInfo GetPasswordTokenInfo(string token, string tokenType)
        {
            return CustomerDA.GetPasswordTokenInfo(token, tokenType);
        }

        /// <summary>
        /// 更新password token状态等信息
        /// </summary>
        /// <param name="passwordToken"></param>
        /// <returns></returns>
        public static void UpdatePasswordToken(string token)
        {
            CustomerDA.UpdatePasswordToken(token);
        }

        #endregion

        #region 创建用户
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="customer">用户的注册信息</param>
        /// <returns>用户信息</returns>
        public static CustomerInfo CreateCustomer(CustomerInfo customer)
        {

            try
            {
                CustomerInfo item = CustomerDA.CreateCustomer(customer);
                if (item.SysNo > 0)
                {
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        AsyncEmail email = new AsyncEmail();
                        string mailSubject = string.Empty;

                        email.MailAddress = customer.Email;
                        email.CustomerID = item.CustomerID;
                        email.ImgBaseUrl = ConstValue.CDNWebDomain;
                        email.Status = (int)EmailStatus.NotSend;
                        email.MailBody = MailHelper.GetMailTemplateBody("RegisterSuccess", out mailSubject);
                        email.MailSubject = mailSubject.Replace("[CustomerID]", item.CustomerID);
                        email.MailBody = email.MailBody.Replace("[CustomerID]", item.CustomerID).Replace("[ImgBaseUrl]", email.ImgBaseUrl)
                            .Replace("[WebBaseUrl]", ConstValue.WebDomain).Replace("[CurrentDateTime]", DateTime.Now.ToString("yyyy-MM-dd")).Replace("[Year]", DateTime.Now.Year.ToString());

                        EmailDA.SendEmail(email);
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                ECommerce.Utility.Logger.WriteLog(string.Format("用户注册异常，异常信息：{0}！", ex.ToString()), "RegisteCustomer", "RegisteCustomerFailure");
                return customer;
            }
        }



        /// <summary>
        /// 增加第三方用户
        /// </summary>
        /// <param name="thirdPartyUserInfo">第三方用户信息</param>
        /// <returns>成功返回ID，否则返回0</returns>
        public static int CreateThirdPartyUser(ThirdPartyUserInfo thirdPartyUserInfo)
        {
            return CustomerDA.CreateThirdPartyUser(thirdPartyUserInfo);
        }


        #endregion


        /// <summary>
        /// 创建找回密码的url并写入数据库
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="ImgBaseUrl"></param>
        /// <returns></returns>
        public static bool SendFindPasswordMail(string customerID, string ImgBaseUrl, string doamin)
        {
            CustomerInfo customer = CustomerDA.GetCustomerByID(customerID);
            AsyncEmail email = new AsyncEmail();
            email.MailAddress = customer.Email;
            email.CustomerID = customerID;

            email.ImgBaseUrl = ImgBaseUrl;
            //email.CustomerSysNo = customer.SysNo;
            //email.MailType = (int)EmailType.FindPassword;
            email.Status = (int)EmailStatus.NotSend;
            bool createToken = false;
            string token = string.Empty;
            while (!createToken)
            {
                token = Guid.NewGuid().ToString("N");
                if (CustomerDA.CreatePasswordToken(customer.SysNo, token, "E"))
                {
                    email.SetNewTokenUrl = "/FindPassword?token=" + token + "&CustomerID=" + customerID;
                    email.CurrentDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                    email.Year = DateTime.Now.Year.ToString();
                    createToken = true;
                }
                else
                {
                    var customerPassToken = CustomerDA.GetCustomerPasswordToken(customer.SysNo, "E");
                    token = customerPassToken.Token;
                    email.SetNewTokenUrl = "/FindPassword?token=" + token + "&CustomerID=" + customerID;
                    email.CurrentDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                    email.Year = DateTime.Now.Year.ToString();
                    createToken = true;
                }
            }

            string secureWebBaseUrl = ConstValue.WebDomain;
            if (ConstValue.HaveSSLWebsite && !string.IsNullOrEmpty(ConstValue.SSLWebsiteHost))
            {
                secureWebBaseUrl = ConstValue.SSLWebsiteHost;
            }

            string sub = string.Empty;
            email.MailBody = MailHelper.GetMailTemplateBody("findpassword", out sub);
            email.MailBody = email.MailBody
                .Replace("[CustomerID]", customerID)
                .Replace("[ImgBaseUrl]", ImgBaseUrl)
                .Replace("[WebBaseUrl]", ConstValue.WebDomain)
                .Replace("[SetNewTokenUrl]", secureWebBaseUrl + email.SetNewTokenUrl)
                .Replace("[CurrentDateTime]", DateTime.Now.ToString("yyyy-MM-dd"))
                .Replace("[Year]", DateTime.Now.Year.ToString());
            email.MailSubject = sub;
            return EmailDA.SendEmail(email);
        }

        public static bool CheckShowAuthCode(string customerID, int showAuthCodeCount = 5)
        {
            int customerLoginFailedCount = GetCustomerLoginFailedCount(customerID);
            return customerLoginFailedCount >= showAuthCodeCount;
        }

        public static void IncrementCustomerLoginFailedCount(string customerID)
        {
            int orignalCount = GetCustomerLoginFailedCount(customerID);
            int count = Interlocked.Increment(ref orignalCount);
            SetCustomerLoginFailedCount(customerID, count);
        }

        private static int GetCustomerLoginFailedCount(string customerID)
        {
            int count = 0;
            string cacheKey = string.Format("customer_{0}_loginfailedcount", customerID);
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                count = (int)HttpRuntime.Cache.Get(cacheKey);
            }
            return count;
        }

        private static int SetCustomerLoginFailedCount(string customerID, int count)
        {
            string cacheKey = string.Format("customer_{0}_loginfailedcount", customerID);
            if (HttpRuntime.Cache[cacheKey] == null)
            {
                HttpRuntime.Cache.Insert(cacheKey, count, null, DateTime.Now.AddSeconds(CacheTime.Short), Cache.NoSlidingExpiration);
            }
            else
            {
                HttpRuntime.Cache[cacheKey] = count;
            }
            return count;
        }

        public static void ClearCustomerLoginFailedCount(string customerID)
        {
            string cacheKey = string.Format("customer_{0}_loginfailedcount", customerID);
            HttpRuntime.Cache.Remove(cacheKey);
        }
    }
}
