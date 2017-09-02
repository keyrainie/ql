using ECommerce.DataAccess.GiftCard;
using ECommerce.Entity;
using ECommerce.Entity.GiftCard;
using ECommerce.Entity.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.DataAccess.GiftCard;
using ECommerce.Entity;
using ECommerce.Entity.GiftCard;
using ECommerce.Utility;

namespace ECommerce.Facade.GiftCard
{
    public static class GiftCardFacade
    {
        /// <summary>
        /// 礼品卡消费记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static QueryResult<GiftCardUseInfo> QueryUsedRecord(UsedRecordQuery query)
        {
            return GiftCardDA.QueryUsedRecord(query);
        }
        /// <summary>
        /// 获取礼品卡商品基本信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<GiftCardProductInfo> QueryGiftCardProductInfo()
        {
            return GiftCardDA.QueryGiftCardProductInfo();
        }

        public static QueryResult<GiftCardInfo> QueryMyGiftCardInfo(MyGiftCardQueryInfoFilter filter)
        {
            return GiftCardDA.QueryMyGiftCardInfo(filter);
        }

        /// <summary>
        /// 通过卡号和密码加载礼品卡
        /// </summary>
        /// <param name="code">卡号</param>
        /// <param name="password">密码</param>
        /// <returns>礼品卡信息</returns>
        public static GiftCardInfo LoadGiftCard(string code, string password)
        {
            password = CryptoManager.Encrypt(password);//加密比对
            return GiftCardDA.LoadGiftCard(code, password);
        }

        /// <summary>
        /// 礼品卡绑定帐号
        /// </summary>
        /// <param name="code">卡号</param>
        /// <param name="customerSysNo">帐户</param>
        /// <returns></returns>
        public static bool BindGiftCard(string code, int customerSysNo)
        {
            return GiftCardDA.BindGiftCard(code, customerSysNo);
        }

        public static string LookPassword(string Code, int CurrentCustomerSysNo)
        {
            return GiftCardDA.LookPassword(Code, CurrentCustomerSysNo);
        }

        /// <summary>
        /// 修改礼品卡密码
        /// </summary>
        /// <param name="code">卡号</param>
        /// <param name="pwd">新密码</param>
        public static void ModifyGiftCardPwd(string code, string pwd)
        {
            pwd = CryptoManager.Encrypt(pwd);//加密
            GiftCardDA.ModifyGiftCardPwd(code, pwd);
        }
    }
}
