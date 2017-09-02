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
using System.ComponentModel.DataAnnotations;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerGiftCreateVM : ModelBase
    {
        public CustomerGiftCreateVM()
        {
            CustomerIDList = new List<int>();
            CusIDList = new List<string>();
        }
        private List<int> _customerIDList;
        /// <summary>
        /// 顾客SysNo列表---已过时
        /// </summary>
        [Validate(ValidateType.Required)]
        public List<int> CustomerIDList
        {
            get
            {
                return _customerIDList;
            }
            set
            {
                base.SetValue("CustomerIDList", ref _customerIDList, value);
            }
        }

        private string _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string ProductSysNo
        {
            get
            {
                return _productSysNo;
            }
            set
            {
                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }

        private string _productID;
        /// <summary>
        /// 商品ID
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ProductID
        {
            get
            {
                return _productID;
            }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }

        private string _quantity;
        /// <summary>
        /// 商品数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^\d{0,4}$", ErrorMessage = "请输入数值小于10000的商品数量!")]
        public string Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                base.SetValue("Quantity", ref _quantity, value);
            }
        }

        private string _channelID;
        /// <summary>
        /// 商品ID
        /// </summary>
        // [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get
            {
                return _channelID;
            }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        private List<string> _cusIDList;
        /// <summary>
        /// 顾客ID列表---2012-8-22新增
        /// </summary>
        public List<string> CusIDList
        {
            get
            {
                return _cusIDList;
            }
            set
            {
                base.SetValue("CusIDList", ref _cusIDList, value);
            }
        }

        private string _cusIDListString;
        /// <summary>
        /// 用于界面验证顾客ID不能为空
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CusIDListString
        {
            get
            {
                return _cusIDListString;
            }
            set
            {
                base.SetValue("CusIDListString", ref _cusIDListString, value);
            }
        }
    }
}
