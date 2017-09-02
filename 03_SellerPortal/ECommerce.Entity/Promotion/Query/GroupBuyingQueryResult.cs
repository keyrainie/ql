using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums.Promotion;

namespace ECommerce.Entity.Promotion
{
    public class GroupBuyingQueryResult : GroupBuyingInfo
    {

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }

        public string UIBeginDate
        {
            get
            {
                return this.BeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string UIEndDate
        {
            get
            {
                return this.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
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
                if (this.Status == GroupBuyingStatus.Active && this.EndDate < DateTime.Now)
                {
                    return "正在终止";
                }
                return GroupBuyingStatus.GetValue(this.Status);
            }
        }

        public bool CanEdit
        {
            get
            {
                if (this.Status == GroupBuyingStatus.Init
                    || this.Status == GroupBuyingStatus.VerifyFaild)
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
                if (this.Status == GroupBuyingStatus.Init
                    || this.Status == GroupBuyingStatus.VerifyFaild)
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
                if (this.Status == GroupBuyingStatus.Init
                    || this.Status == GroupBuyingStatus.WaitingAudit
                    || this.Status == GroupBuyingStatus.Pending
                    || this.Status == GroupBuyingStatus.VerifyFaild)
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
                if (this.Status == GroupBuyingStatus.Active&& this.EndDate>DateTime.Now)
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
                if (this.Status == GroupBuyingStatus.Active && this.EndDate <= DateTime.Now)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
