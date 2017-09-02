using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Customer;
namespace ECCentral.Portal.UI.SO.Models
{
    public class GiftCardInfoVM : ModelBase
   {
        public GiftCardInfoVM()
        {
            BindingCustomer = new CustomerInfo(); 
        }
       private Int32? m_SysNo;
       public Int32? SysNo
       {
           get { return this.m_SysNo; }
           set { this.SetValue("SysNo", ref m_SysNo, value);  }
       }

       private Int32? m_CustomerSysNo;
       public Int32? CustomerSysNo
       {
           get { return this.m_CustomerSysNo; }
           set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
       }

       /// <summary>
       /// 是否选择了此礼品卡
       /// </summary>
       private Boolean? m_IsSelected;
       public Boolean? IsSelected
       {
           get { return this.m_IsSelected; }
           set { this.SetValue("IsSelected", ref m_IsSelected, value); }
       }


       /// <summary>
       /// 卡号
       /// </summary>
       private String m_CardCode;
       public String CardCode
       {
           get { return this.m_CardCode; }
           set { this.SetValue("CardCode", ref m_CardCode, value); }
       }

       /// <summary>
       /// 礼品卡总金额
       /// </summary>
       private Decimal? m_TotalAmount;
       public Decimal? TotalAmount
       {
           get { return this.m_TotalAmount; }
           set { this.SetValue("TotalAmount", ref m_TotalAmount, value); }
       }

       /// <summary>
       /// 可用余额
       /// </summary>
       private Decimal? m_AvailAmount;
       public Decimal? AvailAmount
       {
           get { return this.m_AvailAmount; }
           set { this.SetValue("AvailAmount", ref m_AvailAmount, value); }
       }

       /// <summary>
       /// 使用金额
       /// </summary>
       private Decimal? m_Amountt;
       public Decimal? Amount
       {
           get { return this.m_Amountt; }
           set { this.SetValue("Amount", ref m_Amountt, value); }
       }

       /// <summary>
       /// 是否绑定
       /// </summary>
       public Boolean? IsBinding
       {
           get
           {
               return (BindingCustomer != null && BindingCustomer.SysNo.HasValue);
           }          
       }

       /// <summary>
       /// 页面显示绑定信息
       /// </summary>
       public string BindingDisplay
       {
           get
           {
               return (BindingCustomer != null && BindingCustomer.SysNo.HasValue) ? "绑定卡" : "未绑定";
           }  
       }
        
       /// <summary>
       /// 绑定用户
       /// </summary>
       private CustomerInfo m_BindingCustomer;
       public CustomerInfo BindingCustomer
       {
           get { return this.m_BindingCustomer; }
           set { this.SetValue("BindingCustomer", ref m_BindingCustomer, value); }
       }

        /// <summary>
       /// WHEN [Status]='A' THEN 'Valid' WHEN [Status]='D' THEN 'InValid'  ELSE 'Hold' ___ 状态 A:有效 D:无效 
        /// </summary>
       private GiftCardStatus m_Status;
       public GiftCardStatus Status
       {
           get { return this.m_Status; }
           set { this.SetValue("Status", ref m_Status, value); }
       }

        /// <summary>
        /// 页面显示 状态
        /// </summary>
       public string StatusDisplay
       {
           get 
           {
               return Status == GiftCardStatus.Valid ? "有效" : (Status == GiftCardStatus.InValid ? "无效" : "锁定");
           }          
       }

       /// <summary>
       /// 失效时间
       /// </summary>
       private DateTime? m_EndDate;
       public DateTime? EndDate
       {
           get { return this.m_EndDate; }
           set { this.SetValue("EndDate", ref m_EndDate, value); }
       }
       /// <summary>
       /// 开始时间
       /// </summary>
       private DateTime? m_BeginDate;
       public DateTime? BeginDate
       {
           get { return this.m_BeginDate; }
           set { this.SetValue("BeginDate", ref m_BeginDate, value); }
       }
   }
}
