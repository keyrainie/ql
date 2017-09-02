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
using ECCentral.BizEntity.IM;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;


namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAccessoriesQueryVM : ModelBase
    {

        public ProductAccessoriesQueryVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<ValidStatus>(EnumConverter.EnumAppendItemType.All);
        }
        /// <summary>
        /// 配件功能名称
        /// </summary>
        private string accessoriesQueryName;
        public string AccessoriesQueryName 
        { 
            get{return accessoriesQueryName;}
            set { SetValue("AccessoriesQueryName", ref accessoriesQueryName, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ValidStatus? Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        private string createUserName;
        public string CreateUserName 
        {
            get { return createUserName; }
            set { SetValue("CreateUserName", ref createUserName, value); }
        }

        /// <summary>
        /// 创建起始时间
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        public List<KeyValuePair<ValidStatus?, string>> StatusList { get; set; }
    }
}
