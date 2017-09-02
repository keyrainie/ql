using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using ECommerce.Entity.Member;
using ECommerce.Entity.Passport;
using ECommerce.Facade.Member;
using ECommerce.Facade.Passport.Partner;
using ECommerce.Utility;

namespace ECommerce.Facade.Passport
{
    public class PassportService
    {
        /// <summary>
        /// 第三方登录
        /// </summary>
        /// <param name="identify">第三方标识</param>
        /// <param name="returnUrl">登录之后返回的url</param>
        /// <returns></returns>
        public string Login(string identify, string returnUrl = "")
        {
            PartnerContext context = new PartnerContext();
            context.PartnerIdentify = identify;
            context.ReturnUrl = returnUrl;

            Partners partner = Partners.GetInstance(context);

            return partner != null ? partner.GetRequestContent(context) : string.Empty;
        }

        /// <summary>
        /// 第三方登录回调
        /// </summary>
        /// <param name="identify">第三方标识</param>
        /// <param name="collection">回调参数</param>
        /// <returns></returns>
        public PartnerBackResult LoginBack(string identify, NameValueCollection collection)
        {
            PartnerBackContext context = new PartnerBackContext();
            context.PartnerIdentify = identify;
            context.ResponseParam = collection;

            Partners partner = Partners.GetInstance(context);
            if (!partner.BackVerifySign(context))
            {
                Logger.WriteLog(string.Format("第三方登录回调非法请求，第三方标识：{0}", identify), "Passport", "LoginBack");
                throw new BusinessException("登录失败！");
            }

            CustomerInfo customer = null;
            if (context.ActionType == PassportActionType.Accept)
            {
                partner.GetResponseUserInfo(context);

                customer = new CustomerInfo()
                {
                    CustomerID = context.CustomerID,
                    CustomerName = context.CustomerName,
                    CustomersType = (int)context.CustomerSouce,
                    InitRank = 1,
                    Password = "",
                    CellPhone = context.CellPhone,
                    Email = context.Email
                };

                var existsCustomer = CustomerFacade.GetCustomerByID(context.CustomerID);
                if (existsCustomer == null)
                {
                    int customerSysNo = LoginFacade.CreateCustomer(customer).SysNo;
                    if (customerSysNo <= 0)
                    {
                        Logger.WriteLog(string.Format("第三方登录回调注册用户失败，第三方标识：{0}", identify), "Passport", "LoginBack");
                        throw new BusinessException("第三方登录注册用户失败！");
                    }
                    customer.SysNo = customerSysNo;
                }
                else
                {
                    customer.SysNo = existsCustomer.SysNo;
                }
            }
            PartnerBackResult result = new PartnerBackResult()
            {
                Customer = customer,
                ReturnUrl = context.ReturnUrl,
                ActionType = context.ActionType
            };
            return result;
        }
    }
}
