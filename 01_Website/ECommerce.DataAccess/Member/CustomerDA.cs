using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity.Member;
using ECommerce.Entity;
using ECommerce.Entity.Order;
using System.Data;
using ECommerce.Utility;
using ECommerce.Enums;
using ECommerce.Entity.Product;
using ECommerce.Entity.GiftCard;


namespace ECommerce.DataAccess.Member
{
    public class CustomerDA
    {
        #region 获取用户信息及更新用户信息
        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="sysNo">客户SysNo</param>
        /// <returns>客户信息</returns>
        public static CustomerInfo GetCustomerInfo(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerInfo");
            dataCommand.SetParameterValue("@CustomerSysNo", sysNo);
            return dataCommand.ExecuteEntity<CustomerInfo>();
        }
        /// <summary>
        /// Gets the customer information center database.
        /// </summary>
        /// <param name="sysNo">The system no.</param>
        /// <returns></returns>
        public static CustomerInfo GetCustomerInfoCenterDB(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerInfoCenterDB");
            dataCommand.SetParameterValue("@CustomerSysNo", sysNo);
            return dataCommand.ExecuteEntity<CustomerInfo>();
        }
        /// <summary>
        /// Gets the customer extend information.
        /// </summary>
        /// <param name="customersysno">The customersysno.</param>
        /// <returns></returns>
        public static CustomerExtendInfo GetCustomerExtendInfo(int customersysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerExtendInfo");
            dataCommand.SetParameterValue("@CustomerSysNo", customersysno);
            return dataCommand.ExecuteEntity<CustomerExtendInfo>();
        }
        /// <summary>
        /// Gets the customer extend information center database.
        /// </summary>
        /// <param name="customersysno">The customersysno.</param>
        /// <returns></returns>
        public static CustomerExtendInfo GetCustomerExtendInfoCenterDB(int customersysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerExtendInfoCenterDB");
            dataCommand.SetParameterValue("@CustomerSysNo", customersysno);
            return dataCommand.ExecuteEntity<CustomerExtendInfo>();
        }
        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="email">客户Email</param>
        /// <returns>客户信息</returns>
        public static CustomerInfo GetCustomerByEmail(string email)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerByEmail");
            dataCommand.SetParameterValue("@Email", email);
            return dataCommand.ExecuteEntity<CustomerInfo>();
        }

