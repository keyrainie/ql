using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductERPCategoryVM : ModelBase
    {
        #region 界面展示专用属性
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                base.SetValue("IsChecked", ref _isChecked, value);
            }
        }
        #endregion

        /// <summary>
        /// 大类码ID
        /// </summary>
        private int _sp_ID;
        public int SP_ID
        {
            get { return _sp_ID; }
            set
            {
                base.SetValue("SP_ID", ref _sp_ID, value);
            }
        }

        /// <summary>
        /// 大类码Code
        /// </summary>
        private string _spCode;
        public string SPCode
        {
            get { return _spCode; }
            set
            {
                base.SetValue("SPCode", ref _spCode, value);
            }
        }

        /// <summary>
        /// 大类码名称
        /// </summary>
        private string _spName;
        public string SPName
        {
            get { return _spName; }
            set
            {
                base.SetValue("SPName", ref _spName, value);
            }
        }
    }
}
