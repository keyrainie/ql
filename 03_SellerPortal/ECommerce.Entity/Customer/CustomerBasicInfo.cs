using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Customer
{
    public class CustomerBasicInfo : EntityBase
    {
        public int? SysNo { get; set; }
        /// <summary>
        /// 顾客业务ID
        /// </summary>        
        public string CustomerID { get; set; }
        /// <summary>
        /// 顾客姓名
        /// </summary>        
        public string CustomerName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>        
        public Gender? Gender { get; set; }

        public CustomerStatus? Status { get; set; }
        /// <summary>
        /// 邮件地址
        /// </summary>        
        public string Email { get; set; }
        /// <summary>
        /// 邮箱是否验证
        /// </summary>
        public bool? IsEmailConfirmed { get; set; }
        /// <summary>
        /// 电话
        /// </summary>        
        public string Phone { get; set; }
        /// <summary>
        /// 手机
        /// </summary>        
        public string CellPhone { get; set; }
        /// <summary>
        /// 是否电话验证
        /// </summary>
        public bool? CellPhoneConfirmed { get; set; }
        /// <summary>
        /// 传真
        /// </summary>        
        public string Fax { get; set; }
        /// <summary>
        /// 居住地SysNo
        /// </summary>
        public int? DwellAreaSysNo { get; set; }
        /// <summary>
        /// 居住地地址
        /// </summary>        
        public string DwellAddress { get; set; }
        /// <summary>
        /// 居住地邮编
        /// </summary>        
        public string DwellZip { get; set; }
        /// <summary>
        /// 总积分
        /// </summary>        
        public int? TotalScore { get; set; }
        /// <summary>
        /// 有效积分
        /// </summary>        
        public int? ValidScore { get; set; }
        /// <summary>
        /// 总余额
        /// </summary>
        public decimal? ValidPrepayAmt { get; set; }
        /// <summary>
        /// 总经验值
        /// </summary>
        public decimal? TotalExperience { get; set; }
        /// <summary>
        /// 顾客级别
        /// </summary>        
        public CustomerRank? Rank { get; set; }
        /// <summary>
        /// 购买总次数
        /// </summary>
        public int? BuyCount { get; set; }
        /// <summary>
        /// 购买总金额
        /// </summary>
        public decimal? TotalSOMoney { get; set; }
        /// <summary>
        /// 确认购买总金额
        /// </summary>
        public decimal? ConfirmedTotalAmt { get; set; }
    }
}
