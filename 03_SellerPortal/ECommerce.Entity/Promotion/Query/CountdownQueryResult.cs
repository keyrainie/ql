using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums.Promotion;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class CountdownQueryResult : CountdownInfo
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品卖价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        public string UIBeginDate
        {
            get
            {
                return this.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string UIEndDate
        {
            get
            {
                return this.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string UIInDate
        {
            get
            {
                return this.InDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string UIStatus
        {
            get
            {
                if (this.Status == CountdownStatus.Running && this.EndTime < DateTime.Now)
                {
                    return "正在终止";
                }
                return EnumHelper.GetDescription(this.Status);
            }
        }

        public string UIIsEndIfNoQty
        {
            get
            {
                if (this.IsEndIfNoQty == "Y")
                {
                    return "是";
                }
                return "否";
            }
        }

        public bool CanEdit
        {
            get
            {
                if (this.Status == CountdownStatus.Init
                    || this.Status == CountdownStatus.VerifyFaild)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanSubmit
        {
            get
            {
                if (this.Status == CountdownStatus.Init
                    || this.Status == CountdownStatus.VerifyFaild)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanVoid
        {
            get
            {
                if (this.Status == CountdownStatus.Init
                    || this.Status == CountdownStatus.VerifyFaild
                    || this.Status == CountdownStatus.Ready
                    || this.Status == CountdownStatus.WaitForPrimaryVerify)
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanStop
        {
            get
            {
                if (this.Status == CountdownStatus.Running&&this.EndTime>DateTime.Now)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsStopping
        {
            get
            {
                if (this.Status == CountdownStatus.Running && this.EndTime <= DateTime.Now)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
