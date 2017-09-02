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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ComputerConfigMasterVM : ModelBase
    {
        private string _companyCode;

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
        [Validate(ValidateType.Required)]
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
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 30)]
        public string ComputerConfigName
        {
            get { return _computerConfigName; }
            set
            {
                base.SetValue("ComputerConfigName", ref _computerConfigName, value);
            }
        }
        private int _computerConfigType;
        /// <summary>
        /// 配置单类型系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int ComputerConfigTypeSysNo
        {
            get { return _computerConfigType; }
            set
            {
                base.SetValue("ComputerConfigTypeSysNo", ref _computerConfigType, value);
            }
        }
        private ComputerConfigStatus _status;
        /// <summary>
        /// 状态
        /// </summary>
        public ComputerConfigStatus Status
        {
            get { return _status; }
            set
            {
                base.SetValue("Status", ref _status, value);
            }
        }
        private ComputerConfigOwner _owner;
        /// <summary>
        /// 配置单归属人
        /// </summary>
        public ComputerConfigOwner Owner
        {
            get { return _owner; }
            set
            {
                base.SetValue("Owner", ref _owner, value);
            }
        }

        private string _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string Priority
        {
            get { return _priority; }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }
        private string _note;
        /// <summary>
        /// 配置单说明
        /// </summary>
        [Validate(ValidateType.Required)]
        public string Note
        {
            get { return _note; }
            set
            {
                base.SetValue("Note", ref _note, value);
            }
        }

        private List<ComputerConfigItemVM> _configItemList;
        /// <summary>
        /// 配置组件明细
        /// </summary>
        public List<ComputerConfigItemVM> ConfigItemList
        {
            get { return _configItemList; }
            set
            {
                base.SetValue("ConfigItemList", ref _configItemList, value);
            }
        }

        #region UI相关属性
        private decimal _totalAmount;
        /// <summary>
        /// 配置单金额
        /// </summary>
        public decimal TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                base.SetValue("TotalAmount", ref _totalAmount, value);
            }
        }
        private decimal _totalDiscount;
        /// <summary>
        /// 总折扣
        /// </summary>
        public decimal TotalDiscount
        {
            get { return _totalDiscount; }
            set
            {
                base.SetValue("TotalDiscount", ref _totalDiscount, value);
            }
        }


        public void CalcTotal()
        {
            decimal totalAmount = 0.00M;
            decimal totalDiscount = 0.00M;
            if (this.ConfigItemList != null)
            {
                foreach (var item in this.ConfigItemList)
                {
                    if (item.CurrentPrice.HasValue)
                    {
                        totalAmount += item.CurrentPrice.Value * (item.ProductQty==null?0:item.ProductQty.Value);
                        totalDiscount += item.Discount.Value * (item.ProductQty==null?0:item.ProductQty.Value);
                    }
                }
            }
            this.TotalAmount = totalAmount;
            this.TotalDiscount = totalDiscount;
        }
        #endregion

        public bool HasComputerConfigApproveMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_ComputerConfig_ApproveMaintain); }
        }
    }
}
