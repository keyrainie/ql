using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GiveERPCustomerPoint.Entities;
using CrmProj;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using GiveERPCustomerPoint.DA;

namespace GiveERPCustomerPoint.Biz
{
    public class CRMExternalService
    {

        static string CRMQZJConfigFilePath = ConfigurationManager.AppSettings["CRMQZJConfig"];

        private static CrmOpClaUtil crmOpClaUtilInStance;

        public static CrmOpClaUtil CrmOpClaUtilInStance
        {
            get
            {
                if (crmOpClaUtilInStance == null)
                {
                    CrmProj.Common.QZJCONFIG config = CRMQZJEntity;
                    if (config != null)
                    {
                        crmOpClaUtilInStance = new CrmOpClaUtil(config);
                    }
                }

                return crmOpClaUtilInStance;
            }
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="filePath"></param>
        public static CRMQZJConfig GetConfig()
        {
            string currentDir = System.Threading.Thread.GetDomain().BaseDirectory;
            string configFile = Path.Combine(currentDir, CRMQZJConfigFilePath);
            XmlSerializer xs = new XmlSerializer(typeof(CRMQZJConfig));
            Stream stream = new FileStream(configFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            CRMQZJConfig config = xs.Deserialize(stream) as CRMQZJConfig;
            stream.Close();
            return config;

        }

        private static CrmProj.Common.QZJCONFIG CRMQZJEntity
        {
            get
            {

                CRMQZJConfig config = GetConfig();
                if (config != null)
                {
                    CrmProj.Common.QZJCONFIG entity = new CrmProj.Common.QZJCONFIG();
                    entity.B2CVIPTYPE = config.B2CVIPTYPE;
                    entity.CRM_BFCRM_USER = config.CRM_BFCRM_USER;
                    entity.CRM_LOCALTION_CODE = config.CRM_LOCALTION_CODE;
                    entity.CRM_PASSWORD = config.CRM_PASSWORD;
                    entity.CRMHYKTYPE = config.CRMHYKTYPE;
                    entity.CRMIP = config.CRMIP;
                    entity.CRMMDDM = config.CRMMDDM;
                    entity.CRMPORT = config.CRMPORT;
                    entity.CRMPOSNO = config.CRMPOSNO;
                    entity.CRMPOSTYPE = config.CRMPOSTYPE;
                    entity.CRMSHDM = config.CRMSHDM;
                    return entity;
                }
                return null;
            }
        }

        /// <summary>
        /// 积分类型
        /// 0	注册送积分
        /// 1	登录送积分
        /// 2	激活送积分
        /// 4	邀请好友送积分
        /// 5	评论送积分
        /// 6	精华评论送积分
        /// 7	第一次登录送积分
        /// 8	晒单送积分
        /// 9	签到送积分
        ///101注册送券
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool GiftPoint(CustomerScoreEntity request)
        {
            if (CrmOpClaUtilInStance != null && CrmOpClaUtilInStance.qzjConfig != null && request != null)
            {
                return CrmOpClaUtilInStance.PointChange(request.CrmMemberID, request.ValidScore.ToString(), request.PointType);
            }
            return false;
        }

        public static List<CRMLuckDrawLog> CrmTradeConfirm(CustomerScoreEntity request, out string message)
        {
            message = string.Empty;
            if (crmOpClaUtilInStance != null && crmOpClaUtilInStance.qzjConfig != null && request != null)
            {
                List<CrmProj.Model.CrmLotteryInfo> CrmLotteryInfoList;
                List<CRMLuckDrawLog> CRMLuckDrawLogList = new List<CRMLuckDrawLog>();
                bool success = crmOpClaUtilInStance.CrmTradeConfirm(request.CrmServerBillID.ToString(), request.OrderSysNo.ToString(), request.MembershipCard, out  message, out CrmLotteryInfoList);
                if (success)
                {
                    if (CrmLotteryInfoList != null && CrmLotteryInfoList.Count > 0)
                    {
                        foreach (var item in CrmLotteryInfoList)
                        {
                            CRMLuckDrawLog entity = new CRMLuckDrawLog();
                            entity.LuckDrawID = item.ID;
                            entity.LuckDrawMark = item.LotteryMark;
                            entity.LuckDrawName = item.Name;
                            entity.MemberShipCardID = item.MemberCardCode;
                            entity.OrderSysNo = int.Parse(item.OrderSysNo);
                            entity.PayMark = item.PayMark;
                            entity.LuckDrawCode = item.Number;
                            CRMLuckDrawLogList.Add(entity);
                        }
                        return CRMLuckDrawLogList;
                    }
                }
                return null;
            }
            message = "登陆CRM前置机失败";
            return null;
        }


        /// <summary>
        /// crm退货确认
        /// </summary>
        /// <param name="jlbh"></param>
        /// <returns></returns>
        public static bool CrmReturn(CustomerScoreEntity request, out string message)
        {
            message = string.Empty;
            CrmProj.Model.CrmAppResp_0104 crm0104 = new CrmProj.Model.CrmAppResp_0104();
            if (CrmOpClaUtilInStance != null && CrmOpClaUtilInStance.qzjConfig != null && request != null)
            {
                if (request == null
                    || string.IsNullOrEmpty(request.MembershipCard)
                    || string.IsNullOrEmpty(request.OrderSysNo))
                {
                    return false;
                }
                CustomerScoreLogDA da = new CustomerScoreLogDA();
                //根据订单编号获取SOItemList
                int soSysNo = 0;
                if (int.TryParse(request.OrderSysNo, out soSysNo)) { }
                List<ReturnSoItemInfo> itemList = da.GetReturnSoItemInfoListBySOID(soSysNo);
                if (itemList == null || itemList.Count <= 0)
                {
                    message = "noneed";
                    return false;
                }
                CrmProj.Model.article[] productAryy = new CrmProj.Model.article[itemList.Count];
                decimal totalAmount = 0;//= orderDetail.SOMaster.Amount.CashPay + orderDetail.SOMaster.Amount.PayPrice + orderDetail.Shipping.Price + orderDetail.SOMaster.Amount.PremiumAmount + orderDetail.SOMaster.Amount.DiscountAmount - orderDetail.SOMaster.Amount.GiftCardPay - orderDetail.SOMaster.Amount.PrepayAmount;
                for (int i = 0; i < itemList.Count; i++)
                {
                    CrmProj.Model.article product = new CrmProj.Model.article();
                    product.inx = i;
                    product.dept_sale = "980101";// "980101";//部门代码
                    product.code = itemList[i].MerchantProductID;
                    product.contract_code = itemList[i].ContractCode;
                    product.quantity = itemList[i].Quantity;
                    product.bj_bcjhd = "N";
                    product.amount = itemList[i].CurrentPrice * itemList[i].Quantity;
                    productAryy[i] = product;
                    totalAmount += product.amount;
                }
                bool success = crmOpClaUtilInStance.CrmReturn(request.OrderSysNo, request.CrmMemberID, productAryy, totalAmount, out message, out crm0104);
                if (success)
                {
                    //更新奖票信息为不可用：
                    da.SetLuckTicketVoid(soSysNo);
                    return true;
                }
                return false ;
            }
            message = "CRM前置机登陆失败";
            return false;
        }
    }
}
