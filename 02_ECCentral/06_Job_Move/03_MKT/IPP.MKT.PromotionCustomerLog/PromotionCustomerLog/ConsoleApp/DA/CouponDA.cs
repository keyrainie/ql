using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.EcommerceMgmt.SendCouponCode.Entities;
using System.Configuration;

namespace IPP.EcommerceMgmt.SendCouponCode.DA
{
    public static class CouponDA
    {
        static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        static string editUser = ConfigurationManager.AppSettings["editUser"];

        static int TestMonth = Convert.ToInt32(ConfigurationManager.AppSettings["month"]);
        static int TestDay = Convert.ToInt32(ConfigurationManager.AppSettings["day"]);
        static int TestUserSysNo = Convert.ToInt32(ConfigurationSettings.AppSettings["TestUserSysNo"]);
        static string testSysNo = ConfigurationSettings.AppSettings["TestPromotionSysNo"].ToString();

        public static List<Coupon> GetAvailableCoupon()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAvailableCoupon");
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@TestCouponSysNo", testSysNo);
            return command.ExecuteEntityList<Coupon>();
        }


        public static List<Coupon> GetAliPayCoupon()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAliPayCoupon");
            command.SetParameterValue("@TestCouponSysNo", testSysNo);
            return command.ExecuteEntityList<Coupon>();
        }

        public static int SendMailInfo(MailEntity entity)
        {
            DataCommand command = null;
            if (entity.IsInternal)
            {
                command = DataCommandManager.GetDataCommand("SendMailInternal");
            }
            else
            {
                command = DataCommandManager.GetDataCommand("SendMailInfo");
            }
            command.SetParameterValue("MailAddress", entity.MailAddress);//ConstValues.ToMailAddress);
            command.SetParameterValue("CCMailAddress", entity.CCMailAddress);//ConstValues.CcMailAddress);
            command.SetParameterValue("MailSubject", entity.MailSubject);
            command.SetParameterValue("MailBody", entity.MailBody);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            command.SetParameterValue("LanguageCode", ConstValues.LanguageCode);

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取自选用户列表
        /// </summary>
        /// <param name="eachItem"></param>
        /// <returns></returns>
        internal static List<Customer> GetLimitCustomerForSpecial(Coupon eachItem)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCouponSaleRulesCustomer");
            command.SetParameterValue("CouponSysNo", eachItem.SysNo);
            return command.ExecuteEntityList<Customer>();

        }
        /// <summary>
        /// 获取未过期的通用性的Code
        /// </summary>
        /// <param name="eachItem"></param>
        /// <returns></returns>
        internal static CouponCode QueryBindingCode(Coupon eachItem)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCouponCode");
            command.SetParameterValue("CouponSysNo", eachItem.SysNo);
            return command.ExecuteEntity<CouponCode>();
        }

        internal static void UpdateCouponSaleRulesCustomerStatus(Coupon eachItem)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCouponSaleRulesCustomerStatus");
            command.SetParameterValue("CouponSysNo", eachItem.SysNo);
            command.SetParameterValue("Status", "Y");
            command.ExecuteNonQuery();
        }


        internal static void UpdateBindRulesStatus(Coupon eachItem)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateBindRulesStatus");
            command.SetParameterValue("CouponSysNo", eachItem.SysNo);
            command.SetParameterValue("Status", eachItem.BindStatus);
            command.ExecuteNonQuery();
        }

        public static void InsertCustomerLog(CouponCodeCustomerLog log)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertCouponCodeCustomerLog");
            command.SetParameterValue("CouponSysNo", log.CouponSysNo);
            command.SetParameterValue("CouponCode", log.CouponCode);
            command.SetParameterValue("CustomerSysNo", log.CustomerSysNo);
            command.SetParameterValue("UserCodeType", log.UserCodeType);
            command.SetParameterValue("SOSysNo", log.SOSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取支付宝金账户第一次登陆果大昌优品的
        /// </summary>
        /// <param name="userCodeType"></param>
        /// <param name="customerSysNo"></param>
        /// <param name="master"></param>
        /// <returns></returns>
        public static List<Customer> GetAlipayCustomer(string userCodeType, Coupon master)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetAlipayCustomer");
            command.SetParameterValue("@UserCodeType", userCodeType);//Z支付宝金账户       
            command.SetParameterValue("@TestUserSysNo", TestUserSysNo);
            command.SetParameterValue("@BeginDate", master.BeginDate.ToString("yyyy-MM-dd"));//活动开始时间
            command.SetParameterValue("@EndDate", master.EndDate.ToString("yyyy-MM-dd"));//活动结束时间
            command.SetParameterValue("@CouponSysNo", master.SysNo );//活动编号
            return command.ExecuteEntityList<Customer>();
        }
        /// <summary>
        /// 获取生日和注册用户
        /// </summary>
        /// <param name="userCodeType"></param>
        /// <returns></returns>
        public static List<Customer> GetBirthdayUser(int couponSysNo)
        {
            int month;
            int day;
            if (TestMonth != 0 && TestDay != 0)//测试
            {
                month = TestMonth;
                day = TestDay;
            }
            else
            {
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;
            }
            int nowYear = DateTime.Now.Year;
            DataCommand command = DataCommandManager.GetDataCommand("GetBirthdayCustomer");
            command.SetParameterValue("@Month", month);
            command.SetParameterValue("@Day", day);
            command.SetParameterValue("@FromDate", Convert.ToDateTime(nowYear.ToString() + "-1-1 00:00:00"));
            command.SetParameterValue("@EndDate", Convert.ToDateTime(nowYear.ToString() + "-12-31 23:59:59"));
            command.SetParameterValue("@CouponSysNo", couponSysNo);//B 生日
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@TestUserSysNo", TestUserSysNo);
            return command.ExecuteEntityList<Customer>();
        }

        public static CouponCode CreateCode(CouponCode entity)
        {
            int num = 0;
            CouponCode code = new CouponCode();
            while (num < 1)
            {
                const string template = "234679ABCDEFGHJKLMNPQRTUVWXYZ";
                Random random = new Random();
                random.Next();
                entity.Code = GenerateRandomCode(template, random, 10);
                if (CouponDA.GetExistCode(entity.SysNo, entity.Code) == 0)
                {
                    code = CouponDA.InsertCouponCode(entity);
                    num++;
                }
            }
            return code;
        }

        private static string GenerateRandomCode(string template, Random random, int length)
        {
            StringBuilder builder = new StringBuilder();
            int maxRandom = template.Length - 1;
            for (int i = 0; i < length; i++)
            {
                builder.Append(template[random.Next(maxRandom)]);
            }
            return builder.ToString();
        }

        private static CouponCode InsertCouponCode(CouponCode entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertCouponCode");
            command.SetParameterValue("@CouponSysNo", entity.CouponSysNo);
            command.SetParameterValue("@CouponCode", entity.Code);
            command.SetParameterValue("@CodeType", entity.CodeType);
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@InUser", editUser);
            command.ExecuteNonQuery();
            entity.SysNo = (int)command.GetParameterValue("SysNo");
            return entity;

        }

        private static int GetExistCode(int couponSysNo, string code)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExistCode");
            command.SetParameterValue("@CouponCode", code);
            return command.ExecuteScalar<int>();
        }

        public static List<Customer> GetRegisterUser(string userType, Coupon master)
        {
            string dateBegin = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            string dateEnd = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DataCommand command = DataCommandManager.GetDataCommand("GetNewCustomer");
            command.SetParameterValue("@BeginDate", dateBegin);
            command.SetParameterValue("@EndDate", dateEnd);
            command.SetParameterValue("@CoponSysNo", master.SysNo);//活动编号
            command.SetParameterValue("@UserCodeType", userType);//R 注册，B 生日
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@TestUserSysNo", TestUserSysNo);
            return command.ExecuteEntityList<Customer>();
        }

        // 找出大昌优品热卖前六位商品
        public static IList<ProductTop6Entity> GetProductTop6()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductTop6");
            return command.ExecuteEntityList<ProductTop6Entity>();
        }

        /// <summary>
        /// （所有商品）LimitType = "A"根据商家SysNo获取符合条件的订单信息和顾客信息
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public static List<Customer> GetCustomerAndSO(Coupon master)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerAndSO");
            command.SetParameterValue("@BeginDate", master.BeginDate);
            command.SetParameterValue("@EndDate", master.EndDate);
            command.SetParameterValue("@MerchantSysNo", master.MerchantSysNo);
            command.SetParameterValue("@PayAmount", master.AmountLimit);
            command.SetParameterValue("@CouponSysNo", master.SysNo);
            return command.ExecuteEntityList<Customer>();
        }
        /// <summary>
        /// （指定商品）LimitType = "I";根据商家SysNo获取符合条件的订单信息和顾客信息
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public static List<Customer> GetCustomerAndSoByTypeI(Coupon master)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerAndSoByTypeI");
            command.SetParameterValue("@CouponSysNo", master.SysNo);
            command.SetParameterValue("@BeginDate", master.BeginDate);
            command.SetParameterValue("@EndDate", master.EndDate);
            command.SetParameterValue("@MerchantSysNo", master.MerchantSysNo);
            command.SetParameterValue("@PayAmount", master.AmountLimit);
            command.SetParameterValue("@CouponSysNo", master.SysNo);
            return command.ExecuteEntityList<Customer>();
        }
        /// <summary>
        /// （排他商品）LimitType = "I";根据商家SysNo获取符合条件的订单信息和顾客信息
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public static List<Customer> GetCustomerAndSoByTypeINO(Coupon master)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerAndSoByTypeINO");
            command.SetParameterValue("@CouponSysNo", master.SysNo);
            command.SetParameterValue("@BeginDate", master.BeginDate);
            command.SetParameterValue("@EndDate", master.EndDate);
            command.SetParameterValue("@MerchantSysNo", master.MerchantSysNo);
            command.SetParameterValue("@PayAmount", master.AmountLimit);
            command.SetParameterValue("@CouponSysNo", master.SysNo);
            return command.ExecuteEntityList<Customer>();
        }
        /// <summary>
        /// 查看是否该订单已经发放优惠卷true已发放
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static bool CheckIfSendCustomerCouponCode(int couponSysNo, int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckIfSendCustomerCouponCode");
            command.SetParameterValue("@couponSysNo", couponSysNo);
            command.SetParameterValue("@soSysNo", soSysNo);
            int i = 0;
            i = command.ExecuteScalar<int>();
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 是否限定商品___指定商品（true是）
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static bool CheckIfRelationTypeY(int couponSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CheckIfRelationTypeY");
            command.SetParameterValue("@couponSysNo", couponSysNo);
            int i = 0;
            i = command.ExecuteScalar<int>();
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
