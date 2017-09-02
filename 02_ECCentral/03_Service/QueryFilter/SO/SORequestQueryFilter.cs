using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.QueryFilter.SO
{
    public class SORequestQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        #region 常用查询条件
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public string SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? FromOrderTime
        {
            get;
            set;
        }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? ToOrderTime
        {
            get;
            set;
        }

        /// <summary>
        /// 开始订单总额
        /// </summary>
        public decimal? FromTotalAmount { get; set; }

        /// <summary>
        /// 开始订单总额
        /// </summary>
        public decimal? ToTotalAmount { get; set; }

        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        public SOStatus? SOStatus
        {
            get;
            set;
        }

        public List<SOStatus> SOStatusArray
        {
            get;
            set;
        }

        /// <summary>
        /// 收件人
        /// </summary>
        public string ReceiveName
        {
            get;
            set;
        }
        /// <summary>
        /// 收件人电话
        /// </summary>
        public string ReceivePhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收件人手机
        /// </summary>
        public string ReceiveMobilePhone
        {
            get;
            set;
        }
        /// <summary>
        /// 配送地址
        /// </summary>
        public string ReceiveAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 分仓编号
        /// </summary>
        public int? StockSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public SOIncomeStatus? IncomeStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public NetPayStatus? NetPayStatus
        {
            get;
            set;
        } 
        #endregion


        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? FromAuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ToAuditTime
        {
            get;
            set;
        }
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public int? AuditUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 是否是VIP客户
        /// </summary>
        public bool? IsVIPCustomer
        {
            get;
            set;
        }
        /// <summary>
        /// 是否使用优惠券
        /// </summary>
        public bool? IsUsePromotion
        {
            get;
            set;
        }
        /// <summary>
        /// 是否是电话下单
        /// </summary>
        public bool? IsPhoneOrder
        {
            get;
            set;
        }
        /// <summary>
        /// 是否是VIP订单
        /// </summary>
        public bool? IsVIP
        {
            get;
            set;
        }
        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime? DeliveryDate
        {
            get;
            set;
        }
        /// <summary>
        /// 配送时间段
        /// </summary>
        public int? DeliveryTimeRange
        {
            get;
            set;
        }
        /// <summary>
        /// 商品大类
        /// </summary>
        public int? Category1SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 商品中类
        /// </summary>
        public int? Category2SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 商品小类
        /// </summary>
        public int? Category3SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 运送方式编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 发票编号
        /// </summary>
        public string InvoiceNo
        {
            get;
            set;
        }
        /// <summary>
        /// 是否搜索历史记录
        /// </summary>
        public bool? IncludeHistory
        {
            get;
            set;
        }

        /// <summary>
        /// 是否补偿单
        /// </summary>
        public bool? IsExpiateOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 是否体验单
        /// </summary>
        public bool? IsExperienceOrder
        {
            get;
            set;
        }

        public bool? IsBackOrder
        {
            get;
            set;
        }

        /// <summary>
        /// 客户客户IP地址
        /// </summary>
        public string CustomerIPAddress
        {
            get;
            set;
        }
        /// <summary>
        /// 订单类型
        /// </summary>
        public SOType? SOType
        {
            get;
            set;
        }

        /// <summary>
        /// 原订单编号
        /// </summary>
        public int? ContractSOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 断货分仓
        /// </summary
        public int? OutSubStockSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 优惠券系统编号
        /// </summary>
        public int? PromotionCodeSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 分期状态
        /// </summary>
        [Obsolete("此字段已弃用",true)]
        public string InstallmentStatus
        {
            get;
            set;
        }

        public int? FPStatus
        {
            get;
            set;
        }

        [Obsolete("此字段已弃用", true)]
        public string SIMCardStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 是否需开增票
        /// </summary>
        public bool? IsVAT
        {
            get;
            set;
        }
        /// <summary>
        /// 是否已开增票
        /// </summary>
        public bool? VATIsPrinted
        {
            get;
            set;
        }

        /// <summary>
        /// 是否已经录入16位合同号
        /// </summary>
        public bool? IsInputContractNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? MerchantSysNo
        {
            get;
            set;
        } 
        /// <summary>
        /// 用户类型
        /// </summary>
        public int? KFCType
        {
            get;
            set;
        }

        /// <summary>
        /// 仓储类型
        /// </summary>
        public StockType? StockType
        {
            get;
            set;
        }

        /// <summary>
        /// 配送类型
        /// </summary>
        public ECCentral.BizEntity.Invoice.DeliveryType? DeliveryType
        {
            get;
            set;
        }

        /// <summary>
        /// 发票类型
        /// </summary>
        public InvoiceType? InvoiceType
        {
            get;
            set;
        }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string SynSOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 第三方订单类型
        /// </summary>
        public string SynSOType
        {
            get;
            set;
        }


        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }

        public string StoreCompanyCode
        {
            get;
            set;
        }

        /// <summary>
        /// 查询模式
        /// </summary>
        public int? SearchMode
        {
            get;
            set;
        }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string WebChannelID { get; set; }

        /// <summary>
        /// 出库人系统编号
        /// </summary>
        public int? OutStockUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 来源
        /// </summary>
        public string FromLinkSource { get; set; }

        /// <summary>
        /// 是否包括团购失败的商品
        /// </summary>
        public bool? IncludeFailedGroupBuyingProduct
        {
            get;
            set;
        }

        /// <summary>
        /// 以旧换新材料提交状态
        /// </summary>
        [Obsolete("此字段已弃用",true)]
        public int? OldChangeNewSubmitStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 以旧换新补贴申请状态
        /// </summary>
        [Obsolete("此字段已弃用", true)]
        public int? OldChangeNewRequestStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 购物车编号
        /// </summary>
        public string ShoppingCartNo
        {
            get;
            set;
        }

        /// <summary>
        /// 礼品卡
        /// </summary>
        public string MembershipCard
        {
            get;
            set;
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get;
            set;
        }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? InputTime
        {
            get;
            set;
        }
    }
}
