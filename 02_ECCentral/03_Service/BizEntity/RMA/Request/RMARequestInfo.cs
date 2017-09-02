using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// RMA申请单信息
    /// </summary>
    public class RMARequestInfo : IIdentity, IWebChannel, IMarketingPlace
    {
        public RMARequestInfo()
        {           
            this.Registers = new List<RMARegisterInfo>();            
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }       
        /// <summary>
        /// 所属渠道
        /// </summary>
        public WebChannel WebChannel { get; set; }
        /// <summary>
        /// 商家
        /// </summary>
        public Merchant Merchant { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 是否验证重复性
        /// </summary>
        public bool NeedVerifyDuplicate { get; set; }

        /// <summary>
        /// 销售单号
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
       
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 收货地址区域编号
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 收货时间
        /// </summary>
        public DateTime? ReceiveTime { get; set; }

        /// <summary>
        /// 收货用户信息
        /// </summary>
        public int? ReceiveUserSysNo { get; set; }

        /// <summary>
        /// 收获人姓名
        /// </summary>
        public string ReceiveUserName { get; set; }

        /// <summary>
        /// 申请单处理状态
        /// </summary>
        public RMARequestStatus? Status { get; set; }

        /// <summary>
        /// 客户发货时间
        /// </summary>
        public DateTime? CustomerSendTime { get; set; }

        /// <summary>
        /// 是否是物流拒收
        /// </summary>
        public bool IsRejectRMA { get; set; }

        /// <summary>
        /// 包裹编号
        /// </summary>
        public string PackageNumber { get; set; }

        /// <summary>
        /// 是否提交申请单
        /// </summary>
        public bool? IsSubmit { get; set; }

        /// <summary>
        /// 收货仓库编号
        /// </summary>
        public string ReceiveWarehouse { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 是否经邮局寄送
        /// </summary>
        public bool? IsViaPostOffice { get; set; }

        /// <summary>
        /// 邮资转积分数
        /// </summary>
        public int? PostageToPoint { get; set; }

        /// <summary>
        /// 返还积分
        /// </summary>
        public int? ReturnPoint { get; set; }

        /// <summary>
        /// 预约取件时间
        /// </summary>
        public DateTime? ETakeDate { get; set; }

        /// <summary>
        /// 送货方式
        /// </summary>
        public string ShipViaCode { get; set; }

        /// <summary>
        /// 邮包编号
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 申请单编号
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// 是否打印标签
        /// </summary>
        public string IsLabelPrinted { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? MerchantSysNo { get; set; }

        /// <summary>
        /// 仓储类型
        /// </summary>
        public StockType? StockType { get; set; }

        /// <summary>
        /// 配送类型
        /// </summary>
        public BizEntity.Invoice.DeliveryType? ShippingType { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public InvoiceType? InvoiceType { get; set; }

        /// <summary>
        /// 取回日期
        /// </summary>
        public DateTime? GetbackDate { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public int? GetbackAreaSysNo { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string GetbackAddress { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 单件列表
        /// </summary>
        public List<RMARegisterInfo> Registers { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? CreateUserSysNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 是否发短信
        /// </summary>       

        public bool? IsReceiveMsg { get; set; }


        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode { get; set; }

        public int? AuditUserSysNo { get; set; }
        
        public DateTime? AuditTime { get; set; }
    }
}