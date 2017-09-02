using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class ShipTypeInfo : EntityBase
    {
        public ShipTypeInfo()
        {
            this.DSSysNo = 1;
            this.CurrencySysNo = 1;
        }
        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string ShipTypeName { get; set; }

        /// <summary>
        /// 配送方式编号
        /// </summary>
        public string ShipTypeID { get; set; }
        /// <summary>
        /// 提供方
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 保价费基数
        /// </summary>
        public decimal? PremiumBase { get; set; }

        /// <summary>
        /// 保价费率
        /// </summary>
        public decimal? PremiumRate { get; set; }


        /// <summary>
        /// 赔付金额上限
        /// </summary>
        public decimal? CompensationLimit { get; set; }

        /// <summary>
        /// 送货周期
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// 是否专用配送方式
        /// </summary>
        public Specified? IsSpecified { get; set; }

        ///<summary>
        ///配送方式描述 
        ///<summary>
        public string ShipTypeDesc { get; set; }

        /// <summary>
        /// 免运费金额
        /// </summary>
        public decimal FreeShipBase { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 是否前台显示
        /// </summary>
        public HYNStatus? IsOnlineShow { get; set; }
        /// <summary>
        /// 前台显示名称
        /// </summary>
        public string DisplayShipName { get; set; }

        /// <summary>
        /// 是否24小时配送
        /// </summary>
        public DeliveryPromise? DeliveryPromise { get; set; }

        /// <summary>
        /// 配送当天每多长时间送一次
        /// </summary>
        public int? Availsection { get; set; }

        /// <summary>
        /// 是否收取包裹费
        /// </summary>
        public CommonYesOrNo? IsWithPackFee { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? OnlyForStockSysNo { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 免运费会员等级
        /// </summary>
        public int? CustomerRankFreeShip { get; set; }

        /// <summary>
        /// 配送方式类型
        /// </summary>
        public ShipTypeEnum? ShipTypeEnum { get; set; }

        /// <summary>
        /// 配送方式简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// DS(并单)
        /// </summary>
        public int? DSSysNo { get; set; }

        /// <summary>
        /// 打包材料
        /// </summary>
        public ShippingPackStyle? PackStyle { get; set; }

        /// <summary>
        /// 服务时限
        /// </summary>
        public ShipDeliveryType? DeliveryType { get; set; }

        /// <summary>
        ///  联系电话
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        public string OfficialWebsite { get; set; }

        /// <summary>
        /// 每隔几日配送
        /// </summary>
        public int? IntervalDays { get; set; }

        /// <summary>
        /// 是否自提
        /// </summary>
        public int IsGetBySelf { get; set; }

        /// <summary>
        /// 地区编号
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 自提点联系人
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 自提点联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 自提点地址
        /// </summary>
        public string Address { get; set; }

        ///// <summary>
        ///// 配送方式扩展信息
        ///// </summary>
        //public ShipType_Ex ShipRtpe_Ex { get; set; }

        /// <summary>
        /// 存储方式
        /// </summary>
        public StoreType? StoreType { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }
        public string StoreCompanyCode { get; set; }
        public int? CurrencySysNo { get; set; }
        public int? ShipFeeType { get; set; }
        public Appointment4CombineSO? Appointment4CombineSO { get; set; }
    }
}