        /// <summary>
        /// 获取客户手机信息
        /// </summary>
        /// <param name="email">客户Email</param>
        /// <returns>客户信息</returns>
        public static CustomerInfo GetCustomerByPhone_ForCheck(string phoneNumber)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerByPhoneForCheck");
            dataCommand.SetParameterValue("@PhoneNumber", phoneNumber);
            return dataCommand.ExecuteEntity<CustomerInfo>();
        }

        /// <summary>
        /// 获取客户手机信息
        /// </summary>
        /// <param name="email">客户Email</param>
        /// <returns>客户信息</returns>
        public static CustomerInfo GetCustomerConfirmByPhone_ForCheck(string phoneNumber)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerConfirmByPhoneForCheck");
            dataCommand.SetParameterValue("@PhoneNumber", phoneNumber);
            return dataCommand.ExecuteEntity<CustomerInfo>();
        }

        /// <summary>
        /// 更新用户基本信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool UpdateCustomerPersonInfo(CustomerInfo info)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCustomerPersonInfo");
            dataCommand.SetParameterValue<CustomerInfo>(info);
            return dataCommand.ExecuteNonQuery() > 0 ? true : false;
        }

        /// 更新用户最后登录时间
        /// </summary>
        /// <param name="sysNo">用户的sysNo</param>
        /// <returns></returns>
        public static void UpdateCustomerLastLoginDate(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCustomerLastLoginDate");
            dataCommand.SetParameterValue("@CustomerSysNo", sysNo);
            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取用户最后登录时间
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static DateTime GetLastLoginDate(int sysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerLastLoginDate");
            dataCommand.SetParameterValue("@CustomerSysNo", sysNo);

            object obj = dataCommand.ExecuteScalar();
            if (obj == null || obj == DBNull.Value)
                return DateTime.MinValue;
            else
                return Convert.ToDateTime(obj);
        }

        /// <summary>
        /// 更改用户密码
        /// </summary>
        /// <param name="customerId">用户Id</param>
        /// <param name="password">密码</param>
        /// <param name="passwordSalt"></param>
        /// <returns>更新结果</returns>
        public static bool UpdateCustomerPassword(string customerId, string password, string passwordSalt)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCutomerPassword");
            dataCommand.SetParameterValue("@CustomerID", customerId);
            dataCommand.SetParameterValue("@Password", password);
            dataCommand.SetParameterValue("@PasswordSalt", passwordSalt);
            dataCommand.ExecuteNonQuery();
            return true;
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
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCustomerAvtar");
            dataCommand.SetParameterValue("@CustomerSysNo", customersysno);
            dataCommand.SetParameterValue("@AvtarImage", avatarimg);
            dataCommand.SetParameterValue("@AvtarImageStatus", status == AvtarImageStatus.A ? "A" : "D");
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteNonQuery() > 0;
        }
        #endregion

        #region 检查用户注册状态并登录

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="custLoginName">客户登录名</param>
        /// <returns>是否存在</returns>
        public static bool IsExistCustomer(string custLoginName)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_IsExistCustomer");
            dataCommand.SetParameterValue("@CustomerID", custLoginName);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            object o = dataCommand.ExecuteScalar();
            if (o != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 是否存在相同邮件地址
        /// </summary>
        /// <param name="customerSysNo">客户ID</param>
        /// <param name="email">客户Email</param>
        /// <returns>bool</returns>
        public static bool IsExistsEmail(int customerSysNo, string email)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_IsExistsEmail");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@Email", email);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            object result = dataCommand.ExecuteScalar();
            if (result != null && (int.Parse(result.ToString()) == 1))
                return true;
            else
                return false;
        }

        public static string GetCustomerPasswordSalt(string customerID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_GetCustomerPasswordSalt");
            cmd.SetParameterValue("@CustomerID", customerID);
            object o = cmd.ExecuteScalar();
            return null == o ? null : o.ToString();
        }

        public static EncryptMetaInfo GetCustomerEncryptMeta(string customerID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_GetCustomerEncryptMeta");
            cmd.SetParameterValue("@CustomerID", customerID);
            return cmd.ExecuteEntity<EncryptMetaInfo>();
        }

        /// <summary>
        /// 更新用户的邮件地址
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool UpdateCustomerEmailAddress(string customerID, string email)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_UpdateCustomerEmailAddress");
            cmd.SetParameterValue("@CustomerID", customerID);
            cmd.SetParameterValue("@Email", email);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static CustomerInfo CustomerLogin(string customerid, string password)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_CustomerLogin");
            cmd.SetParameterValue("@CustomerID", customerid);
            cmd.SetParameterValue("@Password", password);
            return cmd.ExecuteEntity<CustomerInfo>();
        }

        public static bool UpdateLastLoginTime(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_UpdateLastLoginTime");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 用户是存拥有该邮箱地址
        /// </summary>
        /// <param name="sysno"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool CheckCustomerEmail(int sysno, string email)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CheckCustomerEmail");
            dataCommand.SetParameterValue("@SysNo", sysno);
            dataCommand.SetParameterValue("@Email", email);
            return dataCommand.ExecuteEntity<CustomerInfo>() != null ? true : false;
        }

        /// <summary>
        /// 用户邮箱地址通过验证
        /// </summary>
        /// <param name="sysno"></param>
        /// <returns></returns>
        public static bool CustomerEmailValidated(int sysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CustomerEmailValidated");
            dataCommand.SetParameterValue("@SysNo", sysno);
            return dataCommand.ExecuteNonQuery() > 0 ? true : false;
        }


        ///// <summary>
        ///// 检查用户邮件和用户名是否存在
        ///// </summary>
        ///// <param name="customer">用户</param>
        ///// <returns>用户信息</returns>
        //public static CustomerInfo CheckCustomerData(CustomerInfo customer)
        //{
        //    DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CheckCustomerData");
        //    dataCommand.SetParameterValue("@CustomerID", customer.CustomerID);
        //    dataCommand.SetParameterValue("@Email", customer.Email);
        //    dataCommand.SetParameterValue("@LanguageCode", ConstValue.LaguageCode);
        //    dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
        //    dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
        //    object o = dataCommand.ExecuteScalar();
        //    if (o != null)
        //        customer.SysNo = int.Parse(o.ToString());
        //    else
        //        customer.SysNo = 0;

        //    return customer;
        //}


        ///// <summary>
        ///// 检查用户绑定手机是否存在
        ///// </summary>
        ///// <param name="customer">手机</param>
        ///// <returns>用户信息</returns>
        //public static bool CheckCustomerIsExistsPhone(string cellPhone)
        //{
        //    DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_IsExistsPhone");
        //    dataCommand.SetParameterValue("@CellPhone", cellPhone);
        //    dataCommand.SetParameterValue("@LanguageCode", ConstValue.LaguageCode);
        //    dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
        //    dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
        //    object o = dataCommand.ExecuteScalar();
        //    if (o != null)
        //        return true;
        //    else
        //        return false;

        //}


        #endregion

        #region 根据客户的手机获取客户信息

        /// <summary>
        /// 根据客户的手机获取客户信息
        /// </summary>
        /// <param name="cellPhone">手机</param>
        /// <returns>用户信息</returns>
        public static CustomerInfo GetCustomerByPhone(string cellPhone)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerByPhone");
            dataCommand.SetParameterValue("@CellPhone", cellPhone);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);

            return dataCommand.ExecuteEntity<CustomerInfo>();
        }

        #endregion

        #region 根据客户的账号获取客户信息

        /// <summary>
        /// 根据客户的账号获取客户信息
        /// </summary>
        /// <param name="cellPhone">账号</param>
        /// <returns>用户信息</returns>
        public static CustomerInfo GetCustomerByID(string customerid)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerByID");
            dataCommand.SetParameterValue("@CustomerID", customerid);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);

            return dataCommand.ExecuteEntity<CustomerInfo>();
        }

        #endregion

        #region 操作用户扩展信息

        /// <summary>
        /// 更新或创建用户扩展信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool UpdateCustomerPersonExtendInfo(CustomerExtendPersonInfo item)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCustomerPersonExtendInfo");
            dataCommand.SetParameterValue<CustomerExtendPersonInfo>(item);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 获取用户的扩展信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static CustomerExtendPersonInfo GetCustomerPersonExtendInfo(int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerPersonExtendInfo");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteEntity<CustomerExtendPersonInfo>();
        }

        #endregion

        /// <summary>
        /// 创建手机号码验证对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static CellPhoneConfirm CreateCellPhoneConfirm(CellPhoneConfirm item)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreateCellPhoneConfirm");
            dataCommand.SetParameterValue<CellPhoneConfirm>(item);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            dataCommand.ExecuteNonQuery();
            item.SysNo = Convert.ToInt32(dataCommand.GetParameterValue("SysNo"));
            return item;
        }

        public static bool ValidateCustomerPhone(string cellNumber, string confimKey)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_ValidateCustomerPhone");
            dataCommand.SetParameterValue("@CellPhone", cellNumber);
            dataCommand.SetParameterValue("@ConfirmKey", confimKey);
            return dataCommand.ExecuteNonQuery() == 1;
        }


        #region PasswordToken

        /// <summary>
        /// 创建设置新密码token
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CreatePasswordToken(int customerSysNo, string token, string tokenType)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreatePasswordToken");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@Token", token);
            dataCommand.SetParameterValue("@TokenType", tokenType);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            object obj = dataCommand.ExecuteScalar();
            return Convert.ToInt32(obj.ToString()) == 1;
        }

        /// <summary>
        /// 获取token对应的customer sysno
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static CustomerPasswordTokenInfo GetPasswordTokenInfo(string token, string tokenType)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetPasswordTokenInfo");
            dataCommand.SetParameterValue("@Token", token);
            dataCommand.SetParameterValue("@TokenType", tokenType);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return dataCommand.ExecuteEntity<CustomerPasswordTokenInfo>();
        }
        /// <summary>
        /// 取得指定用户TOKEN信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public static CustomerPasswordTokenInfo GetCustomerPasswordToken(int customerSysNo, string tokenType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_GetCustomerPasswordTokenInfo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@TokenType", tokenType);
            return cmd.ExecuteEntity<CustomerPasswordTokenInfo>();
        }


        /// <summary>
        /// 更新password token状态等信息
        /// </summary>
        /// <param name="passwordToken"></param>
        /// <returns></returns>
        public static void UpdatePasswordToken(string token)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdatePasswordTokenInfo");
            dataCommand.SetParameterValue("@Token", token);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            dataCommand.ExecuteNonQuery();
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
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreateCustomer");
            dataCommand.SetParameterValue<CustomerInfo>(customer);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            //dataCommand.SetParameterValue("@CustomerID", customer.Code);
            //dataCommand.SetParameterValue("@Pwd", customer.Password);
            //dataCommand.SetParameterValue("@Email", customer.Email);
            //dataCommand.SetParameterValue("@FromLinkSource", customer.FromLinkSource);
            //dataCommand.SetParameterValue("@RegisterIPAddress", customer.CustomerIPAddress);
            //dataCommand.SetParameterValue("@InitRank", customer.CustomerRank == CustomerRankType.Unknown ? 1 : (int)customer.CustomerRank);
            //dataCommand.SetParameterValue("@InitTotalSOMoney", customer.InitTotalSOMoney);
            //dataCommand.SetParameterValue("@SubscribeToPreferentialInformation", customer.SubscribeToPreferentialInformation);
            //dataCommand.SetParameterValue("@NewPassword", customer.NewPassword);
            //dataCommand.SetParameterValue("@PasswordSalt", customer.PasswordSalt);
            //dataCommand.SetParameterValue("@MembershipCard", customer.MembershipCard);
            //dataCommand.SetParameterValue("@CrmMemberID", customer.CrmMemberID);

            dataCommand.ExecuteNonQuery();
            customer.SysNo = Convert.ToInt32(dataCommand.GetParameterValue("SysNo"));

            //if (returnValue == 1)
            //{
            //    newCustomer.ReturnInfoType = CustomerReturnInfoType.Success;
            //}
            //else if (returnValue == -1)
            //{
            //    newCustomer.ReturnInfoType = CustomerReturnInfoType.CustomerIDRepeat;
            //}
            //else if (returnValue == -2)
            //{
            //    newCustomer.ReturnInfoType = CustomerReturnInfoType.CutomerEmailRepeat;
            //}
            //else
            //{
            //    newCustomer.ReturnInfoType = CustomerReturnInfoType.CutomerOtherError;
            //}

            return customer;
        }



        /// <summary>
        /// 增加第三方用户
        /// </summary>
        /// <param name="thirdPartyUserInfo">第三方用户信息</param>
        /// <returns>成功返回ID，否则返回0</returns>
        public static int CreateThirdPartyUser(ThirdPartyUserInfo thirdPartyUserInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreateThirdPartyUser");
            dataCommand.SetParameterValue("@PartnerUserID", thirdPartyUserInfo.PartnerUserID);
            dataCommand.SetParameterValue("@PartnerEmail", thirdPartyUserInfo.PartnerEmail);
            dataCommand.SetParameterValue("@UserSource", thirdPartyUserInfo.UserSource);
            dataCommand.SetParameterValue("@CustomerID", thirdPartyUserInfo.CustomerID);
            dataCommand.SetParameterValue("@CustomerSysNo", thirdPartyUserInfo.CustomerSysNo);
            dataCommand.SetParameterValue("@SubSource", thirdPartyUserInfo.SubSource);
            dataCommand.SetParameterValue("@SourceName", thirdPartyUserInfo.SourceName);

            dataCommand.ExecuteNonQuery();
            return Convert.ToInt32(dataCommand.GetParameterValue("SysNo"));
        }


        #endregion

        #region 账号中心查询

        /// <summary>
        /// 手机是否验证
        /// </summary>
        /// <param name="sysno">CustomerSysNo</param>
        /// <returns></returns>
        public static bool CheckCustomerPhoneValided(int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_IsCustomerPhoneValided");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);

            object o = dataCommand.ExecuteScalar();
            if (o != null)
                return true;
            else
                return false;

        }

        /// <summary>
        /// 获取指定状态的订单数
        /// </summary>
        /// <param name="customerSysNo">CustomerSysNo</param>
        /// <param name="orderStatus">CustomerSysNo</param>
        /// <returns></returns>
        public static int GetOrderCountByCustomerAndStatus(int customerSysNo, int orderStatus)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Order_GetCountByCustomerAndStatus");
            dataCommand.SetParameterValue("@CustomerID", customerSysNo);
            dataCommand.SetParameterValue("@Status", orderStatus);
            dataCommand.ExecuteNonQuery();

            int totalCount = 0;
            int.TryParse(dataCommand.GetParameterValue("@TotalCount").ToString(), out totalCount);

            return totalCount;

        }

        public static int GetRecentListCountByCustomerID(int customerID, int day)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Favorite_GetRecentListCount");
            dataCommand.SetParameterValue("@SysNo", customerID);
            dataCommand.SetParameterValue("@Day", day);
            int recordCount = dataCommand.ExecuteScalar<int>();
            return recordCount;
        }

        /// <summary>
        /// 获取我的收藏
        /// </summary>
        /// <param name="customerID">用户系统编号</param>
        /// <param name="topCount">获取数量</param>
        /// <returns></returns>
        public static List<WishProductInfo> GetMyWishTopList(int customerID, int topCount)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Favorite_GetMyWishTopList");
            dataCommand.SetParameterValue("@CustomerSysNo", customerID);
            dataCommand.SetParameterValue("@Top", topCount);

            List<WishProductInfo> wishInfoList = dataCommand.ExecuteEntityList<WishProductInfo>();

            return wishInfoList;
        }
        /*
        public static PagedList<WishProductInfo> GetByWishList(WishQueryInfo wishQueryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Favorite_GetWishListByCustomerID");
            dataCommand.SetParameterValue("@CustomerSysNo", wishQueryInfo.CustomerSysNo);
            dataCommand.SetParameterValue("@ProductName", wishQueryInfo.ProductName);
            dataCommand.SetParameterValue("@PageSize", wishQueryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@CurrentPageIndex", wishQueryInfo.PagingInfo.PageNumber);
            dataCommand.SetParameterValue("@SortField", wishQueryInfo.SortingInfo.SortField);
            dataCommand.SetParameterValue("@SortType", wishQueryInfo.SortingInfo.SortOrder.ToString());
            List<WishProductInfo> wishInfoList = dataCommand.ExecuteEntityList<WishProductInfo>();
            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            int pageIndex = wishQueryInfo.PagingInfo.PageNumber;
            if ((pageIndex * wishQueryInfo.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % wishQueryInfo.PagingInfo.PageSize) == 0)
                {
                    pageIndex = (int)(totalCount / wishQueryInfo.PagingInfo.PageSize);
                }
                else
                {
                    pageIndex = (int)(totalCount / wishQueryInfo.PagingInfo.PageSize) + 1;
                }
            }

            return new PagedList<WishProductInfo>(wishInfoList, pageIndex, wishQueryInfo.PagingInfo.PageSize, totalCount);
        }
        */
        /// <summary>
        /// 根据条件查询订单列表
        /// </summary>
        /// <param name="queryInfo">查询条件</param>
        /// <returns>订单列表</returns>
        public static QueryResult<OrderInfo> GetOrderList(SOQueryInfo queryInfo, bool isCenter)
        {
            string dataConfigKey = "Order_GetSalesOrderMasterList";
            if (isCenter)
            {
                dataConfigKey = "Order_GetCenterSalesOrderMasterList";
            }
            
            DataCommand dataCommand = DataCommandManager.GetDataCommand(dataConfigKey);

             
            dataCommand.SetParameterValue("@CustomerID", queryInfo.CustomerID);
            dataCommand.SetParameterValue("@SOID", queryInfo.SOID);
            if (!String.IsNullOrEmpty(queryInfo.ProductName))
            {
                queryInfo.ProductName = string.Format("%{0}%", queryInfo.ProductName.Trim());
            }

            dataCommand.SetParameterValue("@SOPayStatus", (int)queryInfo.SOPaymentStatus);
            if (isCenter)
            {
                dataCommand.SetParameterValue("@SearchKey", queryInfo.SearchKey);
            }
            else
            {
                dataCommand.SetParameterValue("@SearchKey", null);
            }
            dataCommand.SetParameterValue("@ProductName", queryInfo.ProductName);
            dataCommand.SetParameterValue("@SearchType", queryInfo.SearchType);
            dataCommand.SetParameterValue("@Status", queryInfo.Status);

            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);


            DataSet dsResult = dataCommand.ExecuteDataSet();
            if (dsResult != null && dsResult.Tables.Count > 0)
            {
                DataTable reviesTable = dsResult.Tables[0];
                List<OrderInfo> orderList = null;
                if (reviesTable.Rows != null && reviesTable.Rows.Count > 0)
                {
                    orderList = DataMapper.GetEntityList<OrderInfo, List<OrderInfo>>(reviesTable.Rows);
                    List<SOItemInfo> itemList = null;
                    if (dsResult.Tables.Count > 1 && dsResult.Tables[1] != null && dsResult.Tables[1].Rows != null && dsResult.Tables[1].Rows.Count > 0)
                    {
                        itemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(dsResult.Tables[1].Rows);
                    }
                    orderList.ForEach(f =>
                    {
                        f.SOItemList = itemList.FindAll(item => item.SOSysNo == f.SoSysNo);
                    });

                    int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                    int pageIndex = queryInfo.PagingInfo.PageIndex;

                    if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
                    {
                        if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                        {
                            pageIndex = totalCount / queryInfo.PagingInfo.PageSize;
                        }
                        else
                        {
                            pageIndex = totalCount / queryInfo.PagingInfo.PageSize + 1;
                        }
                    }
                    QueryResult<OrderInfo> result = new QueryResult<OrderInfo>();
                    result.ResultList = orderList;
                    result.PageInfo = new PageInfo();
                    result.PageInfo.TotalCount = totalCount;
                    result.PageInfo.PageIndex = pageIndex;
                    result.PageInfo.PageSize = queryInfo.PagingInfo.PageSize;
                    result.PageInfo.SortBy = queryInfo.PagingInfo.SortBy;

                    return result;
                }
            }
            return new QueryResult<OrderInfo>();
        }

        public static List<SOLog> GetOrderLogBySOSysNo(int sosysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Order_GetOrderLogBySOSysNo");
            dataCommand.SetParameterValue("@SOSysNo", sosysno);
            return dataCommand.ExecuteEntityList<SOLog>();
        }

        /*
        public static CreateDynamicStatus CreateDynamicConfirmInfo(PhoneDynamicValidationInfo validationInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CreateCustomerCellPhoneDynamicConfirmInfo");
            dataCommand.SetParameterValue("@CustomerID", validationInfo.CustomerSysNo);
            dataCommand.SetParameterValue("@CellPhone", validationInfo.CellPhone);
            dataCommand.SetParameterValue("@ConfirmKey", validationInfo.ConfirmKey);
            dataCommand.SetParameterValue("@FromIP", validationInfo.FromIP);
            dataCommand.SetParameterValue("@IntervalMinute", validationInfo.IntervalMinute);
            dataCommand.SetParameterValue("@IntervalSecond", validationInfo.IntervalSecond);
            dataCommand.SetParameterValue("@TotalSendTimes", validationInfo.TotalSendTimes);

            object result = dataCommand.ExecuteScalar();

            //CreateDynamicStatus phoneCreateDynamicValidateStatus = EnumHelper.GetEnumObject<CreateDynamicStatus>(Convert.ToString(result));
            return CreateDynamicStatus.ValidatePass;
        }

        public static ValidateDynamicStatus ValidateDynamicConfirmInfo(PhoneDynamicValidationInfo validationInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_ValidateCustomerCellPhoneDynamicConfirmInfo");
            dataCommand.SetParameterValue("@CustomerID", validationInfo.CustomerSysNo);
            dataCommand.SetParameterValue("@CellPhone", validationInfo.CellPhone);
            dataCommand.SetParameterValue("@ConfirmKey", validationInfo.ConfirmKey);
            dataCommand.SetParameterValue("@FromIp", validationInfo.FromIP);
            dataCommand.SetParameterValue("@IsRepeatTimes", validationInfo.IsRepeatTimes);
            dataCommand.SetParameterValue("@InvalidateMinute", validationInfo.InvalidateMinute);


            object result = dataCommand.ExecuteScalar();

            //ValidateDynamicStatus phoneUpdateDynamicValidateStatus = EnumHelper.GetEnumObject<ValidateDynamicStatus>(Convert.ToString(result));
            return ValidateDynamicStatus.UpdateValidatePass;
        }


        public static PhoneValidationInfo GetCustomerCellPhoneConfirmInfo(int customerID)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerCellPhoneConfirmInfo");
            dataCommand.SetParameterValue("@CustomerID", customerID);
            return dataCommand.ExecuteEntity<PhoneValidationInfo>();
        }
        */
        public static List<OrderInfo> GetTopOrderMasterList(int customerSysNo, int top)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Order_GetTopOrderMasterList");
            dataCommand.SetParameterValue("@CustomerID", customerSysNo);
            dataCommand.SetParameterValue("@top", top);
            List<OrderInfo> topicInfoList = dataCommand.ExecuteEntityList<OrderInfo>();
            return topicInfoList;
        }

        public static List<OrderInfo> GetQueryOrderMasterList(List<string> sosysnos)
        {

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Order_GetQueryOrderMasterListBySoSysNos");
            dataCommand.CommandText = dataCommand.CommandText.Replace("#SoSysNos#", string.Join(",", sosysnos));
            List<OrderInfo> topicInfoList = dataCommand.ExecuteEntityList<OrderInfo>();
            return topicInfoList;
        }

        public static List<OrderInfo> GetCenterOrderMasterList(int customerSysNo, List<string> sosysnos)
        {
            var sysNoes = new List<int>();
            sosysnos.ForEach(p =>
            {
                int sysNo;
                if (int.TryParse(p, out sysNo))
                {
                    sysNoes.Add(sysNo);
                }
            });
            if (sosysnos.Count <= 0)
            {
                return new List<OrderInfo>();
            }
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Order_GetCenterOrderMasterListBySoSysNos");
            dataCommand.CommandText = dataCommand.CommandText.Replace("#SoSysNos#", string.Join(",", sysNoes));
            dataCommand.SetParameterValue("@top", sosysnos.Count);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            List<OrderInfo> topicInfoList = dataCommand.ExecuteEntityList<OrderInfo>();
            return topicInfoList;
        }

        /// <summary>
        /// 获取SO详细信息
        /// </summary>
        /// <param name="sosysno">SO#</param>
        /// <returns>订单详细信息</returns>
        public static OrderInfo GetQuerySODetailInfo(int customerSysNo, int sosysno)
        {
            OrderInfo orderInfo = null;
            DataCommand command = DataCommandManager.GetDataCommand("Order_GetQuerySODetailInfo");
            command.SetParameterValue("@SOID", sosysno);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            DataSet result = command.ExecuteDataSet();

            if (result != null && result.Tables.Count > 0)
            {
                DataTable masterTable = result.Tables[0];

                if (masterTable.Rows != null && masterTable.Rows.Count > 0)
                {
                    orderInfo = DataMapper.GetEntity<OrderInfo>(masterTable.Rows[0]);
                }
                if (result.Tables != null && result.Tables.Count > 1)
                {
                    DataTable itemTable = result.Tables[1];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.SOItemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(itemTable.Rows);
                    }
                }
                if (result.Tables != null && result.Tables.Count > 2)
                {
                    DataTable itemTable = result.Tables[2];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.GiftCardRedeemLogList = DataMapper.GetEntityList<GiftCardRedeemLog, List<GiftCardRedeemLog>>(itemTable.Rows);
                    }
                }
            }

            return orderInfo;
        }

        /// <summary>
        /// 获取SO详细信息
        /// </summary>
        /// <param name="sosysno">SO#</param>
        /// <returns>订单详细信息</returns>
        public static OrderInfo GetCenterSODetailInfo(int customerSysNo, int sosysno)
        {
            OrderInfo orderInfo = null;
            DataCommand command = DataCommandManager.GetDataCommand("Order_GetCenterSODetailInfo");
            command.SetParameterValue("@SOID", sosysno);
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            DataSet result = command.ExecuteDataSet();

            if (result != null && result.Tables.Count > 0)
            {
                DataTable masterTable = result.Tables[0];

                if (masterTable.Rows != null && masterTable.Rows.Count > 0)
                {
                    orderInfo = DataMapper.GetEntity<OrderInfo>(masterTable.Rows[0]);
                }
                if (result.Tables != null && result.Tables.Count > 1)
                {
                    DataTable itemTable = result.Tables[1];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.SOItemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(itemTable.Rows);
                    }
                }
                if (result.Tables != null && result.Tables.Count > 2)
                {
                    DataTable itemTable = result.Tables[2];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.GiftCardRedeemLogList = DataMapper.GetEntityList<GiftCardRedeemLog, List<GiftCardRedeemLog>>(itemTable.Rows);
                    }
                }
            }

            return orderInfo;
        }

        /// <summary>
        /// 得到账户中心的晒单列表
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static PagedResult<OrderShowMaster> GetMyOrderShow(Product_ReviewQueryInfo queryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("FeedBack_ProductReview_GetMyOrderShow");
            dataCommand.SetParameterValue("@CustomerID", queryInfo.CustomerID);
            dataCommand.SetParameterValue("@ReviewType", queryInfo.ReviewType);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@LanguageCode", "zh-cn");
            dataCommand.SetParameterValue("@CompanyCode", "8601");
            dataCommand.SetParameterValue("@StoreCompanyCode", "8601");

            List<OrderShowMaster> orderShowList = new List<OrderShowMaster>();
            List<MyReview> myReviewList = dataCommand.ExecuteEntityList<MyReview>();
            if (myReviewList != null && myReviewList.Count > 0)
            {
                foreach (MyReview item in myReviewList)
                {
                    OrderShowMaster master = orderShowList.Find(f => f.SoSysno == item.SOSysno);
                    OrderShowProdutCell product = new OrderShowProdutCell();
                    product.SysNo = item.SysNo;
                    product.ID = item.ProductSysNo;
                    product.Code = item.ProductCode;
                    product.PromotionTitle = item.PromotionTitle;
                    product.Name = item.ProductName;
                    product.Title = item.ProductTitle;
                    product.ImageUrl = item.ProductImage;
                    product.InDate = item.InDate;
                    product.ShowAuditingStatus = item.ShowAuditingStatus;
                    if (master != null)
                    {
                        master.ProductList.Add(product);
                    }
                    else
                    {
                        master = new OrderShowMaster();
                        master.SoSysno = item.SOSysno;
                        master.OrderDate = item.OrderDate;
                        master.ProductList.Add(product);
                        orderShowList.Add(master);
                    }
                }
            }
            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            int pageIndex = queryInfo.PagingInfo.PageIndex;

            if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                {
                    pageIndex = totalCount / queryInfo.PagingInfo.PageSize;
                }
                else
                {
                    pageIndex = totalCount / queryInfo.PagingInfo.PageSize + 1;
                }
            }

            return new PagedResult<OrderShowMaster>(totalCount, queryInfo.PagingInfo.PageSize, pageIndex, orderShowList);
        }

        #endregion

        public static void InsertSOLog(int userSysNo, string ip, int soSysNo, string note, int type = 0)
        {
            var cmd = DataCommandManager.GetDataCommand("Customer_InsertSOLog");
            cmd.SetParameterValue("@UserSysNo", userSysNo);
            cmd.SetParameterValue("@IP", ip);
            cmd.SetParameterValue("@Type", type);
            cmd.SetParameterValue("@SOSysNo", soSysNo);
            cmd.SetParameterValue("@Note", note);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.ExecuteNonQuery();
        }

        public static int UpdateOrderMemo(int soSysNo, string memo)
        {
            var cmd = DataCommandManager.GetDataCommand("UpdateOrderMemo");
            cmd.SetParameterValue("@SysNo", soSysNo);
            cmd.SetParameterValue("@Memo", memo);
            return cmd.ExecuteNonQuery();
        }

        #region 我的商品收藏

        public static void AddProductToWishList(int customerSysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_AddProductToWishList");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.ExecuteNonQuery();
        }

        public static QueryResult<ProductFavorite> GetMyFavoriteProductList(int CustomerSysNo, PageInfo pagingInfo)
        {
            var dataCommand = DataCommandManager.GetDataCommand("Customer_GetMyFavoriteProductList");
            dataCommand.SetParameterValue("@CustomerSysNo", CustomerSysNo);

            dataCommand.SetParameterValue("@PageSize", pagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", pagingInfo.PageIndex);

            var productFavoriteList = dataCommand.ExecuteEntityList<ProductFavorite>();

            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            int pageIndex = pagingInfo.PageIndex;

            if ((pageIndex * pagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % pagingInfo.PageSize) == 0)
                {
                    pageIndex = totalCount / pagingInfo.PageSize;
                }
                else
                {
                    pageIndex = totalCount / pagingInfo.PageSize + 1;
                }
            }
            QueryResult<ProductFavorite> result = new QueryResult<ProductFavorite>();
            result.ResultList = productFavoriteList;
            result.PageInfo = new PageInfo();
            result.PageInfo.TotalCount = totalCount;
            result.PageInfo.PageIndex = pageIndex;
            result.PageInfo.PageSize = pagingInfo.PageSize;
            result.PageInfo.SortBy = pagingInfo.SortBy;

            return result;
        }

        public static int DeleteMyFavorite(int wishSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("Customer_DeleteMyFavorite");
            cmd.SetParameterValue("@WishSysNo", wishSysNo);
            return cmd.ExecuteNonQuery();
        }

        public static int DeleteMyFavoriteAll(int customerSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("Customer_DeleteMyFavoriteAll");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region 我的店铺收藏
        public static bool IsMyFavoriteSeller(int customerSysNo, int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_IsMyFavoriteSeller");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteScalar<bool>();
        }

        public static void AddFavoriteSeller(int customerSysNo, int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_AddFavoriteSeller");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.ExecuteNonQuery();
        }

        public static QueryResult<MyFavoriteSeller> GetMyFavoriteSeller(int customerSysNo, PageInfo pagingInfo)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = pagingInfo.SortBy;
            pagingEntity.MaximumRows = pagingInfo.PageSize;
            pagingEntity.StartRowIndex = (pagingInfo.PageIndex-1) * pagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_GetMyFavoriteSellerList");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd, pagingEntity, "ms.SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "ms.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, customerSysNo); 
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "v.Status", DbType.Int32, "@SellerStatus", QueryConditionOperatorType.Equal, 0);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
            }
            QueryResult<MyFavoriteSeller> result = new QueryResult<MyFavoriteSeller>();
            result.ResultList = cmd.ExecuteEntityList<MyFavoriteSeller>();
            result.PageInfo = new PageInfo();
            result.PageInfo.TotalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
            result.PageInfo.PageIndex = pagingInfo.PageIndex;
            result.PageInfo.PageSize = pagingInfo.PageSize;
            result.PageInfo.SortBy = pagingInfo.SortBy;

            return result;
        }

        public static int DeleteMyFavoriteSeller(int wishSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("Customer_DeleteMyFavoriteSeller");
            cmd.SetParameterValue("@WishSysNo", wishSysNo);
            return cmd.ExecuteNonQuery();
        }

        public static int DeleteMyFavoriteSellerAll(int customerSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("Customer_DeleteMyFavoriteSellerAll");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteNonQuery();
        }

        public static int GetRecentFavoriteSellerCountByCustomerID(int customerSysNo, int day)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Favorite_GetRecentFavoriteSellerCount");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@Day", day);
            int recordCount = dataCommand.ExecuteScalar<int>();
            return recordCount;
        }

        #endregion

        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static PrepayQueryResultInfo GetPrepayList(PrepayQueryInfoFilter filter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_GetPrepayList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, filter.ConvertToPaging(), "[SysNo] DESC"))
            {
                command.SetParameterValue("@CustomerID", filter.CustomerID);
                command.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
                command.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
                command.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, filter.CustomerID);
                command.CommandText = sqlBuilder.BuildQuerySql();
                var newsList = command.ExecuteEntityList<PrepayLogInfo>();
                var totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                QueryResult<PrepayLogInfo> result = new QueryResult<PrepayLogInfo>();
                result.ResultList = newsList;
                result.PageInfo = filter.ConvertToPageInfo(totalCount);
                decimal prepayAmount = Convert.ToDecimal(command.GetParameterValue("@PrepayAmount"));

                return new PrepayQueryResultInfo()
                {
                    LogList = result,
                    PrepayAmount = prepayAmount
                };
            }
        }

        public static ExperienceQueryResultInfo GetExperienceList(ExperienceQueryInfoFilter filter)
        {
            var dataCommand = DataCommandManager.GetDataCommand("Experience_GetExperienceList");
            dataCommand.SetParameterValue("@CustomerSysNo", filter.CustomerID);
            dataCommand.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", filter.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            List<ExperienceInfo> prepayInfoList = dataCommand.ExecuteEntityList<ExperienceInfo>();

            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

            int totalExperience = dataCommand.GetParameterValue("@ExperienceTotal") == DBNull.Value ? 0 : Convert.ToInt32(Math.Round(Convert.ToDecimal(dataCommand.GetParameterValue("@ExperienceTotal")), 0));
            int pageIndex = filter.PagingInfo.PageIndex;
            if ((pageIndex * filter.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % filter.PagingInfo.PageSize) == 0)
                {
                    pageIndex = (int)(totalCount / filter.PagingInfo.PageSize);
                }
                else
                {
                    pageIndex = (int)(totalCount / filter.PagingInfo.PageSize) + 1;
                }
            }
            ExperienceQueryResultInfo result = new ExperienceQueryResultInfo();
            result.LogList = new QueryResult<ExperienceInfo>();
            result.LogList.ResultList = prepayInfoList;

            result.LogList.PageInfo = new PageInfo();
            result.LogList.PageInfo.TotalCount = totalCount;
            result.LogList.PageInfo.PageIndex = pageIndex;
            result.LogList.PageInfo.PageSize = filter.PagingInfo.PageSize;
            result.LogList.PageInfo.SortBy = filter.PagingInfo.SortBy;

            result.TotalExperience = totalExperience;
            return result;
        }

        /// <summary>
        /// 积分消费记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult<PointConsumeInfo> GetPointConsumeList(PointQueryInfoFilter filter)
        {

            DataCommand dataCommand = DataCommandManager.GetDataCommand("Point_GetPointObtainAndConsumeList");
            dataCommand.SetParameterValue("@CustomerSysNo", filter.CustomerID);
            dataCommand.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", filter.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@ResultType", -1);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);

            List<PointConsumeInfo> pointInfoList = dataCommand.ExecuteEntityList<PointConsumeInfo>();
            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@RowCount"));
            int pageIndex = filter.PagingInfo.PageIndex;
            if ((pageIndex * filter.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % filter.PagingInfo.PageSize) == 0)
                {
                    pageIndex = (int)(totalCount / filter.PagingInfo.PageSize);
                }
                else
                {
                    pageIndex = (int)(totalCount / filter.PagingInfo.PageSize) + 1;
                }
            }
            QueryResult<PointConsumeInfo> list = new QueryResult<PointConsumeInfo>();
            list.ResultList = pointInfoList;
            list.PageInfo = new PageInfo();
            list.PageInfo.TotalCount = totalCount;
            list.PageInfo.PageIndex = pageIndex;
            list.PageInfo.PageSize = filter.PagingInfo.PageSize;
            list.PageInfo.SortBy = filter.PagingInfo.SortBy;
            return list;
        }

        /// <summary>
        /// 积分获得记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult<PointObtainInfo> GetPointObtainList(PointQueryInfoFilter filter)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Point_GetPointObtainAndConsumeList");
            dataCommand.SetParameterValue("@CustomerSysNo", filter.CustomerID);
            dataCommand.SetParameterValue("@PageSize", filter.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", filter.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@ResultType", 1);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            List<PointObtainInfo> pointInfoList = dataCommand.ExecuteEntityList<PointObtainInfo>();
            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@RowCount"));
            int pageIndex = filter.PagingInfo.PageIndex;
            if ((pageIndex * filter.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % filter.PagingInfo.PageSize) == 0)
                {
                    pageIndex = (int)(totalCount / filter.PagingInfo.PageSize);
                }
                else
                {
                    pageIndex = (int)(totalCount / filter.PagingInfo.PageSize) + 1;
                }
            }
            QueryResult<PointObtainInfo> list = new QueryResult<PointObtainInfo>();
            list.ResultList = pointInfoList;
            list.PageInfo = new PageInfo();
            list.PageInfo.TotalCount = totalCount;
            list.PageInfo.PageIndex = pageIndex;
            list.PageInfo.PageSize = filter.PagingInfo.PageSize;
            list.PageInfo.SortBy = filter.PagingInfo.SortBy;
            return list;
        }


        /// <summary>
        /// 查询用户优惠券
        /// </summary>
        /// <param name="query">查询信息</param>
        /// <returns></returns>
        public static QueryResult<CustomerCouponInfo> QueryCouponCode(CustomerCouponCodeQueryInfo query)
        {
            QueryResult<CustomerCouponInfo> result = new QueryResult<CustomerCouponInfo>();
            result.PageInfo = query.PageInfo;

            PagingInfoEntity page = new PagingInfoEntity();
            page.MaximumRows = query.PageInfo.PageSize;
            page.StartRowIndex = query.PageInfo.PageIndex * query.PageInfo.PageSize;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Customer_QueryCouponCode");
            cmd.SetParameterValue("@CustomerSysNo", query.CustomerSysNo);
            cmd.SetParameterValue("@CustomerRank", query.CustomerRank);
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, page, "T.[Status] ASC, T.UsedCount ASC, T.EndDate ASC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "[Status]", DbType.String, "@Status", QueryConditionOperatorType.Equal, query.Status);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                result.ResultList = cmd.ExecuteEntityList<CustomerCouponInfo>();

                int totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                result.PageInfo.TotalCount = totalCount;

                return result;
            }
        }

        /// <summary>
        /// 取消手机号码验证
        /// </summary>
        /// <param name="cellPhone"></param>
        /// <param name="sysNo"></param>
        public static void CancelCustomerPhone(string cellPhone, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CancelCustomerPhone");
            dataCommand.SetParameterValue("@CellPhone", cellPhone);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }

        public static bool PhoneIsValidate(string phoneNumber)
        {
            var cmd = DataCommandManager.GetDataCommand("Customer_PhoneIsValidate");
            cmd.SetParameterValue("@PhoneNumber", phoneNumber);
            var dt = cmd.ExecuteDataTable();
            if (dt.Rows.Count > 0) return true;
            return false;
        }

        /// <summary>
        /// 获取账户中心 评论列表
        /// </summary>
        /// <param name="reviewQueryInfo"></param>
        /// <returns></returns>
        public static PagedResult<MyReview> GetMyReview(Product_ReviewQueryInfo queryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetMyReview");
            dataCommand.SetParameterValue("@CustomerID", queryInfo.CustomerID);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@Type", queryInfo.ReviewStatus);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);

            List<MyReview> myReviewList = dataCommand.ExecuteEntityList<MyReview>();
            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            int pageIndex = queryInfo.PagingInfo.PageIndex;

            if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                {
                    pageIndex = totalCount / queryInfo.PagingInfo.PageSize;
                }
                else
                {
                    pageIndex = totalCount / queryInfo.PagingInfo.PageSize + 1;
                }
            }

            return new PagedResult<MyReview>(totalCount, queryInfo.PagingInfo.PageSize, pageIndex, myReviewList);
        }


        /// <summary>
        /// 获取账户中心 咨询列表
        /// </summary>
        /// <param name="productSysNo">商品唯一系统ID</param>
        /// <returns>客户指南</returns>
        public static PagedResult<ConsultationInfo> GetConsultListByCustomerSysNo(ConsultQueryInfo queryInfo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetConsultListByCustomerSysNo");
            dataCommand.SetParameterValue("@ProductGroupSysNo", queryInfo.ProductGroupSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", queryInfo.CustomerSysNo);
            dataCommand.SetParameterValue("@Type", queryInfo.QueryType);
            dataCommand.SetParameterValue("@ConsultType", queryInfo.ConsultType);
            dataCommand.SetParameterValue("@KeyWord", queryInfo.KeyWord);
            dataCommand.SetParameterValue("@CustomerReply", queryInfo.CustomerReply ? 1 : 0);
            dataCommand.SetParameterValue("@PageSize", queryInfo.PagingInfo.PageSize);
            dataCommand.SetParameterValue("@PageIndex", queryInfo.PagingInfo.PageIndex);
            dataCommand.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            dataCommand.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            DataSet result = dataCommand.ExecuteDataSet();
            DataTable masterTable = result.Tables[0];


            List<ConsultationInfo> consultInfoList = new List<ConsultationInfo>();
            if (masterTable.Rows != null && masterTable.Rows.Count > 0)
            {
                consultInfoList = DataMapper.GetEntityList<ConsultationInfo, List<ConsultationInfo>>(masterTable.Rows);
            }
            if (consultInfoList != null && consultInfoList.Count > 0)
            {
                DataTable itemTable = result.Tables[1];
                List<ProductConsultReplyInfo> replyList = DataMapper.GetEntityList<ProductConsultReplyInfo, List<ProductConsultReplyInfo>>(itemTable.Rows, (s, t) =>
                {
                    t.CustomerInfo.SysNo = Convert.ToInt32(s["CustomerSysNo"]);
                    t.CustomerInfo.CustomerID = Convert.ToString(s["CustomerID"]);
                });

                if (replyList != null && replyList.Count > 0)
                {
                    ProductConsultReplyInfo manufacutrerReply;
                    ProductConsultReplyInfo tempManufacutrerReply;
                    List<ProductConsultReplyInfo> userReplyList;

                    consultInfoList.ForEach(delegate(ConsultationInfo consultInfo)
                    {
                        //新蛋回复
                        consultInfo.NeweggReply = replyList.Find(delegate(ProductConsultReplyInfo replyInfo)
                        {
                            return replyInfo.ReplyType == FeedbackReplyType.Newegg && replyInfo.ConsultSysNo == consultInfo.SysNo;
                        });

                        //厂商回复
                        manufacutrerReply = replyList.Find(delegate(ProductConsultReplyInfo replyInfo)
                        {
                            return replyInfo.ReplyType == FeedbackReplyType.Web && replyInfo.ConsultSysNo == consultInfo.SysNo && replyInfo.IsTop.ToUpper() == "Y";
                        });

                        if (manufacutrerReply == null)
                        {
                            manufacutrerReply = replyList.Find(delegate(ProductConsultReplyInfo replyInfo)
                            {
                                return replyInfo.ReplyType == FeedbackReplyType.Manufacturer && replyInfo.ConsultSysNo == consultInfo.SysNo;
                            });
                        }
                        else
                        {
                            consultInfo.ReplyCount = consultInfo.ReplyCount - 1;
                            // 返回结果包含网友回复
                            if (queryInfo.CustomerReply)
                            {
                                //网友回复列表（除去用来替换厂商回复的一条数据）
                                tempManufacutrerReply = replyList.Find(delegate(ProductConsultReplyInfo replyInfo)
                                {
                                    return replyInfo.SysNo == manufacutrerReply.SysNo;

                                });
                                if (tempManufacutrerReply != null)
                                {
                                    replyList.Remove(tempManufacutrerReply);
                                }
                            }
                        }
                        consultInfo.ManufactureReply = manufacutrerReply;

                        // 返回结果包含网友回复
                        if (queryInfo.CustomerReply)
                        {
                            //网友回复
                            userReplyList = replyList.FindAll(delegate(ProductConsultReplyInfo replyInfo)
                            {
                                return replyInfo.ReplyType == FeedbackReplyType.Web && replyInfo.ConsultSysNo == consultInfo.SysNo;
                            });
                            if (userReplyList != null && userReplyList.Count > 0)
                            {
                                int count;
                                if (userReplyList.Count > 3)
                                {
                                    count = 3;
                                }
                                else
                                {
                                    count = userReplyList.Count;
                                }
                                consultInfo.ReplyList = userReplyList.GetRange(0, count);
                            }
                        }
                    });
                }
            }

            int totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            int pageIndex = queryInfo.PagingInfo.PageIndex;
            if ((pageIndex * queryInfo.PagingInfo.PageSize) > totalCount)
            {
                if (totalCount != 0 && (totalCount % queryInfo.PagingInfo.PageSize) == 0)
                {
                    pageIndex = (int)(totalCount / queryInfo.PagingInfo.PageSize);
                }
                else
                {
                    pageIndex = (int)(totalCount / queryInfo.PagingInfo.PageSize) + 1;
                }
            }
            //return new PagedList<ProductConsultInfo>(consultInfoList, pageIndex, queryInfo.PagingInfo.PageSize, totalCount);

            PagedResult<ConsultationInfo> list = new PagedResult<ConsultationInfo>(totalCount, queryInfo.PagingInfo.PageSize, pageIndex, consultInfoList);

            if (list != null && list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (ConsultationInfo info in list)
                    sb.Append(info.ProductSysNo.ToString()).Append(",");

                if (sb.Length > 1)
                    sb.Remove(sb.Length - 1, 1);

                DataCommand dataCommand2 = DataCommandManager.GetDataCommand("FeedBack_GetProductGroupByProductSysnos");
                dataCommand2.SetParameterValue("@productSysnos", sb.ToString());

                List<GroupPropertyVendorInfo> groups = dataCommand2.ExecuteEntityList<GroupPropertyVendorInfo>();

                if (groups != null && groups.Count > 0)
                    foreach (ConsultationInfo info in list)
                        foreach (GroupPropertyVendorInfo g in groups)
                            if (g.ProductSysNo == info.ProductSysNo)
                            {
                                info.GroupPropertyInfo = g.GroupPropertyInfo;
                                info.VendorInfo = g.VendorInfo;
                                break;
                            }
            }
            return list;
        }

        /// <summary>
        /// 检测邮箱是否已经被通过验证
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool CheckEmail(string email)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_CheckEmail");
            dataCommand.SetParameterValue("@Email", email);
            return dataCommand.ExecuteScalar<int>() > 0;
        }

        public static bool InsertCustomerLeaveWords(CustomerLeaveWords customerLeaveWords)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_LeaveWords");
            dataCommand.SetParameterValue("@Subject", customerLeaveWords.Subject);
            dataCommand.SetParameterValue("@LeaveWords", customerLeaveWords.LeaveWords);
            dataCommand.SetParameterValue("@SoSysno", customerLeaveWords.@SoSysno);
            dataCommand.SetParameterValue("@CustomerSysNo", customerLeaveWords.CustomerSysNo);
            dataCommand.SetParameterValue("@CustomerName", customerLeaveWords.CustomerName);
            dataCommand.SetParameterValue("@CustomerEmail", customerLeaveWords.CustomerEmail);
            dataCommand.SetParameterValue("@CreateTime", customerLeaveWords.InDate);
            dataCommand.SetParameterValue("@CompanyCode", customerLeaveWords.CompanyCode);
            dataCommand.SetParameterValue("@LanguageCode", customerLeaveWords.LanguageCode);
            dataCommand.SetParameterValue("@StoreCompanyCode", customerLeaveWords.StoreCompanyCode);
            return dataCommand.ExecuteNonQuery() > 0;
        }

        public static CustomerAuthenticationInfo GetCustomerAuthenticationInfo(int customerSysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_GetCustomerAuthenticationInfo");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysno);
            return dataCommand.ExecuteEntity<CustomerAuthenticationInfo>();
        }

        public static CustomerAuthenticationInfo InsertCustomerAuthenticationInfo(CustomerAuthenticationInfo info)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_InsertCustomerAuthenticationInfo");
            dataCommand.SetParameterValue(info);
            dataCommand.ExecuteNonQuery();
            info.SysNo = Convert.ToInt32(dataCommand.GetParameterValue("@SysNo"));
            return info;
        }

        public static CustomerAuthenticationInfo UpdateCustomerAuthenticationInfo(CustomerAuthenticationInfo info)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCustomerAuthenticationInfo");
            dataCommand.SetParameterValue(info);
            dataCommand.ExecuteNonQuery();
            return info;
        }

        /// <summary>
        /// Gets the query database so authentication information.
        /// </summary>
        /// <param name="customerSysNo">The customer system no.</param>
        /// <param name="sosysno">The sosysno.</param>
        /// <returns></returns>
        public static SOAuthenticationInfo GetQueryDbSOAuthenticationInfo(int customerSysNo, int sosysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Order_GetQueryDbSOAuthenticationInfo");

            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@SOSysNo", sosysno);

            return dataCommand.ExecuteEntity<SOAuthenticationInfo>();
        }

        /// <summary>
        /// Gets the center database so authentication information.
        /// </summary>
        /// <param name="customerSysNo">The customer system no.</param>
        /// <param name="sosysno">The sosysno.</param>
        /// <returns></returns>
        public static SOAuthenticationInfo GetCenterDbSOAuthenticationInfo(int customerSysNo, int sosysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Order_GetCenterDbSOAuthenticationInfo");

            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@SOSysNo", sosysno);

            return dataCommand.ExecuteEntity<SOAuthenticationInfo>();
        }


        /// <summary>
        /// 取得用户发票信息
        /// </summary>
        /// <param name="customerSysno"></param>
        /// <returns></returns>
        public static CustomerInvoiceInfo GetCustomerInvoiceInfo(int customerSysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_GetCustomerInvoiceByCustomerSysNo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysno);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return cmd.ExecuteEntity<CustomerInvoiceInfo>();
        }
        /// <summary>
        /// 新增或更新用户购物发票信息
        /// </summary>
        /// <param name="customerInvoiceInfo"></param>
        public static void UpdateCustomerInvoiceInfo(CustomerInvoiceInfo customerInvoiceInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Customer_UpdateCustomerInvoiceInfo");
            cmd.SetParameterValue("@CustomerSysNo", customerInvoiceInfo.CustomerSysNo);
            cmd.SetParameterValue("@InvoiceTitle", customerInvoiceInfo.InvoiceTitle);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates the customer last order pay type identifier.
        /// </summary>
        /// <param name="customerSysNo">The customer system no.</param>
        /// <param name="paytypeID">The paytype identifier.</param>
        /// <returns></returns>
        public static bool UpdateCustomerLastOrderPayTypeID(int customerSysNo, int paytypeID)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateLastOrderPayType");
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.SetParameterValue("@LastOrderPayTypeID", paytypeID);

            return dataCommand.ExecuteNonQuery() > 0 ? true : false;
        }
        /// <summary>
        /// 由于注册时手机验证无法关联还没有注册的用户，所以需要在用户注册成功后将注册的手机添加上去
        /// </summary>
        /// <cellPhoneSysNo>手机表ID</cellPhoneSysNo>
        /// <CustomerSysno>用户ID</CustomerSysno>
        /// <returns></returns>
        public static bool UpdateCustomerIDByCellPhoneSysNo(int cellPhoneSysNo, int customerSysno)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Customer_UpdateCustomerSysNoInCellPhoneConfirm");
            dataCommand.SetParameterValue("@SysNo", cellPhoneSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysno);
            bool result = dataCommand.ExecuteNonQuery() > 0 ? true : false;
            return result;
        }
    }
}
