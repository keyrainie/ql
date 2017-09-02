using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using Newegg.Oversea.Framework.Entity;

namespace GiveERPCustomerPoint.Entities
{
    public class CustomerScoreEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }


        [DataMapping("Type", DbType.Int32)]
        public int Type { get; set; }

        [DataMapping("ValidScore", DbType.Decimal)]
        public decimal ValidScore { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("OrderSysNo", DbType.Int32)]
        public string OrderSysNo { get; set; }

        [DataMapping("GiveDate", DbType.DateTime)]
        public DateTime? GiveDate { get; set; }


        [DataMapping("ErrorMark", DbType.String)]
        public string ErrorMark { get; set; }

        [DataMapping("CreateDate", DbType.DateTime)]
        public DateTime? CreateDate { get; set; }



        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }


        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }


        [DataMapping("CrmMemberID", DbType.String)]
        public string CrmMemberID { get; set; }

        [DataMapping("MembershipCard", DbType.String)]
        public string MembershipCard { get; set; }

        [DataMapping("CrmServerBillID", DbType.Int32)]
        public int CrmServerBillID { get; set; }

        public string PointType 
        {
            get
            {
                string _pointType = "";
                switch (Type)
                {
                    case 0:
                        {
                            _pointType = "签到送积分";
                            break;
                        }
                    case 1:
                        {
                            _pointType = "注册送积分";
                            break;
                        }
                    case 2:
                        {
                            _pointType = "评论送积分";
                            break;
                        }
                    case 3:
                        {
                            _pointType = "晒单送积分";
                            break;
                        }
                    case 4:
                        {
                            _pointType = "下单送积分";
                            break;
                        }
                    default:
                        {
                            _pointType = Type.ToString();
                            break;
                        }
                }

                return _pointType;
            }
        }

    }


    [Serializable]
    public enum CustomerScoreLogType
    {
        [Description("签到送积分")]
        SignIn = 0,

        [Description("注册送积分")]
        Register = 1,

        [Description("评论送积分")]
        Topic = 2,

        [Description("晒单送积分")]
        OrderShow = 3,

        [Description("下单送积分")]
        Order = 4,
    }
}
