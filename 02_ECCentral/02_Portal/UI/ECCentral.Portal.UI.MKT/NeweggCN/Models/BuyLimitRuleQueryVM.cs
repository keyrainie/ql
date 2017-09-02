using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Models
{
    public class BuyLimitRuleQueryVM : ModelBase
    {
        private LimitType? _limitType;
        /// <summary>
        /// 限购类型,0-单品限购，1-套餐限购
        /// </summary>
        public LimitType? LimitType
        {
            get { return _limitType; }
            set
            {
                base.SetValue("LimitType", ref _limitType, value);
            }
        }
        private int? _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }

        private string _comboSysNo;
        /// <summary>
        /// 套餐系统编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string ComboSysNo
        {
            get { return _comboSysNo; }
            set
            {
                base.SetValue("ComboSysNo", ref _comboSysNo, value);
            }
        }

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID
        {
            get { return _productID; }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }


        private string _comboName;
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string ComboName
        {
            get { return _comboName; }
            set
            {
                base.SetValue("ComboName", ref _comboName, value);
            }
        }
        private DateTime? _beginDate;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate
        {
            get { return _beginDate; }
            set
            {
                base.SetValue("BeginDate", ref _beginDate, value);
            }
        }
        private DateTime? _endDate;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                base.SetValue("EndDate", ref _endDate, value);
            }
        }

        private LimitStatus? _status;
        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public LimitStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }


        private string _channelID;
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
    }
}
