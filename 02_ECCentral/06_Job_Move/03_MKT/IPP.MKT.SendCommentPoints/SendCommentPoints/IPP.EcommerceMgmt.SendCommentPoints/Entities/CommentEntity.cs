using System.Data;
using Newegg.Oversea.Framework.Entity;
using System;
using System.Configuration;

namespace IPP.EcommerceMgmt.SendCommentPoints.Entities
{
    public class CommentEntity
    {
        private static int JuniorMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["JuniorMember"]);
        private static int BronzeMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["BronzeMember"]);
        private static int GoldMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["GoldMember"]);
        private static int DiamondMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["DiamondMember"]);
        private static int SuperEggMemberScore = Convert.ToInt32(ConfigurationManager.AppSettings["SuperEggMember"]);


        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("ProductGroupSysNo", DbType.Int32)]
        public int ProductGroupSysNo { get; set; }

        [DataMapping("LogSysNo", DbType.Int32)]
        public int LogSysNo { get; set; }

        private int customerRank;

        [DataMapping("Rank", DbType.Int32)]
        public int CustomerRank
        {
            get
            {
                return customerRank;
            }
            set
            {
                customerRank = value;
                switch (customerRank)
                {
                    //初级会员：1个积分  --------------1
                    case 1:
                        CustomerPoint = JuniorMemberScore;
                        break;
                    //青铜会员：3个积分  --------------2
                    case 2:
                        CustomerPoint = BronzeMemberScore;
                        break;
                    //白银会员、黄金会员：5个积分  ----3,4
                    case 3:
                    case 4:
                        CustomerPoint = GoldMemberScore;
                        break;
                    //钻石会员、皇冠会员：10个积分 ----5,6
                    case 5:
                    case 6:
                        CustomerPoint = DiamondMemberScore;
                        break;
                    //至尊蛋黄：15个积分  -------------7
                    case 7:
                        CustomerPoint = SuperEggMemberScore;
                        break;
                }
            }
        }


        public int CustomerPoint { get; set; }

        [DataMapping("VendorType", DbType.Int32)]
        public int VendorType { get; set; }

        [DataMapping("MerchantSysNo", DbType.Int32)]
        public int MerchantSysNo { get; set; }

        //系统账户ID
        [DataMapping("SystemID", DbType.String)]
        public string  SystemID { get; set; }

        //系统账户积分
        [DataMapping("SystemPoint", DbType.Int32)]
        public int SystemPoint { get; set; }
    }
}
