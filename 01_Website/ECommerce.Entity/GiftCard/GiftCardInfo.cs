using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.GiftCard
{
    public class GiftCardInfo
    {
        //
        public int TransactionNumber { get; set; }

        //卡号
        public string Code { get; set; }

        //密码
        public string Password { get; set; }

        //条形码
        public string Barcode { get; set; }

        //总金额
        public decimal TotalAmount { get; set; }

        //可用金额
        public decimal AvailAmount { get; set; }

        //购买客户编号
        public int CustomerSysNo { get; set; }

        //绑定客户编号
        public int ? BindingCustomerSysNo { get; set; }

        //开始日期
        public DateTime ? BeginDate { get; set; }

        //结束日期
        public DateTime ? EndDate { get; set; }

        //类型 实物卡为 PhysicalCard
        public GiftCardType Type { get; set; }

        //建行换礼积分值
        public decimal ValidScore { get; set; }

        //制表单id
        public int ReferenceId { get; set; }

        //购买礼品卡的SO
        public int ReferenceSOSysNo { get; set; }

        public InternalType InternalType { get; set; }

        //状态
        public string Status { get; set; }

        //兑换SO编号
        public int UseSOSysNo { get; set; }

        //激活人
        public string ActivateUser { get; set; }

        //激活日期
        public DateTime ActivateDate { get; set; }

        public string InUser { get; set; }

        public DateTime InDate { get; set; }

        public string EditUser { get; set; }

        public DateTime EditDate { get; set; }

        public string IsHold { get; set; }
    }
}
