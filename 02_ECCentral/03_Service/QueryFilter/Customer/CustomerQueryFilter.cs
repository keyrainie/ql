using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    public class CustomerQueryFilter
    {
        public CustomerQueryFilter()
        {
            PagingInfo = new PagingInfo();
            PagingInfo.PageIndex = 0;
            PagingInfo.PageSize = 1;
            PagingInfo.SortBy = "";
        }
        public PagingInfo PagingInfo { get; set; }

        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 顾客业务ID
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 顾客姓名(CustomerName,ReceiveName,ReceiveContact)
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 电话(Phone, CellPhone, ReceivePhone, ReceiveCellPhone)
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 地址(DwellAddress, ReceiveAddress)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 0: Valid 有效
        /// 1: InValid  无效
        /// -1: All  全部
        /// <summary>
        public CustomerStatus? Status { get; set; }


        public int? ChannelSysNo { get; set; }


        /// <summary>
        /// 顾客注册时间段开始
        /// </summary>
        public DateTime? RegisterTimeFrom { get; set; }

        /// <summary>
        /// 顾客注册时间结束
        /// </summary>
        public DateTime? RegisterTimeTo { get; set; }

        public bool IsBuyCountCheck { get; set; }

        /// <summary>
        /// 判断更多条件checkbox是否被选中
        /// 获取页面的checkbox的value值 “0：未选中” 或 “1：已选中”
        /// </summary>
        public bool IsMoreQueryBuilderCheck { get; set; }


        /// <summary>
        /// 来源
        /// </summary>
        public string FromLinkSource { get; set; }

        /// <summary>
        /// 推荐人ID
        /// </summary>
        public string RecommendedByCustomerID { get; set; }


        /// <summary>
        /// 顾客Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 判断页面是否为VIP的checkbox是否被选中
        /// 是否贵宾(VIP)  “0：否” 或 “1：是”
        /// </summary>
        public CustomerVipOnly? IsVip { get; set; }


        /// <summary>
        /// 判断页面 购买次数：选择“端点值：0 ”或“区间值：1”
        /// 获取页面的radio的value值
        /// </summary>
        public int? IsBuyCountRadio { get; set; }

        /// <summary>
        /// 0: = 等于
        /// 1: ≥ 大于等于
        /// 2: ≤ 小于等于
        /// 3: > 大于
        /// 4: < 小于
        /// 5: ≠  不等于
        /// </summary>
        public OperationSignType? OperationSign { get; set; }

        /// <summary>
        /// 购买次数端点值
        /// </summary>
        public int? BuyCountValue { get; set; }

        /// <summary>
        /// 购买次数端点起始值
        /// </summary>
        public int? BuyCountBeginPoint { get; set; }

        /// <summary>
        /// 购买次数端点结束值
        /// </summary
        public int? BuyCountEndPoint { get; set; }

        /// <summary>
        /// 邮箱确认？
        /// </summary>
        public Boolean? IsEmailConfirmed { get; set; }

        /// <summary>
        /// 电话确认？
        /// </summary>
        public YNStatus? IsPhoneConfirmed { get; set; }

        /// <summary>
        /// 头像状态
        /// </summary>
        public AvtarShowStatus? AvtarImageStatus { get; set; }
        /// <summary>
        /// 顾客类型
        /// </summary>
        public CustomerType? CustomersType { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdentityCard { get; set; }

        /// <summary>
        /// 会员卡号
        /// </summary>
        public string VipCardNo { get; set; }
    }
}
