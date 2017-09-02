using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Enums;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class ProductNotifyInfoViewModel
    {

        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public NotifyStatus Status { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 邮件
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        public DateTime NotifyTime { get; set; }

        public string NotifyTimeString
        {
            get
            {
                return NotifyTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
        }

        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductMode { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 真实价格
        /// </summary>
        public decimal RealPrice { get; set; }
    }

    public class DeleteProductNotifyInfoRequestModel
    {
        public int NotifySysNo { get; set; }
    }
    public class AddProductNotifyInfoRequestModel
    {
        public int ProductSysNo { get; set; }
        public string Email { get; set; }
    }
}