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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ComputerConfigMasterQueryVM:ModelBase
    {
        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
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
        private string _computerConfigName;
        /// <summary>
        /// 配置单名称
        /// </summary>
        public string ComputerConfigName
        {
            get { return _computerConfigName; }
            set
            {
                base.SetValue("ComputerConfigName", ref _computerConfigName, value);
            }
        }
        private int? _computerConfigType;
        /// <summary>
        /// 配置单类型系统编号
        /// </summary>
        public int? ComputerConfigType
        {
            get { return _computerConfigType; }
            set
            {
                base.SetValue("ComputerConfigType", ref _computerConfigType, value);
            }
        }
        private ComputerConfigStatus? _status;
        /// <summary>
        /// 状态
        /// </summary>
        public ComputerConfigStatus? Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private ComputerConfigOwner? _owner;
        /// <summary>
        /// 配置单归属人
        /// </summary>
        public ComputerConfigOwner? Owner
        {
            get { return _owner; }
            set
            {
                base.SetValue("Owner", ref _owner, value);
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
        private decimal? _minPriceRange;
        /// <summary>
        /// 配置金额从
        /// </summary>
        public decimal? MinPriceRange
        {
            get { return _minPriceRange; }
            set
            {
                base.SetValue("MinPriceRange", ref _minPriceRange, value);
            }
        }
        private decimal? _maxPriceRange;
        /// <summary>
        /// 配置金额到
        /// </summary>
        public decimal? MaxPriceRange
        {
            get { return _maxPriceRange; }
            set
            {
                base.SetValue("MaxPriceRange", ref _maxPriceRange, value);
            }
        }
        private int? _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private string _editUser;
        /// <summary>
        /// 更新人
        /// </summary>
        public string EditUser
        {
            get { return _editUser; }
            set
            {
                base.SetValue("EditUser", ref _editUser, value);
            }
        }

        private int? _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }

    }
}
