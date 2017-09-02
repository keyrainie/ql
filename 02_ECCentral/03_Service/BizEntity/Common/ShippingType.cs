using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ECCentral.BizEntity.IM;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 配送方式信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class ShippingType:ICompany,IIdentity
    {
        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
        /// <summary>
        /// 配送方式名称
        /// </summary>
        [DataMember]
        public string ShippingTypeName { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        [DataMember]
        public string ShipTypeID { get; set; }
        /// <summary>
        /// 提供方
        /// </summary>
        [DataMember]
        public string Provider { get; set; }

        /// <summary>
        /// 保价费基数
        /// </summary>
        [DataMember]
        public decimal? PremiumBase { get; set; }

        /// <summary>
        /// 保价费率
        /// </summary>
        [DataMember]
        public decimal? PremiumRate { get; set; }

        
        /// <summary>
        /// 赔付金额上限
        /// </summary>
        [DataMember]
        public decimal? CompensationLimit { get; set; }
        
        /// <summary>
        /// 送货周期
        /// </summary>
        [DataMember]
        public string Period { get; set; }
  
        /// <summary>
        /// 是否专用配送方式
         /// </summary>
        [DataMember]
        public IsSpecial? IsSpecified { get; set; }

        ///<summary>
        ///配送方式描述 
        ///<summary>
        [DataMember]
        public string ShipTypeDesc { get; set; }

        /// <summary>
        /// 免运费金额
        /// </summary>
        [DataMember]
        public decimal FreeShipBase { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 是否前台显示
        /// </summary>
        [DataMember]      
        public HYNStatus? IsOnlineShow { get; set; }
        /// <summary>
        /// 前台显示名称
        /// </summary>
        [DataMember]
        public string DisplayShipName { get; set; }

        /// <summary>
        /// 是否24小时配送
        /// </summary>
        [DataMember]
        public DeliveryStatusFor24H? DeliveryPromise { get; set; }

        /// <summary>
        /// 配送当天每多长时间送一次
        /// </summary>
        [DataMember]               
        public int? Availsection { get; set; }      

        /// <summary>
        /// 是否收取包裹费
        /// </summary>
        [DataMember]
        public SYNStatus? IsWithPackFee { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        public int? OnlyForStockSysNo { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        public string StockName { get; set; }

        /// <summary>
        /// 免运费会员等级
        /// </summary>
        [DataMember]
        public int? CustomerRankFreeShip { get; set; }

        /// <summary>
        /// 配送方式类型
        /// </summary>
        [DataMember]
        public int? ShipTypeEnum { get; set; }

        /// <summary>
        /// 配送方式简称
        /// </summary>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// DS(并单)
        /// </summary>
        [DataMember]
        public int? DSSysNo { get; set; }

        /// <summary>
        /// 打包材料
        /// </summary>
        [DataMember]
        public ShippingPackStyle? PackStyle { get; set; }

        /// <summary>
        /// 服务时限
        /// </summary>
        [DataMember]
        public ShipDeliveryType? DeliveryType { get; set; }

        /// <summary>
        ///  联系电话
        /// </summary>
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        [DataMember]
        public string OfficialWebsite { get; set; }

        /// <summary>
        /// 每隔几日配送
        /// </summary>
        [DataMember]
        public int? IntervalDays { get; set; }

        /// <summary>
        /// 是否自提
        /// </summary>
        [DataMember]
        public int? IsGetBySelf { get; set; }

        /// <summary>
        /// 地区编号
        /// </summary>
        [DataMember]
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 自提点联系人
        /// </summary>
        [DataMember]
        public string ContactName { get; set; }

        /// <summary>
        /// 自提点联系电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// 自提点地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 配送方式扩展信息
        /// </summary>
        [DataMember]
        public ShipType_Ex ShipRtpe_Ex { get; set; }

        /// <summary>
        /// 存储方式
        /// </summary>
        [DataMember]
        public StoreType? StoreType { get; set; }
    }
}
