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
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.Models
{
    public class CustomerRightMaintainView : ModelBase
    {
        public CustomerRightMaintainView()
        {
            RightList = new List<CustomerRightVM>();
        }
        public int CustomerSysNo { get; set; }
        private List<CustomerRightVM> rightList;
        public List<CustomerRightVM> RightList
        {
            get { return rightList; }
            set { SetValue<List<CustomerRightVM>>("RightList", ref rightList, value); }
        }
    }

    public class CustomerRightVM : ModelBase
    {
        /// <summary>
        /// 是否选中当前权限
        /// </summary>
        private bool _itemChecked;

        public bool ItemChecked
        {
            get { return _itemChecked; }
            set { SetValue("ItemChecked", ref _itemChecked, value); }
        }

        /// <summary>
        /// 权限值
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public string RightDescription { get; set; }

    }
}
