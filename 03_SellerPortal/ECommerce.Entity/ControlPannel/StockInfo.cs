using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class StockInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 渠道仓库编号
        /// </summary>
        public string StockID { get; set; }
        /// <summary>
        /// 渠道仓库名称
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        //public int? WebChannelSysNo { get; set; }
        /// <summary>
        /// 仓库状态
        /// </summary>
        public StockStatus Status { get; set; }
        /// <summary>
        /// 渠道仓库所属仓库
        /// </summary>
        //public int? WarehouseSysNo { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public TradeType StockType { get; set; }
        /// <summary>
        /// 商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        public decimal WarehouseRate { get; set; }

        public string StoreCompanyCode { get; set; }
        public int? WHArea { get; set; }
        public int? WebChannelSysNo { get; set; }
        public int? WarehouseSysNo { get; set; }
        /// <summary>
        /// 海关关区代码
        /// </summary>
        public CustomsCodeMode CustomsCode { get; set; }

        #region 仓库发件信息

        /// <summary>
        /// 发件联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 发件联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 发件省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 发件城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 发件联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 发件地邮编
        /// </summary>
        public string Zip { get; set; }
        /// <summary>
        /// 发件地国家
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// 发件公司名称
        /// </summary>
        public string CompanyName { get; set; }

        #endregion

        #region 仓库收件信息

        /// <summary>
        /// 收件联系人
        /// </summary>
        public string ReceiveContact { get; set; }
        /// <summary>
        /// 收件联系电话
        /// </summary>
        public string ReceiveContactPhone { get; set; }
        /// <summary>
        /// 收件联系地址
        /// </summary>
        public string ReceiveAddress { get; set; }

        #endregion
    }
}
